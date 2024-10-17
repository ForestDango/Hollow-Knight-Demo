using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Set sprite renderer to active or inactive. Can only be one sprite renderer on object.")]
    public class SetSpriteRenderer : FsmStateAction
    {
	[RequiredField]
	public FsmOwnerDefault gameObject;
	public FsmBool active;

	public override void Reset()
	{
	    gameObject = null;
	    active = null;
	}

	public override void OnEnter()
	{
	    if(gameObject != null)
	    {
		GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
		if(ownerDefaultTarget != null)
		{
		    SpriteRenderer component = ownerDefaultTarget.GetComponent<SpriteRenderer>();
		    if(component != null)
		    {
			component.enabled = active.Value;
		    }
		}
	    }
	    Finish();
	}
    }
}
