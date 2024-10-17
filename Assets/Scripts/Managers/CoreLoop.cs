using System;
using System.Collections.Generic;
using UnityEngine;

public class CoreLoop : MonoBehaviour
{
    private static CoreLoop instance;
    private static List<Action> invokeNextActions;
    private static List<Action> invokeNextActionsBuffer;
    private static bool isFiringInvokeNext;
    private static List<DelayedInvoke> delayedInvokes;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
	GameObject gameObject = new GameObject("CoreLoop");
	instance = gameObject.AddComponent<CoreLoop>();
	DontDestroyOnLoad(gameObject);
	invokeNextActions = new List<Action>();
	invokeNextActionsBuffer = new List<Action>();
	isFiringInvokeNext = false;
	delayedInvokes = new List<DelayedInvoke>();
    }

    public static void InvokeNext(Action action)
    {
	invokeNextActions.Add(action);
	EnqueueInvokeNext();
    }

    private static void EnqueueInvokeNext()
    {
	if (!isFiringInvokeNext)
	{
	    isFiringInvokeNext = true;
	    instance.Invoke("FireInvokeNext", 0f);
	}
    }

    protected void FireInvokeNext()
    {
	isFiringInvokeNext = false;
	for (int i = 0; i < invokeNextActions.Count; i++)
	{
	    invokeNextActionsBuffer.Add(invokeNextActions[i]);
	}
	invokeNextActions.Clear();
	for (int j = 0; j < invokeNextActionsBuffer.Count; j++)
	{
	    Action action = invokeNextActionsBuffer[j];
	    if (action != null)
	    {
		try
		{
		    action();
		}
		catch (Exception exception)
		{
		    Debug.LogException(exception);
		}
	    }
	}
	invokeNextActionsBuffer.Clear();
    }

    protected void Update()
    {
	for (int i = 0; i < delayedInvokes.Count; i++)
	{
	    DelayedInvoke delayedInvoke = delayedInvokes[i];
	    delayedInvoke.TimeRemaining -= Time.unscaledDeltaTime;
	    if (delayedInvoke.TimeRemaining <= 0f)
	    {
		delayedInvokes.RemoveAt(i--);
		InvokeNext(delayedInvoke.Action);
	    }
	}
    }


    private class DelayedInvoke
    {
	public float TimeRemaining;
	public Action Action;
    }
}
