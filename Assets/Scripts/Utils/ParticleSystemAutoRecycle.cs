using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemAutoRecycle : MonoBehaviour
{
    private ParticleSystem ps;
    private bool activated;
    public void Start()
    {
	this.ps = base.GetComponent<ParticleSystem>();
	if (!this.ps)
	{
	    this.ps = base.GetComponentInChildren<ParticleSystem>();
	}
    }

    public void Update()
    {
	if (this.ps)
	{
	    if (this.ps.IsAlive())
	    {
		this.activated = true;
	    }
	    if (!this.ps.IsAlive() && this.activated)
	    {
		this.Recycle<ParticleSystemAutoRecycle>();
	    }
	}
    }

}
