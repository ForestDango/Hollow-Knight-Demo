using System;
using UnityEngine;

public class LoadingCanvas : MonoBehaviour
{
    [SerializeField]
    [ArrayForEnum(typeof(GameManager.SceneLoadVisualizations))]
    private GameObject[] visualizationContainers;
    private bool isLoading;
    private GameManager.SceneLoadVisualizations loadingVisualization;
    [SerializeField] private LoadingSpinner defaultLoadingSpinner;
    [SerializeField] private float continueFromSaveDelayAdjustment;

    protected void Start()
    {
	for (int i = 0; i < visualizationContainers.Length; i++)
	{
	    GameObject gameObject = visualizationContainers[i];
	    if (!(gameObject == null))
	    {
		gameObject.SetActive(false);
	    }
	}
    }

    protected void Update()
    {
	GameManager unsafeInstance = GameManager.UnsafeInstance;
	if(unsafeInstance != null && isLoading != unsafeInstance.IsLoadingSceneTransition)
	{
	    isLoading = unsafeInstance.IsLoadingSceneTransition;
	    if (isLoading)
	    {
		defaultLoadingSpinner.DisplayDelayAdjustment = ((unsafeInstance.LoadVisualization == GameManager.SceneLoadVisualizations.ContinueFromSave) ? continueFromSaveDelayAdjustment : 0f);
		GameObject y = null;
		if (unsafeInstance.LoadVisualization >= GameManager.SceneLoadVisualizations.Default && unsafeInstance.LoadVisualization < (GameManager.SceneLoadVisualizations)visualizationContainers.Length)
		{
		    y = visualizationContainers[(int)unsafeInstance.LoadVisualization];
		}
		for (int i = 0; i < visualizationContainers.Length; i++)
		{
		    GameObject gameObject = visualizationContainers[i];
		    if(!(gameObject == null))
		    {
			gameObject.SetActive(gameObject == y);
		    }
		}
	    }
	}
    }

}
