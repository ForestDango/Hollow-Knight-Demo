using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Particle System")]
    [Tooltip("Set particle emission on or off on an object with a particle emitter")]
    public class SetParticleEmission : FsmStateAction
    {
	[RequiredField]
	[Tooltip("The particle emitting GameObject")]
	public FsmOwnerDefault gameObject;
	public FsmBool emission;

	public override void Reset()
	{
	    gameObject = null;
	    emission = false;
	}

	public override void OnEnter()
	{
	    if (gameObject != null)
	    {
		GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
		if (ownerDefaultTarget != null)
		{
		    ownerDefaultTarget.GetComponent<ParticleSystem>().enableEmission = emission.Value;
		}
	    }
	    Finish();
	}
    }
}
