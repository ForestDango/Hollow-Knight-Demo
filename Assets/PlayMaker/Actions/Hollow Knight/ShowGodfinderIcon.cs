using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class ShowGodfinderIcon : FsmStateAction
{
    public FsmFloat delay;
    //[ObjectType(typeof(BossScene))]
    public FsmObject unlockBossScene;

    public override void Reset()
    {
	delay = null;
    }

    public override void OnEnter()
    {
	//TODO:
	base.Finish();
    }
}
