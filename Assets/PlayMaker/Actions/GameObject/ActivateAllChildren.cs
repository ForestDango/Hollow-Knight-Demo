using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Activate or deactivate all children on a GameObject.")]
    public class ActivateAllChildren : FsmStateAction
    {
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmGameObject gameObject;

	public bool activate;

	public override void Reset()
	{
	    gameObject = null;
	    activate = false;
	}

	public override void OnEnter()
	{
	    GameObject value = gameObject.Value;
	    if(value != null)
	    {
		foreach (object obj in value.transform)
		{
		    ((Transform)obj).gameObject.SetActive(activate);
		}
	    }
	    Finish();
	}


    }

}
