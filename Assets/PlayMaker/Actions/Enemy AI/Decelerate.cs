using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Decelerate X and Y until 0 reached.")]
    public class Decelerate : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;

	public FsmFloat deceleration;

	public override void Reset()
	{
	    gameObject = null;
	    deceleration = 0f;
	}

	public override void OnPreprocess()
	{
	    Fsm.HandleFixedUpdate = true;
	}

	public override void Awake()
	{
	    Fsm.HandleFixedUpdate = true;
	}

	public override void OnEnter()
	{
	    CacheRigidBody2d(Fsm.GetOwnerDefaultTarget(gameObject));
	    DecelerateSelf();
	}

	public override void OnFixedUpdate()
	{
	    DecelerateSelf();
	}

	private void DecelerateSelf()
	{
	    if (rb2d == null)
		return;
	    Vector2 velocity = rb2d.velocity;
	    if (velocity.x < 0f)
	    {
		velocity.x += this.deceleration.Value;
		if (velocity.x > 0f)
		{
		    velocity.x = 0f;
		}
	    }
	    else if (velocity.x > 0f)
	    {
		velocity.x -= this.deceleration.Value;
		if (velocity.x < 0f)
		{
		    velocity.x = 0f;
		}
	    }
	    if (velocity.y < 0f)
	    {
		velocity.y += this.deceleration.Value;
		if (velocity.y > 0f)
		{
		    velocity.y = 0f;
		}
	    }
	    else if (velocity.y > 0f)
	    {
		velocity.y -= this.deceleration.Value;
		if (velocity.y < 0f)
		{
		    velocity.y = 0f;
		}
	    }
	    rb2d.velocity = velocity;
	}
    }

}
