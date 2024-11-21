using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(ColorCorrectionCurves))]
public class SceneColorManager : MonoBehaviour
{
    public float Factor;
    public float SaturationA = 1f;
    public AnimationCurve RedA = new AnimationCurve(new Keyframe[]
    {
	new Keyframe(0f, 0f),
	new Keyframe(1f, 1f)
    });
    public AnimationCurve GreenA = new AnimationCurve(new Keyframe[]
    {
	new Keyframe(0f, 0f),
	new Keyframe(1f, 1f)
    });
    public AnimationCurve BlueA = new AnimationCurve(new Keyframe[]
    {
	new Keyframe(0f, 0f),
	new Keyframe(1f, 1f)
    });
    public Color AmbientColorA = Color.white;
    public float AmbientIntensityA = 1f;
    public Color HeroLightColorA = Color.white;
    public float SaturationB = 1f;
    public AnimationCurve RedB = new AnimationCurve(new Keyframe[]
    {
	new Keyframe(0f, 0f),
	new Keyframe(1f, 1f)
    });
    public AnimationCurve GreenB = new AnimationCurve(new Keyframe[]
    {
	new Keyframe(0f, 0f),
	new Keyframe(1f, 1f)
    });
    public AnimationCurve BlueB = new AnimationCurve(new Keyframe[]
    {
	new Keyframe(0f, 0f),
	new Keyframe(1f, 1f)
    });
    public Color AmbientColorB = Color.white;
    public float AmbientIntensityB = 1f;
    public Color HeroLightColorB = Color.white;

    private List<Keyframe[]> RedPairedKeyframes;
    private List<Keyframe[]> GreenPairedKeyframes;
    private List<Keyframe[]> BluePairedKeyframes;
    private ColorCorrectionCurves CurvesScript;
    private const float PAIRING_DISTANCE = 0.01f;
    private const float TANGENT_DISTANCE = 0.0012f;
    private const float UPDATE_RATE = 1f;
    private bool gameplayScene;

    private HeroController hero;
    private GameManager gm;

    private static List<Keyframe> tempA;
    private static List<Keyframe> tempB;
    private static List<Keyframe> finalFramesList;
    private static List<Keyframe[]> simplePairList;

    private bool ChangesInEditor = true;
    private float LastFactor;
    private float LastSaturationA;
    private float LastSaturationB;
    private float LastAmbientIntensityA;
    private float LastAmbientIntensityB;
    private float startBufferDuration = 0.5f;

    public bool markerActive { get; private set; }
    public bool startBufferActive { get; private set; }
    public void GameInit()
    {
	CurvesScript = GetComponent<ColorCorrectionCurves>();
	tempA = new List<Keyframe>(128);
	tempB = new List<Keyframe>(128);
	finalFramesList = new List<Keyframe>(128);
	simplePairList = new List<Keyframe[]>(128);
	gm = GameManager.instance;
	gm.UnloadingLevel += OnLevelUnload;
	UpdateSceneType();
	LastFactor = Factor;
	LastSaturationA = SaturationA;
	LastSaturationB = SaturationB;
	LastAmbientIntensityA = AmbientIntensityA;
	LastAmbientIntensityB = AmbientIntensityB;
    }

    private void OnLevelUnload()
    {
	Factor = 0f;
	markerActive = false;
    }

    public void SceneInit()
    {
	UpdateSceneType();
	if (!gameplayScene)
	{
	    Factor = 0f;
	    return;
	}
	startBufferActive = true;
	markerActive = true;
	UpdateScript(true);
	Invoke("FinishBufferPeriod", startBufferDuration);
    }

    private void Update()
    {
	if ((markerActive || startBufferActive) && Time.frameCount % UPDATE_RATE == 0f)
	{
	    UpdateScript(false);
	}
    }

    private void UpdateScript(bool forceUpdate = false)
    {
	if (CurvesScript == null)
	{
	    GetComponent<ColorCorrectionCurves>();
	}
	if (!PairedListsInitiated())
	{
	    PairCurvesKeyframes();
	}
	if (ChangesInEditor)
	{
	    PairCurvesKeyframes();
	    UpdateScriptParameters();
	    CurvesScript.UpdateParameters();
	    ChangesInEditor = false;
	    return;
	}
	if (forceUpdate)
	{
	    PairCurvesKeyframes();
	    UpdateScriptParameters();
	    CurvesScript.UpdateParameters();
	    return;
	}
	if (Factor != LastFactor || SaturationA != LastSaturationA || SaturationB != LastSaturationB || AmbientIntensityA != LastAmbientIntensityA || AmbientIntensityB != LastAmbientIntensityB)
	{
	    UpdateScriptParameters();
	    CurvesScript.UpdateParameters();
	    LastFactor = Factor;
	    LastSaturationA = SaturationA;
	    LastSaturationB = SaturationB;
	    LastAmbientIntensityA = AmbientIntensityA;
	    LastAmbientIntensityB = AmbientIntensityB;
	}
    }

    private void UpdateScriptParameters()
    {
	if (CurvesScript == null)
	{
	    CurvesScript = GetComponent<ColorCorrectionCurves>();
	}
	Factor = Mathf.Clamp01(Factor);
	SaturationA = Mathf.Clamp(SaturationA, 0f, 5f);
	SaturationB = Mathf.Clamp(SaturationB, 0f, 5f);
	CurvesScript.saturation = Mathf.Lerp(SaturationA, SaturationB, Factor);
	CurvesScript.redChannel = CreateCurveFromKeyframes(RedPairedKeyframes, Factor);
	CurvesScript.greenChannel = CreateCurveFromKeyframes(GreenPairedKeyframes, Factor);
	CurvesScript.blueChannel = CreateCurveFromKeyframes(BluePairedKeyframes, Factor);
	SceneManager.SetLighting(Color.Lerp(AmbientColorA, AmbientColorB, Factor), Mathf.Lerp(AmbientIntensityA, AmbientIntensityB, Factor));
	if (gameplayScene)
	{
	    if (hero == null)
	    {
		hero = HeroController.instance;
	    }
	    hero.heroLight.color = Color.Lerp(HeroLightColorA, HeroLightColorB, Factor);
	}
    }

