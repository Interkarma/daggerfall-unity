// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
        [NonSerialized]
        public const string Version = "1.4.4";

        #region Fields

        bool isReady = false;
        bool isPathValidated = false;
        ContentReader reader;

        WorldTime worldTime;
        MaterialReader materialReader;
        MeshReader meshReader;
        SoundReader soundReader;
        
        #endregion

        #region Public Fields

        public string Arena2Path;
        public int ModelImporter_ModelID = 456;
        public string BlockImporter_BlockName = "MAGEAA01.RMB";
        public string CityImporter_CityName = "Daggerfall/Daggerfall";
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
        public bool Option_CloseCityGates = false;

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
        public bool Option_ImportEnemyPrefabs = true;
        public DaggerfallEnemy Option_EnemyPrefab = null;

        // Time and space options
        public bool Option_AutomateTextureSwaps = true;
        public bool Option_AutomateSky = true;
        public bool Option_AutomateCityWindows = true;
        public bool Option_AutomateCityLights = true;
        public bool Option_AutomateCityGates = false;

        #endregion

        #region Class Properties

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

        public WorldTime WorldTime
        {
            get { return (worldTime != null) ? worldTime : worldTime = GetComponent<WorldTime>(); }
        }

        public ContentReader ContentReader
        {
            get { return reader; }
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

        void Start()
        {
            SetupSingleton();
            SetupArena2Path();
            SetupContentReaders();
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
        /// <summary>
        /// Setup path and content readers again.
        /// Used by editor when setting new Arena2Path.
        /// </summary>
        public void EditorResetArena2Path()
        {
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

        private void SetupArena2Path()
        {
            // Allow implementor to set own Arena2 path (e.g. from custom settings file)
            RaiseOnSetArena2SourceEvent();

            // Check path is valid
            if (ValidateArena2Path(Arena2Path))
            {
                isReady = true;
                isPathValidated = true;
                LogMessage("Arena2 path validated.", true);
                return;
            }
            else
            {
                // Look for arena2 folder in Application.dataPath at runtime
                if (Application.isPlaying)
                {
                    string path = Path.Combine(Application.dataPath, "arena2");
                    if (Directory.Exists(path))
                    {
                        // If it appears valid set this is as our path
                        if (ValidateArena2Path(path))
                        {
                            Arena2Path = path;
                            isReady = true;
                            isPathValidated = true;
                            LogMessage(string.Format("Found valid arena2 path at '{0}'.", path));
                            return;
                        }
                    }
                }
            }

            // No path was found but we can try to carry on without one
            isReady = true;
            isPathValidated = false;

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
            if (showInEditor || Application.isPlaying) Debug.Log(string.Format("DFTFU {0}: {1}", Version, message));
        }

        public static bool FindDaggerfallUnity(out DaggerfallUnity dfUnityOut)
        {
            dfUnityOut = GameObject.FindObjectOfType(typeof(DaggerfallUnity)) as DaggerfallUnity;
            if (dfUnityOut == null)
            {
                LogMessage("Could not locate DaggerfallUnity GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        public static bool ValidateArena2Path(string path)
        {
            DFValidator.ValidationResults results;
            DFValidator.ValidateArena2Folder(path, out results);

            return results.AppearsValid;
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

        #endregion
    }
}
