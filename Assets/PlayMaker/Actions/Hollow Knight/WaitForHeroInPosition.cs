using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class WaitForHeroInPosition : FsmStateAction
{
    [RequiredField]
    public FsmEvent sendEvent;
    public FsmBool skipIfAlreadyPositioned;

    public override void Reset()
    {
	sendEvent = null;
	skipIfAlreadyPositioned = new FsmBool(false);
    }

    public override void OnEnter()
    {
	if(HeroController.instance && !HeroController.instance.isHeroInPosition)
	{
	    HeroController.HeroInPosition temp = null;
	    temp = delegate (bool forecDirece)
	    {
		Fsm.Event(sendEvent);
		HeroController.instance.heroInPosition -= temp;
		Finish();
	    };
	    HeroController.instance.heroInPosition += temp;
	    return;
	}
	Finish();
    }


}
