using System;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]

public class WaitForBossLoad : FsmStateAction
{
    [RequiredField]
    public FsmEvent sendEvent;
    public override void Reset()
    {
	sendEvent = null;
    }

    public override void OnEnter()
    {
	if (GameManager.instance && SceneAdditiveLoadConditional.ShouldLoadBoss)
	{
	    GameManager.BossLoad temp = null;
	    temp = delegate ()
	    {
		Fsm.Event(sendEvent);
		GameManager.instance.OnLoadedBoss -= temp;
		Finish();
	    };
	    GameManager.instance.OnLoadedBoss += temp;
	    return;
	}
	base.Finish();
    }
}
