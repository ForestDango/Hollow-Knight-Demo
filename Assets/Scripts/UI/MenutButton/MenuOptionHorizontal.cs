using System;
using Language;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class MenuOptionHorizontal : MenuSelectable, IMoveHandler, IEventSystemHandler, IPointerEnterHandler, IPointerClickHandler
    {
	[Header("Option List Settings")]
	public Text optionText;
	public string[] optionList;
	public int selectedOptionIndex;
	public MenuSetting menuSetting;
	[Header("Interaction")]
	public ApplyOnType applySettingOn;
	[Header("Localization")]
	public bool localizeText;
	public string sheetTitle;

	protected GameManager gm;

	private new void Awake()
	{
	    gm = GameManager.instance;
	}

	private new void OnEnable()
	{
	    gm.RefreshLanguageText += UpdateText;
	    UpdateText();
	}

	private new void OnDisable()
	{
	    gm.RefreshLanguageText -= UpdateText;
	}

	public new void OnMove(AxisEventData move)
	{
	    if (!interactable)
	    {
		return;
	    }
	    if (!MoveOption(move.moveDir))
	    {
		base.OnMove(move);
	    }
	}

	public void OnPointerClick(PointerEventData eventData)
	{
	    if (!interactable)
	    {
		return;
	    }
	    PointerClickCheckArrows(eventData);
	}

	protected bool MoveOption(MoveDirection dir)
	{
	    if (dir == MoveDirection.Right)
	    {
		IncrementOption();
	    }
	    else
	    {
		if (dir != MoveDirection.Left)
		{
		    return false;
		}
		DecrementOption();
	    }
	    if (uiAudioPlayer)
	    {
		uiAudioPlayer.PlaySlider();
	    }
	    return true;
	}

	protected void PointerClickCheckArrows(PointerEventData eventData)
	{
	    if (leftCursor && IsInside(leftCursor.gameObject, eventData))
	    {
		MoveOption(MoveDirection.Left);
		return;
	    }
	    if (rightCursor && IsInside(rightCursor.gameObject, eventData))
	    {
		MoveOption(MoveDirection.Right);
		return;
	    }
	    MoveOption(MoveDirection.Right);
	}

	protected virtual void UpdateText()
	{
	    if(optionList != null && optionText != null)
	    {
		try
		{
		    if (localizeText)
		    {
			optionText.text = Language.Language.Get(optionList[selectedOptionIndex].ToString(), sheetTitle);
		    }
		    else
		    {
			optionText.text = optionList[selectedOptionIndex].ToString();
		    }
		}
		catch (Exception ex)
		{
		    string[] array = new string[7];
		    array[0] = optionText.text;
		    array[1] = " : ";
		    int num = 2;
		    string[] array2 = optionList;
		    array[num] = ((array2 != null) ? array2.ToString() : null);
		    array[3] = " : ";
		    array[4] = selectedOptionIndex.ToString();
		    array[5] = " ";
		    int num2 = 6;
		    Exception ex2 = ex;
		    array[num2] = ((ex2 != null) ? ex2.ToString() : null);
		    Debug.LogError(string.Concat(array));
		}
		optionText.GetComponent<FixVerticalAlign>().AlignText();
	    }
	}

	protected void DecrementOption()
	{
	    if (selectedOptionIndex > 0)
	    {
		selectedOptionIndex--;
		if (applySettingOn == ApplyOnType.Scroll)
		{
		    UpdateSetting();
		}
		UpdateText();
		return;
	    }
	    if (selectedOptionIndex == 0)
	    {
		selectedOptionIndex = optionList.Length - 1;
		if (applySettingOn == ApplyOnType.Scroll)
		{
		    UpdateSetting();
		}
		UpdateText();
	    }
	}

	protected void IncrementOption()
	{
	    if (selectedOptionIndex >= 0 && selectedOptionIndex < optionList.Length - 1)
	    {
		selectedOptionIndex++;
		if (applySettingOn == ApplyOnType.Scroll)
		{
		    UpdateSetting();
		}
		UpdateText();
		return;
	    }
	    if (selectedOptionIndex == optionList.Length - 1)
	    {
		selectedOptionIndex = 0;
		if (applySettingOn == ApplyOnType.Scroll)
		{
		    UpdateSetting();
		}
		UpdateText();
	    }
	}

	private bool IsInside(GameObject obj, PointerEventData eventData)
	{
	    RectTransform component = obj.GetComponent<RectTransform>();
	    return component && RectTransformUtility.RectangleContainsScreenPoint(component, eventData.position, Camera.main);
	}

	protected void UpdateSetting()
	{
	    if (menuSetting)
	    {
		menuSetting.UpdateSetting(selectedOptionIndex);
	    }
	}
	public void SetOptionList(string[] optionList)
	{
	    this.optionList = optionList;
	}


	public enum ApplyOnType
	{
	    Scroll,
	    Submit
	}
    }
}