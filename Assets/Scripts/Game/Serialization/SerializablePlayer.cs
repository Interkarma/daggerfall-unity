// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut
// 
// Notes:
//

using UnityEngine;
using System;
using DaggerfallWorkshop.Game.Entity;
using System.Collections.Generic;
using System.Linq;

namespace DaggerfallWorkshop.Game.Serialization
{
    /// <summary>
    /// Implements Player serialization. Should be attached to Player GameObject.
    /// </summary>
    public class SerializablePlayer : MonoBehaviour, ISerializableGameObject
    {
        #region Fields

        PlayerEnterExit playerEnterExit;
        StreamingWorld streamingWorld;
        Camera playerCamera;
        PlayerMouseLook playerMouseLook;
        PlayerMotor playerMotor;
        DaggerfallEntityBehaviour playerEntityBehaviour;
        WeaponManager weaponManager;
        TransportManager transportManager;

        #endregion

        #region Properties

        StreamingWorld StreamingWorld
        {
            get { return (streamingWorld != null) ? streamingWorld : FindStreamingWorld(); }
        }

        #endregion

        #region Unity

        void Awake()
        {
            playerEnterExit = GetComponent<PlayerEnterExit>();
            if (!playerEnterExit)
                throw new Exception("PlayerEnterExit not found.");

            playerCamera = GetComponentInChildren<Camera>();
            if (!playerCamera)
                throw new Exception("Player Camera not found.");

            playerMouseLook = playerCamera.GetComponent<PlayerMouseLook>();
            if (!playerMouseLook)
                throw new Exception("PlayerMouseLook not found.");

            playerMotor = GetComponent<PlayerMotor>();
            if (!playerMotor)
                throw new Exception("PlayerMotor not found.");

            playerEntityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            if (!playerEntityBehaviour)
                throw new Exception("PlayerEntityBehaviour not found.");

            weaponManager = GetComponent<WeaponManager>();
            if (!weaponManager)
                throw new Exception("WeaponManager not found.");

            transportManager = GetComponent<TransportManager>();
            if (!transportManager)
                throw new Exception("TransportManager not found.");

            SaveLoadManager.RegisterSerializableGameObject(this);
        }

        void OnDestroy()
        {
            SaveLoadManager.DeregisterSerializableGameObject(this);
        }

        #endregion

        #region ISerializableGameObject

        public ulong LoadID { get { return 1; } }            // Only saves local player
        public bool ShouldSave { get { return true; } }     // Always save player

        public object GetSaveData()
        {
            if (!playerEnterExit || !StreamingWorld)
                return null;

            PlayerData_v1 data = new PlayerData_v1();

