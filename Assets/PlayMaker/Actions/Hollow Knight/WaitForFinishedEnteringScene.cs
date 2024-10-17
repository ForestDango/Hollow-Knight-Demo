using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class WaitForFinishedEnteringScene : FsmStateAction
{
    [RequiredField]
    public FsmEvent sendEvent;

    public override void Reset()
    {
	sendEvent = null;
    }

    public override void OnEnter()
    {
	if (!GameManager.instance)
	{
	    Finish();
	    return;
	}
	if (!GameManager.instance.HasFinishedEnteringScene)
	{
	    GameManager.EnterSceneEvent temp = null;
	    temp = delegate ()
	    {
		Fsm.Event(sendEvent);
		GameManager.instance.OnFinishedEnteringScene -= temp;
		Finish();
	    };
	    GameManager.instance.OnFinishedEnteringScene += temp;
	    return;
	}
	Fsm.Event(sendEvent);
    }

}
