using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Detect 2D trigger collisions between the Owner of this FSM and other Game Objects that have RigidBody2D components.\nNOTE: The system events, TRIGGER ENTER 2D, TRIGGER STAY 2D, and TRIGGER EXIT 2D are sent automatically on collisions triggers with any object. Use this action to filter collision triggers by Tag.")]
    public class SendTrigger2DEvent : FsmStateAction
    {
	[Tooltip("Where to send the event.")]
	public FsmEventTarget eventTarget;
	[Tooltip("The type of trigger to detect.")]
	public PlayMakerUnity2d.Trigger2DType trigger;

	[UIHint(UIHint.Tag)]
	[Tooltip("Filter by Tag.")]
	public FsmString collideTag;
	[UIHint(UIHint.Layer)]
	[Tooltip("Filter by Layer.")]
	public FsmInt collideLayer;
	[RequiredField]
	[Tooltip("Event to send if a collision is detected.")]
	public FsmEvent sendEvent;
	[UIHint(UIHint.Variable)]
	[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
	public FsmGameObject storeCollider;

	private PlayMakerUnity2DProxy _proxy;

	public override void Reset()
	{
	    trigger = PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D;
	    collideTag = new FsmString
	    {
		UseVariable = true
	    };
	    collideLayer = new FsmInt
	    {
		UseVariable = true
	    };
	    sendEvent = null;
	    storeCollider = null;
	}

	public override void OnEnter()
	{
	    _proxy = Owner.GetComponent<PlayMakerUnity2DProxy>();
	    if (_proxy == null)
	    {
		_proxy = Owner.AddComponent<PlayMakerUnity2DProxy>();
	    }
	    switch (trigger)
	    {
		case PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D:
		    _proxy.AddOnTriggerEnter2dDelegate(new PlayMakerUnity2DProxy.OnTriggerEnter2dDelegate(DoTriggerEnter2D));
		    return;
		case PlayMakerUnity2d.Trigger2DType.OnTriggerStay2D:
		    _proxy.AddOnTriggerStay2dDelegate(new PlayMakerUnity2DProxy.OnTriggerStay2dDelegate(DoTriggerStay2D));
		    return;
		case PlayMakerUnity2d.Trigger2DType.OnTriggerExit2D:
		    _proxy.AddOnTriggerExit2dDelegate(new PlayMakerUnity2DProxy.OnTriggerExit2dDelegate(DoTriggerExit2D));
		    return;
		default:
		    return;
	    }
	}

	public override void OnExit()
	{
	    if (_proxy == null)
	    {
		return;
	    }
	    switch (trigger)
	    {
		case PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D:
		    _proxy.RemoveOnTriggerEnter2dDelegate(new PlayMakerUnity2DProxy.OnTriggerEnter2dDelegate(DoTriggerEnter2D));
		    return;
		case PlayMakerUnity2d.Trigger2DType.OnTriggerStay2D:
		    _proxy.RemoveOnTriggerStay2dDelegate(new PlayMakerUnity2DProxy.OnTriggerStay2dDelegate(DoTriggerStay2D));
		    return;
		case PlayMakerUnity2d.Trigger2DType.OnTriggerExit2D:
		    _proxy.RemoveOnTriggerExit2dDelegate(new PlayMakerUnity2DProxy.OnTriggerExit2dDelegate(DoTriggerExit2D));
		    return;
		default:
		    return;
	    }
	}
	private void StoreCollisionInfo(Collider2D collisionInfo)
	{
	    storeCollider.Value = collisionInfo.gameObject;
	}

	public new void DoTriggerEnter2D(Collider2D collisionInfo)
	{
	    if (trigger == PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D && (collisionInfo.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (collisionInfo.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
	    {
		Debug.LogFormat("FSN.event(eventTarget)");
		StoreCollisionInfo(collisionInfo);
		Fsm.Event(eventTarget, sendEvent);
	    }
	}

	public new void DoTriggerStay2D(Collider2D collisionInfo)
	{
	    if (trigger == PlayMakerUnity2d.Trigger2DType.OnTriggerStay2D && (collisionInfo.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (collisionInfo.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
	    {
		StoreCollisionInfo(collisionInfo);
		Fsm.Event(eventTarget, sendEvent);
	    }
	}

	public new void DoTriggerExit2D(Collider2D collisionInfo)
	{
	    if (trigger == PlayMakerUnity2d.Trigger2DType.OnTriggerExit2D && (collisionInfo.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (collisionInfo.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
	    {
		StoreCollisionInfo(collisionInfo);
		Fsm.Event(eventTarget, sendEvent);
	    }
	}

	public override string ErrorCheck()
	{
	    string text = string.Empty;
	    if (Owner != null && Owner.GetComponent<Collider2D>() == null && Owner.GetComponent<Rigidbody2D>() == null)
	    {
		text += "Owner requires a RigidBody2D or Collider2D!\n";
	    }
	    return text;
	}
    }
}
