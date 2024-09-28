using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using GlobalEnums;
using UnityEngine;
using System.Reflection;

public class HeroController : MonoBehaviour
{
    public ActorStates hero_state;
    public ActorStates prev_hero_state;

    public bool acceptingInput = true;
    public bool controlReqlinquished; //控制是否被放弃
    public bool isHeroInPosition = true;

    public delegate void HeroInPosition(bool forceDirect);
    public event HeroInPosition heroInPosition;

    public float move_input;
    public float vertical_input;

    public Vector2 current_velocity;

    public float WALK_SPEED = 3.1f;//走路速度
    public float RUN_SPEED = 5f;//跑步速度
    public float JUMP_SPEED = 5f;//跳跃的速度

    private NailSlash slashComponent; //决定使用哪种攻击的NailSlash
    private PlayMakerFSM slashFsm;//决定使用哪种攻击的PlayMakerFSM

    public NailSlash normalSlash;
    public NailSlash altetnateSlash;
    public NailSlash upSlash;
    public NailSlash downSlash;

    public PlayMakerFSM normalSlashFsm; 
    public PlayMakerFSM altetnateSlashFsm;
    public PlayMakerFSM upSlashFsm;
    public PlayMakerFSM downSlashFsm;

    private bool attackQueuing; //是否开始攻击计数步骤
    private int attackQueueSteps; //攻击计数步骤

    private float attack_time;
    private float attackDuration; //攻击状态持续时间，根据有无护符来决定
    private float attack_cooldown;
    private float altAttackTime; //当时间超出可按二段攻击的时间后，cstate.altattack就会为false

    public float ATTACK_DURATION; //无护符时攻击状态持续时间
    public float ATTACK_COOLDOWN_TIME; //攻击后冷却时间
    public float ATTACK_RECOVERY_TIME; //攻击恢复时间，一旦超出这个时间就退出攻击状态
    public float ALT_ATTACK_RESET; //二段攻击重置时间

    private int ATTACK_QUEUE_STEPS = 5; //超过5步即可开始攻击

    private float NAIL_TERRAIN_CHECK_TIME = 0.12f;

    private bool drainMP; //是否正在流走MP
    private float drainMP_timer; //流走MP的计时器
    private float drainMP_time; //流走MP花费的时间
    private float MP_drained; //已经流走的MP数量
    private float focusMP_amount; //使用focus回血所需要的MP数量
    private float preventCastByDialogueEndTimer;
    public PlayMakerFSM spellControl;


    private int jump_steps; //跳跃的步
    private int jumped_steps; //已经跳跃的步
    private int jumpQueueSteps; //跳跃队列的步
    private bool jumpQueuing; //是否进入跳跃队列中

    private int jumpReleaseQueueSteps; //释放跳跃后的步
    private bool jumpReleaseQueuing; //是否进入释放跳跃队列中
    private bool jumpReleaseQueueingEnabled; //是否允许进入释放跳跃队列中

    public float MAX_FALL_VELOCITY; //最大下落速度(防止速度太快了)
    public int JUMP_STEPS; //最大跳跃的步
    public int JUMP_STEPS_MIN; //最小跳跃的步
    private int JUMP_QUEUE_STEPS; //最大跳跃队列的步
    private int JUMP_RELEASE_QUEUE_STEPS;//最大跳跃释放队列的步

    private int dashQueueSteps;
    private bool dashQueuing;

    private float dashCooldownTimer; //冲刺冷却时间
    private float dash_timer; //正在冲刺计数器
    private float back_dash_timer; ////正在后撤冲刺计数器 (标注：此行代码无用待后续开发)
    private float dashLandingTimer;
    private bool airDashed;//是否是在空中冲刺
    public bool dashingDown;//是否正在执行向下冲刺
    public PlayMakerFSM dashBurst;
    public GameObject dashParticlesPrefab;//冲刺粒子效果预制体
    public GameObject backDashPrefab; //后撤冲刺特效预制体 标注：此行代码无用待后续开发
    private GameObject backDash;//后撤冲刺 (标注：此行代码无用待后续开发)
    private GameObject dashEffect;//后撤冲刺特效生成 (标注：此行代码无用待后续开发)

    public float DASH_SPEED; //冲刺时的速度
    public float DASH_TIME; //冲刺时间
    public float DASH_COOLDOWN; //冲刺冷却时间
    public float BACK_DASH_SPEED;//后撤冲刺时的速度 (标注：此行代码无用待后续开发)
    public float BACK_DASH_TIME;//后撤冲刺时间 (标注：此行代码无用待后续开发)
    public float BACK_DASH_COOLDOWN; //后撤冲刺冷却时间 (标注：此行代码无用待后续开发)
    public float DOWN_DASH_TIME; //向下冲刺持续时间
    public float DASH_LANDING_TIME;
    public int DASH_QUEUE_STEPS; //最大冲刺队列的步

    public delegate void TakeDamageEvent();
    public event TakeDamageEvent OnTakenDamage;
    public delegate void OnDeathEvent();
    public event OnDeathEvent OnDeath;

    public bool takeNoDamage; //不受到伤害
    public PlayMakerFSM damageEffectFSM; //负责的受伤效果playmakerFSM
    public DamageMode damageMode; //受伤类型
    private Coroutine takeDamageCoroutine; //受伤协程
    private float parryInvulnTimer;  //无敌时间
    public float INVUL_TIME;//无敌时间

    public float DAMAGE_FREEZE_DOWN;  //受伤冻结的上半程时间
    public float DAMAGE_FREEZE_WAIT; //受伤冻结切换的时间
    public float DAMAGE_FREEZE_UP;//受伤冻结的下半程时间

    private int recoilSteps; 
    private float recoilTimer; //后坐力计时器
    private bool recoilLarge; //是否是更大的后坐力
    private Vector2 recoilVector; //后坐力二维上的速度

    public float RECOIL_HOR_VELOCITY; //后坐力X轴上的速度
    public float RECOIL_HOR_VELOCITY_LONG; //后坐力X轴上更大的速度
    public float RECOIL_DOWN_VELOCITY; //后坐力Y轴上的速度
    public float RECOIL_HOR_STEPS; //后坐力X轴的步
    public float RECOIL_DURATION; //后坐力持续时间
    public float RECOIL_VELOCITY; //后坐力时的速度(是两个轴上都适用的)


    public float fallTimer { get; private set; }

    private float hardLandingTimer; //正在hardLanding的计时器，大于就将状态改为grounded并BackOnGround()
    private float hardLandFailSafeTimer; //进入hardLand后玩家失去输入的一段时间
    private bool hardLanded; //是否已经hardLand了

    public float HARD_LANDING_TIME; //正在hardLanding花费的时间。
    public float BIG_FALL_TIME;  //判断是否是hardLanding所需要的事件，大于它就是


    public GameObject hardLandingEffectPrefab;

    private float prevGravityScale;

    private int landingBufferSteps;
    private int LANDING_BUFFER_STEPS = 5;
    private bool fallRumble; //是否开启掉落时相机抖动

    private float floatingBufferTimer;
    private float FLOATING_CHECK_TIME = 0.18f;

    private bool startWithWallslide;
    private bool startWithJump;
    private bool startWithFullJump;
    private bool startWithDash;
    private bool startWithAttack;

    public GameObject softLandingEffectPrefab;

    public bool touchingWall; //是否接触到墙
    public bool touchingWallL; //是否接触到的墙左边
    public bool touchingWallR; //是否接触到的墙右边

    private Rigidbody2D rb2d;
    private BoxCollider2D col2d;
    private GameManager gm;
    public PlayerData playerData;
    private InputHandler inputHandler;
    public HeroControllerStates cState;
    private HeroAnimationController animCtrl;
    private HeroAudioController audioCtrl;
    private new MeshRenderer renderer;
    private InvulnerablePulse invPulse;
    private SpriteFlash spriteFlash;
    public PlayMakerFSM proxyFSM { get; private set; }

    private static HeroController _instance;
    public static HeroController instance
    {
	get
	{
            if (_instance == null)
                _instance = FindObjectOfType<HeroController>();
            if(_instance && Application.isPlaying)
	    {
                DontDestroyOnLoad(_instance.gameObject);
	    }
            return _instance;
	}
    }

    public HeroController()
    {
        ATTACK_QUEUE_STEPS = 5;
        NAIL_TERRAIN_CHECK_TIME = 0.12f;
        JUMP_QUEUE_STEPS = 2;
        JUMP_RELEASE_QUEUE_STEPS = 2;

        LANDING_BUFFER_STEPS = 5;
        FLOATING_CHECK_TIME = 0.18f;
    }

