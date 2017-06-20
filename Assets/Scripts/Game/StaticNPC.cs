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

using System;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Attached to every static NPC in the game and maintains two kinds of information:
    ///  1. From static layout data read from game files (RMB/RDB block data)
    ///  2. From instantiation context (in a dungeon, a building, etc.)
    /// This behaviour helps solve the problem of layout classes not knowing
    /// (or needing to know) about runtime quest state, and vice versa.
    /// Other uses include Questor injection, hiding relocated NPCs, and disabling
    /// NPCs that have been permanently removed by quest system.
    /// 
    /// Notes:
    ///  * Static NPCs only exist inside buildings and dungeons, these are not the wandering NPCs in towns.
    ///  * This behaviour will be used to ultimately decouple NPC information from Billboards.
    ///  * Any NPC replacers (e.g. to 3D models) will need to assign this behaviour with original layout data.
    ///  * Correct NPC name seed is currently unknown, using offset position in layout data for now.
    /// </summary>
    public class StaticNPC : MonoBehaviour
    {
        #region Fields

        NPCData npcData;

        #endregion

        #region Properties

        /// <summary>
        /// Gets serializable data pack about this NPC.
        /// </summary>
        public NPCData Data
        {
            get { return npcData; }
        }

        #endregion

        #region Structs & Enums

        /// <summary>
        /// Holds information about this NPC.
        /// </summary>
        [Serializable]
        public struct NPCData
        {
            // Derived at layout
            public int hash;
            public int flags;
            public int factionID;
            public int nameSeed;
            public Genders gender;
            public Context context;

            // Derived at runtime
            public int mapID;
            public int locationID;
            public int buildingKey;
        }

        /// <summary>
        /// The context of where NPC belongs in layout data.
        /// </summary>
        public enum Context
        {
            None,
            Dungeon,
            Building,
        }

        #endregion

        #region Unity

        private void Start()
        {
            // Get runtime-available data on start
            SetRuntimeData();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets layout time data from RDB.
        /// </summary>
        public void SetLayoutData(DFBlock.RdbObject obj)
        {
            // Store common layout data
            npcData.hash = GetPositionHash(obj.XPos, obj.YPos, obj.ZPos);
            npcData.flags = obj.Resources.FlatResource.Flags;
            npcData.factionID = obj.Resources.FlatResource.FactionOrMobileId;
            npcData.nameSeed = (int)obj.Resources.FlatResource.Position;
            npcData.gender = ((npcData.flags & 32) == 32) ? Genders.Female : Genders.Male;
            npcData.context = Context.Dungeon;
        }

        /// <summary>
        /// Sets layout time data from RMB.
        /// </summary>
        public void SetLayoutData(DFBlock.RmbBlockPeopleRecord obj)
        {
            // Store common layout data
            npcData.hash = GetPositionHash(obj.XPos, obj.YPos, obj.ZPos);
            npcData.flags = obj.Flags;
            npcData.factionID = obj.FactionID;
            npcData.nameSeed = (int)obj.Position;
            npcData.gender = ((npcData.flags & 32) == 32) ? Genders.Female : Genders.Male;
            npcData.context = Context.Building;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets NPC information available at instantiation time.
        /// NPCs are only instantiated when player enters their context and layout places them in world.
        /// So a lot can be derived just from player behaviours.
        /// </summary>
        void SetRuntimeData()
        {
            // Get reference to player objects holding world information
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
            PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;

            // Store map ID
            npcData.mapID = playerGPS.CurrentMapID;

            // Store location ID (if any - not all locations carry a unique ID)
            npcData.locationID = (int)playerGPS.CurrentLocation.Exterior.ExteriorData.LocationId;

            // Store building key if player inside a building
            // This can be gleaned from any exterior door if one is available
            npcData.buildingKey = 0;
            if (playerEnterExit.IsPlayerInsideBuilding)
            {
                if (playerEnterExit.ExteriorDoors != null && playerEnterExit.ExteriorDoors.Length > 0)
                    npcData.buildingKey = playerEnterExit.ExteriorDoors[0].buildingKey;
            }
        }

        /// <summary>
        /// Creates a hash from fixed-point layout position.
        /// </summary>
        int GetPositionHash(int x, int y, int z)
        {
            return x ^ y << 2 ^ z >> 2;
        }

        #endregion
    }
}