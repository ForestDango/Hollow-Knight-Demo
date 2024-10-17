using System;
using HutongGames.PlayMaker;
using UnityEngine;


public class SpawnBloodTime : SpawnBlood
{
    public FsmFloat delay;
    private float nextSpawnTime;

    public override void Reset()
    {
	base.Reset();
	delay = new FsmFloat(0.1f);
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
	base.OnUpdate();
	if(Time.time > nextSpawnTime)
	{
	    nextSpawnTime = Time.time + delay.Value;
	    base.Spawn();
	}
    }

}
