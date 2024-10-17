using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningSequence : MonoBehaviour
{
    [SerializeField] private ChainSequence chainSequence;
    [SerializeField] private ThreadPriority streamingLoadPriority;
    [SerializeField] private ThreadPriority completedLoadPriority;
    [SerializeField] private float skipChargeDuration; //跳过不同Sequence之间的冷却时间
    private bool isAsync;
    private bool isLevelReady;
    private AsyncOperation asyncKnightLoad;
    private AsyncOperation asyncWorldLoad;
    private float skipChargeTimer; // 计时器

    protected void OnEnable()
    {
	chainSequence.TransitionedToNextSequence += OnChangingSequences;
    }

    protected void OnDisable()
    {
	chainSequence.TransitionedToNextSequence -= OnChangingSequences;
    }

    protected IEnumerator Start()
    {
	isAsync = Platform.Current.FetchScenesBeforeFade;
	if (isAsync)
	{
	    return StartAsync();
	}
	return StartAsync();
    }

    protected void Update()
    {
	skipChargeTimer += Time.unscaledDeltaTime;
    }

    private static bool IsLevelReady(AsyncOperation operation)
    {
	return operation.progress >= 0.9f;
    }

    private IEnumerator StartAsync()
    {
	GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE SCENE IN");

	PlayMakerFSM.BroadcastEvent("START FADE OUT");
	Debug.LogFormat(this, "Starting opening sequence.", Array.Empty<object>());
	GameManager.instance.ui.SetState(UIState.CUTSCENE);
	GameManager.instance.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING);
	chainSequence.Begin();
	ThreadPriority lastLoadPriority = Application.backgroundLoadingPriority;
	Application.backgroundLoadingPriority = streamingLoadPriority;
	asyncKnightLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Knight_Pickup", LoadSceneMode.Additive);
	asyncKnightLoad.allowSceneActivation = false;
	asyncWorldLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Tutorial_01", LoadSceneMode.Single);
	asyncWorldLoad.allowSceneActivation = false;
	isLevelReady = false;
	while (chainSequence.IsPlaying)
	{
	    if (!isLevelReady)
	    {
		isLevelReady = (IsLevelReady(asyncKnightLoad) && IsLevelReady(asyncWorldLoad));
		if (isLevelReady)
		{
		    Debug.LogFormat(this, "Levels are ready before cinematics are finished. Cinematics made skippable.", Array.Empty<object>());
		}
	    }
	    SkipPromptMode skipPromptMode;
	    if(chainSequence.IsCurrentSkipped || skipChargeTimer < skipChargeDuration)
	    {
		skipPromptMode = SkipPromptMode.NOT_SKIPPABLE;
	    }
	    else if (!isLevelReady)
	    {
		skipPromptMode = SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING;
	    }
	    else
	    {
		skipPromptMode = SkipPromptMode.SKIP_PROMPT;
	    }
	    if(GameManager.instance.inputHandler.skipMode != skipPromptMode)
	    {
		GameManager.instance.inputHandler.SetSkipMode(skipPromptMode);
	    }
	    yield return null;
	}
	if (!isLevelReady)
	{
	    Debug.LogFormat(this, "Cinematics are finished before levels are ready. Blocking.", Array.Empty<object>());
	}
	Application.backgroundLoadingPriority = completedLoadPriority;
	GameManager.instance.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
	yield return new WaitForSeconds(1.2f);
	asyncKnightLoad.allowSceneActivation = true;
	yield return asyncKnightLoad;
	asyncKnightLoad = null;
	GameManager.instance.OnWillActivateFirstLevel();
	asyncWorldLoad.allowSceneActivation = true;
	GameManager.instance.nextSceneName = "Tutorial_01";
	yield return asyncWorldLoad;
	asyncWorldLoad = null;
	Application.backgroundLoadingPriority = lastLoadPriority;
	UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(gameObject.scene);
	GameManager.instance.SetupSceneRefs(true);
	GameManager.instance.BeginScene();
	GameManager.instance.OnNextLevelReady();
    }

    private IEnumerator StartSync()
    {
	GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE SCENE IN");

	PlayMakerFSM.BroadcastEvent("START FADE OUT");
	Debug.LogFormat(this, "Starting opening sequence.", Array.Empty<object>());
	GameManager.instance.ui.SetState(UIState.CUTSCENE);
	chainSequence.Begin();
	while (chainSequence.IsPlaying)
	{
	    SkipPromptMode skipPromptMode;
	    if (chainSequence.IsCurrentSkipped || skipChargeTimer < skipChargeDuration)
	    {
		skipPromptMode = SkipPromptMode.NOT_SKIPPABLE;
	    }
	    else
	    {
		skipPromptMode = SkipPromptMode.SKIP_PROMPT;
	    }
	    if (GameManager.instance.inputHandler.skipMode != skipPromptMode)
	    {
		GameManager.instance.inputHandler.SetSkipMode(skipPromptMode);
	    }
	    yield return null;
	}
	GameManager.instance.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
	AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Knight_Pickup", LoadSceneMode.Additive);
	asyncOperation.allowSceneActivation = true;
	yield return asyncOperation;
	GameManager.instance.OnWillActivateFirstLevel();
	GameManager.instance.nextSceneName = "Tutorial_01";
	AsyncOperation asyncOperation2 = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Tutorial_01", LoadSceneMode.Single);
	asyncOperation2.allowSceneActivation = true;
	yield return asyncOperation2;
	UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(gameObject.scene);
	GameManager.instance.SetupSceneRefs(true);
	GameManager.instance.BeginScene();
	GameManager.instance.OnNextLevelReady();

    }

    public IEnumerator Skip()
    {
	Debug.LogFormat("Opening sequience skipping.", Array.Empty<object>());
	chainSequence.SkipSingle();
	while (chainSequence.IsCurrentSkipped)
	{
	    skipChargeTimer = 0f;
	    yield return null;
	}
	yield break;
    }

    private void OnChangingSequences()
    {
	Debug.LogFormat("Opening sequience changing sequences.", Array.Empty<object>());
	skipChargeTimer = 0f;
	if (isAsync && asyncKnightLoad != null && !asyncKnightLoad.allowSceneActivation)
	{
	    asyncKnightLoad.allowSceneActivation = true;
	}
    }
}
