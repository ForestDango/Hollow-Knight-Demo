// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Begin a GUILayout area that follows the specified game object. Useful for overlays (e.g., playerName). NOTE: Block must end with a corresponding GUILayoutEndArea.")]
	public class GUILayoutBeginAreaFollowObject : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to follow.")]
		public FsmGameObject gameObject;
		
		[RequiredField]
        [Tooltip("Left screen offset.")]
        public FsmFloat offsetLeft;
		
		[RequiredField]
        [Tooltip("Screen offset up.")]
		public FsmFloat offsetTop;
		
		[RequiredField]
        [Tooltip("Width of area.")]
		public FsmFloat width;
		
		[RequiredField]
        [Tooltip("Height of area.")]
		public FsmFloat height;
		
		[Tooltip("Use normalized screen coordinates (0-1).")]
		public FsmBool normalized;
		
		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;
		
		public override void Reset()
		{
			gameObject = null;
			offsetLeft = 0f;
			offsetTop = 0f;
			width = 1f;
			height = 1f;
			normalized = true;
			style = "";
		}

		public override void OnGUI()
		{
			var go = gameObject.Value;
			
			if (go == null || Camera.main == null)
			{
				DummyBeginArea();
				return;
			}
			
			// get go position in camera space
			
			var worldPosition = go.transform.position;
			var positionInCameraSpace = Camera.main.transform.InverseTransformPoint(worldPosition);
			if (positionInCameraSpace.z < 0)
			{
				// behind camera, but still need to BeginArea()
				DummyBeginArea();
				return;
			}

			// get screen position
			
			Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
			
			var left = screenPos.x + (normalized.Value ? offsetLeft.Value * Screen.width : offsetLeft.Value);
			var top = screenPos.y + (normalized.Value ? offsetTop.Value * Screen.width : offsetTop.Value);
			
			var rect = new Rect(left, top, width.Value, height.Value);
			
			if (normalized.Value)
			{
				rect.width *= Screen.width;
				rect.height *= Screen.height;
			}
			
			// convert screen coordinates
			rect.y = Screen.height - rect.y;
			
			GUILayout.BeginArea(rect, style.Value);
		}

		static void DummyBeginArea()
		{
			GUILayout.BeginArea(new Rect());
		}
	}
}