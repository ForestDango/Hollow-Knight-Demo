using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Jitter an object around using its Transform.")]
    public class ObjectJitterLocal : FsmStateAction
    {
	[RequiredField]
	[Tooltip("The game object to translate.")]
	public FsmOwnerDefault gameObject;
	[Tooltip("Jitter along x axis.")]
	public FsmFloat x;
	[Tooltip("Jitter along y axis.")]
	public FsmFloat y;
	[Tooltip("Jitter along z axis.")]
	public FsmFloat z;

	private float startX;
	private float startY;
	private float startZ;

	public override void Reset()
	{
	    gameObject = null;
	    x = new FsmFloat
	    {
		UseVariable = true
	    };
	    y = new FsmFloat
	    {
		UseVariable = true
	    };
	    z = new FsmFloat
	    {
		UseVariable = true
	    };
	}

	public override void OnPreprocess()
	{
	    Fsm.HandleFixedUpdate = true;
	}

	public override void OnEnter()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (ownerDefaultTarget == null)
	    {
		return;
	    }
	    startX = ownerDefaultTarget.transform.localPosition.x;
	    startY = ownerDefaultTarget.transform.localPosition.y;
	    startZ = ownerDefaultTarget.transform.localPosition.z;
	}

	public override void OnFixedUpdate()
	{
	    DoTranslate();
	}

	private void DoTranslate()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (ownerDefaultTarget == null)
	    {
		return;
	    }
	    Vector3 localPosition = new Vector3(startX + Random.Range(-x.Value, x.Value), startY + Random.Range(-y.Value, y.Value), startZ + Random.Range(-z.Value, z.Value));
	    ownerDefaultTarget.transform.localPosition = localPosition;
	}
    }
}
