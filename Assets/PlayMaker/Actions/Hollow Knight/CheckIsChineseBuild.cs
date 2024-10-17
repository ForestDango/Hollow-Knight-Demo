using System;

namespace HutongGames.PlayMaker.Actions
{
    public class CheckIsChineseBuild : FSMUtility.CheckFsmStateAction
    {
	public override bool IsTrue
	{
	    get
	    {
		return false;
	    }
	}
    }
}
