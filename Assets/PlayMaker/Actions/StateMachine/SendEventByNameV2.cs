using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Sends an Event by name after an optional delay. NOTE: Use this over Send Event if you store events as string variables.")]
    public class SendEventByNameV2 : FsmStateAction
    {
	[Tooltip("Where to send the event.")]
	public FsmEventTarget eventTarget;
	[RequiredField]
	[Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
	public FsmString sendEvent;
	[HasFloatSlider(0f, 10f)]
	[Tooltip("Optional delay in seconds.")]
	public FsmFloat delay;
	[Tooltip("Repeat every frame. Rarely needed.")]
	public bool everyFrame;
	private DelayedEvent delayedEvent;

	public override void Reset()
	{
	    eventTarget = null;
	    sendEvent = null;
	    delay = null;
	    everyFrame = false;
	}

	public override void OnEnter()
	{
	    if (delay.Value < 0.001f)
	    {
		Fsm.Event(eventTarget, sendEvent.Value);
		if (!everyFrame)
		{
		    Finish();
		    return;
		}
	    }
	    else
	    {
		delayedEvent = Fsm.DelayedEvent(eventTarget, FsmEvent.GetFsmEvent(sendEvent.Value), delay.Value);
	    }
	}

	public override void OnUpdate()
	{
	    if (!everyFrame)
	    {
		if (DelayedEvent.WasSent(delayedEvent))
		{
		    Finish();
		    return;
		}
	    }
	    else
	    {
		Fsm.Event(eventTarget, sendEvent.Value);
	    }
	}
    }

}
