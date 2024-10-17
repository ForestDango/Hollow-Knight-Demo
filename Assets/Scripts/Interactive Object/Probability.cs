using System;
using System.Collections.Generic;
using UnityEngine;
public class Probability 
{
    public static GameObject GetRandomGameObjectByProbability(ProbabilityGameObject[] array)
    {
	if(array.Length > 1)
	{
	    List<ProbabilityGameObject> list = new List<ProbabilityGameObject>(array);
	    ProbabilityGameObject probabilityGameObject = null;
	    list.Sort((ProbabilityGameObject x, ProbabilityGameObject y) => x.probability.CompareTo(y.probability));
	    float num = 0f;
	    foreach (ProbabilityGameObject probabilityGameObject2 in list)
	    {
		num += ((probabilityGameObject2.probability != 0f) ? probabilityGameObject2.probability : 1f);
	    }
	    float num2 = UnityEngine.Random.Range(0f, num);
	    float num3 = 0f;
	    foreach (ProbabilityGameObject probabilityGameObject3 in list)
	    {
		if(num2 >= num3)
		{
		    probabilityGameObject = probabilityGameObject3;
		}
		num3 += probabilityGameObject3.probability;
	    }
	    return probabilityGameObject.prefab;
	}
	if(array.Length == 1)
	{
	    return array[0].prefab;
	}
	return null;
    }

    [Serializable]
    public class ProbabilityGameObject
    {
	public GameObject prefab;

	[Tooltip("If probability = 0, it will be considered 1.")]
	public float probability;

	public ProbabilityGameObject()
	{
	    probability = 1f;
	}
    }
}
