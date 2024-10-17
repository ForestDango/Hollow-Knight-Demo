using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAudioController : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Sound Effects")]
    public AudioClip select;
    public AudioClip submit;
    public AudioClip cancel;
    public AudioClip slider;
    public AudioClip startGame;

    private void Awake()
    {
	audioSource = GetComponent<AudioSource>();
    }

    public void PlaySelect()
    {
	if (select)
	{
	    audioSource.PlayOneShot(select);
	}
    }
    public void PlaySubmit()
    {
	if (submit)
	{
	    audioSource.PlayOneShot(submit);
	}
    }

    public void PlayCancel()
    {
	if (cancel)
	{
	    audioSource.PlayOneShot(cancel);
	}
    }

    public void PlaySlider()
    {
	if (slider)
	{
	    audioSource.PlayOneShot(slider);
	}
    }

    public void PlayStartGame()
    {
	if (startGame)
	{
	    audioSource.PlayOneShot(startGame);
	}
    }
}
