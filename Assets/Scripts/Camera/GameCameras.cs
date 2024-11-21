using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

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
    public PlayMakerFSM soulOrbFSM;
    public PlayMakerFSM soulVesselFSM;

    [Header("Camera Effects")]
    public ColorCorrectionCurves colorCorrectionCurves;
    public SceneColorManager sceneColorManager;
    public BrightnessEffect brightnessEffect;
    public SceneParticlesController sceneParticlesPrefab;

    [Header("Other")]
    public tk2dCamera tk2dCam;
    public GameObject hudCanvas;
    public Transform cameraParent;
    public GeoCounter geoCounter;

    public SceneParticlesController sceneParticles { get; private set; }

    private bool init;

    private GameManager gm;

    private void Awake()
    {
	if (_instance == null)
	{
	    _instance = this;
	    Debug.LogFormat("GameCameras DontDestroyOnLoad");
	    DontDestroyOnLoad(this);
	    return;
	}
	if (this != _instance)
	{
	    DestroyImmediate(gameObject);
	    return;
	}
    }

    private void OnDestroy()
    {
	DestroyImmediate(sceneParticles);
    }

    public void SceneInit()
    {
	if (this == _instance)
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
	if (gm.IsGameplayScene() || gm.ShouldKeepHUDCameraActive())
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

	sceneParticles.SceneInit();
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
	if (cameraController != null)
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
	if (sceneParticlesPrefab != null)
	{
	    sceneParticles = Instantiate(sceneParticlesPrefab);
	    sceneParticles.name = "SceneParticlesController";
	    sceneParticles.transform.position = new Vector3(tk2dCam.transform.position.x, tk2dCam.transform.position.y, 0f);
	    sceneParticles.transform.SetParent(tk2dCam.transform);
	}
	else
	{
	    Debug.LogError("Scene Particles Prefab not set in inspector.");
	}
	init = true;
    }

    public void DisableImageEffects()
    {
	mainCamera.GetComponent<FastNoise>().enabled = false;
	mainCamera.GetComponent<BloomOptimized>().enabled = false;
	mainCamera.GetComponent<ColorCorrectionCurves>().enabled = false;
    }

    public void EnableImageEffects(bool isGameplayLevel, bool isBloomForced)
    {
	mainCamera.GetComponent<ColorCorrectionCurves>().enabled = true;
	cameraController.ApplyEffectConfiguration(isGameplayLevel, isBloomForced);
    }
    public void StopCameraShake()
    {
	cameraShakeFSM.Fsm.Event("CANCEL SHAKE");
    }
    public void ResumeCameraShake()
    {
	cameraShakeFSM.Fsm.Event("RESUME SHAKE");
    }

}
