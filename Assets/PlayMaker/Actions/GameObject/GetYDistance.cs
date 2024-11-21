using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Measures the Distance betweens 2 Game Objects and stores the result in a Float Variable. Y axis only.")]
    public class GetYDistance : FsmStateAction
    {
	[RequiredField]
	public FsmOwnerDefault gameObject;
	[RequiredField]
	public FsmGameObject target;
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmFloat storeResult;

	public bool everyFrame;

	public override void Reset()
	{
	    gameObject = null;
	    target = null;
	    storeResult = null;
	    everyFrame = true;
	}

	public override void OnEnter()
	{
	    DoGetDistance();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	public override void OnUpdate()
	{
	    DoGetDistance();
	}

	private void DoGetDistance()
	{
	    GameObject gameObject = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? Owner : this.gameObject.GameObject.Value;
	    if (gameObject == null || target.Value == null || storeResult == null)
	    {
		return;
	    }
	    float num = gameObject.transform.position.y - target.Value.transform.position.y;
	    if (num < 0f)
	    {
		num *= -1f;
	    }
	    storeResult.Value = num;
	}
    }
}
