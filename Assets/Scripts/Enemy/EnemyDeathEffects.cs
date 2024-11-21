using System;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyDeathEffects : MonoBehaviour
{
    [SerializeField] private GameObject corpsePrefab;
    [SerializeField] private bool corpseFacesRight;
    [SerializeField] private float corpseFlingSpeed;
    [SerializeField] public Vector3 corpseSpawnPoint;
    [SerializeField] private string deathBroadcastEvent;
    [SerializeField] private Vector3 effectOrigin;
    [SerializeField] private bool lowCorpseArc;

    [SerializeField] private EnemyDeathTypes enemyDeathType;
    [SerializeField] protected AudioSource audioPlayerPrefab;
    [SerializeField] protected AudioEvent enemyDeathSwordAudio;
    [SerializeField] protected AudioEvent enemyDamageAudio;
    [SerializeField] protected AudioClip enemyDeathSwordClip;
    [SerializeField] protected AudioClip enemyDamageClip;
    [SerializeField] private AudioMixerSnapshot audioSnapshotOnDeath;

    [SerializeField] protected GameObject deathWaveInfectedPrefab;
    [SerializeField] protected GameObject deathWaveInfectedSmallPrefab;

    [SerializeField] private bool recycle;
    [SerializeField] private bool rotateCorpse; //尸体需要旋转吗

    [SerializeField] protected GameObject dustPuffMedPrefab;
    [SerializeField] protected GameObject deathPuffLargePrefab;

    protected GameObject corpse;
    private bool didFire;
    [HideInInspector]
    public bool doKillFreeze = true;
    protected void Start()
    {
	PreInstantiate();
    }

    public void PreInstantiate()
    {
	if(!corpse && corpsePrefab)
	{
	    corpse = Instantiate(corpsePrefab, transform.position + corpseSpawnPoint, Quaternion.identity,transform);
	    Debug.LogFormat("Corpse Position is " + transform.position);
	    tk2dSprite[] componentInChildrens = corpse.GetComponentsInChildren<tk2dSprite>(true);
	    for (int i = 0; i < componentInChildrens.Length; i++)
	    {
		componentInChildrens[i].ForceBuild();
	    }
	    corpse.SetActive(false);
	}
    }

    public void RecieveDeathEvent(float? attackDirection, bool resetDeathEvent = false, bool spellBurn = false, bool isWatery = false)
    {
	if (didFire)
	    return;
	didFire = true;
	
	if(corpse != null)
	{
	    EmitCorpse(attackDirection, isWatery, spellBurn);
	}
	if (!isWatery)
	{
	    EmitEffects();
	}
	if (doKillFreeze)
	{
	    GameManager.instance.FreezeMoment(1);
	}
	if (enemyDeathType == EnemyDeathTypes.Infected || enemyDeathType == EnemyDeathTypes.LargeInfected || enemyDeathType == EnemyDeathTypes.SmallInfected || enemyDeathType == EnemyDeathTypes.Uninfected )
	{
	    EmitEssence();
	}
	if (audioSnapshotOnDeath != null)
	{
	    audioSnapshotOnDeath.TransitionTo(2f);
	}
	if (!string.IsNullOrEmpty(deathBroadcastEvent))
	{
	    Debug.LogWarningFormat(this, "Death broadcast event '{0}' not implemented!", new object[]
	    {
		deathBroadcastEvent
	    });
	}
	if (resetDeathEvent)
	{
	    FSMUtility.SendEventToGameObject(gameObject, "CENTIPEDE DEATH", false);
	    didFire = false;
	    return;
	}
	if (recycle)
	{
	    PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(gameObject, "health_manager_enemy");
	    if(playMakerFSM != null)
	    {
		playMakerFSM.FsmVariables.GetFsmBool("Activated").Value = false;
	    }
	    HealthManager component2 = GetComponent<HealthManager>();
	    if(component2 != null)
	    {
		component2.SetIsDead(false);
	    }
	    didFire = false;
	    gameObject.Recycle();
	    return;
	}
	Destroy(gameObject);
    }

    private void EmitCorpse(float? attackDirection, bool isWatery, bool spellBurn)
    {
	if (corpse == null)
	    return;
	corpse.transform.SetParent(null);
	corpse.transform.SetPositionZ(UnityEngine.Random.Range(-0.008f, -0.009f));
	corpse.SetActive(true);
	PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(corpse, "corpse");
	if(playMakerFSM != null)
	{
	    FsmBool fsmBool = playMakerFSM.FsmVariables.GetFsmBool("spellBurn");
	    if(fsmBool!= null)
	    {
		fsmBool.Value = false;
	    }
	}
	Corpse component = corpse.GetComponent<Corpse>();
	if (component)
	{
	    component.Setup(isWatery, spellBurn);
	}
	if (isWatery)
	{
	    return;
	}
	corpse.transform.SetRotation2D(rotateCorpse ? transform.GetRotation2D():0f);
	if(Mathf.Abs(transform.eulerAngles.z) >= 45f)
	{
	    Collider2D component2 = GetComponent<Collider2D>();
	    Collider2D component3 = corpse.GetComponent<Collider2D>();
	    if(!rotateCorpse && component2 && component3)
	    {
		Vector3 b = component2.bounds.center - component3.bounds.center;
		b.z = 0f;
		corpse.transform.position += b;
	    }
	}
	float d = 1f;
	if(attackDirection == null)
	{
	    d = 0f;
	}
	int cardinalDirection = DirectionUtils.GetCardinalDirection(attackDirection.GetValueOrDefault());
	Rigidbody2D component4 = corpse.GetComponent<Rigidbody2D>();
	if(component4 != null && !component4.isKinematic)
	{
	    float num = corpseFlingSpeed;
	    float num2;
	    switch (cardinalDirection)
	    {
		case 0:
		    num2 = lowCorpseArc ? 10f : 60f;
		    corpse.transform.SetScaleX(corpse.transform.localScale.x * (corpseFacesRight ? -1f : 1f) * Mathf.Sign(transform.localScale.x));
		    break;
		case 1:
		    num2 = UnityEngine.Random.Range(75f, 105f);
		    num *= 1.3f;
		    break;
		case 2:
		    num2 = lowCorpseArc ? 170f : 120f;
		    corpse.transform.SetScaleX(corpse.transform.localScale.x * (corpseFacesRight ? 1f : -1f) * Mathf.Sign(transform.localScale.x));
		    break;
		case 3:
		    num2 = 270f;
		    break;
		default:
		    num2 = 90f;
		    break;
	    }
	    component4.velocity = new Vector2(Mathf.Cos(num2 * 0.017453292f), Mathf.Sin(num2 * 0.017453292f)) * num * d;
	}
    }

    private void EmitEffects()
    {
	EnemyDeathTypes enemyDeathTypes = enemyDeathType;
	if(enemyDeathTypes == EnemyDeathTypes.Infected)
	{
	    EmitInfectedEffects();
	    return;
	}
	if (enemyDeathTypes == EnemyDeathTypes.SmallInfected)
	{
	    EmitSmallInfectedEffects();
	    return;
	}
	if (enemyDeathTypes != EnemyDeathTypes.LargeInfected)
	{
	    Debug.LogWarningFormat(this, "Enemy death type {0} not implemented!", new object[]
	    {
		enemyDeathType
	    });
	    return;
	}
	EmitLargeInfectedEffects();
    }

    private void EmitLargeInfectedEffects()
    {
	AudioEvent audioEvent = default(AudioEvent);
	audioEvent.Clip = enemyDeathSwordClip;
	audioEvent.PitchMin = 0.75f;
	audioEvent.PitchMax = 0.75f;
	audioEvent.Volume = 1f;
	audioEvent.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	audioEvent = default(AudioEvent);
	audioEvent.Clip = enemyDamageClip;
	audioEvent.PitchMin = 0.75f;
	audioEvent.PitchMax = 0.75f;
	audioEvent.Volume = 1f;
	audioEvent.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	if(corpse != null)
	{
	    SpriteFlash component = corpse.GetComponent<SpriteFlash>();
	    if(component != null)
	    {
		component.flashInfected();
	    }
	}
	if (!(deathPuffLargePrefab == null))
	{
	    Instantiate(deathPuffLargePrefab, transform.position + effectOrigin, Quaternion.identity);
	}
	ShakeCameraIfVisible("AverageShake");
	if (!(deathWaveInfectedPrefab == null))
	{
	    GameObject gameObject = Instantiate(deathWaveInfectedPrefab, transform.position + effectOrigin, Quaternion.identity);
	    gameObject.transform.SetScaleX(2f);
	    gameObject.transform.SetScaleY(2f);
	}
	GlobalPrefabDefaults.Instance.SpawnBlood(transform.position + effectOrigin, 75, 80, 20f, 25f, 0f, 360f, null);
    }

    private void EmitSmallInfectedEffects()
    {
	AudioEvent audioEvent = default(AudioEvent);
	audioEvent.Clip = enemyDeathSwordClip;
	audioEvent.PitchMin = 1.2f;
	audioEvent.PitchMax = 1.4f;
	audioEvent.Volume = 1f;
	audioEvent.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	audioEvent = default(AudioEvent);
	audioEvent.Clip = enemyDamageClip;
	audioEvent.PitchMin = 1.2f;
	audioEvent.PitchMax = 1.4f;
	audioEvent.Volume = 1f;
	audioEvent.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	if (deathWaveInfectedSmallPrefab != null)
	{
	    GameObject gameObject = Instantiate(deathWaveInfectedSmallPrefab, transform.position + effectOrigin,Quaternion.identity);
	    Vector3 localScale = gameObject.transform.localScale;
	    localScale.x = 0.5f;
	    localScale.y = 0.5f;
	    gameObject.transform.localScale = localScale;
	}
	GlobalPrefabDefaults.Instance.SpawnBlood(transform.position + effectOrigin, 8, 10, 15f, 20f, 0, 360, null);
    }

    private void EmitInfectedEffects()
    {
	EmitSound();
	if(corpse != null)
	{
	    SpriteFlash component = corpse.GetComponent<SpriteFlash>();
	    if(component != null)
	    {
		component.flashInfected();
	    }
	}
	GameObject gameObject = Instantiate(deathWaveInfectedPrefab, transform.position + effectOrigin, Quaternion.identity);
	gameObject.transform.SetScaleX(1.25f);
	gameObject.transform.SetPositionY(1.25f);
	GlobalPrefabDefaults.Instance.SpawnBlood(transform.position + effectOrigin, 8, 10, 15f, 20f, 0, 360, null);
	Instantiate(dustPuffMedPrefab, transform.position + effectOrigin, Quaternion.identity);
	ShakeCameraIfVisible("EnemyKillShake");
    }

    public void EmitSound()
    {
	enemyDeathSwordAudio.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	enemyDamageAudio.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
    }

    private void EmitEssence()
    {
	//TODO:和梦之钉有关的
	PlayerData playerData = GameManager.instance.playerData;
	if (!playerData.hasDreamNail)
	{
	    return; 
	}
    }

    protected void ShakeCameraIfVisible(string eventName)
    {
	Renderer renderer = GetComponent<Renderer>();
	if (renderer == null)
	{
	    renderer = GetComponentInChildren<Renderer>();
	}
	if (renderer != null && renderer.isVisible)
	{
	    GameCameras.instance.cameraShakeFSM.SendEvent(eventName);
	}
    }

}
