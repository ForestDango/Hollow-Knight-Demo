using System;
using GlobalEnums;
using UnityEngine;

public class HeroAnimatorController : MonoBehaviour
{
    private Animator animator;
    private AnimatorClipInfo[] info;
    private HeroController heroCtrl;
    private HeroControllerStates cState;
    private string clipName;
    private float currentClipLength;

    public ActorStates actorStates { get; private set; }
    public ActorStates prevActorState { get; private set; }

    private void Start()
    {
	animator = GetComponent<Animator>();
	heroCtrl = GetComponent<HeroController>();
	actorStates = heroCtrl.hero_state;
	PlayIdle();
    }

    private void Update()
    {
	UpdateAnimation();
    }

    private void UpdateAnimation()
    {
	//info = animator.GetCurrentAnimatorClipInfo(0);
	//currentClipLength = info[0].clip.length;
	//clipName = info[0].clip.name;
	if(actorStates == ActorStates.no_input)
	{
	    //TODO:
	}
	else if(actorStates == ActorStates.idle)
	{
	    //TODO:
	    PlayIdle();
	}
	else if(actorStates == ActorStates.running)
	{
	    PlayRun();
	}
    }

    private void PlayRun()
    {
	animator.Play("Run");
    }

    public void PlayIdle()
    {
	animator.Play("Idle");
    }

    public void UpdateState(ActorStates newState)
    {
	if(newState != actorStates)
	{
	    prevActorState = actorStates;
	    actorStates = newState;
	}
    }
}
