using System;

public struct PlayTime 
{
    public float RawTime;
    private TimeSpan time
    {
	get
	{
	    return TimeSpan.FromSeconds(RawTime);
	}
    }
    public float Hours
    {
	get
	{
	    return (float)Math.Floor(time.TotalHours);
	}
    }
    public float Minutes
    {
	get
	{
	    return time.Minutes;
	}
    }
    public float Seconds
    {
	get
	{
	    return time.Seconds;
	}
    }
    public bool HasHours
    {
	get
	{
	    return time.TotalHours >= 1.0;
	}
    }
    public bool HasMinutes
    {
	get
	{
	    return time.TotalMinutes >= 1.0;
	}
    }
}
