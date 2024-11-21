using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("PlayerData")]
    [Tooltip("Sends a Message to PlayerData to send and receive data.")]
    public class GetPlayerDataString : FsmStateAction
    {
	[RequiredField]
	[Tooltip("GameManager reference, set this to the global variable GameManager.")]
	public FsmOwnerDefault gameObject;
	[RequiredField]
	public FsmString stringName;
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmString storeValue;

	public override void Reset()
	{
	    gameObject = null;
	    stringName = null;
	    storeValue = null;
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
		Debug.Log("GetPlayerDataString: could not find a GameManager on this object, please refere to the GameManager global variable");
		return;
	    }
	    storeValue.Value = component.GetPlayerDataString(stringName.Value);
	    Finish();
	}
    }
}
