using UnityEngine;
using UnityEngine.Video;

public class XB1CinematicVideoPlayer : CinematicVideoPlayer
{
    private VideoPlayer videoPlayer;
    private Texture originalMainTexture;
    private RenderTexture renderTexture;
    private const string TexturePropertyName = "_MainTex";
    private bool isPlayEnqueued;

    public XB1CinematicVideoPlayer(CinematicVideoPlayerConfig config) : base(config)
    {
	originalMainTexture = config.MeshRenderer.material.GetTexture("_MainTex");
	renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
	Graphics.Blit((config.FaderStyle == CinematicVideoFaderStyles.White) ? Texture2D.whiteTexture : Texture2D.blackTexture, renderTexture);
	Debug.LogFormat("Creating Unity Video Player......");
	videoPlayer = config.MeshRenderer.gameObject.AddComponent<VideoPlayer>();
	videoPlayer.playOnAwake = false; //��ʼ�Ͳ���
	videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource; //��Ч���ģʽ
	videoPlayer.SetTargetAudioSource(0, config.AudioSource); //���ò��ŵ�audiosource��Ϸ����
	videoPlayer.renderMode = VideoRenderMode.CameraFarPlane; //������Ⱦģʽ
	videoPlayer.targetCamera = GameCameras.instance.mainCamera; //������ȾĿ�������
	videoPlayer.targetTexture = renderTexture; //����Ŀ������
	config.MeshRenderer.material.SetTexture(TexturePropertyName, renderTexture); // ���ò�������
	VideoClip embeddedVideoClip = config.VideoReference.EmbeddedVideoClip;  //���ò��ŵ�clipΪconfig�����EmbeddedVideoClip
	videoPlayer.clip = embeddedVideoClip;
	videoPlayer.prepareCompleted += OnPrepareCompleted;
	videoPlayer.Prepare(); //׼����ɲ���
    }

    public override bool IsLoading
    {
	get
	{
	    return false;
	}
    }
    public override bool IsPlaying
    {
	get
	{
	    if (videoPlayer != null && videoPlayer.isPrepared)
	    {
		return videoPlayer.isPlaying;
	    }
	    return isPlayEnqueued;
	}
    }
    public override bool IsLooping
    {
	get
	{
	    return videoPlayer != null && videoPlayer.isLooping;
	}
	set
	{
	    if (videoPlayer != null)
	    {
		videoPlayer.isLooping = value;
	    }
	}
    }
    public override float Volume
    {
	get
	{
	    if (base.Config.AudioSource != null)
	    {
		return base.Config.AudioSource.volume;
	    }
	    return 1f;
	}
	set
	{
	    if (base.Config.AudioSource != null)
	    {
		base.Config.AudioSource.volume = value;
	    }
	}
    }

    public override void Dispose()
    {
	base.Dispose();
	if(videoPlayer != null)
	{
	    videoPlayer.Stop();
	    Object.Destroy(videoPlayer);
	    videoPlayer = null;
	    MeshRenderer meshRenderer = Config.MeshRenderer;
	    if(meshRenderer != null)
	    {
		meshRenderer.material.SetTexture("_MainTex", originalMainTexture);
	    }
	}
	if(renderTexture != null)
	{
	    Object.Destroy(renderTexture);
	    renderTexture = null;
	}
    }
    public override void Play()
    {
	if(videoPlayer != null && videoPlayer.isPrepared)
	{
	    videoPlayer.Play();
	}
	isPlayEnqueued = true;
    }
    public override void Stop()
    {
	if (videoPlayer != null)
	{
	    videoPlayer.Stop();
	}
	isPlayEnqueued = false;
    }
    private void OnPrepareCompleted(VideoPlayer source)
    {
	if (source == videoPlayer && videoPlayer != null && isPlayEnqueued)
	{
	    videoPlayer.Play();
	    isPlayEnqueued = false;
	}
    }
}
