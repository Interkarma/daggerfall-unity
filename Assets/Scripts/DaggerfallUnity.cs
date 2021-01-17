// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// DaggerfallUnity main class.
    /// </summary>
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(WorldTime))]
    [RequireComponent(typeof(MaterialReader))]
    [RequireComponent(typeof(MeshReader))]
    [RequireComponent(typeof(SoundReader))]
    public class DaggerfallUnity : MonoBehaviour
    {
        #region Fields

        public const string staticGeometryTag = "StaticGeometry";

        bool isReady = false;
        bool isPathValidated = false;
        ContentReader reader;

        WorldTime worldTime;
        MaterialReader materialReader;
        MeshReader meshReader;
        SoundReader soundReader;
        ItemHelper itemHelper;
        NameHelper nameHelper;
        ITerrainSampler terrainSampler = new DefaultTerrainSampler();
        ITerrainTexturing terrainTexturing = new DefaultTerrainTexturing();
        ITerrainNature terrainNature = new DefaultTerrainNature();
        ITextProvider textProvider = new DefaultTextProvider();

        const ulong startingUID = 0x2000000;
        static ulong currentUID = startingUID;

        #endregion

        #region Public Fields

        // General
        public string Arena2Path;
        public int ModelImporter_ModelID = 456;
        public string BlockImporter_BlockName = "MAGEAA01.RMB";
        public string CityImporter_CityName = "Daggerfall/Daggerfall";
        public string Experimental_CityLayoutName = "Daggerfall/Aldcart";
        public string DungeonImporter_DungeonName = "Daggerfall/Privateer's Hold";

        // Performance options
        public bool Option_SetStaticFlags = true;
        public bool Option_CombineRMB = true;
        public bool Option_CombineRDB = true;
        public bool Option_BatchBillboards = true;

        // Import options
        public bool Option_AddMeshColliders = true;
        public bool Option_AddNavmeshAgents = true;
        public bool Option_RMBGroundPlane = true;

        // Prefab options
        public bool Option_ImportLightPrefabs = true;
        public Light Option_CityLightPrefab = null;
        public Light Option_DungeonLightPrefab = null;
        public Light Option_InteriorLightPrefab = null;
        public bool Option_ImportDoorPrefabs = true;
        public DaggerfallActionDoor Option_DungeonDoorPrefab = null;
        public DaggerfallActionDoor Option_InteriorDoorPrefab = null;
        public DaggerfallRMBBlock Option_CityBlockPrefab = null;
        public DaggerfallRDBBlock Option_DungeonBlockPrefab = null;
        public MobilePersonMotor Option_MobileNPCPrefab = null;
        public bool Option_ImportEnemyPrefabs = true;
        public DaggerfallEnemy Option_EnemyPrefab = null;
        public bool Option_ImportRandomTreasure = true;
        public DaggerfallLoot Option_LootContainerPrefab = null;
        public GameObject Option_DungeonWaterPrefab = null;
        public Vector3 Option_DungeonWaterPlaneSize = Vector3.one;
        public Vector3 Option_DungeonWaterPlaneOffset = Vector3.zero;

        // Time and space options
        public bool Option_AutomateTextureSwaps = true;
        public bool Option_AutomateSky = true;
        public bool Option_AutomateCityWindows = true;
        public bool Option_AutomateCityLights = true;
        public bool Option_AutomateCityGates = false;

        #endregion

        #region Class Properties

        /// <summary>
        /// Gets or sets current unique identifier counter.
        /// This property used for serialization/deserialization.
        /// Should not be changed any other time.
        /// UIDs 0-4095 are reserved.
        /// </summary>
        public static ulong CurrentUID
        {
            get { return currentUID; }
            set { currentUID = (value < startingUID) ? startingUID : value; }
        }

        /// <summary>
        /// Gets a new UID. Counter is incremented on each call.
        /// </summary>
        public static ulong NextUID
        {
            get { return GetNextUID(); }
        }

        public bool IsReady
        {
            get { return isReady; }
        }

        public bool IsPathValidated
        {
            get { return isPathValidated; }
        }

        public MaterialReader MaterialReader
        {
            get { return (materialReader != null) ? materialReader : materialReader = GetComponent<MaterialReader>(); }
        }

        public MeshReader MeshReader
        {
            get { return (meshReader != null) ? meshReader : meshReader = GetComponent<MeshReader>(); }
        }

        public SoundReader SoundReader
        {
            get { return (soundReader != null) ? soundReader : soundReader = GetComponent<SoundReader>(); }
        }

        public ItemHelper ItemHelper
        {
            get { return (itemHelper != null) ? itemHelper : itemHelper = new ItemHelper(); }
        }

        public NameHelper NameHelper
        {
            get { return (nameHelper != null) ? nameHelper : nameHelper = new NameHelper(); }
        }

        public WorldTime WorldTime
        {
            get { return (worldTime != null) ? worldTime : worldTime = GetComponent<WorldTime>(); }
        }

        public ContentReader ContentReader
        {
            get { return reader; }
        }

        public ITerrainSampler TerrainSampler
        {
            get { return terrainSampler; }
            set { terrainSampler = value; }
        }

        public ITerrainTexturing TerrainTexturing
        {
            get { return terrainTexturing; }
            set { terrainTexturing = value; }
        }

        public ITerrainNature TerrainNature
        {
            get { return terrainNature; }
            set { terrainNature = value; }
        }

        public ITextProvider TextProvider
        {
            get { return textProvider; }
            set { textProvider = value; }
        }

        static SettingsManager settingsManager;
        public static SettingsManager Settings
        {
            get { return (settingsManager != null) ? settingsManager : settingsManager = new SettingsManager(); }
        }

        #endregion

        #region Singleton

        static DaggerfallUnity instance = null;
        public static DaggerfallUnity Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindDaggerfallUnity(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "DaggerfallUnity";
                        instance = go.AddComponent<DaggerfallUnity>();
                    }
                }
                return instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return (instance != null);
            }
        }

        #endregion

        #region Unity

        void Awake()
        {
            instance = null;
            SetupSingleton();
            SetupArena2Path();
            SetupContentReaders();
        }

        void Start()
        {
            // Allow external code to set their own interfaces at start
            RaiseOnSetTerrainSamplerEvent();
            RaiseOnSetTextProviderEvent();
        }

        void Update()
        {
#if UNITY_EDITOR
            // Check ready every update in editor as code changes can de-instantiate local objects
            if (!isReady) SetupArena2Path();
            if (reader == null) SetupContentReaders();
#endif
        }

        #endregion

        #region Editor-Only Methods

