using System;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Hollow Knight/Cinematic Video Reference", fileName = "CinematicVideoReference", order = 1000)]
public class CinematicVideoReference : ScriptableObject
{
    [SerializeField] private string videoAssetPath;
    [SerializeField] private string audioAssetPath;
    [SerializeField] private VideoClip embeddedVideoClip;

    public string VideoFileName
    {
	get
	{
	    return name;
	}
    }
    public string VideoAssetPath
    {
	get
	{
	    return videoAssetPath;
	}
    }
    public string AudioAssetPath
    {
	get
	{
	    return audioAssetPath;
	}
    }
    public VideoClip EmbeddedVideoClip
    {
	get
	{
	    return embeddedVideoClip;
	}
    }
}
