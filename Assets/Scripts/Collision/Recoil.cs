using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Collider2D bodyCollider;

    [SerializeField] private bool recoilUp; //是否有向上的后坐力
    [SerializeField] private float recoilSpeedBase = 15f; //基本后坐力速度
    [SerializeField] private float recoilDuration; //后坐力持续时间
    [SerializeField] public bool freezeInPlace; //是否会不动
    [SerializeField] private bool stopVelocityXWhenRecoilingUp; //当正在受上后坐力的时候停止X轴方向的速度
    [SerializeField] private bool preventRecoilUp;

    private bool skipFreezingByController;
    [SerializeField]private States state;
    private float recoilTimeRemaining; //后坐力持续时间
    private float recoilSpeed;//最终后坐力的速度
    private Sweep recoilSweep; //检测地形
    private bool isRecoilSweeping; //是否
    private const int SweepLayerMask = 256; //也就是Layer "Terrain"

    public delegate void CancelRecoilEvent();
    public event CancelRecoilEvent OnCancelRecoil;

    public delegate void FreezeEvent();
    public event FreezeEvent OnHandleFreeze;

    public bool SkipFreezingByController
    {
	get
	{
	    return skipFreezingByController;
	}
	set
	{
	    skipFreezingByController = value;
	}
    }

    public bool IsRecoiling
    {
	get
	{
	    return state == States.Recoiling || state == States.Frozen;
	}
    }

    protected void Reset()
    {
	freezeInPlace = false;
	stopVelocityXWhenRecoilingUp = true;
	recoilDuration = 0.5f;
	recoilSpeedBase = 15f;
	preventRecoilUp = false;
    }

    protected void Awake()
    {
	rb2d = GetComponent<Rigidbody2D>();
	bodyCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
	CancelRecoil();
    }

    protected void FixedUpdate()
    {
	UpdatePhysics(Time.fixedDeltaTime);
    }

    /// <summary>
    /// 更新游戏对象的物理行为
    /// </summary>
    /// <param name="deltaTime"></param>
    private void UpdatePhysics(float deltaTime)
    {
	if(state == States.Frozen)
	{
	    if(rb2d != null)
	    {
		rb2d.velocity = Vector2.zero;
	    }
	    recoilTimeRemaining -= deltaTime;
	    if(recoilTimeRemaining <= 0f)
	    {
		CancelRecoil();
		return;
	    }
	}
	else if(state == States.Recoiling)
	{
	    if (isRecoilSweeping)
	    {
		float num;
		if (recoilSweep.Check(base.transform.position,recoilSpeed * deltaTime, SweepLayerMask,out num))
		{
		    isRecoilSweeping = false;
		}
		if (num > Mathf.Epsilon)
		{
		    transform.Translate(recoilSweep.Direction * num, Space.World);
		}
	    }
	    recoilTimeRemaining -= deltaTime;
	    if (recoilTimeRemaining <= 0f)
	    {
		CancelRecoil();
	    }
	}
    }

    /// <summary>
    /// 在某个方向上受后坐力的行为
    /// </summary>
    /// <param name="attackDirection"></param>
    /// <param name="attackMagnitude"></param>
    public void RecoilByDirection(int attackDirection,float attackMagnitude)
    {
	if(state != States.Ready)
	{
	    return;
	}
	if (freezeInPlace)
	{
	    Freeze();
	    return;
	}
	if(attackDirection == 1 && preventRecoilUp)
	{
	    return;
	}
	if (bodyCollider == null)
	{
	    bodyCollider = GetComponent<Collider2D>();
	}
	state = States.Recoiling;
	recoilSpeed = recoilSpeedBase * attackMagnitude;
	recoilSweep = new Sweep(bodyCollider, attackDirection, 3, 0.1f);
	isRecoilSweeping = true;
	recoilTimeRemaining = recoilDuration;
	switch (attackDirection)
	{
	    case 0:
		FSMUtility.SendEventToGameObject(gameObject, "RECOIL HORIZONTAL", false);
		FSMUtility.SendEventToGameObject(gameObject, "HIT RIGHT", false);
		break;
	    case 1:
		FSMUtility.SendEventToGameObject(gameObject, "HIT UP", false);
		break;
	    case 2:
		FSMUtility.SendEventToGameObject(gameObject, "RECOIL HORIZONTAL", false);
		FSMUtility.SendEventToGameObject(gameObject, "HIT LEFT", false);
		break;
	    case 3:
		FSMUtility.SendEventToGameObject(gameObject, "HIT DOWN", false);
		break;
	}
	UpdatePhysics(0f);
    }

    /// <summary>
    /// 冻结状态发生的行为
    /// </summary>
    private void Freeze()
    {
	if (skipFreezingByController)
	{
	    if (OnHandleFreeze != null)
	    {
		OnHandleFreeze();
	    }
	    state = States.Ready;
	    return;
	}
	state = States.Frozen;
	if(rb2d != null)
	{
	    rb2d.velocity = Vector2.zero;
	}
	PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(gameObject, "Climber Control");
	if(playMakerFSM != null)
	{
	    playMakerFSM.SendEvent("FREEZE IN PLACE");
	}
	recoilTimeRemaining = recoilDuration;
	UpdatePhysics(0f);
    }

    public void CancelRecoil()
    {
	if(state != States.Ready)
	{
	    state = States.Ready;
	    if (OnCancelRecoil != null)
	    {
		OnCancelRecoil();
	    }
	}
    }

    public void SetRecoilSpeed(float newSpeed)
    {
	recoilSpeedBase = newSpeed;
    }

    private enum States
    {
	Ready,
	Frozen,
	Recoiling
    }
}
