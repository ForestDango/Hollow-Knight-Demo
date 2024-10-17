using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFade : MonoBehaviour
{
    [Header("Fade On Scene Start")]
    [Space(6f)]
    [Tooltip("Type of fade to do on Start.")]
    public FadeTypes fadeOnStart;
    [Tooltip("The time in seconds to wait after Start before performing the delay.")]
    public float startDelay;
    [Tooltip("Time to perform fade in seconds on Start.")]

    private GUIStyle backgroundStyle = new GUIStyle();
    private Texture2D fadeTexture;
    private Color currentScreenOverlayColor = new Color(0f, 0f, 0f, 0f);
    private Color targetScreenOverlayColor = new Color(0f, 0f, 0f, 1f);
    private Color deltaColor = new Color(0f, 0f, 0f, 0f);
    private int fadeGUIDepth = -1000;

    public float fadeTime;

    private void Awake()
    {
	fadeTexture = new Texture2D(1, 1);
	backgroundStyle.normal.background = fadeTexture;
    }

    private IEnumerator Start()
    {
	if (fadeOnStart == FadeTypes.BLACK_TO_CLEAR)
	{
	    SetScreenOverlayColor(new Color(0f, 0f, 0f, 1f));
	}
	else if (fadeOnStart == FadeTypes.CLEAR_TO_BLACK)
	{
	    SetScreenOverlayColor(new Color(0f, 0f, 0f, 0f));
	}
	if(startDelay > 0f)
	{
	    yield return new WaitForSeconds(startDelay);
	}
	else
	{
	    yield return new WaitForEndOfFrame();
	}
	if(fadeOnStart == FadeTypes.BLACK_TO_CLEAR)
	{
	    FadeToTransparent(fadeTime);
	}
	else if(fadeOnStart == FadeTypes.CLEAR_TO_BLACK)
	{
	    FadeToBlack(fadeTime);
	}
    }

    private void OnGUI()
    {

    }

    public void SetScreenOverlayColor(Color newScreenOverlayColor)
    {
	currentScreenOverlayColor = newScreenOverlayColor;
	fadeTexture.SetPixel(0, 0, currentScreenOverlayColor);
	fadeTexture.Apply();
    }

    public void StartFade(Color newScreenOverlayColor, float fadeDuration)
    {
	if (fadeDuration <= 0f)
	{
	    SetScreenOverlayColor(newScreenOverlayColor);
	    return;
	}
	targetScreenOverlayColor = newScreenOverlayColor;
	deltaColor = (targetScreenOverlayColor - currentScreenOverlayColor) / (fadeDuration * 2f);
    }

    public void FadeToBlack(float duration)
    {
	SetScreenOverlayColor(new Color(0f, 0f, 0f, 0f));
	StartFade(new Color(0f, 0f, 0f, 1f), duration);
    }

    public void FadeToTransparent(float duration)
    {
	SetScreenOverlayColor(new Color(0f, 0f, 0f, 1f));
	StartFade(new Color(0f, 0f, 0f, 0f), duration);
    }

    public enum FadeTypes
    {
	NONE,
	BLACK_TO_CLEAR,
	CLEAR_TO_BLACK
    }
}
