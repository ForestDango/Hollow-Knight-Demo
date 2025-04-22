using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.Animator)]
    public class SetAnimator : FsmStateAction
    {
	public FsmOwnerDefault target;
	public FsmBool active;

	public override void Reset()
	{
	    target = null;
	    active = null;
	}

	public override void OnEnter()
	{
	    GameObject safe = target.GetSafe(this);
	    if (safe)
	    {
		Animator component = safe.GetComponent<Animator>();
		if (component)
		{
		    component.enabled = active.Value;
		}
	    }
	    Finish();
	}
    }
}
