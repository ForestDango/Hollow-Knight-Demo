using System;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPoolExtensions
{
    public static void CreatePool(this GameObject prefab, int initialPoolSize)
    {
	ObjectPool.CreatePool(prefab, initialPoolSize);
    }

    public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation)
    {
	return ObjectPool.Spawn(prefab, null, position, rotation);
    }

    public static GameObject Spawn(this GameObject prefab)
    {
	return ObjectPool.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
    }
    public static GameObject Spawn(this GameObject prefab, Vector3 position)
    {
	return ObjectPool.Spawn(prefab, null, position, Quaternion.identity);
    }

    public static T Spawn<T>(this T prefab) where T : Component
    {
	return ObjectPool.Spawn<T>(prefab, null, Vector3.zero, Quaternion.identity);
    }

    public static void Recycle(this GameObject obj)
    {
	ObjectPool.Recycle(obj);
    }
    public static void Recycle<T>(this T obj) where T : Component
    {
	ObjectPool.Recycle<T>(obj);
    }

}
