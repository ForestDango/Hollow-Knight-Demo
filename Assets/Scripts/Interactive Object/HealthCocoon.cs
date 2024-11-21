using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCocoon : MonoBehaviour
{
    [Header("Behaviour")]
    public GameObject[] slashEffects;
    public GameObject[] spellEffects;

    public Vector3 effectOrigin = new Vector3(0f, 0.8f, 0f);

    [Space]
    public FlingPrefab[] flingPrefabs;

    [Space]
    public GameObject[] enableChildren;
    public GameObject[] disableChildren;
    public Collider2D[] disableColliders;

    [Space]
    public Rigidbody2D cap;
    public float capHitForce = 10f;

    [Space]
    public AudioClip deathSound;

    [Header("Animation")]
    public string idleAnimation = "Cocoon Idle";
    public string sweatAnimation = "Cocoon Sweat";

    public AudioClip moveSound;
    public float waitMin = 2f;
    public float waitMax = 6f;

    private Coroutine animRoutine;
    private AudioSource source;
    private tk2dSpriteAnimator animator;
    private bool activated;

    private void Awake()
    {
	source = GetComponent<AudioSource>();
	animator = GetComponent<tk2dSpriteAnimator>();

    }


    private void Start()
    {
	animRoutine = StartCoroutine(Animate());
	FlingPrefab[] array = flingPrefabs;
	for (int i = 0; i < array.Length; i++)
	{
	    array[i].SetupPool(transform);
	}
    }

    private void PlaySound(AudioClip clip)
    {
	if(source && clip)
	{
	    source.PlayOneShot(clip);
	}
    }

    private IEnumerator Animate()
    {
	for(; ; )
	{
	    yield return new WaitForSeconds(Random.Range(waitMin, waitMax));
	    PlaySound(moveSound);
	    if (animator)
	    {
		tk2dSpriteAnimationClip clip = animator.GetClipByName(sweatAnimation);
		animator.Play(clip);
		yield return new WaitForSeconds(clip.frames.Length / clip.fps);
		animator.Play(idleAnimation);
	    }
	}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if (!activated)
	{
	    bool flag = false;
	    if(collision.tag == "Nail Attack")
	    {
		flag = true;
		float value = PlayMakerFSM.FindFsmOnGameObject(collision.gameObject, "damages_enemy").FsmVariables.FindFsmFloat("direction").Value;
		float z = 0f;
		Vector2 v = new Vector2(1.5f, 1.5f);
		if (value < 45f)
		{
		    z = Random.Range(340, 380);
		}
		else if (value < 135f)
		{
		    z = Random.Range(340, 380);
		}
		else if (value < 225f)
		{
		    v.x *= -1f;
		    z = Random.Range(70, 110);
		}
		else if (value < 360f)
		{
		    z = Random.Range(250, 290);
		}
		GameObject[] array = slashEffects;
		for (int i = 0; i < array.Length; i++)
		{
		    GameObject gameObject = array[i].Spawn(transform.position + effectOrigin);
		    gameObject.transform.eulerAngles = new Vector3(0f, 0f, z);
		    gameObject.transform.localScale = v;
		}
	    }
	    if(collision.tag == "Hero Spell")
	    {
		flag = true;
		GameObject[] array = slashEffects;
		for (int i = 0; i < array.Length; i++)
		{
		    GameObject gameObject2 = array[i].Spawn(transform.position + effectOrigin);
		    Vector3 position = gameObject2.transform.position;
		    position.z = 0.0031f;
		    gameObject2.transform.position = position;
		}
	    }
	    if (flag)
	    {
		activated = true;
		GameObject[] array = enableChildren;
		for (int i = 0; i < array.Length; i++)
		{
		    array[i].SetActive(true);
		}
		if (cap)
		{
		    cap.gameObject.SetActive(true);
		    Vector3 a = transform.position - collision.transform.position;
		    a.Normalize();
		    cap.AddForce(capHitForce * a, ForceMode2D.Impulse);
		}
		foreach (FlingPrefab fling in flingPrefabs)
		{
		    FlingObjects(fling);
		}
		PlaySound(deathSound);
		SetBroken();
		//TODO:
		GameCameras gameCameras = FindObjectOfType<GameCameras>();
		if (gameCameras)
		{
		    gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
		}
	    }
	}
    }

    private void SetBroken()
    {
	StopCoroutine(animRoutine);
	GetComponent<MeshRenderer>().enabled = false;
	GameObject[] array = disableChildren;
	for (int i = 0; i < array.Length; i++)
	{
	    array[i].SetActive(false);
	}
	Collider2D[] array2 = disableColliders;
	for (int i = 0; i < array2.Length; i++)
	{
	    array2[i].enabled = false;
	}
    }

    private void FlingObjects(FlingPrefab fling)
    {
	if (fling.prefab)
	{
	    int num = Random.Range(fling.minAmount, fling.maxAmount + 1);
	    for (int i = 1; i <= num; i++)
	    {
		GameObject gameObject = fling.Spawn();
		gameObject.transform.position += new Vector3(fling.originVariation.x * Random.Range(-1f, 1f), fling.originVariation.y * Random.Range(-1f, 1f));
		float num2 = Random.Range(fling.minSpeed, fling.maxSpeed);
		float num3 = Random.Range(fling.minAngle, fling.maxAngle);
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

    public void SetScuttlerAmount(int amount)
    {
	foreach (FlingPrefab flingPrefab in flingPrefabs)
	{
	    if(flingPrefab.prefab.name == "Health Scuttler")
	    {
		FlingPrefab flingPrefab2 = flingPrefab;
		flingPrefab.maxAmount = amount;
		flingPrefab.minAmount = amount;
		flingPrefab.SetupPool(transform);
		return;
	    }
	}
    }

    [System.Serializable]
    public class FlingPrefab
    {
	public GameObject prefab;
	public List<GameObject> pool = new List<GameObject>();
	public int minAmount;
	public int maxAmount;
	public Vector2 originVariation = new Vector2(0.5f, 0.5f);
	public int minSpeed;
	public int maxSpeed;
	public float minAngle;
	public float maxAngle;

	public void SetupPool(Transform parent)
	{
	    if (prefab)
	    {
		pool.Capacity = maxAmount;
		for (int i = 0; i < pool.Count; i++)
		{
		    GameObject gameObject = Instantiate(prefab, parent);
		    gameObject.transform.localPosition = Vector3.zero;
		    gameObject.SetActive(false);
		    pool.Add(gameObject);
		}
	    }
	}

	public GameObject Spawn()
	{
	    foreach (GameObject gameObject in pool)
	    {
		if (!gameObject.activeSelf)
		{
		    gameObject.SetActive(true);
		    return gameObject;
		}
	    }
	    return null;
	}
    }
}
