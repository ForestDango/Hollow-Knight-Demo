using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRecycle : MonoBehaviour
{
    //����ű�����ʱ�Եģ�������ʱ�����Զ�����һ�������ɵ����������ϵͳ���Ⱥ��������Ϳ���ɾ������
    public float recycleTimer = 1f;

    private void Update()
    {
	recycleTimer -= Time.deltaTime;
	if(recycleTimer <= 0f)
	{
	    Destroy(gameObject); // TODO:
	}
    }
}
