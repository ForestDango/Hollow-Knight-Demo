using System;
using UnityEngine;

public static class DirectionUtils
{
    public const int Right = 0;
    public const int Up = 1;
    public const int Left = 2;
    public const int Down = 3;

    public static int GetCardinalDirection(float degrees)
    {
	return NegSafeMod(Mathf.RoundToInt(degrees / 90f), 4); //取一个最接近degrees / 90的整数，再获取它最靠近的方向
    }

    public static int NegSafeMod(int val, int len)
    {
	return (val % len + len) % len;
    }

    public static int GetX(int cardinalDirection)
    {
	int num = cardinalDirection % 4;
	if (num == 0)
	{
	    return Up;
	}
	if (num != 2)
	{
	    return Right;
	}
	return -1;
    }

    public static int GetY(int cardinalDirection)
    {
	int num = cardinalDirection % 4;
	if (num == 1)
	{
	    return Up;
	}
	if (num != 3)
	{
	    return Right;
	}
	return -1;
    }


}
