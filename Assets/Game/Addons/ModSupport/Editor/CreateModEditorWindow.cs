// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    TheLacus
// 
// Notes:
//

// #define LOG_BUILD_TIME

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using FullSerializer;


/*
 * Todo:
 * 1. append .byte to .dll files
 * 2. Support for asset bundle variants
 */

namespace DaggerfallWorkshop.Game.Utility.ModSupport
{
    [Serializable]
    public class CreateModEditorWindow : EditorWindow
    {

        const string windowTitle = "Mod Builder";
        const string menuPath = "Daggerfall Tools/Mod Builder";

        static string currentFilePath = "";
        static string modOutPutPath = "";

        public bool fileOpen = false;
        [SerializeField]
        public bool ShowFileFoldOut = false;
        [SerializeField]
        Vector2 scrollPos;
        [SerializeField]
        int assetSelection = -1;
        ModInfo modInfo;

        bool precompiledMod;

        //asset bundles will be created for any targets here
        readonly BuildTarget[] buildTargets = new BuildTarget[]
        {
            BuildTarget.StandaloneWindows,
            BuildTarget.StandaloneOSX,
            BuildTarget.StandaloneLinux64,
        };
        readonly bool[] buildTargetsToggles = new bool[] { true, true, true };
        ModCompressionOptions compressionOption = ModCompressionOptions.LZ4;
        bool ModInfoReady { get { return ModInfoReadyTowrite(); } }
        List<string> Assets { get { return modInfo.Files; } set { modInfo.Files = value; } }         //list of assets to be added
        readonly GUIStyle titleStyle = new GUIStyle();
        readonly GUIStyle fieldStyle = new GUIStyle();
        GUIContent documentationGUIContent;
        GUIContent targetInfoGUIContent;
        bool isSupportedEditorVersion;
        bool automaticallyRegisterQuestLists;

        void OnEnable()
        {
            if (EditorPrefs.HasKey("modOutPutPath"))
                modOutPutPath = EditorPrefs.GetString("modOutPutPath", GetTempModDirPath());
            else
                modOutPutPath = GetTempModDirPath();
            if (EditorPrefs.HasKey("lastModFile"))
                currentFilePath = EditorPrefs.GetString("lastModFile");

            for (int i = 0; i < buildTargetsToggles.Length; i++)
                buildTargetsToggles[i] = EditorPrefs.GetBool($"ModBuildTarget:{buildTargets[i]}", buildTargetsToggles[i]);

            modInfo = ReadModInfoFile(currentFilePath);
            ResetRegisterQuestListsValue();

            titleStyle.fontSize = 15;
            fieldStyle.fontSize = 12;
            minSize = new Vector2(1280, 600);

            documentationGUIContent = new GUIContent(EditorGUIUtility.IconContent("_Help"));
            documentationGUIContent.text = " Mod System Documentation";
            targetInfoGUIContent = new GUIContent($"{VersionInfo.DaggerfallUnityProductName} {VersionInfo.DaggerfallUnityStatus} {VersionInfo.DaggerfallUnityVersion} (Unity {VersionInfo.BaselineUnityVersion})");
            isSupportedEditorVersion = IsSupportedEditorVersion();
        }

        void OnDisable()
        {
            EditorPrefs.SetString("modOutPutPath", modOutPutPath);
            EditorPrefs.SetString("lastModFile", currentFilePath);

            for (int i = 0; i < buildTargetsToggles.Length; i++)
                EditorPrefs.SetBool($"ModBuildTarget:{buildTargets[i]}", buildTargetsToggles[i]);
        }


        private bool ModInfoReadyTowrite()
        {
            if (!fileOpen)
                return false;
            else if (this.modInfo == null)
                return false;
            else if (string.IsNullOrEmpty(this.modInfo.ModTitle))
                return false;
            else
                return true;
        }


        [MenuItem(menuPath)]
        static void Init()
        {
            CreateModEditorWindow window = (CreateModEditorWindow)EditorWindow.GetWindow(typeof(CreateModEditorWindow));
            window.titleContent = new GUIContent(windowTitle);
        }

