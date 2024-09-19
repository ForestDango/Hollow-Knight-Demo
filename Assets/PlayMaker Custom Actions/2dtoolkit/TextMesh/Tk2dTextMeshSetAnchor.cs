// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the anchor of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshSetAnchor : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("The anchor")]
		public TextAnchor textAnchor;	

		[Tooltip("The anchor as a string (text Anchor setting will be ignore if set). \npossible values ( case insensitive): LowerLeft,LowerCenter,LowerRight,MiddleLeft,MiddleCenter,MiddleRight,UpperLeft,UpperCenter or UpperRight ")]
		[UIHint(UIHint.FsmString)]
		public FsmString OrTextAnchorString;
		
		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmBool)]		
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
			textAnchor = TextAnchor.LowerLeft;
			OrTextAnchorString = "";
			commit = true;
		}
		
		public override void OnEnter()
		{
			_getTextMesh();
			
			DoSetAnchor();

			Finish();
		}

		void DoSetAnchor()
		{

			if (_textMesh == null)
			{
				LogWarning("Missing tk2dTextMesh component: " + _textMesh.gameObject.name);
				return;
			}
		
			
			bool dirty = false;
			
			TextAnchor _textAnchor = textAnchor;
			
			
			if (OrTextAnchorString.Value != "")
			{
				bool isValidString = false;
				
				TextAnchor _textAnchorfromString = getTextAnchorFromString(OrTextAnchorString.Value,out isValidString);
				
				if (isValidString)
				{
					_textAnchor = _textAnchorfromString;
				}
			}
			
			
			if (_textMesh.anchor != _textAnchor)
			{
				_textMesh.anchor = _textAnchor;
				dirty = true;
			}
			
			if (commit.Value && dirty)
			{
				_textMesh.Commit();
			}
			
		}
		
		
		public override string ErrorCheck()
		{
			if (OrTextAnchorString.Value != "")
			{
				bool isValidString = false;
				getTextAnchorFromString(OrTextAnchorString.Value,out isValidString);
				
				if (!isValidString)
				{
					return "Text Anchor string '"+OrTextAnchorString.Value+"' is not valid. Use (case insensitive): LowerLeft,LowerCenter,LowerRight,MiddleLeft,MiddleCenter,MiddleRight,UpperLeft,UpperCenter or UpperRight";
				}
			}
			
			return null;
		}
		
		
		private TextAnchor getTextAnchorFromString(string textAnchorString,out bool isValid)
		{

			
			isValid = true;
			string _textAnchorString = textAnchorString.ToLower();
			switch(_textAnchorString)
			{
				case "lowerleft":
					return TextAnchor.LowerLeft;
					
				case "lowercenter":
					return TextAnchor.LowerCenter;
					
				case "lowerright":
					return TextAnchor.LowerRight;
					
				case "middleleft":
					return TextAnchor.MiddleLeft;
					
				case "middlecenter":
					return TextAnchor.MiddleCenter;
					
				case "middleright":
					return TextAnchor.MiddleRight;
					
				case "upperleft":
					return TextAnchor.UpperLeft;
				
				case "uppercenter":
					return TextAnchor.UpperCenter;
				
				case "upperright":
					return TextAnchor.UpperRight;
					
				default:
					isValid = false;
					break;
			}
			
			return TextAnchor.LowerLeft;
		}

	}
}