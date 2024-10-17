using System;
using UnityEngine;

public class DeactivateAfter2dtkAnimation : MonoBehaviour
{
    public tk2dSpriteAnimator spriteAnimator;
    private float timer;

    private void OnEnable()
    {
	timer = 0f;
	if(spriteAnimator == null)
	{
	    spriteAnimator.GetComponent<tk2dSpriteAnimator>();
	}
	spriteAnimator.PlayFromFrame(0);
    }

    private void Update()
    {
	if(timer > 0.1f)
	{
	    timer -= Time.deltaTime;
	}
	if (!spriteAnimator.Playing)
	{
	    gameObject.SetActive(false);
	}
    }

}
