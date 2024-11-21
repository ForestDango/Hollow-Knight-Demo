using System;
using Language;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationSettings", menuName = "Hollow Knight/Localization Settings", order = 1000)]
[Serializable]
public class LocalizationSettings : ScriptableObject
{
    public string[] sheetTitles;
    public bool useSystemLanguagePerDefault = true;
    public string defaultLangCode = "EN";
    public string gDocURL;

    public static LanguageCode GetLanguageEnum(string langCode)
    {
	langCode = langCode.ToUpper();
	foreach (object obj in Enum.GetValues(typeof(LanguageCode)))
	{
	    LanguageCode result = (LanguageCode)obj;
	    if (result.ToString().Equals(langCode, StringComparison.InvariantCultureIgnoreCase))
	    {
		return result;
	    }
	}
	Debug.LogError("ERORR: There is no language: [" + langCode + "]");
	return LanguageCode.EN;
    }
}
