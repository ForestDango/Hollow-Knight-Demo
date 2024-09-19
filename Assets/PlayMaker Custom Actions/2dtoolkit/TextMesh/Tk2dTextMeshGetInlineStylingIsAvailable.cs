// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Check that inline styling can indeed be used ( the font needs to have texture gradients for inline styling to work). \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetInlineStylingIsAvailable : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("Is inline styling available? true if inlineStyling is true AND the font texturGradients is true")]
		[UIHint(UIHint.Variable)]
		public FsmBool InlineStylingAvailable;
		
		[ActionSection("")] 
		
		[Tooltip("Repeat every frame.")]
		public bool everyframe;
		
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
			InlineStylingAvailable = null;
			
			everyframe = false;
		}
		
		public override void OnEnter()
		{
			_getTextMesh();
			
			DoGetInlineStylingAvailable();
			
			if (!everyframe)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			DoGetInlineStylingAvailable();	
		}

		void DoGetInlineStylingAvailable()
		{

			if (_textMesh == null)
			{
				LogWarning("Missing tk2dTextMesh component: ");
				return;
			}
			InlineStylingAvailable.Value = _textMesh.inlineStyling && _textMesh.font.textureGradients;
			
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