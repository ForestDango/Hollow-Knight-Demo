using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicSkipPopup : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [SerializeField] private GameObject[] textGroups;
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float holdDuration;
    [SerializeField]private float fadeOutDuration;

    private bool isShowing;
    private float showTimer;

    protected void Awake()
    {
	canvasGroup = GetComponent<CanvasGroup>();
    }

    protected void Update()
    {
	if (isShowing)
	{
	    float alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, Time.unscaledDeltaTime / fadeInDuration);
	    canvasGroup.alpha = alpha;
	    return;
	}
	float num = Mathf.MoveTowards(canvasGroup.alpha, 0f, Time.unscaledDeltaTime / fadeOutDuration);
	canvasGroup.alpha = num;
	if (num < Mathf.Epsilon)
	{
	    Hide();
	    gameObject.SetActive(false);
	}
    }

    public void Show(Texts texts)
    {
	Debug.LogFormat("Show the CinematicSkipPopup");
	base.gameObject.SetActive(true);
	for (int i = 0; i < textGroups.Length; i++)
	{
	    textGroups[i].SetActive(i == (int)texts);
	}
	StopCoroutine("ShowRoutine");
	StartCoroutine("ShowRoutine");
    }

    protected IEnumerator ShowRoutine()
    {
	isShowing = true;
	yield return new WaitForSecondsRealtime(fadeInDuration);
	yield return new WaitForSecondsRealtime(holdDuration);
	isShowing = false;
	yield break;
    }

    public void Hide()
    {
	StopCoroutine("ShowRoutine");
	isShowing = false;
    }

    public enum Texts
    {
	Skip,
	Loading
    }
}
