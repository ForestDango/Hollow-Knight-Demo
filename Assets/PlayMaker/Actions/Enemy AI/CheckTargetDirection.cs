using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Check whether target is left/right/up/down relative to object")]
    public class CheckTargetDirection : FsmStateAction
    {
	[RequiredField]
	public FsmOwnerDefault gameObject;
	[RequiredField]
	public FsmGameObject target;
	public FsmEvent aboveEvent;
	public FsmEvent belowEvent;
	public FsmEvent rightEvent;
	public FsmEvent leftEvent;

	[UIHint(UIHint.Variable)]
	public FsmBool aboveBool;
	[UIHint(UIHint.Variable)]
	public FsmBool belowBool;
	[UIHint(UIHint.Variable)]
	public FsmBool rightBool;
	[UIHint(UIHint.Variable)]
	public FsmBool leftBool;

	private FsmGameObject self;
	private FsmFloat x;
	private FsmFloat y;
	public bool everyFrame;

	public override void Reset()
	{
	    gameObject = null;
	    target = null;
	    everyFrame = false;
	}

	public override void OnEnter()
	{
	    self = Fsm.GetOwnerDefaultTarget(gameObject);
	    DoCheckDirection();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	public override void OnUpdate()
	{
	    DoCheckDirection();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	private void DoCheckDirection()
	{
	    float num = self.Value.transform.position.x;
	    float num2 = self.Value.transform.position.y;
	    float num3 = target.Value.transform.position.x;
	    float num4 = target.Value.transform.position.y;
	    if(num < num3)
	    {
		Fsm.Event(rightEvent);
		rightBool.Value = true;
	    }
	    else
	    {
		rightBool.Value = false;
	    }
	    if (num > num3)
	    {
		Fsm.Event(leftEvent);
		leftBool.Value = true;
	    }
	    else
	    {
		leftBool.Value = false;
	    }
	    if (num2 < num4)
	    {
		Fsm.Event(aboveEvent);
		aboveBool.Value = true;
	    }
	    else
	    {
		aboveBool.Value = false;
	    }
	    if(num2 > num4)
	    {
		Fsm.Event(belowEvent);
		belowBool.Value = true;
	    }
	    else
	    {
		belowBool.Value = false;
	    }
	}
    }

}
