using System;
using UnityEngine;

public class CorpseChunker : Corpse
{
    [Header("Chunker Variables")]
    public GameObject effects;
    public GameObject chunks;

    protected override void LandEffects()
    {
	if (body)
	{
	    body.velocity = Vector2.zero;
	}
	splatAudioClipTable.SpawnAndPlayOneShot(audioPlayerPrefab, transform.position);
	GlobalPrefabDefaults.Instance.SpawnBlood(transform.position, 30, 30, 5f, 30f, 60f, 120f, null);
	GameCameras gameCameras = FindObjectOfType<GameCameras>();
	if (gameCameras)
	{
	    gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
	}
	if (effects)
	{
	    effects.SetActive(true);
	}
	if (chunks)
	{
	    chunks.SetActive(true);
	    chunks.transform.SetParent(null, true);
	    FlingUtils.FlingChildren(new FlingUtils.ChildrenConfig
	    {
		Parent = chunks,
		SpeedMin = 15f,
		SpeedMax = 20f,
		AngleMin = 60f,
		AngleMax = 120f,
		OriginVariationX = 0f,
		OriginVariationY = 0f
	    }, transform, Vector3.zero);
	}
	meshRenderer.enabled = false;

    }
}
