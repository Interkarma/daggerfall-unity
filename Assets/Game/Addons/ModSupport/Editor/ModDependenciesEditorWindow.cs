// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using UnityEditor;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Utility.ModSupport
{
    public class ModDependenciesEditorWindow : EditorWindow
    {
        private struct ModDependencyEditorData
        {
            internal int ModIndex;
            internal string Name;
            internal bool RequireVersion;
            internal Vector3Int Version;
            internal int IsOptional;
            internal int IsPeer;
            internal GUIContent TitleContent;
            internal bool DeletionRequest;
        }

        readonly GUIContent modName = new GUIContent("Name:", "Filename without extension. Use <other> to manually input a name if missing."); 
        readonly GUIContent requirement = new GUIContent("Requirement:", "Defines if target mod must be available.");
        readonly GUIContent[] requirementOptions = new GUIContent[] { new GUIContent("Is required for core functionalities."), new GUIContent("Is compatible but not required.") };
        readonly GUIContent loadOrder = new GUIContent("Load Order:", "Defines if target mod must be placed above.");
        readonly GUIContent[] loadOrderOptions = new GUIContent[] { new GUIContent("Must be loaded before."), new GUIContent("Can be positioned anywhere.") };
        readonly GUIContent version = new GUIContent("Version:", "The minimum version required, following semantic versioning rules.");

        GUIContent deleteDependencyContent;
        GUIContent addDependencyContent;
        GUIContent saveAndCloseContent;

        GUIStyle foldoutStyle;

        string[] availableMods;

        ModInfo modInfo;
        List<ModDependencyEditorData> dependencies = new List<ModDependencyEditorData>();
        Vector2 scrollPosition;

        private void OnEnable()
        {
            titleContent = new GUIContent("Mod Dependencies");
            minSize = new Vector2(750, 250);

            foldoutStyle = new GUIStyle
            {
                fixedHeight = 30,
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };

            deleteDependencyContent = EditorGUIUtility.IconContent("Toolbar Minus@2x");
            addDependencyContent = new GUIContent("Add Dependency", EditorGUIUtility.IconContent("CreateAddNew@2x").image);
            saveAndCloseContent = new GUIContent("Save and Close", EditorGUIUtility.IconContent("SaveAs@2x").image);

            string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, "StreamingAssets", "Mods"), "*.dfmod");
            var availableMods = files.Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
            availableMods.Add("<other>");
            this.availableMods = availableMods.ToArray();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Mod dependency rules can be used to define relationships between this mod and other ones.", MessageType.Info, true);

            scrollPosition = GUILayoutHelper.ScrollView(scrollPosition, () =>
            {
                bool applyDeletionRequests = false;

                for (int i = 0; i < dependencies.Count; i++)
                {
                    ModDependencyEditorData dependency = dependencies[i];

                    EditorGUILayout.LabelField(dependency.TitleContent ?? RefreshTitleContent(ref dependency, i), foldoutStyle);

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    GUILayoutHelper.Indent(() =>
                    {
                        GUILayoutHelper.Horizontal(() =>
                        {
                            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                            {
                                dependency.ModIndex = EditorGUILayout.Popup(modName, dependency.ModIndex, availableMods, GUILayout.MinWidth(100));
                                if (dependency.ModIndex < availableMods.Length - 1)
                                    dependency.Name = availableMods[dependency.ModIndex];
                                else
                                    dependency.Name = EditorGUILayout.TextField(dependency.Name, GUILayout.MinWidth(200));

                                if (changeCheckScope.changed)
                                    RefreshTitleContent(ref dependency, i);
                            }

                            EditorGUILayout.LabelField(".dfmod");

                            if (GUILayout.Button(deleteDependencyContent, EditorStyles.toolbarButton, GUILayout.Width(70)))
                            {
                                if (dependency.Name == null || EditorUtility.DisplayDialog("Delete dependency", $"Are you sure you want to delete dependency to {dependency.Name}?", "Yes", "No"))
                                    applyDeletionRequests |= dependency.DeletionRequest = true;
                            }
                        });

                        dependency.IsOptional = EditorGUILayout.Popup(requirement, dependency.IsOptional, requirementOptions);
                        dependency.IsPeer = EditorGUILayout.Popup(loadOrder, dependency.IsPeer, loadOrderOptions);

                        GUILayoutHelper.Horizontal(() =>
                        {
                            dependency.RequireVersion = EditorGUILayout.Toggle(version, dependency.RequireVersion);

                            GUILayoutHelper.EnableGroup(dependency.RequireVersion, () =>
                            {
                                Vector3Int version = dependency.Version;
                                version.x = EditorGUILayout.IntField(version.x);
                                version.y = EditorGUILayout.IntField(version.y);
                                version.z = EditorGUILayout.IntField(version.z);
                                dependency.Version = version;
                            });
                        });
                    });

                    dependencies[i] = dependency;

                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }

                if (applyDeletionRequests)
                {
                    var dependencies = new List<ModDependencyEditorData>();
                    for (int i = 0; i < this.dependencies.Count; i++)
                    {
                        if (!this.dependencies[i].DeletionRequest)
                            dependencies.Add(this.dependencies[i]);
                    }
                    this.dependencies = dependencies;
                }
            });

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayoutHelper.Horizontal(() =>
            {
                if (GUILayout.Button(addDependencyContent))
                {
                    dependencies.Add(new ModDependencyEditorData()
                    {
                        ModIndex = availableMods.Length - 1
                    });
                }

                if (GUILayout.Button(saveAndCloseContent))
                {
                    SaveChanges();
                    Close();
                }
            });
        }

        internal void SetModinfo(ModInfo modInfo)
        {
            this.modInfo = modInfo;

            ModDependency[] dependencies = modInfo.Dependencies ?? Array.Empty<ModDependency>();
            var editorDataGroup = new ModDependencyEditorData[dependencies.Length];

            for (int i = 0; i < dependencies.Length; i++)
            {
                ModDependency dependency = dependencies[i];
                ModDependencyEditorData editorData = new ModDependencyEditorData();

                editorData.ModIndex = availableMods.ToList().IndexOf(dependency.Name);
                if (editorData.ModIndex == -1)
                    editorData.ModIndex = availableMods.Length - 1;
                editorData.Name = dependency.Name;

                editorData.IsOptional = dependency.IsOptional ? 1 : 0;
                editorData.IsPeer = dependency.IsPeer ? 1 : 0;

                editorData.RequireVersion = !string.IsNullOrWhiteSpace(dependency.Version);
                if (editorData.RequireVersion)
                {
                    int[] versionParts = dependency.Version.Split('.').Select(x => int.Parse(x)).ToArray();
                    editorData.Version = new Vector3Int(versionParts[0], versionParts[1], versionParts[2]);
                }

                editorDataGroup[i] = editorData;
            }

            this.dependencies = editorDataGroup.ToList();
        }

        private void SaveChanges()
        {
            if (modInfo == null)
                return;

            var dependencies = new List<ModDependency>();

            foreach (ModDependencyEditorData editorData in this.dependencies)
            {
                if (string.IsNullOrWhiteSpace(editorData.Name))
                    continue;

                var dependency = new ModDependency();
                dependency.Name = editorData.Name;

                dependency.IsOptional = editorData.IsOptional == 1;
                dependency.IsPeer = editorData.IsPeer == 1;

                dependency.Version = null;
                if (editorData.RequireVersion)
                {
                    Vector3Int version = editorData.Version;
                    dependency.Version = $"{version.x}.{version.y}.{version.z}";
                }

                dependencies.Add(dependency);
            }

            modInfo.Dependencies = dependencies.ToArray();
        }

        private GUIContent RefreshTitleContent(ref ModDependencyEditorData dependency, int index)
        {
            string title;
            string iconName;

            if (string.IsNullOrWhiteSpace(dependency.Name))
            {
                title = $"New #{index}";
                iconName = "PrefabModel On Icon";
            }
            else
            {
                var rawParts = dependency.Name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var formattedParts = rawParts.Select(x => x.First().ToString().ToUpper() + x.Substring(1));
                title = string.Join(" ", formattedParts);
                iconName = "PrefabModel Icon";
            }

            return dependency.TitleContent = new GUIContent(title, EditorGUIUtility.IconContent(iconName).image);
        }
    }
}
