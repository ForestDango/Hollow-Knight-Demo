using System;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


[Serializable]
public class SceneManager : MonoBehaviour
{
    public int darknessLevel;
    public int GetDarknessLevel()
    {
	return darknessLevel;
    }
}
