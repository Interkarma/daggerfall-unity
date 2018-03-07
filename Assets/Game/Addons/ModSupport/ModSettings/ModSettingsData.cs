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
using System.Linq;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// Holder for mod settings values and control helpers.
    /// </summary>
    public class ModSettingsData
    {
        #region Fields

#if UNITY_EDITOR
        Preset defaultValues = new Preset();
        List<Preset> presets = new List<Preset>();
#endif

        public string Version;
        public SectionCollection Sections = new SectionCollection();

        #endregion

        #region Properties

        /// <summary>
        /// Get all sections shown on GUI.
        /// </summary>
        public IEnumerable<Section> VisibleSections
        {
            get { return Sections.Where(x => x.Name != ModSettingsReader.internalSection); }
        }

        /// <summary>
        /// Get section hidden from GUI. Can be null.
        /// </summary>
        public Section HiddenSection
        {
            get { return Sections.FirstOrDefault(x => x.Name == ModSettingsReader.internalSection); }
        }

        public Section this[int i]
        {
            get { return Sections[i]; }
        }

        public Section this[string section]
        {
            get { return Sections[section]; }
        }

#if UNITY_EDITOR
        public List<Preset> Presets
        {
            get { return presets; }
        }
#endif

        #endregion

        #region Public Methods

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

#if UNITY_EDITOR

        /// <summary>
        /// Change type of key preserving name and description.
        /// </summary>
        public void SetType(int sectionIndex, int keyIndex, ref Key key, KeyType keyType)
        {
            if (key.KeyType == keyType)
                return;

            string name = key.Name;
            string description = key.Description;
            switch (keyType)
            {
                case KeyType.Toggle:
                default:
                    key = new ToggleKey();
                    break;
                case KeyType.MultipleChoice:
                    key = new MultipleChoiceKey();
                    break;
                case KeyType.SliderInt:
                    key = new SliderIntKey();
                    break;
                case KeyType.SliderFloat:
                    key = new SliderFloatKey();
                    break;
                case KeyType.TupleInt:
                    key = new TupleIntKey();
                    break;
                case KeyType.TupleFloat:
                    key = new TupleFloatKey();
                    break;
                case KeyType.Text:
                    key = new TextKey();
                    break;
                case KeyType.Color:
                    key = new ColorKey();
                    break;
            }

            key.Name = name;
            key.Description = description;
            var section = Sections[sectionIndex];
            section.Keys[keyIndex] = key;
            foreach (var preset in presets)
                preset[section.Name, key.Name] = false;
        }

        public void SaveDefaults()
        {
            defaultValues.Values = Sections.ToDictionary(sk => sk.Name,
                sv => sv.Keys.ToDictionary(kn => kn.Name, kv => kv.TextValue));
        }

        public void RestoreDefaults()
        {
            ApplyPreset(defaultValues);
        }

        public void ApplyPreset(Preset preset)
        {
            EnumeratePreset(preset, (key, presetKeys) => key.TextValue = presetKeys[key.Name]);
        }

        public void UpdatePreset(Preset preset)
        {
            EnumeratePreset(preset, (key, presetKeys) => presetKeys[key.Name] = key.TextValue);
            preset.SettingsVersion = Version;
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

        #endregion

        #region Private Methods

        private void EnumeratePreset(Preset preset, Action<Key, Dictionary<string, string>> action)
        {
            if (preset.Values == null)
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
    }
}
