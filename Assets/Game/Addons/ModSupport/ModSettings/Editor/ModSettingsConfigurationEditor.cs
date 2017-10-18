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

        Dictionary<int, bool> sectionExpanded = new Dictionary<int, bool>();
        Dictionary<string, bool> keysExpanded = new Dictionary<string, bool>();

        bool editMode = false;
        int sectionsCount;
        Dictionary<int, int> keysCount = new Dictionary<int, int>();

        ModSettingsConfiguration Target;

        GUIStyle sectionFoldoutStyle;
        GUIStyle keyFoldoutStyle;

        #endregion

        #region Inspector Setup

        private void Awake()
        {
            sectionFoldoutStyle = GetFoldoutStyle(new Color(0.1f, 0.1f, 0.1f, 1));
            keyFoldoutStyle = GetFoldoutStyle(new Color(0.3f, 0.3f, 0.3f, 1));
        }

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

                editMode = EditorGUILayout.Toggle("Edit Mode", editMode);

                using (new EditorGUI.DisabledScope(true))
                {
                    sectionsCount = EditorGUILayout.IntField("Size", sectionsCount);
                }

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

                        using (new EditorGUI.DisabledScope(true))
                        {
                            int thisKeysCount = KeysCount(i);
                        }

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
                                        SerializedProperty _multipleChoice = _key.FindPropertyRelative("multipleChoice");
                                        SerializedProperty _selected = _multipleChoice.FindPropertyRelative("selected");
                                        _selected.intValue = EditorGUILayout.Popup(_selected.intValue, Target.sections[i].keys[j].multipleChoice.choices);
                                        EditorGUILayout.PropertyField(_multipleChoice.FindPropertyRelative("choices"), true);
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

                                if (editMode)
                                    keysCount[i] += InsertOrRemove(_keys, j, i);
                            }
                        }

                        EditorGUI.indentLevel--;

                        if (editMode)
                            sectionsCount += InsertOrRemove(_sections, i);

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
            isExpanded = EditorGUILayout.Foldout(isExpanded, title, true, sectionFoldoutStyle);
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
            isExpanded = EditorGUILayout.Foldout(isExpanded, title, true, keyFoldoutStyle);
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

        /// <summary>
        /// Insert or remove a key or section as requested by controls.
        /// </summary>
        private int InsertOrRemove(SerializedProperty _array, int index, int? section = null)
        {
            int increment = 0;

            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 10);
            GUILayoutOption keyButtonsHeight = GUILayout.Height(14f);

            if (GUILayout.Button("Insert", keyButtonsHeight))
            {
                increment++;

                if (section.HasValue)
                    InsertKeyAtIndex(section.Value, index);
                else
                    InsertSectionAtIndex(index);
            }

            var removeKeyButtonStyle = new GUIStyle(GUI.skin.button);
            removeKeyButtonStyle.normal.textColor = Color.red;
            if (GUILayout.Button("Remove", removeKeyButtonStyle, keyButtonsHeight) && _array.arraySize > 1)
            {
                increment--;

                _array.DeleteArrayElementAtIndex(index);
            }

            GUILayout.EndHorizontal();

            return increment;
        }

        /// <summary>
        /// Insert a section as a new empty instance.
        /// </summary>
        private void InsertSectionAtIndex(int index)
        {
            List<ModSettingsConfiguration.Section> sections = Target.sections.ToList();
            sections.Insert(index + 1, new ModSettingsConfiguration.Section());
            Target.sections = sections.ToArray();
        }

        /// <summary>
        /// Insert a key as a new empty instance.
        /// </summary>
        private void InsertKeyAtIndex(int section, int index)
        {
            List<ModSettingsKey> keys = Target.sections[section].keys.ToList();
            keys.Insert(index + 1, new ModSettingsKey());
            Target.sections[section].keys = keys.ToArray();
        }

        private static GUIStyle GetFoldoutStyle(Color color)
        {
            GUIStyle style = new GUIStyle(EditorStyles.foldout);
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = color;
            style.onNormal.textColor = color;
            style.hover.textColor = color;
            style.onHover.textColor = color;
            style.focused.textColor = color;
            style.onFocused.textColor = color;
            style.active.textColor = color;
            style.onActive.textColor = color;
            return style;
        }

        private static bool DuplicatesDetected(List<string> names)
        {
            return names.GroupBy(x => x).Any(g => g.Count() > 1);
        }

        #endregion
    }
}
