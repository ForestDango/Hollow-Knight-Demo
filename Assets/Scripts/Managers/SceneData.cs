using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneData
{
    private static SceneData _instance;
    public static SceneData instance
    {
	get
	{
	    if (_instance == null)
	    {
		_instance = new SceneData();
	    }
	    return _instance;
	}
	set
	{
	    _instance = value;
	}
    }

    [SerializeField] public List<GeoRockData> geoRocks;

    protected SceneData()
    {
	SetupNewSceneData();
    }

    public void Reset()
    {
	SetupNewSceneData();
    }

    private void SetupNewSceneData()
    {
	geoRocks = new List<GeoRockData>();
    }
    public void SaveMyState(GeoRockData geoRockData)
    {
	int num = FindGeoRockInList(geoRockData);
	if (num == -1)
	{
	    geoRocks.Add(geoRockData);
	    return;
	}
	geoRocks[num] = geoRockData;
    }
    public GeoRockData FindMyState(GeoRockData grd)
    {
	int num = FindGeoRockInList(grd);
	if (num == -1)
	{
	    return null;
	}
	return geoRocks[num];
    }
    private int FindGeoRockInList(GeoRockData grd)
    {
	for (int i = 0; i < geoRocks.Count; i++)
	{
	    if (string.Compare(geoRocks[i].sceneName, grd.sceneName, true) == 0 && geoRocks[i].id == grd.id)
	    {
		return i;
	    }
	}
	return -1;
    }
}
