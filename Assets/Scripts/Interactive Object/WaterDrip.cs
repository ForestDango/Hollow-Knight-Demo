using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(tk2dSpriteAnimator))]
[RequireComponent(typeof(AudioSource))]
public class WaterDrip : MonoBehaviour
{
    public float idleTimeMin = 2f;
    public float idleTimeMax = 8f;
    public float fallVelocity = -7f;
    public RandomAudioClipTable impactAudioClipTable;

    public float impactTranslation = -0.5f;
    private bool impacted;
    private Vector2 startPos;

    private Collider2D col;
    private Rigidbody2D body;
    private tk2dSpriteAnimator anim;
    private AudioSource source;

    private void Awake()
    {
	col = GetComponent<Collider2D>();
	body = GetComponent<Rigidbody2D>();
	anim = GetComponent<tk2dSpriteAnimator>();
	source = GetComponent<AudioSource>();
    }

    private void Start()
    {
	startPos = transform.position;
	StartCoroutine(Drip());
    }

    private IEnumerator Drip()
    {
	for(; ; )
	{
	    anim.Play("Idle");
	    body.gravityScale = 0f;
	    body.velocity = Vector2.zero;
	    col.enabled = false;
	    yield return new WaitForSeconds(UnityEngine.Random.Range(idleTimeMin, idleTimeMax)); //等idle状态的事件结束后执行Drip和Fall状
	    col.enabled = true;
	    yield return StartCoroutine(anim.PlayAnimWait("Drip"));
	    anim.Play("Fall");
	    body.gravityScale = 1f;
	    body.velocity = new Vector2(0f, fallVelocity);
	    impacted = false;
	    while (!impacted)
	    {
		yield return null;
	    }
	    body.gravityScale = 0f;
	    body.velocity = Vector2.zero;
	    col.enabled = false;
	    impactAudioClipTable.PlayOneShot(source);
	    transform.position += new Vector3(0f, impactTranslation, 0f);
	    yield return StartCoroutine(anim.PlayAnimWait("Impact")); //等执行完impact状态的动画后就回到初始的位置继续循环执行
	    transform.position = startPos;
	}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
	impacted = true;
    }

}
