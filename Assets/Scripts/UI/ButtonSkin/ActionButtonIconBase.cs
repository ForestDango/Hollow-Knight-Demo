using System;
using GlobalEnums;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class ActionButtonIconBase : MonoBehaviour
{
    [Header("Optional")]
    [Tooltip("This will update the button skin to reflect the currently active controller at all times.")]
    public bool liveUpdate;
    public TextMeshPro label;
    public TextContainer textContainer;
    protected SpriteRenderer sr;
    private InputHandler ih;
    private float blnkWidth = 1.685f;
    private float blnkHeight = 0.6f;
    private float blnkFontMax = 9.5f;
    private float blnkFontMin = 4f;
    private float sqrWidth = 0.7f;
    private float sqrHeight = 0.8f;
    private float sqrFontMax = 9.5f;
    private float sqrFontMin = 3.35f;
    private float wideWidth = 1.685f;
    private float wideHeight = 0.7f;
    private float wideFontMax = 9.5f;
    private float wideFontMin = 4f;

    public delegate void IconUpdateEvent();
    public event IconUpdateEvent OnIconUpdate;
    public abstract HeroActionButton Action { get; }
    protected void GetButtonIcon(HeroActionButton actionButton)
    {
	//TODO:UIBS
	ButtonSkin buttonSkinFor = null;
	if (buttonSkinFor == null)
	{
	    Debug.LogError("Couldn't get button skin for " + actionButton.ToString(), this);
	    return;
	}
	sr.sprite = buttonSkinFor.sprite;
	if (textContainer != null)
	{
	    if (buttonSkinFor.skinType == ButtonSkinType.BLANK)
	    {
		textContainer.width = blnkWidth;
		textContainer.height = blnkHeight;
	    }
	    else if (buttonSkinFor.skinType == ButtonSkinType.SQUARE)
	    {
		textContainer.width = sqrWidth;
		textContainer.height = sqrHeight;
	    }
	    else if (buttonSkinFor.skinType == ButtonSkinType.WIDE)
	    {
		textContainer.width = wideWidth;
		textContainer.height = wideHeight;
	    }
	}
	if (label != null)
	{
	    if (buttonSkinFor.skinType == ButtonSkinType.BLANK)
	    {
		label.fontSizeMin = blnkFontMin;
		label.fontSizeMax = blnkFontMax;
	    }
	    else if (buttonSkinFor.skinType == ButtonSkinType.SQUARE)
	    {
		label.fontSizeMin = sqrFontMin;
		label.fontSizeMax = sqrFontMax;
	    }
	    else if (buttonSkinFor.skinType == ButtonSkinType.WIDE)
	    {
		label.fontSizeMin = wideFontMin;
		label.fontSizeMax = wideFontMax;
	    }
	    label.text = buttonSkinFor.symbol;
	}
	if (OnIconUpdate != null)
	{
	    OnIconUpdate();
	}
    }

    protected virtual void OnEnable()
    {
	if (ih == null)
	{
	    ih = GameManager.instance.inputHandler;
	}
	if (ih != null)
	{
	    ih.RefreshActiveControllerEvent += RefreshController;
	}
	RefreshButtonIcon();
    }
    protected virtual void OnDisable()
    {
	if (ih != null)
	{
	    ih.RefreshActiveControllerEvent -= RefreshController;
	}
    }

    public void RefreshController()
    {
	if (liveUpdate)
	{
	    RefreshButtonIcon();
	}
    }

    public void RefreshButtonIcon()
    {
	//TODO:Refresh Button Icon
	Debug.LogFormat("TODO:Refresh Button Icon");
    }
}
