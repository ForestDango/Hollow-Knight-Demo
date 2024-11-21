using System;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneManager : MonoBehaviour
{
    [Space(6f)]
    [Tooltip("This denotes the type of this scene, mainly if it is a gameplay scene or not.")]
    public SceneType sceneType;

    [Header("Gameplay Scene Settings")]
    [Tooltip("The area of the map this scene belongs to.")]
    [Space(6f)]
    public MapZone mapZone;

    [Tooltip("Determines if this area is currently windy.")]
    public bool isWindy;
    [Tooltip("Determines if this level experiences tremors.")]
    public bool isTremorZone;
    [Tooltip("Set environment type on scene entry. 0 = Dust, 1 = Grass, 2 = Bone, 3 = Spa, 4 = Metal, 5 = No Effect, 6 = Wet")]
    public int environmentType;
    public int darknessLevel;
    public bool noLantern;

    [Header("Camera Color Correction Curves")]
    [Range(0f, 5f)]
    public float saturation;
    public bool ignorePlatformSaturationModifiers;
    public AnimationCurve redChannel;
    public AnimationCurve greenChannel;
    public AnimationCurve blueChannel;

    [Header("Ambient Light")]
    [Tooltip("The default ambient light colour for this scene.")]
    [Space(6f)]
    public Color defaultColor;
    [Tooltip("The intensity of the ambient light in this scene.")]
    [Range(0f, 1f)]
    public float defaultIntensity;

    [Header("Hero Light")]
    [Tooltip("Color of the hero's light gradient (not point lights)")]
    [Space(6f)]
    public Color heroLightColor;

    [Header("Scene Border")]
    [Space(6f)]
    public GameObject borderPrefab;

    [Header("Scene Particles")]
    public bool noParticles;
    public MapZone overrideParticlesWith;

    [Header("Audio Snapshots")]
    [Space(6f)]
    public AudioMixerSnapshot enviroSnapshot;
    public AudioMixerSnapshot actorSnapshot;
    public AudioMixerSnapshot shadeSnapshot;
    public float transitionTime;

    [Header("Mapping")]
    [Space(6f)]
    public bool manualMapTrigger;

    [Header("Object Spawns")]
    [Space(6f)]
    public GameObject hollowShadeObject;

    private GameManager gm;
    private GameCameras gc;
    private HeroController heroCtrl;
    private PlayerData pd;

    private float enviroTimer;
    private bool enviroSent;
    private bool heroInfoSent;
    private bool setSaturation;
    private bool isGameplayScene;

    public static float AmbientIntesityMix = 0.5f;
    private const float SwitchConstant = 0.17f;
    private const float SwitchConstantGG = 0.1466f;
    private const float RegularConstant = 0.1466f;

    private void Start()
    {
	gm = GameManager.instance;
	gc = GameCameras.instance;
	pd = PlayerData.instance;
	if (gm.IsGameplayScene())
	{
	    isGameplayScene = true;
	    heroCtrl = HeroController.instance;
	}
	else
	{
	    isGameplayScene = false;
	}
	gc.colorCorrectionCurves.saturation = AdjustSaturation(saturation);
	gc.colorCorrectionCurves.redChannel = redChannel;
	gc.colorCorrectionCurves.greenChannel = greenChannel;
	gc.colorCorrectionCurves.blueChannel = blueChannel;
	gc.colorCorrectionCurves.UpdateParameters();
	gc.sceneColorManager.SaturationA = AdjustSaturation(saturation);
	gc.sceneColorManager.RedA = redChannel;
	gc.sceneColorManager.GreenA = greenChannel;
	gc.sceneColorManager.BlueA = blueChannel;
	SetLighting(defaultColor, defaultIntensity);
	gc.sceneColorManager.AmbientColorA = defaultColor;
	gc.sceneColorManager.AmbientIntensityA = defaultIntensity;
	if (isGameplayScene)
	{
	    if (heroCtrl != null)
	    {
		heroCtrl.heroLight.color = heroLightColor;
	    }
	    gc.sceneColorManager.HeroLightColorA = heroLightColor;
	}
	pd.environmentType = environmentType;
	pd.environmentTypeDefault = environmentType;
	if (GameManager.instance)
	{
	    GameManager.EnterSceneEvent temp = null;
	    temp += delegate ()
	    {
		AddSceneMapped();
		GameManager.instance.OnFinishedEnteringScene -= temp;
	    };
	    GameManager.instance.OnFinishedEnteringScene += temp;
	}
	else
	{
	    AddSceneMapped();
	}
	//TODO:Audio
	if (enviroSnapshot != null)
	{
	    enviroSnapshot.TransitionTo(transitionTime);
	}
	if (actorSnapshot != null)
	{
	    actorSnapshot.TransitionTo(transitionTime);
	}
	if (shadeSnapshot != null)
	{
	    shadeSnapshot.TransitionTo(transitionTime);
	}
	if(sceneType == SceneType.GAMEPLAY)
	{
	    GameObject gameObject = GameObject.FindGameObjectWithTag("Vignette");
	    if(gameObject != null)
	    {
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(gameObject, "Darkness Control");
		if (playMakerFSM)
		{
		    FSMUtility.SetInt(playMakerFSM, "Darkness Level", darknessLevel);
		}
		if (!noLantern)
		{
		    FSMUtility.LocateFSM(gameObject, "Darkness Control").SendEvent("RESET");
		}
		else
		{
		    FSMUtility.LocateFSM(gameObject, "Darkness Control").SendEvent("SCENE RESET NO LANTERN");
		    if (heroCtrl != null)
		    {
			//TODO:Lantern
		    }
		}
	    }
	}
	if (isGameplayScene)
	{
	    DrawBlackBorders();
	}
	if(pd.soulLimited && isGameplayScene && pd.shadeScene == gameObject.scene.name)
	{
	    GameObject gameObject2 = Instantiate(hollowShadeObject, new Vector3(pd.shadePositionX, pd.shadePositionY, 0.006f), Quaternion.identity);
	    gameObject2.transform.SetParent(transform, true);
	    gameObject2.transform.SetParent(null);
	}
	//TODO:Dream Gate
    }

    private void Update()
    {
	if (isGameplayScene)
	{
	    if (enviroTimer < 0.25f)
	    {
		enviroTimer += Time.deltaTime;
	    }
	    else if (!enviroSent && heroCtrl != null)
	    {
		heroCtrl.checkEnvironment();
		enviroSent = true;
	    }
	    if (!heroInfoSent && heroCtrl != null)
	    {
		heroCtrl.heroLight.material.SetColor("_Color", Color.white);
		heroCtrl.SetDarkness(darknessLevel);
		heroInfoSent = true;
	    }
	    if (!setSaturation)
	    {
		if (AdjustSaturation(saturation) != gc.colorCorrectionCurves.saturation)
		{
		    gc.colorCorrectionCurves.saturation = AdjustSaturation(saturation);
		}
		setSaturation = true;
	    }
	}
    }


    private void AddSceneMapped()
    {
	if (!pd.scenesVisited.Contains(gm.GetSceneNameString()) && !manualMapTrigger  && mapZone != MapZone.GODS_GLORY)
	{
	    pd.scenesVisited.Add(gm.GetSceneNameString());
	}
    }

    private void DrawBlackBorders()
    {
	GameObject gameObject = Instantiate(borderPrefab);
	gameObject.transform.SetPosition2D(gm.sceneWidth + 10f, gm.sceneHeight / 2f);
	gameObject.transform.localScale = new Vector2(20f, gm.sceneHeight + 40f);
	UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject, base.gameObject.scene);
	GameObject gameObject2 = Instantiate(borderPrefab);
	gameObject2.transform.SetPosition2D(-10f, gm.sceneHeight / 2f);
	gameObject2.transform.localScale = new Vector2(20f, gm.sceneHeight + 40f);
	UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject2, base.gameObject.scene);
	GameObject gameObject3 = Instantiate(borderPrefab);
	gameObject3.transform.SetPosition2D(gm.sceneWidth / 2f, gm.sceneHeight + 10f);
	gameObject3.transform.localScale = new Vector2(40f + gm.sceneWidth, 20f);
	UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject3, base.gameObject.scene);
	GameObject gameObject4 = Instantiate(borderPrefab);
	gameObject4.transform.SetPosition2D(gm.sceneWidth / 2f, -10f);
	gameObject4.transform.localScale = new Vector2(40f + gm.sceneWidth, 20f);
	UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject4, base.gameObject.scene);
    }

    private float AdjustSaturation(float originalSaturation)
    {
	return AdjustSaturationForPlatform(originalSaturation, new MapZone?(mapZone));
    }

    private static float AdjustSaturationForPlatform(float originalSaturation, MapZone? mapZone)
    {
	if(Application.platform == RuntimePlatform.Switch)
	{
	    if(mapZone != null)
	    {
		MapZone? mapZone2 = mapZone;
		MapZone mapZone3 = MapZone.GODS_GLORY;
		if (mapZone2.GetValueOrDefault() == mapZone3 & mapZone2 != null)
		{
		    return originalSaturation + SwitchConstantGG;
		}
	    }
	    return originalSaturation + SwitchConstantGG;
	}
	return originalSaturation + SwitchConstant;
    }

    public int GetDarknessLevel()
    {
	return darknessLevel;
    }

    public static void SetLighting(Color ambientLightColor, float ambientLightIntensity)
    {
	float num = Mathf.Lerp(1f, ambientLightIntensity, AmbientIntesityMix);
	RenderSettings.ambientLight = new Color(ambientLightColor.r * num, ambientLightColor.g * num, ambientLightColor.b * num, 1f);
	RenderSettings.ambientIntensity = 1f;
    }
}
