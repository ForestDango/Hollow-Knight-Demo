using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Particle System")]
    [Tooltip("Set particle emission on or off on an object with a particle emitter")]
    public class SetParticleEmissionSpeed : FsmStateAction
    {
	[RequiredField]
	[Tooltip("The particle emitting GameObject")]
	public FsmOwnerDefault gameObject;
	public FsmFloat emissionSpeed;
	public bool everyFrame;

	private ParticleSystem emitter;

	public override void Reset()
	{
	    gameObject = null;
	    emissionSpeed = null;
	    everyFrame = false;
	}

	public override void OnEnter()
	{
	    if(gameObject != null)
	    {
		GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
		emitter = ownerDefaultTarget.GetComponent<ParticleSystem>();
		DoSetEmitSpeed();
		if (!everyFrame)
		{
		    Finish();
		}
	    }
	}

	public override void OnUpdate()
	{
	    DoSetEmitSpeed();
	}
	private void DoSetEmitSpeed()
	{
	    emitter.startSpeed = emissionSpeed.Value;
	}
    }
}
