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

    public static PlayMakerFSM LocateFSM(GameObject go, string fsmName)
    {
	if (go == null)
	{
	    return null;
	}
	List<PlayMakerFSM> list = ObtainFsmList();
	go.GetComponents<PlayMakerFSM>(list);
	PlayMakerFSM result = null;
	for (int i = 0; i < list.Count; i++)
	{
	    PlayMakerFSM playMakerFSM = list[i];
	    if (playMakerFSM.FsmName == fsmName)
	    {
		result = playMakerFSM;
		break;
	    }
	}
	ReleaseFsmList(list);
	return result;
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

    public static bool ContainsFSM(GameObject go, string fsmName)
    {
	if (go == null)
	{
	    return false;
	}
	List<PlayMakerFSM> list = ObtainFsmList();
	go.GetComponents<PlayMakerFSM>(list);
	bool result = false;
	for (int i = 0; i < list.Count; i++)
	{
	    if (list[i].FsmName == fsmName)
	    {
		result = true;
		break;
	    }
	}
	ReleaseFsmList(list);
	return result;
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
    public static int GetInt(PlayMakerFSM fsm, string variableName)
    {
	return fsm.FsmVariables.FindFsmInt(variableName).Value;
    }

    public static Vector3 GetVector3(PlayMakerFSM fsm, string variableName)
    {
	return fsm.FsmVariables.FindFsmVector3(variableName).Value;
    }
    public static void SetBool(PlayMakerFSM fsm, string variableName, bool value)
    {
	fsm.FsmVariables.GetFsmBool(variableName).Value = value;
    }
    public static void SetInt(PlayMakerFSM fsm, string variableName, int value)
    {
	fsm.FsmVariables.GetFsmInt(variableName).Value = value;
    }

    public static void SetFloat(PlayMakerFSM fsm, string variableName, float value)
    {
	fsm.FsmVariables.GetFsmFloat(variableName).Value = value;
    }

    public static void SetString(PlayMakerFSM fsm, string variableName, string value)
    {
	fsm.FsmVariables.GetFsmString(variableName).Value = value;
    }
    public abstract class CheckFsmStateAction : FsmStateAction
    {
	public FsmEvent trueEvent;
	public FsmEvent falseEvent;
	public abstract bool IsTrue { get; }

	public override void Reset()
	{
	    trueEvent = null;
	    falseEvent = null;
	}

	public override void OnEnter()
	{
	    if (IsTrue)
	    {
		Fsm.Event(trueEvent);
	    }
	    else
	    {
		Fsm.Event(falseEvent);
	    }
	    Finish();
	}
    }
}
