using System;
using System.Collections;
using GlobalEnums;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class SaveSlotButton : MenuButton,ISelectHandler,IEventSystemHandler,IDeselectHandler,ISubmitHandler,IPointerClickHandler
    {
	private bool verboseMode = true;
	[Header("Slot Number")]
	public SaveSlot saveSlot;

	[Header("Animation")]
	public Animator topFleur;
	public Animator highlight;

	[Header("Canvas Group")]
	public CanvasGroup newGameText;
	public CanvasGroup saveCorruptedText;
	public CanvasGroup loadingText;
	public CanvasGroup activeSaveSlot;
	public CanvasGroup clearSaveButton;
	public CanvasGroup clearSavePrompt;
	public CanvasGroup backgroundCg;
	public CanvasGroup slotNumberText;
	public CanvasGroup myCanvasGroup;
	public CanvasGroup defeatedText;
	public CanvasGroup defeatedBackground;
	public CanvasGroup brokenSteelOrb;

	[Header("Text Elements")]
	public Text geoText;
	public Text locationText;
	public Text playTimeText;
	public Text completionText;

	[Header("Soul Orbs")]
	public CanvasGroup normalSoulOrbCg;
	public CanvasGroup hardcoreSoulOrbCg;
	public CanvasGroup ggSoulOrbCg;

	[Header("Visual Elements")]
	public Image background;
	public Image soulOrbIcon;
	public SaveProfileHealthBar healthSlots;
	public Image geoIcon;

	[Header("Raycast Blocker")]
	public GameObject clearSaveBlocker;

	private GameManager gm;
	private UIManager ui;
	private InputHandler ih;
	private CoroutineQueue coroutineQueue;
	private PreselectOption clearSavePromptHighlight;

	private Navigation noNav;
	private Navigation fullSlotNav;
	private Navigation emptySlotNav;

	private IEnumerator currentLoadingTextFadeIn;
	private bool didLoadSaveStats;

	[SerializeField] public SlotState state { get; private set; }
	public SaveFileStates saveFileState;
	[SerializeField] private SaveStats saveStats;

	private int SaveSlotIndex
	{
	    get
	    {
		switch (saveSlot)
		{
		    case SaveSlot.SLOT_1:
			return 1;
		    case SaveSlot.SLOT_2:
			return 2;
		    case SaveSlot.SLOT_3:
			return 3;
		    case SaveSlot.SLOT_4:
			return 4;
		    default:
			return 0;
		}
	    }
	} //获取当前的SaveSlot的值

	private new void Awake()
	{
	    gm = GameManager.instance;
	    clearSavePromptHighlight = clearSavePrompt.GetComponent<PreselectOption>();
	    coroutineQueue = new CoroutineQueue(gm);
	    SetupNavs();
	}

	private new void OnEnable()
	{
	    if(saveStats != null && saveFileState == SaveFileStates.LoadedStats)
	    {
		PresentSaveSlot(saveStats);
	    }
	}

	private new void Start()
	{
	    if (!Application.isPlaying)
	    {
		return;
	    }
	    ui = UIManager.instance;
	    ih = gm.inputHandler;
	    HookUpAudioPlayer();
	}

	/// <summary>
	/// 设置好每一个不同状态下的导航系统
	/// </summary>
	private void SetupNavs()
	{
	    noNav = new Navigation
	    {
		mode = Navigation.Mode.Explicit,
		selectOnLeft = null,
		selectOnRight = null,
		selectOnUp = navigation.selectOnUp,
		selectOnDown = navigation.selectOnDown
	    };
	    emptySlotNav = new Navigation
	    {
		mode = Navigation.Mode.Explicit,
		selectOnRight = null,
		selectOnUp = navigation.selectOnUp,
		selectOnDown = navigation.selectOnDown
	    };
	    fullSlotNav = new Navigation
	    {
		mode = Navigation.Mode.Explicit,
		selectOnRight = clearSaveButton.GetComponent<ClearSaveButton>(),
		selectOnUp = navigation.selectOnUp,
		selectOnDown = navigation.selectOnDown
	    };
	}

	/// <summary>
	/// 准备阶段
	/// </summary>
	/// <param name="gameManager"></param>
	/// <param name="isReload"></param>
	public void Prepare(GameManager gameManager,bool isReload = false) 
	{
	    if(saveFileState == SaveFileStates.NotStarted || (isReload && saveFileState == SaveFileStates.Corrupted))
	    {
		//TODO:先将SaveFileState更改成空闲的状态，等以后做了可持续化数据系统再来完善这段。
		ChangeSaveFileState(SaveFileStates.Empty);
	    }
	}

	private void ChangeSaveFileState(SaveFileStates nextSaveFileState)
	{
	    saveFileState = nextSaveFileState;
	    if (isActiveAndEnabled)
	    {
		ShowRelevantModeForSaveFileState();
	    }
	}

	private void PresentSaveSlot(SaveStats saveStats)
	{
	    geoIcon.enabled = true;
	    geoText.enabled = true;
	    completionText.enabled = true;
	    if (saveStats.bossRushMode)
	    {

	    }
	    else if (saveStats.permadeathMode == 0)
	    {
		normalSoulOrbCg.alpha = 1f;
		hardcoreSoulOrbCg.alpha = 0f;
		ggSoulOrbCg.alpha = 0f;

		geoText.text = saveStats.geo.ToString();
		if (saveStats.unlockedCompletionRate)
		{
		    completionText.text = saveStats.completionPercentage.ToString() + "%";
		}
		else
		{
		    completionText.text = "";
		}
		playTimeText.text = saveStats.GetPlaytimeHHMM();

	    }
	    else if (saveStats.permadeathMode == 1)
	    {

	    }
	    else if(saveStats.permadeathMode == 2)
	    {
		normalSoulOrbCg.alpha = 0f;
		hardcoreSoulOrbCg.alpha = 0f;
		ggSoulOrbCg.alpha = 0f;
	    }
	    locationText.text = "KING'S PASS";
	}

	/// <summary>
	/// 动画化的切换saveslot状态
	/// </summary>
	/// <param name="nextState"></param>
	/// <returns></returns>
	private IEnumerator AnimateToSlotState(SlotState nextState)
	{
	    SlotState state = this.state;
	    if(state == nextState)
	    {
		yield break;
	    }
	    if(currentLoadingTextFadeIn != null)
	    {
		StartCoroutine(currentLoadingTextFadeIn);
		currentLoadingTextFadeIn = null;
	    }
	    if (verboseMode)
	    {
		Debug.LogFormat("{0} SetState: {1} -> {2}", new object[]
		{
		    name,
		    this.state,
		    nextState
		});
	    }
	    this.state = nextState;
	    switch (nextState)
	    {
		case SlotState.HIDDEN:
		case SlotState.OPERATION_IN_PROGRESS:
		    navigation = noNav;
		    break;
		case SlotState.EMPTY_SLOT:
		    navigation = emptySlotNav;
		    break;
		case SlotState.SAVE_PRESENT:
		case SlotState.CORRUPTED:
		case SlotState.CLEAR_PROMPT:
		    navigation = fullSlotNav;
		    break;
	    }
	    //如果当前状态是隐藏
	    if(state == SlotState.HIDDEN)
	    {
		if(nextState == SlotState.OPERATION_IN_PROGRESS)
		{
		    topFleur.ResetTrigger("hide");
		    topFleur.SetTrigger("show");
		    yield return new WaitForSeconds(0.2f);
		    StartCoroutine(currentLoadingTextFadeIn = FadeInCanvasGroupAfterDelay(5f, loadingText));
		}
		else if(nextState == SlotState.EMPTY_SLOT)
		{
		    topFleur.ResetTrigger("hide");
		    topFleur.SetTrigger("show");
		    yield return new WaitForSeconds(0.2f);
		    StartCoroutine(ui.FadeInCanvasGroup(slotNumberText)); //最后的alpha为1f
		    StartCoroutine(ui.FadeInCanvasGroup(newGameText));
		}
		else if(nextState == SlotState.SAVE_PRESENT)
		{
		    topFleur.ResetTrigger("hide");
		    topFleur.SetTrigger("show");
		    yield return new WaitForSeconds(0.2f);
		    StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
		    StartCoroutine(ui.FadeInCanvasGroup(backgroundCg));
		    StartCoroutine(ui.FadeInCanvasGroup(activeSaveSlot));
		    StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = true;
		}
		else if(nextState == SlotState.DEFEATED)
		{
		    topFleur.ResetTrigger("hide");
		    topFleur.SetTrigger("show");
		    yield return new WaitForSeconds(0.2f);
		    StartCoroutine(ui.FadeInCanvasGroup(defeatedBackground));
		    StartCoroutine(ui.FadeInCanvasGroup(defeatedText));
		    StartCoroutine(ui.FadeInCanvasGroup(brokenSteelOrb));
		    StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = true;
		    myCanvasGroup.blocksRaycasts = true;
		}
		else if(nextState == SlotState.CORRUPTED)
		{
		    topFleur.ResetTrigger("hide");
		    topFleur.SetTrigger("show");
		    yield return new WaitForSeconds(0.2f);
		    StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
		    StartCoroutine(ui.FadeInCanvasGroup(saveCorruptedText));
		    StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = true;
		    myCanvasGroup.blocksRaycasts = true;
		}
	    }
	    //如果当前状态是如果正在执行操作
	    else if(state == SlotState.OPERATION_IN_PROGRESS)
	    {
		if(nextState == SlotState.EMPTY_SLOT)
		{
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(loadingText));
		    StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
		    StartCoroutine(ui.FadeInCanvasGroup(newGameText));
		}
		else if(nextState == SlotState.SAVE_PRESENT)
		{
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(loadingText));
		    //TODO:
		    StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
		    StartCoroutine(ui.FadeInCanvasGroup(backgroundCg)); 
		    StartCoroutine(ui.FadeInCanvasGroup(activeSaveSlot));
		    StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = true;
		}
		else if(nextState == SlotState.DEFEATED)
		{
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(loadingText));
		    StartCoroutine(ui.FadeInCanvasGroup(defeatedBackground));
		    StartCoroutine(ui.FadeInCanvasGroup(defeatedText));
		    StartCoroutine(ui.FadeInCanvasGroup(brokenSteelOrb));
		    StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = true;
		    myCanvasGroup.blocksRaycasts = true;
		}
		else if(nextState == SlotState.CORRUPTED)
		{
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(loadingText));
		    StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
		    StartCoroutine(ui.FadeInCanvasGroup(saveCorruptedText));
		    StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = true;
		    myCanvasGroup.blocksRaycasts = true;
		}
	    }
	    //如果当前状态是已经保存了的slot
	    else if(state == SlotState.SAVE_PRESENT)
	    {
		if (nextState == SlotState.CLEAR_PROMPT)
		{
		    ih.StopUIInput();
		    interactable = false;
		    myCanvasGroup.blocksRaycasts = true;
		    StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText)); //从1到0
		    StartCoroutine(ui.FadeOutCanvasGroup(activeSaveSlot));
		    StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = false;
		    clearSaveBlocker.SetActive(true);
		    yield return StartCoroutine(ui.FadeInCanvasGroup(clearSavePrompt)); //从0到1
		    clearSavePrompt.interactable = true;
		    clearSavePrompt.blocksRaycasts = true;
		    clearSavePromptHighlight.HighlightDefault(false);
		    ih.StartUIInput();
		}
		else if(nextState == SlotState.HIDDEN)
		{
		    topFleur.ResetTrigger("show");
		    topFleur.SetTrigger("hide");
		    yield return new WaitForSeconds(0.2f);
		    StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText)); //从1到0
		    StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
		    StartCoroutine(ui.FadeOutCanvasGroup(activeSaveSlot));
		    StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = false;
		}
	    }
	    //如果当前状态是 清除存档确认
	    else if (state == SlotState.CLEAR_PROMPT)
	    {
		if(nextState == SlotState.SAVE_PRESENT) //相当于反悔了回到SAVE_PRESENT状态
		{
		    ih.StopUIInput();
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSavePrompt));
		    clearSaveBlocker.SetActive(false);
		    clearSavePrompt.interactable = false;
		    clearSavePrompt.blocksRaycasts = false;
		    //TODO:
		    StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
		    StartCoroutine(ui.FadeInCanvasGroup(activeSaveSlot));
		    StartCoroutine(ui.FadeInCanvasGroup(backgroundCg));
		    yield return StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = true;
		    interactable = true;
		    myCanvasGroup.blocksRaycasts = true;
		    Select();
		    ih.StartUIInput();
		}
		else if(nextState == SlotState.EMPTY_SLOT) //清除存档了就到EMPTY_SLOT状态了
		{
		    ih.StopUIInput();
		    StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg)); //1->0
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSavePrompt));
		    clearSavePrompt.interactable = false;
		    clearSavePrompt.blocksRaycasts = false;
		    clearSaveBlocker.SetActive(false);
		    StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
		    yield return StartCoroutine(ui.FadeInCanvasGroup(newGameText));
		    myCanvasGroup.blocksRaycasts = true;
		    Select();
		    ih.StartUIInput();
		}
		else if(nextState == SlotState.DEFEATED)
		{
		    ih.StopUIInput();
		    StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg)); //1 -> 0
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSavePrompt));
		    clearSavePrompt.interactable = false;
		    clearSavePrompt.blocksRaycasts = false;
		    clearSaveBlocker.SetActive(false);
		    StartCoroutine(ui.FadeInCanvasGroup(defeatedBackground)); //0 -> 1
		    StartCoroutine(ui.FadeInCanvasGroup(defeatedText));
		    StartCoroutine(ui.FadeInCanvasGroup(brokenSteelOrb));
		    yield return StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = true;
		    myCanvasGroup.blocksRaycasts = true;
		    Select();
		    ih.StartUIInput();
		}
		else if(nextState == SlotState.HIDDEN)
		{
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSavePrompt));
		}
		else if(nextState == SlotState.CORRUPTED)
		{
		    ih.StopUIInput();
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSavePrompt));
		    clearSavePrompt.interactable = false;
		    clearSavePrompt.blocksRaycasts = false;
		    clearSaveBlocker.SetActive(false);
		    StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
		    StartCoroutine(ui.FadeInCanvasGroup(saveCorruptedText));
		    yield return StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = true;
		    myCanvasGroup.blocksRaycasts = true;
		    Select();
		    ih.StartUIInput();
		}
	    }
	    //如果当前状态是空的
	    else if(state == SlotState.EMPTY_SLOT)
	    {
		if(nextState == SlotState.HIDDEN)
		{
		    topFleur.ResetTrigger("show");
		    topFleur.SetTrigger("hide");
		    yield return new WaitForSeconds(0.2f);
		    StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
		    StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
		    StartCoroutine(ui.FadeOutCanvasGroup(newGameText));
		}
	    }
	    //如果当前状态是钢魂档破碎
	    else if (state == SlotState.DEFEATED)
	    {
		if(nextState == SlotState.CLEAR_PROMPT) //进入清除确认状态
		{
		    ih.StopUIInput();
		    interactable = false;
		    myCanvasGroup.blocksRaycasts = false;
		    StartCoroutine(ui.FadeOutCanvasGroup(defeatedBackground));
		    StartCoroutine(ui.FadeOutCanvasGroup(defeatedText));
		    StartCoroutine(ui.FadeOutCanvasGroup(brokenSteelOrb));
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = false;
		    clearSaveBlocker.SetActive(true);
		    yield return StartCoroutine(ui.FadeInCanvasGroup(clearSavePrompt));
		    clearSavePrompt.interactable = true;
		    clearSavePrompt.blocksRaycasts = true;
		    clearSavePromptHighlight.HighlightDefault(false);
		    interactable = false;
		    myCanvasGroup.blocksRaycasts = false;
		    ih.StartUIInput();
		}
		else if(nextState == SlotState.HIDDEN)
		{
		    topFleur.ResetTrigger("show");
		    topFleur.SetTrigger("hide");
		    yield return new WaitForSeconds(0.2f);
		    StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText)); // 1-> 0
		    StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
		    StartCoroutine(ui.FadeOutCanvasGroup(activeSaveSlot));
		    StartCoroutine(ui.FadeOutCanvasGroup(defeatedBackground));
		    StartCoroutine(ui.FadeOutCanvasGroup(defeatedText));
		    StartCoroutine(ui.FadeOutCanvasGroup(brokenSteelOrb));
		    StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = false;
		}
	    }
	    else if(state == SlotState.CORRUPTED)
	    {
		if(nextState == SlotState.CLEAR_PROMPT)
		{
		    ih.StopUIInput();
		    interactable = false;
		    myCanvasGroup.blocksRaycasts = false;
		    StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
		    StartCoroutine(ui.FadeOutCanvasGroup(saveCorruptedText));
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = false;
		    clearSaveBlocker.SetActive(true);
		    yield return StartCoroutine(ui.FadeInCanvasGroup(clearSavePrompt));
		    clearSavePrompt.interactable = true;
		    clearSavePrompt.blocksRaycasts = true;
		    clearSavePromptHighlight.HighlightDefault(false);
		    interactable = false;
		    myCanvasGroup.blocksRaycasts = false;
		    ih.StartUIInput();
		}
		else if(nextState == SlotState.HIDDEN)
		{
		    topFleur.ResetTrigger("show");
		    topFleur.SetTrigger("hide");
		    yield return new WaitForSeconds(0.2f);
		    StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
		    StartCoroutine(ui.FadeOutCanvasGroup(saveCorruptedText));
		    StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton));
		    clearSaveButton.blocksRaycasts = false;
		}
		else if(nextState == SlotState.OPERATION_IN_PROGRESS)
		{
		    StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
		    StartCoroutine(ui.FadeOutCanvasGroup(saveCorruptedText));
		    yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton));
		    StartCoroutine(currentLoadingTextFadeIn = FadeInCanvasGroupAfterDelay(5f, loadingText));
		}
	    }
	    else if(state == SlotState.OPERATION_IN_PROGRESS && nextState == SlotState.HIDDEN)
	    {
		topFleur.ResetTrigger("show");
		topFleur.SetTrigger("hide");
		yield return new WaitForSeconds(0.2f);
		StartCoroutine(ui.FadeOutCanvasGroup(loadingText));
	    }
	}

	public void ShowRelevantModeForSaveFileState()
	{
	    switch (saveFileState)
	    {
		case SaveFileStates.Empty:
		    coroutineQueue.Enqueue(AnimateToSlotState(SlotState.EMPTY_SLOT));
		    return;
		case SaveFileStates.LoadedStats:
		    if(saveStats.permadeathMode == 2)
		    {
			coroutineQueue.Enqueue(AnimateToSlotState(SlotState.DEFEATED));
			return;
		    }
		    coroutineQueue.Enqueue(AnimateToSlotState(SlotState.SAVE_PRESENT));
		    break;
		case SaveFileStates.Corrupted:
		    coroutineQueue.Enqueue(AnimateToSlotState(SlotState.CORRUPTED));
		    break;
		default:
		    break;
	    }
	}

	/// <summary>
	/// 进入清除确认状态CLEAR_PROMPT
	/// </summary>
	public void ClearSavePrompt()
	{
	    coroutineQueue.Enqueue(AnimateToSlotState(SlotState.CLEAR_PROMPT));
	}

	/// <summary>
	/// 进入隐藏状态HIDDEN
	/// </summary>
	public void HideSaveSlot()
	{
	    coroutineQueue.Enqueue(AnimateToSlotState(SlotState.HIDDEN));
	}

	private IEnumerator FadeInCanvasGroupAfterDelay(float delay, CanvasGroup cg)
	{
	    for (float timer = 0f; timer < delay; timer += Time.unscaledDeltaTime)
	    {
		yield return null;
	    }
	    yield return ui.FadeInCanvasGroup(cg);
	}

	public enum SaveSlot
	{
	    SLOT_1,
	    SLOT_2,
	    SLOT_3,
	    SLOT_4
	}

	public enum SaveFileStates
	{
	    NotStarted, //还未开始
	    OperationInProgress, //进展阶段
	    Empty, //空
	    LoadedStats, //上传后阶段
	    Corrupted //被破坏阶段
	}

	public enum SlotState //SaveSlot的状态
	{
	    HIDDEN, //隐藏
	    OPERATION_IN_PROGRESS, //进展
	    EMPTY_SLOT, //空的slot
	    SAVE_PRESENT, //保存当前的
	    CORRUPTED, //被破坏
	    CLEAR_PROMPT, //清除确认
	    DEFEATED //钢魂死亡
	}
    }
}