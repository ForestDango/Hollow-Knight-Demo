using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainSequence : SkippableSequence
{
    [SerializeField] private SkippableSequence[] sequences;
    private int currentSequenceIndex;
    private float fadeByController;
    private bool isSkipped;

    private SkippableSequence CurrentSequence
    {
	get
	{
	    if (currentSequenceIndex < 0 || currentSequenceIndex >= sequences.Length)
	    {
		return null;
	    }
	    return sequences[currentSequenceIndex];
	}
    }
    public bool IsCurrentSkipped
    {
	get
	{
	    return CurrentSequence != null && CurrentSequence.IsSkipped;
	}
    }
    public override bool IsPlaying
    {
	get
	{
	    return currentSequenceIndex < sequences.Length - 1 || (!(CurrentSequence == null) && CurrentSequence.IsPlaying);
	}
    }
    public override bool IsSkipped
    {
	get
	{
	    return isSkipped;
	}
    }
    public override float FadeByController
    {
	get
	{
	    return fadeByController;
	}
	set
	{
	    fadeByController = Mathf.Clamp01(value);
	    for (int i = 0; i < sequences.Length; i++)
	    {
		sequences[i].FadeByController = fadeByController;
	    }
	}
    }

    public delegate void TransitionedToNextSequenceDelegate();
    public event TransitionedToNextSequenceDelegate TransitionedToNextSequence;

    protected void Awake()
    {
	fadeByController = 1f;
    }

    protected void Update()
    {
	if(CurrentSequence != null && !CurrentSequence.IsPlaying && !isSkipped)
	{
	    Next();
	}
    }

    public override void Begin()
    {
	isSkipped = false;
	currentSequenceIndex = -1;
	Next();
    }

    private void Next()
    {
	SkippableSequence currentSequence = CurrentSequence;
	if(currentSequence != null)
	{
	    currentSequence.gameObject.SetActive(false);
	}
	currentSequenceIndex++;
	if (!isSkipped)
	{
	    if(CurrentSequence != null)
	    {
		CurrentSequence.gameObject.SetActive(true);
		CurrentSequence.Begin();
	    }
	    if(TransitionedToNextSequence != null)
	    {
		TransitionedToNextSequence();
	    }
	}
    }

    public override void Skip()
    {
	isSkipped = true;
	for (int i = 0; i < sequences.Length; i++)
	{
	    sequences[i].Skip();
	}
    }

    public void SkipSingle()
    {
	if (CurrentSequence != null)
	{
	    CurrentSequence.Skip();
	}
    }
}
