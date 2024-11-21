using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshRenderer))]
public class CinematicPlayer : MonoBehaviour
{
    [SerializeField] private CinematicVideoReference videoClip;
    private CinematicVideoPlayer cinematicVideoPlayer;
    [SerializeField] private AudioSource additionalAudio;
    [SerializeField] private MeshRenderer selfBlanker;

    [Header("Cinematic Settings")]
    [Tooltip("Determines what will trigger the video playing.")]
    public MovieTrigger playTrigger;

    [Tooltip("The speed of the fade in, comes in different flavours.")]
    public FadeInSpeed fadeInSpeed; //淡入速度

    [Tooltip("The amount of time to wait before fading in the camera. Camera will stay black and the video will play.")]
    [Range(0f, 10f)]
    public float delayBeforeFadeIn; //在淡入（0到1）之前延迟几秒才开始

    [Tooltip("Allows the player to skip the video.")] //允许玩家跳过video
    public SkipPromptMode skipMode;

    [Tooltip("Prevents the skip action from taking place until the lock is released. Useful for animators delaying skip feature.")]
    public bool startSkipLocked = false; //开始时强制锁定跳过

    [Tooltip("The speed of the fade in, comes in different flavours.")]
    public FadeOutSpeed fadeOutSpeed;

    [Tooltip("Video keeps looping until the player is explicitly told to stop.")]
    public bool loopVideo; //是否循环播放video直到控制它停止

    [Space(6f)]
    [Tooltip("The name of the scene to load when the video ends. Leaving this blank will load the \"next scene\" as set in PlayerData.")]
    public VideoType videoType;

    public CinematicVideoFaderStyles faderStyle;
    private AudioSource audioSource;
    private MeshRenderer myRenderer;
    private GameManager gm;
    private UIManager ui;
    private PlayerData pd;
    private PlayMakerFSM cameraFSM;

    private bool videoTriggered;
    private bool loadingLevel;

    [SerializeField] private AudioMixerSnapshot masterOff;
    [SerializeField] private AudioMixerSnapshot masterResume;

    private void Awake()
    {
	audioSource = GetComponent<AudioSource>();
	myRenderer = GetComponent<MeshRenderer>();
	if (videoType == VideoType.InGameVideo)
	{
	    myRenderer.enabled = false;
	}
    }

    protected void OnDestroy()
    {
	if(cinematicVideoPlayer != null)
	{
	    cinematicVideoPlayer.Dispose();
	    cinematicVideoPlayer = null;
	}
    }

    private void Start()
    {
	gm = GameManager.instance;
	ui = UIManager.instance;
	pd = PlayerData.instance;
	if (startSkipLocked)
	{
	    gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
	}
	else
	{
	    gm.inputHandler.SetSkipMode(skipMode);
	}
	if (playTrigger == MovieTrigger.ON_START)
	{
	    StartCoroutine(StartVideo());
	}
    }

    private void Update()
    {
	if (cinematicVideoPlayer != null)
	{
	    cinematicVideoPlayer.Update();
	}
	if (Time.frameCount % 10 == 0)
	{
	    Update10();
	}
    }

    private void Update10()
    {
	//每隔十帧检测一下是否动画已经播放完成。
	if ((cinematicVideoPlayer == null || (!cinematicVideoPlayer.IsLoading && !cinematicVideoPlayer.IsPlaying)) && !loadingLevel && videoTriggered)
	{
	    if (videoType == VideoType.InGameVideo)
	    {
		FinishInGameVideo();
		return;
	    }
	    FinishVideo();
	}
    }

