using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class SetInvincible : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    public FsmOwnerDefault target;
    public FsmBool Invincible;
    public FsmInt InvincibleFromDirection;

    public override void Reset()
    {
	target = new FsmOwnerDefault();
	Invincible = null;
	InvincibleFromDirection = null;
    }

    public override void OnEnter()
    {
	GameObject safe = target.GetSafe(this);
	if (safe != null)
	{
	    HealthManager component = safe.GetComponent<HealthManager>();
	    if (component != null)
	    {
		if (!Invincible.IsNone)
		{
		    component.IsInvincible = Invincible.Value;
		}
		if (!InvincibleFromDirection.IsNone)
		{
		    component.InvincibleFromDirection = InvincibleFromDirection.Value;
		}
	    }
	}
	Finish();
    }

}
