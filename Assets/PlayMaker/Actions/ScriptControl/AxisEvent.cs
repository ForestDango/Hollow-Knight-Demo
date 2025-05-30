// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

// NOTE: The new Input System and legacy Input Manager can both be enabled in a project.
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
#define NEW_INPUT_SYSTEM_ONLY
#endif

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
#if NEW_INPUT_SYSTEM_ONLY
    [Obsolete("This action is not supported in the new Input System. " +
              "Use PlayerInputGetVector2 or GamepadStickEvents instead.")]
#endif
    [ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends events based on the direction of Input Axis (Left/Right/Up/Down...).")]
	public class AxisEvent : FsmStateAction
	{
		[Tooltip("Horizontal axis as defined in the Input Manager")]
		public FsmString horizontalAxis;
		
		[Tooltip("Vertical axis as defined in the Input Manager")]
		public FsmString verticalAxis;
		
		[Tooltip("Event to send if input is to the left.")]
		public FsmEvent leftEvent;
		
		[Tooltip("Event to send if input is to the right.")]
		public FsmEvent rightEvent;
		
		[Tooltip("Event to send if input is to the up.")]
		public FsmEvent upEvent;
		
		[Tooltip("Event to send if input is to the down.")]
		public FsmEvent downEvent;
		
		[Tooltip("Event to send if input is in any direction.")]
		public FsmEvent anyDirection;
		
		[Tooltip("Event to send if no axis input (centered).")]
		public FsmEvent noDirection;
		
		public override void Reset()
		{
			horizontalAxis = "Horizontal";
			verticalAxis = "Vertical";
			leftEvent = null;
			rightEvent = null;
			upEvent = null;
			downEvent = null;
			anyDirection = null;
			noDirection = null;
		}

		public override void OnUpdate()
		{
            // get axes offsets

#if NEW_INPUT_SYSTEM_ONLY
            var x = 0f;
            var y = 0f;
#else
            var x = horizontalAxis.Value != "" ? Input.GetAxis(horizontalAxis.Value) : 0;
			var y = verticalAxis.Value != "" ? Input.GetAxis(verticalAxis.Value) : 0;
#endif           
            // get squared offset from center

            var offset = (x * x) + (y * y);
			
			// no offset?
			
			if (offset.Equals(0))
			{
				if (noDirection != null)
				{
					Fsm.Event(noDirection);
				}
				return;
			}
			
			// get integer direction sector (4 directions)
			// 8 directions? or new action?
			
			var angle = (Mathf.Atan2(y, x) * Mathf.Rad2Deg) + 45f;
			if (angle < 0f) 
			{
				angle += 360f;
			}
			
			var direction = (int)(angle / 90f);
			
			// send events bases on direction
			
			if (direction == 0 && rightEvent != null)
			{
				Fsm.Event(rightEvent);
				//Debug.Log("Right");
			} 
			else if (direction == 1 && upEvent != null)
			{
				Fsm.Event(upEvent);
				//Debug.Log("Up");
			}
			else if (direction == 2 && leftEvent != null)
			{
				Fsm.Event(leftEvent);
				//Debug.Log("Left");
			}			
			else if (direction == 3 && downEvent != null)
			{
				Fsm.Event(downEvent);
				//Debug.Log("Down");
			}
			else if (anyDirection != null)
			{
				// since we already no offset > 0
				
				Fsm.Event(anyDirection);
				//Debug.Log("AnyDirection");
			}
		}


#if NEW_INPUT_SYSTEM_ONLY

        public override string ErrorCheck()
        {
            return "This action is not supported in the new Input System. " +
                   "Use PlayerInputGetVector2 or GamepadStickEvents instead.";
        }
#endif
    }
}

