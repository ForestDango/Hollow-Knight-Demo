using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreInstantiateGameObject : MonoBehaviour
{
    public GameObject prefab;
    private GameObject instantiatedGameObject;
    public GameObject InstantiatedGameObject
    {
	get
	{
	    return instantiatedGameObject;
	}
    }

    private void Awake()
    {
	if (prefab)
	{
	    instantiatedGameObject = Instantiate(prefab);
	    instantiatedGameObject.SetActive(false);
	}
    }
}
