using System;
using System.Collections.Generic;
using System.IO;
using InControl;
using UnityEngine;

public class DesktopPlatform : Platform
{
    public override bool IsControllerImplicit
    {
	get
	{
	    return InputHandler.Instance && InputHandler.Instance.lastActiveController == BindingSourceType.DeviceBindingSource;
	}
    }

    public override AcceptRejectInputStyles AcceptRejectInputStyle
    {
	get
	{
	    return AcceptRejectInputStyles.NonJapaneseStyle;
	}
    }

    public override void EnsureSaveSlotSpace(int slotIndex, Action<bool> callback)
    {
	CoreLoop.InvokeNext(delegate
	{
	    Action<bool> callback2 = callback;
	    if (callback2 == null)
	    {
		return;
	    }
	    callback2(true);
	});
    }
}
