using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRegister : MonoBehaviour
{
    public static Dictionary<string, List<EventRegister>> eventRegister = new Dictionary<string, List<EventRegister>>();
    [SerializeField] public string subscribedEvent = "";

    public delegate void RegisteredEvent();
    public event RegisteredEvent OnReceivedEvent;

    private void Awake()
    {
	SubscribeEvent(this);
    }

    private void OnDestroy()
    {
	UnsubscribeEvent(this);
    }

    public static void SendEvent(string eventName)
    {
	if (eventName == "")
	{
	    return;
	}
	if (eventRegister.ContainsKey(eventName))
	{
	    foreach (EventRegister eventRegister in eventRegister[eventName])
	    {
		eventRegister.ReceiveEvent();
	    }
	}
    }

    public void ReceiveEvent()
    {
	FSMUtility.SendEventToGameObject(gameObject, subscribedEvent, false);
	if (this.OnReceivedEvent != null)
	{
	    this.OnReceivedEvent();
	}
    }

    public static void SubscribeEvent(EventRegister register)
    {
	string key = register.subscribedEvent;
	List<EventRegister> list;
	if (eventRegister.ContainsKey(key))
	{
	    list = eventRegister[key];
	}
	else
	{
	    list = new List<EventRegister>();
	    eventRegister.Add(key, list);
	}
	list.Add(register);
    }

    public static void UnsubscribeEvent(EventRegister register)
    {
	string key = register.subscribedEvent;
	if (eventRegister.ContainsKey(key))
	{
	    List<EventRegister> list = eventRegister[key];
	    if (list.Contains(register))
	    {
		list.Remove(register);
	    }
	    if (list.Count <= 0)
	    {
		eventRegister.Remove(key);
	    }
	}
    }

}
