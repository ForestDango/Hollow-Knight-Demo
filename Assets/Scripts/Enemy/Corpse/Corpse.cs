using System;
using System.Collections;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    private States state;

    protected MeshRenderer meshRenderer;
    protected tk2dSprite sprite;
    protected tk2dSpriteAnimator spriteAnimator;
    protected SpriteFlash spriteFlash;
    protected Rigidbody2D body;
    protected Collider2D bodyCollider;
    [SerializeField] protected ParticleSystem corpseFlame;
    [SerializeField] protected ParticleSystem corpseSteam;
    [SerializeField] protected GameObject landEffects;
    [SerializeField] protected AudioSource audioPlayerPrefab;
    [SerializeField] protected GameObject deathWaveInfectedPrefab;
    [SerializeField] protected GameObject spatterOrangePrefab;

    [SerializeField] private AudioEvent startAudio;

    [SerializeField] private bool resetRotaion;
    [SerializeField] private bool massless;
    [SerializeField] private bool instantChunker;
    [SerializeField] private bool breaker;

    private bool noSteam;
    protected bool spellBurn;
    protected bool hitAcid;

    private float landEffectsDelayRemaining;

    private void Awake()
    {
	meshRenderer = GetComponent<MeshRenderer>();
	sprite = GetComponent<tk2dSprite>();
	spriteAnimator = GetComponent<tk2dSpriteAnimator>();
	spriteFlash = GetComponent<SpriteFlash>();
	body = GetComponent<Rigidbody2D>();
	bodyCollider = GetComponent<Collider2D>();
    }

    public void Setup(bool noSteam, bool spellBurn)
    {
	this.noSteam = noSteam;
	this.spellBurn = spellBurn;
    }

    protected virtual void Start()
    {
	startAudio.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	if (resetRotaion)
	{
	    transform.SetRotation2D(0f);
	}
	if(noSteam && corpseSteam != null)
	{
	    corpseSteam.gameObject.SetActive(false);
	}
	if (spellBurn)
	{
	    if(sprite != null)
	    {
		sprite.color = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);
	    }
	    if(corpseFlame != null)
	    {
		corpseFlame.Play();
	    }
	}
	if (massless)
	{
	    state = States.DeathAnimation;
	}
	else
	{
	    state = States.InAir;
	    if(spriteAnimator != null)
	    {
		tk2dSpriteAnimationClip clipByName = spriteAnimator.GetClipByName("Death Air");
		if(clipByName != null)
		{
		    spriteAnimator.Play(clipByName);
		}
	    }
	}
	if (instantChunker && !breaker)
	{
	    Land();
	}
	StartCoroutine(DisableFlame());
    }

    protected void Update()
    {
	if(state == States.DeathAnimation)
	{
	    if(spriteAnimator == null || !spriteAnimator.Playing)
	    {
		Complete(true, true);
		return;
	    }
	}
	else if(state == States.InAir)
	{
	    if (transform.position.y < -10f)
	    {
		Complete(true, true);
		return;
	    }
	}
	else if(state == States.PendingLandEffects)
	{
	    landEffectsDelayRemaining -= Time.deltaTime;
	    if(landEffectsDelayRemaining <= 0f)
	    {
		Complete(false,false);
	    }
	}
    }

    private void Complete(bool detachChildren, bool destroyMe)
    {
	state = States.Complete;
	enabled = false;
	if (corpseSteam != null)
	{
	    corpseSteam.Stop();
	}
	if (corpseFlame != null)
	{
	    corpseFlame.Stop();
	}
	if (detachChildren)
	{
	    transform.DetachChildren();
	}
	if (destroyMe)
	{
	    Destroy(gameObject);
	}
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
	OnCollision(collision);
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
	OnCollision(collision);
    }

    private void OnCollision(Collision2D collision)
    {
	if(state == States.InAir)
	{
	    Sweep sweep = new Sweep(bodyCollider, 3, 3, 0.1f);
	    float num;
	    if(sweep.Check(transform.position,0.08f,LayerMask.GetMask("Terrain"),out num))
	    {
		Land();
	    }
	}
    }

    private void Land()
    {
	if (breaker)
	{

	}
	else
	{
	    if(spriteAnimator != null && !hitAcid)
	    {
		tk2dSpriteAnimationClip clipByName = spriteAnimator.GetClipByName("Death Land");
		if(clipByName != null)
		{
		    spriteAnimator.Play(clipByName);
		}
	    }
	    landEffectsDelayRemaining = 1f;
	    if(landEffects != null)
	    {
		landEffects.SetActive(true);
	    }
	    state = States.PendingLandEffects;
	    if (!hitAcid)
	    {
		LandEffects();
	    }
	}
    }

    protected virtual void LandEffects()
    {
	
    }

    private IEnumerator DisableFlame()
    {
	yield return new WaitForSeconds(5f);
	if (corpseFlame)
	{
	    corpseFlame.Stop();
	}
	yield break;
    }

    private enum States
    {
	NotStarted,
	InAir,
	DeathAnimation,
	Complete,
	PendingLandEffects
    }
}
