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
	anim.Play("Blank"); //��ʼʱ���ö���ΪBlank�հ׵�
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
    /// ��playmaker����Ϊ����
    /// </summary>
    public void Show()
    {
	anim.Play("Up"); //���Ŷ���Up
	transform.SetPositionZ(0f); //���ú�z��λ��
	fadeGroup.FadeUp(); //fadegroup�ű�����alpha 0 -> 1
	isVisible = true; //����Ϊ����
    }

    /// <summary>
    /// ��playmaker����Ϊ����
    /// </summary>
    public void Hide()
    {
	anim.Play("Down");
	fadeGroup.FadeDown();
	owner = null; //������
	StartCoroutine(RecycleDelayed(fadeGroup.fadeOutTime)); //�ӳ�����
	isVisible = false;
    }

    /// <summary>
    /// ��ʱ����
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
