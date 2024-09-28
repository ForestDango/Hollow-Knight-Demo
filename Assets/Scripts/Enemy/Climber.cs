using System;
using System.Collections;
using UnityEngine;

public class Climber : MonoBehaviour
{
    private tk2dSpriteAnimator anim;
    private Rigidbody2D body;
    private BoxCollider2D col;

    public bool startRight; //开始的方向是右边

    private bool clockwise; //是否顺时针旋转
    public float speed;//移动速度
    public float spinTime; //旋转时间
    [Space]
    public float wallRayPadding; //墙壁射线检测距离
    [Space]
    public Vector2 constrain; //束缚

    public float minTurnDistance; //最小转向距离

    private Vector2 previousPos;
    private Vector2 previousTurnPos;
    [SerializeField]private Direction currentDirection; //Debug用，发现没问题可以删了[SerializeField]
    private Coroutine turnRoutine; //给转向设置为协程，循序渐进的实现转身的效果

    public Climber()
    {
	startRight = true;
	clockwise = true;
	speed = 2f;
	spinTime = 0.25f;
	wallRayPadding = 0.1f;
	constrain = new Vector2(0.1f, 0.1f);
	minTurnDistance = 0.25f;
    }

    private void Awake()
    {
	//公式化三件套
	anim = GetComponent<tk2dSpriteAnimator>();
	body = GetComponent<Rigidbody2D>();
	col = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
	StickToGround();
	float num = Mathf.Sign(transform.localScale.x);
	if (!startRight)
	{
	    num *= -1f;
	}
	clockwise = num > 0f; //判断是顺时针还是逆时针
	float num2 = transform.eulerAngles.z % 360f;
	//获取开始游戏时climber当前方向
	if(num2 > 45f && num2 <= 135f)
	{
	    currentDirection = clockwise ? Direction.Up : Direction.Down;
	}
	else if(num2 > 135f && num2 <= 225f)
	{
	    currentDirection = clockwise ? Direction.Left : Direction.Right;
	}
	else if (num2 > 225f && num2 <= 315f)
	{
	    currentDirection = clockwise ? Direction.Down : Direction.Up;
	}
	else
	{
	    currentDirection = clockwise ? Direction.Right : Direction.Left;
	}
	Recoil component = GetComponent<Recoil>();
	if (component)
	{
	    component.SkipFreezingByController = true;
	    component.OnHandleFreeze += Stun;
	}
	previousPos = transform.position;
	StartCoroutine(Walk());
    }

    private IEnumerator Walk()
    {
	anim.Play("Walk");
	body.velocity = GetVelocity(currentDirection);
	for(; ; )
	{
	    Vector2 vector = transform.position;
	    bool flag = false;
	    if(Mathf.Abs(vector.x - previousPos.x) > constrain.x)
	    {
		vector.x = previousPos.x;
		flag = true;
	    }
	    if (Mathf.Abs(vector.y - previousPos.y) > constrain.y)
	    {
		vector.y = previousPos.y;
		flag = true;
	    }
	    if(flag)
	    {
		transform.position = vector;
	    }
	    else
	    {
		previousPos = transform.position;
	    }
	    if (Vector3.Distance(previousTurnPos, transform.position) >= minTurnDistance)
	    {
		if (!CheckGround())
		{
		    turnRoutine = StartCoroutine(Turn(clockwise, false));
		    yield return turnRoutine;
		}
		else if (CheckWall()) //当不在地面上以及碰到墙壁后挂机并执行Turn协程
		{
		    turnRoutine = StartCoroutine(Turn(!clockwise, true));
		    yield return turnRoutine;
		}
	    }
	    yield return null;
	}
    }

