using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Spawns a prefab Game Object from the Global Object Pool on the Game Manager.")]
    public class SpawnObjectFromGlobalPool : FsmStateAction
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

	[UIHint(UIHint.Variable)]
	[Tooltip("Optionally store the created object.")]
	public FsmGameObject storeObject;

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
	    storeObject = null;
	}

	public override void OnEnter()
	{
	    if (gameObject.Value != null)
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
		    GameObject value = gameObject.Value.Spawn(a, Quaternion.Euler(euler));
		    storeObject.Value = value;
		}
	    }
	    Finish();
	}
    }
}
