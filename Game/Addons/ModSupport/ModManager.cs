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


/* ToDo:
 * 1. In Game UI Menu for Mods 
*  2. IModController stuff
 * 3. Loading Assemblies from bundles
 * 4. Asych. asset loading
 */
namespace DaggerfallWorkshop.Game.Utility.ModSupport
{

    public class ModManager : MonoBehaviour
    {

        #region fields

        public const string MODEXTENSION = "dfmod";
        static ModManager instance;

        [SerializeField]
        bool AlreadyAtStartMenuState = false;
        [SerializeField]
        bool AlreadyAtGameState      = false;
        [SerializeField]
        string modDirectory;                    //path to find mods, if empty, defaults to streaming assets
        [SerializeField]
        int loadedModCount = 0;
        [SerializeField]
        List<SetupOptions> objectsToCreate;
        [SerializeField]
        Dictionary<string, Mod> ModLookup;

        #endregion

        #region properties

        public List<SetupOptions> ObjectsToCreate { get { return objectsToCreate; } private set { objectsToCreate = value; } }
        private List<Assembly> GetAssemblies { get { return ModLookup.Values.SelectMany(d => d.Assemblies).ToList(); } }

        public int LoadedModCount
        {
            get { return loadedModCount; }
            set { loadedModCount = value; }
        }

        public string ModDirectory
        {
            get { return modDirectory; }
            set { modDirectory = value; }
        }
        public static ModManager Instance
        {
            get { return instance; }
            set { instance = value; }
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
            //ModControllers  = new List<IModController>();
            ObjectsToCreate = new List<SetupOptions>();
            ModLookup = new Dictionary<string, Mod>();
        }

        // Use this for initialization
        void Start()
        {
            //load mods
            FindModsFromDirectory();
            LoadAllSourceCodeFromModBundles();
        }

        void Update()
        {
            LoadedModCount = ModLookup.Values.Count;
        }

        public void OnLevelWasLoaded(int index)
        {
            Debug.Log("level index: " + index);
            if(index == SceneControl.GameSceneIndex && (!AlreadyAtStartMenuState && !AlreadyAtGameState))
            {
                Debug.Log("Game Scene Loaded");

                CompileAllSourceCodeFromModBundles();

                foreach (Assembly assembly in GetAssemblies)
                    CreateObjectsFromLoadableTypes(assembly);

                AlreadyAtStartMenuState = true;
                StartCoroutine(InstanitateObjectsFromTypes());

                StateManager.OnStateChange += StateManager_OnStateChange;
            }
        }

        #endregion

        public bool GetMod(string modName, out Mod mod)
        {
            mod = null;

            if (ModLookup.ContainsKey(modName))
            {
                mod = ModLookup[modName];
                return mod != null;
            }
            else
                return false;
        }

        public IEnumerable<Mod>GetAllMods()
        {
            return ModLookup.Values;
        }

        public IEnumerable<ModInfo>GetAllModInfo()
        {
            var selection = from mod in ModLookup.Values
                            where (mod != null && mod.ModInfo != null)
                            select mod.ModInfo;

            return selection;
        }

        public IEnumerable<IModController> GetAllModControllers()
        {
            var selection = from mod in ModLookup.Values
                            where (mod != null && mod.ModController != null)
                            select mod.ModController;

            return selection;
        }

        public bool GetAssestFromMod<T>(string assestName, string modName, out T asset, bool makeCopy = true) where T : UnityEngine.Object
        {
            asset = null;
            Mod mod;

            if (ModLookup.TryGetValue(modName, out mod))
            {
                return false;
            }
            else
            {
                asset = mod.GetAssetFromLoadedBundle<T>(assestName, makeCopy);
                return asset != null;
            }

        }

        public void RegisterNewModController<T>(T controller) where T : IModController
        {
            if (controller == null)
            {
                Debug.LogError("Couldn't register new mod - obj paramater was null");
                return;
            }
            else if (string.IsNullOrEmpty(controller.ModName))
            {
                Debug.LogError("Couldn't register new mod - controller has no mod name");
                return;
            }
            Mod mod;
            if (!GetMod(controller.ModName, out mod))
            {
                Debug.LogError("Couldn't register new mod - invalid mod name");
                return;
            }

            mod.ModController = controller;

            if (OnNewModControllerRegistered != null)
            {
                OnNewModControllerRegistered(controller);
                Debug.Log("new mod controller registered: " + controller.ModName);
            }
        }

