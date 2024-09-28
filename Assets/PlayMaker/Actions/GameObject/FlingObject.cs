using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Fling")]
    public class FlingObject : RigidBody2dActionBase
    {
	[RequiredField]
	public FsmOwnerDefault flungObject;
	public FsmFloat speedMin;
	public FsmFloat speedMax;
	public FsmFloat AngleMin;
	public FsmFloat AngleMax;
	private float vectorX;
	private float vectorY;
	private bool originAdjusted;

	public override void Reset()
	{
	    flungObject = null;
	    speedMin = null;
	    speedMax = null;
	    AngleMin = null;
	    AngleMax = null;
	}
	public override void OnEnter()
	{
	    if(flungObject != null)
	    {
		GameObject owenrDefaultTarget = Fsm.GetOwnerDefaultTarget(flungObject);
		if(owenrDefaultTarget != null)
		{
		    float num = Random.Range(speedMin.Value, speedMax.Value);
		    float num2 = Random.Range(AngleMin.Value, AngleMax.Value);
		    vectorX = num * Mathf.Cos(num2 * 0.017453292f);
		    vectorY = num * Mathf.Sin(num2 * 0.017453292f);
		    Vector2 velocity;
		    velocity.x = vectorX;
		    velocity.y = vectorY;
		    CacheRigidBody2d(owenrDefaultTarget);
		    rb2d.velocity = velocity;
		}
	    }
	    Finish();
	}


    }

}
