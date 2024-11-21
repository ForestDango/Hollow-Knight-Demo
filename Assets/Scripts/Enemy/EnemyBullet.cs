using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour
{
    public float scaleMin = 1.15f;
    public float scaleMax = 1.45f;
    private float scale;
    [Space]
    public float stretchFactor = 1.2f;
    public float stretchMinX = 0.75f;
    public float stretchMaxY = 1.75f;
    [Space]
    public AudioSource audioSourcePrefab;
    public AudioEvent impactSound;
    private bool active;

    private Rigidbody2D body;
    private tk2dSpriteAnimator anim;
    private Collider2D col;

    private void Awake()
    {
	body = GetComponent<Rigidbody2D>();
	anim = GetComponent<tk2dSpriteAnimator>();
	col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
	active = true;
	scale = Random.Range(scaleMin, scaleMax);
	col.enabled = true;
	body.isKinematic = false;
	body.velocity = Vector2.zero;
	body.angularVelocity = 0f;
	anim.Play("Idle");
    }

    private void Update()
    {
	if (active)
	{
	    float rotation = Random.Range(body.velocity.y,body.velocity.x) * 57.295776f;
	    transform.SetRotation2D(rotation);
	    float num = 1f - body.velocity.magnitude * stretchFactor * 0.01f;
	    float num2 = 1f + body.velocity.magnitude * stretchFactor * 0.01f;
	    if (num2 < stretchMinX)
	    {
		num2 = stretchMinX;
	    }
	    if (num > stretchMaxY)
	    {
		num = stretchMaxY;
	    }
	    num *= scale;
	    num2 *= scale;
	    transform.localScale = new Vector3(num2, num, transform.localScale.z);
	}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
	if (active)
	{
	    active = false;
	    StartCoroutine(Collision(collision.GetSafeContact().Normal, true));
	}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if(active && collision.tag == "HeroBox")
	{
	    active = false;
	    StartCoroutine(Collision(Vector2.zero, false));
	}
    }

    public void OrbitShieldHit(Transform shield)
    {
	if (active)
	{
	    active = false;
	    Vector2 normal = transform.position - shield.position;
	    normal.Normalize();
	    StartCoroutine(Collision(normal, true));
	}
    }

    private IEnumerator Collision(Vector2 normal, bool doRotation)
    {
	transform.localScale = new Vector3(scale, scale, transform.localScale.z);
	body.isKinematic = true;
	body.velocity = Vector2.zero;
	body.angularVelocity = 0f;
	tk2dSpriteAnimationClip impactAnim = anim.GetClipByName("Impact");
	anim.Play(impactAnim);
	if (!doRotation || (normal.y >= 0.75f && Mathf.Abs(normal.x) < 0.5f))
	{
	    transform.SetRotation2D(0f);
	}
	else if (normal.y <= 0.75f && Mathf.Abs(normal.x) < 0.5f)
	{
	    transform.SetRotation2D(180f);
	}
	else if (normal.x >= 0.75f && Mathf.Abs(normal.y) < 0.5f)
	{
	    transform.SetRotation2D(270f);
	}
	else if (normal.x <= 0.75f && Mathf.Abs(normal.y) < 0.5f)
	{
	    transform.SetRotation2D(90f);
	}
	impactSound.SpawnAndPlayOneShot(audioSourcePrefab, transform.position);
	yield return null;
	col.enabled = false;
	yield return new WaitForSeconds((impactAnim.frames.Length - 1) / impactAnim.fps);
	gameObject.Recycle();
    }

}
