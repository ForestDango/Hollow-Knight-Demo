using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class RestoreGameObjectPositions : FsmStateAction
{
    private Dictionary<GameObject, Vector3> positions;


    public override void Reset()
    {
	base.Reset();
	positions = null;
    }

    public override void OnEnter()
    {
	base.OnEnter();
	if (positions == null)
	{
	    positions = new Dictionary<GameObject, Vector3>(Owner.transform.childCount);
	    IEnumerator enumerator = Owner.transform.GetEnumerator();
	    while (enumerator.MoveNext())
	    {
		object obj = enumerator.Current;
		Transform transform = (Transform)obj;
		positions.Add(transform.gameObject, transform.localPosition);
	    }
	    goto IL_C2; 
	}
	foreach (KeyValuePair<GameObject, Vector3> keyValuePair in positions)
	{
	    keyValuePair.Key.transform.localPosition = keyValuePair.Value;
	}
	IL_C2:
	Finish();
    }
}
