// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Stops a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dStopAnimation : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		
		private tk2dSpriteAnimator _sprite;
		
		private void _getSprite()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) 
			{
				return;
			}
			
			_sprite =  go.GetComponent<tk2dSpriteAnimator>();
		}
		
				
		public override void Reset()
		{
			gameObject = null;
		}
		
		public override void OnEnter()
		{
			_getSprite();
			
			DoStopAnimation();
			
			Finish();
		}

		void DoStopAnimation()
		{

			if (_sprite == null)
			{
				LogWarning("Missing tk2dSpriteAnimator component: " + _sprite.gameObject.name);
				return;
			}

			_sprite.Stop();	
		}

	}
}