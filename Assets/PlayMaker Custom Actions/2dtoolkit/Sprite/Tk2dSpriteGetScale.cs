// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Get the scale of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteGetScale : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("The scale Id")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 scale;
		
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
			scale = null;
			everyframe = false;
		}
		
		public override void OnEnter()
		{
			_getSprite();
			
			DoGetSpriteScale();
			
			if (!everyframe)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{		
			DoGetSpriteScale();
		}
		

		void DoGetSpriteScale()
		{

			if (_sprite == null)
			{
				LogWarning("Missing tk2dBaseSprite component");
				return;
			}
			
			if (_sprite.scale != scale.Value)
			{
				scale.Value = _sprite.scale;
			}
		}
		
	
	}
}