using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Object idly buzzes about within a defined range")]
    public class IdleBuzz : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;
	public FsmFloat waitMin;
	public FsmFloat waitMax;
	public FsmFloat speedMax;
	public FsmFloat accelerationMax;
	public FsmFloat roamingRange;
	private FsmGameObject target;
	private float startX;
	private float startY;
	private float accelX;
	private float accelY;
	private float waitTime;
	private const float dampener = 1.125f;

	public override void Reset()
	{
	    gameObject = null;
	    waitMin = 0f;
	    waitMax = 0f;
	    accelerationMax = 0f;
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
	    target = Fsm.GetOwnerDefaultTarget(gameObject);
	    startX = target.Value.transform.position.x;
	    startY = target.Value.transform.position.y;
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
	    Vector2 velocity = rb2d.velocity;
	    if (target.Value.transform.position.y < startY - roamingRange.Value)
	    {
		if (velocity.y < 0f)
		{
		    accelY = accelerationMax.Value;
		    accelY /= 2000f;
		    velocity.y /= dampener;
		    waitTime = Random.Range(waitMin.Value, waitMax.Value);
		}
	    }
	    else if (target.Value.transform.position.y > startY + roamingRange.Value && velocity.y > 0f)
	    {
		accelY = -accelerationMax.Value;
		accelY /= 2000f;
		velocity.y /= dampener;
		waitTime = Random.Range(waitMin.Value, waitMax.Value);
	    }
	    if (target.Value.transform.position.x < startX - roamingRange.Value)
	    {
		if (velocity.x < 0f)
		{
		    accelX = accelerationMax.Value;
		    accelX /= 2000f;
		    velocity.x /= dampener;
		    waitTime = Random.Range(waitMin.Value, waitMax.Value);
		}
	    }
	    else if (target.Value.transform.position.x > startX + roamingRange.Value && velocity.x > 0f)
	    {
		accelX = -accelerationMax.Value;
		accelX /= 2000f;
		velocity.x /= dampener;
		waitTime = Random.Range(waitMin.Value, waitMax.Value);
	    }
	    //计时器时间到后：
	    if (waitTime <= Mathf.Epsilon)
	    {
		if (target.Value.transform.position.y < startY - roamingRange.Value)
		{
		    accelY = Random.Range(0f, accelerationMax.Value);
		}
		else if (target.Value.transform.position.y > startY + roamingRange.Value)
		{
		    accelY = Random.Range(-accelerationMax.Value, 0f);
		}
		else
		{
		    accelY = Random.Range(-accelerationMax.Value, accelerationMax.Value);
		}
		if (target.Value.transform.position.x < startX - roamingRange.Value)
		{
		    accelX = Random.Range(0f, accelerationMax.Value);
		}
		else if (target.Value.transform.position.x > startX + roamingRange.Value)
		{
		    accelX = Random.Range(-accelerationMax.Value, 0f);
		}
		else
		{
		    accelX = Random.Range(-accelerationMax.Value, accelerationMax.Value);
		}
		accelY /= 2000f;
		accelX /= 2000f;
		waitTime = Random.Range(waitMin.Value, waitMax.Value);
	    }
	    if (waitTime > Mathf.Epsilon)
	    {
		waitTime -= Time.deltaTime;
	    }
	    //别忘了不能直接给rb2d.velocity的分变量x,y直接复制
	    velocity.x += accelX;
	    velocity.y += accelY;
	    //起到限制速度的作用
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
