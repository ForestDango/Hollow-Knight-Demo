using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using InControl;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public InputDevice gameController;
    public HeroActions inputActions;

    public void Awake()
    {
	inputActions = new HeroActions();

    }

    public void Start()
    {
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
    }

    private void MapKeyboardLayoutFromGameSettings()
    {
	AddKeyBinding(inputActions.up, "UpArrow");
	AddKeyBinding(inputActions.down, "DownArrow");
	AddKeyBinding(inputActions.left, "LeftArrow");
	AddKeyBinding(inputActions.right, "RightArrow");
	AddKeyBinding(inputActions.attack, "Z");
	AddKeyBinding(inputActions.jump, "X");
	AddKeyBinding(inputActions.dash, "D");
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

}
