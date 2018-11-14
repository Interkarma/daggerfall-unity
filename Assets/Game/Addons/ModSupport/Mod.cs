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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Utility.ModSupport
{
    [Serializable]
    public class Mod
    {
        private bool isReady = false;
        private bool enabled = true;
        private int loadPriorty;
        private ModInfo modInfo;
        private AssetBundle assetBundle;                             //.dfmod file
        private string dirPath;                                      //directory the mod file is in
        private string fileName;
        private string[] assetNames;
        private List<Source> sources;                                //any source code found in asset bundle
        private List<System.Reflection.Assembly> assemblies;         //compiled source code for this mod
        private Dictionary<string, LoadedAsset> loadedAssets;
        private DFModMessageReceiver messageReceiver;
        private Table textdatabase;
        private bool textdatabaseLoaded;

        #region properties
        [SerializeField]
        public string FileName { get { return fileName; } private set { fileName = value; } }
        [SerializeField]
        public string Title { get { return ModInfo.ModTitle; } private set { ModInfo.ModTitle = value; } }

        public bool IsReady
        {
            get { return isReady; }
            set { isReady = value; }
        }
        [SerializeField]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        [SerializeField]
        public int LoadPriority
        {
            get { return loadPriorty; }
            set { loadPriorty = value; }
        }

        public ModInfo ModInfo
        {
            get { return modInfo; }
            private set { modInfo = value; }
        }

        public AssetBundle AssetBundle
        {
            get { return assetBundle; }
            private set { assetBundle = value; }
        }

        public string DirPath
        {
            get { return dirPath; }
            private set { dirPath = value; }
        }

        public string GUID
        {
            get { return (modInfo != null) ? modInfo.GUID : "invalid"; }
        }

        public DFModMessageReceiver MessageReceiver
        {
            get { return messageReceiver; }
            set { messageReceiver = value; }
        }

        public bool HasSettings { get; set; }

        public string[] AssetNames { get { return (assetNames != null) ? assetNames : assetNames = GetAllAssetNames(); } }

        public IHasModSaveData SaveDataInterface { internal get; set; }


        #endregion

        #region constructors

        public Mod()
        {
            if (modInfo == null)
            {
                modInfo = new ModInfo();
            }
        }

        public Mod(string name, string dirPath, AssetBundle ab)
        {
            loadedAssets = new Dictionary<string, LoadedAsset>();
            assemblies = new List<System.Reflection.Assembly>(1);
            sources = new List<Source>(5);

            this.AssetBundle = ab;
            if (ab != null)
            {
                this.assetNames = GetAllAssetNames();
            }
            else
            {
                Debug.LogError("asset bundle is null for mod: " + name);
            }
            if(!LoadModInfoFromBundle())
            {
                Debug.LogError("Couldn't locate modinfo for mod: " + name);
                modInfo = new ModInfo();
            }

            this.FileName = name;
            this.dirPath = dirPath;
            this.LoadSourceCodeFromModBundle();
            this.HasSettings = ModSettings.ModSettingsData.HasSettings(this);
#if DEBUG
            Debug.Log(string.Format("Finished Mod setup: {0}",this.Title));
#endif
        }

        #endregion

        /// <summary>
        /// Will retrieve asset either from LoadedAssets or asset bundle and return it. Will load
        /// asset bundle if necessary.
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">name of asset</param>
        /// <param name="loadedBundle">had to load asset bundle</param>
        /// <returns></returns>
        private T LoadAssetFromBundle<T>(string assetName, out bool loadedBundle) where T : UnityEngine.Object
        {
            LoadedAsset la = new LoadedAsset();
            loadedBundle = false;

            if (string.IsNullOrEmpty(assetName))
            {
                return null;
            }
            try
            {
                assetName = ModManager.GetAssetName(assetName);

                if (IsAssetLoaded(assetName))
                {
                    la = loadedAssets[assetName];
                    return la.Obj as T;
                }
                if (assetBundle == null)
                    loadedBundle = LoadAssetBundle();

                if (assetBundle.Contains(assetName))
                {
                    la.Obj = assetBundle.LoadAsset<T>(assetName);

                    if(la.Obj != null)
                    {
                        la.T = la.Obj.GetType();
                        AddAsset(assetName, la);
                    }
                    return la.Obj as T;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }

        }

        #region Public Methods

        public T GetAsset<T>(string assetname, bool clone = false) where T : UnityEngine.Object
        {
            bool loadedBundle;
            return GetAsset<T>(assetname, out loadedBundle, clone);
        }

        /// <summary>
        /// Load asset from bundle and return
        /// </summary>
        /// <typeparam name="T">Type of Asset</typeparam>
        /// <param name="assetName">name of asset to load</param>
        /// <param name="loadedBundle">true if assetbundle had to be loaded</param>
        /// <param name="clone">create copy of asset if true</param>
        /// <returns></returns>
        public T GetAsset<T>(string assetName, out bool loadedBundle, bool clone = false) where T : UnityEngine.Object
        {
            loadedBundle = false;

            T asset = LoadAssetFromBundle<T>(assetName, out loadedBundle);

            if(asset == null)
            {
                Debug.LogWarning(string.Format("Failed to load asset: {0}", assetName));
                return null;
            }
            else if(clone)
            {
                return UnityEngine.Object.Instantiate(asset) as T;
            }
            else
                return asset as T;
        }

        /// <summary>
        /// Loads all assets from asset bundle immediatly.
        /// </summary>
        /// <param name="UnloadBundle">Unload asset bundle if true</param>
        /// <returns></returns>
        public bool LoadAllAssetsFromBundle(bool UnloadBundle = true)
        {
            if (AssetNames == null)
                return false;
            try
            {
                for (int i = 0; i < AssetNames.Length; i++)
                {
                    string assetname = ModManager.GetAssetName(AssetNames[i]);

                    if (IsAssetLoaded(assetname))
                        continue;

                    if (assetBundle == null)
                    {
                        if (!LoadAssetBundle())
                            return false;
                    }

                    LoadedAsset la = new LoadedAsset();
                    la.Obj = assetBundle.LoadAsset(assetname);

                    if(la.Obj == null)
                    {
                        Debug.LogWarning(string.Format("failed to load asset: {0} for mod: {1}", AssetNames[i], Title));
                        continue;
                    }

                    la.T = la.Obj.GetType();
                    AddAsset(assetname, la);
                }

                if(UnloadBundle)
                    UnloadAssetBundle(false);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Load all assets from asset bundle asynchronously.
        /// </summary>
        /// <param name="unloadBundle">Unload asset bundle if true</param>
        /// <returns></returns>
        public IEnumerator LoadAllAssetsFromBundleAsync(bool unloadBundle = true)
        {
            if (AssetNames == null)
                yield return null;

            for(int i = 0; i < AssetNames.Length; i++)
            {
                string assetname = ModManager.GetAssetName(AssetNames[i]);

                if (IsAssetLoaded(assetname))
                    continue;

                if (assetBundle == null)
                {
                    yield return LoadAssetBundleAsync();
                    if (assetBundle == null)
                        yield break;
                }

                AssetBundleRequest request = assetBundle.LoadAssetAsync(assetname);
                yield return request;

                if (request.asset == null)
                {
                    Debug.LogWarning(string.Format("failed to load asset: {0} for mod: {1}", AssetNames[i], Title));
                    continue;
                }

                AddAsset(assetname, request.asset);
            }
            if(unloadBundle)
                UnloadAssetBundle(false);

            yield return null;
        }

        /// <summary>
        /// Return Type from Assemblies using name of type
        /// </summary>
        /// <param name="type">name of Type</param>
        /// <returns>System.Type</returns>
        public Type GetCompiledType(string type)
        {
            Type t = null;

            if (string.IsNullOrEmpty(type))
                return null;

            for (int i = 0; i < assemblies.Count; i++)
            {
                if (assemblies[i] == null)
                    continue;

                t = assemblies[i].GetType(type);
                if (t != null)
                    return t;
            }
            return t;
        }

        /// <summary>
        /// Imports settings for this mod and provides a sanitized read-only access.
        /// </summary>
        public ModSettings.ModSettings GetSettings()
        {
            return new ModSettings.ModSettings(this);
        }

        /// <summary>
        /// Gets a localized string from the text table associated with this mod.
        /// </summary>
        /// <param name="key">Key used in the text table.</param>
        /// <returns>Localized string.</returns>
        public string Localize(string key)
        {
            return TryLocalize(key) ?? string.Format("{0} - MissingText", Title);
        }

        /// <summary>
        /// Gets a localized string from the text table associated with this mod.
        /// </summary>
        /// <param name="keyParts">Key used in the text table as a concatenation of names.</param>
        /// <returns>Localized string.</returns>
        public string Localize(params string[] keyParts)
        {
            return Localize(string.Join(".", keyParts));
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets a localized string from the text table associated with this mod.
        /// </summary>
        /// <param name="key">Key used in the text table.</param>
        /// <returns>Localized string or null.</returns>
        internal string TryLocalize(string key)
        {
            // Read from StreamingAssets/Text
            string databaseName = string.Format("mod_{0}", fileName);
            if (TextManager.Instance.HasText(databaseName, key))
                return TextManager.Instance.GetText(databaseName, key);

            // Get fallback table from mod
            if (!textdatabaseLoaded)
            {
                if (assetBundle.Contains("textdatabase.txt"))
                    textdatabase = new Table(GetAsset<TextAsset>("textdatabase.txt").ToString());
                textdatabaseLoaded = true;
            }

            return textdatabase != null && textdatabase.HasValue(key) ?
                textdatabase.GetValue("text", key) : null;
        }

        /// <summary>
        /// Gets a localized string from the text table associated with this mod.
        /// </summary>
        /// <param name="keyParts">Key used in the text table as a concatenation of names.</param>
        /// <returns>Localized string or null.</returns>
        internal string TryLocalize(params string[] keyParts)
        {
            return TryLocalize(string.Join(".", keyParts));
        }

        #endregion

        #region setup

        // Returns array containing names of all assets in asset bundle
        private string[] GetAllAssetNames()
        {
            try
            {
                if (assetBundle == null)
                    return null;

                string[] assetNames = assetBundle.GetAllAssetNames();
                for (int i = 0; i < assetNames.Length; i++)
                {
                    assetNames[i] = ModManager.GetAssetName(assetNames[i]);
                }

                return assetNames;
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message);
                return null;
            }
        }

        // Loads the serialized mod info file from bundle
        private bool LoadModInfoFromBundle()
        {
            try
            {
                TextAsset modInfoAsset = null;
                for (int i = 0; i < assetNames.Length; i++)
                {
                    if (assetNames[i].EndsWith(ModManager.MODINFOEXTENSION))
                    {
                        modInfoAsset = GetAsset<TextAsset>(assetNames[i]);
                        break;
                    }
                }
                if (modInfoAsset == null)
                    return false;

                string modInfo = modInfoAsset.ToString();
                this.ModInfo = (ModInfo)JsonUtility.FromJson(modInfo, typeof(ModInfo));
                return ModInfo != null;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
        }

        // Loads all the source code found in bundle as Text Assets & adds to Sources list
        private bool LoadSourceCodeFromModBundle()
        {
            try
            {
                if (sources == null)
                    sources = new List<Source>();

                string[] assetNames = assetBundle.GetAllAssetNames();

                string name = null;
                foreach (string assetName in assetNames)
                {
                    name = assetName.ToLower();

                    if (name.EndsWith(".cs.txt") || name.EndsWith(".cs"))// || name.EndsWith(".byte")) //.byte is for .dll - not tested yet
                    {
                        Source newUncompiledSource;
                        TextAsset newSource = GetAsset<TextAsset>(assetName);

                        if (newSource != null)
                        {
                            newUncompiledSource.sourceTxt = newSource;
                            newUncompiledSource.isPreCompiled = name.EndsWith(".dll");
                            sources.Add(newUncompiledSource);
                        }
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Compiles all source files to assembly
        /// </summary>
        /// <returns></returns>
        public List<Assembly> CompileSourceToAssemblies()
        {
            List<string> stringSource   = new List<string>(sources.Count);
            Assembly assembly           = null;

            try
            {
                for (int i = 0; i < sources.Count; i++)
                {
                    if (sources[i].isPreCompiled)
                    {
                        assembly = Assembly.Load(sources[i].sourceTxt.bytes);
                        if (assembly != null)
                            assemblies.Add(assembly);
                    }
                    else
                    {
                        stringSource.Add(sources[i].sourceTxt.ToString());
                    }
                }
                if (stringSource.Count > 0)
                {
                    assembly = ModManager.CompileFromSourceAssets(stringSource.ToArray());

                    if (assembly != null)
                        assemblies.Add(assembly);
                }

                stringSource    = null;
                sources         = null;
                return assemblies;
            }
            catch(Exception ex)
            {
                Debug.Log(ex);
                return null;
            }

        }

        /// <summary>
        /// Returns a list of any valid mod setup functions
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public List<SetupOptions> FindModLoaders(StateManager.StateTypes state)
        {
            if (assemblies == null || assemblies.Count < 1)
                return null;

            List<SetupOptions> modLoaders = new List<SetupOptions>(1);

            for(int i = 0; i < assemblies.Count; i++)
            {
                try
                {
                    Type[] types = assemblies[i].GetTypes();

                    foreach(Type t in types)
                    {
                        if (!t.IsClass)
                            continue;

                        foreach(MethodInfo mi in t.GetMethods())
                        {
                            if (!mi.IsPublic || !mi.IsStatic)
                                continue;
                            else if (mi.ContainsGenericParameters)
                                continue;

                            Invoke initAttribute = (Invoke)Attribute.GetCustomAttribute(mi, typeof(Invoke));
                            if (initAttribute == null)
                                continue;
                            else if (initAttribute.startState != state)
                                continue;
                            ParameterInfo[] pi = mi.GetParameters();
                            if (pi.Length != 1)
                                continue;
                            else if (pi[0].ParameterType != typeof(InitParams))
                                continue;
                            SetupOptions options = new SetupOptions(initAttribute.priority, this, mi);
#if DEBUG
                            Debug.Log(string.Format("found new loader: {0} for mod: {1}", options.mi.Name, this.Title));
#endif
                            modLoaders.Add(options);

                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogError(ex.Message);
                    continue;
                }
            }

            modLoaders.Sort();
            return modLoaders;
        }

        public bool IsAssetLoaded(string AssetName)
        {
            return loadedAssets.ContainsKey(AssetName);
        }


        private bool AddAsset(string assetName, UnityEngine.Object asset)
        {
            if (asset == null)
                return false;

            LoadedAsset la = new LoadedAsset(asset.GetType(), asset);
            return AddAsset(assetName, la);
        }

        private bool AddAsset(string assetName, LoadedAsset la)
        {
            if (IsAssetLoaded(assetName))
                return false;
            else if (la.Obj == null || la.T == null)
                return false;
            else
            {
                if (la.T == typeof(GameObject) && assetBundle.Contains(ImportedComponentAttribute.MakeFileName(assetName)))
                    ImportedComponentAttribute.Restore(this, la.Obj as GameObject);

                loadedAssets.Add(assetName, la);

                if (this.modInfo != null && string.IsNullOrEmpty(this.Title) == false)
                    ModManager.OnLoadAsset(this.Title, assetName, la.T);
#if DEBUG
                Debug.Log(string.Format("added asset: {0}", assetName));
#endif
                return true;
            }

        }

        public void UnloadAssetBundle(bool unloadAllObjects)
        {
            if (assetBundle == null)
                return;

            assetBundle.Unload(unloadAllObjects);
#if DEBUG
            Debug.Log(string.Format("Unloaded asset bundle for mod: {0}", Title));
#endif
        }

        public AssetBundle LoadAssetBundle()
        {
            string abPath = System.IO.Path.Combine(dirPath, FileName + ModManager.MODEXTENSION);
            if (!System.IO.File.Exists(abPath))
                return null;

            AssetBundle ab = AssetBundle.LoadFromFile(abPath);
            if (ab != null)
                this.assetBundle = ab;
#if DEBUG
            Debug.Log(string.Format("Loaded asset bundle for mod: {0}", Title));
#endif
            return ab;
        }

        public IEnumerator LoadAssetBundleAsync()
        {
            string abPath = System.IO.Path.Combine(dirPath, FileName + ModManager.MODEXTENSION);
            if (!System.IO.File.Exists(abPath))
                yield break;

            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(abPath);
            yield return request;

            if (request.assetBundle != null)
                assetBundle = request.assetBundle;
#if DEBUG
            Debug.Log(string.Format("Loaded asset bundle for mod: {0}", Title));
#endif
            yield return request.assetBundle;
        }

        #endregion

        /// <summary>
        /// Uses serialized data from asset bundle to setup prefab
        /// </summary>
        /// <param name="prefabName">name of prefab asset</param>
        /// <param name="serializedDataName">name of serialized data txt asset</param>
        /// <returns></returns>
        public GameObject SetupPrefabHelper(string prefabName, string serializedDataName = null)
        {
            GameObject prefab = GetAsset<GameObject>(prefabName, false);

            if(prefab == null)
            {
                if (!prefabName.EndsWith(".prefab"))
                {
                    prefabName = prefabName + ".prefab";
                    prefab = GetAsset<GameObject>(prefabName);
                }
                if (prefab == null)
                {
                    Debug.LogError(string.Format("Failed to locate prefab in mod file: {0} {1}", this.Title, prefabName));
                    return null;
                }
            }

            //if serializedDataName is null, defaults to checking for prefabName.serialized.prefab.txt
            if (string.IsNullOrEmpty(serializedDataName))
                serializedDataName = prefabName.Substring(0, prefabName.Length-7) + ".serialized" + ".prefab" + ".txt";

            string serializedData = "";

            try
            {
                serializedData = GetAsset<TextAsset>(serializedDataName).text;
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Failed to load serializedData: {0} {1} {2} {3}", this.Title, prefabName, serializedDataName, ex.Message));
                return null;
            }

            if (string.IsNullOrEmpty(serializedData))
            {
                Debug.LogError(string.Format("Failed to locate serialized data for prefab in mod file: {0} {1} {2}", this.Title, prefabName, serializedDataName));
                return null;
            }

            object deserialized = null;
            FullSerializer.fsData data = FullSerializer.fsJsonParser.Parse(serializedData);
            ModManager._serializer.TryDeserialize(data, typeof(Dictionary<string, List<SerializedRecord>>), ref deserialized).AssertSuccessWithoutWarnings();


            if (deserialized != null)
                return SetupPrefabHelper(prefab, deserialized as Dictionary<string, List<SerializedRecord>>);
            else
                return null;
        }

        /// <summary>
        /// Helper function that uses serialized data from asset bundle to setup prefab
        /// </summary>
        /// <param name="prefab">prefab object to setup</param>
        /// <param name="recordDictionary">serialized data</param>
        /// <returns></returns>
        public GameObject SetupPrefabHelper(GameObject prefab, Dictionary<string, List<SerializedRecord>> recordDictionary)
        {
            if (prefab == null || recordDictionary == null)
            {
                Debug.LogError("Failed to setup prefab - either the prefab or the deserialized dictionary was null - stopping");
                return null;
            }

            List<Transform> transforms = new List<Transform>();
            ModManager.GetAllChildren(prefab.transform, ref transforms);

            for (int i = 0; i < transforms.Count; i++)
            {
                GameObject go = transforms[i].gameObject;

                if (recordDictionary.ContainsKey(go.name))
                {
                    List<SerializedRecord> records = recordDictionary[go.name];

                    for (int j = 0; j < records.Count; j++)
                    {
                        SerializedRecord sr = records[j];
                        Component co = go.AddComponent(sr.componentType);
                        Idfmod_Serializable isCustomSerializable = co as Idfmod_Serializable;

                        if (isCustomSerializable == null)
                            continue;
                        if (sr.serializedObjects == null || sr.serializedObjects.Length < 1)
                            continue;

                        isCustomSerializable.Deseralized(sr.serializedObjects);
                    }
                }
            }
            return prefab;

        }


    }

}
