using System;
using GlobalEnums;
using InControl;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ActionButtonIcon : ActionButtonIconBase
{
    public HeroActionButton action;
    public override HeroActionButton Action
    {
	get
	{
	    return action;
	}
    }

    public void SetActionString(string action)
    {
	Debug.LogFormat("TODO:Set Action String");
    }

}
