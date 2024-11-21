using System;
using UnityEngine;

public class HardLandEffect : MonoBehaviour
{
    public GameObject dustObj;

    [Space]
    public GameObject particleRockPrefab;

    private float recycleTime;

    private void OnEnable()
    {
	//TODO:

	dustObj.SetActive(true);
	dustObj.SetActiveChildren(true);

	if (particleRockPrefab)
	{
	    FlingUtils.SpawnAndFling(new FlingUtils.Config
	    {
		Prefab = particleRockPrefab,
		AmountMin = 2,
		AmountMax = 3,
		SpeedMin = 12f,
		SpeedMax = 15f,
		AngleMin = 95f,
		AngleMax = 140f
	    }, transform, new Vector3(0f, -0.9f, 0f));
	    FlingUtils.SpawnAndFling(new FlingUtils.Config
	    {
		Prefab = particleRockPrefab,
		AmountMin = 2,
		AmountMax = 3,
		SpeedMin = 12f,
		SpeedMax = 15f,
		AngleMin = 40f,
		AngleMax = 85f
	    }, transform, new Vector3(0f, -0.9f, 0f));
	}
	recycleTime = Time.time + 1.5f;
    }

    private void Update()
    {
	if(Time.time > recycleTime)
	{
	    gameObject.Recycle();
	}
    }
}
