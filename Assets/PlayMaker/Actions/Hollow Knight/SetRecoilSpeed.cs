using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class SetRecoilSpeed : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    public FsmOwnerDefault target;
    public FsmFloat newRecoilSpeed;

    public override void Reset()
    {
	target = null;
	newRecoilSpeed = new FsmFloat();
    }

    public override void OnEnter()
    {
	GameObject gameObject = target.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : target.GameObject.Value;
	if(gameObject != null)
	{
	    Recoil recoil = gameObject.GetComponent<Recoil>();
	    if(recoil != null)
	    {
		recoil.SetRecoilSpeed(newRecoilSpeed.Value);
	    }
	}
	Finish();
    }

}
