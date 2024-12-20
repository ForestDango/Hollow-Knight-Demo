using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    [ArrayForEnum(typeof(AtmosChannels))]
    private AudioSource[] atmosSources;
    private AtmosCue currentAtmosCue;

    [SerializeField]
    [ArrayForEnum(typeof(MusicChannels))]
    private AudioSource[] musicSources;
    private MusicCue currentMusicCue;

    [SerializeField] private AudioLoopMaster audioLoopMaster;

    private IEnumerator applyAtmosCueRoutine;
    private IEnumerator applyMusicCueRoutine;
    private IEnumerator applyMusicSnapshotRoutine;

    public MusicCue CurrentMusicCue
    {
	get
	{
	    return currentMusicCue;
	}
    }

    protected void Start()
    {
    }

    public void ApplyAtmosCue(AtmosCue atmosCue, float transitionTime)
    {
	if(atmosCue != null)
	{
	    Debug.LogError("Unable to apply null AtmosCue");
	    return;
	}
	if(applyAtmosCueRoutine != null)
	{
	    StopCoroutine(applyAtmosCueRoutine);
	    applyAtmosCueRoutine = null;
	}
	StartCoroutine(applyAtmosCueRoutine = BeginApplyAtmosCue(atmosCue, transitionTime));
    }

    private IEnumerator BeginApplyAtmosCue(AtmosCue atmosCue, float transitionTime)
    {
	currentAtmosCue = atmosCue;
	atmosCue.Snapshot.TransitionTo(transitionTime);
	for (int i = 0; i < atmosSources.Length; i++)
	{
	    if (atmosCue.IsChannelEnabled((AtmosChannels)i))
	    {
		AudioSource audioSource = atmosSources[i];
		if (!audioSource.isPlaying)
		{
		    audioSource.Play();
		}
	    }
	}
	yield return new WaitForSecondsRealtime(transitionTime);
	for (int j = 0; j < atmosSources.Length; j++)
	{
	    if(!atmosCue.IsChannelEnabled((AtmosChannels)j))
	    {
		AudioSource audioSource2 = atmosSources[j];
		if (audioSource2.isPlaying)
		{
		    audioSource2.Stop();
		}
	    }
	}
	
    }

    public void ApplyMusicCue(MusicCue musicCue, float delayTime, float transitionTime, bool applySnapshot)
    {
	if (musicCue == null)
	{
	    Debug.LogError("Unable to apply null MusicCue");
	    return;
	}
	GameManager unsafeInstance = GameManager.UnsafeInstance;
	if(unsafeInstance != null)
	{
	    PlayerData playerData = unsafeInstance.playerData;
	    if (playerData != null)
	    {
		MusicCue y = musicCue;
		musicCue = musicCue.ResolveAlternatives(playerData);
	    }
	}
	if(currentMusicCue == musicCue)
	{
	    return;
	}
	if(applyMusicCueRoutine != null)
	{
	    StopCoroutine(applyMusicCueRoutine);
	    applyMusicCueRoutine = null;
	}
	StartCoroutine(applyMusicCueRoutine = BeginApplyMusicCue(musicCue, delayTime, transitionTime, applySnapshot));
    }

    private IEnumerator BeginApplyMusicCue(MusicCue musicCue, float delayTime, float transitionTime, bool applySnapshot)
    {
	currentMusicCue = musicCue;
	yield return new WaitForSecondsRealtime(delayTime);
	for (int i = 0; i < musicSources.Length; i++)
	{
	    AudioSource audioSource = musicSources[i];
	    audioSource.Stop();
	    audioSource = null;
	}
	for (int j = 0; j < musicSources.Length; j++)
	{
	    AudioSource audioSource2 = musicSources[j];
	    MusicCue.MusicChannelInfo channelInfo = musicCue.GetChannelInfo((MusicChannels)j);
	    if (channelInfo != null && channelInfo.IsEnabled)
	    {
		if (audioSource2.clip != channelInfo.Clip)
		{
		    audioSource2.clip = channelInfo.Clip;
		}
		audioSource2.volume = 1f;
		audioSource2.Play();
	    }
	    UpdateMusicSync((MusicChannels)j, channelInfo != null && channelInfo.IsSyncRequired);
	}
	yield return new WaitForSecondsRealtime(transitionTime);
	for (int k = 0; k < musicSources.Length; k++)
	{
	    MusicCue.MusicChannelInfo channelInfo2 = musicCue.GetChannelInfo((MusicChannels)k);
	    if (channelInfo2 == null || !channelInfo2.IsEnabled)
	    {
		AudioSource audioSource3 = musicSources[k];
		if (audioSource3.isPlaying)
		{
		    audioSource3.clip = null;
		    audioSource3.Stop();
		}
	    }
	}
    }

    private void UpdateMusicSync(MusicChannels musicChannel, bool isSyncRequired)
    {
	switch (musicChannel)
	{
	    case MusicChannels.Main:
		break;
	    case MusicChannels.MainAlt:
		audioLoopMaster.SetSyncMainAlt(isSyncRequired);
		break;
	    case MusicChannels.Action:
		audioLoopMaster.SetSyncAction(isSyncRequired);
		break;
	    case MusicChannels.Sub:
		audioLoopMaster.SetSyncSub(isSyncRequired);
		break;
	    case MusicChannels.Tension:
		audioLoopMaster.SetSyncTension(isSyncRequired);
		break;
	    case MusicChannels.Extra:
		audioLoopMaster.SetSyncExtra(isSyncRequired);
		break;
	    default:
		break;
	}
    }

    public void ApplyMusicSnapshot(AudioMixerSnapshot snapshot, float delayTime, float transitionTime)
    {
	if (applyMusicSnapshotRoutine != null)
	{
	    StopCoroutine(applyMusicSnapshotRoutine);
	    applyMusicSnapshotRoutine = null;
	}
	if (snapshot != null)
	{
	    StartCoroutine(applyMusicSnapshotRoutine = BeginApplyMusicSnapshot(snapshot, delayTime, transitionTime));
	}
    }

    protected IEnumerator BeginApplyMusicSnapshot(AudioMixerSnapshot snapshot, float delayTime, float transitionTime)
    {
	if (delayTime > Mathf.Epsilon)
	{
	    yield return new WaitForSecondsRealtime(delayTime);
	}
	if (snapshot != null)
	{
	    snapshot.TransitionTo(transitionTime);
	}
	yield break;
    }
}
