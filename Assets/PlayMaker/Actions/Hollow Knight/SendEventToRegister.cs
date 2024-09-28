using System;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class SendEventToRegister : FsmStateAction
{
    public FsmString eventName;

    public override void Reset()
    {
	eventName = new FsmString();
    }

    public override void OnEnter()
    {
	if(eventName.Value != "")
	{
	    EventRegister.SendEvent(eventName.Value);
	}
	base.Finish();
    }
}
