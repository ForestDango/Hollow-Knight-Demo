using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class PreventInvincibleEffect : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    public FsmOwnerDefault target;
    public FsmBool preventEffect;

    public override void Reset()
    {
	target = new FsmOwnerDefault();
	preventEffect = new FsmBool();
    }

    public override void OnEnter()
    {
	GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
	if (gameObject != null)
	{
	    HealthManager component = gameObject.GetComponent<HealthManager>();
	    if (component != null)
	    {
		component.SetPreventInvincibleEffect(this.preventEffect.Value);
	    }
	}
	base.Finish();
    }

}
    
