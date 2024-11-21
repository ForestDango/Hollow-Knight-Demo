using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;

public class StagTravel : MonoBehaviour
{
    [SerializeField] private CinematicSequence cinematicSequence;
    [SerializeField] private float minimumDuration;
    [SerializeField] private float fadeRate;
    private bool isAsync;
    private float currentDuration;
    private bool isFetchComplete;
    private bool isReadyToActivate;
    private bool isSkipping;
    private bool isSkipFadeComplete;

    protected bool IsReadyToActivate
    {
	get
	{
	    return isReadyToActivate;
	}
    }

    protected IEnumerator Start()
    {
	isAsync = Platform.Current.FetchScenesBeforeFade;
	GameCameras.instance.DisableImageEffects();
	HeroController.instance.transform.SetPositionY(-2000f);
	if (!isAsync)
	{
	    GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE SCENE IN");
	    GameManager.instance.inputHandler.SetSkipMode(SkipPromptMode.SKIP_INSTANT);
	    cinematicSequence.IsLooping = false;
	    cinematicSequence.Begin();
	    while (cinematicSequence.IsPlaying && !isSkipFadeComplete && cinematicSequence.VideoPlayer != null && cinematicSequence.VideoPlayer.CurrentTime < 3.9f)
	    {
		yield return null;
	    }
	    GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE OUT INSTANT");
	    yield return null;
	    GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
	    {
		EntryGateName = "door_stagExit",
		SceneName = GameManager.instance.playerData.nextScene,
		PreventCameraFadeOut = true,
		Visualization = GameManager.SceneLoadVisualizations.Custom
	    });
	    isReadyToActivate = true;
	}
	else
	{
	    StartCoroutine("FadeInRoutine");
	    cinematicSequence.IsLooping = true;
	    cinematicSequence.Begin();
	    GameManager.instance.BeginSceneTransition(new StagTravelAsyncLoadInfo(this)
	    {
		EntryGateName = "door_stagExit",
		SceneName = GameManager.instance.playerData.nextScene,
		PreventCameraFadeOut = true,
		Visualization = GameManager.SceneLoadVisualizations.Custom
	    });
	}
    }

    private IEnumerator FadeInRoutine()
    {
	GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE OUT INSTANT");
	yield return new WaitForSeconds(1.5f);
	GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE SCENE IN");
    }

    protected void Update()
    {
	currentDuration += Time.unscaledDeltaTime;
	if(isAsync && !isSkipping && isFetchComplete && currentDuration > minimumDuration)
	{
	    StartCoroutine(Skip());
	}
    }

    public IEnumerator Skip()
    {
	if (!isSkipping)
	{
	    StopCoroutine("FadeInRoutine");
	    isSkipping = true;
	    GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE OUT");
	    yield return new WaitForSecondsRealtime(0.5f);
	    isSkipFadeComplete = true;
	    isReadyToActivate = true;
	}
    }

    protected void NotifyFetchComplete()
    {
	isFetchComplete = true;
    }

    private class StagTravelAsyncLoadInfo : GameManager.SceneLoadInfo
    {
	private StagTravel stagTravel;
	public StagTravelAsyncLoadInfo(StagTravel stagTravel)
	{
	    this.stagTravel = stagTravel;
	}

	public override void NotifyFetchComplete()
	{
	    base.NotifyFetchComplete();
	    stagTravel.NotifyFetchComplete();
	}

	public override bool IsReadyToActivate()
	{
	    return base.IsReadyToActivate() && stagTravel.IsReadyToActivate;
	}

	public override void NotifyFinished()
	{
	    base.NotifyFinished();
	    GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE SCENE OUT INSTANT");
	    GameCameras.instance.EnableImageEffects(true, false);
	}
    }
}
