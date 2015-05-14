#if UNITY_4_6
namespace SmartLocalization.Editor
{
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Text))]
public class LocalizedText : MonoBehaviour 
{
	public string localizedKey = "INSERT_KEY_HERE";
	Text textObject;
	
	void Start () 
	{
		textObject = this.GetComponent<Text>();
	
		//Subscribe to the change language event
		LanguageManager languageManager = LanguageManager.Instance;
		languageManager.OnChangeLanguage += OnChangeLanguage;
		
		//Run the method one first time
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
		textObject.text = LanguageManager.Instance.GetTextValue(localizedKey);
	}
}
}
#endif