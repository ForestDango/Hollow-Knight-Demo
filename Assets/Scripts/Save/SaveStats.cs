using System;
using GlobalEnums;

[Serializable]
public class SaveStats
{
    private PlayTime playTimeStruct;
    public int maxHealth { get; private set; }
    public int geo { get; private set; }
    public float playTime { get; private set; }
    public int maxMPReserve { get; private set; }
    public int permadeathMode { get; private set; }
    public bool bossRushMode { get; private set; }
    public float completionPercentage { get; private set; }
    public bool unlockedCompletionRate { get; private set; }
    public SaveStats(int maxHealth, int geo,float playTime,int maxMPReserve,int permadeathMode, bool bossRushMode, float completionPercentage, bool unlockedCompletionRate)
    {
	this.maxHealth = maxHealth;
	this.geo = geo;
	this.playTime = playTime;
	this.maxMPReserve = maxMPReserve;
	this.permadeathMode = permadeathMode;
	this.bossRushMode = bossRushMode;
	this.completionPercentage = completionPercentage;
	this.unlockedCompletionRate = unlockedCompletionRate;
    }
    public string GetPlaytimeHHMM()
    {
	if (playTimeStruct.HasHours)
	{
	    return string.Format("{0:0}h {1:00}m", (int)playTimeStruct.Hours, (int)playTimeStruct.Minutes);
	}
	return string.Format("{0:0}m", (int)playTimeStruct.Minutes);
    }

    public string GetPlaytimeHHMMSS()
    {
	if (!playTimeStruct.HasHours)
	{
	    return string.Format("{0:0}m {1:00}s", (int)playTimeStruct.Minutes, (int)playTimeStruct.Seconds);
	}
	if (!playTimeStruct.HasMinutes)
	{
	    return string.Format("{0:0}s", (int)playTimeStruct.Seconds);
	}
	return string.Format("{0:0}h {1:00}m {2:00}s", (int)playTimeStruct.Hours, (int)playTimeStruct.Minutes, (int)playTimeStruct.Seconds);
    }

    public string GetCompletionPercentage()
    {
	return completionPercentage.ToString() + "%";
    }

    public int GetMPSlotsVisible()
    {
	return (int)((float)maxMPReserve / 33f);
    }
}
