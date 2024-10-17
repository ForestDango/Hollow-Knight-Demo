using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Audio)]
    [Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Optionally plays a one shot Audio Clip.")]
    public class AudioPlayV2 : FsmStateAction
    {
	[RequiredField]
	[CheckForComponent(typeof(AudioSource))]
	[Tooltip("The GameObject with an AudioSource component.")]
	public FsmOwnerDefault gameObject;
	[HasFloatSlider(0f, 1f)]
	[Tooltip("Set the volume.")]
	public FsmFloat volume;
	[ObjectType(typeof(AudioClip))]
	[Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
	public FsmObject oneShotClip;
	[Tooltip("Event to send when the AudioClip finishes playing.")]
	public FsmEvent finishedEvent;

	private AudioSource audio;

	public override void Reset()
	{
	    gameObject = null;
	    volume = 1f;
	    oneShotClip = null;
	    finishedEvent = null;
	}

	public override void OnEnter()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (ownerDefaultTarget != null)
	    {
		audio = ownerDefaultTarget.GetComponent<AudioSource>();
		if (audio != null)
		{
		    AudioClip audioClip = oneShotClip.Value as AudioClip;
		    if (audioClip == null)
		    {
			if (!audio.isPlaying)
			{
			    audio.Play();
			}
			if (!volume.IsNone)
			{
			    audio.volume = volume.Value;
			}
			return;
		    }
		    if (!volume.IsNone)
		    {
			audio.PlayOneShot(audioClip, volume.Value);
			return;
		    }
		    audio.PlayOneShot(audioClip);
		    return;
		}
	    }
	    Finish();
	}

	public override void OnUpdate()
	{
	    if(audio == null)
	    {
		Finish();
		return;
	    }
	    if (!audio.isPlaying)
	    {
		Fsm.Event(finishedEvent);
		Finish();
		return;
	    }
	    if(!volume.IsNone && volume.Value != audio.volume)
	    {
		audio.volume = volume.Value;
	    }
	}
    }
}
