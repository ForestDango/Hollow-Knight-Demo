using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Combat")]
    [Tooltip("Detect 2D entry collisions or triggers between the Owner of this FSM and other Game Objects that have a Damager FSM.")]
    public class ReceivedDamage : FsmStateAction
    {
	[UIHint(UIHint.Tag)]
	[Tooltip("Filter by Tag.")]
	public FsmString collideTag;
	[RequiredField]
	[Tooltip("Event to send if a collision is detected.")]
	public FsmEvent sendEvent;
	[Tooltip("Name of FSM to look for on colliding object.")]
	public FsmString fsmName;
	[UIHint(UIHint.Variable)]
	[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
	public FsmGameObject storeGameObject;
	[Tooltip("Ignore damage from Acid")]
	public FsmBool ignoreAcid;
	[Tooltip("Ignore damage from Water")]
	public FsmBool ignoreWater;

	private PlayMakerUnity2DProxy _proxy;

	public override void Reset()
	{
	    collideTag = new FsmString
	    {
		UseVariable = true
	    };
	    sendEvent = null;
	    storeGameObject = null;
	}
	public override void OnEnter()
	{
	    _proxy = Owner.GetComponent<PlayMakerUnity2DProxy>();
	    if (_proxy == null)
	    {
		_proxy = Owner.AddComponent<PlayMakerUnity2DProxy>();
	    }
	    _proxy.AddOnCollisionEnter2dDelegate(new PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate(DoCollisionEnter2D));
	    _proxy.AddOnTriggerEnter2dDelegate(new PlayMakerUnity2DProxy.OnTriggerEnter2dDelegate(DoTriggerEnter2D));
	    _proxy.AddOnTriggerStay2dDelegate(new PlayMakerUnity2DProxy.OnTriggerStay2dDelegate(DoTriggerStay2D));
	}


	public override void OnExit()
	{
	    if (_proxy == null)
		return;
	    _proxy.RemoveOnCollisionEnter2dDelegate(new PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate(DoCollisionEnter2D));
	    _proxy.RemoveOnTriggerEnter2dDelegate(new PlayMakerUnity2DProxy.OnTriggerEnter2dDelegate(DoTriggerEnter2D));
	    _proxy.RemoveOnTriggerStay2dDelegate(new PlayMakerUnity2DProxy.OnTriggerStay2dDelegate(DoTriggerStay2D));
	}

	public new void DoCollisionEnter2D(Collision2D collisionInfo)
	{
	    if ((collisionInfo.collider.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (!ignoreAcid.Value || collisionInfo.gameObject.tag != "Acid") && (!ignoreWater.Value || collisionInfo.gameObject.tag != "Water Surface"))
	    {
		StoreCollisionInfo(collisionInfo);
	    }
	}

	public new void DoTriggerEnter2D(Collider2D collisionInfo)
	{
	    if ((collisionInfo.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (!ignoreAcid.Value || collisionInfo.gameObject.tag != "Acid") && (!ignoreWater.Value || collisionInfo.gameObject.tag != "Water Surface"))
	    {
		StoreTriggerInfo(collisionInfo);
	    }
	}
	public new void DoTriggerStay2D(Collider2D collisionInfo)
	{
	    if ((collisionInfo.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (!ignoreAcid.Value || collisionInfo.gameObject.tag != "Acid") && (!ignoreWater.Value || collisionInfo.gameObject.tag != "Water Surface"))
	    {
		StoreTriggerInfo(collisionInfo);
	    }
	}

	private void StoreCollisionInfo(Collision2D collisionInfo)
	{
	    storeGameObject.Value = collisionInfo.gameObject;
	    StoreIfDamagingObject(collisionInfo.gameObject);
	}
	private void StoreTriggerInfo(Collider2D collisionInfo)
	{
	    storeGameObject.Value = collisionInfo.gameObject;
	    StoreIfDamagingObject(collisionInfo.gameObject);
	}

	private void StoreIfDamagingObject(GameObject go)
	{
	    PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(go, fsmName.Value);
	    if (playMakerFSM != null && playMakerFSM.FsmVariables.GetFsmInt("damageDealt").Value > 0)
	    {
		storeGameObject.Value = go;
		Fsm.Event(sendEvent);
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
