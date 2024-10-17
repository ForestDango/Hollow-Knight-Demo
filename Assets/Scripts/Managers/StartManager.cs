using System;
using System.Collections;
using InControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    private RuntimePlatform platform;

    private void Awake()
    {
	platform = Application.platform;
    }

    private IEnumerator Start()
    {
	AsyncOperation loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Menu_Title");
	loadOperation.allowSceneActivation = false;
	bool didWaitForPlayerPrefs = false;
	if (!didWaitForPlayerPrefs)
	{
	    Debug.LogFormat("Didn't need to wait for PlayerPrefs load.", Array.Empty<object>());
	}
	else
	{
	    Debug.LogFormat("Finished waiting for PlayerPrefs load.", Array.Empty<object>());
	}
	loadOperation.allowSceneActivation = true;
	yield return loadOperation;
    }
}