            // Store player entity data
            PlayerEntity entity = playerEntityBehaviour.Entity as PlayerEntity;
            data.playerEntity = new PlayerEntityData_v1();
            data.playerEntity.gender = entity.Gender;
            data.playerEntity.raceTemplate = entity.RaceTemplate;
            data.playerEntity.faceIndex = entity.FaceIndex;
            data.playerEntity.reflexes = entity.Reflexes;
            data.playerEntity.careerTemplate = entity.Career;
            data.playerEntity.name = entity.Name;
            data.playerEntity.level = entity.Level;
            data.playerEntity.stats = entity.Stats;
            data.playerEntity.skills = entity.Skills;
            data.playerEntity.maxHealth = entity.MaxHealth;
            data.playerEntity.currentHealth = entity.CurrentHealth;
            data.playerEntity.currentFatigue = entity.CurrentFatigue;
            data.playerEntity.currentMagicka = entity.CurrentMagicka;
            data.playerEntity.currentBreath = entity.CurrentBreath;
            data.playerEntity.skillUses = entity.SkillUses;
            data.playerEntity.timeOfLastSkillIncreaseCheck = entity.TimeOfLastSkillIncreaseCheck;
            data.playerEntity.timeOfLastSkillTraining = entity.TimeOfLastSkillTraining;
            data.playerEntity.startingLevelUpSkillSum = entity.StartingLevelUpSkillSum;
            data.playerEntity.equipTable = entity.ItemEquipTable.SerializeEquipTable();
            data.playerEntity.items = entity.Items.SerializeItems();
            data.playerEntity.wagonItems = entity.WagonItems.SerializeItems();
            data.playerEntity.otherItems = entity.OtherItems.SerializeItems();
            data.playerEntity.goldPieces = entity.GoldPieces;
            data.playerEntity.globalVars = entity.GlobalVars.SerializeGlobalVars();
            data.playerEntity.minMetalToHit = entity.MinMetalToHit;
            data.playerEntity.biographyResistDiseaseMod = entity.BiographyResistDiseaseMod;
            data.playerEntity.biographyResistMagicMod = entity.BiographyResistMagicMod;
            data.playerEntity.biographyAvoidHitMod = entity.BiographyAvoidHitMod;
            data.playerEntity.biographyResistPoisonMod = entity.BiographyResistPoisonMod;
            data.playerEntity.biographyFatigueMod = entity.BiographyFatigueMod;
            data.playerEntity.biographyReactionMod = entity.BiographyReactionMod;
            data.playerEntity.timeForThievesGuildLetter = entity.TimeForThievesGuildLetter;
            data.playerEntity.timeForDarkBrotherhoodLetter = entity.TimeForDarkBrotherhoodLetter;
            data.playerEntity.thievesGuildRequirementTally = entity.ThievesGuildRequirementTally;
            data.playerEntity.darkBrotherhoodRequirementTally = entity.DarkBrotherhoodRequirementTally;
            data.playerEntity.lastTimePlayerAteOrDrankAtTavern = entity.LastTimePlayerAteOrDrankAtTavern;

            data.playerEntity.regionData = entity.RegionData;
            data.playerEntity.rentedRooms = entity.RentedRooms.ToArray();
            data.playerEntity.disease = entity.Disease.GetSaveData();

            // Store player position data
            data.playerPosition = GetPlayerPositionData();

            // Store weapon state
            data.weaponDrawn = !weaponManager.Sheathed;
            // Store transport mode
            data.transportMode = transportManager.TransportMode;
            // Store pre boarding ship position
            data.boardShipPosition = transportManager.BoardShipPosition;

            // Store building exterior door data
            if (playerEnterExit.IsPlayerInsideBuilding)
            {
                data.playerPosition.exteriorDoors = playerEnterExit.ExteriorDoors;
                data.playerPosition.buildingDiscoveryData = playerEnterExit.BuildingDiscoveryData;
            }
            // Store guild memberships
            data.guildMemberships = GameManager.Instance.GuildManager.GetMembershipData();

            return data;
        }

        public PlayerPositionData_v1 GetPlayerPositionData()
        {
            PlayerPositionData_v1 playerPosition = new PlayerPositionData_v1();
            playerPosition.position = transform.position;
            playerPosition.yaw = playerMouseLook.Yaw;
            playerPosition.pitch = playerMouseLook.Pitch;
            playerPosition.isCrouching = playerMotor.IsCrouching;
            playerPosition.worldPosX = StreamingWorld.LocalPlayerGPS.WorldX;
            playerPosition.worldPosZ = StreamingWorld.LocalPlayerGPS.WorldZ;
            playerPosition.insideDungeon = playerEnterExit.IsPlayerInsideDungeon;
            playerPosition.insideBuilding = playerEnterExit.IsPlayerInsideBuilding;
            playerPosition.insideOpenShop = playerEnterExit.IsPlayerInsideOpenShop;
            playerPosition.terrainSamplerName = DaggerfallUnity.Instance.TerrainSampler.ToString();
            playerPosition.terrainSamplerVersion = DaggerfallUnity.Instance.TerrainSampler.Version;
            playerPosition.weather = GameManager.Instance.WeatherManager.PlayerWeather.WeatherType;
            return playerPosition;
        }

