using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Adds a 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis. I added the ability to limit speed.")]
    public class AddForce2dAsAngle : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	[Tooltip("The GameObject to apply the force to.")]
	public FsmOwnerDefault gameObject;
	[UIHint(UIHint.Variable)]
	[Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollision2dInfo actions.")]
	public FsmVector2 atPosition;
	[RequiredField]
	public FsmFloat angle;
	[RequiredField]
	public FsmFloat speed;
	private float x;
	private float y;
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
	    angle = null;
	    speed = null;
	    maxSpeed = null;
	    maxSpeedX = null;
	    maxSpeedY = null;
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
	    x = speed.Value * Mathf.Cos(angle.Value * 0.017453292f);
	    y = speed.Value * Mathf.Sin(angle.Value * 0.017453292f);
	    if (!rb2d)
	    {
		return;
	    }
	    Vector2 force = new Vector2(x, y);
	    if (!atPosition.IsNone)
	    {
		rb2d.AddForceAtPosition(force, atPosition.Value);
	    }
	    else
	    {
		rb2d.AddForce(force);
	    }
	    if (!maxSpeedX.IsNone)
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
	    if (!maxSpeedY.IsNone)
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
	    if (!maxSpeed.IsNone)
	    {
		Vector2 vector = rb2d.velocity;
		vector = Vector2.ClampMagnitude(vector, maxSpeed.Value);
		rb2d.velocity = vector;
	    }
	}
    }
}
