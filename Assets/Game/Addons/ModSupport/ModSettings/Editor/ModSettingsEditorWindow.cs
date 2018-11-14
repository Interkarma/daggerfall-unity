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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    public class ModSettingsEditorWindow : EditorWindow
    {
        #region Prefs

        private static class Prefs
        {
            public const string SaveOnExit      = "ModSettings_SaveOnExit";
            public const string CurrentTarget   = "ModSettings_CurrentTarget";
        }

        #endregion

        #region Fields

        static string rootPath;
        static bool saveOnExit;

        // Data
        string targetPath;
        string modName = "None";
        string localPath;
        string textPath;
        ModSettingsData data;

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
        bool duplicateKeys = false;

        // Layout
        float lineHeight;
        Vector2 mainScrollPosition = new Vector2(0, 0);
        Vector2 presetsScrollPosition = new Vector2(0, 0);

        Dictionary<string, object> cache = new Dictionary<string, object>();

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

            saveOnExit = EditorPrefs.GetBool(Prefs.SaveOnExit, true);
            rootPath = Path.Combine(Path.Combine(Application.dataPath, "Game"), "Addons");
            targetPath = EditorPrefs.GetString(Prefs.CurrentTarget, rootPath);
            textPath = Path.Combine(Path.Combine(Application.dataPath, "StreamingAssets"), "Text");

            if (targetPath != rootPath)
                Load();
        }

        private void OnGUI()
        {
            lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            GUILayoutHelper.Vertical(() =>
            {
                GUILayoutHelper.Area(new Rect(0, 0, 3 * position.width / 8, position.height), () =>
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

                    saveOnExit = EditorGUILayout.ToggleLeft(new GUIContent("Save on Exit", "Save automatically when window is closed."), saveOnExit);

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
                    presetsScrollPosition = GUILayoutHelper.ScrollView(presetsScrollPosition, () => presets.DoLayoutList());
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
                    if (GUILayout.Button("Import Legacy INI"))
                    {
                        string iniPath;
                        if (!string.IsNullOrEmpty(iniPath = EditorUtility.OpenFilePanel("Select ini file", rootPath, "ini")))
                            data.ImportLegacyIni(new IniParser.FileIniDataParser().ReadFile(iniPath));
                    }
                    else if (GUILayout.Button("Merge presets"))
                    {
                        string path;
                        if (!string.IsNullOrEmpty(path = EditorUtility.OpenFilePanel("Select preset file", rootPath, "json")))
                            data.LoadPresets(path, true);
                    }
                    else if (GUILayout.Button("Export localization table"))
                    {
                        string path;
                        if (!string.IsNullOrEmpty(path = EditorUtility.OpenFolderPanel("Select a folder", textPath, "")))
                            MakeTextDatabase(Path.Combine(path, string.Format("mod_{0}.txt", modName)));
                    }

                    EditorGUILayout.Space();
                });

                float areaWidth = 5 * position.width / 8;
                GUILayoutHelper.Area(new Rect(position.width - areaWidth, 0, areaWidth, position.height), () =>
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

                    mainScrollPosition = GUILayoutHelper.ScrollView(mainScrollPosition, () =>
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
            if (saveOnExit)
                Save();

            EditorPrefs.SetBool(Prefs.SaveOnExit, saveOnExit);
            EditorPrefs.SetString(Prefs.CurrentTarget, targetPath);
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
            Section section = data.Sections[currentSection = index];

            if (sectionsExpanded)
                keys[index].DoList(rect);
            else if (IsPreset)
                CurrentPreset[section.Name] = EditorGUI.ToggleLeft(LineRect(rect), section.Name, CurrentPreset[section.Name]);
            else
            {
                GUILayoutHelper.Vertical(rect, 1,
                    (line) => GUILayoutHelper.Horizontal(line, null,
                        (r) => section.Name = EditorGUI.TextField(r, "Name", section.Name),
                        (r) => section.IsAdvanced = EditorGUI.ToggleLeft(r, "Is Advanced or experimental", section.IsAdvanced)),
                    (line) => section.Description = EditorGUI.TextField(line, "Description", section.Description));
            }
        }

        private float Sections_ElementHeightCallback(int index)
        {
            if (!sectionsExpanded)
                return IsPreset ? lineHeight : lineHeight * 3;

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
            if (section.IsAdvanced)
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
                ModSettingsData.FormattedName(section.Name), style);
        }

        private void Keys_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            int line = 0;

            Section section = data.Sections[currentSection];
            Key key = section[index];

            if (!sectionExpanded[currentSection])
            {
                EditorGUI.LabelField(LineRect(rect), ModSettingsData.FormattedName(key.Name));
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
                rect.y += lineHeight * 3;
                int lines = key.OnEditorWindow(
                    rect,
                    (subrect, label, rects) => GUILayoutHelper.Horizontal(subrect, label, rects),
                    (subrect, linesPerItem, rects) => GUILayoutHelper.Vertical(subrect, linesPerItem, rects),
                    cache);

                SetKeySize(currentSection, index, (lines + 4) * lineHeight);
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
            var key = Key.Make();
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
            data = ModSettingsData.Make(SettingsPath);
            data.SaveDefaults();
            data.LoadPresets(PresetPath);

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

            presets = new ReorderableList(data.Presets, typeof(Preset), true, true, true, true);
            presets.drawHeaderCallback = r => presetsExpanded = EditorGUI.Foldout(r, presetsExpanded,  "Presets");
            presets.drawElementCallback = Presets_DrawElementCallback;
            presets.elementHeightCallback = x => presetsExpanded ? lineHeight : 0;
            
            currentPreset = -1;
            LoadPreset(-1);
            data.SyncPresets();
            modName = GetModName(targetPath);
            localPath = GetLocalPath(targetPath);
        }

        private void Save()
        {
            if (IsPreset)
                data.FillPreset(CurrentPreset, false);

            data.Save(SettingsPath);
            data.SavePresets(PresetPath);
        }

        private void LoadPreset(int index)
        {
            // Save current preset
            if (IsPreset)
                data.FillPreset(data.Presets[currentPreset], false);
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

        private void MakeTextDatabase(string path)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("- Text database for mod {0}.\n", modName);
            stringBuilder.AppendLine("- how to use: translate and place this file named mod_filename.txt inside StreamingAssets/Text. Note that an english table is NOT mandatory.");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("schema: *key, $text");

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("- Mod details");
            stringBuilder.AppendLine("- Mod.Description, \"description\"");

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("- Settings");

            foreach (Section section in data.Sections)
            {
                stringBuilder.AppendFormat("Settings.{0}.Name, \"{1}\"\n", section.Name, section.Name);
                if (!string.IsNullOrEmpty(section.Description))
                    stringBuilder.AppendFormat("Settings.{0}.Description, \"{1}\"\n", section.Name, section.Description);

                foreach (Key key in section.Keys)
                {
                    stringBuilder.AppendFormat("Settings.{0}.{1}.Name, \"{2}\"\n", section.Name, key.Name, key.Name);
                    if (!string.IsNullOrEmpty(key.Description))
                        stringBuilder.AppendFormat("Settings.{0}.{1}.Description, \"{2}\"\n", section.Name, key.Name, key.Description);
                }
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("- Presets");

            foreach (Preset preset in data.Presets)
            {
                stringBuilder.AppendFormat("Presets.{0}.Title, \"{1}\"\n", preset.Title, preset.Title);
                stringBuilder.AppendFormat("Presets.{0}.Description, \"{1}\"\n", preset.Title, preset.Description);
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("- Additional strings which can be accessed from Mod.Localize().");
            stringBuilder.AppendLine("- Default values can be provided with a table named textdatabase.txt inside the mod bundle.");

            File.WriteAllText(path, stringBuilder.ToString());
        }

        #endregion

        #region Static Helpers

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
