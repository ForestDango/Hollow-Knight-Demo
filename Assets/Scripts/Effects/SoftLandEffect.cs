using System;
using UnityEngine;

public class SoftLandEffect : MonoBehaviour
{
    public GameObject dustEffects;

    public AudioClip softLandClip;

    private PlayerData pd;
    private GameObject heroObject;
    private AudioSource audioSource;
    private Rigidbody2D heroRigibody;
    private tk2dSpriteAnimator jumpPuffAnimator;

    private float recycleTimer;
    private void OnEnable()
    {
	if (pd == null)
	    pd = PlayerData.instance;
	if(audioSource == null)
	{
	    audioSource = GetComponent<AudioSource>();
	}
	foreach (object obj in transform)
	{
	    ((Transform)obj).gameObject.SetActive(false);
	}
	recycleTimer = 1f;
	HeroController instance = HeroController.instance;
	if(instance != null)
	{

	    dustEffects.SetActive(true);
	    audioSource.PlayOneShot(softLandClip);
	}
    }

    private void Update()
    {
	if(recycleTimer <= 0f)
	{
	    gameObject.Recycle();
	    return;
	}
	recycleTimer -= Time.deltaTime;
    }

}
