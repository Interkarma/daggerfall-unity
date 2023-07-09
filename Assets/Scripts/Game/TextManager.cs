// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Wenzil.Console;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine.Localization.Tables;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Text manager singleton class.
    /// </summary>
    public class TextManager : MonoBehaviour
    {
        #region Fields

        public static string defaultInternalStringsCollectionName = "Internal_Strings";
        public static string defaultInternalRSCCollectionName = "Internal_RSC";
        public static string defaultInternalFlatsCollectionName = "Internal_Flats";
        public static string defaultInternalQuestsCollectionName = "Internal_Quests";
        public static string defaultInternalLocationsCollectionName = "Internal_Locations";
        public static string defaultInternalSettingsCollectionName = "Internal_Settings";
        public static string defaultInternalSpellsCollectionName = "Internal_Spells";
        public static string defaultInternalItemsCollectionName = "Internal_Items";
        public static string defaultInternalMagicItemsCollectionName = "Internal_MagicItems";
        public static string defaultInternalFactionsCollectionName = "Internal_Factions";

        const string localizedTextLookupError = "<LocaleText-NotFound>";
        const string textFolderName = "Text";
        const string textColumn = "text";

        readonly char[] trimAtEnd = { '\n', '\r' };
        readonly string[] newlineSequences = { "\r\n", "\r", "\n" };

        public string runtimeInternalStrings = defaultInternalStringsCollectionName;
        public string runtimeRSCStrings = defaultInternalRSCCollectionName;
        public string runtimeFlatStrings = defaultInternalFlatsCollectionName;
        public string runtimeQuestsStrings = defaultInternalQuestsCollectionName;
        public string runtimeLocationsStrings = defaultInternalLocationsCollectionName;
        public string runtimeSettingsStrings = defaultInternalSettingsCollectionName;
        public string runtimeSpellsStrings = defaultInternalSpellsCollectionName;
        public string runtimeItemsStrings = defaultInternalItemsCollectionName;
        public string runtimeMagicItemsStrings = defaultInternalMagicItemsCollectionName;
        public string runtimeFactionsStrings = defaultInternalFactionsCollectionName;

        // String table copy editor properties
        public bool tableCopyOverwriteTargetStringTables = false;
        public string tableCopyTargetInternalStrings = null;
        public string tableCopyTargetRSCStrings = null;
        public string tableCopyTargetBOKStrings = null;
        public string tableCopyTargetFlatStrings = null;
        public string tableCopyTargetLocationStrings = null;

        Dictionary<string, Table> textDatabases = new Dictionary<string, Table>();
        Dictionary<string, string[]> cachedLocalizedTextLists = new Dictionary<string, string[]>();
        Dictionary<string, DaggerfallFont> localizedFonts = new Dictionary<string, DaggerfallFont>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets name of StringTable collection to use in place of Internal_Strings.
        /// </summary>
        public string RuntimeInternalStrings
        {
            get { return runtimeInternalStrings; }
            set { runtimeInternalStrings = value; }
        }

        /// <summary>
        /// Gets or sets name of StringTable collection to use in place of Internal_RSC.
        /// </summary>
        public string RuntimeRSCStrings
        {
            get { return runtimeRSCStrings; }
            set { runtimeRSCStrings = value; }
        }

        #endregion

        #region Unity

        private void Awake()
        {
            EnumerateTextDatabases();    
        }

        private void Start()
        {
            ConsoleCommandsDatabase.RegisterCommand(Locale_Print.name, Locale_Print.description, Locale_Print.usage, Locale_Print.Execute);
            ConsoleCommandsDatabase.RegisterCommand(Locale_Set.name, Locale_Set.description, Locale_Set.usage, Locale_Set.Execute);
            ConsoleCommandsDatabase.RegisterCommand(Locale_Debug.name, Locale_Debug.description, Locale_Debug.usage, Locale_Debug.Execute);
        }

        #endregion

        #region Localized Fonts

        /// <summary>
        /// Register a DaggerfallFont replacement to be used for specified locale.
        /// If this font name and locale replacement has already been registered, it will be replaced by this font.
        /// Note: Localized fonts can only be SDF capable. SDF font setting will be forced on by registering any custom font.
        /// </summary>
        /// <param name="locale">Locale of font.</param>
        /// <param name="fontName">Name of font, which must be one of the 5 Daggerfall fonts.</param>
        /// <param name="font">DaggerfallFont object to use for this font name and locale.</param>
        public void RegisterLocalizedFont(Locale locale, DaggerfallFont.FontName fontName, DaggerfallFont font)
        {
            if (locale == null || font == null)
                Debug.LogError("RegisterLocalizedFont() locale and font cannot be null.");

            string key = GetLocaleFontKey(locale, fontName);
            if (localizedFonts.ContainsKey(key))
                localizedFonts.Remove(key);

            localizedFonts.Add(key, font);
            DaggerfallUnity.Settings.SDFFontRendering = true;
        }

        /// <summary>
        /// Check if font of specified name and locale have been registered.
        /// </summary>
        /// <param name="locale">Locale of font.</param>
        /// <param name="fontName">Name of font, which must be one of the 5 Daggerfall fonts.</param>
        /// <returns>True if font of this name and locale has been registered.</returns>
        public bool HasLocalizedFont(Locale locale, DaggerfallFont.FontName fontName)
        {
            if (locale == null)
                return false;

            return localizedFonts.ContainsKey(GetLocaleFontKey(locale, fontName));
        }

        /// <summary>
        /// Check if font of specified name is available in current locale.
        /// </summary>
        /// <param name="fontName">Name of font, which must be one of the 5 Daggerfall fonts.</param>
        /// <returns>True if font of this name has been registered in current locale.</returns>
        public bool HasLocalizedFont(DaggerfallFont.FontName fontName)
        {
            Locale selectedLocale = null;
            var op = LocalizationSettings.SelectedLocaleAsync;
            op.WaitForCompletion();
            if (op.IsDone)
                selectedLocale = op.Result;
            else
                Debug.LogError("HasLocalizedFont() failed LocalizationSettings.SelectedLocaleAsync operation");

            return (selectedLocale != null) ? localizedFonts.ContainsKey(GetLocaleFontKey(selectedLocale, fontName)) : false;
        }

        /// <summary>
        /// Gets a registered Daggerfall replacement for specified name and locale.
        /// </summary>
        /// <param name="locale">Locale of font.</param>
        /// <param name="fontName">Name of font, which must be one of the 5 Daggerfall fonts.</param>
        /// <returns>DaggerfallFont object if registered, otherwise null.</returns>
        public DaggerfallFont GetLocalizedFont(Locale locale, DaggerfallFont.FontName fontName)
        {
            if (locale == null)
                return null;

            DaggerfallFont font;
            if (localizedFonts.TryGetValue(GetLocaleFontKey(locale, fontName), out font))
                return font;

            return null;
        }

        /// <summary>
        /// Gets a registered DaggerfallFont replacement for current locale.
        /// </summary>
        /// <param name="fontName"></param>
        /// <returns></returns>
        public DaggerfallFont GetLocalizedFont(DaggerfallFont.FontName fontName)
        {
            Locale selectedLocale = null;
            var op = LocalizationSettings.SelectedLocaleAsync;
            op.WaitForCompletion();
            if (op.IsDone)
                selectedLocale = op.Result;
            else
                Debug.LogError("GetLocalizedFont() failed LocalizationSettings.SelectedLocaleAsync operation");

            return (selectedLocale != null) ? GetLocalizedFont(selectedLocale, fontName) : null;
        }

        private string GetLocaleFontKey(Locale locale, DaggerfallFont.FontName fontName)
        {
            return string.Format("{0}_{1}", fontName, locale.name);
        }

        #endregion

        #region Text Database Methods

        /// <summary>
        /// Checks if text database table was found enumerated StreamingAssets/Text folder.
        /// </summary>
        /// <param name="databaseName">Name of database.</param>
        /// <returns>True if database was enumerated.</returns>
        public bool HasDatabase(string databaseName)
        {
            return textDatabases.ContainsKey(databaseName);
        }

        /// <summary>
        /// Checks if both text database and text key exists.
        /// </summary>
        /// <param name="databaseName">Name of database.</param>
        /// <param name="key">Key of text in database.</param>
        /// <returns>True if both database and text key enumerated.</returns>
        public bool HasText(string databaseName, string key)
        {
            if (!HasDatabase(databaseName))
                return false;

            return textDatabases[databaseName].HasValue(key);
        }

        /// <summary>
        /// Gets text value from database.
        /// </summary>
        /// <param name="databaseName">Name of text database.</param>
        /// <param name="key">Key of text in database.</param>
        /// <returns>Text if found, otherwise return an error string instead.</returns>
        public string GetText(string databaseName, string key)
        {
            // Show an error if text not found
            if (!HasText(databaseName, key))
                return "<TextError-NotFound>";

            return textDatabases[databaseName].GetValue(textColumn, key);
        }

        /// <summary>
        /// Gets live collection name from TextManager based on collection enum value.
        /// </summary>
        /// <param name="collection">Collection enum value.</param>
        /// <returns>Name of collection set to specified collection value.</returns>
        private string GetRuntimeCollectionName(TextCollections collection)
        {
            string collectionName;
            switch (collection)
            {
                default:
                case TextCollections.Internal:
                    collectionName = runtimeInternalStrings;
                    break;
                case TextCollections.TextRSC:
                    collectionName = runtimeRSCStrings;
                    break;
                case TextCollections.TextFlats:
                    collectionName = runtimeFlatStrings;
                    break;
                case TextCollections.TextQuests:
                    collectionName = runtimeQuestsStrings;
                    break;
                case TextCollections.TextLocations:
                    collectionName = runtimeLocationsStrings;
                    break;
                case TextCollections.TextSettings:
                    collectionName = runtimeSettingsStrings;
                    break;
                case TextCollections.TextSpells:
                    collectionName = runtimeSpellsStrings;
                    break;
                case TextCollections.TextItems:
                    collectionName = runtimeItemsStrings;
                    break;
                case TextCollections.TextMagicItems:
                    collectionName = runtimeMagicItemsStrings;
                    break;
                case TextCollections.Factions:
                    collectionName = runtimeFactionsStrings;
                    break;
            }

            return collectionName;
        }

        /// <summary>
        /// Gets default internal collection name for fallback.
        /// </summary>
        /// <param name="collection">Collection enum value.</param>
        /// <returns>Default internal name of collection for specified collection value.</returns>
        private string GetDefaultCollectionName(TextCollections collection)
        {
            string collectionName;
            switch (collection)
            {
                default:
                case TextCollections.Internal:
                    collectionName = defaultInternalStringsCollectionName;
                    break;
                case TextCollections.TextRSC:
                    collectionName = defaultInternalRSCCollectionName;
                    break;
                case TextCollections.TextQuests:
                    collectionName = defaultInternalQuestsCollectionName;
                    break;
                case TextCollections.TextLocations:
                    collectionName = defaultInternalLocationsCollectionName;
                    break;
                case TextCollections.TextSettings:
                    collectionName = defaultInternalSettingsCollectionName;
                    break;
                case TextCollections.TextSpells:
                    collectionName = defaultInternalSpellsCollectionName;
                    break;
                case TextCollections.TextItems:
                    collectionName = defaultInternalItemsCollectionName;
                    break;
                case TextCollections.TextMagicItems:
                    collectionName = defaultInternalMagicItemsCollectionName;
                    break;
                case TextCollections.Factions:
                    collectionName = defaultInternalFactionsCollectionName;
                    break;
            }

            return collectionName;
        }

        #endregion

        #region Public Localized Text Methods

        /// <summary>
        /// Gets text value from localized text collection.
        /// If text not found for live collection then will try to fallback to internal text.
        /// If text still not found will return an error string.
        /// </summary>
        /// <param name="key">Key of text in table.</param>
        /// <param name="collection">Enum value to lookup collection name in TextManager.</param>
        /// <param name="exception">True to throw detailed exception if text not found. False to just return error string.</param>
        /// <returns>Text if found, then fallback if found, then exception or error string if nothing found.</returns>
        public string GetLocalizedText(string key, TextCollections collection = TextCollections.Internal, bool exception = false)
        {
            string localizedText;
            if (TryGetLocalizedText(GetRuntimeCollectionName(collection), key, out localizedText))
                return localizedText;
            else if (TryGetLocalizedText(GetDefaultCollectionName(collection), key, out localizedText))
                return localizedText;
            else
            {
                if (exception)
                    throw new Exception(string.Format("Localized text not found for collection='{0}', key='{1}'", collection.ToString(), key));
                else
                    return localizedTextLookupError;
            }
        }

        /// <summary>
        /// Gets array of text from discrete items using a key array.
        /// </summary>
        /// <param name="keyArray">Array of keys to resolve back to text in list.</param>
        /// <param name="collection">Enum value to lookup collection name in TextManager.</param>
        /// <param name="exception">True to throw exception if text not found. False to just return null.</param>
        /// <returns>Text array if all keys found, otherwise returns null or throws exception.</returns>
        public string[] GetLocalizedTextListFromKeyArray(string[] keyArray, TextCollections collection = TextCollections.Internal, bool exception = true)
        {
            return GetLocalizedTextListFromKeyArray(GetRuntimeCollectionName(collection), keyArray, exception);
        }

        /// <summary>
        /// Gets an array of text where each line is considered an item in array.
        /// Entry will be read from table and split by newline characters into array.
        /// </summary>
        /// <param name="key">Key of text in table.</param>
        /// <param name="collection">Enum value to lookup collection name in TextManager.</param>
        /// <param name="exception">True to throw exception if text not found. False to just return null.</param>
        /// <returns>Text array if found, otherwise returns null or throws exception.</returns>
        public string[] GetLocalizedTextList(string key, TextCollections collection = TextCollections.Internal, bool exception = true)
        {
            return GetLocalizedTextList(GetRuntimeCollectionName(collection), key, exception);
        }

        /// <summary>
        /// Gets display name of an enemy from their ID.
        /// </summary>
        /// <param name="enemyID">ID of enemy. Valid IDs are 0-42 and 128-146, or values registered in Daggerfallentity.CustomCareerTemplates</param>
        /// <returns>Name of enemy from localization if found, or exception if not found.</returns>
        public string GetLocalizedEnemyName(int enemyID)
        {
            if (Enum.IsDefined(typeof(MobileTypes), (MobileTypes)enemyID))
            {
                string[] enemyNames = GetLocalizedTextList("enemyNames", exception: true);
                if (enemyID < 128)
                    return enemyNames[enemyID];
                else
                    return enemyNames[43 + enemyID - 128];
            }
            // Handle custom enemies
            else
            {
                string name = DaggerfallUnity.Instance.TextProvider.GetCustomEnemyName(enemyID);
                if (!string.IsNullOrEmpty(name))
                {
                    return name;
                }

                // Fallback to career name
                DaggerfallConnect.DFCareer career = DaggerfallEntity.GetCustomCareerTemplate(enemyID);
                if (career == null)
                {
                    Debug.LogError($"Enemy ID '{enemyID}' did not have a registered custom career template");
                    return "(invalid enemy)";
                }

                return career.Name;
            }
        }

        /// <summary>
        /// Gets localized version of region name from string tables for display purposes only.
        /// This is distinct from the canonical name which is used internally as a key and to group discovery data.
        /// </summary>
        /// <param name="regionIndex">Index of region.</param>
        /// <returns>Localized name of region.</returns>
        public string GetLocalizedRegionName(int regionIndex)
        {
            string[] regionNames = GetLocalizedTextList("regionNames");
            if (regionNames == null || regionNames.Length == 0 ||
                regionIndex < 0 || regionIndex >= regionNames.Length)
            {
                // Fallback to canonical name using MapsFile when localization not provided or index out of range
                return DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegionName(regionIndex);
            }
            return regionNames[regionIndex];
        }

        /// <summary>
        /// Gets localized version of location name from string tables for display purposes only.
        /// This is distinct from the canonical name which is used internally as a key and to group discovery data.
        /// Must provide canonical fallback name when calling method as it's expensive to reload location from data to lookup name.
        /// Caller should already have working location data loaded and canonical name readily available.
        /// </summary>
        /// <param name="mapId">MapTableData.MapId key of location.</param>
        /// <param name="fallback">Fallback canonical name.</param>
        /// <returns>Localized name of region or fallback if key not found in string tables.</returns>
        public string GetLocalizedLocationName(int mapId, string fallback)
        {
            string name;
            if (TryGetLocalizedText(TextCollections.TextLocations, mapId.ToString(), out name))
                return name;
            else
                return fallback;
        }

        /// <summary>
        /// Gets localized version of spell name from its ID.
        /// Fallback to default spell name if localized string not found.
        /// </summary>
        /// <param name="id">Spell ID.</param>
        /// <returns>Localized name of spell or default spell name.</returns>
        public string GetLocalizedSpellName(int id)
        {
            string name;
            if (TryGetLocalizedText(TextCollections.TextSpells, id.ToString(), out name))
                return name;
            else
                return GameManager.Instance.EntityEffectBroker.GetStandardSpellName(id);
        }

        /// <summary>
        /// Gets localized version of item template name from its ID.
        /// Caller must provide a fallback template name.
        /// </summary>
        /// <param name="id">Item ID.</param>
        /// <returns>Localized item template name or fallback name.</returns>
        public string GetLocalizedItemName(int id, string fallback)
        {
            string name;
            if (TryGetLocalizedText(TextCollections.TextItems, id.ToString(), out name))
                return name;
            else
                return fallback;
        }

        /// <summary>
        /// Gets localized version of magic item template name from its ID.
        /// Caller must provide a fallback template name.
        /// </summary>
        /// <param name="id">Magic item ID.</param>
        /// <returns>Localized magic item template name or fallback name.</returns>
        public string GetLocalizedMagicItemName(int id, string fallback)
        {
            string name;
            if (TryGetLocalizedText(TextCollections.TextMagicItems, id.ToString(), out name))
                return name;
            else
                return fallback;
        }

        /// <summary>
        /// Gets localized version of faction name from its ID.
        /// Caller must provide a fallback name.
        /// </summary>
        /// <param name="id">Faction ID.</param>
        /// <param name="fallback">Fallback name.</param>
        /// <returns>Localized faction name or fallback.</returns>
        public string GetLocalizedFactionName(int id, string fallback)
        {
            string name;
            if (TryGetLocalizedText(TextCollections.Factions, id.ToString(), out name))
                return name;
            else
                return fallback;
        }

        /// <summary>
        /// Tries to gets text value from localization in TextProvider.
        /// Will use current locale if available in collection.
        /// </summary>
        /// <param name="collection">Enum value to lookup collection name in TextManager.</param>
        /// <param name="key">Key of text in table.</param>
        /// <param name="localizedString">Result of lookup if found.</param>
        /// <returns>True if text found, otherwise false.</returns>
        public bool TryGetLocalizedText(TextCollections collection, string key, out string localizedString)
        {
            return TryGetLocalizedText(GetRuntimeCollectionName(collection), key, out localizedString);
        }

        #endregion

        #region Private Localized Text Methods

        /// <summary>
        /// Gets text value from localization in TextProvider.
        /// Will use current locale if available in collection.
        /// </summary>
        /// <param name="collectionName">Name of table collection.</param>
        /// <param name="key">Key of text in table.</param>
        /// <returns>Text if found, otherwise returns null or empty string.</returns>
        private string GetLocalizedText(string collectionName, string key)
        {
            string localizedString;
            if (!DaggerfallUnity.Instance.TextProvider.GetLocalizedString(collectionName, key, out localizedString))
                return null;

            return localizedString;
        }

        /// <summary>
        /// Tries to gets text value from localization in TextProvider.
        /// Will use current locale if available in collection.
        /// </summary>
        /// <param name="collectionName">Name of table collection.</param>
        /// <param name="key">Key of text in table.</param>
        /// <param name="localizedString">Result of lookup if found.</param>
        /// <returns>True if text found, otherwise false.</returns>
        private bool TryGetLocalizedText(string collectionName, string key, out string localizedString)
        {
            if (!DaggerfallUnity.Instance.TextProvider.GetLocalizedString(collectionName, key, out localizedString))
                return false;

            return true;
        }

        /// <summary>
        /// Gets an array of text where each line is considered an item in array.
        /// Entry will be read from table and split by newline characters into array.
        /// </summary>
        /// <param name="collectionName">Name of table collection.</param>
        /// <param name="key">Key of text in table.</param>
        /// <param name="exception">True to throw exception if text not found. False to just return null.</param>
        /// <returns>Text array if found, otherwise returns null or throws exception.</returns>
        private string[] GetLocalizedTextList(string collectionName, string key, bool exception = true)
        {
            string cacheKey = collectionName + key;
            string[] cachedList = null;
            if (cachedLocalizedTextLists.TryGetValue(cacheKey, out cachedList))
                return cachedList;

            string localizedString;
            if (!DaggerfallUnity.Instance.TextProvider.GetLocalizedString(collectionName, key, out localizedString))
            {
                if (exception)
                    throw new Exception(string.Format("{0} array text not found", cacheKey));
                else
                    return null;
            }

            cachedList = localizedString.TrimEnd(trimAtEnd).Split(newlineSequences, StringSplitOptions.None);
            cachedLocalizedTextLists.Add(cacheKey, cachedList);

            return cachedList;
        }

        /// <summary>
        /// Gets array of text from discrete items using a key array.
        /// </summary>
        /// <param name="collectionName">Name of table collection.</param>
        /// <param name="keyArray">Array of keys to resolve back to text in list.</param>
        /// <param name="exception">True to throw exception if text not found. False to just return null.</param>
        /// <returns>Text array if all keys found, otherwise returns null or throws exception.</returns>
        private string[] GetLocalizedTextListFromKeyArray(string collectionName, string[] keyArray, bool exception = true)
        {
            if (keyArray == null || keyArray.Length == 0)
            {
                if (exception)
                    throw new Exception("keyArray is null or empty");
                else
                    return null;
            }

            List<string> results = new List<string>();
            foreach (string key in keyArray)
            {
                string localizedString;
                if (!TryGetLocalizedText(collectionName, key, out localizedString))
                {
                    if (exception)
                        throw new Exception(string.Format("Text for key {0} not found", key));
                    else
                        return null;
                }
                results.Add(localizedString);
            }

            return results.ToArray();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Enumerate all available text databases.
        /// </summary>
        protected void EnumerateTextDatabases()
        {
            // Get all text files in target path
            Debug.Log("TextManager enumerating text databases.");
            string path = Path.Combine(Application.streamingAssetsPath, textFolderName);
            string[] files = Directory.GetFiles(path, "*.txt");

            // Attempt to read each file as a table with a text schema
            foreach (string file in files)
            {
                try
                {
                    // Create table from text file
                    Table table = new Table(File.ReadAllLines(file));

                    // Get database key from filename
                    string databaseName = Path.GetFileNameWithoutExtension(file);
                    if (HasDatabase(databaseName))
                        throw new Exception(string.Format("TextManager database name {0} already exists.", databaseName));

                    // Assign database to collection
                    textDatabases.Add(databaseName, table);
                    Debug.LogFormat("TextManager read text database table {0} with {1} rows", databaseName, table.RowCount);
                }
                catch (Exception ex)
                {
                    Debug.LogFormat("TextManager unable to parse text database table {0} with exception message {1}", file, ex.Message);
                    continue;
                }
            }
        }

        #endregion

        #region Singleton

        static TextManager instance = null;
        public static TextManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindSingleton(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "TextManager";
                        instance = go.AddComponent<TextManager>();
                    }
                }
                return instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return (instance != null);
            }
        }

        public static bool FindSingleton(out TextManager singletonOut)
        {
            singletonOut = GameObject.FindObjectOfType<TextManager>();
            if (singletonOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate TextManager GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        #endregion

        #region Console Commands

        private static class Locale_Print
        {
            public static readonly string name = "locale_print";
            public static readonly string description = "Output available locales.";
            public static readonly string usage = "locale_print";

            public static string Execute(params string[] args)
            {
                int index = 0;
                string output = string.Empty;
                var locales = LocalizationSettings.AvailableLocales.Locales;
                if (locales == null || locales.Count == 0)
                    return "No available locales.";

                foreach (var locale in locales)
                {
                    output += string.Format("{0}. {1}\n", index++, locale.name);
                }
                output += string.Format("Current locale: {0}", LocalizationSettings.SelectedLocale.name);

                return output;
            }
        }

        private static class Locale_Set
        {
            public static readonly string name = "locale_set";
            public static readonly string description = "Sets current locale. Use locale_print to list available locales.";
            public static readonly string usage = "locale_set <index>";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length < 1)
                    return string.Format("Usage: {0}", usage);

                int index;
                if (!int.TryParse(args[0], out index))
                    return usage;

                var locales = LocalizationSettings.AvailableLocales.Locales;
                if (locales == null || locales.Count == 0)
                    return "No available locales.";

                if (index < 0 || index > locales.Count - 1)
                    return "Locale index out of range.";

                // Set locale
                Locale locale = LocalizationSettings.AvailableLocales.Locales[index];
                LocalizationSettings.SelectedLocale = locale;
                return string.Format("Set locale to '{0}'", locale.name);
            }
        }

        private static class Locale_Debug
        {
            public static readonly string name = "locale_debug";
            public static readonly string description = "Enables verbose localization debug to player log.";
            public static readonly string usage = "locale_debug on|off";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length < 1)
                    return string.Format("Usage: {0}", usage);

                if (args[0] == "on")
                {
                    DaggerfallUnity.Instance.TextProvider.EnableLocalizedStringDebug(true);
                    return string.Format("Localization debug enabled.\nRuntimePath='{0}'", UnityEngine.AddressableAssets.Addressables.RuntimePath);
                }
                else if (args[0] == "off")
                {
                    DaggerfallUnity.Instance.TextProvider.EnableLocalizedStringDebug(false);
                    return "Localization debug disabled";
                }
                else
                {
                    return usage;
                }
            }
        }

        #endregion
    }
}
