using UnityEngine;
using System.Collections.Generic;
using System;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
    [Tooltip("Detect additional collisions between the Owner of this FSM and other object with additional raycasting.")]
    public class CheckCollisionSideEnter : FsmStateAction
    {
	[UIHint(UIHint.Variable)]
	public FsmBool topHit;
	[UIHint(UIHint.Variable)]
	public FsmBool rightHit;
	[UIHint(UIHint.Variable)]
	public FsmBool bottomHit;
	[UIHint(UIHint.Variable)]
	public FsmBool leftHit;

	public FsmEvent topHitEvent;
	public FsmEvent rightHitEvent;
	public FsmEvent bottomHitEvent;
	public FsmEvent leftHitEvent;

	public bool otherLayer;
	public int otherLayerNumber;

	public FsmBool ignoreTriggers;
	private PlayMakerUnity2DProxy _proxy;
	private Collider2D col2d;
	private const float RAYCAST_LENGTH = 0.08f;

	private List<Vector2> topRays;
	private List<Vector2> rightRays;
	private List<Vector2> bottomRays;
	private List<Vector2> leftRays;

	public override void Reset()
	{
	}
	public override void OnEnter()
	{
	    col2d = Fsm.GameObject.GetComponent<Collider2D>();
	    _proxy = Owner.GetComponent<PlayMakerUnity2DProxy>();
	    if(_proxy == null)
	    {
		_proxy = Owner.AddComponent<PlayMakerUnity2DProxy>();
	    }
	    _proxy.AddOnCollisionEnter2dDelegate(new PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate(DoCollisionEnter2D));
	}

	public override void OnUpdate()
	{    		
	}

	public override void OnExit()
	{
	    _proxy.RemoveOnCollisionEnter2dDelegate(new PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate(DoCollisionEnter2D));
	}

	public new void DoCollisionEnter2D(Collision2D collision)
	{
	    if (!otherLayer)
	    {
		if(LayerMask.LayerToName(collision.gameObject.layer) == "Terrain")
		{
		    CheckTouching(LayerMask.NameToLayer("Terrain"));
		    return;
		}
	    }
	    else
	    {
		CheckTouching(otherLayerNumber);
	    }
	}

	private void CheckTouching(LayerMask layer)
	{
	    topRays = new List<Vector2>();
	    topRays.Add(new Vector2(col2d.bounds.min.x, col2d.bounds.max.y));
	    topRays.Add(new Vector2(col2d.bounds.center.x, col2d.bounds.max.y));
	    topRays.Add(col2d.bounds.max);
	    rightRays = new List<Vector2>();
	    rightRays.Add(col2d.bounds.max);
	    rightRays.Add(new Vector2(col2d.bounds.max.x, col2d.bounds.center.y));
	    rightRays.Add(new Vector2(col2d.bounds.max.x, col2d.bounds.min.y));
	    bottomRays = new List<Vector2>();
	    bottomRays.Add(new Vector2(col2d.bounds.min.x, col2d.bounds.min.y));
	    bottomRays.Add(new Vector2(col2d.bounds.center.x, col2d.bounds.min.y));
	    bottomRays.Add(col2d.bounds.min);
	    leftRays = new List<Vector2>();
	    leftRays.Add(col2d.bounds.min);
	    leftRays.Add(new Vector2(col2d.bounds.min.x, col2d.bounds.center.y));
	    leftRays.Add(new Vector2(col2d.bounds.min.x, col2d.bounds.max.y));
	    topHit.Value = false;
	    rightHit.Value = false;
	    bottomHit.Value = false;
	    leftHit.Value = false;
	    foreach (Vector2 v in topRays)
	    {
		RaycastHit2D raycastHit2D = Physics2D.Raycast(v, Vector2.up, RAYCAST_LENGTH, 1 << layer);
		if(raycastHit2D.collider != null && (!ignoreTriggers.Value || !raycastHit2D.collider.isTrigger))
		{
		    topHit.Value = true;
		    Fsm.Event(topHitEvent);
		    break;
		}
	    }
	    foreach (Vector2 v2 in rightRays)
	    {
		RaycastHit2D raycastHit2D2 = Physics2D.Raycast(v2, Vector2.right, RAYCAST_LENGTH, 1 << layer);
		if (raycastHit2D2.collider != null && (!ignoreTriggers.Value || !raycastHit2D2.collider.isTrigger))
		{
		    rightHit.Value = true;
		    Fsm.Event(rightHitEvent);
		    break;
		}
	    }
	    foreach (Vector2 v3 in bottomRays)
	    {
		RaycastHit2D raycastHit2D3 = Physics2D.Raycast(v3, Vector2.down, RAYCAST_LENGTH, 1 << layer);
		if(raycastHit2D3.collider != null && (!ignoreTriggers.Value || !raycastHit2D3.collider.isTrigger))
		{
		    bottomHit.Value = true;
		    Fsm.Event(bottomHitEvent);
		    Debug.LogFormat("Bottom Hit Terrain");
		    break;
		}
	    }
	    foreach (Vector2 v4 in leftRays)
	    {
		RaycastHit2D raycastHit2D4 = Physics2D.Raycast(v4, Vector2.left, RAYCAST_LENGTH, 1 << layer);
		if (raycastHit2D4.collider != null && (!ignoreTriggers.Value || !raycastHit2D4.collider.isTrigger))
		{
		    leftHit.Value = true;
		    Fsm.Event(leftHitEvent);
		    break;
		}
	    }
	}
    }

}
