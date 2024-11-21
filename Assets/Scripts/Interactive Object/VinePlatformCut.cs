using System;
using UnityEngine;

public class VinePlatformCut : MonoBehaviour
{
    public Rigidbody2D body;

    [Space]
    public GameObject sprites;

    [Space]
    public GameObject cutParticles;
    public GameObject cutPointParticles;
    public GameObject cutEffectPrefab;

    [Space]
    public AudioClip cutSound;

    private bool activated;
    private VinePlatform platform;
    private AudioSource audioSource;
    private Collider2D col;

    private void Awake()
    {
	platform = GetComponentInParent<VinePlatform>();
	audioSource = GetComponentInParent<AudioSource>();
	col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if (activated)
	{
	    return;
	}
	if(collision.tag == "Nail Attack")
	{
	    if (collision.transform.position.y < col.bounds.min.y || collision.transform.position.y > col.bounds.max.y)
	    {
		return;
	    }
	    float value = PlayMakerFSM.FindFsmOnGameObject(collision.gameObject, "damages_enemy").FsmVariables.FindFsmFloat("direction").Value;
	    if (value >= 45f)
	    {
		if (value < 135f)
		{
		    return;
		}
		if (value >= 225f && value < 360f)
		{
		    return;
		}
	    }
	    Vector3 position = cutPointParticles.transform.position;
	    position.y = collision.transform.position.y;
	    if (cutEffectPrefab)
	    {
		cutEffectPrefab.Spawn(position);
	    }
	    if (cutPointParticles)
	    {
		cutPointParticles.transform.position = position;
		cutPointParticles.SetActive(true);
	    }
	    Cut();
	}
    }

    private void Cut()
    {
	Debug.LogFormat("Cut");
	activated = true;
	if (body)
	{
	    body.isKinematic = false;
	}
	if (cutParticles)
	{
	    cutParticles.SetActive(true);
	}
	if (platform.enemyDetector)
	{
	    platform.enemyDetector.gameObject.SetActive(true);
	}
	platform.respondOnLand = false;
	if (platform.landRoutine != null)
	{
	    StopCoroutine(platform.landRoutine);
	}
	if (audioSource && cutSound)
	{
	    audioSource.PlayOneShot(cutSound);
	}
	Disable(false);
    }

    public void Disable(bool disableAll = false)
    {
	if (disableAll)
	{
	    gameObject.SetActive(false);
	    return;
	}
	Debug.LogFormat("Can Disable");
	if (sprites != null)
	{
	    Debug.LogFormat("Disable");
	    sprites.SetActive(false);
	}
    }
}
