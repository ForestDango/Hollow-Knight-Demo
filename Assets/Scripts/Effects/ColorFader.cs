using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ColorFader : MonoBehaviour
{
    public Color downColour = new Color(1f, 1f, 1f, 0f);
    public float downTime = 0.4f;
    public Color upColour = new Color(1f, 1f, 1f, 1f);
    public float upDelay;
    public float upTime = 0.4f;
    private Color initialColour;
    public bool useInitialColour = true;

    private SpriteRenderer spriteRenderer;
    private TextMeshPro textRenderer;
    private tk2dSprite tk2dSprite;
    private bool setup;

    private Coroutine fadeRoutine;

    public delegate void FadeEndEvent(bool up); 
    public event FadeEndEvent OnFadeEnd;

    private void Reset()
    {
	foreach (PlayMakerFSM playMakerFSM  in GetComponents<PlayMakerFSM>())
	{
	    if ((playMakerFSM.FsmTemplate ? playMakerFSM.FsmTemplate.name : playMakerFSM.FsmName) == "color_fader")
	    {
		downColour = playMakerFSM.FsmVariables.GetFsmColor("Down Colour").Value;
		downTime = playMakerFSM.FsmVariables.GetFsmFloat("Down Time").Value;
		upColour = playMakerFSM.FsmVariables.GetFsmColor("Up Colour").Value;
		upDelay = playMakerFSM.FsmVariables.GetFsmFloat("Up Delay").Value;
		upTime = playMakerFSM.FsmVariables.GetFsmFloat("Up Time").Value;
		return;
	    }
	}
    }

    private void Start()
    {
	Setup();
    }

    private void Setup()
    {
	if (!setup)
	{
	    setup = true;
	    if (!spriteRenderer)
	    {
		spriteRenderer = GetComponent<SpriteRenderer>();
	    }
	    if (spriteRenderer)
	    {
		initialColour = (useInitialColour ? spriteRenderer.color : Color.white);
		spriteRenderer.color = downColour * initialColour;
		return;
	    }
	    if (!textRenderer)
	    {
		textRenderer = GetComponent<TextMeshPro>();
	    }
	    if (textRenderer)
	    {
		initialColour = (useInitialColour ? textRenderer.color : Color.white);
		textRenderer.color = downColour * initialColour;
		return;
	    }
	    if (!tk2dSprite)
	    {
		tk2dSprite = GetComponent<tk2dSprite>();
	    }
	    if (tk2dSprite)
	    {
		initialColour = (useInitialColour ? tk2dSprite.color : Color.white);
		tk2dSprite.color = downColour * initialColour;
	    }
	}
    }

    public void Fade(bool up)
    {
	Setup();
	if (fadeRoutine != null)
	{
	    StopCoroutine(fadeRoutine);
	}
	if (up)
	{
	    fadeRoutine = StartCoroutine(Fade(upColour, upTime, upDelay));
	    return;
	}
	fadeRoutine = StartCoroutine(Fade(downColour, downTime, 0f));
    }

    private IEnumerator Fade(Color to, float time, float delay)
    {
	if (!spriteRenderer)
	{
	    spriteRenderer = GetComponent<SpriteRenderer>();
	}
	Color from = spriteRenderer ? spriteRenderer.color : (textRenderer ? textRenderer.color : (tk2dSprite ? tk2dSprite.color : Color.white));
	if (delay > 0f)
	{
	    yield return new WaitForSeconds(upDelay);
	}
	for (float elapsed = 0f; elapsed < time; elapsed += Time.deltaTime)
	{
	    Color color = Color.Lerp(from, to, elapsed / time) * initialColour;
	    if (spriteRenderer)
	    {
		spriteRenderer.color = color;
	    }
	    else if (textRenderer)
	    {
		textRenderer.color = color;
	    }
	    else if (tk2dSprite)
	    {
		tk2dSprite.color = color;
	    }
	    yield return null;
	}
	if (spriteRenderer)
	{
	    spriteRenderer.color = to * initialColour;
	}
	else if (textRenderer)
	{
	    textRenderer.color = to * initialColour;
	}
	else if (tk2dSprite)
	{
	    tk2dSprite.color = to * initialColour;
	}
	if (OnFadeEnd != null)
	{
	    OnFadeEnd(to == upColour);
	}
    }

}
