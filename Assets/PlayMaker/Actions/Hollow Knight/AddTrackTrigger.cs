using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class AddTrackTrigger : FsmStateAction
{
    public FsmOwnerDefault target;
    public FsmBool skipIfPresent;

    public override void Reset()
    {
	target = null;
	skipIfPresent = true;
    }

    public override void OnEnter()
    {
	GameObject safe = target.GetSafe(this);
	if (safe && !safe.GetComponent<TrackTriggerObjects>())
	{
	    safe.AddComponent<TrackTriggerObjects>();
	}
	Finish();
    }

}
