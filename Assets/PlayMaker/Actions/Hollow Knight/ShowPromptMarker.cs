using HutongGames.PlayMaker;
using UnityEngine;


[ActionCategory("Hollow Knight")]

public class ShowPromptMarker : FsmStateAction
{
    public FsmGameObject prefab;
    public FsmString labelName;
    [UIHint(UIHint.Variable)]
    public FsmGameObject spawnPoint;
    [UIHint(UIHint.Variable)]
    public FsmGameObject storeObject;

    public override void Reset()
    {
	prefab = new FsmGameObject();
	labelName = new FsmString();
	spawnPoint = new FsmGameObject();
	storeObject = new FsmGameObject();
    }

    public override void OnEnter()
    {
	if (prefab.Value && spawnPoint.Value)
	{
	    GameObject gameObject;
	    if (storeObject.Value)
	    {
		gameObject = storeObject.Value;
	    }
	    else
	    {
		gameObject = prefab.Value.Spawn();
		storeObject.Value = gameObject;
	    }
	    gameObject.transform.position = spawnPoint.Value.transform.position;
	    PromptMarker component = gameObject.GetComponent<PromptMarker>();
	    if (component)
	    {
		component.SetLabel(labelName.Value);
		component.SetOwner(Owner);
		component.Show();
	    }
	}
	base.Finish();
    }

}
