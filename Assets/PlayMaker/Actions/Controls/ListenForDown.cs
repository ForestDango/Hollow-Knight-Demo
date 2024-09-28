using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory("Controls")]
    [Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
    public class ListenForDown : FsmStateAction
    {

	[Tooltip("Where to send the event.")]
	public FsmEventTarget eventTarget;
	public FsmEvent wasPressed;
	public FsmEvent wasReleased;
	public FsmEvent isPressed;
	public FsmEvent isNotPressed;

	[UIHint(UIHint.Variable)]
	public FsmBool isPressedBool;

	public bool stateEntryOnly;

	private GameManager gm;
	private InputHandler inputHandler;

	public override void Reset()
	{
	    eventTarget = null;
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
	    if (!gm.isPaused)
	    {
		if (inputHandler.inputActions.down.WasPressed)
		{
		    Fsm.Event(wasPressed);
		}
		if (inputHandler.inputActions.down.WasReleased)
		{
		    Fsm.Event(wasReleased);
		}
		if (inputHandler.inputActions.down.IsPressed)
		{
		    Fsm.Event(isPressed);
		    if (!isPressedBool.IsNone)
		    {
			isPressedBool.Value = true;
		    }
		}
		if (!inputHandler.inputActions.down.IsPressed)
		{
		    Fsm.Event(isNotPressed);
		    if (!isPressedBool.IsNone)
		    {
			isPressedBool.Value = false;
		    }
		}
	    }
	}
    }
}
