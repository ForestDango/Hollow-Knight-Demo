using System;
using UnityEngine;

public static class FSMActionReplacements
{
    public static void SetMaterialColor(Component me, Color color)
    {
	Renderer component = me.GetComponent<Renderer>();
	if (component != null)
	{
	    component.material.color = color;
	}
    }

    public static Directions CheckDirectionWithBrokenBehaviour(float angle)
    {
	if (angle < 45f)
	{
	    return Directions.Right;
	}
	if (angle < 135f)
	{
	    return Directions.Up;
	}
	if (angle < 225f)
	{
	    return Directions.Left;
	}
	if (angle < 360f)
	{
	    return Directions.Down;
	}
	return Directions.Unknown;
    }

    public enum Directions
    {
	Right,
	Up,
	Left,
	Down,
	Unknown
    }
}
