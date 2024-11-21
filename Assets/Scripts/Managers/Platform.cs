using System;
using System.Text;
using UnityEngine;

public abstract class Platform : MonoBehaviour
{
    private static Platform current;
    public static Platform Current
    {
	get
	{
	    return current;
	}
    }


    public static event Action PlatformBecameCurrent;

    public virtual SystemLanguage GetSystemLanguage()
    {
	return Application.systemLanguage;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
	CreatePlatform().BecomeCurrent();
    }
    protected virtual void BecomeCurrent()
    {
	current = this;
	if (PlatformBecameCurrent != null)
	{
	    PlatformBecameCurrent();
	}
    }
    private static Platform CreatePlatform()
    {
	return CreatePlatform<DesktopPlatform>();
    }
    private static Platform CreatePlatform<PlatformTy>() where PlatformTy : Platform
    {
	GameObject gameObject = new GameObject(typeof(PlatformTy).Name);
	PlatformTy platformTy = gameObject.AddComponent<PlatformTy>();
	DontDestroyOnLoad(gameObject);
	return platformTy;
    }

    public virtual bool IsControllerImplicit
    {
	get
	{
	    return false;
	}
    }
    public abstract AcceptRejectInputStyles AcceptRejectInputStyle { get; }
    public MenuActions GetMenuAction(bool menuSubmitInput, bool menuCancelInput, bool jumpInput, bool attackInput, bool castInput)
    {
	bool isControllerImplicit = IsControllerImplicit;
	if (isControllerImplicit)
	{
	    if (menuSubmitInput)
	    {
		return MenuActions.Submit;
	    }
	    if (menuCancelInput)
	    {
		return MenuActions.Cancel;
	    }
	}
	AcceptRejectInputStyles acceptRejectInputStyle = AcceptRejectInputStyle;
	if (acceptRejectInputStyle != AcceptRejectInputStyles.NonJapaneseStyle)
	{
	    if (acceptRejectInputStyle == AcceptRejectInputStyles.JapaneseStyle)
	    {
		if (castInput)
		{
		    return MenuActions.Submit;
		}
		if (jumpInput || attackInput)
		{
		    return MenuActions.Cancel;
		}
	    }
	}
	else
	{
	    if (jumpInput)
	    {
		return MenuActions.Submit;
	    }
	    if (attackInput || castInput)
	    {
		return MenuActions.Cancel;
	    }
	}
	if (!isControllerImplicit)
	{
	    if (menuSubmitInput)
	    {
		return MenuActions.Submit;
	    }
	    if (menuCancelInput)
	    {
		return MenuActions.Cancel;
	    }
	}
	return MenuActions.None;
    }
  
    public abstract void EnsureSaveSlotSpace(int slotIndex, Action<bool> callback);
    public virtual bool FetchScenesBeforeFade
    {
	get
	{
	    return false;
	}
    }

    private GraphicsTiers graphicsTier;
    public GraphicsTiers GraphicsTier
    {
	get
	{
	    return graphicsTier;
	}
    }
    public GraphicsTiers InitialGraphicsTier
    {
	get
	{
	    return GraphicsTiers.High;
	}
    }

    public enum GraphicsTiers
    {
	VeryLow,
	Low,
	Medium,
	High
    }

    public enum AcceptRejectInputStyles
    {
	NonJapaneseStyle,
	JapaneseStyle
    }

    public enum MenuActions
    {
	None,
	Submit,
	Cancel
    }
}
