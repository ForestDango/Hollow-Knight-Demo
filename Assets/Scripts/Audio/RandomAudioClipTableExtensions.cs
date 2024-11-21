using System;
using UnityEngine;

public static class RandomAudioClipTableExtensions
{
    public static void PlayOneShot(this RandomAudioClipTable table, AudioSource audioSource)
    {
	if (table == null)
	{
	    return;
	}
	table.PlayOneShotUnsafe(audioSource);
    }

    public static void SpawnAndPlayOneShot(this RandomAudioClipTable table, AudioSource prefab, Vector3 position)
    {
	if (table == null)
	{
	    return;
	}
	if (prefab == null)
	{
	    return;
	}
	AudioClip audioClip = table.SelectClip();
	if (audioClip == null)
	{
	    return;
	}
	AudioSource audioSource = prefab.Spawn<AudioSource>();
	//AudioSource audioSource = GameObject.Instantiate(prefab).GetComponent<AudioSource>();
	audioSource.transform.position = position;
	audioSource.pitch = table.SelectPitch();
	audioSource.volume = 1f;
	audioSource.PlayOneShot(audioClip);
    }
}
