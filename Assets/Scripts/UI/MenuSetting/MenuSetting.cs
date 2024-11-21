using System;
using UnityEngine;
using UnityEngine.UI;
public class MenuSetting : MonoBehaviour
{
    public MenuSettingType settingType;
    public MenuOptionHorizontal optionList;
    private GameManager gm;
    private GameSettings gs;
    private bool verboseMode;

    private void Start()
    {
	gm = GameManager.instance;
	gs = gm.gameSettings;
    }

    public void RefreshValueFromGameSettings(bool alsoApplySetting = false)
    {
	//TODO:RefreshValueFromGameSettings
    }

    public void UpdateSetting(int settingIndex)
    {
	if (verboseMode)
	{
	    Debug.Log(settingType.ToString() + " " + settingIndex.ToString());
	}
	if (gs == null)
	{
	    gs = GameManager.instance.gameSettings;
	}
	//TODO:UpdateSetting
    }



    public enum MenuSettingType
    {
	Resolution = 10,
	FullScreen,
	VSync,
	MonitorSelect = 14,
	FrameCap,
	ParticleLevel,
	ShaderQuality,
	GameLanguage = 33,
	GameBackerCredits,
	NativeAchievements,
	NativeInput,
	ControllerRumble
    }
}