    /// <summary>
    /// 影片结束后的行为
    /// </summary>
    private void FinishVideo()
    {
	Debug.LogFormat("Finishing the video.", Array.Empty<object>());
	videoTriggered = false;
	//判断video类型，目前只有OpeningCutscene和OpeningPrologue
	if (videoType == VideoType.OpeningCutscene) 
	{
	    GameCameras.instance.cameraFadeFSM.Fsm.Event("JUST FADE");
	    ui.SetState(UIState.INACTIVE);
	    loadingLevel = true;
	    StartCoroutine(gm.LoadFirstScene());
	    return;
	}
	if(videoType == VideoType.OpeningPrologue)
	{
	    GameCameras.instance.cameraFadeFSM.Fsm.Event("JUST FADE");
	    ui.SetState(UIState.INACTIVE);
	    loadingLevel = true;
	    //gm.LoadOpeningCinematic();
	    return;
	}
	if(videoType == VideoType.StagTravel)
	{
	    ui.SetState(UIState.INACTIVE);
	    loadingLevel = true;
	    gm.ChangeToScene(pd.GetString("nextScene"), "door_stagExit", 0f);
	    return;
	}
	//TODO:还有其他的videotype要做
    }

    /// <summary>
    /// 结束游戏内的视频video
    /// </summary>
    private void FinishInGameVideo()
    {
	Debug.LogFormat("Finishing in-game video.", Array.Empty<object>());
	PlayMakerFSM.BroadcastEvent("CINEMATIC END");
	myRenderer.enabled = false;
	selfBlanker.enabled = false;
	if(masterResume != null)
	{
	    masterResume.TransitionTo(0f);
	}
	if(additionalAudio != null)
	{
	    additionalAudio.Stop();
	}
	if(cinematicVideoPlayer != null)
	{
	    cinematicVideoPlayer.Stop();
	    cinematicVideoPlayer.Dispose();
	    cinematicVideoPlayer = null;
	}
	videoTriggered = false;
	gm.gameState = GameState.PLAYING;
    }

    /// <summary>
    /// 开启视频video
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartVideo()
    {
	if(masterOff != null)
	{
	    masterOff.TransitionTo(0f);
	}
	videoTriggered = true;
	if(videoType == VideoType.InGameVideo)
	{
	    gm.gameState = GameState.CUTSCENE;
	    if(cinematicVideoPlayer == null)
	    {
		Debug.LogFormat("Creating new CinematicVideoPlayer for in game video", Array.Empty<object>());
		cinematicVideoPlayer = CinematicVideoPlayer.Create(new CinematicVideoPlayerConfig(videoClip, myRenderer, audioSource, faderStyle, GameManager.instance.GetImplicitCinematicVolume()));
	    }
	    Debug.LogFormat("Waiting for CinematicVideoPlayer in game video load...", Array.Empty<object>());
	    while (cinematicVideoPlayer != null && cinematicVideoPlayer.IsLoading)
	    {
		yield return null;
	    }
	    Debug.LogFormat("Starting cinematic video player in game video.", Array.Empty<object>());
	    if(cinematicVideoPlayer != null)
	    {
		cinematicVideoPlayer.IsLooping = loopVideo;
		cinematicVideoPlayer.Play();
		myRenderer.enabled = true;
	    }
	    if (additionalAudio)
	    {
		additionalAudio.Play();
	    }
	    yield return new WaitForSeconds(delayBeforeFadeIn);
	    if (fadeInSpeed == FadeInSpeed.SLOW)
	    {
		GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE SCENE IN SLOWLY");
	    }
	    else if (fadeInSpeed == FadeInSpeed.NORMAL)
	    {
		GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE SCENE IN");
	    }
	}
	else if(videoType == VideoType.StagTravel)
	{
	    //TODO:
	    GameCameras.instance.DisableImageEffects();
	    if (cinematicVideoPlayer == null)
	    {
		    cinematicVideoPlayer = CinematicVideoPlayer.Create(new CinematicVideoPlayerConfig(videoClip, myRenderer, audioSource, faderStyle, GameManager.instance.GetImplicitCinematicVolume()));
	    }
	    while(cinematicVideoPlayer != null && cinematicVideoPlayer.IsLoading)
	    {
		yield return null;
	    }
	    if (cinematicVideoPlayer != null)
	    {
		cinematicVideoPlayer.IsLooping = loopVideo;
		cinematicVideoPlayer.Play();
		myRenderer.enabled = true;
	    }
	    yield return new WaitForSeconds(delayBeforeFadeIn);
	    if(fadeInSpeed == FadeInSpeed.SLOW)
	    {
		GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE SCENE IN SLOWLY");
	    }
	    else if(fadeInSpeed == FadeInSpeed.NORMAL)
	    {
		GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE SCENE IN");
	    }
	    StartCoroutine(WaitForStagFadeOut());
	    pd.disablePause = true;
	}
	else
	{
	    Debug.LogFormat("Start the Video");
	    if (cinematicVideoPlayer == null)
	    {
		cinematicVideoPlayer = CinematicVideoPlayer.Create(new CinematicVideoPlayerConfig(videoClip, myRenderer, audioSource, faderStyle, GameManager.instance.GetImplicitCinematicVolume()));
	    }
	    while (cinematicVideoPlayer != null && cinematicVideoPlayer.IsLoading)
	    {
		yield return null;
	    }
	    if (cinematicVideoPlayer != null)
	    {
		cinematicVideoPlayer.IsLooping = loopVideo;
		cinematicVideoPlayer.Play();
		myRenderer.enabled = true;
	    }
	    yield return new WaitForSeconds(delayBeforeFadeIn);
	    if(fadeInSpeed == FadeInSpeed.SLOW)
	    {
		GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE SCENE IN SLOWLY");
	    }
	    else if(fadeInSpeed == FadeInSpeed.NORMAL)
	    {
		GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE SCENE IN");
	    }
	}
    }

