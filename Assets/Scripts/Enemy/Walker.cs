using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour
{
    [Header("Structure")]
    //检测玩家的脚本一个不能少
    [SerializeField] private LineOfSightDetector lineOfSightDetector;
    [SerializeField] private AlertRange alertRange; 

    //每一个敌人的四件公式化挂载rb2d,col2d,animator,audiosource,再加一个摄像头和hero位置
    private Rigidbody2D body;
    private Collider2D bodyCollider;
    private tk2dSpriteAnimator animator;
    private AudioSource audioSource;
    private Camera mainCamera;
    private HeroController hero;

    private const float CameraDistanceForActivation = 60f;
    private const float WaitHeroXThreshold = 1f; //距离玩家X方向上的极限距离值

    [Header("Configuration")]
    [SerializeField] private bool ambush; //是否埋伏
    [SerializeField] private string idleClip; //idle的动画片段名字
    [SerializeField] private string turnClip; //turn的动画片段名字
    [SerializeField] private string walkClip; //walk的动画片段名字
    [SerializeField] private float edgeXAdjuster; //检测墙沿x上的增加值
    [SerializeField] private bool preventScaleChange; //是否防止x轴的localscale发生变化
    [SerializeField] private bool preventTurn; //是否阻止转向
    [SerializeField] private float pauseTimeMin; //停止不动的时间
    [SerializeField] private float pauseTimeMax;
    [SerializeField] private float pauseWaitMin; //走路的时间
    [SerializeField] private float pauseWaitMax;
    [SerializeField] private bool pauses;  //是否需要静止状态
    [SerializeField] private float rightScale; //开始时的x轴方向
    [SerializeField] public bool startInactive; //开始时不活跃
    [SerializeField] private int turnAfterIdlePercentage; //Idle状态过后进入转身Turn状态的概率

    [SerializeField] private float turnPause; //设置转身的冷却时间
    [SerializeField] private bool waitForHeroX; //是否等待玩家X方向到位
    [SerializeField] private float waitHeroX; //等待玩家X方向距离
    [SerializeField] public float walkSpeedL; //向左走路的速度
    [SerializeField] public float walkSpeedR;//向右走路的速度
    [SerializeField] public bool ignoreHoles; //是否忽略洞
    [SerializeField] private bool preventTurningToFaceHero; //防止转向玩家的位置

    [SerializeField] private Walker.States state;
    [SerializeField] private Walker.StopReasons stopReason;
    private bool didFulfilCameraDistanceCondition; //暂时没有用到
    private bool didFulfilHeroXCondition; //暂时没有用到
    private int currentFacing;//Debug的时候可以在前面加个[SerializeField]
    private int turningFacing;
    //三个计时器且顾名思义
    private float walkTimeRemaining;
    private float pauseTimeRemaining;
    private float turnCooldownRemaining;

    protected void Awake()
    {
	body = GetComponent<Rigidbody2D>();
	bodyCollider = GetComponent<BoxCollider2D>();
	animator = GetComponent<tk2dSpriteAnimator>();
	audioSource = GetComponent<AudioSource>();
    }

    protected void Start()
    {
	mainCamera = Camera.main;
	hero = HeroController.instance;
	if(currentFacing == 0)
	{
	    currentFacing = ((transform.localScale.x * rightScale >= 0f) ? 1 : -1); //左边是-1，右边是1
	}
	if(state == States.NotReady)
	{
	    turnCooldownRemaining = -Mathf.Epsilon;
	    BeginWaitingForConditions();
	}
    }

    /// <summary>
    /// 我们创建另一个状态机，分为四种状态，每一种都有Update和Stop的方法。
    /// </summary>
    protected void Update()
    {
	turnCooldownRemaining -= Time.deltaTime;
	switch (state)
	{
	    case States.WaitingForConditions:
		UpdateWaitingForConditions();
		break;
	    case States.Stopped:
		UpdateStopping();
		break;
	    case States.Walking:
		UpdateWalking();
		break;
	    case States.Turning:
		UpdateTurning();
		break;
	    default:
		break;
	}
    }

    /// <summary>
    /// 从Waiting状态进入开始移动状态(不一定是Walk也可能是Turn)
    /// </summary>
    public void StartMoving()
    {
	if(state == States.Stopped || state == States.WaitingForConditions)
	{
	    startInactive = false;
	    int facing;
	    if(currentFacing == 0)
	    {
		facing = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
	    }
	    else
	    {
		facing = currentFacing;
	    }
	    BeginWalkingOrTurning(facing);
	}
	Update();
    }

    /// <summary>
    /// 在需要时取消转向
    /// </summary>
    public void CancelTurn()
    {
	if(state == States.Turning)
	{
	    BeginWalking(currentFacing);
	}
    }

    public void Go(int facing)
    {
	turnCooldownRemaining = -Time.deltaTime;
	if(state == States.Stopped || state == States.Walking)
	{
	    BeginWalkingOrTurning(facing);
	}
	else if(state == States.Turning && currentFacing == facing)
	{
	    CancelTurn();
	}
	Update();
    }

    public void RecieveGoMessage(int facing)
    {
	if(state != States.Stopped || stopReason != StopReasons.Controlled)
	{
	    Go(facing);
	}
    }

    /// <summary>
    /// 被脚本StopWalker.cs调用，更改reason为controlled
    /// </summary>
    /// <param name="reason"></param>
    public void Stop(StopReasons reason)
    {
	BeginStopped(reason);
    }

    /// <summary>
    /// 更改turningFacing和currentFacing，属于Turn状态的行为
    /// </summary>
    /// <param name="facing"></param>
    public void ChangeFacing(int facing)
    {
	if(state == States.Turning)
	{
	    turningFacing = facing;
	    currentFacing = -facing;
	    return;
	}
	currentFacing = facing;
    }

    /// <summary>
    /// 开始进入等待状态
    /// </summary>
    private void BeginWaitingForConditions()
    {
	state = States.WaitingForConditions;
	didFulfilCameraDistanceCondition = false;
	didFulfilHeroXCondition = false;
	UpdateWaitingForConditions();
    }

    /// <summary>
    /// 在Update以及BeginWaitingForConditions两大函数中调用，更新等待状态下的行为
    /// </summary>
    private void UpdateWaitingForConditions()
    {
	if (!didFulfilCameraDistanceCondition && (mainCamera.transform.position - transform.position).sqrMagnitude < CameraDistanceForActivation * CameraDistanceForActivation)
	{
	    didFulfilCameraDistanceCondition = true;
	}
	if(didFulfilCameraDistanceCondition && !didFulfilHeroXCondition && hero != null && 
	    Mathf.Abs(hero.transform.GetPositionX() - waitHeroX) < WaitHeroXThreshold)
	{
	    didFulfilHeroXCondition = true;
	}
	if(didFulfilCameraDistanceCondition && (!waitForHeroX || didFulfilHeroXCondition) && !startInactive && !ambush)
	{
	    BeginStopped(StopReasons.Bored);
	    StartMoving();
	}
    }

    /// <summary>
    /// 开始进入停止状态
    /// </summary>
    /// <param name="reason"></param>
    private void BeginStopped(StopReasons reason)
    {
	state = States.Stopped;
	stopReason = reason;
	if (audioSource)
	{
	    audioSource.Stop();
	}
	if(reason == StopReasons.Bored)
	{
	    tk2dSpriteAnimationClip clipByName = animator.GetClipByName(idleClip);
	    if(clipByName != null)
	    {
		animator.Play(clipByName);
	    }
	    body.velocity = Vector2.Scale(body.velocity, new Vector2(0f, 1f)); //相当于把x方向上的速度设置为0
	    if (pauses)
	    {
		pauseTimeRemaining = UnityEngine.Random.Range(pauseTimeMin, pauseTimeMax);
		return;
	    }
	    EndStopping();
	}
    }

    /// <summary>
    /// 在Update中被调用，执行停止Stop状态的行为
    /// </summary>
    private void UpdateStopping()
    {
	if(stopReason == StopReasons.Bored)
	{
	    pauseTimeRemaining -= Time.deltaTime;
	    if(pauseTimeRemaining <= 0f)
	    {
		EndStopping();
	    }
	}
    }

    /// <summary>
    /// 终止停止状态
    /// </summary>
    private void EndStopping()
    {
	if(currentFacing == 0)
	{
	    BeginWalkingOrTurning(UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1);
	    return;
	}
	if(UnityEngine.Random.Range(0,100) < turnAfterIdlePercentage)
	{
	    BeginTurning(-currentFacing);
	    return;
	}
	BeginWalking(currentFacing); //这里应该是开始行走Walk而不是开始转向Turn
    }

    /// <summary>
    /// 要不走路要不转身
    /// </summary>
    /// <param name="facing"></param>
    private void BeginWalkingOrTurning(int facing)
    {
	if(currentFacing == facing)
	{
	    BeginWalking(facing);
	    return;
	}
	BeginTurning(facing);
    }

    /// <summary>
    /// 开始进入Walking状态
    /// </summary>
    /// <param name="facing"></param>
    private void BeginWalking(int facing)
    {
	state = States.Walking;
	animator.Play(walkClip);
	if (!preventScaleChange)
	{
	    transform.SetScaleX(facing * rightScale);
	}
	walkTimeRemaining = UnityEngine.Random.Range(pauseWaitMin, pauseWaitMax);
	if (audioSource)
	{
	    audioSource.Play();
	}
	body.velocity = new Vector2((facing > 0) ? walkSpeedR : walkSpeedL,body.velocity.y);
    }

    /// <summary>
    /// 在Update中被调用，动态执行Walking状态，根据情况决定是否要进入Turning状态或者Stopped状态
    /// </summary>
    private void UpdateWalking()
    {
	if(turnCooldownRemaining <= 0f)
	{
	    Sweep sweep = new Sweep(bodyCollider, 1 - currentFacing, Sweep.DefaultRayCount,Sweep.DefaultSkinThickness);
	    if (sweep.Check(transform.position, bodyCollider.bounds.extents.x + 0.5f, LayerMask.GetMask("Terrain")))
	    {
		BeginTurning(-currentFacing);
		return;
	    }
	    if (!preventTurningToFaceHero && (hero != null && hero.transform.GetPositionX() > transform.GetPositionX() != currentFacing > 0) && lineOfSightDetector != null && lineOfSightDetector.CanSeeHero && alertRange != null && alertRange.IsHeroInRange)
	    {
		BeginTurning(-currentFacing);
		return;
	    }
	    if (!ignoreHoles)
	    {
		Sweep sweep2 = new Sweep(bodyCollider, DirectionUtils.Down, Sweep.DefaultRayCount, 0.1f);
		if (!sweep2.Check((Vector2)transform.position + new Vector2((bodyCollider.bounds.extents.x + 0.5f + edgeXAdjuster) * currentFacing, 0f), 0.25f, LayerMask.GetMask("Terrain")))
		{
		    BeginTurning(-currentFacing);
		    return;
		}
	    }
	}
	if (pauses)
	{
	    walkTimeRemaining -= Time.deltaTime;
	    if(walkTimeRemaining <= 0f)
	    {
		BeginStopped(StopReasons.Bored);
		return;
	    }
	}
	body.velocity = new Vector2((currentFacing > 0) ? walkSpeedR : walkSpeedL, body.velocity.y);
    }

    private void BeginTurning(int facing)
    {
	state = States.Turning;
	turningFacing = facing;
	if (preventTurn)
	{
	    EndTurning();
	    return;
	}
	turnCooldownRemaining = turnPause;
	body.velocity = Vector2.Scale(body.velocity, new Vector2(0f, 1f));
	animator.Play(turnClip);
	FSMUtility.SendEventToGameObject(gameObject, (facing > 0) ? "TURN RIGHT" : "TURN LEFT", false);
    }
    
   /// <summary>
   /// 在Update中被调用，执行Turning转身状态。
   /// </summary>
    private void UpdateTurning()
    {
	body.velocity = Vector2.Scale(body.velocity, new Vector2(0f, 1f));
	if (!animator.Playing)
	{
	    EndTurning();
	}
    }

    /// <summary>
    /// 被UpdateTurning()调用，当动画播放完成后切换到Walking状态。
    /// 被BeginTurning()调用，当preventTurn为true时就不再向下执行了。
    /// </summary>
    private void EndTurning()
    {
	currentFacing = turningFacing;
	BeginWalking(currentFacing);
    }

    /// <summary>
    /// 就清空turnCooldownRemaining
    /// </summary>
    public void ClearTurnCoolDown()
    {
	turnCooldownRemaining = -Mathf.Epsilon;
    }

    public enum States
    {
	NotReady,
	WaitingForConditions,
	Stopped,
	Walking,
	Turning
    }

    public enum StopReasons
    {
	Bored,
	Controlled
    }

}
