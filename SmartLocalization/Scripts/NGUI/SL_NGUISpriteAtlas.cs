// SL_NGUISpriteAtlas.cs
//
// Copyright (c) 2013-2014 Niklas Borglund, Jakob Hillerström
//

//#define SMART_LOC_NGUI //<--- UNCOMMENT THIS FOR NGUI CLASSES

#if SMART_LOC_NGUI
using UnityEngine;
using System.Collections;
using SmartLocalization;

public class SL_NGUISpriteAtlas : MonoBehaviour 
{
	public string localizedKey = "INSERT KEY HERE";
	UISprite uiSprite = null;

	void Awake()
	{
		uiSprite = GetComponent<UISprite>();
	}
	
	void Start () 
	{
		//Subscribe to the change language event
		LanguageManager thisLanguageManager = LanguageManager.Instance;
		thisLanguageManager.OnChangeLanguage += OnChangeLanguage;
		
		OnChangeLanguage(thisLanguageManager);
	}
	
	void OnDestroy()
	{
		if(LanguageManager.HasInstance)
			LanguageManager.Instance.OnChangeLanguage -= OnChangeLanguage;
	}
	
	void OnChangeLanguage(LanguageManager thisLanguageManager)
	{
		GameObject atlasPrefab = thisLanguageManager.GetPrefab(localizedKey);
		if(atlasPrefab != null)
		{
			UIAtlas atlasObject = atlasPrefab.GetComponent<UIAtlas>();
			if(atlasObject == null)
			{
				if(thisLanguageManager.VerboseLogging)
					Debug.LogError("No UIAtlas was found with key:" + localizedKey);
			}
			else
			{
				uiSprite.atlas = atlasObject;
			}
		}
	}	
}
#endif