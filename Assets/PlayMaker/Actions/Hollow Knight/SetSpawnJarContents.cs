using System;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class SetSpawnJarContents : FsmStateAction
{
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmGameObject storedObject;
    public FsmGameObject enemyPrefab;
    public FsmInt enemyHealth;

    public override void Reset()
    {
	storedObject = null;
	enemyPrefab = null;
	enemyHealth = null;
    }

    public override void OnEnter()
    {
	if (storedObject.Value)
	{
	    SpawnJarControl component = storedObject.Value.GetComponent<SpawnJarControl>();
	    if (component)
	    {
		component.SetEnemySpawn(enemyPrefab.Value, enemyHealth.Value);
	    }
	}
	Finish();
    }
}
