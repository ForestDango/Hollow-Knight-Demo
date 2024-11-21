using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    public class WalkLeftRight : FsmStateAction
    {
	private Rigidbody2D body;
	private tk2dSpriteAnimator spriteAnimator;
	private Collider2D collider;

	public FsmOwnerDefault gameObject;
	public float walkSpeed; //移动速度
	public bool spriteFacesLeft; //sprite开始时是向左的吗
	public string groundLayer; //也就是Terrain
	public float turnDelay; //转向延时

	private float nextTurnTime; //下一次转身的时间

	[Header("Animation")]
	public FsmString walkAnimName; //walk的动画名字
	public FsmString turnAnimName; //turn的动画名字

	public FsmBool startLeft;
	public FsmBool startRight;
	public FsmBool keepDirection;

	private float scaleX_pos;
	private float scaleX_neg;

	private const float wallRayHeight = 0.5f; //检测墙壁的射线高度
	private const float wallRayLength = 0.1f; //检测墙壁的射线长度
	private const float groundRayLength = 1f; //检测地面的射线高度

	private GameObject target; //目标
	private Coroutine walkRoutine; //walk的协程
	private Coroutine turnRoutine; //turn的协程
	private bool shouldTurn; //应该转身了吗

	private float Direction
	{
	    get
	    {
		if (target)
		{
		    return Mathf.Sign(target.transform.localScale.x) * (spriteFacesLeft ? -1 : 1); //记录方向属性
		}
		return 0f;
	    }
	}


	public override void OnEnter()
	{
	    UpdateIfTargetChanged();
	    SetupStartingDirection();
	    walkRoutine = StartCoroutine(Walk());
	}

	/// <summary>
	/// 退出时停掉所有正在执行的协程
	/// </summary>
	public override void OnExit()
	{
	    if(walkRoutine != null)
	    {
		StopCoroutine(walkRoutine);
		walkRoutine = null;
	    }
	    if (turnRoutine != null)
	    {
		StopCoroutine(turnRoutine);
		turnRoutine = null;
	    }
	}

	/// <summary>
	/// 如果目标target发生变化后重新初始化
	/// </summary>
	private void UpdateIfTargetChanged()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if(ownerDefaultTarget != target)
	    {
		target = ownerDefaultTarget;
		body = target.GetComponent<Rigidbody2D>();
		collider = target.GetComponent<Collider2D>();
		spriteAnimator = target.GetComponent<tk2dSpriteAnimator>();
	    }
	}

	private IEnumerator Walk()
	{
	    if (spriteAnimator)
	    {
		spriteAnimator.Play(walkAnimName.Value);
	    }
	    for(; ; )
	    {
		if (body)
		{
		    Vector2 velocity = body.velocity;
		    velocity.x = walkSpeed * Direction;
		    body.velocity = velocity;
		    if(shouldTurn || (CheckIsGrounded() && (CheckWall() || CheckFloor()) && Time.time >= nextTurnTime))
		    {
			shouldTurn = false;
			nextTurnTime = Time.time + turnDelay;
			turnRoutine = StartCoroutine(Turn());
			yield return turnRoutine;
		    }
		}
		yield return new WaitForFixedUpdate();
	    }
	}

	private IEnumerator Turn()
	{
	    Vector2 velocity = body.velocity;
	    velocity.x = 0f;
	    body.velocity = velocity;
	    tk2dSpriteAnimationClip clipByName = spriteAnimator.GetClipByName(turnAnimName.Name);
	    if(clipByName != null)
	    {
		float seconds = clipByName.frames.Length / clipByName.fps;//计算出动画播放的时间
		spriteAnimator.Play(clipByName);
		yield return new WaitForSeconds(seconds);
	    }
	    Vector3 localScale = target.transform.localScale;
	    localScale.x *= -1f;
	    target.transform.localScale = localScale;
	    if (spriteAnimator)
	    {
		spriteAnimator.Play(walkAnimName.Value);
	    }
	    turnRoutine = null;
	}

	/// <summary>
	/// 检测是否接触到墙面
	/// </summary>
	/// <returns></returns>
	private bool CheckWall()
	{
	    Vector2 vector = collider.bounds.center + new Vector3(0f, -(collider.bounds.size.y / 2f));
	    Vector2 vector2 = Vector2.right * Direction;
	    float num = collider.bounds.center.x / 2f + wallRayLength;
	    //Debug.DrawLine(vector, vector + vector2 * num);
	    return Physics2D.Raycast(vector, vector2, num, LayerMask.GetMask(groundLayer)).collider != null;
	}

	/// <summary>
	/// 检测是否接触到地板
	/// </summary>
	/// <returns></returns>
	private bool CheckFloor()
	{
	    Vector2 vector = collider.bounds.center + new Vector3((collider.bounds.size.x / 2f + wallRayLength) * Direction, -(collider.bounds.size.y / 2f) + wallRayHeight);
	    //Debug.DrawLine(vector, vector + Vector2.down * groundRayLength);
	    return !(Physics2D.Raycast(vector, Vector2.down, groundRayLength, LayerMask.GetMask(groundLayer)).collider != null);
	}

	/// <summary>
	/// 检测是否已经接触到地面
	/// </summary>
	/// <returns></returns>
	private bool CheckIsGrounded()
	{
	    Vector2 vector = collider.bounds.min;
	    //Debug.DrawLine(vector, vector + Vector2.down * groundRayLength);
	    return Physics2D.Raycast(vector, Vector2.down, groundRayLength, LayerMask.GetMask(groundLayer)).collider != null;
	}

	/// <summary>
	/// 设置开始时GameObject的方向
	/// </summary>
	private void SetupStartingDirection()
	{
	    if (target.transform.localScale.x < 0f)
	    {
		if (!spriteFacesLeft && startRight.Value)
		{
		    shouldTurn = true;
		}
		if (spriteFacesLeft && startLeft.Value)
		{
		    shouldTurn = true;
		}
	    }
	    else
	    {
		if (spriteFacesLeft && startRight.Value)
		{
		    shouldTurn = true;
		}
		if (!spriteFacesLeft && startLeft.Value)
		{
		    shouldTurn = true;
		}
	    }
	    if (!startLeft.Value && !startRight.Value && !keepDirection.Value && UnityEngine.Random.Range(0f, 100f) <= 50f)//随机选择一边
	    {
		shouldTurn = true;
	    }
	    startLeft.Value = false;
	    startRight.Value = false;
	}

	public WalkLeftRight()
	{
	    walkSpeed = 4f;
	    groundLayer = "Terrain";
	    turnDelay = 1f;
	}

    }

}
