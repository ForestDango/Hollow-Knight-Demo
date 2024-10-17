using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    public class LerpTk2dSpriteColor : FsmStateAction
    {
	public FsmOwnerDefault Target;
	public FsmColor TargetColor;
	public FsmFloat LerpTime;
	private tk2dSprite sprite;
	private Color initialColor;

	public override void Reset()
	{
	    Target = null;
	    TargetColor = null;
	    LerpTime = null;
	}

	public override void OnEnter()
	{
	    GameObject safe = Target.GetSafe(this);
	    if (safe)
	    {
		sprite = safe.GetComponent<tk2dSprite>();
	    }
	    else
	    {
		sprite = null;
	    }
	    if (!sprite)
	    {
		Finish();
		return;
	    }
	    initialColor = sprite.color;
	    DoAction();
	}

	public override void OnUpdate()
	{
	    DoAction();
	}

	private void DoAction()
	{
	    float num = State.StateTime / LerpTime.Value;
	    sprite.color = Color.Lerp(initialColor, TargetColor.Value, num);
	    if (num >= 1f)
	    {
		Finish();
	    }
	}
    }
}
