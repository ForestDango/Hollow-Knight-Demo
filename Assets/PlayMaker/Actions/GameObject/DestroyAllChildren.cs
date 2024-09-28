using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Destroy all children on a GameObject.")]
    public class DestroyAllChildren : FsmStateAction
    {
	[RequiredField]
	public FsmGameObject gameObject;
	public FsmBool disable;

	public override void Reset()
	{
	    gameObject = null;
	    disable = new FsmBool();
	}

	public override void OnEnter()
	{
	    GameObject value = gameObject.Value;
	    if(value != null)
	    {
		foreach (object obj in value.transform)
		{
		    Transform transform = (Transform)obj;
		    if (disable.Value)
		    {
			transform.gameObject.SetActive(false);
		    }
		    else
		    {
			Object.Destroy(transform.gameObject);
		    }
		}
	    }
	    Finish();
	}
    }
}
