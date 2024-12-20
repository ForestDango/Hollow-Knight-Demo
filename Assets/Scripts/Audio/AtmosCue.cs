using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewAtmosCue",menuName = "Hollow Knight/Atmos Cue",order = 1000)]
public class AtmosCue : ScriptableObject
{
    [SerializeField]
    private AudioMixerSnapshot snapshot;
    [SerializeField]
    [ArrayForEnum(typeof(AtmosChannels))]
    private bool[] isChannelEnabled;

    public AudioMixerSnapshot Snapshot
    {
	get
	{
	    return snapshot;
	}
    }

    public bool IsChannelEnabled(AtmosChannels channel)
    {
	return channel >= AtmosChannels.CaveWind && channel < (AtmosChannels)isChannelEnabled.Length && isChannelEnabled[(int)channel];
    }

}
