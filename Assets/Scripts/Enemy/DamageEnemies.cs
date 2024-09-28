using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemies : MonoBehaviour
{
    public AttackTypes attackType = AttackTypes.Generic;
    public bool circleDirection;
    public int damageDealt;
    public float direction;
    public bool ignoreInvuln = true;
    public float magnitudeMult;
    public bool moveDirection;
    public SpecialTypes specialType;

    private List<Collider2D> enteredColliders = new List<Collider2D>();

    private void Reset()
    {
	foreach (PlayMakerFSM playMakerFSM in GetComponents<PlayMakerFSM>())
	{
	    if (playMakerFSM.name == "damages_enemy")
	    {
		attackType = (AttackTypes)playMakerFSM.FsmVariables.GetFsmInt("attackType").Value;
		circleDirection = playMakerFSM.FsmVariables.GetFsmBool("circleDirection").Value;
		damageDealt = playMakerFSM.FsmVariables.GetFsmInt("damageDealt").Value;
		direction = playMakerFSM.FsmVariables.GetFsmFloat("direction").Value;
		ignoreInvuln = playMakerFSM.FsmVariables.GetFsmBool("Ignore Invuln").Value;
		magnitudeMult = playMakerFSM.FsmVariables.GetFsmFloat("magnitudeMult").Value;
		moveDirection = playMakerFSM.FsmVariables.GetFsmBool("moveDirection").Value;
		specialType = (SpecialTypes)playMakerFSM.FsmVariables.GetFsmInt("Special Type").Value;
		return;
	    }
	}
    }

    private void OnDisable()
    {
	enteredColliders.Clear();
    }

    private void FixedUpdate()
    {
	for (int i = enteredColliders.Count - 1; i >= 0; i--)
	{
	    Collider2D collider2D = enteredColliders[i];
	    if (collider2D == null || !collider2D.isActiveAndEnabled)
	    {
		enteredColliders.RemoveAt(i);
	    }
	    else
	    {
		DoDamage(collider2D.gameObject);
	    }
	}
    }

    private void OnCollisionEnter2D(Collision2D collider2D)
    {
	DoDamage(collider2D.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
	if (!enabled)
	{
	    return;
	}
	int layer = collider2D.gameObject.layer;
	if (layer == 20 || layer == 9 || layer == 26 || layer == 31) //如果layer是player和herobox就返回
	{
	    return;
	}
	if (collider2D.CompareTag("Geo"))
	{
	    return;
	}
	if (!enteredColliders.Contains(collider2D))
	{
	    enteredColliders.Add(collider2D);
	}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
	if (enteredColliders.Contains(collision))
	{
	    enteredColliders.Remove(collision);
	}
    }

    private void DoDamage(GameObject target)
    {
	if (damageDealt <= 0)
	{
	    return;
	}
	FSMUtility.SendEventToGameObject(target, "TAKE DAMAGE", false);
	HitTaker.Hit(target, new HitInstance
	{
	    Source = gameObject,
	    AttackType = attackType,
	    CircleDirection = circleDirection,
	    DamageDealt = damageDealt,
	    Direction = direction,
	    IgnoreInvulnerable = ignoreInvuln,
	    MagnitudeMultiplier = magnitudeMult,
	    MoveAngle = 0f,
	    MoveDirection = moveDirection,
	    Multiplier = 1f,
	    SpecialType = specialType,
	    IsExtraDamage = false
	}, 3);
    }
}
