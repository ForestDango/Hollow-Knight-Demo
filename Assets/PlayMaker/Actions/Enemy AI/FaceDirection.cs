using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Object will flip to face the direction it is moving on X Axis.")]
    public class FaceDirection : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;

	[Tooltip("Does the target's sprite face right?")]
	public FsmBool spriteFacesRight;
	public bool playNewAnimation;
	public FsmString newAnimationClip;
	public bool everyFrame;
	public bool pauseBetweenTurns; //是否播放转身动画时暂停
	public FsmFloat pauseTime;
	private FsmGameObject target;
	private tk2dSpriteAnimator _sprite;
	private float xScale;

	private float pauseTimer;

	public override void Reset()
	{
	    gameObject = null;
	    spriteFacesRight = false;
	    everyFrame = false;
	    playNewAnimation = false;
	    newAnimationClip = null;
	}

	public override void OnEnter()
	{
	    CacheRigidBody2d(Fsm.GetOwnerDefaultTarget(gameObject));
	    target = Fsm.GetOwnerDefaultTarget(gameObject);
	    _sprite = target.Value.GetComponent<tk2dSpriteAnimator>();
	    xScale = target.Value.transform.localScale.x;
	    if(xScale < 0f)
	    {
		xScale *= -1f;
	    }
	    DoFace();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	public override void OnUpdate()
	{
	    DoFace();
	}

	private void DoFace()
	{
	    if (rb2d == null)
		return;
	    Vector2 velocity = rb2d.velocity;
	    Vector3 localScale = target.Value.transform.localScale; 
	    float x = velocity.x;
	    if(pauseTimer <= 0f || !pauseBetweenTurns)
	    {
		if(x > 0f)
		{
		    if (spriteFacesRight.Value)
		    {
			if(localScale.x != xScale)
			{
			    pauseTimer = pauseTime.Value;
			    localScale.x = xScale;
			    if (playNewAnimation)
			    {
				_sprite.Play(newAnimationClip.Value);
				_sprite.PlayFromFrame(0);
			    }
			}
		    }
		    else if(localScale.x != -xScale)
		    {
			pauseTimer = pauseTime.Value;
			localScale.x = -xScale;
			if (playNewAnimation)
			{
			    _sprite.Play(newAnimationClip.Value);
			    _sprite.PlayFromFrame(0);
			}
		    }
		}
		else if(x <= 0f)
		{
		    if (spriteFacesRight.Value)
		    {
			if (localScale.x != -xScale)
			{
			    pauseTimer = pauseTime.Value;
			    localScale.x = -xScale;
			    if (playNewAnimation)
			    {
				_sprite.Play(newAnimationClip.Value);
				_sprite.PlayFromFrame(0);
			    }
			}
		    }
		    else if (localScale.x != xScale)
		    {
			pauseTimer = pauseTime.Value;
			localScale.x = xScale;
			if (playNewAnimation)
			{
			    _sprite.Play(newAnimationClip.Value);
			    _sprite.PlayFromFrame(0);
			}
		    }
		}
	    }
	    else
	    {
		pauseTimer -= Time.deltaTime;//开始计时
	    }
	    target.Value.transform.localScale = new Vector3(localScale.x, target.Value.transform.localScale.y, target.Value.transform.localScale.z);
	}
    }
}
