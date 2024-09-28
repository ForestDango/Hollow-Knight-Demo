using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Controls")]
    [Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
    public class ListenForCast : FsmStateAction
    {
	[Tooltip("Where to send the event.")]
	public FsmEventTarget eventTarget;
	public FsmEvent wasPressed;
	public FsmEvent wasReleased;
	public FsmEvent isPressed;
	public FsmEvent isNotPressed;
	public FsmBool activeBool;
	public bool stateEntryOnly;

	private GameManager gm;
	private InputHandler inputHandler;

	public override void Reset()
	{
	    eventTarget = null;
	    activeBool = new FsmBool
	    {
		UseVariable = true
	    };
	}

	public override void OnEnter()
	{
	    gm = GameManager.instance;
	    inputHandler = gm.GetComponent<InputHandler>();
	    CheckForInput();
	    if (stateEntryOnly)
	    {
		Finish();
	    }
	}

	
	public override void OnUpdate()
	{
	    CheckForInput();
	}

	private void CheckForInput()
	{
	    if (!gm.isPaused && (activeBool.IsNone || activeBool.Value))
	    {
		if (inputHandler.inputActions.cast.WasPressed)
		{
		    Fsm.Event(wasPressed);
		}
		if (inputHandler.inputActions.cast.WasReleased)
		{
		    Fsm.Event(wasReleased);
		}
		if (inputHandler.inputActions.cast.IsPressed)
		{
		    Fsm.Event(isPressed);
		}
		if (!inputHandler.inputActions.cast.IsPressed)
		{
		    Fsm.Event(isNotPressed);
		}
	    }
	}
    }

}
