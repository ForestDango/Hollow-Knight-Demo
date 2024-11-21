using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Custom/Brightness Effect")]
public class BrightnessEffect : ImageEffectBase
{
    [Range(0f, 2f)]
    public float _Brightness = 1f;
    [Range(0f, 2f)]
    public float _Contrast = 1f;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
	base.material.SetFloat("_Brightness", _Brightness);
	base.material.SetFloat("_Contrast", _Contrast);
	Graphics.Blit(source, destination, base.material);

    }

    public void SetBrightness(float value)
    {
	_Brightness = value;
    }
    public void SetContrast(float value)
    {
	_Contrast = value;
    }
}
