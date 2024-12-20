using System;
using UnityEngine;

[DisallowMultipleComponent]
public class LiftChain : MonoBehaviour
{
    private tk2dSpriteAnimator[] spriteAnimators;
    private int currentDirection;

    protected void Awake()
    {
	spriteAnimators = GetComponentsInChildren<tk2dSpriteAnimator>();
	currentDirection = 0;
    }

    public void GoDown()
    {
	Debug.LogFormat(this, "Chain {0} going down.", new object[]
	{
	    name
	});
	for (int i = 0; i < spriteAnimators.Length; i++)
	{
	    tk2dSpriteAnimator tk2dSpriteAnimator = spriteAnimators[i];
	    tk2dSpriteAnimator.Resume();
	    if (currentDirection != -1)
	    {
		tk2dSpriteAnimator.Play("Chain Down");
	    }
	}
	currentDirection = -1;
    }

    public void GoUp()
    {
	Debug.LogFormat(this, "Chain {0} going up.", new object[]
	{
	    name
	});
	for (int i = 0; i < spriteAnimators.Length; i++)
	{
	    tk2dSpriteAnimator tk2dSpriteAnimator = spriteAnimators[i];
	    tk2dSpriteAnimator.Resume();
	    if (currentDirection != 1)
	    {
		tk2dSpriteAnimator.Play("Chain Up");
	    }
	}
	currentDirection = 1;
    }

    public void Stop()
    {
	Debug.LogFormat(this, "Chain {0} stopping.", new object[]
	{
	    name
	});
	for (int i = 0; i < spriteAnimators.Length; i++)
	{
	    spriteAnimators[i].Pause();
	}
    }
}
