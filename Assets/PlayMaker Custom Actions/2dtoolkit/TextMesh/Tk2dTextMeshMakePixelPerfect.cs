// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Make a TextMesh pixelPerfect. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshMakePixelPerfect : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

	
		private tk2dTextMesh _textMesh;
		
		private void _getTextMesh()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) 
			{
				return;
			}
			
			_textMesh =  go.GetComponent<tk2dTextMesh>();
		}
		
				
		public override void Reset()
		{
			gameObject = null;
		}
		
		public override void OnEnter()
		{
			_getTextMesh();
			
			MakePixelPerfect();
			
			Finish();

		}

		void MakePixelPerfect()
		{

			if (_textMesh == null)
			{
				LogWarning("Missing tk2dTextMesh component ");
				return;
			}

			_textMesh.MakePixelPerfect();

			
		}
		
		/*
		public override string ErrorCheck()
		{
			if (ORSpriteName.Value != "")
			{
			//	id = _sprite.GetSpriteIdByName(ORSpriteName.Value);
			}
			
		}
		*/

	}
}