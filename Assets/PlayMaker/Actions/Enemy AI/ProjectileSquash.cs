using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Squash projectile to match speed")]
    public class ProjectileSquash : RigidBody2dActionBase
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;
	[Tooltip("Increase this value to make the object's stretch more pronounced")]
	public FsmFloat stretchFactor = 1.4f;
	[Tooltip("Minimum scale value for X.")]
	public float stretchMinX = 0.5f;
	[Tooltip("Maximum scale value for Y")]
	public float stretchMaxY = 2f;
	[Tooltip("After other calculations, multiply scale by this modifier.")]
	public FsmFloat scaleModifier;
	public bool everyFrame;
	private FsmGameObject target;
	private float stretchX = 1f;
	private float stretchY = 1f;

	public override void Reset()
	{
	    gameObject = null;
	    scaleModifier = 1f;
	    everyFrame = false;
	    stretchX = 1f;
	    stretchY = 1f;
	}

	public override void OnPreprocess()
	{
	    Fsm.HandleFixedUpdate = true;
	}

	public override void OnEnter()
	{
	    CacheRigidBody2d(Fsm.GetOwnerDefaultTarget(gameObject));
	    target = Fsm.GetOwnerDefaultTarget(gameObject);
	    DoStretch();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}
	public override void OnFixedUpdate()
	{
	    DoStretch();
	}

	private void DoStretch()
	{
	    if (rb2d == null)
	    {
		return;
	    }
	    stretchY = 1f - rb2d.velocity.magnitude * stretchFactor.Value * 0.01f;
	    stretchX = 1f + rb2d.velocity.magnitude * stretchFactor.Value * 0.01f;
	    if (stretchX < stretchMinX)
	    {
		stretchX = stretchMinX;
	    }
	    if (stretchY > stretchMaxY)
	    {
		stretchY = stretchMaxY;
	    }
	    stretchY *= scaleModifier.Value;
	    stretchX *= scaleModifier.Value;
	    target.Value.transform.localScale = new Vector3(stretchX, stretchY, target.Value.transform.localScale.z);
	}

    }
}
