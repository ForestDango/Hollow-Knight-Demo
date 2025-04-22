using System;
using System.Collections;
using UnityEngine;

public class SpawnJarControl : MonoBehaviour
{
    public float spawnY = 106.52f;
    public float breakY = 94.55f;
    public ParticleSystem readyDust;
    public ParticleSystem dustTrail;
    public ParticleSystem ptBreakS;
    public ParticleSystem ptBreakL;
    public GameObject strikeNailR;
    public AudioEventRandom breakSound;
    public AudioSource audioSourcePrefab;

    private GameObject enemyToSpawn;
    private int enemyHealth;
    private CircleCollider2D col;
    private Rigidbody2D body;
    private SpriteRenderer sprite;

    private void Awake()
    {
	col = GetComponent<CircleCollider2D>();
	body = GetComponent<Rigidbody2D>();
	sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
	col.enabled = false;
	sprite.enabled = false;
	StartCoroutine(Behaviour());
    }

    private IEnumerator Behaviour()
    {
	transform.SetPositionY(spawnY);
	transform.SetPositionZ(0.01f);
	readyDust.Play();
	yield return new WaitForSeconds(0.5f);
	col.enabled = true;
	body.velocity = new Vector2(0f, -25f);
	body.angularVelocity = (UnityEngine.Random.Range(0, 2) > 0) ? -300 : 300;
	readyDust.Stop();
	dustTrail.Play();
	sprite.enabled = true;
	while (transform.position.y > breakY)
	{
	    yield return null;
	}
	transform.SetPositionY(breakY);
	GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
	dustTrail.Stop();
	ptBreakS.Play();
	ptBreakL.Play();
	strikeNailR.Spawn(transform.position);
	col.enabled = false;
	body.velocity = Vector2.zero;
	body.angularVelocity = 0f;
	sprite.enabled = false;
	breakSound.SpawnAndPlayOneShot(audioSourcePrefab, transform.position);
	if (enemyToSpawn)
	{
	    GameObject gameObject = enemyToSpawn.Spawn(transform.position);
	    HealthManager component = gameObject.GetComponent<HealthManager>();
	    if (component)
	    {
		component.hp = enemyHealth;
	    }
	    gameObject.tag = "Boss";
	}
	yield return new WaitForSeconds(2f);
	gameObject.Recycle();
    }

    public void SetEnemySpawn(GameObject prefab, int health)
    {
	enemyToSpawn = prefab;
	enemyHealth = health;
    }

}
