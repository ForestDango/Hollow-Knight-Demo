using System;
using GlobalEnums;
using InControl;
using UnityEngine;

[Serializable]
public class GameSettings
{
    [Header("Audio Settings")]
    public float masterVolume;
    public float musicVolume;
    public float soundVolume;

    [Header("Video Settings")]
    public int fullScreen;
    public int vSync;
    public ShaderQualities shaderQuality;

    public float brightnessAdjustment = 20f;
    public void ResetAudioSettings()
    {
	masterVolume = 10f;
	musicVolume = 10f;
	soundVolume = 10f;
    }

    public void SaveAudioSettings()
    {
	//TODO:Save Audio Settings
    }

    public void ResetVideoSettings()
    {
	//TODO:Reset Video Settings
    }
}

