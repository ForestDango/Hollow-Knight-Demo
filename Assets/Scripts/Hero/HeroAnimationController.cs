using System;
using GlobalEnums;
using UnityEngine;

public class HeroAnimationController : MonoBehaviour
{
    private HeroController heroCtrl;
    private HeroControllerStates cState;
    private tk2dSpriteAnimator animator;
    private PlayerData pd;

    private bool wasFacingRight;
    private bool playLanding;
    private bool playRunToIdle;//播放"Run To Idle"动画片段
    private bool playDashToIdle; //播放"Dash To Idle"动画片段
    private bool playBackDashToIdleEnd; //播放"Back Dash To Idle"动画片段(其实并不会播放)

    private bool changedClipFromLastFrame;

    public ActorStates actorStates { get; private set; }
    public ActorStates prevActorStates { get; private set; }
    public ActorStates stateBeforeControl { get; private set; }
    public bool controlEnabled { get; private set; }

    private void Awake()
    {
	heroCtrl = HeroController.instance;
	cState = heroCtrl.cState;
	animator = GetComponent<tk2dSpriteAnimator>();
    }

    private void Start()
    {
	pd = PlayerData.instance;
	ResetAll();
	actorStates = heroCtrl.hero_state;
	if (!controlEnabled)
	{
	    animator.Stop();
	}
	if(heroCtrl.hero_state == ActorStates.airborne)
	{
	    animator.PlayFromFrame("Airborne", 7);
	    return;
	}
	PlayIdle();
    }

    private void Update()
    {
	if(controlEnabled)
	{
	    UpdateAnimation();
	}
	else if (cState.facingRight)
	{
	    wasFacingRight = true;
	}
	else
	{
	    wasFacingRight = false;
	}
    }

    private void UpdateAnimation()
    {
	changedClipFromLastFrame = false;
	if (playLanding)
	{
	    Play("Land");
	    animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(AnimationCompleteDelegate);
	    playLanding = false;
	}
	if (playRunToIdle)
	{
	    Play("Run To Idle");
	    animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(AnimationCompleteDelegate);
	    playRunToIdle = false;
	}
	if (playBackDashToIdleEnd)
	{
	    Play("Backdash Land 2");
	    //处理animation播放完成后的事件(其实并不会播放)
	    animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(AnimationCompleteDelegate);
	    playDashToIdle = false;
	}
	if (playDashToIdle)
	{
	    Play("Dash To Idle");
	    //处理animation播放完成后的事件
	    animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(AnimationCompleteDelegate);
	    playDashToIdle = false;
	}
	if (actorStates == ActorStates.no_input)
	{
	    //TODO:
	    if (cState.recoilFrozen)
	    {
		Play("Stun");
	    }
	    else if (cState.recoiling)
	    {
		Play("Recoil");
	    }
	}
	else if (cState.dashing)
	{
	    if (heroCtrl.dashingDown)
	    {
		Play("Dash Down");
	    }
	    else
	    {
		Play("Dash"); //通过cState.dashing判断是否播放Dash动画片段
	    }
	}
	else if (cState.backDashing)
	{
	    Play("Back Dash");
	}
	else if(cState.attacking)
	{
	    if (cState.upAttacking)
	    {
		Play("UpSlash");
	    }
	    else if (cState.downAttacking)
	    {
		Play("DownSlash");
	    }
	    else if (!cState.altAttack)
	    {
		Play("Slash");
	    }
	    else
	    {
		Play("SlashAlt");
	    }
	}
	else if (actorStates == ActorStates.idle)
	{
	    //TODO:
	    if (CanPlayIdle())
	    {
		PlayIdle();
	    }
	}
	else if (actorStates == ActorStates.running)
	{
	    if (!animator.IsPlaying("Turn"))
	    {
		if (cState.inWalkZone)
		{
		    if (!animator.IsPlaying("Walk"))
		    {
			Play("Walk");
		    }
		}
		else
		{
		    PlayRun();
		}
	    }
	}
	else if (actorStates == ActorStates.airborne)
	{
	    if (cState.jumping)
	    {
		if (!animator.IsPlaying("Airborne"))
		{
		    animator.PlayFromFrame("Airborne", 0);
		}
	    }
	    else if (cState.falling)
	    {
		if (!animator.IsPlaying("Airborne"))
		{
		    animator.PlayFromFrame("Airborne", 7);
		}
	    }
	    else if (!animator.IsPlaying("Airborne"))
	    {
		animator.PlayFromFrame("Airborne", 3);
	    }
	}
	//(其实并不会播放)
	else if (actorStates == ActorStates.dash_landing)
	{
	    animator.Play("Dash Down Land");
	}
	else if(actorStates == ActorStates.hard_landing)
	{
	    animator.Play("HardLand");
	}
	if (cState.facingRight)
	{
	    if(!wasFacingRight && cState.onGround && CanPlayTurn())
	    {
		Play("Turn");
	    }
	    wasFacingRight = true;
	}
	else
	{
	    if (wasFacingRight && cState.onGround && CanPlayTurn())
	    {
		Play("Turn");
	    }
	    wasFacingRight = false;
	}
	ResetPlays();
    }