    private IEnumerator Turn(bool turnClockwise, bool tweenPos = false)
    {
	body.velocity = Vector2.zero;
	float currentRotation = transform.eulerAngles.z;
	float targetRotation = currentRotation + (turnClockwise ? -90 : 90);
	Vector3 currentPosition = transform.position;
	Vector3 targetPosition = currentPosition + GetTweenPos(currentDirection);
	for (float elapsed = 0f; elapsed < spinTime; elapsed += Time.deltaTime)
	{
	    float t = elapsed / spinTime;
	    transform.SetRotation2D(Mathf.Lerp(currentRotation, targetRotation, t)); //更改rotation和position
	    if (tweenPos)
	    {
		transform.position = Vector3.Lerp(currentPosition, targetPosition, t);
	    }
	    yield return null;
	}
	transform.SetRotation2D(targetRotation);
	int num = (int)currentDirection;
	num += (turnClockwise ? 1 : -1);
	int num2 = Enum.GetNames(typeof(Direction)).Length; //4
	//防止数字超出枚举长度或者小于0
	if(num < 0)
	{
	    num = num2 - 1;
	}
	else if(num >= num2)
	{
	    num = 0;
	}
	currentDirection = (Direction)num;
	body.velocity = GetVelocity(currentDirection);
	previousPos = transform.position;
	previousTurnPos = previousPos;
	turnRoutine = null;
    }

    /// <summary>
    /// 不同方向上赋值的速度不同
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private Vector2 GetVelocity(Direction direction)
    {
	Vector2 zero = Vector2.zero;
	switch (direction)
	{
	    case Direction.Right:
		zero = new Vector2(speed, 0f);
		break;
	    case Direction.Down:
		zero = new Vector2(0f, -speed);
		break;
	    case Direction.Left:
		zero = new Vector2(-speed, 0f);
		break;
	    case Direction.Up:
		zero = new Vector2(0f, speed);
		break;
	}
	return zero;
    }

    private bool CheckGround()
    {
	return FireRayLocal(Vector2.down, 1f).collider != null;
    }

    private bool CheckWall()
    {
	return FireRayLocal(clockwise ? Vector2.right : Vector2.left, col.size.x / 2f + wallRayPadding).collider != null;
    }

    /// <summary>
    /// 以后做到人物攻击时才要用到
    /// </summary>
    public void Stun()
    {
	if(turnRoutine == null)
	{
	    StopAllCoroutines();
	    StartCoroutine(DoStun());
	}
    }

    private IEnumerator DoStun()
    {
	body.velocity = Vector2.zero;
	yield return StartCoroutine(anim.PlayAnimWait("Stun"));
	StartCoroutine(Walk());
    }

    private RaycastHit2D FireRayLocal(Vector2 direction, float length)
    {
	Vector2 vector = transform.TransformPoint(col.offset);
	Vector2 vector2 = transform.TransformDirection(direction);
	RaycastHit2D result = Physics2D.Raycast(vector, vector2, length, LayerMask.GetMask("Terrain"));
	Debug.DrawRay(vector, vector2);
	return result;
    }

    private Vector3 GetTweenPos(Direction direction)
    {
	Vector2 result = Vector2.zero;
	switch (direction)
	{
	    case Direction.Right:
		result = (clockwise ? new Vector2(col.size.x / 2f, col.size.y / 2f) : new Vector2(col.size.x / 2f, -(col.size.y / 2f)));
		result.x += wallRayPadding;
		break;
	    case Direction.Down:
		result = (clockwise ? new Vector2(col.size.x / 2f, -(col.size.y / 2f)) : new Vector2(-(col.size.x / 2f), -(col.size.y / 2f)));
		result.y -= wallRayPadding;
		break;
	    case Direction.Left:
		result = (clockwise ? new Vector2(-(col.size.x / 2f), -(col.size.y / 2f)) : new Vector2(-(col.size.x / 2f), col.size.y / 2f));
		result.x -= wallRayPadding;
		break;
	    case Direction.Up:
		result = (clockwise ? new Vector2(-(col.size.x / 2f), col.size.y / 2f) : new Vector2(col.size.x / 2f, col.size.y / 2f));
		result.y += wallRayPadding;
		break;
	}
	return result;
    }

    /// <summary>
    /// 在开始游戏时让它粘在离它向下射线2f最近的地面。
    /// </summary>
    private void StickToGround()
    {
	RaycastHit2D raycastHit2D = FireRayLocal(Vector2.down, 2f);
	if(raycastHit2D.collider != null)
	{
	    transform.position = raycastHit2D.point;
	}
    }

    private enum Direction
    {
	Right,
	Down,
	Left,
	Up
    }
}
