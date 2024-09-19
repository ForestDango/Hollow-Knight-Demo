using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class StopWalker : WalkerAction
{
    /// <summary>
    /// ����walker.cs��Stop������ԭ��Ϊcontrolled
    /// </summary>
    /// <param name="walker"></param>
    protected override void Apply(Walker walker)
    {
	walker.Stop(Walker.StopReasons.Controlled);
    }
}
