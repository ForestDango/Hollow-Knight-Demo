using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour,IHitResponder
{
    private Collider2D bodyCollider;

    [Tooltip("Renderer which presents the undestroyed object.")]
    [SerializeField] private Renderer wholeRenderer;

    [Tooltip("List of child game objects which also represent the whole object.")]
    [SerializeField] public  GameObject[] wholeParts;

    [Tooltip("List of child game objects which represent remnants that remain static after destruction.")]
    [SerializeField] private GameObject[] remnantParts;

    [SerializeField] private List<GameObject> debrisParts;

    [SerializeField] private float angleOffset = -60f;

    [Tooltip("Breakables behind this threshold are inert.")]
    [SerializeField] private float inertBackgroundThreshold;

    [Tooltip("Breakables in front of this threshold are inert.")]
    [SerializeField] private float inertForegroundThreshold;

    [Tooltip("Breakable effects are spawned at this offset.")]
    [SerializeField] private Vector3 effectOffset;

    [Tooltip("Prefab to spawn for audio.")]
    [SerializeField] private AudioSource audioSourcePrefab;

    [Tooltip("Table of audio clips to play upon break.")]
    [SerializeField] private AudioEvent breakAudioEvent;

    [Tooltip("Table of audio clips to play upon break.")]
    [SerializeField] private RandomAudioClipTable breakAudioClipTable;

    [Tooltip("Prefab to spawn when hit from a non-down angle.")]
    [SerializeField] private Transform dustHitRegularPrefab;

    [Tooltip("Prefab to spawn when hit from a down angle.")]
    [SerializeField] private Transform dustHitDownPrefab;

    [Tooltip("Prefab to spawn when hit from a down angle.")]
    [SerializeField] private float flingSpeedMin;
    [Tooltip("Prefab to spawn when hit from a down angle.")]
    [SerializeField] private float flingSpeedMax;
    [Tooltip("Strike effect prefab to spawn.")]
    [SerializeField] private Transform strikeEffectPrefab;

    [Tooltip("Nail hit prefab to spawn.")]
    [SerializeField] private Transform nailHitEffectPrefab;

    [Tooltip("Spell hit effect prefab to spawn.")]
    [SerializeField] private Transform spellHitEffectPrefab;

    [Tooltip("Object to send HIT event to.")]
    [SerializeField] private GameObject hitEventReciever;

    [Tooltip("Forward break effect to sibling FSMs.")]
    [SerializeField] private bool forwardBreakEvent;

    [Space]
    public Probability.ProbabilityGameObject[] containingParticles;
    public FlingObject[] flingObjectRegister;

    private bool isBroken;

    private void Awake()
    {
	bodyCollider = GetComponent<Collider2D>();
    }

    protected void Reset()
    {
	inertBackgroundThreshold = 1f;
	inertForegroundThreshold = -1f;
	effectOffset = new Vector3(0f, 0.5f, 0f);
	flingSpeedMin = 10f;
	flingSpeedMax = 17f;
    }

    protected void Start()
    {
	CreateAdditionalDebrisParts(debrisParts);
	float z = transform.position.z;
	if(z > inertBackgroundThreshold || z < inertForegroundThreshold)
	{
	    BoxCollider2D component = GetComponent<BoxCollider2D>();
	    if(component != null)
	    {
		component.enabled = false;
	    }
	    Destroy(this);
	    return;
	}
	for (int i = 0; i < remnantParts.Length; i++)
	{
	    GameObject gameObject = remnantParts[i];
	    if(gameObject != null && gameObject.activeSelf)
	    {
		gameObject.SetActive(false);
	    }
	}
	angleOffset *= Mathf.Sign(transform.localScale.x);
    }

    protected virtual void CreateAdditionalDebrisParts(List<GameObject> debrisParts)
    {
    }

    public void Hit(HitInstance damageInstance)
    {
	if (isBroken)
	{
	    return;
	}
	Debug.LogFormat("Breakable Take Hit");
	float impactAngle = damageInstance.Direction;
	float num = damageInstance.MagnitudeMultiplier;
	if(damageInstance.AttackType == AttackTypes.Spell)
	{
	    Instantiate(spellHitEffectPrefab, base.transform.position, Quaternion.identity).SetPositionZ(0.0031f);
	}
	else
	{
	    if (damageInstance.AttackType != AttackTypes.Nail && damageInstance.AttackType != AttackTypes.Generic)
	    {
		impactAngle = 90f;
		num = 1f;
	    }
	    Instantiate(strikeEffectPrefab, base.transform.position,Quaternion.identity);
	    Vector3 position = (damageInstance.Source.transform.position + base.transform.position) * 0.5f;
	    SpawnNailHitEffect(nailHitEffectPrefab, position, impactAngle);
	}
	int cardinalDirection = DirectionUtils.GetCardinalDirection(damageInstance.Direction);
	Transform transform = dustHitRegularPrefab;
	float flingAngleMin;
	float flingAngleMax;
	Vector3 euler;
	if (cardinalDirection == 2)
	{
	    angleOffset *= -1f;
	    flingAngleMin = 120f;
	    flingAngleMax = 160f;
	    euler = new Vector3(180f, 90f, 270f);
	}
	else if (cardinalDirection == 0)
	{
	    flingAngleMin = 30f;
	    flingAngleMax = 70f;
	    euler = new Vector3(0f, 90f, 270f);
	}
	else if (cardinalDirection == 1)
	{
	    angleOffset = 0f;
	    flingAngleMin = 70f;
	    flingAngleMax = 110f;
	    num *= 1.5f;
	    euler = new Vector3(270f, 90f, 270f);
	}
	else
	{
	    angleOffset = 0f;
	    flingAngleMin = 160f;
	    flingAngleMax = 380f;
	    transform = dustHitDownPrefab;
	    euler = new Vector3(-72.5f, -180f, -180f);
	}
	if(transform != null)
	{
	    Instantiate(transform, transform.position + effectOffset, Quaternion.Euler(euler));
	}
	Break(flingAngleMin, flingAngleMax, num);
    }

    private static Transform SpawnNailHitEffect(Transform nailHitEffectPrefab, Vector3 position, float impactAngle)
    {
	if (nailHitEffectPrefab == null)
	    return null;
	int cardinalDirection = DirectionUtils.GetCardinalDirection(impactAngle);
	float y = 1.5f;
	float minInclusive;
	float maxInclusive;
	if (cardinalDirection == 3)
	{
	    minInclusive = 270f;
	    maxInclusive = 290f;
	}
	else if (cardinalDirection == 1)
	{
	    minInclusive = 70f;
	    maxInclusive = 110f;
	}
	else
	{
	    minInclusive = 340f;
	    maxInclusive = 380f;
	}
	float x = (cardinalDirection == 2) ? -1.5f : 1.5f;
	Transform transform = Instantiate(nailHitEffectPrefab,position,Quaternion.identity);
	Vector3 eulerAngles = transform.eulerAngles;
	eulerAngles.z = Random.Range(minInclusive, maxInclusive);
	transform.eulerAngles = eulerAngles;
	Vector3 localScale = transform.localScale;
	localScale.x = x;
	localScale.y = y;
	transform.localScale = localScale;
	return transform;
    }

    public void Break(float flingAngleMin, float flingAngleMax, float impactMultiplier)
    {
	if (isBroken)
	    return;
	SetStaticPartsActivation(true);
	for (int i = 0; i < debrisParts.Count; i++)
	{
	    GameObject gameObject = debrisParts[i];
	    if (gameObject == null)
	    {
		Debug.LogErrorFormat(this, "Unassigned debris part in {0}", new object[]
		{
		    this
		});
	    }
	    else
	    {
		gameObject.SetActive(true);
		gameObject.transform.SetRotationZ(gameObject.transform.localEulerAngles.z + angleOffset);
		Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
		if (component != null)
		{
		    float num = Random.Range(flingAngleMin, flingAngleMax);
		    Vector2 a = new Vector2(Mathf.Cos(num * 0.017453292f), Mathf.Sin(num * 0.017453292f));
		    float d = Random.Range(flingSpeedMin, flingSpeedMax) * impactMultiplier;
		    component.velocity = a * d;
		}
	    }
	}
	if (containingParticles.Length != 0)
	{
	    GameObject gameObject2 = Probability.GetRandomGameObjectByProbability(containingParticles);
	    if (gameObject2)
	    {
		if (gameObject2.transform.parent != transform)
		{
		    FlingObject flingObject = null;
		    foreach (FlingObject flingObject2 in flingObjectRegister)
		    {
			if (flingObject2.referenceObject == gameObject2)
			{
			    flingObject = flingObject2;
			    break;
			}
		    }
		    if (flingObject != null)
		    {
			flingObject.Fling(transform.position);
		    }
		    else
		    {
			gameObject2 = Instantiate(gameObject2, transform.position, Quaternion.identity);
		    }
		}
		gameObject2.SetActive(true);
	    }
	}
	breakAudioEvent.SpawnAndPlayOneShot(audioSourcePrefab, transform.position);
	breakAudioClipTable.SpawnAndPlayOneShot(audioSourcePrefab, transform.position);
	if (hitEventReciever != null)
	{
	    FSMUtility.SendEventToGameObject(hitEventReciever, "HIT", false);
	}
	if (forwardBreakEvent)
	{
	    FSMUtility.SendEventToGameObject(gameObject, "BREAK", false);
	}
	GameObject gameObject3 = GameObject.FindGameObjectWithTag("CameraParent");
	if (gameObject3 != null)
	{
	    PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(gameObject3, "CameraShake");
	    if(playMakerFSM != null)
	    {
		playMakerFSM.SendEvent("EnemyKillShake");
	    }
	}
	wholeRenderer.enabled = false;
	bodyCollider.enabled = false;
	isBroken = true;
    }

    private void SetStaticPartsActivation(bool broken) //发送来的是true
    {
	if (wholeRenderer != null)
	{
	    wholeRenderer.enabled = !broken;
	}
	for (int i = 0; i < wholeParts.Length; i++)
	{
	    GameObject gameObject = wholeParts[i];
	    if (gameObject == null)
	    {
		Debug.LogErrorFormat(this, "Unassigned whole part in {0}", new object[]
		{
		    this
		});
	    }
	    else
	    {
		gameObject.SetActive(!broken);
	    }
	}
	for (int j = 0; j < remnantParts.Length; j++)
	{
	    GameObject gameObject2 = remnantParts[j];
	    if (gameObject2 == null)
	    {
		Debug.LogErrorFormat(this, "Unassigned remnant part in {0}", new object[]
		{
		    this
		});
	    }
	    else
	    {
		gameObject2.SetActive(broken);
	    }
	}
	if (hitEventReciever != null)
	{
	    FSMUtility.SendEventToGameObject(hitEventReciever, "HIT", false);
	}
	if (bodyCollider)
	{
	    bodyCollider.enabled = !broken;
	}
    }

    [System.Serializable]
    public class FlingObject
    {
	public GameObject referenceObject;
	[Space]
	public int spawnMin;
	public int spawnMax;
	public float speedMin;
	public float speedMax;
	public float angleMin;
	public float angleMax;
	public Vector2 originVariation;

	public FlingObject()
	{
	    spawnMin = 25;
	    spawnMax = 35;
	    speedMin = 9f;
	    speedMax = 20f;
	    angleMin = 20f;
	    angleMax = 160f;
	    originVariation = new Vector2(0.5f, 0.5f);
	}

	public void Fling(Vector3 origin)
	{
	    if (!referenceObject)
	    {
		return;
	    }
	    int num = Random.Range(spawnMin, spawnMax + 1);
	    for (int i = 0; i < num; i++)
	    {
		GameObject gameObject = referenceObject.Spawn();
		if (gameObject)
		{
		    gameObject.transform.position = origin + new Vector3(Random.Range(-originVariation.x, originVariation.x), Random.Range(-originVariation.y, originVariation.y), 0f);
		    float num2 = Random.Range(speedMin, speedMax);
		    float num3 = Random.Range(angleMin, angleMax);
		    float x = num2 * Mathf.Cos(num3 * 0.017453292f);
		    float y = num2 * Mathf.Sin(num3 * 0.017453292f);
		    Vector2 force = new Vector2(x, y);
		    Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
		    if (component)
		    {
			component.AddForce(force, ForceMode2D.Impulse);
		    }
		}
	    }
	}

    }
}
