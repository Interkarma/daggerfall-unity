// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
using SimpleJson;


/*
 * Todo:
 * 1. append .byte to .dll files
 * 2. Support for asset bundle variants
 * 3. Fix bug where multiple paths get added if they use different / \
 */

namespace DaggerfallWorkshop.Game.Utility.ModSupport
{
    [Serializable]
    public class CreateModEditorWindow : EditorWindow
    {

        const string windowTitle = "Daggerfall Unity Mod Builder";
        const string menuPath = "Daggerfall Tools/Mod Builder";

        string currentFilePath = "";
        string modOutPutPath = "";

        [SerializeField]
        public string TempDirectory = "ModBuilder";

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
        BuildTarget.StandaloneOSXUniversal,
        BuildTarget.StandaloneLinux,
        };

        bool ModInfoReady { get { return ModInfoReadyTowrite(); } }
        List<string> Assets { get { return modInfo.Files; } set { modInfo.Files = value; } }         //list of assets to be added

        void OnEnable()
        {
            if (!fileOpen)
                modInfo = new ModInfo();
        }


        private bool ModInfoReadyTowrite()
        {
            if (!fileOpen)
                return false;
            else if (this.modInfo == null)
                return false;
            else if (string.IsNullOrEmpty(this.modInfo.ModFileName))
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

        void OnGUI()
        {
            EditorGUI.indentLevel++;

            GUILayoutHelper.Horizontal(() =>
            {
                if (GUILayout.Button("Create New Mod"))
                {
                    fileOpen = true;
                    if(modInfo != null)
                    {
                        modInfo.DFUnity_Verion = VersionInfo.DaggerfallUnityVersion;
                    }
                }

                else if (GUILayout.Button("Open Existing Mod Settings File"))
                {
                    try
                    {
                        currentFilePath = EditorUtility.OpenFilePanelWithFilters("", Application.dataPath, new string[] { "JSON", "dfmod.json"});

                        if (!File.Exists(currentFilePath))
                        {
                            Debug.Log("Invalid file selection");
                            currentFilePath = null;
                            fileOpen = false;
                            return;
                        }

                        string inPut = File.ReadAllText(currentFilePath);
                        modInfo = (ModInfo)JsonUtility.FromJson(inPut, typeof(ModInfo));

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
            });

            if(modInfo == null)
            {
                fileOpen = false;
                modInfo = new ModInfo();
            }
            if (!fileOpen) // if no fileopen, hide rest of UI
                return;

            GUILayoutHelper.Vertical(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Mod File Path:" + currentFilePath));
                EditorGUI.indentLevel++;

                GUILayoutHelper.Vertical(() =>
                {
                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Mod File Name:"));
                        modInfo.ModFileName = EditorGUILayout.TextField(modInfo.ModFileName.ToLower(), GUILayout.MinWidth(600));
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Mod Title:"));
                        modInfo.ModTitle = EditorGUILayout.TextField(modInfo.ModTitle, GUILayout.MinWidth(600));
                    });

                    EditorGUILayout.Space();


                    GUILayoutHelper.Horizontal(() =>
                    {

                        EditorGUILayout.LabelField(new GUIContent("Mod Version:"));
                        modInfo.ModVersion = EditorGUILayout.TextField(modInfo.ModVersion, GUILayout.MinWidth(600));
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Mod Creator:"));
                        modInfo.ModAuthor = EditorGUILayout.TextField(modInfo.ModAuthor, GUILayout.MinWidth(600));
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Contact Info:"));
                        modInfo.ContactInfo = EditorGUILayout.TextField(modInfo.ContactInfo, GUILayout.MinWidth(600));
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("DFUnity Version:"));
                        modInfo.DFUnity_Verion = EditorGUILayout.TextField(modInfo.DFUnity_Verion, GUILayout.MinWidth(600));
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Mod Description:"));
                        modInfo.ModDescription = EditorGUILayout.TextArea(modInfo.ModDescription, GUILayout.MinWidth(600));
                    });

                });

                EditorGUILayout.Space();
                EditorGUI.indentLevel--;

                GUILayoutHelper.Horizontal(() =>
                {
                    if (GUILayout.Button("Add Selected Asset(s)"))
                        AddAssetToMod();

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

            GUILayoutHelper.Horizontal(() =>
            {

                if (GUILayout.Button("Save Mod Settings to File"))
                {
                    SaveModFile();
                    Debug.Log("got path: " + currentFilePath);
                }
                else if (GUILayout.Button("Build Mod"))
                {
                    SaveModFile();
                    BuildMod();
                }

            });
        }


        //create mod info json text asset, add path to file list
        bool SaveModFile()
        {
            try
            {
                if (!ModInfoReady)
                {
                    Debug.LogError("Error in mod builder - couldn't save mod file");
                    return false;
                }

                string path = GetTempModDirPath();
                string outPut = JsonUtility.ToJson(modInfo, true);

                path = Path.Combine(path, modInfo.ModFileName + ModManager.MODINFOEXTENSION);

                Debug.Log("writing to: " + path);
                File.WriteAllText(path, outPut);
                AssetDatabase.Refresh();

                while (Assets.FindAll(x => x.EndsWith(ModManager.MODINFOEXTENSION)).Count > 0)
                {
                    for (int i = 0; i < Assets.Count; i++ )
                    {
                        if (Assets[i].EndsWith(ModManager.MODINFOEXTENSION))
                            Assets.RemoveAt(i);
                    }
                        //Assets.RemoveAt(Assets.LastIndexOf(path));
                }

                //get Asset path
                path = GetAssetPathFromFilePath(path);
                Assets.Add(path);

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log("error saving mod file: " + ex.Message);
                return false;
            }
        }

        //Add selected assets in editor to File list
        void AddAssetToMod()
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
                string path = AssetDatabase.GetAssetPath(selection[i].GetInstanceID());
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
                    Assets.Add(path);
            }
        }


        //Builds the actual asset bundle.  Only builds if files added & required information set in mod info fields
        bool BuildMod()
        {
            if (!ModInfoReady)
                return false;

            //get destination for mod
            modOutPutPath = EditorUtility.SaveFolderPanel("Select Destination,", Application.dataPath, "");

            if (!Directory.Exists(modOutPutPath))
            {
                return false;
            }
            else if (Assets == null || Assets.Count < 0)
            {
                return false;
            }

            //refresh
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = modInfo.ModFileName + ".dfmod";
            buildMap[0].assetBundleVariant = "";                                //TODO
            buildMap[0].assetNames = Assets.ToArray();

            for (int i = 0; i < buildMap[0].assetNames.Length; i++ )
            {
                string filePath = buildMap[0].assetNames[i];
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
                    filePath = CreateNewTextAssetFromScript(filePath);
                    buildMap[0].assetNames[i] = GetAssetPathFromFilePath(filePath);
                    Debug.Log(string.Format("{0} {1}", i, filePath));
                }
            }

            //build for every target in buildTarget array
            for (int i = 0; i < buildTargets.Length; i++)
            {
                string fullPath = Path.Combine(modOutPutPath, buildTargets[i].ToString());
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                BuildPipeline.BuildAssetBundles(fullPath, buildMap, BuildAssetBundleOptions.CompleteAssets, buildTargets[i]);
            }

            return true;
        }


        string CreateNewTextAssetFromScript(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            if (!File.Exists(filePath))
                return null;
            string name = filePath.Substring(filePath.LastIndexOfAny(new char[]{'\\', '/'}) + 1) + ".txt";

            Debug.Log("file name: " + name);

            string newPath = Path.Combine(GetTempModDirPath(), name);

            string content = File.ReadAllText(filePath);
            File.WriteAllText(newPath, content, System.Text.Encoding.UTF8);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return newPath;
        }


        string GetAssetPathFromFilePath(string fullPath)
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
            return fullPath.Substring(index);
        }

        string GetTempModDirPath()
        {
            string path = Path.Combine(Application.dataPath, TempDirectory);
            if (Directory.Exists(path))
                return path;
            else
            {
                path = AssetDatabase.CreateFolder("Assets", TempDirectory);
                path = AssetDatabase.GUIDToAssetPath(path);
                return path;
            }
        }


    }
}
