using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Returns the X/Y Min and max bounds for a box2d collider (in world space)")]
    public class GetColliderRange : FsmStateAction
    {
	[RequiredField]
	public FsmOwnerDefault gameObject;
	[UIHint(UIHint.Variable)]
	public FsmFloat minX;
	[UIHint(UIHint.Variable)]
	public FsmFloat maxX;
	[UIHint(UIHint.Variable)]
	public FsmFloat minY;
	[UIHint(UIHint.Variable)]
	public FsmFloat maxY;

	public bool everyFrame;
	public override void Reset()
	{
	    gameObject = null;
	    minX = null;
	    maxX = null;
	    minY = null;
	    maxY = null;
	    everyFrame = false;
	}

	public void GetRange()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    BoxCollider2D component = ownerDefaultTarget.GetComponent<BoxCollider2D>();
	    Vector2 size = component.size;
	    Vector3 vector = ownerDefaultTarget.transform.TransformPoint(component.offset);
	    maxY.Value = vector.y + size.y / 2f;
	    minY.Value = vector.y - size.y / 2f;
	    minX.Value = vector.x - size.x / 2f;
	    maxX.Value = vector.x + size.x / 2f;
	}

	public override void OnEnter()
	{
	    GetRange();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	public override void OnUpdate()
	{
	    GetRange();
	}
    }
}
