// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Get the sprite id of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
	public class Tk2dSpriteGetId : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("The sprite Id")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt spriteID;
		
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
			spriteID = null;
			everyframe = false;
		}
		
		public override void OnEnter()
		{
			_getSprite();
			
			DoGetSpriteID();
			
			if (!everyframe)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{		
			DoGetSpriteID();
		}
		
		void DoGetSpriteID()
		{

			if (_sprite == null)
			{
				LogWarning("Missing tk2dBaseSprite component");
				return;
			}
			
			if (spriteID.Value != _sprite.spriteId)
			{
				spriteID.Value = _sprite.spriteId;	
			}
		}
	}
}