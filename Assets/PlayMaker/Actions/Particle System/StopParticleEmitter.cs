using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
[ActionCategory("Particle System")]
[Tooltip("Set particle emission on or off on an object with a particle emitter")]
    public class StopParticleEmitter : FsmStateAction
    {
	[RequiredField]
	[Tooltip("The particle emitting GameObject")]
	public FsmOwnerDefault gameObject;

	public override void Reset()
	{
	    gameObject = null;
	}

	public override void OnEnter()
	{
	    if (gameObject != null)
	    {
		GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
		if (ownerDefaultTarget != null)
		{
		    ParticleSystem component = ownerDefaultTarget.GetComponent<ParticleSystem>();
		    if (component && component.isPlaying)
		    {
			component.Stop();
		    }
		}
	    }
	    Finish();
	}



    }

}
