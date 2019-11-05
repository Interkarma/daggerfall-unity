// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility.AssetInjection;

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
        const string questDataFilename = "QuestData.txt";
        const string discoveryDataFilename = "DiscoveryData.txt";
        const string conversationDataFilename = "ConversationData.txt";
        const string notebookDataFilename = "NotebookData.txt";
        const string worldVariationDataFilename = "WorldVariationData.txt";
        const string automapDataFilename = "AutomapData.txt";
        const string questExceptionsFilename = "QuestExceptions.txt";
        const string screenshotFilename = "Screenshot.jpg";
        const string bioFileName = "bio.txt";
        const string notReadyExceptionText = "SaveLoad not ready.";

        // Serializable state manager for stateful game objects
        SerializableStateManager stateManager = new SerializableStateManager();

        // Enumerated save info
        Dictionary<int, string> enumeratedSaveFolders = new Dictionary<int, string>();
        Dictionary<int, SaveInfo_v1> enumeratedSaveInfo = new Dictionary<int, SaveInfo_v1>();
        Dictionary<string, List<int>> enumeratedCharacterSaves = new Dictionary<string, List<int>>();

        string unitySavePath = string.Empty;
        string daggerfallSavePath = string.Empty;
        bool loadInProgress = false;

        #endregion

        #region Properties

        public static SerializableStateManager StateManager
        {
            get { return Instance.stateManager; }
        }

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

        public bool LoadInProgress
        {
            get { return loadInProgress; }
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
            File.Delete(Path.Combine(path, questExceptionsFilename));
            File.Delete(Path.Combine(path, conversationDataFilename));
            File.Delete(Path.Combine(path, discoveryDataFilename));
            File.Delete(Path.Combine(path, factionDataFilename));
            File.Delete(Path.Combine(path, questDataFilename));
            File.Delete(Path.Combine(path, bioFileName));
            File.Delete(Path.Combine(path, notebookDataFilename));
            File.Delete(Path.Combine(path, worldVariationDataFilename));
            if (ModManager.Instance != null)
            {
                foreach (Mod mod in ModManager.Instance.GetAllModsWithSaveData())
                    File.Delete(Path.Combine(path, GetModDataFilename(mod)));
            }

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

        public void Save(string characterName, string saveName, bool instantReload = false)
        {
            // Must be ready
            if (!IsReady())
                throw new Exception(notReadyExceptionText);

            // Do nothing if load in progress
            if (LoadInProgress)
                return;

            // Save game
            StartCoroutine(SaveGame(characterName, saveName, instantReload));
        }

        public void QuickSave(bool instantReload = false)
        {
            if (!LoadInProgress)
                Save(GameManager.Instance.PlayerEntity.Name, quickSaveName, instantReload);
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
            singletonOut = FindObjectOfType<SaveLoadManager>();
            return singletonOut != null;
        }

        /// <summary>
        /// Register ISerializableGameObject with SerializableStateManager.
        /// </summary>
        public static void RegisterSerializableGameObject(ISerializableGameObject serializableObject)
        {
            if (sceneUnloaded)
                return;
            Instance.stateManager.RegisterStatefulGameObject(serializableObject);
        }

        /// <summary>
        /// Deregister ISerializableGameObject from SerializableStateManager.
        /// </summary>
        public static void DeregisterSerializableGameObject(ISerializableGameObject serializableObject)
        {
            if (sceneUnloaded)
                return;
            Instance.stateManager.DeregisterStatefulGameObject(serializableObject);
        }

        /// <summary>
        /// Force deregister all ISerializableGameObject instances from SerializableStateManager.
        /// </summary>
        public static void DeregisterAllSerializableGameObjects(bool keepPlayer = true)
        {
            if (sceneUnloaded)
                return;
            Instance.stateManager.DeregisterAllStatefulGameObjects(keepPlayer);
        }

        /// <summary>
        /// Stores the current scene in the SerializableStateManager cache using the given name.
        /// </summary>
        public static void CacheScene(string sceneName)
        {
            if (!sceneUnloaded)
                Instance.stateManager.CacheScene(sceneName);
        }

        /// <summary>
        /// Restores the current scene from the SerializableStateManager cache using the given name.
        /// </summary>
        public static void RestoreCachedScene(string sceneName)
        {
            if (!sceneUnloaded)
                Instance.StartCoroutine(Instance.RestoreCachedSceneNextFrame(sceneName));
        }

        private IEnumerator RestoreCachedSceneNextFrame(string sceneName)
        {
            // Wait another frame so everthing has a chance to register
            yield return new WaitForEndOfFrame();
            // Restore the scene from cache
            stateManager.RestoreCachedScene(sceneName);
        }

        /// <summary>
        /// Clears the SerializableStateManager scene cache.
        /// </summary>
        /// <param name="start">True if starting a new or loaded game, so also clear permanent scene list</param>
        public static void ClearSceneCache(bool start)
        {
            if (!sceneUnloaded)
                Instance.stateManager.ClearSceneCache(start);
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
            saveData.playerData = stateManager.GetPlayerData();
            saveData.dungeonData = GetDungeonData();
            saveData.enemyData = stateManager.GetEnemyData();
            saveData.lootContainers = stateManager.GetLootContainerData();
            saveData.bankAccounts = GetBankAccountData();
            saveData.bankDeeds = GetBankDeedData();
            saveData.escortingFaces = DaggerfallUI.Instance.DaggerfallHUD.EscortingFaces.GetSaveData();
            saveData.sceneCache = stateManager.GetSceneCache();
            saveData.travelMapData = DaggerfallUI.Instance.DfTravelMapWindow.GetTravelMapSaveData();

            return saveData;
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
            data.actionDoors = stateManager.GetActionDoorData();
            data.actionObjects = stateManager.GetActionObjectData();

            return data;
        }

        BankRecordData_v1[] GetBankAccountData()
        {
            List<BankRecordData_v1> records = new List<BankRecordData_v1>();

            foreach (var record in DaggerfallBankManager.BankAccounts)
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

        BankDeedData_v1 GetBankDeedData()
        {
            return new BankDeedData_v1() {
                shipType = (int) DaggerfallBankManager.OwnedShip,
                houses = GetHousesData(),
            };
        }

        HouseData_v1[] GetHousesData()
        {
            List<HouseData_v1> records = new List<HouseData_v1>();
            foreach (var record in DaggerfallBankManager.Houses)
            {
                if (record == null)
                    continue;
                else if (record.mapID == 0 && record.buildingKey == 0)
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
            stateManager.RestorePlayerData(saveData.playerData);
            RestoreDungeonData(saveData.dungeonData);
            stateManager.RestoreEnemyData(saveData.enemyData);
            stateManager.RestoreLootContainerData(saveData.lootContainers);
            RestoreBankData(saveData.bankAccounts);
            RestoreBankDeedData(saveData.bankDeeds);
            RestoreEscortingFacesData(saveData.escortingFaces);
            stateManager.RestoreSceneCache(saveData.sceneCache);
        }

        void RestoreDateTimeData(DateAndTime_v1 dateTimeData)
        {
            if (dateTimeData == null)
                return;

            DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.FromSeconds(dateTimeData.gameTime);
        }

        void RestoreDungeonData(DungeonData_v1 dungeonData)
        {
            if (dungeonData == null)
                return;

            stateManager.RestoreActionDoorData(dungeonData.actionDoors);
            stateManager.RestoreActionObjectData(dungeonData.actionObjects);
        }

        void RestoreBankData(BankRecordData_v1[] bankData)
        {
            DaggerfallBankManager.SetupAccounts();
            DaggerfallBankManager.SetupHouses();    // Covers case when loading old save with no house data

            if (bankData == null)
                return;

            for (int i = 0; i < bankData.Length; i++)
            {
                if (bankData[i].regionIndex < 0 || bankData[i].regionIndex >= DaggerfallBankManager.BankAccounts.Length)
                    continue;

                DaggerfallBankManager.BankAccounts[bankData[i].regionIndex] = bankData[i];
            }
        }

        void RestoreBankDeedData(BankDeedData_v1 deedData)
        {
            DaggerfallBankManager.OwnedShip = (deedData == null) ? ShipType.None : (ShipType) deedData.shipType;
            if (deedData != null)
                RestoreHousesData(deedData.houses);
        }

        void RestoreHousesData(HouseData_v1[] housesData)
        {
            DaggerfallBankManager.SetupHouses();

            if (housesData == null)
                return;

            for (int i = 0; i < housesData.Length; i++)
            {
                if (housesData[i].regionIndex < 0 || housesData[i].regionIndex >= DaggerfallBankManager.Houses.Length)
                    continue;

                DaggerfallBankManager.Houses[housesData[i].regionIndex] = housesData[i];
            }
        }

        void RestoreEscortingFacesData(FaceDetails[] escortingFaces)
        {
            if (DaggerfallUI.Instance.DaggerfallHUD == null)
                return;

            if (escortingFaces == null)
                DaggerfallUI.Instance.DaggerfallHUD.EscortingFaces.ClearFaces();
            else
                DaggerfallUI.Instance.DaggerfallHUD.EscortingFaces.RestoreSaveData(escortingFaces);
        }

        #endregion

        #region Utility

        IEnumerator SaveGame(string characterName, string saveName, bool instantReload = false)
        {
            // Look for existing save with this character and name
            int key = FindSaveFolderByNames(characterName, saveName);

            // Get or create folder
            string path;
            if (key == -1)
                path = CreateNewSavePath(enumeratedSaveFolders);
            else
                path = GetSaveFolder(key);

            // Build save data
            SaveData_v1 saveData = BuildSaveData();

            // Build save info
            SaveInfo_v1 saveInfo = new SaveInfo_v1();
            saveInfo.saveVersion = LatestSaveVersion;
            saveInfo.saveName = saveName;
            saveInfo.characterName = saveData.playerData.playerEntity.name;
            saveInfo.dateAndTime = saveData.dateAndTime;
            saveInfo.dfuVersion = VersionInfo.DaggerfallUnityVersion;

            // Build faction data
            FactionData_v2 factionData = stateManager.GetPlayerFactionData();

            // Build quest data
            QuestMachine.QuestMachineData_v1 questData = QuestMachine.Instance.GetSaveData();

            // Get discovery data
            Dictionary<int, PlayerGPS.DiscoveredLocation> discoveryData = GameManager.Instance.PlayerGPS.GetDiscoverySaveData();

            // Get conversation data
            TalkManager.SaveDataConversation conversationData = GameManager.Instance.TalkManager.GetConversationSaveData();

            // Get notebook data
            PlayerNotebook.NotebookData_v1 notebookData = GameManager.Instance.PlayerEntity.Notebook.GetNotebookSaveData();

            // Get WorldData Variants data
            WorldDataVariants.WorldVariationData_v1 worldVariationData = WorldDataVariants.GetWorldVariationSaveData();

            // Serialize save data to JSON strings
            string saveDataJson = Serialize(saveData.GetType(), saveData);
            string saveInfoJson = Serialize(saveInfo.GetType(), saveInfo);
            string factionDataJson = Serialize(factionData.GetType(), factionData);
            string questDataJson = Serialize(questData.GetType(), questData);
            string discoveryDataJson = Serialize(discoveryData.GetType(), discoveryData);
            string conversationDataJson = Serialize(conversationData.GetType(), conversationData);
            string notebookDataJson = Serialize(notebookData.GetType(), notebookData);
            string worldVariationDataJson = Serialize(worldVariationData.GetType(), worldVariationData);

            //// Attempt to hide UI for screenshot
            //bool rawImageEnabled = false;
            //UnityEngine.UI.RawImage rawImage = GUI.GetDiegeticCanvasRawImage();
            //if (rawImage)
            //{
            //    rawImageEnabled = rawImage.enabled;
            //    rawImage.enabled = false;
            //}

            // Create screenshot for save
            // TODO: Hide UI for screenshot or use a different method
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Texture2D screenshot = new Texture2D(Screen.width, Screen.height);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();

            //// Restore UI after screenshot
            //if (rawImageEnabled)
            //{
            //    rawImage.enabled = true;
            //}

            // Save data to files
            WriteSaveFile(Path.Combine(path, saveDataFilename), saveDataJson);
            WriteSaveFile(Path.Combine(path, saveInfoFilename), saveInfoJson);
            WriteSaveFile(Path.Combine(path, factionDataFilename), factionDataJson);
            WriteSaveFile(Path.Combine(path, questDataFilename), questDataJson);
            WriteSaveFile(Path.Combine(path, discoveryDataFilename), discoveryDataJson);
            WriteSaveFile(Path.Combine(path, conversationDataFilename), conversationDataJson);
            WriteSaveFile(Path.Combine(path, notebookDataFilename), notebookDataJson);
            WriteSaveFile(Path.Combine(path, worldVariationDataFilename), worldVariationDataJson);

            // Save quest exceptions
            QuestMachine.StoredException[] storedExceptions = QuestMachine.Instance.GetStoredExceptions();
            string questExceptionsJson = Serialize(storedExceptions.GetType(), storedExceptions);
            WriteSaveFile(Path.Combine(path, questExceptionsFilename), questExceptionsJson);

            // Save backstory text
            if (!File.Exists(Path.Combine(path, bioFileName)))
            {
                StreamWriter file = new StreamWriter(Path.Combine(path, bioFileName).ToString());
                foreach (string line in GameManager.Instance.PlayerEntity.BackStory)
                {
                    file.WriteLine(line);
                }
                file.Close();
            }

            // Save automap state
            try
            {
                Dictionary<string, Automap.AutomapDungeonState> automapState = GameManager.Instance.InteriorAutomap.GetState();
                string automapDataJson = Serialize(automapState.GetType(), automapState);
                WriteSaveFile(Path.Combine(path, automapDataFilename), automapDataJson);
            }
            catch(Exception ex)
            {
                string message = string.Format("Failed to save automap state. Message: {0}", ex.Message);
                Debug.Log(message);
            }

            // Save mod data
            if (ModManager.Instance != null)
            {
                foreach (Mod mod in ModManager.Instance.GetAllModsWithSaveData())
                {
                    object modData = mod.SaveDataInterface.GetSaveData();
                    if (modData != null)
                    {
                        string modDataJson = Serialize(modData.GetType(), modData);
                        WriteSaveFile(Path.Combine(path, GetModDataFilename(mod)), modDataJson);
                    }
                    else
                    {
                        File.Delete(Path.Combine(path, GetModDataFilename(mod)));
                    }
                }
            }

            // Save screenshot
            byte[] bytes = screenshot.EncodeToJPG();
            File.WriteAllBytes(Path.Combine(path, screenshotFilename), bytes);

            // Raise OnSaveEvent
            RaiseOnSaveEvent(saveData);

            // Update saves if needed
            if (key == -1)
                EnumerateSaves();

            // Notify
            DaggerfallUI.Instance.PopupMessage(HardStrings.gameSaved);

            // Reload this save instantly if requested
            if (instantReload)
                Load(saveData.playerData.playerEntity.name, saveName);
        }

        IEnumerator LoadGame(string path)
        {
            GameManager.Instance.PlayerDeath.ClearDeathAnimation();
            GameManager.Instance.PlayerMotor.CancelMovement = true;
            InputManager.Instance.ClearAllActions();
            QuestMachine.Instance.ClearState();
            stateManager.ClearSceneCache();
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            playerEntity.Reset();

            // Read save data from files
            string saveDataJson = ReadSaveFile(Path.Combine(path, saveDataFilename));
            string factionDataJson = ReadSaveFile(Path.Combine(path, factionDataFilename));
            string questDataJson = ReadSaveFile(Path.Combine(path, questDataFilename));
            string discoveryDataJson = ReadSaveFile(Path.Combine(path, discoveryDataFilename));
            string conversationDataJson = ReadSaveFile(Path.Combine(path, conversationDataFilename));
            string notebookDataJson = ReadSaveFile(Path.Combine(path, notebookDataFilename));
            string worldVariantsDataJson = ReadSaveFile(Path.Combine(path, worldVariationDataFilename));

            // Read quest exceptions
            if (File.Exists(Path.Combine(path, questExceptionsFilename)))
            {
                string questExceptionsJson = ReadSaveFile(Path.Combine(path, questExceptionsFilename));
                QuestMachine.StoredException[] storedExceptions = Deserialize(typeof(QuestMachine.StoredException[]), questExceptionsJson) as QuestMachine.StoredException[];
                QuestMachine.Instance.SetStoredExceptions(storedExceptions);
            }

            // Load backstory text
            playerEntity.BackStory = new List<string>();
            if (File.Exists(Path.Combine(path, bioFileName)))
            {
                StreamReader file = new StreamReader(Path.Combine(path, bioFileName).ToString());
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    playerEntity.BackStory.Add(line);
                }
                file.Close();
            }

            // Deserialize JSON strings
            SaveData_v1 saveData = Deserialize(typeof(SaveData_v1), saveDataJson) as SaveData_v1;

            // Must have a serializable player
            if (!stateManager.SerializablePlayer)
                yield break;

            // Call start load event
            RaiseOnStartLoadEvent(saveData);

            // Immediately set date so world is loaded with correct season
            RestoreDateTimeData(saveData.dateAndTime);

            // Restore discovery data
            if (!string.IsNullOrEmpty(discoveryDataJson))
            {
                Dictionary<int, PlayerGPS.DiscoveredLocation> discoveryData =
                    Deserialize(typeof(Dictionary<int, PlayerGPS.DiscoveredLocation>), discoveryDataJson) as Dictionary<int, PlayerGPS.DiscoveredLocation>;
                GameManager.Instance.PlayerGPS.RestoreDiscoveryData(discoveryData);
            }
            else
            {
                // Clear discovery data when not in save, or live state will be retained from previous session
                GameManager.Instance.PlayerGPS.ClearDiscoveryData();
            }

            // Must have PlayerEnterExit to respawn player at saved location
            PlayerEnterExit playerEnterExit = stateManager.SerializablePlayer.GetComponent<PlayerEnterExit>();
            if (!playerEnterExit)
                yield break;

            // Restore building summary, house ownership, and guild membership early for interior layout code
            if (saveData.playerData.playerPosition.insideBuilding)
            {
                playerEnterExit.BuildingDiscoveryData = saveData.playerData.playerPosition.buildingDiscoveryData;
                playerEnterExit.IsPlayerInsideOpenShop = saveData.playerData.playerPosition.insideOpenShop;
                if (saveData.bankDeeds != null)
                    RestoreHousesData(saveData.bankDeeds.houses);
                GameManager.Instance.GuildManager.RestoreMembershipData(saveData.playerData.guildMemberships);
            }

            // Restore faction data to player entity
            // This is done early as later objects may require faction information on restore
            if (!string.IsNullOrEmpty(factionDataJson))
            {
                FactionData_v2 factionData = Deserialize(typeof(FactionData_v2), factionDataJson) as FactionData_v2;
                stateManager.RestoreFactionData(factionData);
                Debug.Log("LoadGame() restored faction state from save.");
            }
            else
            {
                Debug.Log("LoadGame() did not find saved faction data. Player will resume with default faction state.");
            }

            // Restore quest machine state
            if (!string.IsNullOrEmpty(questDataJson))
            {
                QuestMachine.QuestMachineData_v1 questData = Deserialize(typeof(QuestMachine.QuestMachineData_v1), questDataJson) as QuestMachine.QuestMachineData_v1;
                QuestMachine.Instance.RestoreSaveData(questData);
            }

            // Restore conversation data (must be done after quest data restoration)
            if (!string.IsNullOrEmpty(conversationDataJson))
            {
                TalkManager.SaveDataConversation conversationData = Deserialize(typeof(TalkManager.SaveDataConversation), conversationDataJson) as TalkManager.SaveDataConversation;
                GameManager.Instance.TalkManager.RestoreConversationData(conversationData);
            }
            else
            {
                GameManager.Instance.TalkManager.RestoreConversationData(null);
            }

            // Restore notebook data
            if (!string.IsNullOrEmpty(notebookDataJson))
            {
                PlayerNotebook.NotebookData_v1 notebookData = Deserialize(typeof(PlayerNotebook.NotebookData_v1), notebookDataJson) as PlayerNotebook.NotebookData_v1;
                playerEntity.Notebook.RestoreNotebookData(notebookData);
            }

            // Restore WorldData variants data
            if (!string.IsNullOrEmpty(worldVariantsDataJson))
            {
                WorldDataVariants.WorldVariationData_v1 worldVariantsData = Deserialize(typeof(WorldDataVariants.WorldVariationData_v1), worldVariantsDataJson) as WorldDataVariants.WorldVariationData_v1;
                WorldDataVariants.RestoreWorldVariationData(worldVariantsData);
            }

            // Restore player position to world
            playerEnterExit.RestorePositionHelper(saveData.playerData.playerPosition, true, false);

            //Restore Travel Map settings
            DaggerfallUI.Instance.DfTravelMapWindow.SetTravelMapFromSaveData(saveData.travelMapData);

            // Smash to black while respawning
            DaggerfallUI.Instance.FadeBehaviour.SmashHUDToBlack();

            // Keep yielding frames until world is ready again
            while (playerEnterExit.IsRespawning)
            {
                yield return new WaitForEndOfFrame();
            }

            // Wait another frame so everthing has a chance to register
            yield return new WaitForEndOfFrame();

            // Restore save data to objects in newly spawned world
            RestoreSaveData(saveData);

            // Load automap state
            try
            {
                string automapDataJson = ReadSaveFile(Path.Combine(path, automapDataFilename));
                Dictionary<string, Automap.AutomapDungeonState> automapState = null;

                if (!string.IsNullOrEmpty(automapDataJson))
                    automapState = Deserialize(typeof(Dictionary<string, Automap.AutomapDungeonState>), automapDataJson) as Dictionary<string, Automap.AutomapDungeonState>;

                if (automapState != null)
                    GameManager.Instance.InteriorAutomap.SetState(automapState);
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed to load automap state. Message: {0}", ex.Message);
                Debug.Log(message);
            }

            // Clear any orphaned quest items
            RemoveAllOrphanedItems();

            // Check mod manager is available
            if (ModManager.Instance != null)
            {
                // Restore mod data
                foreach (Mod mod in ModManager.Instance.GetAllModsWithSaveData())
                {
                    string modDataPath = Path.Combine(path, GetModDataFilename(mod));
                    object modData;
                    if (File.Exists(modDataPath))
                        modData = Deserialize(mod.SaveDataInterface.SaveDataType, ReadSaveFile(modDataPath));
                    else
                        modData = mod.SaveDataInterface.NewSaveData();
                    mod.SaveDataInterface.RestoreSaveData(modData);
                }
            }

            // Clamp legal reputation
            playerEntity.ClampLegalReputations();

            // Lower load in progress flag
            loadInProgress = false;

            // Fade out from black
            DaggerfallUI.Instance.FadeBehaviour.FadeHUDFromBlack(1.0f);

            // Raise OnLoad event
            RaiseOnLoadEvent(saveData);
        }

        /// <summary>
        /// Looks for orphaned items (e.g. quest no longer active or invalid template) remaining in player item collections.
        /// </summary>
        void RemoveAllOrphanedItems()
        {
            int count = 0;
            Entity.PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            count += playerEntity.Items.RemoveOrphanedItems();
            count += playerEntity.WagonItems.RemoveOrphanedItems();
            count += playerEntity.OtherItems.RemoveOrphanedItems();
            if (count > 0)
            {
                Debug.LogFormat("Removed {0} orphaned items.", count);
            }
        }

        private static string GetModDataFilename(Mod mod)
        {
            // Use filename because title may contains invalid path chars.
            return string.Format("mod_{0}.txt", mod.FileName);
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