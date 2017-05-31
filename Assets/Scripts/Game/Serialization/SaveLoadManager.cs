// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Lypyl (lypyldf@gmail.com)
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
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Serialization
{
    /// <summary>
    /// Implements save/load logic.
    /// Games are saved in PersistentDataPath\Saves.
    /// Each save game will have a screenshot and multiple files.
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        #region Fields

        const int latestSaveVersion = 1;

        const string rootSaveFolder = "Saves";
        const string savePrefix = "SAVE";
        const string quickSaveName = "QuickSave";
        const string autoSaveName = "AutoSave";
        const string saveInfoFilename = "SaveInfo.txt";
        const string saveDataFilename = "SaveData.txt";
        const string factionDataFilename = "FactionData.txt";
        const string containerDataFilename = "ContainerData.txt";
        const string automapDataFilename = "AutomapData.txt";
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

        // Enumerated save info
        Dictionary<int, string> enumeratedSaveFolders = new Dictionary<int, string>();
        Dictionary<int, SaveInfo_v1> enumeratedSaveInfo = new Dictionary<int, SaveInfo_v1>();
        Dictionary<string, List<int>> enumeratedCharacterSaves = new Dictionary<string, List<int>>();

        string unitySavePath = string.Empty;
        string daggerfallSavePath = string.Empty;
        bool loadInProgress = false;

        #endregion

        #region Properties

        public int LatestSaveVersion
        {
            get { return latestSaveVersion; }
        }

        public string UnitySavePath
        {
            get { return GetUnitySavePath(); }
        }

        public string DaggerfallSavePath
        {
            get { return GetDaggerfallSavePath(); }
        }

        public int CharacterCount
        {
            get { return enumeratedCharacterSaves.Count; }
        }

        public string[] CharacterNames
        {
            get { return GetCharacterNames(); }
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

        /// <summary>
        /// Checks if save/load system is ready.
        /// </summary>
        /// <returns>True if ready.</returns>
        public bool IsReady()
        {
            if (!DaggerfallUnity.Instance.IsReady || !DaggerfallUnity.Instance.IsPathValidated)
                return false;

            return true;
        }

        /// <summary>
        /// Updates save game enumerations.
        /// Must call this before working with existing saves.
        /// For example, this is called in save UI every time window pushed to stack.
        /// </summary>
        public void EnumerateSaves()
        {
            enumeratedSaveFolders = EnumerateSaveFolders();
            enumeratedSaveInfo = EnumerateSaveInfo(enumeratedSaveFolders);
            enumeratedCharacterSaves = EnumerateCharacterSaves(enumeratedSaveInfo);
        }

        /// <summary>
        /// Gets array of save keys for the specified character.
        /// </summary>
        /// <param name="characterName">Name of character.</param>
        /// <returns>Array of save keys, excluding </returns>
        public int[] GetCharacterSaveKeys(string characterName)
        {
            if (!enumeratedCharacterSaves.ContainsKey(characterName))
                return new int[0];

            return enumeratedCharacterSaves[characterName].ToArray();
        }

        public string[] GetCharacterNames()
        {
            List<string> names = new List<string>();
            foreach(var kvp in enumeratedCharacterSaves)
            {
                names.Add(kvp.Key);
            }

            return names.ToArray();
        }

        /// <summary>
        /// Gets folder containing save by key.
        /// </summary>
        /// <param name="key">Save key.</param>
        /// <returns>Path to save folder or empty string if key not found.</returns>
        public string GetSaveFolder(int key)
        {
            if (!enumeratedSaveFolders.ContainsKey(key))
                return string.Empty;

            return enumeratedSaveFolders[key];
        }

        /// <summary>
        /// Gets save information by key.
        /// </summary>
        /// <param name="key">Save key.</param>
        /// <returns>SaveInfo populated with save details, or empty struct if save not found.</returns>
        public SaveInfo_v1 GetSaveInfo(int key)
        {
            if (!enumeratedSaveInfo.ContainsKey(key))
                return new SaveInfo_v1();

            return enumeratedSaveInfo[key];
        }

        public Texture2D GetSaveScreenshot(int key)
        {
            if (!enumeratedSaveFolders.ContainsKey(key))
                return null;

            string path = Path.Combine(GetSaveFolder(key), screenshotFilename);
            byte[] data = File.ReadAllBytes(path);

            Texture2D screenshot = new Texture2D(0, 0);
            if (screenshot.LoadImage(data))
                return screenshot;

            return null;
        }

        /// <summary>
        /// Finds existing save folder.
        /// </summary>
        /// <param name="characterName">Name of character to match.</param>
        /// <param name="saveName">Name of save to match.</param>
        /// <returns>Save key or -1 if save not found.</returns>
        public int FindSaveFolderByNames(string characterName, string saveName)
        {
            int[] saves = GetCharacterSaveKeys(characterName);
            foreach (int key in saves)
            {
                SaveInfo_v1 compareInfo = GetSaveInfo(key);
                if (compareInfo.characterName == characterName &&
                    compareInfo.saveName == saveName)
                {
                    return key;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds most recent save.
        /// </summary>
        /// <returns>Save key of most recent save, or -1 if no saves found.</returns>
        public int FindMostRecentSave()
        {
            long mostRecentTime = -1;
            int mostRecentKey = -1;
            foreach (var kvp in enumeratedSaveInfo)
            {
                if (kvp.Value.dateAndTime.realTime > mostRecentTime)
                {
                    mostRecentTime = kvp.Value.dateAndTime.realTime;
                    mostRecentKey = kvp.Key;
                }
            }

            return mostRecentKey;
        }

        /// <summary>
        /// Deletes save folder.
        /// </summary>
        /// <param name="key">Save key.</param>
        public void DeleteSaveFolder(int key)
        {
            if (!enumeratedSaveFolders.ContainsKey(key))
                return;

            // For safety only delete known save files - do not perform a recursive delete
            // This way we don't blow up folder if user has placed something custom inside
            string path = GetSaveFolder(key);
            File.Delete(Path.Combine(path, saveDataFilename));
            File.Delete(Path.Combine(path, saveInfoFilename));
            File.Delete(Path.Combine(path, screenshotFilename));
            File.Delete(Path.Combine(path, containerDataFilename));
            File.Delete(Path.Combine(path, automapDataFilename));

            // Attempt to delete path itself
            // Even if delete fails path should be invalid with save info removed
            // Folder index will be excluded from enumeration and recycled later
            try
            {
                Directory.Delete(path);
            }
            catch(Exception ex)
            {
                string message = string.Format("Could not delete save folder '{0}'. Exception message: {1}", path, ex.Message);
                DaggerfallUnity.LogMessage(message);
            }

            // Update saves
            EnumerateSaves();
        }

        public void Save(string characterName, string saveName)
        {
            // Must be ready
            if (!IsReady())
                throw new Exception(notReadyExceptionText);

            // Look for existing save with this character and name
            int key = FindSaveFolderByNames(characterName, saveName);

            // Get or create folder
            string path;
            if (key == -1)
                path = CreateNewSavePath(enumeratedSaveFolders);
            else
                path = GetSaveFolder(key);

            // Save game
            StartCoroutine(SaveGame(saveName, path));
        }

        public void QuickSave()
        {
            Save(GameManager.Instance.PlayerEntity.Name, quickSaveName);
        }

        public void Load(int key)
        {
            // Must be ready
            if (!IsReady())
                throw new Exception(notReadyExceptionText);

            // Load must not be in progress
            if (loadInProgress)
                return;

            // Get folder
            string path;
            if (key == -1)
                return;
            else
                path = GetSaveFolder(key);

            // Load game
            loadInProgress = true;
            GameManager.Instance.PauseGame(false);
            StartCoroutine(LoadGame(path));

            // Notify
            DaggerfallUI.Instance.PopupMessage(HardStrings.gameLoaded);
        }

        public void Load(string characterName, string saveName)
        {
            //// Must be ready
            //if (!IsReady())
            //    throw new Exception(notReadyExceptionText);

            //// Load must not be in progress
            //if (loadInProgress)
            //    return;

            // Look for existing save with this character and name
            int key = FindSaveFolderByNames(characterName, saveName);
            Load(key);

            //// Get folder
            //string path;
            //if (key == -1)
            //    return;
            //else
            //    path = GetSaveFolder(key);

            //// Load game
            //loadInProgress = true;
            //GameManager.Instance.PauseGame(false);
            //StartCoroutine(LoadGame(path));

            //// Notify
            //DaggerfallUI.Instance.PopupMessage(HardStrings.gameLoaded);
        }

        public void QuickLoad()
        {
            Load(GameManager.Instance.PlayerEntity.Name, quickSaveName);
        }

        /// <summary>
        /// Checks if quick save folder exists.
        /// </summary>
        /// <returns>True if quick save exists.</returns>
        public bool HasQuickSave(string characterName)
        {
            // Look for existing save with this character and name
            int key = FindSaveFolderByNames(characterName, quickSaveName);

            // Get folder
            return key != -1;
        }

        #endregion

        #region Public Static Methods

        public static bool FindSingleton(out SaveLoadManager singletonOut)
        {
            singletonOut = FindObjectOfType(typeof(SaveLoadManager)) as SaveLoadManager;
            return singletonOut != null;
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
            else if (serializableObject is SerializableLootContainer)
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
            try
            {
                return File.ReadAllText(path);
            }
            catch(Exception ex)
            {
                DaggerfallUnity.LogMessage(ex.Message);
                return string.Empty;
            }
        }

        Dictionary<int, string> EnumerateSaveFolders()
        {
            // Get directories in save path matching prefix
            string[] directories = Directory.GetDirectories(UnitySavePath, savePrefix + "*", SearchOption.TopDirectoryOnly);

            // Build dictionary keyed by save index
            Dictionary<int, string> saveFolders = new Dictionary<int, string>();
            foreach (string directory in directories)
            {
                // Get everything right of prefix in folder name (should be a number)
                int key;
                string indexStr = Path.GetFileName(directory).Substring(savePrefix.Length);
                if (int.TryParse(indexStr, out key))
                {
                    // Must contain a save info file to be a valid save folder
                    if (File.Exists(Path.Combine(directory, saveInfoFilename)))
                        saveFolders.Add(key, directory);
                }
            }

            return saveFolders;
        }

        Dictionary<int, SaveInfo_v1> EnumerateSaveInfo(Dictionary<int, string> saveFolders)
        {
            Dictionary<int, SaveInfo_v1> saveInfoDict = new Dictionary<int, SaveInfo_v1>();
            foreach (var kvp in saveFolders)
            {
                try
                {
                    SaveInfo_v1 saveInfo = ReadSaveInfo(kvp.Value);
                    saveInfoDict.Add(kvp.Key, saveInfo);
                }
                catch (Exception ex)
                {
                    DaggerfallUnity.LogMessage(string.Format("Failed to read {0} in save folder {1}. Exception.Message={2}", saveInfoFilename, kvp.Value, ex.Message));
                }
            }

            return saveInfoDict;
        }

        Dictionary<string, List<int>> EnumerateCharacterSaves(Dictionary<int, SaveInfo_v1> saveInfo)
        {
            Dictionary<string, List<int>> characterSaves = new Dictionary<string, List<int>>();
            foreach (var kvp in saveInfo)
            {
                // Add character to name dictionary
                if (!characterSaves.ContainsKey(kvp.Value.characterName))
                {
                    characterSaves.Add(kvp.Value.characterName, new List<int>());
                }

                // Add save key to character save list
                characterSaves[kvp.Value.characterName].Add(kvp.Key);
            }

            return characterSaves;
        }

        SaveInfo_v1 ReadSaveInfo(string saveFolder)
        {
            string saveInfoJson = ReadSaveFile(Path.Combine(saveFolder, saveInfoFilename));
            SaveInfo_v1 saveInfo = Deserialize(typeof(SaveInfo_v1), saveInfoJson) as SaveInfo_v1;

            return saveInfo;
        }

        /// <summary>
        /// Checks if save folder exists.
        /// </summary>
        /// <param name="folderName">Folder name of save.</param>
        /// <returns>True if folder exists.</returns>
        bool HasSaveFolder(string folderName)
        {
            return Directory.Exists(Path.Combine(UnitySavePath, folderName));
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
            saveData.bankAccounts = GetBankData();

            return saveData;
        }

        PlayerData_v1 GetPlayerData()
        {
            if (!serializablePlayer)
                return null;

            return (PlayerData_v1)serializablePlayer.GetSaveData();
        }

        FactionData_v1 GetPlayerFactionData()
        {
            if (!serializablePlayer)
                return null;

            return (FactionData_v1)serializablePlayer.GetFactionSaveData();
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

        BankRecordData_v1[] GetBankData()
        {
            List<BankRecordData_v1> records = new List<BankRecordData_v1>();

            foreach (var record in Banking.DaggerfallBankManager.BankAccounts)
            {
                if (record == null)
                    continue;
                else if (record.accountGold == 0 && record.loanTotal == 0 && record.loanDueDate == 0)
                    continue;
                else
                    records.Add(record);
            }

            return records.ToArray();
        }

        /// <summary>
        /// Gets a specific save path.
        /// </summary>
        /// <param name="folderName">Folder name of save.</param>
        /// <param name="create">Creates folder if it does not exist.</param>
        /// <returns>Save path.</returns>
        string GetSavePath(string folderName, bool create)
        {
            // Compose folder path
            string path = Path.Combine(UnitySavePath, folderName);

            // Create directory if it does not exist
            if (create && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        /// <summary>
        /// Creates a new indexed save path.
        /// </summary>
        /// <param name="saveFolders">Save folder enumeration.</param>
        /// <returns>Save path.</returns>
        string CreateNewSavePath(Dictionary<int, string> saveFolders)
        {
            // Find first available save index in dictionary
            int key = 0;
            while (saveFolders.ContainsKey(key))
            {
                key++;
            }

            return GetSavePath(savePrefix + key, true);
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
            RestoreBankData(saveData.bankAccounts);
        }

        void RestoreFactionData(FactionData_v1 factionData)
        {
            if (factionData == null)
                return;

            if (serializablePlayer)
                serializablePlayer.RestoreFactionData(factionData);
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
                // Skip null containers
                if (lootContainers[i] == null)
                    continue;

                // Restore loot containers
                ulong key = lootContainers[i].loadID;
                if (serializableLootContainers.ContainsKey(key))
                {
                    // Apply to known loot container that is part of scene build
                    serializableLootContainers[key].RestoreSaveData(lootContainers[i]);
                }
                else
                {
                    // Add custom drop containers back to scene (e.g. dropped loot, slain foes)
                    if (lootContainers[i].customDrop)
                    {
                        DaggerfallLoot customLootContainer = GameObjectHelper.CreateDroppedLootContainer(GameManager.Instance.PlayerObject, key);
                        SerializableLootContainer serializableLootContainer = customLootContainer.GetComponent<SerializableLootContainer>();
                        if (serializableLootContainer)
                        {
                            serializableLootContainer.RestoreSaveData(lootContainers[i]);
                        }
                    }
                }
            }
        }

        void RestoreBankData(BankRecordData_v1 [] bankData)
        {
            Banking.DaggerfallBankManager.SetupAccounts();

            if (bankData == null)
                return;

            for (int i = 0; i < bankData.Length; i++)
            {
                if (bankData[i].regionIndex < 0 || bankData[i].regionIndex >= Banking.DaggerfallBankManager.BankAccounts.Length)
                    continue;

                Banking.DaggerfallBankManager.BankAccounts[bankData[i].regionIndex] = bankData[i];
            }
        }

        #endregion

        #region Utility

        IEnumerator SaveGame(string saveName, string path)
        {
            // Build save data
            SaveData_v1 saveData = BuildSaveData();

            // Build save info
            SaveInfo_v1 saveInfo = new SaveInfo_v1();
            saveInfo.saveVersion = LatestSaveVersion;
            saveInfo.saveName = saveName;
            saveInfo.characterName = saveData.playerData.playerEntity.name;
            saveInfo.dateAndTime = saveData.dateAndTime;

            // Build faction data
            FactionData_v1 factionData = GetPlayerFactionData();

            // Serialize save data to JSON strings
            string saveDataJson = Serialize(saveData.GetType(), saveData);
            string saveInfoJson = Serialize(saveInfo.GetType(), saveInfo);
            string factionDataJson = Serialize(factionData.GetType(), factionData);

            // Create screenshot for save
            // TODO: Hide UI for screenshot or use a different method
            yield return new WaitForEndOfFrame();
            Texture2D screenshot = new Texture2D(Screen.width, Screen.height);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();

            // Save data to files
            WriteSaveFile(Path.Combine(path, saveDataFilename), saveDataJson);
            WriteSaveFile(Path.Combine(path, saveInfoFilename), saveInfoJson);
            WriteSaveFile(Path.Combine(path, factionDataFilename), factionDataJson);

            // Save automap state
            try
            {
                Dictionary<string, DaggerfallAutomap.AutomapGeometryDungeonState> automapState = GameManager.Instance.Automap.GetState();
                string automapDataJson = Serialize(automapState.GetType(), automapState);
                WriteSaveFile(Path.Combine(path, automapDataFilename), automapDataJson);
            }
            catch(Exception ex)
            {
                string message = string.Format("Failed to save automap state. Message: {0}", ex.Message);
                Debug.Log(message);
            }

            // Save screenshot
            byte[] bytes = screenshot.EncodeToJPG();
            File.WriteAllBytes(Path.Combine(path, screenshotFilename), bytes);

            // Raise OnSaveEvent
            RaiseOnSaveEvent(saveData);

            // Notify
            DaggerfallUI.Instance.PopupMessage(HardStrings.gameSaved);
        }

        IEnumerator LoadGame(string path)
        {
            GameManager.Instance.PlayerDeath.ClearDeathAnimation();
            GameManager.Instance.PlayerMotor.CancelMovement = true;
            InputManager.Instance.ClearAllActions();

            // Read save data from files
            string saveDataJson = ReadSaveFile(Path.Combine(path, saveDataFilename));
            string factionDataJson = ReadSaveFile(Path.Combine(path, factionDataFilename));

            // Deserialize JSON strings
            SaveData_v1 saveData = Deserialize(typeof(SaveData_v1), saveDataJson) as SaveData_v1;

            // Must have a serializable player
            if (!serializablePlayer)
                yield break;

            // Call start load event
            RaiseOnStartLoadEvent(saveData);

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

            // Smash to black while respawning
            DaggerfallUI.Instance.SmashHUDToBlack();

            // Keep yielding frames until world is ready again
            while (playerEnterExit.IsRespawning)
            {
                yield return new WaitForEndOfFrame();
            }

            // Wait another frame so everthing has a chance to register
            yield return new WaitForEndOfFrame();

            // Restore save data to objects in newly spawned world
            RestoreSaveData(saveData);

            // Restore faction data to player entity
            if (!string.IsNullOrEmpty(factionDataJson))
            {
                FactionData_v1 factionData = Deserialize(typeof(FactionData_v1), factionDataJson) as FactionData_v1;
                RestoreFactionData(factionData);
                Debug.Log("LoadGame() restored faction state from save.");
            }
            else
            {
                Debug.Log("LoadGame() did not find saved faction data. Player will resume with default faction state.");
            }

            // Load automap state
            try
            {
                string automapDataJson = ReadSaveFile(Path.Combine(path, automapDataFilename));
                Dictionary<string, DaggerfallAutomap.AutomapGeometryDungeonState> automapState = null;

                if (!string.IsNullOrEmpty(automapDataJson))
                    automapState = Deserialize(typeof(Dictionary<string, DaggerfallAutomap.AutomapGeometryDungeonState>), automapDataJson) as Dictionary<string, DaggerfallAutomap.AutomapGeometryDungeonState>;

                if (automapState != null)
                    GameManager.Instance.Automap.SetState(automapState);
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed to load automap state. Message: {0}", ex.Message);
                Debug.Log(message);
            }

            // Lower load in progress flag
            loadInProgress = false;

            // Fade out from black
            DaggerfallUI.Instance.FadeHUDFromBlack(1.5f);

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

        // OnStartLoad
        public delegate void OnStartLoadEventHandler(SaveData_v1 saveData);
        public static event OnStartLoadEventHandler OnStartLoad;
        protected virtual void RaiseOnStartLoadEvent(SaveData_v1 saveData)
        {
            if (OnStartLoad != null)
                OnStartLoad(saveData);
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