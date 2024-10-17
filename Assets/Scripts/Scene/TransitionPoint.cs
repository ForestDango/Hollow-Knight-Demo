using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Audio;

public class TransitionPoint : MonoBehaviour
{
    private GameManager gm;
    private PlayerData playerData;
    private bool activated;

    [Header("Door Type Gate Settings")]
    [Space(5f)]
    public bool isADoor;
    public bool dontWalkOutOfDoor;

    [Header("Gate Entry")]
    [Tooltip("The wait time before entering from this gate (not the target gate).")]
    public float entryDelay;
    public bool alwaysEnterRight;
    public bool alwaysEnterLeft;

    [Header("Force Hard Land (Top Gates Only)")]
    [Space(5f)]
    public bool hardLandOnExit;

    [Header("Destination Scene")]
    [Space(5f)]
    public string targetScene;
    public string entryPoint;
    public Vector2 entryOffset;

    [SerializeField] private bool alwaysUnloadUnusedAssets;
    public PlayMakerFSM customFadeFSM;

    [Header("Hazard Respawn")]
    [Space(5f)]
    public bool nonHazardGate;
    public HazardRespawnMarker respawnMarker;

    [Header("Set Audio Snapshots")]
    [Space(5f)]
    public AudioMixerSnapshot atmosSnapshot;
    public AudioMixerSnapshot enviroSnapshot;
    public AudioMixerSnapshot actorSnapshot;
    public AudioMixerSnapshot musicSnapshot;

    private Color myGreen = new Color(0f, 0.8f, 0f, 0.5f);

    [Header("Cosmetics")]
    public GameManager.SceneLoadVisualizations sceneLoadVisualization;
    public bool customFade;
    public bool forceWaitFetch;

    private static List<TransitionPoint> transitionPoints;
    public static string lastEntered = "";

    public delegate void BeforeTransitionEvent();
    public event BeforeTransitionEvent OnBeforeTransition;

    public static List<TransitionPoint> TransitionPoints
    {
	get
	{
	    return transitionPoints;
	}
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
	transitionPoints = new List<TransitionPoint>();
    }

    protected void Awake()
    {
	transitionPoints.Add(this);
    }

    protected void OnDestroy()
    {
	transitionPoints.Remove(this);
    }

    private void Start()
    {
	gm = GameManager.instance;
	playerData = PlayerData.instance;
	if(!nonHazardGate && respawnMarker == null)
	{
	    Debug.LogError(string.Concat(new string[]
	    {
		"Transition Gate ",
		name,
		" in ",
		gm.sceneName,
		" does not have its respawn marker set in inspector."
	    }));
	}
    }

    private void OnTriggerEnter2D(Collider2D movingObj)
    {
	if(!isADoor && movingObj .gameObject.layer == 9 && gm.gameState == GameState.PLAYING)
	{
	    if(!string.IsNullOrEmpty(targetScene) && !string.IsNullOrEmpty(entryPoint))
	    {
		if (customFadeFSM)
		{
		    customFadeFSM.SendEvent("FADE");
		}
		if (atmosSnapshot != null)
		{
		    atmosSnapshot.TransitionTo(1.5f);
		}
		if (enviroSnapshot != null)
		{
		    enviroSnapshot.TransitionTo(1.5f);
		}
		if (actorSnapshot != null)
		{
		    actorSnapshot.TransitionTo(1.5f);
		}
		if (musicSnapshot != null)
		{
		    musicSnapshot.TransitionTo(1.5f);
		}
		activated = true;
		lastEntered = gameObject.name;
		if (OnBeforeTransition != null)
		{
		    OnBeforeTransition();
		}
		gm.BeginSceneTransiton(new GameManager.SceneLoadInfo
		{
		    SceneName = targetScene,
		    EntryGateName = entryPoint,
		    HeroLeaveDirection = new GatePosition?(GetGatePosition()),
		    EntryDelay = entryDelay,
		    WaitForSceneTransitionCameraFade = true,
		    PreventCameraFadeOut = (customFadeFSM != null),
		    Visualization = sceneLoadVisualization,
		    AlwaysUnloadUnusedAssets = alwaysUnloadUnusedAssets,
		    forceWaitFetch = forceWaitFetch
		});
		return;
	    }
	    Debug.LogError(gm.sceneName + " " + name + " no target scene has been set on this gate.");
	}
    }

    private void OnTriggerStay2D(Collider2D movingObj)
    {
	if (!activated)
	{
	    OnTriggerEnter2D(movingObj);
	}
    }

    private void OnDrawGizmos()
    {
	if (transform != null)
	{
	    Vector3 position = transform.position + new Vector3(0f, GetComponent<BoxCollider2D>().bounds.extents.y + 1.5f, 0f);
	    GizmoUtility.DrawText(GUI.skin, targetScene, position, new Color?(myGreen), 10, 0f);
	}
    }

    public GatePosition GetGatePosition()
    {
	string name = base.name;
	if (name.Contains("top"))
	{
	    return GatePosition.top;
	}
	if (name.Contains("right"))
	{
	    return GatePosition.right;
	}
	if (name.Contains("left"))
	{
	    return GatePosition.left;
	}
	if (name.Contains("bot"))
	{
	    return GatePosition.bottom;
	}
	if (name.Contains("door") || isADoor)
	{
	    return GatePosition.door;
	}
	Debug.LogError("Gate name " + name + "does not conform to a valid gate position type. Make sure gate name has the form 'left1'");
	return GatePosition.unknown;
    }

    public void SetTargetSceneName(string newScene)
    {
	targetScene = newScene;
    }

}
