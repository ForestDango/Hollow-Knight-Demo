using System;
using System.Collections.Generic;
using System.Reflection;
using GlobalEnums;
using UnityEngine;

[Serializable]
public class PlayerData
{
    private static PlayerData _instance;
    public static PlayerData instance
    {
	get
	{
	    if(_instance == null)
	    {
		_instance = new PlayerData();
	    }
	    return _instance;
	}
	set
	{
	    _instance = value;
	}
    }

    public bool disablePause;

    public int health;
    public int maxHealth;
    public int maxHealthBase;
    public int prevHealth;

    public int nailDamage;

    public string respawnScene;
    [NonSerialized]
    public Vector3 hazardRespawnLocation;
    public bool hazardRespawnFacingRight;
    public string respawnMarkerName;
    public bool isInvincible;

    public int currentArea;

    public int maxMP; //最大MP容量
    public int MPCharge; //当前MP存储量
    public int MPReverse; //花费的MP
    public int MPReverseMax; //花费的最大MP
    public bool soulLimited; //是否灵魂容量被限制了
    public int focusMP_amount; //聚集法术所需要的MP

    public bool hasDash;
    public bool canDash;
    public bool hasBackDash;
    public bool canBackDash;

    public bool hasSpell; //是否有法术
    public int fireballLevel;//火球法术等级，0表示没有
    public int quakeLevel;//地震法术等级，0表示没有
    public int screamLevel;//狂啸法术等级，0表示没有

    public bool hasLantern;
    public bool hasDreamNail;

    public int charmsOwned;
    public bool overcharmed;

    public bool gotCharm_6; //复仇之魂
    public bool equippedCharm_6;

    public bool gotCharm_7; //快速聚集
    public bool equippedCharm_7;

    public bool gotCharm_10; //英勇者勋章
    public bool equippedCharm_10;

    public bool gotCharm_11; //吸虫之巢
    public bool equippedCharm_11;

    public bool gotCharm_17; 
    public bool equippedCharm_17;

    public bool gotCharm_19; //萨满之石
    public bool equippedCharm_19;

    public bool gotCharm_28; //乌恩之形
    public bool equippedCharm_28;

    public bool gotCharm_31; //冲刺大师
    public bool equippedCharm_31;

    public bool gotCharm_34; //深度聚集
    public bool equippedCharm_34;

    public bool giantFlyDefeated;

    public bool falseKnightWallBroken;
    public bool falseKnightWallRepaired;
    public bool shamanPillar;

    public bool falseKnightDefeated;
    public bool falseKnightFirstPlop;
    public bool killedFalseKnight;
    public bool newDataFalseKnight;
    public int killsFalseKnight;
    public bool corn_crossroadsLeft;
    public bool openedMapperShop;

    public bool crossroadsMawlekWall;

    public int CurrentMaxHealth
    {
	get
	{
	    return maxHealth;
	}
    }


    protected PlayerData()
    {
	SetupNewPlayerData();
    }

    public void Reset()
    {
	SetupNewPlayerData();
    }

    private void SetupNewPlayerData()
    {
	disablePause = false;

	currentArea = 0;

	health = 5;
	maxHealth = 5;
	maxHealthBase = 5;
	prevHealth = health;
	nailDamage = 5;

	maxMP = 99;
	MPCharge = 0;
	MPReverse = 0;
	MPReverseMax = 0;
	soulLimited = false;
	focusMP_amount = 33;

	respawnScene = "Tutorial_01";
	respawnMarkerName = "Death Respawn Marker";
	hazardRespawnLocation = Vector3.zero;
	hazardRespawnFacingRight = false;
	isInvincible = false;

	hasDash = true; //测试阶段先设置为true方便测试
	canDash = true;
	hasBackDash = false;
	canBackDash = false;

	hasSpell = false;
	fireballLevel = 0;
	quakeLevel = 0;
	screamLevel = 0;

	hasLantern = false;
	hasDreamNail = false;

	charmsOwned = 0;
	overcharmed = false;
	gotCharm_6 = false;
	equippedCharm_6 = false;
	gotCharm_7 = false;
	equippedCharm_7 = false;
	gotCharm_10 = false;
	equippedCharm_10 = false;
	gotCharm_11 = false;
	equippedCharm_11 = false;
	gotCharm_17 = false;
	equippedCharm_17 = false;
	gotCharm_19 = false;
	equippedCharm_19 = false;
	gotCharm_28 = false;
	equippedCharm_28 = false;
	gotCharm_31 = true;
	equippedCharm_31 = true;
	gotCharm_34 = false;
	equippedCharm_34 = false;

	giantFlyDefeated = false;


	falseKnightFirstPlop = false;
	falseKnightDefeated = false;
	killedFalseKnight = false;
	newDataFalseKnight = false;
	killsFalseKnight = 0;

	corn_crossroadsLeft = false;

	openedMapperShop = false;

	crossroadsMawlekWall = false;
    }

