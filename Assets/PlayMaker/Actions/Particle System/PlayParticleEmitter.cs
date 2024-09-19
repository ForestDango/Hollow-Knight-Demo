using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Particle System")]
    [Tooltip("Set particle emission on or off on an object with a particle emitter")]
    public class PlayParticleEmitter : FsmStateAction
    {
	[RequiredField]
	[Tooltip("The particle emitting GameObject")]
	public FsmOwnerDefault gameObject;

	public FsmInt emit;

	public override void Reset()
	{
	    gameObject = null;
	    emit = new FsmInt(0);
	}

	public override void OnEnter()
	{
	    if(gameObject != null)
	    {
		GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
		if (ownerDefaultTarget != null)
		{
		    ParticleSystem component = ownerDefaultTarget.GetComponent<ParticleSystem>();
		    if (component && !component.isPlaying && emit.Value <= 0)
		    {
			component.Play();
		    }
		    else if (emit.Value > 0)
		    {
			component.Emit(emit.Value);
		    }
		}
	    }
	    Finish();
	}


    }

}
