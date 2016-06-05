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
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DaggerfallWorkshop.Game.Utility.ModSupport
{

    public class ModManager : MonoBehaviour
    {

        #region fields

        public const string MODEXTENSION        = ".dfmod";
        public const string MODINFOEXTENSION    = ".dfmod.json";
        static ModManager instance;
        bool AlreadyAtStartMenuState            = false;
        static bool AlreadyStartedInit          = false;
        string modDirectory;
        int loadedModCount = 0;
        List<SetupOptions> modLoaders;
        [SerializeField]
        List<Mod> Mods;

        #endregion

        #region properties

        public int LoadedModCount
        {
            get { return loadedModCount; }
            private set { loadedModCount = value; }
        }

        public string ModDirectory
        {
            get { return modDirectory; }
            set { modDirectory = value; }
        }
        public static ModManager Instance
        {
            get { return instance; }
            private set { instance = value; }
        }

        #endregion


        #region unity
        void Awake()
        {
            if(string.IsNullOrEmpty(ModDirectory))
            {
                ModDirectory = Application.streamingAssetsPath;
            }

            SetupSingleton();

            if (instance == this)
                StateManager.OnStateChange += StateManager_OnStateChange;
        }

        // Use this for initialization
        void Start()
        {
            modLoaders = new List<SetupOptions>();
            Mods = new List<Mod>();
            FindModsFromDirectory();
            LoadAllSourceCodeFromModBundles();
        }

        void Update()
        {
            LoadedModCount = Mods.Count;

            if(modLoaders.Count > 0)
            {
                if(AlreadyAtStartMenuState)
                {
                    for (int i = 0; i < modLoaders.Count; i++)
                    {
                        SetupOptions options = modLoaders[i];
                        try
                        {
                            MethodInfo mi = options.T.GetMethod(options.targetName, BindingFlags.Public | BindingFlags.Static);

                            if (mi != null)
                            {
                                ParameterInfo[] pi = mi.GetParameters();

                                InitParams initParams = new InitParams(options.mod.Title, GetModIndex(options.mod.Title), options.mod.LoadPriority, LoadedModCount);

                                if (pi.Length > 1)
                                    Debug.Log(string.Format("error: invalid init method specified for: {0} ; method name: {1} - too many paramaters", options.T.Name, options.targetName));
                                else if (pi.Length == 1)
                                {
                                    if (pi[0].ParameterType != typeof(InitParams))
                                        Debug.Log(string.Format("error: invalid init method specified for: {0} ; method name: {1} - incorrect paramater type", options.T.Name, options.targetName));
                                    else
                                        mi.Invoke(null, new object[] { initParams });
                                }
                            }
                            else
                            {
                                Debug.Log(string.Format("Failed to invoke static method {0} type {1}", options.targetName, options.T.Name));
                            }
                        }
                        catch(Exception ex)
                        {
                            Debug.LogError(ex.Message);
                        }
                        modLoaders.RemoveAt(i);
                    }
                }
            }
        }

        private void Init()
        {
            if (AlreadyStartedInit)
                return;

            AlreadyStartedInit = true;

            Mods.Sort();

            Debug.Log("Mod Manager - Init");

            for (int i = Mods.Count-1; i >= 0; i--)
            {
                Mod mod = Mods[i];
                Debug.Log("ModManager - started loading mod: " + mod.Name);

                if (mod == null || !mod.Enabled)
                {
                    Debug.Log("removing mod at index: " + i);
                    UnloadMod(mod.Title, true);
                    continue;
                }

                mod.CompileSourceToAssemblies();
                List<SetupOptions> setupOptions = Mods[i].FindModLoaders();
                if (setupOptions == null)
                    continue;
                else
                {
                    for (int k = 0; k < setupOptions.Count; k++)
                    {
                        modLoaders.Add(setupOptions[k]);
                    }

                }
                Debug.Log("ModManager - init finished.  Mod Count: " + LoadedModCount);
            }
        }

        #endregion

        /// <summary>
        /// Get index for mod using title
        /// </summary>
        /// <param name="modTitle"></param>
        /// <returns></returns>
        public int GetModIndex(string modTitle)
        {
            if (string.IsNullOrEmpty(modTitle))
                return -1;

            for(int i = 0; i < Mods.Count; i++)
            {
                if (Mods[i].Title == modTitle)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Get mod using index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public bool GetMod(int index, out Mod mod)
        {
            mod = null;
            if (index < 0 || index > Mods.Count)
                return false;
            else
                mod = Mods[index];
            return mod != null;
        }

        /// <summary>
        /// Get mod from Mod Title
        /// </summary>
        /// <param name="modTitle"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public bool GetMod(string modTitle, out Mod mod)
        {
            mod = null;

            int index = GetModIndex(modTitle);

            if(GetModIndex(modTitle) >= 0)
            {
                mod = Mods[index];
                return true;
            }
            else
                return false;
        }

        public Mod[] GetAllMods(bool loadOrder = false)
        {
            var selection = from mod in Mods
                            select mod;
            if (loadOrder)
                return selection.OrderBy(x => x.LoadPriority).ToArray();
            else
                return selection.ToArray();
        }

        public ModInfo[] GetAllModInfo()
        {
            var selection = from mod in GetAllMods()
                            where (mod.ModInfo != null)
                            select mod.ModInfo;
            return selection.ToArray();
        }

        //public IModController[] GetAllModControllers()
        //{
        //    var selection = from mod in Mods
        //                    where (mod != null && mod.ModController != null)
        //                    select mod.ModController;

        //    return selection.ToArray();
        //}

        /// <summary>
        /// Get asset from mod using asset name
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">name of asset</param>
        /// <param name="modName">name of mod</param>
        /// <param name="asset">asset to return</param>
        /// <param name="makeCopy">if true, returns a copy</param>
        /// <returns>bool</returns>
        public bool GetAssestFromMod<T>(string assetName, string modTitle, out T asset, bool makeCopy = true) where T : UnityEngine.Object
        {
            asset = null;
            Mod mod;

            if (!GetMod(modTitle, out mod))
                return false;
            else
            {
                asset = mod.GetAssetFromLoadedBundle<T>(assetName, makeCopy);
                return asset != null;
            }
        }


        public static string GetAssetName(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return null;
            int startIndex = assetPath.LastIndexOfAny(new char[] { '\\', '/' }) + 1;
            return assetPath.Substring(startIndex);
        }

        //public void RegisterNewModController<T>(T controller) where T : IModController
        //{
        //    if (controller == null)
        //    {
        //        Debug.LogError("Couldn't register new mod - obj paramater was null");
        //        return;
        //    }
        //    else if (string.IsNullOrEmpty(controller.ModName))
        //    {
        //        Debug.LogError("Couldn't register new mod - controller has no mod name");
        //        return;
        //    }
        //    Mod mod;
        //    if (!GetMod(controller.ModName, out mod))
        //    {
        //        Debug.LogError("Couldn't register new mod - invalid mod name");
        //        return;
        //    }

        //    mod.ModController = controller;

        //    if (OnNewModControllerRegistered != null)
        //    {
        //        OnNewModControllerRegistered(controller);
        //        Debug.Log("new mod controller registered: " + controller.ModName);
        //    }
        //}

        private static string GetModNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            return path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1).Replace(MODEXTENSION, "");
        }

        private void SetupSingleton()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                instance = this;
            }
            else if (instance != this)
            {
                if (Application.isPlaying)
                {
                    DaggerfallUnity.LogMessage("Multiple ModManager instances detected in scene!", true);
                    Destroy(this);
                }
            }
        }

        #region Mod Loading

        //look for changes in mod directory before the compiling / loading process has begun
        public void Refresh()
        {
            if (!AlreadyAtStartMenuState)
                FindModsFromDirectory(true);
        }

        /// <summary>
        /// Locates all the .dfmod files in the mod path
        /// </summary>
        private void FindModsFromDirectory(bool refresh = false)
        {
            if(!Directory.Exists(ModDirectory))
            {
                Debug.Log("invalid mod directory: " + ModDirectory);
                return;
            }

            var modFiles = Directory.GetFiles(ModDirectory, "*" + MODEXTENSION, SearchOption.AllDirectories);
            var modNames = new string[modFiles.Length];

            for (int i = 0; i < modFiles.Length; i++)
            {

                string modFilePath = modFiles[i];

                string DirPath = modFilePath.Substring(0, modFilePath.LastIndexOf(Path.DirectorySeparatorChar));
                modNames[i] = GetModNameFromPath(modFilePath);

                if(string.IsNullOrEmpty(modNames[i]))
                {
                    Debug.Log("failed to get mod name of mod");
                    continue;
                }

                AssetBundle ab;
                if (!LoadModAssetBundle(modFilePath, out ab))
                    continue;
                Mod mod = new Mod(modNames[i], DirPath, ab);

                mod.LoadPriority = i;
                int index = GetModIndex(mod.Title);
                if (index < 0)
                    Mods.Add(mod);
            }

            if (refresh)
            {
                for (int j = 0; j < Mods.Count; j++)
                {
                    if (!modNames.Contains(Mods[j].Name))
                    {
                        Debug.Log(string.Format("mod {0} no longer loaded", Mods[j].Name));
                        UnloadMod(Mods[j].Title, true);
                    }
                }
            }
        }

        /// <summary>
        /// Loads Asset bundle and adds to ModLookUp dictionary
        /// </summary>
        /// <param name="modFilePath"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        private static bool LoadModAssetBundle(string modFilePath, out AssetBundle ab)
        {
            ab = null;
            if (!File.Exists(modFilePath))
            {
                Debug.Log(string.Format("Asset Bundle not found: {0}", modFilePath));
                return false;
            }

            try
            {
                ab = AssetBundle.LoadFromFile(modFilePath);
                return ab != null;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Unload mod and related asset bundle.
        /// TODO - raise event
        /// </summary>
        /// <param name="modName"></param>
        /// <param name="unloadAllAssets"></param>
        /// <returns></returns>
        public bool UnloadMod(string modTitle, bool unloadAllAssets)
        {
            Mod mod;

            try
            {
                if(!GetMod(modTitle, out mod))
                {
                    Debug.Log("Failed to unload mod as mod name wasn't found: " + modTitle);
                    return false;
                }
                Mods.Remove(mod);
                mod.AssetBundle.Unload(unloadAllAssets);
                mod = null;
                return true;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }

        }

        #endregion

        #region Mod Source Loading/Compiling

        private void LoadAllSourceCodeFromModBundles()
        {
            for (int i = 0; i < Mods.Count; i++)
            {
                Debug.Log("Getting source for Mod: " + Mods[i].Name);
                Mods[i].LoadSourceCodeFromModBundle();
            }
        }

        public static Assembly CompileFromSourceAssets(string[] source)
        {
            if (source == null || source.Length < 1)
            {
                Debug.Log("nothing to compile");
                return null;
            }

            System.Reflection.Assembly assembly;

            try
            {
                assembly = Compiler.CompileSource(source, true);
                return assembly;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }

        #endregion


        #region events
        //public delegate void NewObjectCreatedHandler(object obj, SetupOptions options);
        //public static event NewObjectCreatedHandler OnNewObjectCreated;

        //public delegate void NewModRegistered(IModController newModController);
        //public static event NewModRegistered OnNewModControllerRegistered;

        public void StateManager_OnStateChange(StateManager.StateTypes state)
        {
            Debug.Log("new state: " + state.ToString());
            if(state != StateManager.StateTypes.Setup && state != StateManager.StateTypes.None)
            {
                if(!AlreadyAtStartMenuState)
                {
                    AlreadyAtStartMenuState = true;
                    StateManager.OnStateChange -= StateManager_OnStateChange;
                    Init();
                }
            }
        }

        #endregion

    }

}
