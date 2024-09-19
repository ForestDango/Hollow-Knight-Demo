// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the font of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetFont : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[Tooltip("The font gameObject")]
		[UIHint(UIHint.FsmGameObject)]
		public FsmGameObject font;
	
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
			font = null;
		}
		
		public override void OnEnter()
		{
			_getTextMesh();
			
			DoGetFont();

			Finish();
		}

		void DoGetFont()
		{

			if (_textMesh == null)
			{
				LogWarning("Missing tk2dTextMesh component: " + _textMesh.gameObject.name);
				return;
			}
			
			GameObject go = font.Value;
			if (go == null) 
			{
				return;
			}
			
			tk2dFont _font =  go.GetComponent<tk2dFont>();
			
			
			if (_font == null)
			{
				return;
			}
			
			//unsupported yet, I don't think it's possible currently to get the font associated with a fontdata. Need to enquire with the guys.
		}
		

	}
}