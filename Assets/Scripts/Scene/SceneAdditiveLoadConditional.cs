using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAdditiveLoadConditional : MonoBehaviour
{
    public string sceneNameToLoad = "";
    public string altSceneNameToLoad = "";
    private bool loadAlt;

    [Header("Main Tests")]
    public string needsPlayerDataBool = "";
    public bool playerDataBoolValue;

    [Space]
    public string doorTrigger = "";

    private bool sceneLoaded;
    private static List<SceneAdditiveLoadConditional> additiveSceneLoads = new List<SceneAdditiveLoadConditional>();
    public static bool loadInSequence = false;

    private string SceneNameToLoad
    {
	get
	{
	    if (!loadAlt)
	    {
		return sceneNameToLoad;
	    }
	    return altSceneNameToLoad;
	}
    }

    public static bool ShouldLoadBoss
    {
	get
	{
	    return additiveSceneLoads != null && additiveSceneLoads.Count > 0;
	}
    }

    private void OnEnable()
    {
	if(sceneNameToLoad != null)
	{
	    bool flag = false;
	    if (needsPlayerDataBool != "" && GameManager.instance.GetPlayerDataBool(needsPlayerDataBool) != playerDataBoolValue)
	    {
		flag = true;
	    }

	    if (GameManager.instance.entryGateName != "dreamGate" && !flag && doorTrigger != "" && TransitionPoint.lastEntered != doorTrigger)
	    {
		flag = true;
	    }
	    if (flag)
	    {
		if(altSceneNameToLoad == "")
		{
		    return;
		}
		loadAlt = true;
	    }
	    if (loadInSequence)
	    {
		additiveSceneLoads.Add(this);
		return;
	    }
	    StartCoroutine(LoadRoutine(true));
	}
    }

    private void OnDisable()
    {
	if (sceneLoaded)
	{
	    SceneAdditiveLoadConditional.additiveSceneLoads.Remove(this);
	    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneNameToLoad);
	}
    }

    private IEnumerator LoadRoutine(bool callEvent = false)
    {
	bool inSequence = SceneAdditiveLoadConditional.loadInSequence;
	yield return null;
	AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneNameToLoad, LoadSceneMode.Additive);
	yield return asyncOperation;
	Debug.Log("Additively loaded scene: " + SceneNameToLoad + (inSequence ? " in sequence" : " out of sequence"));
	sceneLoaded = true;
	if (callEvent && GameManager.instance)
	{
	    GameManager.instance.LoadedBoss();
	}
    }

    [Serializable]
    public struct BoolTest
    {
	public string playerDataBool;
	public bool value;
    }

    [Serializable]
    public struct IntTest
    {
	public string playerDataInt;
	public string otherPlayerDataInt;
	public int value;
	public SceneAdditiveLoadConditional.IntTest.TestType testType;

	public enum TestType
	{
	    Equal,
	    Less,
	    More,
	    NotEqual,
	    LessOrEqual,
	    MoreOrEqual
	}
    }

}