    private void Awake()
    {
        if(_instance == null)
	{
            _instance = this;
            DontDestroyOnLoad(this);
	}
        else if(this != _instance)
	{
            Destroy(gameObject);
            return;
	}
        SetupGameRefs();
    }

    private void SetupGameRefs()
    {
        if (cState == null)
            cState = new HeroControllerStates();
        rb2d = GetComponent<Rigidbody2D>();
        col2d = GetComponent<BoxCollider2D>();
        animCtrl = GetComponent<HeroAnimationController>();
        audioCtrl = GetComponent<HeroAudioController>();
        gm = GameManager.instance;
        playerData = PlayerData.instance;
        inputHandler = gm.GetComponent<InputHandler>();
	renderer = GetComponent<MeshRenderer>();
        invPulse = GetComponent<InvulnerablePulse>();
        spriteFlash = GetComponent<SpriteFlash>();
        proxyFSM = FSMUtility.LocateFSM(gameObject, "ProxyFSM");
    }

    private void Start()
    {
        heroInPosition += delegate(bool forceDirect)
        {
            isHeroInPosition = true;
        };
        playerData = PlayerData.instance;
        if (dashBurst == null)
	{
            Debug.Log("DashBurst came up null, locating manually");
            dashBurst = FSMUtility.GetFSM(transform.Find("Effects").Find("Dash Burst").gameObject);
	}
        if (spellControl == null)
        {
            Debug.Log("SpellControl came up null, locating manually");
	    spellControl = FSMUtility.LocateFSM(gameObject, "Spell Control");
        }
        if(heroInPosition != null)
	{
            heroInPosition(false);
	}
    }

    private void Update()
    {
        current_velocity = rb2d.velocity;
        FallCheck();
        FailSafeCheck();
        if (hero_state == ActorStates.running && !cState.dashing && !cState.backDashing && !controlReqlinquished)
        {
            if (cState.inWalkZone)
            {
                audioCtrl.StopSound(HeroSounds.FOOTSETP_RUN);
                audioCtrl.PlaySound(HeroSounds.FOOTSTEP_WALK);
            }
            else
            {
                audioCtrl.StopSound(HeroSounds.FOOTSTEP_WALK);
                audioCtrl.PlaySound(HeroSounds.FOOTSETP_RUN);
            }
        }
        else
        {
            audioCtrl.StopSound(HeroSounds.FOOTSETP_RUN);
            audioCtrl.StopSound(HeroSounds.FOOTSTEP_WALK);
        }
        if (hero_state == ActorStates.dash_landing)
        {
            dashLandingTimer += Time.deltaTime;
            if (dashLandingTimer > DOWN_DASH_TIME)
            {
                BackOnGround();
            }
        }
        if (hero_state == ActorStates.hard_landing)
        {
            hardLandingTimer += Time.deltaTime;
            if (hardLandingTimer > HARD_LANDING_TIME)
            {
                SetState(ActorStates.grounded);
                BackOnGround();
            }
        }
        if (hero_state == ActorStates.no_input)
        {
            if (cState.recoiling)
            {
                if (recoilTimer < RECOIL_DURATION)
                {
                    recoilTimer += Time.deltaTime;
                }
                else
                {
                    CancelDamageRecoil();
                    if ((prev_hero_state == ActorStates.idle || prev_hero_state == ActorStates.running) && !CheckTouchingGround())
                    {
                        cState.onGround = false;
                        SetState(ActorStates.airborne);
                    }
                    else
                    {
                        SetState(ActorStates.previous);
                    }

                }
            }
        }
        else if (hero_state != ActorStates.no_input)
        {
            LookForInput();
            if (cState.recoiling)
            {
                cState.recoiling = false;
                AffectedByGravity(true);
            }
            if (cState.attacking && !cState.dashing)
            {
                attack_time += Time.deltaTime;
                if (attack_time >= attackDuration)
                {
                    ResetAttacks();
                    animCtrl.StopAttack();
                }
            }

            if(hero_state == ActorStates.idle)
	    {
                if(!controlReqlinquished && !gm.isPaused)
		{
                    //TODO:
		}
                
	    }
        }
        LookForQueueInput();
        if (drainMP)
	{
            drainMP_timer += Time.deltaTime;
            while (drainMP_timer >= drainMP_time) //drainMP_time在无护符下等于0.027
            {
                MP_drained += 1f;
                drainMP_timer -= drainMP_time;
                TakeMp(1);

                if(MP_drained == focusMP_amount)
		{
                    MP_drained -= drainMP_time;
                    proxyFSM.SendEvent("HeroCtrl-FocusCompleted");

		}
	    }
	}
	if (attack_cooldown > 0f)
	{
            attack_cooldown -= Time.deltaTime;
	}
        if (dashCooldownTimer > 0f) //计时器在Update中-= Time.deltaTime
        {
            dashCooldownTimer -= Time.deltaTime;
	}

	preventCastByDialogueEndTimer -= Time.deltaTime;

	if (parryInvulnTimer > 0f)
	{
            parryInvulnTimer -= Time.deltaTime;
	}
    }

    private void FixedUpdate()
    {
        if(cState.recoilingLeft || cState.recoilingRight)
	{
            if(recoilSteps <= RECOIL_HOR_STEPS)
	    {
                recoilSteps++;
	    }
	    else
	    {
                CancelRecoilHorizonal();
	    }
	}
        if(hero_state == ActorStates.hard_landing || hero_state == ActorStates.dash_landing)
	{
            ResetMotion();
	}
        else if(hero_state == ActorStates.no_input)
	{
	    if (cState.recoiling)
	    {
                AffectedByGravity(false);
                rb2d.velocity = recoilVector;
	    }
	}
        else if (hero_state != ActorStates.no_input)
	{
            if(hero_state == ActorStates.running)
	    {
                if(move_input > 0f)
		{
		    if (CheckForBump(CollisionSide.right))
		    {
                        //rb2d.velocity = new Vector2(rb2d.velocity.x, BUMP_VELOCITY);
		    }
		}
                else if (CheckForBump(CollisionSide.left))
		{
                    //rb2d.velocity = new Vector2(rb2d.velocity.x, -BUMP_VELOCITY);
                }
            }
            if (!cState.dashing && !cState.backDashing)
            {
                Move(move_input);
                if (!cState.attacking || attack_time >= ATTACK_RECOVERY_TIME)
                {
                    if (move_input > 0f && !cState.facingRight)
                    {
                        FlipSprite();
                        CancelAttack();
                    }
                    else if (move_input < 0f && cState.facingRight)
                    {
                        FlipSprite();
                        CancelAttack();
                    }
                }
                if(cState.recoilingLeft)
		{
                    float num;
                    if (recoilLarge)
                    {
                        num = RECOIL_HOR_VELOCITY_LONG;
                    }
                    else
                    {
                        num = RECOIL_HOR_VELOCITY;
                    }
                    if(rb2d.velocity.x > -num)
		    {
                        rb2d.velocity = new Vector2(-num, rb2d.velocity.y);
		    }
		    else
		    {
                        rb2d.velocity = new Vector2(rb2d.velocity.x - num, rb2d.velocity.y);
		    }
		}
                if (cState.recoilingRight)
                {
                    float num2;
                    if(recoilLarge)
		    {
                        num2 = RECOIL_HOR_VELOCITY_LONG;
		    }
                    else
		    {
                        num2 = RECOIL_HOR_VELOCITY;
                    }
                    if (rb2d.velocity.x < num2)
                    {
                        rb2d.velocity = new Vector2(num2, rb2d.velocity.y);
                    }
                    else
                    {
                        rb2d.velocity = new Vector2(rb2d.velocity.x + num2, rb2d.velocity.y);
                    }
                }
            }
	}

	if (cState.jumping) //如果cState.jumping就Jump
        {
            Jump();
	}
	if (cState.dashing)//如果cState.dashing就Dash
        {
            Dash();
	}
        //限制速度
        if(rb2d.velocity.y < -MAX_FALL_VELOCITY && !controlReqlinquished )
	{
            rb2d.velocity = new Vector2(rb2d.velocity.x, -MAX_FALL_VELOCITY);
	}
	if (jumpQueuing)
	{
            jumpQueueSteps++;
	}

	if (dashQueuing) //跳跃队列开始
	{
            dashQueueSteps++;
	}
        if(attackQueuing)
	{
            attackQueueSteps++;
	}
        if (landingBufferSteps > 0)
        {
            landingBufferSteps--;
        }
        if (jumpReleaseQueueSteps > 0)
	{
            jumpReleaseQueueSteps--;
	}

        cState.wasOnGround = cState.onGround;
    }

