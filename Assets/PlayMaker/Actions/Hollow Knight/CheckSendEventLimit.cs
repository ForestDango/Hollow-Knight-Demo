using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class CheckSendEventLimit : FsmStateAction
{
    public FsmGameObject gameObject;
    public FsmEventTarget target;
    public FsmEvent trueEvent;
    public FsmEvent falseEvent;

    public override void Reset()
    {
	gameObject = null;
	target = null;
	trueEvent = null;
	falseEvent = null;
    }

    public override void OnEnter()
    {
	if (gameObject.Value)
	{
	    LimitSendEvents component = Owner.gameObject.GetComponent<LimitSendEvents>(); 
	    if(component && !component.Add(gameObject.Value))
	    {
		Fsm.Event(target, falseEvent);
	    }
	    else
	    {
		Fsm.Event(target, trueEvent);
	    }
	}
	base.Finish();
    }

}
