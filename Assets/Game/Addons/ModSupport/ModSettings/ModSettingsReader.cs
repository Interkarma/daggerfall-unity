// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using IniParser;
using IniParser.Model;
using UnityEngine;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// Read/Write settings files.
    /// </summary>
    public static class ModSettingsReader
    {
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

        /// <summary>
        /// Check if a mod support settings. If configuration file
        /// is missing it will be recreated with default values.
        /// </summary>
        public static bool HasSettings(Mod mod)
        {
            // Get path
            string settingPath = Path.Combine(mod.DirPath, mod.FileName + ".ini");

            // File on disk
            if (File.Exists(settingPath))
                return true;

            if (mod.AssetBundle.Contains("modsettings.ini.txt"))
            {
                // Recreate file on disk using default values
                IniData defaultSettings = GetDefaultSettings(mod);
                parser.WriteFile(settingPath, defaultSettings);
                return true;
            }
            else if (mod.AssetBundle.Contains(mod.Title + ".ini.txt"))
            {
                // Recreate file on disk using default values
                IniData defaultSettings = GetDefaultSettings(mod);
                parser.WriteFile(settingPath, defaultSettings);
                return true;
            }
            else if(mod.AssetBundle.Contains(mod.FileName + ".ini.txt"))
            {
                IniData defaultSettings = GetDefaultSettings(mod);
                parser.WriteFile(settingPath, defaultSettings);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get settings for mod. (optionally) if configuration file
        /// is missing it will be recreated with default values.
        /// </summary>
        /// <returns></returns>
        public static IniData GetSettings(Mod mod, bool getDefaultsAsFallback = true)
        {
            string path = Path.Combine(mod.DirPath, mod.FileName + ".ini");

            if (File.Exists(path))
                return parser.ReadFile(path);

            Debug.LogError(mod.Title + ": failed to read " + path);
            if (getDefaultsAsFallback == false)
                return null;

            IniData defaultSettings = GetDefaultSettings(mod);
            if (defaultSettings != null)
            {
                // Create settings file
                parser.WriteFile(path, defaultSettings);
                Debug.Log(mod.Title + ": A new " + mod.FileName + ".ini has been recreated with default settings");
                return defaultSettings;
            }
            else
            {
                Debug.LogError(mod.Title + ": Failed to get default settings");
                return null;
            }
        }

        /// <summary>
        /// Get default settings for mod.
        /// </summary>
        public static IniData GetDefaultSettings(Mod mod)
        {
            // Get modName.ini.txt from Mod
            TextAsset iniFile = null;
            if (mod.AssetBundle.Contains(mod.Title + ".ini.txt"))
                iniFile = mod.GetAsset<TextAsset>(mod.Title + ".ini.txt");
            else if (mod.AssetBundle.Contains("modsettings.ini.txt"))
                iniFile = mod.GetAsset<TextAsset>("modsettings.ini.txt");
            else if (mod.AssetBundle.Contains(mod.FileName + ".ini.txt"))
                iniFile = mod.GetAsset<TextAsset>(mod.FileName + ".ini.txt"); //should not be used, as file name can be changed by user
            else
            {
                Debug.LogError("Failed to get default settings from " + mod.Title);
                return null;
            }

            return GetIniDataFromTextAsset(iniFile);
        }

        /// <summary>
        /// Check compatibility between user settings and current version of mod.
        /// </summary>
        public static void UpdateSettings(ref IniData userSettings, IniData defaultSettings, Mod mod)
        {
            try
            {
                if (userSettings[internalSection][settingsVersionKey] != defaultSettings[internalSection][settingsVersionKey])
                {
                    parser.WriteFile(Path.Combine(mod.DirPath, mod.FileName + ".ini"), defaultSettings);
                    userSettings = defaultSettings;
                    Debug.Log("Outdated settings for " + mod.Title +
                        " have been found and replaced with default values from new version.");
                }
            }
            catch
            {
                Debug.LogError("Failed to read internal settings for " + mod.Title + ".");
            }
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

            // Get presets from mod
            int index = 0;
            while (mod.AssetBundle.Contains("settingspreset" + index + ".ini.txt"))
            {
                TextAsset presetFile = mod.GetAsset<TextAsset>("settingspreset" + index + ".ini.txt");
                presets.Add(GetIniDataFromTextAsset(presetFile));
                index++;
            }

            return presets;
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

        private static IniData GetIniDataFromTextAsset (TextAsset textAsset)
        {
            MemoryStream stream = new MemoryStream(textAsset.bytes);
            StreamReader reader = new StreamReader(stream);
            IniData iniData = parser.ReadData(reader);
            reader.Close();
            return iniData;
        }
    }
}
