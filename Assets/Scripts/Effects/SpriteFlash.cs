using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlash : MonoBehaviour
{
    private Renderer rend;
    private Color flashColour;
    private Color prevColor;

    private float amount;
    private float amountCurrent;

    private float timeUp;
    private float stayTime;
    private float timeDown;

    private int flashingState;
    private float flashTimer;
    private float t;

    private bool repeatFlash;
    private bool cancelFlash;

    private float geoTimer;
    private bool geoFlash;

    private MaterialPropertyBlock block;
    private bool sendToChildren = true;

    private void Start()
    {
	if(rend == null)
	{
	    rend = GetComponent<Renderer>();
	    prevColor = rend.material.color;
	}
	if (block == null)
	{
	    block = new MaterialPropertyBlock();
	}
    }

    private void OnDisable()
    {
	if (rend == null)
	{
	    rend = GetComponent<Renderer>();
	}
	if (block == null)
	{
	    block = new MaterialPropertyBlock();
	}
	block.SetFloat("_FlashAmount", 0f);
	rend.SetPropertyBlock(block);
	flashTimer = 0f;
	flashingState = 0;
	repeatFlash = false;
	cancelFlash = false;
	geoFlash = false;
    }

    private void Update()
    {
	if (cancelFlash)
	{
	    block.SetFloat("_FlashAmount", 0f);
	    rend.SetPropertyBlock(block);
	    flashingState = 0;
	    cancelFlash = false;
	}
	if(flashingState == 1)
	{
	    if (flashTimer < timeUp)
	    {
		flashTimer += Time.deltaTime;
		t = flashTimer / timeUp;
		amountCurrent = Mathf.Lerp(0f, amount, t);
		block.SetFloat("_FlashAmount", amountCurrent);
		rend.SetPropertyBlock(block);
	    }
	    else
	    {
		block.SetFloat("_FlashAmount", amount);
		rend.SetPropertyBlock(block);
		flashTimer = 0f;
		flashingState = 2;
	    }
	}
	if(flashingState == 2)
	{
	    if(flashTimer < stayTime)
	    {
		flashTimer += Time.deltaTime;
	    }
	    else
	    {
		flashTimer = 0f;
		flashingState = 3;
	    }
	}
	if(flashingState == 3)
	{
	    if (flashTimer < timeDown)
	    {
		flashTimer += Time.deltaTime;
		t = flashTimer / timeDown;
		amountCurrent = Mathf.Lerp(amount, 0f, t);
		block.SetFloat("_FlashAmount", amountCurrent);
		rend.SetPropertyBlock(block);
	    }
	    else
	    {
		block.SetFloat("_FlashAmount", 0f);
		block.SetColor("_FlashColor", prevColor);
		rend.SetPropertyBlock(block);
		flashTimer = 0f;
		if (repeatFlash)
		{
		    flashingState = 1;
		}
		else
		{
		    flashingState = 0;
		}
	    }
	}
	if (geoFlash)
	{
	    if(geoTimer > 0f)
	    {
		geoTimer -= Time.deltaTime;
		return;
	    }
	    FlashingSuperDash();
	    geoFlash = false;
	}
    }

    public void GeoFlash()
    {
	geoFlash = false;
	geoTimer = 0.25f;
    }

    public void flashInfected()
    {
	if (block == null)
	{
	    block = new MaterialPropertyBlock();
	}
	flashColour = new Color(1f, 0.31f, 0f);
	amount = 0.9f;
	timeUp = 0.01f;
	timeDown = 0.25f;
	block.Clear();
	block.SetColor("_FlashColor", flashColour);
	flashingState = 1;
	flashTimer = 0f;
	repeatFlash = false;
	SendToChildren(new Action(flashInfected));
    }

    public void flashInfectedLong()
    {
	flashColour = new Color(1f, 0.31f, 0f);
	amount = 0.9f;
	timeUp = 0.01f;
	stayTime = 0.25f;
	timeDown = 0.35f;
	block.Clear();
	block.SetColor("_FlashColor", flashColour);
	flashingState = 1;
	flashTimer = 0f;
	repeatFlash = false;
	//SendToChildren(new Action(flashInfectedLong));
    }

    public void flashArmoured()
    {
	flashColour = new Color(1f, 1f, 1f);
	amount = 0.9f;
	timeUp = 0.01f;
	stayTime = 0.01f;
	timeDown = 0.25f;
	if (block != null)
	{
	    block.Clear();
	    block.SetColor("_FlashColor", flashColour);
	}
	flashingState = 1;
	flashTimer = 0f;
	repeatFlash = false;
	SendToChildren(new Action(flashArmoured));
    }

    public void flashFocusHeal()
    {
	Start();
	flashColour = new Color(1f, 1f, 1f);
	amount = 0.85f;
	timeUp = 0.1f;
	stayTime = 0.01f;
	timeDown = 0.35f;
	block.Clear();
	block.SetColor("_FlashColor", flashColour);
	flashingState = 1;
	flashTimer = 0f;
	repeatFlash = false;
	//SendToChildren(new Action(flashFocusHeal));
    }

    public void flashBenchRest()
    {
	flashColour = new Color(1f, 1f, 1f);
	amount = 0.7f;
	timeUp = 0.01f;
	stayTime = 0.1f;
	timeDown = 0.75f;
	block.Clear();
	block.SetColor("_FlashColor", flashColour);
	flashingState = 1;
	flashTimer = 0f;
	repeatFlash = false;
	//SendToChildren(new Action(flashBenchRest));
    }

    private void flashFocusGet()
    {
	Start();
	flashColour = new Color(1f, 1f, 1f);
	amount = 0.5f;
	timeUp = 0.1f;
	stayTime = 0.01f;
	timeDown = 0.35f;
	block.Clear();
	block.SetColor("_FlashColor", flashColour);
	flashingState = 1;
	flashTimer = 0f;
	repeatFlash = false;
	//SendToChildren(new Action(flashFocusGet));
    }

    public void flashSoulGet()
    {
	flashColour = new Color(1f, 1f, 1f);
	amount = 0.5f;
	timeUp = 0.01f;
	stayTime = 0.01f;
	timeDown = 0.25f;
	block.Clear();
	block.SetColor("_FlashColor", flashColour);
	flashingState = 1;
	flashTimer = 0f;
	repeatFlash = false;
	//SendToChildren(new Action(flashSoulGet));
    }
    public void flashShadeGet()
    {
	flashColour = new Color(0f, 0f, 0f);
	amount = 0.5f;
	timeUp = 0.01f;
	stayTime = 0.01f;
	timeDown = 0.25f;
	block.Clear();
	block.SetColor("_FlashColor", flashColour);
	flashingState = 1;
	flashTimer = 0f;
	repeatFlash = false;
	//SendToChildren(new Action(flashShadeGet));
    }

    public void FlashingSuperDash()
    {
	flashColour = new Color(1f, 1f, 1f);
	amount = 0.7f;
	timeUp = 0.1f;
	stayTime = 0.01f;
	timeDown = 0.1f;
	block.Clear();
	block.SetColor("_FlashColor", flashColour);
	flashingState = 1;
	flashTimer = 0f;
	repeatFlash = true;
	//SendToChildren(new Action(FlashingSuperDash));
    }

    public void FlashingFury()
    {
	Start();
	flashColour = new Color(0.71f, 0.18f, 0.18f);
	amount = 0.75f;
	timeUp = 0.25f;
	stayTime = 0.01f;
	timeDown = 0.25f;
	block.Clear();
	block.SetColor("_FlashColor", flashColour);
	flashingState = 1;
	repeatFlash = true;
	//SendToChildren(new Action(FlashingFury));
    }
    public void CancelFlash()
    {
	cancelFlash = true;
    }
    private void SendToChildren(Action function)
    {
	if (!sendToChildren)
	    return;
	foreach (SpriteFlash spriteFlash in GetComponentsInChildren<SpriteFlash>())
	{
	    if(!(spriteFlash == null))
	    {
		spriteFlash.sendToChildren = false;
		spriteFlash.GetType().GetMethod(function.Method.Name).Invoke(spriteFlash, null);
	    }
	}
    }
}
