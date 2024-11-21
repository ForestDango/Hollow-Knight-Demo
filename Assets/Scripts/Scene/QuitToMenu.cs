using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitToMenu : MonoBehaviour
{
    public IEnumerator Start()
    {
	yield return null;
	UIManager instance = UIManager.instance;
	if (instance != null)
	{
	    UIManager.instance.AudioGoToGameplay(0f);
	    Destroy(instance.gameObject);
	}
	HeroController instance2 = HeroController.instance;
	if (instance2 != null)
	{
	    Destroy(instance2.gameObject);
	}
	GameCameras instance3 = GameCameras.instance;
	if(instance3 != null)
	{
	    Destroy(instance3.gameObject);
	}
	GameManager instance4 = GameManager.instance;
	if(instance4 != null)
	{
	    try
	    {
		ObjectPool.RecycleAll();
	    }
	    catch (Exception exception)
	    {
		Debug.LogErrorFormat("Error while recycling all as part of quit, attempting to continue regardless.", Array.Empty<object>());
		Debug.LogException(exception);
	    }
	    instance4.playerData.Reset();
	    //TODO:sceneData
	    Destroy(instance4.gameObject);
	}
	TimeController.GenericTimeScale = 1f;
	//TODO:Boss
	yield return null;
	GC.Collect();
	UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Menu_Title", LoadSceneMode.Single);
    }
}
