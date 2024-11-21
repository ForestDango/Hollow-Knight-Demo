using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.SpriteRenderer)]
    public class SetSpriteRendererColor : FsmStateAction
    {
	[RequiredField]
	public FsmOwnerDefault gameObject;

	public FsmColor color;

	public bool everyFrame;

	public override void Reset()
	{
	    gameObject = null;
	    color = new FsmColor
	    {
		UseVariable = true
	    };
	    everyFrame = false;
	}

	public override void OnEnter()
	{
	    if (gameObject != null)
	    {
		GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
		SpriteRenderer component = ownerDefaultTarget.GetComponent<SpriteRenderer>();
		if (component != null)
		{
		    component.color = color.Value;
		}
	    }
	    if (!everyFrame)
	    {
		Finish();
	    }
	}

	public override void OnUpdate()
	{
	    if (gameObject != null)
	    {
		GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
		SpriteRenderer component = ownerDefaultTarget.GetComponent<SpriteRenderer>();
		if (component != null)
		{
		    component.color = color.Value;
		}
	    }
	    if (!everyFrame)
	    {
		Finish();
	    }
	}
    }
}
