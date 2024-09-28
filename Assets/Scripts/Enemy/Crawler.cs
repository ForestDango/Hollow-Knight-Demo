using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : MonoBehaviour
{
    public float speed;
    [Space]
    public Transform wallCheck; //墙面检测的位置
    public Transform groundCheck; //地面检测的位置
    private Vector2 velocity; //记录速度
    private CrawlerType type;

    private Rigidbody2D body;
    private Recoil recoil;
    private tk2dSpriteAnimator anim;

    private void Awake()
    {
	body = GetComponent<Rigidbody2D>();
	recoil = GetComponent<Recoil>();
	anim = GetComponent<tk2dSpriteAnimator>();
    }

    private void Start()
    {
	float z = transform.eulerAngles.z;
	//通过transform.eulerAngles.z来判断哪种类型的Crawler
	if (z >= 45f && z <= 135f)
	{
	    type = CrawlerType.Wall;
	    velocity = new Vector2(0f, Mathf.Sign(-transform.localScale.x) * speed);
	}
	else if (z >= 135f && z <= 225f)
	{
	    type = ((transform.localScale.y > 0f) ? CrawlerType.Roof : CrawlerType.Floor);
	    velocity = new Vector2(Mathf.Sign(transform.localScale.x) * speed, 0f);
	}
	else if (z >= 225f && z <= 315f)
	{
	    type = CrawlerType.Wall;
	    velocity = new Vector2(0f, Mathf.Sign(transform.localScale.x) * speed);
	}
	else
	{
	    type = ((transform.localScale.y > 0f) ? CrawlerType.Floor : CrawlerType.Roof);
	    velocity = new Vector2(Mathf.Sign(-transform.localScale.x) * speed, 0f);
	}
	recoil.SetRecoilSpeed(0f);
	recoil.OnCancelRecoil += delegate()
	{
	    body.velocity = velocity;
	};
	CrawlerType crawlerType = type;
	if(crawlerType != CrawlerType.Floor)
	{
	    if(crawlerType - CrawlerType.Roof <= 1)
	    {
		body.gravityScale = 0;//如果在墙面面上rb2d的重力就设置为1
		recoil.freezeInPlace = true;	     
	    }
	}
	else
	{
	    body.gravityScale = 1; //如果在地面上rb2d的重力就设置为1
	    recoil.freezeInPlace = false;
	}
	StartCoroutine(nameof(Walk));
    }

    /// <summary>
    /// 使用协程实现Walk函数，循环直至hit=true后挂起然后启用协程Turn()
    /// </summary>
    /// <returns></returns>
    private IEnumerator Walk()
    {
	for(; ; )
	{
	    anim.Play("Walk");
	    body.velocity = velocity;
	    bool hit = false;
	    while (!hit)
	    {
		if(CheckRayLocal(wallCheck.localPosition,(transform.localScale.x > 0f )? Vector2.left : Vector2.right, 1f))
		{
		    hit = true;
		    break;
		}
		if (CheckRayLocal(groundCheck.localPosition, (transform.localScale.y > 0f) ? Vector2.down : Vector2.up, 1f))
		{
		    hit = true;
		    break;
		}
		yield return null;
	    }
	    yield return StartCoroutine(Turn());
	    yield return null;
	}
    }

    /// <summary>
    /// 使用协程实现转向函数
    /// </summary>
    /// <returns></returns>
    private IEnumerator Turn()
    {
	body.velocity = Vector2.zero;
	yield return StartCoroutine(anim.PlayAnimWait("Turn"));
	transform.SetScaleX(transform.localScale.x * -1f);
	velocity.x = velocity.x * -1f;
	velocity.y = velocity.y * -1f;
    }

    /// <summary>
    /// 发射射线，检测是否有LayerMask.GetMask("Terrain").collider
    /// </summary>
    /// <param name="originLocal"></param>
    /// <param name="directionLocal"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public bool CheckRayLocal(Vector3 originLocal, Vector2 directionLocal, float length)
    {
	Vector2 vector = transform.TransformPoint(originLocal);
	Vector2 vector2 = transform.TransformDirection(directionLocal);
	RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, vector2, length, LayerMask.GetMask("Terrain"));
	Debug.DrawLine(vector, vector + vector2 * length);
	return raycastHit2D.collider != null;
    }

    private enum CrawlerType
    {
	Floor,
	Roof,
	Wall
    }
}
