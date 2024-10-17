using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
[HutongGames.PlayMaker.Tooltip("Check and respond to the amount of objects in a Trigger that has TrackTriggerObjects attached to the same object.")]
public class CheckTrackTriggerCount : FsmStateAction
{
    public FsmOwnerDefault target;
    public FsmInt count;
    [ObjectType(typeof(IntTest))]
    public FsmEnum test;
    public bool everyFrame;
    [Space]
    public FsmEvent successEvent;
    private TrackTriggerObjects track;

    public override void Reset()
    {
	target = null;
	count = null;
	test = null;
	everyFrame = true;
	successEvent = null;
    }
    public override void OnPreprocess()
    {
	base.Fsm.HandleFixedUpdate = true;
    }
    public override void OnEnter()
    {
	GameObject safe = target.GetSafe(this);
	if (safe)
	{
	    track = safe.GetComponent<TrackTriggerObjects>();
	    if (track)
	    {
		if (!CheckCount())
		{
		    if (everyFrame)
		    {
			return;
		    }
		}
		else
		{
		    base.Fsm.Event(successEvent);
		}
	    }
	    else
	    {
		Debug.LogError("Target GameObject does not have a TrackTriggerObjects component attached!", Owner);
	    }
	}
	base.Finish();
    }

    public override void OnFixedUpdate()
    {
	if (everyFrame && CheckCount())
	{
	    base.Fsm.Event(successEvent);
	}
    }

    public bool CheckCount()
    {
	if (track)
	{
	    switch ((IntTest)test.Value)
	    {
		case CheckTrackTriggerCount.IntTest.Equal:
		    return track.InsideCount == count.Value;
		case CheckTrackTriggerCount.IntTest.LessThan:
		    return track.InsideCount < count.Value;
		case CheckTrackTriggerCount.IntTest.MoreThan:
		    return track.InsideCount > count.Value;
		case CheckTrackTriggerCount.IntTest.LessThanOrEqual:
		    return track.InsideCount <= count.Value;
		case CheckTrackTriggerCount.IntTest.MoreThanOrEqual:
		    return track.InsideCount >= count.Value;
		default:
		    Debug.LogError(string.Format("IntTest type {0} not implemented!", ((IntTest)test.Value).ToString()), Owner);
		    break;
	    }
	}
	return false;
    }


    public enum IntTest
    {
	Equal,
	LessThan,
	MoreThan,
	LessThanOrEqual,
	MoreThanOrEqual
    }
}
