using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Object rotates to face direction it is travelling in.")]
    public class FaceAngleV2 : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;
	[Tooltip("Offset the angle. If sprite faces right, leave as 0.")]
	public FsmFloat angleOffset;
	[Tooltip("Use local or world space.")]
	public FsmBool worldSpace;
	public bool everyFrame;
	private FsmGameObject target;

	public override void Reset()
	{
	    gameObject = null;
	    angleOffset = 0f;
	    everyFrame = false;
	}

	public override void Awake()
	{
	    Fsm.HandleFixedUpdate = true;
	}

	public override void OnPreprocess()
	{
	    Fsm.HandleFixedUpdate = true;
	}

	public override void OnEnter()
	{
	    CacheRigidBody2d(Fsm.GetOwnerDefaultTarget(gameObject));
	    target = Fsm.GetOwnerDefaultTarget(gameObject);
	    DoAngle();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	public override void OnFixedUpdate()
	{
	    DoAngle();
	}
	private void DoAngle()
	{
	    if (rb2d == null)
	    {
		return;
	    }
	    Vector2 velocity = rb2d.velocity;
	    float z = Mathf.Atan2(velocity.y, velocity.x) * 57.295776f + angleOffset.Value;
	    if (worldSpace.Value)
	    {
		target.Value.transform.eulerAngles = new Vector3(0f, 0f, z);
		return;
	    }
	    target.Value.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}

    }

}
