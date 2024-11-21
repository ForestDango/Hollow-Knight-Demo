using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectPool : MonoBehaviour
{
    private bool startupPoolsCreated;

    public StartupPool[] startupPools;

    private static List<GameObject> tempList = new List<GameObject>();
    private Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    private static ObjectPool _instance;

    private static Vector2 activeStashLocation = new Vector2(-20f, -20f);
    private static bool isRecycling;

    public static ObjectPool instance
    {
	get
	{
	    if(_instance == null)
	    {
		_instance = FindObjectOfType<ObjectPool>();
		if(_instance == null)
		{
		    Debug.LogError("Couldn't find an Object Pool, make sure a Game Manager exists in the scene.");
		}
		else
		{
		    DontDestroyOnLoad(_instance.gameObject);
		}
	    }
	    return _instance;
	}
    }
    private void Awake()
    {
	if (_instance == null)
	{
	    _instance = this;
	    DontDestroyOnLoad(this);
	    return;
	}
	if (!(this != _instance))
	{
	    return;
	}
	Debug.LogFormat("An extra Global Object Pool has been created by {0} please remove this script. Master Object Pool: {1} (Scene: {2} at time: {3})", new object[]
	{
	    transform.parent.name,
	    _instance.name,
	    Application.loadedLevelName,
	    Time.realtimeSinceStartup
	});
	if(transform.parent.name == "GameManager")
	{
	    Debug.Log("Object Pool instance is no longer set to master object pool, another Object Pool exists in this scene. Instance currently set to : " + _instance.name);
	    _instance = this;
	    return;
	}
	Destroy(gameObject);
    }

    private void Start()
    {
	if (!instance.startupPoolsCreated)
	{
	    CreateStartupPools();
	    return;
	}
	for (int i = 0; i < startupPools.Length; i++)
	{
	    startupPools[i].prefab.CreatePool(startupPools[i].size);
	}
    }

    private void CreateStartupPools()
    {
	if (!instance.startupPoolsCreated)
	{
	    instance.startupPoolsCreated = true;
	    StartupPool[] array = instance.startupPools;
	    if(array != null && array.Length != 0)
	    {
		for (int i = 0; i < array.Length; i++)
		{
		    CreatePool(array[i].prefab, array[i].size);
		}
	    }
	}
    }

    public static void CreatePool<T>(T prefab, int initialPoolSize) where T : Component
    {
	CreatePool(prefab.gameObject, initialPoolSize);
    }

    /// <summary>
    /// 创建一个游戏对象的对象池，包括设置它的初始大小initialPoolSize，parent，
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="initialPoolSize"></param>
    public static void CreatePool(GameObject prefab, int initialPoolSize)
    {
	if(prefab != null)
	{
	    List<GameObject> list;
	    if (!instance.pooledObjects.ContainsKey(prefab)) 
	    {
		list = new List<GameObject>();
		instance.pooledObjects.Add(prefab, list);
		if(initialPoolSize > 0)
		{
		    bool activeSelf = prefab.activeSelf;
		    bool flag;
		    if (prefab.GetComponent<ActiveRecycler>() != null)
		    {
			flag = true;
			prefab.SetActive(true);
		    }
		    else
		    {
			flag = false;
			prefab.SetActive(false);
		    }
		    Transform transform = instance.transform;
		    while (list.Count < initialPoolSize)
		    {
			GameObject gameObject = Instantiate<GameObject>(prefab);
			gameObject.transform.parent = transform;
			if (flag)
			{
			    gameObject.transform.SetPosition2D(activeStashLocation);
			}
			list.Add(gameObject);
		    }
		    prefab.SetActive(activeSelf);
		}
	    }
	    else
	    {
		list = instance.pooledObjects[prefab];
		if(initialPoolSize > 0)
		{
		    int num = list.Count + initialPoolSize;
		    bool activeSelf2 = prefab.activeSelf;
		    bool flag2;
		    if(prefab.GetComponent<ActiveRecycler>() != null)
		    {
			flag2 = true;
			prefab.SetActive(true);
		    }
		    else
		    {
			flag2 = false;
			prefab.SetActive(false);
		    }
		    Transform transform2 = instance.transform;
		    while (list.Count < num)
		    {
			GameObject gameObject2 = Instantiate<GameObject>(prefab);
			gameObject2.transform.parent = transform2;
			if (flag2)
			{
			    gameObject2.transform.SetPosition2D(activeStashLocation);
			}
			list.Add(gameObject2);
		    }
		    prefab.SetActive(activeSelf2);
		}
	    }
	    if(list == null)
	    {
		return;
	    }
	    using (List<GameObject>.Enumerator enumerator = list.GetEnumerator())
	    {
		while (enumerator.MoveNext())
		{
		    GameObject gameObject3 = enumerator.Current;
		    tk2dSprite[] componentsInChildren = gameObject3.GetComponentsInChildren<tk2dSprite>(true);
		    for (int i = 0; i < componentsInChildren.Length; i++)
		    {
			componentsInChildren[i].ForceBuild();
		    }
		}
		return;
	    }
	}
	if (prefab == null)
	{
	    Debug.LogError("Trying to create an Object Pool for a prefab that is null.");
	}
    }

    public static GameObject Spawn(GameObject prefab,Transform parent,Vector3 position,Quaternion rotation)
    {
	bool flag = prefab.GetComponent<ActiveRecycler>() != null;
	List<GameObject> list;
	if(instance.pooledObjects.TryGetValue(prefab,out list))
	{
	    GameObject gameObject = null;
	    if(list.Count > 0)
	    {
		while (gameObject == null && list.Count >0)
		{
		    gameObject = list[0];
		    list.RemoveAt(0);
		}
		if(gameObject != null)
		{
		    Transform transform = gameObject.transform;
		    transform.parent = parent;
		    transform.localPosition = position;
		    transform.localRotation = rotation;
		    if (flag)
		    {
			FSMUtility.SendEventToGameObject(gameObject, "A SPAWN", false);
		    }
		    else
		    {
			gameObject.SetActive(true);
		    }
		    instance.spawnedObjects.Add(gameObject, prefab);

		    return gameObject;
		}
	    }
	    Debug.LogWarningFormat("Object Pool attached to {0} has run out of {1} prefabs, Instantiating an additional one.", new object[]
	    {
		instance.name,
		prefab.name
	    });
	    gameObject = Instantiate(prefab);
	    Transform transform2 = gameObject.transform;
	    transform2.parent = parent;
	    transform2.localPosition = position;
	    transform2.localRotation = rotation;
	    if (flag)
	    {
		FSMUtility.SendEventToGameObject(gameObject, "A SPAWN", false);
	    }
	    instance.spawnedObjects.Add(gameObject, prefab);

	    return gameObject;
	}
	if (prefab == null)
	{
	    Debug.LogErrorFormat("Object Pool attached to {0} was asked for a NULL prefab.", new object[]
	    {
		instance.name
	    });
	    return null;
	}
	Debug.LogWarningFormat("Object Pool attached to {0} could not find a copy of {1}, Instantiating a new one.", new object[]
	{
	    instance.name,
	    prefab.name
	});
	CreatePool(prefab.gameObject, 1);
	return Spawn(prefab);
    }
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
	return Spawn(prefab, null, position, rotation);
    }
    public static GameObject Spawn(GameObject prefab)
    {
	return Spawn(prefab, null, Vector3.zero, Quaternion.identity);
    }
    public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
    {
	return Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
    }

    public static void Recycle<T>(T obj) where T : Component
    {
	Recycle(obj.gameObject);
    }
    public static void Recycle(GameObject obj)
    {
	GameObject prefab;
	if(instance != null && instance.spawnedObjects.TryGetValue(obj,out prefab))
	{
	    Recycle(obj, prefab);
	    return;
	}

	Destroy(obj);
    }

    private static void Recycle(GameObject obj, GameObject prefab)
    {
	isRecycling = true;
	if (obj != null && prefab != null)
	{
	    instance.pooledObjects[prefab].Add(obj);
	    instance.spawnedObjects.Remove(obj);
	    obj.transform.parent = instance.transform;
	    if(obj.GetComponent<ActiveRecycler>() != null)
	    {
		obj.transform.SetPosition2D(activeStashLocation);
		FSMUtility.SendEventToGameObject(obj, "A RECYCLE", false);
	    }
	    else
	    {
		obj.SetActive(false);
	    }

	}
	isRecycling = false;
    }

    public static void RecycleAll()
    {
	tempList.AddRange(instance.spawnedObjects.Keys);
	for (int i = 0; i < tempList.Count; i++)
	{
	    Recycle(tempList[i]);
	}
	tempList.Clear();
    }

    [Serializable]
    public class StartupPool
    {
	public int size;
	public GameObject prefab;
    }
}
