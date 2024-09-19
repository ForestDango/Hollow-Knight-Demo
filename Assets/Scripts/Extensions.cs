using System;
using System.Collections;
using UnityEngine;

public static  class Extensions 
{
    public static void SetScaleX(this Transform t, float newXScale)
    {
	t.localScale = new Vector3(newXScale, t.localScale.y, t.localScale.z);
    }

    public static void SetRotation2D(this Transform t,float rotation)
    {
	Vector3 eulerAngles = t.eulerAngles;
	eulerAngles.z = rotation;
	t.eulerAngles = eulerAngles;
    }

    public static IEnumerator PlayAnimWait(this tk2dSpriteAnimator self, string anim)
    {
	tk2dSpriteAnimationClip clipByName = self.GetClipByName(anim);
	self.Play(clipByName);
	yield return new WaitForSeconds(clipByName.Duration);
	yield return new WaitForEndOfFrame();
	yield break;
    }

    public static float GetPositionX(this Transform t)
    {
	return t.position.x;
    }

    public static Vector2 MultiplyElements(this Vector2 self, Vector2 other)
    {
	Vector2 result = self;
	result.x *= other.x;
	result.y *= other.y;
	return result;
    }

}
