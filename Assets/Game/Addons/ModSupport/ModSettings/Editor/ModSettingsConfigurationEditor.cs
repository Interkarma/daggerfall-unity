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
using UnityEditorInternal;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    [CustomEditor(typeof(ModSettingsConfiguration))]
    public class ModSettingsConfigurationEditor : Editor
    {
        #region Fields

        SerializedProperty _version;
        SerializedProperty _isPreset;
        SerializedProperty _presetSettings;
        SerializedProperty _presets;
        SerializedProperty _sections;

        bool presetSyncExpanded = false;
        bool addNewKeys = false;
        int selectionGridSelected = 0;
        int parseFormat = 0;

        ReorderableList sections;
        Dictionary<int, bool> sectionExpanded = new Dictionary<int, bool>();
        int currentSection;

        List<ReorderableList> keys = new List<ReorderableList>();
        Dictionary<string, float> keysSizes = new Dictionary<string, float>();

        bool isPreset;
        float lineHeight;
        List<string> cachedChoices;

        ModSettingsConfiguration Target;
        ModSettingsConfiguration parent;

        #endregion

        #region Unity

        private void OnEnable()
        {
            _version = serializedObject.FindProperty("version");
            _isPreset = serializedObject.FindProperty("isPreset");
            _presetSettings = serializedObject.FindProperty("presetSettings");
            _presets = serializedObject.FindProperty("presets");
            _sections = serializedObject.FindProperty("sections");

            sections = new ReorderableList(serializedObject, _sections, true, true, true, true);
            sections.drawHeaderCallback = Sections_DrawHeaderCallback;
            sections.drawElementCallback = Sections_DrawElementCallback;
            sections.onAddCallback = Sections_OnAddCallback;
            sections.onRemoveCallback = Sections_OnRemoveCallback;

            for (int i = 0; i < _sections.arraySize; i++)
                AddKeysList(i);

            Target = (ModSettingsConfiguration)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (isPreset = _isPreset.boolValue)
                EditorGUILayout.HelpBox("Create a preset for a mod. This is a group of a portion or all settings values. " +
                    "Remember to sync it with the main settings asset.", MessageType.Info);
            else
                EditorGUILayout.HelpBox("Create settings for a mod. Remember to name the file 'modsettings.asset'. " +
                    "You can acces them in game through DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings.", MessageType.Info);

            // Header
            EditorGUILayout.LabelField("Header", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            using (new EditorGUI.DisabledScope(isPreset))
                EditorGUILayout.PropertyField(_version);
            EditorGUILayout.PropertyField(_isPreset);

            // Presets tools
            if (isPreset)
            {
                presetSyncExpanded = EditorGUILayout.Foldout(presetSyncExpanded, "Sync", true);
                if (presetSyncExpanded)
                {
                    EditorGUILayout.HelpBox("Sync preset with main settings (doesn't reset compatible values)", MessageType.None);
                    parent = (ModSettingsConfiguration)EditorGUILayout.ObjectField("Parent", parent, typeof(ModSettingsConfiguration), true);
                    if (parent)
                    {
                        addNewKeys = EditorGUILayout.Toggle(new GUIContent("Add New Keys", "Add missing keys or only sync existing ones?"), addNewKeys);
                        if (GUILayout.Button("Sync"))
                            Target.Sync(parent, addNewKeys);
                    }
                }

                EditorGUILayout.PropertyField(_presetSettings, new GUIContent("Info"), true);
            }
            else
                EditorGUILayout.PropertyField(_presets, true);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Settings
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            selectionGridSelected = GUILayout.SelectionGrid(selectionGridSelected, new string[] { "Sections", "Keys" }, 2);

            sections.draggable = sections.displayAdd = sections.displayRemove = !isPreset;
            foreach (var keyControl in keys)
                keyControl.draggable = keyControl.displayAdd = keyControl.displayRemove = !isPreset;

            if (selectionGridSelected == 0)
                sections.DoLayoutList();

            bool duplicateSections = false, duplicateKeys = false;
            var sectionNames = new List<string>();
            for (int i = 0; i < _sections.arraySize; i++)
            {
                SerializedProperty _section = _sections.GetArrayElementAtIndex(i);
                SerializedProperty _sectionName = _section.FindPropertyRelative("name");
                if (string.IsNullOrEmpty(_sectionName.stringValue))
                    _sectionName.stringValue = "Section";
                sectionNames.Add(_sectionName.stringValue);

                EditorGUI.indentLevel++;

                if (selectionGridSelected == 1)
                    keys[currentSection = i].DoLayoutList();

                SerializedProperty _keys = _section.FindPropertyRelative("keys");
                var keyNames = new List<string>();
                for (int j = 0; j < _keys.arraySize; j++)
                    keyNames.Add(_keys.GetArrayElementAtIndex(j).FindPropertyRelative("name").stringValue);

                EditorGUI.indentLevel--;

                duplicateKeys |= DuplicatesDetected(keyNames);
            }

            duplicateSections |= DuplicatesDetected(sectionNames);

            if (duplicateSections)
                EditorGUILayout.HelpBox("Multiple sections with the same name detected!", MessageType.Error);
            if (duplicateKeys)
                EditorGUILayout.HelpBox("Multiple keys with the same name in a section detected!", MessageType.Error);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Import/Export
            EditorGUILayout.LabelField("Import/Export", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox(string.Format("Import from/export to Untracked/modsettings.{0}", parseFormat == 0 ? "ini" : "json"), MessageType.None);
            parseFormat = GUILayout.SelectionGrid(parseFormat, new string[] { "Ini", "Json" }, 2);
            if (parseFormat == 0)
            {
                if (GUILayout.Button("Import"))
                    Target.ImportFromIni();
                else if (GUILayout.Button("Export"))
                    Target.ExportToIni();
            }
            else
            { 
            
                if (GUILayout.Button("Import"))
                    Target.Import();
                else if (GUILayout.Button("Export"))
                    Target.Export();
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Callbacks

        private void Sections_DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(LineRect(rect), "Sections");
        }

        private void Sections_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty _sectionName = _sections.GetArrayElementAtIndex(index).FindPropertyRelative("name");

            using (new EditorGUI.DisabledScope(isPreset))
                EditorGUI.PropertyField(LineRect(rect), _sectionName, GUIContent.none, false);
        }

        private void Sections_OnAddCallback(ReorderableList l)
        {
            int index = l.serializedProperty.arraySize++;
            SerializedProperty section = l.serializedProperty.GetArrayElementAtIndex(index);
            section.FindPropertyRelative("name").stringValue = "Section";
            section.FindPropertyRelative("keys").arraySize = 0;
            AddKeysList(_sections.arraySize - 1);
            l.index = index;
        }

        private void Sections_OnRemoveCallback(ReorderableList l)
        {
            if (EditorUtility.DisplayDialog("Delete section", "Are you sure you want to delete this section?", "Yes", "No"))
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
        }

        private void Keys_DrawHeaderCallback(Rect rect)
        {
            SerializedProperty _sectionName = _sections.GetArrayElementAtIndex(currentSection).FindPropertyRelative("name");
            if (!sectionExpanded.ContainsKey(currentSection))
                sectionExpanded.Add(currentSection, false);
            var style = new GUIStyle(EditorStyles.foldout);
            if (_sectionName.stringValue == ModSettingsReader.internalSection)
                style.normal.textColor = Color.red;
            sectionExpanded[currentSection] = EditorGUI.Foldout(LineRect(rect), sectionExpanded[currentSection], _sectionName.stringValue, style);
        }

        private void Keys_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            int line = 0;

            SerializedProperty _section = _sections.GetArrayElementAtIndex(currentSection);
            SerializedProperty _keys = _section.FindPropertyRelative("keys");

            SerializedProperty _key = _keys.GetArrayElementAtIndex(index);
            SerializedProperty _keyName = _key.FindPropertyRelative("name");
            if (string.IsNullOrEmpty(_keyName.stringValue))
                _keyName.stringValue = "Key";

            if (!sectionExpanded[currentSection])
            {
                EditorGUI.LabelField(LineRect(rect), _keyName.stringValue);
                SetKeySize(currentSection, index, lineHeight);
                return;
            }

            SerializedProperty _type = _key.FindPropertyRelative("type");

            using (new EditorGUI.DisabledScope(isPreset))
            {
                EditorGUI.PropertyField(LineRect(rect, line++), _keyName, GUIContent.none);
                EditorGUI.PropertyField(LineRect(rect, line++), _key.FindPropertyRelative("description"));
                EditorGUI.PropertyField(LineRect(rect, line++), _type, new GUIContent("UI Control"));
            }

            int lines = 4;
            switch ((ModSettingsKey.KeyType)_type.enumValueIndex)
            {
                case ModSettingsKey.KeyType.Toggle:
                    SerializedProperty _value = _key.FindPropertyRelative("toggle").FindPropertyRelative("value");
                    EditorGUI.PropertyField(LineRect(rect, line++), _value, true);
                    break;

                case ModSettingsKey.KeyType.MultipleChoice:
                    SerializedProperty _multipleChoice = _key.FindPropertyRelative("multipleChoice");
                    SerializedProperty _selected = _multipleChoice.FindPropertyRelative("selected");
                    SerializedProperty _choices = _multipleChoice.FindPropertyRelative("choices");
                    var multipleChoice = Target.sections[currentSection].keys[index].multipleChoice;
                    string[] choices = multipleChoice.choices;
                    _selected.intValue = EditorGUI.Popup(LineRect(rect, line++), _selected.intValue, choices);
                    using (new EditorGUI.DisabledScope(isPreset))
                    {
                        if (DropDownButton(rect, line))
                        {
                            var menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Copy choices"), false, CopyChoices, new List<string>(choices));
                            if (cachedChoices != null)
                                menu.AddItem(new GUIContent("Paste choices"), false, PasteChoices, multipleChoice);
                            menu.ShowAsContext();
                        }
                        EditorGUI.PropertyField(LineRect(rect, line++), _choices, true);
                    }
                    lines += _choices.isExpanded ? 2 + choices.Length : 1;
                    break;

                case ModSettingsKey.KeyType.Slider:
                    SerializedProperty _slider = _key.FindPropertyRelative("slider");
                    SerializedProperty _sliderMin = _slider.FindPropertyRelative("min");
                    SerializedProperty _sliderMax = _slider.FindPropertyRelative("max");
                    if (_sliderMin.intValue == 0 && _sliderMax.intValue == 0)
                        _sliderMax.intValue = 100;
                    EditorGUI.IntSlider(LineRect(rect, line++),
                        _slider.FindPropertyRelative("value"), _sliderMin.intValue, _sliderMax.intValue);
                    GUILayout.BeginHorizontal();
                    using (new EditorGUI.DisabledScope(isPreset))
                    {
                        EditorGUI.PropertyField(HalfLineRect(rect, line), _sliderMin);
                        EditorGUI.PropertyField(HalfLineRect(rect, line++, true), _sliderMax);
                    }
                    GUILayout.EndHorizontal();
                    lines += 1;
                    break;

                case ModSettingsKey.KeyType.FloatSlider:
                    SerializedProperty _floatSlider = _key.FindPropertyRelative("floatSlider");
                    SerializedProperty _floatSliderValue = _floatSlider.FindPropertyRelative("value");
                    SerializedProperty _floatSliderMin = _floatSlider.FindPropertyRelative("min");
                    SerializedProperty _floatSliderMax = _floatSlider.FindPropertyRelative("max");
                    if (_floatSliderMin.floatValue == 0 && _floatSliderMax.floatValue == 0)
                        _floatSliderMax.floatValue = 1;
                    EditorGUI.Slider(LineRect(rect, line++), _floatSliderValue, _floatSliderMin.floatValue, _floatSliderMax.floatValue);
                    GUILayout.BeginHorizontal();
                    using (new EditorGUI.DisabledScope(isPreset))
                    {
                        EditorGUI.PropertyField(HalfLineRect(rect, line), _floatSliderMin);
                        EditorGUI.PropertyField(HalfLineRect(rect, line++, true), _floatSliderMax);
                    }
                    GUILayout.EndHorizontal();
                    lines += 1;
                    break;

                case ModSettingsKey.KeyType.Tuple:
                    SerializedProperty _tuple = _key.FindPropertyRelative("tuple");
                    GUILayout.BeginHorizontal();
                    EditorGUI.PropertyField(HalfLineRect(rect, line), _tuple.FindPropertyRelative("first"), GUIContent.none);
                    EditorGUI.PropertyField(HalfLineRect(rect, line++, true), _tuple.FindPropertyRelative("second"), GUIContent.none);
                    GUILayout.EndHorizontal();
                    break;

                case ModSettingsKey.KeyType.FloatTuple:
                    SerializedProperty _floatTuple = _key.FindPropertyRelative("floatTuple");
                    GUILayout.BeginHorizontal();
                    EditorGUI.PropertyField(HalfLineRect(rect, line), _floatTuple.FindPropertyRelative("first"), GUIContent.none);
                    EditorGUI.PropertyField(HalfLineRect(rect, line++, true), _floatTuple.FindPropertyRelative("second"), GUIContent.none);
                    GUILayout.EndHorizontal();
                    break;

                case ModSettingsKey.KeyType.Text:
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + 3 * lineHeight, rect.width, EditorGUIUtility.singleLineHeight * 3),
                        _key.FindPropertyRelative("text").FindPropertyRelative("text"));
                    lines += 2;
                    break;

                case ModSettingsKey.KeyType.Color:
                    EditorGUI.PropertyField(LineRect(rect, line++), _key.FindPropertyRelative("color").FindPropertyRelative("color"));
                    break;
            }

            SetKeySize(currentSection, index, (lines + 1) * lineHeight);
        }

        private float Keys_ElementHeightCallback(int index)
        {
            float size;
            keysSizes.TryGetValue(currentSection + " - " + index, out size);
            return size;
        }

        private void Keys_OnAddCallback(ReorderableList l)
        {
            int index = l.serializedProperty.arraySize++;
            SerializedProperty key = l.serializedProperty.GetArrayElementAtIndex(index);
            key.FindPropertyRelative("name").stringValue = "Key";
            key.FindPropertyRelative("description").stringValue = string.Empty;
            key.FindPropertyRelative("type").enumValueIndex = 0;
            l.index = index;
        }

        private void Keys_OnRemoveCallback(ReorderableList l)
        {
            if (EditorUtility.DisplayDialog("Delete key", "Are you sure you want to delete this key?", "Yes", "No"))
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
        }

        #endregion

        #region Helpers

        private void AddKeysList(int section)
        {
            ReorderableList key = new ReorderableList(serializedObject,
                    _sections.GetArrayElementAtIndex(section).FindPropertyRelative("keys"),
                    true, true, true, true);
            key.drawHeaderCallback = Keys_DrawHeaderCallback;
            key.drawElementCallback = Keys_DrawElementCallback;
            key.elementHeightCallback = Keys_ElementHeightCallback;
            key.onAddCallback = Keys_OnAddCallback;
            key.onRemoveCallback = Keys_OnRemoveCallback;
            keys.Add(key);
        }

        private void SetKeySize(int section, int key, float size)
        {
            string dictKey = currentSection + " - " + key;
            if (keysSizes.ContainsKey(dictKey))
                keysSizes[dictKey] = size;
            else keysSizes.Add(dictKey, size);
        }

        private Rect LineRect(Rect rect, int line = 0)
        {
            return new Rect(rect.x, rect.y + line * lineHeight,
                rect.width, EditorGUIUtility.singleLineHeight);
        }

        private Rect HalfLineRect(Rect rect, int line = 0, bool right = false)
        {
            return new Rect(right ? rect.x + rect.width / 2 : rect.x,
                rect.y + line * lineHeight, rect.width / 2, EditorGUIUtility.singleLineHeight);
        }

        private bool DropDownButton(Rect rect, int line)
        {
            return GUI.Button(new Rect(rect.x + rect.width * 15f / 16,
                rect.y + line * lineHeight, rect.width / 16, EditorGUIUtility.singleLineHeight),
                "✲", EditorStyles.boldLabel);
        }

        private void CopyChoices(object choices)
        {
            cachedChoices = choices as List<string>;
        }

        private void PasteChoices(object choicesKey)
        {
            var multipleChoice = choicesKey as ModSettingsKey.MultipleChoice;
            multipleChoice.choices = cachedChoices.ToArray();
        }

        private static bool DuplicatesDetected(List<string> names)
        {
            return names.GroupBy(x => x).Any(g => g.Count() > 1);
        }

        #endregion
    }
}
