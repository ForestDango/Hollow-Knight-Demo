using System;
using UnityEngine;

public class HUDCamera : MonoBehaviour
{
    private GameCameras gc;
    private InputHandler ih;
    private bool shouldEnablePause;

    private void OnEnable()
    {
	if (!gc)
	{
	    gc = GameCameras.instance;
	}
	if (!ih)
	{
	    ih = GameManager.instance.inputHandler;
	}
	if (ih.pauseAllowed)
	{
	    shouldEnablePause = true;
	    ih.PreventPause();
	}
	else
	{
	    shouldEnablePause = false;
	}
	Invoke("MoveMenuToHUDCamera", 0.5f);
    }

    private void MoveMenuToHUDCamera()
    {
	gc.MoveMenuToHUDCamera();
	if (shouldEnablePause)
	{
	    ih.AllowPause();
	    shouldEnablePause = false;
	}
    }
}
