// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using IniParser;
using IniParser.Model;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// Read/Write settings files.
    /// </summary>
    public static class ModSettingsReader
    {
        #region Fields

        /// <summary>
        /// Section containing information used by the modding system.
        /// </summary>
        public const string internalSection = "Internal";

        /// <summary>
        /// Key with version of settings file.
        /// </summary>
        public const string settingsVersionKey = "SettingsVersion";

        /// <summary>
        /// Delimiter between First and Second value of a tuple.
        /// </summary>
        public const string tupleDelimiterChar = "<,>";

        static FileIniDataParser parser = new FileIniDataParser();

        #endregion

        #region Load/Save Settings

        /// <summary>
        /// Check if a mod support settings. If configuration file
        /// is missing it will be recreated with default values.
        /// </summary>
        public static bool HasSettings(Mod mod)
        {
            // Get path
            string settingPath = SettingsPath(mod);

            // File on disk
            if (File.Exists(settingPath))
                return true;

            IniData defaultSettings;
            if (TryGetDefaultSettings(mod, out defaultSettings))
            {
                // Recreate file on disk using default values
                parser.WriteFile(settingPath, defaultSettings);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get user settings and default settings for mod.
        /// </summary>
        public static void GetSettings(Mod mod, out IniData settings, out IniData defaultSettings)
        {
            // Load default settings
            defaultSettings = GetDefaultSettings(mod);

            // Load serialized settings or recreate them
            string path = SettingsPath(mod);
            if (File.Exists(path))
            {
                settings = parser.ReadFile(path);

                try
                {
                    if (settings[internalSection][settingsVersionKey] != defaultSettings[internalSection][settingsVersionKey])
                    {
                        ResetSettings(mod, ref settings, defaultSettings);
                        Debug.LogFormat("Settings for {0} are incompatible with current version. " +
                            "New settings have been recreated with default values", mod.Title);
                    }
                }
                catch
                {
                    ResetSettings(mod, ref settings, defaultSettings);
                    Debug.LogErrorFormat("Failed to read settings for {0}. " +
                        "New settings have been recreated with default values", mod.Title);
                }
            }
            else
            {
                settings = null;
                ResetSettings(mod, ref settings, defaultSettings);
                Debug.LogFormat("Missing settings for {0}. " +
                    "New settings have been recreated with default values.", mod.Title);
            }
        }

        public static bool TryGetDefaultSettings(Mod mod, out IniData settings)
        {
            // Get default settings from ini file
            if (mod.AssetBundle.Contains(mod.Title + ".ini.txt"))
            {
                settings = GetIniDataFromTextAsset(mod.GetAsset<TextAsset>(mod.Title + ".ini.txt"));
                return true;
            }
            else if (mod.AssetBundle.Contains("modsettings.ini.txt"))
            {
                settings = GetIniDataFromTextAsset(mod.GetAsset<TextAsset>("modsettings.ini.txt"));
                return true;
            }

            // Eventually this will no longer be supported as file name can be changed by user.
            if (mod.AssetBundle.Contains(mod.FileName + ".ini.txt"))
            {
                Debug.LogWarningFormat("{0} is using an obsolete modsettings filename!", mod.Title);
                settings = GetIniDataFromTextAsset(mod.GetAsset<TextAsset>(mod.FileName + ".ini.txt"));
                return true;
            }

            // Get defaults from config file
            var config = GetConfig(mod);
            if (config)
            {
                settings = ParseConfigToIni(config);
                return true;
            }

            settings = null;
            return false;
        }

        /// <summary>
        /// Get default settings for mod.
        /// </summary>
        public static IniData GetDefaultSettings(Mod mod)
        {
            IniData defaultSettings;
            if (TryGetDefaultSettings(mod, out defaultSettings))
                return defaultSettings;

            Debug.LogErrorFormat("Failed to get default settings from {0}", mod.Title);
            return null;
        }

        /// <summary>
        /// Save settings to disk.
        /// </summary>
        public static void SaveSettings(Mod mod, IniData settings)
        {
            parser.WriteFile(SettingsPath(mod), settings);
        }

        /// <summary>
        /// Save default settings to disk and set them as current settings.
        /// </summary>
        public static void ResetSettings(Mod mod, ref IniData settings, IniData defaultSettings)
        {
            settings = new IniData(defaultSettings);
            parser.WriteFile(SettingsPath(mod), settings);
        }

        public static List<IniData> GetPresets (Mod mod)
        {
            List<IniData> presets = new List<IniData>();

            // Get presets from disk
            string[] presetsDirectories = Directory.GetFiles(mod.DirPath, mod.FileName + "preset" + "*.ini");
            foreach (string path in presetsDirectories)
            {
                presets.Add(parser.ReadFile(path));
            }

            // Get presets from mod (TextAsset)
            int index = 0;
            while (mod.AssetBundle.Contains("settingspreset" + index + ".ini.txt"))
            {
                TextAsset presetFile = mod.GetAsset<TextAsset>("settingspreset" + index + ".ini.txt");
                presets.Add(GetIniDataFromTextAsset(presetFile));
                index++;
            }

            // Get preset from mod (Config)
            var config = GetConfig(mod);
            if (config != null)
            {
                foreach (var presetName in config.presets)
                {
                    var presetConfig = mod.GetAsset<ModSettingsConfiguration>(presetName);
                    presets.Add(ParseConfigToIni(presetConfig));
                }
            }

            return presets;
        }

        public static ModSettingsConfiguration GetConfig(Mod mod)
        {
            if (mod.AssetBundle.Contains("modsettings.asset"))
                return mod.GetAsset<ModSettingsConfiguration>("modsettings.asset");

            return null;
        }

        #endregion

        #region Helper Methods

        public static IniData ParseConfigToIni(ModSettingsConfiguration config)
        {
            var iniData = new IniData();

            // Header
            var header = new SectionData(internalSection);
            header.Keys.AddKey(settingsVersionKey, config.version);

            if (config.isPreset)
            {
                header.Keys.AddKey("PresetName", config.presetSettings.name);
                header.Keys.AddKey("PresetAuthor", config.presetSettings.author);
                header.Keys.AddKey("Description", config.presetSettings.description);
            }

            iniData.Sections.Add(header);

            // Settings
            foreach (var section in config.sections)
            {
                var sectionData = new SectionData(section.name);

                foreach (var key in section.keys)
                {
                    KeyData keyData = new KeyData(key.name);

                    switch (key.type)
                    {
                        case ModSettingsKey.KeyType.Toggle:
                            keyData.Value = key.toggle.value.ToString();
                            break;

                        case ModSettingsKey.KeyType.MultipleChoice:
                            keyData.Value = key.multipleChoice.selected.ToString();
                            break;

                        case ModSettingsKey.KeyType.Slider:
                            keyData.Value = key.slider.value.ToString();
                            break;

                        case ModSettingsKey.KeyType.FloatSlider:
                            keyData.Value = key.floatSlider.value.ToString();
                            break;

                        case ModSettingsKey.KeyType.Tuple:
                            keyData.Value = key.tuple.first + tupleDelimiterChar + key.tuple.second;
                            break;

                        case ModSettingsKey.KeyType.FloatTuple:
                            keyData.Value = key.floatTuple.first + tupleDelimiterChar + key.floatTuple.second;
                            break;

                        case ModSettingsKey.KeyType.Text:
                            keyData.Value = key.text.text;
                            break;

                        case ModSettingsKey.KeyType.Color:
                            keyData.Value = key.color.HexColor;
                            break;
                    }

                    sectionData.Keys.AddKey(keyData);
                }

                if (section.name == internalSection)
                    iniData.Sections.GetSectionData(internalSection).Merge(sectionData);
                else
                    iniData.Sections.Add(sectionData);
            }

            return iniData;
        }

        public static bool IsHexColor(string stringColor)
        {
            int hexColor;
            return (stringColor.Length == 8 &&
                int.TryParse(stringColor, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out hexColor));
        }

        /// <summary>
        /// Convert settings string to Color
        /// (ex: 000000FF --> Color.black).
        /// </summary>
        /// <param name="colorStr">settingsData["SectionName"]["colorName"]</param>
        public static Color ColorFromString(string colorStr)
        {
            if (colorStr.Length != 8)
            {
                Debug.LogError("Failed to get color from " + colorStr +
                    ". Color must be in 32-bit RGBA format, e.g. black is 000000FF.");
                return Color.black;
            }

            // Convert string to Color32 (from SettingsManager.cs)
            byte r = byte.Parse(colorStr.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(colorStr.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(colorStr.Substring(4, 2), NumberStyles.HexNumber);
            byte a = byte.Parse(colorStr.Substring(6, 2), NumberStyles.HexNumber);

            return new Color32(r, g, b, a);
        }

        #endregion

        #region Private Methods

        private static string SettingsPath(Mod mod)
        {
            return Path.Combine(mod.DirPath, mod.FileName + ".ini");
        }

        private static IniData GetIniDataFromTextAsset (TextAsset textAsset)
        {
            MemoryStream stream = new MemoryStream(textAsset.bytes);
            StreamReader reader = new StreamReader(stream);
            IniData iniData = parser.ReadData(reader);
            reader.Close();
            return iniData;
        }

        #endregion
    }
}
