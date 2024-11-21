using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvAnimateUpAndDown : MonoBehaviour
{
    public string upAnimation;
    public string downAnimation;
    public float upDelay;
    public int randomStartFrameSpriteMax;
    private tk2dSpriteAnimator spriteAnimator;
    private MeshRenderer meshRenderer;
    private float timer;
    private bool animatingDown;
    private bool readyingAnimUp;

    private void Awake()
    {
	spriteAnimator = GetComponent<tk2dSpriteAnimator>();
	meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
	if(animatingDown && !spriteAnimator.Playing)
	{
	    meshRenderer.enabled = false;
	    animatingDown = false;
	}
	if(timer > 0f)
	{
	    timer -= Time.deltaTime;
	}
	if(readyingAnimUp && timer <= 0f)
	{
	    animatingDown = false;
	    meshRenderer.enabled = true;
	    if (randomStartFrameSpriteMax > 0)
	    {
		int frame = Random.Range(0, randomStartFrameSpriteMax);
		spriteAnimator.PlayFromFrame(upAnimation, frame);
	    }
	    else
	    {
		spriteAnimator.Play(upAnimation);
	    }
	    readyingAnimUp = false;
	}
    }

    public void AnimateUp()
    {
	readyingAnimUp = true;
	timer = upDelay;
    }

    public void AnimateDown()
    {
	spriteAnimator.Play(downAnimation);
	animatingDown = true;
    }

    public void ReplayUpAnim()
    {
	meshRenderer.enabled = true;
	spriteAnimator.PlayFromFrame(0);
    }


}
