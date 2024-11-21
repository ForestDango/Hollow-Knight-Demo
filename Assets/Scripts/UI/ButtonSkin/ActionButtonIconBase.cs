using System;
using GlobalEnums;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class ActionButtonIconBase : MonoBehaviour
{
    public abstract HeroActionButton Action { get; }

    public void RefreshButtonIcon()
    {
	Debug.LogFormat("TODO:Refresh Button Icon");
    }
}
