using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "MusicCue", menuName = "Hollow Knight/Music Cue", order = 1000)]
public class MusicCue : ScriptableObject
{
    [SerializeField] private string originalMusicEventName;
    [SerializeField] private int originalMusicTrackNumber;
    [SerializeField] private AudioMixerSnapshot snapshot;
    [SerializeField]
    [ArrayForEnum(typeof(MusicChannels))]
    private MusicChannelInfo[] channelInfos;
    [SerializeField] private Alternative[] alternatives;

    public string OriginalMusicEventName
    {
	get
	{
	    return originalMusicEventName;
	}
    }

    public int OriginalMusicTrackNumber
    {
	get
	{
	    return originalMusicTrackNumber;
	}
    }

    public AudioMixerSnapshot Snapshot
    {
	get
	{
	    return snapshot;
	}
    }

    public MusicChannelInfo GetChanelInfo(MusicChannels channel)
    {
	if (channel < MusicChannels.Main || channel >= (MusicChannels)channelInfos.Length)
	{
	    return null;
	}
	return channelInfos[(int)channel];
    }

    public MusicCue ResolveAlternatives(PlayerData playerData)
    {
	if (alternatives != null)
	{
	    int i = 0;
	    while (i < alternatives.Length)
	    {
		MusicCue.Alternative alternative = alternatives[i];
		if (playerData.GetBool(alternative.PlayerDataBoolKey))
		{
		    MusicCue cue = alternative.Cue;
		    if (!(cue != null))
		    {
			return null;
		    }
		    return cue.ResolveAlternatives(playerData);
		}
		else
		{
		    i++;
		}
	    }
	}
	return this;
    }

    [Serializable]
    public class MusicChannelInfo
    {
	[SerializeField] private AudioClip clip;
	[SerializeField] private MusicChannelSync sync;

	public AudioClip Clip
	{
	    get
	    {
		return clip;
	    }
	}
	public bool IsEnabled
	{
	    get
	    {
		return clip != null;
	    }
	}
	public bool IsSyncRequired
	{
	    get
	    {
		if(sync == MusicChannelSync.Implicit)
		{
		    return clip != null;
		}
		return sync == MusicChannelSync.ExplicitOn;
	    }
	}
    }

    [Serializable]
    public struct Alternative
    {
	public string PlayerDataBoolKey;
	public MusicCue Cue;
    }
}
