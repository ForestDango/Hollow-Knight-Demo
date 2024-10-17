using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class Helper
{
    public static int GetCollidingLayerMaskForLayer(int layer)
    {
	int num = 0;
	for (int i = 0; i < 32; i++)
	{
	    if (!Physics2D.GetIgnoreLayerCollision(layer, i))
	    {
		num |= 1 << i;
	    }
	}
	return num;
    }
}
