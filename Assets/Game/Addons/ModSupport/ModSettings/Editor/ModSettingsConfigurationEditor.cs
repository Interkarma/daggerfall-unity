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

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    [CustomEditor(typeof(ModSettingsConfiguration))]
    public class ModSettingsConfigurationEditor : Editor
    {
        #region Fields

        const bool foldoutStartExpanded = true;
        const int newSectionKeyCount = 1;

        SerializedProperty _version;
        SerializedProperty _isPreset;
        SerializedProperty _presetSettings;
        SerializedProperty _presets;

        SerializedProperty _sections;

        bool sectionsExpanded = foldoutStartExpanded;
        int sectionsCount;

        Dictionary<int, bool> sectionExpanded = new Dictionary<int, bool>();
        Dictionary<int, int> keysCount = new Dictionary<int, int>();
        Dictionary<string, bool> keysExpanded = new Dictionary<string, bool>();

        ModSettingsConfiguration Target;

        #endregion

        #region Inspector Setup

        private void OnEnable()
        {
            _version = serializedObject.FindProperty("version");
            _isPreset = serializedObject.FindProperty("isPreset");
            _presetSettings = serializedObject.FindProperty("presetSettings");
            _presets = serializedObject.FindProperty("presets");

            _sections = serializedObject.FindProperty("sections");

            Target = (ModSettingsConfiguration)target;
            sectionsCount = Target.sections.Length;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox("Create settings for a mod. Remember to name the file 'modsettings.asset'", MessageType.Info);

            // Header
            EditorGUILayout.LabelField("Header", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_version);
            EditorGUILayout.PropertyField(_isPreset);

            if (_isPreset.boolValue)
                EditorGUILayout.PropertyField(_presetSettings, true);
            else
                EditorGUILayout.PropertyField(_presets, true);

            // Settings
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

            sectionsExpanded = EditorGUILayout.Foldout(sectionsExpanded, "Sections", true);
            if (sectionsExpanded)
            {
                EditorGUI.indentLevel++;
                sectionsCount = EditorGUILayout.IntField("Size", sectionsCount);
                if (_sections.arraySize != sectionsCount)
                    ResizeArray(ref _sections, sectionsCount);

                var sectionNames = new List<string>();

                for (int i = 0; i < _sections.arraySize; i++)
                {
                    SerializedProperty _section = _sections.GetArrayElementAtIndex(i);
                    SerializedProperty _sectionName = _section.FindPropertyRelative("name");
                    if (string.IsNullOrEmpty(_sectionName.stringValue))
                        _sectionName.stringValue = "Section";
                    sectionNames.Add(_sectionName.stringValue);

                    if (IsSectionFoldoutExpanded(i, _sectionName.stringValue))
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(_sectionName);

                        SerializedProperty _keys = _section.FindPropertyRelative("keys");

                        var keyNames = new List<string>();

                        int thisKeysCount = KeysCount(i);
                        if (_keys.arraySize != thisKeysCount)
                            ResizeArray(ref _keys, thisKeysCount);

                        for (int j = 0; j < _keys.arraySize; j++)
                        {
                            SerializedProperty _key = _keys.GetArrayElementAtIndex(j);
                            SerializedProperty _keyName = _key.FindPropertyRelative("name");
                            if (string.IsNullOrEmpty(_keyName.stringValue))
                                _keyName.stringValue = "Key";
                            keyNames.Add(_keyName.stringValue);

                            if (IsKeyFoldoutExpanded(i, j, _keyName.stringValue))
                            {
                                EditorGUILayout.PropertyField(_keyName);
                                EditorGUILayout.PropertyField(_key.FindPropertyRelative("description"));

                                SerializedProperty _type = _key.FindPropertyRelative("type");
                                EditorGUILayout.PropertyField(_type);

                                var keyType = (ModSettingsKey.KeyType)_type.enumValueIndex;
                                switch (keyType)
                                {
                                    case ModSettingsKey.KeyType.Toggle:
                                        EditorGUILayout.PropertyField(_key.FindPropertyRelative("toggle").FindPropertyRelative("value"));
                                        break;

                                    case ModSettingsKey.KeyType.MultipleChoice:
                                        EditorGUILayout.PropertyField(_key.FindPropertyRelative("multipleChoice").FindPropertyRelative("choices"), true);
                                        break;

                                    case ModSettingsKey.KeyType.Slider:
                                        SerializedProperty _slider = _key.FindPropertyRelative("slider");
                                        SerializedProperty _sliderMin = _slider.FindPropertyRelative("min");
                                        SerializedProperty _sliderMax = _slider.FindPropertyRelative("max");
                                        if (_sliderMin.intValue == 0 && _sliderMax.intValue == 0)
                                            _sliderMax.intValue = 100;
                                        EditorGUILayout.IntSlider(_slider.FindPropertyRelative("value"), _sliderMin.intValue, _sliderMax.intValue);
                                        GUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(_sliderMin);
                                        EditorGUILayout.PropertyField(_sliderMax);
                                        GUILayout.EndHorizontal();
                                        break;

                                    case ModSettingsKey.KeyType.FloatSlider:
                                        SerializedProperty _floatSlider = _key.FindPropertyRelative("floatSlider");
                                        SerializedProperty _floatSliderValue = _floatSlider.FindPropertyRelative("value");
                                        SerializedProperty _floatSliderMin = _floatSlider.FindPropertyRelative("min");
                                        SerializedProperty _floatSliderMax = _floatSlider.FindPropertyRelative("max");
                                        if (_floatSliderMin.floatValue == 0 && _floatSliderMax.floatValue == 0)
                                            _floatSliderMax.floatValue = 1;
                                        EditorGUILayout.Slider(_floatSliderValue, _floatSliderMin.floatValue, _floatSliderMax.floatValue);
                                        GUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(_floatSliderMin);
                                        EditorGUILayout.PropertyField(_floatSliderMax);
                                        GUILayout.EndHorizontal();
                                        break;

                                    case ModSettingsKey.KeyType.Tuple:
                                        SerializedProperty _tuple = _key.FindPropertyRelative("tuple");
                                        GUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(_tuple.FindPropertyRelative("first"));
                                        EditorGUILayout.PropertyField(_tuple.FindPropertyRelative("second"));
                                        GUILayout.EndHorizontal();
                                        break;

                                    case ModSettingsKey.KeyType.FloatTuple:
                                        SerializedProperty _floatTuple = _key.FindPropertyRelative("floatTuple");
                                        GUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(_floatTuple.FindPropertyRelative("first"));
                                        EditorGUILayout.PropertyField(_floatTuple.FindPropertyRelative("second"));
                                        GUILayout.EndHorizontal();
                                        break;

                                    case ModSettingsKey.KeyType.Text:
                                        EditorGUILayout.PropertyField(_key.FindPropertyRelative("text").FindPropertyRelative("text"));
                                        break;

                                    case ModSettingsKey.KeyType.Color:
                                        EditorGUILayout.PropertyField(_key.FindPropertyRelative("color").FindPropertyRelative("color"));
                                        break;
                                }
                            }
                        }

                        EditorGUI.indentLevel--;

                        if (DuplicatesDetected(keyNames))
                            EditorGUILayout.HelpBox("Multiple keys with the same name in a section detected!", MessageType.Error);
                    }
                }

                EditorGUI.indentLevel--;

                if (DuplicatesDetected(sectionNames))
                    EditorGUILayout.HelpBox("Multiple sections with the same name detected!", MessageType.Error);
            }

            // Import/Export
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Import/Export", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Import from/export to Untracked/modsettings.json, export to Untracked/modsettings.ini", MessageType.None);

            if (GUILayout.Button("Import"))
                Target.Import();

            if (GUILayout.Button("Export"))
                Target.Export();

            if (GUILayout.Button("Export (Ini)"))
                Target.ExportToIni();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Is this section visible?
        /// </summary>
        private bool IsSectionFoldoutExpanded(int section, string title)
        {
            bool isExpanded;
            if (!sectionExpanded.TryGetValue(section, out isExpanded))
            {
                isExpanded = foldoutStartExpanded;
                sectionExpanded.Add(section, isExpanded);
            }
            isExpanded = EditorGUILayout.Foldout(isExpanded, title, true);
            sectionExpanded[section] = isExpanded;
            return isExpanded;
        }

        /// <summary>
        /// Is this key visible?
        /// </summary>
        private bool IsKeyFoldoutExpanded(int section, int key, string title)
        {
            bool isExpanded;
            string dictKey = section + "_" + key;
            if (!keysExpanded.TryGetValue(dictKey, out isExpanded))
            {
                isExpanded = foldoutStartExpanded;
                keysExpanded.Add(dictKey, isExpanded);
            }
            isExpanded = EditorGUILayout.Foldout(isExpanded, title, true);
            keysExpanded[dictKey] = isExpanded;
            return isExpanded;
        }

        /// <summary>
        /// Get number of keys in a section
        /// </summary>
        private int KeysCount(int section)
        {
            int count;
            if (!keysCount.TryGetValue(section, out count))
            {
                if (section < Target.sections.Length)
                    count = Target.sections[section].keys.Length;
                else
                    count = newSectionKeyCount;

                keysCount.Add(section, count);
            }
            count = EditorGUILayout.IntField("Keys", count);
            keysCount[section] = count;
            return count;
        }

        private static void ResizeArray(ref SerializedProperty _array, int length)
        {
            while (_array.arraySize > length)
                _array.DeleteArrayElementAtIndex(_array.arraySize - 1);

            while (_array.arraySize < length)
                _array.InsertArrayElementAtIndex(_array.arraySize);
        }

        private static bool DuplicatesDetected(List<string> names)
        {
            return names.GroupBy(x => x).Any(g => g.Count() > 1);
        }

        #endregion
    }
}
