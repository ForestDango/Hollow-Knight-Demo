using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Sets the Angular Velocity of a Game Object. NOTE: Game object must have a rigidbody 2D.")]
    public class SetAngularVelocity2d : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;

	public FsmFloat angularVelocity;

	public override void Reset()
	{
	    angularVelocity = null;
	    everyFrame = false;
	}

	public bool everyFrame;
	public override void OnEnter()
	{
	    CacheRigidBody2d(Fsm.GetOwnerDefaultTarget(gameObject));
	    DoSetVelocity();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	public override void Awake()
	{
	    Fsm.HandleFixedUpdate = true;
	}

	public override void OnPreprocess()
	{
	    Fsm.HandleFixedUpdate = true;
	}

	public override void OnFixedUpdate()
	{
	    DoSetVelocity();
	}

	private void DoSetVelocity()
	{
	    if (rb2d == null)
	    {
		return;
	    }
	    if (!angularVelocity.IsNone)
	    {
		rb2d.angularVelocity = angularVelocity.Value;
	    }
	}
    }

}
