using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class GetNailDamage : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    public FsmInt storeValue;

    public override void Reset()
    {
	storeValue = null;
    }

    public override void OnEnter()
    {
	//TODO:
	if(!storeValue.IsNone)
	{
	    storeValue.Value = GameManager.instance.playerData.nailDamage;
	}
	base.Finish();
    }

}
