using System;
using UnityEngine;

public class WalkArea : MonoBehaviour
{
    private Collider2D myCollider;
    private GameManager gm;
    private HeroController heroCtrl;
    private bool activated;
    private bool verboseMode;

    protected void Awake()
    {
	myCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
	gm = GameManager.instance;
	gm.UnloadingLevel += Deactivate;
	heroCtrl = HeroController.instance;
    }

    private void Deactivate()
    {
	activated = false;
	heroCtrl.SetWalkZone(false);
    }

    private void OnDisable()
    {
	if(gm != null)
	{
	    gm.UnloadingLevel -= Deactivate;
	}
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
	if(otherCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
	{
	    activated = true;
	    heroCtrl.SetWalkZone(true);
	}
    }

    private void OnTriggerStay2D(Collider2D otherCollider)
    {
	if (!activated && myCollider.enabled && otherCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
	{
	    activated = true;
	    heroCtrl.SetWalkZone(true);
	}
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
	if (otherCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
	{
	    activated = false;
	    heroCtrl.SetWalkZone(false);
	}
    }

}
