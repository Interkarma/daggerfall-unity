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
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using IniParser;
using KeyType = DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings.ModSettingsKey.KeyType;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    [CreateAssetMenu(fileName = "modsettings", menuName = "Mods/Settings", order = 0)]
    [HelpURL("http://www.dfworkshop.net/projects/daggerfall-unity/modding/")]
    public class ModSettingsConfiguration : ScriptableObject
    {
        #region Fields

        [Tooltip("Version of settings config file. This is not the version of mod or preset.")]
        public string version = "1.0";

        [Tooltip("Is a preset or the main config?")]
        public bool isPreset = false;

        [Tooltip("Settings for this preset")]
        public PresetSettings presetSettings;

        [Tooltip("Presets for this mod")]
        public string[] presets;

        public Section[] sections = new Section[1];

        #endregion

        #region Properties

#if UNITY_EDITOR

        static string JsonPath
        {
            get { return Path.Combine(Path.Combine(Application.dataPath, "Untracked"), "modsettings.json"); }
        }

        static string IniPath
        {
            get { return Path.Combine(Path.Combine(Application.dataPath, "Untracked"), "modsettings.ini"); }
        }

#endif

        /// <summary>
        /// Get all sections shown on GUI.
        /// </summary>
        public IEnumerable<Section> VisibleSections
        {
            get { return sections.Where(x => x.name != ModSettingsReader.internalSection); }
        }

        /// <summary>
        /// Get section hidden from GUI. Can be null.
        /// </summary>
        public Section HiddenSection
        {
            get { return sections.FirstOrDefault(x => x.name == ModSettingsReader.internalSection); }
        }

        /// <summary>
        /// Get a section with the given name.
        /// </summary>
        public Section this[string sectionName]
        {
            get { return sections.FirstOrDefault(x => x.name == sectionName); }
        }

        #endregion

        #region Public Methods

        public bool Key(string sectionName, string keyName, out ModSettingsKey keyOut)
        {
            var section = this[sectionName];
            if (section != null)
            {
                var key = section[keyName];
                if (key != null)
                {
                    keyOut = key;
                    return true;
                }
            }

            keyOut = null;
            return false;
        }

        public bool Key(string sectionName, string keyName, KeyType keyType, out ModSettingsKey keyOut)
        {
            return Key(sectionName, keyName, out keyOut) && keyOut.type == keyType;
        }

        #endregion

        #region Editor Methods

#if UNITY_EDITOR

        /// <summary>
        /// Sync version and UI controls/checks from parent to this preset.
        /// A preset can have even only a portion of keys (typically a section).
        /// </summary>
        /// <param name="parent">Main settings.</param>
        /// <param name="addNewKeys">Add missing keys or sync only found ones?</param>
        public void Sync(ModSettingsConfiguration parent, bool addNewKeys)
        {
            if (!parent)
            {
                Debug.LogError("Parent not found");
                return;
            }

            version = parent.version;

            var childSections = new List<Section>();
            foreach (var parentSection in parent.sections)
            {
                Section section = this[parentSection.name];
                if (section == null)
                {
                    if (!addNewKeys)
                        continue;

                    section = new Section();
                    section.name = parentSection.name;
                    section.keys = new ModSettingsKey[0];
                }

                var childKeys = new List<ModSettingsKey>();
                foreach (var parentKey in parentSection.keys)
                {
                    ModSettingsKey key = section[parentKey.name];
                    if (key == null)
                    {
                        if (!addNewKeys)
                            continue;

                        key = new ModSettingsKey();
                        key.name = parentKey.name;
                    }

                    key.description = parentKey.description;
                    key.type = parentKey.type;
                    switch (parentKey.type)
                    {
                        case KeyType.MultipleChoice:
                            if (key.multipleChoice == null)
                                key.multipleChoice = new ModSettingsKey.MultipleChoice();
                            key.multipleChoice.choices = parentKey.multipleChoice.choices;
                            break;

                        case KeyType.Slider:
                            if (key.slider == null)
                                key.slider = new ModSettingsKey.Slider();
                            key.slider.max = parentKey.slider.max;
                            key.slider.min = parentKey.slider.min;
                            break;

                        case KeyType.FloatSlider:
                            if (key.floatSlider == null)
                                key.floatSlider = new ModSettingsKey.FloatSlider();
                            key.floatSlider.max = parentKey.floatSlider.max;
                            key.floatSlider.min = parentKey.floatSlider.min;
                            break;
                    }

                    childKeys.Add(key);
                }

                section.keys = childKeys.ToArray();

                childSections.Add(section);
            }

            sections = childSections.ToArray();

            // Add to list of presets
            string presetPath = UnityEditor.AssetDatabase.GetAssetPath(this.GetInstanceID());
            string presetName = Path.GetFileNameWithoutExtension(presetPath);
            if (!parent.presets.Contains(presetName))
            {
                var presets = new List<string>(parent.presets);
                presets.Add(presetName);
                parent.presets = presets.ToArray();
            }
        }
		
        public void Import()
        {
            if (!Deserialize(this))
                Debug.LogError("Failed to import mod settings");
        }

        public void Export()
        {
            if (!Serialize(this))
                Debug.LogError("Failed to export mod settings");
        }

        public void ImportFromIni()
        {
            ModSettingsReader.ParseIniToConfig((new FileIniDataParser()).ReadFile(IniPath), this);
        }

        public void ExportToIni()
        {
            File.WriteAllText(IniPath, ModSettingsReader.ParseConfigToIni(this).ToString());
        }

        private static bool Deserialize(ModSettingsConfiguration config)
        {
            fsSerializer fsSerializer = new fsSerializer();
            if (File.Exists(JsonPath))
            {
                string serializedData = File.ReadAllText(JsonPath);
                fsData data = fsJsonParser.Parse(serializedData);
                return fsSerializer.TryDeserialize(data, ref config).Succeeded;
            }

            return false;
        }

        private static bool Serialize(ModSettingsConfiguration config)
        {
            fsSerializer fsSerializer = new fsSerializer();
            fsData data;
            if (fsSerializer.TrySerialize(typeof(fsSerializer), config, out data).Succeeded)
            {
                File.WriteAllText(JsonPath, fsJsonPrinter.PrettyJson(data));
                return true;
            }

            return false;
        }

#endif

        #endregion

        #region Classes

        [Serializable]
        public class Section
        {
            public string name;
            public ModSettingsKey[] keys = new ModSettingsKey[1];

            public ModSettingsKey this[string keyName]
            {
                get { return keys.FirstOrDefault(x => x.name == keyName); }
            }
        }

        [Serializable]
        public class PresetSettings
        {
            public string name;
            public string author;

            [TextAreaAttribute(1, 4)]
            public string description;
        }

        #endregion
    }
}
