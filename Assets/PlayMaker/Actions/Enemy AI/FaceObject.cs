using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Object A will flip to face Object B horizontally.")]
    public class FaceObject : FsmStateAction
    {
	//A朝着B看
	[RequiredField]
	public FsmGameObject objectA;
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmGameObject objectB;
	[Tooltip("Does object A's sprite face right?")]
	public FsmBool spriteFacesRight;
	public bool playNewAnimation;
	public FsmString newAnimationClip;
	public bool resetFrame;
	[Tooltip("Repeat every frame.")]
	public bool everyFrame;
	private float xScale;
	private FsmVector3 vector;
	private tk2dSpriteAnimator _sprite;

	public override void Reset()
	{
	    objectA = null;
	    objectB = null;
	    newAnimationClip = null;
	    spriteFacesRight = false;
	    everyFrame = false;
	    resetFrame = false;
	    playNewAnimation = false;
	}

	public override void OnEnter()
	{
	    _sprite = objectA.Value.GetComponent<tk2dSpriteAnimator>();
	    if (_sprite == null)
	    {
		Finish();
	    }
	    xScale = objectA.Value.transform.localScale.x; //xsclae= 1f;
	    if (xScale < 0f)
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
	    Vector3 localScale = objectA.Value.transform.localScale;
	    if(objectB.Value == null || objectB.IsNone)
	    {
		Finish();
	    }
	    if (objectA.Value.transform.position.x < objectB.Value.transform.position.x) //B在A的右边，A向右看
	    {
		if (spriteFacesRight.Value)
		{
		    if (localScale.x != xScale)
		    {
			localScale.x = xScale;
			if (resetFrame)
			{
			    _sprite.PlayFromFrame(0);
			}
			if (playNewAnimation)
			{
			    _sprite.Play(newAnimationClip.Value);
			}
		    }
		}
		else if (localScale.x != -xScale)
		{
		    localScale.x = -xScale;
		    if (resetFrame)
		    {
			_sprite.PlayFromFrame(0);
		    }
		    if (playNewAnimation)
		    {
			_sprite.Play(newAnimationClip.Value);
		    }
		}
	    }
	    else if (spriteFacesRight.Value)
	    {
		if (localScale.x != -xScale)
		{
		    localScale.x = -xScale;
		    if (resetFrame)
		    {
			_sprite.PlayFromFrame(0);
		    }
		    if (playNewAnimation)
		    {
			_sprite.Play(newAnimationClip.Value);
		    }
		}
	    }
	    else if (localScale.x != xScale)//B在A的左边，A向左看
	    {
		localScale.x = xScale;
		if (resetFrame)
		{
		    _sprite.PlayFromFrame(0);
		}
		if (playNewAnimation)
		{
		    _sprite.Play(newAnimationClip.Value);
		}
	    }
	    objectA.Value.transform.localScale = new Vector3(localScale.x, objectA.Value.transform.localScale.y, objectA.Value.transform.localScale.z);
	}
    }

}
