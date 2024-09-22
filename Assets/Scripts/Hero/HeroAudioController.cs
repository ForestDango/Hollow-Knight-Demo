using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

public class HeroAudioController : MonoBehaviour
{
    private HeroController heroCtrl;

    private void Awake()
    {
	heroCtrl = GetComponent<HeroController>();
    }

    [Header("Sound Effects")]
    public AudioSource softLanding;
    public AudioSource hardLanding;
    public AudioSource jump;
    public AudioSource footStepsRun;
    public AudioSource footStepsWalk;
    public AudioSource falling;
    public AudioSource backDash;
    public AudioSource dash;

    private Coroutine fallingCo;

    public void PlaySound(HeroSounds soundEffect)
    {
	if(!heroCtrl.cState.isPaused)
	{
	    switch (soundEffect)
	    {
		case HeroSounds.FOOTSETP_RUN:
		    if(!footStepsRun.isPlaying && !softLanding.isPlaying)
		    {
			footStepsRun.Play();
			return;
		    }
		    break;
		case HeroSounds.FOOTSTEP_WALK:
		    if (!footStepsWalk.isPlaying && !softLanding.isPlaying)
		    {
			footStepsWalk.Play();
			return;
		    }
		    break;
		case HeroSounds.SOFT_LANDING:
		    RandomizePitch(softLanding, 0.9f, 1.1f);
		    softLanding.Play();
		    break;
		case HeroSounds.HARD_LANDING:
		    hardLanding.Play();
		    break;
		case HeroSounds.JUMP:
		    RandomizePitch(jump, 0.9f, 1.1f);
		    jump.Play();
		    break;
		case HeroSounds.BACK_DASH:
		    backDash.Play();
		    break;
		case HeroSounds.DASH:
		    dash.Play();
		    break;
		case HeroSounds.FALLING:
		    fallingCo = StartCoroutine(FadeInVolume(falling, 0.7f));
		    falling.Play();
		    break;
		default:
		    break;
	    }
	}
    }

    public void StopSound(HeroSounds soundEffect)
    {
	if(soundEffect == HeroSounds.FOOTSETP_RUN)
	{
	    footStepsRun.Stop();
	    return;
	}
	if (soundEffect == HeroSounds.FOOTSTEP_WALK)
	{
	    footStepsWalk.Stop();
	    return;
	}
	switch (soundEffect)
	{
	    case HeroSounds.FALLING:
		falling.Stop();
		if(fallingCo != null)
		{
		    StopCoroutine(fallingCo);
		}
		return;
	    default:
		return;
	}
    }

    public void StopAllSounds()
    {
	softLanding.Stop();
	hardLanding.Stop();
	jump.Stop();
	falling.Stop();
	backDash.Stop();
	dash.Stop();
	footStepsRun.Stop();
	footStepsWalk.Stop();
    }

    public void PauseAllSounds()
    {
	softLanding.Pause();
	hardLanding.Pause();
	jump.Pause();
	falling.Pause();
	backDash.Pause();
	dash.Pause();
	footStepsRun.Pause();
	footStepsWalk.Pause();
    }

    public void UnPauseAllSounds()
    {
	softLanding.UnPause();
	hardLanding.UnPause();
	jump.UnPause();
	falling.UnPause();
	backDash.UnPause();
	dash.UnPause();
	footStepsRun.UnPause();
	footStepsWalk.UnPause();
    }

    /// <summary>
    /// �����������Բ�ֵ�Ĵ�0��1
    /// </summary>
    /// <param name="src"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator FadeInVolume(AudioSource src, float duration)
    {
	float elapsedTime = 0f;
	src.volume = 0f;
	while (elapsedTime < duration)
	{
	    elapsedTime += Time.deltaTime;
	    float t = elapsedTime / duration;
	    src.volume = Mathf.Lerp(0f, 1f, t);
	    yield return null;
	}
    }

    /// <summary>
    /// �����תһ���ں�֮���pitch��ֵ���ظ�audiosource
    /// </summary>
    /// <param name="src"></param>
    /// <param name="minPitch"></param>
    /// <param name="maxPitch"></param>
    private void RandomizePitch(AudioSource src, float minPitch, float maxPitch)
    {
	float pitch = Random.Range(minPitch, maxPitch);
	src.pitch = pitch;
    }

    /// <summary>
    /// ����audiosource��pitch
    /// </summary>
    /// <param name="src"></param>
    private void ResetPitch(AudioSource src)
    {
	src.pitch = 1f;
    }

}
