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

    [Header("Cameras")]
    public Camera hudCamera;
    public Camera mainCamera;

    [Header("Controllers")]
    public CameraController cameraController;
    public CameraTarget cameraTarget;

    [Header("FSMs")]
    public PlayMakerFSM cameraShakeFSM;
    public PlayMakerFSM cameraFadeFSM;

    private bool init;

    private GameManager gm;

    private void Awake()
    {
	if(_instance == null)
	{
	    _instance = this;
	    Debug.LogFormat("GameCameras DontDestroyOnLoad");
	    DontDestroyOnLoad(this);
	    return;
	}
	if(this != _instance)
	{
	    DestroyImmediate(gameObject);
	    return;
	}
    }

    public void SceneInit()
    {
	if(this == _instance)
	{
	    StartScene();
	}
    }

    private void StartScene()
    {
	if (!init)
	{
	    SetupGameRefs();
	}
	if(gm.IsGameplayScene() || gm.ShouldKeepHUDCameraActive())
	{
	    MoveMenuToHUDCamera();
	    if (!hudCamera.gameObject.activeSelf)
	    {
		hudCamera.gameObject.SetActive(true);
	    }
	}
	else
	{
	    DisableHUDCamIfAllowed();
	}
	if (gm.IsMenuScene())
	{
	    cameraController.transform.SetPosition2D(14.6f, 8.5f);
	}
	cameraController.SceneInit();
	cameraTarget.SceneInit();

    }

    public void MoveMenuToHUDCamera()
    {
	int cullingMask = mainCamera.cullingMask;
	int cullingMask2 = hudCamera.cullingMask;
	UIManager.instance.UICanvas.worldCamera = hudCamera; //让uimanager的canvas相机设置成hudcamera
	UIManager.instance.UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
	mainCamera.cullingMask = (cullingMask ^ 134217728);
	hudCamera.cullingMask = (cullingMask2 | 134217728);
    }

    public void DisableHUDCamIfAllowed()
    {
	if (gm.IsNonGameplayScene() && !gm.ShouldKeepHUDCameraActive())
	{
	    hudCamera.gameObject.SetActive(false);
	}
    }

    private void SetupGameRefs()
    {
	gm = GameManager.instance;
	if(cameraController != null)
	{
	    cameraController.GameInit();
	}
	else
	{
	    Debug.LogError("CameraController not set in inspector.");
	}
	if (cameraTarget != null)
	{
	    cameraTarget.GameInit();
	}
	else
	{
	    Debug.LogError("CameraTarget not set in inspector.");
	}
	init = true;
    }

}