        public object GetFactionSaveData()
        {
            FactionData_v2 factionData = new FactionData_v2();

            PlayerEntity entity = playerEntityBehaviour.Entity as PlayerEntity;
            factionData.factionDict = entity.FactionData.FactionDict;
            factionData.factionNameToIDDict = entity.FactionData.FactionNameToIDDict;

            return factionData;
        }

        public void RestoreFactionData(FactionData_v2 factionData)
        {
            PlayerEntity entity = playerEntityBehaviour.Entity as PlayerEntity;
            entity.FactionData.FactionDict = factionData.factionDict;
            entity.FactionData.FactionNameToIDDict = factionData.factionNameToIDDict;
        }

        public void RestoreSaveData(object dataIn)
        {
            if (!playerEnterExit || !StreamingWorld || !playerCamera || !playerMouseLook)
                return;

            // Restore player entity data
            PlayerData_v1 data = (PlayerData_v1)dataIn;
            PlayerEntity entity = playerEntityBehaviour.Entity as PlayerEntity;

            entity.Gender = data.playerEntity.gender;
            entity.RaceTemplate = data.playerEntity.raceTemplate;
            entity.FaceIndex = data.playerEntity.faceIndex;
            entity.Reflexes = data.playerEntity.reflexes;
            entity.Career = data.playerEntity.careerTemplate;
            entity.Name = data.playerEntity.name;
            entity.Level = data.playerEntity.level;
            entity.Stats = data.playerEntity.stats;
            entity.Skills = data.playerEntity.skills;
            entity.MaxHealth = data.playerEntity.maxHealth;
            entity.CurrentHealth = data.playerEntity.currentHealth;
            entity.CurrentFatigue = data.playerEntity.currentFatigue;
            entity.CurrentMagicka = data.playerEntity.currentMagicka;
            entity.CurrentBreath = data.playerEntity.currentBreath;
            entity.SkillUses = data.playerEntity.skillUses;
            entity.TimeOfLastSkillIncreaseCheck = data.playerEntity.timeOfLastSkillIncreaseCheck;
            entity.TimeOfLastSkillTraining = data.playerEntity.timeOfLastSkillTraining;
            entity.StartingLevelUpSkillSum = data.playerEntity.startingLevelUpSkillSum;
            entity.Items.DeserializeItems(data.playerEntity.items);
            entity.WagonItems.DeserializeItems(data.playerEntity.wagonItems);
            entity.OtherItems.DeserializeItems(data.playerEntity.otherItems);
            entity.ItemEquipTable.DeserializeEquipTable(data.playerEntity.equipTable, entity.Items);
            entity.GoldPieces = data.playerEntity.goldPieces;
            entity.GlobalVars.DeserializeGlobalVars(data.playerEntity.globalVars);
            entity.MinMetalToHit = data.playerEntity.minMetalToHit;
            entity.BiographyResistDiseaseMod = data.playerEntity.biographyResistDiseaseMod;
            entity.BiographyResistMagicMod = data.playerEntity.biographyResistMagicMod;
            entity.BiographyAvoidHitMod = data.playerEntity.biographyAvoidHitMod;
            entity.BiographyResistPoisonMod = data.playerEntity.biographyResistPoisonMod;
            entity.BiographyFatigueMod = data.playerEntity.biographyFatigueMod;
            entity.BiographyReactionMod = data.playerEntity.biographyReactionMod;
            entity.TimeForThievesGuildLetter = data.playerEntity.timeForThievesGuildLetter;
            entity.TimeForDarkBrotherhoodLetter = data.playerEntity.timeForDarkBrotherhoodLetter;
            entity.ThievesGuildRequirementTally = data.playerEntity.thievesGuildRequirementTally;
            entity.DarkBrotherhoodRequirementTally = data.playerEntity.darkBrotherhoodRequirementTally;
            entity.LastTimePlayerAteOrDrankAtTavern = data.playerEntity.lastTimePlayerAteOrDrankAtTavern;
            entity.SetCurrentLevelUpSkillSum();

            entity.RentedRooms = (data.playerEntity.rentedRooms != null) ? data.playerEntity.rentedRooms.ToList() : new List<RoomRental_v1>();
            entity.Disease.RestoreSaveData(data.playerEntity.disease);

            if (data.playerEntity.regionData != null)
                entity.RegionData = data.playerEntity.regionData;
            else // If data doesn't exist, initialize to random values
                entity.InitializeRegionPrices();

            // Set time tracked in player entity
            entity.LastGameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

            // Fill in missing data for saves
            if (entity.StartingLevelUpSkillSum <= 0)
                entity.EstimateStartingLevelUpSkillSum();
            if (entity.SkillUses == null)
                entity.SkillUses = new short[DaggerfallSkills.Count];

            // Initialize body part armor values to 100 (no armor)
            for (int i = 0; i < entity.ArmorValues.Length; i++)
            {
                entity.ArmorValues[i] = 100;
            }

            // Apply armor values from equipped armor
            Items.DaggerfallUnityItem[] equipTable = entity.ItemEquipTable.EquipTable;
            for (int i = 0; i < equipTable.Length; i++)
            {
                if (equipTable[i] != null && (equipTable[i].ItemGroup == Items.ItemGroups.Armor))
                {
                    entity.UpdateEquippedArmorValues(equipTable[i], true);
                }
            }

            // Flag determines if player position is restored
            bool restorePlayerPosition = true;

            // Check exterior doors are included in save, we need these to exit building
            bool hasExteriorDoors;
            if (data.playerPosition.exteriorDoors == null || data.playerPosition.exteriorDoors.Length == 0)
                hasExteriorDoors = false;
            else
                hasExteriorDoors = true;

            // Lower player position flag if player outside and terrain sampler changed
            if (data.playerPosition.terrainSamplerName != DaggerfallUnity.Instance.TerrainSampler.ToString() ||
                data.playerPosition.terrainSamplerVersion != DaggerfallUnity.Instance.TerrainSampler.Version)
            {
                restorePlayerPosition = false;
            }

            // Restore building summary
            if (data.playerPosition.insideBuilding)
            {
                playerEnterExit.BuildingDiscoveryData = data.playerPosition.buildingDiscoveryData;
                playerEnterExit.IsPlayerInsideOpenShop = data.playerPosition.insideOpenShop;
            }

            // Lower player position flag if inside with no doors
            if (data.playerPosition.insideBuilding && !hasExteriorDoors)
            {
                restorePlayerPosition = false;
            }

            // Restore player position
            if (restorePlayerPosition)
            {
                RestorePosition(data.playerPosition);
            }

            // Restore sheath state
            weaponManager.Sheathed = !data.weaponDrawn;
            // Restore transport mode
            transportManager.TransportMode = data.transportMode;
            // Restore pre boarding ship position
            transportManager.BoardShipPosition = data.boardShipPosition;

            // Restore guild memberships
            GameManager.Instance.GuildManager.RestoreMembershipData(data.guildMemberships);
        }

        public void RestorePosition(PlayerPositionData_v1 positionData)
        {
            transform.position = positionData.position;
            playerMouseLook.Yaw = positionData.yaw;
            playerMouseLook.Pitch = positionData.pitch;
            playerMotor.IsCrouching = positionData.isCrouching;
        }

        #endregion

        #region Private Methods

        StreamingWorld FindStreamingWorld()
        {
            streamingWorld = FindObjectOfType<StreamingWorld>();
            if (!streamingWorld)
                throw new Exception("StreamingWorld not found.");

            return streamingWorld;
        }

        #endregion
    }
}