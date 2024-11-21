using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Checks whether a player bool is true and another is false. Sends event.")]
    public class PlayerDataBoolTrueAndFalse : FsmStateAction
    {
	[RequiredField]
	[Tooltip("GameManager reference, set this to the global variable GameManager.")]
	public FsmOwnerDefault gameObject;
	[RequiredField]
	public FsmString trueBool;
	[RequiredField]
	public FsmString falseBool;
	[Tooltip("Event to send if conditions met.")]
	public FsmEvent isTrue;
	[Tooltip("Event to send if conditions not met.")]
	public FsmEvent isFalse;

	public override void Reset()
	{
	    gameObject = null;
	    trueBool = null;
	    falseBool = null;
	    isTrue = null;
	    isFalse = null;
	}

	public override void OnEnter()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if(ownerDefaultTarget == null)
	    {
		return;
	    }
	    GameManager component = ownerDefaultTarget.GetComponent<GameManager>();
	    if (component == null)
	    {
		return;
	    }
	    if (component.GetPlayerDataBool(trueBool.Value) && !component.GetPlayerDataBool(falseBool.Value))
	    {
		Fsm.Event(isTrue);
	    }
	    else
	    {
		Fsm.Event(isFalse);
	    }
	    Finish();
	}


    }

}
