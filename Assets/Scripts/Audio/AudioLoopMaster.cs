using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoopMaster : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioSource action;
    public AudioSource sub;
    public AudioSource mainAlt;
    public AudioSource tension;
    public AudioSource extra;

    private bool reset;
    private bool syncAction;
    private bool syncSub;
    private bool syncMainAlt;
    private bool syncTension;
    private bool syncExtra;

    private void Start()
    {
	audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
	float time = audioSource.time;
	if(time >= 0f && time <= 2f && !reset)
	{
	    int timeSamples = audioSource.timeSamples;
	    if (syncAction)
	    {
		action.timeSamples = timeSamples;
	    }
	    if (syncSub)
	    {
		sub.timeSamples = timeSamples;
	    }
	    if (syncMainAlt)
	    {
		mainAlt.timeSamples = timeSamples;
	    }
	    if (syncTension)
	    {
		tension.timeSamples = timeSamples;
	    }
	    if (syncExtra)
	    {
		extra.timeSamples = timeSamples;
	    }
	    reset = true;
	}
	if(time > 1f && reset)
	{
	    reset = false;
	}
    }

    public void SetSyncAction(bool set)
    {
	syncAction = set;
    }

    public void SetSyncSub(bool set)
    {
	syncSub = set;
    }

    public void SetSyncMainAlt(bool set)
    {
	syncMainAlt = set;
    }

    public void SetSyncTension(bool set)
    {
	syncTension = set;
    }

    public void SetSyncExtra(bool set)
    {
	syncExtra = set;
    }

}
