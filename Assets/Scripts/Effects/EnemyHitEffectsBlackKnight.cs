using System;
using UnityEngine;

public class EnemyHitEffectsBlackKnight : MonoBehaviour,IHitEffectReciever
{
    public Vector3 effectOrigin;
    [Space]
    public AudioSource audioPlayerPrefab;
    public AudioEvent enemyDamage;
    [Space]
    public GameObject hitFlashOrange;
    public GameObject hitPuffLarge;
    private SpriteFlash spriteFlash;

    private bool didFireThisFrame;

    private void Awake()
    {
	spriteFlash = GetComponent<SpriteFlash>();
    }

    protected void Update()
    {
	didFireThisFrame = false;
    }

    public void ReceiverHitEffect(float attackDirection)
    {
	if (didFireThisFrame)
	    return;
	FSMUtility.SendEventToGameObject(this.gameObject, "DAMAGE FLASH", false);
	enemyDamage.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	if (spriteFlash)
	{
	    spriteFlash.flashInfected();
	}
	GameObject gameObject = Instantiate(hitFlashOrange, transform.position + effectOrigin, Quaternion.identity);
	switch (DirectionUtils.GetCardinalDirection(attackDirection))
	{
	    case 0:
		gameObject.transform.eulerAngles = new Vector3(0f, 90f, 270f);
		break;
	    case 1:
		gameObject.transform.eulerAngles = new Vector3(270f, 90f, 270f);
		break;
	    case 2:
		gameObject.transform.eulerAngles = new Vector3(180f, 90f, 270f);
		break;
	    case 3:
		gameObject.transform.eulerAngles = new Vector3(-72.5f, -180f, -180f);
		break;
	}
	didFireThisFrame = true;
    }

   
}
