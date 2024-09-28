using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Audio)]
    [Tooltip("Instantiate an Audio Player object and play a oneshot sound via its Audio Source.")]
    public class AudioPlayerOneShotSingle : FsmStateAction
    {
	[RequiredField]
	[Tooltip("The object to spawn. Select Audio Player prefab.")]
	public FsmGameObject audioPlayer;

	[RequiredField]
	[Tooltip("Object to use as the spawn point of Audio Player")]
	public FsmGameObject spawnPoint;

	[ObjectType(typeof(AudioClip))]
	public FsmObject audioClip;

	public FsmFloat pitchMin;
	public FsmFloat pitchMax;
	public FsmFloat volume = 1f;
	public FsmFloat delay;
	public FsmGameObject storePlayer;

	private AudioSource audio;
	private float timer;

	public override void Reset()
	{
	    spawnPoint = null;
	    pitchMin = 1f;
	    pitchMax = 1f;
	    volume = 1f;
	}

	public override void OnEnter()
	{
	    timer = 0f;
	    if(delay.Value == 0f)
	    {
		DoPlayRandomClip();
		Finish();
	    }
	}
	
	public override void OnUpdate()
	{
	    if (delay.Value > 0f)
	    {
		timer += Time.deltaTime;
		return;
	    }
	    DoPlayRandomClip();
	    Finish();
	}

	private void DoPlayRandomClip()
	{
	    if(!audioPlayer.IsNone && !spawnPoint.IsNone && spawnPoint.Value != null)
	    {

		GameObject value = audioPlayer.Value;
		Vector3 position = spawnPoint.Value.transform.position;
		Vector3 up = Vector3.up;
		if (audioPlayer.Value != null)
		{
		    GameObject gameObject = GameObject.Instantiate(audioPlayer.Value, position, Quaternion.Euler(up));
		    audio = gameObject.GetComponent<AudioSource>();
		    storePlayer.Value = gameObject;
		    AudioClip audioClip = this.audioClip.Value as AudioClip;
		    float pitch = UnityEngine.Random.Range(pitchMin.Value, pitchMax.Value);
		    audio.pitch = pitch;
		    audio.volume = volume.Value;
		    if (audioClip != null)
		    {
			audio.PlayOneShot(audioClip);
			return;
		    }
		}
		else
		{
		    Debug.LogError("AudioPlayer object not set!");
		}
	    }
	}
    }

}
