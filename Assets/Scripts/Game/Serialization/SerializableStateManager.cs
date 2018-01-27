// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Lypyl (lypyldf@gmail.com), Hazelnut
// 
// Notes:           Extracted from SaveLoadManager class
//

using UnityEngine;
using System;
using System.Collections.Generic;
using FullSerializer;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Serialization
{
    public enum StatefulGameObjectTypes
    {
        LootContainer,
        ActionDoor,
        ActionObject,
        Enemy
    }

    /// <summary>
    /// Manages stateful game objects. (implementations of ISerializableGameObject)
    /// Used by SaveLoadManager to serialize scene state.
    /// Uses a scene cache to persist building interiors & player owned areas.
    /// </summary>
    public class SerializableStateManager
    {
        #region Fields

        const string invalidLoadIDExceptionText = "serializableObject does not have a valid LoadID";
        const string duplicateLoadIDErrorText = "{0} detected duplicate LoadID {1}. This object will not be serialized.";

        // Serializable stateful game objects in current scene
        SerializablePlayer serializablePlayer;
        Dictionary<ulong, SerializableActionDoor> serializableActionDoors = new Dictionary<ulong, SerializableActionDoor>();
        Dictionary<ulong, SerializableActionObject> serializableActionObjects = new Dictionary<ulong, SerializableActionObject>();
        Dictionary<ulong, SerializableEnemy> serializableEnemies = new Dictionary<ulong, SerializableEnemy>();
        Dictionary<ulong, SerializableLootContainer> serializableLootContainers = new Dictionary<ulong, SerializableLootContainer>();

        #endregion

        #region Properties

        public SerializablePlayer SerializablePlayer
        {
            get { return serializablePlayer; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if a LoadID is already in enemy serialization list.
        /// </summary>
        public bool ContainsEnemy(ulong id)
        {
            return (serializableEnemies.ContainsKey(id));
        }

        /// <summary>
        /// Check if a LoadID is already in action door serialization list.
        /// </summary>
        public bool ContainsActionDoor(ulong id)
        {
            return (serializableActionDoors.ContainsKey(id));
        }

        /// <summary>
        /// Register ISerializableGameObject with SaveLoadManager.
        /// </summary>
        public void RegisterSerializableGameObject(ISerializableGameObject serializableObject)
        {
            if (serializableObject.LoadID == 0)
                throw new Exception(invalidLoadIDExceptionText);

            if (serializableObject is SerializablePlayer)
                serializablePlayer = serializableObject as SerializablePlayer;
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
        public void DeregisterSerializableGameObject(ISerializableGameObject serializableObject)
        {
            if (serializableObject.LoadID == 0)
                throw new Exception(invalidLoadIDExceptionText);

            if (serializableObject is SerializableActionDoor)
                serializableActionDoors.Remove(serializableObject.LoadID);
            else if (serializableObject is SerializableActionObject)
                serializableActionObjects.Remove(serializableObject.LoadID);
            else if (serializableObject is SerializableEnemy)
                serializableEnemies.Remove(serializableObject.LoadID);
            else if (serializableObject is SerializableLootContainer)
                serializableLootContainers.Remove(serializableObject.LoadID);
        }

        /// <summary>
        /// Force deregister all ISerializableGameObject instances from SaveLoadManager.
        /// </summary>
        public void DeregisterAllSerializableGameObjects(bool keepPlayer = true)
        {
            // Optionally deregister player
            if (!keepPlayer)
                serializablePlayer = null;

            // Deregister other objects
            serializableActionDoors.Clear();
            serializableActionObjects.Clear();
            serializableEnemies.Clear();
            serializableLootContainers.Clear();
        }

        #endregion

        #region Public Serialization Accessor Methods

        public PlayerData_v1 GetPlayerData()
        {
            if (!serializablePlayer)
                return null;

            return (PlayerData_v1)serializablePlayer.GetSaveData();
        }

        public FactionData_v2 GetPlayerFactionData()
        {
            if (!serializablePlayer)
                return null;

            return (FactionData_v2)serializablePlayer.GetFactionSaveData();
        }

        public ActionDoorData_v1[] GetActionDoorData()
        {
            List<ActionDoorData_v1> actionDoors = new List<ActionDoorData_v1>();

            foreach (var value in serializableActionDoors.Values)
            {
                if (value.ShouldSave)
                    actionDoors.Add((ActionDoorData_v1)value.GetSaveData());
            }

            return actionDoors.ToArray();
        }

        public ActionObjectData_v1[] GetActionObjectData()
        {
            List<ActionObjectData_v1> actionObjects = new List<ActionObjectData_v1>();

            foreach (var value in serializableActionObjects.Values)
            {
                if (value.ShouldSave)
                    actionObjects.Add((ActionObjectData_v1)value.GetSaveData());
            }

            return actionObjects.ToArray();
        }

        public EnemyData_v1[] GetEnemyData()
        {
            List<EnemyData_v1> enemies = new List<EnemyData_v1>();

            foreach (var value in serializableEnemies.Values)
            {
                if (value.ShouldSave)
                    enemies.Add((EnemyData_v1)value.GetSaveData());
            }

            return enemies.ToArray();
        }

        public LootContainerData_v1[] GetLootContainerData()
        {
            List<LootContainerData_v1> containers = new List<LootContainerData_v1>();

            foreach (var value in serializableLootContainers.Values)
            {
                if (value.ShouldSave)
                    containers.Add((LootContainerData_v1)value.GetSaveData());
            }

            return containers.ToArray();
        }

        public void RestorePlayerData(PlayerData_v1 playerData)
        {
            if (playerData == null)
                return;

            if (serializablePlayer)
                serializablePlayer.RestoreSaveData(playerData);
        }

        public void RestoreFactionData(FactionData_v2 factionData)
        {
            if (factionData == null)
                return;

            if (serializablePlayer)
                serializablePlayer.RestoreFactionData(factionData);
        }

        public void RestoreActionDoorData(ActionDoorData_v1[] actionDoors)
        {
            if (actionDoors == null || actionDoors.Length == 0)
                return;

            for (int i = 0; i < actionDoors.Length; i++)
            {
                ulong key = actionDoors[i].loadID;
                if (serializableActionDoors.ContainsKey(key))
                {
                    serializableActionDoors[key].RestoreSaveData(actionDoors[i]);
                }
            }
        }

        public void RestoreActionObjectData(ActionObjectData_v1[] actionObjects)
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

        public void RestoreEnemyData(EnemyData_v1[] enemies)
        {
            if (enemies == null || enemies.Length == 0)
                return;

            for (int i = 0; i < enemies.Length; i++)
            {
                // Create target GameObject
                GameObject go = GameObjectHelper.InstantiatePrefab(DaggerfallUnity.Instance.Option_EnemyPrefab.gameObject, enemies[i].gameObjectName, null, Vector3.zero);
                go.transform.parent = GameObjectHelper.GetSpawnParentTransform();

                // Set LoadID
                DaggerfallEnemy enemy = go.GetComponent<DaggerfallEnemy>();
                enemy.LoadID = enemies[i].loadID;

                // Restore save data
                SerializableEnemy serializableEnemy = go.GetComponent<SerializableEnemy>();
                serializableEnemy.RestoreSaveData(enemies[i]);
            }
        }

        public void RestoreLootContainerData(LootContainerData_v1[] lootContainers)
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

        #endregion

        #region Private Methods

        private void AddSerializableActionDoor(SerializableActionDoor serializableObject)
        {
            if (serializableActionDoors.ContainsKey(serializableObject.LoadID))
            {
                string message = string.Format(duplicateLoadIDErrorText, "AddSerializableActionDoor()", serializableObject.LoadID);
                DaggerfallUnity.LogMessage(message);
                return;
            }
            serializableActionDoors.Add(serializableObject.LoadID, serializableObject);
        }

        private void AddSerializableActionObject(SerializableActionObject serializableObject)
        {
            if (serializableActionObjects.ContainsKey(serializableObject.LoadID))
            {
                string message = string.Format(duplicateLoadIDErrorText, "AddSerializableActionObject()", serializableObject.LoadID);
                DaggerfallUnity.LogMessage(message);
                return;
            }

            serializableActionObjects.Add(serializableObject.LoadID, serializableObject);
        }

        private void AddSerializableEnemy(SerializableEnemy serializableObject)
        {
            if (serializableEnemies.ContainsKey(serializableObject.LoadID))
            {
                string message = string.Format(duplicateLoadIDErrorText, "AddSerializableEnemy()", serializableObject.LoadID);
                DaggerfallUnity.LogMessage(message);
                return;
            }

            serializableEnemies.Add(serializableObject.LoadID, serializableObject);
        }

        private void AddSerializableLootContainer(SerializableLootContainer serializableObject)
        {
            if (serializableLootContainers.ContainsKey(serializableObject.LoadID))
            {
                string message = string.Format(duplicateLoadIDErrorText, "AddSerializableLootContainer()", serializableObject.LoadID);
                DaggerfallUnity.LogMessage(message);
                return;
            }

            serializableLootContainers.Add(serializableObject.LoadID, serializableObject);
        }

        #endregion
    }
}