    private void AnimationCompleteDelegate(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
    {
	if(clip.name == "Land")
	{
	    PlayIdle();
	}
	if(clip.name == "Run To Idle")
	{
	    PlayIdle();
	}
	if(clip.name == "Backdash To Idle")//(其实并不会播放)
	{
	    PlayIdle();
	}
	if(clip.name == "Dash To Idle")
	{
	    PlayIdle();
	}
    }

    private void Play(string clipName)
    {
	if(clipName != animator.CurrentClip.name)
	{
	    changedClipFromLastFrame = true;
	}
	animator.Play(clipName);
    }

    private void PlayRun()
    {
	animator.Play("Run");
    }

    public void PlayIdle()
    {
	animator.Play("Idle");
    }

    public void StopAttack()
    {
	if(animator.IsPlaying("UpSlash") || animator.IsPlaying("DownSlash"))
	{
	    animator.Stop();
	}
    }

    public void FinishedDash()
    {
	playDashToIdle = true;
    }

    private void ResetAll()
    {
	playLanding = false;
	playRunToIdle = false;
	playDashToIdle = false;
	wasFacingRight = false;
	controlEnabled = true;
    }

    private void ResetPlays()
    {
	playLanding = false;
	playRunToIdle = false;
	playDashToIdle = false;
    }

    public void UpdateState(ActorStates newState)
    {
	if(controlEnabled &&  newState != actorStates)
	{
	    if(actorStates == ActorStates.airborne && newState == ActorStates.idle && !playLanding)
	    {
		playLanding = true;
	    }
	    if(actorStates == ActorStates.running && newState == ActorStates.idle && !playRunToIdle && !cState.inWalkZone)
	    {
		playRunToIdle = true;
	    }
	    prevActorStates = actorStates;
	    actorStates = newState;
	}
    }

    public void PlayClip(string clipName)
    {
	if (controlEnabled)
	{
	    Play(clipName);
	}
    }

    public void StartControl()
    {
	actorStates = heroCtrl.hero_state;
	controlEnabled = true;
	PlayIdle();
    }

    public void StopControl()
    {
	if(controlEnabled)
	{
	    controlEnabled = false;
	    stateBeforeControl = actorStates;
	}
    }
    public void StartControlWithoutSettingState()
    {
	controlEnabled = true;
	if (stateBeforeControl == ActorStates.running && actorStates == ActorStates.running)
	{
	    actorStates = ActorStates.idle;
	}
    }

    private bool CanPlayIdle()
    {
	return !animator.IsPlaying("Land") && !animator.IsPlaying("Run To Idle") && !animator.IsPlaying("Dash To Idle") && !animator.IsPlaying("Backdash Land") && !animator.IsPlaying("Backdash Land 2") && !animator.IsPlaying("LookUpEnd") && !animator.IsPlaying("LookDownEnd") && !animator.IsPlaying("Exit Door To Idle") && !animator.IsPlaying("Wake Up Ground") && !animator.IsPlaying("Hazard Respawn");
    }
    private bool CanPlayTurn()
    {
	return !animator.IsPlaying("Wake Up Ground") && !animator.IsPlaying("Hazard Respawn"); ;
    }

    /// <summary>
    /// 获取动画的播放时间
    /// </summary>
    /// <param name="clipName"></param>
    /// <returns></returns>
    public float GetClipDuration(string clipName)
    {
	if (animator == null)
	{
	    animator = GetComponent<tk2dSpriteAnimator>();
	}
	tk2dSpriteAnimationClip clipByName = animator.GetClipByName(clipName);
	if (clipByName == null)
	{
	    Debug.LogError("HeroAnim: Could not find animation clip with the name " + clipName);
	    return -1f;
	}
	return clipByName.frames.Length / clipByName.fps;
    }

}
