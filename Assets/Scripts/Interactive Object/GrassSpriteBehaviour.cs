using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpriteBehaviour : MonoBehaviour
{
    [Header("Variables")]
    public bool isWindy;
    public bool noPushAnimation;

    [Space]
    public GameObject deathParticles;
    public GameObject deathParticlesWindy;
    public GameObject cutEffectPrefab;

    [Space]
    public AudioClip[] pushSounds;
    public AudioClip[] cutSounds;

    [Header("Animation State Names")]
    public string idleAnimation = "Idle";
    public string pushAnimation = "Push";
    public string cutAnimation = "Dead";
    public string idleWindyAnimation = "WindyIdle";
    public string pushWindyAnimation = "WindyPush";

    private bool isCut;
    private bool interaction = true;
    private bool visible;

    private Animator animator;
    private AudioSource audioSource;
    public SpriteRenderer grassAlive;
    public SpriteRenderer grassDead;

    private void Awake()
    {
	animator = GetComponentInChildren<Animator>();
	audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
	if (Mathf.Abs(transform.position.z - 0.004f) > 1.8f)
	{
	    interaction = false;
	}
	Init();
    }

    private void OnBecameVisible()
    {
	visible = true;
    }

    private void OnBecameInvisible()
    {
	visible = true;
    }

    private void Init()
    {
	visible = true;
	animator.Play(isWindy ? idleAnimation : idleWindyAnimation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if (!isCut && interaction && visible)
	{
	    if (GrassCut.ShouldCut(collision))
	    {
		Debug.LogFormat("Grass Should Cut");
		animator.Play(cutAnimation);
		isCut = true;
		if(isWindy && deathParticlesWindy)
		{
		    deathParticlesWindy.SetActive(true);
		}
		else if (deathParticles)
		{
		    deathParticles.SetActive(true);
		}
		if(audioSource && cutSounds.Length != 0)
		{
		    audioSource.PlayOneShot(cutSounds[UnityEngine.Random.Range(0, cutSounds.Length)]);
		}
		grassAlive.enabled = false;
		grassDead.enabled = true;
		if (cutEffectPrefab)
		{
		    int num = (int)Mathf.Sign(collision.transform.position.x - transform.position.x);
		    Vector3 position = (collision.transform.position + transform.position) / 2f;
		    GameObject gameObject = cutEffectPrefab.Spawn(position);
		    //GameObject gameObject = Instantiate(cutEffectPrefab, position, Quaternion.identity);
		    Vector3 localScale = gameObject.transform.localScale;
		    localScale.x = Mathf.Abs(localScale.x) * -num;
		    gameObject.transform.localScale = localScale;
		    return;
		}
	    }
	    else
	    {
		if (!noPushAnimation)
		{
		    animator.Play(isWindy ? pushWindyAnimation : pushAnimation);
		}
		if (audioSource && cutSounds.Length != 0)
		{
		    audioSource.PlayOneShot(cutSounds[UnityEngine.Random.Range(0, cutSounds.Length)]);
		}
	    }
	}
    }

}
