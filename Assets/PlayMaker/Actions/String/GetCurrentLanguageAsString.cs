using System;
using Language;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.String)]
    [Tooltip("Get currently set language as a string.")]
    public class GetCurrentLanguageAsString : FsmStateAction
    {
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmString stringVariable;

	public override void Reset()
	{
	    stringVariable = null;
	}

	public override void OnEnter()
	{
	    stringVariable.Value = Language.Language.CurrentLanguage().ToString();
	    Finish();
	}
    }
}
