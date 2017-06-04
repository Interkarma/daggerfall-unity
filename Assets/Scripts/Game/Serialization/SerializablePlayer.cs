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
using DaggerfallWorkshop.Game.Entity;

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
            data.playerEntity.skillUses = entity.SkillUses;
            data.playerEntity.timeOfLastSkillIncreaseCheck = entity.TimeOfLastSkillIncreaseCheck;
            data.playerEntity.startingLevelUpSkillSum = entity.StartingLevelUpSkillSum;
            data.playerEntity.equipTable = entity.ItemEquipTable.SerializeEquipTable();
            data.playerEntity.items = entity.Items.SerializeItems();
            data.playerEntity.wagonItems = entity.WagonItems.SerializeItems();
            data.playerEntity.otherItems = entity.OtherItems.SerializeItems();
            data.playerEntity.goldPieces = entity.GoldPieces;

            // Store player position data
            data.playerPosition = new PlayerPositionData_v1();
            data.playerPosition.position = transform.position;
            data.playerPosition.yaw = playerMouseLook.Yaw;
            data.playerPosition.pitch = playerMouseLook.Pitch;
            data.playerPosition.isCrouching = playerMotor.IsCrouching;
            data.playerPosition.worldPosX = StreamingWorld.LocalPlayerGPS.WorldX;
            data.playerPosition.worldPosZ = StreamingWorld.LocalPlayerGPS.WorldZ;
            data.playerPosition.insideDungeon = playerEnterExit.IsPlayerInsideDungeon;
            data.playerPosition.insideBuilding = playerEnterExit.IsPlayerInsideBuilding;
            data.playerPosition.terrainSamplerName = DaggerfallUnity.Instance.TerrainSampler.ToString();
            data.playerPosition.terrainSamplerVersion = DaggerfallUnity.Instance.TerrainSampler.Version;
            data.playerPosition.weather = GameManager.Instance.WeatherManager.PlayerWeather.WeatherType;

            // Store weapon state
            data.weaponDrawn = !weaponManager.Sheathed;

            // Store building exterior door data
            if (playerEnterExit.IsPlayerInsideBuilding)
            {
                data.playerPosition.exteriorDoors = playerEnterExit.ExteriorDoors;
            }

            return data;
        }

        public object GetFactionSaveData()
        {
            FactionData_v1 factionData = new FactionData_v1();

            PlayerEntity entity = playerEntityBehaviour.Entity as PlayerEntity;
            factionData.factionDict = entity.FactionData.FactionDict;
            factionData.factionNameToIDDict = entity.FactionData.FactionNameToIDDict;

            return factionData;
        }

        public void RestoreFactionData(FactionData_v1 factionData)
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
            entity.SkillUses = data.playerEntity.skillUses;
            entity.TimeOfLastSkillIncreaseCheck = data.playerEntity.timeOfLastSkillIncreaseCheck;
            entity.StartingLevelUpSkillSum = data.playerEntity.startingLevelUpSkillSum;
            entity.Items.DeserializeItems(data.playerEntity.items);
            entity.WagonItems.DeserializeItems(data.playerEntity.wagonItems);
            entity.OtherItems.DeserializeItems(data.playerEntity.otherItems);
            entity.ItemEquipTable.DeserializeEquipTable(data.playerEntity.equipTable, entity.Items);
            entity.GoldPieces = data.playerEntity.goldPieces;
            entity.SetCurrentLevelUpSkillSum();

            // Fill in missing data for saves
            if (entity.StartingLevelUpSkillSum <= 0)
                entity.EstimateStartingLevelUpSkillSum();
            if (entity.SkillUses == null)
                entity.SkillUses = new short[DaggerfallSkills.Count];

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

            // Lower player position flag if inside with no doors
            if (data.playerPosition.insideBuilding && !hasExteriorDoors)
            {
                restorePlayerPosition = false;
            }

            // Restore player position
            if (restorePlayerPosition)
            {
                transform.position = data.playerPosition.position;
                playerMouseLook.Yaw = data.playerPosition.yaw;
                playerMouseLook.Pitch = data.playerPosition.pitch;
                playerMotor.IsCrouching = data.playerPosition.isCrouching;
            }

            // Restore sheath state
            weaponManager.Sheathed = !data.weaponDrawn;
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