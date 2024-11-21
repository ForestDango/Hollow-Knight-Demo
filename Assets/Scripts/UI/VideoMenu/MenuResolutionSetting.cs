using System;
using System.Collections;
using HKMenu;
using UnityEngine.EventSystems;
namespace UnityEngine.UI
{
    public class MenuResolutionSetting : MenuOptionHorizontal, ISubmitHandler, IEventSystemHandler, IMoveHandler, IPointerClickHandler, IMenuOptionListSetting
    {
	private Resolution[] availableResolutions;
	private Resolution previousRes; private bool foundResolutionInList;
	private int currentlyActiveResIndex = -1;
	private int previousResIndex;
	[Header("Resolution Setting Specific")]
	public CanvasGroup applyButton;

	public Resolution currentRes { get; private set; }
	public Resolution screenRes { get; private set; }

	public new void OnEnable()
	{
	    RefreshControls();
	    UpdateApplyButton();
	}
	public void OnSubmit(BaseEventData eventData)
	{
	    if (currentlyActiveResIndex != selectedOptionIndex)
	    {
		ForceDeselect();
		uiAudioPlayer.PlaySubmit();
		ApplySettings();
	    }
	}

	public new void OnMove(AxisEventData move)
	{
	    if (MoveOption(move.moveDir))
	    {
		UpdateApplyButton();
		return;
	    }
	    base.OnMove(move);
	}

	public new void OnPointerClick(PointerEventData eventData)
	{
	    base.OnPointerClick(eventData);
	    if (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Right)
	    {
		UpdateApplyButton();
	    }
	}

	/// <summary>
	/// 更新当前使用分辨率的下标
	/// </summary>
	public void UpdateApplyButton()
	{
	    if (currentlyActiveResIndex == selectedOptionIndex)
	    {
		HideApplyButton();
		return;
	    }
	    ShowApplyButton();
	}

	/// <summary>
	/// 给Apply的Button按钮调用
	/// </summary>
	public void ApplySettings()
	{
	    if (selectedOptionIndex >= 0)
	    {
		previousRes = this.currentRes;
		previousResIndex = currentlyActiveResIndex;
		Resolution currentRes = availableResolutions[selectedOptionIndex];
		Screen.SetResolution(currentRes.width, currentRes.height, Screen.fullScreen, currentRes.refreshRate);
		this.currentRes = currentRes;
		currentlyActiveResIndex = selectedOptionIndex;
		HideApplyButton();
		UIManager.instance.UIShowResolutionPrompt(true);
	    }
	}

	/// <summary>
	/// 重置回初始分辨率，给Default按钮调用
	/// </summary>
	public void ResetToDefaultResolution()
	{
	    Screen.SetResolution(1920, 1080, Screen.fullScreen);
	    currentRes = Screen.currentResolution;
	    StartCoroutine(RefreshOnNextFrame());
	}

	public void RefreshControls()
	{
	    RefreshAvailableResolutions();
	    RefreshCurrentIndex();
	    PushUpdateOptionList();
	    UpdateText();
	}

	/// <summary>
	/// 回到上一个分辨率选项
	/// </summary>
	public void RollbackResolution()
	{
	    Screen.SetResolution(previousRes.width, previousRes.height, Screen.fullScreen);
	    currentRes = Screen.currentResolution;
	    StartCoroutine(RefreshOnNextFrame());
	}

	/// <summary>
	/// 刷新当前的分辨率下标index
	/// </summary>
	public void RefreshCurrentIndex()
	{
	    foundResolutionInList = false;
	    for (int i = 0; i < availableResolutions.Length; i++)
	    {
		if (currentRes.Equals(availableResolutions[i]))
		{
		    selectedOptionIndex = i;
		    currentlyActiveResIndex = i;
		    foundResolutionInList = true;
		    break;
		}
	    }
	    if (!foundResolutionInList)
	    {
		Resolution[] array = new Resolution[availableResolutions.Length + 1];
		array[0] = currentRes;
		for (int j = 0; j < availableResolutions.Length; j++)
		{
		    array[j + 1] = availableResolutions[j];
		}
		availableResolutions = array;
		selectedOptionIndex = 0;
		currentlyActiveResIndex = 0;
	    }
	}

	/// <summary>
	/// 更新OptionList
	/// </summary>
	public void PushUpdateOptionList()
	{
	    string[] array = new string[availableResolutions.Length];
	    for (int i = 0; i < availableResolutions.Length; i++)
	    {
		array[i] = availableResolutions[i].ToString();
	    }
	    SetOptionList(array);
	}

	/// <summary>
	/// 隐藏apply应用按钮
	/// </summary>
	private void HideApplyButton()
	{
	    applyButton.alpha = 0f;
	    applyButton.interactable = false;
	    applyButton.blocksRaycasts = false;
	}

	/// <summary>
	/// 显示apply应用按钮
	/// </summary>
	private void ShowApplyButton()
	{
	    applyButton.alpha = 1f;
	    applyButton.interactable = true;
	    applyButton.blocksRaycasts = true;
	}

	/// <summary>
	/// 重新加载可以选用的分辨率
	/// </summary>
	private void RefreshAvailableResolutions()
	{
	    screenRes = Screen.currentResolution;
	    if (!Screen.fullScreen)
	    {
		currentRes = new Resolution
		{
		    width = Screen.width,
		    height = Screen.height,
		    refreshRate = screenRes.refreshRate
		};
	    }
	    else
	    {
		currentRes = screenRes;
	    }
	    availableResolutions = Screen.resolutions;
	}

	/// <summary>
	/// 下一帧刷新可用分辨率
	/// </summary>
	/// <returns></returns>
	private IEnumerator RefreshOnNextFrame()
	{
	    yield return null;
	    RefreshAvailableResolutions();
	    RefreshCurrentIndex();
	    PushUpdateOptionList();
	    UpdateApplyButton();
	    UpdateText();
	    yield break;
	}

    }
}