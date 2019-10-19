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
using FullSerializer;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;

namespace DaggerfallWorkshop.Game.Utility.ModSupport
{
    /// <summary>
    /// A mod for Daggerfall Unity.
    /// </summary>
    [Serializable]
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class Mod
    {
        #region Fields

        private readonly List<Assembly> assemblies = new List<Assembly>(1);
        private readonly Dictionary<string, LoadedAsset> loadedAssets = new Dictionary<string, LoadedAsset>();
#if UNITY_EDITOR
        private readonly Type[] types;
#endif
        private string[] assetNames;
        private List<Source> sources;                               //any source code found in asset bundle
        private Table textdatabase;
        private bool textdatabaseLoaded;

        #endregion

        #region Properties

        /// <summary>
        /// The name of the mod file on disk without extension.
        /// </summary>
        [SerializeField]
        public string FileName { get; private set; }

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
        public bool IsReady { get; set; }

        /// <summary>
        /// If this mod is enabled from the mods window, it will be loaded by the Mod Manager
        /// and methods marked with the <see cref="Invoke"/> attribute will be called at the specified state.
        /// </summary>
        [SerializeField]
        public bool Enabled { get; set; }

        /// <summary>
        /// The position in the load order, which affects the invocation order and
        /// the automatic asset loading by the Asset-Injection framework.
        /// </summary>
        [SerializeField]
        public int LoadPriority { get; internal set; }

        /// <summary>
        /// Mod informations defined from the mod builder.
        /// </summary>
        public ModInfo ModInfo { get; private set; }

        /// <summary>
        /// If not null, this is the assetbundle where all the assets for this mod are stored.
        /// Assets should be retrieved with <see cref="GetAsset{T}(string, bool)"/> which benefits of a cache system.
        /// </summary>
        public AssetBundle AssetBundle { get; private set; }

        /// <summary>
        /// The directory where the mod file is stored.
        /// This is equal or a sub-directory of <see cref="ModManager.ModDirectory"/>.
        /// </summary>
        public string DirPath { get; private set; }

        /// <summary>
        /// An unique identifier for this mod or <c>"invalid"</c> if not defined.
        /// </summary>
        public string GUID
        {
            get { return (ModInfo != null) ? ModInfo.GUID : "invalid"; }
        }

        /// <summary>
        /// A directory for persistent mod configuration.
        /// </summary>
        internal string ConfigurationDirectory
        {
            get { return Path.Combine(ModManager.Instance.ModDataDirectory, GUID); }
        }

        /// <summary>
        /// A directory for persistent mod data. It is ensured that the directory is writable but not that exists.
        /// Use <see cref="Directory.CreateDirectory(string)"/> before accessing it.
        /// </summary>
        public string PersistentDataDirectory
        {
            get { return Path.Combine(ConfigurationDirectory, "Data"); }
        }

        /// <summary>
        /// A directory for temporary mod cache. It is ensured that the directory is writable but not that exists.
        /// Use <see cref="Directory.CreateDirectory(string)"/> before accessing it.
        /// </summary>
        public string TemporaryCacheDirectory
        {
            get { return Path.Combine(ModManager.Instance.ModCacheDirectory, GUID); }
        }

        /// <summary>
        /// An optional callback that allows to efficiently send messages to this mod without using reflections.
        /// </summary>
        public DFModMessageReceiver MessageReceiver { get; set; }

        /// <summary>
        /// If this mod has settings, they can be retrieved with <see cref="GetSettings()"/>.
        /// </summary>
        public bool HasSettings { get; private set; }

        /// <summary>
        /// If not null, this callback is invoked when settings are changed or when raised with <see cref="LoadSettings()"/>.
        /// </summary>
        public Action<ModSettings.ModSettings, ModSettingsChange> LoadSettingsCallback { internal get; set; }

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
            if (ModInfo == null)
                ModInfo = new ModInfo();

            Enabled = true;
        }

        /// <summary>
        /// Makes a mod from an assetbundle.
        /// </summary>
        /// <param name="name">Mod filename without the extension.</param>
        /// <param name="dirPath">Path to mod file directory.</param>
        /// <param name="ab">The assetbundle to associate to new mod instance.</param>
        public Mod(string name, string dirPath, AssetBundle ab)
            : this()
        {
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
                ModInfo = new ModInfo();
            }

