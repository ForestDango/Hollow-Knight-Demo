using System;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class ClearSaveButton : MenuButton,ISubmitHandler,IEventSystemHandler,IPointerClickHandler,ISelectHandler
    {
	public SaveSlotButton saveSlotButton;

	public new void OnSubmit(BaseEventData eventData)
	{
	    ForceDeselect();
	    saveSlotButton.ClearSavePrompt();
	}

	public new void OnPointerClick(PointerEventData eventData)
	{
	    OnSubmit(eventData);
	}

	public new void OnSelect(BaseEventData eventData)
	{
	    base.OnSelect(eventData);
	    if (!GetComponent<CanvasGroup>().interactable)
	    {
		StartCoroutine(SelectAfterFrame(navigation.selectOnLeft.gameObject));
	    }
	}

	private IEnumerator SelectAfterFrame(GameObject gameObject)
	{
	    yield return new WaitForEndOfFrame();
	    EventSystem.current.SetSelectedGameObject(gameObject);
	    yield break;
	}

    }
}