    /// <summary>
    /// 小骑士移动的函数
    /// </summary>
    /// <param name="move_direction"></param>
    private void Move(float move_direction)
    {
        if (cState.onGround)
        {
            SetState(ActorStates.grounded);
        }
        if(acceptingInput)
	{
            if (cState.inWalkZone)
            {
                rb2d.velocity = new Vector2(move_direction * WALK_SPEED, rb2d.velocity.y);
                return;
            }
            rb2d.velocity = new Vector2(move_direction * RUN_SPEED, rb2d.velocity.y);
	}
    }

    private void Attack(AttackDirection attackDir)
    {
        if(Time.timeSinceLevelLoad - altAttackTime > ALT_ATTACK_RESET)
	{
            cState.altAttack = false;
	}
        cState.attacking = true;
        attackDuration = ATTACK_DURATION;

        if (attackDir == AttackDirection.normal)
        {
            if (!cState.altAttack)
            {
                slashComponent = normalSlash;
                slashFsm = normalSlashFsm;
                cState.altAttack = true;

            }
            else
            {
                slashComponent = altetnateSlash;
                slashFsm = altetnateSlashFsm;
                cState.altAttack = false;
            }
        }
        else if (attackDir == AttackDirection.upward) 
        {
            slashComponent = upSlash;
            slashFsm = upSlashFsm;
            cState.upAttacking = true;

        }
        else if (attackDir == AttackDirection.downward)
        {
            slashComponent = downSlash;
            slashFsm = downSlashFsm;
            cState.downAttacking = true;

        }

        if(attackDir == AttackDirection.normal && cState.facingRight)
	{
            slashFsm.FsmVariables.GetFsmFloat("direction").Value = 0f;
	}
        else if (attackDir == AttackDirection.normal && !cState.facingRight)
        {
            slashFsm.FsmVariables.GetFsmFloat("direction").Value = 180f;
        }
        else if (attackDir == AttackDirection.upward)
        {
            slashFsm.FsmVariables.GetFsmFloat("direction").Value = 90f;
        }
        else if (attackDir == AttackDirection.downward)
        {
            slashFsm.FsmVariables.GetFsmFloat("direction").Value = 270f;
        }
        altAttackTime = Time.timeSinceLevelLoad;
        slashComponent.StartSlash();

    }

    private void DoAttack()
    {

        cState.recoiling = false;
        attack_cooldown = ATTACK_COOLDOWN_TIME;
        if(vertical_input > Mathf.Epsilon)
	{
            Attack(AttackDirection.upward);
            StartCoroutine(CheckForTerrainThunk(AttackDirection.upward));
            return;
	}
        if(vertical_input >= -Mathf.Epsilon)
	{
            Attack(AttackDirection.normal);
            StartCoroutine(CheckForTerrainThunk(AttackDirection.normal));
            return;
        }
        if(hero_state != ActorStates.idle && hero_state != ActorStates.running)
	{
            Attack(AttackDirection.downward);
            StartCoroutine(CheckForTerrainThunk(AttackDirection.downward));
            return;
        }
        Attack(AttackDirection.normal);
        StartCoroutine(CheckForTerrainThunk(AttackDirection.normal));
    }

    private bool CanAttack()
    {
        return hero_state != ActorStates.no_input && hero_state != ActorStates.hard_landing && hero_state != ActorStates.dash_landing && attack_cooldown <= 0f && !cState.attacking && !cState.dashing && !cState.dead && !cState.hazardDeath && !controlReqlinquished;
    }

    private void CancelAttack()
    {
	if (cState.attacking)
	{
            slashComponent.CancelAttack();
            ResetAttacks();
	}
    }

    private void ResetAttacks()
    {
        cState.attacking = false;
	cState.upAttacking = false;
        cState.downAttacking = false;
        attack_time = 0f;
    }

    public void AddMPCharge(int amount)
    {
        int mpreverse = playerData.MPReverse;
        playerData.AddMPCharge(amount);

        if(playerData.MPReverse != mpreverse && gm)
	{

	}
    }

    public void SetMPCharge(int amount)
    {
        playerData.MPCharge = amount;
        //TODO:
    }


    public void SoulGain()
    {
        int num;
        if(playerData.MPCharge < playerData.maxMP)
	{
            num = 11;
	}
	else
	{
            num = 6;
	}
        int mpreverse = playerData.MPReverse;
        playerData.AddMPCharge(num);
        if(playerData.MPReverse != mpreverse)
	{

	}
    }

    public void TakeMp(int amount)
    {
        if(playerData.MPCharge > 0)
	{
            playerData.TakeMP(amount);
            if(amount > 1)
	    {

	    }
	}
    }

    public void AddHealth(int amount)
    {
        playerData.AddHealth(amount);
        proxyFSM.SendEvent("HeroCtrl-Healed");
    }

    public void TakeHealth(int amount)
    {
        playerData.TakeHealth(amount);
        proxyFSM.SendEvent("HeroCtrl-HeroDamaged");
    }

    public void MaxHealth()
    {
        proxyFSM.SendEvent("HeroCtrl-MaxHealth");
	playerData.MaxHealth();
    }


    public void StartMPDrain(float time)
    {
        Debug.LogFormat("Start MP Drain");
        drainMP = true;
        drainMP_timer = 0f;
        MP_drained = 0f;
        drainMP_time = time; //
        focusMP_amount = (float)playerData.GetInt("focusMP_amount");
    }

    public void StopMPDrain()
    {
        drainMP = false;
    }
    
    public bool CanFocus()
    {
        return !gm.isPaused && hero_state != ActorStates.no_input && !cState.dashing && !cState.backDashing && (!cState.attacking || attack_time > ATTACK_RECOVERY_TIME) && !cState.recoiling && cState.onGround && !cState.recoilFrozen && !cState.hazardDeath && CanInput();
    }

    public bool CanCast()
    {
        return !gm.isPaused && !cState.dashing && hero_state != ActorStates.no_input && !cState.backDashing && (!cState.attacking || attack_time >= ATTACK_RECOVERY_TIME) && !cState.recoiling && !cState.recoilFrozen && CanInput() && preventCastByDialogueEndTimer <= 0f;
    }

    public bool CanInput()
    {
        return acceptingInput;
    }

    public void IgnoreInput()
    {
	if (acceptingInput)
	{
            acceptingInput = false;
            ResetInput();
	}
    }

    public void AcceptInput()
    {
        acceptingInput = true;
    }

    /// <summary>
    /// 放弃控制
    /// </summary>
    public void RelinquishControl()
    {
        if(!controlReqlinquished && !cState.dead)
	{
            ResetInput();
            ResetMotion();
            IgnoreInput();
            controlReqlinquished = true;

            ResetAttacks();
            touchingWallL = false;
            touchingWallR = false;
	}
    }

    /// <summary>
    /// 重新获得控制
    /// </summary>
    public void RegainControl()
    {
        AcceptInput();
        hero_state = ActorStates.idle;
        if(controlReqlinquished && !cState.dead)
	{
            AffectedByGravity(true);
            SetStartingMotionState();
            controlReqlinquished = false;
	    if (startWithWallslide)
	    {

                cState.willHardLand = false;
                cState.touchingWall = true;

                if(transform.localScale.x< 0f)
		{
                    touchingWallR = true;
                    return;
		}
                touchingWallL = true;
	    }
	    else
	    {
		if (startWithJump)
		{
                    HeroJumpNoEffect();

                    startWithJump = false;
                    return;
		}
                if (startWithFullJump)
                {
                    HeroJump();

                    startWithFullJump = false;
                    return;
                }
                if (startWithDash)
                {
                    HeroDash();

                    startWithDash = false;
                    return;
                }
                if (startWithAttack)
                {
                    DoAttack();

                    startWithAttack = false;
                    return;
                }
                cState.touchingWall = false;
                touchingWallL = false;
                touchingWallR = false;
            }
	}
    }

    private void SetStartingMotionState()
    {
        SetStartingMotionState(false);
    }

    private void SetStartingMotionState(bool preventRunDip)
    {
        move_input = (acceptingInput || preventRunDip) ? inputHandler.inputActions.moveVector.X : 0f;
        cState.touchingWall = false;
	if (CheckTouchingGround())
	{
            cState.onGround = true;
            SetState(ActorStates.grounded);
            
	}
	else
	{
            cState.onGround = false;
            SetState(ActorStates.airborne);
	}
        animCtrl.UpdateState(hero_state);
    }


