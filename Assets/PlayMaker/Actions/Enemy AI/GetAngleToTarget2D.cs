using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Enemy AI")]
    [Tooltip("Get the angle from Game Object to the target. 0 is right, 90 is up etc.")]
    public class GetAngleToTarget2D : FsmStateAction
    {
	[RequiredField]
	public FsmOwnerDefault gameObject;
	[RequiredField]
	public FsmGameObject target;

	public FsmFloat offsetX;
	public FsmFloat offsetY;

	[RequiredField]
	public FsmFloat storeAngle;

	private FsmGameObject self;
	private FsmFloat x;
	private FsmFloat y;

	public bool everyFrame;

	public override void Reset()
	{
	    gameObject = null;
	    target = null;
	    offsetX = null;
	    offsetY = null;
	    storeAngle = null;
	    everyFrame = false;
	}

	public override void OnEnter()
	{
	    self = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (offsetX.IsNone)
	    {
		offsetX.Value = 0;
	    }
	    if (offsetY.IsNone)
	    {
		offsetY.Value = 0;
	    }
	    DoGetAngle();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	public override void OnUpdate()
	{
	    DoGetAngle();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	private void DoGetAngle()
	{
	    float num = target.Value.transform.position.y + offsetY.Value - self.Value.transform.position.y;
	    float num2 = target.Value.transform.position.x + offsetX.Value - self.Value.transform.position.x;
	    float num3;
	    for (num3 = Mathf.Atan2(num, num2) * 57.295776f; num3 < 0f; num3 += 360f)
	    {
	    }
	    storeAngle.Value = num3;
	}
    }
}
