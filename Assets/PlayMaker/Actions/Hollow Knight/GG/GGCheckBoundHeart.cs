using System;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckBoundHeart : FSMUtility.CheckFsmStateAction
{
    public FsmInt healthNumber;
    public CheckSource checkSource;

    public override void Reset()
    {
	healthNumber = null;
	checkSource = CheckSource.Regular;
	base.Reset();
    }

    public override bool IsTrue
    {
	get
	{
	    int num = -1;
	    CheckSource checkSource = this.checkSource;
	    if (checkSource != CheckSource.Regular)
	    {
		if (checkSource == CheckSource.Joni)
		{
		    num = (int)(healthNumber.Value * 0.71428573f) + 1;
		}
	    }
	    else
	    {
		num = healthNumber.Value;
	    }
	    //TODO:
	    return false;
	}
    }

    public enum CheckSource
    {
	Regular,
	Joni
    }
}
