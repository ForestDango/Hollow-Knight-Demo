using System.Collections;
using UnityEngine;

public class CorpseFungusExplode : Corpse
{
    [Header("Fungus Explode Variables")]
    public ParticleSystem anticSteam;
    public ParticleSystem gasAttack;
    public AudioEvent gushSound;
    public AudioEvent explodeSound;
    public GameObject gasHitBox;

    protected override void LandEffects()
    {
	StartCoroutine(DoLandEffects());
    }

    private IEnumerator DoLandEffects()
    {
	body.isKinematic = true;
	body.velocity = Vector3.zero;
	yield return new WaitForSeconds(1f);
	if (corpseFlame)
	{
	    corpseFlame.Stop();
	}
	if (anticSteam)
	{
	    anticSteam.Play();
	}
	body.velocity = Vector2.zero;
	gushSound.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	yield return StartCoroutine(Jitter(0.9f));
	explodeSound.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	if (anticSteam)
	{
	    anticSteam.Stop();
	}
	GameCameras gameCameras = FindObjectOfType<GameCameras>();
	if (gameCameras)
	{
	    gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
	}
	if (gasAttack)
	{
	    gasAttack.Play();
	}
	if (gasHitBox)
	{
	    gasHitBox.SetActive(true);
	    Vector3 localScale = gasHitBox.transform.localScale;
	    gasHitBox.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
	    iTween.ScaleTo(gasHitBox, iTween.Hash(new object[]
	    {
		"scale",
		localScale,
		"time",
		0.4f,
		"easetype",
		iTween.EaseType.easeOutCirc,
		"islocal",
		true
	    }));
	    yield return new WaitForSeconds(0.4f);
	}
	meshRenderer.enabled = false;
	yield return new WaitForSeconds(0.4f);
	if (gasHitBox)
	{
	    gasHitBox.SetActive(false);
	}
	yield break;
    }

    private IEnumerator Jitter(float duration)
    {
	Transform sprite = spriteAnimator.transform;
	Vector3 initialPos = sprite.position;
	for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
	{
	    sprite.position = initialPos + new Vector3(Random.Range(-0.1f, 0.1f), 0f, 0f);
	    yield return null;
	}
	sprite.position = initialPos;
	yield break;
    }

}
