using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class GetHP : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    public FsmOwnerDefault target;
    [UIHint(UIHint.Variable)]
    public FsmInt storeValue;

    public bool everyFrame;

    public override void Reset()
    {
	target = new FsmOwnerDefault();
	storeValue = new FsmInt
	{
	    UseVariable = true
	};
	everyFrame = false;
    }

    public override void OnEnter()
    {
	DoGetHP();
	if(!everyFrame)
	{
	    base.Finish();
	}
    }

    public override void OnUpdate()
    {
	DoGetHP();
    }

    private void DoGetHP()
    {
	GameObject safe = target.GetSafe(this);
	if (safe != null)
	{
	    HealthManager component = safe.GetComponent<HealthManager>();
	    if (component != null && !storeValue.IsNone)
	    {
		storeValue.Value = component.hp;
	    }
	}
    }
}