        private string GetModNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            return path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
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

        /// <summary>
        /// Locates all the .dfmod files in the mod path
        /// </summary>
        private void FindModsFromDirectory()
        {
            if(!Directory.Exists(ModDirectory))
            {
                Debug.Log("invalid mod directory: " + ModDirectory);
                return;
            }

            var modFiles = Directory.GetFiles(ModDirectory, "*."+MODEXTENSION, SearchOption.AllDirectories);

            foreach(string modFilePath in modFiles)
            {
                Mod mod = new Mod();
                Debug.Log(modFilePath + " " + Path.DirectorySeparatorChar);

                mod.DirPath = modFilePath.Substring(0, modFilePath.LastIndexOf(Path.DirectorySeparatorChar));
                string modNameFromPath = GetModNameFromPath(modFilePath);

                if(ModLookup.ContainsKey(modNameFromPath))
                {

                    Debug.LogWarning("Duplicate mod name found: " + modNameFromPath);
                    mod = null;
                    continue;
                }

                //Debug.Log(mod.dirPath);
                //Debug.Log(modNameFromPath);

                if (!LoadModAssetBundle(modFilePath, ref mod))
                    continue;
                if (!GetModInfoFromBundle(modNameFromPath, ref mod))
                    continue;

                if(!ModLookup.ContainsKey(mod.ModInfo.ModName))
                    ModLookup[mod.ModInfo.ModName] = mod;
            }
        }

        /// <summary>
        /// Loads Asset bundle and adds to ModLookUp dictionary
        /// </summary>
        /// <param name="modFilePath"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        private bool LoadModAssetBundle(string modFilePath, ref Mod mod)
        {
            if (mod == null)
            {
                Debug.LogWarning("mod file was null");
                return false;
            }

            if (!File.Exists(modFilePath))
            {
                Debug.Log(string.Format("Asset Bundle not found: {0}", modFilePath));
                return false;
            }

            try
            {
                mod.AssetBundle = AssetBundle.LoadFromFile(modFilePath);
                return mod.AssetBundle != null;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }

        }

