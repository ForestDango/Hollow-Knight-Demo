using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    private GameManager gm;
    [SerializeField]public PlayerData playerData;
    private void Awake()
    {
	gm = GameManager.instance;
	playerData = gm.playerData;
    }

    private void Update()
    {
	if (Input.GetKeyDown(KeyCode.P))
	{
	    playerData.hasDash = true;
	    playerData.MPCharge = playerData.maxMP;
	    playerData.fireballLevel = 1;
	}
    }

}
