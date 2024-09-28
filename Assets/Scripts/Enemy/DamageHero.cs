using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHero : MonoBehaviour
{
    public int damageDealt = 1;
    public int hazardType = 1;
    public bool shadowDashHazard;
    public bool resetOnEnable;
    private int? initialValue;

    private void OnEnable()
    {
	if (resetOnEnable)
	{
	    if(initialValue == null)
	    {
		initialValue = new int?(damageDealt);
		return;
	    }
	    damageDealt = initialValue.Value;
	}
    }
}
