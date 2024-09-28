using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Particle System")]
    [Tooltip("Set particle emission on or off on an object with a particle emitter")]
    public class SetParticleEmissionRate : FsmStateAction
    {
	[RequiredField]
	[Tooltip("The particle emitting GameObject")]
	public FsmOwnerDefault gameObject;

	public FsmFloat emissionRate;
	public bool everyFrame;
	private ParticleSystem emitter;

	public override void Reset()
	{
	    gameObject = null;
	    emissionRate = null;
	    everyFrame = false;
	}

	public override void OnEnter()
	{
	    if(gameObject != null)
	    {
		GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
		if (ownerDefaultTarget)
		{
		    emitter = ownerDefaultTarget.GetComponent<ParticleSystem>();
		}
		DoSetEmitRate();
		if (!everyFrame)
		{
		    Finish();
		}
	    }
	}

	public override void OnUpdate()
	{
	    DoSetEmitRate();
	}

	private void DoSetEmitRate()
	{
	    if (emitter)
	    {
		emitter.emissionRate = emissionRate.Value;
	    }
	}
    }

}
