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
	    //˵���˾��Ǽ��targetGameObject�Լ�this,������parent,ү����grandparent��û��IHitResponder���еĻ�ִ��Hit
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
