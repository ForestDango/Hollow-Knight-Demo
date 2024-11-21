using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.Audio)]
    [Tooltip("Instantiate an Audio Player object and play a oneshot sound via its Audio Source.")]
    public class AudioPlayerOneShot : FsmStateAction
    {

	[RequiredField]
	[CheckForComponent(typeof(AudioSource))]
	[Tooltip("The object to spawn. Select Audio Player prefab.")]
	public FsmGameObject audioPlayer;

	[RequiredField]
	[Tooltip("Object to use as the spawn point of Audio Player")]
	public FsmGameObject spawnPoint;

	[CompoundArray("Audio Clips", "Audio Clip", "Weight")]
	public AudioClip[] audioClips;

	[HasFloatSlider(0f, 1f)]
	public FsmFloat[] weights;
	public FsmFloat pitchMin;
	public FsmFloat pitchMax;
	public FsmFloat volume;
	public FsmFloat delay;
	public FsmGameObject storePlayer;

	private AudioSource audio;

	private float timer;

	public override void Reset()
	{
	    spawnPoint = null;
	    audioClips = new AudioClip[3];
	    weights = new FsmFloat[]
	    {
		1f,
		1f,
		1f
	    };
	    pitchMin = 1f;
	    pitchMax = 1f;
	    volume = 1f;
	    timer = 0f;
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
	    if(delay.Value > 0f)
	    {
		if(timer < delay.Value)
		{
		    timer += Time.deltaTime;
		    return;
		}
		DoPlayRandomClip();
		Finish();
	    }	
	}

	private void DoPlayRandomClip()
	{
	    if (audioClips.Length == 0)
		return;
	    GameObject value = audioPlayer.Value;
	    Vector3 position = spawnPoint.Value.transform.position;
	    Vector3 up = Vector3.up;
	    GameObject gameObject = audioPlayer.Value.Spawn(position, Quaternion.Euler(up));
	    //GameObject gameObject = UnityEngine.Object.Instantiate(audioPlayer.Value, position, Quaternion.Euler(up));
	    audio = gameObject.GetComponent<AudioSource>();
	    int randomWeightIndex = ActionHelpers.GetRandomWeightedIndex(weights);
	    if(randomWeightIndex != -1)
	    {
		AudioClip audioClip = audioClips[randomWeightIndex];
		if(audioClip != null)
		{
		    float pitch = UnityEngine.Random.Range(pitchMin.Value, pitchMax.Value);
		    audio.pitch = pitch;
		    audio.PlayOneShot(audioClip);
		}
	    }
	    audio.volume = volume.Value;
	}

	public AudioPlayerOneShot()
	{
	    pitchMin = 1f;
	    pitchMax = 2f;
	}
    }

}
