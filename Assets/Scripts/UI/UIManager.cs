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
    public UIState uiState;
    public MainMenuState menuState;

    [Header("Event System")]
    [Space(6f)]
    public EventSystem eventSystem;

    [Header("Main Elements")]
    [Space(6f)]
    public Canvas UICanvas;

    [Header("Main Menu")]
    [Space(6f)]
    public CanvasGroup mainMenuScreen;
    public MainMenuOptions mainMenuButtons;
    public SpriteRenderer gameTitle;
    public PlayMakerFSM subTitleFSM;

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

    [Header("Cinematics")]
    [SerializeField] private CinematicSkipPopup cinematicSkipPopup;

    public MenuScreen playModeMenuScreen;



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
    private HeroController hero_ctrl;
    private PlayerData playerData;
    private InputHandler ih;
    private GraphicRaycaster graphicRaycaster;
    public MenuAudioController uiAudioPlayer;
    public HollowKnightInputModule inputModule;

    [Space]
    public float MENU_FADE_SPEED = 3.2f;

    private static UIManager _instance;
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
	if (this == UIManager._instance)
	{
	    SetupRefs();
	}
    }

    private void Start()
    {
	if(this == _instance)
	{
	    SetupRefs();
	    if (gm.IsMenuScene())
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
	if(uiState == UIState.MAIN_MENU_HOME)
	{

	}
    }

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

	    }
	    else if (uiState == UIState.PLAYING && newState == UIState.PAUSED)
	    {
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
    private void SetMenuState(MainMenuState newState)
    {
	menuState = newState;
    }
    public void UIGoToMainMenu()
    {
	StartMenuAnimationCoroutine(GoToMainMenu());
    }
    private IEnumerator GoToMainMenu()
    {
	Debug.LogFormat("Go To Main Menu");
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
	subTitleFSM.SendEvent("FADE IN");
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

    private IEnumerator GoToProfileMenu()
    {
	ih.StopUIInput();
	if(menuState == MainMenuState.MAIN_MENU)
	{
	    StartCoroutine(FadeOutSprite(gameTitle));
	    subTitleFSM.SendEvent("FADE OUT");
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

    public void UIStartNewGame()
    {
	StartNewGame(false, false);
    }

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

    public IEnumerator HideCurrentMenu()
    {
	isFadingMenu = true;
	MenuScreen menu;
	switch (menuState)
	{
	    case MainMenuState.OPTIONS_MENU:
		break;
	    case MainMenuState.GAMEPAD_MENU:
		break;
	    case MainMenuState.KEYBOARD_MENU:
		break;
	    case MainMenuState.SAVE_PROFILES:
		break;
	    case MainMenuState.AUDIO_MENU:
		break;
	    case MainMenuState.VIDEO_MENU:
		break;
	    case MainMenuState.EXIT_PROMPT:
		break;
	    case MainMenuState.OVERSCAN_MENU:
		break;
	    case MainMenuState.GAME_OPTIONS_MENU:
		break;
	    case MainMenuState.ACHIEVEMENTS_MENU:
		break;
	    case MainMenuState.QUIT_GAME_PROMPT:
		break;
	    case MainMenuState.RESOLUTION_PROMPT:
		break;
	    case MainMenuState.BRIGHTNESS_MENU:
		break;
	    case MainMenuState.PAUSE_MENU:
		break;
	    case MainMenuState.PLAY_MODE_MENU:
		menu = playModeMenuScreen;
		break;
	    case MainMenuState.EXTRAS_MENU:
		break;
	    case MainMenuState.REMAP_GAMEPAD_MENU:
		break;
	    case MainMenuState.ENGAGE_MENU:
		break;
	    case MainMenuState.NO_SAVE_MENU:
		break;
	    default:
		yield break;
	}
	ih.StopUIInput();
	//TODO:
	yield return null;
	ih.StartUIInput();
	isFadingMenu = false;
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
}
