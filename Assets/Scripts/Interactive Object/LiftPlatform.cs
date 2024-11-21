using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftPlatform : MonoBehaviour
{
    public GameObject part1;
    public GameObject part2;
    public ParticleSystem dustParticle;
    public AudioSource source;
    private float part1_start_y;
    private float part2_start_y;
    private int state;
    private float timer;

    private void Start()
    {
	part1_start_y = part1.transform.position.y;
	part2_start_y = part2.transform.position.y;
    }

    private void Update()
    {
	if(state == 1)
	{
	    if (timer < 0.125f)
	    {
		part1.transform.position = new Vector3(part1.transform.position.x, part1_start_y - timer * 0.75f, part1.transform.position.z);
		part2.transform.position = new Vector3(part2.transform.position.x, part2_start_y - timer * 0.75f, part2.transform.position.z);
		timer += Time.deltaTime;
	    }
	    else
	    {
		part1.transform.position = new Vector3(part1.transform.position.x, part1_start_y - 0.09f, part1.transform.position.z);
		part2.transform.position = new Vector3(part2.transform.position.x, part2_start_y - 0.09f, part2.transform.position.z);
		state = 2;
		timer = 0.12f;
	    }
	}
	if(state == 2)
	{
	    if (timer > 0f)
	    {
		part1.transform.position = new Vector3(part1.transform.position.x, part1_start_y - timer * 0.75f, part1.transform.position.z);
		part2.transform.position = new Vector3(part2.transform.position.x, part2_start_y - timer * 0.75f, part2.transform.position.z);
		timer -= Time.deltaTime;
		return;
	    }
	    part1.transform.position = new Vector3(part1.transform.position.x, part1_start_y, part1.transform.position.z);
	    part2.transform.position = new Vector3(part2.transform.position.x, part2_start_y, part2.transform.position.z);
	    state = 0;
	}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
	if(state == 0 && collision.collider.gameObject.layer != LayerMask.NameToLayer("Item") && collision.gameObject.layer != LayerMask.NameToLayer("Particle") &&  collision.gameObject.layer != LayerMask.NameToLayer("Enemies") && collision.GetSafeContact().Normal.y < 0.1f)
	{
	    source.pitch = Random.Range(0.85f, 1.15f);
	    source.Play();
	    dustParticle.Play();
	    state = 0;
	    timer = 0f;
	}
    }

}
