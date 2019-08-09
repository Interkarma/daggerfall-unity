// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    TheLacus
// 
// Notes:
//

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Utility.ModSupport
{
    [Serializable]
    public class Mod
    {
        #region Fields

        private bool isReady = false;
        private bool enabled = true;
        private int loadPriorty;
        private ModInfo modInfo;
        private AssetBundle assetBundle;                             //.dfmod file
        private string dirPath;                                      //directory the mod file is in
        private string fileName;
        private string[] assetNames;
        private List<Source> sources;                                //any source code found in asset bundle
        private List<Assembly> assemblies;                           //compiled source code for this mod
        private Dictionary<string, LoadedAsset> loadedAssets;
        private DFModMessageReceiver messageReceiver;
        private Table textdatabase;
        private bool textdatabaseLoaded;

        #endregion

        #region Properties

        /// <summary>
        /// The name of the mod file on disk without extension.
        /// </summary>
        [SerializeField]
        public string FileName
        {
            get { return fileName; }
            private set { fileName = value; }
        }

        /// <summary>
        /// The readable title of the mod, which may contain invalid path characters.
        /// </summary>
        [SerializeField]
        public string Title
        {
            get { return ModInfo.ModTitle; }
            private set { ModInfo.ModTitle = value; }
        }

        /// <summary>
        /// A value indicating whether this mod is ready; It should be set by the mod itself after initialization.
        /// </summary>
        public bool IsReady
        {
            get { return isReady; }
            set { isReady = value; }
        }

        /// <summary>
        /// If this mod is enabled from the mods window, it will be loaded by the Mod Manager
        /// and methods marked with the <see cref="Invoke"/> attribute will be called at the specified state.
        /// </summary>
        [SerializeField]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// The position in the load order, which affects the invocation order and
        /// the automatic asset loading by the Asset-Injection framework.
        /// </summary>
        [SerializeField]
        public int LoadPriority
        {
            get { return loadPriorty; }
            internal set { loadPriorty = value; }
        }

        /// <summary>
        /// Mod informations defined from the mod builder.
        /// </summary>
        public ModInfo ModInfo
        {
            get { return modInfo; }
            private set { modInfo = value; }
        }

        /// <summary>
        /// If not null, this is the assetbundle where all the assets for this mod are stored.
        /// Assets should be retrieved with <see cref="GetAsset{T}(string, bool)"/> which benefits of a cache system.
        /// </summary>
        public AssetBundle AssetBundle
        {
            get { return assetBundle; }
            private set { assetBundle = value; }
        }

        /// <summary>
        /// The directory where the mod file is stored.
        /// This is equal or a sub-directory of <see cref="ModManager.ModDirectory"/>.
        /// </summary>
        public string DirPath
        {
            get { return dirPath; }
            private set { dirPath = value; }
        }

        /// <summary>
        /// An unique identifier for this mod or <c>"invalid"</c> if not defined.
        /// </summary>
        public string GUID
        {
            get { return (modInfo != null) ? modInfo.GUID : "invalid"; }
        }

        /// <summary>
        /// An optional callback that allows to efficiently send messages to this mod without using reflections.
        /// </summary>
        public DFModMessageReceiver MessageReceiver
        {
            get { return messageReceiver; }
            set { messageReceiver = value; }
        }

        /// <summary>
        /// If this mod has settings, they can be retrieved with <see cref="GetSettings()"/>.
        /// </summary>
        public bool HasSettings { get; set; }

        /// <summary>
        /// Cached list of all asset names (not the relative paths).
        /// </summary>
        public string[] AssetNames { get { return assetNames ?? (assetNames = GetAllAssetNames()); } }

        /// <summary>
        /// An optional implementation of the interface that allows mods to take part of load/save process
        /// and store custom data associated to a specific save on disk.
        /// </summary>
        public IHasModSaveData SaveDataInterface { internal get; set; }

#if UNITY_EDITOR
        /// <summary>
        /// If true this mod is associated to a standalone manifest file rather than an assetbundle.
        /// This is only useful for mod development and testing, specifically to benefit of debuggers.
        /// </summary>
        public bool IsVirtual { get; private set; }
#endif

        #endregion

        #region Constructors

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
            if (!LoadModInfoFromBundle())
            {
                Debug.LogError("Couldn't locate modinfo for mod: " + name);
                modInfo = new ModInfo();
            }

            this.FileName = name;
            this.dirPath = dirPath;
            this.LoadSourceCodeFromModBundle();
            this.HasSettings = ModSettings.ModSettingsData.HasSettings(this);
#if DEBUG
            Debug.Log(string.Format("Finished Mod setup: {0}", this.Title));
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Makes a mod from a manifest file without an assetbundle.
        /// This is only useful for mod development and testing, specifically to benefit of debuggers.
        /// </summary>
        /// <param name="manifestPath">Full or relative path to .dfmod.json file, rooted at Mods/.</param>
        public Mod(string manifestPath)
        {
            if (!Path.IsPathRooted(manifestPath))
                manifestPath = Application.dataPath + "/Game/Mods/" + manifestPath;

            IsVirtual = true;
            modInfo = JsonUtility.FromJson<ModInfo>(File.ReadAllText(manifestPath));
            loadedAssets = new Dictionary<string, LoadedAsset>();
        }
#endif

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if this mod contains an asset with the given name.
        /// </summary>
        /// <param name="assetName">The name of the asset.</param>
        /// <returns>True if asset is provided by this mod.</returns>
        public bool HasAsset(string assetName)
        {
#if UNITY_EDITOR
            if (IsVirtual)
                return modInfo.Files.Any(CompareNameWithPath(assetName));
#endif

            if (assetBundle)
                return assetBundle.Contains(assetName);

            return false;
        }

        /// <summary>
        /// Checks if an asset has already been loaded and can be retrieved without loading it again.
        /// </summary>
        /// <param name="assetName">The name of the asset.</param>
        /// <returns>True if the asset is already loaded.</returns>
        public bool IsAssetLoaded(string assetName)
        {
            return loadedAssets.ContainsKey(assetName);
        }

        /// <summary>
        /// Loads an asset from the assetbundle of this mod and cache it.
        /// If required, the assetbundle will be automatically loaded.
        /// </summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="assetName">The name of the asset with or without extension.</param>
        /// <param name="clone">Instantiate a cloned instance if true.</param>
        /// <returns>A reference to the loaded asset or a cloned instance.</returns>
        public T GetAsset<T>(string assetName, bool clone = false) where T : UnityEngine.Object
        {
            bool loadedBundle;
            return GetAsset<T>(assetName, out loadedBundle, clone);
        }

        /// <summary>
        /// Loads an asset from the assetbundle of this mod and cache it.
        /// If required, the assetbundle will be automatically loaded.
        /// </summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="assetname">The name of the asset with or without extension.</param>
        /// <param name="loadedBundle">True if assetbundle had to be loaded.</param>
        /// <param name="clone">Instantiate a cloned instance if true.</param>
        /// <returns>A reference to the loaded asset or a cloned instance.</returns>
        public T GetAsset<T>(string assetName, out bool loadedBundle, bool clone = false) where T : UnityEngine.Object
        {
            loadedBundle = false;

            T asset = LoadAssetFromBundle<T>(assetName, out loadedBundle);

            if (asset == null)
            {
                Debug.LogWarning(string.Format("Failed to load asset: {0}", assetName));
                return null;
            }
            else if (clone)
            {
                return UnityEngine.Object.Instantiate(asset) as T;
            }
            else
                return asset as T;
        }

        /// <summary>
        /// Loads all assets from asset bundle immediately.
        /// Invidual assets can then be retrieved with <see cref="GetAsset{T}(string, bool)"/>.
        /// </summary>
        /// <param name="unloadBundle">Unload asset bundle if true</param>
        /// <returns>True if assetbundle has been loaded succesfully.</returns>
        public bool LoadAllAssetsFromBundle(bool unloadBundle = true)
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

                    if (la.Obj == null)
                    {
                        Debug.LogWarning(string.Format("failed to load asset: {0} for mod: {1}", AssetNames[i], Title));
                        continue;
                    }

                    la.T = la.Obj.GetType();
                    AddAsset(assetname, la);
                }

                if (unloadBundle)
                    UnloadAssetBundle(false);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Load all assets from asset bundle asynchronously.
        /// Invidual assets can then be retrieved with <see cref="GetAsset{T}(string, bool)"/>.
        /// </summary>
        /// <param name="unloadBundle">Unload asset bundle if true.</param>
        public IEnumerator LoadAllAssetsFromBundleAsync(bool unloadBundle = true)
        {
            if (AssetNames == null)
                yield return null;

            for (int i = 0; i < AssetNames.Length; i++)
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
            if (unloadBundle)
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
#if UNITY_EDITOR
            if (IsVirtual)
            {
                string path = modInfo.Files.First(CompareNameWithPath("modsettings.json"));
                return new ModSettings.ModSettings(path.Replace("Assets", Application.dataPath));
            }
#endif

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
                string tableContent = ReadText("textdatabase.txt");
                if (tableContent != null)
                    textdatabase = new Table(tableContent);
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

        #region Private Methods

        /// <summary>
        /// Will retrieve asset either from LoadedAssets or asset bundle and return it. Will load
        /// asset bundle if necessary.
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">name of asset</param>
        /// <param name="loadedBundle">had to load asset bundle</param>
        /// <returns>A reference to the loaded asset or null if not found.</returns>
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

#if UNITY_EDITOR
                if (IsVirtual)
                {
                    la.Obj = LoadAssetFromResources<T>(assetName);
                    if (la.Obj != null)
                    {
                        la.T = la.Obj.GetType();
                        loadedAssets.Add(assetName, la);
                    }
                    return la.Obj as T;
                }
#endif

                if (assetBundle == null)
                    loadedBundle = LoadAssetBundle();

                if (assetBundle.Contains(assetName))
                {
                    la.Obj = assetBundle.LoadAsset<T>(assetName);

                    if (la.Obj != null)
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

        /// <summary>
        /// Gets content of a text file.
        /// </summary>
        /// <param name="name">Name of text file.</param>
        /// <returns>Content of text file or null.</returns>
        private string ReadText(string name)
        {
#if UNITY_EDITOR
            if (IsVirtual)
            {
                string path = modInfo.Files.FirstOrDefault(CompareNameWithPath(name));
                if (path != null)
                    return File.ReadAllText(path);
            }
#endif

            if (assetBundle.Contains(name))
                return GetAsset<TextAsset>(name).ToString();

            return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Load an asset from its name. The asset path must be defined in the manifest file.
        /// </summary>
        /// <typeparam name="T">Asset type.</typeparam>
        /// <param name="name">Name of the asset.</param>
        /// <returns>The loaded asset or null.</returns>
        private T LoadAssetFromResources<T>(string name) where T : UnityEngine.Object
        {
            return modInfo.Files.Where(CompareNameWithPath(name)).Select(x =>
                AssetDatabase.LoadAssetAtPath<T>(x)).FirstOrDefault(x => x != null);
        }

        private Func<string, bool> CompareNameWithPath(string name)
        {
            if (Path.HasExtension(name))
                return x => Path.GetFileName(x).ToLower() == name;
            return x => Path.GetFileNameWithoutExtension(x).ToLower() == name;
        }
#endif

        #endregion

        #region Setup

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
            catch (Exception ex)
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
            catch (Exception ex)
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
            List<string> stringSource = new List<string>(sources.Count);
            Assembly assembly = null;

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

                stringSource = null;
                sources = null;
                return assemblies;
            }
            catch (Exception ex)
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

            for (int i = 0; i < assemblies.Count; i++)
            {
                try
                {
                    Type[] types = assemblies[i].GetTypes();

                    foreach (Type t in types)
                    {
                        if (!t.IsClass)
                            continue;

                        foreach (MethodInfo mi in t.GetMethods())
                        {
                            if (!mi.IsPublic || !mi.IsStatic)
                                continue;
                            else if (mi.ContainsGenericParameters)
                                continue;

                            Invoke initAttribute = (Invoke)Attribute.GetCustomAttribute(mi, typeof(Invoke));
                            if (initAttribute == null)
                                continue;
                            else if (initAttribute.StartState != state)
                                continue;
                            ParameterInfo[] pi = mi.GetParameters();
                            if (pi.Length != 1)
                                continue;
                            else if (pi[0].ParameterType != typeof(InitParams))
                                continue;
                            SetupOptions options = new SetupOptions(initAttribute.Priority, this, mi);
#if DEBUG
                            Debug.Log(string.Format("found new loader: {0} for mod: {1}", options.mi.Name, this.Title));
#endif
                            modLoaders.Add(options);

                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                    continue;
                }
            }

            modLoaders.Sort();
            return modLoaders;
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

        /// <summary>
        /// Unloads the asset bundle associated to this mod. Loaded assets can still be retrieved from cache, 
        /// unless they are also unloaded.
        /// </summary>
        /// <param name="unloadAllObjects">Remove all loaded assets from memory.</param>
        public void UnloadAssetBundle(bool unloadAllObjects)
        {
            if (assetBundle == null)
                return;

            assetBundle.Unload(unloadAllObjects);
#if DEBUG
            Debug.Log(string.Format("Unloaded asset bundle for mod: {0}", Title));
#endif
        }

        /// <summary>
        /// Loads the asset bundle associated to this mod.
        /// </summary>
        /// <returns>The loaded asset bundle or null.</returns>
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

        /// <summary>
        /// Loads the asset bundle associated to this mod asynchronously.
        /// </summary>
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
    }
}
