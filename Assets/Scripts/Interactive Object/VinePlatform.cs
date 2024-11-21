using System;
using System.Collections;
using UnityEngine;

public class VinePlatform : MonoBehaviour
{
    public GameObject platformSprite;
    public GameObject activatedSprite;
    public Collider2D col;
    [Space]
    public AudioClip playerLandSound;
    public ParticleSystem playerLandParticles;
    public AnimationCurve playerLandAnimCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});
    public float playerLandAnimLength = 0.5f;

    [HideInInspector]
    public Coroutine landRoutine;
    [HideInInspector]
    public bool respondOnLand = true;

    private Action landReturnAction;

    [Space]
    public TriggerEnterEvent landingDetector;
    public AudioClip landSound;
    public ParticleSystem[] landParticles;
    public GameObject slamEffect;
    [Space]
    public TriggerEnterEvent enemyDetector;
    [Space]
    public bool acidLander;
    public float acidTargetY;
    public AudioClip acidSplashSound;
    public GameObject acidSplashPrefab;

    private AudioSource audioSource;
    private Rigidbody2D body;
    private bool activated;

    private void Awake()
    {
	audioSource = GetComponent<AudioSource>();
	body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
	//TODO:Persistent Item
	if (landingDetector && !acidLander)
	{
	    landingDetector.OnTriggerEntered += delegate (Collider2D collider, GameObject sender)
	    {
		Land();
	    };
	}
	if (enemyDetector)
	{
	    enemyDetector.OnTriggerEntered += delegate (Collider2D collider, GameObject sender)
	    {
		HealthManager component2 = collider.GetComponent<HealthManager>();
		if (component2)
		{
		    component2.Die(new float?(0f), AttackTypes.Splatter, false);
		}
	    };
	    enemyDetector.gameObject.SetActive(false);
	}
    }

    private void Update()
    {
	if(acidLander && !activated && col.bounds.min.y <= acidTargetY)
	{
	    Land();
	}
    }

    private void Land()
    {
	PlaySound(landSound);
	if (!acidLander)
	{
	    GameCameras gameCameras = FindObjectOfType<GameCameras>();
	    if (gameCameras)
	    {
		gameCameras.cameraShakeFSM.SendEvent("AverageShake");
	    }
	    foreach (ParticleSystem particleSystem in landParticles)
	    {
		if (particleSystem.gameObject.activeInHierarchy)
		{
		    particleSystem.Play();
		}
	    }
	    if (slamEffect)
	    {
		slamEffect.SetActive(true);
	    }
	}
	else
	{
	    PlaySound(acidSplashSound);
	    if (acidSplashPrefab)
	    {
		Instantiate(acidSplashPrefab, new Vector3(transform.position.x, GetComponent<Collider>().bounds.min.y, transform.position.z), Quaternion.identity);
	    }
	    float num = transform.position.y - col.bounds.min.y;
	    transform.SetPositionY(acidTargetY + num);
	}
	if (body)
	{
	    body.isKinematic = true;
	    body.velocity = Vector2.zero;
	}
	if (enemyDetector)
	{
	    enemyDetector.gameObject.SetActive(false);
	}
	activated = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
	//这就是判断玩家是否踩到platform的上面，三个条件：第一个是固定位true，第二个是判断是否是玩家，第三个是判断玩家的collider的y是不是在自己
	//的collider的y的上面
	if(respondOnLand && collision.gameObject.layer == 9 && collision.collider.bounds.min.y >= col.bounds.max.y)
	{
	    if (landRoutine != null)
	    {
		StopCoroutine(landRoutine);
	    }
	    if (landReturnAction != null)
	    {
		landReturnAction();
	    }
	    landRoutine = StartCoroutine(PlayerLand());
	    return;
	}
	if(!body.isKinematic && collision.gameObject.layer != 8 && collision.gameObject.layer != 9)
	{
	    Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
	}
    }
    private void PlaySound(AudioClip clip)
    {
	if (audioSource && clip)
	{
	    audioSource.PlayOneShot(clip);
	}
    }

    private IEnumerator PlayerLand()
    {
	PlaySound(playerLandSound);
	if (playerLandParticles)
	{
	    playerLandParticles.Play();
	}
	if (platformSprite)
	{
	    Vector3 initialPos = platformSprite.transform.position;
	    landReturnAction = delegate ()
	    {
		platformSprite.transform.position = initialPos;
	    };
	    for (float elapsed = 0f; elapsed < playerLandAnimLength; elapsed += Time.deltaTime)
	    {
		Vector3 initialPos2 = initialPos;
		initialPos2.y += playerLandAnimCurve.Evaluate(elapsed / playerLandAnimLength);
		platformSprite.transform.position = initialPos2;
		yield return null;
	    }
	}
	if(landReturnAction != null)
	{
	    landReturnAction();
	}
	landRoutine = null;
    }

    private void OnDrawGizmosSelected()
    {
	if (acidLander)
	{
	    Vector3 position = transform.position;
	    position.y = acidTargetY;
	    Gizmos.DrawWireSphere(position, 0.5f);
	}
    }
}
