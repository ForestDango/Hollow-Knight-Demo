using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions 
{
    /// <summary>
    /// 找到第一个可交互的Selectable对象，如果它不可交互就继续寻找它的下一位selectable
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    public static Selectable GetFirstInteractable(this Selectable start)
    {
	if (start == null)
	{
	    return null;
	}
	if (start.interactable)
	{
	    return start;
	}
	return start.navigation.selectOnDown.GetFirstInteractable();
    }

    public static void SetActiveChildren(this GameObject self, bool value)
    {
	int childCount = self.transform.childCount;
	for (int i = 0; i < childCount; i++)
	{
	    self.transform.GetChild(i).gameObject.SetActive(value);
	}
    }

    public static void SetActiveWithChildren(this MeshRenderer self, bool value)
    {
	if (self.transform.childCount > 0)
	{
	    MeshRenderer[] componentsInChildren = self.GetComponentsInChildren<MeshRenderer>();
	    for (int i = 0; i < componentsInChildren.Length; i++)
	    {
		componentsInChildren[i].enabled = value;
	    }
	    return;
	}
	self.enabled = value;
    }

    public static void SetPositionX(this Transform t, float newX)
    {
	t.position = new Vector3(newX, t.position.y, t.position.z);
    }
    public static void SetPositionY(this Transform t, float newY)
    {
	t.position = new Vector3(t.position.x, newY, t.position.z);
    }
    public static void SetPositionZ(this Transform t, float newZ)
    {
	t.position = new Vector3(t.position.x, t.position.y, newZ);
    }

    public static void SetPosition2D(this Transform t, float x, float y)
    {
	t.position = new Vector3(x, y, t.position.z);
    }
    public static void SetPosition2D(this Transform t, Vector2 position)
    {
	t.position = new Vector3(position.x, position.y, t.position.z);
    }

    public static void SetPosition3D(this Transform t, float x, float y, float z)
    {
	t.position = new Vector3(x, y, z);
    }

    public static void SetScaleX(this Transform t, float newXScale)
    {
	t.localScale = new Vector3(newXScale, t.localScale.y, t.localScale.z);
    }
    public static void SetScaleY(this Transform t, float newYScale)
    {
	t.localScale = new Vector3(t.localScale.x, newYScale, t.localScale.z);
    }

    public static void SetRotationZ(this Transform t, float newZRotation)
    {
	t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y, newZRotation); 
    }

    public static float GetRotation2D(this Transform t)
    {
	return t.localEulerAngles.z;
    }

    public static void SetRotation2D(this Transform t,float rotation)
    {
	Vector3 eulerAngles = t.eulerAngles;
	eulerAngles.z = rotation;
	t.eulerAngles = eulerAngles;
    }

    public static bool HasParameter(this Animator self, string paramName, AnimatorControllerParameterType? type = null)
    {
	foreach (AnimatorControllerParameter animatorControllerParameter in self.parameters)
	{
	    if (animatorControllerParameter.name == paramName)
	    {
		if (type != null)
		{
		    AnimatorControllerParameterType type2 = animatorControllerParameter.type;
		    AnimatorControllerParameterType? animatorControllerParameterType = type;
		    if (!(type2 == animatorControllerParameterType.GetValueOrDefault() & animatorControllerParameterType != null))
		    {
			break;
		    }
		}
		return true;
	    }
	}
	return false;
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

    public static float GetPositionY(this Transform t)
    {
	return t.position.y;
    }

    public static Vector2 MultiplyElements(this Vector2 self, Vector2 other)
    {
	Vector2 result = self;
	result.x *= other.x;
	result.y *= other.y;
	return result;
    }

}
