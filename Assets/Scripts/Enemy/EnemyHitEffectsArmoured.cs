using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitEffectsArmoured : MonoBehaviour,IHitEffectReciever
{
    public Vector3 effectOrigin;
    [Space]
    public AudioSource audioPlayerPrefab;
    public AudioEvent enemeyDamage;
    [Space]
    public GameObject dustHit;
    public GameObject armourHit;

    private SpriteFlash spriteFlash;
    private bool didFireThisFrame;

    protected void Awake()
    {
	spriteFlash = GetComponent<SpriteFlash>();
    }

    protected void Update()
    {
	didFireThisFrame = false;
    }

    public void RecieveHitEffect(float attackDirection)
    {
	if (didFireThisFrame)
	{
	    return;
	}
	enemeyDamage.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	if (spriteFlash)
	{
	    spriteFlash.flashArmoured();
	}
	GameObject gameObject = dustHit ? Instantiate(dustHit, transform.position + effectOrigin, Quaternion.identity) : null;
	if (gameObject)
	{
	    gameObject.transform.SetPositionZ(-0.01f);
	}
	switch (DirectionUtils.GetCardinalDirection(attackDirection))
	{
	    case 0:
		if (gameObject)
		{
		    gameObject.transform.eulerAngles = new Vector3(180f, 90f, 270f);
		}
		if (armourHit)
		{
		    FSMUtility.SendEventToGameObject(armourHit, "ARMOUR HIT R", false);
		}
		break;
	    case 1:
		if (gameObject)
		{
		    gameObject.transform.eulerAngles = new Vector3(270f, 90f, 270f);
		}
		if (armourHit)
		{
		    FSMUtility.SendEventToGameObject(armourHit, "ARMOUR HIT U", false);
		}
		break;
	    case 2:
		if (gameObject)
		{
		    gameObject.transform.eulerAngles = new Vector3(0f, 90f, 270f);
		}
		if (armourHit)
		{
		    FSMUtility.SendEventToGameObject(armourHit, "ARMOUR HIT L", false);
		}
		break;
	    case 3:
		if (gameObject)
		{
		    gameObject.transform.eulerAngles = new Vector3(-72.5f, -180f, -180f);
		}
		if (armourHit)
		{
		    FSMUtility.SendEventToGameObject(armourHit, "ARMOUR HIT D", false);
		}
		break;
	}
	didFireThisFrame = true;
    }

    
}
