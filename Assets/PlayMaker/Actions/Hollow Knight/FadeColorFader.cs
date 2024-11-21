using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class FadeColorFader : FsmStateAction
{
    public FsmOwnerDefault target;
    [ObjectType(typeof(FadeType))]
    public FsmEnum fadeType;
    public FsmBool useChildren;

    public override void Reset()
    {
	target = null;
	fadeType = null;
	useChildren = new FsmBool(true);
    }

    public override void OnEnter()
    {
	GameObject safe = target.GetSafe(this);
	if (safe)
	{
	    ColorFader[] array;
	    if (useChildren.Value)
	    {
		array = safe.GetComponentsInChildren<ColorFader>();
	    }
	    else
	    {
		array = new ColorFader[]
		{
		    safe.GetComponent<ColorFader>()
		};
	    }
	    ColorFader[] array2 = array;
	    for (int i = 0; i < array2.Length; i++)
	    {
		array2[i].Fade((FadeType)fadeType.Value == FadeType.UP);
	    }
	}
	base.Finish();
    }

    public enum FadeType
    {
	UP,
	DOWN
    }
}
