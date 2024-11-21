using System.Collections;
using UnityEngine;

public class BounceShroom : MonoBehaviour
{
    [Tooltip("Active false by default since this script may be used elsewhere as just a flag")]
    public bool active = true;

    [Space]
    public GameObject idleParticlePrefab;
    public GameObject bounceSmallPrefab;
    public GameObject bounceLargePrefab;
    public GameObject heroParticlePrefab;

    [Header("Animations")]
    public string idleAnim = "Idle 1";
    public string bobAnim = "Bob 1";
    public string bounceAnim = "Bounce 1";

    [Space]
    public GameObject hitEffect;
    [Space]
    public AudioSource audioSourcePrefab;
    public RandomAudioClipTable hitSound;

    private tk2dSpriteAnimator anim;
    private Coroutine idleRoutine;
    private Coroutine bounceRoutine;
    private static GameObject heroParticles;

    private const float bounceParticleDelay = 0.25f;
    private static float nextBounceParticleTime;
    private static float nextCamShakeTime;

    private void Awake()
    {
	anim = GetComponentInChildren<tk2dSpriteAnimator>();
    }
    private void Start()
    {
	if (!active)
	    return;
	GameObject gameObject = Instantiate(idleParticlePrefab);
	if (gameObject)
	{
	    gameObject.transform.SetPositionX(transform.position.x);
	    gameObject.transform.SetPositionY(transform.position.y);
	}
	idleRoutine = StartCoroutine(Idle());
	CollisionEnterEvent[] componentsInChildren = GetComponentsInChildren<CollisionEnterEvent>();
	for (int i = 0; i < componentsInChildren.Length; i++)
	{
	    componentsInChildren[i].OnCollisionEnteredDirectional += delegate (CollisionEnterEvent.Direction direction, Collision2D collision)
	    {
		switch (direction)
		{
		    case CollisionEnterEvent.Direction.Left:
			HeroController.instance.SendMessage("RecoilLeft");
			break;
		    case CollisionEnterEvent.Direction.Right:
			HeroController.instance.SendMessage("RecoilRight");
			break;
		    case CollisionEnterEvent.Direction.Top:
			HeroController.instance.SendMessage("Bounce");
			HeroController.instance.SendMessage((Random.Range(0, 2) == 0) ? "RecoilLeft" : "RecoilRight");
			break;
		}
		BounceSmall();
	    };
	}
    }

    private IEnumerator Idle()
    {
	for(; ; )
	{
	    PlayAnims(AnimType.Idle);
	    yield return new WaitForSeconds(Random.Range(2f, 8f));
	    yield return new WaitForSeconds(PlayAnims(AnimType.Bob));
	}
    }

    private float PlayAnims(AnimType animType)
    {
	tk2dSpriteAnimationClip clipByName = anim.GetClipByName(GetAnimName(animType));
	anim.Play(clipByName);
	return clipByName.Duration;
    }

    private string GetAnimName(AnimType animType)
    {
	string result = "NONE";
	switch (animType)
	{
	    case AnimType.Idle:
		result = idleAnim;
		break;
	    case AnimType.Bob:
		result = bobAnim;
		break;
	    case AnimType.Bounce:
		result = bounceAnim;
		break;
	}
	return result;
    }

    public void BounceSmall()
    {
	if (!active)
	    return;
	if (bounceSmallPrefab)
	{
	    Instantiate(bounceSmallPrefab, new Vector3(transform.position.x,transform.position.y,-0.001f), Quaternion.identity);
	}
	if (bounceRoutine == null)
	{
	    bounceRoutine = StartCoroutine(Bounce());
	}
    }

    private IEnumerator Bounce()
    {
	if (idleRoutine != null)
	{
	    StopCoroutine(idleRoutine);
	}
	hitSound.SpawnAndPlayOneShot(audioSourcePrefab, transform.position);
	yield return new WaitForSeconds(PlayAnims(AnimType.Bounce));
	bounceRoutine = null;
	idleRoutine = StartCoroutine(Idle());
    }

    public void BounceLarge(bool useEffects)
    {
	if (!active)
	    return;
	if (useEffects)
	{
	    if (Time.time >= nextBounceParticleTime)
	    {
		nextBounceParticleTime = Time.time + 0.25f;
	    }
	    else
	    {
		useEffects = false;
	    }
	}
	if(bounceLargePrefab && useEffects)
	{
	    Instantiate(bounceLargePrefab, new Vector3(transform.position.x, transform.position.y, -0.001f), Quaternion.identity);
	}
	if(bounceRoutine == null)
	{
	    StartCoroutine(Bounce());
	}
	if(Time.time > nextCamShakeTime)
	{
	    GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
	    nextCamShakeTime = Time.time + 0.25f;
	}
	if (useEffects)
	{
	    if (hitEffect)
	    {
		hitEffect.SetActive(true);
	    }
	    if (heroParticlePrefab)
	    {
		ParticleSystem particleSystem = heroParticles ? heroParticles.GetComponent<ParticleSystem>() : null;
		if (!heroParticles || !heroParticles.activeSelf || (particleSystem && !particleSystem.isEmitting))
		{
		    heroParticles = Instantiate(heroParticlePrefab,new Vector3(0f, -1.5f, -0.002f),Quaternion.identity);
		    heroParticles.transform.SetParent(HeroController.instance.transform);
		}
	    }
	}
    }

    private enum AnimType
    {
	Idle,
	Bob,
	Bounce
    }
}
