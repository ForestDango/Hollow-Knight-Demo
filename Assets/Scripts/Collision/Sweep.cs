using System;
using UnityEngine;

public struct Sweep
{
    public int CardinalDirection;//基数(1-9的数字)的方向
    public Vector2 Direction;
    public Vector2 ColliderOffset;
    public Vector2 ColliderExtents;
    public float SkinThickness;
    public int RayCount;
    public const float DefaultSkinThickness = 0.1f;
    public const int DefaultRayCount = 3;

    public Sweep(Collider2D collider, int cardinalDirection, int rayCount, float skinThickness = DefaultSkinThickness)
    {
	CardinalDirection = cardinalDirection;
	Direction = new Vector2(DirectionUtils.GetX(cardinalDirection), DirectionUtils.GetY(cardinalDirection));
	ColliderOffset = collider.offset.MultiplyElements(collider.transform.localScale);
	ColliderExtents = collider.bounds.extents;
	RayCount = rayCount;
	SkinThickness = skinThickness;
    }

    public bool Check(Vector2 offset, float distance, int layerMask)
    {
	float num;
	return Check(offset, distance, layerMask, out num);
    }

    public bool Check(Vector2 offset, float distance, int layerMask, out float clippedDistance)
    {
	if (distance <= 0f)
	{
	    clippedDistance = 0f;
	    return false;
	}
	Vector2 a = ColliderOffset + Vector2.Scale(ColliderExtents, Direction);
	Vector2 a2 = Vector2.Scale(ColliderExtents, new Vector2(Mathf.Abs(Direction.y), Mathf.Abs(Direction.x)));
	float num = distance;
	for (int i = 0; i < RayCount; i++)
	{
	    float d = (2f * ((float)i / (RayCount - 1))) - 1f;
	    Vector2 b = a + a2 * d + Direction * -SkinThickness;
	    Vector2 vector = offset + b;
	    RaycastHit2D hit = Physics2D.Raycast(vector, Direction, num + SkinThickness, layerMask);
	    float num2 = hit.distance - SkinThickness;
	    if (hit && num2 < num)
	    {
		num = num2;
		Debug.DrawLine(vector, vector + Direction * hit.distance, Color.red);
	    }
	    else
	    {
		Debug.DrawLine(vector, vector + Direction * (distance + SkinThickness), Color.green);
	    }
	}
	clippedDistance = num;
	return distance - num > Mathf.Epsilon;
    }
}
