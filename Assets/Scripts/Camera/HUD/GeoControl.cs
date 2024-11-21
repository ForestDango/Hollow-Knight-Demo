using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoControl : MonoBehaviour
{
    public Size[] sizes = new Size[]
    {
	new Size("Small Idle", "Small Air", 1),
	new Size("Med Idle", "Med Air", 5),
	new Size("Large Idle", "Large Air", 25)
    };
    public int type;
    private Size size;
    [Space]
    public AudioClip[] pickupSounds;
    [Space]
    public ParticleSystem acidEffect;
    public GameObject getterBug;
    private Coroutine getterRoutine;
    private HeroController hero;
    private Transform player;
    private bool activated;
    private bool attracted;
    private const float pickupStartDelay = 0.25f;
    private float pickupStartTime;
    private float defaultGravity;

    private tk2dSpriteAnimator anim;
    private AudioSource audioSource;
    private Renderer rend;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private SpriteFlash spriteFlash;

    private void Awake()
    {
	anim = GetComponent<tk2dSpriteAnimator>();
	audioSource = GetComponent<AudioSource>();
	rend = GetComponent<Renderer>();
	body = GetComponent<Rigidbody2D>();
	boxCollider = GetComponent<BoxCollider2D>();
	spriteFlash = GetComponent<SpriteFlash>();
	defaultGravity = body.gravityScale;
    }

    private void Start()
    {
	hero = HeroController.instance;
    }

    private void OnEnable()
    {
	SetSize(type);
	transform.SetPositionZ(UnityEngine.Random.Range(0.001f, 0.002f));
	activated = false;
	attracted = false;
	body.gravityScale = defaultGravity;
	if (rend)
	{
	    rend.enabled = true;
	}
	if (getterBug)
	{
	    getterBug.SetActive(false);
	}
	if (acidEffect)
	{
	    acidEffect.gameObject.SetActive(false);
	}
	boxCollider.isTrigger = false;
	if (GameManager.instance.sceneName == "Crossroads_38")
	{
	    return;
	}
	if (GameManager.instance.GetPlayerDataBool("equippedCharm_1"))
	{
	    getterRoutine = StartCoroutine(Getter());
	}
	pickupStartTime = Time.time + pickupStartDelay;
    }

    private void FixedUpdate()
    {
	if (attracted)
	{
	    Vector2 vector = new Vector2(hero.transform.position.x - transform.position.x, hero.transform.position.y - 0.5f - transform.position.y);
	    vector = Vector2.ClampMagnitude(vector, 1f);
	    vector = new Vector2(vector.x * 150f, vector.y * 150f);
	    body.AddForce(vector);
	    Vector2 vector2 = body.velocity;
	    vector2 = Vector2.ClampMagnitude(vector2, 20f);
	    body.velocity = vector2;
	}
    }

    private void SetSize(int index)
    {
	if (index >= sizes.Length)
	{
	    index = sizes.Length - 1;
	}
	else if (index < 0)
	{
	    index = 0;
	}
	size = sizes[index];
	if (anim)
	{
	    anim.Play(size.airAnim);
	}
    }

    private IEnumerator Getter()
    {
	yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 1.7f));
	if (getterBug)
	{
	    //插值执行捡钱虫子开始时淡入淡出效果
	    getterBug.SetActive(true);
	    Vector3 destination = new Vector3(-0.06624349f, 0.1932119f, -0.001f);
	    Vector3 source = destination + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0.5f, 1.5f), 0f);
	    float easeTime = UnityEngine.Random.Range(0.3f, 0.5f);
	    for (float timer = 0f; timer < easeTime; timer += Time.deltaTime)
	    {
		float t = Mathf.Sin(timer / easeTime * 1.5707964f);
		getterBug.transform.localPosition = Vector3.Lerp(source, destination, t);
		yield return null;
	    }
	    getterBug.transform.localPosition = destination;
	    boxCollider.isTrigger = true;
	    body.gravityScale = 0f;
	    attracted = true; //然后激活attracted让虫子和钱朝着玩家的方向移动
	    destination = default(Vector3);
	    source = default(Vector3);
	}
    }

    /// <summary>
    /// 碰到地面上
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
	if (anim)
	{
	    tk2dSpriteAnimationClip clipByName = anim.GetClipByName(size.idleAnim);
	    if(clipByName != null)
	    {
		anim.PlayFromFrame(clipByName, UnityEngine.Random.Range(0, clipByName.frames.Length));
	    }
	}
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
	if (activated || Time.time < pickupStartTime)
	{
	    return;
	}
	bool flag = false;
	float num = 0f;
	if(collision.tag == "HeroBox") //如果碰到玩家了就加钱
	{
	    Debug.LogFormat("size.value = " + size.value);
	    hero.AddGeo(size.value);
	    num = Mathf.Max(num, PlayCollectSound());
	    flag = true;
	}
	else if (collision.tag == "Acid") //如果掉到酸水里了就播放特效
	{
	    if (acidEffect)
	    {
		acidEffect.gameObject.SetActive(true);
		num = Mathf.Max(num, acidEffect.main.duration + acidEffect.main.startLifetime.constant);
	    }
	    flag = true;
	}
	if (flag) //暂停捡钱的虫子的routine，销毁该物体
	{
	    if (getterRoutine != null)
	    {
		StopCoroutine(getterRoutine);
	    }
	    Disable(num);
	}
    }

    /// <summary>
    /// 获取随机一个音频的播放时间
    /// </summary>
    /// <returns></returns>
    private float PlayCollectSound()
    {
	if (audioSource && pickupSounds.Length != 0)
	{
	    AudioClip audioClip = pickupSounds[UnityEngine.Random.Range(0, pickupSounds.Length)];
	    if (audioClip)
	    {
		audioSource.PlayOneShot(audioClip);
		return audioClip.length;
	    }
	    Debug.LogError("GeoControl encountered missing audio!", this);
	}
	return 0f;
    }

    private void Disable(float waitTime)
    {
	activated = true;
	if (rend)
	{
	    rend.enabled = false;
	}
	if (getterBug)
	{
	    getterBug.SetActive(false);
	}
	StartCoroutine(DisableAfterTime(waitTime));
    }

    private IEnumerator DisableAfterTime(float waitTime)
    {
	yield return new WaitForSeconds(waitTime);
	gameObject.Recycle();
    }

    public void SetFlashing()
    {
	if (spriteFlash)
	{
	    spriteFlash.GeoFlash();
	}
    }

    [Serializable]
    public struct Size
    {
	public string idleAnim;
	public string airAnim;
	public int value;
	public Size(string idleAnim, string airAnim, int value)
	{
	    this.idleAnim = idleAnim;
	    this.airAnim = airAnim;
	    this.value = value;
	}
    }
}
