// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Pause a sprite animation. Can work everyframe to pause resume animation on the fly. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W720")]
	public class Tk2dPauseAnimation : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Pause flag")]
		public FsmBool pause;
		
		[ActionSection("")] 
		
		[Tooltip("Repeat every frame.")]
		public bool everyframe;
		
		
		
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
			pause = true;
			everyframe = false;

		}
		
		public override void OnEnter()
		{
			_getSprite();
			
			DoPauseAnimation();
			
			if (!everyframe)
			{
				Finish();
			}
			
		}
		public override void OnUpdate()
		{
			DoPauseAnimation();
		}
		

		void DoPauseAnimation()
		{

			if (_sprite == null)
			{
				LogWarning("Missing tk2dSpriteAnimator component: " + _sprite.gameObject.name);
				return;
			}
			if (_sprite.Paused != pause.Value){
				if (pause.Value){
					_sprite.Pause();
				}else{
					_sprite.Resume();
				}
			}

			
		}

	}
}