            this.FileName = name;
            this.DirPath = dirPath;
            this.LoadSourceCodeFromModBundle();
            this.HasSettings = ModSettings.ModSettingsData.HasSettings(this);
#if DEBUG
            Debug.Log(string.Format("Finished Mod setup: {0}", this.Title));
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Makes a mod from a manifest file without an assetbundle for debug.
        /// </summary>
        /// <param name="manifestPath">Path to manifest file.</param>
        /// <param name="modInfo">Content of manifest file.</param>
        internal Mod(string manifestPath, ModInfo modInfo)
            : this()
        {
            if (!manifestPath.EndsWith(ModManager.MODINFOEXTENSION))
                throw new ArgumentException(string.Format("Path is rejected because it doesn't end with {0}", ModManager.MODINFOEXTENSION), "manifestPath");

            if (modInfo == null)
                throw new ArgumentNullException("modInfo");

            IsVirtual = true;
            ModInfo = modInfo;
            types = modInfo.Files.Where(x => x.EndsWith(".cs"))
                .Select(x => AssetDatabase.LoadAssetAtPath<MonoScript>(x))
                .Where(x => x != null).Select(x => x.GetClass()).Where(x => x != null).ToArray();
            FileName = Path.GetFileName(manifestPath.Remove(manifestPath.IndexOf(ModManager.MODINFOEXTENSION)));
            DirPath = ModManager.Instance.ModDirectory;
            HasSettings = ModSettings.ModSettingsData.HasSettings(this);
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
                return ModInfo.Files.Any(CompareNameWithPath(assetName));
#endif

            if (AssetBundle)
                return AssetBundle.Contains(assetName);

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

                    if (AssetBundle == null)
                    {
                        if (!LoadAssetBundle())
                            return false;
                    }

                    LoadedAsset la = new LoadedAsset();
                    la.Obj = AssetBundle.LoadAsset(assetname);

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

                if (AssetBundle == null)
                {
                    yield return LoadAssetBundleAsync();
                    if (AssetBundle == null)
                        yield break;
                }

                AssetBundleRequest request = AssetBundle.LoadAssetAsync(assetname);
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
        /// Seeks assets inside a directory provided by this mod. An asset is accepted if its directory ends with the given subdirectory.
        /// For example "Assets/Textures" matches "Water.png" from "Assets/Game/Mods/Example/Assets/Textures/Water.png".
        /// </summary>
        /// <param name="names">Null or a buffer of names. Will be filled with matches with the extension but without the directory.</param>
        /// <param name="relativeDirectory">A relative directory with forward slashes (i.e. "Assets/Textures").</param>
        /// <param name="extension">An extension including the dots (i.e ".json") or null.</param>
        /// <returns>The number of assets found.</returns>
        public int FindAssetNames(ref List<string> names, string relativeDirectory, string extension = null)
        {
            if (relativeDirectory == null)
                throw new ArgumentNullException("relativeDirectory");

            int initialCount = names != null ? names.Count : 0;

            for (int i = 0; i < ModInfo.Files.Count; i++)
            {
                string path = ModInfo.Files[i];

                // Must have at least one folder
                int nameStart = path.LastIndexOf('/');
                if (nameStart == -1)
                    continue;

                // Must be rooted at Assets or a child directory
                int dirStart = nameStart - relativeDirectory.Length;
                if (dirStart < 0 || (dirStart > 0 && path[dirStart - 1] != '/'))
                    continue;

                // Validate name
                if (extension != null && string.CompareOrdinal(path, path.Length - extension.Length, extension, 0, extension.Length) != 0)
                    continue;

                // Validate directory
                if (string.CompareOrdinal(path, dirStart, relativeDirectory, 0, relativeDirectory.Length) != 0)
                    continue;

                if (names == null)
                    names = new List<string>();
                names.Add(path.Substring(nameStart + 1));
            }

            return names != null ? names.Count - initialCount : 0;
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
        /// Use <see cref="LoadSettings()"/> if you want to support live changes.
        /// </summary>
        public ModSettings.ModSettings GetSettings()
        {
            return new ModSettings.ModSettings(this);
        }

        /// <summary>
        /// Loads mod settings using <see cref="LoadSettingsCallback"/> with an event where all settings are considered changed.
        /// Use <see cref="GetSettings"/> if you don't want to support live changes.
        /// </summary>
        public void LoadSettings()
        {
            if (LoadSettingsCallback == null)
                throw new InvalidOperationException("LoadSettingsCallback is not set.");

            LoadSettingsCallback(GetSettings(), new ModSettingsChange());
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
        /// Checks if this mod is expected to run on current version of Daggerfall Unity.
        /// </summary>
        /// <returns>True if game version is satisfied, false if is not, null if unknown.</returns>
        internal bool? IsGameVersionSatisfied()
        {
            return ModManager.IsVersionLowerOrEqual(ModInfo.DFUnity_Version, VersionInfo.DaggerfallUnityVersion);
        }

        /// <summary>
        /// Gets a localized string from the text table associated with this mod.
        /// </summary>
        /// <param name="key">Key used in the text table.</param>
        /// <returns>Localized string or null.</returns>
        internal string TryLocalize(string key)
        {
            // Read from StreamingAssets/Text
            string databaseName = string.Format("mod_{0}", FileName);
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

                if (AssetBundle == null)
                    loadedBundle = LoadAssetBundle();

                if (AssetBundle.Contains(assetName))
                {
                    la.Obj = AssetBundle.LoadAsset<T>(assetName);

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
                string path = ModInfo.Files.FirstOrDefault(CompareNameWithPath(name));
                return path != null ? File.ReadAllText(path) : null;
            }
#endif

            if (AssetBundle.Contains(name))
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
            return ModInfo.Files.Where(CompareNameWithPath(name)).Select(x =>
                AssetDatabase.LoadAssetAtPath<T>(x)).FirstOrDefault(x => x != null);
        }

        /// <summary>
        /// Makes a delegate that checks if a given path is the path to the asset with the given name.
        /// Input path must be an editor asset path (forward slashes as separators) and the match is case-insensitive.
        /// </summary>
        /// <param name="name">Asset name with or without extension; case is not important.</param>
        /// <returns>A comparer delegate that matches a path from a filename.</returns>
        /// <remarks>
        /// Searching a match among all mod asset paths is very expensive, so this method relies on ordinal comparison
        /// without creating substrings or using Path API.
        /// Nevertheless, this method is only for Editor testing; AssetBundle API only should be used at run-time.
        /// </remarks>
        /// <example>
        /// <code>
        /// var predicate = CompareNameWithPath("name");
        /// Debug.Log(predicate("Assets/SubDir/Name.foo")); // True
        /// </code>
        /// </example>
        private Func<string, bool> CompareNameWithPath(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return path =>
            {
                if (path == null)
                    throw new ArgumentNullException("null");

                int separatorIndex = path.LastIndexOf('/');
                if (separatorIndex == -1)
                    throw new ArgumentException("path is not a valid asset path.", "path");

                if (string.Compare(path, separatorIndex + 1, name, 0, name.Length, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    int end = separatorIndex + 1 + name.Length;
                    if (end == path.Length || path[end] == '.')
                        return true;
                }

                return false;
            };
        }
#endif

        #endregion

        #region Setup

        // Returns array containing names of all assets in asset bundle
        private string[] GetAllAssetNames()
        {
            try
            {
                if (AssetBundle == null)
                    return null;

                string[] assetNames = AssetBundle.GetAllAssetNames();
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

                string[] assetNames = AssetBundle.GetAllAssetNames();

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
#if UNITY_EDITOR
            if (IsVirtual)
                return null;
#endif

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
            List<SetupOptions> modLoaders;

#if UNITY_EDITOR
            if (IsVirtual)
            {
                modLoaders = new List<SetupOptions>();
                foreach (Type type in types)
                    FindModLoaders(state, type, modLoaders);
                modLoaders.Sort();
                return modLoaders;           
            }
#endif
            if (assemblies == null || assemblies.Count < 1)
                return null;

            modLoaders = new List<SetupOptions>(1);

            for (int i = 0; i < assemblies.Count; i++)
            {
                try
                {
                    Type[] types = assemblies[i].GetTypes();

                    foreach (Type t in types)
                    {
                        if (t.IsClass)
                            FindModLoaders(state, t, modLoaders);
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

        private void FindModLoaders(StateManager.StateTypes state, Type type, List<SetupOptions> modLoaders)
        {
            foreach (MethodInfo mi in type.GetMethods())
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
                if (la.T == typeof(GameObject) && AssetBundle.Contains(ImportedComponentAttribute.MakeFileName(assetName)))
                    ImportedComponentAttribute.Restore(this, la.Obj as GameObject);

                loadedAssets.Add(assetName, la);

                if (this.ModInfo != null && string.IsNullOrEmpty(this.Title) == false)
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
            if (AssetBundle == null)
                return;

            AssetBundle.Unload(unloadAllObjects);
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
#if UNITY_EDITOR
            if (IsVirtual)
                return null;
#endif

            string abPath = Path.Combine(DirPath, FileName + ModManager.MODEXTENSION);
            if (!File.Exists(abPath))
                return null;

            AssetBundle ab = AssetBundle.LoadFromFile(abPath);
            if (ab != null)
                this.AssetBundle = ab;
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
#if UNITY_EDITOR
            if (IsVirtual)
                yield break;
#endif

            string abPath = Path.Combine(DirPath, FileName + ModManager.MODEXTENSION);
            if (!File.Exists(abPath))
                yield break;

            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(abPath);
            yield return request;

            if (request.assetBundle != null)
                AssetBundle = request.assetBundle;
#if DEBUG
            Debug.Log(string.Format("Loaded asset bundle for mod: {0}", Title));
#endif
            yield return request.assetBundle;
        }

        #endregion
    }
}
