using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;

public class SceneParticlesController : MonoBehaviour
{
    public SceneParticles defaultParticles;
    public SceneParticles[] sceneParticles;
    private GameManager gm;
    private SceneManager sm;
    private GameCameras gc;
    private bool foundMatch;
    private MapZone sceneParticleZoneType;

    public void SceneInit()
    {
	BeginScene();
    }

    private void BeginScene()
    {
	gm = GameManager.instance;
	sm = gm.sm;
	if (sm == null)
	{
	    sm = FindObjectOfType<SceneManager>();
	}
	gc = GameCameras.instance;
	if(!gm.IsGameplayScene() || gm.IsCinematicScene())
	{
	    DisableParticles();
	    return;
	}
	if (!sm.noParticles)
	{
	    EnableParticles();
	    return;
	}
	DisableParticles();
    }

    public void EnableParticles()
    {
	foundMatch = false;
	if(sm.overrideParticlesWith == MapZone.NONE)
	{
	    sceneParticleZoneType = sm.mapZone; //如果不需要override的话那就正常 
	}
	else
	{
	    sceneParticleZoneType = sm.overrideParticlesWith;
	}
	for (int i = 0; i < sceneParticles.Length; i++)
	{
	    if(sceneParticles[i].mapZone == sceneParticleZoneType)
	    {
		if(sceneParticles[i].particleObject != null)
		{
		    foundMatch = true;
		    sceneParticles[i].particleObject.gameObject.SetActive(true);
		}
		else
		{
		    Debug.LogError("Trying to enable Particle Object for MapZone: " + sceneParticleZoneType.ToString() + " but Particle Object is not set.");
		}
	    }
	    else if(sceneParticles[i].particleObject != null)
	    {
		sceneParticles[i].particleObject.gameObject.SetActive(false);
	    }
	    else
	    {
		Debug.LogError("Trying to disable Particle Object for MapZone: " + sceneParticleZoneType.ToString() + " but Particle Object is not set.");
	    }
	}
	if (foundMatch)
	{
	    if(defaultParticles.particleObject != null)
	    {
		defaultParticles.particleObject.gameObject.SetActive(false);
		return;
	    }
	    Debug.LogError("Trying to disable Default Particle Object but Default Particle Object is not set.");
	    return;
	}
    }

    public void DisableParticles()
    {
	for (int i = 0; i < sceneParticles.Length; i++)
	{
	    if(sceneParticles[i].particleObject != null)
	    {
		sceneParticles[i].particleObject.gameObject.SetActive(false);
	    }
	    else
	    {
		Debug.LogError("Trying to disable Particle Object for MapZone: " + sceneParticleZoneType.ToString() + " but Particle Object is not set.");
	    }
	}
	if(defaultParticles.particleObject != null)
	{
	    defaultParticles.particleObject.gameObject.SetActive(false);
	    return;
	}
	Debug.LogError("Trying to disable Default Particle Object but Default Particle Object is not set.");
    }
}
