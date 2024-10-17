using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Sets the velocity of all children of chosen GameObject")]
    public class FlingObjects : RigidBody2dActionBase
    {
	[RequiredField]
	[Tooltip("Object containing the objects to be flung.")]
	public FsmGameObject containerObject;
	public FsmVector3 adjustPosition;
	public FsmBool randomisePosition;
	[Tooltip("Minimum speed clones are fired at.")]
	public FsmFloat speedMin;
	[Tooltip("Maximum speed clones are fired at.")]
	public FsmFloat speedMax;
	[Tooltip("Minimum angle clones are fired at.")]
	public FsmFloat angleMin;
	[Tooltip("Maximum angle clones are fired at.")]
	public FsmFloat angleMax;

	private float vectorX;
	private float vectorY;
	private bool originAdjusted;

	public override void Reset()
	{
	    containerObject = null;
	    adjustPosition = null;
	    speedMin = null;
	    speedMax = null;
	    angleMin = null;
	    angleMax = null;
	}

	public override void OnEnter()
	{
	    GameObject value = containerObject.Value;
	    if(value != null)
	    {
		int childCount = value.transform.childCount;
		for (int i = 1; i <= childCount; i++)
		{
		    GameObject gameObject = value.transform.GetChild(i - 1).gameObject;
		    CacheRigidBody2d(gameObject);
		    if (rb2d != null)
		    {
			float num = Random.Range(speedMin.Value, speedMax.Value);
			float num2 = Random.Range(angleMin.Value, angleMax.Value);
			vectorX = num * Mathf.Cos(num2 * 0.017453292f);
			vectorY = num2 * Mathf.Sin(num2 * 0.017453292f);
			Vector2 velocity;
			velocity.x = vectorX;
			velocity.y = vectorY;
			if (!adjustPosition.IsNone)
			{
			    if (randomisePosition.Value)
			    {
				gameObject.transform.position = new Vector3(gameObject.transform.position.x + Random.Range(-adjustPosition.Value.x, adjustPosition.Value.x), gameObject.transform.position.y + Random.Range(-adjustPosition.Value.y, adjustPosition.Value.y), gameObject.transform.position.z);
			    }
			    else
			    {
				gameObject.transform.position += adjustPosition.Value;
			    }
			}
		    }
		}
	    }
	    Finish();
	}
    }
}