    private bool PairedListsInitiated()
    {
	return RedPairedKeyframes != null && GreenPairedKeyframes != null && BluePairedKeyframes != null;
    }

    private void PairCurvesKeyframes()
    {
	RedPairedKeyframes = PairKeyframes(RedA, RedB);
	GreenPairedKeyframes = PairKeyframes(GreenA, GreenB);
	BluePairedKeyframes = PairKeyframes(BlueA, BlueB);
    }

    private void UpdateSceneType()
    {
	if (gm == null)
	{
	    gm = GameManager.instance;
	}
	if (gm.IsGameplayScene())
	{
	    gameplayScene = true;
	    if (hero == null)
	    {
		hero = HeroController.instance;
		return;
	    }
	}
	else
	{
	    gameplayScene = false;
	}
    }
    public static AnimationCurve CreateCurveFromKeyframes(IList<Keyframe[]> keyframePairs, float factor)
    {
	finalFramesList.Clear();
	for (int i = 0; i < keyframePairs.Count; i++)
	{
	    Keyframe[] array = keyframePairs[i];
	    finalFramesList.Add(AverageKeyframe(array[0], array[1], factor));
	}
	return new AnimationCurve(finalFramesList.ToArray());
    }

    public static Keyframe AverageKeyframe(Keyframe a, Keyframe b, float factor)
    {
	return new Keyframe
	{
	    time = a.time * (UPDATE_RATE - factor) + b.time * factor,
	    value = a.value * (UPDATE_RATE - factor) + b.value * factor,
	    inTangent = a.inTangent * (UPDATE_RATE - factor) + b.inTangent * factor,
	    outTangent = a.outTangent * (UPDATE_RATE - factor) + b.outTangent * factor
	};
    }

    public static List<Keyframe[]> PairKeyframes(AnimationCurve curveA, AnimationCurve curveB)
    {
	if (curveA.length == curveB.length)
	{
	    return SimplePairKeyframes(curveA, curveB);
	}
	List<Keyframe[]> list = new List<Keyframe[]>();
	tempA.Clear();
	tempA.AddRange(curveA.keys);
	tempB.Clear();
	tempB.AddRange(curveB.keys);
	int i = 0;
	bool flag = false;
	Keyframe aKeyframe = tempA[i];
	Predicate<Keyframe> predicate;
	while (i < tempA.Count)
	{
	    if (flag)
	    {
		aKeyframe = tempA[i];
	    }
	    List<Keyframe> list2 = tempB;
	    Predicate<Keyframe> match;
	    match = (predicate = ((Keyframe bKeyframe) => Mathf.Abs(aKeyframe.time - bKeyframe.time) < PAIRING_DISTANCE));	    
	    int num = list2.FindIndex(match);
	    if (num >= 0)
	    {
		Keyframe[] item = new Keyframe[]
		{
		    tempA[i],
		    tempB[num]
		};
		list.Add(item);
		tempA.RemoveAt(i);
		tempB.RemoveAt(num);
		flag = false;
	    }
	    else
	    {
		i++;
		flag = true;
	    }
	}
	for (int j = 0; j < tempA.Count; j++)
	{
	    Keyframe keyframe = CreatePair(tempA[j], curveB);
	    list.Add(new Keyframe[]
	    {
		tempA[j],
		keyframe
	    });
	}
	for (int k = 0; k < tempB.Count; k++)
	{
	    Keyframe keyframe2 = CreatePair(tempB[k], curveA);
	    list.Add(new Keyframe[]
	    {
		keyframe2,
		tempB[k]
	    });
	}
	return list;
    }

    private static List<Keyframe[]> SimplePairKeyframes(AnimationCurve curveA, AnimationCurve curveB)
    {
	if (curveA.length != curveB.length)
	{
	    throw new UnityException("Simple Pair cannot work with curves with different number of Keyframes.");
	}
	List<Keyframe[]> list = new List<Keyframe[]>();
	Keyframe[] keys = curveA.keys;
	Keyframe[] keys2 = curveB.keys;
	for (int i = 0; i < curveA.length; i++)
	{
	    list.Add(new Keyframe[]
	    {
		keys[i],
		keys2[i]
	    });
	}
	return list;
    }

    private static Keyframe CreatePair(Keyframe kf, AnimationCurve curve)
    {
	Keyframe result = default(Keyframe);
	result.time = kf.time;
	result.value = curve.Evaluate(kf.time);
	if (kf.time >= TANGENT_DISTANCE)
	{
	    float num = kf.time - TANGENT_DISTANCE;
	    result.inTangent = (curve.Evaluate(num) - curve.Evaluate(kf.time)) / (num - kf.time);
	}
	if (kf.time + TANGENT_DISTANCE <= UPDATE_RATE)
	{
	    float num2 = kf.time + TANGENT_DISTANCE;
	    result.outTangent = (curve.Evaluate(num2) - curve.Evaluate(kf.time)) / (num2 - kf.time);
	}
	return result;
    }
    private void FinishBufferPeriod()
    {
	UpdateScript(true);
	startBufferActive = false;
    }

    public void SetFactor(float factor)
    {
	Factor = factor;
    }
    public void SetSaturationA(float saturationA)
    {
	SaturationA = saturationA;
    }
    public void SetSaturationB(float saturationB)
    {
	SaturationB = saturationB;
    }
}
