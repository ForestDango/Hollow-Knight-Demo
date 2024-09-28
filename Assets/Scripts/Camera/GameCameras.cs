using System;
using UnityEngine;
using UnityEngine.UI;

public class GameCameras : MonoBehaviour
{
    private static GameCameras _instance;
    public static GameCameras instance
    {
	get
	{
	    if (_instance == null)
	    {
		_instance = FindObjectOfType<GameCameras>();
		if (_instance == null)
		{
		    Debug.LogError("Couldn't find GameCameras, make sure one exists in the scene.");
		}
		DontDestroyOnLoad(_instance.gameObject);
	    }
	    return _instance;
	}
    }

    [Header("Controllers")]
    public CameraController cameraController;
    public CameraTarget cameraTarget;

    private GameManager gm;

    private void Awake()
    {
	if(_instance == null)
	{
	    _instance = this;
	    DontDestroyOnLoad(this);
	    return;
	}
	if(this != _instance)
	{
	    DestroyImmediate(gameObject);
	    return;
	}
    }

    private void Start()
    {
	SetupGameRefs();
    }

    private void SetupGameRefs()
    {
	gm = GameManager.instance;
	if (cameraTarget != null)
	{
	    cameraTarget.GameInit();
	}
    }
}
