using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory("Controls")]
    [Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
    public class ListenForAttack : FsmStateAction
    {
	[Tooltip("Where to send the event.")]
	public FsmEventTarget eventTarget;
	public FsmEvent wasPressed;
	public FsmEvent wasReleased;
	public FsmEvent isPressed;
	public FsmEvent isNotPressed;

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
	}

	public override void OnUpdate()
	{
	    if (!gm.isPaused)
	    {
		if (inputHandler.inputActions.attack.WasPressed)
		{
		    Fsm.Event(wasPressed);
		}
		if (inputHandler.inputActions.attack.WasReleased)
		{
		    Fsm.Event(wasReleased);
		}
		if (inputHandler.inputActions.attack.IsPressed)
		{
		    Fsm.Event(isPressed);
		}
		if (!inputHandler.inputActions.attack.IsPressed)
		{
		    Fsm.Event(isNotPressed);
		}
	    }
	}


    }

}
