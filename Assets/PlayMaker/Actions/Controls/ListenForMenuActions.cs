using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Controls")]
    [Tooltip("Listens for menu actions, and safely disambiguates jump/submit/attack/cancel/cast.")]
    public class ListenForMenuActions : FsmStateAction
    {
	public FsmEventTarget eventTarget;
	public FsmEvent submitPressed;
	public FsmEvent cancelPressed;
	public FsmBool ignoreAttack;

	private GameManager gm;
	private InputHandler inputHandler;

	public override void Reset()
	{
	    submitPressed = null;
	    cancelPressed = null;
	    eventTarget = null;
	}

	public override void OnEnter()
	{
	    gm = GameManager.instance;
	    if(gm == null)
	    {
		LogError("Cannot listen for buttons without game manager.");
		return;
	    }
	    inputHandler = gm.inputHandler;
	    if(inputHandler == null)
	    {
		LogError("Cannot listen for buttons without input handler.");
	    }
	}
	public override void OnUpdate()
	{
	    if(gm != null && !gm.isPaused && inputHandler != null)
	    {
		HeroActions inputActions = inputHandler.inputActions;
		bool attackInput = ignoreAttack.Value && inputActions.attack.WasPressed;
		Platform.MenuActions menuActions = Platform.Current.GetMenuAction(inputActions.menuSubmit.WasPressed, inputActions.menuCancel.WasPressed, inputActions.jump.WasPressed, attackInput, inputActions.cast.WasPressed);
		if(menuActions == Platform.MenuActions.Submit)
		{
		    Fsm.Event(eventTarget, submitPressed);
		    return;
		}
		if (menuActions == Platform.MenuActions.Cancel)
		{
		    Fsm.Event(eventTarget, cancelPressed);
		}
	    }
	}


    }
}
