using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuAudioSlider : MonoBehaviour
{
    public Slider slider;
    public Text textUI;
    public AudioMixer masterMixer;
    public AudioSettingType audioSetting;
    [SerializeField] private float value;

    private GameSettings gs;

    private void Start()
    {
	gs = GameManager.instance.gameSettings;
	UpdateValue();
    }

    private void UpdateValue()
    {
	textUI.text = slider.value.ToString();
    }

    public void RefreshValueFromSettings()
    {
	if(gs == null)
	{
	    gs = GameManager.instance.gameSettings;
	}
	if(audioSetting == AudioSettingType.MasterVolume)
	{
	    slider.value = gs.masterVolume;
	    UpdateValue();
	    return;
	}
	if (audioSetting == AudioSettingType.MusicVolume)
	{
	    slider.value = gs.musicVolume;
	    UpdateValue();
	    return;
	}
	if (audioSetting == AudioSettingType.SoundVolume)
	{
	    slider.value = gs.soundVolume;
	    UpdateValue();
	}
    }

    public void UpdateTextValue(float value)
    {
	textUI.text = slider.value.ToString();
    }

    public void SetMasterLevel(float masterLevel)
    {
	float num;
	if (masterLevel > 9f)
	{
	    num = 0f;
	}
	else
	{
	    num = GetVolumeLevel(masterLevel);
	}
	masterMixer.SetFloat("MasterVolume", num);
	gs.masterVolume = masterLevel;
    }
    public void SetMusicLevel(float musicLevel)
    {
	float num;
	if (musicLevel > 9f)
	{
	    num = 0f;
	}
	else
	{
	    num = GetVolumeLevel(musicLevel);
	}
	masterMixer.SetFloat("MusicVolume", num);
	gs.musicVolume = musicLevel;
    }

    public void SetSoundLevel(float soundLevel)
    {
	float num;
	if (soundLevel > 9f)
	{
	    num = 0f;
	}
	else
	{
	    num = GetVolumeLevel(soundLevel);
	}
	masterMixer.SetFloat("SFXVolume", num);
	gs.soundVolume = soundLevel;
    }
    private float GetVolumeLevel(float x)
    {
	return -1.02f * (x * x) + 17.5f * x - 76.6f;
    }

    public enum AudioSettingType
    {
	MasterVolume,
	MusicVolume,
	SoundVolume
    }
}
