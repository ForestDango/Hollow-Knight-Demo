using System;
using System.Collections.Generic;
using UnityEngine;

public static class LocalisationHelper
{
    public static string GetProcessed(this string text, FontSource fontSource)
    {
	if (substitutions.ContainsKey(fontSource))
	{
	    string text2 = text;
	    foreach (KeyValuePair<string, string> keyValuePair in substitutions[fontSource])
	    {
		text2 = text2.Replace(keyValuePair.Key, keyValuePair.Value);
	    }
	    if (text2 != text)
	    {
		Debug.Log(string.Format("LocalisationHelper processed string \"<b>{0}</b>\", result: \"<b>{1}</b>\".", text, text2));
		text = text2;
	    }
	}
	return text;
    }

    private static Dictionary<FontSource, Dictionary<string, string>> substitutions = new Dictionary<FontSource, Dictionary<string, string>>
    {
	{
	    FontSource.Trajan,
	    new Dictionary<string, string>
	    {
	    {
		"ß",
		"ss"
	    }
	    }
	}
    };

    public enum FontSource
    {
	Trajan,
	Perpetua
    }
}
