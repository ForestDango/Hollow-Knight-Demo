using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GlobControl : MonoBehaviour
{
    public Renderer rend;
    [Space]
    public float minScale = 0.6f;
    public float maxScale = 1.6f;

    [Space]
    public string landAnim = "Glob Land";
    public string wobbleAnim = "Glob Wobble";
    public string breakAnim = "Glob Break";

    [Space]
    public AudioSource audioPlayerPrefab;
    public AudioEvent breakSound;
    public Color bloodColorOverride = new Color(1f, 0.537f, 0.188f);

    [Space]
    public GameObject splatChild;

    [Space]
    public Collider2D groundCollider;

    private bool landed;
    private bool broken;
    private tk2dSpriteAnimator anim;

    private void Awake()
    {
	anim = GetComponent<tk2dSpriteAnimator>();
    }

    private void OnEnable()
    {
	float num = Random.Range(minScale, maxScale);
	transform.localScale = new Vector3(num, num, 1f);
	if (splatChild)
	{
	    splatChild.SetActive(false);
	}
	landed = false;
	broken = false;
    }

    private void Start()
    {
	CollisionEnterEvent collision = GetComponent<CollisionEnterEvent>();
	if (collision)
	{
	    collision.OnCollisionEnteredDirectional += delegate (CollisionEnterEvent.Direction direction, Collision2D col)
	    {
		if (!landed)
		{
		    if(direction == CollisionEnterEvent.Direction.Bottom)
		    {
			landed = true;
			collision.doCollisionStay = false;
			if (CheckForGround()) //检测是否碰到地面
			{
			    anim.Play(landAnim);
			    return;
			}
			StartCoroutine(Break());
			return;
		    }
		    else
		    {
			collision.doCollisionStay = true;
		    }
		}
	    };
	}
	TriggerEnterEvent componentInChildren = GetComponentInChildren<TriggerEnterEvent>();
	if (componentInChildren)
	{
	    componentInChildren.OnTriggerEntered += delegate (Collider2D col, GameObject sender)
	    {
		if (!landed || broken)
		{
		    return;
		}
		if (col.gameObject.layer == LayerMask.NameToLayer("Enemies"))
		{
		    anim.Play(wobbleAnim); //检测如果敌人碰到了执行动画wobbleAnim
		}
	    };
	}
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
	if (!landed || broken)
	{
	    return;
	}
	if (col.tag == "Nail Attack") //如果是骨钉攻击，执行break函数
	{
	    StartCoroutine(Break());
	    return;
	}
	if (col.tag == "HeroBox") //如果是玩家碰到了执行wobble的动画
	{
	    anim.Play(wobbleAnim);
	}
    }

    private IEnumerator Break()
    {
	broken = true;
	breakSound.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	GlobalPrefabDefaults.Instance.SpawnBlood(transform.position, 4, 5, 5f, 20f, 80f, 100f, new Color?(bloodColorOverride));
	if (splatChild)
	{
	    splatChild.SetActive(true); //生成一些效果和子对象splatChild
	}
	yield return anim.PlayAnimWait(breakAnim);
	if (rend)
	{
	    rend.enabled = false;
	}
	yield break;
    }

    private bool CheckForGround()
    {
	if (!groundCollider)
	{
	    return true;
	}
	Vector2 vector = groundCollider.bounds.min;
	Vector2 vector2 = groundCollider.bounds.max;
	float num = vector2.y - vector.y;
	vector.y = vector2.y;
	vector.x += 0.1f;
	vector2.x -= 0.1f;
	RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, Vector2.down, num + 0.25f, LayerMask.GetMask("Terrain"));
	RaycastHit2D raycastHit2D2 = Physics2D.Raycast(vector2, Vector2.down, num + 0.25f, LayerMask.GetMask("Terrain"));
	return raycastHit2D.collider != null && raycastHit2D2.collider != null;
    }

}
