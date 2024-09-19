using HutongGames.PlayMaker;
using UnityEngine;


[ActionCategory("Hollow Knight")]
public class SetWalkerFacing : WalkerAction
{
    public FsmBool walkRight;
    public FsmBool randomStartDir;

    public override void Reset()
    {
	base.Reset();
	walkRight = new FsmBool
	{
	    UseVariable = true
	};
	randomStartDir = new FsmBool();
    }
    /// <summary>
    /// 调用Walker.cs中的ChangeFacing函数来改变朝向
    /// </summary>
    /// <param name="walker"></param>
    protected override void Apply(Walker walker)
    {
	if (randomStartDir.Value)
	{
	    walker.ChangeFacing((Random.Range(0, 2) == 0) ? -1 : 1);
	    return;
	}
	if (!walkRight.IsNone)
	{
	    walker.ChangeFacing(walkRight.Value ? 1 : -1);
	}
    }

}
