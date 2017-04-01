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
using UnityEngine;
using IniParser;
using IniParser.Model;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// Helper class for mods.
    /// </summary>
    public static class ReadModSettings
    {
        /// <summary>
        /// Get settings for mod. If configuration file is
        /// missing it will be recreated with default values.
        /// </summary>
        /// <returns></returns>
        public static IniData GetSettings(Mod mod)
        {
            var parser = new FileIniDataParser();
            string path = Path.Combine(ModManager.Instance.ModDirectory, mod.Name + ".ini");

            if (File.Exists(path))
                return parser.ReadFile(path);

            Debug.LogError(mod.Title + ": failed to read " + path);
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
        /// Convert settings string to Color
        /// (ex: 000000FF --> Color.black).
        /// </summary>
        /// <param name="colorStr">settingsData["SectionName"]["colorName"]</param>
        public static Color ColorFromString (string colorStr)
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
            var parser = new FileIniDataParser();
            MemoryStream stream = new MemoryStream(iniFile.bytes);
            StreamReader reader = new StreamReader(stream);
            IniData defaultSettings = parser.ReadData(reader);
            reader.Close();
            return defaultSettings;
        }
    }
}
