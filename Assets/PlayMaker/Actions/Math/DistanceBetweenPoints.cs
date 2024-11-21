using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Math)]
    [Tooltip("Calculate the distance between two points and store it as a float.")]
    public class DistanceBetweenPoints : FsmStateAction
    {

	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmFloat distanceResult;
	[RequiredField]
	public FsmVector3 point1;
	[RequiredField]
	public FsmVector3 point2;

	public bool ignoreZ;
	public bool everyFrame;

	public override void Reset()
	{
	    distanceResult = null;
	    point1 = null;
	    point2 = null;
	    everyFrame = false;
	}
	public override void OnEnter()
	{
	    DoCalcDistance();
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	public override void OnUpdate()
	{
	    DoCalcDistance();
	}

	private void DoCalcDistance()
	{
	    if (distanceResult == null)
	    {
		return;
	    }
	    if (ignoreZ)
	    {
		Vector2 a = new Vector2(point1.Value.x, point1.Value.y);
		Vector2 b = new Vector2(point2.Value.x, point2.Value.y);
		distanceResult.Value = Vector2.Distance(a, b);
		return;
	    }
	    Vector3 a2 = new Vector3(point1.Value.x, point1.Value.y, point1.Value.z);
	    Vector2 v = new Vector3(point2.Value.x, point2.Value.y, point2.Value.z);
	    distanceResult.Value = Vector3.Distance(a2, v);
	}
    }
}
