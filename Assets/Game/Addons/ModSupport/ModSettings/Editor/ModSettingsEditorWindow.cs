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
using UnityEditor;
using UnityEditorInternal;
using DaggerfallWorkshop.Utility;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    public class ModSettingsEditorWindow : EditorWindow
    {
        #region Fields

        static string rootPath = Path.Combine(Path.Combine(Application.dataPath, "Game"), "Addons");
        static fsSerializer fsSerializer = new fsSerializer();

        // Data
        string targetPath = rootPath;
        string modName = "None";
        string localPath;
        ModSettingsData data = new ModSettingsData();

        // Presets
        int currentPreset;
        bool presetsExpanded = true;
        ReorderableList presets;

        // Sections
        int currentSection;
        bool sectionsExpanded = false;
        ReorderableList sections;
        Dictionary<int, bool> sectionExpanded = new Dictionary<int, bool>();
        List<string> sectionNames = new List<string>();
        bool duplicateSections = false;

        // Keys
        List<ReorderableList> keys = new List<ReorderableList>();
        Dictionary<string, float> keysSizes = new Dictionary<string, float>();
        List<List<string>> keyNames = new List<List<string>>();
        List<string> cachedChoices;
        Dictionary<string, ReorderableList> multipleChoices = new Dictionary<string, ReorderableList>();
        bool duplicateKeys = false;

        // Layout
        float lineHeight;
        Vector2 mainScrollPosition = new Vector2(0, 0);
        Vector2 presetsScrollPosition = new Vector2(0, 0);

        #endregion

        #region Properties

        string SettingsPath { get { return Path.Combine(targetPath, "modsettings.json"); } }
        string PresetPath { get { return Path.Combine(targetPath, "modpresets.json"); } }

        bool IsPreset { get { return currentPreset != -1; } }
        Preset CurrentPreset { get { return data.Presets[currentPreset]; } }

        #endregion

        #region Unity

        [MenuItem("Daggerfall Tools/Mod Settings")]
        static void Init()
        {
            var window = GetWindow<ModSettingsEditorWindow>();
            window.titleContent = new GUIContent("Mod Settings", (Texture2D)EditorGUIUtility.Load("icons/SettingsIcon.png"));
        }

        private void OnEnable()
        {
            minSize = new Vector2(1000, 500);
            Load();
        }

        private void OnGUI()
        {
            lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            GUILayoutHelper.Vertical(() =>
            {
                LayoutArea(new Rect(0, 0, 3 * position.width / 8, position.height), () =>
                {
                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        GUILayout.Box((Texture2D)EditorGUIUtility.Load("Assets/Resources/AppIcon1.png"), GUILayout.Width(50), GUILayout.Height(50));
                        var titleStyle = new GUIStyle(EditorStyles.largeLabel);
                        titleStyle.font = EditorGUIUtility.Load("Assets/Resources/Fonts/TESFonts/Kingthings Petrock.ttf") as Font;
                        titleStyle.fontSize = 50;
                        GUILayout.Label(modName, titleStyle);
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        GUILayout.Label(localPath);

                        if (IconButton("d_editicon.sml", "Select target path"))
                        {
                            if (!string.IsNullOrEmpty(targetPath = EditorUtility.OpenFolderPanel("Select mod folder", rootPath, "Mod")))
                                Load();
                        }
                        if (IconButton("d_Refresh", "Reload and discharge unsaved changes"))
                            Load();
                        using (new EditorGUI.DisabledScope(modName == "None" || duplicateSections || duplicateKeys))
                            if (IconButton("d_P4_CheckOutLocal", "Save settings to disk"))
                                Save();
                    });

                    if (modName == "None")
                        EditorGUILayout.HelpBox("Select a folder to store settings.", MessageType.Info);
                    if (duplicateSections)
                        EditorGUILayout.HelpBox("Multiple sections with the same name detected!", MessageType.Error);
                    if (duplicateKeys)
                        EditorGUILayout.HelpBox("Multiple keys with the same name in a section detected!", MessageType.Error);

                    DrawHeader("Header");
                    EditorGUILayout.HelpBox("Version of settings checked against local values and presets.", MessageType.None);
                    data.Version = EditorGUILayout.TextField("Version", data.Version);

                    DrawHeader("Presets");
                    EditorGUILayout.HelpBox("Pre-defined values for all or a set of settings. Author is an optional field.", MessageType.None);
                    ScrollView(ref presetsScrollPosition, () => presets.DoLayoutList());
                    EditorGUILayout.Space();
                    if (presets.index != -1)
                    {
                        var preset = data.Presets[presets.index];
                        preset.Author = EditorGUILayout.TextField("Author", preset.Author);
                        EditorGUILayout.PrefixLabel("Description");
                        preset.Description = EditorGUILayout.TextArea(preset.Description);
                    }

                    GUILayout.FlexibleSpace();

                    DrawHeader("Tools");
                    if (GUILayout.Button("Import INI"))
                    {
                        string iniPath;
                        if (!string.IsNullOrEmpty(iniPath = EditorUtility.OpenFilePanel("Select ini file", rootPath, "ini")))
                            ModSettingsReader.ParseIniToSettings(data, new IniParser.FileIniDataParser().ReadFile(iniPath));
                    }
                    else if (GUILayout.Button("Export INI"))
                    {
                        string iniPath;
                        if (!string.IsNullOrEmpty(iniPath = EditorUtility.SaveFilePanel("Select target file", rootPath, "modsettings", "ini")))
                            new IniParser.FileIniDataParser().WriteFile(iniPath, ModSettingsReader.ParseSettingsToIni(data));
                    }
                    else if (GUILayout.Button("Merge presets"))
                    {
                        string path;
                        var newPresets = new List<Preset>();
                        if (!string.IsNullOrEmpty(path = EditorUtility.OpenFilePanel("Select preset file", rootPath, "json")) && TryDeserialize(newPresets, path))
                            data.Presets.AddRange(newPresets);
                    }

                    EditorGUILayout.Space();
                });

                float areaWidth = 5 * position.width / 8;
                LayoutArea(new Rect(position.width - areaWidth, 0, areaWidth, position.height), () =>
                {
                    EditorGUILayout.Space();

                    if (data.Presets.Count > 0 && presets.index != -1)
                    {
                        if (GUILayout.SelectionGrid(IsPreset ? 1 : 0, new string[] { "Defaults", data.Presets[presets.index].Title }, 2) == 0)
                        {
                            if (IsPreset)
                                LoadPreset(-1);
                        }
                        else
                        {
                            if (currentPreset != presets.index)
                                LoadPreset(presets.index);
                        }
                    }

                    ScrollView(ref mainScrollPosition, () =>
                    {
                        sections.DoLayoutList();

                        duplicateSections = duplicateKeys = false;
                        var sectionNames = new List<string>();
                        foreach (var section in data.Sections)
                        {
                            section.Name = !string.IsNullOrEmpty(section.Name) ? section.Name.Replace(" ", string.Empty) : GetUniqueName(sectionNames, "Section");
                            sectionNames.Add(section.Name);

                            var keyNames = new List<string>();
                            foreach (var key in section.Keys)
                            {
                                key.Name = !string.IsNullOrEmpty(key.Name) ? key.Name.Replace(" ", string.Empty) : GetUniqueName(keyNames, "Key");
                                keyNames.Add(key.Name);
                            }

                            duplicateKeys |= DuplicatesDetected(keyNames);
                            this.keyNames.Add(keyNames);
                        }
                        this.sectionNames = sectionNames;
                        duplicateSections |= DuplicatesDetected(sectionNames);
                    });

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.Space();
                });
            });
        }

        private void OnDisable()
        {
            Save();
        }

        #endregion

        #region Callbacks

        private void Presets_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (presetsExpanded || index == presets.index)
                data.Presets[index].Title = EditorGUI.TextField(LineRect(rect), data.Presets[index].Title);
        }

        private void Sections_DrawHeaderCallback(Rect rect)
        {
            var style = new GUIStyle(EditorStyles.foldout);
            style.fontStyle = FontStyle.Bold;
            sectionsExpanded = EditorGUI.Foldout(LineRect(rect), sectionsExpanded, "Settings", style);
        }

        private void Sections_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            string sectionName = data.Sections[currentSection = index].Name;

            if (sectionsExpanded)
                keys[index].DoList(rect);
            else if (IsPreset)
                CurrentPreset[sectionName] = EditorGUI.ToggleLeft(LineRect(rect), sectionName, CurrentPreset[sectionName]);
            else
                data.Sections[index].Name = EditorGUI.TextField(LineRect(rect), sectionName);
        }

        private float Sections_ElementHeightCallback(int index)
        {
            if (!sectionsExpanded)
                return lineHeight;

            float totalSize = 0;
            for (int i = 0; i < data.Sections[index].Keys.Count; i++)
            {
                float size;
                keysSizes.TryGetValue(index + " - " + i, out size);
                totalSize += size;
            }

            if (totalSize == 0)
                totalSize = lineHeight;

            if (index == sections.count - 1)
                totalSize += 30f;

            return totalSize + 2.5f * lineHeight;
        }

        private void Sections_OnAddCallback(ReorderableList l)
        {
            var section = new Section();
            section.Name = GetUniqueName(sectionNames, "Section");
            data.Sections.Insert(++l.index, section);
            keys.Insert(l.index, GetKeyList(data.Sections[l.index]));
            data.SyncPresets();
        }

        private void Sections_OnRemoveCallback(ReorderableList l)
        {
            string message = string.Format("Are you sure you want to delete section '{0}'?", data[l.index].Name);
            if (EditorUtility.DisplayDialog("Delete section", message, "Yes", "No"))
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
        }

        private void Sections_OnReorderCallback(ReorderableList list)
        {
            for (int i = 0; i < data.Sections.Count; i++)
                keys[i].list = data.Sections[i].Keys;
        }

        private void Keys_DrawHeaderCallback(Rect rect)
        {
            Section section = data.Sections[currentSection];
            var style = new GUIStyle(EditorStyles.foldout);
            if (section.Name == ModSettingsReader.internalSection)
                style.normal.textColor = Color.red;

            if (IsPreset)
            {
                var toggleRect = new Rect(rect.x, rect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                CurrentPreset[section.Name] = EditorGUI.Toggle(toggleRect, CurrentPreset[section.Name]);
                rect.x += toggleRect.width;
            }

            if (!sectionExpanded.ContainsKey(currentSection))
                sectionExpanded.Add(currentSection, false);
            sectionExpanded[currentSection] = EditorGUI.Foldout(LineRect(rect), sectionExpanded[currentSection],
                ModSettingsReader.FormattedName(section.Name), style);
        }

        private void Keys_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            int line = 0;

            Section section = data.Sections[currentSection];
            Key key = section[index];

            if (!sectionExpanded[currentSection])
            {
                EditorGUI.LabelField(LineRect(rect), ModSettingsReader.FormattedName(key.Name));
                SetKeySize(currentSection, index, lineHeight);
                return;
            }

            if (IsPreset)
            {
                using (new EditorGUI.DisabledScope(!CurrentPreset[section.Name]))
                {
                    CurrentPreset[section.Name, key.Name] = EditorGUI.ToggleLeft(LineRect(rect, line++), key.Name, CurrentPreset[section.Name, key.Name]);
                    EditorGUI.LabelField(LineRect(rect, line++), key.Description);
                }
            }
            else
            {
                key.Name = EditorGUI.TextField(LineRect(rect, line++), "Name", key.Name);
                key.Description = EditorGUI.TextField(LineRect(rect, line++), "Description", key.Description);
            }

            using (new EditorGUI.DisabledScope(IsPreset))
                data.SetType(currentSection, index, ref key, (KeyType)EditorGUI.EnumPopup(LineRect(rect, line++), "UI Control", key.KeyType));

            using (new EditorGUI.DisabledScope(IsPreset && !CurrentPreset[section.Name]))
            {
                int lines = 4;
                switch (key.KeyType)
                {
                    case KeyType.Toggle:
                        var toggle = (ToggleKey)key;
                        toggle.Value = EditorGUI.Toggle(LineRect(rect, line++), "Checked", toggle.Value);
                        break;

                    case KeyType.MultipleChoice:
                        var mult = (MultipleChoiceKey)key;
                        mult.Value = EditorGUI.Popup(LineRect(rect, line++), "Selected", mult.Value, mult.Options.ToArray());
                        using (new EditorGUI.DisabledScope(IsPreset))
                        {
                            string dictKey = data[currentSection].Name + "-" + key.Name;
                            ReorderableList options;
                            if (!multipleChoices.TryGetValue(dictKey, out options))
                            {
                                options = new ReorderableList(mult.Options, typeof(string), true, true, true, true);
                                options.drawHeaderCallback = r =>
                                {
                                    EditorGUI.LabelField(r, "Options");
                                    if (GUI.Button(new Rect(r.x + r.width - 30, r.y, 30, EditorGUIUtility.singleLineHeight), (Texture2D)EditorGUIUtility.Load("icons/SettingsIcon.png"), EditorStyles.toolbarButton))
                                    {
                                        var menu = new GenericMenu();
                                        menu.AddItem(new GUIContent("Copy choices"), false, () => cachedChoices = mult.Options.ToList());
                                        if (cachedChoices != null)
                                            menu.AddItem(new GUIContent("Paste choices"), false, () => {options.list = mult.Options = cachedChoices;});
                                        menu.ShowAsContext();
                                    }
                                };
                                options.onAddCallback = x => x.list.Add(string.Empty);
                                options.drawElementCallback = (r, i, a, f) => options.list[i] = EditorGUI.TextField(LineRect(r), (string)options.list[i]);
                                multipleChoices.Add(dictKey, options);
                            }
                            options.DoList(LineRect(rect, line++));
                            lines += options.list.Count == 0 ? 3 : options.list.Count + 2;
                        }
                        break;

                    case KeyType.SliderInt:
                        var slider = (SliderIntKey)key;
                        slider.Value = EditorGUI.IntSlider(LineRect(rect, line++), "Value", slider.Value, slider.Min, slider.Max);
                        using (new EditorGUI.DisabledScope(IsPreset))
                        {
                            slider.Min = EditorGUI.IntField(HalfLineRect(rect, line), "Min", slider.Min);
                            slider.Max = EditorGUI.IntField(HalfLineRect(rect, line++, true), "Max", slider.Max);
                        }
                        lines++;
                        break;

                    case KeyType.SliderFloat:
                        var fslider = (SliderFloatKey)key;
                        fslider.Value = EditorGUI.Slider(LineRect(rect, line++), "Value", fslider.Value, fslider.Min, fslider.Max);
                        using (new EditorGUI.DisabledScope(IsPreset))
                        {
                            fslider.Min = EditorGUI.FloatField(HalfLineRect(rect, line), "Min", fslider.Min);
                            fslider.Max = EditorGUI.FloatField(HalfLineRect(rect, line++, true), "Max", fslider.Max);
                        }
                        lines++;
                        break;

                    case KeyType.TupleInt:
                        var tuple = (TupleIntKey)key;
                        if (tuple.Value == null) tuple.Value = Tuple<int, int>.Make(0, 100);
                        tuple.Value.First = EditorGUI.IntField(HalfLineRect(rect, line), "First", tuple.Value.First);
                        tuple.Value.Second = EditorGUI.IntField(HalfLineRect(rect, line++, true), "Second", tuple.Value.Second);
                        lines++;
                        break;

                    case KeyType.TupleFloat:
                        var ftuple = (TupleFloatKey)key;
                        if (ftuple.Value == null) ftuple.Value = Tuple<float, float>.Make(0, 1);
                        ftuple.Value.First = EditorGUI.FloatField(HalfLineRect(rect, line), "First", ftuple.Value.First);
                        ftuple.Value.Second = EditorGUI.FloatField(HalfLineRect(rect, line++, true), "Second", ftuple.Value.Second);
                        lines++;
                        break;

                    case KeyType.Text:
                        var text = (TextKey)key;
                        text.Value = EditorGUI.TextArea(new Rect(rect.x, rect.y + 3 * lineHeight, rect.width, EditorGUIUtility.singleLineHeight * 3), text.Value);
                        lines += 2;
                        break;

                    case KeyType.Color:
                        var color = (ColorKey)key;
                        color.Value = EditorGUI.ColorField(LineRect(rect, line++), "Color", color.Value);
                        break;
                }

                SetKeySize(currentSection, index, (lines + 1) * lineHeight);
            }
        }

        private float Keys_ElementHeightCallback(int index)
        {
            float size;
            keysSizes.TryGetValue(currentSection + " - " + index, out size);
            return size;
        }

        private void Keys_OnAddCallback(ReorderableList l)
        {
            var key = Key.NewDefaultKey();
            key.Name = GetUniqueName(((KeyCollection)l.list).Select(x => x.Name).ToList(), "Key");
            l.list.Add(key);

            data.SyncPresets();
        }

        private void Keys_OnRemoveCallback(ReorderableList l)
        {
            string message = string.Format("Are you sure you want to delete key '{0}'?", data[currentSection][l.index].Name);
            if (EditorUtility.DisplayDialog("Delete key", message, "Yes", "No"))
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
        }

        #endregion

        #region Helpers

        private void Load()
        {
            if (!TryDeserialize(data, SettingsPath))
                data = new ModSettingsData();

            sections = new ReorderableList(data.Sections, typeof(SectionCollection), true, true, true, true);
            sections.drawHeaderCallback = Sections_DrawHeaderCallback;
            sections.drawElementCallback = Sections_DrawElementCallback;
            sections.elementHeightCallback = Sections_ElementHeightCallback;
            sections.onAddCallback = Sections_OnAddCallback;
            sections.onRemoveCallback = Sections_OnRemoveCallback;
            sections.onReorderCallback = Sections_OnReorderCallback;

            keys.Clear();
            foreach (var section in data.Sections)
            {
                ReorderableList key = new ReorderableList(section.Keys, typeof(KeyCollection), true, true, true, true);
                key.drawHeaderCallback = Keys_DrawHeaderCallback;
                key.drawElementCallback = Keys_DrawElementCallback;
                key.elementHeightCallback = Keys_ElementHeightCallback;
                key.onAddCallback = Keys_OnAddCallback;
                key.onRemoveCallback = Keys_OnRemoveCallback;
                keys.Add(key);
            }

            TryDeserialize(data.Presets, PresetPath);

            presets = new ReorderableList(data.Presets, typeof(Preset), true, true, true, true);
            presets.drawHeaderCallback = r => presetsExpanded = EditorGUI.Foldout(r, presetsExpanded,  "Presets");
            presets.drawElementCallback = Presets_DrawElementCallback;
            presets.elementHeightCallback = x => presetsExpanded ? lineHeight : 0;

            data.SaveDefaults();
            currentPreset = -1;
            LoadPreset(-1);
            data.SyncPresets();
            modName = GetModName(targetPath);
            localPath = GetLocalPath(targetPath);
        }

        private void Save()
        {
            // Save defaults
            TrySerialize(data, SettingsPath);

            // Save presets            
            if (data.Presets.Count > 0)
                TrySerialize(data.Presets, PresetPath);
            else if (File.Exists(PresetPath))
                File.Delete(PresetPath);
        }

        private void LoadPreset(int index)
        {
            // Save current preset
            if (IsPreset)
                data.UpdatePreset(data.Presets[currentPreset]);
            else
                data.SaveDefaults();

            // Load new preset
            if ((currentPreset = index) != -1)
                data.ApplyPreset(data.Presets[currentPreset]);
            else
                data.RestoreDefaults();
        }

        private ReorderableList GetKeyList(Section section)
        {
            return new ReorderableList(section.Keys, typeof(Key), true, true, true, true)
            {
                drawHeaderCallback = Keys_DrawHeaderCallback,
                drawElementCallback = Keys_DrawElementCallback,
                elementHeightCallback = Keys_ElementHeightCallback,
                onAddCallback = Keys_OnAddCallback,
                onRemoveCallback = Keys_OnRemoveCallback
            };
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

        #endregion

        #region Static Helpers

        private static bool TryDeserialize<T>(T data, string path)
        {
            if (!File.Exists(path))
                return false;

            fsResult fsResult = ModManager._serializer.TryDeserialize(fsJsonParser.Parse(File.ReadAllText(path)), ref data);
            if (fsResult.Failed)
                Debug.LogErrorFormat("Failed to deserialize '{0}':\n{1}", path, fsResult.FormattedMessages);
            return fsResult.Succeeded;
        }

        private static bool TrySerialize<T>(T data, string path)
        {
            fsData fsData;
            fsResult fsResult = ModManager._serializer.TrySerialize(data, out fsData);
            if (fsResult.Succeeded)
            {
                using (StreamWriter writer = new StreamWriter(path, false))
                    writer.WriteLine(fsJsonPrinter.PrettyJson(fsData));
                return true;
            }

            Debug.LogErrorFormat("Failed to serialize '{0}':\n{1}", path, fsResult.FormattedMessages);
            return false;
        }

        private static bool IconButton(string iconName, string tooltip)
        {
            var icon = (Texture2D)EditorGUIUtility.Load(string.Format("icons/{0}.png", iconName));
            return GUILayout.Button(new GUIContent(icon, tooltip), GUILayout.Width(30), GUILayout.Height(20));
        }

        private static void DrawHeader(string title)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label(title, EditorStyles.boldLabel);
        }

        private static void LayoutArea(Rect rect, Action callback)
        {
            GUILayout.BeginArea(rect);
            callback();
            GUILayout.EndArea();
        }

        private static void ScrollView(ref Vector2 position, Action callback)
        {
            position = EditorGUILayout.BeginScrollView(position);
            callback();
            EditorGUILayout.EndScrollView();
        }

        // Same as ObjectNames.GetUniqueName but without spaces.
        private static string GetUniqueName(List<string> existingNames, string baseName)
        {
            string name = baseName;
            int index = 1;

            // Appends the next available numerical increment
            while (existingNames.Any(x => x == name))
                name = string.Format("{0}({1})", baseName, index++);

            return name;
        }

        private static string GetModName(string path)
        {
            int index = path.IndexOf("Addons/");
            if (index != -1)
            {
                string name = path.Substring(index + "Addons/".Length);
                int endIndex = name.IndexOf('/');
                if (endIndex != -1)
                    name = name.Substring(0, endIndex);
                return name;
            }

            return "Unknown";
        }

        private static string GetLocalPath(string path)
        {
            int index = path.LastIndexOf("Assets");
            if (index != -1)
                return path.Substring(index + "Assets".Length);

            return "Unknown";
        }

        private static bool DuplicatesDetected(List<string> names)
        {
            return names.GroupBy(x => x).Any(g => g.Count() > 1);
        }

        #endregion
    }
}
