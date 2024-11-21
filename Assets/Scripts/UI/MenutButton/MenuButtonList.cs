using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuButtonList : MonoBehaviour
{
    private MenuSelectable lastSelected;
    private List<Selectable> activeSelectables;
    private static List<MenuButtonList> menuButtonLists = new List<MenuButtonList>();
    private bool started;

    private void Awake()
    {
	MenuScreen component = GetComponent<MenuScreen>();
	if(component != null)
	{
	    component.defaultHighlight = null;
	}
    }
    protected void Start()
    {
	menuButtonLists.Add(this);
	activeSelectables = new List<Selectable>();

    }
    protected void OnDestroy()
    {
	menuButtonLists.Remove(this);
    }

    public static void ClearAllLastSelected()
    {
	foreach (MenuButtonList menuButtonList in menuButtonLists)
	{
	    menuButtonList.ClearLastSelected();
	}
    }
    public void ClearLastSelected()
    {
	lastSelected = null;
    }

}
