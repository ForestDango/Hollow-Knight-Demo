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
	public float walkSpeed; //�ƶ��ٶ�
	public bool spriteFacesLeft; //sprite��ʼʱ���������
	public string groundLayer; //Ҳ����Terrain
	public float turnDelay; //ת����ʱ

	private float nextTurnTime; //��һ��ת���ʱ��

	[Header("Animation")]
	public FsmString walkAnimName; //walk�Ķ�������
	public FsmString turnAnimName; //turn�Ķ�������

	public FsmBool startLeft;
	public FsmBool startRight;
	public FsmBool keepDirection;

	private float scaleX_pos;
	private float scaleX_neg;

	private const float wallRayHeight = 0.5f; //���ǽ�ڵ����߸߶�
	private const float wallRayLength = 0.1f; //���ǽ�ڵ����߳���
	private const float groundRayLength = 1f; //����������߸߶�

	private GameObject target; //Ŀ��
	private Coroutine walkRoutine; //walk��Э��
	private Coroutine turnRoutine; //turn��Э��
	private bool shouldTurn; //Ӧ��ת������

	private float Direction
	{
	    get
	    {
		if (target)
		{
		    return Mathf.Sign(target.transform.localScale.x) * (spriteFacesLeft ? -1 : 1); //��¼��������
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
	/// �˳�ʱͣ����������ִ�е�Э��
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
	/// ���Ŀ��target�����仯�����³�ʼ��
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
		float seconds = clipByName.frames.Length / clipByName.fps;//������������ŵ�ʱ��
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
	/// ����Ƿ�Ӵ���ǽ��
	/// </summary>
	/// <returns></returns>
	private bool CheckWall()
	{
	    Vector2 vector = collider.bounds.center + new Vector3(0f, -(collider.bounds.size.y / 2f) + wallRayHeight);
	    Vector2 vector2 = Vector2.right * Direction;
	    float num = collider.bounds.center.x / 2f + wallRayLength;
	    Debug.DrawLine(vector, vector + vector2 * num);
	    return Physics2D.Raycast(vector, vector2, num, LayerMask.GetMask(groundLayer)).collider != null;
	}

	/// <summary>
	/// ����Ƿ�Ӵ����ذ�
	/// </summary>
	/// <returns></returns>
	private bool CheckFloor()
	{
	    Vector2 vector = collider.bounds.center + new Vector3((collider.bounds.size.x / 2f + wallRayLength) * Direction, -(collider.bounds.size.y / 2f) + wallRayHeight);
	    Debug.DrawLine(vector, vector + Vector2.down * groundRayLength);
	    return !(Physics2D.Raycast(vector, Vector2.down, groundRayLength, LayerMask.GetMask(groundLayer)).collider != null);
	}

	/// <summary>
	/// ����Ƿ��Ѿ��Ӵ�������
	/// </summary>
	/// <returns></returns>
	private bool CheckIsGrounded()
	{
	    Vector2 vector = collider.bounds.center + new Vector3(0f,-(collider.bounds.center.y / 2f) + wallRayHeight);
	    Debug.DrawLine(vector, vector + Vector2.down * groundRayLength);
	    return Physics2D.Raycast(vector, Vector2.down, groundRayLength, LayerMask.GetMask(groundLayer)).collider != null;
	}

	/// <summary>
	/// ���ÿ�ʼʱGameObject�ķ���
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
	    if (!startLeft.Value && !startRight.Value && !keepDirection.Value && UnityEngine.Random.Range(0f, 100f) <= 50f)//���ѡ��һ��
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
