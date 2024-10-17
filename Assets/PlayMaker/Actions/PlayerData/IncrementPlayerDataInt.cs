using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("PlayerData")]
    [Tooltip("Sends a Message to PlayerData to send and receive data.")]
    public class IncrementPlayerDataInt : FsmStateAction
    {
	[RequiredField]
	[Tooltip("GameManager reference, set this to the global variable GameManager.")]
	public FsmOwnerDefault gameObject;

	[RequiredField]
	public FsmString intName;

	public override void Reset()
	{
	    gameObject = null;
	    intName = null;
	}

	public override void OnEnter()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (ownerDefaultTarget == null)
	    {
		return;
	    }
	    GameManager component = ownerDefaultTarget.GetComponent<GameManager>();
	    if(component == null)
	    {
		Debug.Log("IncrementPlayerDataInt: could not find a GameManager on this object, please refere to the GameManager global variable");
		return;
	    }
	    component.IncrementPlayerDataInt(intName.Value);
	    Finish();
	}


    }

}
