using System;
using GlobalEnums;
using UnityEngine;

public class NailSlash : MonoBehaviour
{
    public string animName;
    public Vector3 scale;
    private HeroController heroCtrl;
    private PlayMakerFSM slashFsm;
    private tk2dSpriteAnimator anim;
    private MeshRenderer mesh;
    private AudioSource audioSource;
    private PolygonCollider2D poly;
    private PolygonCollider2D clashTinkpoly;

    private float slashAngle;
    private bool slashing;
    private bool animCompleted;
    private int stepCounter;
    private int polyCounter;

    private void Awake()
    {
	try
	{
	    heroCtrl = transform.root.GetComponent<HeroController>();
	}
	catch(NullReferenceException ex)
	{
	    string str = "NailSlash: could not find HeroController on parent: ";
	    string name = transform.root.name;
	    string str2 = " ";
	    NullReferenceException ex2 = ex;
	    Debug.LogError(str + name + str2 + ((ex2 != null) ? ex2.ToString() : null));
	}
	slashFsm = GetComponent<PlayMakerFSM>();
	audioSource = GetComponent<AudioSource>();
	anim = GetComponent<tk2dSpriteAnimator>();
	mesh = GetComponent<MeshRenderer>();
	poly = GetComponent<PolygonCollider2D>();
	clashTinkpoly = transform.Find("Clash Tink").GetComponent<PolygonCollider2D>();
	poly.enabled = false;
	mesh.enabled = false;
    }

    private void FixedUpdate()
    {
	if (slashing)
	{
	    if(stepCounter == 1)
	    {
		poly.enabled = true;
		clashTinkpoly.enabled = true;
	    }
	    if(stepCounter >= 5 && polyCounter > 0f)
	    {
		poly.enabled = false;
		clashTinkpoly.enabled = false;
	    }
	    if(animCompleted && polyCounter > 1)
	    {
		CancelAttack();
	    }
	    if (poly.enabled)
	    {
		polyCounter++;
	    }
	    stepCounter++;
	}
    }

    public void StartSlash()
    {
	audioSource.Play();
	slashAngle = slashFsm.FsmVariables.GetFsmFloat("direction").Value;

	transform.localScale = scale;
	anim.Play(animName);
	anim.PlayFromFrame(0);
	stepCounter = 0;
	polyCounter = 0;
	poly.enabled = false;
	clashTinkpoly.enabled = false;
	animCompleted = false;
	anim.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(Disable);
	slashing = true;
	mesh.enabled = true;
    }

    private void Disable(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
    {
	animCompleted = true;
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
	if(otherCollider != null)
	{
	    if(slashAngle == 0f)
	    {
		int layer = otherCollider.gameObject.layer;
		if(layer == 11 && (otherCollider.gameObject.GetComponent<NonBouncer>() == null || !otherCollider.gameObject.GetComponent<NonBouncer>().active))
		{
		    if (otherCollider.gameObject.GetComponent<BounceShroom>() != null)
		    {
			heroCtrl.RecoilLeftLong();
			Bounce(otherCollider, false);
		    }
		    else
		    {
			heroCtrl.RecoilLeft();
		    }
		}
		if (layer == 19 && otherCollider.gameObject.GetComponent<BounceShroom>() != null)
		{
		    heroCtrl.RecoilLeftLong();
		    Bounce(otherCollider, false);
		    return;
		}

	    }
	    else if(slashAngle == 180f)
	    {
		int layer2 = otherCollider.gameObject.layer;
		if (layer2 == 11 && (otherCollider.gameObject.GetComponent<NonBouncer>() == null || !otherCollider.gameObject.GetComponent<NonBouncer>().active))
		{
		    if (otherCollider.gameObject.GetComponent<BounceShroom>() != null)
		    {
			heroCtrl.RecoilRightLong();
			Bounce(otherCollider, false);
		    }
		    else
		    {
			heroCtrl.RecoilRight();
		    }
		}
		if (layer2 == 19 && otherCollider.gameObject.GetComponent<BounceShroom>() != null)
		{
		    heroCtrl.RecoilRightLong();
		    Bounce(otherCollider, false);
		    return;
		}
	    }
	    else if (slashAngle == 90f)
	    {
		int layer3 = otherCollider.gameObject.layer;
		if (layer3 == 11 && (otherCollider.gameObject.GetComponent<NonBouncer>() == null || !otherCollider.gameObject.GetComponent<NonBouncer>().active))
		{
		    if (otherCollider.gameObject.GetComponent<BounceShroom>() != null)
		    {
			heroCtrl.RecoilDown();
			Bounce(otherCollider, false);
		    }
		    else
		    {
			heroCtrl.RecoilDown();
		    }
		}
		if (layer3 == 19 && otherCollider.gameObject.GetComponent<BounceShroom>() != null)
		{
		    heroCtrl.RecoilDown();
		    Bounce(otherCollider, false);
		    return;
		}
	    }
	    else if(slashAngle == 270f)
	    {
		PhysLayers layer4 = (PhysLayers)otherCollider.gameObject.layer;
		if((layer4 == PhysLayers.ENEMIES || layer4 == PhysLayers.INTERACTIVE_OBJECT || layer4 == PhysLayers.HERO_ATTACK) && (otherCollider.gameObject.GetComponent<NonBouncer>() == null || !otherCollider.gameObject.GetComponent<NonBouncer>().active))
		{
		    if (otherCollider.gameObject.GetComponent<BigBouncer>() != null)
		    {
			heroCtrl.BounceHigh();
			return;
		    }
		    if (otherCollider.gameObject.GetComponent<BounceShroom>() != null)
		    {
			heroCtrl.ShroomBounce();
			Bounce(otherCollider, true);
			return;
		    }
		    heroCtrl.Bounce();
		}
	    }
	}
    }

    private void OnTriggerStay2D(Collider2D otherCollision)
    {
	OnTriggerEnter2D(otherCollision);
    }

    private void Bounce(Collider2D otherCollider, bool useEffects)
    {
	PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(otherCollider.gameObject, "Bounce Shroom");
	if (playMakerFSM)
	{
	    playMakerFSM.SendEvent("BOUNCE UPWARD");
	    return;
	}
	BounceShroom component = otherCollider.GetComponent<BounceShroom>();
	if (component)
	{
	    component.BounceLarge(useEffects);
	}
    }

    public void CancelAttack()
    {
	slashing = false;
	poly.enabled = false;
	clashTinkpoly.enabled = false;
	mesh.enabled = false;
    }

}
