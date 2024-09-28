using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.Audio)]
    [Tooltip("Sets the Volume of the Audio Clip played by the AudioSource component on a Game Object.")]
    public class FadeAudio : ComponentAction<AudioSource>
    {
	[RequiredField]
	[CheckForComponent(typeof(AudioSource))]
	public FsmOwnerDefault gameObject;

	public FsmFloat startVolume;
	public FsmFloat endVolume;
	public FsmFloat time;

	private float timeElapsed;
	private float timePercentage;
	private bool fadingDown;

	public override void Reset()
	{
	    gameObject = null;
	    startVolume = 1f;
	    endVolume = 0f;
	    time = 1f;
	}

	public override void OnEnter()
	{
	    if(startVolume.Value > endVolume.Value)
	    {
		fadingDown = true;
	    }
	    else
	    {
		fadingDown = false;
	    }
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (UpdateCache(ownerDefaultTarget))
	    {
		audio.volume = startVolume.Value;
	    }
	}

	public override void OnUpdate()
	{
	    DoSetAudioVolume();
	}

	public override void OnExit()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (UpdateCache(ownerDefaultTarget))
	    {
		audio.volume = endVolume.Value;
	    }
	}

	private void DoSetAudioVolume()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (UpdateCache(ownerDefaultTarget))
	    {
		timeElapsed += Time.deltaTime;
		timePercentage = timeElapsed / time.Value * 100f;
		float num = (endVolume.Value - startVolume.Value) * (timePercentage / 100f);
		audio.volume = audio.volume + num;
		if (fadingDown && audio.volume <= endVolume.Value)
		{
		    audio.volume = endVolume.Value;
		    Finish();
		}
		else if (!fadingDown && audio.volume >= endVolume.Value)
		{
		    audio.volume = endVolume.Value;
		    Finish();
		}
		timeElapsed = 0f;
	    }
	}


    }

}
