using System;

using UnityEngine;
using UnityEngine.UI;

public class LogoLanguage : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    [Space]
    public Image uiImage;
    public bool setNativeSize = true;

    [Space]
    public Sprite englishSprite;
    public Sprite chineseSprite;

    private void OnEnable()
    {
	SetSprite();
    }

    private void SetSprite()
    {
	if (spriteRenderer)
	{
	    spriteRenderer.sprite = englishSprite;
	}
	if(uiImage)
	{
	    uiImage.sprite = englishSprite;
	}
	if(uiImage && setNativeSize)
	{
	    uiImage.SetNativeSize();
	}
    }
}
