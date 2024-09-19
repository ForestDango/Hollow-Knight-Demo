using System;
using GlobalEnums;
using UnityEngine;

public class HeroAnimationController : MonoBehaviour
{

    private HeroController heroCtrl;
    private HeroControllerStates cState;
    private tk2dSpriteAnimator animator;

    public ActorStates actorStates { get; private set; }
    public ActorStates prevActorStates { get; private set; }

    private void Awake()
    {
	heroCtrl = HeroController.instance;
	cState = heroCtrl.cState;
	animator = GetComponent<tk2dSpriteAnimator>();
    }

    private void Start()
    {
	actorStates = heroCtrl.hero_state;
	PlayIdle();
    }

    private void Update()
    {
	UpdateAnimation();
    }

    private void UpdateAnimation()
    {
	if (actorStates == ActorStates.no_input)
	{
	    //TODO:
	}
	else if (actorStates == ActorStates.idle)
	{
	    //TODO:
	    PlayIdle();
	}
	else if (actorStates == ActorStates.running)
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
	prevActorStates = actorStates;
	actorStates = newState;
    }

}
