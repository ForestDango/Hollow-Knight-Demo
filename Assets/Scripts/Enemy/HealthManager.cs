using System;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Audio;

public class HealthManager : MonoBehaviour, IHitResponder
{
    private BoxCollider2D boxCollider;
    private IHitEffectReciever hitEffectReceiver;
    private EnemyDeathEffects enemyDeathEffects;
    private Recoil recoil;
    private tk2dSpriteAnimator animator;
    private tk2dSprite sprite;
    private DamageHero damageHero;

    [Header("Scene")]
    [SerializeField] private GameObject battleScene;
    [SerializeField] private string sendKilledToName;
    [SerializeField] private GameObject sendKilledToObject;

    [Header("Asset")]
    [SerializeField] private AudioSource audioPlayerPrefab; //声音播放器预制体
    [SerializeField] private GameObject corpseSplatPrefab;
    [SerializeField] private AudioEvent regularInvincibleAudio;
    [SerializeField] private GameObject blockHitPrefab;
    [SerializeField] private bool hasAlternateInvincibleSound;
    [SerializeField] private AudioClip alternateInvincibleSound;

    [Header("Body")]
    [SerializeField] public int hp; //血量
    [SerializeField] public int enemyType; //敌人类型
    [SerializeField] private Vector3 effectOrigin; //生效偏移量

    [Header("Invincible")]
    [SerializeField] private bool invincible;
    [SerializeField] private int invincibleFromDirection;
    [SerializeField] private bool preventInvincibleEffect;

    [Header("Geo")]
    [SerializeField] private int smallGeoDrops;
    [SerializeField] private int mediumGeoDrops;
    [SerializeField] private int largeGeoDrops;
    [SerializeField] private bool megaFlingGeo;

    [SerializeField] private GameObject smallGeoPrefab;
    [SerializeField] private GameObject mediumGeoPrefab;
    [SerializeField] private GameObject largeGeoPrefab;

    [Header("Hit")]
    [SerializeField] private bool hasAlternateHitAnimation;
    [SerializeField] private string alternateHitAnimation;

    [Header("Death")]
    [SerializeField] private AudioMixerSnapshot deathAudioSnapshot;
    [SerializeField] public bool deathReset;
    [SerializeField] public bool hasSpecialDeath;

    private PlayMakerFSM stunControlFSM;

    private bool notifiedBattleScene;
    private GameObject sendKilledTo;

    public bool IsInvincible
    {
	get
	{
	    return invincible;
	}
	set
	{
	    invincible = value;
	}
    }

    public int InvincibleFromDirection
    {
	get
	{
	    return invincibleFromDirection;
	}
	set
	{
	    invincibleFromDirection = value;
	}
    }

    public bool isDead;

    private int directionOfLastAttack; //最后一次受到攻击的方向
    private float evasionByHitRemaining; //被攻击后的剩余无敌时间
    private const string CheckPersistenceKey = "CheckPersistence";

    public delegate void DeathEvent();
    public event DeathEvent OnDeath;

    protected void Awake()
    {
	boxCollider = GetComponent<BoxCollider2D>();
	hitEffectReceiver = GetComponent<IHitEffectReciever>();
	enemyDeathEffects = GetComponent<EnemyDeathEffects>();
	recoil = GetComponent<Recoil>();
	animator = GetComponent<tk2dSpriteAnimator>();
	sprite = GetComponent<tk2dSprite>();
	damageHero = GetComponent<DamageHero>();
	foreach (PlayMakerFSM playMakerFSM in gameObject.GetComponents<PlayMakerFSM>())
	{
	    if (playMakerFSM.FsmName == "Stun Control" || playMakerFSM.FsmName == "Stun")
	    {
		stunControlFSM = playMakerFSM;
		break;
	    }
	}
    }

    protected void OnEnable()
    {
	StartCoroutine(CheckPersistenceKey);
    }

    protected void Start()
    {
	evasionByHitRemaining = -1f;
	if (!string.IsNullOrEmpty(sendKilledToName))
	{
	    sendKilledTo = GameObject.Find(sendKilledToName);
	    if (this.sendKilledTo == null)
	    {
		Debug.LogErrorFormat(this, "Failed to find GameObject '{0}' to send KILLED to.", new object[]
		{
		    this.sendKilledToName
		});
	    }
	}
	else if(sendKilledToObject != null)
	{
	    sendKilledTo = this.sendKilledToObject;
	}
    }

