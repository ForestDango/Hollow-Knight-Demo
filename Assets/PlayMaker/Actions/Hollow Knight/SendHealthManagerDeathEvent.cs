using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class SendHealthManagerDeathEvent : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    public FsmOwnerDefault target;

    public override void Reset()
    {
	target = null;
    }

    public override void OnEnter()
    {
	GameObject gameObject = (target.OwnerOption == OwnerDefaultOption.UseOwner) ? Owner : target.GameObject.Value;
	if(gameObject != null)
	{
	    HealthManager component = gameObject.GetComponent<HealthManager>();
	    if(component != null)
	    {
		component.SendDeathEvent();
	    }
	}
	Finish();
    }
}
