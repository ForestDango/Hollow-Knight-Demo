using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Try to keep a certain distance from target.")]
    public class DistanceWalk : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault gameObject;
	[UIHint(UIHint.Variable)]
	public FsmGameObject target;

	public FsmFloat distance;
	public FsmFloat speed;
	public FsmFloat range;

	public bool changeAnimation;
	public bool spriteFacesRight;
	public FsmString forwardAnimation;
	public FsmString backAnimation;

	private float distanceAway;
	private FsmGameObject self;
	private tk2dSpriteAnimator animator;
	private bool movingRight;

	private float ANIM_CHANGE_TIME = 0.6f;
	private float changeTimer;
	private bool randomStart;
	public override void Reset()
	{
	    gameObject = null;
	    target = null;
	    speed = 0f;
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
	    if (changeAnimation)
	    {
		animator = self.Value.GetComponent<tk2dSpriteAnimator>();
	    }
	    DoWalk();
	}

	public override void OnUpdate()
	{
	    if (changeTimer > 0f)
	    {
		changeTimer -= Time.deltaTime;
	    }
	}

	public override void OnFixedUpdate()
	{
	    DoWalk();
	}

	private void DoWalk()
	{
	    if (rb2d == null)
	    {
		return;
	    }
	    distanceAway = self.Value.transform.position.x - target.Value.transform.position.x;
	    if (distanceAway < 0f)
	    {
		distanceAway *= -1f;
	    }
	    Vector2 velocity = rb2d.velocity;
	    if (distanceAway > distance.Value + range.Value)
	    {
		if (self.Value.transform.position.x < target.Value.transform.position.x)
		{
		    if (!movingRight && changeTimer <= 0f)
		    {
			velocity.x = speed.Value;
			movingRight = true;
			changeTimer = ANIM_CHANGE_TIME;
		    }
		}
		else if (movingRight && changeTimer <= 0f)
		{
		    velocity.x = -speed.Value;
		    movingRight = false;
		    changeTimer = ANIM_CHANGE_TIME;
		}
	    }
	    else if (distanceAway < distance.Value - range.Value)
	    {
		if (self.Value.transform.position.x < target.Value.transform.position.x)
		{
		    if (movingRight && changeTimer <= 0f)
		    {
			velocity.x = -speed.Value;
			movingRight = false;
			changeTimer = ANIM_CHANGE_TIME;
		    }
		}
		else if (!movingRight && changeTimer <= 0f)
		{
		    velocity.x = speed.Value;
		    movingRight = true;
		    changeTimer = ANIM_CHANGE_TIME;
		}
	    }
	    if (rb2d.velocity.x > -0.1f && rb2d.velocity.x < 0.1f)
	    {
		if (Random.value > 0.5f)
		{
		    velocity.x = speed.Value;
		    movingRight = true;
		}
		else
		{
		    velocity.x = -speed.Value;
		    movingRight = false;
		}
		randomStart = true;
	    }
	    rb2d.velocity = velocity;
	    if (changeAnimation)
	    {
		if (self.Value.transform.localScale.x > 0f)
		{
		    if ((spriteFacesRight && movingRight) || (!spriteFacesRight && !movingRight))
		    {
			animator.Play(forwardAnimation.Value);
		    }
		    if ((!spriteFacesRight && movingRight) || (spriteFacesRight && !movingRight))
		    {
			animator.Play(backAnimation.Value);
			return;
		    }
		}
		else
		{
		    if ((spriteFacesRight && movingRight) || (!spriteFacesRight && !movingRight))
		    {
			animator.Play(backAnimation.Value);
		    }
		    if ((!spriteFacesRight && movingRight) || (spriteFacesRight && !movingRight))
		    {
			animator.Play(forwardAnimation.Value);
		    }
		}
	    }
	}
    }

}
