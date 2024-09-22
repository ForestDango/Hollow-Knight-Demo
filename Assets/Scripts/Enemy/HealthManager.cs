using System;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Audio;

public class HealthManager : MonoBehaviour, IHitResponder
{
    private BoxCollider2D boxCollider;
    private IHitEffectReciever hitEffectReceiver;
    private tk2dSpriteAnimator animator;
    private tk2dSprite sprite;

    [Header("Asset")]
    [SerializeField] private AudioSource audioPlayerPrefab;

    [Header("Body")]
    [SerializeField] public int hp;
    [SerializeField] public int enemyType;
    [SerializeField] private Vector3 effectOrigin;

    public bool isDead;

    private int directionOfLastAttack; //���һ���ܵ������ķ���
    private float evasionByHitRemaining; //ʣ�๥���µ��ӱ�ʱ��
    private const string CheckPersistenceKey = "CheckPersistence";

    public delegate void DeathEvent();
    public event DeathEvent OnDeath;

    protected void Awake()
    {
	boxCollider = GetComponent<BoxCollider2D>();
	hitEffectReceiver = GetComponent<IHitEffectReciever>();
	animator = GetComponent<tk2dSpriteAnimator>();
	sprite = GetComponent<tk2dSprite>();
    }

    protected void OnEnable()
    {
	StartCoroutine(CheckPersistenceKey);
    }

    protected void Start()
    {
	evasionByHitRemaining = -1f;
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

    private void Invincible(HitInstance hitInstance)
    {
	int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(transform));
	directionOfLastAttack = cardinalDirection;
	FSMUtility.SendEventToGameObject(gameObject, "BLOCKED HIT", false);
	FSMUtility.SendEventToGameObject(hitInstance.Source, "HIT LANDED", false);
	if (!(GetComponent<DontClinkGates>() != null))
	{
	    FSMUtility.SendEventToGameObject(gameObject, "HIT", false);

	    if(hitInstance.AttackType == AttackTypes.Nail)
	    {
		if(cardinalDirection == 0)
		{

		}
		else if(cardinalDirection == 2)
		{

		}
	    }

	    Vector2 v;
	    Vector3 eulerAngles;
	    if (boxCollider != null)
	    {
		switch (cardinalDirection)
		{
		    case 0:
			v = new Vector2(transform.GetPositionX() + boxCollider.offset.x - boxCollider.size.x * 0.5f, hitInstance.Source.transform.GetPositionY());
			eulerAngles = new Vector3(0f, 0f, 0f);
			break;
		    case 1:
			v = new Vector2(hitInstance.Source.transform.GetPositionX(), Mathf.Max(hitInstance.Source.transform.GetPositionY(), transform.GetPositionY() + boxCollider.offset.y - boxCollider.size.y * 0.5f));
			eulerAngles = new Vector3(0f, 0f, 90f);
			break;
		    case 2:
			v = new Vector2(transform.GetPositionX() + boxCollider.offset.x + boxCollider.size.x * 0.5f, hitInstance.Source.transform.GetPositionY());
			eulerAngles = new Vector3(0f, 0f, 180f);
			break;
		    case 3:
			v = new Vector2(hitInstance.Source.transform.GetPositionX(), Mathf.Min(hitInstance.Source.transform.GetPositionY(), transform.GetPositionY() + boxCollider.offset.y + boxCollider.size.y * 0.5f));
			eulerAngles = new Vector3(0f, 0f, 270f);
			break;
		    default:
			break;
		}
	    }
	    else
	    {
		v = transform.position;
		eulerAngles = new Vector3(0f, 0f, 0f);
	    }
	}
	evasionByHitRemaining = 0.15f;
    }

    private void TakeDamage(HitInstance hitInstance)
    {
	Debug.LogFormat("Enemy Take Damage");
	int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(transform));
	directionOfLastAttack = cardinalDirection;
	FSMUtility.SendEventToGameObject(gameObject, "HIT", false);
	FSMUtility.SendEventToGameObject(hitInstance.Source, "HIT LANDED", false);
	FSMUtility.SendEventToGameObject(gameObject, "TOOK DAMAGE", false);
	switch (hitInstance.AttackType)
	{
	    case AttackTypes.Nail:
		if(hitInstance.AttackType == AttackTypes.Nail && enemyType !=3 && enemyType != 6)
		{

		}
		Vector3 position = (hitInstance.Source.transform.position + transform.position) * 0.5f + effectOrigin;
		break;
	    case AttackTypes.Generic:
		break;
	    default:
		break;
	}
	if(hitEffectReceiver != null)
	{
	    hitEffectReceiver.ReceiverHitEffect(hitInstance.GetActualDirection(transform));
	}
	int num = Mathf.RoundToInt((float)hitInstance.DamageDealt * hitInstance.Multiplier);

	hp = Mathf.Max(hp - num, -50);
	if(hp > 0)
	{

	}
	else
	{
	    Die(new float?(hitInstance.GetActualDirection(transform)), hitInstance.AttackType, hitInstance.IgnoreInvulnerable);
	}
    }

    public void Die(float? v, AttackTypes attackType, bool ignoreInvulnerable)
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
	isDead = true;
	SendDeathEvent();
	Destroy(gameObject); //TODO:
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

	switch (cardinalDirection)
	{

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

}
