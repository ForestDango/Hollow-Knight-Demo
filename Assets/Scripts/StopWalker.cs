using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class StopWalker : WalkerAction
{
    /// <summary>
    /// 调用walker.cs的Stop函数并原因为controlled
    /// </summary>
    /// <param name="walker"></param>
    protected override void Apply(Walker walker)
    {
	walker.Stop(Walker.StopReasons.Controlled);
    }
}