    protected void Update()
    {
	evasionByHitRemaining -= Time.deltaTime;
    }

    public void Hit(HitInstance hitInstance)
    {
	if (isDead)
	{
	    return;
	}
	if(evasionByHitRemaining > 0f) 
	{ 
	    return;
	}
	if(hitInstance.DamageDealt < 0f)
	{
	    return;
	}
	FSMUtility.SendEventToGameObject(hitInstance.Source, "DEALT DAMAGE", false);
	int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(transform));
	if (IsBlockingByDirection(cardinalDirection, hitInstance.AttackType))
	{
	    Invincible(hitInstance);
	    return;
	}
	TakeDamage(hitInstance);
    }

    public void Invincible(HitInstance hitInstance)
    {
	int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(transform));
	directionOfLastAttack = cardinalDirection;
	FSMUtility.SendEventToGameObject(gameObject, "BLOCKED HIT", false);
	FSMUtility.SendEventToGameObject(hitInstance.Source, "HIT LANDED", false);
	if (!(GetComponent<DontClinkGates>() != null))
	{
	    FSMUtility.SendEventToGameObject(base.gameObject, "HIT", false);
	    if (!preventInvincibleEffect)
	    {
		if (hitInstance.AttackType == AttackTypes.Nail)
		{
		    if (cardinalDirection == 0)
		    {
			HeroController.instance.RecoilLeft();
		    }
		    else if (cardinalDirection == 2)
		    {
			HeroController.instance.RecoilRight();
		    }
		}
		GameManager.instance.FreezeMoment(1);
		GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
		Vector2 v;
		Vector3 eulerAngles;
		if (boxCollider != null)
		{
		    switch (cardinalDirection)
		    {
			case 0:
			    v = new Vector2(transform.GetPositionX() + boxCollider.offset.x - boxCollider.size.x * 0.5f, hitInstance.Source.transform.GetPositionY());
			    eulerAngles = new Vector3(0f, 0f, 0f);
			    FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED HIT R", false);
			    break;
			case 1:
			    v = new Vector2(hitInstance.Source.transform.GetPositionX(), Mathf.Max(hitInstance.Source.transform.GetPositionY(), transform.GetPositionY() + boxCollider.offset.y - boxCollider.size.y * 0.5f));
			    eulerAngles = new Vector3(0f, 0f, 90f);
			    FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED HIT U", false);
			    break;
			case 2:
			    v = new Vector2(transform.GetPositionX() + boxCollider.offset.x + boxCollider.size.x * 0.5f, hitInstance.Source.transform.GetPositionY());
			    eulerAngles = new Vector3(0f, 0f, 180f);
			    FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED HIT L", false);
			    break;
			case 3:
			    v = new Vector2(hitInstance.Source.transform.GetPositionX(), Mathf.Min(hitInstance.Source.transform.GetPositionY(), transform.GetPositionY() + boxCollider.offset.y + boxCollider.size.y * 0.5f));
			    eulerAngles = new Vector3(0f, 0f, 270f);
			    FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED DOWN", false);
			    break;
			default:
			    v = transform.position;
			    eulerAngles = new Vector3(0f, 0f, 0f);
			    break;
		    }
		}
		else
		{
		    v = transform.position;
		    eulerAngles = new Vector3(0f, 0f, 0f);
		}
		GameObject gameObject = blockHitPrefab.Spawn();
		gameObject.transform.position = v;
		gameObject.transform.eulerAngles = eulerAngles;
		if (hasAlternateInvincibleSound)
		{
		    AudioSource component = GetComponent<AudioSource>();
		    if (alternateInvincibleSound != null && component != null)
		    {
			component.PlayOneShot(alternateInvincibleSound);
		    }
		}
		else
		{
		    regularInvincibleAudio.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
		}
	    }
	}
	evasionByHitRemaining = 0.15f;
    }

    public void TakeDamage(HitInstance hitInstance)
    {
	int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(transform));
	directionOfLastAttack = cardinalDirection;
	FSMUtility.SendEventToGameObject(gameObject, "HIT", false);
	FSMUtility.SendEventToGameObject(hitInstance.Source, "HIT LANDED", false);
	FSMUtility.SendEventToGameObject(gameObject, "TOOK DAMAGE", false);
	if(recoil != null)
	{
	    recoil.RecoilByDirection(cardinalDirection,hitInstance.MagnitudeMultiplier);
	}
	switch (hitInstance.AttackType)
	{
	    case AttackTypes.Nail:
		if(hitInstance.AttackType == AttackTypes.Nail && enemyType !=3 && enemyType != 6)
		{
		    HeroController.instance.SoulGain();
		}
		Vector3 position = (hitInstance.Source.transform.position + transform.position) * 0.5f + effectOrigin;
		break;
	    case AttackTypes.Generic:
		break;
	    case AttackTypes.Spell:
		break;
	}
	if(hitEffectReceiver != null)
	{
	    hitEffectReceiver.RecieveHitEffect(hitInstance.GetActualDirection(transform));
	}
	int num = Mathf.RoundToInt(hitInstance.DamageDealt * hitInstance.Multiplier);

	hp = Mathf.Max(hp - num, -50);
	if(hp > 0)
	{
	    NonFatalHit(hitInstance.IgnoreInvulnerable);
	    if (stunControlFSM)
	    {
		stunControlFSM.SendEvent("STUN DAMAGE");
		return;
	    }
	}
	else
	{
	    Die(new float?(hitInstance.GetActualDirection(transform)), hitInstance.AttackType, hitInstance.IgnoreInvulnerable);
	}
    }

    private void NonFatalHit(bool ignoreEvasion)
    {
	if (!ignoreEvasion)
	{
	    if (hasAlternateHitAnimation)
	    {
		if (animator != null)
		{
		    animator.Play(alternateHitAnimation);
		    return;
		}
	    }
	    else
	    {
		evasionByHitRemaining = 0.2f;
	    }
	}
    }

    public void Die(float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
    {
	if (isDead)
	{ 
	    return;
	}
	if (sprite)
	{
	    sprite.color = Color.white;
	}
	FSMUtility.SendEventToGameObject(gameObject, "ZERO HP", false);
	if (hasSpecialDeath)
	{
	    NonFatalHit(ignoreEvasion);
	    return;
	}
	isDead = true;
	if(damageHero != null)
	{
	    damageHero.damageDealt = 0;
	}
	if(battleScene != null && !notifiedBattleScene)
	{
	    PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(battleScene, "Battle Control");
	    if(playMakerFSM != null)
	    {
		FsmInt fsmInt = playMakerFSM.FsmVariables.GetFsmInt("Battle Enemies");
		if(fsmInt != null)
		{
		    fsmInt.Value--;
		    notifiedBattleScene = true;
		}
	    }
	}
	if (deathAudioSnapshot != null)
	{
	    deathAudioSnapshot.TransitionTo(6f);
	}
	if (sendKilledTo != null)
	{
	    FSMUtility.SendEventToGameObject(sendKilledTo, "KILLED", false);
	}
	if(attackType == AttackTypes.Splatter)
	{
	    GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
	    Debug.LogWarningFormat(this, "Instantiate!", Array.Empty<object>());
	    Instantiate<GameObject>(corpseSplatPrefab, transform.position + effectOrigin, Quaternion.identity);
	    if (enemyDeathEffects)
	    {
		enemyDeathEffects.EmitSound();
	    }
	    Destroy(gameObject);
	    return;
	}
	if(attackType != AttackTypes.RuinsWater)
	{
	    float angleMin = megaFlingGeo ? 65 : 80;
	    float angleMax = megaFlingGeo ? 115 : 100;
	    float speedMin = megaFlingGeo ? 30 : 15;
	    float speedMax = megaFlingGeo ? 45 : 30;
	    int num = smallGeoDrops;
	    int num2 = mediumGeoDrops;
	    int num3 = largeGeoDrops;
	    bool flag = false;
	    if(GameManager.instance.playerData.equippedCharm_24 && !GameManager.instance.playerData.brokenCharm_24)
	    {
		num += Mathf.CeilToInt(num * 0.2f);
		num2 += Mathf.CeilToInt(num2 * 0.2f);
		num3 += Mathf.CeilToInt(num3 * 0.2f);
		flag = true;
	    }
	    GameObject[] gameObjects = FlingUtils.SpawnAndFling(new FlingUtils.Config
	    {
		Prefab = smallGeoPrefab,
		AmountMin = num,
		AmountMax = num,
		SpeedMin = speedMin,
		SpeedMax = speedMax,
		AngleMin = angleMin,
		AngleMax = angleMax
	    }, transform, effectOrigin);
	    if (flag)
	    {
		SetGeoFlashing(gameObjects, smallGeoDrops);
	    }
	    gameObjects = FlingUtils.SpawnAndFling(new FlingUtils.Config
	    {
		Prefab = mediumGeoPrefab,
		AmountMin = num2,
		AmountMax = num2,
		SpeedMin = speedMin,
		SpeedMax = speedMax,
		AngleMin = angleMin,
		AngleMax = angleMax
	    }, transform, effectOrigin);
	    if (flag)
	    {
		SetGeoFlashing(gameObjects, mediumGeoDrops);
	    }
	    gameObjects = FlingUtils.SpawnAndFling(new FlingUtils.Config
	    {
		Prefab = largeGeoPrefab,
		AmountMin = num3,
		AmountMax = num3,
		SpeedMin = speedMin,
		SpeedMax = speedMax,
		AngleMin = angleMin,
		AngleMax = angleMax
	    }, transform, effectOrigin);
	    if (flag)
	    {
		SetGeoFlashing(gameObjects, largeGeoDrops);
	    }
	}
	if (enemyDeathEffects != null)
	{
	    if (attackType == AttackTypes.Generic)
	    {
		enemyDeathEffects.doKillFreeze = false;
	    }
	    enemyDeathEffects.RecieveDeathEvent(attackDirection, deathReset, attackType == AttackTypes.Spell, false);
	}
	SendDeathEvent();
	Destroy(gameObject); //TODO:
    }

    private void SetGeoFlashing(GameObject[] gameObjects, int originalAmount)
    {
	for (int i = gameObjects.Length - 1; i >= originalAmount; i--)
	{
	    GeoControl component = gameObjects[i].GetComponent<GeoControl>();
	    if (component)
	    {
		component.SetFlashing();
	    }
	}
    }

    public void SetIsDead(bool set)
    {
	isDead = set;
    }

    public void SendDeathEvent()
    {
	if (OnDeath != null)
	{
	    OnDeath();
	}
    }

    public bool IsBlockingByDirection(int cardinalDirection,AttackTypes attackType)
    {
	if(attackType == AttackTypes.Spell && gameObject.CompareTag("Spell Vulnerable"))
	{
	    return false;
	}
	if (!invincible)
	{
	    return false;
	}
	if(invincibleFromDirection == 0)
	{
	    return true;
	}
	switch (cardinalDirection)
	{
	    case 0:
	    {
		    Debug.LogFormat("Invincible from card0");
		int num = invincibleFromDirection;
		if (num <= 5)
		{
		    if (num != 1 && num != 5)
		    {
			return false;
		    }
		 }
		else if (num != 8 && num != 10)
		{
		    return false;
		}
		return true;
	    }   
	    case 1:
	    {
		    Debug.LogFormat("Invincible from card1");
		    int num = invincibleFromDirection;
		return num == 2 || num - 5 <= 4;
	    }
	    case 2:
		{
		    Debug.LogFormat("Invincible from card2");
		    int num = invincibleFromDirection;
		    if (num <= 6)
		    {
			if (num != 3 && num != 6)
			{
			    return false;
			}
		    }
		    else if (num != 9 && num != 11)
		    {
			return false;
		    }
		    return true;
		}
	    case 3:
		{
		    Debug.LogFormat("Invincible from card3");
		    int num = invincibleFromDirection;
		    return num == 4 || num - 7 <= 4;
		}
	    default:
		return false;
	}

    }

    protected IEnumerator CheckPersistence()
    {
	yield return null;
	if (isDead)
	{
	    gameObject.SetActive(false);
	}
	yield break;
    }

    public void SetBattleScene(GameObject newBattleScene)
    {
	battleScene = newBattleScene;
    }

    public void SetPreventInvincibleEffect(bool set)
    {
	preventInvincibleEffect = set;
    }

    public int GetAttackDirection()
    {
	return directionOfLastAttack;
    }

}
