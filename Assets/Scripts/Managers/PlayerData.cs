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

    public string version;
    public int profileID;
    public bool isFirstGame;
    public bool backerCredits;
    public bool bossRushMode;
    public int permadeathMode;
    public bool enteredTutorialFirstTime;
    public bool unlockedCompletionRate;

    public bool travelling;
    public string nextScene;

    public int geo;
    public int geoPool;
    public bool firstGeo;

    public int health;
    public int maxHealth;
    public int maxHealthBase;
    public int prevHealth;

    public int healthBlue;
    public int joniHealthBlue;
    public bool damagedBlue;

    public int nailDamage;

    public MapZone mapZone;

    public List<string> scenesEncounteredBench;

    public bool hasQuill;
    public bool hasMap;
    public bool mapAllRooms;
    public bool atMapPrompt;
    public bool mapDirtmouth;
    public bool mapCrossroads;
    public bool mapGreenpath;
    public bool mapFogCanyon;
    public bool mapRoyalGardens;
    public bool mapFungalWastes;
    public bool mapCity;
    public bool mapWaterways;
    public bool mapMines;
    public bool mapDeepnest;
    public bool mapCliffs;
    public bool mapOutskirts;
    public bool mapRestingGrounds;
    public bool mapAbyss;

    public bool atBench;
    public bool charmBenchMsg;

    public string respawnMarkerName;
    public string respawnScene;
    public int respawnType;
    public bool respawnFacingRight;

    [NonSerialized]
    public Vector3 hazardRespawnLocation;
    public bool hazardRespawnFacingRight;
    public bool isInvincible;

    public string shadeScene;
    public string shadeMapZone;
    public float shadePositionX;
    public float shadePositionY;
    public int shadeHealth;
    public int shadeMP;
    public int shadeFireballLevel;
    public int shadeQuakeLevel;
    public int shadeScreamLevel;
    public int shadeSpecialType;

    public int currentArea;
    public int environmentType;
    public int environmentTypeDefault;

    public bool seenFocusTablet; //是否看到了提示Focus回血的石碑
    public bool promptFocus; //是否是第一次按Focus回血按键之前
    public int maxMP; //最大MP容量
    public int MPCharge; //MP消耗量
    public int MPReserve; //保存的MP
    public int MPReserveMax; //保存的最大MP
    public bool soulLimited; //是否灵魂容量被限制了(意思是玩家死了后容量会变少一个)
    public int focusMP_amount; //聚集法术所需要的MP

    public bool hasDash;
    public bool canDash;
    public bool hasBackDash;
    public bool canBackDash;

    public bool hasSpell; //是否有法术
    public int fireballLevel;//火球法术等级，0表示没有
    public int quakeLevel;//地震法术等级，0表示没有
    public int screamLevel;//狂啸法术等级，0表示没有

    public bool hasWalljump;
    public bool canWallJump;
    public bool corn_fungalWastesLeft;

    public bool hasLantern;
    public bool hasDreamNail;
    public bool hasAcidArmour;

    public bool foundTrinket2;
    public int trinket2;

    public int charmSlotsFilled;
    public int charmSlots;

    public bool hasCharm;
    public int charmsOwned;
    public bool overcharmed;

    public bool gotCharm_1; //采集虫群
    public bool equippedCharm_1;

    public bool gotCharm_6; //复仇之魂
    public bool equippedCharm_6;

    public bool gotCharm_7; //快速聚集
    public bool equippedCharm_7;

    public bool gotCharm_10; //英勇者勋章
    public bool equippedCharm_10;

    public bool gotCharm_11; //吸虫之巢
    public bool equippedCharm_11;

    public bool gotCharm_15; //沉重之击
    public bool equippedCharm_15;

    public bool gotCharm_17; 
    public bool equippedCharm_17;

    public bool gotCharm_18;
    public bool equippedCharm_18;

    public bool gotCharm_19; //萨满之石
    public bool equippedCharm_19;

    public bool gotCharm_23; //易碎生命
    public bool equippedCharm_23;
    public bool brokenCharm_23; //易碎生命破碎
    public int charmCost_23;

    public bool gotCharm_24; //易碎贪婪
    public bool equippedCharm_24;
    public bool brokenCharm_24; //易碎贪婪破碎
    public int charmCost_24;

    public bool gotCharm_25; //易碎力量
    public bool equippedCharm_25;
    public bool brokenCharm_25; //易碎力量破碎
    public int charmCost_25;

    public bool gotCharm_27; //乔尼的祝福
    public bool equippedCharm_27;

    public bool gotCharm_28; //乌恩之形
    public bool equippedCharm_28;

    public bool gotCharm_29; //蜂巢之血
    public bool equippedCharm_29;

    public bool gotCharm_31; //冲刺大师
    public bool equippedCharm_31;

    public bool gotCharm_34; //深度聚集
    public bool equippedCharm_34;

    public bool fragileHealth_unbreakable;
    public bool fragileGreed_unbreakable;
    public bool fragileStrength_unbreakable;

    public List<string> scenesVisited;
    public bool visitedDirtmouth;
    public bool visitedCrossroads;

    public bool stagConvoTram;
    public bool stagConvoTiso;
    public bool stagRemember1;
    public bool stagRemember2;
    public bool stagRemember3;
    public bool stagEggInspected;
    public bool stagHopeConvo;
    public int stagPosition;
    public int stationsOpened;
    public bool openedTown;
    public bool openedTownBuilding;
    public bool openedCrossroads;
    public bool openedGreenpath;
    public bool openedRuins1;
    public bool openedRuins2;
    public bool openedFungalWastes;
    public bool openedRoyalGardens;
    public bool openedRestingGrounds;
    public bool openedDeepnest;
    public bool openedStagNest;
    public bool openedHiddenStation;

    public bool metStag;


    public bool giantFlyDefeated;

    public bool falseKnightWallBroken;
    public bool falseKnightWallRepaired;

    public int shaman;
    public bool shamanScreamConvo;
    public bool shamanQuakeConvo;
    public bool shamanFireball2Convo;
    public bool shamanScream2Convo;
    public bool shamanQuake2Convo;
    public bool shamanPillar;

    public bool falseKnightDefeated;
    public bool falseKnightFirstPlop;
    public bool killedFalseKnight;
    public bool newDataFalseKnight;
    public int killsFalseKnight;
    public bool corn_crossroadsLeft;
    public bool openedMapperShop;

    public bool crossroadsMawlekWall;

    public bool killedBigBuzzer;

    public int zote;
    public bool zoteDead;
    public bool zoteRescuedBuzzer;
    public int zoteDeathPos;

    public int hornetGreenpath;
    public bool hornet_f19;
    public bool hornet1Defeated;
    public bool hornetCityBridge_ready;
    public bool hornetCityBridge_completed;
    public int hornetFung;

    public bool megaMossChargerEncountered;
    public bool megaMossChargerDefeated;

    public bool defeatedMantisLords;
    public bool killedMantisLord;
    public bool newDataMantisLord;
    public int killsMantisLord;

    public bool dreamerScene1;

    public int royalCharmState;

    public bool metHunter;
    public bool hasJournal;
    public bool hunterRoared;
    public int journalEntriesCompleted;
    public int journalNotesCompleted;
    public int journalEntriesTotal;
    public bool hasHuntersMark; //猎人印记

    public bool metCloth;

    public bool kingsStationNonDisplay;
    public bool queensStationNonDisplay;

    public bool hasXunFlower;
    public bool xunFlowerBroken;
    public int xunFlowerBrokeTimes;

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
	version = "1.5.78.11833";

	disablePause = false;
	isFirstGame = true;
	backerCredits = false;
	bossRushMode = false;
	permadeathMode = 0;
	enteredTutorialFirstTime = false;
	unlockedCompletionRate = false;
	travelling = false;
	nextScene = "";

	currentArea = 0;
	mapZone = MapZone.KINGS_PASS;
	scenesEncounteredBench = new List<string>();

	hasQuill = false;
	hasMap = false;

	environmentType = 0;
	environmentTypeDefault = 0;

	geo = 0;
	geoPool = 0;
	firstGeo = false;

	health = 5;
	maxHealth = 5;
	maxHealthBase = 5;
	prevHealth = health;

	healthBlue = 0;
	joniHealthBlue = 0;
	damagedBlue = false;

	nailDamage = 5;

	seenFocusTablet = false;
	promptFocus = false;
	maxMP = 99;
	MPCharge = 0;
	MPReserve = 0;
	MPReserveMax = 0;
	soulLimited = false;
	focusMP_amount = 33;

	atBench = false;
	charmBenchMsg = false;
	respawnScene = "Tutorial_01";
	respawnMarkerName = "Death Respawn Marker";
	hazardRespawnLocation = Vector3.zero;
	hazardRespawnFacingRight = false;
	isInvincible = false;

	shadeScene = "None";
	shadeMapZone = "";
	shadePositionX = -999f;
	shadePositionY = -999f;
	shadeHealth = 0;
	shadeMP = 0;
	shadeFireballLevel = 2;
	shadeQuakeLevel = 2;
	shadeScreamLevel = 2;
	shadeSpecialType = 0;

	hasDash = false;
	canDash = true;
	hasBackDash = false;
	canBackDash = false;

	hasSpell = false;
	fireballLevel = 0;
	quakeLevel = 0;
	screamLevel = 0;

	hasWalljump = false;

	hasLantern = false;
	hasDreamNail = false;
	hasAcidArmour = false;

	foundTrinket2 = false;
	trinket2 = 0;

	charmSlots = 3;
	charmSlotsFilled = 0;
	hasCharm = false;
	charmsOwned = 0;
	overcharmed = false;
	gotCharm_1 = false;
	equippedCharm_1 = false;
	gotCharm_6 = false;
	equippedCharm_6 = false;
	gotCharm_7 = false;
	equippedCharm_7 = false;
	gotCharm_10 = false;
	equippedCharm_10 = false;
	gotCharm_11 = false;
	equippedCharm_11 = false;
	gotCharm_15 = false;
	equippedCharm_15 = false;

	gotCharm_17 = false;
	equippedCharm_17 = false;
	gotCharm_18 = false;
	equippedCharm_18 = false;
	gotCharm_19 = false;
	equippedCharm_19 = false;

	gotCharm_23 = false;
	equippedCharm_23 = false;
	brokenCharm_23 = false;
	charmCost_23 = 2; 
	gotCharm_24 = false;
	equippedCharm_24 = false;
	brokenCharm_24 = false;
	charmCost_24 = 2;

	gotCharm_25 = false;
	equippedCharm_25 = false;
	brokenCharm_25 = false;
	charmCost_25 = 3;

	gotCharm_27 = false;
	equippedCharm_27 = false;
	gotCharm_28 = false;
	equippedCharm_28 = false;
	gotCharm_29 = false;
	equippedCharm_29 = false;
	gotCharm_31 = true;
	equippedCharm_31 = true;
	gotCharm_34 = false;
	equippedCharm_34 = false;

	fragileHealth_unbreakable = false;
	fragileGreed_unbreakable = false;
	fragileStrength_unbreakable = false;

	scenesVisited = new List<string>();
	visitedDirtmouth = false;
	visitedCrossroads = false;

	stagConvoTram = false;
	stagConvoTiso = false;
	stagRemember1 = false;
	stagRemember2 = false;
	stagRemember3 = false;
	stagEggInspected = false;
	stagHopeConvo = false;
	metStag = false;
	stagPosition = -1;
	stationsOpened = 0;
	openedTown = false;
	openedTownBuilding = false;
	openedCrossroads = false;
	openedGreenpath = false;
	openedRuins1 = false;
	openedRuins2 = false;
	openedFungalWastes = false;
	openedRoyalGardens = false;
	openedRestingGrounds = false;
	openedDeepnest = false;
	openedStagNest = false;
	openedHiddenStation = false;

	hasMap = false;
	mapAllRooms = false;
	atMapPrompt = false;
	mapDirtmouth = true;
	mapCrossroads = false;
	mapGreenpath = false;
	mapFogCanyon = false;
	mapRoyalGardens = false;
	mapFungalWastes = false;
	mapCity = false;
	mapWaterways = false;
	mapMines = false;
	mapDeepnest = false;
	mapCliffs = false;
	mapOutskirts = false;
	mapRestingGrounds = false;
	mapAbyss = false;

	giantFlyDefeated = false;

	shaman = 0;
	shamanFireball2Convo = false;
	shamanQuakeConvo = false;
	shamanQuake2Convo = false;
	shamanScreamConvo = false;
	shamanScream2Convo = false;

	falseKnightFirstPlop = false;
	falseKnightDefeated = false;
	killedFalseKnight = false;
	newDataFalseKnight = false;
	killsFalseKnight = 0;

	corn_crossroadsLeft = false;

	openedMapperShop = false;

	crossroadsMawlekWall = false;

	killedBigBuzzer = false;

	zote = 0;
	zoteDead = false;
	zoteDeathPos = 0;
	zoteRescuedBuzzer = false;

	hornetGreenpath = 0;
	hornet_f19 = false;
	hornet1Defeated = false;
	hornetCityBridge_ready = false;
	hornetCityBridge_completed = false;
	hornetFung = 0;

	defeatedMantisLords = false;
	killedMantisLord = false;
	newDataMantisLord = false;
	killsMantisLord = 0;

	megaMossChargerEncountered = false;
	megaMossChargerDefeated = false;

	dreamerScene1 = false;

	royalCharmState = 0;

	metHunter = false;
	hasJournal = false;
	hunterRoared = false;
	journalEntriesCompleted = 0;
	journalNotesCompleted = 0;
	journalEntriesTotal = 146;
	hasHuntersMark = false;

	metCloth = false;

	kingsStationNonDisplay = false;
	queensStationNonDisplay = false;

	hasXunFlower = false;
	xunFlowerBroken = false;
	xunFlowerBrokeTimes = 0;

    }

    public void CountGameCompletion()
    {
	//TODO:CountGameCompletion
    }

    public void SetBenchRespawn(RespawnMarker spawnMarker, string sceneName, int spawnType)
    {
	Debug.LogFormat("SetBenchRespawn 1");
	respawnMarkerName = spawnMarker.name;
	respawnScene = sceneName;
	respawnType = spawnType;
	respawnFacingRight = spawnMarker.respawnFacingRight;
	GameManager.instance.SetCurrentMapZoneAsRespawn();
    }
    public void SetBenchRespawn(string spawnMarker, string sceneName, bool facingRight)
    {
	Debug.LogFormat("SetBenchRespawn 2");
	respawnMarkerName = spawnMarker;
	respawnScene = sceneName;
	respawnFacingRight = facingRight;
	GameManager.instance.SetCurrentMapZoneAsRespawn();
    }

    public void SetBenchRespawn(string spawnMarker, string sceneName, int spawnType, bool facingRight)
    {
	Debug.LogFormat("SetBenchRespawn 3");
	respawnMarkerName = spawnMarker;
	respawnScene = sceneName;
	respawnType = spawnType;
	respawnFacingRight = facingRight;
	GameManager.instance.SetCurrentMapZoneAsRespawn();
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
	prevHealth = health;
	health = CurrentMaxHealth;

	UpdateBlueHealth();
    }

    public void UpdateBlueHealth()
    {
	healthBlue = 0;
    }

    /// <summary>
    /// 加入MP到存储量中
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
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
	    if(MPReserve < MPReserveMax)
	    {
		MPReserve += amount - (maxMP - MPCharge);
		result = true;
		if(MPReserve > MPReserveMax)
		{
		    MPReserve = MPReserveMax;
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

    /// <summary>
    /// 带走MP
    /// </summary>
    /// <param name="amount"></param>
    public void TakeMP(int amount)
    {
	if(amount <= MPCharge)
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

    /// <summary>
    /// 带走现有的MP
    /// </summary>
    /// <param name="amount"></param>
    public void TakeReserveMP(int amount)
    {
	MPReserve -= amount;
	if(MPReserve < 0)
	{
	    MPReserve = 0;
	}
    }

    /// <summary>
    /// 清除现有的MP
    /// </summary>
    public void ClearMP()
    {
	MPCharge = 0;
	MPReserve = 0;
    }

    public void StartSoulLimiter()
    {
	soulLimited = true;
	maxMP = 66;
    }

    public void EndSoulLimiter()
    {
	soulLimited = false;
	maxMP = 99;
    }

    public void AddGeo(int amount)
    {
	geo += amount;
	if (geo > 9999999)
	{
	    geo = 9999999;
	}
    }

    public void TakeGeo(int amount)
    {
	geo -= amount;
    }

    public void EquipCharm(int charmNum)
    {
	//TODO:Equip Charm
    }

    public void UnequipCharm(int charmNum)
    {
	//TODO:Unequip Charm
    }

    public void CountJournalEntries()
    {
	journalEntriesCompleted = 0;
	journalNotesCompleted = 0;
	journalEntriesTotal = 146;
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

    public string GetString(string stringName)
    {
	if (string.IsNullOrEmpty(stringName))
	{
	    Debug.LogError("PlayerData: String with an EMPTY name requested.");
	    return " ";
	}
	FieldInfo field = GetType().GetField(stringName);
	if (field != null)
	{
	    return (string)field.GetValue(instance);
	}
	Debug.LogError("PlayerData: Could not find string named " + stringName + " in PlayerData");
	return " ";
    }

    public void SetString(string stringName, string value)
    {
	FieldInfo field = GetType().GetField(stringName);
	if (field != null)
	{
	    field.SetValue(instance, value);
	    return;
	}
	Debug.Log("PlayerData: Could not find field named " + stringName + ", check variable name exists and FSM variable string is correct.");
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

    public void SetFloat(string floatName, float value)
    {
	FieldInfo field = GetType().GetField(floatName);
	if (field != null)
	{
	    field.SetValue(instance, value);
	    return;
	}
	Debug.Log("PlayerData: Could not find field named " + floatName + ", check variable name exists and FSM variable string is correct.");
    }
}
