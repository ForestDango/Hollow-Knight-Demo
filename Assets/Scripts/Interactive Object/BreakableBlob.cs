using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlob : MonoBehaviour
{
    public GameObject effect;
    private AudioSource source;
    private Collider2D newCollider2D;
    private SpriteRenderer sprite;

    [Space]
    public int spatterAmount = 5;
    public float spatterSpeedMin = 10f;
    public float spatterSpeedMax = 20f;
    public float spatterAngleMin = 40f;
    public float spatterAngleMax = 140f;

    [Space]
    public float audioPitchMin = 0.8f;
    public float audioPitchMax = 1.1f;

    private void Awake()
    {
	source = GetComponent<AudioSource>();
	newCollider2D = GetComponent<Collider2D>();
	sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if(collision.tag == "Nail Attack" || collision.tag == "Hero Spell")
	{
	    sprite.enabled = false;
	    if(effect != null)
	    {
		effect.SetActive(true);
	    }
	    if (source)
	    {
		source.pitch = Random.Range(audioPitchMin, audioPitchMax);
		source.Play();
	    }
	    SpawnSpatters(transform.position);
	}
    }
    private void SpawnSpatters(Vector3 position)
    {
	GlobalPrefabDefaults.Instance.SpawnBlood(position, (short)spatterAmount, (short)spatterAmount, spatterSpeedMin, spatterSpeedMax, spatterAngleMin, spatterAngleMax, null);
    }
}
