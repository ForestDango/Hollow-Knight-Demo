using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Activates a certain amount of objects held in designated Object Pool and fires them off")]
    public class SpawnFromPool : RigidBody2dActionBase
    {
	[RequiredField]
	[Tooltip("Pool object to draw from.")]
	public FsmGameObject pool;
	public FsmVector3 adjustPosition;
	[Tooltip("Minimum amount of objects to be spawned.")]
	public FsmInt spawnMin;
	[Tooltip("Maximum amount of clones to be spawned.")]
	public FsmInt spawnMax;
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
	    pool = null;
	    adjustPosition = null;
	    spawnMin = null;
	    spawnMax = null;
	    speedMin = null;
	    speedMax = null;
	    angleMin = null;
	    angleMax = null;
	}
	public override void OnEnter()
	{
	    GameObject value = pool.Value;
	    if(value != null)
	    {
		int num = Random.Range(spawnMin.Value, spawnMax.Value);
		for (int i = 1; i <= num; i++)
		{
		    int childCount = value.transform.childCount;
		    if (childCount <= 0)
		    {
			Finish();
		    }
		    if(childCount == 0)
		    {
			return;
		    }
		    GameObject gameObject = value.transform.GetChild(Random.Range(0, childCount)).gameObject;
		    gameObject.SetActive(true);
		    CacheRigidBody2d(gameObject);
		    float num2 = Random.Range(speedMin.Value, speedMax.Value);
		    float num3 = Random.Range(angleMin.Value, angleMax.Value);
		    vectorX = num2 * Mathf.Cos(num3 * 0.017453292f);
		    vectorY = num2 * Mathf.Sin(num3 * 0.017453292f);
		    Vector2 velocity;
		    velocity.x = vectorX;
		    velocity.y = vectorY;
		    rb2d.velocity = velocity;
		    if (!adjustPosition.IsNone)
		    {
			gameObject.transform.position += adjustPosition.Value;
		    }
		    gameObject.transform.parent = null;
		}
	    }
	    Finish();
	}
    }
}
