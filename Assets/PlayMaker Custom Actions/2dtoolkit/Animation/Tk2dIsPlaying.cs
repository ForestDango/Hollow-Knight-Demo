// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Check if a sprite animation is playing. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W720")]
	public class Tk2dIsPlaying : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[Tooltip("The clip name to play")]
		public FsmString clipName;
		
		[Tooltip("is the clip playing?")]
		[UIHint(UIHint.Variable)]
		public FsmBool isPlaying;
		
		[Tooltip("EVvnt sent if clip is playing")]
		public FsmEvent isPlayingEvent;
	
		[Tooltip("Event sent if clip is not playing")]
		public FsmEvent isNotPlayingEvent;
		
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
			clipName = null;
			everyframe = false;
			
			isPlayingEvent = null;
			isNotPlayingEvent = null;

		}
		
		public override void OnEnter()
		{
			_getSprite();
			
			DoIsPlaying();
			
			if (!everyframe)
			{
				Finish();
			}
			
		}
		public override void OnUpdate()
		{
			DoIsPlaying();
		}
		

		void DoIsPlaying()
		{

			if (_sprite == null)
			{
				LogWarning("Missing tk2dSpriteAnimator component: " + _sprite.gameObject.name);
				return;
			}
			
			bool _isPlaying =_sprite.IsPlaying(clipName.Value);
			isPlaying.Value = _isPlaying;
			
			if (_isPlaying)
			{
				Fsm.Event(isPlayingEvent);
			}else{
				Fsm.Event(isNotPlayingEvent);
			}

			
		}

	}
}