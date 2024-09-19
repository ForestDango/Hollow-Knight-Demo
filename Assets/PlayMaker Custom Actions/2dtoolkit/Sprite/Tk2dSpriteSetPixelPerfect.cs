// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Set the pixel perfect flag of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteSetPixelPerfect : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Does the sprite needs to be kept pixelPerfect? This is only necessary when using a perspective camera.")]
		[UIHint(UIHint.FsmBool)]
		public FsmBool pixelPerfect;

		[ActionSection("")] 
		
		[Tooltip("Repeat every frame.")]
		public bool everyframe;
		
		
		private tk2dBaseSprite _sprite;
		
		private void _getSprite()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) 
			{
				return;
			}
			
			_sprite =  go.GetComponent<tk2dBaseSprite>();
		}
		
				
		public override void Reset()
		{
			gameObject = null;
			pixelPerfect = null;
			
			everyframe = false;
		}
		
		public override void OnEnter()
		{
			_getSprite();
			
			DoSetSpritePixelPerfect();
			
			if (!everyframe)
			{
				Finish();
			}

		}
		
		public override void OnUpdate()
		{			
			DoSetSpritePixelPerfect();

		}
		
		void DoSetSpritePixelPerfect()
		{

			if (_sprite == null)
			{
				LogWarning("Missing tk2dBaseSprite component");
				return;
			}
			
			if (pixelPerfect.Value)
			{
				_sprite.MakePixelPerfect();
			}
		}
		
	
	}
}