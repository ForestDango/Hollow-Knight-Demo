using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Audio)]
    [ActionTarget(typeof(MusicCue), "musicCue", false)]
    [Tooltip("Plays music cues.")]
    public class ApplyMusicCue : FsmStateAction
    {
	[Tooltip("Music cue to play.")]
	[ObjectType(typeof(MusicCue))]
	public FsmObject musicCue;

	[Tooltip("Delay before starting transition")]
	public FsmFloat delayTime;

	[Tooltip("Transition duration.")]
	public FsmFloat transitionTime;
	public override void Reset()
	{
	    musicCue = null;
	    delayTime = 0f;
	    transitionTime = 0f;
	}
	public override void OnEnter()
	{
	    MusicCue x = musicCue.Value as MusicCue;
	    GameManager instance = GameManager.instance;
	    if (!(x == null))
	    {
		if (instance == null)
		{
		    Debug.LogErrorFormat(Owner, "Failed to play music cue, because the game manager is not ready", Array.Empty<object>());
		}
		else
		{
		    instance.AudioManager.ApplyMusicCue(x, delayTime.Value, transitionTime.Value, false);
		}
	    }
	    Finish();
	}


    }

}
