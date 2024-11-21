using System;
using TMPro;
using UnityEngine;

public class ChangeFontByLanguage : MonoBehaviour
{
    public TMP_FontAsset defaultFont;
    public bool onlyOnStart;
    private TextMeshPro tmpro;
    private float startFontSize;
    public FontScaleLangTypes fontScaleLangType;

    private void Awake()
    {
	tmpro = GetComponent<TextMeshPro>();
	if (tmpro)
	{
	    if(defaultFont == null)
	    {
		defaultFont = tmpro.font;
	    }
	    startFontSize = tmpro.fontSize;
	}
    }

    private void Start()
    {
	SetFont();
    }

    private void OnEnable()
    {
	if (!onlyOnStart)
	{
	    SetFont();
	}
    }

    private void SetFont()
    {
	if(tmpro == null)
	{
	    return;
	}
	//TODO:
	tmpro.fontSize = startFontSize;
	if (defaultFont != null)
	{
	    tmpro.font = defaultFont;
	}
    }

    public enum FontScaleLangTypes
    {
	None,
	AreaName,
	SubAreaName,
	WideMap,
	CreditsTitle,
	ExcerptAuthor
    }
}
