using System;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckBoundSoul : FsmStateAction
{
    public FsmEvent boundEvent;
    public FsmEvent unboundEvent;

    public override void Reset()
    {
	boundEvent = null;
	unboundEvent = null;
    }

    public override void OnEnter()
    {
	//TODO:
	Fsm.Event(unboundEvent);
	base.Finish();
    }

}
