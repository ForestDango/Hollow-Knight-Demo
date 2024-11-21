using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Spawns a prefab Game Object from the Global Object Pool on the Game Manager.")]
    public class SpawnObjectFromGlobalPoolOverTimeV2 : FsmStateAction
    {
	[RequiredField]
	[Tooltip("GameObject to create. Usually a Prefab.")]
	public FsmGameObject gameObject;
	[Tooltip("Optional Spawn Point.")]
	public FsmGameObject spawnPoint;
	[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
	public FsmVector3 position;
	[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
	public FsmVector3 rotation;
	[Tooltip("How often, in seconds, spawn occurs.")]
	public FsmFloat frequency;
	[Tooltip("Minimum scale of clone.")]
	public FsmFloat scaleMin = 1f;
	[Tooltip("Maximum scale of clone.")]
	public FsmFloat scaleMax = 1f;

	private float timer;
	public override void Reset()
	{
	    gameObject = null;
	    spawnPoint = null;
	    position = new FsmVector3
	    {
		UseVariable = true
	    };
	    rotation = new FsmVector3
	    {
		UseVariable = true
	    };
	    frequency = null;
	}

	public override void OnUpdate()
	{
	    timer += Time.deltaTime;
	    if(timer > frequency.Value)
	    {
		timer = 0f;
		if(gameObject.Value != null)
		{
		    Vector3 a = Vector3.zero;
		    Vector3 euler = Vector3.up;
		    if (spawnPoint.Value != null)
		    {
			a = spawnPoint.Value.transform.position;
			if (!position.IsNone)
			{
			    a += position.Value;
			}
			euler = ((!rotation.IsNone) ? rotation.Value : spawnPoint.Value.transform.eulerAngles);
		    }
		    else
		    {
			if (!position.IsNone)
			{
			    a = position.Value;
			}
			if (!rotation.IsNone)
			{
			    euler = rotation.Value;
			}
		    }
		    if (gameObject != null)
		    {
			GameObject gameObject = this.gameObject.Value.Spawn(a, Quaternion.Euler(euler));
			if (scaleMin != null && scaleMax != null)
			{
			    float num = Random.Range(scaleMin.Value, scaleMax.Value);
			    if (num != 1f)
			    {
				gameObject.transform.localScale = new Vector3(num, num, num);
			    }
			}
		    }
		}
	    }
	}
    }
}