    /// <summary>
    /// 小骑士跳跃的函数
    /// </summary>
    private void Jump()
    {
	if (jump_steps <= JUMP_STEPS)
	{
	    rb2d.velocity = new Vector2(rb2d.velocity.x, JUMP_SPEED);
            jump_steps++;
            jumped_steps++;
            return;
        }
        CancelJump();
    }

    /// <summary>
    /// 取消跳跃，这个在释放跳跃键时有用
    /// </summary>
    private void CancelJump()
    {
        cState.jumping = false;
        jumpReleaseQueuing = false;
        jump_steps = 0;
    }

    /// <summary>
    /// 标注：此函数暂且不具备任何内容待后续开发
    /// </summary>
    private void BackDash()
    {

    }

    /// <summary>
    /// 冲刺时执行的函数
    /// </summary>
    private void Dash()
    {
        AffectedByGravity(false); //不受到重力影响
        ResetHardLandingTimer();
        if(dash_timer > DASH_TIME)
	{
            FinishedDashing();//大于则结束冲刺
            return;
	}
        float num;
	num = DASH_SPEED;
	if (dashingDown)
	{
            rb2d.velocity = new Vector2(0f, -num);
        }
	else if (cState.facingRight)
	{
	    if (CheckForBump(CollisionSide.right))
	    {
                //rb2d.velocity = new Vector2(num, cState.onGround ? BUMP_VELOCITY : BUMP_VELOCITY_DASH);
	    }
	    else
	    {
                rb2d.velocity = new Vector2(num, 0f); //为人物的velocity赋值DASH_SPEED
	    }
	}
        else if (CheckForBump(CollisionSide.left))
	{
            //rb2d.velocity = new Vector2(-num, cState.onGround ? BUMP_VELOCITY : BUMP_VELOCITY_DASH);
        }
	else
	{
            rb2d.velocity = new Vector2(-num, 0f);
	}
        dash_timer += Time.deltaTime;
    }

    private void HeroDash()
    {
	if (!cState.onGround)
	{
            airDashed = true;
	}

        audioCtrl.StopSound(HeroSounds.FOOTSETP_RUN);
        audioCtrl.StopSound(HeroSounds.FOOTSTEP_WALK);
        audioCtrl.PlaySound(HeroSounds.DASH);

        cState.recoiling = false;

	if (inputHandler.inputActions.right.IsPressed)
	{
            FaceRight();
	}
        else if (inputHandler.inputActions.left.IsPressed)
        {
            FaceLeft();
        }
        cState.dashing = true;
        dashQueueSteps = 0;
        HeroActions inputActions = inputHandler.inputActions;
        if(inputActions.down.IsPressed && !cState.onGround && playerData.equippedCharm_31 && !inputActions.left.IsPressed && !inputActions.right.IsPressed)
        {
            dashBurst.transform.localPosition = new Vector3(-0.07f, 3.74f, 0.01f); //生成dashBurst后设置位置和旋转角
            dashBurst.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
            dashingDown = true;
        }
	else
	{
            dashBurst.transform.localPosition = new Vector3(4.11f, -0.55f, 0.001f); //生成dashBurst后设置位置和旋转角
            dashBurst.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            dashingDown = false;
        }


        dashCooldownTimer = DASH_COOLDOWN;

        dashBurst.SendEvent("PLAY"); //发送dashBurst的FSM的事件PLAY
        dashParticlesPrefab.GetComponent<ParticleSystem>().enableEmission = true;

	if (cState.onGround)
	{
            dashEffect = Instantiate(backDashPrefab, transform.position, Quaternion.identity);
            dashEffect.transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
	}
    }

    /// <summary>
    /// 判断是否可以后撤冲刺
    /// </summary>
    /// <returns></returns>
    public bool CanBackDash()
    {
        return !cState.dashing && hero_state != ActorStates.no_input && !cState.backDashing && (!cState.attacking || attack_time >= ATTACK_RECOVERY_TIME)  &&!cState.preventBackDash && !cState.backDashCooldown && !controlReqlinquished && !cState.recoilFrozen && !cState.recoiling && cState.onGround && playerData.canBackDash;
    } 

    /// <summary>
    /// 判断是否可以冲刺
    /// </summary>
    /// <returns></returns>
    public bool CanDash()
    {
        return hero_state != ActorStates.no_input && hero_state != ActorStates.hard_landing && hero_state != ActorStates.dash_landing &&
           dashCooldownTimer <= 0f && !cState.dashing && !cState.backDashing && !cState.preventDash && (cState.onGround || !airDashed)  && playerData.canDash;
    }

    /// <summary>
    /// 结束冲刺
    /// </summary>
    private void FinishedDashing()
    {
        CancelDash();
        AffectedByGravity(true);//物体重新受到重力的影响
        animCtrl.FinishedDash(); //该播放Dash To Idle动画片段了

        if (cState.touchingWall && !cState.onGround)
	{
	    if (touchingWallL)
	    {

	    }
	    if (touchingWallR)
	    {

	    }
	}
    }

    /// <summary>
    /// 取消冲刺，将cState.dashing设置为false后动画将不再播放
    /// </summary>
    public void CancelDash()
    {

        cState.dashing = false;
        dash_timer = 0f; //重置冲刺时的计时器
        AffectedByGravity(true); //物体重新受到重力的影响

        if (dashParticlesPrefab.GetComponent<ParticleSystem>().enableEmission)
	{
            dashParticlesPrefab.GetComponent<ParticleSystem>().enableEmission = false;
        }
    }

    private void CancelBackDash()
    {
        cState.backDashing = false;
        back_dash_timer = 0f;
    }

    /// <summary>
    /// 物体是否受到重力的影响
    /// </summary>
    /// <param name="gravityApplies"></param>
    private void AffectedByGravity(bool gravityApplies)
    {
        float gravityScale = rb2d.gravityScale;
        if(rb2d.gravityScale > Mathf.Epsilon && !gravityApplies)
	{
            prevGravityScale = rb2d.gravityScale;
            rb2d.gravityScale = 0f;
            return;
	}
        if(rb2d.gravityScale <= Mathf.Epsilon && gravityApplies)
	{
            rb2d.gravityScale = prevGravityScale;
            prevGravityScale = 0f;
	}
    }

    private void FailSafeCheck()
    {
        if(hero_state == ActorStates.hard_landing)
	{
            hardLandFailSafeTimer += Time.deltaTime;
            if(hardLandFailSafeTimer > HARD_LANDING_TIME + 0.3f)
	    {
                SetState(ActorStates.grounded);
                BackOnGround();
                hardLandFailSafeTimer = 0f;
	    }
	}
	else
	{
            hardLandFailSafeTimer = 0f;
	}

        if(rb2d.velocity.y == 0f && !cState.onGround && !cState.falling && !cState.jumping && !cState.dashing && hero_state != ActorStates.hard_landing && hero_state != ActorStates.no_input)
	{
	    if (CheckTouchingGround())
	    {
                floatingBufferTimer += Time.deltaTime;
                if(floatingBufferTimer > FLOATING_CHECK_TIME)
		{
		    if (cState.recoiling)
		    {
                        CancelDamageRecoil();
		    }
                    BackOnGround();
                    floatingBufferTimer = 0f;
                    return;
		}
	    }
	    else
	    {
                floatingBufferTimer = 0f;
	    }
	}
    }

    /// <summary>
    /// 进入降落状态的检查
    /// </summary>
    private void FallCheck()
    {
        //如果y轴上的速度小于-1E-06F判断是否到地面上了
        if (rb2d.velocity.y <= -1E-06F)
	{
	    if (!CheckTouchingGround())
	    {
                cState.falling = true;
                cState.onGround = false;

                if(hero_state != ActorStates.no_input)
		{
                    SetState(ActorStates.airborne);
		}
                fallTimer += Time.deltaTime;
                if(fallTimer > BIG_FALL_TIME)
		{
		    if (!cState.willHardLand)
		    {
                        cState.willHardLand = true;
		    }
		    if (!fallRumble)
		    {
                        StartFallRumble();
		    }
		}
	    }
	}
	else
	{
            cState.falling = false;
            fallTimer = 0f;

	    if (fallRumble)
	    {
                CancelFallEffects();
	    }
	}
    }

    private void DoHardLanding()
    {
        AffectedByGravity(true);
        ResetInput();
        SetState(ActorStates.hard_landing);

        hardLanded = true;
        audioCtrl.PlaySound(HeroSounds.HARD_LANDING);
        Instantiate(hardLandingEffectPrefab, transform.position,Quaternion.identity);
    }

