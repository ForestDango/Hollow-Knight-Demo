using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBehaviour : MonoBehaviour
{
    [Header("Animation")]
    public float walkReactAmount = 1f;
    public AnimationCurve walkReactCurve;
    public float walkReactLength;
    [Space]
    public float attackReactAmount = 2f;
    public AnimationCurve attackReactCurve;
    public float attackReactLength;
    [Space]
    public string pushAnim = "Push";
    private Animator animator;

    [Header("Sound")]
    public AudioClip[] pushSounds;
    public AudioClip[] cutPushSounds;
    public AudioClip[] cutSounds;
    private AudioSource source;

    [Header("Extra")]
    public Color infectedColor = Color.white;
    public bool neverInfected;
    private static bool colorSet = false;
    private AnimationCurve curve;
    private float animLength = 2f;
    private float animElapsed;
    private float pushAmount = 1f;
    private float pushDirection;
    private bool returned = true;
    private bool cutFirstFrame;
    private bool isCut;
    private float pushAmountError;

    private Rigidbody2D player;
    private Vector3 oldPlayerPos;
    private SpriteRenderer[] renderers;
    private static Dictionary<string, Material> sharedMaterials = new Dictionary<string, Material>();

    private static int grassCount = 0;
    private Material sharedMaterial;
    private MaterialPropertyBlock propertyBlock;
    public Material SharedMaterial
    {
	get
	{
	    return sharedMaterial;
	}
    }

    private void Awake()
    {
	source = GetComponent<AudioSource>();
	animator = GetComponent<Animator>();
	propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
	if (Mathf.Abs(transform.position.z - 0.004f) > 1.8f)
	{
	    GrassCut component = GetComponent<GrassCut>();
	    if (component)
	    {
		Destroy(component);
	    }
	    Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>();
	    for (int i = 0; i < componentsInChildren.Length; i++)
	    {
		Destroy(componentsInChildren[i]);
	    }
	}

	renderers = GetComponentsInChildren<SpriteRenderer>(true);
	if(renderers.Length != 0)
	{
	    string key = renderers[0].material.name + (neverInfected ? "_neverInfected" : "");
	    if (sharedMaterials.ContainsKey(key))
	    {
		sharedMaterial = sharedMaterials[key];
	    }
	    if (!sharedMaterial)
	    {
		sharedMaterial = new Material(renderers[0].material);
		sharedMaterials[key] = sharedMaterial;
	    }
	}
	if (sharedMaterial)
	{
	    SpriteRenderer[] array = renderers;
	    for (int i = 0; i < array.Length; i++)
	    {
		array[i].sharedMaterial = sharedMaterial;
	    }
	}
	if (!colorSet && !neverInfected)
	{
	    StartCoroutine(DelayedInfectedCheck());
	}
	pushAmountError = Random.Range(-0.01f, 0.01f);
	foreach (SpriteRenderer rend in renderers)
	{
	    SetPushAmount(rend, pushAmountError);
	}
	transform.SetPositionZ(transform.position.z + Random.Range(-0.0001f, 0.0001f));
    }
    private void OnEnable()
    {
	grassCount++;
    }
    private void OnDisable()
    {
	grassCount--;
	if(colorSet)
	{
	    colorSet = false;
	    sharedMaterial = null;
	}
	if(grassCount <= 0)
	{
	    sharedMaterials.Clear();
	}
    }

    private void LateUpdate()
    {
	if (!returned)
	{
	    float value = curve.Evaluate(animElapsed / animLength) * pushAmount * pushDirection * Mathf.Sign(transform.localScale.x) + pushAmountError;
	    foreach (SpriteRenderer rend in renderers)
	    {
		SetPushAmount(rend, value);
	    }
	    if (animElapsed >= animLength)
	    {
		returned = true;
		if (animator && animator.HasParameter(pushAnim, new AnimatorControllerParameterType?(AnimatorControllerParameterType.Bool)))
		{
		    animator.SetBool(pushAnim, false);
		}
	    }
	    animElapsed += Time.deltaTime;
	}
    }

    private void FixedUpdate()
    {
	if (player && returned && Mathf.Abs(player.velocity.x) >= 0.1f)
	{
	    pushDirection = Mathf.Sign(player.velocity.x);
	    returned = false;
	    animElapsed = 0f;
	    pushAmount = walkReactAmount;
	    curve = walkReactCurve;
	    animLength = walkReactLength;
	    PlayRandomSound(isCut ? cutPushSounds : pushSounds);
	    if (animator)
	    {
		if (animator.HasParameter(pushAnim, new AnimatorControllerParameterType?(AnimatorControllerParameterType.Bool)))
		{
		    animator.SetBool(pushAnim, true);
		    return;
		}
		if (animator.HasParameter(pushAnim, new AnimatorControllerParameterType?(AnimatorControllerParameterType.Trigger)))
		{
		    animator.SetTrigger(pushAnim);
		}
	    }
	}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if (returned)
	{
	    pushDirection = Mathf.Sign(transform.position.x - collision.transform.position.x);
	    returned = false;
	    animElapsed = 0f;
	    if (GrassCut.ShouldCut(collision))
	    {
		pushAmount = attackReactAmount;
		curve = attackReactCurve;
		animLength = attackReactLength;
		PlayRandomSound(isCut ? cutPushSounds : pushSounds);
	    }
	    else
	    {
		pushAmount = walkReactAmount;
		curve = walkReactCurve;
		animLength = walkReactLength;
		if (collision.tag == "Player")
		{
		    player = collision.GetComponent<Rigidbody2D>();
		}
		PlayRandomSound(isCut ? cutPushSounds : pushSounds);
	    }
	    if (animator && animator.HasParameter(pushAnim, null))
	    {
		animator.SetBool(pushAnim, true);
	    }
	}
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
	if(collision.tag == "Player")
	{
	    player = null;
	}
    }

    private void PlayRandomSound(AudioClip[] clips)
    {
	if (source && clips.Length != 0)
	{
	    AudioClip clip = clips[Random.Range(0, clips.Length)];
	    source.PlayOneShot(clip);
	}
    }
    private void SetPushAmount(Renderer rend, float value)
    {
	rend.GetPropertyBlock(propertyBlock);
	propertyBlock.SetFloat("_PushAmount", value);
	rend.SetPropertyBlock(propertyBlock);
    }

    private IEnumerator DelayedInfectedCheck()
    {
	yield return null;
	if(sharedMaterial && GameObject.FindWithTag("Infected Flag"))
	{
	    colorSet = true;
	    sharedMaterial.color = infectedColor;
	    SpriteRenderer[] array = renderers;
	    for (int i = 0; i < array.Length; i++)
	    {
		array[i].sharedMaterial = sharedMaterial;
	    }
	}
    }


}
