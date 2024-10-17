using System;
using UnityEngine;

public class CinematicVideoPlayerConfig
{
    private CinematicVideoReference videoReference;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;
    private CinematicVideoFaderStyles faderStyle;
    private float implicitVolume;

    public CinematicVideoReference VideoReference
    {
	get
	{
	    return videoReference;
	}
    }
    public MeshRenderer MeshRenderer
    {
	get
	{
	    return meshRenderer;
	}
    }
    public AudioSource AudioSource
    {
	get
	{
	    return audioSource;
	}
    }
    public CinematicVideoFaderStyles FaderStyle
    {
	get
	{
	    return faderStyle;
	}
    }
    public float ImplicitVolume
    {
	get
	{
	    return implicitVolume;
	}
    }
    public CinematicVideoPlayerConfig(CinematicVideoReference videoReference, MeshRenderer meshRenderer, AudioSource audioSource, CinematicVideoFaderStyles faderStyle, float implicitVolume)
    {
	this.videoReference = videoReference;
	this.meshRenderer = meshRenderer;
	this.audioSource = audioSource;
	this.faderStyle = faderStyle;
	this.implicitVolume = implicitVolume;
    }
}
