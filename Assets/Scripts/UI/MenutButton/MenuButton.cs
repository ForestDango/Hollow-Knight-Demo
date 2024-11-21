using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class MenuButton : MenuSelectable,ISubmitHandler,IEventSystemHandler,IPointerClickHandler
    {
	public MenuButtonType buttonType;
	public Animator flashEffect;
	private new void Start()
	{
	    HookUpAudioPlayer();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
	    OnSubmit(eventData);
	}

	public void OnSubmit(BaseEventData eventData)
	{
	    if(buttonType == MenuButtonType.Proceed)
	    {
		try
		{
		    flashEffect.ResetTrigger("Flash");
		    flashEffect.SetTrigger("Flash");
		}
		catch
		{

		}
		ForceDeselect();
	    }
	    else if(buttonType == MenuButtonType.Activate)
	    {
		try
		{
		    flashEffect.ResetTrigger("Flash");
		    flashEffect.SetTrigger("Flash");
		}
		catch
		{

		}
		PlaySubmitSound();
	    }
	}

        public enum MenuButtonType
	{
	    Proceed,
	    Activate
	}
    }
}