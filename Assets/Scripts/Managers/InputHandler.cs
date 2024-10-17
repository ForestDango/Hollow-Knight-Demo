using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GlobalEnums;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    [SerializeField] public bool pauseAllowed { get; private set; }
    public bool acceptingInput = true;

    public bool skippingCutscene;
    private float skipCooldownTime;

    private bool isGameplayScene;
    private bool isMenuScene;

    public static InputHandler Instance;
    private GameManager gm;
    private PlayerData playerData;

    public InputDevice gameController;
    public HeroActions inputActions;

    public BindingSourceType lastActiveController;
    public InputDeviceStyle lastInputDeviceStyle;

    public delegate void CursorVisibilityChange(bool isVisible); //指针显示变化时发生的委托
    public event CursorVisibilityChange OnCursorVisibilityChange;//指针显示变化时发生的事件

    public bool readyToSkipCutscene;
    public SkipPromptMode skipMode { get; private set; }

    public delegate void ActiveControllerSwitch();
    public event ActiveControllerSwitch RefreshActiveControllerEvent;

    public void Awake()
    {
	Instance = this;
	gm = GetComponent<GameManager>();
	inputActions = new HeroActions();
	acceptingInput = true;
	pauseAllowed = true;
	skipMode = SkipPromptMode.NOT_SKIPPABLE;

    }

    public void Start()
    {
	playerData = gm.playerData;
	SetupNonMappableBindings();
	MapKeyboardLayoutFromGameSettings();
	if(InputManager.ActiveDevice != null && InputManager.ActiveDevice.IsAttached)
	{

	}
	else
	{
	    gameController = InputDevice.Null;
	}
	Debug.LogFormat("Input Device set to {0}.", new object[]
	{
	    gameController.Name
	});
	lastActiveController = BindingSourceType.None;
    }

    private void Update()
    {
	UpdateActiveController();
	if (acceptingInput)
	{
	    if(gm.gameState == GameState.PLAYING)
	    {
		PlayingInput();
	    }
	    else if(gm.gameState == GameState.CUTSCENE)
	    {
		CutSceneInput();
	    }
	}
    }

    public void UpdateActiveController()
    {
	if (lastActiveController != inputActions.LastInputType || lastInputDeviceStyle != inputActions.LastDeviceStyle)
	{
	    lastActiveController = inputActions.LastInputType;
	    lastInputDeviceStyle = inputActions.LastDeviceStyle;
	    if (RefreshActiveControllerEvent != null)
	    {
		RefreshActiveControllerEvent();
	    }
	}
    }
    private void PlayingInput()
    {

    }

    private void CutSceneInput()
    {
	if (!Input.anyKeyDown && !gameController.AnyButton.WasPressed)
	{
	    return;
	}
	if (skippingCutscene)
	{
	    return;
	}
	switch (skipMode)
	{
	    case SkipPromptMode.SKIP_PROMPT:
		if (!readyToSkipCutscene)
		{
		    //TODO:
		    gm.ui.ShowCutscenePrompt(CinematicSkipPopup.Texts.Skip);
		    readyToSkipCutscene = true;
		    CancelInvoke("StopCutsceneInput");
		    Invoke("StopCutsceneInput", 5f * Time.timeScale);
		    skipCooldownTime = Time.time + 0.3f;
		    return;
		}
		if(Time.time < skipCooldownTime)
		{
		    return;
		}
		CancelInvoke("StopCutsceneInput");
		readyToSkipCutscene = false;
		skippingCutscene = true;
		gm.SkipCutscene();
		return;
	    case SkipPromptMode.SKIP_INSTANT:
		skippingCutscene = true;
		gm.SkipCutscene();
		return;
	    case SkipPromptMode.NOT_SKIPPABLE:
		return;
	    case SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING:
		gm.ui.ShowCutscenePrompt(CinematicSkipPopup.Texts.Skip);
		CancelInvoke("StopCutsceneInput");
		Invoke("StopCutsceneInput", 5f * Time.timeScale);
		break;
	    default:
		return;
	}
    }

    private void StopCutsceneInput()
    {
	readyToSkipCutscene = false;
	gm.ui.HideCutscenePrompt();
    }

    private void MapKeyboardLayoutFromGameSettings()
    {
	AddKeyBinding(inputActions.menuSubmit, "Return");
	AddKeyBinding(inputActions.menuCancel, "Escape");
	AddKeyBinding(inputActions.up, "UpArrow");
	AddKeyBinding(inputActions.down, "DownArrow");
	AddKeyBinding(inputActions.left, "LeftArrow");
	AddKeyBinding(inputActions.right, "RightArrow");
	AddKeyBinding(inputActions.attack, "Z");
	AddKeyBinding(inputActions.jump, "X");
	AddKeyBinding(inputActions.dash, "D");
	AddKeyBinding(inputActions.cast, "F");
	AddKeyBinding(inputActions.quickCast, "Q");
	AddKeyBinding(inputActions.openInventory, "I");
    }

    private void SetupNonMappableBindings()
    {
	inputActions = new HeroActions();
	inputActions.menuSubmit.AddDefaultBinding(new Key[]
	{
	    Key.Return
	});
	inputActions.menuCancel.AddDefaultBinding(new Key[]
	{
	    Key.Escape
	});
	inputActions.up.AddDefaultBinding(new Key[]
	{
	    Key.UpArrow
	});
	inputActions.down.AddDefaultBinding(new Key[]
	{
	    Key.DownArrow
	});
	inputActions.left.AddDefaultBinding(new Key[]
	{
	    Key.LeftArrow
	});
	inputActions.right.AddDefaultBinding(new Key[]
	{
	    Key.RightArrow
	});
	inputActions.attack.AddDefaultBinding(new Key[]
	{
	    Key.Z
	});
	inputActions.jump.AddDefaultBinding(new Key[]
	{
	    Key.X
	});
	inputActions.dash.AddDefaultBinding(new Key[]
	{
	    Key.D
	});
	inputActions.cast.AddDefaultBinding(new Key[]
	{
	    Key.F
	});
	inputActions.quickCast.AddDefaultBinding(new Key[]
	{
	    Key.Q
	});
	inputActions.openInventory.AddDefaultBinding(new Key[]
	{
	    Key.I
	});
    }


    private static void AddKeyBinding(PlayerAction action, string savedBinding)
    {
	Mouse mouse = Mouse.None;
	Key key;
	if (!Enum.TryParse(savedBinding, out key) && !Enum.TryParse(savedBinding, out mouse))
	{
	    return;
	}
	if (mouse != Mouse.None)
	{
	    action.AddBinding(new MouseBindingSource(mouse));
	    return;
	}
	action.AddBinding(new KeyBindingSource(new Key[]
	{
	    key
	}));
    }

    public void SceneInit()
    {
	if (gm.IsGameplayScene())
	{
	    isGameplayScene = true;
	}
	else
	{
	    isGameplayScene = false;
	}
	if (gm.IsMenuScene())
	{
	    isMenuScene = true;
	}
	else
	{
	    isMenuScene = false;
	}
    }

    public void SetSkipMode(SkipPromptMode newMode)
    {
	Debug.Log("Setting skip mode: " + newMode.ToString());
	if (newMode == SkipPromptMode.NOT_SKIPPABLE)
	{
	    StopAcceptingInput();
	}
	else if (newMode == SkipPromptMode.SKIP_PROMPT)
	{
	    readyToSkipCutscene = false;
	    StartAcceptingInput();
	}
	else if (newMode == SkipPromptMode.SKIP_INSTANT)
	{
	    StartAcceptingInput();
	}
	else if (newMode == SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING)
	{
	    readyToSkipCutscene = false;
	    StartAcceptingInput();
	}
	skipMode = newMode;
    }


    public void StopUIInput()
    {
	acceptingInput = false;
	EventSystem.current.sendNavigationEvents = false;
	UIManager.instance.inputModule.allowMouseInput = false;
    }

    public void StartUIInput()
    {
	acceptingInput = true;
	EventSystem.current.sendNavigationEvents = true;
	UIManager.instance.inputModule.allowMouseInput = true;
    }

    public void StopMouseInput()
    {
	UIManager.instance.inputModule.allowMouseInput = false;
    }

    public void StartMouseInput()
    {
	UIManager.instance.inputModule.allowMouseInput = true;
    }

    public void PreventPause()
    {
	
    }

    public void StopAcceptingInput()
    {
	acceptingInput = false;
    }

    public void StartAcceptingInput()
    {
	acceptingInput = true;
    }

    public void AllowPause()
    {
	pauseAllowed = true;
    }

}
