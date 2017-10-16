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
        SerializedProperty _version;
        SerializedProperty _isPreset;
        SerializedProperty _presetSettings;
        SerializedProperty _presets;

        SerializedProperty _sections;

        bool sectionsExpanded = true;
        int sectionsCount;

        Dictionary<int, bool> sectionExpanded = new Dictionary<int, bool>();
        Dictionary<int, int> keysCount = new Dictionary<int, int>();

        Dictionary<string, bool> keysExpanded = new Dictionary<string, bool>();

        ModSettingsConfiguration Target;

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

            EditorGUILayout.LabelField("Header", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_version);
            EditorGUILayout.PropertyField(_isPreset);

            if (_isPreset.boolValue)
                EditorGUILayout.PropertyField(_presetSettings, true);
            else
                EditorGUILayout.PropertyField(_presets, true);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

            sectionsExpanded = EditorGUILayout.Foldout(sectionsExpanded, "Sections", true);
            if (sectionsExpanded)
            {
                EditorGUI.indentLevel++;
                sectionsCount = EditorGUILayout.IntField("Size", sectionsCount);
                while (_sections.arraySize > sectionsCount)
                    _sections.DeleteArrayElementAtIndex(_sections.arraySize - 1);
                while (_sections.arraySize < sectionsCount)
                    _sections.InsertArrayElementAtIndex(_sections.arraySize);

                var sectionNames = new List<string>();

                for (int i = 0; i < _sections.arraySize; i++)
                {
                    SerializedProperty _section = _sections.GetArrayElementAtIndex(i);
                    SerializedProperty _sectionName = _section.FindPropertyRelative("name");
                    sectionNames.Add(_sectionName.stringValue);

                    bool thisSectionsExpanded;
                    if (!sectionExpanded.TryGetValue(i, out thisSectionsExpanded))
                    {
                        thisSectionsExpanded = true;
                        sectionExpanded.Add(i, true);
                    }
                    thisSectionsExpanded = EditorGUILayout.Foldout(thisSectionsExpanded, _sectionName.stringValue, true);
                    sectionExpanded.Remove(i);
                    sectionExpanded.Add(i, thisSectionsExpanded);
                    if (thisSectionsExpanded)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(_sectionName);

                        SerializedProperty _keys = _section.FindPropertyRelative("keys");

                        var keyNames = new List<string>();

                        int thisKeysCount;
                        if (!keysCount.TryGetValue(i, out thisKeysCount))
                        {
                            try
                            {
                                thisKeysCount = Target.sections[i].keys.Length;
                            }
                            catch
                            {
                                thisKeysCount = 1;
                            }
                            keysCount.Add(i, thisKeysCount);
                        }
                        thisKeysCount = EditorGUILayout.IntField("Keys", thisKeysCount);
                        while (_keys.arraySize > thisKeysCount)
                            _keys.DeleteArrayElementAtIndex(_sections.arraySize - 1);
                        while (_keys.arraySize < thisKeysCount)
                            _keys.InsertArrayElementAtIndex(_keys.arraySize);
                        keysCount.Remove(i);
                        keysCount.Add(i, thisKeysCount);

                        for (int j = 0; j < _keys.arraySize; j++)
                        {
                            SerializedProperty _key = _keys.GetArrayElementAtIndex(j);
                            SerializedProperty _keyName = _key.FindPropertyRelative("name");

                            bool thisKeyExpanded;
                            string dictKey = i + "_" + j;
                            if (!keysExpanded.TryGetValue(dictKey, out thisKeyExpanded))
                            {
                                thisKeyExpanded = true;
                                keysExpanded.Add(dictKey, true);
                            }
                            thisKeyExpanded = EditorGUILayout.Foldout(thisKeyExpanded, _keyName.stringValue, true);
                            keysExpanded.Remove(dictKey);
                            keysExpanded.Add(dictKey, thisKeyExpanded);
                            if (thisKeyExpanded)
                            {
                                EditorGUILayout.PropertyField(_keyName);
                                EditorGUILayout.PropertyField(_key.FindPropertyRelative("description"));

                                SerializedProperty _type = _key.FindPropertyRelative("type");
                                EditorGUILayout.PropertyField(_type);

                                var keyType = (ModSettingsKey.KeyType)_type.enumValueIndex;
                                switch(keyType)
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

                                keyNames.Add(_keyName.stringValue);
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

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Import/Export", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Import from/export to Untracked/modsettings.json", MessageType.None);

            if (GUILayout.Button("Import"))
                Target.Import();

            if (GUILayout.Button("Export"))
                Target.Export();

            serializedObject.ApplyModifiedProperties();
        }

        private static bool DuplicatesDetected(List<string> names)
        {
            return names.GroupBy(x => x).Any(g => g.Count() > 1);
        }
    }
}
