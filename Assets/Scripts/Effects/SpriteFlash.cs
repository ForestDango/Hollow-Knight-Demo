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

    private void flashFocusHeal()
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
	SendToChildren(new Action(flashFocusGet));
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
