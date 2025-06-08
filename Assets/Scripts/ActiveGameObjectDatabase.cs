using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Utility;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace DaggerfallWorkshop
{
    // Generic GameObject cache
    // To be used as Singleton storage, to replace "FindObjectsOfType" lookups
    // From https://docs.unity3d.com/ScriptReference/Object.FindObjectsOfType.html
    // "This function is very slow. It is not recommended to use this function every frame. In most cases you can use the singleton pattern instead."
    // In addition, DFU has historically had crashes associated to frequent "FindObjectsOfType" lookups
    //
    // We can register inactive GameObjects, but the cache will only return active GameObjects.
    public class GameObjectCache
    {
        string cacheName;
        ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        List<WeakReference<GameObject>> cachedObjects = new List<WeakReference<GameObject>>();

        public GameObjectCache(string name)
        {
            cacheName = name;
        }

        // Returns all the active GameObjects in the cache
        // If a system calls SetActive(false) on an object without destroying it, it will not be returned here
        public IEnumerable<GameObject> GetActiveObjects()
        {
            cacheLock.EnterReadLock();
            try
            {
                List<GameObject> gameObjects = new List<GameObject>();
                foreach (var weakObject in cachedObjects)
                {
                    if (weakObject.TryGetTarget(out GameObject activeObject))
                    {
                        // A null check on a GameObject does more than C#'s reference check,
                        // it also checks if the object has been destroyed
                        // Like Object.FindObjectsOfType, we should only include active objects 
                        if (activeObject != null && activeObject.activeInHierarchy)
                            gameObjects.Add(activeObject);
                    }
                }

                return gameObjects;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        // Returns all the enabled components of active GameObjects in the cache
        // If a system calls SetActive(false) on an object without destroying it, it will not be returned here
        public IEnumerable<T> GetActiveComponents<T>() where T : MonoBehaviour
        {
            foreach (GameObject gameObject in GetActiveObjects())
            {
                var t = gameObject.GetComponent<T>();
                if (t != null && t.isActiveAndEnabled)
                    yield return t;
            }
        }

        // Adds a GameObject to the cache
        // Input GameObject can be inactive in the scene. It will not be removed from the cache until Destroyed
        public void AddObject(GameObject gameObject)
        {
#if UNITY_EDITOR
            // Editor-only test:
            // Check if the object is already there before adding
            cacheLock.EnterUpgradeableReadLock();
            try
            {
                foreach(WeakReference<GameObject> weakObject in cachedObjects)
                {
                    if(weakObject.TryGetTarget(out GameObject cachedObject))
                    {
                        // Already added
                        if (cachedObject == gameObject)
                            throw new ArgumentException($"GameObject '{gameObject.name}' was already registered in cache '{cacheName}'");
                    }
                }
#endif                
                // We're not already in the cache
                cacheLock.EnterWriteLock();
                try
                {
                    ClearDestroyedObjects();
                    cachedObjects.Add(new WeakReference<GameObject>(gameObject));
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
#if UNITY_EDITOR
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
#endif
        }

        // Adds a GameObjects to the cache. More efficient if multiple objects can be added at the same time.
        // Input GameObjects can be inactive in the scene. They will not be removed from the cache until Destroyed
        public void AddObjects(IEnumerable<GameObject> newActiveObjects)
        {
            cacheLock.EnterWriteLock();
            try
            {
                ClearDestroyedObjects();
                foreach (GameObject gameObject in newActiveObjects)
                {
                    cachedObjects.Add(new WeakReference<GameObject>(gameObject));
                }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        void ClearDestroyedObjects()
        {
            // We assume we already have the write lock

#if UNITY_EDITOR
            int previousCachedObjectCount = cachedObjects.Count;
#endif

            // Clear only null or destroyed objects
            // Objects may be inactive, but we are keeping them, in case they get activated
            cachedObjects.RemoveAll(weakRef => !weakRef.TryGetTarget(out GameObject target) || target == null);

#if UNITY_EDITOR
            int newCachedObjectCount = cachedObjects.Count;
            if (newCachedObjectCount < previousCachedObjectCount)
            {
                Debug.Log($"Removed {previousCachedObjectCount - newCachedObjectCount} entries from cache '{cacheName}'");
            }
#endif
        }

        public string GetDebugString()
        {
            int activeCount = 0;
            int destroyedCount = 0;
            int inactiveCount = 0;

            cacheLock.EnterReadLock();
            try
            {
                foreach (var weakObject in cachedObjects)
                {
                    if (weakObject.TryGetTarget(out GameObject activeObject))
                    {
                        if(activeObject == null)
                        {
                            ++destroyedCount;
                        }
                        else if(!activeObject.activeInHierarchy)
                        {
                            ++inactiveCount;
                        }
                        else
                        {
                            ++activeCount;
                        }
                    }
                    else
                    {
                        ++destroyedCount;
                    }
                }
            }
            finally
            {
                cacheLock.ExitReadLock();
            }

            return $"{cacheName}: {activeCount} active, {destroyedCount} destroyed, {inactiveCount} inactive";
        }
    }

    // Namespace for DFU object lookups
    // We keep separate caches for different GameObject types
    // For example, we have an Enemy cache, for entities of type EntityMonster or EntityClass
    // Even though Civilian Mobile NPCs are also similar entities, they are tracked separately
    public static class ActiveGameObjectDatabase
    {
        static GameObjectCache enemyCache = new GameObjectCache("Enemy");
        static GameObjectCache civilianCache = new GameObjectCache("Civilian Mobile");
        static GameObjectCache lootCache = new GameObjectCache("Loot");
        static GameObjectCache foeSpawnerCache = new GameObjectCache("Foe Spawner");
        static GameObjectCache staticNpcCache = new GameObjectCache("Static NPC");
        static GameObjectCache actionDoorCache = new GameObjectCache("Action Door");
        static GameObjectCache rdbCache = new GameObjectCache("RDB");

        // Gets all the active enemy GameObjects. Must be registered as Enemy (see below)
        public static IEnumerable<GameObject> GetActiveEnemyObjects()
        {
            return enemyCache.GetActiveObjects();
        }

        // Gets all the enabled DaggerfallEntityBehaviour components from active registered enemies
        public static IEnumerable<DaggerfallEntityBehaviour> GetActiveEnemyBehaviours()
        {
            return enemyCache.GetActiveComponents<DaggerfallEntityBehaviour>();
        }

        // Gets all the enabled DaggerfallEnemy components from active registered enemies
        public static IEnumerable<DaggerfallEnemy> GetActiveEnemyEntities()
        {
            return enemyCache.GetActiveComponents<DaggerfallEnemy>();
        }

        // Gets all the enabled QuestResourceBehaviour components from active registered enemies
        public static IEnumerable<QuestResourceBehaviour> GetActiveEnemyQuestResourceBehaviours()
        {
            return enemyCache.GetActiveComponents<QuestResourceBehaviour>();
        }

        // Gets all the enabled EnemyMotor components from active registered enemies
        public static IEnumerable<EnemyMotor> GetActiveEnemyMotors()
        {
            return enemyCache.GetActiveComponents<EnemyMotor>();
        }

        // Registers an enemy (monster or class) to the enemy cache. Does not have to be active
        public static void RegisterEnemy(GameObject enemy)
        {
            enemyCache.AddObject(enemy);
        }

        // Gets all the active Civilian Mobile GameObjects. Must be registered as Civilian Mobile (see below)
        public static IEnumerable<GameObject> GetActiveCivilianMobileObjects()
        {
            return civilianCache.GetActiveObjects();
        }

        // Gets all the enabled DaggerfallEntityBehaviour components from active registered Civilian Mobiles
        public static IEnumerable<DaggerfallEntityBehaviour> GetActiveCivilianMobileBehaviours()
        {
            return civilianCache.GetActiveComponents<DaggerfallEntityBehaviour>();
        }

        // Registers a mobile civilian NPC to the civilian cache. Does not have to be active
        public static void RegisterCivilianMobile(GameObject civilian)
        {
            civilianCache.AddObject(civilian);
        }

        // Gets all the active loot GameObjects. Must be registered as Loot (see below)
        public static IEnumerable<GameObject> GetActiveLootObjects()
        {
            return lootCache.GetActiveObjects();
        }

        // Gets all the enabled DaggerfallLoot components from active registered loot
        public static IEnumerable<DaggerfallLoot> GetActiveLoot()
        {
            return lootCache.GetActiveComponents<DaggerfallLoot>();
        }

        // Registers a loot object to the loot cache. Does not have to be active
        public static void RegisterLoot(GameObject loot)
        {
            lootCache.AddObject(loot);
        }

        // Gets all the active Foe Spawner GameObjects. Must be registered as Foe Spawner (see below)
        public static IEnumerable<GameObject> GetActiveFoeSpawnerObjects()
        {
            return foeSpawnerCache.GetActiveObjects();
        }

        // Gets all the enabled FoeSpawner components from active registered foe spawners
        public static IEnumerable<FoeSpawner> GetActiveFoeSpawners()
        {
            return foeSpawnerCache.GetActiveComponents<FoeSpawner>();
        }

        // Registers a foe spawner object to the foe spawner cache. Does not have to be active
        public static void RegisterFoeSpawner(GameObject foeSpawner)
        {
            foeSpawnerCache.AddObject(foeSpawner);
        }

        // Gets all the active Static NPC GameObjects. Must be registered as a Static NPC (see below)
        public static IEnumerable<GameObject> GetActiveStaticNPCObjects()
        {
            return staticNpcCache.GetActiveObjects();
        }

        // Gets all the enabled StaticNPC components from active registered static NPCs
        public static IEnumerable<StaticNPC> GetActiveStaticNPCs()
        {
            return staticNpcCache.GetActiveComponents<StaticNPC>();
        }

        // Gets all the enabled QuestResourceBehaviour components from active registered static NPCs
        public static IEnumerable<QuestResourceBehaviour> GetActiveStaticNPCQuestResourceBehaviours()
        {
            return staticNpcCache.GetActiveComponents<QuestResourceBehaviour>();
        }

        // Registers a static NPC object to the Static NPC cache. Does not have to be active
        public static void RegisterStaticNPC(GameObject staticNPC)
        {
            staticNpcCache.AddObject(staticNPC);
        }

        // Gets all the active Action Door GameObjects. Must be registered as Action Door (see below)
        public static IEnumerable<GameObject> GetActiveActionDoorObjects()
        {
            return actionDoorCache.GetActiveObjects();
        }

        // Gets all the enabled DaggerfallActionDoor components from active registered doors
        public static IEnumerable<DaggerfallActionDoor> GetActiveActionDoors()
        {
            return actionDoorCache.GetActiveComponents<DaggerfallActionDoor>();
        }

        // Registers an Action Door object to the Action Door cache. Does not have to be active
        // Action Doors are not to be confused with static doors, which exist on RDBs and Interiors on their DaggerfallStaticDoors component
        public static void RegisterActionDoor(GameObject door)
        {
            actionDoorCache.AddObject(door);
        }

        // Gets all the active RDB GameObjects. Must be registered as RDB (see below)
        public static IEnumerable<GameObject> GetActiveRDBObjects()
        {
            return rdbCache.GetActiveObjects();
        }

        // Gets all the enabled DaggerfallStaticDoors components from active registered RDBs
        public static IEnumerable<DaggerfallStaticDoors> GetActiveRDBStaticDoors()
        {
            return rdbCache.GetActiveComponents<DaggerfallStaticDoors>();
        }

        // Registers a Daggerfall dungeon block "RDB" game object to the RDB cache. Does not have to be active
        public static void RegisterRDB(GameObject rdb)
        {
            rdbCache.AddObject(rdb);
        }

        // Used for debugging
        public static IEnumerable<string> GetCacheDebugLines()
        {
            yield return enemyCache.GetDebugString();
            yield return civilianCache.GetDebugString();
            yield return lootCache.GetDebugString();
            yield return foeSpawnerCache.GetDebugString();
            yield return staticNpcCache.GetDebugString();
            yield return actionDoorCache.GetDebugString();
            yield return rdbCache.GetDebugString();
        }
    }
}