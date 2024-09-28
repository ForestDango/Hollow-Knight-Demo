using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AlertRange : MonoBehaviour
{
    private bool isHeroInRange;
    private Collider2D[] colliders;

    public bool IsHeroInRange
    {
	get
	{
	    return isHeroInRange;
	}
    }

    protected void Awake()
    {
	colliders = GetComponents<Collider2D>();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
	isHeroInRange = true;
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
	if(colliders.Length <= 1 || !StillInColliders())
	{
	    isHeroInRange = false;
	}
    }

    private bool StillInColliders()
    {
	bool flag = false;
	foreach (Collider2D collider2D in colliders)
	{
	    if (collider2D is CircleCollider2D)
	    {
		CircleCollider2D circleCollider2D = (CircleCollider2D)collider2D;
		flag = Physics2D.OverlapCircle(transform.TransformPoint(circleCollider2D.offset), circleCollider2D.radius * Mathf.Max(transform.localScale.x, transform.localScale.y),LayerMask.GetMask("Player")) != null;
	    }
	    else if (collider2D is BoxCollider2D)
	    {
		BoxCollider2D boxCollider2D = (BoxCollider2D)collider2D;
		flag = Physics2D.OverlapBox(transform.TransformPoint(boxCollider2D.offset), new Vector2(boxCollider2D.size.x * transform.localScale.x, boxCollider2D.size.y * transform.localScale.y), transform.eulerAngles.z, LayerMask.GetMask("Player")) != null;
	    }
	    if (flag)
	    {
		break;
	    }
	}
	return flag;
    }

    public static AlertRange Find(GameObject root,string childName)
    {
	if (root == null)
	    return null;
	bool flag = !string.IsNullOrEmpty(childName);
	Transform transform = root.transform;
	for (int i = 0; i < transform.childCount; i++)
	{
	    Transform child = transform.GetChild(i);
	    AlertRange component = child.GetComponent<AlertRange>();
	    if(component != null && (!flag || !(child.gameObject.name != childName)))
	    {
		return component;
	    }
	}
	return null;
    }

}
