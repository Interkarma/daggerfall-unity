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
using System.IO;
using System.Linq;
using UnityEngine;
using FullSerializer;
using IniParser.Model;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// Holder for mod settings.
    /// </summary>
    public class ModSettingsData
    {
        #region Fields

        const string settingsFileName = "modsettings.json";
        const string presetsFileName = "modpresets.json";

        readonly Mod mod;

        SettingsValues defaultValues;
        List<Preset> presets;

        #endregion

        #region Properties

        string SettingsPath
        {
            get { return Path.Combine(mod.DirPath, string.Format("{0}.json", mod.FileName)); }
        }

        string LocalPresetsPath
        {
            get { return Path.Combine(mod.DirPath, string.Format("{0}_presets.json", mod.FileName)); }
        }

        string[] ImportedPresetsPaths
        {
            get { return Directory.GetFiles(mod.DirPath, string.Format("{0}_presets_*.json", mod.FileName)); }
        }

        /// <summary>
        /// Version of mod settings.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// All Sections in this collection.
        /// </summary>
        public SectionCollection Sections { get; set; }

        public Section this[int index]
        {
            get { return Sections[index]; }
        }

        public Section this[string sectionName]
        {
            get { return Sections[sectionName]; }
        }

        public List<Preset> Presets
        {
            get { return presets ?? (presets = new List<Preset>()); }
        }

        [fsIgnore]
        public bool HasLoadedPresets { get; private set; }

        #endregion

        #region Constructors

        private ModSettingsData()
        {
            Sections = new SectionCollection();
        }

        private ModSettingsData(Mod mod)
            :this()
        {
            this.mod = mod;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Save a copy of current values which can be restored later.
        /// </summary>
        public void SaveDefaults()
        {
            if (defaultValues == null)
                defaultValues = new SettingsValues();

            FillPreset(defaultValues, true);
        }

        /// <summary>
        /// Restore values previously set as defaults.
        /// </summary>
        public void RestoreDefaults()
        {
            ApplyPreset(defaultValues);
        }

        /// <summary>
        /// Deserialize local values from disk. Be sure to save defaults before if needed.
        /// </summary>
        public void LoadLocalValues()
        {
            var settings = new SettingsValues();
            string path = SettingsPath;
            if (TryDeserialize(path, ref settings) && IsCompatible(settings))
            {
                // Apply local values
                ApplyPreset(settings);
            }
            else
            {
                // Save to disk default values
                Save();
                Debug.LogFormat("Settings for {0} are missing or incompatible with current version. " +
                        "New settings have been recreated with default values", mod.Title);
            }
        }

        /// <summary>
        /// Import presets from mod and local presets from disk.
        /// </summary>
        public void LoadPresets()
        {
            if (Presets.Count > 0)
                Presets.Clear();

            // Get presets from mod
            if (mod.AssetBundle.Contains(presetsFileName))
            {
                List<Preset> modPresets = new List<Preset>();
                if (TryDeserialize(mod, presetsFileName, ref modPresets))
                    Presets.AddRange(modPresets);
            }

            // Local presets (managed from gui)
            string localPresetsPath = LocalPresetsPath;
            if (File.Exists(localPresetsPath))
            {
                List<Preset> localPresets = new List<Preset>();
                if (TryDeserialize(localPresetsPath, ref localPresets))
                {
                    foreach (var preset in localPresets)
                        preset.IsLocal = true;
                    Presets.AddRange(localPresets);
                }
            }

            // Other imported presets (readonly)
            foreach (string path in ImportedPresetsPaths)
            {
                List<Preset> importedPresets = new List<Preset>();
                if (TryDeserialize(path, ref importedPresets))
                    Presets.AddRange(importedPresets);
            }

            HasLoadedPresets = true;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Import presets from path.
        /// </summary>
        public void LoadPresets(string path, bool merge = false)
        {
            if (!merge && Presets.Count > 0)
                Presets.Clear();

            List<Preset> presetData = new List<Preset>();
            if (TryDeserialize(path, ref presetData))
                Presets.AddRange(presetData);
        }

#endif

        /// <summary>
        /// True if preset has the same version of this instance.
        /// Do not rely on this for anything else.
        /// </summary>
        public bool IsCompatible(IPreset preset)
        {
            return string.IsNullOrEmpty(Version) || Version == preset.SettingsVersion;
        }

        /// <summary>
        /// Override current values with a preset.
        /// </summary>
        public void ApplyPreset(IPreset preset)
        {
            EnumeratePreset(preset, (key, presetKeys) => key.TextValue = presetKeys[key.Name]);
        }

        /// <summary>
        /// Save preset settings from current values.
        /// </summary>
        public void FillPreset(IPreset preset, bool addNewKeys)
        {
            preset.SettingsVersion = Version;

            if (addNewKeys)
                preset.Values = Sections.ToDictionary(sk => sk.Name, sv => sv.Keys.ToDictionary(kn => kn.Name, kv => kv.TextValue));
            else
                EnumeratePreset(preset, (key, presetKeys) => presetKeys[key.Name] = key.TextValue);
        }

        /// <summary>
        /// Save local settings to disk.
        /// </summary>
        public void Save()
        {
            var values = new SettingsValues();
            FillPreset(values, true);
            Serialize(SettingsPath, values);
        }

#if UNITY_EDITOR

        /// <summary>
        /// Save mod settings to disk.
        /// </summary>
        public void Save(string path)
        {
            Serialize(path, this);
        }

#endif

        /// <summary>
        /// Write all local presets to disk.
        /// </summary>
        public void SavePresets()
        {
            string localPresetsPath = LocalPresetsPath;
            Preset[] localPresets = Presets.Where(x => x.IsLocal).ToArray();

            if (localPresets.Length > 0)
                Serialize(localPresetsPath, localPresets);
            else if (File.Exists(localPresetsPath))
                File.Delete(localPresetsPath);
        }

#if UNITY_EDITOR

        /// <summary>
        /// Save mod presets to disk.
        /// </summary>
        public void SavePresets(string path)
        {
            if (Presets.Count > 0)
                Serialize(path, Presets);
            else if (File.Exists(path))
                File.Delete(path);
        }

#endif

        /// <summary>
        /// Seeks a key with corresponding name and type.
        /// </summary>
        public bool TryGetKey<T>(string sectionName, string keyName, out T key) where T : Key
        {
            Key baseKey;
            Section section;

            if (Sections.TryGetValue(sectionName, out section) && section.Keys.TryGetValue(keyName, out baseKey))
                return (key = baseKey as T) != null;

            key = null;
            return false;
        }

        /// <summary>
        /// Gets a value with corresponding type from key name.
        /// </summary>
        public T GetValue<T>(string sectionName, string keyName)
        {
            Key<T> key;
            if (TryGetKey(sectionName, keyName, out key))
                return key.Value;

            throw new KeyNotFoundException(string.Format("The key <{0}>({1},{2}) was not present in {3} settings.", typeof(T), sectionName, keyName, mod.Title));
        }

#if UNITY_EDITOR

        /// <summary>
        /// Change type of key preserving name and description.
        /// </summary>
        public void SetType(int sectionIndex, int keyIndex, ref Key key, KeyType keyType)
        {
            if (key.KeyType == keyType)
                return;

            // Transfer name and description to a new key with given type.
            string name = key.Name;
            string description = key.Description;
            key = Key.Make(keyType);
            key.Name = name;
            key.Description = description;

            // Update key in settings data
            var section = Sections[sectionIndex];
            section.Keys[keyIndex] = key;
            foreach (var preset in presets)
                preset[section.Name, key.Name] = false;
        }

        /// <summary>
        /// Allow names to be synced with presets.
        /// </summary>
        /// <remarks>
        /// Keys are indexed with their name to support addition/removal.
        /// When a key is renamed, change must be applied on all presets to preserve values.
        /// </remarks>
        public void SyncPresets()
        {
            foreach (var section in Sections)
            {
                section.SyncSectionCallback = SyncSection;
                section.SyncKeysCallback = SyncKey;
            }
        }

#endif

        /// <summary>
        /// Try to parse a legacy ini file.
        /// </summary>
        public void ImportLegacyIni(IniData iniData)
        {
            Sections.Clear();
            Presets.Clear();

            foreach (SectionData section in iniData.Sections)
            {
                var dataSection = new Section();
                dataSection.Name = section.SectionName;

                foreach (KeyData key in section.Keys)
                {
                    Key dataKey;
                    if (key.Value == "True" || key.Value == "False")
                        dataKey = new ToggleKey();
                    else
                        dataKey = new TextKey();

                    if (section.SectionName == "Internal" && key.KeyName == "SettingsVersion")
                    {
                        Version = key.Value;
                        continue;
                    }

                    dataKey.Name = key.KeyName;
                    dataKey.TextValue = key.Value;
                    dataSection.Keys.Add(dataKey);
                }

                if (dataSection.Keys.Count > 0)
                    Sections.Add(dataSection);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Check if a mod has settings.
        /// </summary>       
        public static bool HasSettings(Mod mod)
        {
            return mod.AssetBundle.Contains(settingsFileName);
        }

        /// <summary>
        /// Import settings data from a mod.
        /// </summary>
        public static ModSettingsData Make(Mod mod)
        {
            ModSettingsData instance = new ModSettingsData(mod);
            if (HasSettings(mod) && TryDeserialize(mod, settingsFileName, ref instance))
                return instance;

            throw new ArgumentException(string.Format("Failed to load settings for mod {0}.", mod.Title));
        }

#if UNITY_EDITOR

        /// <summary>
        /// Import settings data from mod resources folder.
        /// </summary>
        public static ModSettingsData Make(string path)
        {
            ModSettingsData instance = new ModSettingsData();
            if (TryDeserialize(path, ref instance))
                return instance;

            return new ModSettingsData();
        }

#endif

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

        private void EnumeratePreset(IPreset preset, Action<Key, Dictionary<string, string>> action)
        {
            if (preset == null || preset.Values == null)
                return;

            foreach (var section in Sections)
            {
                Dictionary<string, string> presetSection;
                if (preset.Values.TryGetValue(section.Name, out presetSection))
                    foreach (var key in section.Keys.Where(x => presetSection.ContainsKey(x.Name)))
                        action(key, presetSection);
            }
        }

#if UNITY_EDITOR

        // Propagate section name change to presets
        private void SyncSection(string oldName, string name)
        {
            Sections.ChangeKey(Sections[oldName], name);
            foreach (var preset in Presets)
            {
                Dictionary<string, string> section;
                if (preset.Values.TryGetValue(oldName, out section))
                {
                    preset.Values.Remove(oldName);
                    preset.Values.Add(name, section);
                }
            }
        }

        // Propagate key name change to presets
        private void SyncKey(string sectionName, string oldName, string name)
        {
            foreach (var preset in Presets)
            {
                Dictionary<string, string> section;
                if (preset.Values.TryGetValue(sectionName, out section))
                {
                    string key;
                    if (section.TryGetValue(oldName, out key))
                    {
                        section.Remove(oldName);
                        section.Add(name, key);
                    }
                }
            }
        }

#endif

        #endregion

        #region Static Methods

        /// <summary>
        /// Deserialize a TextAsset from a mod.
        /// </summary>
        private static bool TryDeserialize<T>(Mod mod, string assetName, ref T instance)
        {
            var textAsset = mod.GetAsset<TextAsset>(assetName);
            fsResult fsResult = ModManager._serializer.TryDeserialize(fsJsonParser.Parse(textAsset.text), ref instance);
            if (fsResult.Failed)
                Debug.LogErrorFormat("{0}: Failed to import {1}:\n{2}", mod.Title, assetName, fsResult.FormattedMessages);
            return fsResult.Succeeded;
        }

        /// <summary>
        /// Deserialize a file on disk.
        /// </summary>
        private static bool TryDeserialize<T>(string path, ref T instance)
        {
            if (!File.Exists(path))
                return false;

            string input = File.ReadAllText(path);
            fsResult fsResult = ModManager._serializer.TryDeserialize(fsJsonParser.Parse(input), ref instance);
            if (fsResult.Failed)
                Debug.LogErrorFormat("Failed to import {0}:\n{1}", path, fsResult.FormattedMessages);
            return fsResult.Succeeded;
        }

        /// <summary>
        /// Derialize to a file on disk.
        /// </summary>
        private static void Serialize<T>(string path, T instance)
        {
            fsData fsData;
            fsResult fsResult = ModManager._serializer.TrySerialize(instance, out fsData);
            if (fsResult.Succeeded)
                File.WriteAllText(path, fsJsonPrinter.PrettyJson(fsData));
            else
                Debug.LogErrorFormat("Failed to write {0}:\n{1}", path, fsResult.FormattedMessages);
        }

        #endregion
    }
}
