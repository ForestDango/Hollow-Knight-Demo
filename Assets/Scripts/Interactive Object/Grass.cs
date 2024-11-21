using System;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour, IHitResponder
{
    private static List<Grass> grasses;
    private static readonly int IdleStateId = Animator.StringToHash("Idle");
    private static readonly int PushStateId = Animator.StringToHash("Push");
    private static readonly int DeadStateId = Animator.StringToHash("Dead");

    [SerializeField] private float inertBackgroundThreshold;
    [SerializeField] private float inertForegroundThreshold;
    [SerializeField] private bool isInfectable;
    [SerializeField] private Color infectedColor;
    [SerializeField] private bool preventPushAnimation;

    [SerializeField] private GameObject slashImpactPrefab;
    [SerializeField] private float slashImpactRotationMin;
    [SerializeField] private float slashImpactRotationMax;
    [SerializeField] private float slashImpactScale;

    [SerializeField] private GameObject cutPrefab0;
    [SerializeField] private GameObject cutPrefab1;
    [SerializeField] public GameObject infectedCutPrefab0;
    [SerializeField] public GameObject infectedCutPrefab1;

    [SerializeField] private ParticleSystem childParticleSystem;
    [SerializeField] private float childParticleSystemDuration;
    private float childParticleSystemTimer;

    [SerializeField] private RandomAudioClipTable pushAudioClipTable;
    [SerializeField] private RandomAudioClipTable cutAudioClipTable;


    private bool isInfected;
    private bool isCut;

    private Animator anim;
    private Collider2D bodyCollider;
    private AudioSource audioSource;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
	grasses = new List<Grass>();
    }

    protected void Reset()
    {
	inertBackgroundThreshold = -1.8f;
	inertForegroundThreshold =  1.8f;
	infectedColor = new Color32(byte.MaxValue, 140, 54, byte.MaxValue);
	slashImpactRotationMin = 340f;
	slashImpactRotationMax = 380f;
	slashImpactScale = 0.6f;
	preventPushAnimation = false;
	childParticleSystemDuration = 5f;
    }

    protected void Awake()
    {
	anim = GetComponentInChildren<Animator>();
	bodyCollider = GetComponent<Collider2D>();
	audioSource = GetComponent<AudioSource>();
	grasses.Add(this);
    }

    protected void OnDestroy()
    {
	grasses.Remove(this);
    }

    protected void Start()
    {
	float z = transform.position.z;
	if(z < inertBackgroundThreshold || z > inertForegroundThreshold)
	{
	    enabled = false;
	    return;
	}
	isInfected = isInfectable && GameObject.FindGameObjectWithTag("Infected Flag") != null;
	if (isInfected)
	{
	    FSMActionReplacements.SetMaterialColor(this, infectedColor);
	}
	anim.Play(IdleStateId, 0, UnityEngine.Random.Range(0f, 1f));
    }

    protected void Update()
    {
	childParticleSystemTimer -= Time.deltaTime;
	if(childParticleSystemTimer <= 0f)
	{
	    if (childParticleSystem != null)
	    {
		childParticleSystem.Stop();
	    }
	    //enabled = false;
	}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if (preventPushAnimation)
	{
	    return;
	}
	Push(false);
    }

    public void Push(bool isAllGrass)
    {
	if (isCut)
	{
	    return;
	}
	if (!isAllGrass)
	{
	    pushAudioClipTable.PlayOneShot(audioSource);
	}
	anim.Play(PushStateId, 0, 0f);
    }

    public void Hit(HitInstance damageInstance)
    {
	if(damageInstance.DamageDealt <= 0)
	{
	    return;
	}
	if (isCut)
	{
	    return;
	}
	isCut = true;
	bodyCollider.enabled = false;
	FSMActionReplacements.Directions directions = FSMActionReplacements.CheckDirectionWithBrokenBehaviour(0f);
	GameObject gameObject = slashImpactPrefab.Spawn(transform.position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(slashImpactRotationMin, slashImpactRotationMax)));
	//GameObject gameObject = Instantiate(slashImpactPrefab, transform.position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(slashImpactRotationMin, slashImpactRotationMax)));
	gameObject.transform.localScale = new Vector3((directions == FSMActionReplacements.Directions.Left) ? (-slashImpactScale) : slashImpactScale, slashImpactScale, 1f);
	Vector3 localPosition = gameObject.transform.localPosition;
	localPosition.z = 0f;
	gameObject.transform.localPosition = localPosition;
	Quaternion rotation = Quaternion.Euler(-90f, -90f, -0.01f);
	if (isInfected)
	{
	    if(infectedCutPrefab0 != null)
	    {
		Instantiate(infectedCutPrefab0, transform.position, rotation);
	    }
	    if (infectedCutPrefab1 != null)
	    {
		Instantiate(infectedCutPrefab1, transform.position, rotation);
	    }
	}
	else
	{
	    if (cutPrefab0 != null)
	    {
		Instantiate(cutPrefab0, transform.position, rotation);
	    }
	    if (cutPrefab1 != null)
	    {
		Instantiate(cutPrefab1, transform.position, rotation);
	    }
	}
	cutAudioClipTable.PlayOneShot(audioSource);
	anim.Play(DeadStateId, 0, 0f);
	if(!isInfected && childParticleSystem != null)
	{
	    childParticleSystem.Play();
	    childParticleSystemTimer = childParticleSystemDuration;
	    enabled = false;
	    return;
	}
	enabled = false;
    }

    public enum GrassTypes
    {
	White,
	Green,
	SimpleType,
	Rag,
	ChildType
    }
}
