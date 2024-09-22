using System;
using UnityEngine;

public class HitTaker : MonoBehaviour
{
    private const int DefaultRecursionDepth = 3;
    public static void Hit(GameObject targetGameObject,HitInstance damageInstance,int recursionDepth = DefaultRecursionDepth)
    {
	if (targetGameObject != null)
	{
	    Transform transform = targetGameObject.transform;
	    for (int i = 0; i < recursionDepth; i++) //this,parent,grandparent
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
