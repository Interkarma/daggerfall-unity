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

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.Serialization
{
    /// <summary>
    /// Implements save/load logic.
    /// Games are saved in PersistentDataPath\Saves\CharacterName\SaveName.
    /// Each save game will have a screenshot and multiple files.
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        #region Fields

        const string rootSaveFolder = "Saves";
        const string quickSaveFolderName = "QuickSave";
        const string autoSaveFolderName = "AutoSave";
        const string saveDataFilename = "SaveData.txt";
        const string containerDataFilename = "ContainerData.txt";
        const string screenshotFilename = "Screenshot.jpg";
        const string notReadyExceptionText = "SaveLoad not ready.";
        const string invalidLoadIDExceptionText = "serializableObject does not have a valid LoadID";
        const string duplicateLoadIDErrorText = "{0} detected duplicate LoadID {1}. This object will not be serialized.";

        // Serializable objects in scene
        SerializablePlayer serializablePlayer;
        Dictionary<ulong, SerializableActionDoor> serializableActionDoors = new Dictionary<ulong, SerializableActionDoor>();
        Dictionary<ulong, SerializableActionObject> serializableActionObjects = new Dictionary<ulong, SerializableActionObject>();
        Dictionary<ulong, SerializableEnemy> serializableEnemies = new Dictionary<ulong, SerializableEnemy>();
        Dictionary<ulong, SerializableLootContainer> serializableLootContainers = new Dictionary<ulong, SerializableLootContainer>();

        string unitySavePath = string.Empty;
        string daggerfallSavePath = string.Empty;

        #endregion

        #region Properties

        public string UnitySavePath
        {
            get { return GetUnitySavePath(); }
        }

        public string DaggerfallSavePath
        {
            get { return GetDaggerfallSavePath(); }
        }

        #endregion
        
        #region Singleton

        static SaveLoadManager instance = null;
        public static SaveLoadManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindSingleton(out instance))
                        return null;
                }
                return instance;
            }
        }

        public static bool HasInstance
        {
            get { return (instance != null); }
        }

        #endregion

        #region Unity

        void Awake()
        {
            sceneUnloaded = false;
        }

        void Start()
        {
            SetupSingleton();

            // Init classic game startup time at startup
            // This will also be modified when deserializing save game data
            DaggerfallUnity.Instance.WorldTime.Now.SetClassicGameStartTime();
        }

        static bool sceneUnloaded = false;
        void OnApplicationQuit()
        {
            sceneUnloaded = true;
        }

        void OnDestroy()
        {
            sceneUnloaded = true;
        }

        #endregion

        #region Public Methods

        public bool IsReady()
        {
            if (!DaggerfallUnity.Instance.IsReady || !DaggerfallUnity.Instance.IsPathValidated)
                return false;

            return true;
        }

        public string GetDestinationSavePath(string characterName, string saveName)
        {
            string path = Path.Combine(UnitySavePath, characterName);
            path = Path.Combine(path, saveName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            return path;
        }

        public bool HasQuickSave()
        {
            // Must be ready
            if (!IsReady())
                throw new Exception(notReadyExceptionText);

            //string path = Path.Combine(UnitySavePath, quickSaveFilename);
            //if (File.Exists(path))
            //    return true;

            return false;
        }

        public void QuickSave()
        {
            // Must be ready
            if (!IsReady())
                throw new Exception(notReadyExceptionText);

            // Save game
            StartCoroutine(SaveGame(quickSaveFolderName));
        }

        public void QuickLoad()
        {
            // Must be ready
            if (!IsReady())
                throw new Exception(notReadyExceptionText);

            //// Read save data from file
            //string json = ReadSaveFile(Path.Combine(UnitySavePath, quickSaveFilename));

            //// Deserialize JSON string to save data
            //SaveData_v1 saveData = Deserialize(typeof(SaveData_v1), json) as SaveData_v1;

            //// Restore save data
            //GameManager.Instance.PauseGame(false);
            //DaggerfallUI.Instance.FadeHUDFromBlack();
            //StartCoroutine(LoadGame(saveData));

            //// Notify
            //DaggerfallUI.Instance.PopupMessage(HardStrings.gameLoaded);
        }

        #endregion

        #region Public Static Methods

        public static bool FindSingleton(out SaveLoadManager singletonOut)
        {
            singletonOut = GameObject.FindObjectOfType(typeof(SaveLoadManager)) as SaveLoadManager;
            if (singletonOut == null)
                return false;

            return true;
        }

        /// <summary>
        /// Register ISerializableGameObject with SaveLoadManager.
        /// </summary>
        public static void RegisterSerializableGameObject(ISerializableGameObject serializableObject)
        {
            if (sceneUnloaded)
                return;

            if (serializableObject.LoadID == 0)
                throw new Exception(invalidLoadIDExceptionText);

            if (serializableObject is SerializablePlayer)
                Instance.serializablePlayer = serializableObject as SerializablePlayer;
            else if (serializableObject is SerializableActionDoor)
                AddSerializableActionDoor(serializableObject as SerializableActionDoor);
            else if (serializableObject is SerializableActionObject)
                AddSerializableActionObject(serializableObject as SerializableActionObject);
            else if (serializableObject is SerializableEnemy)
                AddSerializableEnemy(serializableObject as SerializableEnemy);
            else if (serializableObject is SerializableLootContainer)
                AddSerializableLootContainer(serializableObject as SerializableLootContainer);
        }

        /// <summary>
        /// Deregister ISerializableGameObject from SaveLoadManager.
        /// </summary>
        public static void DeregisterSerializableGameObject(ISerializableGameObject serializableObject)
        {
            if (sceneUnloaded)
                return;

            if (serializableObject.LoadID == 0)
                throw new Exception(invalidLoadIDExceptionText);

            if (serializableObject is SerializableActionDoor)
                Instance.serializableActionDoors.Remove(serializableObject.LoadID);
            else if (serializableObject is SerializableActionObject)
                Instance.serializableActionObjects.Remove(serializableObject.LoadID);
            else if (serializableObject is SerializableEnemy)
                Instance.serializableEnemies.Remove(serializableObject.LoadID);
            else if (serializableObject is SerializableEnemy)
                Instance.serializableLootContainers.Remove(serializableObject.LoadID);
        }

        /// <summary>
        /// Force deregister all ISerializableGameObject instances from SaveLoadManager.
        /// </summary>
        public static void DeregisterAllSerializableGameObjects(bool keepPlayer = true)
        {
            if (sceneUnloaded)
                return;

            // Optionally deregister player
            if (!keepPlayer)
                Instance.serializablePlayer = null;

            // Deregister other objects
            Instance.serializableActionDoors.Clear();
            Instance.serializableActionObjects.Clear();
            Instance.serializableEnemies.Clear();
            Instance.serializableLootContainers.Clear();
        }

        #endregion

        #region Private Static Methods

        private static void AddSerializableActionDoor(SerializableActionDoor serializableObject)
        {
            if (Instance.serializableActionDoors.ContainsKey(serializableObject.LoadID))
            {
                string message = string.Format(duplicateLoadIDErrorText, "AddSerializableActionDoor()", serializableObject.LoadID);
                DaggerfallUnity.LogMessage(message);
                return;
            }

            Instance.serializableActionDoors.Add(serializableObject.LoadID, serializableObject);
        }

        private static void AddSerializableActionObject(SerializableActionObject serializableObject)
        {
            if (Instance.serializableActionObjects.ContainsKey(serializableObject.LoadID))
            {
                string message = string.Format(duplicateLoadIDErrorText, "AddSerializableActionObject()", serializableObject.LoadID);
                DaggerfallUnity.LogMessage(message);
                return;
            }

            Instance.serializableActionObjects.Add(serializableObject.LoadID, serializableObject);
        }

        private static void AddSerializableEnemy(SerializableEnemy serializableObject)
        {
            if (Instance.serializableEnemies.ContainsKey(serializableObject.LoadID))
            {
                string message = string.Format(duplicateLoadIDErrorText, "AddSerializableEnemy()", serializableObject.LoadID);
                DaggerfallUnity.LogMessage(message);
                return;
            }

            Instance.serializableEnemies.Add(serializableObject.LoadID, serializableObject);
        }

        private static void AddSerializableLootContainer(SerializableLootContainer serializableObject)
        {
            if (Instance.serializableLootContainers.ContainsKey(serializableObject.LoadID))
            {
                string message = string.Format(duplicateLoadIDErrorText, "AddSerializableLootContainer()", serializableObject.LoadID);
                DaggerfallUnity.LogMessage(message);
                return;
            }

            Instance.serializableLootContainers.Add(serializableObject.LoadID, serializableObject);
        }

        #endregion

        #region Serialization Helpers

        static readonly fsSerializer _serializer = new fsSerializer();

        public static string Serialize(Type type, object value, bool pretty = true)
        {
            // Serialize the data
            fsData data;
            _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

            // Emit the data via JSON
            return (pretty) ? fsJsonPrinter.PrettyJson(data) : fsJsonPrinter.CompressedJson(data);
        }

        public static object Deserialize(Type type, string serializedState)
        {
            // Step 1: Parse the JSON data
            fsData data = fsJsonParser.Parse(serializedState);

            // Step 2: Deserialize the data
            object deserialized = null;
            _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

            return deserialized;
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
                    DaggerfallUnity.LogMessage("Multiple SaveLoad instances detected in scene!", true);
                    Destroy(gameObject);
                }
            }
        }

        string GetUnitySavePath()
        {
            if (!string.IsNullOrEmpty(unitySavePath))
                return unitySavePath;

            string result = string.Empty;

            // Try settings
            result = DaggerfallUnity.Settings.MyDaggerfallUnitySavePath;
            if (string.IsNullOrEmpty(result) || !Directory.Exists(result))
            {
                // Default to dataPath
                result = Path.Combine(Application.persistentDataPath, rootSaveFolder);
                if (!Directory.Exists(result))
                {
                    // Attempt to create path
                    Directory.CreateDirectory(result);
                }
            }

            // Test result is a valid path
            if (!Directory.Exists(result))
                throw new Exception("Could not locate valid path for Unity save files. Check 'MyDaggerfallUnitySavePath' in settings.ini.");

            // Log result and save path
            DaggerfallUnity.LogMessage(string.Format("Using path '{0}' for Unity saves.", result), true);
            unitySavePath = result;

            return result;
        }

        string GetDaggerfallSavePath()
        {
            if (!string.IsNullOrEmpty(daggerfallSavePath))
                return daggerfallSavePath;

            string result = string.Empty;

            // Test result is a valid path
            result = Path.GetDirectoryName(DaggerfallUnity.Instance.Arena2Path);
            if (!Directory.Exists(result))
                throw new Exception("Could not locate valid path for Daggerfall save files. Check 'MyDaggerfallPath' in settings.ini points to your Daggerfall folder.");

            // Log result and save path
            DaggerfallUnity.LogMessage(string.Format("Using path '{0}' for Daggerfall save importing.", result), true);
            daggerfallSavePath = result;

            return result;
        }

        void WriteSaveFile(string path, string json)
        {
            File.WriteAllText(path, json);
        }

        string ReadSaveFile(string path)
        {
            return File.ReadAllText(path);
        }

        #endregion

        #region Saving

        SaveData_v1 BuildSaveData()
        {
            SaveData_v1 saveData = new SaveData_v1();
            saveData.header = new SaveDataDescription_v1();
            saveData.currentUID = DaggerfallUnity.CurrentUID;
            saveData.dateAndTime = GetDateTimeData();
            saveData.playerData = GetPlayerData();
            saveData.dungeonData = GetDungeonData();
            saveData.enemyData = GetEnemyData();
            saveData.lootContainers = GetLootContainerData();

            return saveData;
        }

        PlayerData_v1 GetPlayerData()
        {
            if (!serializablePlayer)
                return null;

            return (PlayerData_v1)serializablePlayer.GetSaveData();
        }

        DateAndTime_v1 GetDateTimeData()
        {
            DateAndTime_v1 data = new DateAndTime_v1();
            data.gameTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds();
            data.realTime = DateTime.Now.Ticks;

            return data;
        }

        DungeonData_v1 GetDungeonData()
        {
            DungeonData_v1 data = new DungeonData_v1();
            data.actionDoors = GetActionDoorData();
            data.actionObjects = GetActionObjectData();

            return data;
        }

        ActionDoorData_v1[] GetActionDoorData()
        {
            List<ActionDoorData_v1> actionDoors = new List<ActionDoorData_v1>();

            foreach (var value in serializableActionDoors.Values)
            {
                if (value.ShouldSave)
                    actionDoors.Add((ActionDoorData_v1)value.GetSaveData());
            }

            return actionDoors.ToArray();
        }

        ActionObjectData_v1[] GetActionObjectData()
        {
            List<ActionObjectData_v1> actionObjects = new List<ActionObjectData_v1>();

            foreach (var value in serializableActionObjects.Values)
            {
                if (value.ShouldSave)
                    actionObjects.Add((ActionObjectData_v1)value.GetSaveData());
            }

            return actionObjects.ToArray();
        }

        EnemyData_v1[] GetEnemyData()
        {
            List<EnemyData_v1> enemies = new List<EnemyData_v1>();

            foreach (var value in serializableEnemies.Values)
            {
                if (value.ShouldSave)
                    enemies.Add((EnemyData_v1)value.GetSaveData());
            }

            return enemies.ToArray();
        }

        LootContainerData_v1[] GetLootContainerData()
        {
            List<LootContainerData_v1> containers = new List<LootContainerData_v1>();

            foreach (var value in serializableLootContainers.Values)
            {
                if (value.ShouldSave)
                    containers.Add((LootContainerData_v1)value.GetSaveData());
            }

            return containers.ToArray();
        }

        #endregion

        #region Loading

        void RestoreSaveData(SaveData_v1 saveData)
        {
            DaggerfallUnity.CurrentUID = saveData.currentUID;
            RestoreDateTimeData(saveData.dateAndTime);
            RestorePlayerData(saveData.playerData);
            RestoreDungeonData(saveData.dungeonData);
            RestoreEnemyData(saveData.enemyData);
            RestoreLootContainerData(saveData.lootContainers);
        }

        void RestoreDateTimeData(DateAndTime_v1 dateTimeData)
        {
            if (dateTimeData == null)
                return;

            DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.FromSeconds(dateTimeData.gameTime);
        }

        void RestorePlayerData(PlayerData_v1 playerData)
        {
            if (playerData == null)
                return;

            if (serializablePlayer)
                serializablePlayer.RestoreSaveData(playerData);
        }

        void RestoreDungeonData(DungeonData_v1 dungeonData)
        {
            if (dungeonData == null)
                return;

            RestoreActionDoorData(dungeonData.actionDoors);
            RestoreActionObjectData(dungeonData.actionObjects);
        }

        void RestoreActionDoorData(ActionDoorData_v1[] actionDoors)
        {
            if (actionDoors == null || actionDoors.Length == 0)
                return;

            for(int i = 0; i < actionDoors.Length; i++)
            {
                ulong key = actionDoors[i].loadID;
                if (serializableActionDoors.ContainsKey(key))
                {
                    serializableActionDoors[key].RestoreSaveData(actionDoors[i]);
                }
            }
        }

        void RestoreActionObjectData(ActionObjectData_v1[] actionObjects)
        {
            if (actionObjects == null || actionObjects.Length == 0)
                return;

            for (int i = 0; i < actionObjects.Length; i++)
            {
                ulong key = actionObjects[i].loadID;
                if (serializableActionObjects.ContainsKey(key))
                {
                    serializableActionObjects[key].RestoreSaveData(actionObjects[i]);
                }
            }
        }

        void RestoreEnemyData(EnemyData_v1[] enemies)
        {
            if (enemies == null || enemies.Length == 0)
                return;

            for (int i = 0; i < enemies.Length; i++)
            {
                ulong key = enemies[i].loadID;
                if (serializableEnemies.ContainsKey(key))
                {
                    serializableEnemies[key].RestoreSaveData(enemies[i]);
                }
            }
        }

        void RestoreLootContainerData(LootContainerData_v1[] lootContainers)
        {
            if (lootContainers == null || lootContainers.Length == 0)
                return;

            for (int i = 0; i < lootContainers.Length; i++)
            {
                ulong key = lootContainers[i].loadID;
                if (serializableLootContainers.ContainsKey(key))
                {
                    serializableLootContainers[key].RestoreSaveData(lootContainers[i]);
                }
            }
        }

        #endregion

        #region Utility

        IEnumerator SaveGame(string saveName)
        {
            // Build save data
            SaveData_v1 saveData = BuildSaveData();

            // Serialize save data to JSON strings
            string saveDataJson = Serialize(saveData.GetType(), saveData);
            //string persistentLootDataJson = 

            // Create screenshot for save
            yield return new WaitForEndOfFrame();
            Texture2D screenshot = new Texture2D(Screen.width, Screen.height);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();

            // Get destination save path
            string dstPath = GetDestinationSavePath(saveData.playerData.playerEntity.name, saveName);

            // Save data to file
            WriteSaveFile(Path.Combine(dstPath, saveDataFilename), saveDataJson);

            // Save screenshot
            byte[] bytes = screenshot.EncodeToJPG();
            File.WriteAllBytes(Path.Combine(dstPath, screenshotFilename), bytes);

            // Raise OnSaveEvent
            RaiseOnSaveEvent(saveData);

            // Notify
            DaggerfallUI.Instance.PopupMessage(HardStrings.gameSaved);
        }

        IEnumerator LoadGame(SaveData_v1 saveData)
        {
            // Must have a serializable player
            if (!serializablePlayer)
                yield break;

            // Immediately set date so world is loaded with correct season
            RestoreDateTimeData(saveData.dateAndTime);

            // Must have PlayerEnterExit to respawn player at saved location
            PlayerEnterExit playerEnterExit = serializablePlayer.GetComponent<PlayerEnterExit>();
            if (!playerEnterExit)
                yield break;

            // Check exterior doors are included in save, we need these to exit building
            bool hasExteriorDoors;
            if (saveData.playerData.playerPosition.exteriorDoors == null || saveData.playerData.playerPosition.exteriorDoors.Length == 0)
                hasExteriorDoors = false;
            else
                hasExteriorDoors = true;

            // Raise reposition flag if terrain sampler changed
            // This is required as changing terrain samplers will invalidate serialized player coordinates
            bool repositionPlayer = false;
            if (saveData.playerData.playerPosition.terrainSamplerName != DaggerfallUnity.Instance.TerrainSampler.ToString() ||
                saveData.playerData.playerPosition.terrainSamplerVersion != DaggerfallUnity.Instance.TerrainSampler.Version)
            {
                repositionPlayer = true;
                if (DaggerfallUI.Instance.DaggerfallHUD != null)
                    DaggerfallUI.Instance.DaggerfallHUD.PopupText.AddText("Terrain sampler changed. Repositioning player.");
            }

            // Raise reposition flag if player is supposed to start indoors but building has no doors
            if (saveData.playerData.playerPosition.insideBuilding && !hasExteriorDoors)
            {
                repositionPlayer = true;
                if (DaggerfallUI.Instance.DaggerfallHUD != null)
                    DaggerfallUI.Instance.DaggerfallHUD.PopupText.AddText("Building has no exterior doors. Repositioning player.");
            }

            // Start the respawn process based on saved player location
            if (saveData.playerData.playerPosition.insideDungeon && !repositionPlayer)
            {
                // Start in dungeon
                playerEnterExit.RespawnPlayer(
                    saveData.playerData.playerPosition.worldPosX,
                    saveData.playerData.playerPosition.worldPosZ,
                    true);
            }
            else if (saveData.playerData.playerPosition.insideBuilding && hasExteriorDoors && !repositionPlayer)
            {
                // Start in building
                playerEnterExit.RespawnPlayer(
                    saveData.playerData.playerPosition.worldPosX,
                    saveData.playerData.playerPosition.worldPosZ,
                    saveData.playerData.playerPosition.insideDungeon,
                    saveData.playerData.playerPosition.insideBuilding,
                    saveData.playerData.playerPosition.exteriorDoors);
            }
            else
            {
                // Start outside
                playerEnterExit.RespawnPlayer(
                    saveData.playerData.playerPosition.worldPosX,
                    saveData.playerData.playerPosition.worldPosZ,
                    false,
                    false,
                    null,
                    repositionPlayer);
            }

            // Keep yielding frames until world is ready again
            while (playerEnterExit.IsRespawning)
            {
                yield return new WaitForEndOfFrame();
            }

            // Wait another frame so everthing has a chance to register
            yield return new WaitForEndOfFrame();

            // Restore save data to objects in newly spawned world
            RestoreSaveData(saveData);

            // Raise OnLoad event
            RaiseOnLoadEvent(saveData);
        }

        #endregion

        #region Events

        // OnSave
        public delegate void OnSaveEventHandler(SaveData_v1 saveData);
        public static event OnSaveEventHandler OnSave;
        protected virtual void RaiseOnSaveEvent(SaveData_v1 saveData)
        {
            if (OnSave != null)
                OnSave(saveData);
        }

        // OnLoad
        public delegate void OnLoadEventHandler(SaveData_v1 saveData);
        public static event OnLoadEventHandler OnLoad;
        protected virtual void RaiseOnLoadEvent(SaveData_v1 saveData)
        {
            if (OnLoad != null)
                OnLoad(saveData);
        }

        #endregion
    }
}