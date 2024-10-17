using System;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class PlayVibration : FsmStateAction
{
    public FsmFloat loopTime;
    public FsmBool isLooping;
    public FsmString tag;
    private float cooldownTimer;

    public override void Reset()
    {
	loopTime = new FsmFloat
	{
	    UseVariable = true
	};
    }

    public override void OnEnter()
    {
	base.OnEnter();
	Play(false);
	EnqueueNextLoop();
    }

    public override void OnUpdate()
    {
	base.OnUpdate();
	cooldownTimer -= Time.deltaTime;
	if(cooldownTimer <= 0f)
	{
	    Play(false);
	    EnqueueNextLoop();
	}
    }

    private void Play(bool loop)
    {
	Debug.LogFormat("Is playing Vibration //TODO:");
    }

    private void EnqueueNextLoop()
    {
	float num = 0f;
	if (!loopTime.IsNone)
	{
	    num = loopTime.Value;
	}
	if (num < Mathf.Epsilon)
	{
	    Finish();
	    return;
	}
	cooldownTimer = num;
    }

}
