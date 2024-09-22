using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("PlayerData")]
    [Tooltip("Sends a Message to PlayerData to send and receive data.")]
    public class GetPlayerDataInt : FsmStateAction
    {
	[RequiredField]
	[Tooltip("GameManager reference, set this to the global variable GameManager.")]
	public FsmOwnerDefault gameObject;

	[RequiredField]
	public FsmString intName;

	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmInt storeValue;

	public override void Reset()
	{
	    gameObject = null;
	    intName = null;
	    storeValue = null;
	}

	public override void OnEnter()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if(ownerDefaultTarget == null)
	    {
		return;
	    }
	    GameManager gameManager = ownerDefaultTarget.GetComponent<GameManager>();
	    if(gameManager == null)
	    {
		Debug.Log("GetPlayerDataInt: could not find a GameManager on this object, please refere to the GameManager global variable");
		return;
	    }
	    storeValue.Value = gameManager.GetPlayerDataInt(intName.Value);
	    Finish();
	}
    }

}
