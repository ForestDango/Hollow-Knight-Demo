using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Set TextMeshPro text.")]
    public class SetTextMeshProText : FsmStateAction
    {
	[RequiredField]
	public FsmOwnerDefault gameObject;
	[RequiredField]
	public FsmString textString;
	private GameObject go;
	private TextMeshPro textMesh;

	public override void Reset()
	{
	    gameObject = null;
	    textString = null;
	}

	public override void OnEnter()
	{
	    go = Fsm.GetOwnerDefaultTarget(gameObject);
	    if(gameObject != null)
	    {
		go = Fsm.GetOwnerDefaultTarget(gameObject);
		textMesh = go.GetComponent<TextMeshPro>();
		if(textMesh != null)
		{
		    textMesh.text = textString.Value;
		}
	    }
	    Finish();
	}
    }
}
