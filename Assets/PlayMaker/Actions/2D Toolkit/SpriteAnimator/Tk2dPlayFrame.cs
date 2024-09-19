using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("2D Toolkit/SpriteAnimator")]
    [Tooltip("Goto a specific frame for current animation.")]
    public class Tk2dPlayFrame : FsmStateAction
    {
	[RequiredField]
	[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
	[CheckForComponent(typeof(tk2dSpriteAnimator))]
	public FsmOwnerDefault gameObject;

	[RequiredField]
	public FsmInt frame;

	private tk2dSpriteAnimator _sprite;

	public override void Reset()
	{
	    gameObject = null;
	    frame = 0;
	}

	public override void OnEnter()
	{
	    _getSprite();
	    if (_sprite)
	    {
		_sprite.PlayFromFrame(frame.Value);
	    }
	    else
	    {
		Debug.LogWarning("No tk2d sprite animator found on " + Owner.name);
	    }
	    Finish();
	}

	private void _getSprite()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (ownerDefaultTarget == null)
		return;
	    _sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
	}
    }

}
