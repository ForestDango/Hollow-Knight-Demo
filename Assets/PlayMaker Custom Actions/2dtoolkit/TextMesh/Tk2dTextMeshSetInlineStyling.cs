// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the inlineStyling flag of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshSetInlineStyling : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;
		

		[Tooltip("Does the text features inline styling?")]
		[UIHint(UIHint.FsmBool)]
		public FsmBool inlineStyling;
		
		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmString)]		
		public FsmBool commit;

	
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
			inlineStyling = true;
			commit = true;
		}
		
		public override void OnEnter()
		{
			_getTextMesh();
			
			DoSetInlineStyling();
			
			Finish();

		}

		void DoSetInlineStyling()
		{

			if (_textMesh == null)
			{
				LogWarning("Missing tk2dTextMesh component: ");
				return;
			}
			

			if (_textMesh.inlineStyling != inlineStyling.Value)
			{
				_textMesh.inlineStyling = inlineStyling.Value;
				if (commit.Value)
				{
					_textMesh.Commit();
				}
				
			}
			
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