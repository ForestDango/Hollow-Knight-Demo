using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class CompareNames : FsmStateAction
{
    public FsmString name;
    [ArrayEditor(VariableType.String, "", 0, 0, 65536)]
    public FsmArray strings;
    public FsmEventTarget target;
    public FsmEvent trueEvent;
    public FsmEvent falseEvent;

    public override void Reset()
    {
	name = new FsmString();
	target = new FsmEventTarget();
	strings = new FsmArray();
	trueEvent = null;
	falseEvent = null;
    }

    public override void OnEnter()
    {
	if(!name.IsNone && name.Value != "")
	{
	    foreach (string value in strings.stringValues)
	    {
		if(name.Value.Contains(value))
		{
		    Fsm.Event(target, trueEvent);
		    base.Finish();
		    return;
		}
	    }
	    Fsm.Event(target, falseEvent);
	}
	base.Finish();
    }

}
