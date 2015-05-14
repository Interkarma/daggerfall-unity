// SL_NGUISpriteName.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

//#define SMARTLOC_NGUI //<--- UNCOMMENT THIS FOR NGUI CLASSES

#if SMARTLOC_NGUI
using UnityEngine;
using System.Collections;
using SmartLocalization;

public class SL_NGUISpriteName : MonoBehaviour 
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
		LanguageManager languageManager = LanguageManager.Instance;
		languageManager.OnChangeLanguage += OnChangeLanguage;
		
		OnChangeLanguage(languageManager);
	}
	
	void OnDestroy()
	{
		if(LanguageManager.HasInstance)
		{
			LanguageManager.Instance.OnChangeLanguage -= OnChangeLanguage;
		}
	}
	
	void OnChangeLanguage(LanguageManager languageManager)
	{
		uiSprite.spriteName = languageManager.GetTextValue(localizedKey);
	}	
}
#endif