using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeoRock : MonoBehaviour
{
    [SerializeField]
    public GeoRockData geoRockData;
    private GameManager gm;

    private void OnEnable()
    {
	UnityEngine.SceneManagement.SceneManager.activeSceneChanged += LevelActivated;
	gm = GameManager.instance;
	gm.SavePersistentObjects += SaveState;
    }

    private void OnDisable()
    {
	UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= LevelActivated;
	if (gm != null)
	{
	    gm.SavePersistentObjects -= SaveState;
	}
    }

    private void Start()
    {
	SetMyID();
    }

    private void SetMyID()
    {
	if (string.IsNullOrEmpty(geoRockData.id))
	{
	    geoRockData.id = name;
	}
	if (string.IsNullOrEmpty(geoRockData.sceneName))
	{
	    geoRockData.sceneName = GameManager.GetBaseSceneName(gameObject.scene.name);
	}
    }

    private void SaveState()
    {
	SetMyID();
	UpdateHitsLeftFromFSM();
	SceneData.instance.SaveMyState(geoRockData);
    }

    private void LevelActivated(Scene sceneFrom, Scene sceneTo)
    {
	SetMyID();
	GeoRockData geoRockData = SceneData.instance.FindMyState(this.geoRockData);
	if (geoRockData != null)
	{
	    this.geoRockData.hitsLeft = geoRockData.hitsLeft;
	    GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt("Hits").Value = geoRockData.hitsLeft;
	    return;
	}
	UpdateHitsLeftFromFSM();
    }

    private void UpdateHitsLeftFromFSM()
    {
	geoRockData.hitsLeft = GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt("Hits").Value;
    }
}
