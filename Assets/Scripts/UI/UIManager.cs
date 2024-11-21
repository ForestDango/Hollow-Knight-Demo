using System;
using System.Collections;
using GlobalEnums;
using InControl;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("State")]
    [Space(6f)]
    public UIState uiState; //UI状态
    public MainMenuState menuState; //主菜单的界面

    [Header("Event System")]
    [Space(6f)]
    public EventSystem eventSystem;

    [Header("Main Elements")]
    [Space(6f)]
    public Canvas UICanvas;
    public CanvasGroup modalDimmer;

    [Header("Menu Audio")]
    [Space(6f)]
    public AudioMixerSnapshot gameplaySnapshot;
    public AudioMixerSnapshot menuSilenceSnapshot;
    public AudioMixerSnapshot menuPauseSnapshot;

    [Header("Main Menu")]
    [Space(6f)]
    public CanvasGroup mainMenuScreen; //主菜单界面的Cg
    public MainMenuOptions mainMenuButtons; //主菜单button选项
    public SpriteRenderer gameTitle; //游戏标题
    public PlayMakerFSM subtitleFSM; //游戏副标题FSM

    [Header("Save Profile Menu")]
    [Space(6f)]
    public CanvasGroup saveProfileScreen;
    public CanvasGroup saveProfileTitle;
    public CanvasGroup saveProfileControls;
    public Animator saveProfileTopFleur;
    public PreselectOption saveSlots;
    public SaveSlotButton slotOne;
    public SaveSlotButton slotTwo;
    public SaveSlotButton slotThree;
    public SaveSlotButton slotFour;

    [Header("Options Menu")]
    [Space(6f)]
    public MenuScreen optionsMenuScreen;

    [Header("Audio Menu")]
    [Space(6f)]
    public MenuScreen audioMenuScreen;
    public MenuAudioSlider masterSlider;
    public MenuAudioSlider musicSlider;
    public MenuAudioSlider soundSlider;

    [Header("Video Menu")]
    [Space(6f)]
    public MenuScreen videoMenuScreen;
    public VideoMenuOptions videoMenuOptions;
    public MenuResolutionSetting resolutionOption;
    public MenuSetting fullscreenOption;
    public MenuSetting vsyncOption;
    public MenuSetting particlesOption;
    public MenuSetting shadersOption;
    public MenuSetting frameCapOption;

    [Header("Game Options Menu")]
    [Space(6f)]
    public MenuScreen gameOptionsMenuScreen;
    public GameMenuOptions gameMenuOptions;

    [Header("Brightness Setting Menu")]
    [Space(6f)]
    public MenuScreen brightnessMenuScreen;
    public BrightnessSetting brightnessSetting;

    [Header("Prompts")]
    [Space(6f)]
    public MenuScreen quitGamePrompt;
    public MenuScreen returnMainMenuPrompt;
    public MenuScreen resolutionPrompt;

    [Header("Pause Menu")]
    [Space(6f)]
    public Animator pauseMenuAnimator;
    public MenuScreen pauseMenuScreen;
    private Coroutine togglePauseCo;
    private Coroutine goToPauseMenuCo;
    private Coroutine leavePauseMenuCo; 
    private bool ignoreUnpause; //判断可以取消暂停的变量

    [Header("Cinematics")]
    [SerializeField] private CinematicSkipPopup cinematicSkipPopup;

    public MenuScreen playModeMenuScreen;

    private MenuScreen activePrompt;

    private float startMenuTime;
    private bool isFadingMenu;
    public bool IsFadingMenu
    {
	get
	{
	    return isFadingMenu || Time.time < startMenuTime;
	}
    }

    private int menuAnimationCounter;
    public bool IsAnimatingMenu
    {
	get
	{
	    return menuAnimationCounter > 0;
	}
    }

    private GameManager gm;
    private GameSettings gs;
    private HeroController hero_ctrl;
    private PlayerData playerData;
    private InputHandler ih;
    private GraphicRaycaster graphicRaycaster;
    public MenuAudioController uiAudioPlayer;
    public HollowKnightInputModule inputModule;

    [Space]
    public float MENU_FADE_SPEED = 3.2f;

    private static UIManager _instance; //单例模式
    public static UIManager instance
    {
	get
	{
	    if (_instance == null)
	    {
		_instance = FindObjectOfType<UIManager>();
		if (_instance == null)
		{
		    Debug.LogError("Couldn't find a UIManager, make sure one exists in the scene.");
		}
		if (Application.isPlaying)
		{
		    DontDestroyOnLoad(_instance.gameObject);
		}
	    }
	    return _instance;
	}
    }

    private void Awake()
    {
	if(_instance == null)
	{
	    _instance = this;
	    DontDestroyOnLoad(this);
	}
	else if(this != _instance)
	{
	    Destroy(gameObject);
	    return;
	}
	graphicRaycaster = GetComponentInChildren<GraphicRaycaster>();
    }

    public void SceneInit()
    {
	if (this == _instance)
	{
	    SetupRefs();
	}
    }

    private void Start()
    {
	if(this == _instance)
	{
	    SetupRefs();
	    if (gm.IsMenuScene()) //判断当前场景是否是菜单场景
	    {
		startMenuTime = Time.time + 0.5f;
		GameCameras.instance.cameraController.FadeSceneIn();
		ConfigureMenu();
	    }
	    if(graphicRaycaster && InputHandler.Instance)
	    {
		InputHandler.Instance.OnCursorVisibilityChange += delegate (bool isVisible)
		{
		    graphicRaycaster.enabled = isVisible;
		};
	    }
	}
    }

    private void SetupRefs()
    {
	gm = GameManager.instance;
	gs = gm.gameSettings;
	playerData = PlayerData.instance;
	ih = gm.inputHandler;
	if (gm.IsGameplayScene())
	{
	    hero_ctrl = HeroController.instance;
	}
	if(gm.IsMenuScene() && gameTitle == null)
	{
	    gameTitle = GameObject.Find("LogoTitle").GetComponent<SpriteRenderer>();
	}
	if(UICanvas.worldCamera == null)
	{
	    UICanvas.worldCamera = GameCameras.instance.mainCamera;
	}
    }
    public void ConfigureMenu()
    {
	if(mainMenuButtons != null)
	{
	    mainMenuButtons.ConfigureNavigation();
	}
	if (gameMenuOptions != null)
	{
	    gameMenuOptions.ConfigureNavigation();
	}
	if (videoMenuOptions != null)
	{
	    videoMenuOptions.ConfigureNavigation();
	}
	if (uiState == UIState.MAIN_MENU_HOME)
	{
	    if (slotOne != null)
	    {
		slotOne.healthSlots.Awake();
	    }
	    if (slotTwo != null)
	    {
		slotTwo.healthSlots.Awake();
	    }
	    if (slotThree != null)
	    {
		slotThree.healthSlots.Awake();
	    }
	    if (slotFour != null)
	    {
		slotFour.healthSlots.Awake();
	    }
	}
    }

    /// <summary>
    /// 设置UI状态
    /// </summary>
    /// <param name="newState"></param>
    public void SetState(UIState newState)
    {
	if (gm == null) 
	{
	    gm = GameManager.instance;
	}
	if(newState != uiState)
	{
	    if (uiState == UIState.PAUSED && newState == UIState.PLAYING)
	    {
		UIClosePauseMenu();
	    }
	    else if (uiState == UIState.PLAYING && newState == UIState.PAUSED)
	    {
		UIGoToPauseMenu();
	    }
	    else if (newState == UIState.INACTIVE)
	    {
		DisableScreens();
	    }
	    else if (newState == UIState.MAIN_MENU_HOME)
	    {
		//TODO:
		UIGoToMainMenu();
	    }
	    else if (newState == UIState.LOADING)
	    {
		DisableScreens();
	    }
	    else if (newState == UIState.PLAYING)
	    {
		DisableScreens();
	    }
	    else if (newState == UIState.CUTSCENE)
	    {
		DisableScreens();
	    }
	    uiState = newState;
	    return;
	}
	if (newState == UIState.MAIN_MENU_HOME)
	{
	    UIGoToMainMenu();
	}
    }

    /// <summary>
    /// 关闭某些特定的屏幕
    /// </summary>
    private void DisableScreens()
    {
	for (int i = 0; i < UICanvas.transform.childCount; i++)
	{
	    if (!(UICanvas.transform.GetChild(i).name == "PauseMenuScreen"))
	    {
		UICanvas.transform.GetChild(i).gameObject.SetActive(false);
	    }
	}
    }

    /// <summary>
    /// 设置UI初始状态
    /// </summary>
    /// <param name="gameState"></param>
    public void SetUIStartState(GameState gameState)
    {
	if (gameState == GameState.MAIN_MENU)
	{
	    SetState(UIState.MAIN_MENU_HOME);
	    return;
	}
	if (gameState == GameState.LOADING)
	{
	    SetState(UIState.LOADING);
	    return;
	}
	if (gameState == GameState.ENTERING_LEVEL)
	{
	    SetState(UIState.PLAYING);
	    return;
	}
	if (gameState == GameState.PLAYING)
	{
	    SetState(UIState.PLAYING);
	    return;
	}
	if (gameState == GameState.CUTSCENE)
	{
	    SetState(UIState.CUTSCENE);
	}
    }

    /// <summary>
    /// 设置新的主菜单界面
    /// </summary>
    /// <param name="newState"></param>
    private void SetMenuState(MainMenuState newState)

    {
	menuState = newState;
    }
    public void UIGoToMainMenu()
    {
	StartMenuAnimationCoroutine(GoToMainMenu());
    }

    /// <summary>
    /// 前往主菜单界面
    /// </summary>
    /// <returns></returns>
    private IEnumerator GoToMainMenu()
    {
	if(ih == null)
	{
	    ih = InputHandler.Instance;
	}
	ih.StopUIInput();
	if (menuState == MainMenuState.OPTIONS_MENU || menuState == MainMenuState.ACHIEVEMENTS_MENU || menuState == MainMenuState.QUIT_GAME_PROMPT || menuState == MainMenuState.EXTRAS_MENU || menuState == MainMenuState.ENGAGE_MENU || menuState == MainMenuState.NO_SAVE_MENU || menuState == MainMenuState.PLAY_MODE_MENU)
	{
	    yield return StartCoroutine(HideCurrentMenu());
	}
	else if(menuState == MainMenuState.SAVE_PROFILES)
	{
	    yield return StartCoroutine(HideSaveProfileMenu());
	}
	ih.StopUIInput();
	gameTitle.gameObject.SetActive(true);
	mainMenuScreen.gameObject.SetActive(true);

	StartCoroutine(FadeInSprite(gameTitle));
	subtitleFSM.SendEvent("FADE IN");
	yield return StartCoroutine(FadeInCanvasGroup(mainMenuScreen));
	mainMenuScreen.interactable = true;
	ih.StartUIInput();
	yield return null;
	mainMenuButtons.HighlightDefault(false);
	SetMenuState(MainMenuState.MAIN_MENU);
    }

    public void UIGoToProfileMenu()
    {
	StartMenuAnimationCoroutine(GoToProfileMenu());
    }

    /// <summary>
    /// 前往存档选择界面
    /// </summary>
    /// <returns></returns>
    private IEnumerator GoToProfileMenu()
    {
	ih.StopUIInput();
	if(menuState == MainMenuState.MAIN_MENU)
	{
	    StartCoroutine(FadeOutSprite(gameTitle));
	    subtitleFSM.SendEvent("FADE OUT");
	    yield return StartCoroutine(FadeOutCanvasGroup(mainMenuScreen));
	}
	else if(menuState == MainMenuState.PLAY_MODE_MENU)
	{
	    yield return StartCoroutine(HideCurrentMenu());
	    ih.StopUIInput();
	}
	StartCoroutine(FadeInCanvasGroup(saveProfileScreen));
	saveProfileTopFleur.ResetTrigger("hide");
	saveProfileTopFleur.SetTrigger("show");
	StartCoroutine(FadeInCanvasGroup(saveProfileTitle));
	StartCoroutine(FadeInCanvasGroup(saveProfileScreen));
	StartCoroutine(PrepareSaveFilesInOrder());
	yield return new WaitForSeconds(0.165f);
	SaveSlotButton[] slotButtons = new SaveSlotButton[]
	{
	    slotOne,
	    slotTwo,
	    slotThree,
	    slotFour
	};
	int num;
	for (int i = 0; i < slotButtons.Length; i++)
	{
	    slotButtons[i].ShowRelevantModeForSaveFileState();
	    yield return new WaitForSeconds(0.165f);
	    num = i + 1;
	}
	yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.695f));
	StartCoroutine(FadeInCanvasGroup(saveProfileControls));
	ih.StartUIInput();
	yield return null;
	saveSlots.HighlightDefault(false);
	SetMenuState(MainMenuState.SAVE_PROFILES);

    }
    public void UIGoToOptionsMenu()
    {
	StartMenuAnimationCoroutine(GoToOptionsMenu());
    }

    public void UILeaveOptionsMenu()
    {
	StartMenuAnimationCoroutine(LeaveOptionsMenu());
    }


    public IEnumerator GoToOptionsMenu()
    {
	ih.StopUIInput();
	if(menuState == MainMenuState.MAIN_MENU)
	{
	    StartCoroutine(FadeOutSprite(gameTitle));
	    subtitleFSM.SendEvent("FADE OUT");
	    yield return StartCoroutine(FadeOutCanvasGroup(mainMenuScreen));
	}
	else if (menuState == MainMenuState.AUDIO_MENU || menuState == MainMenuState.VIDEO_MENU || menuState == MainMenuState.GAMEPAD_MENU || menuState == MainMenuState.GAME_OPTIONS_MENU || menuState == MainMenuState.PAUSE_MENU)
	{
	    yield return StartCoroutine(HideCurrentMenu());
	}
	else if(menuState == MainMenuState.KEYBOARD_MENU)
	{
	    yield return StartCoroutine(HideCurrentMenu());
	}
	yield return StartCoroutine(ShowMenu(optionsMenuScreen));
	SetMenuState(MainMenuState.OPTIONS_MENU);
	ih.StartUIInput();
    }

    public IEnumerator LeaveOptionsMenu()
    {
	yield return StartCoroutine(HideCurrentMenu());
	if (uiState == UIState.PAUSED)
	{
	    UIGoToPauseMenu();
	}
	else
	{
	    UIGoToMainMenu();
	}
	yield break;
    }

    public IEnumerator ShowMenu(MenuScreen menu)
    {
	isFadingMenu = true;
	ih.StopUIInput();
	if (menu.screenCanvasGroup != null)
	{
	    StartCoroutine(FadeInCanvasGroup(menu.screenCanvasGroup));
	}
	if(menu.title != null)
	{
	    StartCoroutine(FadeInCanvasGroup(menu.title));
	}
	if(menu.topFleur != null)
	{
	    yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
	    menu.topFleur.ResetTrigger("hide");
	    menu.topFleur.SetTrigger("show");
	}
	yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
	if(menu.content != null)
	{
	    StartCoroutine(FadeInCanvasGroup(menu.content));
	}
	if (menu.controls != null)
	{
	    StartCoroutine(FadeInCanvasGroup(menu.controls));
	}
	if (menu.bottomFleur != null)
	{
	    menu.bottomFleur.ResetTrigger("hide");
	    menu.bottomFleur.SetTrigger("show");
	}
	yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
	ih.StartUIInput();
	yield return null;
	menu.HighlightDefault();
	isFadingMenu = false;
    }
    public void UIGoToGameOptionsMenu()
    {
	StartMenuAnimationCoroutine(GoToGameOptionsMenu());
    }

    public void UIGoToAudioMenu()
    {
	StartMenuAnimationCoroutine(GoToAudioMenu());
    }

    public IEnumerator GoToGameOptionsMenu()
    {
	yield return StartCoroutine(HideCurrentMenu());
	yield return StartCoroutine(ShowMenu(gameOptionsMenuScreen));
	SetMenuState(MainMenuState.GAME_OPTIONS_MENU);
    }

    public IEnumerator GoToAudioMenu()
    {
	yield return StartCoroutine(HideCurrentMenu());
	yield return StartCoroutine(ShowMenu(audioMenuScreen));
	SetMenuState(MainMenuState.AUDIO_MENU);
    }

    /// <summary>
    /// 给AudioMenuScreen的DefaultsButton调用
    /// </summary>
    public void DefaultAudioMenuSettings()
    {
	gs.ResetAudioSettings();
	RefreshAudioControls();
    }

    private void RefreshAudioControls()
    {
	masterSlider.RefreshValueFromSettings();
	musicSlider.RefreshValueFromSettings();
	soundSlider.RefreshValueFromSettings();
    }

    public void UIGoToVideoMenu(bool rollbackRes = false)
    {
	StartMenuAnimationCoroutine(GoToVideoMenu(rollbackRes));
    }

    public IEnumerator GoToVideoMenu(bool rollbackRes = false)
    {
	if (menuState == MainMenuState.OPTIONS_MENU || menuState == MainMenuState.OVERSCAN_MENU || menuState == MainMenuState.BRIGHTNESS_MENU)
	{
	    yield return StartCoroutine(HideCurrentMenu());
	}
	else if (menuState == MainMenuState.RESOLUTION_PROMPT)
	{
	    if (rollbackRes)
	    {
		HideMenuInstant(resolutionPrompt);
		videoMenuScreen.gameObject.SetActive(true);
		videoMenuScreen.content.gameObject.SetActive(true);
		eventSystem.SetSelectedGameObject(null);
		resolutionOption.RollbackResolution();
	    }
	    else
	    {
		yield return StartCoroutine(HideCurrentMenu());
	    }
	}
	yield return StartCoroutine(ShowMenu(videoMenuScreen));
	SetMenuState(MainMenuState.VIDEO_MENU);
	yield break;
    }
    public void DefaultVideoMenuSettings()
    {
	gs.ResetVideoSettings();

	resolutionOption.ResetToDefaultResolution();
	fullscreenOption.UpdateSetting(gs.fullScreen);
	vsyncOption.UpdateSetting(gs.vSync);
	shadersOption.UpdateSetting((int)gs.shaderQuality);
	RefreshVideoControls();
    }

    private void RefreshVideoControls()
    {
	//todo:
	resolutionOption.RefreshControls();
	fullscreenOption.RefreshValueFromGameSettings(false);
	vsyncOption.RefreshValueFromGameSettings(true);
	frameCapOption.RefreshValueFromGameSettings(true);
	particlesOption.RefreshValueFromGameSettings(false);
	shadersOption.RefreshValueFromGameSettings(false);
    }
    public void UIGoToBrightnessMenu()
    {
	StartMenuAnimationCoroutine(GoToBrightnessMenu());
    }

    public IEnumerator GoToBrightnessMenu()
    {
	if(menuState == MainMenuState.VIDEO_MENU)
	{
	    yield return StartCoroutine(HideCurrentMenu());
	    brightnessSetting.NormalMode();
	}
	else if (menuState == MainMenuState.OVERSCAN_MENU)
	{
	    yield return StartCoroutine(HideCurrentMenu());
	    brightnessSetting.DoneMode();
	}
	else if (menuState == MainMenuState.SAVE_PROFILES)
	{
	    yield return StartCoroutine(HideSaveProfileMenu());
	    brightnessSetting.DoneMode();
	}
	else if (menuState == MainMenuState.PLAY_MODE_MENU)
	{
	    yield return StartCoroutine(HideCurrentMenu());
	    brightnessSetting.DoneMode();
	}
	yield return StartCoroutine(ShowMenu(brightnessMenuScreen));
	SetMenuState(MainMenuState.BRIGHTNESS_MENU);
    }

    public void UIShowResolutionPrompt(bool startTimer = false)
    {
	//TODO:UIShowResolutionPrompt
	Debug.LogFormat("//TODO:UIShowResolutionPrompt");
    }
    public void UIShowQuitGamePrompt()
    {
	StartMenuAnimationCoroutine(GoToQuitGamePrompt());
    }

    public IEnumerator GoToQuitGamePrompt()
    {
	ih.StopUIInput();
	if (menuState == MainMenuState.MAIN_MENU)
	{
	    StartCoroutine(FadeOutSprite(gameTitle));
	    subtitleFSM.SendEvent("FADE OUT");
	    yield return StartCoroutine(FadeOutCanvasGroup(mainMenuScreen));
	}
	else
	{
	    Debug.LogError("Switching between these menus is not implemented.");
	}
	activePrompt = quitGamePrompt;
	yield return StartCoroutine(ShowMenu(quitGamePrompt));
	SetMenuState(MainMenuState.QUIT_GAME_PROMPT);
	ih.StartUIInput();
    }

    public void QuitGame()
    {
	ih.StopUIInput();
	StartMenuAnimationCoroutine(gm.QuitGame());
    }

    public void UIGoToPauseMenu()
    {
	goToPauseMenuCo = StartMenuAnimationCoroutine(GoToPauseMenu());
    }

    public IEnumerator GoToPauseMenu()
    {
	ih.StopUIInput();
	ignoreUnpause = true;
	if (uiState == UIState.PAUSED)
	{
	    if (menuState == MainMenuState.OPTIONS_MENU || menuState == MainMenuState.EXIT_PROMPT)
	    {
		yield return StartCoroutine(HideCurrentMenu());
	    }
	}
	else
	{
	    StartCoroutine(FadeInCanvasGroupAlpha(modalDimmer, 0.8f));
	}
	yield return StartCoroutine(ShowMenu(pauseMenuScreen));
	SetMenuState(MainMenuState.PAUSE_MENU);
	ih.StartUIInput();
	ignoreUnpause = false;
	yield break;
    }

    public void UIClosePauseMenu()
    {
	ih.StopUIInput();
	StartCoroutine(HideCurrentMenu());
	StartMenuAnimationCoroutine(FadeOutCanvasGroup(modalDimmer));
    }
    public void UIClearPauseMenu()
    {
	pauseMenuAnimator.SetBool("clear", true);
    }

    public void UnClearPauseMenu()
    {
	pauseMenuAnimator.SetBool("clear", false);
    }

    public void TogglePauseGame()
    {
	if (!ignoreUnpause)
	{
	    togglePauseCo = StartCoroutine(gm.PauseGameToggleByMenu());
	}
    }

    /// <summary>
    /// 重新回到游戏界面，给pauseMenu的Continue的button使用
    /// </summary>
    public void ContinueGame()
    {
	ih.StopUIInput();
	uiAudioPlayer.PlayStartGame();
	//TODO:MenuStyles
	if (menuState == MainMenuState.SAVE_PROFILES)
	{
	    StartCoroutine(HideSaveProfileMenu());
	}
    }

    /// <summary>
    /// UI离开主菜单确认界面，给returnMainMenuPrompt的Yes和No的button使用
    /// </summary>
    public void UILeaveExitToMenuPrompt()
    {
	StartMenuAnimationCoroutine(LeaveExitToMenuPrompt());
    }

    public IEnumerator LeaveExitToMenuPrompt()
    {
	yield return StartCoroutine(HideCurrentMenu());
	if (uiState == UIState.PAUSED)
	{
	    UnClearPauseMenu();
	}
    }

    /// <summary>
    /// UI展示退出到主菜单确认界面，给pauseMenu的Quit的button使用
    /// </summary>
    public void UIShowReturnMenuPrompt()
    {
	StartMenuAnimationCoroutine(GoToReturnMenuPrompt());
    }

    public IEnumerator GoToReturnMenuPrompt()
    {
	ih.StopUIInput();
	if (menuState == MainMenuState.PAUSE_MENU)
	{
	    yield return StartCoroutine(HideCurrentMenu());
	}
	else
	{
	    Debug.LogError("Switching between these menus is not implemented.");
	}
	activePrompt = quitGamePrompt;
	yield return StartCoroutine(ShowMenu(returnMainMenuPrompt));
	SetMenuState(MainMenuState.EXIT_PROMPT);
	ih.StartUIInput();
	yield break;
    }

    public void UIReturnToMainMenu()
    {
	StartMenuAnimationCoroutine(ReturnToMainMenu());
    }

    public IEnumerator ReturnToMainMenu()
    {
	ih.StopUIInput();
	bool calledBack = false;
	bool willSave = true; //TODO:
	GameManager.ReturnToMainMenuSaveModes saveMode = GameManager.ReturnToMainMenuSaveModes.SaveAndCancelOnFail;
	if (!willSave)
	{
	    saveMode = GameManager.ReturnToMainMenuSaveModes.SaveAndContinueOnFail;
	}
	StartCoroutine(gm.ReturnToMainMenu(saveMode, delegate (bool willComplete)
	{
	    calledBack = true;
	    if (!willComplete && willSave)
	    {
		ih.StartUIInput();
		returnMainMenuPrompt.HighlightDefault();
		return;
	    }
	    StartCoroutine(HideCurrentMenu());
	}));
	while (!calledBack)
	{
	    yield return null;
	}
    }

    public void UIStartNewGame()
    {
	StartNewGame(false, false);
    }

    /// <summary>
    /// 开启新存档游戏
    /// </summary>
    /// <param name="permaDeath"></param>
    /// <param name="bossRush"></param>
    private void StartNewGame(bool permaDeath = false, bool bossRush = false)
    {
	uiAudioPlayer.PlayStartGame();
	gm.EnsureSaveSlotSpace(delegate (bool hasSpace)
	{
	    if (hasSpace)
	    {
		if (menuState == MainMenuState.SAVE_PROFILES)
		{
		    StartCoroutine(HideSaveProfileMenu());
		}
		else
		{
		    StartCoroutine(HideCurrentMenu());
		}
		uiAudioPlayer.PlayStartGame();
		gm.StartNewGame(permaDeath, bossRush);
		return;
	    }
	    ih.StartUIInput();
	    SaveSlotButton saveSlotButton;
	    switch (gm.profileID)
	    {
		default:
		    saveSlotButton = slotOne;
		    break;
		case 2:
		    saveSlotButton = slotTwo;
		    break;
		case 3:
		    saveSlotButton = slotThree;
		    break;
		case 4:
		    saveSlotButton = slotFour;
		    break;
	    }
	    saveSlotButton.Select();
	});
    }

    /// <summary>
    /// 预备保存存档顺序队列
    /// </summary>
    /// <returns></returns>
    private IEnumerator PrepareSaveFilesInOrder()
    {
	SaveSlotButton[] slotButtons = new SaveSlotButton[]
	{
	    slotOne,
	    slotTwo,
	    slotThree,
	    slotFour
	};
	int num;
	for (int i = 0; i < slotButtons.Length; i++)
	{
	    SaveSlotButton slotButton = slotButtons[i];
	    if (slotButton.saveFileState == SaveSlotButton.SaveFileStates.NotStarted)
	    {
		slotButton.Prepare(gm, false);
		while (slotButton.saveFileState == SaveSlotButton.SaveFileStates.OperationInProgress)
		{
		    yield return null;
		}
	    }
	    slotButton = null;
	    num = i + 1;
	}
	yield return null;
    }

    /// <summary>
    /// 隐藏选择存档界面
    /// </summary>
    /// <returns></returns>
    public IEnumerator HideSaveProfileMenu()
    {
	StartCoroutine(FadeOutCanvasGroup(saveProfileTitle));
	saveProfileTopFleur.ResetTrigger("show");
	saveProfileTopFleur.SetTrigger("hide");
	yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.165f));
	slotOne.HideSaveSlot();
	yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.165f));
	slotTwo.HideSaveSlot();
	yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.165f));
	slotThree.HideSaveSlot();
	yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.165f));
	slotFour.HideSaveSlot();
	yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.33f));
	yield return StartCoroutine(FadeOutCanvasGroup(saveProfileControls));
	yield return StartCoroutine(FadeOutCanvasGroup(saveProfileScreen));
    }

    /// <summary>
    /// 隐藏当前界面
    /// </summary>
    /// <returns></returns>
    public IEnumerator HideCurrentMenu()
    {
	isFadingMenu = true;
	MenuScreen menu = null;
	switch (menuState)
	{
	    case MainMenuState.OPTIONS_MENU:
		menu = optionsMenuScreen;
		goto IL_268;
	    case MainMenuState.GAMEPAD_MENU:
		goto IL_268;
	    case MainMenuState.KEYBOARD_MENU:
		goto IL_268;
	    case MainMenuState.SAVE_PROFILES:
		goto IL_268;
	    case MainMenuState.AUDIO_MENU:
		menu = audioMenuScreen;
		gs.SaveAudioSettings();
		goto IL_268;
	    case MainMenuState.VIDEO_MENU:
		menu = videoMenuScreen;
		goto IL_268;
	    case MainMenuState.EXIT_PROMPT:
		menu = returnMainMenuPrompt;
		goto IL_268;
	    case MainMenuState.OVERSCAN_MENU:
		goto IL_268;
	    case MainMenuState.GAME_OPTIONS_MENU:
		menu = gameOptionsMenuScreen;
		goto IL_268;
	    case MainMenuState.ACHIEVEMENTS_MENU:
		goto IL_268;
	    case MainMenuState.QUIT_GAME_PROMPT:
		menu = quitGamePrompt;
		goto IL_268;
	    case MainMenuState.RESOLUTION_PROMPT:
		goto IL_268;
	    case MainMenuState.BRIGHTNESS_MENU:
		menu = brightnessMenuScreen;
		goto IL_268;
	    case MainMenuState.PAUSE_MENU:
		menu = pauseMenuScreen;
		goto IL_268;
	    case MainMenuState.PLAY_MODE_MENU:
		menu = playModeMenuScreen;
		goto IL_268;
	    case MainMenuState.EXTRAS_MENU:
		goto IL_268;
	    case MainMenuState.REMAP_GAMEPAD_MENU:
		goto IL_268;
	    case MainMenuState.ENGAGE_MENU:
		goto IL_268;
	    case MainMenuState.NO_SAVE_MENU:
		goto IL_268;
	}
	yield break;
	IL_268:
	ih.StopUIInput();
	if (menu.title != null)
	{
	    StartCoroutine(FadeOutCanvasGroup(menu.title));
	    yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
	}
	if (menu.topFleur != null)
	{
	    menu.topFleur.ResetTrigger("show");
	    menu.topFleur.SetTrigger("hide");
	    yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
	}
	if (menu.content != null)
	{
	    StartCoroutine(FadeOutCanvasGroup(menu.content));
	}
	if (menu.controls != null)
	{
	    StartCoroutine(FadeOutCanvasGroup(menu.controls));
	}
	if (menu.bottomFleur != null)
	{
	    menu.bottomFleur.ResetTrigger("show");
	    menu.bottomFleur.SetTrigger("hide");
	    yield return StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
	}
	if (menu.screenCanvasGroup != null)
	{
	    yield return StartCoroutine(FadeOutCanvasGroup(menu.screenCanvasGroup));
	}
	ih.StartUIInput();
	isFadingMenu = false;
    }

    public void HideMenuInstant(MenuScreen menu)
    {
	ih.StopUIInput();
	if (menu.title != null)
	{
	    HideCanvasGroup(menu.title);
	}
	if (menu.topFleur != null)
	{
	    menu.topFleur.ResetTrigger("show");
	    menu.topFleur.SetTrigger("hide");
	}
	if (menu.content != null)
	{
	    HideCanvasGroup(menu.content);
	}
	if (menu.controls != null)
	{
	    HideCanvasGroup(menu.controls);
	}
	HideCanvasGroup(menu.screenCanvasGroup);
	ih.StartUIInput();
    }

    public void ShowCutscenePrompt(CinematicSkipPopup.Texts text)
    {
	cinematicSkipPopup.gameObject.SetActive(true);
	cinematicSkipPopup.Show(text);
    }

    public void HideCutscenePrompt()
    {
	cinematicSkipPopup.Hide();
    }


    public void MakeMenuLean()
    {
	Debug.Log("Making UI menu lean.");
	if (saveProfileScreen)
	{
	    Destroy(saveProfileScreen.gameObject);
	    saveProfileScreen = null;
	}
	//TODO:
    }
    public void ShowCanvasGroup(CanvasGroup cg)
    {
	cg.gameObject.SetActive(true);
	cg.interactable = true;
	cg.alpha = 1f;
    }

    public void HideCanvasGroup(CanvasGroup cg)
    {
	cg.interactable = false;
	cg.alpha = 0f;
	cg.gameObject.SetActive(false);
    }

    public void AudioGoToPauseMenu(float duration)
    {
	menuPauseSnapshot.TransitionTo(duration);
    }
    public void AudioGoToGameplay(float duration)
    {
	gameplaySnapshot.TransitionTo(duration);
    }

    private Coroutine StartMenuAnimationCoroutine(IEnumerator routine)
    {
	return StartCoroutine(StartMenuAnimationCoroutineWorker(routine));
    }
    private IEnumerator StartMenuAnimationCoroutineWorker(IEnumerator routine)
    {
	menuAnimationCounter++;
	yield return StartCoroutine(routine);
	menuAnimationCounter--;
    }
    /// <summary>
    /// 线性插值淡入CanvasGroup
    /// </summary>
    /// <param name="cg"></param>
    /// <returns></returns>
    public IEnumerator FadeInCanvasGroup(CanvasGroup cg)
    {
	float loopFailsafe = 0f;
	cg.alpha = 0f;
	cg.gameObject.SetActive(true);
	while (cg.alpha < 1f)
	{
	    cg.alpha += Time.unscaledDeltaTime * MENU_FADE_SPEED;
	    loopFailsafe += Time.unscaledDeltaTime;
	    if (cg.alpha >= 0.95f)
	    {
		cg.alpha = 1f;
		break;
	    }
	    if (loopFailsafe >= 2f)
	    {
		break;
	    }
	    yield return null;
	}
	cg.alpha = 1f;
	cg.interactable = true;
	cg.gameObject.SetActive(true);
	yield return null;
	yield break;
    }
    /// <summary>
    /// 线性插值淡出CanvasGroup
    /// </summary>
    /// <param name="cg"></param>
    /// <returns></returns>
    public IEnumerator FadeOutCanvasGroup(CanvasGroup cg)
    {
	float loopFailsafe = 0f;
	cg.interactable = false;
	while(cg.alpha > 0.05f)
	{
	    cg.alpha -= Time.unscaledDeltaTime * MENU_FADE_SPEED;
	    loopFailsafe += Time.unscaledDeltaTime;
	    if(cg.alpha <= 0.05f || loopFailsafe >= 2f)
	    {
		break;
	    }
	    yield return null;
	}
	cg.alpha = 0f;
	cg.gameObject.SetActive(false);
	yield return null;
    }
    /// <summary>
    /// 线性插值淡入SpriteRenderer
    /// </summary>
    /// <param name="sprite"></param>
    /// <returns></returns>
    private IEnumerator FadeInSprite(SpriteRenderer sprite)
    {
	while (sprite.color.a < 1f)
	{
	    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a + Time.unscaledDeltaTime * MENU_FADE_SPEED);
	    yield return null;
	}
	sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
	yield return null;
    }
    /// <summary>
    /// 线性插值淡出SpriteRenderer
    /// </summary>
    /// <param name="sprite"></param>
    /// <returns></returns>
    private IEnumerator FadeOutSprite(SpriteRenderer sprite)
    {
	while(sprite.color.a > 0f)
	{
	    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - Time.unscaledDeltaTime * MENU_FADE_SPEED);
	    yield return null;
	}
	sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
	yield return null;
    }
    public IEnumerator FadeInCanvasGroupAlpha(CanvasGroup cg, float endAlpha)
    {
	float loopFailsafe = 0f;
	if (endAlpha > 1f)
	{
	    endAlpha = 1f;
	}
	cg.alpha = 0f;
	cg.gameObject.SetActive(true);
	while (cg.alpha < endAlpha - 0.05f)
	{
	    cg.alpha += Time.unscaledDeltaTime * MENU_FADE_SPEED;
	    loopFailsafe += Time.unscaledDeltaTime;
	    if (cg.alpha >= endAlpha - 0.05f)
	    {
		cg.alpha = endAlpha;
		break;
	    }
	    if (loopFailsafe >= 2f)
	    {
		break;
	    }
	    yield return null;
	}
	cg.alpha = endAlpha;
	cg.interactable = true;
	cg.gameObject.SetActive(true);
	yield return null;
	yield break;
    }
}
