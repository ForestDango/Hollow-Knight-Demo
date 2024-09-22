using System;
using System.Collections.Generic;
using System.Reflection;
using GlobalEnums;
using UnityEngine;

[Serializable]
public class PlayerData
{
    private static PlayerData _instance;
    public static PlayerData instance
    {
	get
	{
	    if(_instance == null)
	    {
		_instance = new PlayerData();
	    }
	    return _instance;
	}
	set
	{
	    _instance = value;
	}
    }

    public int nailDamage;

    public bool hasDash;
    public bool canDash;
    public bool hasBackDash;
    public bool canBackDash;

    public bool gotCharm_31;
    public bool equippedCharm_31;


    protected PlayerData()
    {
	SetupNewPlayerData();
    }

    public void Reset()
    {
	SetupNewPlayerData();
    }

    private void SetupNewPlayerData()
    {
	nailDamage = 5;

	hasDash = true; //≤‚ ‘Ω◊∂Œœ»…Ë÷√Œ™true∑Ω±„≤‚ ‘
	canDash = true;
	hasBackDash = false;
	canBackDash = false;
	gotCharm_31 = true;
	equippedCharm_31 = true;
    }


    public int GetInt(string intName)
    {
	if (string.IsNullOrEmpty(intName))
	{
	    Debug.LogError("PlayerData: Int with an EMPTY name requested.");
	    return -9999;
	}
	FieldInfo fieldInfo = GetType().GetField(intName);
	if(fieldInfo != null)
	{
	    return (int)fieldInfo.GetValue(instance);
	}
	Debug.LogError("PlayerData: Could not find int named " + intName + " in PlayerData");
	return -9999;
    }

}
