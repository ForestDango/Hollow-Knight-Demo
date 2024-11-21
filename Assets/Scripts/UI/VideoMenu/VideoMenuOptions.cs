using System;
using HKMenu;
using UnityEngine;
using UnityEngine.UI;

public class VideoMenuOptions : MonoBehaviour, IMenuOptionLayout
{
    public MenuScreen videoMenuScreen;
    public MenuSelectable vsyncOption;
    public MenuSelectable frameCapOption;
    public MenuSelectable screenScaleOption;
    public MenuSelectable resetButton;
    public MenuSelectable applyButton;

    public void ConfigureNavigation()
    {
	frameCapOption.transform.parent.gameObject.SetActive(true);
	Navigation navigation = vsyncOption.navigation;
	navigation.selectOnDown = frameCapOption;
	vsyncOption.navigation = navigation;
	Navigation navigation2 = screenScaleOption.navigation;
	navigation2.selectOnUp = frameCapOption;
	screenScaleOption.navigation = navigation2;
    }
}
