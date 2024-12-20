using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory("Enemy AI")]
    [Tooltip("Flies and keeps a certain distance from target, with smoother movement")]
    public class DistanceFlySmooth : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault gameObject;
	[UIHint(UIHint.Variable)]
	public FsmGameObject target;
	public FsmFloat distance;
	public FsmFloat speedMax;
	public FsmFloat accelerationForce;
	public FsmFloat targetRadius;
	public FsmFloat deceleration;
	public FsmVector3 offset;

	private float distanceAway;
	private FsmGameObject self;

	public override void Reset()
	{
	    gameObject = null;
	    target = null;
	    accelerationForce = 0f;
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
	    DoChase();
	}

	public override void OnFixedUpdate()
	{
	    DoChase();
	}

	private void DoChase()
	{
	    if (rb2d == null)
	    {
		return;
	    }
	    distanceAway = Mathf.Sqrt(Mathf.Pow(self.Value.transform.position.x - (target.Value.transform.position.x + offset.Value.x), 2f) + Mathf.Pow(self.Value.transform.position.y - (target.Value.transform.position.y + offset.Value.y), 2f));
	    Vector2 vector = rb2d.velocity;
	    if (distanceAway <= distance.Value - targetRadius.Value || distanceAway >= distance.Value + targetRadius.Value)
	    {
		Vector2 vector2 = new Vector2(target.Value.transform.position.x + offset.Value.x - self.Value.transform.position.x, target.Value.transform.position.y + offset.Value.y - self.Value.transform.position.y);
		vector2 = Vector2.ClampMagnitude(vector2, 1f);
		vector2 = new Vector2(vector2.x * accelerationForce.Value, vector2.y * accelerationForce.Value);
		if (distanceAway < distance.Value)
		{
		    vector2 = new Vector2(-vector2.x, -vector2.y);
		}
		rb2d.AddForce(vector2);
		vector = Vector2.ClampMagnitude(vector, speedMax.Value);
		rb2d.velocity = vector;
		return;
	    }
	    vector = rb2d.velocity;
	    if (vector.x < 0f)
	    {
		vector.x *= deceleration.Value;
		if (vector.x > 0f)
		{
		    vector.x = 0f;
		}
	    }
	    else if (vector.x > 0f)
	    {
		vector.x *= deceleration.Value;
		if (vector.x < 0f)
		{
		    vector.x = 0f;
		}
	    }
	    if (vector.y < 0f)
	    {
		vector.y *= deceleration.Value;
		if (vector.y > 0f)
		{
		    vector.y = 0f;
		}
	    }
	    else if (vector.y > 0f)
	    {
		vector.y *= deceleration.Value;
		if (vector.y < 0f)
		{
		    vector.y = 0f;
		}
	    }
	    rb2d.velocity = vector;
	}
    }
}
