using System;
using System.Collections.Generic;
using UnityEngine;

public class TrackTriggerObjects : MonoBehaviour
{
    [SerializeField]
    private LayerMask ignoreLayers;
    private List<GameObject> insideGameObjects = new List<GameObject>();
    private int layerMask = -1;
    private bool gottenOverlappedColliders;
    private bool subscribed;
    private static readonly Collider2D[] _tempResults = new Collider2D[10];
    private static readonly List<GameObject> _refreshTemp = new List<GameObject>();

    public int InsideCount
    {
	get
	{
	    int num = 0;
	    using (List<GameObject>.Enumerator enumerator = insideGameObjects.GetEnumerator())
	    {
		while (enumerator.MoveNext())
		{
		    if (enumerator.Current)
		    {
			num++;
		    }
		}
	    }
	    return num;
	}
    }

    private void OnDisable()
    {
	insideGameObjects.Clear();
	gottenOverlappedColliders = false;
	if (subscribed)
	{
	    HeroController silentInstance = HeroController.SilentInstance;
	    if (silentInstance)
	    {
		silentInstance.heroInPosition -= OnHeroInPosition;
	    }
	    subscribed = false;
	}
    }

    private void OnEnable()
    {
	if (layerMask < 0)
	{
	    layerMask = Helper.GetCollidingLayerMaskForLayer(gameObject.layer);
	}
	HeroController instance = HeroController.instance;
	if (instance && !instance.isHeroInPosition)
	{
	    HeroController.instance.heroInPosition += OnHeroInPosition;
	    subscribed = true;
	    return;
	}
	GetOverlappedColliders(false);
    }

    private void FixedUpdate()
    {
	for (int i = insideGameObjects.Count - 1; i >= 0; i--)
	{
	    GameObject gameObject = insideGameObjects[i];
	    if (!gameObject || !gameObject.activeInHierarchy)
	    {
		insideGameObjects.RemoveAt(i);
	    }
	}
    }

    private void OnHeroInPosition(bool forceDirect)
    {
	if (subscribed)
	{
	    HeroController.instance.heroInPosition -= OnHeroInPosition;
	    subscribed = false;
	}
	if (!this)
	{
	    Debug.LogError("TrackTriggerObjects native Object was destroyed! This should not happen...", this);
	    return;
	}
	GetOverlappedColliders(false);
    }

    private void GetOverlappedColliders(bool isRefresh = false)
    {
	if (!enabled || !gameObject.activeInHierarchy)
	{
	    return;
	}
	if (gottenOverlappedColliders && !isRefresh)
	{
	    return;
	}
	gottenOverlappedColliders = true;
	Collider2D[] components = GetComponents<Collider2D>();
	Collider2D[] array = components;
	for (int i = 0; i < array.Length; i++)
	{
	    if (array[i].OverlapCollider(new ContactFilter2D
	    {
		useTriggers = true,
		useLayerMask = true,
		layerMask = layerMask
	    }, _tempResults) > 0)
	    {
		foreach (Collider2D collider2D in _tempResults)
		{
		    if (collider2D)
		    {
			OnTriggerEnter2D(collider2D);
		    }
		}
	    }
	}
	for (int k = 0; k < _tempResults.Length; k++)
	{
	    _tempResults[k] = null;
	}
	if (isRefresh)
	{
	    _refreshTemp.AddRange(insideGameObjects);
	    foreach (GameObject gameObject in _refreshTemp)
	    {
		bool flag = false;
		array = components;
		for (int i = 0; i < array.Length; i++)
		{
		    if (array[i].gameObject == gameObject)
		    {
			flag = true;
			break;
		    }
		}
		if (!flag)
		{
		    OnExit(gameObject);
		}
	    }
	    _refreshTemp.Clear();
	}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if (!gottenOverlappedColliders)
	{
	    return;
	}
	GameObject gameObject = collision.gameObject;
	if (IsIgnored(gameObject))
	{
	    return;
	}
	if (!insideGameObjects.Contains(gameObject))
	{
	    insideGameObjects.Add(gameObject);
	}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
	GameObject gameObject = collision.gameObject;
	OnExit(gameObject);
    }

    private void OnExit(GameObject obj)
    {
	insideGameObjects.Remove(obj);
    }

    private bool IsIgnored(GameObject obj)
    {
	int layer = obj.layer;
	int num = 1 << layer;
	return (ignoreLayers.value & num) == num;
    }

}
