// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the colors of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetColors : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;
		

		[Tooltip("Main color")]
		[UIHint(UIHint.Variable)]
		public FsmColor mainColor;
		
		[Tooltip("Gradient color. Only used if gradient is true")]
		[UIHint(UIHint.Variable)]
		public FsmColor gradientColor;	
		
		[Tooltip("Use gradient.")]
		[UIHint(UIHint.Variable)]		
		public FsmBool useGradient;		
		
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
			mainColor = null;
			gradientColor = null;
			useGradient = false;

			everyframe = false;
		}
		
		public override void OnEnter()
		{
			_getTextMesh();
			
			DoGetColors();
			
			if (!everyframe)
			{
				Finish();
			}
		}
		public override void OnUpdate()
		{
			DoGetColors();
		}

		void DoGetColors()
		{

			if (_textMesh == null)
			{
				LogWarning("Missing tk2dTextMesh component: ");
				return;
			}
			
			useGradient.Value =_textMesh.useGradient;

			mainColor.Value =_textMesh.color ;
	
			gradientColor.Value = _textMesh.color2;

		}
		
	}
}