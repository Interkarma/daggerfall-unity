// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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

        const string windowTitle = "Daggerfall Unity Mod Builder";
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


        bool ModInfoReady { get { return ModInfoReadyTowrite(); } }
        List<string> Assets { get { return modInfo.Files; } set { modInfo.Files = value; } }         //list of assets to be added

        void OnEnable()
        {
            if (EditorPrefs.HasKey("modOutPutPath"))
                modOutPutPath = EditorPrefs.GetString("modOutPutPath", GetTempModDirPath());
            else
                modOutPutPath = GetTempModDirPath();
            if (EditorPrefs.HasKey("lastModFile"))
                currentFilePath = EditorPrefs.GetString("lastModFile");

            modInfo = ReadModInfoFile(currentFilePath);
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
                info = (ModInfo)JsonUtility.FromJson(inPut, typeof(ModInfo));
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
                    //currentFilePath = EditorUtility.SaveFilePanel("", GetTempModDirPath(), "", "dfmod.json");
                    if (modInfo != null)
                    {
                        modInfo.DFUnity_Version = VersionInfo.DaggerfallUnityVersion;
                    }
                }

                else if (GUILayout.Button("Open Existing Mod Settings File"))
                {
                    try
                    {
                        currentFilePath = EditorUtility.OpenFilePanelWithFilters("", GetTempModDirPath(), new string[] { "JSON", "dfmod.json"});

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
                EditorGUILayout.Space();
                GUILayoutHelper.Horizontal(() =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Current File:  " + currentFilePath));

                });
                EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

                EditorGUI.indentLevel++;

                GUILayoutHelper.Vertical(() =>
                {

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
                        modInfo.DFUnity_Version = EditorGUILayout.TextField(modInfo.DFUnity_Version, GUILayout.MinWidth(600));
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Mod Description:"));
                        modInfo.ModDescription = EditorGUILayout.TextArea(modInfo.ModDescription, GUILayout.MinWidth(600));
                    });

                    EditorGUILayout.Space();

                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Mod GUID: "));
                        EditorGUILayout.LabelField(new GUIContent(modInfo.GUID));
                        if (GUILayout.Button("Generate New GUID"))
                            modInfo.GUID = System.Guid.NewGuid().ToString();
                        //modInfo.ModDescription = EditorGUILayout.TextArea(modInfo.ModDescription, GUILayout.MinWidth(600));
                    });

                });

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

            GUILayout.Label("Build Targets");

            GUILayoutHelper.Horizontal(() =>
            {
                for (int i = 0; i < buildTargetsToggles.Length; i++)
                {
                    buildTargetsToggles[i] = EditorGUILayout.Toggle(buildTargets[i].ToString(), buildTargetsToggles[i], GUILayout.ExpandWidth(true));
                }
            });

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);



            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField("Build Path:     " + modOutPutPath);
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
            string path = currentFilePath;
            string outPut = JsonUtility.ToJson(modInfo, true);
            string directory = "";

            if (!supressWindow)
                path = EditorUtility.SaveFilePanel("Save", GetTempModDirPath(), modInfo.ModTitle, "dfmod.json");

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
            //if(modOutPutPath == null || Directory.Exists(modOutPutPath) == false
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

                    string serializedPrefabPath = SerializePrefab(assetPath); // serialize the copy
                    if (serializedPrefabPath != null)
                    {
                        if(!tempAssetPaths.Contains(serializedPrefabPath))
                            tempAssetPaths.Add(serializedPrefabPath);
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

                BuildPipeline.BuildAssetBundles(fullPath, buildMap, BuildAssetBundleOptions.None, buildTargets[i]);
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

        /// <summary>
        /// Serializes prefab & adds serialized txt asset to mod
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <returns></returns>
        private string SerializePrefab(string prefabPath)
        {
            GameObject prefabObject = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
            if (prefabObject == null)
            {
                Debug.LogWarning("Failed to load prefab: " + prefabObject);
                return null;
            }

            string path = GetTempModDirPath(modInfo.ModTitle);
            string serialized = "";

            try
            {
                if (!SerializePrefabHelper(prefabObject, out serialized))
                {
                    Debug.LogWarning("Failed to serialize prefab: " + prefabObject.name);
                    return null;
                }
            }
            catch (Exception ex) 
            {
                Debug.LogError(string.Format("Error trying to serialize prefab: {0} {1} {2}", modInfo.ModTitle, prefabPath, ex.InnerException));
                return null;
            }

            if (string.IsNullOrEmpty(serialized))
                return null;

            path = GetAssetPathFromFilePath(Path.Combine(path, prefabObject.name + ".serialized" + ".prefab" + ".txt"));
            File.WriteAllText(path, serialized);
            AddAssetToMod(path);
            AssetDatabase.Refresh();
            return path;
        }

        /// <summary>
        /// Serializes components on gameobject & children w/ Idfmod_Serializable interface
        /// WARNING: This will destroy any serialized component from prefab
        /// </summary>
        /// <param name="prefab">base prefab</param>
        /// <param name="serialized">serialized string</param>
        /// <returns></returns>
        private static bool SerializePrefabHelper(GameObject prefab, out string serialized)
        {
            serialized = "";

            Dictionary<string, List<SerializedRecord>> recordDictionary = new Dictionary<string, List<SerializedRecord>>();
            List<Transform> transforms = new List<Transform>();
            ModManager.GetAllChildren(prefab.transform, ref transforms);

            for (int i = 0; i < transforms.Count; i++)
            {
                if (transforms[i] == null)
                    continue;

                GameObject go = transforms[i].gameObject;
                List<SerializedRecord> serializedRecords = new List<SerializedRecord>();
                Component[] components = go.GetComponents<Component>();

                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j] == null)
                        continue;

                    Component co = components[j];
                    Idfmod_Serializable sModInterface = co as Idfmod_Serializable;

                    if (sModInterface == null)
                        continue;
                    else if (sModInterface.Ignore)
                        continue;

                    object[] toSerialize = sModInterface.ToSerialize();
                    if (toSerialize != null)
                    {
                        for (int k = 0; k < toSerialize.Length; k++)
                        {
                            if (toSerialize[k].GetType().IsSubclassOf(typeof(Component)))
                            {
                                Debug.LogError("Can't serialize monobehaviours: " + toSerialize[k].ToString());
                                return false;
                            }
                        }
                    }

                    SerializedRecord sr = new SerializedRecord(go.name, co, toSerialize);
                    serializedRecords.Add(sr);
                    UnityEngine.Object.DestroyImmediate(co, true);

                }

                if (serializedRecords.Count > 0)
                {
                    if (recordDictionary.ContainsKey(go.name))
                    {
                        Debug.LogWarning("Please make sure all game objects have unique names, can't serialize objects for: " + go.name);
                        continue;
                    }
                    else
                    {
                        recordDictionary.Add(go.name, serializedRecords);
                    }
                }
            }

            FullSerializer.fsData sData;
            FullSerializer.fsResult result = ModManager._serializer.TrySerialize(typeof(Dictionary<string, List<SerializedRecord>>), recordDictionary, out sData).AssertSuccessWithoutWarnings();

            serialized = FullSerializer.fsJsonPrinter.PrettyJson(sData);
            return result.Succeeded;
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
    }
}
