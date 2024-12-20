using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Sets the Position of a Game Object to another Game Object's position")]
    public class SetPositionToObject : FsmStateAction
    {
	[RequiredField]
	[Tooltip("The GameObject to position.")]
	public FsmOwnerDefault gameObject;
	public FsmGameObject targetObject;
	public FsmFloat xOffset;
	public FsmFloat yOffset;
	public FsmFloat zOffset;

	public override void Reset()
	{
	    gameObject = null;
	    targetObject = null;
	    xOffset = null;
	    yOffset = null;
	    zOffset = null;
	}

	public override void OnEnter()
	{
	    DoSetPosition();
	    Finish();
	}

	private void DoSetPosition()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (ownerDefaultTarget == null || targetObject.IsNone || targetObject.Value == null)
	    {
		return;
	    }
	    Vector3 position = targetObject.Value.transform.position;
	    if (!xOffset.IsNone)
	    {
		position = new Vector3(position.x + xOffset.Value, position.y, position.z);
	    }
	    if (!yOffset.IsNone)
	    {
		position = new Vector3(position.x, position.y + yOffset.Value, position.z);
	    }
	    if (!zOffset.IsNone)
	    {
		position = new Vector3(position.x, position.y, position.z + zOffset.Value);
	    }
	    ownerDefaultTarget.transform.position = position;
	}
    }

}
