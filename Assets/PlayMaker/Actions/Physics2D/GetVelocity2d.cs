// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Physics 2d")]
    [Tooltip("Gets the 2d Velocity of a Game Object and stores it in a Vector2 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body 2D.")]
    public class GetVelocity2d : ComponentAction<Rigidbody2D>
    {
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;
	[UIHint(UIHint.Variable)]
	public FsmVector2 vector;
	[UIHint(UIHint.Variable)]
	public FsmFloat x;
	[UIHint(UIHint.Variable)]
	public FsmFloat y;
	[Tooltip("The space reference to express the velocity")]
	public Space space;
	[Tooltip("Repeat every frame.")]
	public bool everyFrame;

	public override void Reset()
	{
	    gameObject = null;
	    vector = null;
	    x = null;
	    y = null;
	    space = Space.World;
	    everyFrame = false;
	}

	public override void OnEnter()
	{
	    DoGetVelocity();
	    if (!everyFrame)
		Finish();
	}

	public override void OnUpdate()
	{
	    DoGetVelocity();
	}

	void DoGetVelocity()
	{
	    GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
	    if (!UpdateCache(ownerDefaultTarget))
	    {
		return;
	    }

	    Vector2 vector = rigidbody2d.velocity;

	    if (space == Space.Self)
		vector = rigidbody2d.transform.InverseTransformDirection(vector);

	    this.vector.Value = vector;
	    x.Value = vector.x;
	    y.Value = vector.y;
	}


    }
}