using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitSendEvents : MonoBehaviour
{
    public Collider2D monitorCollider;
    private List<GameObject> sentList = new List<GameObject>();
    private bool? previousColliderState;

    private void OnEnable()
    {
	sentList.Clear();
    }

    private void Update()
    {
	if (monitorCollider)
	{
	    bool enabled = monitorCollider.enabled;
	    bool? flag = previousColliderState;
	    if(enabled == flag.GetValueOrDefault() && flag != null)
	    {
		return;
	    }
	    previousColliderState = new bool?(monitorCollider.enabled);
	}
	if(sentList.Count > 0)
	{
	    sentList.Clear();
	}
    }

    public bool Add(GameObject obj)
    {
	if (!sentList.Contains(obj))
	{
	    sentList.Add(obj);
	    return true;
	}
	return false;
    }

}
