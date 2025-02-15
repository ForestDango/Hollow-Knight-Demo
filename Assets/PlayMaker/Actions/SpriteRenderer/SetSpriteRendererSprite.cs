using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.SpriteRenderer)]
    public class SetSpriteRendererSprite : FsmStateAction
    {
	[RequiredField]
	public FsmOwnerDefault gameObject;
	[ObjectType(typeof(Sprite))]
	public FsmObject sprite;

	public override void Reset()
	{
	    gameObject = null;
	    sprite = null;
	}

	public override void OnEnter()
	{
	    if (gameObject != null)
	    {
		GameObject ownerDefaultTarget = Fsm.GetOwnerDefaultTarget(gameObject);
		Sprite sprite = this.sprite.Value as Sprite;
		SpriteRenderer component = ownerDefaultTarget.GetComponent<SpriteRenderer>();
		if (component != null)
		{
		    component.sprite = sprite;
		}
	    }
	    Finish();
	}
    }
}
