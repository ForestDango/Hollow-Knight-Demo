using System;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessSetting : MonoBehaviour
{
    private GameSettings gs;
    private float valueMultiplier = 5f;
    public Slider slider;
    public MenuButton doneButton;
    public MenuButton backButton;
    public Text textUI;


    private void Start()
    {
	gs = GameManager.instance.gameSettings;
	UpdateValue();
    }

    public void UpdateTextValue(float value)
    {
	value = slider.value;
	textUI.text = (value * valueMultiplier).ToString() + "%";
    }

    public void UpdateValue()
    {
	textUI.text = (slider.value * valueMultiplier).ToString() + "%";
    }

    public void SetBrightness(float value)
    {
	value = slider.value;
	GameCameras.instance.brightnessEffect.SetBrightness(value / 20f);
	gs.brightnessAdjustment = value;
    }

    public void RefreshValueFromSettings()
    {
	if (gs == null)
	{
	    gs = GameManager.instance.gameSettings;
	}
	slider.value = gs.brightnessAdjustment;
	slider.onValueChanged.Invoke(slider.value);
	UpdateValue();
    }

    public void DoneMode()
    {
	doneButton.gameObject.SetActive(true);
	backButton.gameObject.SetActive(false);
	slider.navigation = new Navigation
	{
	    mode = Navigation.Mode.Explicit,
	    selectOnDown = doneButton,
	    selectOnUp = doneButton
	};
    }

    public void NormalMode()
    {
	doneButton.gameObject.SetActive(false);
	backButton.gameObject.SetActive(true);
	slider.navigation = new Navigation
	{
	    mode = Navigation.Mode.Explicit,
	    selectOnDown = backButton,
	    selectOnUp = backButton
	};
    }

}
