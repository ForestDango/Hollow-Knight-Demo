using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.iTween)]
    [Tooltip("Changes a GameObject's opacity over time.")]
    public class iTweenFadeTo : iTweenFsmAction
    {
	[RequiredField]
	public FsmOwnerDefault gameObject;

	[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
	public FsmString id;
	[Tooltip("The end alpha value of the animation.")]
	public FsmFloat alpha;
	[Tooltip("Whether or not to include children of this GameObject. True by default.")]
	public FsmBool includeChildren;
	[Tooltip("Which color of a shader to use. Uses '_Color' by default.")]
	public FsmString namedValueColor;
	[Tooltip("The time in seconds the animation will take to complete.")]
	public FsmFloat time;
	[Tooltip("The time in seconds the animation will wait before beginning.")]
	public FsmFloat delay;
	[Tooltip("The shape of the easing curve applied to the animation.")]
	public iTween.EaseType easeType = iTween.EaseType.linear;
	[Tooltip("The type of loop to apply once the animation has completed.")]
	public iTween.LoopType loopType;

	public override void Reset()
	{
	    base.Reset();
	    id = new FsmString
	    {
		UseVariable = true
	    };
	    alpha = 0f;
	    includeChildren = true;
	    namedValueColor = "_Color";
	    time = 1f;
	    delay = 0f;
	}

	public override void OnEnter()
	{
	    DoiTween();
	    if (loopType != iTween.LoopType.none)
	    {
		IsLoop(true);
	    }
	    OnEnteriTween(gameObject);
	}


	public override void OnExit()
	{
	    base.OnExitiTween(gameObject);
	}

	private void DoiTween()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (ownerDefaultTarget == null)
		return;
	    itweenType = "fade";
	    iTween.FadeTo(ownerDefaultTarget, iTween.Hash(new object[]
		{
		"name",
		id.IsNone ? "" : id.Value,
		"alpha",
		alpha.Value,
		"includechildren",
		includeChildren.IsNone || includeChildren.Value,
		"NamedValueColor",
		namedValueColor.Value,
		"time",
		time.Value,
		"delay",
		delay.IsNone ? 0f : delay.Value,
		"easetype",
		easeType,
		"looptype",
		loopType,
		"oncomplete",
		"iTweenOnComplete",
		"oncompleteparams",
		itweenID,
		"onstart",
		"iTweenOnStart",
		"onstartparams",
		itweenID,
		"ignoretimescale",
		!realTime.IsNone && realTime.Value
		}));
	}

    }

}
