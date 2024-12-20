using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaTitleController : MonoBehaviour
{
    private List<Area> areaList;
    public bool waitForHeroInPosition;

    [Header("Values copied from FSM")]
    public string areaEvent;
    public bool displayRight;
    public string doorTrigger;
    public bool onlyOnRevisit;
    public float unvisitedPause;
    public float visitedPause;
    public bool waitForTrigger;

    [Space]
    public GameObject areaTitle;
    private Area area;
    private bool played;
    private bool doFinish;
    private HeroController hc;
    private HeroController.HeroInPosition heroInPositionResponder;
    public AreaTitleController()
    {
	List<Area> list = new List<Area>();
	list.Add(new Area("ABYSS", 13, false, "visitedAbyss"));
	list.Add(new Area("CROSSROADS", 2, false, "visitedCrossroads", delegate (Area self, AreaTitleController sender)
	{
	    if (GameManager.instance.playerData.GetBool("crossroadsInfected"))
	    {
		self.identifier = "CROSSROADS_INF";
		sender.areaEvent = "CROSSROADS_INF";
	    }
	}));
	list.Add(new Area("DEEPNEST", 9, false, "visitedDeepnest"));
	list.Add(new Area("DIRTMOUTH", 1, false, "visitedDirtmouth"));
	list.Add(new Area("EGGTEMPLE", 0, true, ""));
	list.Add(new Area("FOG_CANYON", 4, false, "visitedFogCanyon"));
	list.Add(new Area("FUNGUS", 5, false, "visitedFungus"));
	list.Add(new Area("GREENPATH", 3, false, "visitedGreenpath"));
	list.Add(new Area("HIVE", 11, false, "visitedHive", delegate (Area self, AreaTitleController sender)
	{
	    sender.doFinish = false;
	    if (GameManager.instance.playerData.GetBool("visitedHive"))
	    {
		sender.StartCoroutine(sender.VisitPause(false));
		return;
	    }
	    sender.StartCoroutine(sender.UnvisitPause(false));
	}));
	list.Add(new Area("KINGSPASS", 0, true, "", delegate (Area self, AreaTitleController sender)
	{
	    if (!GameManager.instance.playerData.GetBool("visitedCrossroads"))
	    {
		sender.doFinish = false;
		sender.gameObject.SetActive(false);
	    }
	}));
	list.Add(new Area("MINES", 8, false, "visitedMines"));
	list.Add(new Area("RESTING_GROUNDS", 12, false, "visitedRestingGrounds"));
	list.Add(new Area("ROYAL_GARDENS", 10, false, "visitedRoyalGardens"));
	list.Add(new Area("RUINS", 6, false, "visitedRuins"));
	list.Add(new Area("SHAMANTEMPLE", 0, true, ""));
	list.Add(new Area("WATERWAYS", 7, false, "visitedWaterways"));
	list.Add(new Area("MANTIS_VILLAGE", 0, true, ""));
	list.Add(new Area("FUNGUS_CORE", 0, true, ""));
	list.Add(new Area("MAGE_TOWER", 0, true, ""));
	list.Add(new Area("FUNGUS_SHAMAN", 0, true, ""));
	list.Add(new Area("QUEENS_STATION", 0, true, ""));
	list.Add(new Area("KINGS_STATION", 0, true, ""));
	list.Add(new Area("BLUE_LAKE", 0, true, ""));
	list.Add(new Area("ACID_LAKE", 0, true, ""));
	list.Add(new Area("OUTSKIRTS", 14, false, "visitedOutskirts"));
	list.Add(new Area("LOVE_TOWER", 0, true, ""));
	list.Add(new Area("SPIDER_VILLAGE", 0, true, ""));
	list.Add(new Area("HEGEMOL_NEST", 0, true, ""));
	list.Add(new Area("WHITE_PALACE", 15, false, "visitedWhitePalace"));
	list.Add(new Area("COLOSSEUM", 0, true, "seenColosseumTitle", delegate (Area self, AreaTitleController sender)
	{
	    sender.doFinish = false;
	    if (GameManager.instance.playerData.GetBool("seenColosseumTitle"))
	    {
		sender.StartCoroutine(sender.VisitPause(true));
		return;
	    }
	    sender.StartCoroutine(sender.UnvisitPause(true));
	}));
	list.Add(new Area("ABYSS_DEEP", 16, false, "visitedAbyssLower"));
	list.Add(new Area("CLIFFS", 17, false, "visitedCliffs"));
	list.Add(new Area("GODHOME", 18, false, "visitedGodhome"));
	list.Add(new Area("GODSEEKER_WASTE", 0, true, ""));
	areaList = list;
	waitForHeroInPosition = true;
	areaEvent = "";
	doorTrigger = "";
	unvisitedPause = 2f;
	visitedPause = 2f;
	doFinish = true;
    }
    private void Start()
    {
	PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(gameObject, "Area Title Controller");
	if (playMakerFSM)
	{
	    areaEvent = FSMUtility.GetString(playMakerFSM, "Area Event");
	    displayRight = FSMUtility.GetBool(playMakerFSM, "Display Right");
	    doorTrigger = FSMUtility.GetString(playMakerFSM, "Door Trigger");
	    onlyOnRevisit = FSMUtility.GetBool(playMakerFSM, "Only On Revisit");
	    unvisitedPause = FSMUtility.GetFloat(playMakerFSM, "Unvisited Pause");
	    visitedPause = FSMUtility.GetFloat(playMakerFSM, "Visited Pause");
	    waitForTrigger = FSMUtility.GetBool(playMakerFSM, "Wait for Trigger");
	}
	else
	{
	    Debug.LogError("No FSM attached to " + gameObject.name + " to get data from!");
	}
	if (waitForHeroInPosition)
	{
	    hc = HeroController.instance;
	    if(hc != null)
	    {
		FindAreaTitle();
		heroInPositionResponder = delegate (bool v0)
		{
		    FindAreaTitle();
		    DoPlay();
		    hc.heroInPosition -= heroInPositionResponder;
		    heroInPositionResponder = null;
		};
		hc.heroInPosition += heroInPositionResponder;
		return;
	    }
	}
	else
	{
	    FindAreaTitle();
	    DoPlay();
	}
    }

    protected void OnDestroy()
    {
	if (hc != null && heroInPositionResponder != null)
	{
	    hc.heroInPosition -= heroInPositionResponder;
	    hc = null;
	    heroInPositionResponder = null;
	}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if (played)
	{
	    return;
	}
	if (collision.tag == "Player")
	{
	    Play();
	}
    }

    private void FindAreaTitle()
    {
	if (AreaTitle.instance)
	{
	    areaTitle = AreaTitle.instance.gameObject;
	    Debug.LogFormat("areaTitle = " + areaTitle);
	}
    }

    private void DoPlay()
    {
	if (!waitForTrigger)
	{
	    Play();
	}
    }

    private void Play()
    {
	if (played) 
	{ 
	    return; 
	}
	played = true;
	if(doorTrigger == "")
	{
	    CheckArea();
	    return;
	}
	if (HeroController.instance.GetEntryGateName() == doorTrigger)
	{
	    CheckArea();
	    return;
	}
	gameObject.SetActive(false);
    }

    private void CheckArea()
    {
	area = areaList.FirstOrDefault((Area o) => o.identifier == areaEvent);
	if(area != null)
	{
	    if(area.evaluateDelegate != null)
	    {
		area.evaluateDelegate(area, this);
	    }
	}
	else
	{
	    Debug.LogWarning("No area with identifier \"" + areaEvent + "\" found in area list. Creating default SubArea.");
	    area = new Area(areaEvent, 0, true, "");
	}
	if (doFinish)
	{
	    Finish();
	}
    }

    private void Finish()
    {
	Debug.LogFormat("Finish()");
	if (area.subArea)
	{
	    StartCoroutine(VisitPause(true));
	    return;
	}
	int current = GameManager.instance.playerData.GetInt("currentArea");
	bool visited = GameManager.instance.playerData.GetBool(area.visitedBool);
	bool flag = true;
	if ((!visited && onlyOnRevisit) || area.areaID == current)
	{
	    flag = false;
	    gameObject.SetActive(false);
	}
	else
	{
	    GameManager.instance.playerData.SetInt("currentArea", area.areaID);
	}
	if (flag)
	{
	    StartCoroutine(visited ? VisitPause(true) : UnvisitPause(true));
	}
    }

    private IEnumerator VisitPause(bool pause = true)
    {
	if (pause)
	{
	    yield return new WaitForSeconds(visitedPause);
	}
	GameManager.instance.StoryRecord_travelledToArea(area.identifier);
	if (areaTitle)
	{
	    areaTitle.SetActive(true);
	    PlayMakerFSM fsm = FSMUtility.GetFSM(areaTitle);
	    if (fsm)
	    {
		FSMUtility.SetBool(fsm, "Visited", true);
		FSMUtility.SetBool(fsm, "NPC Title", false);
		FSMUtility.SetBool(fsm, "Display Right", displayRight);
		FSMUtility.SetString(fsm, "Area Event", areaEvent);
	    }
	}
    }

    private IEnumerator UnvisitPause(bool pause = true)
    {
	Debug.LogFormat("UnvisitPause");
	if (pause)
	{
	    yield return new WaitForSeconds(unvisitedPause);
	}
	Debug.LogFormat("StoryRecord_discoveredArea");
	GameManager.instance.StoryRecord_discoveredArea(area.identifier);
	if (areaTitle)
	{
	    areaTitle.SetActive(true);
	    PlayMakerFSM fsm = FSMUtility.GetFSM(areaTitle);
	    if (fsm)
	    {
		FSMUtility.SetBool(fsm, "Visited", false);
		FSMUtility.SetBool(fsm, "NPC Title", false);
		FSMUtility.SetString(fsm, "Area Event", areaEvent);
		GameManager.instance.playerData.SetBool(area.visitedBool, true);
	    }
	}
    }

    public class Area
    {
        public string identifier = "AREA";
        public int areaID;
        public bool subArea;
        public string visitedBool = "";
        public Action<Area, AreaTitleController> evaluateDelegate;

	public Area(string identifier, int areaID, bool subArea, string visitedBool)
	{
	    this.identifier = identifier;
	    this.areaID = areaID;
	    this.subArea = subArea;
	    this.visitedBool = visitedBool;
	}

	public Area(string identifier, int areaID, bool subArea, string visitedBool, Action<Area, AreaTitleController> evaluateDelegate)
	{
	    this.identifier = identifier;
	    this.areaID = areaID;
	    this.subArea = subArea;
	    this.visitedBool = visitedBool;
	    this.evaluateDelegate = evaluateDelegate;
	}
    }
}
