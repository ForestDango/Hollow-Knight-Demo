using System;
using UnityEngine;

public class GlobalPrefabDefaults : MonoBehaviour
{
    public static GlobalPrefabDefaults Instance;
    public GameObject bloodSplatterParticle;
    public float speedMultiplier = 1.2f;
    public float amountMultiplier = 1.3f;
    private ParticleSystem.MinMaxGradient initialBloodColour;

    private void Awake()
    {
	Instance = this;
    }

    private void Start()
    {
	if (bloodSplatterParticle)
	{
	    ParticleSystem system = bloodSplatterParticle.GetComponent<ParticleSystem>();
	    if (system)
	    {
		initialBloodColour = system.main.startColor;
	    }
	}
    }

    public void SpawnBlood(Vector3 position, short minCount, short maxCount, float minSpeed, float maxSpeed, float angleMin = 0f, float angleMax = 360f, Color? colorOverride = null)
    {
	if (bloodSplatterParticle)
	{
	    ParticleSystem component = bloodSplatterParticle.Spawn().GetComponent<ParticleSystem>();
	    if (component)
	    {
		component.Stop();
		component.emission.SetBursts(new ParticleSystem.Burst[]
		{
		    new ParticleSystem.Burst(0f, (short)Mathf.RoundToInt(minCount * amountMultiplier), (short)Mathf.RoundToInt(maxCount * amountMultiplier))
		});
		ParticleSystem.MainModule main = component.main;
		main.maxParticles = Mathf.RoundToInt(maxCount * amountMultiplier);
		main.startSpeed = new ParticleSystem.MinMaxCurve(minSpeed * speedMultiplier, maxSpeed * speedMultiplier);
		if (colorOverride == null)
		{
		    main.startColor = initialBloodColour;
		}
		else
		{
		    main.startColor = new ParticleSystem.MinMaxGradient(colorOverride.Value);
		}
		//component.shape.arc = angleMax - angleMin;
		component.transform.SetRotation2D(angleMin);
		component.transform.position = position;
		component.Play();
	    }
	}
    }

}
