using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCut : MonoBehaviour
{
    public SpriteRenderer[] disable;
    public SpriteRenderer[] enable;
    [Space]
    public Collider2D[] disableColliders;
    public Collider2D[] enableColliders;
    [Space]
    public GameObject particles;
    public GameObject cutEffectPrefab;
    private Collider2D col;

    private void Awake()
    {
	col = GetComponent<Collider2D>();
    }

    public static bool ShouldCut(Collider2D collision)
    {
	//如果是骨钉攻击或者是锋利之影攻击
	return collision.tag == "Nail Attack" || collision.tag == "Sharp Shadow";
    }

}
