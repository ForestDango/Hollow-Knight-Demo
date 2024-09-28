using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Sends Events based on the value of a Boolean Variable.")]
    public class PlayerDataBoolTest : FsmStateAction
	{
	[RequiredField]
	[Tooltip("GameManager reference, set this to the global variable GameManager.")]
	public FsmOwnerDefault gameObject;

	[RequiredField]
	public FsmString boolName;

	[Tooltip("Event to send if the Bool variable is True.")]
	public FsmEvent isTrue;

	[Tooltip("Event to send if the Bool variable is False.")]
	public FsmEvent isFalse;

	private bool boolCheck;

	public override void Reset()
	{
	    gameObject = null;
	    boolName = null;
	    isTrue = null;
	    isFalse = null;
	}

	public override void OnEnter()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (ownerDefaultTarget == null)
	    {
		return;
	    }
	    GameManager component = ownerDefaultTarget.GetComponent<GameManager>();
	    if (component == null)
	    {
		return;
	    }
	    boolCheck = component.GetPlayerDataBool(boolName.Value);
	    if (boolCheck)
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
