using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Spawns a random amount of chosen GameObject from global pool and fires them off in random directions.")]
    public class FlingObjectsFromGlobalPoolTime : RigidBody2dActionBase
    {
	[RequiredField]
	[Tooltip("GameObject to spawn.")]
	public FsmGameObject gameObject;
	[Tooltip("GameObject to spawn at (optional).")]
	public FsmGameObject spawnPoint;
	[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
	public FsmVector3 position;
	[Tooltip("How often, in seconds, spawn occurs.")]
	public FsmFloat frequency;
	[Tooltip("Minimum amount of objects to be spawned.")]
	public FsmInt spawnMin;
	[Tooltip("Maximum amount of objects to be spawned.")]
	public FsmInt spawnMax;
	[Tooltip("Minimum speed objects are fired at.")]
	public FsmFloat speedMin;
	[Tooltip("Maximum speed objects are fired at.")]
	public FsmFloat speedMax;
	[Tooltip("Minimum angle objects are fired at.")]
	public FsmFloat angleMin;
	[Tooltip("Maximum angle objects are fired at.")]
	public FsmFloat angleMax;
	[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
	public FsmFloat originVariationX;
	public FsmFloat originVariationY;

	private float vectorX;
	private float vectorY;
	private float timer;
	private bool originAdjusted;

	public override void Reset()
	{
	    gameObject = null;
	    spawnPoint = null;
	    position = new FsmVector3
	    {
		UseVariable = true
	    };
	    spawnMin = null;
	    spawnMax = null;
	    speedMin = null;
	    speedMax = null;
	    angleMin = null;
	    angleMax = null;
	    originVariationX = null;
	    originVariationY = null;
	}

	public override void OnEnter()
	{

	}

	public override void OnUpdate()
	{
	    timer += Time.deltaTime;
	    if (timer >= frequency.Value)
	    {
		timer = 0f;
		if (gameObject.Value != null)
		{
		    Vector3 a = Vector3.zero;
		    Vector3 zero = Vector3.zero;
		    if (spawnPoint.Value != null)
		    {
			a = spawnPoint.Value.transform.position;
			if (!position.IsNone)
			{
			    a += position.Value;
			}
		    }
		    else if (!position.IsNone)
		    {
			a = position.Value;
		    }
		    int num = Random.Range(spawnMin.Value, spawnMax.Value + 1);
		    for (int i = 1; i <= num; i++)
		    {
			GameObject gameObject = this.gameObject.Value.Spawn(a, Quaternion.Euler(zero));
			//GameObject gameObject = GameObject.Instantiate(this.gameObject.Value, a, Quaternion.Euler(zero));
			float x = gameObject.transform.position.x;
			float y = gameObject.transform.position.y;
			float z = gameObject.transform.position.z;
			if (originVariationX != null)
			{
			    x = gameObject.transform.position.x + Random.Range(-originVariationX.Value, originVariationX.Value);
			    originAdjusted = true;
			}
			if (originVariationY != null)
			{
			    y = gameObject.transform.position.y + Random.Range(-originVariationY.Value, originVariationY.Value);
			    originAdjusted = true;
			}
			if (originAdjusted)
			{
			    gameObject.transform.position = new Vector3(x, y, z);
			}
			CacheRigidBody2d(gameObject);
			float num2 = Random.Range(speedMin.Value, speedMax.Value);
			float num3 = Random.Range(angleMin.Value, angleMax.Value);
			vectorX = num2 * Mathf.Cos(num3 * 0.017453292f);
			vectorY = num2 * Mathf.Sin(num3 * 0.017453292f);
			Vector2 velocity;
			velocity.x = vectorX;
			velocity.y = vectorY;
			rb2d.velocity = velocity;
		    }
		}
	    }
	}

    }
}
