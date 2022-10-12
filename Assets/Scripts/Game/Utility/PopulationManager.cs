// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;

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

        const float ticksPerSecond = 10;                        // How often population manager will tick per second

        const string mobileNPCName = "MobileNPC";               // Name displayed in scene view
        const int maxPlayerDistanceOutsideRect = 2500;          // Max world units beyond location rect where no mobiles are spawned
        const int populationIndexPer16Blocks = 24;              // This many NPCs will be spawned around player per 16 RMB blocks in location
        const int navGridSpawnRadius = 96;                      // Radius of spawn distance around player or target point
        const float recycleDistance = 150f;                     // Distance in world units after which NPCs are recycled
        const float allowVisiblePopRange = 120f;                // Distance in world units after which visible popin/popout is allowed

        bool playerInLocationRange = false;
        int maxPopulation = 0;
        float updateTimer = 0;

        FactionFile.FactionRaces populationRace;

        PlayerGPS playerGPS;
        DaggerfallLocation dfLocation;
        CityNavigation cityNavigation;

        List<PoolItem> populationPool = new List<PoolItem>();

        #endregion

        #region Structs & Enums

        public struct PoolItem
        {
            public bool active;                             // NPC is currently active/inactive
            public bool scheduleEnable;                     // NPC is active and waiting to be made visible
            public bool scheduleRecycle;                    // NPC is active and waiting to be hidden for recycling
            public float distanceToPlayer;                  // Distance to player
            public MobilePersonNPC npc;                     // NPC motor
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

        public List<PoolItem> PopulationPool
        {
            get { return populationPool; }
        }

        #endregion

        #region Unity

        private void Start()
        {
            // Cache references
            playerGPS = GameManager.Instance.PlayerGPS;
            dfLocation = GetComponent<DaggerfallLocation>();
            cityNavigation = GetComponent<CityNavigation>();

            // Get dominant race in locations climate zone
            populationRace = playerGPS.ClimateSettings.People;

            // Calculate maximum population
            int totalBlocks = dfLocation.Summary.BlockWidth * dfLocation.Summary.BlockHeight;
            int populationBlocks = Mathf.Clamp(totalBlocks / 16, 1, 4);
            maxPopulation = populationBlocks * populationIndexPer16Blocks;
        }

        private void Update()
        {
            // Increment update timer
            updateTimer += Time.deltaTime;
            if (updateTimer < (1f / ticksPerSecond))
                return;
            else
                updateTimer = 0;

            // Check if player inside max world range for population to exist
            playerInLocationRange = false;
            RectOffset locationRect = dfLocation.LocationRect;
            if (playerGPS.WorldX >= locationRect.left - maxPlayerDistanceOutsideRect &&
                playerGPS.WorldX <= locationRect.right + maxPlayerDistanceOutsideRect &&
                playerGPS.WorldZ >= locationRect.top - maxPlayerDistanceOutsideRect &&
                playerGPS.WorldZ <= locationRect.bottom + maxPlayerDistanceOutsideRect)
            {
                playerInLocationRange = true;
            }

            // Update population
            SpawnAvailableMobile();
            UpdateMobiles();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Spawn a new pool item within range of player.
        /// </summary>
        void SpawnAvailableMobile()
        {
            // Player must be in range of location
            if (!playerInLocationRange)
                return;

            // Get a free mobile from pool
            int item = GetNextFreePoolItem();
            if (item == -1)
                return;

            // Get closest point on navgrid to player position in world
            DFPosition playerWorldPos = new DFPosition(playerGPS.WorldX, playerGPS.WorldZ);
            DFPosition playerGridPos = cityNavigation.WorldToNavGridPosition(playerWorldPos);

            // Spawn mobile at a random position and schedule to be live
            DFPosition spawnPosition;
            if (cityNavigation.GetRandomSpawnPosition(playerGridPos, out spawnPosition, navGridSpawnRadius))
            {
                PoolItem poolItem = populationPool[item];

                // Setup spawn position
                DFPosition worldPosition = cityNavigation.NavGridToWorldPosition(spawnPosition);
                Vector3 scenePosition = cityNavigation.WorldToScenePosition(worldPosition);                
                poolItem.npc.Motor.transform.position = scenePosition;
                GameObjectHelper.AlignBillboardToGround(poolItem.npc.Motor.gameObject, new Vector2(0, 2f));

                // Schedule for enabling
                poolItem.active = true;
                poolItem.scheduleEnable = true;

                populationPool[item] = poolItem;
            }
        }

        /// <summary>
        /// Promote pending mobiles to live status and recycle out of range mobiles.
        /// </summary>
        void UpdateMobiles()
        {
            // Racial override can suppress population, e.g. transformed lycanthrope
            MagicAndEffects.MagicEffects.RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            bool suppressPopulationSpawns = racialOverride != null && racialOverride.SuppressPopulationSpawns;

            bool isDaytime = DaggerfallUnity.Instance.WorldTime.Now.IsDay;
            for (int i = 0; i < populationPool.Count; i++)
            {
                PoolItem poolItem = populationPool[i];

                // Get distance to player
                poolItem.distanceToPlayer = Vector3.Distance(playerGPS.transform.position, poolItem.npc.Motor.transform.position);

                // Show pending mobiles when available
                if (poolItem.active &&
                    poolItem.scheduleEnable &&
                    AllowMobileActivationChange(ref poolItem) &&
                    isDaytime &&
                    !suppressPopulationSpawns)
                {
                    poolItem.npc.Motor.gameObject.SetActive(true);
                    poolItem.scheduleEnable = false;

                    if (MobileNPCGenerator != null)
                    {
                        MobileNPCGenerator(poolItem);
                    }
                    else
                    {
                        poolItem.npc.RandomiseNPC(GetEntityRace());
                    }

                    poolItem.npc.Motor.InitMotor();

                    // Adjust billboard position for actual size
                    Vector2 size = poolItem.npc.Asset.GetSize();
                    if (Mathf.Abs(size.y - 2f) > 0.1f)
                        poolItem.npc.Asset.transform.Translate(0, (size.y - 2f) * 0.52f, 0);

                    OnMobileNPCEnable?.Invoke(poolItem);
                }

                // Mark for recycling
                if (poolItem.npc.Motor.SeekCount > 4 ||
                    poolItem.distanceToPlayer > recycleDistance ||
                    !isDaytime)
                {
                    poolItem.scheduleRecycle = true;
                }

                // Recycle pending mobiles when available
                if (poolItem.active && poolItem.scheduleRecycle && AllowMobileActivationChange(ref poolItem))
                {
                    poolItem.npc.Motor.gameObject.SetActive(false);
                    poolItem.active = false;
                    poolItem.scheduleEnable = false;
                    poolItem.scheduleRecycle = false;
                    if (poolItem.npc.Asset)
                        poolItem.npc.Asset.transform.localPosition = Vector3.zero;

                    OnMobileNPCDisable?.Invoke(poolItem);
                }

                populationPool[i] = poolItem;

                // Do not render active mobile until it has made at least 1 full tile move
                // This hides skating effect while unit aligning to navigation grid
                if (poolItem.active && poolItem.npc.Asset)
                {
                    MeshRenderer billboardRenderer = poolItem.npc.Asset.GetComponent<MeshRenderer>();
                    if (billboardRenderer)
                        billboardRenderer.enabled = (poolItem.npc.Motor.MoveCount > 0) ? true : false;
                }
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

            // Get MobilePersonNPC reference
            MobilePersonNPC npc = go.GetComponent<MobilePersonNPC>();

            // Get motor and set reference to navgrid
            MobilePersonMotor motor = go.GetComponent<MobilePersonMotor>();
            motor.cityNavigation = cityNavigation;

            // Create the pool item and assign new GameObject
            // This pool item starts inactive and can be used later
            PoolItem poolItem = new PoolItem();
            poolItem.npc = npc;
            poolItem.npc.Motor = motor;
            poolItem.npc.Asset = motor.MobileAsset;

            // Add to pool
            populationPool.Add(poolItem);

            OnMobileNPCCreate?.Invoke(poolItem);

            return populationPool.Count - 1;
        }

        bool AllowMobileActivationChange(ref PoolItem poolItem)
        {
            const float fieldOfView = 180f;

            // Allow visible popin/popout beyond control range
            if (poolItem.distanceToPlayer > allowVisiblePopRange)
                return true;

            // Check if outside player's main field of view
            Vector3 directionToMobile = poolItem.npc.Motor.transform.position - playerGPS.transform.position;
            float angle = Vector3.Angle(directionToMobile, playerGPS.transform.forward);
            if (angle > fieldOfView * 0.5f)
            {
                return true;
            }

            return false;
        }

        Races GetEntityRace()
        {
            // Convert factionfile race to entity race
            // DFTFU is mostly isolated from game classes and does not know entity races
            // Need to convert this into something the billboard can use
            // Only Redguard, Nord, Breton have mobile NPC assets
            switch(populationRace)
            {
                case FactionFile.FactionRaces.Redguard:
                    return Races.Redguard;
                case FactionFile.FactionRaces.Nord:
                    return Races.Nord;
                default:
                case FactionFile.FactionRaces.Breton:
                    return Races.Breton;
            }
        }

        #endregion

        #region Events

        //OnMobileNPCCreate
        public delegate void OnMobileNPCCreateHandler(PoolItem poolItem);
        public static event OnMobileNPCCreateHandler OnMobileNPCCreate;

        // MobileNPCGenerator
        public delegate void MobileNPCGenerationHandler(PoolItem poolItem);
        public static MobileNPCGenerationHandler MobileNPCGenerator;

        //OnMobileNPCEnable
        public delegate void OnMobileNPCEnableHandler(PoolItem poolItem);
        public static event OnMobileNPCEnableHandler OnMobileNPCEnable;

        //OnMobileNPCDisable
        public delegate void OnMobileNPCDisableHandler(PoolItem poolItem);
        public static event OnMobileNPCDisableHandler OnMobileNPCDisable;

        #endregion
    }
}