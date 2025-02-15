using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatterOrange : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public CircleCollider2D circleCollider;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    private float stretchFactor = 1.4f;
    private float stretchMinX = 0.6f;
    private float stretchMaxY = 1.75f;
    private float scaleModifier;
    public float scaleModifierMin = 0.7f;
    public float scaleModifierMax = 1.3f;
    public float splashScaleMin = 1.5f;
    public float splashScaleMax = 2f;
    private float state;
    public float fps = 30f;
    private float idleTimer;
    private float animTimer;
    private int animFrame;

    private void Start()
    {
	scaleModifier = Random.Range(scaleModifierMin, scaleModifierMax);
    }

    private void OnEnable()
    {
	rb2d.isKinematic = true;
	circleCollider.enabled = true;
	idleTimer = 0f;
	animTimer = 0f;
	spriteRenderer.sprite = sprites[0];
	animFrame = 1;
	state = 0f;
    }

    private void Update()
    {
	if(state == 0f)
	{
	    FaceAngle();
	    ProjectileSquash();
	    idleTimer += Time.deltaTime;
	    if(idleTimer > 3f)
	    {
		Impact();
	    }
	}
	if(state == 1f)
	{
	    animTimer += Time.deltaTime;
	    if(animTimer >= 1f / fps)
	    {
		animTimer = 0f;
		animFrame++;
		if(animFrame > 6)
		{
		    gameObject.Recycle();
		    return;
		}
		spriteRenderer.sprite = sprites[animFrame];
	    }
	}
    }

    private void Impact()
    {
	float num = Random.Range(splashScaleMin, splashScaleMax);
	transform.localScale = new Vector2(num, num);
	circleCollider.enabled = false;
	rb2d.isKinematic = true;
	rb2d.velocity = Vector2.zero;
	spriteRenderer.sprite = sprites[1];
	state = 1f;
    }

    private void ProjectileSquash()
    {
	float num = 1f - rb2d.velocity.magnitude * stretchFactor * 0.01f;
	float num2 = 1f + rb2d.velocity.magnitude * stretchFactor * 0.01f;
	if(num2 < stretchMinX)
	{
	    num2 = stretchMinX;
	}
	if(num > stretchMaxY)
	{
	    num = stretchMaxY;
	}
	num *= scaleModifier;
	num2 *= scaleModifier;
	transform.localScale = new Vector3(num2, num, transform.localScale.z);
    }

    private void FaceAngle()
    {
	Vector2 velocity = rb2d.velocity;
	float z = Mathf.Atan2(velocity.y, velocity.x) * 57.295776f;
	transform.localEulerAngles = new Vector3(0f, 0f, z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
	Collision2DUtils.Collision2DSafeContact safeContact = collision.GetSafeContact();
	float x = safeContact.Normal.x;
	float y = safeContact.Normal.y;
	if (y == -1f)
	{
	    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 180f);
	}
	else if (y == 1f)
	{
	    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
	}
	else if (x == 1f)
	{
	    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 270f);
	}
	else if (x == -1f)
	{
	    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 90f);
	}
	else
	{
	    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + 90f);
	}
	Impact();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
	transform.position = new Vector3(transform.position.x, transform.position.y + 0.65f, transform.position.z);
	Impact();
    }
}
