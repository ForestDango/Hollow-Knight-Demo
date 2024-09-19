// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Commit a TextMesh. This is so you can change multiple parameters without reconstructing the mesh repeatedly, simply use that after you have set all the different properties. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W723")]
	public class Tk2dTextMeshCommit : FsmStateAction
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
			
			DoCommit();

			Finish();
		}

		void DoCommit()
		{

			if (_textMesh == null)
			{
				LogWarning("Missing tk2dTextMesh component: " + _textMesh.gameObject.name);
				return;
			}
			
			
			_textMesh.Commit();
		}

	}
}