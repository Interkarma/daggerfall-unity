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

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// Read/Write settings files.
    /// </summary>
    public static class ModSettingsReader
    {
        static FileIniDataParser parser = new FileIniDataParser();
        public const string internalSection = "Internal";
        public const string settingsVersionKey = "SettingsVersion";

        /// <summary>
        /// Check if a mod support settings. If configuration file
        /// is missing it will be recreated with default values.
        /// </summary>
        public static bool HasSettings(Mod mod)
        {
            // Get path
            string settingPath = Path.Combine(ModManager.Instance.ModDirectory, mod.Name + ".ini");

            // File on disk
            if (File.Exists(settingPath))
                return true;

            if (mod.AssetBundle.Contains(mod.Name + ".ini.txt"))
            {
                // Recreate file on disk using default values
                IniData defaultSettings = GetDefaultSettings(mod);
                parser.WriteFile(settingPath, defaultSettings);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get settings for mod. (optionally) if configuration file
        /// is missing it will be recreated with default values.
        /// </summary>
        /// <returns></returns>
        public static IniData GetSettings(Mod mod, bool getDefaultsAsFallback = true)
        {
            string path = Path.Combine(ModManager.Instance.ModDirectory, mod.Name + ".ini");

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
                Debug.Log(mod.Title + ": A new " + mod.Name + ".ini has been recreated with default settings");
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
            var iniFile = mod.GetAsset<TextAsset>(mod.Name + ".ini.txt");
            if (iniFile == null)
            {
                Debug.LogError("Failed to get default settings from " + mod);
                return null;
            }

            // Read ini as stream
            MemoryStream stream = new MemoryStream(iniFile.bytes);
            StreamReader reader = new StreamReader(stream);
            IniData defaultSettings = parser.ReadData(reader);
            reader.Close();
            return defaultSettings;
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
                    parser.WriteFile(Path.Combine(ModManager.Instance.ModDirectory, mod.Name + ".ini"), defaultSettings);
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
    }
}
