using System;
using UnityEngine;

public class AreaTitle : MonoBehaviour
{
    public static AreaTitle instance;
    private void Awake()
    {
	instance = this;
	gameObject.SetActive(false);
    }
}
