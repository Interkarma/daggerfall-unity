Smart Localization 2.0 by Niklas Borglund & Jakob Hillerström 

niklasborglund[at]gmail[dot]com, @NiklasBorglund
zurric[at]gmail[dot]com, @zuric 
---------------------------------------
Localizing your game has never been this easy. Localize your game with only a few mouse clicks.

Just open the Smart Localization Window in Window->Smart Localization and start by pressing 
"Create new localization system".
Edit the root language file by adding some keys and base values.

To create a new language, choose a language from the drop down menu and press "Create Language". To edit and
translate a language, press the name of the language in the language list in the main window.

The example scene in Assets->SmartLocalization->Scenes->LoadAllLanguages will load a list of all 
the available languages that you have created along with your keys/values.

This plugin also supports Microsoft Translator. All you have to do is to create a Microsoft Translator account
and paste in your credentials in the main window and press "Save" and "Authenticate". The translate language
window will show a drop down menu of languages to translate from if that specific language is supported.
Microsoft Translator gives you 2 million characters to translate each month for free.
Guide to get a Microsoft Translator Account: http://blogs.msdn.com/b/translation/p/gettingstarted1.aspx 

To get the values at runtime:

//Returns a text value in the current language for the key
string myKey = LanguageManager.Instance.GetTextValue("MYKEY");

//Gets the audio clip for the current language
AudioClip myClip = LanguageManager.Instance.GetAudioClip("MYKEY"); 

//Gets the prefab game object for the current language
GameObject myPrefab = LanguageManager.Instance.GetPrefab("MYKEY");

//Gets the texture for the current language
Texture myTexture = LanguageManager.Instance.GetTexture("MYKEY");

//To cache the LanguageManager in a variable
LanguageManager languageManager = LanguageManager.Instance;

//Get a list of all the available languages
List<SmartCultureInfo> availableLanguages = thisLanguageManager.GetSupportedLanguages();

Get the smart culture info of the system language if it is supported. otherwise it will return null
SmartCultureInfo systemLanguage = thisLanguageManager.GetSupportedSystemLanguage();

//Check if a language is supported(string = "en" "sv" "es" etc.)
LanguageManager.Instance.IsLanguageSupported("en");

//Change a language(string = "en" "sv" "es" etc., Make sure the language is supported)
LanguageManager.Instance.ChangeLanguage("en");

//Set the default language(The language that will be loaded(if it exists) at startup)
//takes System.Globalization.CultureInfo or string
LanguageManager.Instance.SetDefaultLanguage("en");
LanguageManager.Instance.SetDefaultLanguage(currentCultureInfo);


Smart Localization saves the language files in the .resx file format. ResX is commonly used for globalization
and localization. A wide variety of software is used to translate .resx files.
An example of a good editor is: http://resex.codeplex.com/ 

Video tutorials(They are made for v.1.2, but most of it is still valid): 
http://www.youtube.com/watch?v=f6f2Osufjno
http://www.youtube.com/watch?v=DABGRPXhooc

Unity forum thread:
http://forum.unity3d.com/threads/173837-RELEASED-Smart-Localization-for-Unity3D


contact information: 
niklasborglund[at]gmail[dot]com
https://twitter.com/NiklasBorglund

zurric[at]gmail[dot]com
https://twitter.com/zuric