using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
	get
	{
	    if(_instance == null)
	    {
		_instance = FindObjectOfType<GameManager>();
	    }
	    if (_instance == null)
	    {
		Debug.LogError("Couldn't find a Game Manager, make sure one exists in the scene.");
	    }
	    else if (Application.isPlaying)
	    {
		DontDestroyOnLoad(_instance.gameObject);
	    }
	    return _instance;
	}
    }

    [SerializeField] public PlayerData playerData;

    private void Awake()
    {
	if(_instance != this)
	{
	    _instance = this;
	    DontDestroyOnLoad(this);
	    SetupGameRefs();
	    return;
	}
	if(this != _instance)
	{
	    Destroy(gameObject);
	    return;
	}
	SetupGameRefs();
    }

    private void SetupGameRefs()
    {
	playerData = PlayerData.instance;
    }



    public int GetPlayerDataInt(string intName)
    {
	return playerData.GetInt(intName);
    }
}
