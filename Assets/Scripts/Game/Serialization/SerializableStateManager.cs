// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Lypyl (lypyldf@gmail.com), Hazelnut
// 
// Notes:           Extracted from SaveLoadManager class (Hazelnut Jan2018)
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Serialization
{
    /// <summary>
    /// Manages stateful game objects (implementations of <see cref="ISerializableGameObject"/>).
    /// Used by <see cref="SaveLoadManager"/> to serialize scene state.
    /// Uses a scene cache to persist building interiors and player owned areas.
    /// </summary>
    public class SerializableStateManager
    {
        #region Fields

        const string invalidLoadIDExceptionText = "serializableObject does not have a valid LoadID";
        const string duplicateLoadIDErrorText = "Duplicate LoadID {1} detected for {0} object. This object will not be serialized.";

        private static int numStatefulGameObjectTypes = Enum.GetNames(typeof(StatefulGameObjectTypes)).Length;

        // Serializable player (there can be only one!)
        SerializablePlayer serializablePlayer;

        // Serializable stateful game objects in current scene
        List<Dictionary<ulong, ISerializableGameObject>> statefulGameObjects = new List<Dictionary<ulong, ISerializableGameObject>>(numStatefulGameObjectTypes);

        // Scene cache for persisting state across transitions
        Dictionary<string, List<object[]>> sceneDataCache = new Dictionary<string, List<object[]>>();

        // Scenes to persist permanently (i.e. player owned ship/house or rented room in tavern)
        List<string> permanentScenes = new List<string>();

        #endregion

        #region Properties

        public SerializablePlayer SerializablePlayer
        {
            get { return serializablePlayer; }
        }

        #endregion

        #region Constructors

        public SerializableStateManager()
        {
            foreach (StatefulGameObjectTypes type in Enum.GetValues(typeof(StatefulGameObjectTypes)))
                statefulGameObjects.Add(new Dictionary<ulong, ISerializableGameObject>());
        }

        #endregion

        #region SceneCache Public Methods

        public void AddPermanentScene(string sceneName)
        {
            if (!permanentScenes.Contains(sceneName))
                permanentScenes.Add(sceneName);
        }

        public bool ContainsPermanentScene(string sceneName)
        {
            return permanentScenes.Contains(sceneName);
        }

        public void RemovePermanentScene(string sceneName)
        {
            permanentScenes.Remove(sceneName);
        }

        public void CacheScene(string sceneName)
        {
            Debug.LogFormat("Caching scene: {0}", sceneName);

            // Only cache loot containers & action doors for scenes
            List<object[]> sceneData = new List<object[]>(numStatefulGameObjectTypes);
            LootContainerData_v1[] containerData = GetLootContainerData();
            ActionDoorData_v1[] actionDoorData = GetActionDoorData();
            sceneData.Insert((int)StatefulGameObjectTypes.LootContainer, containerData);
            sceneData.Insert((int)StatefulGameObjectTypes.ActionDoor, actionDoorData);
            sceneData.Insert((int)StatefulGameObjectTypes.ActionObject, new object[0]);
            sceneData.Insert((int)StatefulGameObjectTypes.Enemy, new object[0]);
            sceneDataCache[sceneName] = sceneData;
        }

        public void RestoreCachedScene(string sceneName)
        {
            Debug.LogFormat("Restoring scene: {0}", sceneName);

            List<object[]> sceneData;
            sceneDataCache.TryGetValue(sceneName, out sceneData);
            if (sceneData != null)
            {
                LootContainerData_v1[] containerData = (LootContainerData_v1[])sceneData[(int)StatefulGameObjectTypes.LootContainer];
                ActionDoorData_v1[] actionDoorData = (ActionDoorData_v1[])sceneData[(int)StatefulGameObjectTypes.ActionDoor];
                RestoreLootContainerData(containerData);
                RestoreActionDoorData(actionDoorData);
            }
            // Delete scene from cache after restore
            sceneDataCache.Remove(sceneName);
        }

        public void ClearSceneCache(bool start = true)
        {
            Debug.Log("Clearing scene cache. start=" + start);
            if (start)
            {
                sceneDataCache.Clear();
                permanentScenes.Clear();
            }
            else
            {
                // Copy any permanent scenes (sans corpses) into a new cache and replace existing
                Dictionary<string, List<object[]>> newSceneDataCache = new Dictionary<string, List<object[]>>();
                List<object[]> sceneData;
                foreach (string sceneName in permanentScenes)
                {
                    if (sceneDataCache.TryGetValue(sceneName, out sceneData))
                    {
                        if (sceneData != null)
                        {
                            object[] lootContainers = sceneData[(int)StatefulGameObjectTypes.LootContainer];
                            List<LootContainerData_v1> lootNoCorpses = new List<LootContainerData_v1>();
                            foreach (LootContainerData_v1 loot in lootContainers)
                                if (loot.containerType != LootContainerTypes.CorpseMarker)
                                    lootNoCorpses.Add(loot);
                            sceneData[(int)StatefulGameObjectTypes.LootContainer] = lootNoCorpses.ToArray();
                        }
                    }
                    if (sceneData != null && sceneData.Count > 0)
                        newSceneDataCache[sceneName] = sceneData;
                }
                sceneDataCache = newSceneDataCache;
            }
        }

        public SceneCache_v1 GetSceneCache()
        {
            List<SceneCacheEntry_v1> entries = new List<SceneCacheEntry_v1>(sceneDataCache.Count);
            foreach (string sceneName in sceneDataCache.Keys)
            {
                List<object[]> sceneData;
                sceneDataCache.TryGetValue(sceneName, out sceneData);
                if (sceneData != null)
                {
                    LootContainerData_v1[] containerData = (LootContainerData_v1[])sceneData[(int)StatefulGameObjectTypes.LootContainer];
                    ActionDoorData_v1[] actionDoorData = (ActionDoorData_v1[])sceneData[(int)StatefulGameObjectTypes.ActionDoor];
                    entries.Add(new SceneCacheEntry_v1()
                    {
                        sceneName = sceneName,
                        lootContainers = containerData,
                        actionDoors = actionDoorData
                    });
                }
            }
            SceneCache_v1 data = new SceneCache_v1()
            {
                sceneCache = entries.ToArray(),
                permanentScenes = permanentScenes.ToArray()
            };
            return data;
        }

        public void RestoreSceneCache(SceneCache_v1 sceneCacheData)
        {
            if (sceneCacheData == null)
                return;

            if (sceneCacheData.permanentScenes != null && sceneCacheData.permanentScenes.Length > 0)
                permanentScenes = new List<string>(sceneCacheData.permanentScenes);

            SceneCacheEntry_v1[] sceneCacheEntries = sceneCacheData.sceneCache;
            if (sceneCacheEntries == null || sceneCacheEntries.Length == 0)
                return;

            for (int i = 0; i < sceneCacheEntries.Length; i++)
            {
                SceneCacheEntry_v1 scene = sceneCacheEntries[i];
                List<object[]> sceneData = new List<object[]>(numStatefulGameObjectTypes);
                sceneData.Insert((int)StatefulGameObjectTypes.LootContainer, scene.lootContainers);
                sceneData.Insert((int)StatefulGameObjectTypes.ActionDoor, scene.actionDoors);
                sceneData.Insert((int)StatefulGameObjectTypes.ActionObject, new object[0]);
                sceneData.Insert((int)StatefulGameObjectTypes.Enemy, new object[0]);
                sceneDataCache[scene.sceneName] = sceneData;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if a LoadID is already in enemy serialization list.
        /// </summary>
        public bool ContainsEnemy(ulong id)
        {
            return SerializableEnemies.ContainsKey(id);
        }

        /// <summary>
        /// Gets enemy by LoadID if present in enemy serialization list.
        /// </summary>
        public SerializableEnemy GetEnemy(ulong id)
        {
            if (!ContainsEnemy(id))
                return null;

            return SerializableEnemies[id] as SerializableEnemy;
        }

        /// <summary>
        /// Check if a LoadID is already in action door serialization list.
        /// </summary>
        public bool ContainsActionDoor(ulong id)
        {
            return SerializableActionDoors.ContainsKey(id);
        }

        /// <summary>
        /// Register ISerializableGameObject with SerializableStateManager.
        /// </summary>
        public void RegisterStatefulGameObject(ISerializableGameObject serializableObject)
        {
            if (serializableObject.LoadID == 0)
                throw new Exception(invalidLoadIDExceptionText);

            if (serializableObject is SerializablePlayer)
            {
                serializablePlayer = serializableObject as SerializablePlayer;
            }
            else
            {
                StatefulGameObjectTypes sgObjType = GetStatefulGameObjectType(serializableObject);
                Dictionary<ulong, ISerializableGameObject> serializableObjects = statefulGameObjects[(int)sgObjType];
                if (serializableObjects.ContainsKey(serializableObject.LoadID))
                    DaggerfallUnity.LogMessage(string.Format(duplicateLoadIDErrorText, sgObjType, serializableObject.LoadID));
                else
                    serializableObjects.Add(serializableObject.LoadID, serializableObject);
            }
        }

        /// <summary>
        /// Deregister ISerializableGameObject from SerializableStateManager.
        /// </summary>
        public void DeregisterStatefulGameObject(ISerializableGameObject serializableObject)
        {
            if (serializableObject.LoadID == 0)
                throw new Exception(invalidLoadIDExceptionText);

            StatefulGameObjectTypes sgObjType = GetStatefulGameObjectType(serializableObject);
            Dictionary<ulong, ISerializableGameObject> serializableObjects = statefulGameObjects[(int)sgObjType];
            serializableObjects.Remove(serializableObject.LoadID);
        }

        /// <summary>
        /// Force deregister all ISerializableGameObject instances from SerializableStateManager.
        /// </summary>
        public void DeregisterAllStatefulGameObjects(bool keepPlayer = true)
        {
            // Optionally deregister player
            if (!keepPlayer)
                serializablePlayer = null;

            // Deregister other objects
            foreach (Dictionary<ulong, ISerializableGameObject> serializableObjects in statefulGameObjects)
            {
                serializableObjects.Clear();
            }
        }

        #endregion

        #region Public Serialization Methods

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

            foreach (var value in SerializableActionDoors.Values)
            {
                if (value.ShouldSave)
                    actionDoors.Add((ActionDoorData_v1)value.GetSaveData());
            }

            return actionDoors.ToArray();
        }

        public ActionObjectData_v1[] GetActionObjectData()
        {
            List<ActionObjectData_v1> actionObjects = new List<ActionObjectData_v1>();

            foreach (var value in SerializableActionObjects.Values)
            {
                if (value.ShouldSave)
                    actionObjects.Add((ActionObjectData_v1)value.GetSaveData());
            }

            return actionObjects.ToArray();
        }

        public EnemyData_v1[] GetEnemyData()
        {
            List<EnemyData_v1> enemies = new List<EnemyData_v1>();

            foreach (var value in SerializableEnemies.Values)
            {
                if (value.ShouldSave)
                    enemies.Add((EnemyData_v1)value.GetSaveData());
            }

            return enemies.ToArray();
        }

        public LootContainerData_v1[] GetLootContainerData()
        {
            List<LootContainerData_v1> containers = new List<LootContainerData_v1>();

            foreach (var value in SerializableLootContainers.Values)
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
                if (SerializableActionDoors.ContainsKey(key))
                {
                    SerializableActionDoors[key].RestoreSaveData(actionDoors[i]);
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
                if (SerializableActionObjects.ContainsKey(key))
                {
                    SerializableActionObjects[key].RestoreSaveData(actionObjects[i]);
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

                GameManager.Instance?.RaiseOnEnemySpawnEvent(go);
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
                if (SerializableLootContainers.ContainsKey(key))
                {
                    // Apply to known loot container that is part of scene build
                    SerializableLootContainers[key].RestoreSaveData(lootContainers[i]);
                }
                else
                {
                    // Add custom drop containers back to scene (e.g. dropped loot, slain foes)
                    if (lootContainers[i].customDrop)
                    {
                        DaggerfallLoot customLootContainer = GameObjectHelper.CreateDroppedLootContainer(GameManager.Instance.PlayerObject, key, lootContainers[i].textureArchive, lootContainers[i].textureRecord);
                        SerializableLootContainer serializableLootContainer = customLootContainer.GetComponent<SerializableLootContainer>();
                        if (serializableLootContainer)
                        {
                            serializableLootContainer.RestoreSaveData(lootContainers[i]);
                        }
                        //Debug.LogFormat("created loot container {0} containing {1} parent {2}", key, customLootContainer.Items.GetItem(0).shortName, customLootContainer.transform.parent.name);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<ulong, ISerializableGameObject> SerializableActionDoors
        {
            get { return statefulGameObjects[(int)StatefulGameObjectTypes.ActionDoor]; }
        }

        private Dictionary<ulong, ISerializableGameObject> SerializableActionObjects
        {
            get { return statefulGameObjects[(int)StatefulGameObjectTypes.ActionObject]; }
        }

        private Dictionary<ulong, ISerializableGameObject> SerializableEnemies
        {
            get { return statefulGameObjects[(int)StatefulGameObjectTypes.Enemy]; }
        }

        private Dictionary<ulong, ISerializableGameObject> SerializableLootContainers
        {
            get { return statefulGameObjects[(int)StatefulGameObjectTypes.LootContainer]; }
        }

        private StatefulGameObjectTypes GetStatefulGameObjectType(ISerializableGameObject sgObj)
        {
            if (sgObj is SerializableActionDoor) return StatefulGameObjectTypes.ActionDoor;
            else if (sgObj is SerializableActionObject) return StatefulGameObjectTypes.ActionObject;
            else if (sgObj is SerializableEnemy) return StatefulGameObjectTypes.Enemy;
            else if (sgObj is SerializableLootContainer) return StatefulGameObjectTypes.LootContainer;
            else
            {
                // type is not accounted for
                if (sgObj is ISerializableGameObject)
                    throw new NotImplementedException($"{nameof(GetStatefulGameObjectType)} does not know what to do with {sgObj.GetType().FullName}");
                else
                    throw new Exception($"{nameof(ISerializableGameObject)} type not implemented for {sgObj.GetType().FullName}");
            }
        }

        #endregion
    }
}