#if UNITY_EDITOR
        public void EditorUpdate()
        {
            // Try to get ready
            if (!isReady)
            {
                SetupSingleton();
                SetupArena2Path();
                SetupContentReaders();
            }

            // Check content ready every update in editor as code changes can de-instantiate local objects
            if (reader == null)
            {
                SetupContentReaders();
            }
        }

        /// <summary>
        /// Setup path and content readers again.
        /// Used by editor when setting new Arena2Path.
        /// </summary>
        public void EditorResetArena2Path()
        {
            Settings.LoadSettings();
            SetupArena2Path();
            SetupContentReaders(true);
        }

        /// <summary>
        /// Clear Arena2 path in editor.
        /// Used when you wish to decouple from Arena2 for certain builds.
        /// </summary>
        public void EditorClearArena2Path()
        {
            Arena2Path = string.Empty;
            EditorResetArena2Path();
        }
#endif

        #endregion

        #region Startup and Shutdown

        /// <summary>
        /// Sets new arena2 path and sets up DaggerfallUnity.
        /// </summary>
        /// <param name="arena2Path">New arena2 path. Must be valid.</param>
        public void ChangeArena2Path(string arena2Path)
        {
            Arena2Path = arena2Path;
            SetupArena2Path();
            SetupContentReaders(true);
        }

        private void SetupArena2Path()
        {
            // Clear path validated flag
            isPathValidated = false;

#if !UNITY_EDITOR
            // When starting a build, always clear stored path
            if (Application.isPlaying)
            {
                Arena2Path = string.Empty;
            }
#endif

            // Allow implementor to set own Arena2 path (e.g. from custom settings file)
            RaiseOnSetArena2SourceEvent();

#if UNITY_EDITOR
            // Check editor singleton path is valid
            if (ValidateArena2Path(Arena2Path))
            {
                isReady = true;
                isPathValidated = true;
                LogMessage("Arena2 path validated.", true);
                return;
            }
#endif

            // Look for arena2/ARENA2 folder inside Settings.MyDaggerfallPath
            bool found = false;
            string path = TestArena2Exists(Settings.MyDaggerfallPath);
            if (!string.IsNullOrEmpty(path))
            {
                //LogMessage("Trying Daggerfall path " + path, true);
                if (Directory.Exists(path))
                    found = true;
                else
                    LogMessage("Daggerfall path not found.", true);
            }

            // Otherwise, look for arena2 folder in Application.dataPath at runtime
            if (Application.isPlaying && !found)
            {
                path = TestArena2Exists(Application.dataPath);
                if (!string.IsNullOrEmpty(path))
                    found = true;
            }

            // Did we find a path?
            if (found)
            {
                // If it appears valid set this is as our path
                //LogMessage(string.Format("Testing arena2 path at '{0}'.", path), true);
                if (ValidateArena2Path(path))
                {
                    Arena2Path = path;
                    isReady = true;
                    isPathValidated = true;
                    LogMessage(string.Format("Found valid arena2 path at '{0}'.", path), true);
                    //Generate log file
                    GenerateDiagLog.PrintInfo(Settings.MyDaggerfallPath);
                    return;
                }
            }
            else
            {
                LogMessage(string.Format("Could not find arena2 path. Try setting MyDaggerfallPath in settings.ini."), true);
            }

            // No path was found but we can try to carry on without one
            // Many features will not work without a valid path
            isReady = true;

            // Singleton is now ready
            RaiseOnReadyEvent();
        }

        private void SetupContentReaders(bool force = false)
        {
            if (reader == null || force)
            {
                // Ensure content readers available even when path not valid
                if (isPathValidated)
                {
                    DaggerfallUnity.LogMessage(string.Format("Setting up content readers with arena2 path '{0}'.", Arena2Path));
                    reader = new ContentReader(Arena2Path);
                }
                else
                {
                    DaggerfallUnity.LogMessage(string.Format("Setting up content readers without arena2 path. Not all features will be available."));
                    reader = new ContentReader(string.Empty);
                }
            }
        }

        #endregion

        #region Public Static Methods

        public static void LogMessage(string message, bool showInEditor = false)
        {
            if (showInEditor || Application.isPlaying) Debug.Log(string.Format("DFTFU {0}: {1}", VersionInfo.DaggerfallToolsForUnityVersion, message));
        }

        public static bool FindDaggerfallUnity(out DaggerfallUnity dfUnityOut)
        {
            dfUnityOut = GameObject.FindObjectOfType<DaggerfallUnity>();
            if (dfUnityOut == null)
            {
                LogMessage("Could not locate DaggerfallUnity GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        public static string TestArena2Exists(string parent)
        {
            // Accept either upper or lower case
            string pathLower = Path.Combine(parent, "arena2");
            string pathUpper = Path.Combine(parent, "ARENA2");

            if (Directory.Exists(pathLower))
                return pathLower;
            else if (Directory.Exists(pathUpper))
                return pathUpper;
            else
                return string.Empty;
        }

        public static bool ValidateArena2Path(string path)
        {
            DFValidator.ValidationResults results;
            DFValidator.ValidateArena2Folder(path, out results);

            return results.AppearsValid;
        }

        static ulong GetNextUID()
        {
            return currentUID++;
        }

        /// <summary>
        /// Resets UID back to starting state.
        /// Should only be used when starting a fresh new game (e.g. classic save import).
        /// </summary>
        public static void ResetUID()
        {
            currentUID = startingUID;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Removes from cache all assets that have not been accessed from the time in minutes defined by
        /// <see cref="SettingsManager.AssetCacheThreshold"/>. If equals to <c>0</c> assets are never removed from cache.
        /// </summary>
        internal void PruneCache()
        {
            if (Settings.AssetCacheThreshold == 0)
                return;

            float time = Time.realtimeSinceStartup;
            float threshold = Settings.AssetCacheThreshold * 60;

            MaterialReader.PruneCache(time, threshold);
            if (ModManager.Instance)
                ModManager.Instance.PruneCache(time, threshold);

            RaiseOnPruneCacheEvent(time, threshold);
        }

        #endregion

        #region Private Methods

        private void SetupSingleton()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
            {
                if (Application.isPlaying)
                {
                    LogMessage("Multiple DaggerfallUnity instances detected in scene!", true);
                    Destroy(gameObject);
                }
            }
        }

        #endregion

        #region Event Handlers

        // OnReady
        public delegate void OnReadyEventHandler();
        public static event OnReadyEventHandler OnReady;
        protected virtual void RaiseOnReadyEvent()
        {
            if (OnReady != null)
                OnReady();
        }

        // OnSetArena2Source
        public delegate void OnSetArena2SourceEventHandler();
        public static event OnSetArena2SourceEventHandler OnSetArena2Source;
        protected virtual void RaiseOnSetArena2SourceEvent()
        {
            if (OnSetArena2Source != null)
                OnSetArena2Source();
        }

        // OnSetTerrainSampler
        public delegate void OnSetTerrainSamplerEventHandler();
        public static event OnSetTerrainSamplerEventHandler OnSetTerrainSampler;
        protected virtual void RaiseOnSetTerrainSamplerEvent()
        {
            if (OnSetTerrainSampler != null)
                OnSetTerrainSampler();
        }

        // OnSetTextProvider
        public delegate void OnSetTextProviderEventHandler();
        public static event OnSetTextProviderEventHandler OnSetTextProvider;
        protected virtual void RaiseOnSetTextProviderEvent()
        {
            if (OnSetTextProvider != null)
                OnSetTextProvider();
        }

        public delegate void OnPruneCacheEventHandler(float time, float threshold);
        public event OnPruneCacheEventHandler OnPruneCache;
        private void RaiseOnPruneCacheEvent(float time, float threshold)
        {
            if (OnPruneCache != null)
                OnPruneCache(time, threshold);
        }

        #endregion
    }
}
