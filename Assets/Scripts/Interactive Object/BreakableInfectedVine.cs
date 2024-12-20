using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableInfectedVine : MonoBehaviour
{
    public GameObject[] blobs;
    [Space]
    public GameObject[] effects;

    [Space]
    public int spatterAmount = 5;
    public float spatterSpeedMin = 10f;
    public float spatterSpeedMax = 20f;
    public float spatterAngleMin = 40f;
    public float spatterAngleMax = 140f;

    [Space]
    public float audioPitchMin = 0.8f;
    public float audioPitchMax = 1.1f;

    private bool activated;
    private AudioSource source;

    private void Awake()
    {
	source = GetComponent<AudioSource>();
    }

    private void Start()
    {
	if (Mathf.Abs(transform.position.z - 0.004f) > 1f)
	{
	    if (source)
	    {
		source.enabled = false;
	    }
	    Collider2D component = GetComponent<Collider2D>();
	    if (component)
	    {
		component.enabled = false;
	    }
	    enabled = false;
	}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if (activated)
	{
	    return;
	}
	bool flag = false;
	if (collision.tag == "Nail Attack")
	{
	    flag = true;
	}
	else if (collision.tag == "Hero Spell")
	{
	    flag = true;
	}

	if (flag)
	{
	    foreach (GameObject gameObject in blobs)
	    {
		gameObject.SetActive(false);
		SpawnSpatters(gameObject.transform.position);
	    }
	    GameObject[] array = effects;
	    for (int i = 0; i < array.Length; i++)
	    {
		array[i].SetActive(true);
	    }
	    if (source)
	    {
		source.pitch = Random.Range(audioPitchMin, audioPitchMax);
		source.Play();
	    }

	    activated = true;
	}
    }

    private void SpawnSpatters(Vector3 position)
    {
	GlobalPrefabDefaults.Instance.SpawnBlood(position, (short)spatterAmount, (short)spatterAmount, spatterSpeedMin, spatterSpeedMax, spatterAngleMin, spatterAngleMax, null);
    }
}
