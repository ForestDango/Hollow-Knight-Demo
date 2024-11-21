using GlobalEnums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraController : MonoBehaviour
{
    private bool verboseMode;

    public CameraMode mode;
    private CameraMode prevMode;

    public bool atSceneBounds;//是否位于场景的边界外
    public bool atHorizontalSceneBounds;//是否位于场景的X轴方向的边界外

    private bool isGameplayScene;
    public tk2dTileMap tilemap; //通过tk2dTileMap的高度和宽度来确定场景的宽度和长度以及摄像机限制
    public float sceneWidth; //场景的高度
    public float sceneHeight; //场景的宽度
    public float xLimit;
    public float yLimit;

    public Vector3 destination;
    private float targetDeltaX; //目标在Time.DeltaTime移动的X方向距离
    private float targetDeltaY;//目标在Time.DeltaTime移动的Y方向距离

    public float lookOffset; //视线偏移量

    private Vector3 velocity;
    private Vector3 velocityX;
    private Vector3 velocityY;
    private float maxVelocityCurrent;
    public float maxVelocity; //可容忍的最大速度

    public float dampTime; //damp时间
    public float dampTimeX;//X方向的damp时间
    public float dampTimeY;//Y方向的damp时间

    private float startLockedTimer; //开始计入锁定区域的倒计时
    public List<CameraLockArea> lockZoneList; //场景中所有的CameraLockArea
    public float xLockMin;
    public float xLockMax;
    public float yLockMin;
    public float yLockMax;
    private CameraLockArea currentLockArea; //当前的锁定区域
    public Vector2 lastLockPosition; 

    private Camera cam;
    public CameraTarget camTarget;
    private GameManager gm;
    private PlayMakerFSM fadeFSM;
    private HeroController hero_ctrl;
    private Transform cameraParent;


    private void LateUpdate()
    {
	float x = transform.position.x;
	float y = transform.position.y;
	float z = transform.position.z;
	float x2 = cameraParent.position.x;
	float y2 = cameraParent.position.y;
	if(isGameplayScene && mode != CameraMode.FROZEN)
	{
	    lookOffset = 0f;
	    UpdateTargetDestinationDelta();
	    Vector3 vector = cam.WorldToViewportPoint(camTarget.transform.position);
	    Vector3 vector2 = new Vector3(targetDeltaX, targetDeltaY, 0f) - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, vector.z));
	    destination = new Vector3(x + vector2.x, y + vector2.y, z);
	    if(mode == CameraMode.LOCKED && currentLockArea != null)
	    {
		if(lookOffset > 0f && currentLockArea.preventLookUp && destination.y > currentLockArea.cameraYMax)
		{
		    if (transform.position.y > currentLockArea.cameraYMax)
		    {
			destination = new Vector3(destination.x, destination.y - lookOffset, destination.z);
		    }
		    else
		    {
			destination = new Vector3(destination.x, currentLockArea.cameraYMax, destination.z);
		    }
		}
		if(lookOffset < 0f && currentLockArea.preventLookDown && destination.y < currentLockArea.cameraYMin)
		{
		    if (transform.position.y < currentLockArea.cameraYMin)
		    {
			destination = new Vector3(destination.x, destination.y - lookOffset, destination.z);
		    }
		    else
		    {
			destination = new Vector3(destination.x, currentLockArea.cameraYMin, destination.z);
		    }
		}
	    }
	    if(mode == CameraMode.FOLLOWING || mode == CameraMode.LOCKED)
	    {
		destination = KeepWithinSceneBounds(destination);
	    }
	    Vector3 vector3 = Vector3.SmoothDamp(transform.position, new Vector3(destination.x, y, z), ref velocityX, dampTimeX);
	    Vector3 vector4 = Vector3.SmoothDamp(transform.position, new Vector3(x, destination.y, z), ref velocityY, dampTimeY);
	    transform.SetPosition2D(vector3.x, vector4.y);
	    x = transform.position.x;
	    y = transform.position.y;
	    if(velocity.magnitude > maxVelocityCurrent)
	    {
		velocity = velocity.normalized * maxVelocityCurrent;
	    }
	}
	if (isGameplayScene)
	{
	    if (x + x2 < 14.6f)
	    {
		transform.SetPositionX(14.6f);
	    }
	    if (transform.position.x + x2 > xLimit)
	    {
		transform.SetPositionX(xLimit);
	    }
	    if (transform.position.y + y2 < 8.3f)
	    {
		transform.SetPositionY(8.3f);
	    }
	    if (transform.position.y + y2 > yLimit)
	    {
		transform.SetPositionY(yLimit);
	    }
	    if (startLockedTimer > 0f)
	    {
		startLockedTimer -= Time.deltaTime;
	    }
	}
    }

    private void OnDisable()
    {
	if(hero_ctrl != null)
	{
	    hero_ctrl.heroInPosition -= PositionToHero;
	}
    }

    public void GameInit()
    {
	gm = GameManager.instance;
	cam = GetComponent<Camera>();
	cameraParent = transform.parent.transform;
	fadeFSM = FSMUtility.LocateFSM(gameObject, "CameraFade");
	ApplyEffectConfiguration(false, false);
	gm.UnloadingLevel += OnLevelUnload;
    }

    public void SceneInit()
    {
	startLockedTimer = 0.5f;
	velocity = Vector3.zero;
	bool isBloomForced = false;
	if (gm.IsGameplayScene())
	{
	    isGameplayScene = true;
	    if (hero_ctrl == null)
	    {
		hero_ctrl = HeroController.instance;
		hero_ctrl.heroInPosition += PositionToHero;
	    }
	    lockZoneList = new List<CameraLockArea>();
	    GetTilemapInfo();
	    xLockMin = 0f;
	    xLockMax = xLimit;
	    yLockMin = 0f;
	    yLockMax = yLimit;
	    dampTimeX = dampTime;
	    dampTimeY = dampTime;
	    maxVelocityCurrent = maxVelocity;
	    string currentMapZone = gm.GetCurrentMapZone();
	    string name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
	    if (name != null && name.StartsWith("Dream_Guardian_"))
	    {
		isBloomForced = true;
	    }
	}
	else
	{
	    isGameplayScene = false;
	    if (gm.IsMenuScene())
	    {
		isBloomForced = true;
	    }
	}
	ApplyEffectConfiguration(isGameplayScene, isBloomForced);
    }

    public void PositionToHero(bool forceDirect)
    {
	StartCoroutine(DoPositionToHero(forceDirect));
    }

    public void FadeSceneIn()
    {
	GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE SCENE IN");
    }

    /// <summary>
    /// 如果卸载掉该场景了就把lockZoneList里的内容全部清空
    /// </summary>
    private void OnLevelUnload()
    {
	if (verboseMode)
	{
	    Debug.Log("Removing cam locks. (" + lockZoneList.Count.ToString() + " total)");
	}
	while (lockZoneList.Count > 0)
	{
	    ReleaseLock(lockZoneList[0]);
	}
    }

    /// <summary>
    /// 进入锁定区域
    /// </summary>
    /// <param name="lockArea"></param>
    public void LockToArea(CameraLockArea lockArea)
    {
	if (!lockZoneList.Contains(lockArea))
	{
	    if (verboseMode)
	    {
		Debug.LogFormat("LockZone Activated: {0} at startLockedTimer {1} ({2}s)", new object[]
		{
		    lockArea.name,
		    startLockedTimer,
		    Time.timeSinceLevelLoad
		});
	    }
	    lockZoneList.Add(lockArea);
	    if (currentLockArea != null && currentLockArea.maxPriority && !lockArea.maxPriority)
		return;
	    currentLockArea = lockArea;
	    SetMode(CameraMode.LOCKED);
	    if(lockArea.cameraXMin < 0f)
	    {
		xLockMin = 14.6f;
	    }
	    else
	    {
		xLockMin = lockArea.cameraXMin;
	    }
	    if(lockArea.cameraXMax < 0f)
	    {
		xLockMax = xLimit;
	    }
	    else
	    {
		xLockMax = lockArea.cameraXMax;
	    }
	    if(lockArea.cameraXMin < 0f)
	    {
		yLockMin = 8.3f;
	    }
	    else
	    {
		yLockMin = lockArea.cameraYMin;
	    }
	    if (lockArea.cameraYMax < 0f)
	    {
		yLockMax = yLimit;
	    }
	    else
	    {
		yLockMax = lockArea.cameraYMax;
	    }
	    if(startLockedTimer > 0f) //迅速进入锁定区域
	    {
		camTarget.transform.SetPosition2D(KeepWithinLockBounds(hero_ctrl.transform.position));
		camTarget.destination = camTarget.transform.position;
		camTarget.EnterLockZoneInstant(xLockMin, xLockMax, yLockMin, yLockMax);
		transform.SetPosition2D(KeepWithinLockBounds(hero_ctrl.transform.position));
		destination = transform.position;
		return;
	    }
	    camTarget.EnterLockZone(xLockMin, xLockMax, yLockMin, yLockMax); //不用这么急的进入锁定区域
	}
    }

    /// <summary>
    /// 释放锁定区域
    /// </summary>
    /// <param name="lockArea"></param>
    public void ReleaseLock(CameraLockArea lockArea)
    {
	lockZoneList.Remove(lockArea);
	if (verboseMode)
	{
	    Debug.Log("LockZone Released " + lockArea.name);
	}
	if (lockArea == currentLockArea)
	{
	    if(lockZoneList.Count > 0)
	    {
		currentLockArea = lockZoneList[lockZoneList.Count - 1];
		xLockMin = currentLockArea.cameraXMin;
		xLockMax = currentLockArea.cameraXMax;
		yLockMin = currentLockArea.cameraYMin;
		yLockMax = currentLockArea.cameraYMax;
		camTarget.enteredFromLockZone = true;
		camTarget.EnterLockZone(xLockMin, xLockMax, yLockMin, yLockMax);
		return;
	    }
	    lastLockPosition = transform.position;
	    if (camTarget != null)
	    {
		camTarget.enteredFromLockZone = false;
		camTarget.ExitLockZone();
	    }
	    currentLockArea = null;
	    if (!hero_ctrl.cState.dead)
	    {
		SetMode(CameraMode.FOLLOWING);
		return;	
	    }
	}
	else if (verboseMode)
	{
	    Debug.Log("LockZone was not the current lock when removed.");
	}
    }

    /// <summary>
    /// 将位置锁定到主角身上
    /// </summary>
    /// <param name="forceDirect"></param>
    /// <returns></returns>
    private IEnumerator DoPositionToHero(bool forceDirect)
    {
	yield return new WaitForFixedUpdate();
	GetTilemapInfo();
	camTarget.PositionToStart();
	CameraMode previousMode = mode;
	SetMode(CameraMode.FROZEN);

	Vector3 newPosition = KeepWithinSceneBounds(camTarget.transform.position);
	if (verboseMode)
	{
	    Debug.LogFormat("CC - STR: NewPosition: {0} TargetDelta: ({1}, {2}) CT-XOffset: {3} HeroPos: {4} CT-Pos: {5}", new object[]
	    {
		newPosition,
		targetDeltaX,
		targetDeltaY,
		camTarget.xOffset,
		hero_ctrl.transform.position,
		camTarget.transform.position
	    });
	}
	if (forceDirect)
	{
	    if (verboseMode)
	    {
		Debug.Log("====> TEST 1a - ForceDirect Positioning Mode");
	    }
	    transform.SetPosition2D(newPosition);
	}
	else
	{
	    bool flag2;
	    bool flag = IsAtHorizontalSceneBounds(newPosition, out flag2);
	    bool flag3 = false;
	    if(currentLockArea != null)
	    {
		flag3 = true;
	    }
	    if (flag3)
	    {
		if (verboseMode)
		{
		    Debug.Log("====> TEST 3 - Lock Zone Active");
		}
		PositionToHeroFacing(newPosition, true);
		transform.SetPosition2D(KeepWithinLockBounds(transform.position));
	    }
	    else
	    {
		if (verboseMode)
		{
		    Debug.Log("====> TEST 4 - No Lock Zone");
		}
		PositionToHeroFacing(newPosition, false);
	    }
	    if (flag)
	    {
		if (verboseMode)
		{
		    Debug.Log("====> TEST 2 - At Horizontal Scene Bounds");
		}
		if ((flag2 && !hero_ctrl.cState.facingRight) || (!flag2 && hero_ctrl.cState.facingRight))
		{
		    if (verboseMode)
		    {
			Debug.Log("====> TEST 2a - Hero Facing Bounds");
		    }
		    transform.SetPosition2D(newPosition);
		}
		else
		{
		    if (verboseMode)
		    {
			Debug.Log("====> TEST 2b - Hero Facing Inwards");
		    }
		    if (IsTouchingSides(targetDeltaX))
		    {
			if (verboseMode)
			{
			    Debug.Log("Xoffset still touching sides");
			}
			transform.SetPosition2D(newPosition);
		    }
		    else
		    {
			if (verboseMode)
			{
			    Debug.LogFormat("Not Touching Sides with Xoffset CT: {0} Hero: {1}", new object[]
			    {
				camTarget.transform.position,
				hero_ctrl.transform.position
			    });
			}
			if (hero_ctrl.cState.facingRight)
			{
			    transform.SetPosition2D(hero_ctrl.transform.position.x + 1f, newPosition.y);
			}
			else
			{
			    transform.SetPosition2D(hero_ctrl.transform.position.x - 1f, newPosition.y);
			}
		    }
		}
	    }
	}
	destination = transform.position;
	velocity = Vector3.zero;
	velocityX = Vector3.zero;
	velocityY = Vector3.zero;
	yield return new WaitForSeconds(0.1f);
	GameCameras.instance.cameraFadeFSM.Fsm.Event("LEVEL LOADED");
	if (previousMode == CameraMode.FROZEN)
	{
	    SetMode(CameraMode.FOLLOWING);
	}
	else if(previousMode == CameraMode.LOCKED)
	{
	    if (currentLockArea != null)
	    {
		SetMode(previousMode);
	    }
	    else
	    {
		SetMode(CameraMode.FOLLOWING);
	    }
	}
	else
	{
	    SetMode(previousMode);
	}
	if (verboseMode)
	{
	    Debug.LogFormat("CC - PositionToHero FIN: - TargetDelta: ({0}, {1}) Destination: {2} CT-XOffset: {3} NewPosition: {4} CamTargetPos: {5} HeroPos: {6}", new object[]
	    {
		targetDeltaX,
		targetDeltaY,
		destination,
		camTarget.xOffset,
		newPosition,
		camTarget.transform.position,
		hero_ctrl.transform.position
	    });
	}
    }

    public Vector2 KeepWithinLockBounds(Vector2 targetDest)
    {
	float x = targetDest.x;
	float y = targetDest.y;
	if (x < xLockMin)
	{
	    x = xLockMin;
	}
	if (x > xLockMax)
	{
	    x = xLockMax;
	}
	if (y < yLockMin)
	{
	    y = yLockMin;
	}
	if (y > yLockMax)
	{
	    y = yLockMax;
	}
	return new Vector2(x, y);
    }

    private void PositionToHeroFacing(Vector3 newPosition, bool useXOffset)
    {
	if (useXOffset)
	{
	    transform.SetPosition2D(newPosition.x + camTarget.xOffset, newPosition.y);
	    return;
	}
	if (hero_ctrl.cState.facingRight)
	{
	    transform.SetPosition2D(newPosition.x + 1f, newPosition.y);
	    return;
	}
	transform.SetPosition2D(newPosition.x - 1f, newPosition.y);
    }

    /// <summary>
    /// 获取当前场景的TileMap的信息
    /// </summary>
    private void GetTilemapInfo()
    {
	tilemap = gm.tilemap;
	sceneWidth = tilemap.width;
	sceneHeight = tilemap.height;
	xLimit = sceneWidth - 14.6f;
	yLimit = sceneHeight - 8.3f;
    }

    /// <summary>
    /// 更新当前的targetDeltaX和targetDeltaY
    /// </summary>
    private void UpdateTargetDestinationDelta()
    {
	targetDeltaX = camTarget.transform.position.x + camTarget.xOffset + camTarget.dashOffset;
	targetDeltaY = camTarget.transform.position.y + camTarget.fallOffset + lookOffset;
    }

    /// <summary>
    /// 是否位于横向的场景边界
    /// </summary>
    /// <param name="targetDest"></param>
    /// <param name="leftSide"></param>
    /// <returns></returns>
    private bool IsAtHorizontalSceneBounds(Vector2 targetDest, out bool leftSide)
    {
	bool result = false;
	leftSide = false;
	if (targetDest.x <= 14.6f)
	{
	    result = true;
	    leftSide = true;
	}
	if (targetDest.x >= xLimit)
	{
	    result = true;
	    leftSide = false;
	}
	return result;
    }

    /// <summary>
    /// 保持在场景边界内
    /// </summary>
    /// <param name="targetDest"></param>
    /// <returns></returns>
    public Vector3 KeepWithinSceneBounds(Vector3 targetDest)
    {
	Vector3 vector = targetDest;
	bool flag = false;
	bool flag2 = false;
	if (vector.x < 14.6f)
	{
	    vector = new Vector3(14.6f, vector.y, vector.z);
	    flag = true;
	    flag2 = true;
	}
	if (vector.x > xLimit)
	{
	    vector = new Vector3(xLimit, vector.y, vector.z);
	    flag = true;
	    flag2 = true;
	}
	if (vector.y < 8.3f)
	{
	    vector = new Vector3(vector.x, 8.3f, vector.z);
	    flag = true;
	}
	if (vector.y > yLimit)
	{
	    vector = new Vector3(vector.x, yLimit, vector.z);
	    flag = true;
	}
	atSceneBounds = flag;
	atHorizontalSceneBounds = flag2;
	return vector;
    }

    /// <summary>
    /// 是否碰到场景边缘
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private bool IsTouchingSides(float x)
    {
	bool result = false;
	if (x <= 14.6f)
	{
	    result = true;
	}
	if (x >= xLimit)
	{
	    result = true;
	}
	return result;
    }

    public void FreezeInPlace(bool freezeTargetAlso = false)
    {
	SetMode(CameraMode.FROZEN);
	if (freezeTargetAlso)
	{
	    camTarget.FreezeInPlace();
	}
    }

    public void FadeOut(CameraFadeType type)
    {
	SetMode(CameraMode.FROZEN);
	if (type == CameraFadeType.LEVEL_TRANSITION)
	{
	    fadeFSM.Fsm.Event("FADE OUT");
	    return;
	}
	if(type == CameraFadeType.HERO_DEATH)
	{
	    fadeFSM.Fsm.Event("RESPAWN FADE");
	    return;
	}
	if (type == CameraFadeType.HERO_HAZARD_DEATH)
	{
	    fadeFSM.Fsm.Event("HAZARD FADE");
	    return;
	}
	if (type == CameraFadeType.JUST_FADE)
	{
	    fadeFSM.Fsm.Event("JUST FADE");
	    return;
	}
	if (type == CameraFadeType.START_FADE)
	{
	    fadeFSM.Fsm.Event("START FADE");
	}
    }

    public void ApplyEffectConfiguration(bool isGameplayLevel, bool isBloomForced)
    {
	bool flag = Platform.Current.InitialGraphicsTier > Platform.GraphicsTiers.Low;
	GetComponent<FastNoise>().enabled = (isGameplayLevel && flag);
	GetComponent<BloomOptimized>().enabled = (flag || isBloomForced);
	GetComponent<BrightnessEffect>().enabled = flag;
	GetComponent<ColorCorrectionCurves>().enabled = false; //TODO:
    }

    public void ResetStartTimer()
    {
	startLockedTimer = 0.5f;
    }

    public void SetMode(CameraMode newMode)
    {
	if (newMode != mode)
	{
	    if (newMode == CameraMode.PREVIOUS)
	    {
		mode = prevMode;
		return;
	    }
	    prevMode = mode;
	    mode = newMode;
	}
    }

    public enum CameraMode
    {
	FROZEN,
	FOLLOWING,
	LOCKED,
	PANNING,
	FADEOUT,
	FADEIN,
	PREVIOUS
    }
}
