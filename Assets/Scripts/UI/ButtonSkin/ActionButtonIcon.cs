using System;
using GlobalEnums;
using InControl;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ActionButtonIcon : ActionButtonIconBase
{
    public bool showForControllerOnly;
    private Vector3? initialScale;

    public HeroActionButton action;
    public override HeroActionButton Action
    {
	get
	{
	    return action;
	}
    }
    protected override void OnEnable()
    {
	OnIconUpdate += CheckHideIcon;
	base.OnEnable();
    }

    protected override void OnDisable()
    {
	OnIconUpdate -= CheckHideIcon;
	base.OnDisable();
    }

    private void CheckHideIcon()
    {
	if (showForControllerOnly && sr)
	{
	    if (initialScale == null)
	    {
		initialScale = new Vector3?(transform.localScale);
	    }
	    InputHandler instance = InputHandler.Instance;
	    if (instance.lastActiveController == BindingSourceType.KeyBindingSource || instance.lastActiveController == BindingSourceType.None)
	    {
		transform.localScale = Vector3.zero;
		return;
	    }
	    if (instance.lastActiveController == BindingSourceType.DeviceBindingSource)
	    {
		transform.localScale = initialScale.Value;
	    }
	}
    }

    public void SetAction(HeroActionButton action)
    {
	this.action = action;
	GetButtonIcon(action);
    }
    public void SetActionString(string action)
    {
	Debug.LogFormat("TODO:Set Action String");
    }

}
