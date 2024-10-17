using System;
using System.Collections;
using GlobalEnums;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UnityEngine.UI
{
    public class MenuSelectable : Selectable, ISelectHandler, IEventSystemHandler, IDeselectHandler, ICancelHandler, IPointerExitHandler
    {
	[Header("On Cancel")]
	public CancelAction cancelAction;

	[Header("Fleurs")]
	public Animator leftCursor;
	public Animator rightCursor;

	[Header("Highlight")]
	public Animator selectHighlight;
	public bool playSubmitSound = true;

	protected MenuAudioController uiAudioPlayer;
	protected GameObject prevSelectedObject; //先前选定的selectable对象
	protected bool deselectWasForced; //强制执行取消

	protected bool dontPlaySelectSound;
	public bool DontPlaySelectSound
	{
	    get
	    {
		return dontPlaySelectSound;
	    }
	    set
	    {
		dontPlaySelectSound = value;
	    }
	}

	private MenuButtonList parentList;

	public delegate void OnSelectedEvent(MenuSelectable self);
	public event OnSelectedEvent OnSelected;

	private new void Awake()
	{
	    transition = Transition.None;
	    if(navigation.mode != Navigation.Mode.Explicit)
	    {
		navigation = new Navigation
		{
		    mode = Navigation.Mode.Explicit
		};
	    }
	}

	private new void Start()
	{
	    HookUpAudioPlayer();
	}

	protected void HookUpAudioPlayer()
	{
	    uiAudioPlayer = UIManager.instance.uiAudioPlayer;
	}

	public new void OnSelect(BaseEventData eventData)
	{
	    if (!interactable)
	    {
		return;
	    }
	    if(OnSelected != null)
	    {
		OnSelected(this);
	    }
	    if (leftCursor != null)
	    {
		leftCursor.ResetTrigger("hide");
		leftCursor.SetTrigger("show");
	    }
	    if (rightCursor != null)
	    {
		rightCursor.ResetTrigger("hide");
		rightCursor.SetTrigger("show");
	    }
	    if (selectHighlight != null)
	    {
		selectHighlight.ResetTrigger("hide");
		selectHighlight.SetTrigger("show");
	    }
	    if (!DontPlaySelectSound)
	    {
		try
		{
		    uiAudioPlayer.PlaySelect();
		    return;
		}
		catch (Exception ex)
		{
		    string name = base.name;
		    string str = " doesn't have a select sound specified. ";
		    Exception ex2 = ex;
		    Debug.LogError(name + str + ((ex2 != null) ? ex2.ToString() : null));
		    return;
		}
	    }
	    dontPlaySelectSound = false;
	}

	public new void OnDeselect(BaseEventData eventData)
	{
	    StartCoroutine(ValidateDeselect());
	}

	private IEnumerator ValidateDeselect()
	{
	    prevSelectedObject = EventSystem.current.currentSelectedGameObject;
	    yield return new WaitForEndOfFrame();
	    if (EventSystem.current.currentSelectedGameObject != null)
	    {
		if (leftCursor != null)
		{
		    leftCursor.ResetTrigger("show");
		    leftCursor.SetTrigger("hide");
		}
		if (rightCursor != null)
		{
		    rightCursor.ResetTrigger("show");
		    rightCursor.SetTrigger("hide");
		}
		if (selectHighlight != null)
		{
		    selectHighlight.ResetTrigger("show");
		    selectHighlight.SetTrigger("hide");
		}
		deselectWasForced = false;
	    }
	    else if (deselectWasForced)
	    {
		if (leftCursor != null)
		{
		    leftCursor.ResetTrigger("show");
		    leftCursor.SetTrigger("hide");
		}
		if (rightCursor != null)
		{
		    rightCursor.ResetTrigger("show");
		    rightCursor.SetTrigger("hide");
		}
		if (selectHighlight != null)
		{
		    selectHighlight.ResetTrigger("show");
		    selectHighlight.SetTrigger("hide");
		}
		deselectWasForced = false;
	    }
	    else
	    {
		deselectWasForced = false;
		dontPlaySelectSound = true;
		EventSystem.current.SetSelectedGameObject(prevSelectedObject);
	    }
	}

	public void OnCancel(BaseEventData eventData)
	{
	    if(cancelAction != CancelAction.DoNothing)
	    {
		ForceDeselect();
	    }
	    if (!parentList)
	    {
		parentList = GetComponentInParent<MenuButtonList>();
	    }
	    if (parentList)
	    {
		
	    }
	    if(cancelAction != CancelAction.DoNothing)
	    {
		if(cancelAction == CancelAction.GoToMainMenu)
		{
		    UIManager.instance.UIGoToMainMenu();
		}
	    }
	    if (cancelAction != CancelAction.DoNothing)
	    {
		PlayCancelSound();
	    }
	}

	protected void ForceDeselect()
	{
	    if (EventSystem.current.currentSelectedGameObject != null)
	    {
		deselectWasForced = true;
		EventSystem.current.SetSelectedGameObject(null);
	    }
	}

	protected void PlaySubmitSound()
	{
	    if (playSubmitSound)
	    {
		uiAudioPlayer.PlaySubmit();
	    }
	}

	protected void PlayCancelSound()
	{
	    uiAudioPlayer.PlayCancel();
	}

	protected void PlaySelectSound()
	{
	    uiAudioPlayer.PlaySelect();
	}
    }
}