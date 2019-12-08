// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

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

        //asset bundles will be created for any targets here
        BuildTarget[] buildTargets = new BuildTarget[]
        {
            BuildTarget.StandaloneWindows,
            BuildTarget.StandaloneOSX,
            BuildTarget.StandaloneLinux,
        };

        bool[] buildTargetsToggles = new bool[] {true, true, true};
        ModCompressionOptions compressionOption = ModCompressionOptions.LZ4;
        bool ModInfoReady { get { return ModInfoReadyTowrite(); } }
        List<string> Assets { get { return modInfo.Files; } set { modInfo.Files = value; } }         //list of assets to be added
        GUIStyle titleStyle = new GUIStyle();
        GUIStyle fieldStyle = new GUIStyle();
        GUIContent documentationGUIContent;

        void OnEnable()
        {
            if (EditorPrefs.HasKey("modOutPutPath"))
                modOutPutPath = EditorPrefs.GetString("modOutPutPath", GetTempModDirPath());
            else
                modOutPutPath = GetTempModDirPath();
            if (EditorPrefs.HasKey("lastModFile"))
                currentFilePath = EditorPrefs.GetString("lastModFile");

            modInfo = ReadModInfoFile(currentFilePath);
            titleStyle.fontSize = 15;
            fieldStyle.fontSize = 12;
            minSize = new Vector2(1280, 600);

            documentationGUIContent = new GUIContent(EditorGUIUtility.IconContent("_Help"));
            documentationGUIContent.text = " Mod System Documentation";
        }

        void OnDisable()
        {
            EditorPrefs.SetString("modOutPutPath", modOutPutPath);
            EditorPrefs.SetString("lastModFile", currentFilePath);
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
            catch(System.Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ModInfo();
            }

            fileOpen = true;
            return info;
        }


        void OnGUI()
        {
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

            GUILayout.Space(100);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

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
        }


        bool SaveModFile(bool supressWindow = false)
        {
            ModManager.SeekModContributes(modInfo);

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

            var selection = Selection.GetFiltered(typeof(object), SelectionMode.Deep);
            if (selection == null)
            {
                Debug.Log("selection null");
                return;
            }

            for (int i = 0; i < selection.Length; i++)
            {
                string path = FixSeperatorCharacters(AssetDatabase.GetAssetPath(selection[i].GetInstanceID()));
                Debug.Log("Adding asset at: " + path);

                if (File.Exists(path))
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
                else
                    Debug.LogWarning("Asset not found for: " + path);

                if (!Assets.Contains(path))
                    AddAssetToMod(path);
            }
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

            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = modFilePath.Substring(modFilePath.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
            buildMap[0].assetBundleVariant = "";       //TODO
            AddAssetToMod(GetAssetPathFromFilePath(currentFilePath));
            List<string> tempAssetPaths = new List<string>(Assets);

            for (int i = 0; i < tempAssetPaths.Count; i++ )
            {
                string filePath =  Assets[i];

                if(!File.Exists(filePath))
                {
                    Debug.LogError("Asset not found: " + filePath);
                    return false;
                }
                else
                {
                    Debug.Log("Adding Asset: " + filePath);
                }
                //replace c# file with text asset
                if (filePath.ToLower().EndsWith(".cs"))
                {
                    filePath = CopyAsset<TextAsset>(filePath, ".txt");
                    tempAssetPaths[i] = GetAssetPathFromFilePath(filePath);
                }
                else if (filePath.ToLower().EndsWith(".prefab"))
                {
                    string assetPath = CopyAsset<GameObject>(filePath); //create a copy of prefab in temp. directory
                    if(assetPath == null)
                    {
                        Debug.LogError("Failed to duplicate prefab: " + assetPath);
                        continue;
                    }
                    else
                    {
                        tempAssetPaths[i] = assetPath;                  //replace path to original prefab w/ path to copy
                    }

                    string importedComponentsPath = CheckForImportedComponents(assetPath);
                    if (importedComponentsPath != null)
                    {
                        if (!tempAssetPaths.Contains(importedComponentsPath))
                            tempAssetPaths.Add(importedComponentsPath);
                    }
                }
            }

            buildMap[0].assetNames = tempAssetPaths.ToArray();

            //build for every target in buildTarget array
            for (int i = 0; i < buildTargets.Length; i++)
            {
                if (buildTargetsToggles[i] == false) { continue;  }

                string fullPath = Path.Combine(modOutPutPath, buildTargets[i].ToString());
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
                BuildPipeline.BuildAssetBundles(fullPath, buildMap, ToBuildAssetBundleOptions(compressionOption), buildTargets[i]);
            }
            return true;
        }

        string CopyAsset<T>(string path, string suffix = "") where T : UnityEngine.Object
        {
            T oldAsset = AssetDatabase.LoadAssetAtPath<T>(path);

            if(oldAsset == null)
            {
                Debug.LogError("Failed to load asset: " + path);
            }

            string name = path.Substring(path.LastIndexOfAny(new char[] { '\\', '/' }) + 1) + suffix;
            string newPath = Path.Combine(GetTempModDirPath(modInfo.ModTitle), name);
            newPath = GetAssetPathFromFilePath(newPath);

            if (!AssetDatabase.CopyAsset(path, newPath))
            {
                Debug.LogError("Failed to Copy asset: " + path);
                return null;
            }

            return newPath;
        } 


        static string GetAssetPathFromFilePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                Debug.Log("Invalid string, can't get AssetPath");
                return null;
            }

            int index = fullPath.LastIndexOf("Assets");

            if (index == -1)
            {
                Debug.Log(string.Format("Invalid string: {0}, can't get AssetPath", fullPath));
                return null;
            }
            else
            {
                Debug.Log(string.Format("index: {0} full path: {1} Asset Path: {2} ", index, fullPath, fullPath.Substring(index)));
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

        private string CheckForImportedComponents(string prefabPath)
        {
            var go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
            string importedComponentsPath = ImportedComponentAttribute.Save(go, GetTempModDirPath(modInfo.ModTitle));
            if (importedComponentsPath != null)
            {
                importedComponentsPath = GetAssetPathFromFilePath(importedComponentsPath);
                AddAssetToMod(importedComponentsPath);
                AssetDatabase.Refresh();
                return importedComponentsPath;
            }

            return null;
        }

        private static UnityEditor.BuildAssetBundleOptions ToBuildAssetBundleOptions(ModCompressionOptions value)
        {
            switch(value)
            {
                case ModCompressionOptions.LZ4:
                    return UnityEditor.BuildAssetBundleOptions.ChunkBasedCompression;
                case ModCompressionOptions.Uncompressed:
                    return UnityEditor.BuildAssetBundleOptions.UncompressedAssetBundle;
                default:
                    return UnityEditor.BuildAssetBundleOptions.None;
            }
        }
    }
}
