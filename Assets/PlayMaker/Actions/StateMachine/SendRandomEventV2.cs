using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Sends a Random Event picked from an array of Events. Optionally set the relative weight of each event. Use ints to keep events from being fired x times in a row.")]
    public class SendRandomEventV2 : FsmStateAction
    {
	[CompoundArray("Events", "Event", "Weight")]
	public FsmEvent[] events;

	[HasFloatSlider(0f, 1f)]
	public FsmFloat[] weights;

	[UIHint(UIHint.Variable)]
	public FsmInt[] trackingInts;
	public FsmInt[] eventMax;

	private DelayedEvent delayedEvent;

	public override void Reset()
	{
	    events = new FsmEvent[3];
	    weights = new FsmFloat[]
	    {
		1f,
		1f,
		1f
	    };
	}

	public override void OnEnter()
	{
	    bool flag = false;
	    while (!flag)
	    {
		int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
		if(randomWeightedIndex != -1 && trackingInts[randomWeightedIndex].Value < eventMax[randomWeightedIndex].Value)
		{
		    int value = ++trackingInts[randomWeightedIndex].Value;
		    for (int i = 0; i < trackingInts.Length; i++)
		    {
			trackingInts[i].Value = 0;
		    }
		    trackingInts[randomWeightedIndex].Value = value;
		    flag = true;
		    Fsm.Event(events[randomWeightedIndex]);
		}
	    }
	    Finish();
	}


    }

}
