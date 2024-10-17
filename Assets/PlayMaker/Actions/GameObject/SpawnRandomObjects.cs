using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Spawns a random amount of chosen GameObject and fires them off in random directions.")]
    public class SpawnRandomObjects : RigidBody2dActionBase
    {
	[RequiredField]
	[Tooltip("GameObject to create.")]
	public FsmGameObject gameObject;
	[Tooltip("GameObject to spawn at (optional).")]
	public FsmGameObject spawnPoint;
	[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
	public FsmVector3 position;
	[Tooltip("Minimum amount of clones to be spawned.")]
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
	[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
	public FsmFloat originVariation;

	private float vectorX;
	private float vectorY;

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
	    originVariation = null;
	}

	public override void OnEnter()
	{
	    GameObject value = gameObject.Value;
	    if (value != null)
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
		    GameObject gameObject = Object.Instantiate(value, a, Quaternion.Euler(zero));
		    if (originVariation != null)
		    {
			float x = gameObject.transform.position.x + Random.Range(-originVariation.Value, originVariation.Value);
			float y = gameObject.transform.position.y + Random.Range(-originVariation.Value, originVariation.Value);
			float z = gameObject.transform.position.z;
			gameObject.transform.position = new Vector3(x, y, z);
		    }
		    base.CacheRigidBody2d(gameObject);
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
	    base.Finish();
	}


    }

}
