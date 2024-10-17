using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckIfBossScene : FsmStateAction
{
    public FsmEvent bossSceneEvent;
    public FsmEvent regularEvent;

    public override void Reset()
    {
	bossSceneEvent = null;
	regularEvent = null;
    }

    public override void OnEnter()
    {
	Fsm.Event(regularEvent);
	base.Finish();
    }
}
