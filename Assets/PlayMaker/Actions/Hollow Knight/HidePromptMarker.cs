using System;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class HidePromptMarker : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    public FsmGameObject storedObject;

    public override void Reset()
    {
	storedObject = null;
    }

    public override void OnEnter()
    {
	if (storedObject.Value)
	{
	    PromptMarker component = storedObject.Value.GetComponent<PromptMarker>();
	    if (component)
	    {
		component.Hide();
		storedObject.Value = null;
	    }
	}
	Finish();
    }

}
