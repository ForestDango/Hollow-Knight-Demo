using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Language
{
    public static class Language
    {
	public static string settingsAssetPath = "Assets/Localization/Resources/Languages/LocalizationSettings.asset";
	private static LocalizationSettings _settings = null;
        private static List<string> availableLanguages;
        private static LanguageCode currentLanguage = LanguageCode.N;
        private static Dictionary<string, Dictionary<string, string>> currentEntrySheets;
	public static LocalizationSettings settings
	{
	    get
	    {
		if (_settings == null)
		{
		    _settings = (LocalizationSettings)Resources.Load("Languages/" + Path.GetFileNameWithoutExtension(settingsAssetPath), typeof(LocalizationSettings));
		}
		return _settings;
	    }
	}

	static Language()
	{
	    LoadAvailableLanguages();
	    LoadLanguage();
	}

	/// <summary>
	/// 加载当前能够使用的语言
	/// </summary>
	private static void LoadAvailableLanguages()
	{
	    availableLanguages = new List<string>();
	    if(settings.sheetTitles == null ||settings.sheetTitles.Length == 0)
	    {
		Debug.Log("None available");
		return;
	    }
	    foreach (object obj in Enum.GetValues(typeof(LanguageCode)))
	    {
		LanguageCode languageCode = (LanguageCode)obj;
		if (HasLanguageFile(languageCode.ToString() ?? "", settings.sheetTitles[0]))
		{
		    availableLanguages.Add(languageCode.ToString() ?? "");
		}
	    }
	    StringBuilder stringBuilder = new StringBuilder("Discovered supported languages: ");
	    for (int i = 0; i < availableLanguages.Count; i++)
	    {
		stringBuilder.Append(availableLanguages[i]);
		if (i < availableLanguages.Count - 1)
		{
		    stringBuilder.Append(", ");
		}
	    }
	    Debug.Log(stringBuilder.ToString());//发现英语EN是可以使用的 加入到availableLanguages的列表当中
	    Resources.UnloadUnusedAssets();
	}

	/// <summary>
	/// 格式是语言代号EN_sheetTitle的名字
	/// </summary>
	/// <param name="lang"></param>
	/// <param name="sheetTitle"></param>
	/// <returns></returns>
	private static bool HasLanguageFile(string lang, string sheetTitle)
	{
	    return (TextAsset)Resources.Load("Languages/" + lang + "_" + sheetTitle, typeof(TextAsset)) != null;
	}

	/// <summary>
	/// 加载当前使用的语言
	/// </summary>
	public static void LoadLanguage()
	{
	    string text = RestoreLanguageSelection();
	    Debug.LogFormat("Restored language code '{0}'", new object[]
	    {
		text
	    });
	    SwitchLanguage(text);
	}

	/// <summary>
	/// 暂时先加载UNITY系统正在使用的语言
	/// </summary>
	/// <returns></returns>
	private static string RestoreLanguageSelection()
	{

	    if (settings.useSystemLanguagePerDefault)
	    {
		SystemLanguage systemLanguage = Platform.Current.GetSystemLanguage();
		Debug.LogFormat("Loaded system language '{0}'", new object[]
		{
		    systemLanguage
		});
		string text = LanguageNameToCode(systemLanguage).ToString();
		Debug.LogFormat("Loaded system language code '{0}'", new object[]
		{
		    text
		});
		if (availableLanguages.Contains(text))
		{
		    return text;
		}
		Debug.LogErrorFormat("System language code '{0}' is not an available language", new object[]
		{
		    text
		});
	    }
	    Debug.LogFormat("Falling back to default language code '{0}'", new object[]
	    {
		settings.defaultLangCode
	    });
	    return LocalizationSettings.GetLanguageEnum(settings.defaultLangCode).ToString();
	}

	public static bool SwitchLanguage(string langCode)
	{
	    return SwitchLanguage(LocalizationSettings.GetLanguageEnum(langCode));
	}

	public static bool SwitchLanguage(LanguageCode code)
	{
	    if(availableLanguages.Contains(code.ToString() ?? ""))
	    {
		DoSwitch(code);
		return true;
	    }
	    Debug.LogError("Could not switch from language " + currentLanguage.ToString() + " to " + code.ToString());
	    if(currentLanguage == LanguageCode.N)
	    {
		if(availableLanguages.Count > 0)
		{
		    DoSwitch(LocalizationSettings.GetLanguageEnum(availableLanguages[0]));
		    Debug.LogError("Switched to " + currentLanguage.ToString() + " instead");
		}
		else
		{
		    Debug.LogError("Please verify that you have the file: Resources/Languages/" + code.ToString());
		    Debug.Break();
		}
	    }
	    return false;
	}

	private static void DoSwitch(LanguageCode newLang)
	{
	    currentLanguage = newLang;
	    currentEntrySheets = new Dictionary<string, Dictionary<string, string>>();
	    foreach (string text in settings.sheetTitles)
	    {
		currentEntrySheets[text] = new Dictionary<string, string>();
		string languageFileContents = GetLanguageFileContents(text);
		if(languageFileContents != "")
		{
		    using (XmlReader xmlReader = XmlReader.Create(new StringReader(languageFileContents)))
		    {
			while (xmlReader.ReadToFollowing("entry"))
			{
			    xmlReader.MoveToFirstAttribute();
			    string value = xmlReader.Value;
			    xmlReader.MoveToElement();
			    string text2 = xmlReader.ReadElementContentAsString().Trim();
			    text2 = text2.UnescapeXML();
			    currentEntrySheets[text][value] = text2;
			}
		    }
		}
	    }
	    //TODO:LocalizeAsset
	    SendMonoMessage("ChangedLanguage", new object[]
	    {
		currentLanguage
	    });
	    //TODO:Config
	}

	private static string GetLanguageFileContents(string sheetTitle)
	{
	    TextAsset textAsset = (TextAsset)Resources.Load("Languages/" + currentLanguage.ToString() + "_" + sheetTitle, typeof(TextAsset));
	    if (!(textAsset != null))
	    {
		return "";
	    }
	    return textAsset.text;
	}

	private static void SendMonoMessage(string methodString, params object[] parameters)
	{
	    if (parameters != null && parameters.Length > 1)
	    {
		Debug.LogError("We cannot pass more than one argument currently!");
	    }
	    foreach (GameObject gameObject in (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
	    {
		if (gameObject && gameObject.transform.parent == null)
		{
		    if (parameters != null && parameters.Length == 1)
		    {
			gameObject.gameObject.BroadcastMessage(methodString, parameters[0], SendMessageOptions.DontRequireReceiver);
		    }
		    else
		    {
			gameObject.gameObject.BroadcastMessage(methodString, SendMessageOptions.DontRequireReceiver);
		    }
		}
	    }
	}

	public static LanguageCode CurrentLanguage()
	{
	    return currentLanguage;
	}

	/// <summary>
	/// 获取文本默认为第0个sheetTitle
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public static string Get(string key)
	{
	    return Get(key, settings.sheetTitles[0]);
	}

	/// <summary>
	/// 获取文本根据某个特定的sheetTitle,一定要有key在currentEntrySheets[sheetTitle]中
	/// </summary>
	/// <param name="key"></param>
	/// <param name="sheetTitle"></param>
	/// <returns></returns>
	public static string Get(string key, string sheetTitle)
	{
	    if (currentEntrySheets == null || !currentEntrySheets.ContainsKey(sheetTitle))
	    {
		Debug.LogError("The sheet with title \"" + sheetTitle + "\" does not exist!");
		return "";
	    }
	    if (currentEntrySheets[sheetTitle].ContainsKey(key))
	    {
		return currentEntrySheets[sheetTitle][key];
	    }
	    return "#!#" + key + "#!#";
	}

	public static LanguageCode LanguageNameToCode(SystemLanguage name)
        {
            if(name == SystemLanguage.English)
	    {
                Debug.LogFormat("Current Activate Language is EN!");
                return LanguageCode.EN;
	    }
            return LanguageCode.EN;
        }
    }
}