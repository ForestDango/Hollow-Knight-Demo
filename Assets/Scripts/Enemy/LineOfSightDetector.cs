using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSightDetector : MonoBehaviour
{
    [SerializeField] private AlertRange[] alertRanges;
    private bool canSeeHero;

    public bool CanSeeHero
    {
	get
	{
	    return canSeeHero;
	}
    }

    protected void Awake()
    {
    }

    protected void Update()
    {
	bool flag = false;
	for (int i = 0; i < alertRanges.Length; i++)
	{
	    AlertRange alertRange = alertRanges[i];
	    if(!(alertRange == null) && alertRange.IsHeroInRange)
	    {
		flag = true;
	    }
	}
	if(alertRanges.Length != 0 && !flag)
	{
	    canSeeHero = false;
	    return;
	}
	HeroController instance = HeroController.instance;
	if(instance == null)
	{
	    canSeeHero = false;
	    return;
	}
	Vector2 vector = transform.position;
	Vector2 vector2 = instance.transform.position;
	Vector2 vector3 = vector2 - vector;
	if (Physics2D.Raycast(vector, vector3.normalized, vector3.magnitude, LayerMask.GetMask("Terrain")))
	{
	    canSeeHero = false;
	}
	else
	{
	    canSeeHero = true;
	}
	Debug.DrawLine(vector, vector2, canSeeHero ? Color.green : Color.yellow);
    }
}
