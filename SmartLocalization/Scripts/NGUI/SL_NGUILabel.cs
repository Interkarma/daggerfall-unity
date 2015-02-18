// SL_NGUILabel.cs
//
// Copyright (c) 2013-2014 Niklas Borglund, Jakob Hillerström
//

//#define SMART_LOC_NGUI //<--- UNCOMMENT THIS FOR NGUI CLASSES

#if SMART_LOC_NGUI
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
		uiLabel.text = LanguageManager.Instance.GetTextValue(localizedKey);
	}	
}
#endif