// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut, Numidium
// 
// Notes:
//

using UnityEngine;
using System;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using System.Collections.Generic;
using System.Linq;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect.Arena2;

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
            data.playerEntity.raceTemplate = entity.BirthRaceTemplate;
            data.playerEntity.faceIndex = entity.FaceIndex;
            data.playerEntity.reflexes = entity.Reflexes;
            data.playerEntity.careerTemplate = entity.Career;
            data.playerEntity.name = entity.Name;
            data.playerEntity.level = entity.Level;
            data.playerEntity.stats = entity.Stats;
            data.playerEntity.skills = entity.Skills;
            data.playerEntity.resistances = entity.Resistances;
            data.playerEntity.maxHealth = entity.RawMaxHealth;
            data.playerEntity.currentHealth = entity.CurrentHealth;
            data.playerEntity.currentFatigue = entity.CurrentFatigue;
            data.playerEntity.currentMagicka = entity.CurrentMagicka;
            data.playerEntity.currentBreath = entity.CurrentBreath;
            data.playerEntity.skillUses = entity.SkillUses;
            data.playerEntity.timeOfLastSkillIncreaseCheck = entity.TimeOfLastSkillIncreaseCheck;
            data.playerEntity.skillsRecentlyRaised = entity.SkillsRecentlyRaised;
            data.playerEntity.timeOfLastSkillTraining = entity.TimeOfLastSkillTraining;
            data.playerEntity.startingLevelUpSkillSum = entity.StartingLevelUpSkillSum;
            data.playerEntity.currentLevelUpSkillSum = entity.CurrentLevelUpSkillSum;
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
            data.playerEntity.timeToBecomeVampireOrWerebeast = entity.TimeToBecomeVampireOrWerebeast;
            data.playerEntity.lastTimePlayerAteOrDrankAtTavern = entity.LastTimePlayerAteOrDrankAtTavern;
            data.playerEntity.spellbook = entity.SerializeSpellbook();
            data.playerEntity.crimeCommitted = entity.CrimeCommitted;
            data.playerEntity.haveShownSurrenderToGuardsDialogue = entity.HaveShownSurrenderToGuardsDialogue;
            data.playerEntity.lightSourceUID = (entity.LightSource == null) ? 0 : entity.LightSource.UID;
            data.playerEntity.reputationCommoners = entity.SGroupReputations[(int)FactionFile.SocialGroups.Commoners];
            data.playerEntity.reputationMerchants = entity.SGroupReputations[(int)FactionFile.SocialGroups.Merchants];
            data.playerEntity.reputationScholars = entity.SGroupReputations[(int)FactionFile.SocialGroups.Scholars];
            data.playerEntity.reputationNobility = entity.SGroupReputations[(int)FactionFile.SocialGroups.Nobility];
            data.playerEntity.reputationUnderworld = entity.SGroupReputations[(int)FactionFile.SocialGroups.Underworld];
            data.playerEntity.reputationSGroup5 = entity.SGroupReputations[(int)FactionFile.SocialGroups.SGroup5];
            data.playerEntity.reputationSupernaturalBeings = entity.SGroupReputations[(int)FactionFile.SocialGroups.SupernaturalBeings];
            data.playerEntity.reputationGuildMembers = entity.SGroupReputations[(int)FactionFile.SocialGroups.GuildMembers];
            data.playerEntity.reputationSGroup8 = entity.SGroupReputations[(int)FactionFile.SocialGroups.SGroup8];
            data.playerEntity.reputationSGroup9 = entity.SGroupReputations[(int)FactionFile.SocialGroups.SGroup9];
            data.playerEntity.reputationSGroup10 = entity.SGroupReputations[(int)FactionFile.SocialGroups.SGroup10];
            data.playerEntity.previousVampireClan = entity.PreviousVampireClan;
            data.playerEntity.daedraSummonDay = entity.DaedraSummonDay;
            data.playerEntity.daedraSummonIndex = entity.DaedraSummonIndex;

            data.playerEntity.regionData = entity.RegionData;
            data.playerEntity.rentedRooms = entity.RentedRooms.ToArray();

            // Store player position data
            data.playerPosition = GetPlayerPositionData();

            // Store weapon state
            data.weaponDrawn = !weaponManager.Sheathed;
            data.usingLeftHand = !weaponManager.UsingRightHand;
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
            data.vampireMemberships = GameManager.Instance.GuildManager.GetMembershipData(true);
            // Store one time quest acceptances
            data.oneTimeQuestsAccepted = GameManager.Instance.QuestListsManager.oneTimeQuestsAccepted;

            // Store instanced effect bundles
            data.playerEntity.instancedEffectBundles = GetComponent<EntityEffectManager>().GetInstancedBundlesSaveData();

            return data;
        }

        public PlayerPositionData_v1 GetPlayerPositionData()
        {
            PlayerPositionData_v1 playerPosition = new PlayerPositionData_v1();
            //PlayerHeightChanger heightChanger = GetComponent<PlayerHeightChanger>();
            playerPosition.position = transform.position;
            playerPosition.worldCompensation = GameManager.Instance.StreamingWorld.WorldCompensation;
            playerPosition.worldContext = playerEnterExit.WorldContext;
            playerPosition.floatingOriginVersion = FloatingOrigin.floatingOriginVersion;
            playerPosition.yaw = playerMouseLook.Yaw;
            playerPosition.pitch = playerMouseLook.Pitch;
            playerPosition.isCrouching = playerMotor.IsCrouching;
            playerPosition.worldPosX = StreamingWorld.LocalPlayerGPS.WorldX;
            playerPosition.worldPosZ = StreamingWorld.LocalPlayerGPS.WorldZ;
            playerPosition.insideDungeon = playerEnterExit.IsPlayerInsideDungeon;
            playerPosition.insideBuilding = playerEnterExit.IsPlayerInsideBuilding;
            playerPosition.insideOpenShop = playerEnterExit.IsPlayerInsideOpenShop;
            playerPosition.insideTavern = playerEnterExit.IsPlayerInsideTavern;
            playerPosition.insideResidence = playerEnterExit.IsPlayerInsideResidence;
            playerPosition.terrainSamplerName = DaggerfallUnity.Instance.TerrainSampler.ToString();
            playerPosition.terrainSamplerVersion = DaggerfallUnity.Instance.TerrainSampler.Version;
            playerPosition.smallerDungeonsState = (DaggerfallUnity.Settings.SmallerDungeons) ? QuestSmallerDungeonsState.Enabled : QuestSmallerDungeonsState.Disabled;
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
            // There are a mixture of saved games that were made using the 1-based region IDs
            // straight from FACTION.TXT, while others made earlier used 0-based IDs.
            // Here we convert saves with 1-based to be 0-based. 0-based is assumed by code using these IDs.
            if (factionData.factionDict[201].region == 18) // To check for 1-based, see if Daggerfall is 18 rather than 17
            {
                Debug.Log("Updating 1-based faction region IDs to 0-based.");
                entity.FactionData.FactionDict = new Dictionary<int, DaggerfallConnect.Arena2.FactionFile.FactionData>();
                foreach (int key in factionData.factionDict.Keys)
                {
                    var dict = factionData.factionDict[key];
                    if (factionData.factionDict[key].region != -1)
                        dict.region--;

                    entity.FactionData.FactionDict.Add(key, dict);
                }
            }
            else
                entity.FactionData.FactionDict = factionData.factionDict;

            entity.FactionData.FactionNameToIDDict = factionData.factionNameToIDDict;
            // Add any registered custom factions
            entity.FactionData.AddCustomFactions();
        }

        public void RestoreSaveData(object dataIn)
        {
            if (!playerEnterExit || !StreamingWorld || !playerCamera || !playerMouseLook)
                return;

            // Restore player entity data
            PlayerData_v1 data = (PlayerData_v1)dataIn;
            PlayerEntity entity = playerEntityBehaviour.Entity as PlayerEntity;

            entity.Gender = data.playerEntity.gender;
            entity.BirthRaceTemplate = data.playerEntity.raceTemplate;
            entity.FaceIndex = data.playerEntity.faceIndex;
            entity.Reflexes = data.playerEntity.reflexes;
            entity.Career = data.playerEntity.careerTemplate;
            entity.Name = data.playerEntity.name;
            entity.Level = data.playerEntity.level;
            entity.Stats = data.playerEntity.stats;
            entity.Skills = data.playerEntity.skills;
            entity.Resistances = (data.playerEntity.resistances != null) ? data.playerEntity.resistances : new DaggerfallResistances();
            entity.MaxHealth = data.playerEntity.maxHealth;
            entity.SetHealth(data.playerEntity.currentHealth, true);
            entity.SetFatigue(data.playerEntity.currentFatigue, true);
            entity.SetMagicka(data.playerEntity.currentMagicka, true);
            entity.CurrentBreath = data.playerEntity.currentBreath;
            entity.SkillUses = data.playerEntity.skillUses;
            entity.SkillsRecentlyRaised = (data.playerEntity.skillsRecentlyRaised != null ? data.playerEntity.skillsRecentlyRaised : new uint[2]);
            entity.TimeOfLastSkillIncreaseCheck = data.playerEntity.timeOfLastSkillIncreaseCheck;
            entity.TimeOfLastSkillTraining = data.playerEntity.timeOfLastSkillTraining;
            entity.StartingLevelUpSkillSum = data.playerEntity.startingLevelUpSkillSum;
            if ((entity.CurrentLevelUpSkillSum = data.playerEntity.currentLevelUpSkillSum) == 0)
                entity.SetCurrentLevelUpSkillSum();
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
            entity.TimeToBecomeVampireOrWerebeast = data.playerEntity.timeToBecomeVampireOrWerebeast;
            entity.LastTimePlayerAteOrDrankAtTavern = data.playerEntity.lastTimePlayerAteOrDrankAtTavern;
            entity.CrimeCommitted = data.playerEntity.crimeCommitted;
            entity.HaveShownSurrenderToGuardsDialogue = data.playerEntity.haveShownSurrenderToGuardsDialogue;
            if (DaggerfallUnity.Settings.PlayerTorchFromItems)
                entity.LightSource = entity.Items.GetItem(data.playerEntity.lightSourceUID);
            entity.SGroupReputations[(int)FactionFile.SocialGroups.Commoners] = data.playerEntity.reputationCommoners;
            entity.SGroupReputations[(int)FactionFile.SocialGroups.Merchants] = data.playerEntity.reputationMerchants;
            entity.SGroupReputations[(int)FactionFile.SocialGroups.Scholars] = data.playerEntity.reputationScholars;
            entity.SGroupReputations[(int)FactionFile.SocialGroups.Nobility] = data.playerEntity.reputationNobility;
            entity.SGroupReputations[(int)FactionFile.SocialGroups.Underworld] = data.playerEntity.reputationUnderworld;
            entity.SGroupReputations[(int)FactionFile.SocialGroups.SGroup5] = data.playerEntity.reputationSGroup5;
            entity.SGroupReputations[(int)FactionFile.SocialGroups.GuildMembers] = data.playerEntity.reputationGuildMembers;
            entity.SGroupReputations[(int)FactionFile.SocialGroups.SGroup8] = data.playerEntity.reputationSGroup8;
            entity.SGroupReputations[(int)FactionFile.SocialGroups.SGroup9] = data.playerEntity.reputationSGroup9;
            entity.SGroupReputations[(int)FactionFile.SocialGroups.SGroup10] = data.playerEntity.reputationSGroup10;
            entity.PreviousVampireClan = data.playerEntity.previousVampireClan;
            entity.DaedraSummonDay = data.playerEntity.daedraSummonDay;
            entity.DaedraSummonIndex = data.playerEntity.daedraSummonIndex;

            entity.RentedRooms = (data.playerEntity.rentedRooms != null) ? data.playerEntity.rentedRooms.ToList() : new List<RoomRental_v1>();

            // Set time tracked in player entity
            entity.LastGameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

            // Fill in missing data for saves.
            // Older saves may have regionData non-null but Values, Flags and Flags2 are uninitialized.
            // Just checking that any one of these is null is enough. Initialize to 0 (no conditions) for now to avoid null references.
            if (data.playerEntity.regionData != null && data.playerEntity.regionData[0].Values != null)
                entity.RegionData = data.playerEntity.regionData;
            else // If data doesn't exist, initialize it
                entity.InitializeRegionData();

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
                if (equipTable[i] != null)
                {
                    entity.UpdateEquippedArmorValues(equipTable[i], true);
                }
            }

            // Trim name strings as these might contain trailing whitespace characters previously imported from classic saves
            entity.Name = entity.Name.Trim();
            entity.Career.Name = entity.Career.Name.Trim();

            // Flag determines if player position is restored
            bool restorePlayerPosition = true;

            // Check exterior doors are included in save, we need these to exit building
            bool hasExteriorDoors;
            if (data.playerPosition.exteriorDoors == null || data.playerPosition.exteriorDoors.Length == 0)
                hasExteriorDoors = false;
            else
                hasExteriorDoors = true;

            // Lower player position flag if player outside and terrain sampler changed
            // Make an exception for dungeons as exterior world does not matter
            if ((data.playerPosition.terrainSamplerName != DaggerfallUnity.Instance.TerrainSampler.ToString() ||
                data.playerPosition.terrainSamplerVersion != DaggerfallUnity.Instance.TerrainSampler.Version) &&
                !data.playerPosition.insideDungeon)
            {
                restorePlayerPosition = false;
            }

            // Restore building summary
            if (data.playerPosition.insideBuilding)
            {
                playerEnterExit.BuildingDiscoveryData = data.playerPosition.buildingDiscoveryData;
                playerEnterExit.IsPlayerInsideOpenShop = data.playerPosition.insideOpenShop;
                playerEnterExit.IsPlayerInsideTavern = data.playerPosition.insideTavern;
                playerEnterExit.IsPlayerInsideResidence = data.playerPosition.insideResidence;
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
            weaponManager.UsingRightHand = !data.usingLeftHand;
            // Restore transport mode
            transportManager.TransportMode = data.transportMode;
            // Restore pre boarding ship position
            transportManager.BoardShipPosition = data.boardShipPosition;

            // Validate spellbook item
            DaggerfallUnity.Instance.ItemHelper.ValidateSpellbookItem(entity);

            // Restore guild memberships, also done early in SaveLoadManager for interiors
            GameManager.Instance.GuildManager.RestoreMembershipData(data.guildMemberships);
            GameManager.Instance.GuildManager.RestoreMembershipData(data.vampireMemberships, true);
            // Restore one time quest acceptances
            GameManager.Instance.QuestListsManager.oneTimeQuestsAccepted = data.oneTimeQuestsAccepted;

            entity.DeserializeSpellbook(data.playerEntity.spellbook);

            // Restore instanced effect bundles
            GetComponent<EntityEffectManager>().RestoreInstancedBundleSaveData(data.playerEntity.instancedEffectBundles);
        }

        public void RestorePosition(PlayerPositionData_v1 positionData)
        {
            // Floating origin v2 saves are only missing world context
            WorldContext playerContext = playerEnterExit.WorldContext;
            if (positionData.floatingOriginVersion == 2)
            {
                positionData.worldContext = playerContext;
            }

            // Restore position
            if (playerContext == WorldContext.Exterior)
            {
                RestoreExteriorPositionHandler(gameObject, positionData, playerContext);
            }
            else
            {
                transform.position = positionData.position;
            }

            // Restore orientation and crouch state
            playerMouseLook.Yaw = positionData.yaw;
            playerMouseLook.Pitch = positionData.pitch;
            playerMotor.IsCrouching = positionData.isCrouching;
        }

        #endregion

        #region Private Methods

        void RestoreExteriorPositionHandler(GameObject player, PlayerPositionData_v1 data, WorldContext playerContext)
        {
            // If player context matches serialized world context then player was saved after floating y change
            // Need to get relative difference between current and serialized world compensation to get actual y position
            if (playerContext == data.worldContext)
            {
                float diffY = GameManager.Instance.StreamingWorld.WorldCompensation.y - data.worldCompensation.y;
                player.transform.position = data.position + new Vector3(0, diffY, 0);
                return;
            }

            // Otherwise we migrate a legacy exterior position by adjusting for world compensation
            player.transform.position = data.position + GameManager.Instance.StreamingWorld.WorldCompensation;
        }

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