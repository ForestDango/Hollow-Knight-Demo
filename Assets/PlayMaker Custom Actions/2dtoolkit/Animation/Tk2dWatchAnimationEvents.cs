// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Receive animation events and animation complete event of the current animation playing. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dWatchAnimationEvents : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Trigger event defined in the clip. The event holds the following triggers infos: the eventInt, eventInfo and eventFloat properties")]
		public FsmEvent animationTriggerEvent;
		
		[Tooltip("Animation complete event. The event holds the clipId reference")]
		public FsmEvent animationCompleteEvent;
		
		
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
			animationTriggerEvent = null;
			animationCompleteEvent = null;
		}
		
		public override void OnEnter()
		{
			_getSprite();
			
			DoWatchAnimationWithEvents();		
		}

		void DoWatchAnimationWithEvents()
		{
			if (_sprite == null)
			{
				LogWarning("Missing tk2dSpriteAnimator component");
				return;
			}
		
			if (animationTriggerEvent !=null){
				_sprite.AnimationEventTriggered = AnimationEventDelegate;
			}
			if (animationCompleteEvent !=null){
				_sprite.AnimationCompleted = AnimationCompleteDelegate;
			}
		}

		void AnimationEventDelegate (tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frameNum)
		{
			tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
			Fsm.EventData.IntData = frame.eventInt;
			Fsm.EventData.StringData = frame.eventInfo;
			Fsm.EventData.FloatData = frame.eventFloat;
			Fsm.Event(animationTriggerEvent);
		}

		void AnimationCompleteDelegate (tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
		{ 
			int clipId = -1;
			tk2dSpriteAnimationClip[] clips = (sprite.Library != null) ? sprite.Library.clips : null;
			if (clips != null) {
				for (int i = 0; i < clips.Length; ++i) {
					if (clips[i] == clip) {
						clipId = i;
						break;
					}
				}
			}

			Fsm.EventData.IntData = clipId;
			Fsm.Event (animationCompleteEvent);      
		}
	}

}