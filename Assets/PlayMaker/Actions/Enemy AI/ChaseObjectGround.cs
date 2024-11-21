using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Object runs towards target")]
    public class ChaseObjectGround : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault gameObject;
	[UIHint(UIHint.Variable)]
	public FsmGameObject target;
	public FsmFloat speedMax;
	public FsmFloat acceleration;
	public bool animateTurnAndRun;
	public FsmString runAnimation;
	public FsmString turnAnimation;
	public FsmFloat turnRange;
	private FsmGameObject self;
	private tk2dSpriteAnimator animator;
	private bool turning;
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
	public override void OnEnter()
	{
	    CacheRigidBody2d(Fsm.GetOwnerDefaultTarget(gameObject));
	    self = Fsm.GetOwnerDefaultTarget(gameObject);
	    animator = self.Value.GetComponent<tk2dSpriteAnimator>();
	    DoChase();
	}

	public override void OnPreprocess()
	{
	    Fsm.HandleFixedUpdate = true;
	}

	public override void OnFixedUpdate()
	{
	    DoChase();
	}

	private void DoChase()
	{
	    if(rb2d == null)
	    {
		return;
	    }
	    Vector2 velocity = rb2d.velocity;
	    if (self.Value.transform.position.x < target.Value.transform.position.x - turnRange.Value || self.Value.transform.position.x > target.Value.transform.position.x + turnRange.Value)
	    {
		if (self.Value.transform.position.x < target.Value.transform.position.x)
		{
		    velocity.x += acceleration.Value;
		    if (animateTurnAndRun)
		    {
			if (velocity.x < 0f && !turning)
			{
			    animator.Play(turnAnimation.Value);
			    turning = true;
			}
			if (velocity.x > 0f && turning)
			{
			    animator.Play(runAnimation.Value);
			    turning = false;
			}
		    }
		}
		else
		{
		    velocity.x -= acceleration.Value;
		    if (animateTurnAndRun)
		    {
			if (velocity.x > 0f && !turning)
			{
			    animator.Play(turnAnimation.Value);
			    turning = true;
			}
			if (velocity.x < 0f && turning)
			{
			    animator.Play(runAnimation.Value);
			    turning = false;
			}
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
		rb2d.velocity = velocity;
	    }
	}
    }

}
