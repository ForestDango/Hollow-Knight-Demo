using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    public CanvasGroup title;
    public Animator topFleur;
    public Animator bottomFleur;
    public CanvasGroup content;
    public CanvasGroup controls;
    public Selectable defaultHighlight;

    public CanvasGroup screenCanvasGroup
    {
	get
	{
	    return GetComponent<CanvasGroup>();
	}
    }

    public void HighlightDefault()
    {
	EventSystem current = EventSystem.current;
	if (defaultHighlight != null && current.currentSelectedGameObject == null)
	{
	    Selectable firstInteractable = defaultHighlight.GetFirstInteractable();
	    if (firstInteractable)
	    {
		firstInteractable.Select();
		foreach (object obj in defaultHighlight.transform)
		{
		    Animator component = ((Transform)obj).GetComponent<Animator>();
		    if (component != null)
		    {
			component.ResetTrigger("hide");
			component.SetTrigger("show");
			break;
		    }
		}
	    }
	}
    }

}
