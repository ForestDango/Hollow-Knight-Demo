using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Audio)]
    [Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Optionally plays a one shot Audio Clip.")]
    public class AudioPlaySimple : FsmStateAction
    {
	[RequiredField]
	[CheckForComponent(typeof(AudioSource))]
	[Tooltip("The GameObject with an AudioSource component.")]
	public FsmOwnerDefault gameObject;

	[HasFloatSlider(0f,1f)]
	[Tooltip("Set the volume.")]
	public FsmFloat volume;

	[ObjectType(typeof(AudioClip))]
	[Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
	public FsmObject oneShotClip;

	private AudioSource audio;

	public override void Reset()
	{
	    gameObject = null;
	    volume = 1f;
	    oneShotClip = null;
	}

	public override void OnEnter()
	{
	    GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
	    if(ownerDefaultTarget != null)
	    {
		audio = ownerDefaultTarget.GetComponent<AudioSource>();
		if(audio != null)
		{
		    AudioClip audioClip = oneShotClip.Value as AudioClip;
		    if(audioClip == null)
		    {
			if (!this.audio.isPlaying)
			{
			    this.audio.Play();
			}
			if (!this.volume.IsNone)
			{
			    this.audio.volume = this.volume.Value;
			}
		    }
		    if (!volume.IsNone)
		    {
			audio.PlayOneShot(audioClip, volume.Value);
		    }
		    else
		    {
			audio.PlayOneShot(audioClip);
		    }
		}
	    }
	    Finish();
	}
    }
}
