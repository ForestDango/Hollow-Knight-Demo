using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitEffectsUninfected : MonoBehaviour,IHitEffectReciever
{
    public Vector3 effectOrigin;
    [Space]
    public AudioSource audioPlayerPrefab;
    public AudioEvent enemyDamage;
    [Space]
    public GameObject uninfectedHitPt;
    public GameObject slashEffectGhost1;
    public GameObject slashEffectGhost2;
    private SpriteFlash spriteFlash;
    [Tooltip("Disable if there are no listeners for this event, to save the expensive recursive send event.")]
    public bool sendDamageFlashEvent = true;
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
	if (sendDamageFlashEvent)
	{
	    FSMUtility.SendEventToGameObject(base.gameObject, "DAMAGE FLASH", true);
	}
	enemyDamage.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	if (spriteFlash)
	{
	    spriteFlash.flashFocusHeal();
	}
	GameObject gameObject = Instantiate(uninfectedHitPt, transform.position + effectOrigin, Quaternion.identity);
	switch (DirectionUtils.GetCardinalDirection(attackDirection))
	{
	    case 0:
		if (gameObject)
		{
		    gameObject.transform.SetRotation2D(-45f);
		}
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
		    Prefab = slashEffectGhost1,
		    AmountMin = 2,
		    AmountMax = 3,
		    SpeedMin = 20f,
		    SpeedMax = 35f,
		    AngleMin = -40f,
		    AngleMax = 40f,
		    OriginVariationX = 0f,
		    OriginVariationY = 0f
		}, transform, effectOrigin);
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
		    Prefab = slashEffectGhost2,
		    AmountMin = 2,
		    AmountMax = 3,
		    SpeedMin = 10f,
		    SpeedMax = 35f,
		    AngleMin = -40f,
		    AngleMax = 40f,
		    OriginVariationX = 0f,
		    OriginVariationY = 0f
		}, transform, effectOrigin);
		break;
	    case 1:
		if (gameObject)
		{
		    gameObject.transform.SetRotation2D(45f);
		}
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
		    Prefab = slashEffectGhost1,
		    AmountMin = 2,
		    AmountMax = 3,
		    SpeedMin = 20f,
		    SpeedMax = 35f,
		    AngleMin = 50f,
		    AngleMax = 130f,
		    OriginVariationX = 0f,
		    OriginVariationY = 0f
		}, transform, effectOrigin);
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
		    Prefab = slashEffectGhost2,
		    AmountMin = 2,
		    AmountMax = 3,
		    SpeedMin = 10f,
		    SpeedMax = 35f,
		    AngleMin = 50f,
		    AngleMax = 130f,
		    OriginVariationX = 0f,
		    OriginVariationY = 0f
		}, transform, effectOrigin);
		break;
	    case 2:
		if (gameObject)
		{
		    gameObject.transform.SetRotation2D(-225f);
		}
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
		    Prefab = slashEffectGhost1,
		    AmountMin = 2,
		    AmountMax = 3,
		    SpeedMin = 20f,
		    SpeedMax = 35f,
		    AngleMin = 140f,
		    AngleMax = 220f,
		    OriginVariationX = 0f,
		    OriginVariationY = 0f
		}, transform, effectOrigin);
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
		    Prefab = slashEffectGhost2,
		    AmountMin = 2,
		    AmountMax = 3,
		    SpeedMin = 10f,
		    SpeedMax = 35f,
		    AngleMin = 140f,
		    AngleMax = 220f,
		    OriginVariationX = 0f,
		    OriginVariationY = 0f
		}, transform, effectOrigin);
		break;
	    case 3:
		if (gameObject)
		{
		    gameObject.transform.SetRotation2D(225f);
		}
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
		    Prefab = slashEffectGhost1,
		    AmountMin = 2,
		    AmountMax = 3,
		    SpeedMin = 20f,
		    SpeedMax = 35f,
		    AngleMin = 230f,
		    AngleMax = 310f,
		    OriginVariationX = 0f,
		    OriginVariationY = 0f
		}, transform, effectOrigin);
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
		    Prefab = slashEffectGhost2,
		    AmountMin = 2,
		    AmountMax = 3,
		    SpeedMin = 10f,
		    SpeedMax = 35f,
		    AngleMin = 230f,
		    AngleMax = 310f,
		    OriginVariationX = 0f,
		    OriginVariationY = 0f
		}, transform, effectOrigin);
		break;
	}
	didFireThisFrame = true;
    }
}
