using UnityEngine;

public class WaveEffectControl : MonoBehaviour
{
    private float timer;

    public Color colour;
    public SpriteRenderer spriteRenderer;
    public float accel;
    public float accelStart = 5f;
    public bool doNotRecycle;
    public bool doNotPositionZ;
    public bool blackWave;
    public bool otherColour;
    public float scaleMultiplier = 1f;

    private void Start()
    {
	spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
	timer = 0f;
	if (blackWave)
	{
	    colour = new Color(0f, 0f, 0f, 1f);
	}
	else
	{
	    colour = new Color(1f, 1f, 1f, 1f);
	}
	accel = accelStart;
	if (!doNotPositionZ)
	{
	    transform.position = new Vector3(transform.position.x, transform.position.y, 0.1f);
	}
    }

    private void Update()
    {
	timer += Time.deltaTime * accel;
	float num = (1f + timer * 4f) * scaleMultiplier;
	transform.localScale = new Vector3(num, num, num);
	Color color = spriteRenderer.color;
	color.a = 1f - timer;
	spriteRenderer.color = color;
	if (timer > 1f)
	{
	    if (!doNotRecycle)
	    {
		gameObject.Recycle();
		return;
	    }
	    gameObject.SetActive(false);
	}
    }

    private void FixedUpdate()
    {
	accel *= 0.95f;
	if (accel < 0.5f)
	{
	    accel = 0.5f;
	}
    }

}
