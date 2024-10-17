using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEnterEvent : MonoBehaviour
{
    public bool checkDirection;
    public bool ignoreTriggers;
    public int otherLayer = 9;
    [HideInInspector]
    public bool doCollisionStay;

    private Collider2D col2d;
    private const float RAYCAST_LENGTH = 0.08f;
    private List<Vector2> topRays;
    private List<Vector2> rightRays;
    private List<Vector2> bottomRays;
    private List<Vector2> leftRays;

    public delegate void CollisionEvent(Collision2D collision, GameObject sender);
    public event CollisionEvent OnCollisionEntered;

    public delegate void DirectionalCollisionEvent(Direction direction, Collision2D collision);
    public event DirectionalCollisionEvent OnCollisionEnteredDirectional;

    private void Awake()
    {
	col2d = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
	HandleCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
	if (doCollisionStay)
	{
	    HandleCollision(collision);
	}
    }

    private void HandleCollision(Collision2D collision)
    {
	if (OnCollisionEntered != null)
	{
	    OnCollisionEntered(collision, gameObject);
	}
	if (checkDirection)
	{
	    CheckTouching(otherLayer, collision);
	}
    }

    private void CheckTouching(int layer, Collision2D collision)
    {
	topRays = new List<Vector2>();
	topRays.Add(new Vector2(col2d.bounds.min.x, col2d.bounds.max.y));
	topRays.Add(new Vector2(col2d.bounds.center.x, col2d.bounds.max.y));
	topRays.Add(col2d.bounds.max);
	rightRays = new List<Vector2>();
	rightRays.Add(col2d.bounds.max);
	rightRays.Add(new Vector2(col2d.bounds.max.x, col2d.bounds.center.y));
	rightRays.Add(new Vector2(col2d.bounds.max.x, col2d.bounds.min.y));
	bottomRays = new List<Vector2>();
	bottomRays.Add(new Vector2(col2d.bounds.max.x, col2d.bounds.min.y));
	bottomRays.Add(new Vector2(col2d.bounds.center.x, col2d.bounds.min.y));
	bottomRays.Add(col2d.bounds.min);
	leftRays = new List<Vector2>();
	leftRays.Add(col2d.bounds.min);
	leftRays.Add(new Vector2(col2d.bounds.min.x, col2d.bounds.center.y));
	leftRays.Add(new Vector2(col2d.bounds.min.x, col2d.bounds.max.y));
	foreach (Vector2 v in topRays)
	{
	    RaycastHit2D raycastHit2D = Physics2D.Raycast(v, Vector2.up, RAYCAST_LENGTH, 1 << layer);
	    if (raycastHit2D.collider != null && (!ignoreTriggers || !raycastHit2D.collider.isTrigger))
	    {
		if (OnCollisionEnteredDirectional != null)
		{
		    OnCollisionEnteredDirectional(Direction.Top, collision);
		    break;
		}
		break;
	    }
	}
	foreach (Vector2 v2 in rightRays)
	{
	    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(v2, Vector2.right, RAYCAST_LENGTH, 1 << layer);
	    if (raycastHit2D2.collider != null && (!ignoreTriggers || !raycastHit2D2.collider.isTrigger))
	    {
		if (OnCollisionEnteredDirectional != null)
		{
		    OnCollisionEnteredDirectional(Direction.Right, collision);
		    break;
		}
		break;
	    }
	}
	foreach (Vector2 v3 in bottomRays)
	{
	    RaycastHit2D raycastHit2D3 = Physics2D.Raycast(v3, -Vector2.up, RAYCAST_LENGTH, 1 << layer);
	    if (raycastHit2D3.collider != null && (!ignoreTriggers || !raycastHit2D3.collider.isTrigger))
	    {
		if (OnCollisionEnteredDirectional != null)
		{
		    OnCollisionEnteredDirectional(Direction.Bottom, collision);
		    break;
		}
		break;
	    }
	}
	foreach (Vector2 v4 in leftRays)
	{
	    RaycastHit2D raycastHit2D4 = Physics2D.Raycast(v4, -Vector2.right, RAYCAST_LENGTH, 1 << layer);
	    if (raycastHit2D4.collider != null && (!ignoreTriggers || !raycastHit2D4.collider.isTrigger))
	    {
		if (OnCollisionEnteredDirectional != null)
		{
		    OnCollisionEnteredDirectional(Direction.Left, collision);
		    break;
		}
		break;
	    }
	}
    }

    public enum Direction
    {
	Left,
	Right,
	Top,
	Bottom
    }
}
