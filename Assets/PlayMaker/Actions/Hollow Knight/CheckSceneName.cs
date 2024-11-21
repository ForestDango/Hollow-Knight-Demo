using System;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class CheckSceneName : FsmStateAction
{
    [RequiredField]
    public FsmString sceneName;
    public FsmEvent equalEvent;
    public FsmEvent notEqualEvent;

    public override void Reset()
    {
	sceneName = null;
	equalEvent = null;
	notEqualEvent = null;
    }

    public override void OnEnter()
    {
	if (GameManager.instance)
	{
	    if(sceneName.Value == GameManager.instance.GetSceneNameString())
	    {
		Fsm.Event(equalEvent);
	    }
	    else
	    {
		Fsm.Event(notEqualEvent);
	    }
	}
	base.Finish();
    }

}
