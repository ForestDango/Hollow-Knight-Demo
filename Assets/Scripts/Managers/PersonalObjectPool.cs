using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalObjectPool : MonoBehaviour
{
    public StartupPool[] startupPool;
    private GameManager gm;
    private bool createdStartupPools;

    private void Start()
    {
	CreateStartupPools();
    }
    private void OnEnable()
    {
	gm = GameManager.instance;
	gm.DestroyPersonalPools += DestroyMyPooledObjects;
    }

    public void CreateStartupPools()
    {
	if (createdStartupPools)
	{
	    return;
	}
	createdStartupPools = true;
	if (startupPool != null && startupPool.Length != 0)
	{
	    for (int i = 0; i < startupPool.Length; i++)
	    {
		CreatePool(startupPool[i].prefab, startupPool[i].size);
	    }
	}
    }

    public void CreatePool(GameObject prefab, int initialPoolSize)
    {
	ObjectPool.CreatePool(prefab, initialPoolSize);
    }

    public void DestroyMyPooledObjects()
    {
	if (startupPool != null && startupPool.Length != 0)
	{
	    for (int i = 0; i < startupPool.Length; i++)
	    {
		ObjectPool.DestroyPooled(startupPool[i].prefab, startupPool[i].size);
	    }
	}
	gm.DestroyPersonalPools -= DestroyMyPooledObjects;
    }
}
