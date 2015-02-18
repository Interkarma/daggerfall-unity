//
//  LoadAllLanguages.cs
//
//
// Copyright (c) 2013-2014 Niklas Borglund, Jakob Hillerström
//

namespace SmartLocalization
{
using UnityEngine;
using System.Collections.Generic;

public class LoadAllLanguages : MonoBehaviour 
{
	private Dictionary<string,string> currentLanguageValues;
	private List<SmartCultureInfo> availableLanguages;
	private LanguageManager thisLanguageManager;
	private Vector2 valuesScrollPosition = Vector2.zero;
	private Vector2 languagesScrollPosition = Vector2.zero;

	void Start () 
	{
		thisLanguageManager = LanguageManager.Instance;
		
		SmartCultureInfo systemLanguage = thisLanguageManager.GetSupportedSystemLanguage();
		if(systemLanguage != null)
		{
			thisLanguageManager.ChangeLanguage(systemLanguage);	
		}
		
		if(thisLanguageManager.NumberOfSupportedLanguages > 0)
		{
			currentLanguageValues = thisLanguageManager.RawTextDatabase;
			availableLanguages = thisLanguageManager.GetSupportedLanguages();
		}
		else
		{
			Debug.LogError("No languages are created!, Open the Smart Localization plugin at Window->Smart Localization and create your language!");
		}

		LanguageManager.Instance.OnChangeLanguage += OnLanguageChanged;
	}

	void OnDestroy()
	{
		if(LanguageManager.HasInstance)
			LanguageManager.Instance.OnChangeLanguage -= OnLanguageChanged;
	}

	void OnLanguageChanged(LanguageManager thisLanguageManager)
	{
		currentLanguageValues = thisLanguageManager.RawTextDatabase;
	}
	
	void OnGUI() 
	{
		if(thisLanguageManager.NumberOfSupportedLanguages > 0)
		{
			if(thisLanguageManager.CurrentlyLoadedCulture != null)
				GUILayout.Label("Current Language:" + thisLanguageManager.CurrentlyLoadedCulture.ToString());
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Keys:", GUILayout.Width(460));
			GUILayout.Label("Values:", GUILayout.Width(460));
			GUILayout.EndHorizontal();
			
			valuesScrollPosition = GUILayout.BeginScrollView(valuesScrollPosition);
			foreach(KeyValuePair<string,string> languageValue in currentLanguageValues)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(languageValue.Key, GUILayout.Width(460));
				GUILayout.Label(languageValue.Value, GUILayout.Width(460));
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			
			languagesScrollPosition = GUILayout.BeginScrollView (languagesScrollPosition);
			foreach(SmartCultureInfo language in availableLanguages)
			{
				if(GUILayout.Button(language.nativeName, GUILayout.Width(960)))
				{
					thisLanguageManager.ChangeLanguage(language);
				}
			}

			GUILayout.EndScrollView();
		}
	}
}
}//namespace SmartLocalization
