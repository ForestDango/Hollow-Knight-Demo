using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Object Pool")]
    [Tooltip("Recycles the Owner of the Fsm. Useful for Object Pool spawned Prefabs that need to kill themselves, e.g., a projectile that explodes on impact.")]
    public class RecycleSelf : FsmStateAction
    {
	// TODO:后续有了ObjectPool的时候就替换掉
	public override void OnEnter()
	{
	    if (Owner != null)
		GameObject.Destroy(Owner);
	    Finish();
	}
    }
}
