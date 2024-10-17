using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePole : MonoBehaviour,IHitResponder
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite brokenSprite;
    [SerializeField] private float inertBackgroundThreshold;
    [SerializeField] private float inertForegroundThreshold;
    [SerializeField] private AudioSource audioSourcePrefab;
    [SerializeField] private RandomAudioClipTable hitClip;
    [SerializeField] private GameObject slashImpactPrefab;
    [SerializeField] private Rigidbody2D top;

    protected void Reset()
    {
	inertBackgroundThreshold = -1f;
	inertForegroundThreshold = -1f;
    }

    protected void Start()
    {
	float z = transform.position.z;
	if(z < inertBackgroundThreshold || z > inertForegroundThreshold)
	{
	    enabled = false;
	    return;
	}
    }

    public void Hit(HitInstance damageInstance)
    {
	int cardinalDirection = DirectionUtils.GetCardinalDirection(damageInstance.Direction);
	if (cardinalDirection != 2 && cardinalDirection != 0)
	{
	    return;
	}
	spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,0f);
	Transform transform = Instantiate(slashImpactPrefab).transform;
	transform.eulerAngles = new Vector3(0f, 0f, Random.Range(340f, 380f));
	Vector3 localScale = transform.localScale;
	localScale.x = ((cardinalDirection == 2) ? -1f : 1f);
	localScale.y = 1f;
	hitClip.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
	if (top != null)
	{
	    top.gameObject.SetActive(true);
	    float num = (cardinalDirection == 2) ? Random.Range(120, 140) : Random.Range(40, 60);
	    top.transform.localScale = new Vector3(localScale.x, localScale.y, top.transform.localScale.z);
	    top.velocity = new Vector2(Mathf.Cos(num * 0.017453292f), Mathf.Sin(num * 0.017453292f)) * 5f;
	    top.transform.Rotate(new Vector3(0f, 0f, num));
	    base.enabled = false;
	}
    }
}
