using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Object buzzes towards target")]
    public class ChaseObject : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault gameObject;

	[UIHint(UIHint.Variable)]
	public FsmGameObject target;
	public FsmFloat speedMax;
	public FsmFloat acceleration;
	public FsmFloat targetSpread;
	public FsmFloat spreadResetTimeMin;
	public FsmFloat spreadResetTimeMax;
	private bool spreadSet;
	private float spreadResetTime;
	private float spreadX;
	private float spreadY;
	private FsmGameObject self;
	private float timer;
	private float spreadResetTimer;

	public override void Reset()
	{
	    gameObject = null;
	    target = null;
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
		return;
	    if(targetSpread.Value > 0f)
	    {
		if(timer >= spreadResetTime)
		{
		    this.spreadX = UnityEngine.Random.Range(-this.targetSpread.Value, this.targetSpread.Value);
		    this.spreadY = UnityEngine.Random.Range(-this.targetSpread.Value, this.targetSpread.Value);
		    this.timer = 0f;
		    this.spreadResetTime = UnityEngine.Random.Range(this.spreadResetTimeMin.Value, this.spreadResetTimeMax.Value);
		}
		else
		{
		    timer += Time.deltaTime;
		}
	    }
	    Vector2 velocity = rb2d.velocity;
	    if (this.self.Value.transform.position.x < this.target.Value.transform.position.x + this.spreadX)
	    {
		velocity.x += this.acceleration.Value;
	    }
	    else
	    {
		velocity.x -= this.acceleration.Value;
	    }
	    if (this.self.Value.transform.position.y < this.target.Value.transform.position.y + this.spreadY)
	    {
		velocity.y += this.acceleration.Value;
	    }
	    else
	    {
		velocity.y -= this.acceleration.Value;
	    }
	    //限制速度
	    if(velocity.x > speedMax.Value)
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
