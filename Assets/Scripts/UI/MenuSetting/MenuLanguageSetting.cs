using System;
using GlobalEnums;
using HKMenu;
using Language;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class MenuLanguageSetting : MenuOptionHorizontal, IMoveHandler, IEventSystemHandler, IMenuOptionListSetting, IPointerClickHandler
    {
	private SupportedLanguages[] langs;

	public void UpdateAlpha()
	{
	    CanvasGroup component = GetComponent<CanvasGroup>();
	    if (component)
	    {
		if (!interactable)
		{
		    component.alpha = 0.5f;
		    return;
		}
		component.alpha = 1f;
	    }
	}

	public void PushUpdateOptionList()
	{
	    
	}

	public void RefreshControls()
	{
	    
	}

	public void RefreshCurrentIndex()
	{
	    
	}
    }
}
