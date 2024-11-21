using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Gets the Scale of a Game Object sends an event based on positivity or negativity of x/y value")]
    public class SendEventByScale : FsmStateAction
    {
	[RequiredField]
	public FsmOwnerDefault gameObject;
	[Tooltip("Where to send the event.")]
	public FsmEventTarget eventTarget;
	[Tooltip("If false, check Y scale")]
	public bool xScale;
	public FsmEvent positiveEvent;
	public FsmEvent negativeEvent;
	public Space space;
	public override void Reset()
	{
	    xScale = true;
	    gameObject = null;
	    space = Space.World;
	}

	public override void OnEnter()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (ownerDefaultTarget == null)
	    {
		return;
	    }
	    Vector3 vector = (space == Space.World) ? ownerDefaultTarget.transform.lossyScale : ownerDefaultTarget.transform.localScale;
	    float num;
	    if (xScale)
	    {
		num = vector.x;
	    }
	    else
	    {
		num = vector.y;
	    }
	    if (num > 0f)
	    {
		Fsm.Event(eventTarget, positiveEvent);
	    }
	    else
	    {
		Fsm.Event(eventTarget, negativeEvent);
	    }
	    Finish();
	}
    }
}
