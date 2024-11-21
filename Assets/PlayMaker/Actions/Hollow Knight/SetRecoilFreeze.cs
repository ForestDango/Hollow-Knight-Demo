using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class SetRecoilFreeze : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    public FsmOwnerDefault target;
    public FsmBool freeze;


    public override void OnEnter()
    {
	GameObject gameObject = (target.OwnerOption == OwnerDefaultOption.UseOwner) ? Owner : target.GameObject.Value;
	if (gameObject)
	{
	    Recoil component = gameObject.GetComponent<Recoil>();
	    if (component)
	    {
		component.freezeInPlace = freeze.Value;
	    }
	}
	Finish();
    }
}
