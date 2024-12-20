using System;
using HutongGames.PlayMaker;
using UnityEngine;

public class GetPreInstantiatedGameObject : FsmStateAction
{

    [RequiredField]
    public FsmOwnerDefault target;
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmGameObject storeGameObject;

    public override void Reset()
    {
	target = new FsmOwnerDefault();
    }

    public override void OnEnter()
    {
	GameObject safe = target.GetSafe(this);
	if (safe)
	{
	    PreInstantiateGameObject component = safe.GetComponent<PreInstantiateGameObject>();
	    if (component)
	    {
		GameObject instantiatedGameObject = component.InstantiatedGameObject;
		if (instantiatedGameObject)
		{
		    instantiatedGameObject.SetActive(true);
		    storeGameObject.Value = instantiatedGameObject;
		}
	    }
	}
	base.Finish();
    }
}
