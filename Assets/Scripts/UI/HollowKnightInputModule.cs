using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace InControl
{
    [AddComponentMenu("Event/Hollow Knight Input Module")]
    public class HollowKnightInputModule : StandaloneInputModule
    {
	public HeroActions heroActions;
	public PlayerAction SubmitAction 
	{
	    get
	    {
		return InputHandler.Instance.inputActions.menuSubmit;
	    }
	    set
	    {

	    }
	}
	public PlayerAction CancelAction {
	    get
	    {
		return InputHandler.Instance.inputActions.menuCancel;
	    }
	    set
	    {

	    }
	}
	public PlayerAction JumpAction {
	    get
	    {
		return InputHandler.Instance.inputActions.jump;
	    }
	    set
	    {

	    }
	}
	public PlayerAction CastAction {
	    get
	    {
		return InputHandler.Instance.inputActions.cast;
	    }
	    set
	    {

	    }
	}
	public PlayerAction AttackAction {
	    get
	    {
		return InputHandler.Instance.inputActions.attack;
	    }
	    set
	    {

	    }
	}
	public PlayerTwoAxisAction MoveAction {
	    get
	    {
		return InputHandler.Instance.inputActions.moveVector;
	    }
	    set
	    {

	    }
	}

	[Range(0.1f, 0.9f)]
	public float analogMoveThreshold = 0.5f;
	public float moveRepeatFirstDuration = 0.8f;
	public float moveRepeatDelayDuration = 0.1f;

	[FormerlySerializedAs("allowMobileDevice")]
	public new bool forceModuleActive;
	public bool allowMouseInput = true;
	public bool focusOnMouseHover;

	private InputDevice inputDevice;
	private Vector3 thisMousePosition;
	private Vector3 lastMousePosition;
	private Vector2 thisVectorState;
	private Vector2 lastVectorState;
	private float nextMoveRepeatTime;
	private float lastVectorPressedTime;
	private TwoAxisInputControl direction;

	public HollowKnightInputModule()
	{
	    heroActions = new HeroActions();
	    direction = new TwoAxisInputControl();
	    direction.StateThreshold = analogMoveThreshold;
	}
	public override void UpdateModule()
	{
	    lastMousePosition = thisMousePosition;
	    thisMousePosition = Input.mousePosition;
	}

	public override bool IsModuleSupported()
	{
	    return forceModuleActive || Input.mousePresent;
	}

	public override bool ShouldActivateModule()
	{
	    if (!enabled || !gameObject.activeInHierarchy)
	    {
		return false;
	    }
	    UpdateInputState();
	    bool flag = false;
	    flag |= SubmitAction.WasPressed;
	    flag |= CancelAction.WasPressed;
	    flag |= JumpAction.WasPressed;
	    flag |= CastAction.WasPressed;
	    flag |= AttackAction.WasPressed;
	    flag |= VectorWasPressed;
	    if (allowMouseInput)
	    {
		flag |= MouseHasMoved;
		flag |= MouseButtonIsPressed;
	    }
	    if (Input.touchCount > 0)
	    {
		flag = true;
	    }
	    return flag;
	}

	public override void ActivateModule()
	{
	    base.ActivateModule();
	    thisMousePosition = Input.mousePosition;
	    lastMousePosition = Input.mousePosition;
	    GameObject gameObject = eventSystem.currentSelectedGameObject;
	    if (gameObject == null)
	    {
		gameObject = eventSystem.firstSelectedGameObject;
	    }
	    eventSystem.SetSelectedGameObject(gameObject, GetBaseEventData());
	}

	public override void Process()
	{
	    bool flag = SendUpdateEventToSelectedObject();
	    if (eventSystem.sendNavigationEvents)
	    {
		if (!flag)
		{
		    flag = SendVectorEventToSelectedObject();
		}
		if (!flag)
		{
		    SendButtonEventToSelectedObject();
		}
	    }
	    if (allowMouseInput)
	    {
		ProcessMouseEvent();
	    }
	}

	private bool SendButtonEventToSelectedObject()
	{
	    if (eventSystem.currentSelectedGameObject == null)
	    {
		return false;
	    }
	    if (UIManager.instance.IsFadingMenu)
	    {
		return false;
	    }
	    BaseEventData baseEventData = GetBaseEventData();
	    Platform.MenuActions menuAction = Platform.Current.GetMenuAction(SubmitAction.WasPressed, CancelAction.WasPressed, JumpAction.WasPressed, AttackAction.WasPressed, CastAction.WasPressed);
	    if (menuAction == Platform.MenuActions.Submit)
	    {
		ExecuteEvents.Execute<ISubmitHandler>(eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
	    }
	    else if (menuAction == Platform.MenuActions.Cancel)
	    {
		PlayerAction playerAction = AttackAction.WasPressed ? AttackAction : CastAction;
		if (!playerAction.WasPressed || playerAction.FindBinding(new MouseBindingSource(Mouse.LeftButton)) == null)
		{
		    ExecuteEvents.Execute<ICancelHandler>(eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
		}
	    }
	    return baseEventData.used;
	}

	private bool SendVectorEventToSelectedObject()
	{
	    if (!VectorWasPressed)
	    {
		return false;
	    }
	    AxisEventData axisEventData = GetAxisEventData(thisVectorState.x, thisVectorState.y, 0.5f);
	    if (axisEventData.moveDir != MoveDirection.None)
	    {
		if (eventSystem.currentSelectedGameObject == null)
		{
		    eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject, GetBaseEventData());
		}
		else
		{
		    ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
		}
		SetVectorRepeatTimer();
	    }
	    return axisEventData.used;
	}

	protected override void ProcessMove(PointerEventData pointerEvent)
	{
	    GameObject pointerEnter = pointerEvent.pointerEnter;
	    base.ProcessMove(pointerEvent);
	    if (focusOnMouseHover && pointerEnter != pointerEvent.pointerEnter)
	    {
		GameObject eventHandler = ExecuteEvents.GetEventHandler<ISelectHandler>(pointerEvent.pointerEnter);
		eventSystem.SetSelectedGameObject(eventHandler, pointerEvent);
	    }
	}

	private void Update()
	{
	    direction.Filter(Device.Direction, Time.deltaTime);
	}

	private void UpdateInputState()
	{
	    lastVectorState = thisVectorState;
	    thisVectorState = Vector2.zero;
	    TwoAxisInputControl twoAxisInputControl = MoveAction ?? direction;
	    if (Utility.AbsoluteIsOverThreshold(twoAxisInputControl.X, analogMoveThreshold))
	    {
		thisVectorState.x = Mathf.Sign(twoAxisInputControl.X);
	    }
	    if (Utility.AbsoluteIsOverThreshold(twoAxisInputControl.Y, analogMoveThreshold))
	    {
		thisVectorState.y = Mathf.Sign(twoAxisInputControl.Y);
	    }
	    if (VectorIsReleased)
	    {
		nextMoveRepeatTime = 0f;
	    }
	    if (VectorIsPressed)
	    {
		if (lastVectorState == Vector2.zero)
		{
		    if (Time.realtimeSinceStartup > lastVectorPressedTime + 0.1f)
		    {
			nextMoveRepeatTime = Time.realtimeSinceStartup + moveRepeatFirstDuration;
		    }
		    else
		    {
			nextMoveRepeatTime = Time.realtimeSinceStartup + moveRepeatDelayDuration;
		    }
		}
		lastVectorPressedTime = Time.realtimeSinceStartup;
	    }
	}

	public InputDevice Device
	{
	    get
	    {
		return inputDevice ?? InputManager.ActiveDevice;
	    }
	    set
	    {
		inputDevice = value;
	    }
	}

	private void SetVectorRepeatTimer()
	{
	    nextMoveRepeatTime = Mathf.Max(nextMoveRepeatTime, Time.realtimeSinceStartup + moveRepeatDelayDuration);
	}

	private bool VectorIsPressed
	{
	    get
	    {
		return thisVectorState != Vector2.zero;
	    }
	}

	private bool VectorIsReleased
	{
	    get
	    {
		return thisVectorState == Vector2.zero;
	    }
	}

	private bool VectorHasChanged
	{
	    get
	    {
		return thisVectorState != lastVectorState;
	    }
	}


	private bool VectorWasPressed
	{
	    get
	    {
		return (VectorIsPressed && Time.realtimeSinceStartup > nextMoveRepeatTime) || (VectorIsPressed && lastVectorState == Vector2.zero);
	    }
	}

	private bool MouseHasMoved
	{
	    get
	    {
		return (thisMousePosition - lastMousePosition).sqrMagnitude > 0f;
	    }
	}

	private bool MouseButtonIsPressed
	{
	    get
	    {
		return Input.GetMouseButtonDown(0);
	    }
	}
    }
}
