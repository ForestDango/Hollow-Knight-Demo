// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the number of drawn characters of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetNumDrawnCharacters : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[Tooltip("The number of drawn characters")]
		[UIHint(UIHint.Variable)]
		public FsmInt numDrawnCharacters;
		
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
			numDrawnCharacters = null;
			
			everyframe = false;
		}
		
		public override void OnEnter()
		{
			_getTextMesh();
			
			DoGetNumDrawnCharacters();
			
			if (!everyframe)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			DoGetNumDrawnCharacters();
		}
		

		void DoGetNumDrawnCharacters()
		{

			if (_textMesh == null)
			{
				LogWarning("Missing tk2dTextMesh component");
				return;
			}
			
			numDrawnCharacters.Value =_textMesh.NumDrawnCharacters();
		}
		
	}
}