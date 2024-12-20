using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class InstaDeath : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    public FsmOwnerDefault target;
    public FsmFloat direction;

    public override void Reset()
    {
	target = new FsmOwnerDefault();
    }

    public override void OnEnter()
    {
	GameObject safe = target.GetSafe(this);
	if (safe != null)
	{
	    HealthManager component = safe.GetComponent<HealthManager>();
	    if (component != null && !component.isDead)
	    {
		component.Die(new float?(Mathf.Sign(direction.Value)), AttackTypes.Generic, false);
	    }
	}
	Finish();
    }

}
