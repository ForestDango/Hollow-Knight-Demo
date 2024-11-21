using System.Collections;
using UnityEngine;

public class SoulOrb : MonoBehaviour
{
    public RandomAudioClipTable soulOrbCollectSounds;
    public ParticleSystem getParticles;
    public bool awardSoul = true;
    public bool dontRecycle;

    private SpriteRenderer sprite;
    private TrailRenderer trail;
    private Rigidbody2D body;
    private AudioSource source;
    private Coroutine zoomRoutine;

    public float stretchFactor = 2f;
    public float stretchMinY = 1f;
    public float stretchMaxX = 2f;
    public float scaleModifier;
    public float scaleModifierMin = 1f;
    public float scaleModifierMax = 2f;

    private Transform target;

    private float speed;
    private float acceleration;

    private void Awake()
    {
	sprite = GetComponent<SpriteRenderer>();
	trail = GetComponent<TrailRenderer>();
	body = GetComponent<Rigidbody2D>();
	source = GetComponent<AudioSource>();
	target = HeroController.instance.transform;
    }

    private void Start()
    {
	transform.SetPositionZ(Random.Range(-0.001f, -0.1f));
    }

    private void OnEnable()
    {
	if (sprite)
	{
	    sprite.enabled = true;
	}
	if (trail)
	{
	    trail.enabled = true;
	}
	if (body)
	{
	    body.isKinematic = false;
	}
	if (zoomRoutine != null)
	{
	    StopCoroutine(zoomRoutine);
	}
	zoomRoutine = null;
	GameManager.instance.UnloadingLevel += SceneLoading;
	scaleModifier = Random.Range(scaleModifierMin, scaleModifierMax);
    }

    private void OnDisable()
    {
	if (sprite)
	{
	    sprite.enabled = false;
	}
	if (trail)
	{
	    trail.enabled = false;
	}
	if (body)
	{
	    body.isKinematic = false;
	}
	GameManager.instance.UnloadingLevel -= SceneLoading;
    }

    private void Update()
    {
	if(body && body.velocity.magnitude < 2.5f && zoomRoutine == null)
	{
	    zoomRoutine = StartCoroutine(Zoom(true));
	}
	FaceAngle();
	ProjectileSquash();
    }

    private void SceneLoading()
    {
	if (zoomRoutine != null)
	{
	    StopCoroutine(zoomRoutine);
	}
	zoomRoutine = StartCoroutine(Zoom(false));
    }

    private IEnumerator Zoom(bool doZoom = true)
    {
	if (doZoom)
	{
	    speed = 15f;
	    while (target)
	    {
		speed += acceleration;
		speed = Mathf.Clamp(speed, 0f, 30f);
		acceleration += 0.07f;
		FireAtTarget();
		if (Vector2.Distance(target.position, transform.position) < 0.8f)
		{
		    goto IL_E8;
		}
		yield return null;
	    }
	    Debug.LogError("Soul orb could not get player target!");
	}
	IL_E8:
	body.velocity = Vector2.zero;
	if (soulOrbCollectSounds)
	{
	    soulOrbCollectSounds.PlayOneShot(source);
	}
	if (getParticles)
	{
	    getParticles.Play();
	}
	if (sprite)
	{
	    sprite.enabled = false;
	}
	if (awardSoul)
	{
	    HeroController.instance.AddMPCharge(2);
	}
	SpriteFlash component = HeroController.instance.gameObject.GetComponent<SpriteFlash>();
	if (component)
	{
	    component.flashSoulGet();
	}
	yield return new WaitForSeconds(0.4f);
	if (dontRecycle)
	{
	    gameObject.SetActive(false);
	}
	else
	{
	    gameObject.Recycle();
	}
    }
    private void FireAtTarget()
    {
	float y = target.position.y - transform.position.y;
	float x = target.position.x - transform.position.x;
	float num = Mathf.Atan2(y, x) * 57.295776f;
	Vector2 velocity;
	velocity.x = speed * Mathf.Cos(num * 0.017453292f);
	velocity.y = speed * Mathf.Sin(num * 0.017453292f);
	body.velocity = velocity;
    }

    private void FaceAngle()
    {
	Vector2 velocity = body.velocity;
	float z = Mathf.Atan2(velocity.y, velocity.x) * 57.295776f;
	transform.localEulerAngles = new Vector3(0f, 0f, z);
    }

    private void ProjectileSquash()
    {
	float num = 1f - body.velocity.magnitude * stretchFactor * 0.01f;
	float num2 = 1f + body.velocity.magnitude * stretchFactor * 0.01f;
	if (num2 > stretchMaxX)
	{
	    num2 = stretchMaxX;
	}
	if (num < stretchMinY)
	{
	    num = stretchMinY;
	}
	num *= scaleModifier;
	num2 *= scaleModifier;
	transform.localScale = new Vector3(num2, num, transform.localScale.z);
    }
}