    public void SetHazardRespawn(HazardRespawnMarker location)
    {
	hazardRespawnLocation = location.transform.position;
	hazardRespawnFacingRight = location.respawnFacingRight;
	Debug.LogFormat("hazardRespawnLocation =" + hazardRespawnLocation);
	Debug.LogFormat("hazardRespawnFacingRight =" + hazardRespawnFacingRight);
    }
    public void SetHazardRespawn(Vector3 position, bool facingRight)
    {
	hazardRespawnLocation = position;
	hazardRespawnFacingRight = facingRight;
    }

    public void AddHealth(int amount)
    {
	if (health + amount >= maxHealth)
	{
	    health = maxHealth;
	}
	else
	{
	    health += amount;
	}
	if (health >= CurrentMaxHealth)
	{
	    health = maxHealth;
	}
    }

    public void TakeHealth(int amount)
    {
	if(amount > 0 && health == maxHealth && health != CurrentMaxHealth)
	{
	    health = CurrentMaxHealth;
	}
	if(health - amount < 0)
	{
	    health = 0;
	    return;
	}
	health -= amount;
    }

    public void MaxHealth()
    {
	health = CurrentMaxHealth;
    }

    public bool AddMPCharge(int amount)
    {
	bool result = false;
	if(soulLimited && maxMP != 66)
	{
	    maxMP = 66;
	}
	if (!soulLimited && maxMP != 99)
	{
	    maxMP = 99;
	}
	if(MPCharge + amount > maxMP)
	{
	    if(MPReverse < MPReverseMax)
	    {
		MPReverse += amount - (maxMP - MPCharge);
		result = true;
		if(MPReverse > MPReverseMax)
		{
		    MPReverse = MPReverseMax;
		}
	    }
	    MPCharge = maxMP;
	}
	else
	{
	    MPCharge += amount;
	    result = true;
	}
	return result;
    }

    public void TakeMP(int amount)
    {
	if(amount < MPCharge)
	{
	    MPCharge -= amount;
	    if(MPCharge < 0)
	    {
		MPCharge = 0;
		return;
	    }
	}
	else
	{
	    MPCharge = 0;
	}
    }

    public int GetInt(string intName)
    {
	if (string.IsNullOrEmpty(intName))
	{
	    Debug.LogError("PlayerData: Int with an EMPTY name requested.");
	    return -9999;
	}
	FieldInfo fieldInfo = GetType().GetField(intName);
	if(fieldInfo != null)
	{
	    return (int)fieldInfo.GetValue(instance);
	}
	Debug.LogError("PlayerData: Could not find int named " + intName + " in PlayerData");
	return -9999;
    }

    public void IncrementInt(string intName)
    {
	FieldInfo field = GetType().GetField(intName);
	if (field != null)
	{
	    int num = (int)field.GetValue(instance);
	    field.SetValue(instance, num + 1);
	    return;
	}
	Debug.Log("PlayerData: Could not find field named " + intName + ", check variable name exists and FSM variable string is correct.");
    }

    public void SetInt(string intName, int value)
    {
	FieldInfo field = GetType().GetField(intName);
	if (field != null)
	{
	    field.SetValue(instance, value);
	    return;
	}
	Debug.Log("PlayerData: Could not find field named " + intName + ", check variable name exists and FSM variable string is correct.");
    }

    public bool GetBool(string boolName)
    {
	if (string.IsNullOrEmpty(boolName))
	{
	    return false;
	}
	FieldInfo field = GetType().GetField(boolName);
	if (field != null)
	{
	    return (bool)field.GetValue(instance);
	}
	Debug.Log("PlayerData: Could not find bool named " + boolName + " in PlayerData");
	return false;
    }

    public void SetBool(string boolName, bool value)
    {
	FieldInfo field = GetType().GetField(boolName);
	if (field != null)
	{
	    field.SetValue(instance, value);
	    return;
	}
	Debug.Log("PlayerData: Could not find field named " + boolName + ", check variable name exists and FSM variable string is correct.");
    }

}