        //retrieve mod info json file from asset bundle, and set to Mod.ModInfo
        //returns false if failed, true if successful
        private bool GetModInfoFromBundle(string modName, ref Mod mod)
        {

            if (string.IsNullOrEmpty(modName))
            {
                Debug.Log("invalid modname - can't retrive modinfo from asset bundle");
                return false;
            }
            else if (mod == null)
            {
                Debug.LogWarning("mod object was null for mod: " + modName);
                return false;
            }

            //get mod info file from asset bundle.  must be == modname.json, i.e. examplemod.dfmod.json
            string modInfoAssetName = modName + ".json";

            try
            {
                string modInfo = mod.GetAssetFromLoadedBundle<TextAsset>(modInfoAssetName).ToString();

                if (modInfo == null)
                {
                    Debug.Log("failed to retrieve mod info asset from asset bundle");
                    return false;
                }

                mod.ModInfo = (ModInfo)JsonUtility.FromJson(modInfo, typeof(ModInfo));
                return mod.ModInfo != null;
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
        public bool UnloadMod(string modName, bool unloadAllAssets)
        {
            Mod mod;
            try
            {
                if(!GetMod(modName, out mod))
                {
                    Debug.Log("Failed to unload mod as mod name wasn't found: " + modName);
                    return false;
                }

                mod = ModLookup[modName];
                ModLookup.Remove(modName);
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


        /// <summary>
        /// Compiles all the source code that has been loaded from mod asset bundles
        /// </summary>
        private void CompileAllSourceCodeFromModBundles()
        {

            foreach (Mod mod in ModLookup.Values)
            {
                try
                {
                    List<string> stringSource = new List<string>(mod.Sources.Length);

                    for (int i = 0; i < mod.Sources.Length; i++)
                    {
                        if (mod.Sources[i].isPreCompiled)
                        {
                            Assembly assembly = Assembly.Load(mod.Sources[i].sourceTxt.bytes);

                            if (assembly != null)
                                mod.Assemblies.Add(assembly);
                        }
                        else
                        {
                            stringSource.Add(mod.Sources[i].sourceTxt.ToString());
                        }
                    }

                    if (stringSource.Count > 0)
                    {
                        Assembly assembly = CompileFromSourceAssets(stringSource.ToArray());

                        if (assembly != null)
                            mod.Assemblies.Add(assembly);
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogError(string.Format("Failed to compile source for mod: {0} \n {1}", ex.Message));
                    continue;
                }

            }
        }


        private void LoadAllSourceCodeFromModBundles()
        {
            foreach (Mod mod in ModLookup.Values)
            {
                Debug.Log("Getting source for Mod: " + mod.ModInfo.ModName);
                mod.Sources = LoadSourceCodeFromModBundle(mod).ToArray();
            }
        }


        List<Source> LoadSourceCodeFromModBundle(Mod mod)
        {

            AssetBundle bundle = mod.AssetBundle;
            List<Source> source = new List<Source>();

            string[] assetNames = bundle.GetAllAssetNames();


            string name = null;
            foreach (string assetName in assetNames)
            {
                //Debug.Log(string.Format("{0}", assetName));

                name  = assetName.ToLower();

                if (name.EndsWith(".cs.txt") || name.EndsWith(".cs"))// || name.EndsWith(".byte")) //.byte is for .dll - not tested yet
                {
                    Source newUncompiledSource;
                    TextAsset newSource = mod.GetAssetFromLoadedBundle<TextAsset>(assetName);

                    if(newSource != null)
                    {
                        newUncompiledSource.sourceTxt = newSource;
                        newUncompiledSource.isPreCompiled = name.EndsWith(".dll");
                        source.Add(newUncompiledSource);
                    }
                }
            }

            return source;
        }

        public Assembly CompileFromSourceAssets(string[] source)
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


        #region Compiling & Instantiating

        IEnumerator InstanitateObjectsFromTypes()
        {
            while(true)
            {
                yield return new WaitForEndOfFrame();

                if (!AlreadyAtGameState || !AlreadyAtStartMenuState)
                {
                    if (GameManager.Instance.StateManager.CurrentState == StateManager.StateTypes.Game)
                    {
                        AlreadyAtStartMenuState = true;
                        AlreadyAtGameState = true;
                    }
                    else if (GameManager.Instance.StateManager.CurrentState == StateManager.StateTypes.Start)
                        AlreadyAtStartMenuState = true;
                    else
                        continue;
                }

                for (int i = 0; i < ObjectsToCreate.Count; i++)
                {
                    Debug.Log(string.Format("OBJECT: {0}: INDEX: {1} COUNT: {2}", objectsToCreate[i].T.Name, i, objectsToCreate.Count));

                    SetupOptions options = ObjectsToCreate[i];

                    if (options.setupType == SetupType.None)
                        ObjectsToCreate.RemoveAt(i);
                    else if (options.setupState == SetupState.None)
                        ObjectsToCreate.RemoveAt(i);
                    if (options.targetObj == null)
                    {
                        if (!GetTargetObject(ref options))
                        {
                            ObjectsToCreate.RemoveAt(i);
                            Debug.Log(string.Format("Failed to setup {0} {1} {1}", options.T, options.modName, options.objName));
                            continue;
                        }
                    }

                    if (options.setupState == SetupState.MenuState && (AlreadyAtGameState || AlreadyAtStartMenuState))
                        CreateNewInstance(ref options);

                    else if (options.setupState == SetupState.GameState && AlreadyAtGameState)
                        CreateNewInstance(ref options);
                    else
                        continue;

                    ObjectsToCreate.RemoveAt(i);
                }
            }
        }


        private bool CreateNewInstance(ref SetupOptions options)
        {
            if(options.T == null || (options.targetObj == null && options.isMonoBehvaiour))
            {
                Debug.LogError(string.Format("Failed to Create new Instance. {0} {1}", options.T, options.targetObj));
                return false;
            }
            try
            {
                object obj = null;
                if (options.isMonoBehvaiour)
                {
                    obj = options.targetObj;

                    if (options.setupType != SetupType.Prefab_Load)//target gets instant. already, don't re-instantiate
                        options.targetObj.AddComponent(options.T);

                    if (options.setupType == SetupType.Component_Prefab || options.setupType == SetupType.Prefab_Load)//target gets instant. already, don't re-instantiate
                        obj = options.targetObj;
                }
                else if (options.hasDefaultConstructor && !options.isMonoBehvaiour)
                    obj = Activator.CreateInstance(options.T);


                if (OnNewObjectCreated != null)
                    OnNewObjectCreated(obj, options);

                return true;

            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
        }


        /// <summary>
        /// Finds / creates object (if any) for object
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private bool GetTargetObject(ref SetupOptions options)
        {
            switch (options.setupType)
            {
                case SetupType.None:
                    break;
                case SetupType.Non_MonoBehaviour:
                    return true;
                case SetupType.Component_ModManager:
                    options.targetObj = this.gameObject;
                    break;
                case SetupType.Component_Player:
                    options.targetObj = GameManager.Instance.PlayerObject;
                    break;
                case SetupType.Component_Camera:
                    options.targetObj = GameManager.Instance.MainCameraObject;
                    break;
                case SetupType.Component_DaggerfallUnity:
                    options.targetObj = DaggerfallUnity.Instance.gameObject;
                    break;
                case SetupType.Component_GameManager:
                    options.targetObj = GameManager.Instance.gameObject;
                    break;
                case SetupType.Component_ByName:
                    options.targetObj = GameObject.Find(options.objName);
                    break;
                case SetupType.Component_Prefab:
                    if(ModLookup.ContainsKey(options.modName))
                    {
                        options.targetObj = ModLookup[options.modName].GetAssetFromLoadedBundle<GameObject>(options.objName);
                    }
                    break;
                case SetupType.Prefab_Load:
                    if(ModLookup.ContainsKey(options.modName))
                    {
                        options.targetObj = ModLookup[options.modName].GetAssetFromLoadedBundle<GameObject>(options.objName);
                    }
                    break;
                case SetupType.NewObject:
                        options.targetObj = new GameObject(options.objName);
                    break;
                default:
                    break;
            }

            return options.targetObj != null;
        }


        /// <summary>
        /// Adds loadable types with the SetupInstructionsAttribute to the ObjectsToCreate list
        /// Called on Game Scene level load
        /// </summary>
        /// <param name="assembly"></param>
        private void CreateObjectsFromLoadableTypes(Assembly assembly)
        {
            if (assembly == null)
                return;
            try
            {
                var loadableTypes = Compiler.GetLoadableTypes(assembly);

                foreach (Type t in loadableTypes)
                {
                    SetupOptions options;
                    GetSetupDirectives(t, out options);

                    //add to list for future creation
                    if (options.setupType != SetupType.None && options.setupState != SetupState.None)
                        ObjectsToCreate.Add(options);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return;
            }
        }

        /// <summary>
        /// Parses the SetupInstructionsAttribute, and converts it to a SetupOptions Struct
        /// </summary>
        /// <param name="t"></param>
        /// <param name="options"></param>
        private void GetSetupDirectives(Type t, out SetupOptions options)
        {
            options = new SetupOptions();
            options.setupType = SetupType.None;

            SetupInstructionsAttribute setupAttribute = (SetupInstructionsAttribute)Attribute.GetCustomAttribute(t, typeof(SetupInstructionsAttribute));
            if (setupAttribute == null)
                return;

            options.setupType = setupAttribute.setupType;
            options.setupState = setupAttribute.setupTime;
            options.objName = setupAttribute.objName;
            options.modName = setupAttribute.modName;
            options.isMonoBehvaiour = typeof(Component).IsAssignableFrom(t);
            options.hasDefaultConstructor = (t.GetConstructor(Type.EmptyTypes) != null && !t.IsAbstract);
            options.T = t;
        }

        #endregion


        #region events
        public delegate void NewObjectCreatedHandler(object obj, SetupOptions options);
        public static event NewObjectCreatedHandler OnNewObjectCreated;

        public delegate void NewModRegistered(IModController newModController);
        public static event NewModRegistered OnNewModControllerRegistered;

        public void StateManager_OnStateChange(StateManager.StateTypes state)
        {
            if(state == StateManager.StateTypes.Game)
            {
                AlreadyAtGameState = true;
                StateManager.OnStateChange -= StateManager_OnStateChange;
            }
        }

        #endregion

    }

}
