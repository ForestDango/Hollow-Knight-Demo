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
	    playerData.nailDamage = 100;
	    playerData.hasDash = true;
	    playerData.hasSpell = true;
	    Restore();
	    //playerData.MPCharge = playerData.maxMP;
	    playerData.fireballLevel = 1;
	    playerData.AddGeo(500);
	    playerData.hasCityKey = true;
	}
    }
    private void Restore()
    {
	GameManager unsafeInstance = GameManager.UnsafeInstance;
	if (unsafeInstance != null)
	{
	    HeroController hero_ctrl = unsafeInstance.hero_ctrl;
	    if (hero_ctrl != null)
	    {
		hero_ctrl.AddHealth(unsafeInstance.playerData.maxHealth - unsafeInstance.playerData.health);
		hero_ctrl.AddMPCharge(unsafeInstance.playerData.maxMP - unsafeInstance.playerData.MPCharge);
	    }
	}
    }

}
