using System;
using UnityEngine;

/// <summary>
/// 根据权重weight确定随机播放某些音乐片段的概率
/// </summary>

[CreateAssetMenu(fileName = "RandomAudioClipTable", menuName = "Hollow Knight/Random Audio Clip Table", order = 1000)]
public class RandomAudioClipTable : ScriptableObject
{
    [SerializeField] private RandomAudioClipTable.Option[] options;
    [SerializeField] private float pitchMin;
    [SerializeField] private float pitchMax;

    protected void Reset()
    {
	pitchMax = 1f;
	pitchMin = 1f;
    }

    public AudioClip SelectClip()
    {
	if (options.Length == 0)
	{
	    return null;
	}
	if (options.Length == 1)
	{
	    return options[0].Clip;
	}
	float num = 0f;
	for (int i = 0; i < options.Length; i++)
	{
	    RandomAudioClipTable.Option option = options[i];
	    num += option.Weight;
	}
	float num2 = UnityEngine.Random.Range(0f, num);
	float num3 = 0f;
	for (int j = 0; j < options.Length - 1; j++)
	{
	    RandomAudioClipTable.Option option2 = options[j];
	    num3 += option2.Weight;
	    if (num2 < num3)
	    {
		return option2.Clip;
	    }
	}
	return options[options.Length - 1].Clip;
    }

    public float SelectPitch()
    {
	if (Mathf.Approximately(pitchMin, pitchMax))
	{
	    return pitchMax;
	}
	return UnityEngine.Random.Range(pitchMin, pitchMax);
    }

    public void PlayOneShotUnsafe(AudioSource audioSource)
    {
	if (audioSource == null)
	{
	    return;
	}
	AudioClip audioClip = SelectClip();
	if (audioClip == null)
	{
	    return;
	}
	audioSource.pitch = SelectPitch();
	audioSource.PlayOneShot(audioClip);
    }

    [Serializable]
    private struct Option
    {
	public AudioClip Clip;
	[Range(1f, 10f)]
	public float Weight;
    }
}
