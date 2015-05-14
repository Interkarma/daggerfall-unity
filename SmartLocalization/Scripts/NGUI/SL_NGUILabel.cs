// SL_NGUILabel.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

//#define SMARTLOC_NGUI //<--- UNCOMMENT THIS FOR NGUI CLASSES

#if SMARTLOC_NGUI
using UnityEngine;
using System.Collections;
using SmartLocalization;

public class SL_NGUILabel : MonoBehaviour 
{
	public string localizedKey = "INSERT_KEY_HERE";
	UILabel	uiLabel	= null;
	
	void Awake()
	{
		uiLabel = GetComponent<UILabel>();
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
		uiLabel.text = LanguageManager.Instance.GetTextValue(localizedKey);
	}	
}
#endif