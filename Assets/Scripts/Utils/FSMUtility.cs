using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public static class FSMUtility
{
    private static List<List<PlayMakerFSM>> fsmListPool;
    private const int FsmListPoolSizeMax = 20;

    static FSMUtility()
    {
	fsmListPool = new List<List<PlayMakerFSM>>();
    }

    private static List<PlayMakerFSM> ObtainFsmList()
    {
	if (fsmListPool.Count > 0)
	{
	    List<PlayMakerFSM> result = fsmListPool[fsmListPool.Count - 1];
	    fsmListPool.RemoveAt(fsmListPool.Count - 1);
	    return result;
	}
	return new List<PlayMakerFSM>();
    }

    private static void ReleaseFsmList(List<PlayMakerFSM> fsmList)
    {
	fsmList.Clear();
	if (fsmListPool.Count < FsmListPoolSizeMax)
	{
	    fsmListPool.Add(fsmList);
	}
    }

    public static PlayMakerFSM GetFSM(GameObject go)
    {
	return go.GetComponent<PlayMakerFSM>();
    }

    public static GameObject GetSafe(this FsmOwnerDefault ownerDefault, FsmStateAction stateAction)
    {
	if (ownerDefault.OwnerOption == OwnerDefaultOption.UseOwner)
	{
	    return stateAction.Owner;
	}
	return ownerDefault.GameObject.Value;
    }

    public static void SendEventToGameObject(GameObject go, string eventName, bool isRecursive = false)
    {
	if (go != null)
	{
	    SendEventToGameObject(go, FsmEvent.FindEvent(eventName), isRecursive);
	}
    }

    public static void SendEventToGameObject(GameObject go, FsmEvent ev, bool isRecursive = false)
    {
	if (go != null)
	{
	    List<PlayMakerFSM> list = ObtainFsmList();
	    go.GetComponents<PlayMakerFSM>(list);
	    for (int i = 0; i < list.Count; i++)
	    {
		list[i].Fsm.Event(ev);
	    }
	    ReleaseFsmList(list);
	    if (isRecursive)
	    {
		Transform transform = go.transform;
		for (int j = 0; j < transform.childCount; j++)
		{
		    SendEventToGameObject(transform.GetChild(j).gameObject, ev, isRecursive);
		}
	    }
	}
    }

}
