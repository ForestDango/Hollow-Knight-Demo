using System.Collections;
using UnityEngine;

public class PromptMarker : MonoBehaviour
{
    public GameObject labels;
    private FadeGroup fadeGroup;
    private tk2dSpriteAnimator anim;
    private GameObject owner;
    private bool isVisible;

    private void Awake()
    {
	anim = GetComponent<tk2dSpriteAnimator>();
	if (labels)
	{
	    fadeGroup = labels.GetComponent<FadeGroup>();
	}
    }

    private void Start()
    {
	if (GameManager.instance)
	{
	    GameManager.instance.UnloadingLevel += RecycleOnLevelLoad;
	}
    }
    private void OnDestroy()
    {
	if (GameManager.instance)
	{
	    GameManager.instance.UnloadingLevel -= RecycleOnLevelLoad;
	}
    }

    private void RecycleOnLevelLoad()
    {
	if (gameObject.activeSelf)
	{
	    gameObject.Recycle();
	}
    }

    private void OnEnable()
    {
	anim.Play("Blank"); //开始时设置动画为Blank空白的
    }

    private void Update()
    {
	if (isVisible && (!owner || !owner.activeInHierarchy))
	{
	    Hide();
	}
    }

    public void SetLabel(string labelName)
    {
	if (labels)
	{
	    foreach (object obj in labels.transform)
	    {
		Transform transform = (Transform)obj;
		transform.gameObject.SetActive(transform.name == labelName);
	    }
	}
    }

    /// <summary>
    /// 被playmaker的行为调用
    /// </summary>
    public void Show()
    {
	anim.Play("Up"); //播放动画Up
	transform.SetPositionZ(0f); //设置好z轴位置
	fadeGroup.FadeUp(); //fadegroup脚本设置alpha 0 -> 1
	isVisible = true; //设置为可视
    }

    /// <summary>
    /// 被playmaker的行为调用
    /// </summary>
    public void Hide()
    {
	anim.Play("Down");
	fadeGroup.FadeDown();
	owner = null; //空引用
	StartCoroutine(RecycleDelayed(fadeGroup.fadeOutTime)); //延迟销毁
	isVisible = false;
    }

    /// <summary>
    /// 延时销毁
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator RecycleDelayed(float delay)
    {
	yield return new WaitForSeconds(delay);
	gameObject.Recycle();
	yield break;
    }

    public void SetOwner(GameObject obj)
    {
	owner = obj;
    }

}
