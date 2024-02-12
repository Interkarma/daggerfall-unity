// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Attached to every static NPC in the game and maintains two kinds of information:
    ///  1. From static layout data read from game files (RMB/RDB block data)
    ///  2. From instantiation context (in a dungeon, a building, etc.)
    /// This behaviour helps solve the problem of layout classes not knowing
    /// (or needing to know) about runtime quest state, and vice versa.
    /// It also decouples NPC information from Billboards.
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

        /// <summary>
        /// Gets display name of NPC from individual faction data or random seed.
        /// </summary>
        public string DisplayName
        {
            get { return GetDisplayName(); }
        }

        /// <summary>
        /// Checks if this is a child NPC using texture or faction.
        /// </summary>
        public bool IsChildNPC
        {
            get { return IsChildNPCData(Data); }
        }

        /// <summary>
        /// Sets the NPC name bank for name generation.
        /// </summary>
        public NameHelper.BankTypes NameBank
        {
            set { npcData.nameBank = value; }
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
            public Races race;
            public Context context;
            public int mapID;

            // Derived at runtime
            public int locationID;
            public int buildingKey;
            public NameHelper.BankTypes nameBank;

            // appearance (which texture used for billboard)
            public int billboardArchiveIndex;
            public int billboardRecordIndex;
        }

        /// <summary>
        /// The context of where NPC belongs in layout data.
        /// </summary>
        public enum Context
        {
            Custom,         // Can be found anywhere, usually created by quest system
            Dungeon,        // Home is inside a dungeon
            Building,       // Home is inside a building
        }

        #endregion

        #region Unity

        private void Start()
        {
            // Get runtime-available data on start
            SetRuntimeData();

            // Assign QuestResourceBehaviour if this NPC matches a known Questor for an active quest
            AssignQuestResourceBehaviour();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets NPC data from RDB layout.
        /// </summary>
        public void SetLayoutData(DFBlock.RdbObject obj)
        {
            PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;
            SetLayoutData(ref npcData,
                obj.XPos, obj.YPos, obj.ZPos,
                obj.Resources.FlatResource.Flags,
                obj.Resources.FlatResource.FactionOrMobileId,
                obj.Resources.FlatResource.TextureArchive,
                obj.Resources.FlatResource.TextureRecord,
                obj.Resources.FlatResource.Position,
                playerGPS.CurrentMapID,
                playerGPS.CurrentLocation.LocationIndex,
                0);
            npcData.context = Context.Dungeon;
        }

        /// <summary>
        /// Sets NPC data from RMB layout.
        /// </summary>
        public void SetLayoutData(DFBlock.RmbBlockPeopleRecord obj, int buildingKey = 0)
        {
            PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;
            SetLayoutData(ref npcData,
                obj.XPos, obj.YPos, obj.ZPos,
                obj.Flags,
                obj.FactionID,
                obj.TextureArchive,
                obj.TextureRecord,
                obj.Position,
                playerGPS.CurrentMapID,
                playerGPS.CurrentLocation.LocationIndex,
                buildingKey);
            npcData.context = Context.Building;
        }

        /// <summary>
        /// Sets NPC data from RMB layout flat record. (exterior NPCs)
        /// Requires mapID and locationIndex to be passed in as layout may occur without player being in the location.
        /// </summary>
        public void SetLayoutData(DFBlock.RmbBlockFlatObjectRecord obj, int mapId, int locationIndex)
        {
            // Gender flag is invalid for RMB exterior NPCs: get it from FLATS.CFG instead
            int flatID = FlatsFile.GetFlatID(obj.TextureArchive, obj.TextureRecord);
            if (DaggerfallUnity.Instance.ContentReader.FlatsFileReader.GetFlatData(flatID, out FlatsFile.FlatData flatCFG))
            {
                if (flatCFG.gender.Contains("2"))
                    obj.Flags |= 32;
                else
                    obj.Flags &= 223;
            }

            SetLayoutData(ref npcData,
                obj.XPos, obj.YPos, obj.ZPos,
                obj.Flags,
                obj.FactionID,
                obj.TextureArchive,
                obj.TextureRecord,
                obj.Position,
                mapId,
                locationIndex,
                0);
            npcData.context = Context.Custom;
        }

        public static void SetLayoutData(ref NPCData data, int XPos, int YPos, int ZPos, int flags, int factionId, int archive, int record, long position, int mapId, int locationIndex, int buildingKey)
        {
            // Store common layout data
            data.hash = GetPositionHash(XPos, YPos, ZPos);
            data.flags = flags;
            data.factionID = factionId;
            data.billboardArchiveIndex = archive;
            data.billboardRecordIndex = record;
            data.nameSeed = (int)position ^ buildingKey + locationIndex;
            data.gender = ((flags & 32) == 32) ? Genders.Female : Genders.Male;
            data.race = GetRaceFromFaction(factionId);
            data.buildingKey = buildingKey;
            data.mapID = mapId;
        }

        /// <summary>
        /// Sets NPC data from quest Person resource.
        /// </summary>
        /// <param name="person"></param>
        public void SetLayoutData(int x, int y, int z, Person person)
        {
            SetLayoutData(x, y, z, person.Gender, person.FactionIndex, person.NameSeed);
        }

        /// <summary>
        /// Sets NPC data directly.
        /// </summary>
        public void SetLayoutData(int x, int y, int z, Genders gender, int factionID = 0, int nameSeed = -1)
        {
            SetLayoutData(GetPositionHash(x, y, z), gender, factionID, nameSeed);
        }

        /// <summary>
        /// Sets NPC data directly.
        /// </summary>
        public void SetLayoutData(int hash, Genders gender, int factionID = 0, int nameSeed = -1)
        {
            // Store common layout data
            npcData.hash = hash;
            npcData.flags = (gender == Genders.Male) ? 0 : 32;
            npcData.factionID = factionID;
            npcData.nameSeed = (nameSeed == -1) ? npcData.hash : nameSeed;
            npcData.gender = gender;
            npcData.race = GetRaceFromFaction(factionID);
            npcData.context = Context.Custom;
        }

        /// <summary>
        /// Assigns a new QuestResourceBehaviour component if this is a Questor NPC.
        /// Can happen either at runtime or during scene layout.
        /// </summary>
        public void AssignQuestResourceBehaviour()
        {
            // Static NPC data must match a known questor Person resource
            Person questorPerson = QuestMachine.Instance.ActiveQuestor(Data);
            if (questorPerson == null)
                return;

            // Can only have a single QuestResourceBehaviour
            if (GetComponent<QuestResourceBehaviour>())
                return;

            // Assign new QuestResourceBehaviour and link to Person resource in quest system
            QuestResourceBehaviour resourceBehaviour = gameObject.AddComponent<QuestResourceBehaviour>();
            if (resourceBehaviour)
            {
                resourceBehaviour.AssignResource(questorPerson);
                Debug.LogFormat("Added new QuestResourceBehaviour to object {0} and assigned Questor Person resource {1}", gameObject.name, questorPerson.DisplayName);
            }
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

            // Store name bank
            npcData.nameBank = GameManager.Instance.PlayerGPS.GetNameBankOfCurrentRegion();
        }

        /// <summary>
        /// Gets display name of NPC as fixed individual or from random seed.
        /// </summary>
        string GetDisplayName()
        {
            FactionFile.FactionData factionData;
            bool foundFaction = GameManager.Instance.PlayerEntity.FactionData.GetFactionData(npcData.factionID, out factionData);
            if (foundFaction && factionData.type == (int)FactionFile.FactionTypes.Individual)
            {
                return factionData.name;
            }
            else
            {
                DFRandom.srand(npcData.nameSeed);
                return DaggerfallUnity.Instance.NameHelper.FullName(npcData.nameBank, npcData.gender);
            }
        }

        /// <summary>
        /// Creates a hash from fixed-point layout position.
        /// </summary>
        public static int GetPositionHash(int x, int y, int z)
        {
            return x ^ y << 2 ^ z >> 2;
        }

        /// <summary>
        /// Check if a known child NPC.
        /// </summary>
        /// <returns>True if NPC data matches known children textures or faction.</returns>
        public static bool IsChildNPCData(NPCData data)
        {
            const int childrenFactionID = 514;

            bool isChildNPCTexture = DaggerfallWorkshop.Utility.TextureReader.IsChildNPCTexture(data.billboardArchiveIndex, data.billboardRecordIndex);
            bool isChildrenFaction = data.factionID == childrenFactionID;

            return isChildNPCTexture || isChildrenFaction;
        }

        /// <summary>
        /// Return the race corresponding to a given faction ID.
        /// </summary>
        /// <param name="factionId"></param>
        /// <returns>The faction race if available, otherwise the race of the current region.</returns>
        public static Races GetRaceFromFaction(int factionId)
        {
            if (factionId != 0)
            {
                FactionFile.FactionData fd;
                GameManager.Instance.PlayerEntity.FactionData.GetFactionData(factionId, out fd);
                Races race = RaceTemplate.GetRaceFromFactionRace((FactionFile.FactionRaces)fd.race);
                if (race != Races.None)
                    return race;
            }

            return GameManager.Instance.PlayerGPS.GetRaceOfCurrentRegion();
        }

        #endregion
    }
}