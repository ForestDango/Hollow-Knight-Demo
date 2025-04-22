using UnityEngine;

public class TinkEffect : MonoBehaviour
{
    public GameObject blockEffect;
    public bool useNailPosition;
    public bool sendFSMEvent;
    public string FSMEvent;
    public PlayMakerFSM fsm;
    public bool sendDirectionalFSMEvents;


    private BoxCollider2D boxCollider;
    private bool hasBoxCollider;
    private HeroController heroController;
    private GameCameras gameCam;
    private Vector2 centre;
    private float halfWidth;
    private float halfHeight;

    private const float repeatDelay = 0.25f;
    private float nextTinkTime;

    private void Awake()
    {
	boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
	gameCam = GameCameras.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
	if(collision.tag == "Nail Attack")
	{
	    Debug.LogFormat("Nail Attack");
	    if (Time.time < nextTinkTime)
		return;
	    nextTinkTime = Time.time + repeatDelay;
	    PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(collision.gameObject, "damages_enemy");
	    float degrees = (playMakerFSM != null) ? playMakerFSM.FsmVariables.FindFsmFloat("direction").Value : 0f;
	    if (gameCam)
	    {
		gameCam.cameraShakeFSM.SendEvent("EnemyKillShake");
	    }
	    Vector3 position = new Vector3(0f, 0f, 0f);
	    Vector3 euler = new Vector3(0f, 0f, 0f);
	    Vector3 position2 = HeroController.instance.transform.position;
	    Vector3 position3 = collision.gameObject.transform.position;
	    bool flag = boxCollider != null;
	    if (useNailPosition)
	    {
		flag = false;
	    }
	    Vector2 vector = Vector2.zero;
	    float num = 0f;
	    float num2 = 0f;
	    if (flag)
	    {
		vector = transform.TransformPoint(boxCollider.offset);
		num = boxCollider.bounds.size.x * 0.5f;
		num2 = boxCollider.bounds.size.y * 0.5f;
	    }
	    int cardinalDirection = DirectionUtils.GetCardinalDirection(degrees);
	    if (cardinalDirection == 0)
	    {
		HeroController.instance.RecoilLeft();
		if (sendDirectionalFSMEvents)
		{
		    fsm.SendEvent("TINK RIGHT");
		}
		if (flag)
		{
		    position = new Vector3(vector.x - num, position3.y, 0.002f);
		}
		else
		{
		    position = new Vector3(position2.x + 2f, position2.y, 0.002f);
		}
	    }
	    else if (cardinalDirection == 1)
	    {
		HeroController.instance.RecoilDown();
		if (sendDirectionalFSMEvents)
		{
		    fsm.SendEvent("TINK UP");
		}
		if (flag)
		{
		    position = new Vector3(position3.x, Mathf.Max(vector.y - num2, position3.y), 0.002f);
		}
		else
		{
		    position = new Vector3(position2.x, position2.y + 2f, 0.002f);
		}
		euler = new Vector3(0f, 0f, 90f);
	    }
	    else if (cardinalDirection == 2)
	    {
		HeroController.instance.RecoilRight();
		if (sendDirectionalFSMEvents)
		{
		    fsm.SendEvent("TINK LEFT");
		}
		if (flag)
		{
		    position = new Vector3(vector.x + num, position3.y, 0.002f);
		}
		else
		{
		    position = new Vector3(position2.x - 2f, position2.y, 0.002f);
		}
		euler = new Vector3(0f, 0f, 180f);
	    }
	    else
	    {
		Debug.LogFormat("TINK DOWN");
		if (sendDirectionalFSMEvents)
		{
		    fsm.SendEvent("TINK DOWN");
		}
		if (flag)
		{
		    position = new Vector3(position3.x, Mathf.Min(vector.y + num2, position3.y), 0.002f);
		}
		else
		{
		    position = new Vector3(position2.x, position2.y - 2f, 0.002f);
		}
		euler = new Vector3(0f, 0f, 0f);
	    }
	    blockEffect.Spawn(position, Quaternion.Euler(euler)).GetComponent<AudioSource>().pitch = Random.Range(0.85f, 1.15f);    
	    if (sendFSMEvent)
	    {
		fsm.SendEvent(FSMEvent);
	    }
	}
    }

}
