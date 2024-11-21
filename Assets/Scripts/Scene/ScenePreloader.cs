using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ScenePreloader : MonoBehaviour
{
    public string sceneNameToLoad = "";
    public string sceneNameToLoadDefeated = "";
    public string needsPlayerDataBool = "";
    public bool playerDataBoolValue;

    private float startLoadTime;
    private float endLoadTime;
    private float? loadTime;

    private static List<SceneLoadOp> pendingOperations = new List<SceneLoadOp>();
    private static List<SceneLoadOp> completedOperations = new List<SceneLoadOp>();

    private void Start()
    {
	loadTime = null;
	if(sceneNameToLoad != null)
	{
	    if(needsPlayerDataBool != "" && GameManager.instance.GetPlayerDataBool(needsPlayerDataBool) != playerDataBoolValue)
	    {
		if (!string.IsNullOrEmpty(sceneNameToLoadDefeated))
		{
		    StartCoroutine(LoadRoutineDefeated());
		}
		return;
	    }
	    StartCoroutine(LoadRoutine());
	}
    }

    private void OnDestroy()
    {
	//TODO:
	UnityEngine.SceneManagement.SceneManager.UnloadScene(sceneNameToLoad);
    }

    private void OnGUI()
    {
	if ((Debug.isDebugBuild || Application.isEditor || Application.platform == RuntimePlatform.Switch) && loadTime != null)
	{
	    GUI.Label(new Rect(10f, 5f, 500f, 50f), string.Format("Preloaded Level:{0}, Time: {1}", sceneNameToLoad, loadTime));
	}
    }

    private IEnumerator LoadRoutine()
    {
	yield return null;
	AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneNameToLoad, LoadSceneMode.Additive);
	async.allowSceneActivation = false;
	pendingOperations.Add(new SceneLoadOp(sceneNameToLoad, async));
	startLoadTime = Time.unscaledTime;
	while (async.progress < 0.9f)
	{
	    yield return null;
	}
	async.allowSceneActivation = true;
	endLoadTime = Time.unscaledTime;
	loadTime = new float?(endLoadTime - startLoadTime);
    }

    public IEnumerator LoadRoutineDefeated()
    {
	yield return null;
	AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneNameToLoadDefeated, LoadSceneMode.Additive);
	async.allowSceneActivation = false;
	pendingOperations.Add(new SceneLoadOp(sceneNameToLoadDefeated, async));
	startLoadTime = Time.unscaledTime;
	while (async.progress < 0.9f)
	{
	    yield return null;
	}
	async.allowSceneActivation = true;
	endLoadTime = Time.unscaledTime;
	loadTime = new float?(endLoadTime - startLoadTime);
    }

    public static IEnumerator FinishPendingOperations()
    {
	if (pendingOperations != null)
	{
	    foreach (SceneLoadOp sceneLoadOp in pendingOperations)
	    {
		sceneLoadOp.operation.allowSceneActivation = true;
		completedOperations.Add(sceneLoadOp);
		yield return sceneLoadOp.operation;
	    }
	    List<SceneLoadOp>.Enumerator enumerator = default(List<SceneLoadOp>.Enumerator);
	    pendingOperations.Clear();
	}
    }

    public static void Cleanup()
    {
	if (completedOperations != null)
	{
	    foreach (SceneLoadOp sceneLoadOp in completedOperations)
	    {
		UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneLoadOp.sceneName);
	    }
	    completedOperations.Clear();
	}
    }

    public class SceneLoadOp
    {
	public AsyncOperation operation;
	public string sceneName;
	public SceneLoadOp(string sceneName, AsyncOperation operation)
	{
	    this.sceneName = sceneName;
	    this.operation = operation;
	}
    }
}
