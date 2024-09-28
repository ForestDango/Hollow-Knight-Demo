using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isPaused;

    private int timeSlowedCount;
    public bool TimeSlowed
    {
	get
	{
	    return timeSlowedCount > 0;
	}
    }

    [SerializeField] public PlayerData playerData;
    public CameraController cameraCtrl { get; private set; }
    private GameCameras gameCams;

    public string sceneName; //场景名字
    public float sceneWidth;//场景宽度
    public float sceneHeight;//场景高度
    public tk2dTileMap tilemap{ get; private set; }
    private static readonly string[] SubSceneNameSuffixes = new string[]
	{
		"_boss_defeated",
		"_boss",
		"_preload"
	};


    private static GameManager _instance;
    public static GameManager instance
    {
	get
	{
	    if(_instance == null)
	    {
		_instance = FindObjectOfType<GameManager>();
	    }
	    if (_instance == null)
	    {
		Debug.LogError("Couldn't find a Game Manager, make sure one exists in the scene.");
	    }
	    else if (Application.isPlaying)
	    {
		DontDestroyOnLoad(_instance.gameObject);
	    }
	    return _instance;
	}
    }

    private void Awake()
    {
	if(_instance != this)
	{
	    _instance = this;
	    DontDestroyOnLoad(this);
	    SetupGameRefs();
	    SetupSceneRefs();
	    return;
	}
	if(this != _instance)
	{
	    Destroy(gameObject);
	    return;
	}
	SetupGameRefs();
	SetupSceneRefs();
    }

    private void SetupGameRefs()
    {
	playerData = PlayerData.instance;
	gameCams = GameCameras.instance;
	cameraCtrl = gameCams.cameraController;
    }

    public void SetupSceneRefs()
    {
	UpdateSceneName();
	RefreshTilemapInfo(sceneName);
    }

    private void UpdateSceneName()
    {
	sceneName = GetBaseSceneName(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 获取场景的基础名字
    /// </summary>
    /// <param name="fullSceneName"></param>
    /// <returns></returns>
    public static string GetBaseSceneName(string fullSceneName)
    {
	for (int i = 0; i < SubSceneNameSuffixes.Length; i++)
	{
	    string text = SubSceneNameSuffixes[i];
	    if (fullSceneName.EndsWith(text, StringComparison.InvariantCultureIgnoreCase))
	    {
		return fullSceneName.Substring(0, fullSceneName.Length - text.Length);
	    }
	}
	return fullSceneName;
    }

    /// <summary>
    /// 重新刷新场景的tilemap的信息
    /// </summary>
    /// <param name="targetScene"></param>
    public void RefreshTilemapInfo(string targetScene)
    {
	tk2dTileMap tk2dTileMap = null;
	GameObject gameObject = GameObject.Find("TileMap");
	if(gameObject != null)
	{
	    tk2dTileMap = GetTileMap(gameObject);
	}
	if (tk2dTileMap == null)
	{
	    Debug.LogErrorFormat("Failed to find tilemap in {0} entirely.", new object[]
	    {
		targetScene
	    });
	    return;
	}
	tilemap = tk2dTileMap;
	sceneWidth = tilemap.width;
	sceneHeight = tilemap.height;
    }

    private static tk2dTileMap GetTileMap(GameObject gameObject)
    {
	if (gameObject.CompareTag("TileMap"))
	{
	    return gameObject.GetComponent<tk2dTileMap>();
	}
	return null;
    }

    public int GetPlayerDataInt(string intName)
    {
	return playerData.GetInt(intName);
    }

    public bool GetPlayerDataBool(string boolName)
    {
	return playerData.GetBool(boolName);
    }

    public void SetPlayerDataBool(string boolName,bool value)
    {
	playerData.SetBool(boolName, value); 
    }

    private IEnumerator SetTimeScale(float newTimeScale,float duration)
    {
	float lastTimeScale = TimeController.GenericTimeScale;
	for (float timer = 0f; timer < duration; timer += Time.unscaledDeltaTime)
	{
	    float t = Mathf.Clamp01(timer / duration);
	    SetTimeScale(Mathf.Lerp(lastTimeScale, newTimeScale, t));
	    yield return null;
	}
	SetTimeScale(newTimeScale);
    }

    private void SetTimeScale(float newTimeScale)
    {
	if(timeSlowedCount > 1)
	{
	    newTimeScale = Mathf.Min(newTimeScale, TimeController.GenericTimeScale);
	}
	TimeController.GenericTimeScale = ((newTimeScale > 0.01f) ? newTimeScale : 0f);
    }

    public IEnumerator FreezeMoment(float rampDownTime,float waitTime,float rampUpTime,float targetSpeed)
    {
	timeSlowedCount++;
	yield return StartCoroutine(SetTimeScale(targetSpeed, rampDownTime));
	for (float timer = 0f; timer < waitTime; timer += Time.unscaledDeltaTime)
	{
	    yield return null;
	}
	yield return StartCoroutine(SetTimeScale(1f, rampUpTime));
	timeSlowedCount--;
    }
}
