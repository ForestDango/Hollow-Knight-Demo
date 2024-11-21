using UnityEngine;

public class JellyEgg : MonoBehaviour
{
    public bool bomb;
    public GameObject explosionObject;
    public ParticleSystem popEffect;
    public GameObject strikeEffect;
    public MeshRenderer meshRenderer;
    public AudioSource audioSource;
    public CircleCollider2D circleCollider;
    public GameObject falseShiny;
    public GameObject shinyItem;

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
	if (otherCollider.gameObject.tag == "Nail Attack" || otherCollider.gameObject.tag == "Hero Spell" || otherCollider.gameObject.tag == "HeroBox")
	{
	    Burst();
	}
    }

    private void Burst()
    {
	meshRenderer.enabled = false;
	popEffect.Play();
	audioSource.Play();
	circleCollider.enabled = false;
	if (bomb)
	{
	    Instantiate(explosionObject, transform.position, transform.localRotation);
	    return;
	}
	float num = Random.Range(1f, 1.5f);
	strikeEffect.transform.localScale = new Vector3(num, num, num);
	strikeEffect.transform.localEulerAngles = new Vector3(strikeEffect.transform.localEulerAngles.x, strikeEffect.transform.localEulerAngles.y, Random.Range(0f, 360f));
	strikeEffect.SetActive(true);
	if(falseShiny != null)
	{
	    falseShiny.SetActive(false);
	}
	if (shinyItem != null)
	{
	    shinyItem.SetActive(true);
	}
    }
}
