using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuinsLift : MonoBehaviour
{
    public float[] stopPositions;
    public Transform chainsParent;
    private List<Transform> chains;

    private void Start()
    {
	if (chainsParent)
	{
	    int childCount = chainsParent.childCount;
	    chains = new List<Transform>(childCount);
	    for (int i = 0; i < childCount; i++)
	    {
		Transform child = chainsParent.GetChild(i);
		if (child.gameObject.activeSelf)
		{
		    SpriteRenderer component = child.GetComponent<SpriteRenderer>();
		    if (!component || (component.enabled && !(component.sprite == null)))
		    {
			chains.Add(child);
		    }
		}
	    }
	    chains.Sort((Transform a, Transform b) => a.transform.position.y.CompareTo(b.transform.position.y));
	    chains.Reverse();
	    StartCoroutine(HideChains());
	}
    }

    private IEnumerator HideChains()
    {
	List<float> list = new List<float>(stopPositions);
	list.Sort();
	float minYPos = list[0];
	float maxYPos = list[list.Count - 1] - minYPos;
	float lastYPos = 0f;
	for (; ; )
	{
	    yield return null;
	    if (transform.position.y != lastYPos)
	    {
		lastYPos = transform.position.y;
		int num = Mathf.FloorToInt((transform.position.y - minYPos) / maxYPos * chains.Count);
		for (int i = 0; i < chains.Count; i++)
		{
		    chains[i].gameObject.SetActive(i >= num);
		}
	    }
	}
    }

    /// <summary>
    /// 获取当前的position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public float GetPositionY(int position)
    {
	position--;
	if (position < 0 || position + 1 > stopPositions.Length)
	{
	    position = 0;
	}
	return stopPositions[position];
    }

    /// <summary>
    /// 是否当前位置是上下的终点
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsCurrentPositionTerminus(int position)
    {
	bool result = false;
	if (position == 1 || position == stopPositions.Length)
	{
	    result = true;
	}
	return result;
    }

    public int KeepInBounds(int position)
    {
	position--;
	if (position < 0 || position + 1 > stopPositions.Length)
	{
	    position = 0;
	}
	return position + 1;
    }

}
