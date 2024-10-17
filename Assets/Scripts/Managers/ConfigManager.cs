using System;
using System.IO;
using UnityEngine;

public static class ConfigManager
{
    private static bool _isInit;
    public static float CameraShakeMultiplier { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
	_isInit = true;
	CameraShakeMultiplier = 1f;
    }
}
