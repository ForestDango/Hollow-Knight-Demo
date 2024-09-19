// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Set the sprite id of a sprite. Can use id or name. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteSetId : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("The sprite Id")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt spriteID;
		
		[Tooltip("OR The sprite name ")]
		[UIHint(UIHint.FsmString)]
		public FsmString ORSpriteName;
		
		[CheckForComponent(typeof(tk2dSpriteCollection))]
		public FsmGameObject spriteCollection;
		
		
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
			ORSpriteName = null;
			
			spriteCollection = new FsmGameObject(){UseVariable=true};
			
		}
		
		public override void OnEnter()
		{
			_getSprite();
			
			DoSetSpriteID();
			
			Finish();
		}

		void DoSetSpriteID()
		{

			if (_sprite == null)
			{
				LogWarning("Missing tk2dBaseSprite component: " + _sprite.gameObject.name);
				return;
			}
			
			tk2dSpriteCollectionData _collectionData = _sprite.Collection;


			if (!spriteCollection.IsNone)
			{
				GameObject _goCol = spriteCollection.Value;
				if (_goCol!=null)
				{
					tk2dSpriteCollection _col = _goCol.GetComponent<tk2dSpriteCollection>();
					if (_col!=null)
					{
						_collectionData = _col.spriteCollection;

					}
				}
			}

			
			int id = spriteID.Value;
					
			if (ORSpriteName.Value != "")
			{

				_sprite.SetSprite(_collectionData,ORSpriteName.Value);

			}else if (id!=_sprite.spriteId)
			{
				_sprite.SetSprite(_collectionData,id);
			}
		}
		
	
	}
}