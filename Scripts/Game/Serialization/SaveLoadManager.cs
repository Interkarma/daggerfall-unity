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
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        #region Fields

        const string saveFilename = "SaveData.txt";
        const string testSavePath = @"D:\Test\Saves\QuickSave";
        const string notReadyExceptionText = "SaveLoad not ready.";
        const string invalidLoadIDExceptionText = "serializableObject does not have a valid LoadID";

        // Serializable objects in scene
        SerializablePlayer serializablePlayer;
        Dictionary<long, SerializableActionDoor> serializableActionDoors = new Dictionary<long, SerializableActionDoor>();

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
        }

        static bool applicationQuit = false;
        void OnApplicationQuit()
        {
            applicationQuit = true;
        }

        #endregion

        #region Public Methods

        public bool IsReady()
        {
            if (!DaggerfallUnity.Instance.IsReady || !DaggerfallUnity.Instance.IsPathValidated)
                return false;

            return true;
        }

        public void QuickSave()
        {
            // Must be ready
            if (!IsReady())
                throw new Exception(notReadyExceptionText);

            // Build save data
            SaveData_v1 saveData = BuildSaveData();

            // Serialize save data to JSON string
            string json = Serialize(saveData.GetType(), saveData);

            // Save data to file
            WriteSaveFile(Path.Combine(testSavePath, saveFilename), json);
        }

        public void QuickLoad()
        {
            // Must be ready
            if (!IsReady())
                throw new Exception(notReadyExceptionText);

            // Read save data from file
            string json = ReadSaveFile(Path.Combine(testSavePath, saveFilename));

            // Deserialize JSON string to save data
            SaveData_v1 saveData = Deserialize(typeof(SaveData_v1), json) as SaveData_v1;

            // Restore save data
            StartCoroutine(FadeHUDBackground());
            StartCoroutine(LoadGame(saveData));
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
            if (applicationQuit)
                return;

            if (serializableObject.LoadID == 0)
                throw new Exception(invalidLoadIDExceptionText);

            if (serializableObject is SerializablePlayer)
                Instance.serializablePlayer = serializableObject as SerializablePlayer;
            else if (serializableObject is SerializableActionDoor)
                Instance.serializableActionDoors.Add(serializableObject.LoadID, serializableObject as SerializableActionDoor);
        }

        /// <summary>
        /// Deregister ISerializableGameObject from SaveLoadManager.
        /// </summary>
        public static void DeregisterSerializableGameObject(ISerializableGameObject serializableObject)
        {
            if (applicationQuit)
                return;

            if (serializableObject.LoadID == 0)
                throw new Exception(invalidLoadIDExceptionText);

            if (serializableObject is SerializableActionDoor)
                Instance.serializableActionDoors.Remove(serializableObject.LoadID);
        }

        /// <summary>
        /// Force deregister all ISerializableGameObject instances from SaveLoadManager.
        /// </summary>
        public static void DeregisterAllSerializableGameObjects(bool keepPlayer = true)
        {
            if (applicationQuit)
                return;

            // Optionally deregister player
            if (!keepPlayer)
                Instance.serializablePlayer = null;

            // Deregister other objects
            Instance.serializableActionDoors.Clear();
        }

        #endregion

        #region Private Static Methods

        static readonly fsSerializer _serializer = new fsSerializer();

        static string Serialize(Type type, object value, bool pretty = true)
        {
            // Serialize the data
            fsData data;
            _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

            // Emit the data via JSON
            return (pretty) ? fsJsonPrinter.PrettyJson(data) : fsJsonPrinter.CompressedJson(data);
        }

        static object Deserialize(Type type, string serializedState)
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
            saveData.playerData = GetPlayerData();
            saveData.dungeonData = GetDungeonData();

            return saveData;
        }

        PlayerData_v1 GetPlayerData()
        {
            if (!serializablePlayer)
                return null;

            return (PlayerData_v1)serializablePlayer.GetSaveData();
        }

        DungeonData_v1 GetDungeonData()
        {
            DungeonData_v1 data = new DungeonData_v1();
            data.actionDoors = GetActionDoorData();

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

        #endregion

        #region Loading

        void RestoreSaveData(SaveData_v1 saveData)
        {
            RestorePlayerData(saveData.playerData);
            RestoreDungeonData(saveData.dungeonData);
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
        }

        void RestoreActionDoorData(ActionDoorData_v1[] actionDoors)
        {
            if (actionDoors == null || actionDoors.Length == 0)
                return;

            for(int i = 0; i < actionDoors.Length; i++)
            {
                long key = actionDoors[i].loadID;
                if (serializableActionDoors.ContainsKey(key))
                {
                    serializableActionDoors[key].RestoreSaveData(actionDoors[i]);
                }
            }
        }

        #endregion

        #region Utility

        // Fades HUD background in from black to briefly hide world while loading
        IEnumerator FadeHUDBackground()
        {
            const float fadeStep = 0.01f;
            const float fadeDuration = 1.0f;

            // Must have PlayerEnterExit to respawn player at saved location
            PlayerEnterExit playerEnterExit = serializablePlayer.GetComponent<PlayerEnterExit>();
            if (!playerEnterExit)
                yield break;

            // Must have a HUD to fade
            DaggerfallHUD hud = DaggerfallUI.Instance.DaggerfallHUD;
            if (hud == null)
                yield break;

            // Setup fade
            Color startColor = Color.black;
            Color targetColor = hud.ParentPanel.BackgroundColor;
            hud.ParentPanel.BackgroundColor = startColor;

            // Progress fade
            float progress = 0;
            float increment = fadeStep / fadeDuration;
            while(progress < 1)
            {
                hud.ParentPanel.BackgroundColor = Color.Lerp(startColor, targetColor, progress);
                progress += increment;
                yield return new WaitForSeconds(fadeStep);
            }

            // Ensure starting colour is restored
            hud.ParentPanel.BackgroundColor = targetColor;
        }

        IEnumerator LoadGame(SaveData_v1 saveData)
        {
            // Must have a serializable player
            if (!serializablePlayer)
                yield break;

            // Must have PlayerEnterExit to respawn player at saved location
            PlayerEnterExit playerEnterExit = serializablePlayer.GetComponent<PlayerEnterExit>();
            if (!playerEnterExit)
                yield break;

            // Start the respawn process based on saved player location
            playerEnterExit.RespawnPlayer(saveData.playerData.worldPosX, saveData.playerData.worldPosZ, saveData.playerData.insideDungeon);

            // Keep yielding frames until world is ready again
            while (playerEnterExit.IsRespawning)
            {
                yield return new WaitForEndOfFrame();
            }

            // Wait another frame so everthing has a chance to register
            yield return new WaitForEndOfFrame();

            // Restore save data to objects in newly spawned world
            RestoreSaveData(saveData);
        }

        #endregion
    }
}