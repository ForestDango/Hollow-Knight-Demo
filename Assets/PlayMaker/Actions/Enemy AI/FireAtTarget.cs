using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Travel in a straight line towards target at set speed.")]
    public class FireAtTarget : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;
	[RequiredField]
	public FsmGameObject target;
	[RequiredField]
	public FsmFloat speed;
	public FsmVector3 position;
	public FsmFloat spread;

	private FsmGameObject self;
	private FsmFloat x;
	private FsmFloat y;
	public bool everyFrame;

	public override void Reset()
	{
	    gameObject = null;
	    target = null;
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
	    self = Fsm.GetOwnerDefaultTarget(gameObject);
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
	    {
		return;
	    }
	    float num = target.Value.transform.position.y + position.Value.y - self.Value.transform.position.y;
	    float num2 = target.Value.transform.position.x + position.Value.x - self.Value.transform.position.x;
	    float num3 = Mathf.Atan2(num, num2) * 57.295776f;
	    if (!spread.IsNone)
	    {
		num3 += Random.Range(-spread.Value, spread.Value);
	    }
	    x = speed.Value * Mathf.Cos(num3 * 0.017453292f);
	    y = speed.Value * Mathf.Sin(num3 * 0.017453292f);
	    Debug.LogFormat("num3 = " + num3);
	    Debug.LogFormat("x = " + x);
	    Debug.LogFormat("y = " + y);
	    Vector2 velocity;
	    velocity.x = x.Value;
	    velocity.y = y.Value;
	    rb2d.velocity = velocity;
	    Debug.LogFormat("rb2d.velocity.y = " + rb2d.velocity.y);
	}
    }

}
