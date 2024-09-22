using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Sets the 2d Velocity of a Game Object, using an angle and a speed value. For the angle, 0 is to the right and the degrees increase clockwise.")]
    public class SetVelocityAsAngle : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;

	[RequiredField]
	public FsmFloat angle;

	[RequiredField]
	public FsmFloat speed;

	private FsmFloat x;
	private FsmFloat y;
	public bool everyFrame;

	public override void Reset()
	{
	    gameObject = null;
	    angle = new FsmFloat
	    {
		UseVariable = true
	    };
	    speed = new FsmFloat
	    {
		UseVariable = true
	    };
	    everyFrame = false;
	}

	public override void Awake()
	{
	    Fsm.HandleFixedUpdate = true;
	}

	public override void OnPreprocess()
	{
	    Fsm.HandleFixedUpdate = true;
	}

	public override void OnEnter()
	{
	    CacheRigidBody2d(Fsm.GetOwnerDefaultTarget(gameObject));
	    DoSetVelocity();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	public override void OnFixedUpdate()
	{
	    DoSetVelocity();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	private void DoSetVelocity()
	{
	    if (rb2d == null)
		return;
	    x = speed.Value * Mathf.Cos(angle.Value * 0.017453292f); //将角度转化为速度
	    y = speed.Value * Mathf.Sin(angle.Value * 0.017453292f);
	    Vector2 velocity;
	    velocity.x = x.Value;
	    velocity.y = y.Value;
	    rb2d.velocity = velocity;
	}
    }

}
