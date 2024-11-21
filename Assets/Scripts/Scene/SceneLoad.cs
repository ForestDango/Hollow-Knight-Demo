using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoad
{
    public bool IsFetchAllowed { get; set; } //是否允许获取下一个场景
    public bool IsActivationAllowed { get; set; } //是否允许激活创建
    public bool IsUnloadAssetsRequired { get; set; } //是否需要卸载掉当前场景的Assets
    public float BeginTime { get; set; } //开启时间
    public bool IsGarbageCollectRequired { get; set; } //是否需要GC垃圾回收

    public delegate void FetchCompleteDelegate(); 
    public event FetchCompleteDelegate FetchComplete;

    public delegate void WillActivateDelegate();
    public event WillActivateDelegate WillActivate;

    public delegate void ActivationCompleteDelegate();
    public event ActivationCompleteDelegate ActivationComplete;

    public delegate void CompleteDelegate();
    public event CompleteDelegate Complete;

    public delegate void StartCalledDelegate();
    public event StartCalledDelegate StartCalled;

    public delegate void FinishDelegate();
    public event FinishDelegate Finish;

    private readonly MonoBehaviour runner;
    private readonly string targetSceneName;
    public const int PhaseCount = 8;
    private readonly PhaseInfo[] phaseInfos;
    public bool IsFinished { get; private set; }

    public SceneLoad(MonoBehaviour runner,string targetSceneName)
    {
	this.runner = runner;
	this.targetSceneName = targetSceneName;
	phaseInfos = new PhaseInfo[PhaseCount];
	for (int i = 0; i < PhaseCount; i++)
	{
	    phaseInfos[i] = new PhaseInfo
	    {
		BeginTime = null
	    };
	}
    }

    public void Begin()
    {
	runner.StartCoroutine(BeginRoutine());
    }

    private IEnumerator BeginRoutine()
    {
	RecordBeginTime(Phases.FetchBlocked);
	while (!IsFetchAllowed)
	{
	    yield return null;
	}
	RecordEndTime(Phases.FetchBlocked);
	RecordBeginTime(Phases.Fetch);
	AsyncOperation loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
	loadOperation.allowSceneActivation = true;
	while(loadOperation.progress < 0.9f)
	{
	    yield return null;
	}
	RecordEndTime(Phases.Fetch);
	if (FetchComplete != null)
	{
	    try
	    {
		FetchComplete();
	    }
	    catch (Exception exception)
	    {
		Debug.LogError("Exception in responders to SceneLoad.FetchComplete. Attempting to continue load regardless.");
		Debug.LogException(exception);
	    }
	}
	RecordBeginTime(Phases.ActivationBlocked);
	while (!IsActivationAllowed)
	{
	    yield return null;
	}
	RecordEndTime(Phases.ActivationBlocked);
	RecordBeginTime(Phases.Activation);
	if(WillActivate != null)
	{
	    try
	    {
		WillActivate();
	    }
	    catch (Exception exception2)
	    {
		Debug.LogError("Exception in responders to SceneLoad.WillActivate. Attempting to continue load regardless.");
		Debug.LogException(exception2);
	    }
	}
	loadOperation.allowSceneActivation = true;
	yield return loadOperation;
	RecordEndTime(Phases.Activation);
	if(ActivationComplete != null)
	{
	    try
	    {
		ActivationComplete();
	    }
	    catch (Exception exception3)
	    {
		Debug.LogError("Exception in responders to SceneLoad.ActivationComplete. Attempting to continue load regardless.");
		Debug.LogException(exception3);
	    }
	}
	RecordBeginTime(Phases.UnloadUnusedAssets);
	if (IsUnloadAssetsRequired)
	{
	    AsyncOperation asyncOperation = Resources.UnloadUnusedAssets();
	    yield return asyncOperation;
	}
	RecordEndTime(Phases.UnloadUnusedAssets);
	RecordBeginTime(Phases.GarbageCollect);
	if (IsGarbageCollectRequired)
	{
	    
	}
	RecordEndTime(Phases.GarbageCollect);
	if(Complete != null)
	{
	    try
	    {
		Complete();
	    }
	    catch (Exception exception4)
	    {
		Debug.LogError("Exception in responders to SceneLoad.Complete. Attempting to continue load regardless.");
		Debug.LogException(exception4);
	    }
	}
	RecordBeginTime(Phases.StartCall);
	yield return null;
	RecordEndTime(Phases.StartCall);
	if (StartCalled != null)
	{
	    try
	    {
		StartCalled();
	    }
	    catch (Exception exception5)
	    {
		Debug.LogError("Exception in responders to SceneLoad.StartCalled. Attempting to continue load regardless.");
		Debug.LogException(exception5);
	    }
	}
	IsFinished = true;
	if (Finish != null)
	{
	    try
	    {
		Finish();
		yield break;
	    }
	    catch (Exception exception8)
	    {
		Debug.LogError("Exception in responders to SceneLoad.Finish. Attempting to continue load regardless.");
		Debug.LogException(exception8);
		yield break;
	    }
	}
    }

    /// <summary>
    /// 记录开启转移的时间
    /// </summary>
    /// <param name="phase"></param>
    private void RecordBeginTime(Phases phase)
    {
	phaseInfos[(int)phase].BeginTime = new float?(Time.realtimeSinceStartup);
    }

    /// <summary>
    /// 记录结束转移后的时间
    /// </summary>
    /// <param name="phase"></param>
    private void RecordEndTime(Phases phase)
    {
	phaseInfos[(int)phase].EndTime = new float?(Time.realtimeSinceStartup);
    }

    private class PhaseInfo
    {
	public float? BeginTime;
	public float? EndTime;
    }

    public enum Phases
    {
	FetchBlocked,
	Fetch,
	ActivationBlocked,
	Activation,
	UnloadUnusedAssets,
	GarbageCollect,
	StartCall,
	LoadBoss
    }
}
