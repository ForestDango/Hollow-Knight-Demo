using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    private bool verboseMode;
    public bool isGameplayScene;

    private float slowTimer;
    public float slowTime = 0.5f;
    private float dampTimeX;
    private float dampTimeY;
    public float dampTimeSlow;
    public float dampTimeNormal;

    private float snapDistance = 0.15f;

    private Vector3 heroPrevPosition;
    public Vector3 destination;
    public Vector3 velocityX;
    public Vector3 velocityY;

    public float xOffset; //相机X轴方向上的偏移量
    public float xLookAhead; //相机X轴方向上的提前量

    public float dashOffset; //冲刺时的相机偏移量
    public float fallOffset; //下落时相机的偏移量
    public float dashLookAhead;//冲刺时相机的提前量

    public bool enteredLeft; //从哪个方向进入锁定区域的
    public bool enteredRight;
    public bool enteredTop;
    public bool enteredBot;
    public bool enteredFromLockZone;

    public float xLockMin;
    public float xLockMax;
    public float yLockMin;
    public float yLockMax;

    public bool exitedLeft;//从哪个方向离开锁定区域的
    public bool exitedRight;
    public bool exitedTop;
    public bool exitedBot;

    [HideInInspector]
    public GameManager gm;
    [HideInInspector]
    public HeroController hero_ctrl;
    private Transform heroTransform;

    public CameraController cameraCtrl;
    public TargetMode mode;
    public bool stickToHeroX;//是否黏住玩家的X位置
    public bool stickToHeroY;//是否黏住玩家的Y位置

    public bool fallStick;//是否黏住玩家下落的位置
    public float fallCatcher; //下落位置捕捉

    public void GameInit()
    {
	gm = GameManager.instance;
	if(cameraCtrl == null)
	{
	    cameraCtrl = transform.parent.GetComponent<CameraController>();
	}
    }

    public void SceneInit()
    {
	if(gm == null)
	{
	    gm = GameManager.instance;
	}
	isGameplayScene = true;
	hero_ctrl = HeroController.instance;
	heroTransform = hero_ctrl.transform;
	mode = TargetMode.FOLLOW_HERO;
	stickToHeroX = true;
	stickToHeroY = true;
	fallCatcher = 0f;
	xLockMin = 0f;
	xLockMax = cameraCtrl.xLimit;
	yLockMin = 0f;
	yLockMax = cameraCtrl.yLimit;
    }

    private void Update()
    {
	if(hero_ctrl == null || !isGameplayScene)
	{
	    mode = TargetMode.FREE;
	    return;
	}
	if (isGameplayScene)
	{
	    float num = transform.position.x;
	    float num2 = transform.position.y;
	    float z = transform.position.z;
	    float x = heroTransform.position.x;
	    float y = heroTransform.position.y;
	    Vector3 position = heroTransform.position;
	    if(mode == TargetMode.FOLLOW_HERO)
	    {
		SetDampTime();
		destination = heroTransform.position;
		if (!fallStick && fallCatcher <= 0f)
		{
		    transform.position = new Vector3(Vector3.SmoothDamp(transform.position, new Vector3(destination.x, transform.position.y, z), ref velocityX, dampTimeX).x, Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, destination.y, z), ref velocityY, dampTimeY).y, z);
		}
		else
		{
		    transform.position = new Vector3(Vector3.SmoothDamp(transform.position, new Vector3(destination.x, transform.position.y, z), ref velocityX, dampTimeX).x, transform.position.y, z);
		}
		num = transform.position.x;
		num2 = transform.position.y;
		z = transform.position.z;
		if ((heroPrevPosition.x < num && x > num) || (heroPrevPosition.x > num && x < num) || (num >= x - snapDistance && num <= x + snapDistance))
		{
		    stickToHeroX = true;
		}
		if ((heroPrevPosition.y < num2 && y > num2) || (heroPrevPosition.y > num2 && y < num2) || (num2 >= y - snapDistance && num2 <= y + snapDistance))
		{
		    stickToHeroY = true;
		}
		if (stickToHeroX)
		{
		    transform.SetPositionX(x);
		    num = x;
		}
		if (stickToHeroY)
		{
		    transform.SetPositionY(y);
		    num2 = y;
		}
	    }
	    if(mode == TargetMode.LOCK_ZONE)
	    {
		SetDampTime();
		destination = heroTransform.position;
		if(destination.x < xLockMin)
		{
		    destination.x = xLockMin;
		}
		if (destination.x > xLockMax)
		{
		    destination.x = xLockMax;
		}
		if (destination.y < yLockMin)
		{
		    destination.y = yLockMin;
		}
		if (destination.y > yLockMax)
		{
		    destination.y = yLockMax;
		}
		if (!fallStick && fallCatcher <= 0f)
		{
		    transform.position = new Vector3(Vector3.SmoothDamp(transform.position, new Vector3(destination.x, num2, z), ref velocityX, dampTimeX).x, Vector3.SmoothDamp(transform.position, new Vector3(num, destination.y, z), ref velocityY, dampTimeY).y, z);
		}
		else
		{
		    transform.position = new Vector3(Vector3.SmoothDamp(transform.position, new Vector3(destination.x, num2, z), ref velocityX, dampTimeX).x, num2, z);
		}
		num = transform.position.x;
		num2 = transform.position.y;
		z = transform.position.z;
		if ((heroPrevPosition.x < num && x > num) || (heroPrevPosition.x > num && x < num) || (num >= x - snapDistance && num <= x + snapDistance))
		{
		    stickToHeroX = true;
		}
		if ((heroPrevPosition.y < num2 && y > num2) || (heroPrevPosition.y > num2 && y < num2) || (num2 >= y - snapDistance && num2 <= y + snapDistance))
		{
		    stickToHeroY = true;
		}
		if (stickToHeroX)
		{
		    bool flag = false;
		    if (x >= xLockMin && x <= xLockMax)
		    {
			flag = true;
		    }
		    if (x <= xLockMax && x >= num)
		    {
			flag = true;
		    }
		    if (x >= xLockMin && x <= num)
		    {
			flag = true;
		    }
		    if (flag)
		    {
			transform.SetPositionX(x);
			num = x;
		    }
		}
		if (stickToHeroY)
		{
		    bool flag2 = false;
		    if (y >= yLockMin && y <= yLockMax)
		    {
			flag2 = true;
		    }
		    if (y <= yLockMax && y >= num2)
		    {
			flag2 = true;
		    }
		    if (y >= yLockMin && y <= num2)
		    {
			flag2 = true;
		    }
		    if (flag2)
		    {
			transform.SetPositionY(y);
		    }
		}
	    }
	    if(hero_ctrl != null)
	    {
		if (hero_ctrl.cState.facingRight)
		{
		    if(xOffset < xLookAhead)
		    {
			xOffset += Time.deltaTime * 6f;
		    }
		}
		else if (xOffset > -xLookAhead)
		{
		    xOffset -= Time.deltaTime * 6f;
		}
		if(xOffset < -xLookAhead)
		{
		    xOffset = -xLookAhead;
		}
		if (xOffset > xLookAhead)
		{
		    xOffset = xLookAhead;
		}
		if(mode == TargetMode.LOCK_ZONE)
		{
		    if (x < xLockMin && hero_ctrl.cState.facingRight)
		    {
			xOffset = x - num + 1f;
		    }
		    if (x > xLockMax && !hero_ctrl.cState.facingRight)
		    {
			xOffset = x - num - 1f;
		    }
		    if (num + xOffset > xLockMax)
		    {
			xOffset = xLockMax - num;
		    }
		    if (num + xOffset < xLockMin)
		    {
			xOffset = xLockMin - num;
		    }
		}
		if (xOffset < -xLookAhead)
		{
		    xOffset = xLookAhead;
		}
		if (xOffset > xLookAhead)
		{
		    xOffset = xLookAhead;
		}
		if (hero_ctrl.cState.dashing && (hero_ctrl.current_velocity.x > 5f || hero_ctrl.current_velocity.x < -5f))
		{
		    if (hero_ctrl.cState.facingRight)
		    {
			dashOffset = dashLookAhead;
		    }
		    else
		    {
			dashOffset = -dashLookAhead;
		    }
		    if (mode == TargetMode.LOCK_ZONE)
		    {
			if (num + dashOffset > xLockMax)
			{
			    dashOffset = 0f;
			}
			if (num + dashOffset < xLockMin)
			{
			    dashOffset = 0f;
			}
			if (x > xLockMax || x < xLockMin)
			{
			    dashOffset = 0f;
			}
		    }
		}
		else
		{
		    dashOffset = 0f;
		}
		heroPrevPosition = heroTransform.position;
	    }
	    if(hero_ctrl != null && !hero_ctrl.cState.falling)
	    {
		fallCatcher = 0f;
		fallStick = false;
	    }
	    if (mode == TargetMode.FOLLOW_HERO || mode == TargetMode.LOCK_ZONE)
	    {
		if (hero_ctrl.cState.falling && cameraCtrl.transform.position.y > y + 0.1f && !fallStick && (cameraCtrl.transform.position.y - 0.1f >= yLockMin || mode != TargetMode.LOCK_ZONE))
		{
		    //Debug.LogFormat("Fall Catcher");								    
		    cameraCtrl.transform.SetPositionY(cameraCtrl.transform.position.y - fallCatcher * Time.deltaTime);
		    if (mode == TargetMode.LOCK_ZONE && cameraCtrl.transform.position.y < yLockMin)
		    {
			cameraCtrl.transform.SetPositionY(yLockMin);
		    }
		    if (cameraCtrl.transform.position.y < 8.3f)
		    {
			cameraCtrl.transform.SetPositionY(8.3f);
		    }
		    if (fallCatcher < 25f)
		    {
			fallCatcher += 80f * Time.deltaTime;
		    }
		    if (cameraCtrl.transform.position.y < heroTransform.position.y + 0.1f)
		    {
			fallStick = true;
		    }
		    transform.SetPositionY(cameraCtrl.transform.position.y);
		    num2 = cameraCtrl.transform.position.y;
		}
		if (fallStick)
		{
		    fallCatcher = 0f;
		    if (heroTransform.position.y + 0.1f >= yLockMin || mode != TargetMode.LOCK_ZONE)
		    {
			//Debug.LogFormat("将cameraCtrl的Y坐标设置成heroTransform，再将cameraTarget的Y坐标设置成cameraCtrl的Y坐标");
			cameraCtrl.transform.SetPositionY(heroTransform.position.y + 0.1f);
			transform.SetPositionY(cameraCtrl.transform.position.y);
			num2 = cameraCtrl.transform.position.y;
		    }
		    if (mode == TargetMode.LOCK_ZONE && cameraCtrl.transform.position.y < yLockMin)
		    {
			cameraCtrl.transform.SetPositionY(yLockMin);
		    }
		    if (cameraCtrl.transform.position.y < 8.3f)
		    {
			cameraCtrl.transform.SetPositionY(8.3f);
		    }
		}
	    }
	}
    }

    public void PositionToStart()
    {
	float x = transform.position.x;
	Vector3 position = transform.position;
	float x2 = heroTransform.position.x;
	float y = heroTransform.position.y;
	velocityX = Vector3.zero;
	velocityY = Vector3.zero;
	destination = heroTransform.position;
	if (hero_ctrl.cState.facingRight)
	{
	    xOffset = 1f;
	}
	else
	{
	    xOffset = -1f;
	}
	if (mode == TargetMode.LOCK_ZONE)
	{
	    if (x2 < xLockMin && hero_ctrl.cState.facingRight)
	    {
		xOffset = x2 - x + 1f;
	    }
	    if (x2 > xLockMax && !hero_ctrl.cState.facingRight)
	    {
		xOffset = x2 - x - 1f;
	    }
	    if (x + xOffset > xLockMax)
	    {
		xOffset = xLockMax - x;
	    }
	    if (x + xOffset < xLockMin)
	    {
		xOffset = xLockMin - x;
	    }
	}
	if (xOffset < -xLookAhead)
	{
	    xOffset = -xLookAhead;
	}
	if (xOffset > xLookAhead)
	{
	    xOffset = xLookAhead;
	}
	if (verboseMode)
	{
	    Debug.LogFormat("CT PTS - xOffset: {0} HeroPos: {1}, {2}", new object[]
	    {
		xOffset,
		x2,
		y
	    });
	}
	if (mode == TargetMode.FOLLOW_HERO)
	{
	    if (verboseMode)
	    {
		Debug.LogFormat("CT PTS - Follow Hero - CT Pos: {0}", new object[]
		{
		    base.transform.position
		});
	    }
	    transform.position = cameraCtrl.KeepWithinSceneBounds(destination);
	}
	else if (mode == TargetMode.LOCK_ZONE)
	{
	    if (destination.x < xLockMin)
	    {
		destination.x = xLockMin;
	    }
	    if (destination.x > xLockMax)
	    {
		destination.x = xLockMax;
	    }
	    if (destination.y < yLockMin)
	    {
		destination.y = yLockMin;
	    }
	    if (destination.y > yLockMax)
	    {
		destination.y = yLockMax;
	    }
	    transform.position = destination;
	    if (verboseMode)
	    {
		Debug.LogFormat("CT PTS - Lock Zone - CT Pos: {0}", new object[]
		{
		    transform.position
		});
	    }
	}
	if (verboseMode)
	{
	    Debug.LogFormat("CT - PTS: HeroPos: {0} Mode: {1} Dest: {2}", new object[]
	    {
		heroTransform.position,
		mode,
		destination
	    });
	}
	heroPrevPosition = heroTransform.position;
    }

    /// <summary>
    /// 进入锁定区域，使用的是dampTimeSlow
    /// </summary>
    /// <param name="xLockMin_var"></param>
    /// <param name="xLockMax_var"></param>
    /// <param name="yLockMin_var"></param>
    /// <param name="yLockMax_var"></param>
    public void EnterLockZone(float xLockMin_var, float xLockMax_var, float yLockMin_var, float yLockMax_var)
    {
	xLockMin = xLockMin_var;
	xLockMax = xLockMax_var;
	yLockMin = yLockMin_var;
	yLockMax = yLockMax_var;
	mode = TargetMode.LOCK_ZONE;
	float x = transform.position.x;
	float y = transform.position.y;
	Vector3 position = transform.position;
	float x2 = heroTransform.position.x;
	float y2 = heroTransform.position.y;
	Vector3 position2 = heroTransform.position;
	if ((!enteredLeft || xLockMin != 14.6f) && (!enteredRight || xLockMax != cameraCtrl.xLimit))
	{
	    dampTimeX = dampTimeSlow;
	}
	if ((!enteredBot || yLockMin != 8.3f) && (!enteredTop || yLockMax != cameraCtrl.yLimit))
	{
	    dampTimeY = dampTimeSlow;
	}
	slowTimer = slowTime;
	if (x >= x2 - snapDistance && x <= x2 + snapDistance)
	{
	    stickToHeroX = true;
	}
	else
	{
	    stickToHeroX = false;
	}
	if (y >= y2 - snapDistance && y <= y2 + snapDistance)
	{
	    stickToHeroY = true; 
	}
	else
	{
	    stickToHeroY = false;
	}
    }

    /// <summary>
    /// 迅速进入锁定区域，
    /// </summary>
    /// <param name="xLockMin_var"></param>
    /// <param name="xLockMax_var"></param>
    /// <param name="yLockMin_var"></param>
    /// <param name="yLockMax_var"></param>
    public void EnterLockZoneInstant(float xLockMin_var, float xLockMax_var, float yLockMin_var, float yLockMax_var)
    {
	xLockMin = xLockMin_var;
	xLockMax = xLockMax_var;
	yLockMin = yLockMin_var;
	yLockMax = yLockMax_var;
	mode = TargetMode.LOCK_ZONE;
	if(transform.position.x < xLockMin)
	{
	    transform.SetPositionX(xLockMin);
	}
	if (transform.position.x > xLockMax)
	{
	    transform.SetPositionX(xLockMax);
	}
	if (transform.position.y < yLockMin)
	{
	    transform.SetPositionY(yLockMin);
	}
	if (transform.position.y > yLockMax)
	{
	    transform.SetPositionY(yLockMax);
	}
	stickToHeroX = true;
	stickToHeroY = true;
    }

    /// <summary>
    /// 离开锁定区域
    /// </summary>
    public void ExitLockZone()
    {
	if (mode == TargetMode.FREE)
	    return;
	if (hero_ctrl.cState.hazardDeath || hero_ctrl.cState.dead )
	{
	    mode = TargetMode.FREE;
	}
	else
	{
	    mode = TargetMode.FOLLOW_HERO;
	}
	if ((!exitedLeft || xLockMin != 14.6f) && (!exitedRight || xLockMax != cameraCtrl.xLimit))
	{
	    dampTimeX = dampTimeSlow;
	}
	if ((!exitedBot || yLockMin != 8.3f) && (!exitedTop || yLockMax != cameraCtrl.yLimit))
	{
	    dampTimeY = dampTimeSlow;
	}
	slowTimer = slowTime;
	stickToHeroX = false;
	stickToHeroY = false;
	fallStick = false;
	xLockMin = 0f;
	xLockMax = cameraCtrl.xLimit;
	yLockMin = 0f;
	yLockMax = cameraCtrl.yLimit;
	if(hero_ctrl!= null)
	{
	    if(transform.position.x >= heroTransform.position.x - snapDistance && transform.position.x <= heroTransform.position.x + snapDistance)
	    {
		stickToHeroX = true;
	    }
	    else
	    {
		stickToHeroX = false;
	    }
	    if (transform.position.y >= heroTransform.position.y - snapDistance && transform.position.y <= heroTransform.position.y + snapDistance)
	    {
		stickToHeroY = true;
	    }
	    else
	    {
		stickToHeroY = false;
	    }
	}
    }

    /// <summary>
    /// 设置Damp时间
    /// </summary>
    private void SetDampTime()
    {
	if (slowTimer > 0f)
	{
	    slowTimer -= Time.deltaTime;
	    return;
	}
	if (dampTimeX > dampTimeNormal)
	{
	    dampTimeX -= 0.007f;
	}
	else if (dampTimeX < dampTimeNormal)
	{
	    dampTimeX = dampTimeNormal;
	}
	if (dampTimeY > dampTimeNormal)
	{
	    dampTimeY -= 0.007f;
	    return;
	}
	if (dampTimeY < dampTimeNormal)
	{
	    dampTimeY = dampTimeNormal;
	}
    }

    public void FreezeInPlace()
    {
	mode = TargetMode.FREE;
    }

    public enum TargetMode
    {
	FOLLOW_HERO,
	LOCK_ZONE,
	BOSS,
	FREE
    }
}
