using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Try to keep a certain distance from target.")]
    public class DistanceFly : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault gameObject;
	[UIHint(UIHint.Variable)]
	public FsmGameObject target;
	public FsmFloat distance;
	public FsmFloat speedMax;
	public FsmFloat acceleration;
	[Tooltip("If true, object tries to keep to a certain height relative to target")]
	public bool targetsHeight;
	public FsmFloat height;

	private float distanceAway;
	private FsmGameObject self;

	public override void Reset()
	{
	    gameObject = null;
	    target = null;
	    targetsHeight = false;
	    height = null;
	    acceleration = 0f;
	    speedMax = 0f;
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
	    self = Fsm.GetOwnerDefaultTarget(gameObject);
	    DoBuzz();
	}

	public override void OnFixedUpdate()
	{
	    DoBuzz();
	}

	private void DoBuzz()
	{
	    if (rb2d == null)
	    {
		return;
	    }
	    distanceAway = Mathf.Sqrt(Mathf.Pow(self.Value.transform.position.x - target.Value.transform.position.x, 2f) + Mathf.Pow(self.Value.transform.position.y - target.Value.transform.position.y, 2f));
	    Vector2 velocity = rb2d.velocity;
	    if (distanceAway > distance.Value)
	    {
		if (self.Value.transform.position.x < target.Value.transform.position.x)
		{
		    velocity.x += acceleration.Value;
		}
		else
		{
		    velocity.x -= acceleration.Value;
		}
		if (!targetsHeight)
		{
		    if (self.Value.transform.position.y < target.Value.transform.position.y)
		    {
			velocity.y += acceleration.Value;
		    }
		    else
		    {
			velocity.y -= acceleration.Value;
		    }
		}
	    }
	    else
	    {
		if (self.Value.transform.position.x < target.Value.transform.position.x)
		{
		    velocity.x -= acceleration.Value;
		}
		else
		{
		    velocity.x += acceleration.Value;
		}
		if (!targetsHeight)
		{
		    if (self.Value.transform.position.y < target.Value.transform.position.y)
		    {
			velocity.y -= acceleration.Value;
		    }
		    else
		    {
			velocity.y += acceleration.Value;
		    }
		}
	    }
	    if (targetsHeight)
	    {
		if (self.Value.transform.position.y < target.Value.transform.position.y + height.Value)
		{
		    velocity.y += acceleration.Value;
		}
		if (self.Value.transform.position.y > target.Value.transform.position.y + height.Value)
		{
		    velocity.y -= acceleration.Value;
		}
	    }
	    if (velocity.x > speedMax.Value)
	    {
		velocity.x = speedMax.Value;
	    }
	    if (velocity.x < -speedMax.Value)
	    {
		velocity.x = -speedMax.Value;
	    }
	    if (velocity.y > speedMax.Value)
	    {
		velocity.y = speedMax.Value;
	    }
	    if (velocity.y < -speedMax.Value)
	    {
		velocity.y = -speedMax.Value;
	    }
	    rb2d.velocity = velocity;
	}
    }
}
