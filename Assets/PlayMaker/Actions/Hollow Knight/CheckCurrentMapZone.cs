using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class CheckCurrentMapZone : FsmStateAction
{
    [RequiredField]
    public FsmString mapZone;
    public FsmEvent equalEvent;
    public FsmEvent notEqualEvent;



    public override void Reset()
    {
	mapZone = null;
	equalEvent = null;
	notEqualEvent = null;
    }

    public override void OnEnter()
    {
	if (GameManager.instance)
	{
	    if (mapZone.Value == GameManager.instance.GetCurrentMapZone())
	    {
		Fsm.Event(equalEvent);
	    }
	    else
	    {
		Fsm.Event(notEqualEvent);
	    }
	}
	Finish();
    }
}
