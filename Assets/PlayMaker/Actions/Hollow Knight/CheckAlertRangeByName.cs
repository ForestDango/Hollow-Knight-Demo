using System;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class CheckAlertRangeByName : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    public FsmBool storeResult;
    public string childName;
    public bool everyFrame;
    private AlertRange source;

    public override void Reset()
    {
	storeResult = new FsmBool();
    }

    public override void OnEnter()
    {
	source = AlertRange.Find(Owner, childName);

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
	if (source != null)
	{
	    storeResult.Value = source.IsHeroInRange;
	    return;
	}
	storeResult.Value = false;
    }
}