    public void ResetHardLandingTimer()
    {
        cState.willHardLand = false;
        hardLandingTimer = 0f;
        fallTimer = 0f;
        hardLanded = false;
    }

    private bool ShouldHardLand(Collision2D collision)
    {
        return !collision.gameObject.GetComponent<NoHardLanding>() && cState.willHardLand && hero_state != ActorStates.hard_landing;
    }

    private void ResetInput()
    {
        move_input = 0f;
        vertical_input = 0f;
    }

    private void ResetMotion()
    {
        CancelJump();
        CancelDash();
        CancelBackDash();
        CancelRecoilHorizonal();
        rb2d.velocity = Vector2.zero;

    }

    /// <summary>
    /// 翻转小骑士的localScale.x
    /// </summary>
    public void FlipSprite()
    {
        cState.facingRight = !cState.facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void FaceRight()
    {
        cState.facingRight = true;
        Vector3 localScale = transform.localScale;
        localScale.x = -1f;
        transform.localScale = localScale;
    }

    public void FaceLeft()
    {
        cState.facingRight = false;
        Vector3 localScale = transform.localScale;
        localScale.x = 1f;
        transform.localScale = localScale;
    }

    public void RecoilLeft()
    {
        if(!cState.recoilingLeft && !cState.recoilingRight && !controlReqlinquished)
	{
            CancelDash();
            recoilSteps = 0;
            cState.recoilingLeft = true;
            cState.recoilingRight = false;
            recoilLarge = false;
            rb2d.velocity = new Vector2(-RECOIL_HOR_VELOCITY, rb2d.velocity.y);
	}
    }

    public void RecoilRight()
    {
        if (!cState.recoilingLeft && !cState.recoilingRight && !controlReqlinquished)
        {
            CancelDash();
            recoilSteps = 0;
            cState.recoilingLeft = false;
            cState.recoilingRight = true;
            recoilLarge = false;
            rb2d.velocity = new Vector2(RECOIL_HOR_VELOCITY, rb2d.velocity.y);
        }
    }

    public void RecoilLeftLong()
    {
        if (!cState.recoilingLeft && !cState.recoilingRight && !controlReqlinquished)
        {
            CancelDash();
            ResetAttacks();
            recoilSteps = 0;
            cState.recoilingLeft = true;
            cState.recoilingRight = false;
            recoilLarge = true;
            rb2d.velocity = new Vector2(-RECOIL_HOR_VELOCITY_LONG, rb2d.velocity.y);
        }
    }

    public void RecoilRightLong()
    {
        if (!cState.recoilingLeft && !cState.recoilingRight && !controlReqlinquished)
        {
            CancelDash();
            ResetAttacks();
            recoilSteps = 0;
            cState.recoilingLeft = false;
            cState.recoilingRight = true;
            recoilLarge = true;
            rb2d.velocity = new Vector2(RECOIL_HOR_VELOCITY_LONG, rb2d.velocity.y);
        }
    }

    public void RecoilDown()
    {
        CancelJump();
        if(rb2d.velocity.y > RECOIL_DOWN_VELOCITY && !controlReqlinquished)
	{
            rb2d.velocity = new Vector2(rb2d.velocity.x, RECOIL_DOWN_VELOCITY);
	}
    }

    public void CancelRecoilHorizonal()
    {
        cState.recoilingLeft = false;
        cState.recoilingRight = false;
        recoilSteps = 0;
    }

    private bool CanTakeDamage()
    {
        return damageMode != DamageMode.NO_DAMAGE && !cState.invulnerable && !cState.recoiling && !cState.dead;
    }

    public void TakeDamage(GameObject go,CollisionSide damageSide,int damageAmount,int hazardType)
    {
        bool spawnDamageEffect = true;
        if (damageAmount > 0)
        {
            if (CanTakeDamage())
            {
                if (damageMode == DamageMode.HAZARD_ONLY && hazardType == 1)
                {
                    return;
                }
                if (parryInvulnTimer > 0f && hazardType == 1)
                {
                    return;
                }

                proxyFSM.SendEvent("HeroCtrl-HeroDamaged");
                CancelAttack();

                if (cState.touchingWall)
                {
                    cState.touchingWall = false;
                }
                if (cState.recoilingLeft || cState.recoilingRight)
                {
                    CancelRecoilHorizonal();
                }
                audioCtrl.PlaySound(HeroSounds.TAKE_HIT);
                if (!takeNoDamage)
                {
                    playerData.TakeHealth(damageAmount);
                }

                if (damageAmount > 0 && OnTakenDamage != null)
                {
                    OnTakenDamage();
                }
                if (playerData.health == 0)
                {
                    StartCoroutine(Die());
                    return;
                }
                if (hazardType == 2)
                {
                    Debug.LogFormat("Die From Spikes");
                    return;
                }
                if (hazardType == 3)
                {
                    Debug.LogFormat("Die From Acid");
                    return;
                }
                if (hazardType == 4)
                {
                    Debug.LogFormat("Die From Lava");
                    return;
                }
                if (hazardType == 5)
                {
                    Debug.LogFormat("Die From Pit");
                    return;
                }
                StartCoroutine(StartRecoil(damageSide, spawnDamageEffect, damageAmount));
                return;
            }
            else if (cState.invulnerable && !cState.hazardDeath)
	    {
                if(hazardType == 2)
		{
		    if (!takeNoDamage)
		    {
                        playerData.TakeHealth(damageAmount);
		    }
                    proxyFSM.SendEvent("HeroCtrl-HeroDamaged");
                    if(playerData.health == 0)
		    {
                        StartCoroutine(Die());
                        return;
		    }
                    audioCtrl.PlaySound(HeroSounds.TAKE_HIT);
                    StartCoroutine(DieFromHazard(HazardTypes.SPIKES, (go != null) ? go.transform.rotation.z : 0f));
                    return;
		}
                else if (hazardType == 3)
                {             
                    playerData.TakeHealth(damageAmount);
                    proxyFSM.SendEvent("HeroCtrl-HeroDamaged");
                    if (playerData.health == 0)
                    {
                        StartCoroutine(Die());
                        return;
                    }
                    audioCtrl.PlaySound(HeroSounds.TAKE_HIT);
                    StartCoroutine(DieFromHazard(HazardTypes.ACID, 0f));
                    return;
                }
                else if(hazardType == 4)
		{
                    Debug.LogFormat("Die From Lava");
                }
            }
	}
    }

    private IEnumerator Die()
    {
	if (OnDeath != null)
	{
            OnDeath();
	}
	if (!cState.dead)
	{
            playerData.disablePause = true;

            rb2d.velocity = Vector2.zero;
            CancelRecoilHorizonal();

            AffectedByGravity(false);
            HeroBox.inactive = true;
            rb2d.isKinematic = true;
            SetState(ActorStates.no_input);
            cState.dead = true;
            ResetMotion();
            ResetHardLandingTimer();
            renderer.enabled = false;
            gameObject.layer = 2;

            yield return null;

	}
    }

    private IEnumerator DieFromHazard(HazardTypes hazardType,float angle)
    {
	if (!cState.hazardDeath)
	{
            playerData.disablePause = true;

            SetHeroParent(null);
            SetState(ActorStates.no_input);
            cState.hazardDeath = true;
            ResetMotion();
            ResetHardLandingTimer();
            AffectedByGravity(false);
            renderer.enabled = false;
            gameObject.layer = 2;
            if(hazardType == HazardTypes.SPIKES)
	    {

	    }
            else if(hazardType == HazardTypes.ACID)
	    {

	    }
            yield return null;

	}
    }

    private IEnumerator StartRecoil(CollisionSide impactSide, bool spawnDamageEffect, int damageAmount)
    {
	if (!cState.recoiling)
	{
            playerData.disablePause = true;
            ResetMotion();
            AffectedByGravity(false);
            if(impactSide == CollisionSide.left)
	    {
		recoilVector = new Vector2(RECOIL_VELOCITY, RECOIL_VELOCITY * 0.5f);
		if (cState.facingRight)
		{
                    FlipSprite();
		}
	    }
            else if (impactSide == CollisionSide.right)
            {
                recoilVector = new Vector2(-RECOIL_VELOCITY, RECOIL_VELOCITY * 0.5f);
                if (!cState.facingRight)
                {
                    FlipSprite();
                }
            }
	    else
	    {
                recoilVector = Vector2.zero;
	    }
            SetState(ActorStates.no_input);
            cState.recoilFrozen = true;
	    if (spawnDamageEffect)
	    {
                damageEffectFSM.SendEvent("DAMAGE");
                if(damageAmount > 1)
		{

		}
	    }
            StartCoroutine(Invulnerable(INVUL_TIME));
            yield return takeDamageCoroutine = StartCoroutine(gm.FreezeMoment(DAMAGE_FREEZE_DOWN, DAMAGE_FREEZE_WAIT, DAMAGE_FREEZE_UP, 0.0001f));
            cState.recoilFrozen = false;
            cState.recoiling = true;
            playerData.disablePause = false;
        }
    }

    private IEnumerator Invulnerable(float duration)
    {
        cState.invulnerable = true;
        yield return new WaitForSeconds(DAMAGE_FREEZE_DOWN);
        invPulse.StartInvulnerablePulse();
        yield return new WaitForSeconds(duration);
        invPulse.StopInvulnerablePulse();
	cState.invulnerable = false;
        cState.recoiling = false;
    }

    public void CancelDamageRecoil()
    {
        cState.recoiling = false;
        recoilTimer = 0f;
        ResetMotion();
        AffectedByGravity(true);

    }

    private void LookForInput()
    {
        if (acceptingInput)
        {
            move_input = inputHandler.inputActions.moveVector.Vector.x; //获取X方向的键盘输入
            vertical_input = inputHandler.inputActions.moveVector.Vector.y;//获取Y方向的键盘输入
            FilterInput();//规整化


            if (inputHandler.inputActions.jump.WasReleased && jumpReleaseQueueingEnabled)
            {
                jumpReleaseQueueSteps = JUMP_RELEASE_QUEUE_STEPS;
                jumpReleaseQueuing = true;
            }
            if (!inputHandler.inputActions.jump.IsPressed)
            {
                JumpReleased();
            }
	    if (!inputHandler.inputActions.dash.IsPressed)
	    {
                if(cState.preventDash && !cState.dashCooldown)
		{
                    cState.preventDash = false;
		}
                dashQueuing = false;
	    }
	    if (!inputHandler.inputActions.attack.IsPressed)
	    {
                attackQueuing = false;
	    }
        }
    }

    private void LookForQueueInput()
    {
	if (acceptingInput)
	{
	    if (inputHandler.inputActions.jump.WasPressed)
	    {
                if (CanJump())
		{
                    HeroJump();
		}
		else
		{
                    jumpQueueSteps = 0;
                    jumpQueuing = true;

		}
	    }
	    if (inputHandler.inputActions.dash.WasPressed)
	    {
		if (CanDash())
		{
                    HeroDash();
		}
		else
		{
                    dashQueueSteps = 0;
                    dashQueuing = true;
		}
	    }
            if(inputHandler.inputActions.attack.WasPressed)
	    {
                if (CanAttack())
		{
                    DoAttack();
		}
		else
		{
                    attackQueueSteps = 0;
                    attackQueuing = true;
                }
	    }
	    if (inputHandler.inputActions.jump.IsPressed)
	    {
                if(jumpQueueSteps <= JUMP_QUEUE_STEPS && CanJump() && jumpQueuing)
		{
                    Debug.LogFormat("Execute Hero Jump");
                    HeroJump();
		}
	    }
            if(inputHandler.inputActions.dash.IsPressed && dashQueueSteps <= DASH_QUEUE_STEPS && CanDash() && dashQueuing)
	    {
                Debug.LogFormat("Start Hero Dash");
                HeroDash();
	    }
            if(inputHandler.inputActions.attack.IsPressed && attackQueueSteps <= ATTACK_QUEUE_STEPS && CanAttack() && attackQueuing)
	    {
                Debug.LogFormat("Start Do Attack");
                DoAttack();
	    }
	}
    }

    /// <summary>
    /// 可以跳跃吗
    /// </summary>
    /// <returns></returns>
    private bool CanJump()
    {
	if(hero_state == ActorStates.no_input || hero_state == ActorStates.hard_landing || hero_state == ActorStates.dash_landing || cState.dashing || cState.backDashing ||  cState.jumping)
	{
            return false;
	}
	if (cState.onGround)
	{
            return true; //如果在地面上就return true
	}
        
        return false;
    }

    /// <summary>
    /// 小骑士跳跃行为播放声音以及设置cstate.jumping
    /// </summary>
    private void HeroJump()
    {

        audioCtrl.PlaySound(HeroSounds.JUMP);

        cState.recoiling = false;
        cState.jumping = true;
        jumpQueueSteps = 0;
        jumped_steps = 0;
    }

    private void HeroJumpNoEffect()
    {

        audioCtrl.PlaySound(HeroSounds.JUMP);

        cState.jumping = true;
        jumpQueueSteps = 0;
        jumped_steps = 0;
    }

    /// <summary>
    /// 取消跳跃
    /// </summary>
    public void CancelHeroJump()
    {
	if (cState.jumping)
	{
            CancelJump();
            
            if(rb2d.velocity.y > 0f)
	    {
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
	    }
	}
    }

    private void JumpReleased()
    {
        if(rb2d.velocity.y > 0f &&jumped_steps >= JUMP_STEPS_MIN)
	{
	    if (jumpReleaseQueueingEnabled)
	    {
                if(jumpReleaseQueuing && jumpReleaseQueueSteps <= 0)
		{
                    rb2d.velocity = new Vector2(rb2d.velocity.x, 0f); //取消跳跃并且设置y轴速度为0
                    CancelJump();
		}
	    }
	    else
	    {
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
                CancelJump();
	    }
	}
        jumpQueuing = false;


    }

    /// <summary>
    /// 设置玩家的ActorState的新类型
    /// </summary>
    /// <param name="newState"></param>
    private void SetState(ActorStates newState)
    {
        if(newState == ActorStates.grounded)
	{
            if(Mathf.Abs(move_input) > Mathf.Epsilon)
	    {
                newState  = ActorStates.running;
	    }
	    else
	    {
                newState = ActorStates.idle;
            }
	}
        else if(newState == ActorStates.previous)
	{
            newState = prev_hero_state;
	}
        if(newState != hero_state)
	{
            prev_hero_state = hero_state;
            hero_state = newState;
            animCtrl.UpdateState(newState);
        }
    }

    /// <summary>
    /// 回到地面上时执行的函数
    /// </summary>
    public void BackOnGround()
    {
        if(landingBufferSteps <= 0)
	{
            landingBufferSteps = LANDING_BUFFER_STEPS;
            if(!cState.onGround && !hardLanded)
	    {
                Instantiate(softLandingEffectPrefab, transform.position,Quaternion.identity); //TODO:

            }
        }
        cState.falling = false;
        fallTimer = 0f;
        dashLandingTimer = 0f;
        cState.willHardLand = false;
        hardLandingTimer = 0f;
        hardLanded = false;
        jump_steps = 0;
        SetState(ActorStates.grounded);
	cState.onGround = true;
        airDashed = false;
    }

    /// <summary>
    /// 开启在下落时晃动
    /// </summary>
    public void StartFallRumble()
    {
        fallRumble = true;
        audioCtrl.PlaySound(HeroSounds.FALLING);
    }

    public void CancelFallEffects()
    {
        fallRumble = false;
        audioCtrl.StopSound(HeroSounds.FALLING);
    }

    /// <summary>
    /// 规整化输入
    /// </summary>
    private void FilterInput()
    {
        if (move_input > 0.3f)
        {
            move_input = 1f;
        }
        else if (move_input < -0.3f)
        {
            move_input = -1f;
        }
        else
        {
            move_input = 0f;
        }
        if (vertical_input > 0.5f)
        {
            vertical_input = 1f;
            return;
        }
        if (vertical_input < -0.5f)
        {
            vertical_input = -1f;
            return;
        }
        vertical_input = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {


        if(collision.gameObject.layer == LayerMask.NameToLayer("Terrain") && collision.gameObject.CompareTag("HeroWalkable") && CheckTouchingGround())
	{

	}
        if(hero_state != ActorStates.no_input)
	{

            if(collision.gameObject.layer == LayerMask.NameToLayer("Terrain") || collision.gameObject.CompareTag("HeroWalkable"))
	    {
                CollisionSide collisionSide = FindCollisionSide(collision);
                //如果头顶顶到了
                if (collisionSide == CollisionSide.top)
		{
		    if (cState.jumping)
		    {
                        CancelJump();

		    }


		}

                //如果底下碰到了
                if (collisionSide == CollisionSide.bottom)
		{
                    if(ShouldHardLand(collision))
		    {
                        DoHardLanding();
		    }
                    else if(collision.gameObject.GetComponent<SteepSlope>() == null && hero_state != ActorStates.hard_landing)
		    {
                        BackOnGround();
		    }
                    if(cState.dashing && dashingDown)
		    {
                        AffectedByGravity(true);
                        SetState(ActorStates.dash_landing);
                        hardLanded = true;
                        return;
		    }
		}
	    }
	}
        else if(hero_state == ActorStates.no_input)
	{

	}
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(hero_state != ActorStates.no_input && collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
	{
	    if (collision.gameObject.GetComponent<NonSlider>() == null)
	    {
		if (CheckStillTouchingWall(CollisionSide.left, false))
		{
                    cState.touchingWall = true;
                    touchingWallL = true;
                    touchingWallR = false;
		}
                else if (CheckStillTouchingWall(CollisionSide.right, false))
                {
                    cState.touchingWall = true;
                    touchingWallL = false;
                    touchingWallR = true;
                }
		else
		{
                    cState.touchingWall = false;
                    touchingWallL = false;
                    touchingWallR = false;
                }
		if (CheckTouchingGround())
		{
		    if (ShouldHardLand(collision))
		    {
                        DoHardLanding();
		    }
                    if(hero_state != ActorStates.hard_landing && hero_state != ActorStates.dash_landing && cState.falling)
		    {
                        BackOnGround();
                        return;
		    }
		}
                else if(cState.jumping || cState.falling)
		{
                    cState.onGround = false;

                    SetState(ActorStates.airborne);
                    return;
		}
            }
	    else
	    {

	    }
	}
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(touchingWallL && !CheckStillTouchingWall(CollisionSide.left, false))
	{
            cState.touchingWall = false;
            touchingWallL = false;
	}
        if (touchingWallR && !CheckStillTouchingWall(CollisionSide.left, false))
        {
            cState.touchingWall = false;
            touchingWallR = false;
        }
        if(hero_state != ActorStates.no_input && collision.gameObject.layer == LayerMask.NameToLayer("Terrain") && !CheckTouchingGround())
	{

            cState.onGround = false;

            SetState(ActorStates.airborne);
            
	}
    }

    /// <summary>
    /// 检查是否接触到地面
    /// </summary>
    /// <returns></returns>
    public bool CheckTouchingGround()
    {
        Vector2 vector = new Vector2(col2d.bounds.min.x, col2d.bounds.center.y);
        Vector2 vector2 = col2d.bounds.center;
	Vector2 vector3 = new Vector2(col2d.bounds.max.x, col2d.bounds.center.y);
        float distance = col2d.bounds.extents.y + 0.16f;
        Debug.DrawRay(vector, Vector2.down, Color.yellow);
        Debug.DrawRay(vector2, Vector2.down, Color.yellow);
        Debug.DrawRay(vector3, Vector2.down, Color.yellow);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, Vector2.down, distance, LayerMask.GetMask("Terrain"));
        RaycastHit2D raycastHit2D2 = Physics2D.Raycast(vector2, Vector2.down, distance, LayerMask.GetMask("Terrain"));
        RaycastHit2D raycastHit2D3 = Physics2D.Raycast(vector3, Vector2.down, distance, LayerMask.GetMask("Terrain"));
        return raycastHit2D.collider != null || raycastHit2D2.collider != null || raycastHit2D3.collider != null;
    }

    /// <summary>
    /// 检查是否保持着接触着墙
    /// </summary>
    /// <param name="side"></param>
    /// <param name="checkTop"></param>
    /// <returns></returns>
    private bool CheckStillTouchingWall(CollisionSide side,bool checkTop = false)
    {
        Vector2 origin = new Vector2(col2d.bounds.min.x, col2d.bounds.max.y);
        Vector2 origin2 = new Vector2(col2d.bounds.min.x, col2d.bounds.center.y);
        Vector2 origin3 = new Vector2(col2d.bounds.min.x, col2d.bounds.min.y);
        Vector2 origin4 = new Vector2(col2d.bounds.max.x, col2d.bounds.max.y);
        Vector2 origin5 = new Vector2(col2d.bounds.max.x, col2d.bounds.center.y);
        Vector2 origin6 = new Vector2(col2d.bounds.max.x, col2d.bounds.min.y);
        float distance = 0.1f;
        RaycastHit2D raycastHit2D = default(RaycastHit2D);
        RaycastHit2D raycastHit2D2 = default(RaycastHit2D);
        RaycastHit2D raycastHit2D3 = default(RaycastHit2D);
        if(side == CollisionSide.left)
	{
	    if (checkTop)
	    {
                raycastHit2D = Physics2D.Raycast(origin, Vector2.left, distance, LayerMask.GetMask("Terrain"));
	    }
            raycastHit2D2 = Physics2D.Raycast(origin2, Vector2.left, distance, LayerMask.GetMask("Terrain"));
            raycastHit2D3 = Physics2D.Raycast(origin3, Vector2.left, distance, LayerMask.GetMask("Terrain"));
        }
	else
	{
            if(side != CollisionSide.right)
	    {
                Debug.LogError("Invalid CollisionSide specified.");
                return false;
            }
            if (checkTop)
            {
                raycastHit2D = Physics2D.Raycast(origin4, Vector2.right, distance, LayerMask.GetMask("Terrain"));
            }
            raycastHit2D2 = Physics2D.Raycast(origin5, Vector2.right, distance, LayerMask.GetMask("Terrain"));
            raycastHit2D3 = Physics2D.Raycast(origin6, Vector2.right, distance, LayerMask.GetMask("Terrain"));
        }
        if(raycastHit2D2.collider != null)
	{
            bool flag = true;
	    if (raycastHit2D2.collider.isTrigger)
	    {
                flag = false;
	    }
            if(raycastHit2D2.collider.GetComponent<SteepSlope>() != null)
	    {
                flag = false;
	    }
            if (raycastHit2D2.collider.GetComponent<NonSlider>() != null)
            {
                flag = false;
            }
	    if (flag)
	    {
                return true;
	    }
        }
        if (raycastHit2D3.collider != null)
        {
            bool flag2 = true;
            if (raycastHit2D3.collider.isTrigger)
            {
                flag2 = false;
            }
            if (raycastHit2D3.collider.GetComponent<SteepSlope>() != null)
            {
                flag2 = false;
            }
            if (raycastHit2D3.collider.GetComponent<NonSlider>() != null)
            {
                flag2 = false;
            }
            if (flag2)
            {
                return true;
            }
        }
        if (checkTop && raycastHit2D.collider != null)
        {
            bool flag3 = true;
            if (raycastHit2D.collider.isTrigger)
            {
                flag3 = false;
            }
            if (raycastHit2D.collider.GetComponent<SteepSlope>() != null)
            {
                flag3 = false;
            }
            if (raycastHit2D.collider.GetComponent<NonSlider>() != null)
            {
                flag3 = false;
            }
            if (flag3)
            {
                return true;
            }
        }
        return false;
    }

    public IEnumerator CheckForTerrainThunk(AttackDirection attackDir)
    {
        bool terrainHit = false;
        float thunkTimer = NAIL_TERRAIN_CHECK_TIME;
	while (thunkTimer > 0.12f)
	{
	    if (!terrainHit)
	    {
		float num = 0.25f;
		float num2;
		if (attackDir == AttackDirection.normal)
		{
		    num2 = 2f;
		}
		else
		{
		    num2 = 1.5f;
		}
		float num3 = 1f;
		//TODO:
		num2 *= num3;
		Vector2 size = new Vector2(0.45f, 0.45f);
		Vector2 origin = new Vector2(col2d.bounds.center.x, col2d.bounds.center.y + num);
		Vector2 origin2 = new Vector2(col2d.bounds.center.x, col2d.bounds.max.y);
		Vector2 origin3 = new Vector2(col2d.bounds.center.x, col2d.bounds.min.y);
		int layerMask = 33554432; //2的25次方，也就是Layer Soft Terrain；
		RaycastHit2D raycastHit2D = default(RaycastHit2D);
		if (attackDir == AttackDirection.normal)
		{
		    if ((cState.facingRight && !cState.wallSliding) || (!cState.facingRight && !cState.wallSliding))
		    {
			raycastHit2D = Physics2D.BoxCast(origin, size, 0f, Vector2.right, num2, layerMask);
		    }
		    else
		    {
			raycastHit2D = Physics2D.BoxCast(origin, size, 0f, Vector2.right, num3, layerMask);
		    }
		}
		else if (attackDir == AttackDirection.upward)
		{
		    raycastHit2D = Physics2D.BoxCast(origin2, size, 0f, Vector2.up, num2, layerMask);
		}
		else if (attackDir == AttackDirection.downward)
		{
		    raycastHit2D = Physics2D.BoxCast(origin3, size, 0f, Vector2.down, num2, layerMask);
		}
		if (raycastHit2D.collider != null && !raycastHit2D.collider.isTrigger)
		{
		    NonThunker component = raycastHit2D.collider.GetComponent<NonThunker>();
		    bool flag = !(component != null) || !component.active;
		    if (flag)
		    {
			terrainHit = true;

			if (attackDir == AttackDirection.normal)
			{
			    if (cState.facingRight)
			    {
                                RecoilLeft();
			    }
			    else
			    {
                                RecoilRight();
			    }
			}
			else if (attackDir == AttackDirection.upward)
			{
                            RecoilDown();
			}
		    }
		}
		thunkTimer -= Time.deltaTime;
	    }
            yield return null;
	}
    }

    public bool CheckForBump(CollisionSide side)
    {
        float num = 0.025f;
        float num2 = 0.2f;
        Vector2 vector = new Vector2(col2d.bounds.min.x + num2, col2d.bounds.min.y + 0.2f);
        Vector2 vector2 = new Vector2(col2d.bounds.min.x + num2, col2d.bounds.min.y - num);
        Vector2 vector3 = new Vector2(col2d.bounds.max.x - num2, col2d.bounds.min.y + 0.2f);
        Vector2 vector4 = new Vector2(col2d.bounds.max.x - num2, col2d.bounds.min.y - num);
        float num3 = 0.32f + num2;
        RaycastHit2D raycastHit2D = default(RaycastHit2D);
        RaycastHit2D raycastHit2D2 = default(RaycastHit2D);
        if(side == CollisionSide.left)
	{
            Debug.DrawLine(vector2, vector2 + Vector2.left * num3, Color.cyan, 0.15f);
            Debug.DrawLine(vector, vector + Vector2.left * num3, Color.cyan, 0.15f);
            raycastHit2D = Physics2D.Raycast(vector2, Vector2.left, num3, LayerMask.GetMask("Terrain"));
            raycastHit2D2 = Physics2D.Raycast(vector, Vector2.left, num3, LayerMask.GetMask("Terrain"));
        }
        else if (side == CollisionSide.right)
        {
            Debug.DrawLine(vector4, vector4 + Vector2.right * num3, Color.cyan, 0.15f);
            Debug.DrawLine(vector3, vector3 + Vector2.right * num3, Color.cyan, 0.15f);
            raycastHit2D = Physics2D.Raycast(vector4, Vector2.right, num3, LayerMask.GetMask("Terrain"));
            raycastHit2D2 = Physics2D.Raycast(vector3, Vector2.right, num3, LayerMask.GetMask("Terrain"));
	}
	else
	{
            Debug.LogError("Invalid CollisionSide specified.");
        }
        if(raycastHit2D2.collider != null && raycastHit2D.collider == null)
	{
            Vector2 vector5 = raycastHit2D2.point + new Vector2((side == CollisionSide.right) ? 0.1f : -0.1f, 1f);
            RaycastHit2D raycastHit2D3 = Physics2D.Raycast(vector5, Vector2.down, 1.5f, LayerMask.GetMask("Terrain"));
            Vector2 vector6 = raycastHit2D2.point + new Vector2((side == CollisionSide.right) ? -0.1f : 0.1f, 1f);
	    RaycastHit2D raycastHit2D4 = Physics2D.Raycast(vector6, Vector2.down, 1.5f, LayerMask.GetMask("Terrain"));
            if(raycastHit2D3.collider != null)
	    {
		Debug.DrawLine(vector5, raycastHit2D3.point, Color.cyan, 0.15f);
                if (!(raycastHit2D4.collider != null))
                {
                    return true;
		}
		Debug.DrawLine(vector6, raycastHit2D4.point, Color.cyan, 0.15f);
                float num4 = raycastHit2D3.point.y - raycastHit2D4.point.y;
                if(num4 > 0f)
		{
                    return true;
                }
	    }
	}
        return false;
    }

    /// <summary>
    /// 找到碰撞点的方向也就是上下左右
    /// </summary>
    /// <param name="collision"></param>
    /// <returns></returns>
    private CollisionSide FindCollisionSide(Collision2D collision)
    {
        Vector2 normal = collision.GetSafeContact().Normal ;
        float x = normal.x;
        float y = normal.y;
        if(y >= 0.5f)
	{
            return CollisionSide.bottom; 
	}
        if (y <= -0.5f)
        {
            return CollisionSide.top;
        }
        if (x < 0)
        {
            return CollisionSide.right;
        }
        if (x > 0)
        {
            return CollisionSide.left;
        }
        Debug.LogError(string.Concat(new string[]
        {
            "ERROR: unable to determine direction of collision - contact points at (",
            normal.x.ToString(),
            ",",
            normal.y.ToString(),
            ")"
        }));
        return CollisionSide.bottom;
    }

    public void SetHeroParent(Transform newParent)
    {
        transform.parent = newParent;
        if (newParent == null)
        {
	    DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// 设置一个新的HeroControllerStates状态
    /// </summary>
    /// <param name="stateName"></param>
    /// <param name="value"></param>
    public void SetCState(string stateName, bool value)
    {
        cState.SetState(stateName, value);
    }

    /// <summary>
    /// 获取当前HeroControllerStates状态
    /// </summary>
    /// <param name="stateName"></param>
    /// <returns></returns>
    public bool GetCState(string stateName)
    {
        return cState.GetState(stateName);
    }

    public void StartAnimationControl()
    {
        animCtrl.StartControl();
    }

    public void StopAnimationControl()
    {
        animCtrl.StopControl();
    }


}

[Serializable]
public class HeroControllerStates
{
    public bool facingRight;
    public bool onGround;
    public bool wasOnGround;
    public bool attacking;
    public bool altAttack;
    public bool upAttacking;
    public bool downAttacking;
    public bool inWalkZone;
    public bool jumping;
    public bool falling;
    public bool dashing;
    public bool backDashing;
    public bool touchingWall;
    public bool wallSliding;
    public bool willHardLand;
    public bool recoilFrozen;
    public bool recoiling;
    public bool recoilingLeft;
    public bool recoilingRight;
    public bool freezeCharge;
    public bool focusing;
    public bool dead;
    public bool hazardDeath;
    public bool invulnerable;
    public bool preventDash;
    public bool preventBackDash;
    public bool dashCooldown;
    public bool backDashCooldown;
    public bool isPaused;

    public HeroControllerStates()
    {
        facingRight = false;
        onGround = false;
        wasOnGround = false;
        attacking = false;
        altAttack = false;
        upAttacking = false;
        downAttacking = false;
        inWalkZone = false;
        jumping = false;
        falling = false;
        dashing = false;
        backDashing = false;
        touchingWall = false;
        wallSliding = false;
        willHardLand = false;
        recoilFrozen = false;
        recoiling = false;
        recoilingLeft = false;
        recoilingRight = false;
        freezeCharge = false;
        focusing = false;
        dead = false;
        hazardDeath = false;
        invulnerable = false;
        preventDash = false;
        preventBackDash = false;
	dashCooldown = false;
        backDashCooldown = false;
	isPaused = false;
    }

    /// <summary>
    /// 设置一个新的状态（通常用在playmakerFSM上）
    /// </summary>
    /// <param name="stateName"></param>
    /// <param name="value"></param>
    public void SetState(string stateName, bool value)
    {
        FieldInfo field = GetType().GetField(stateName);
        if (field != null)
        {
            try
            {
                field.SetValue(HeroController.instance.cState, value);
                return;
            }
            catch (Exception ex)
            {
                string str = "Failed to set cState: ";
                Exception ex2 = ex;
                Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
                return;
            }
        }
        Debug.LogError("HeroControllerStates: Could not find bool named" + stateName + "in cState");
    }

    /// <summary>
    /// 获取一个新的状态（通常用在playmakerFSM上）
    /// </summary>
    /// <param name="stateName"></param>
    /// <returns></returns>
    public bool GetState(string stateName)
    {
        FieldInfo field = GetType().GetField(stateName);
        if (field != null)
        {
            return (bool)field.GetValue(HeroController.instance.cState);
        }
        Debug.LogError("HeroControllerStates: Could not find bool named" + stateName + "in cState");
        return false;
    }

}