    /// <summary>
    /// 跳过视频
    /// </summary>
    /// <returns></returns>
    public IEnumerator SkipVideo()
    {
	if (videoTriggered)
	{
	    if(videoType == VideoType.InGameVideo)
	    {
		if(fadeOutSpeed != FadeOutSpeed.NONE)
		{
		    float duration = 0f; 
		    if (fadeOutSpeed == FadeOutSpeed.NORMAL)
		    {
			duration = 0.5f;
		    }
		    else if (fadeOutSpeed == FadeOutSpeed.SLOW)
		    {
			duration = 2.3f;
		    }
		    selfBlanker.enabled = true;
		    float timer = 0f;
		    while (videoTriggered)
		    {
			if (timer >= duration)
			{
			    break;
			}
			float a = Mathf.Clamp01(timer / duration);
			selfBlanker.material.color = new Color(0f, 0f, 0f, a);
			yield return null;
			timer += Time.unscaledDeltaTime;
		    }
		}
		else
		{
		    yield return null;
		}
	    }
	    else if(fadeOutSpeed == FadeOutSpeed.NORMAL)
	    {
		PlayMakerFSM.BroadcastEvent("JUST FADE");
		yield return new WaitForSeconds(0.5f);
	    }
	    else if (fadeOutSpeed == FadeOutSpeed.SLOW)
	    {
		PlayMakerFSM.BroadcastEvent("START FADE");
		yield return new WaitForSeconds(2.3f);
	    }
	    else
	    {
		yield return null;
	    }
	    if(cinematicVideoPlayer != null)
	    {
		cinematicVideoPlayer.Stop();
	    }
	}
    }
    private IEnumerator WaitForStagFadeOut()
    {
	yield return new WaitForSeconds(2.6f);
	GameCameras.instance.cameraFadeFSM.Fsm.Event("JUST FADE");
	yield break;
    }

    public enum MovieTrigger
    {
	ON_START,
	MANUAL_TRIGGER
    }

    public enum FadeInSpeed
    {
	NORMAL,
	SLOW,
	NONE
    }

    public enum FadeOutSpeed
    {
	NORMAL,
	SLOW,
	NONE
    }

    public enum VideoType
    {
	OpeningCutscene,
	StagTravel,
	InGameVideo,
	OpeningPrologue,
	EndingA,
	EndingB,
	EndingC,
	EndingGG
    }
}
