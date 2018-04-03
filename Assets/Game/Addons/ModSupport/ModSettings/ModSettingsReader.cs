// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using IniParser;
using IniParser.Model;
using FullSerializer;

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
            // Check file on disk
            if (File.Exists(SettingsPath(mod)))
                return true;

            // Recreate file on disk using default values
            ModSettingsData data;
            if (TryGetSettingsData(mod, out data))
            {
                ResetSettings(mod, data);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Read user and default mod settings.
        /// </summary>
        public static void GetSettings(Mod mod, out IniData settings, out ModSettingsData data)
        {
            // Load config
            if (!TryGetSettingsData(mod, out data))
                throw new ArgumentException("Mod has no associated settings.");

            // Load serialized settings or recreate them
            string path = SettingsPath(mod);
            if (File.Exists(path))
            {
                settings = parser.ReadFile(path);

                if (!string.IsNullOrEmpty(data.Version))
                {
                    var header = settings.Sections.GetSectionData(internalSection);
                    if (header == null || header.Keys[settingsVersionKey] != data.Version)
                    {
                        ResetSettings(mod, ref settings, data);
                        Debug.LogFormat("Settings for {0} are incompatible with current version. " +
                            "New settings have been recreated with default values", mod.Title);
                    }
                }
            }
            else
            {
                settings = null;
                ResetSettings(mod, ref settings, data);
                Debug.LogFormat("Missing settings for {0}. " +
                    "New settings have been recreated with default values.", mod.Title);
            }
        }

        /// <summary>
        /// Seek and load default settings from mod.
        /// </summary>
        public static bool TryGetSettingsData(Mod mod, out ModSettingsData data)
        {
            if (mod.AssetBundle.Contains("modsettings.json"))
            {
                data = new ModSettingsData();
                return TryDeserializeFromMod(mod, "modsettings.json", ref data);
            }

            // Legacy support
            if (mod.AssetBundle.Contains(mod.Title + ".ini.txt"))
            {
                Debug.LogFormat("{0} is using a legacy modsettings format.", mod.Title);
                data = LoadLegacySettingsFromMod(mod, mod.Title + ".ini.txt");
                return true;
            }
            else if (mod.AssetBundle.Contains("modsettings.ini.txt"))
            {
                Debug.LogFormat("{0} is using a legacy modsettings format.", mod.Title);
                data = LoadLegacySettingsFromMod(mod, "modsettings.ini.txt");
                return true;
            }
            else if (mod.AssetBundle.Contains(mod.FileName + ".ini.txt"))
            {
                // Note: this is unsafe because file name can be changed by user.
                Debug.LogWarningFormat("{0} is using an obsolete modsettings filename!", mod.Title);
                data = LoadLegacySettingsFromMod(mod, mod.FileName + ".ini.txt");
                return true;
            }

            data = null;
            return false;
        }

        /// <summary>
        /// Save settings to disk.
        /// </summary>
        public static void SaveSettings(Mod mod, IniData settings)
        {
            parser.WriteFile(SettingsPath(mod), settings);
        }

        /// <summary>
        /// Save default settings to disk.
        /// </summary>
        public static void ResetSettings(Mod mod, ModSettingsData data)
        {
            parser.WriteFile(SettingsPath(mod), ParseSettingsToIni(data));
        }

        /// <summary>
        /// Save default settings to disk and set them as current settings.
        /// </summary>
        public static void ResetSettings(Mod mod, ref IniData settings, ModSettingsData data)
        {
            parser.WriteFile(SettingsPath(mod), settings = ParseSettingsToIni(data));
        }

        /// <summary>
        /// Import presets from mod and local presets from disk.
        /// </summary>
        public static List<Preset> GetPresets(Mod mod)
        {
            var presets = new List<Preset>();

            // Get presets from mod
            if (mod.AssetBundle.Contains("modpresets.json"))
                TryDeserializeFromMod(mod, "modpresets.json", ref presets);

            // Get presets from disk
            foreach (string path in Directory.GetFiles(mod.DirPath, mod.FileName + "preset*.ini"))
            {
                var preset = ParseIniToPreset(parser.ReadFile(path));
                preset.DiskPath = path;
                presets.Add(preset);
            }

            return presets;
        }

        /// <summary>
        /// Copy values from user data to preset and save to disk as IniFile.
        /// </summary>
        public static void CreatePreset(Mod mod, IniData data, Preset preset)
        {
            // Set preset instance from current values
            preset.Values.Clear();
            foreach (var sectionData in data.Sections)
            {
                var pSection = new Dictionary<string, string>();
                foreach (var keyData in sectionData.Keys)
                    pSection.Add(keyData.KeyName, keyData.Value);
                preset.Values.Add(sectionData.SectionName, pSection);
            }

            // Save to disk as Ini file
            parser.WriteFile(preset.GetSafePath(mod), ParsePresetToIni(preset));  
        }

        #endregion

        #region Helper Methods

        public static IniData ParseSettingsToIni(ModSettingsData data)
        {
            var iniData = new IniData();

            // Header
            var header = new SectionData(internalSection);
            header.Keys.AddKey(settingsVersionKey, data.Version);
            iniData.Sections.Add(header);

            // Settings
            foreach (var section in data.Sections)
            {
                var sectionData = new SectionData(section.Name);

                foreach (var key in section.Keys)
                    sectionData.Keys.AddKey(key.Name, key.TextValue);

                if (section.Name == internalSection)
                    iniData.Sections.GetSectionData(internalSection).Merge(sectionData);
                else
                    iniData.Sections.Add(sectionData);
            }

            return iniData;
        }

        public static void ParseIniToSettings(ModSettingsData data, IniData iniData)
        {
            data.Sections.Clear();
#if UNITY_EDITOR
            data.Presets.Clear();
#endif
            foreach (SectionData section in iniData.Sections)
            {
                var dataSection = new Section();
                dataSection.Name = section.SectionName;

                foreach (KeyData key in section.Keys)
                {
                    Key dataKey;
                    if (key.Value == "True" || key.Value == "False")
                    {
                        dataKey = new ToggleKey();
                    }
                    else if (key.Value.Contains(tupleDelimiterChar))
                    {
                        dataKey = new TupleFloatKey();
                    }
                    else if (IsHexColor(key.Value))
                    {
                        dataKey = new ColorKey();
                    }
                    else
                    {
                        dataKey = new TextKey();
                    }

                    if (section.SectionName == internalSection && key.KeyName == settingsVersionKey)
                    {
                        data.Version = key.Value;
                        continue;
                    }

                    dataKey.Name = key.KeyName;
                    dataKey.TextValue = key.Value;
                    dataSection.Keys.Add(dataKey);
                }

                if (dataSection.Keys.Count > 0)
                    data.Sections.Add(dataSection);
            }
        }

        public static IniData ParsePresetToIni(Preset preset)
        {
            var iniData = new IniData();

            // Header
            var header = new SectionData(internalSection);
            header.Keys.AddKey(settingsVersionKey, preset.SettingsVersion);
            header.Keys.AddKey("PresetName", preset.Title);
            header.Keys.AddKey("PresetAuthor", preset.Author);
            header.Keys.AddKey("Description", preset.Description);
            iniData.Sections.Add(header);

            // Settings
            foreach (var section in preset.Values)
            {
                var sectionData = new SectionData(section.Key);
                foreach (var key in section.Value)
                    sectionData.Keys.AddKey(key.Key, key.Value);
                iniData.Sections.Add(sectionData);
            }

            return iniData;
        }

        private static Preset ParseIniToPreset(IniData presetData)
        {
            var preset = new Preset();
            foreach (var sectionData in presetData.Sections)
            {
                if (sectionData.SectionName == internalSection)
                {
                    // Header
                    preset.Title = sectionData.Keys["PresetName"];
                    preset.Author = sectionData.Keys["PresetAuthor"];
                    preset.Description = sectionData.Keys["Description"];
                    preset.SettingsVersion = sectionData.Keys["SettingsVersion"];
                }
                else
                {
                    // Settings
                    var section = new Dictionary<string, string>();
                    foreach (var keyData in sectionData.Keys)
                        section.Add(keyData.KeyName, keyData.Value);
                    preset.Values.Add(sectionData.SectionName, section);
                }
            }
            return preset;
        }

        /// <summary>
        /// Make a displayable name for a key or section.
        /// </summary>
        public static string FormattedName(string name)
        {
            return string.Concat((name.First().ToString().ToUpper() + name.Substring(1))
                .Select(x => Char.IsUpper(x) ? " " + x : x.ToString()).ToArray()).TrimStart(' ');
        }

        #endregion

        #region Private Methods

        private static string SettingsPath(Mod mod)
        {
            return Path.Combine(mod.DirPath, mod.FileName + ".ini");
        }

        private static bool TryDeserializeFromMod<T>(Mod mod, string assetName, ref T data)
        {
            var textAsset = mod.GetAsset<TextAsset>(assetName);
            fsResult fsResult = ModManager._serializer.TryDeserialize(fsJsonParser.Parse(textAsset.text), ref data);
            if (fsResult.Failed)
                Debug.LogErrorFormat("{0}: Failed to import {1}:\n{2}", mod.Title, assetName, fsResult.FormattedMessages);
            return fsResult.Succeeded;
        }

        private static ModSettingsData LoadLegacySettingsFromMod(Mod mod, string assetName)
        {
            var data = new ModSettingsData();
            var textAsset = mod.GetAsset<TextAsset>(assetName);
            using (var stream = new MemoryStream(textAsset.bytes))
            using (var reader = new StreamReader(stream))
                ParseIniToSettings(data, parser.ReadData(reader));
            return data;
        }

        private static bool IsHexColor(string stringColor)
        {
            int hexColor;
            return (stringColor.Length == 8 &&
                int.TryParse(stringColor, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out hexColor));
        }

        #endregion
    }
}
