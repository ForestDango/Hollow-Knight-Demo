using System;
using System.Collections;
using UnityEngine;

public class StalactiteControl : MonoBehaviour
{
    public GameObject top;
    [Space]
    public float startFallOffset = 0.1f; //开始降落时的偏移量
    public GameObject startFallEffect; //开始降落时生成的效果
    public AudioClip startFallSound;//开始降落时的声音
    public float fallDelay = 0.25f; //延迟降落时间
    [Space]
    public GameObject fallEffect;
    public GameObject trailEffect;
    public GameObject nailStrikePrefab;

    [Space]
    public GameObject embeddedVersion; //嵌入式版本
    public GameObject[] landEffectPrefabs; //落地后生成的效果预制体

    [Space]
    public float hitVelocity = 40f; //下落的速度

    [Space]
    public GameObject[] hitUpEffectPrefabs; //击中钟乳石后的效果预制体
    public AudioClip hitSound; //击中后的声音
    public GameObject hitUpRockPrefabs; //击中后的延时预制体

    public int spawnMin = 10;
    public int spawnMax = 12;
    public int speedMin = 15;
    public int speedMax = 20;

    public AudioClip breakSound;

    private TriggerEnterEvent trigger;
    private DamageHero damageHero;
    private DamageEnemies damageEnemies;
    private Rigidbody2D body;
    private AudioSource source;
    private bool fallen;

    private void Awake()
    {
	damageEnemies = GetComponent<DamageEnemies>();
	damageHero = GetComponent<DamageHero>();
	body = GetComponent<Rigidbody2D>();
	source = GetComponent<AudioSource>();
    }

    private void Start()
    {
	trigger = GetComponentInChildren<TriggerEnterEvent>();
	if(trigger)
	{
	    trigger.OnTriggerEntered += HandleTriggerEnter;
	}
	if (damageHero)
	{
	    damageHero.damageDealt = 0;
	}
	body.isKinematic = true;
	if (damageEnemies)
	{
	    damageEnemies.enabled = false;
	}
    }

    /// <summary>
    /// 订阅TriggerEnterEvent的OnTriggerEntered事件，当发生事件时执行该函数里的内容来处理碰撞检测
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="sender"></param>
    private void HandleTriggerEnter(Collider2D collider, GameObject sender)
    {
	if (collider.tag == "Player" && Physics2D.Linecast(transform.position, collider.transform.position, LayerMask.GetMask("Terrain")).collider == null)
	{
	    StartCoroutine(Fall(fallDelay));
	    trigger.OnTriggerEntered -= HandleTriggerEnter;
	    sender.SetActive(false);
	}
    }

    private IEnumerator Fall(float delay)
    {
	if (top)
	{
	    top.transform.SetParent(transform.parent);
	}
	transform.position += Vector3.down * startFallOffset;
	if (startFallEffect)
	{
	    startFallEffect.SetActive(true);
	    startFallEffect.transform.SetParent(transform.parent);
	}
	if (source && startFallSound)
	{
	    source.PlayOneShot(startFallSound);
	}
	yield return new WaitForSeconds(delay);
	if (fallEffect)
	{
	    fallEffect.SetActive(true);
	    fallEffect.transform.SetParent(transform.parent);
	}
	if (trailEffect)
	{
	    trailEffect.SetActive(true);
	}
	if (damageHero)
	{
	    damageHero.damageDealt = 1;
	}
	if (damageEnemies)
	{
	    damageEnemies.damageDealt = 1;
	}
	body.isKinematic = false;
	fallen = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if (fallen && collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
	{
	    body.isKinematic = true;
	    if (trailEffect)
	    {
		trailEffect.transform.parent = null;
	    }
	    trailEffect.GetComponent<ParticleSystem>().Stop();
	    if (embeddedVersion)
	    {
		embeddedVersion.SetActive(true);
		embeddedVersion.transform.SetParent(transform.parent, true);
	    }
	    RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, Vector2.down, 10f, LayerMask.GetMask("Terrain"));
	    foreach (GameObject gameObject in landEffectPrefabs)
	    {
		Vector3 vector = new Vector3(raycastHit2D.point.x, raycastHit2D.point.y, gameObject.transform.position.z);
		gameObject.Spawn((raycastHit2D.collider != null) ? vector : transform.position);
	    }
	    gameObject.SetActive(false);
	    return;
	}
	if (collision.tag == "Nail Attack")
	{
	    if (!fallen)
	    {
		StartCoroutine(Fall(0f));
	    }
	    if (damageHero)
	    {
		damageHero.damageDealt = 0;
		damageHero = null;
	    }
	    float value = PlayMakerFSM.FindFsmOnGameObject(collision.gameObject, "damages_enemy").FsmVariables.FindFsmFloat("direction").Value;
	    float num = 0f;
	    if (value < 45f)
	    {
		num = 45f;
	    }
	    else
	    {
		if (value < 135f)
		{
		    GameObject[] array = hitUpEffectPrefabs;
		    for (int i = 0; i < array.Length; i++)
		    {
			array[i].Spawn(transform.position);
		    }
		    FlingObjects();
		    if (source && breakSound)
		    {
			AudioSource audioSource = new GameObject("StalactiteBreakEffect").AddComponent<AudioSource>();
			audioSource.outputAudioMixerGroup = source.outputAudioMixerGroup;
			audioSource.loop = false;
			audioSource.playOnAwake = false;
			audioSource.rolloffMode = source.rolloffMode;
			audioSource.minDistance = source.minDistance;
			audioSource.maxDistance = source.maxDistance;
			audioSource.clip = breakSound;
			audioSource.volume = source.volume;
			audioSource.Play();
		    }
		    gameObject.SetActive(false);
		    return;
		}
		if (value < 225f)
		{
		    num = -45f;
		}
		else if (value < 360f)
		{
		    num = 0f;
		}
	    }
	    Vector3 v = Quaternion.Euler(0f, 0f, num) * Vector3.down * hitVelocity;
	    body.rotation = num;
	    body.gravityScale = 0f;
	    body.velocity = v;
	    nailStrikePrefab.Spawn(transform.position);
	    if (source && hitSound)
	    {
		source.PlayOneShot(hitSound);
	    }
	}
    }

    private void FlingObjects()
    {
	int num = UnityEngine.Random.Range(spawnMin, speedMax + 1);
	for (int i = 0; i <= num; i++)
	{
	    GameObject gameObject = hitUpRockPrefabs.Spawn(transform.position, transform.rotation);
	    Vector3 position = gameObject.transform.position;
	    Vector3 rotatiton2 = gameObject.transform.position;
	    Vector3 position3 = gameObject.transform.position;
	    float num2 = UnityEngine.Random.Range(speedMin, speedMax);
	    float num3 = UnityEngine.Random.Range(0f, 360f);
	    float x = num2 * Mathf.Cos(num3 * 0.017453292f);
	    float y = num2 * Mathf.Sin(num3 * 0.017453292f);
	    Vector2 velocity;
	    velocity.x = x;
	    velocity.y = y;
	    Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
	    if (component)
	    {
		component.velocity = velocity;
	    }
	}
    }
}
