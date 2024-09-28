using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Adds a 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis. I added the ability to limit speed.")]
    public class AddForce2dV2 : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	[Tooltip("The GameObject to apply the force to.")]
	public FsmOwnerDefault gameObject;

	[UIHint(UIHint.Variable)]
	[Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollision2dInfo actions.")]
	public FsmVector2 atPosition;

	[UIHint(UIHint.Variable)]
	[Tooltip("A Vector2 force to add. Optionally override any axis with the X, Y parameters.")]
	public FsmVector2 vector;

	[Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
	public FsmFloat x;

	[Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
	public FsmFloat y;

	[Tooltip("A Vector3 force to add. z is ignored")]
	public FsmVector3 vector3;

	public FsmFloat maxSpeed;
	public FsmFloat maxSpeedX;
	public FsmFloat maxSpeedY;

	[Tooltip("Repeat every frame while the state is active.")]
	public bool everyFrame;

	public override void Reset()
	{
	    gameObject = null;
	    atPosition = new FsmVector2
	    {
		UseVariable = true
	    };
	    vector = null;
	    vector3 = new FsmVector3
	    {
		UseVariable = true
	    };
	    x = new FsmFloat
	    {
		UseVariable = true
	    };
	    y = new FsmFloat
	    {
		UseVariable = true
	    };
	    maxSpeed = null;
	    maxSpeedX = null;
	    maxSpeedY = null;
	    everyFrame = false;
	}
	public override void OnPreprocess()
	{
	    base.OnPreprocess();
	    Fsm.HandleFixedUpdate = true;
	}
	public override void OnEnter()
	{
	    CacheRigidBody2d(Fsm.GetOwnerDefaultTarget(gameObject));
	    DoAddForce();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	public override void OnFixedUpdate()
	{
	    DoAddForce();
	}

	private void DoAddForce()
	{
	    if (!rb2d)
	    {
		return;
	    }
	    Vector2 force = vector.IsNone ? new Vector2(x.Value, y.Value) : vector.Value;
	    if (!vector3.IsNone)
	    {
		force.x = vector3.Value.x;
		force.y = vector3.Value.y;
	    }
	    if (!x.IsNone)
	    {
		force.x = x.Value;
	    }
	    if (!y.IsNone)
	    {
		force.y = y.Value;
	    }
	    if (!atPosition.IsNone)
	    {
		rb2d.AddForceAtPosition(force, atPosition.Value);
	    }
	    else
	    {
		rb2d.AddForce(force);
	    }
	    if (maxSpeedX != null)
	    {
		Vector2 velocity = rb2d.velocity;
		if (velocity.x > maxSpeedX.Value)
		{
		    velocity = new Vector2(maxSpeedX.Value, velocity.y);
		}
		if (velocity.x < -maxSpeedX.Value)
		{
		    velocity = new Vector2(-maxSpeedX.Value, velocity.y);
		}
		rb2d.velocity = velocity;
	    }
	    if (maxSpeedY != null)
	    {
		Vector2 velocity2 = rb2d.velocity;
		if (velocity2.y > maxSpeedY.Value)
		{
		    velocity2 = new Vector2(velocity2.x, maxSpeedY.Value);
		}
		if (velocity2.y < -maxSpeedY.Value)
		{
		    velocity2 = new Vector2(velocity2.x, -maxSpeedY.Value);
		}
		rb2d.velocity = velocity2;
	    }
	    if (maxSpeed != null)
	    {
		Vector2 velocity3 = rb2d.velocity;
		velocity3 = Vector2.ClampMagnitude(velocity3, maxSpeed.Value);
		rb2d.velocity = velocity3;
	    }
	}


    }

}
