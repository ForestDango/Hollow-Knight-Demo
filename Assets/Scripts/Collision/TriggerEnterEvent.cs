using System;
using UnityEngine;

public class TriggerEnterEvent : MonoBehaviour
{
    public bool waitForHeroInPosition;
    private bool active;

    public delegate void CollisionEvent(Collider2D collider, GameObject sender);
    public event CollisionEvent OnTriggerEntered;
    public event CollisionEvent OnTriggerExited;
    public event CollisionEvent OnTriggerStayed;

    private void Start()
    {
	active = false;
	if (!waitForHeroInPosition)
	{
	    active = true;
	    return;
	}
	if (HeroController.instance.isHeroInPosition)
	{
	    active = true;
	    return;
	}
	HeroController.HeroInPosition temp = null;
	temp = delegate (bool forceDirect)
	 {
	     active = true;
	     HeroController.instance.heroInPosition -= temp;
	 };
	HeroController.instance.heroInPosition += temp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if (!active)
	{
	    return;
	}
	if(OnTriggerEntered != null)
	{
	    OnTriggerEntered(collision,gameObject);
	}
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
	if (!active)
	{
	    return;
	}
	if (OnTriggerStayed != null)
	{
	    OnTriggerStayed(collision, gameObject);
	}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
	if (!active)
	{
	    return;
	}
	if (OnTriggerExited != null)
	{
	    OnTriggerExited(collision, gameObject);
	}
    }


}
