using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Accelerates objects velocity, and clamps top speed")]
    public class AccelerateVelocity : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;

	public FsmFloat xAccel;
	public FsmFloat yAccel;
	public FsmFloat xMaxSpeed;
	public FsmFloat yMaxSpeed;

	public override void Reset()
	{
	    gameObject = null;
	    xAccel = new FsmFloat
	    {
		UseVariable = true
	    };
	    yAccel = new FsmFloat
	    {
		UseVariable = true
	    };
	    xMaxSpeed = new FsmFloat
	    {
		UseVariable = true
	    };
	    yMaxSpeed = new FsmFloat
	    {
		UseVariable = true
	    };
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
	    Vector2 velocity = rb2d.velocity;
	    if (!xAccel.IsNone)
	    {
		float num = velocity.x + xAccel.Value;
		num = Mathf.Clamp(num, -xMaxSpeed.Value, xMaxSpeed.Value);
		velocity = new Vector2(num, velocity.y);
	    }
	    if (!yAccel.IsNone)
	    {
		float num2 = velocity.y + yAccel.Value;
		num2 = Mathf.Clamp(num2, -yMaxSpeed.Value, yMaxSpeed.Value);
		velocity = new Vector2(velocity.x, num2);
	    }
	    rb2d.velocity = velocity;
	}

    }

}
