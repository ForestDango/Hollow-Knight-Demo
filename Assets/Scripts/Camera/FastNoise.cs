using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Noise/Fast Noise")]
    public class FastNoise : PostEffectsBase
    {
	private bool monochrome = true;
	[Header("Update Rate")]
	public FrameMultiple frameRateMultiplier = FrameMultiple.Always;
	[Header("Intensity")]
	public float intensityMultiplier = 0.25f;
	public float generalIntensity = 0.5f;
	public float blackIntensity = 1f;
	public float whiteIntensity = 1f;
	[Range(0f, 1f)]
	public float midGrey = 0.2f;
	[Header("Noise Shape")]
	public Texture2D noiseTexture;
	public FilterMode filterMode = FilterMode.Bilinear;
	[Range(0f, 0.99f)]
	private Vector3 intensities = new Vector3(1f, 1f, 1f);
	[Range(0f, 0.99f)]
	public float softness = 0.052f;

	[Header("Advanced")]
	public float monochromeTiling = 64f;
	public Shader noiseShader;
	private Material noiseMaterial;
	private static float TILE_AMOUNT = 64f;
	private byte frameCount;
	private RenderTexture softnessTexture;
	protected void OnDisable()
	{
	    if (softnessTexture != null)
	    {
		Destroy(softnessTexture);
		softnessTexture = null;
	    }
	}

	public override bool CheckResources()
	{
	    CheckSupport(false);
	    noiseMaterial = CheckShaderAndCreateMaterial(noiseShader, noiseMaterial);
	    if (!isSupported)
	    {
		ReportAutoDisable();
	    }
	    return isSupported;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
	    if (Application.isEditor && !Application.isPlaying)
	    {
		return;
	    }
	    if (!CheckResources() || null == noiseTexture)
	    {
		Graphics.Blit(source, destination);
		if (null == noiseTexture)
		{
		    Debug.LogWarning("FastNoise effect failing as noise texture is not assigned. please assign.", transform);
		}
		return;
	    }
	    softness = Mathf.Clamp(softness, 0f, 0.99f);
	    if (noiseTexture)
	    {
		noiseTexture.wrapMode = TextureWrapMode.Repeat;
		noiseTexture.filterMode = filterMode;
	    }
	    noiseMaterial.SetTexture("_NoiseTex", noiseTexture);
	    noiseMaterial.SetVector("_NoisePerChannel", monochrome ? Vector3.one : intensities);
	    noiseMaterial.SetVector("_NoiseTilingPerChannel", Vector3.one * monochromeTiling);
	    noiseMaterial.SetVector("_MidGrey", new Vector3(midGrey, 1f / (1f - midGrey), -1f / midGrey));
	    noiseMaterial.SetVector("_NoiseAmount", new Vector3(generalIntensity, blackIntensity, whiteIntensity) * intensityMultiplier);
	    if (softness > Mathf.Epsilon)
	    {
		int num = (int)(source.width * (1f - softness));
		int num2 = (int)(source.height * (1f - softness));
		if (softnessTexture != null && (softnessTexture.width != num || softnessTexture.height != num2))
		{
		    Destroy(softnessTexture);
		    softnessTexture = null;
		}
		if (softnessTexture == null)
		{
		    softnessTexture = new RenderTexture(num, num2, 0);
		}
		DrawNoiseQuadGrid(source, softnessTexture, noiseMaterial, noiseTexture, 2, (int)frameRateMultiplier);
		noiseMaterial.SetTexture("_NoiseTex", softnessTexture);
		Graphics.Blit(source, destination, noiseMaterial, 1);
	    }
	    else
	    {
		DrawNoiseQuadGrid(source, destination, noiseMaterial, noiseTexture, 0, (int)frameRateMultiplier);
	    }
	    frameCount += 1;
	}

	private static void DrawNoiseQuadGrid(RenderTexture source, RenderTexture dest, Material fxMaterial, Texture2D noise, int passNr, int frameMultiple)
	{
	    if (Time.frameCount % frameMultiple != 0)
	    {
		return;
	    }
	    RenderTexture.active = dest;
	    float num = noise.width * 1f;
	    float num2 = 1f * source.width / TILE_AMOUNT;
	    fxMaterial.SetTexture("_MainTex", source);
	    GL.PushMatrix();
	    GL.LoadOrtho();
	    float num3 = 1f * source.width / (1f * source.height);
	    float num4 = 1f / num2;
	    float num5 = num4 * num3;
	    float num6 = num / (noise.width * 1f);
	    fxMaterial.SetPass(passNr);
	    GL.Begin(7);
	    for (float num7 = 0f; num7 < 1f; num7 += num4)
	    {
		for (float num8 = 0f; num8 < 1f; num8 += num5)
		{
		    float num9 = Random.Range(0f, 1f);
		    float num10 = Random.Range(0f, 1f);
		    num9 = Mathf.Floor(num9 * num) / num;
		    num10 = Mathf.Floor(num10 * num) / num;
		    float num11 = 1f / num;
		    GL.MultiTexCoord2(0, num9, num10);
		    GL.MultiTexCoord2(1, 0f, 0f);
		    GL.Vertex3(num7, num8, 0.1f);
		    GL.MultiTexCoord2(0, num9 + num6 * num11, num10);
		    GL.MultiTexCoord2(1, 1f, 0f);
		    GL.Vertex3(num7 + num4, num8, 0.1f);
		    GL.MultiTexCoord2(0, num9 + num6 * num11, num10 + num6 * num11);
		    GL.MultiTexCoord2(1, 1f, 1f);
		    GL.Vertex3(num7 + num4, num8 + num5, 0.1f);
		    GL.MultiTexCoord2(0, num9, num10 + num6 * num11);
		    GL.MultiTexCoord2(1, 0f, 1f);
		    GL.Vertex3(num7, num8 + num5, 0.1f);
		}
	    }
	    GL.End();
	    GL.PopMatrix();
	}
	public enum FrameMultiple
	{
	    Always = 1,
	    Half,
	    Third,
	    Quarter,
	    Fifth,
	    Sixth,
	    Eighth = 8,
	    Tenth = 10
	}
    }
}