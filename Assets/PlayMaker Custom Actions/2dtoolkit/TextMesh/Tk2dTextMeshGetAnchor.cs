// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the anchor of a TextMesh. \nThe anchor is stored as a string. tk2dTextMeshSetAnchor can work with this string. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetAnchor : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[Tooltip("The anchor as a string. \npossible values: LowerLeft,LowerCenter,LowerRight,MiddleLeft,MiddleCenter,MiddleRight,UpperLeft,UpperCenter or UpperRight ")]
		[UIHint(UIHint.Variable)]
		public FsmString textAnchorAsString;
	
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
			textAnchorAsString = "";
			
			everyframe = false;
		}
		
		public override void OnEnter()
		{
			_getTextMesh();
			
			DoGetAnchor();

			if (!everyframe)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			DoGetAnchor();
		}

		void DoGetAnchor()
		{

			if (_textMesh == null)
			{
				LogWarning("Missing tk2dTextMesh component");
				return;
			}
	
			textAnchorAsString.Value = _textMesh.anchor.ToString();
			
		}

	}
}