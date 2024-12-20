using System;
using HutongGames.PlayMaker;
using Language;

[ActionCategory("Game Text")]
public class GetLanguageStringProcessed : FsmStateAction
{
    [RequiredField]
    public FsmString sheetName;
    [RequiredField]
    public FsmString convName;
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmString storeValue;
    [ObjectType(typeof(LocalisationHelper.FontSource))]
    public FsmEnum fontSource;


    public override void Reset()
    {
	sheetName = null;
	convName = null;
	storeValue = null;
	fontSource = null;
    }

    public override void OnEnter()
    {
	storeValue.Value = Language.Language.Get(convName.Value, sheetName.Value);
	storeValue.Value = storeValue.Value.Replace("<br>", "\n");
	storeValue.Value = storeValue.Value.GetProcessed((LocalisationHelper.FontSource)fontSource.Value);
	Finish();
    }
}
