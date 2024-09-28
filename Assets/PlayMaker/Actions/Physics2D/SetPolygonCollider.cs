using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Set PolygonCollider to active or inactive. Can only be one collider on object.")]
    public class SetPolygonCollider : FsmStateAction
    {
	[RequiredField]
	[Tooltip("The particle emitting GameObject")]
	public FsmOwnerDefault gameObject;

	public FsmBool active;

	public override void OnEnter()
	{
	    if(gameObject != null)
	    {
		PolygonCollider2D component = Fsm.GetOwnerDefaultTarget(gameObject).GetComponent<PolygonCollider2D>();
		if (component)
		{
		    component.enabled = active.Value;
		}
	    }
	}

	public override void Reset()
	{
	    gameObject = null;
	    active = false;
	}
    }
}
