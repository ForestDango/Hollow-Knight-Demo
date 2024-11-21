using UnityEngine;

public static class FlingUtils
{
    public static GameObject[] SpawnAndFling(Config config,Transform spawnPoint,Vector3 positionOffset)
    {
	if(config.Prefab == null)
	{
	    return null;
	}
	int num = Random.Range(config.AmountMin, config.AmountMax + 1);
	Vector3 a = (spawnPoint != null) ? spawnPoint.TransformPoint(positionOffset) : positionOffset;
	GameObject[] array = new GameObject[num];
	for (int i = 0; i < num; i++)
	{
	    Vector3 position = a + new Vector3(Random.Range(-config.OriginVariationX, config.OriginVariationX), Random.Range(-config.OriginVariationY, config.OriginVariationY));
	    GameObject gameObject = config.Prefab.Spawn(position);
	    gameObject.transform.position = position;
	    Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
	    if(component != null)
	    {
		float d = Random.Range(config.SpeedMin, config.SpeedMax);
		float num2 = Random.Range(config.AngleMin, config.AngleMax);
		component.velocity = new Vector2(Mathf.Cos(num2 * 0.017453292f) * d, Mathf.Sin(num2 * 0.017453292f) * d);
	    }
	    array[i] = gameObject;
	}
	return array;
    }

    public static void FlingChildren(ChildrenConfig config, Transform spawnPoint, Vector3 positionOffset)
    {
	if (config.Parent == null)
	{
	    return;
	}
	Vector3 a = (spawnPoint != null) ? spawnPoint.TransformPoint(positionOffset) : positionOffset;
	int num = (config.AmountMax > 0) ? Random.Range(config.AmountMin, config.AmountMax) : config.Parent.transform.childCount;
	for (int i = 0; i < num; i++)
	{
	    Transform child = config.Parent.transform.GetChild(i);
	    child.gameObject.SetActive(true);
	    child.transform.position = a + new Vector3(Random.Range(-config.OriginVariationX, config.OriginVariationX), Random.Range(-config.OriginVariationY, config.OriginVariationY), 0f);
	    Rigidbody2D component = child.GetComponent<Rigidbody2D>();
	    if (component != null)
	    {
		float d = Random.Range(config.SpeedMin, config.SpeedMax);
		float num2 = Random.Range(config.AngleMin, config.AngleMax);
		component.velocity = new Vector2(Mathf.Cos(num2 * 0.017453292f), Mathf.Sin(num2 * 0.017453292f)) * d;
	    }
	}
    }

    public struct Config
    {
	public GameObject Prefab;
	public float SpeedMin;
	public float SpeedMax;
	public float AngleMin;
	public float AngleMax;
	public float OriginVariationX;
	public float OriginVariationY;
	public int AmountMin;
	public int AmountMax;
    }

    public struct ChildrenConfig
    {
	public GameObject Parent;
	public int AmountMin;
	public int AmountMax;
	public float SpeedMin;
	public float SpeedMax;
	public float AngleMin;
	public float AngleMax;
	public float OriginVariationX;
	public float OriginVariationY;
    }

    public struct SelfConfig
    {
	public GameObject Object;
	public float SpeedMin;
	public float SpeedMax;
	public float AngleMin;
	public float AngleMax;
    }

}
