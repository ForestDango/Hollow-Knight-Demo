using System;
using UnityEngine;

public static class Collision2DUtils
{
    private static ContactPoint2D[] contactsBuffer;
    static Collision2DUtils()
    {
	contactsBuffer = new ContactPoint2D[1];
    }

    public static Collision2DSafeContact GetSafeContact(this Collision2D collision)
    {
	if(collision.GetContacts(contactsBuffer) >= 1)
	{
	    ContactPoint2D contactPoint2D = contactsBuffer[0];
	    return new Collision2DSafeContact
	    {
		Point = contactPoint2D.point,
		Normal = contactPoint2D.normal,
		IsLegitimate = true
	    };
	}
	Vector2 b = collision.collider.transform.TransformPoint(collision.collider.offset);
	Vector2 a = collision.otherCollider.transform.TransformPoint(collision.otherCollider.offset);
	return new Collision2DSafeContact
	{
	    Point = (a + b) * 0.5f,
	    Normal = (a - b).normalized,
	    IsLegitimate = false
	};
    }

    public struct Collision2DSafeContact
    {
	public Vector2 Point;
	public Vector2 Normal;
	public bool IsLegitimate;
    }
}
