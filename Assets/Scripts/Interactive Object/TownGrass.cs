using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownGrass : MonoBehaviour
{
    public GameObject[] debris;
    public GameObject nailEffectPrefab;
    public AudioClip[] cutSound;
    public AudioSource source;

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if (GrassCut.ShouldCut(collision))
	{
	    int num = (int)Mathf.Sign(collision.transform.position.x - base.transform.position.x);
	    Vector3 position = (collision.transform.position + base.transform.position) / 2f;
	    if (nailEffectPrefab)
	    {
		GameObject gameObject = Instantiate(nailEffectPrefab);
		Vector3 localScale = gameObject.transform.localScale;
		localScale.x = Mathf.Abs(localScale.x) * (float)(-num);
		gameObject.transform.localScale = localScale;
	    }
	    else
	    {
		Debug.Log("No nail effect assigned to " + gameObject.name);
	    }
	    if (debris.Length != 0)
	    {
		foreach (GameObject gameObject2 in debris)
		{
		    gameObject2.SetActive(true);
		    gameObject2.transform.SetParent(null, true);
		}
	    }
	    else
	    {
		Debug.Log("No debris assigned to " + gameObject.name);
	    }
	    if(source && cutSound .Length != 0)
	    {
		source.transform.SetParent(null, true);
		source.PlayOneShot(cutSound[Random.Range(0, cutSound.Length)]);
	    }
	    gameObject.SetActive(false);
	}
    }
}
