using System;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public float attackMagnitude = 6f;
    [Space]
    public Direction right;
    public Direction left;
    public Direction up;
    public Direction down;

    [Space]
    public GameObject objectNailEffectPrefab;
    public GameObject midpointNailEffectPrefab;
    public GameObject spellHitEffectPrefab;

    private AudioSource source;
    private bool activated;
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
	int num = (int)Mathf.Sign(transform.position.x - collision.transform.position.x);
	float num2 = 1f;
	Direction direction = null;
	if (collision.tag == "Nail Attack")
	{
	    flag = true;
	    num2 = attackMagnitude;
	    if (objectNailEffectPrefab)
	    {
		GameObject gameObject = objectNailEffectPrefab.Spawn(transform.position);
		Vector3 localScale = gameObject.transform.localScale;
		localScale.x = Mathf.Abs(localScale.x) * num;
		gameObject.transform.localScale = localScale;
	    }
	    if (midpointNailEffectPrefab)
	    {
		GameObject gameObject2 = midpointNailEffectPrefab.Spawn((collision.transform.position + transform.position) / 2f);
		Vector3 localScale2 = gameObject2.transform.localScale;
		localScale2.x = Mathf.Abs(localScale2.x) * num;
		gameObject2.transform.localScale = localScale2;
	    }
	    float value = PlayMakerFSM.FindFsmOnGameObject(collision.gameObject, "damages_enemy").FsmVariables.FindFsmFloat("direction").Value;
	    if (value < 45f)
	    {
		direction = right;
	    }
	    else if (value < 135f)
	    {
		direction = up;
	    }
	    else if (value < 225f)
	    {
		direction = left;
	    }
	    else if (value < 360f)
	    {
		direction = down;
	    }
	    if (direction != null && direction.effectPrefab)
	    {
		GameObject gameObject3 = direction.effectPrefab.Spawn(transform.position);
		if (gameObject3)
		{
		    gameObject3.transform.localScale = direction.scale;
		    gameObject3.transform.localEulerAngles = direction.rotation;
		}
	    }
	}
	else if (collision.tag == "Hero Spell")
	{
	    flag = true;
	    if (spellHitEffectPrefab)
	    {
		spellHitEffectPrefab.Spawn(transform.position);
	    }
	    else
	    {
		Debug.Log("No spell hit effect assigned to: " + gameObject.name);
	    }
	}
    }

    [Serializable]
    public class Direction
    {
	public GameObject effectPrefab;
	public Vector3 scale = Vector3.one;
	public Vector3 rotation;

	[Space]
	public float minFlingSpeed = 4f;
	public float maxFlingSpeed = 4f;
	public float minFlingAngle = 5f;
	public float maxFlingAngle = 5f;
    }
}
