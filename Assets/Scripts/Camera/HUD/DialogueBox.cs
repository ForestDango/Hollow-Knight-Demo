using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    [Header("Conversation Info")]
    public string currentConversation;
    public int currentPage;

    [Header("Typewriter")]
    [Tooltip("Enables the typewriter effect.")]
    public bool useTypeWriter;

    [Range(1f, 100f)]
    public float revealSpeed = 20f;
    private float normalRevealSpeed;

    private TextMeshPro textMesh;
    private PlayMakerFSM proxyFSM;

    private bool typing;
    private bool fastTyping;
    private bool hidden;
    private TMP_PageInfo[] pageInfo;

    private void Start()
    {
	textMesh = gameObject.GetComponent<TextMeshPro>();
	normalRevealSpeed = revealSpeed;
	HideText();
	proxyFSM = FSMUtility.LocateFSM(gameObject, "Dialogue Page Control");
	if (proxyFSM == null)
	{
	    Debug.LogWarning("DialogueBox: Couldn't find an FSM on this GameObject to use as a proxy, events will not be fired from this dialogue box.");
	}
    }

    public void StartConversation(string convName, string sheetName)
    {
	SetConversation(convName, sheetName);
	ShowPage(1);
	PrintPageInfoAll();
    }

    public void SetConversation(string convName, string sheetName)
    {
	currentConversation = convName;
	currentPage = 1;
	Debug.LogFormat("Start using Language Text!");
	textMesh.text = Language.Language.Get(convName, sheetName);
	textMesh.ForceMeshUpdate();
    }

    public void ShowPage(int pageNum)
    {
	if (pageNum < 1 || pageNum > textMesh.textInfo.pageCount)
	{
	    SendConvEndEvent();
	    return;
	}
	if (hidden)
	{
	    hidden = false;
	}
	if (useTypeWriter)
	{
	    if (typing)
	    {
		StopTypewriter();
	    }
	    textMesh.pageToDisplay = pageNum;
	    currentPage = pageNum;
	    textMesh.maxVisibleCharacters = 600;
	    //textMesh.maxVisibleCharacters = GetFirstCharIndexOnPage() - 1;
	    string text = textMesh.text;
	    text = text.Replace("<br>", "\n");
	    textMesh.text = text;
	    StartCoroutine("TypewriteCurrentPage");
	    return;
	}
	textMesh.pageToDisplay = pageNum;
	currentPage = pageNum;
	textMesh.maxVisibleCharacters = 600;
	//textMesh.maxVisibleCharacters = GetLastCharIndexOnPage();
	SendEndEvent();
    }

    private int GetFirstCharIndexOnPage()
    {
	return textMesh.textInfo.pageInfo[currentPage - 1].firstCharacterIndex + 1;
    }

    public void HideText()
    {
	if (typing)
	{
	    StopTypewriter();
	}
	textMesh.maxVisibleCharacters = 0;
	hidden = true;
    }

    public void SpeedupTypewriter()
    {
	if (typing && !fastTyping)
	{
	    StopTypewriter();
	    normalRevealSpeed = revealSpeed;
	    revealSpeed = 200f;
	    fastTyping = true;
	    StartCoroutine(TypewriteCurrentPage());
	}
    }

    private IEnumerator TypewriteCurrentPage()
    {
	if (!typing)
	{
	    InvokeRepeating("ShowNextChar", 0f, 1f / revealSpeed);
	    typing = true;
	}
	while (typing)
	{
	    if(textMesh.maxVisibleCharacters >= GetLastCharIndexOnPage())
	    {
		StopTypewriter();
		SendEndEvent();
	    }
	    else
	    {
		yield return null;
	    }
	}
    }

    private void SendEndEvent()
    {
	if(currentPage == textMesh.textInfo.pageCount)
	{
	    SendConvEndEvent();
	    return;
	}
	SendPageEndEvent();
    }

    private void SendPageEndEvent()
    {
	if (proxyFSM != null)
	{
	    proxyFSM.SendEvent("PAGE_END");
	}
    }

    private void SendConvEndEvent()
    {
	if (proxyFSM != null)
	{
	    proxyFSM.SendEvent("CONVERSATION_END");
	}
    }

    private void StopTypewriter()
    {
	CancelInvoke("ShowNextChar");
	typing = false;
	fastTyping = false;
	revealSpeed = normalRevealSpeed;
    }

    private int GetLastCharIndexOnPage()
    {
	//这个函数有问题需要修复
	return textMesh.textInfo.pageInfo[currentPage-1].lastCharacterIndex + 1;
    }

    private void ShowNextChar()
    {
	TextMeshPro textMeshPro = textMesh;
	int maxVisibleCharacter = textMeshPro.maxVisibleCharacters;
	textMeshPro.maxVisibleCharacters = maxVisibleCharacter + 1;
    }

    public void PrintPageInfoAll()
    {
	Debug.LogFormat("Textmesh Length is" + textMesh.maxVisibleCharacters);
	Debug.LogFormat("PageInfo: Current conversation {0} contains {1} pages.\n", new object[]
	{
	    currentConversation,
	    textMesh.textInfo.pageCount
	});
	for (int i = 0; i < textMesh.textInfo.pageCount; i++)
	{
	    Debug.LogFormat("[Page {0}] Start/End: {1}/{2}\n", new object[]
	    {
		i + 1,
		textMesh.textInfo.pageInfo[i+1].firstCharacterIndex,
		textMesh.textInfo.pageInfo[i+1].lastCharacterIndex
	    });
	}
    }

}
