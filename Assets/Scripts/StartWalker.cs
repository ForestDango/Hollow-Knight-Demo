using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class StartWalker : WalkerAction
{
    public FsmBool walkRight;

    public override void Reset()
    {
	base.Reset();
	walkRight = new FsmBool
	{
	    UseVariable = true
	};
    }

    /// <summary>
    /// 调用了walker的两个方法，如果不存在walkright就按原计划接着走路
    /// 如果存在则根据方向判断行走的方向
    /// </summary>
    /// <param name="walker"></param>
    protected override void Apply(Walker walker)
    {
	if (walkRight.IsNone)
	{
	    walker.StartMoving();
	}
	else
	{
	    walker.Go(walkRight.Value ? 1 : -1);
	}
	walker.ClearTurnCoolDown();
    }
}
