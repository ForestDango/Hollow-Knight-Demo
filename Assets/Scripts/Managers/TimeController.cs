using System;
using UnityEngine;

public static class TimeController
{
    private static float slowMotionTimeScale = 1f;
    private static float pauseTimeScale = 1f;
    private static float platformBackgroundTimeScale = 1f;
    private static float genericTimeScale = 1f;
    public static float GenericTimeScale
    {
	get
	{
	    return genericTimeScale;
	}
	set
	{
	    SetTimeScaleFactor(ref genericTimeScale, value);
	}
    }

    private static void SetTimeScaleFactor(ref float field, float val)
    {
	if (field != val)
	{
	    field = val;
	    float num = slowMotionTimeScale * pauseTimeScale * platformBackgroundTimeScale * genericTimeScale;
	    if (num < 0.01f)
	    {
		num = 0f;
	    }
	    Time.timeScale = num;
	}
    }
}
