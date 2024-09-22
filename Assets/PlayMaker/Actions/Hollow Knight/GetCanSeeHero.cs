using HutongGames.PlayMaker;
using System;

[ActionCategory("Hollow Knight")]
public class GetCanSeeHero : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    [ObjectType(typeof(LineOfSightDetector))]
    public FsmObject lineOfSightDetector;

    [UIHint(UIHint.Variable)]
    public FsmBool storeResult;

    public bool everyFrame;

    public override void Reset()
    {
	lineOfSightDetector = new FsmObject();
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
	LineOfSightDetector lineOfSightDetector = this.lineOfSightDetector.Value as LineOfSightDetector;
	if (lineOfSightDetector != null)
	{
	    storeResult.Value = lineOfSightDetector.CanSeeHero;
	    return;
	}
	storeResult.Value = false;
    }
}
