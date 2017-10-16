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

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using FullSerializer;
using KeyType = DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings.ModSettingsKey.KeyType;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    [CreateAssetMenu(fileName = "modsettings", menuName = "Mods/Settings", order = 0)]
    public class ModSettingsConfiguration : ScriptableObject
    {
        static string SerializedFile
        {
            get { return Path.Combine(Path.Combine(Application.dataPath, "Untracked"), "modsettings.json"); }
        }

        [Tooltip("Version of settings config file. This is not the version of mod or preset.")]
        public string version = "1.0";

        [Tooltip("Is a preset or the main config?")]
        public bool isPreset = false;

        [Tooltip("Settings for this preset")]
        public PresetSettings presetSettings;

        [Tooltip("Presets for this mod")]
        public string[] presets;

        public Section[] sections = new Section[1];

        public Section this[string sectionName]
        {
            get { return sections.FirstOrDefault(x => x.name == sectionName); }
        }

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

        public KeyType GetKeyType(string sectionName, string keyName)
        {
            ModSettingsKey key;
            if (Key(sectionName, keyName, out key))
                return key.type;

            return (KeyType)(-1);
        }

        public bool IsSlider(string sectionName, string keyName)
        {
            KeyType type = GetKeyType(sectionName, keyName);
            return type == KeyType.Slider || type == KeyType.FloatSlider;
        }

        public bool IsTuple(string sectionName, string keyName)
        {
            KeyType type = GetKeyType(sectionName, keyName);
            return type == KeyType.Tuple || type == KeyType.FloatTuple;
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

        private static bool Deserialize(ModSettingsConfiguration config)
        {
            fsSerializer fsSerializer = new fsSerializer();
            if (File.Exists(SerializedFile))
            {
                string serializedData = File.ReadAllText(SerializedFile);
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
                File.WriteAllText(SerializedFile, fsJsonPrinter.PrettyJson(data));
                return true;
            }

            return false;
        }

        [Serializable]
        public class Section
        {
            public string name = "Section";
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
    }
}
