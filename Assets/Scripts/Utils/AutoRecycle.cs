using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRecycle : MonoBehaviour
{
    //这个脚本是暂时性的，用在暂时处理自动回收一次性生成的物体和粒子系统，等后续开发就可以删除掉了
    public float recycleTimer = 1f;

    private void Update()
    {
	recycleTimer -= Time.deltaTime;
	if(recycleTimer <= 0f)
	{
	    gameObject.Recycle();
	}
    }
}
