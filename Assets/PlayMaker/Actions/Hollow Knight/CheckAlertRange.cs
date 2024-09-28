using HutongGames.PlayMaker;
using System;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class CheckAlertRange : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    [ObjectType(typeof(AlertRange))]
    public FsmObject alertRange;

    [UIHint(UIHint.Variable)]
    public FsmBool storeResult;

    public bool everyFrame;

    public override void Reset()
    {
	alertRange = new FsmObject();
	storeResult = new FsmBool();
    }

    public override void OnEnter()
    {
	Apply();
	if (!everyFrame)
	{
	    Finish();
	}
    }

    public override void OnUpdate()
    {
	Apply();
    }

    private void Apply()
    {
	AlertRange alertRange = this.alertRange.Value as AlertRange;
	if(alertRange != null)
	{
	    storeResult.Value = alertRange.IsHeroInRange;
	    return;
	}
	storeResult.Value = false;
    }
}
