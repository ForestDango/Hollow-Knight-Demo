using UnityEngine;

public class HazardRespawnTrigger : MonoBehaviour
{
    public HazardRespawnMarker respawnMarker;
    private PlayerData playerData;
    public bool fireOnce;
    private bool inactive;

    private void Awake()
    {
	playerData = PlayerData.instance;
	if (playerData == null)
	{
	    Debug.LogError(name + "- Player Data reference is null, please check this is being set correctly.");
	}
    }

    private void Start()
    {
	if (respawnMarker == null)
	{
	    Debug.LogWarning(name + " does not have a Hazard Respawn Marker Set");
	}
    }

    private void OnTriggerEnter2D(Collider2D otherCollision)
    {
	if (!inactive && otherCollision.gameObject.layer == 9)
	{
	    playerData.SetHazardRespawn(respawnMarker);
	    if (fireOnce)
	    {
		inactive = true;
	    }
	}
    }

}
