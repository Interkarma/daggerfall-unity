Smart Localization 2 by Niklas Borglund & Jakob Hillerström 

niklasborglund[at]gmail[dot]com, @NiklasBorglund
zurric[at]gmail[dot]com, @zuric 

http://www.janetech.co/

Unity forum support thread: http://forum.unity3d.com/threads/173837-RELEASED-Smart-Localization-for-Unity3D

---------------------------------------

Localizing your game has never been this easy. Localize your game with only a few mouse clicks.

Just open the Smart Localization Window in Window->Smart Localization and start by pressing 
"Create new localization system".
Edit the root language file by adding some keys and base values.

To add a new language to your project, choose a language in the "Add/Update Languages" panel and click "Create".
To edit and translate a language, press the "Translate" button on the language you want to work with in the 
Created Languages panel.

There's an example scene in SmartLocalization->Examples called LoadAllLanguages, which draws all the available
languages and values on the screen. The code for the example is also included and lies within the same folder.

Code Examples
--------------
//Returns a text value in the current language for the key
string myKey = LanguageManager.Instance.GetTextValue("MYKEY");

//Gets the audio clip for the current language
AudioClip myClip = LanguageManager.Instance.GetAudioClip("MYKEY"); 

//Gets the prefab game object for the current language
GameObject myPrefab = LanguageManager.Instance.GetPrefab("MYKEY");

//Gets the texture for the current language
Texture myTexture = LanguageManager.Instance.GetTexture("MYKEY");

//(Pro feature)Gets a localized TextAsset
TextAsset myTextAsset = LanguageManager.Instance.GetTextAsset("MYKEY");

//To cache the LanguageManager in a variable
LanguageManager languageManager = LanguageManager.Instance;

//Get a list of all the available languages
List<SmartCultureInfo> availableLanguages = thisLanguageManager.GetSupportedLanguages();

Get the smart culture info of the system language if it is supported. otherwise it will return null
SmartCultureInfo systemLanguage = thisLanguageManager.GetSupportedSystemLanguage();

//Check if a language is supported with an ISO-639 language code (string = "en" "sv" "es" etc.)
LanguageManager.Instance.IsLanguageSupported("en");

//Check if a language is supported with an instance of SmartCultureInfo
SmartCultureInfo swedishCulture = new SmartCultureInfo("sv", "Swedish", "Svenska", false);
LanguageManager.Instance.IsLanguageSupported(swedishCulture);

//Change a language with an ISO-639 language code ("en" "sv" "es" etc., Make sure the language is supported)
LanguageManager.Instance.ChangeLanguage("en");

//Change the language with a SmartCultureInfo instance
SmartCultureInfo swedishCulture = new SmartCultureInfo("sv", "Swedish", "Svenska", false);
LanguageManager.Instance.ChangeLanguage(swedishCulture);

//How to register on the event that fires when a language changed
LanguageManager.Instance.OnChangeLanguage += OnLanguageChanged; //OnLanguageChanged = delegate method that you created

//Enable extensive debug logging
LanguageManager.Instance.VerboseLogging = true;

//Check if a localized key exists
LanguageManager.Instance.HasKey("myKey")

Smart Localization saves the language files in the .resx file format. ResX is commonly used for globalization
and localization. A wide variety of software is used to translate .resx files.
An example of a good editor is: http://resex.codeplex.com/ 

The plugin also supports automatic translation with Microsoft Translator to help you create temporary values
for different languages and cultures while developing your game. Microsoft Translator gives you two million
characters each month for free that you can use with this plugin. 
To enable this feature, you need to create a new Microsoft Translator account. Instructions on how you can do that
can be found here: http://blogs.msdn.com/b/translation/p/gettingstarted1.aspx 
