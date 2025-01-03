using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Set the dimensions of the first BoxCollider 2D on object. Uses vector2s")]
    public class SetBoxCollider2DSizeVector : FsmStateAction
    {
	[RequiredField]
	public FsmOwnerDefault gameObject1;
	public FsmVector2 size;
	public FsmVector2 offset;

	public override void Reset()
	{
	    size = new FsmVector2
	    {
		UseVariable = true
	    };
	    offset = new FsmVector2
	    {
		UseVariable = true
	    };
	}

	public override void OnEnter()
	{
	    SetDimensions();
	    Finish();
	}

	private void SetDimensions()
	{
	    BoxCollider2D component = Fsm.GetOwnerDefaultTarget(gameObject1).GetComponent<BoxCollider2D>();
	    if (!size.IsNone)
	    {
		component.size = size.Value;
	    }
	    if (!offset.IsNone)
	    {
		component.offset = offset.Value;
	    }
	}
    }

}
