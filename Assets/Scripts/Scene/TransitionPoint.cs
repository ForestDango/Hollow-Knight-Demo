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
    public bool isADoor; //是否是个门，意思是当你到传送点的时候还需要UI提示后按键输入才能触发转移
    public bool dontWalkOutOfDoor; //不要走出门

    [Header("Gate Entry")]
    [Tooltip("The wait time before entering from this gate (not the target gate).")]
    public float entryDelay; //转移后的延迟
    public bool alwaysEnterRight; //进入这个转移点总是朝右看
    public bool alwaysEnterLeft; //进入这个转移点总是朝左看

    [Header("Force Hard Land (Top Gates Only)")]
    [Space(5f)]
    public bool hardLandOnExit; //强制重着地

    [Header("Destination Scene")]
    [Space(5f)]
    public string targetScene; //目标场景
    public string entryPoint; //进入的点
    public Vector2 entryOffset; //进入时的偏移量

    [SerializeField] private bool alwaysUnloadUnusedAssets;
    public PlayMakerFSM customFadeFSM; //自定义Fade的playmakerFSM

    [Header("Hazard Respawn")]
    [Space(5f)]
    public bool nonHazardGate; //这个门不能用来做HazardRespawn的重生点
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
    public static string lastEntered = ""; //记录最后进入的TransitionPoint

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
		gm.BeginSceneTransition(new GameManager.SceneLoadInfo
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

    /// <summary>
    /// 获取当前门的位置，请注意你的TransitionPoint名字一定要有如下字段
    /// </summary>
    /// <returns></returns>
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
