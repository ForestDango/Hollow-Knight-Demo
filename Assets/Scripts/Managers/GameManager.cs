using System;
using GlobalEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int profileID; //这个用来记录这是第几个saveslot的,但需要注意的是，第2,3,4个slot对应的是相同的数字，而第一个slot是除了2,3,4数字以外的数字。
    public GameState gameState;
    public bool isPaused;

    public TimeScaleIndependentUpdate timeTool;
    private int timeSlowedCount;
    public bool TimeSlowed
    {
	get
	{
	    return timeSlowedCount > 0;
	}
    }

    [SerializeField] public PlayerData playerData;
    public InputHandler inputHandler{ get; private set; }
    public SceneManager sm { get; private set; }
    public HeroController hero_ctrl { get; private set; }
    public CameraController cameraCtrl { get; private set; }
    public UIManager ui { get; private set; }

    [SerializeField] private AudioManager audioManager;
    public AudioManager AudioManager
    {
	get
	{
	    return audioManager;
	}
    }
    private GameCameras gameCams;

    private bool hazardRespawningHero;

    public bool startedOnThisScene = true;
    public float sceneWidth;//场景宽度
    public float sceneHeight;//场景高度
    public tk2dTileMap tilemap{ get; private set; }
    private static readonly string[] SubSceneNameSuffixes = new string[]
	{
	    "_boss_defeated",
	    "_boss",
	    "_preload"
	};

    private SceneLoad sceneLoad;
    public bool RespawningHero { get; set; }
    public bool IsInSceneTransition { get; private set; }
    private bool isLoading;
    private int sceneLoadsWithoutGarbageCollect;
    private SceneLoadVisualizations loadVisualization;
    [Space]
    public string sceneName;
    public string nextSceneName;
    public string entryGateName;
    private string targetScene;
    private float entryDelay;
    private bool hasFinishedEnteringScene;
    public bool HasFinishedEnteringScene
    {
	get
	{
	    return hasFinishedEnteringScene;
	}
    }

    private bool waitForManualLevelStart;

    public delegate void SceneTransitionBeganDelegate(SceneLoad sceneLoad);
    public static event SceneTransitionBeganDelegate SceneTransitionBegan;

    public delegate void SceneTransitionFinishEvent();
    public event SceneTransitionFinishEvent OnFinishedSceneTransition;

    public delegate void UnloadLevel();
    public event UnloadLevel UnloadingLevel;

    public delegate void EnterSceneEvent();
    public event EnterSceneEvent OnFinishedEnteringScene;

    public static GameManager _instance;
    public static GameManager instance
    {
	get
	{
	    if (_instance == null)
	    {
		_instance = FindObjectOfType<GameManager>();
		if (_instance == null)
		{
		    Debug.LogError("Couldn't find a Game Manager, make sure one exists in the scene.");
		}
		else if (Application.isPlaying)
		{
		    DontDestroyOnLoad(_instance.gameObject);
		}
	    }
	    return _instance;
	}
    }
    public static GameManager UnsafeInstance
    {
	get
	{
	    return _instance;
	}
    }

    private void Awake()
    {
	if(_instance == null)
	{
	    _instance = this;
	    DontDestroyOnLoad(this);
	    SetupGameRefs();
	    return;
	}
	if(this != _instance)
	{
	    Destroy(gameObject);
	    return;
	}
	SetupGameRefs();
    }

    private void SetupGameRefs()
    {
	playerData = PlayerData.instance;
	gameCams = GameCameras.instance;
	cameraCtrl = gameCams.cameraController;
	inputHandler = GetComponent<InputHandler>();
	if (inputHandler == null)
	{
	    Debug.LogError("Couldn't find InputHandler component.");
	}
	UnityEngine.SceneManagement.SceneManager.activeSceneChanged += LevelActivated;

    }

    private void OnDisable()
    {
	UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= LevelActivated;
    }

    private void LevelActivated(Scene sceneFrom, Scene sceneTo)
    {
	if(this == _instance)
	{
	    if (!waitForManualLevelStart)
	    {
		Debug.LogFormat(this, "Performing automatic level start.", Array.Empty<object>());
		if (startedOnThisScene && IsGameplayScene())
		{

		}
		SetupSceneRefs(true);
		BeginScene();
		OnNextLevelReady();
		return;
	    }
	}
    }

    public void SetupSceneRefs(bool refreshTilemapInfo)
    {
	UpdateSceneName();
	if(ui == null)
	{
	    ui = UIManager.instance;
	}
	GameObject gameObject = GameObject.FindGameObjectWithTag("SceneManager");
	if(gameObject != null)
	{
	    sm = gameObject.GetComponent<SceneManager>();
	}
	else
	{
	    Debug.Log("Scene Manager missing from scene " + sceneName);
	}
	if (IsGameplayScene())
	{
	    if (hero_ctrl == null)
	    {
		SetupHeroRefs();
	    }
	    if (refreshTilemapInfo)
	    {
		RefreshTilemapInfo(sceneName);
	    }
	}
    }

    private void SetupHeroRefs()
    {
	hero_ctrl = HeroController.instance;

    }

    public void StartNewGame(bool permadeathMode = false, bool bossRushMode = false)
    {
	if (permadeathMode)
	{

	}
	else
	{

	}
	if (bossRushMode)
	{

	}
	StartCoroutine(RunStartNewGame());
    }

    private IEnumerator RunStartNewGame()
    {
	cameraCtrl.FadeOut(CameraFadeType.START_FADE);
	//TODO:AudioSnap

	yield return new WaitForSeconds(2.6f);
	ui.MakeMenuLean();
	BeginSceneTransiton(new SceneLoadInfo
	{
	    AlwaysUnloadUnusedAssets = true,
	    IsFirstLevelForPlayer = true,
	    PreventCameraFadeOut = true,
	    WaitForSceneTransitionCameraFade = false,
	    SceneName = "Opening_Sequence",
	    Visualization = SceneLoadVisualizations.Custom
	});
    }

    public void OnWillActivateFirstLevel()
    {
	HeroController.instance.isEnteringFirstLevel = true;
	entryGateName = "top1";
	SetState(GameState.PLAYING);
	ui.ConfigureMenu();
    }

    public IEnumerator LoadFirstScene()
    {
	yield return new WaitForEndOfFrame();
	OnWillActivateFirstLevel();
	LoadScene("Tutorial_01");
    }

    public void LoadOpeningCinematic()
    {
	SetState(GameState.CUTSCENE);
	LoadScene("Intro_Cutscene");
    }

    public void LoadScene(string destName)
    {

	startedOnThisScene = false;
	nextSceneName = destName;
	if (UnloadingLevel != null)
	{
	    UnloadingLevel();
	}
	UnityEngine.SceneManagement.SceneManager.UnloadScene(destName);
    }

    public IEnumerator LoadSceneAdditive(string destScene)
    {

	startedOnThisScene = false;
	nextSceneName = destScene;
	if (UnloadingLevel != null)
	{
	    UnloadingLevel();
	}
	string exitingScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
	AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(destScene, LoadSceneMode.Additive);
	asyncOperation.allowSceneActivation = true;
	yield return asyncOperation;
	UnityEngine.SceneManagement.SceneManager.UnloadScene(exitingScene);
	RefreshTilemapInfo(destScene);
	if (IsUnloadAssetsRequired(exitingScene, destScene))
	{
	    Debug.LogFormat(this, "Unloading assets due to zone transition", Array.Empty<object>());
	    yield return Resources.UnloadUnusedAssets();
	}
	SetupSceneRefs(true);
	BeginScene();
	OnNextLevelReady();
	waitForManualLevelStart = false;
    }

    private void UpdateSceneName()
    {
	sceneName = GetBaseSceneName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 获取场景的基础名字
    /// </summary>
    /// <param name="fullSceneName"></param>
    /// <returns></returns>
    public static string GetBaseSceneName(string fullSceneName)
    {
	for (int i = 0; i < SubSceneNameSuffixes.Length; i++)
	{
	    string text = SubSceneNameSuffixes[i];
	    if (fullSceneName.EndsWith(text, StringComparison.InvariantCultureIgnoreCase))
	    {
		return fullSceneName.Substring(0, fullSceneName.Length - text.Length);
	    }
	}
	return fullSceneName;
    }

    /// <summary>
    /// 重新刷新场景的tilemap的信息
    /// </summary>
    /// <param name="targetScene"></param>
    public void RefreshTilemapInfo(string targetScene)
    {
	if (IsNonGameplayScene())
	{
	    return;
	}
	tk2dTileMap tk2dTileMap = null;
	int num = 0;
	while (tk2dTileMap == null && num < UnityEngine.SceneManagement.SceneManager.sceneCount)
	{
	    Scene sceneAt = UnityEngine.SceneManagement.SceneManager.GetSceneAt(num);
	    if (string.IsNullOrEmpty(targetScene) || !(sceneAt.name != targetScene))
	    {
		GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
		int num2 = 0;
		while (tk2dTileMap == null && num2 < rootGameObjects.Length)
		{
		    tk2dTileMap = GetTileMap(rootGameObjects[num2]);
		    num2++;
		}
	    }
	    num++;
	}
	if (tk2dTileMap == null)
	{
	    Debug.LogErrorFormat("Using fallback 1 to find tilemap. Scene {0} requires manual fixing.", new object[]
	    {
		targetScene
	    });
	    GameObject[] array = GameObject.FindGameObjectsWithTag("TileMap");
	    int num3 = 0;
	    while (tk2dTileMap == null && num3 < array.Length)
	    {
		tk2dTileMap = array[num3].GetComponent<tk2dTileMap>();
		num3++;
	    }
	}
	if (tk2dTileMap == null)
	{
	    Debug.LogErrorFormat("Using fallback 2 to find tilemap. Scene {0} requires manual fixing.", new object[]
	    {
		targetScene
	    });
	    GameObject gameObject = GameObject.Find("TileMap");
	    if (gameObject != null)
	    {
		tk2dTileMap = GetTileMap(gameObject);
	    }
	}
	if (tk2dTileMap == null)
	{
	    Debug.LogErrorFormat("Failed to find tilemap in {0} entirely.", new object[]
	    {
		targetScene
	    });
	    return;
	}
	tilemap = tk2dTileMap;
	sceneWidth = tilemap.width;
	sceneHeight = tilemap.height;
    }

    private static tk2dTileMap GetTileMap(GameObject gameObject)
    {
	if (gameObject.CompareTag("TileMap"))
	{
	    return gameObject.GetComponent<tk2dTileMap>();
	}
	return null;
    }

    public IEnumerator PlayerDeadFromHazard(float waitTime)
    {
	cameraCtrl.FreezeInPlace(true);
	NoLongerFirstGame();
	SaveLevelState();
	yield return new WaitForSeconds(waitTime);
	cameraCtrl.FadeOut(CameraFadeType.HERO_HAZARD_DEATH);
	yield return new WaitForSeconds(0.8f);
	PlayMakerFSM.BroadcastEvent("HAZARD RELOAD");
	HazardRespawn();
    }

    public void HazardRespawn()
    {
	hazardRespawningHero = true;
	entryGateName = "";
	cameraCtrl.ResetStartTimer();
	cameraCtrl.camTarget.mode = CameraTarget.TargetMode.FOLLOW_HERO;
	EnterHero(false);
    }

    public void ReadyForRespawn(bool isFirstLevelForPlayer)
    {
	RespawningHero = true;
	BeginSceneTransiton(new SceneLoadInfo
	{
	    PreventCameraFadeOut = true,
	    WaitForSceneTransitionCameraFade = false,
	    EntryGateName = "",
	    SceneName = playerData.respawnScene,
	    Visualization = (isFirstLevelForPlayer ? SceneLoadVisualizations.ContinueFromSave : SceneLoadVisualizations.Default),
	    AlwaysUnloadUnusedAssets = true,
	    IsFirstLevelForPlayer = isFirstLevelForPlayer
	});
    }

    public void EnterHero(bool additiveGateSearch = false)
    {
	if (RespawningHero)
	{
	    StartCoroutine(hero_ctrl.Respawn());
	    FinishedEnteringScene();
	    RespawningHero = false;
	    return;
	}
	if (hazardRespawningHero)
	{
	    StartCoroutine(hero_ctrl.HazardRespawn());
	    FinishedEnteringScene();
	    hazardRespawningHero = false;
	    return;
	}
	if (startedOnThisScene)
	{
	    if (IsGameplayScene())
	    {
		FinishedEnteringScene();
		FadeSceneIn();
	    }
	    return;
	}
	SetState(GameState.ENTERING_LEVEL);
	if (string.IsNullOrEmpty(entryGateName))
	{
	    Debug.LogError("No entry gate has been defined in the Game Manager, unable to move hero into position.");
	    FinishedEnteringScene();
	    return;
	}
	if (additiveGateSearch)
	{
	    Debug.Log("Searching for entry gate " + entryGateName + " !in the next scene: " + nextSceneName);
	    foreach (GameObject gameObject in UnityEngine.SceneManagement.SceneManager.GetSceneByName(nextSceneName).GetRootGameObjects() )
	    {
		TransitionPoint component = gameObject.GetComponent<TransitionPoint>();
		if(component != null && component.name == entryGateName)
		{
		    Debug.Log("SUCCESS - Found as root object");
		    StartCoroutine(hero_ctrl.EnterScene(component, entryDelay));
		    return;
		}
		if(gameObject.name == "_Transition Gates")
		{
		    TransitionPoint[] componentsInChildren = gameObject.GetComponentsInChildren<TransitionPoint>();
		    for (int i = 0; i < componentsInChildren.Length; i++)
		    {
			if(componentsInChildren[i].name == entryGateName)
			{
			    Debug.Log("SUCCESS - Found in _Transition Gates folder");
			    StartCoroutine(hero_ctrl.EnterScene(componentsInChildren[i], entryDelay));
			    return;
			}
		    }
		}
		TransitionPoint[] componentsInChildren2 = gameObject.GetComponentsInChildren<TransitionPoint>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
		    if (componentsInChildren2[j].name == entryGateName)
		    {
			Debug.Log("SUCCESS - Found in _Transition Gates folder");
			StartCoroutine(hero_ctrl.EnterScene(componentsInChildren2[j], entryDelay));
			return;
		    }
		}
	    }
	    Debug.LogError("Searching in next scene for TransitionGate failed.");
	    return;
	}
	GameObject gameObject2 = GameObject.Find(entryGateName);
	if(gameObject2 != null)
	{
	    TransitionPoint component2 = gameObject2.GetComponent<TransitionPoint>();
	    StartCoroutine(hero_ctrl.EnterScene(component2, entryDelay));
	    return;
	}
	Debug.LogError(string.Concat(new string[]
	{
	    "No entry point found with the name \"",
	    entryGateName,
	    "\" in this scene (",
	    sceneName,
	    "). Unable to move hero into position, trying alternative gates..."
	}));
	TransitionPoint[] array = FindObjectsOfType<TransitionPoint>();
	if(array != null)
	{
	    StartCoroutine(hero_ctrl.EnterScene(array[0], entryDelay));
	    return;
	}
	Debug.LogError("Could not find any gates in this scene. Trying last ditch spawn...");
	hero_ctrl.transform.SetPosition2D(tilemap.width / 2f, tilemap.height / 2f);
    }

    public void FinishedEnteringScene()
    {
	SetState(GameState.PLAYING);
	entryDelay = 0f;
	hasFinishedEnteringScene = true;
	if (OnFinishedSceneTransition != null)
	{
	    OnFinishedSceneTransition();
	}
    }

    public void EnterWithoutInput()
    {

    }

    public void FadeSceneIn()
    {
	cameraCtrl.FadeSceneIn();
    }
    public IEnumerator FadeSceneInWithDelay(float delay)
    {
	if (delay >= 0f)
	{
	    yield return new WaitForSeconds(delay);
	}
	else
	{
	    yield return null;
	}
	FadeSceneIn();
	yield break;
    }

    public void SaveLevelState()
    {
	//TODO:
    }

    public void BeginSceneTransiton(SceneLoadInfo info)
    {

	if(info.IsFirstLevelForPlayer)
	{

	}
	Debug.LogFormat("BeginSceneTransiton EntryGateName =" + info.EntryGateName);
	StartCoroutine(BeginSceneTransitionRoutine(info));
    }

    private IEnumerator BeginSceneTransitionRoutine(SceneLoadInfo info)
    {
	if (sceneLoad != null)
	{
	    Debug.LogErrorFormat(this, "Cannot scene transition to {0}, while a scene transition is in progress", new object[]
	    {
		info.SceneName
	    });
	    yield break;
	}
	IsInSceneTransition = true;
	sceneLoad = new SceneLoad(this, info.SceneName);
	isLoading = true;
	loadVisualization = info.Visualization;
	if (hero_ctrl != null)
	{

	    hero_ctrl.proxyFSM.SendEvent("HeroCtrl-LeavingScene");
	    hero_ctrl.SetHeroParent(null);
	}
	if (!info.IsFirstLevelForPlayer)
	{
	    NoLongerFirstGame();
	}
	SaveLevelState();
	SetState(GameState.EXITING_LEVEL);
	entryGateName = info.EntryGateName ?? "";
	targetScene = info.SceneName;
	if (hero_ctrl != null)
	{
	    hero_ctrl.LeaveScene(info.HeroLeaveDirection);
	}
	if (!info.PreventCameraFadeOut)
	{
	    cameraCtrl.FreezeInPlace(true);
	    cameraCtrl.FadeOut(CameraFadeType.LEVEL_TRANSITION);
	}

	startedOnThisScene = false;
	nextSceneName = info.SceneName;
	waitForManualLevelStart = true;
	if (UnloadingLevel != null)
	{
	    UnloadingLevel();
	}
	string lastSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
	sceneLoad.FetchComplete += delegate ()
	{
	    info.NotifyFetchComplete();
	};
	sceneLoad.WillActivate += delegate ()
	{

	    entryDelay = info.EntryDelay;
	};
	sceneLoad.ActivationComplete += delegate ()
	{
	    UnityEngine.SceneManagement.SceneManager.UnloadScene(lastSceneName);
	    RefreshTilemapInfo(info.SceneName);
	    sceneLoad.IsUnloadAssetsRequired = (info.AlwaysUnloadUnusedAssets || IsUnloadAssetsRequired(lastSceneName, info.SceneName));
	    bool flag2 = false;
	    if (!sceneLoad.IsUnloadAssetsRequired)
	    {
		float? beginTime = sceneLoad.BeginTime;
		if (beginTime != null && Time.realtimeSinceStartup - beginTime.Value > 0f && sceneLoadsWithoutGarbageCollect < 0f)
		{
		    flag2 = false;
		}
	    }
	    if (flag2)
	    {
		sceneLoadsWithoutGarbageCollect = 0;
	    }
	    else
	    {
		sceneLoadsWithoutGarbageCollect++;
	    }
	    sceneLoad.IsGarbageCollectRequired = flag2;
	};
	sceneLoad.Complete += delegate ()
	{
	    SetupSceneRefs(false);
	    BeginScene();

	};
	sceneLoad.Finish += delegate ()
	{
	    sceneLoad = null;
	    isLoading = false;
	    waitForManualLevelStart = false;
	    info.NotifyFetchComplete();
	    OnNextLevelReady();
	    IsInSceneTransition = false;
	    if (OnFinishedSceneTransition != null)
	    {
		OnFinishedSceneTransition();
	    }
	};
	if(SceneTransitionBegan != null)
	{
	    try
	    {
		SceneTransitionBegan(sceneLoad);
	    }
	    catch (Exception exception)
	    {
		Debug.LogError("Exception in responders to GameManager.SceneTransitionBegan. Attempting to continue load regardless.");
		Debug.LogException(exception);
	    }
	}
	sceneLoad.IsFetchAllowed = (!info.forceWaitFetch && (info.PreventCameraFadeOut));
	sceneLoad.IsActivationAllowed = false;
	sceneLoad.Begin();
	float cameraFadeTimer = 0.5f;
	for (; ; )
	{
	    bool flag = false;
	    cameraFadeTimer -= Time.unscaledDeltaTime;
	    if (info.WaitForSceneTransitionCameraFade && cameraFadeTimer > 0f)
	    {
		flag = true;
	    }
	    if (!info.IsReadyToActivate())
	    {
		flag = true;
	    }
	    if (!flag)
	    {
		break;
	    }
	    yield return null;
	}
	sceneLoad.IsFetchAllowed = true;
	sceneLoad.IsActivationAllowed = true;
    }

    public void LeftScene(bool doAdditiveLoad = false)
    {
	UnityEngine.SceneManagement.SceneManager.GetSceneByName(targetScene);	
	if (doAdditiveLoad)
	{
	    StartCoroutine(LoadSceneAdditive(targetScene));
	    return;
	};
	LoadScene(targetScene);
    }

    public void BeginScene()
    {
	inputHandler.SceneInit();

	if (hero_ctrl)
	{
	    hero_ctrl.SceneInit();
	}
	gameCams.SceneInit();
	if (IsMenuScene())
	{
	    SetState(GameState.MAIN_MENU);
	    UpdateUIStateFromGameState();

	    return;
	}
	if (IsGameplayScene())
	{
	    if ((!Application.isEditor && !Debug.isDebugBuild) || Time.renderedFrameCount > 3)
	    {
		PositionHeroAtSceneEntrance();
	    }
	    if(sm != null)
	    {

		return;
	    }
	}
	else
	{
	    if (IsNonGameplayScene())
	    {
		SetState(GameState.CUTSCENE);
		UpdateUIStateFromGameState();
		return;
	    }
	    Debug.LogError("GM - Scene type is not set to a standard scene type.");
	    UpdateUIStateFromGameState();
	}
    }

    private void UpdateUIStateFromGameState()
    {
	if(ui != null)
	{
	    ui.SetUIStartState(gameState);
	    return;
	}
	ui = FindObjectOfType<UIManager>();
	if (ui != null)
	{
	    ui.SetUIStartState(gameState);
	    return;
	}
	Debug.LogError("GM: Could not find the UI manager in this scene.");
    }

    private void PositionHeroAtSceneEntrance()
    {
	Vector2 position = FindEntryPoint(entryGateName, default(Scene)) ?? new Vector2(-2000f, 2000f);
	if(hero_ctrl != null)
	{
	    hero_ctrl.transform.SetPosition2D(position);
	}
    }

    private Vector2? FindEntryPoint(string entryPointName, Scene filterScene)
    {
	if (RespawningHero)
	{
	    Transform transform = hero_ctrl.LocateSpawnPoint();
	    if(transform != null)
	    {
		return new Vector2?(transform.transform.position);
	    }
	    return null;
	}
	else
	{
	    if (hazardRespawningHero)
	    {
		return new Vector2?(playerData.hazardRespawnLocation);
	    }
	    TransitionPoint transitionPoint = FindTransitionPoint(entryPointName, filterScene, true);
	    if(transitionPoint != null)
	    {
		return new Vector2?((Vector2)transitionPoint.transform.position + transitionPoint.entryOffset);
	    }
	    return null;
	}
    }

    private TransitionPoint FindTransitionPoint(string entryPointName, Scene filterScene, bool fallbackToAnyAvailable)
    {
	List<TransitionPoint> transitionPoints = TransitionPoint.TransitionPoints;
	for (int i = 0; i < transitionPoints.Count; i++)
	{
	    TransitionPoint transitionPoint = transitionPoints[i];
	    if(transitionPoint.name == entryPointName && (!filterScene.IsValid() || transitionPoint.gameObject.scene == filterScene))
	    {
		return transitionPoint;
	    }
	}
	if(fallbackToAnyAvailable && transitionPoints.Count > 0)
	{
	    return transitionPoints[0];
	}
	return null;
    }

    public void OnNextLevelReady()
    {
	if (IsGameplayScene())
	{
	    SetState(GameState.ENTERING_LEVEL);
	    playerData.disablePause = false;
	    inputHandler.AllowPause();
	    inputHandler.StartAcceptingInput();
	    Debug.LogFormat("OnNextLevelReady entryGateName =" + entryGateName);
	    EnterHero(true);
	    UpdateUIStateFromGameState();
	}
    }

    public bool IsUnloadAssetsRequired(string sourceSceneName, string destinationSceneName)
    {
	return false;
    }

    private void NoLongerFirstGame()
    {
	
    }

    public bool IsMenuScene()
    {
	UpdateSceneName();
	return sceneName == "Menu_Title";
    }

    public bool IsGameplayScene()
    {
	UpdateSceneName();
	return !IsNonGameplayScene();
    }

    public bool IsNonGameplayScene()
    {
	return IsCinematicScene() || sceneName == "Knight Pickup" || sceneName == "Pre_Menu_Intro" || sceneName == "Menu_Title" || sceneName == "End_Credits" || sceneName == "Menu_Credits" || sceneName == "Cutscene_Boss_Door" || sceneName == "PermaDeath_Unlock" || sceneName == "GG_Unlock" || sceneName == "GG_End_Sequence" || sceneName == "End_Game_Completion" || sceneName == "BetaEnd" || sceneName == "PermaDeath" || sceneName == "GG_Entrance_Cutscene" || sceneName == "GG_Boss_Door_Entrance";
    }

    public bool IsCinematicScene()
    {
	UpdateSceneName();
	return sceneName == "Intro_Cutscene_Prologue" || sceneName == "Opening_Sequence" || sceneName == "Prologue_Excerpt" || sceneName == "Intro_Cutscene" || sceneName == "Cinematic_Stag_travel" || sceneName == "PermaDeath" || sceneName == "Cinematic_Ending_A" || sceneName == "Cinematic_Ending_B" || sceneName == "Cinematic_Ending_C" || sceneName == "Cinematic_Ending_D" || sceneName == "Cinematic_Ending_E" || sceneName == "Cinematic_MrMushroom" || sceneName == "BetaEnd";
    }

    public bool ShouldKeepHUDCameraActive()
    {
	UpdateSceneName();
	return sceneName == "GG_Entrance_Cutscene" || sceneName == "GG_Boss_Door_Entrance" || sceneName == "GG_End_Sequence" || sceneName == "Cinematic_Ending_D";
    }

    public void SetState(GameState newState)
    {
	gameState = newState;
    }

    public int GetPlayerDataInt(string intName)
    {
	return playerData.GetInt(intName);
    }

    public bool GetPlayerDataBool(string boolName)
    {
	return playerData.GetBool(boolName);
    }

    public void IncrementPlayerDataInt(string intName)
    {
	playerData.IncrementInt(intName);
    }

    public void SetPlayerDataInt(string intName, int value)
    {
	playerData.SetInt(intName, value);
    }

    public void SetPlayerDataBool(string boolName,bool value)
    {
	playerData.SetBool(boolName, value); 
    }

    private IEnumerator SetTimeScale(float newTimeScale,float duration)
    {
	float lastTimeScale = TimeController.GenericTimeScale;
	for (float timer = 0f; timer < duration; timer += Time.unscaledDeltaTime)
	{
	    float t = Mathf.Clamp01(timer / duration);
	    SetTimeScale(Mathf.Lerp(lastTimeScale, newTimeScale, t));
	    yield return null;
	}
	SetTimeScale(newTimeScale);
    }

    private void SetTimeScale(float newTimeScale)
    {
	if(timeSlowedCount > 1)
	{
	    newTimeScale = Mathf.Min(newTimeScale, TimeController.GenericTimeScale);
	}
	TimeController.GenericTimeScale = ((newTimeScale > 0.01f) ? newTimeScale : 0f);
    }

    public void FreezeMoment(int type)
    {
	if (type == 0)
	{
	    StartCoroutine(FreezeMoment(0.01f, 0.35f, 0.1f, 0f));
	}
	else if (type == 1)
	{
	    StartCoroutine(FreezeMoment(0.04f, 0.03f, 0.04f, 0f));
	}
	else if (type == 2)
	{
	    StartCoroutine(FreezeMoment(0.25f, 2f, 0.25f, 0.15f));
	}
	else if (type == 3)
	{
	    StartCoroutine(FreezeMoment(0.01f, 0.25f, 0.1f, 0f));
	}
	if (type == 4)
	{
	    StartCoroutine(FreezeMoment(0.01f, 0.25f, 0.1f, 0f));
	}
	if (type == 5)
	{
	    StartCoroutine(FreezeMoment(0.01f, 0.25f, 0.1f, 0f));
	}
    }

    public IEnumerator FreezeMoment(float rampDownTime,float waitTime,float rampUpTime,float targetSpeed)
    {
	timeSlowedCount++;
	yield return StartCoroutine(SetTimeScale(targetSpeed, rampDownTime));
	for (float timer = 0f; timer < waitTime; timer += Time.unscaledDeltaTime)
	{
	    yield return null;
	}
	yield return StartCoroutine(SetTimeScale(1f, rampUpTime));
	timeSlowedCount--;
    }
    public void EnsureSaveSlotSpace(Action<bool> callback)
    {
	Platform.Current.EnsureSaveSlotSpace(profileID, callback);
    }

    public void StoryRecord_acquired(string item)
    {
    }

    public void AwardAchievement(string key)
    {
	//TODO:
    }

    public void SkipCutscene()
    {
	StartCoroutine(SkipCutsceneNoMash());
    }

    private IEnumerator SkipCutsceneNoMash()
    {
	if(gameState == GameState.CUTSCENE)
	{
	    ui.HideCutscenePrompt();
	    //TODO:
	    OpeningSequence openingSequence = FindObjectOfType<OpeningSequence>();
	    if(openingSequence != null)
	    {
		yield return StartCoroutine(openingSequence.Skip());
		inputHandler.skippingCutscene = false;
		yield break;
	    }
	    CinematicPlayer cinematicPlayer = FindObjectOfType<CinematicPlayer>();
	    if (cinematicPlayer != null)
	    {
		yield return StartCoroutine(cinematicPlayer.SkipVideo());
		inputHandler.skippingCutscene = false;
		yield break;
	    }
	    Debug.LogError("Unable to skip, please ensure there is a CinematicPlayer or CutsceneHelper in this scene.");
	}
	yield return null;
    }

    public float GetImplicitCinematicVolume()
    {
	//TODO:
	return Mathf.Clamp01(10f / 10f) * Mathf.Clamp01(10f / 10f);
    }

    public enum SceneLoadVisualizations
    {
	Default,
	Custom = -1,
	Dream = 1,
	Colosseum,
	GrimmDream,
	ContinueFromSave,
	GodsAndGlory
    }

    public class SceneLoadInfo
    {
	public bool IsFirstLevelForPlayer;
	public string SceneName;
	public GatePosition? HeroLeaveDirection;
	public string EntryGateName;
	public float EntryDelay;
	public bool PreventCameraFadeOut;
	public bool WaitForSceneTransitionCameraFade;
	public SceneLoadVisualizations Visualization;
	public bool AlwaysUnloadUnusedAssets;
	public bool forceWaitFetch;

	public virtual void NotifyFetchComplete()
	{
	}

	public virtual bool IsReadyToActivate()
	{
	    return true;
	}

	public virtual void NotifyFinished()
	{
	}
    }

    public enum ReturnToMainMenuSaveModes
    {
	SaveAndCancelOnFail,
	SaveAndContinueOnFail,
	DontSave
    }

    public enum ControllerConnectionStates
    {
	DetachedDevice,
	DummyDevice,
	NullDevice,
	PossiblyConnected,
	ConnectedAndReady
    }

}
