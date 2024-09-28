using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Detect 2D collisions between the Owner of this FSM and other Game Objects that have RigidBody2D components.\nNOTE: The system events, COLLISION ENTER 2D, COLLISION STAY 2D, and COLLISION EXIT 2D are sent automatically on collisions with any object. Use this action to filter collisions by Tag.")]
    public class Collision2dEventLayer : FsmStateAction
    {
	[Tooltip("The type of collision to detect.")]
	public PlayMakerUnity2d.Collision2DType collision;
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
	[UIHint(UIHint.Variable)]
	[Tooltip("Store the force of the collision. NOTE: Use Get Collision Info to get more info about the collision.")]
	public FsmFloat storeForce;

	private PlayMakerUnity2DProxy _proxy;

	public override void Reset()
	{
	    collision = PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D;
	    collideTag = new FsmString
	    {
		UseVariable = true
	    };
	    sendEvent = null;
	    storeCollider = null;
	    storeForce = null;
	}

	public override void OnEnter()
	{
	    _proxy = Owner.GetComponent<PlayMakerUnity2DProxy>();
	    if(_proxy == null)
	    {
		_proxy = Owner.AddComponent<PlayMakerUnity2DProxy>();
	    }
	    switch (collision)
	    {
		case PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D:
		    _proxy.AddOnCollisionEnter2dDelegate(new PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate(DoCollisionEnter2D));
		    break;
		case PlayMakerUnity2d.Collision2DType.OnCollisionStay2D:
		    _proxy.AddOnCollisionStay2dDelegate(new PlayMakerUnity2DProxy.OnCollisionStay2dDelegate(DoCollisionStay2D));
		    break;
		case PlayMakerUnity2d.Collision2DType.OnCollisionExit2D:
		    _proxy.AddOnCollisionExit2dDelegate(new PlayMakerUnity2DProxy.OnCollisionExit2dDelegate(DoCollisionExit2D));
		    break;
		default:
		    break;
	    }
	}

	
	public override void OnExit()
	{
	    if(_proxy == null)
	    {
		return;
	    }
	    switch (collision)
	    {
		case PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D:
		    _proxy.RemoveOnCollisionEnter2dDelegate(new PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate(DoCollisionEnter2D));
		    break;
		case PlayMakerUnity2d.Collision2DType.OnCollisionStay2D:
		    _proxy.RemoveOnCollisionStay2dDelegate(new PlayMakerUnity2DProxy.OnCollisionStay2dDelegate(DoCollisionStay2D));
		    break;
		case PlayMakerUnity2d.Collision2DType.OnCollisionExit2D:
		    _proxy.RemoveOnCollisionExit2dDelegate(new PlayMakerUnity2DProxy.OnCollisionExit2dDelegate(DoCollisionExit2D));
		    break;
		default:
		    break;
	    }
	}

	public new void DoCollisionEnter2D(Collision2D collisionInfo)
	{
	    if (collision == PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D && (collisionInfo.collider.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (collisionInfo.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
	    {
		StoreCollisionInfo(collisionInfo);
		Fsm.Event(sendEvent);
	    }
	}

	public new void DoCollisionStay2D(Collision2D collisionInfo)
	{
	    if (collision == PlayMakerUnity2d.Collision2DType.OnCollisionStay2D && (collisionInfo.collider.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (collisionInfo.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
	    {
		StoreCollisionInfo(collisionInfo);
		Fsm.Event(sendEvent);
	    }
	}

	public new void DoCollisionExit2D(Collision2D collisionInfo)
	{
	    if (collision == PlayMakerUnity2d.Collision2DType.OnCollisionExit2D && (collisionInfo.collider.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (collisionInfo.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
	    {
		StoreCollisionInfo(collisionInfo);
		Fsm.Event(sendEvent);
	    }
	}

	private void StoreCollisionInfo(Collision2D collisionInfo)
	{
	    storeCollider.Value = collisionInfo.gameObject;
	    storeForce.Value = collisionInfo.relativeVelocity.magnitude;
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
