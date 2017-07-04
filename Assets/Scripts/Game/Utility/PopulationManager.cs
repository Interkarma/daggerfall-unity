// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// Manages a pool of civilian mobiles (wandering NPCs) for the local town environment.
    /// Attached to the same GameObject as DaggerfallLocation and CityNavigation by environment layout process in StreamingWorld.
    /// </summary>
    [RequireComponent(typeof(DaggerfallLocation))]
    [RequireComponent(typeof(CityNavigation))]
    public class PopulationManager : MonoBehaviour
    {
        #region Fields

        const string mobileNPCName = "MobileNPC";               // Name displayed in scene view
        const int maxPlayerDistanceOutsideRect = 2500;          // Max world units beyond location rect where all NPCs are despawned
        const int populationIndexPer16Blocks = 12;              // This many NPCs will be spawned around player per 16 RMB blocks in location
        const int navGridSpawnRadius = 64;                      // Radius of spawn distance around player or target point

        bool playerInRange = false;
        bool playerWasInRange = false;
        int maxPopulation = 0;

        //public int MaxPoolSize = 50;                        // Maximum number of NPCs allowed in pool
        //public float MaxSpawnDistance = 50;                 // Maximum distance from player NPCs will spawn
        //public float MaxDistanceFromPlayer = 1000;          // Maximum distance from player before NPC is recycled
        //public bool AllowVisibleSpawn = false;              // Allow NPCs to spawn while player can see them
        //public bool AllowVisibleDespawn = false;            // Allow NPCs to despawn while player can see them

        //int nextAvailableItem = -1;

        PlayerGPS playerGPS;
        DaggerfallLocation dfLocation;
        CityNavigation cityNavigation;

        List<PoolItem> populationPool = new List<PoolItem>();

        #endregion

        #region Structs & Enums

        struct PoolItem
        {
            public bool active;                             // NPC is currently active/inactive
            public bool scheduleEnable;                     // NPC is active and waiting to be made visible
            public bool scheduleRecycle;                    // NPC is active and waiting to be hidden for recycling
            public int nameSeed;                            // Current nameseed of NPC
            public MobilePersonMotor motor;                 // NPC motor
            public float distanceToPlayer;                  // Last measured distance to player
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets max population calculated for this location.
        /// </summary>
        public int MaxPopulation
        {
            get { return maxPopulation; }
        }

        /// <summary>
        /// Checks if player inside valid population spawn range for this location.
        /// </summary>
        public bool PlayerInRange
        {
            get { return playerInRange; }
        }

        #endregion

        #region Unity

        private void Start()
        {
            // Cache references
            playerGPS = GameManager.Instance.PlayerGPS;
            dfLocation = GetComponent<DaggerfallLocation>();
            cityNavigation = GetComponent<CityNavigation>();
        }

        private void Update()
        {
            // 1. Drop NPCs around closest navgrid position to player
            // 2. When player moves far enough away, NPCs should be recycled and placed near player's current position
            // 3. NPCs need to start spawning after 6am and vanish after 6pm

            // Check if player inside max world range for population to exist
            playerInRange = false;
            RectOffset locationRect = dfLocation.LocationRect;
            if (playerGPS.WorldX >= locationRect.left - maxPlayerDistanceOutsideRect &&
                playerGPS.WorldX <= locationRect.right + maxPlayerDistanceOutsideRect &&
                playerGPS.WorldZ >= locationRect.top - maxPlayerDistanceOutsideRect &&
                playerGPS.WorldZ <= locationRect.bottom + maxPlayerDistanceOutsideRect)
            {
                playerInRange = true;
            }

            // Spawn or despawn population when player enters/exits hard range limits
            if (playerInRange && !playerWasInRange)
            {
                // Spawn new population
                CreateNewPopulation();
                playerWasInRange = true;
            }
            else if (!playerInRange && playerWasInRange)
            {
                // TODO: Despawn old population
                playerWasInRange = false;
            }

            // Collect mobiles for recycling
            CollectPoolItems();
        }

        #endregion

        #region Private Methods

        void CreateNewPopulation()
        {
            // Get closest point on navgrid to player position in world
            DFPosition playerWorldPos = new DFPosition(playerGPS.WorldX, playerGPS.WorldZ);
            DFPosition playerGridPos = cityNavigation.WorldToNavGridPosition(playerWorldPos);

            Debug.LogFormat("Closest player grid position {0}, {1}", playerGridPos.X, playerGridPos.Y);

            // Calculate maximum population for this location
            // This is determined to be total RMB blocks in location / 16 clamped to 1-4
            // So a large 8x8 city like Daggerfall will have 64/16 = 4 * populationIndexPer16Blocks max people around player
            // A small 1x1 town will have 1/16 = 1 * populationIndexPer16Blocks max people around player
            // This should lead to larger cities feeling more bustling than smaller towns
            // Very likely to be tuned or changed completely over time
            int totalBlocks = dfLocation.Summary.BlockWidth * dfLocation.Summary.BlockHeight;
            int populationBlocks = Mathf.Clamp(totalBlocks / 16, 1, 4);
            maxPopulation = populationBlocks * populationIndexPer16Blocks;

            // Place all NPCs on the navgrid when player first enters range
            // Due to long draw distances, this could lead to some pop-in
            // Something that can be refined later
            for (int i = 0; i < maxPopulation; i++)
            {
                int item = GetNextFreePoolItem();
                if (item == -1)
                    break;

                // Set item as active and schedule to become enabled
                // TODO: Assign to spawn position on navgrid
                PoolItem poolItem = populationPool[item];
                poolItem.active = true;
                poolItem.scheduleEnable = true;

                // Get a random spawn position
                // If unable to spawn now they will be added later during recycling updates
                DFPosition spawnPosition;
                if (cityNavigation.GetRandomSpawnPosition(playerGridPos, out spawnPosition, navGridSpawnRadius))
                {
                    // Liven up the motor
                    DFPosition worldPosition = cityNavigation.NavGridToWorldPosition(spawnPosition);
                    Vector3 scenePosition = cityNavigation.WorldToScenePosition(worldPosition);
                    poolItem.motor.transform.position = scenePosition;
                    GameObjectHelper.AlignBillboardToGround(poolItem.motor.gameObject, new Vector2(0, 2f));
                    poolItem.motor.gameObject.SetActive(true);
                    poolItem.scheduleEnable = false;
                }

                // Update pool item
                populationPool[item] = poolItem;
            }
        }

        // Gets next free pool item
        // Will attempt to create new item if none could be found - up to max population
        // Returns -1 if no free item could be found or created
        int GetNextFreePoolItem()
        {
            // Look for an available inactive pool item
            for (int i = 0; i < populationPool.Count; i++)
            {
                if (!populationPool[i].active)
                    return i;
            }

            // Create a new item if population not at maximum
            if (populationPool.Count < maxPopulation)
                return CreateNewPoolItem();

            return -1;
        }

        // Creates a new pool item with NPC prefab - returns -1 if could not be created
        int CreateNewPoolItem()
        {
            // Must have an NPC prefab set
            if (!DaggerfallUnity.Instance.Option_MobileNPCPrefab)
                return -1;

            // Instantiate NPC prefab
            GameObject go = GameObjectHelper.InstantiatePrefab(DaggerfallUnity.Instance.Option_MobileNPCPrefab.gameObject, mobileNPCName, dfLocation.transform, Vector3.zero);
            go.SetActive(false);

            // Get motor and set reference to navgrid
            MobilePersonMotor motor = go.GetComponent<MobilePersonMotor>();
            motor.cityNavigation = cityNavigation;

            // Create the pool item and assign new GameObject
            // This pool item starts inactive and can be used later
            PoolItem poolItem = new PoolItem();
            poolItem.motor = motor;

            // Add to pool
            populationPool.Add(poolItem);

            return populationPool.Count - 1;
        }

        // Updates population pool and mark NPCs for recycling
        void CollectPoolItems()
        {
            GameObject player = GameManager.Instance.PlayerObject;

            for (int i = 0; i < populationPool.Count; i++)
            {
                // Get pool item and skip if not active or not setup
                PoolItem poolItem = populationPool[i];
                if (!poolItem.active || !poolItem.motor)
                    continue;

                // Immediately recycle pool item if scene target is zero and is still searching for a tile
                // This indicates mobile was not placed properly on grid and cannot find a target position
                // Mobile will be recycled to a new position later
                if (poolItem.motor.TargetScenePosition == Vector3.zero && poolItem.motor.SeekCount > 3)
                    ImmediatelyRecyclePoolItem(ref poolItem);

                //// Disable pool item when too far from player
                //poolItem.distanceToPlayer = Vector3.Distance(poolItem.gameObject.transform.position, player.transform.position);
                //if (poolItem.distanceToPlayer > MaxDistanceFromPlayer)
                //    poolItem.scheduleRecycle = true;
                //else
                //    poolItem.scheduleRecycle = false;

                // Update poolItem data
                populationPool[i] = poolItem;
            }
        }

        void ImmediatelyRecyclePoolItem(ref PoolItem poolItem)
        {
            poolItem.motor.gameObject.SetActive(false);
            poolItem.active = false;
            poolItem.scheduleEnable = false;
            poolItem.scheduleRecycle = false;
        }

        #endregion
    }
}