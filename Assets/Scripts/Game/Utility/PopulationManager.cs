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
using System;
using System.Collections.Generic;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// Manages a pool of civilian mobiles (wandering NPCs) for the local town environment.
    /// Attached to the same GameObject as DaggerfallLocation and CityNavigation by environment layout process in StreamingWorld.
    /// </summary>
    public class PopulationManager : MonoBehaviour
    {
        #region Fields

        public int MaxPoolSize = 50;                        // Maximum number of NPCs allowed in pool
        public float MaxSpawnDistance = 50;                 // Maximum distance from player NPCs will spawn
        public float MaxDistanceFromPlayer = 1000;          // Maximum distance from player before NPC is recycled
        public bool AllowVisibleSpawn = false;              // Allow NPC to spawn while player is looking
        public bool AllowVisibleDespawn = false;            // Allow NPC to despawn while player is looking

        int nextAvailableItem = -1;
        List<MobileNPCPoolItem> populationPool = new List<MobileNPCPoolItem>();

        #endregion

        #region Structs & Enums

        struct MobileNPCPoolItem
        {
            public bool active;                             // NPC is currently active 
            public bool scheduleRecycle;                    // Schedule NPC to become inactive and recycled
            public int nameSeed;                            // Current nameseed of NPC
            public GameObject gameObject;                   // GameObject for NPC
            public float distanceToPlayer;                  // Last measured distance to player
        }

        #endregion

        #region Unity

        private void Start()
        {
            populationPool.Capacity = MaxPoolSize;
        }

        private void Update()
        {
            UpdatePoolItems();
        }

        #endregion

        #region Private Methods

        // Updates population pool and mark NPCs for recycling
        void UpdatePoolItems()
        {
            GameObject player = GameManager.Instance.PlayerObject;

            for (int i = 0; i < populationPool.Count; i++)
            {
                // Get poolItem and skip if not active or not setup
                MobileNPCPoolItem poolItem = populationPool[i];
                if (!poolItem.active || poolItem.gameObject == null)
                    continue;

                // Disable poolItem when too far from player
                poolItem.distanceToPlayer = Vector3.Distance(poolItem.gameObject.transform.position, player.transform.position);
                if (poolItem.distanceToPlayer > MaxDistanceFromPlayer)
                    poolItem.scheduleRecycle = true;
                else
                    poolItem.scheduleRecycle = false;

                // Update poolItem data
                populationPool[i] = poolItem;
            }
        }

        #endregion
    }
}