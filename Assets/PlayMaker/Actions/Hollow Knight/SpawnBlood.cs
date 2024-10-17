using System;
using HutongGames.PlayMaker;
using UnityEngine;

public class SpawnBlood : FsmStateAction
{
    public FsmGameObject spawnPoint;
    public FsmVector3 position;
    public FsmInt spawnMin;
    public FsmInt spawnMax;
    public FsmFloat speedMin;
    public FsmFloat speedMax;
    public FsmFloat angleMin;
    public FsmFloat angleMax;
    public FsmColor colorOverride;

    public override void Reset()
    {
	spawnPoint = new FsmGameObject
	{
	    UseVariable = true
	};
	position = new FsmVector3();
	spawnMin = null;
	spawnMax = null;
	speedMin = null;
	speedMax = null;
	angleMin = null;
	angleMax = null;
	colorOverride = new FsmColor
	{
	    UseVariable = true
	};
    }

    public override void OnEnter()
    {
	Spawn();
	Finish();
    }

    protected void Spawn()
    {
	if (GlobalPrefabDefaults.Instance)
	{
	    Vector3 a = position.Value;
	    if (spawnPoint.Value)
	    {
		a += spawnPoint.Value.transform.position;
	    }
	    GlobalPrefabDefaults.Instance.SpawnBlood(a, (short)spawnMin.Value, (short)spawnMax.Value, speedMin.Value, speedMax.Value, angleMin.Value, angleMax.Value, colorOverride.IsNone ? null : new Color?(colorOverride.Value));
	}
    }
}