        public ModInfo ReadModInfoFile(string path)
        {
            ModInfo info = null;
            try
            {
                if (string.IsNullOrEmpty(path) || File.Exists(path) == false)
                    return new ModInfo();

                string inPut = File.ReadAllText(currentFilePath);
                ModManager._serializer.TryDeserialize(fsJsonParser.Parse(inPut), ref info).AssertSuccessWithoutWarnings();
                if (string.IsNullOrEmpty(info.GUID) || info.GUID == "invalid")
                    info.GUID = System.Guid.NewGuid().ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ModInfo();
            }

            fileOpen = true;
            return info;
        }


        void OnGUI()
        {
            bool openDependenciesWindow = false;
            EditorGUI.indentLevel++;

            GUILayoutHelper.Horizontal(() =>
            {
                if (GUILayout.Button("Create New Mod"))
                {
                    modInfo = new ModInfo();
                    modInfo.GUID = System.Guid.NewGuid().ToString();
                    fileOpen = SaveModFile();
                    if (modInfo != null)
                    {
                        modInfo.DFUnity_Version = VersionInfo.DaggerfallUnityVersion;
                    }
                }

                else if (GUILayout.Button("Open Existing Mod Settings File"))
                {
                    try
                    {
                        currentFilePath = EditorUtility.OpenFilePanelWithFilters("", ModManager.EditorModsDirectory, new string[] { "JSON", "dfmod.json"});

                        if (!File.Exists(currentFilePath))
                        {
                            Debug.Log("Invalid file selection");
                            currentFilePath = null;
                            fileOpen = false;
                            return;
                        }

                        modInfo = ReadModInfoFile(currentFilePath);
                        ResetRegisterQuestListsValue();
                        Debug.Log(string.Format("opened mod file for: {0}", modInfo.ModTitle));

                    }
                    catch (Exception ex)
                    {
                        Debug.Log(string.Format("Error while trying to open mod file at: {0} \n {1}", currentFilePath, ex.Message));
                        currentFilePath = null;
                        fileOpen = false;
                        return;
                    }

                    fileOpen = true;
                }

                if (GUILayout.Button(documentationGUIContent))
                    Help.BrowseURL("https://www.dfworkshop.net/projects/daggerfall-unity/modding/");
            });

            if (!isSupportedEditorVersion)
                EditorGUILayout.HelpBox("Unsupported version of Unity Editor: generated mods may not be compatible with release builds!", MessageType.Warning);

            if (modInfo == null)
            {
                fileOpen = false;
                modInfo = new ModInfo();
            }

            if (!fileOpen) // if no fileopen, hide rest of UI
            {
                EditorGUILayout.HelpBox("Open a manifest file or create a new one to edit or build a mod.", MessageType.Info);
                return;
            }

            GUILayoutHelper.Vertical(() =>
            {
                EditorGUILayout.Space();

                GUILayoutHelper.Horizontal(() =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Current Target: "), titleStyle);
                    GUILayout.Space(-1000);
                    EditorGUILayout.LabelField(targetInfoGUIContent, fieldStyle);
                });

                GUILayoutHelper.Horizontal(() =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Current File: "), titleStyle);
                    GUILayout.Space(-1000);
                    EditorGUILayout.LabelField(new GUIContent(currentFilePath), fieldStyle);
                });
                EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

                EditorGUI.indentLevel++;

                GUILayoutHelper.Vertical(() =>
                {

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Mod Title:"), titleStyle);
                        modInfo.ModTitle = EditorGUILayout.TextField(modInfo.ModTitle, GUILayout.MinWidth(1000));
                    });

                    EditorGUILayout.Space();


                    GUILayoutHelper.Horizontal(() =>
                    {

                        EditorGUILayout.LabelField(new GUIContent("Mod Version:"), titleStyle);
                        modInfo.ModVersion = EditorGUILayout.TextField(modInfo.ModVersion, GUILayout.MinWidth(1000));
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Mod Creator:"), titleStyle);
                        modInfo.ModAuthor = EditorGUILayout.TextField(modInfo.ModAuthor, GUILayout.MinWidth(1000));
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Contact Info:"), titleStyle);
                        modInfo.ContactInfo = EditorGUILayout.TextField(modInfo.ContactInfo, GUILayout.MinWidth(1000));
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("DFUnity Version:"), titleStyle);
                        modInfo.DFUnity_Version = EditorGUILayout.TextField(modInfo.DFUnity_Version, GUILayout.MinWidth(1000));
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Mod Description:"), titleStyle);
                        modInfo.ModDescription = EditorGUILayout.TextArea(modInfo.ModDescription, GUILayout.MinWidth(1000));
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Mod GUID: "), titleStyle);
                        EditorGUILayout.LabelField(new GUIContent(modInfo.GUID));
                        if (GUILayout.Button("Generate New GUID"))
                            modInfo.GUID = System.Guid.NewGuid().ToString();
                        GUILayout.Space(300);
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Mod Dependencies: "), titleStyle);
                        if (GUILayout.Button("Open Dependencies Editor"))
                            openDependenciesWindow = true;
                        GUILayout.Space(300);
                    });
                });

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;

                GUILayoutHelper.Horizontal(() =>
                {
                    if (GUILayout.Button("Add Selected Asset(s)"))
                        AddSelectedAssetsToMod();

                    else if (GUILayout.Button("Remove Selected Asset(s)"))
                    {
                        if (Assets == null || Assets.Count < 1)
                            return;
                        else if (assetSelection < 0 || assetSelection > Assets.Count)
                            return;
                        else
                            Assets.RemoveAt(assetSelection);
                    }
                    else if (GUILayout.Button("Clear ALL Asset(s)"))
                    {
                        if (Assets != null)
                            Assets = new List<string>();
                    }

                });

            });

            EditorGUILayout.Space();

            ShowFileFoldOut = GUILayoutHelper.Foldout(ShowFileFoldOut, new GUIContent("Files"), () =>
            {
                scrollPos = GUILayoutHelper.ScrollView(scrollPos, () =>
                {
                    assetSelection = GUILayout.SelectionGrid(assetSelection, modInfo.Files.ToArray(), 1);
                });
            });

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            GUILayout.Label("\tBuild Targets:\n", titleStyle);

            for (int i = 0; i < buildTargetsToggles.Length; i++)
            {
                buildTargetsToggles[i] = EditorGUILayout.Toggle(buildTargets[i].ToString(), buildTargetsToggles[i], GUILayout.ExpandWidth(false));
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();

            GUILayout.Label("Compression Type:\n", titleStyle);
            compressionOption = (ModCompressionOptions)EditorGUILayout.EnumPopup("", compressionOption, GUILayout.MaxWidth(125));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            GUILayout.Label("Dependencies:\n", titleStyle);
            if(GUILayout.Button("Collect Dependencies", GUILayout.MaxWidth(200)) && ModInfoReady)
            {
                foreach(var assetPath in Assets.ToArray())
                {
                    var depends = AssetDatabase.GetDependencies(assetPath);
                    foreach(var d in depends)
                    {
                        AddAssetToMod(d);
                    }
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            GUILayout.Label("\tQuestLists:\n", titleStyle);
            automaticallyRegisterQuestLists = EditorGUILayout.ToggleLeft(new GUIContent("Automatically Register QuestLists", "Automatically discover Quest Lists and register them in game."), automaticallyRegisterQuestLists, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

            EditorGUILayout.Space();

            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField("Build Path:", titleStyle);
                GUILayout.Space(-1000);
                EditorGUILayout.LabelField(modOutPutPath, fieldStyle);
                if (GUILayout.Button("Set", GUILayout.Width(50)))
                {
                    modOutPutPath = EditorUtility.SaveFolderPanel("Select Destination,", Application.dataPath, "");
                    Debug.Log("build path: " + modOutPutPath);
                }
            });
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

            EditorGUILayout.Space();
            precompiledMod = EditorGUILayout.ToggleLeft(new GUIContent("Precompiled (experimental)", "Compile C# files into a .dll."), precompiledMod);

            GUILayoutHelper.Horizontal(() =>
            {

                if (GUILayout.Button("Save Mod Settings to File"))
                {
                    SaveModFile(File.Exists(currentFilePath));
                    Debug.Log("got path: " + currentFilePath);
                }
                else if (GUILayout.Button("Build Mod"))
                {
                    SaveModFile(true);
                    BuildMod();
                }

            });

            if (openDependenciesWindow)
            {
                var modDependenciesWindow = CreateInstance<ModDependenciesEditorWindow>();
                modDependenciesWindow.SetModinfo(modInfo);
                modDependenciesWindow.ShowUtility();
            }
        }


        bool SaveModFile(bool supressWindow = false)
        {
            ModManager.SeekModContributes(modInfo, automaticallyRegisterQuestLists);

            string path = currentFilePath;
            fsData fsData;
            ModManager._serializer.TrySerialize(modInfo, out fsData).AssertSuccessWithoutWarnings();
            string outPut = fsJsonPrinter.PrettyJson(fsData);
            string directory = "";

            if (!supressWindow)
                path = EditorUtility.SaveFilePanel("Save", ModManager.EditorModsDirectory, modInfo.ModTitle, "dfmod.json");

            Debug.Log("save path: " + path);

            if (!string.IsNullOrEmpty(path))
                directory = path.Substring(0, path.LastIndexOfAny(new char[] { '\\', '/' }));

            if (Directory.Exists(directory))
                File.WriteAllText(path, outPut);
            else
            {
                Debug.LogWarning("Save canceled or failed to save: " + path);
                return false;
            }

            if (File.Exists(path))
            {
                currentFilePath = path;
                EditorPrefs.SetString("lastModFile", currentFilePath);
                ModManager.ImportAsset(path);
                return true;
            }
            else
            {
                Debug.LogError("Error saving file to: " + path);
                return false;
            }

        }

        //Add selected assets in editor to File list
        void AddSelectedAssetsToMod()
        {
            if (Assets == null || !fileOpen)
            {
                Debug.LogError("error in mod builder");
                return;
            }

            var selection = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Deep);
            if (selection.Length == 0)
            {
                EditorUtility.DisplayDialog("Mod Builder", "No assets selected.", "OK");
                return;
            }

            for (int i = 0; i < selection.Length; i++)
            {
                string path = FixSeperatorCharacters(AssetDatabase.GetAssetPath(selection[i].GetInstanceID()));
                if (string.IsNullOrWhiteSpace(path))
                {
                    const string errorMessage = "Selected asset is not saved to disk. Make sure you selected an asset from the Project window and not the Hierarchy window.";
                    EditorUtility.DisplayDialog("Mod Builder", errorMessage, "OK");
                }
                else if (!File.Exists(path))
                {
                    EditorUtility.DisplayDialog("Mod Builder", $"Path {path} doesn't exist.", "OK");
                }
                else
                {
                    UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
                    for (int j = 0; j < objs.Length; j++)
                    {
                        string subAssetPath = AssetDatabase.GetAssetPath(objs[j]);
                        if (!Assets.Contains(subAssetPath))
                        {
                            Assets.Add(subAssetPath);
                        }
                    }
                }
            }
        }

        // Check if there is already added Quest Lists in the ModInfo
        void ResetRegisterQuestListsValue()
        {
            automaticallyRegisterQuestLists = modInfo.Contributes?.QuestLists?.Length > 0;
        }

        bool AddAssetToMod(string assetPath)
        {
            assetPath = FixSeperatorCharacters(assetPath);
            if (!Assets.Contains(assetPath))
            {
                Assets.Add(assetPath);
                return true;
            }
            else
                return false;
        }

        //Builds the actual asset bundle.  Only builds if files added & required information set in mod info fields
        bool BuildMod()
        {
            const string modBuilderLabel = "Mod Builder";

            if (!ModInfoReady)
            {
                Debug.LogWarning("Not ready to build, canceled.");
                return false;
            }

            //get destination for mod
            modOutPutPath = (Directory.Exists(modOutPutPath) ? modOutPutPath : Application.dataPath);
            string modFilePath = EditorUtility.SaveFilePanel("Save", modOutPutPath, modInfo.ModTitle, "dfmod");

            if (!Directory.Exists(modOutPutPath) || string.IsNullOrEmpty(modFilePath))
            {
                Debug.LogWarning("Invalid build path");
                return false;
            }
            else if (Assets == null || Assets.Count < 0)
            {
                Debug.LogWarning("No assets selected");
                return false;
            }

            modOutPutPath = modFilePath.Substring(0, modFilePath.LastIndexOfAny(new char[] { '\\', '/'})+1);

            //refresh
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string fileName = modFilePath.Substring(modFilePath.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
            AddAssetToMod(GetAssetPathFromFilePath(currentFilePath));
            List<string> assets = Assets;

            if (precompiledMod)
            {
                string dataPath = Application.dataPath;
                var scriptPaths = new List<string>();
                var otherAssets = new List<string>();

                foreach (string assetPath in assets)
                {
                    if (assetPath.EndsWith(".cs", StringComparison.Ordinal))
                        scriptPaths.Add(Path.Combine(dataPath, assetPath.Substring("Assets/".Length)));
                    else
                        otherAssets.Add(assetPath);
                }

                assets = otherAssets;

                if (scriptPaths.Count > 0)
                {
                    string assemblyPath = Path.Combine(GetTempModDirPath(), fileName.Replace("dfmod", "dll.bytes"));

                    if (!ModAssemblyBuilder.Compile(assemblyPath, scriptPaths.ToArray()))
                        return false;

                    string outputAssetPath = assemblyPath.Substring(assemblyPath.LastIndexOf("Assets"));
                    AssetDatabase.ImportAsset(outputAssetPath);
                    assets.Add(outputAssetPath);
                }
            }

            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = fileName;
            buildMap[0].assetBundleVariant = "";       //TODO

            var tempAssetPaths = new List<string>(assets);

#if LOG_BUILD_TIME
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
#endif

            AssetDatabase.StartAssetEditing();

            try
            {
                for (int i = 0; i < tempAssetPaths.Count; i++)
                {
                    string filePath = tempAssetPaths[i];

                    EditorUtility.DisplayProgressBar(modBuilderLabel, filePath, (float)i / tempAssetPaths.Count);

                    if (!File.Exists(filePath))
                    {
                        Debug.LogError("Asset not found: " + filePath);
                        return false;
                    }
                    
                    if (filePath.EndsWith(".cs", StringComparison.Ordinal))
                    {
                        // Create a copy of C# script as a .txt text asset
                        string assetPath = CopyAsset<TextAsset>(filePath, ".txt");
                        if (assetPath == null)
                            return false;

                        tempAssetPaths[i] = assetPath;
                    }
                    else if (filePath.EndsWith(".prefab", StringComparison.Ordinal))
                    {
                        // Serialize custom components
                        // Will create a prefab copy without custom components if needed
                        var prefabCopy = CheckForImportedComponents(filePath);
                        if (prefabCopy.HasValue)
                        {
                            tempAssetPaths[i] = prefabCopy.Value.prefabPath;
                            tempAssetPaths.Add(prefabCopy.Value.dataPath);
                        }
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.StopAssetEditing();
            }

#if LOG_BUILD_TIME
            stopWatch.Stop();
            Debug.Log($"Mod Builder: elapsed {stopWatch.ElapsedMilliseconds} milliseconds to prepare assets.");
#endif

            buildMap[0].assetNames = tempAssetPaths.ToArray();

            //build for every target in buildTarget array
            for (int i = 0; i < buildTargets.Length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar(modBuilderLabel, $"Building for {buildTargets[i]}.", (float)i / buildTargets.Length))
                    return false;

                if (buildTargetsToggles[i] == false) { continue;  }

                string fullPath = Path.Combine(modOutPutPath, buildTargets[i].ToString());
                Directory.CreateDirectory(fullPath);
                BuildPipeline.BuildAssetBundles(fullPath, buildMap, ToBuildAssetBundleOptions(compressionOption), buildTargets[i]);
            }

            EditorUtility.ClearProgressBar();
            return true;
        }

        string CopyAsset<T>(string path, string suffix = "") where T : UnityEngine.Object
        {
            string fileName = Path.GetFileName(path) + suffix;
            string newFilePath = Path.Combine(GetTempModDirPath(modInfo.ModTitle), fileName);
            string newAssetPath = GetAssetPathFromFilePath(newFilePath);

            if (!AssetDatabase.CopyAsset(path, newAssetPath))
            {
                Debug.LogError("Failed to Copy asset: " + path);
                return null;
            }

            return newAssetPath;
        }


        static string GetAssetPathFromFilePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                Debug.Log("Invalid string, can't get AssetPath.");
                return null;
            }

            int index = fullPath.LastIndexOf("Assets", StringComparison.Ordinal);

            if (index == -1)
            {
                Debug.LogError($"Invalid string: {fullPath}, can't get AssetPath.");
                return null;
            }

            return FixSeperatorCharacters(fullPath.Substring(index));
        }

        /// <summary>
        /// Get the path to directory where temp. files are stored for building process
        /// </summary>
        /// <returns></returns>
        static string GetTempModDirPath(string name = "")
        {
            string path = Path.Combine(Application.dataPath, "Untracked");
            path = Path.Combine(path, "ModBuilder");
            path = Path.Combine(path, name);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return FixSeperatorCharacters(path);
        }

        static string FixSeperatorCharacters(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            path = path.Replace('\\', '/');
            return path;

        }

        /// <summary>
        /// Checks if a prefab has custom components that need to be deserialized by mod manager.
        /// Custom components are removed and edited prefab is saved to a temp location.
        /// </summary>
        /// <param name="prefabPath">Asset path of original prefab.</param>
        /// <returns>Paths of prefab copy and json data; null if not copied./returns>
        private (string prefabPath, string dataPath)? CheckForImportedComponents(string prefabPath)
        {
            var go = PrefabUtility.LoadPrefabContents(prefabPath);

            try
            {
                string tempModPath = GetTempModDirPath(modInfo.ModTitle);
                string importedComponentsPath = ImportedComponentAttribute.Save(go, tempModPath);
                if (importedComponentsPath != null)
                {
                    string tempPrefabPath = GetAssetPathFromFilePath($"{tempModPath}/{go.name}.prefab");
                    PrefabUtility.SaveAsPrefabAsset(go, tempPrefabPath);                    
                    string tempDataPath = GetAssetPathFromFilePath(importedComponentsPath);
                    return (tempPrefabPath, tempDataPath);
                }

                return null;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(go);
            }
        }

        private static BuildAssetBundleOptions ToBuildAssetBundleOptions(ModCompressionOptions value)
        {
            switch(value)
            {
                case ModCompressionOptions.LZ4:
                    return BuildAssetBundleOptions.ChunkBasedCompression;
                case ModCompressionOptions.Uncompressed:
                    return BuildAssetBundleOptions.UncompressedAssetBundle;
                default:
                    return BuildAssetBundleOptions.None;
            }
        }

        private static bool IsSupportedEditorVersion()
        {
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
            return Application.unityVersion.Equals(VersionInfo.BaselineUnityVersion, StringComparison.Ordinal);
#elif UNITY_EDITOR_LINUX
            return true;
#else
            return false;
#endif
        }
    }
}
