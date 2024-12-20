using UnityEngine;

public class CorpseHatcher : Corpse
{

    protected override void Smash()
    {
	if (!hitAcid)
	{
	    GlobalPrefabDefaults.Instance.SpawnBlood(transform.position, 40, 40, 15f, 20f, 75f, 105f, null);
	    GameObject gameObject = GameObject.FindWithTag("Extra Tag");
	    if (gameObject)
	    {
		for (int i = 0; i < 2; i++)
		{
		    int index = Random.Range(0, gameObject.transform.childCount);
		    Transform child = gameObject.transform.GetChild(index);
		    if (child)
		    {
			child.SetParent(null);
			child.position = transform.position;
			FSMUtility.SendEventToGameObject(child.gameObject, "SPAWN", false);
		    }
		}
	    }
	}
	base.Smash();
    }
}
