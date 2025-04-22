using System;
using UnityEngine;

[Serializable]
public struct AudioEventRandom
{
    public AudioClip[] Clips;
    public float PitchMin;
    public float PitchMax;
    public float Volume;

    public void Reset()
    {
	PitchMin = 0.75f;
	PitchMax = 1.25f;
	Volume = 1f;
    }

    public float SelectPitch()
    {
	if (Mathf.Approximately(PitchMin, PitchMax))
	{
	    return PitchMax;
	}
	return UnityEngine.Random.Range(PitchMin, PitchMax);
    }

    public void SpawnAndPlayOneShot(AudioSource prefab, Vector3 position)
    {
	if (Clips.Length == 0)
	    return;
	AudioClip audioClip = Clips[UnityEngine.Random.Range(0, Clips.Length)];
	if (audioClip == null)
	    return;
	if (Volume < Mathf.Epsilon)
	    return;
	if (prefab == null)
	    return;
	AudioSource audioSource = prefab.Spawn(position);
	audioSource.volume = Volume;
	audioSource.pitch = SelectPitch();
	audioSource.PlayOneShot(audioClip);
    }
}
