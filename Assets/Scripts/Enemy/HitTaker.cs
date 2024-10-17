using System;
using UnityEngine;

public static class HitTaker
{
    private const int DefaultRecursionDepth = 3;
    public static void Hit(GameObject targetGameObject,HitInstance damageInstance,int recursionDepth = DefaultRecursionDepth)
    {
	if (targetGameObject != null)
	{
	    Transform transform = targetGameObject.transform;
	    //说白了就是检测targetGameObject自己this,父对象parent,爷对象grandparent有没有IHitResponder，有的话执行Hit
	    for (int i = 0; i < recursionDepth; i++) 
	    {
		IHitResponder component = transform.GetComponent<IHitResponder>();
		if(component != null)
		{
		    component.Hit(damageInstance);
		}
		transform = transform.parent;
		if(transform == null)
		{
		    break;
		}
	    }
	}
    }
}
