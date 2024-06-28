// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: Numidium
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Assist player controller to enter and exit building interiors and dungeons.
    /// Should be attached to player object with PlayerGPS for climate tracking.
    /// </summary>
    public class PlayerEnterExit : MonoBehaviour
    {
        const HideFlags defaultHideFlags = HideFlags.None;
        UnderwaterFog underwaterFog;
        DaggerfallUnity dfUnity;
        CharacterController controller;
        bool isCreatingDungeonObjects = false;
        bool isPlayerInside = false;
        bool isPlayerInsideDungeon = false;
        bool isPlayerInsideDungeonCastle = false;
        bool isPlayerInsideSpecialArea = false;
        bool isPlayerInsideOpenShop = false;
        bool isPlayerSwimming = false;
        bool isPlayerSubmerged = false;
        bool isPlayerInSunlight = false;
        bool isPlayerInHolyPlace = false;
        bool isRespawning = false;
        bool lastInteriorStartFlag;
        bool displayAfloatMessage = false;
        DaggerfallInterior interior;
        DaggerfallDungeon dungeon;
        StreamingWorld world;
        PlayerGPS playerGPS;
        Entity.DaggerfallEntityBehaviour player;
        LevitateMotor levitateMotor;

        List<StaticDoor> exteriorDoors = new List<StaticDoor>();

        public GameObject ExteriorParent;
        public GameObject InteriorParent;
        public GameObject DungeonParent;
        public DaggerfallLocation OverrideLocation;

        int lastPlayerDungeonBlockIndex = -1;
        DFLocation.DungeonBlock playerDungeonBlockData = new DFLocation.DungeonBlock();

        /// <summary>
        /// If different than <c>10000</c> this is the height level of water in current dungeon.
        /// Otherwise player is not inside a dungeon with water.
        /// </summary>
        public short blockWaterLevel = 10000;

        DFLocation.BuildingTypes buildingType = DFLocation.BuildingTypes.None;
        ushort factionID = 0;
        PlayerGPS.DiscoveredBuilding buildingDiscoveryData;

        DaggerfallLocation holidayTextLocation;
        bool holidayTextPrimed = false;
        float holidayTextTimer = 0f;

        /// <summary>
        /// Gets player world context.
        /// </summary>
        public WorldContext WorldContext
        {
            get { return GetWorldContext(); }
        }

        /// <summary>
        /// Gets start flag from most recent interior transition.
        /// Helps inform other systems if first-time load or enter/exit transition
        /// </summary>
        public bool LastInteriorStartFlag
        {
            get { return lastInteriorStartFlag; }
        }

        /// <summary>
        /// True when GameObjectHelper is creating the RDB Base Game Objects
        /// </summary>
        public bool IsCreatingDungeonObjects
        {
            get { return isCreatingDungeonObjects; }
            set { isCreatingDungeonObjects = value; }
        }

        /// <summary>
        /// True when player is inside any structure.
        /// </summary>
        public bool IsPlayerInside
        {
            get { return isPlayerInside; }
        }

        /// <summary>
        /// True only when player is inside a building.
        /// </summary>
        public bool IsPlayerInsideBuilding
        {
            get { return (IsPlayerInside && !IsPlayerInsideDungeon); }
        }

        /// <summary>
        /// True only when player is inside a dungeon.
        /// </summary>
        public bool IsPlayerInsideDungeon
        {
            get { return isPlayerInsideDungeon; }
        }

        /// <summary>
        /// True only when player inside castle blocks of a dungeon.
        /// For example, main hall in Daggerfall castle.
        /// </summary>
        public bool IsPlayerInsideDungeonCastle
        {
            get { return isPlayerInsideDungeonCastle; }
        }

        /// <summary>
        /// True only when player inside special blocks of a dungeon.
        /// For example, treasure room in Daggerfall castle.
        /// </summary>
        public bool IsPlayerInsideSpecialArea
        {
            get { return isPlayerInsideSpecialArea; }
        }

        /// <summary>
        /// True when player is inside an open shop.
        /// Set upon entry, so doesn't matter if shop 'closes' with player inside.
        /// </summary>
        public bool IsPlayerInsideOpenShop
        {
            get { return isPlayerInsideOpenShop; }
            set { isPlayerInsideOpenShop = value; }
        }

        /// <summary>
        /// True when player is inside a tavern.
        /// Set upon entry.
        /// </summary>
        public bool IsPlayerInsideTavern { get; set; }

        /// <summary>
        /// True when player is inside a residence.
        /// Set upon entry.
        /// </summary>
        public bool IsPlayerInsideResidence { get; set; }

        /// <summary>
        /// True when player is swimming in water.
        /// </summary>
        public bool IsPlayerSwimming
        {
            get { return isPlayerSwimming; }
            set { isPlayerSwimming = value; }
        }

        /// <summary>
        /// True when player is submerged in water.
        /// </summary>
        public bool IsPlayerSubmerged
        {
            get { return isPlayerSubmerged; }
        }

        /// <summary>
        /// True when player is in sunlight.
        /// </summary>
        public bool IsPlayerInSunlight
        {
            get { return isPlayerInSunlight; }
        }

        /// <summary>
        /// True when player is in darkness.
        /// Same as !IsPlayerInSunlight.
        /// </summary>
        public bool IsPlayerInDarkness
        {
            get { return !isPlayerInSunlight; }
        }

        /// <summary>
        /// True when player is in a holy place.
        /// Holy places include all Temples and guildhalls of the Fighter Trainers (faction #849)
        /// https://en.uesp.net/wiki/Daggerfall:ClassMaker#Special_Disadvantages
        /// Refreshed once per game minute.
        /// </summary>
        public bool IsPlayerInHolyPlace
        {
            get { return isPlayerInHolyPlace; }
        }

        /// <summary>
        /// True when a player respawn is in progress.
        /// e.g. After loading a game or teleporting back to a marked location.
        /// </summary>
        public bool IsRespawning
        {
            get { return isRespawning; }
        }

        /// <summary>
        /// True when player just teleported into a dungeon via Teleport spell, otherwise false.
        /// Flag is only raised by Teleport spell and is lowered any time player exits a dungeon or interior, or teleports to a non-dungeon anchor.
        /// </summary>
        public bool PlayerTeleportedIntoDungeon { get; set; }

        /// <summary>
        /// Gets current player dungeon.
        /// Only valid when player is inside a dungeon.
        /// </summary>
        public DaggerfallDungeon Dungeon
        {
            get { return dungeon; }
        }

        /// <summary>
        /// Gets information about current player dungeon block.
        /// Only valid when player is inside a dungeon.
        /// </summary>
        public DFLocation.DungeonBlock DungeonBlock
        {
            get { return playerDungeonBlockData; }
        }

        /// <summary>
        /// Gets current building interior.
        /// Only valid when player inside building.
        /// </summary>
        public DaggerfallInterior Interior
        {
            get { return interior; }
        }

        /// <summary>
        /// Gets current building type.
        /// Only valid when player inside building.
        /// </summary>
        public DFLocation.BuildingTypes BuildingType
        {
            get { return buildingType; }
        }

        /// <summary>
        /// Gets current building's faction ID.
        /// </summary>
        public uint FactionID
        {
            get { return factionID; }
        }

        /// <summary>
        /// Gets current building's discovery data.
        /// Only valid when player is inside a building.
        /// This is set every time player enters a building and is saved/loaded with each save game.
        /// Notes:
        ///  Older save games will not carry this data until player exits and enters building again.
        ///  When consuming this property, try to handle empty BuildingDiscoveryData (buildingKey == 0) if possible.
        /// </summary>
        public PlayerGPS.DiscoveredBuilding BuildingDiscoveryData
        {
            get { return buildingDiscoveryData; }
            set { buildingDiscoveryData = value; }
        }

        /// <summary>
        /// Gets or sets exterior doors of current interior.
        /// Returns empty array if player not inside.
        /// </summary>
        public StaticDoor[] ExteriorDoors
        {
            get { return exteriorDoors.ToArray(); }
            set { SetExteriorDoors(value); }
        }

        /// <summary>
        /// Gets instance of UnderwaterFog controlling fog when below the surface of dungeon water.
        /// </summary>
        public UnderwaterFog UnderwaterFog
        {
            get { return underwaterFog; }
        }

        void Awake()
        {
            dfUnity = DaggerfallUnity.Instance;
            playerGPS = GetComponent<PlayerGPS>();
            world = FindObjectOfType<StreamingWorld>();
            player = GameManager.Instance.PlayerEntityBehaviour;
        }

        void Start()
        {
            // Wire event for when player enters a new location
            PlayerGPS.OnEnterLocationRect += PlayerGPS_OnEnterLocationRect;
            EntityEffectBroker.OnNewMagicRound += EntityEffectBroker_OnNewMagicRound;
            levitateMotor = GetComponent<LevitateMotor>();
            underwaterFog = new UnderwaterFog();
        }

        void Update()
        {            
            // Track which dungeon block player is inside of
            if (dungeon && isPlayerInsideDungeon)
            {
                int playerBlockIndex = dungeon.GetPlayerBlockIndex(transform.position);
                if (playerBlockIndex != lastPlayerDungeonBlockIndex)
                {
                    lastPlayerDungeonBlockIndex = playerBlockIndex;
                    if (playerBlockIndex != -1)
                    {
                        dungeon.GetBlockData(playerBlockIndex, out playerDungeonBlockData);
                        blockWaterLevel = playerDungeonBlockData.WaterLevel;
                        isPlayerInsideDungeonCastle = playerDungeonBlockData.CastleBlock;
                        SpecialAreaCheck();
                    }
                    else
                    {
                        blockWaterLevel = 10000;
                        isPlayerInsideDungeonCastle = false;
                        isPlayerInsideSpecialArea = false;
                    }
                    //Debug.Log(string.Format("Player is now inside block {0}", playerDungeonBlockData.BlockName));
                }
                if (playerBlockIndex != -1)
                {
                    underwaterFog.UpdateFog(blockWaterLevel);
                }
            }

            if (holidayTextPrimed && holidayTextLocation != GameManager.Instance.StreamingWorld.CurrentPlayerLocationObject)
            {
                holidayTextTimer = 0;
                holidayTextPrimed = false;
            }

            // Count down holiday text display
            if (holidayTextTimer > 0)
                holidayTextTimer -= Time.deltaTime;
            if (holidayTextTimer <= 0 && holidayTextPrimed && GameManager.Instance.IsPlayerOnHUD)
            {
                holidayTextPrimed = false;
                ShowHolidayText();
            }

            // Player in sunlight or darkness
            isPlayerInSunlight = DaggerfallUnity.Instance.WorldTime.Now.IsDay && !IsPlayerInside && !GameManager.Instance.PlayerEntity.InPrison;

            // Do not process underwater logic if not playing game
            // This prevents player catching breath during load
            if (!GameManager.Instance.IsPlayingGame())
                return;

            // Underwater swimming logic should only be processed in dungeons at this time
            if (isPlayerInsideDungeon)
            {
                // NOTE: Player's y value in DF unity is 0.95 units off from classic, so subtracting it to get correct comparison
                if (blockWaterLevel == 10000 || (player.transform.position.y + (50 * MeshReader.GlobalScale) - 0.95f) >= (blockWaterLevel * -1 * MeshReader.GlobalScale))
                {
                    isPlayerSwimming = false;
                    levitateMotor.IsSwimming = false;
                }
                else
                {
                    if (!isPlayerSwimming)
                        SendMessage("PlayLargeSplash", SendMessageOptions.DontRequireReceiver);
                    isPlayerSwimming = true;
                    levitateMotor.IsSwimming = true;
                }

                bool overEncumbered = (GameManager.Instance.PlayerEntity.CarriedWeight * 4 > 250);
                if ((overEncumbered && levitateMotor.IsSwimming) && !displayAfloatMessage && !GameManager.Instance.PlayerEntity.IsWaterWalking)
                {
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("cannotFloat"), 1.75f);
                    displayAfloatMessage = true;
                }
                else if ((!overEncumbered || !levitateMotor.IsSwimming) && displayAfloatMessage)
                {
                    displayAfloatMessage = false;
                }

                // Check if player is submerged and needs to start holding breath
                if (blockWaterLevel == 10000 || (player.transform.position.y + (76 * MeshReader.GlobalScale) - 0.95f) >= (blockWaterLevel * -1 * MeshReader.GlobalScale))
                {
                    isPlayerSubmerged = false;
                }
                else
                    isPlayerSubmerged = true;
            }
            else
            {
                // Clear flags when not in a dungeon
                // don't clear swimming if we're outside on a water tile - MeteoricDragon
                if (GameManager.Instance.StreamingWorld.PlayerTileMapIndex != 0)
                    isPlayerSwimming = false;
                isPlayerSubmerged = false;
                levitateMotor.IsSwimming = false;
            }
        }

        #region Public Methods

        /// <summary>
        /// Respawn player at the specified world coordinates, optionally inside dungeon.
        /// </summary>
        public void RespawnPlayer(
            int worldX,
            int worldZ,
            bool insideDungeon = false,
            bool importEnemies = true)
        {
            RespawnPlayer(worldX, worldZ, insideDungeon, false, null, false, importEnemies);
        }

        /// <summary>
        /// Respawn player at the specified world coordinates, optionally inside dungeon or building.
        /// Player can be forced to respawn to closest start marker or origin.
        /// </summary>
        public void RespawnPlayer(
            int worldX,
            int worldZ,
            bool insideDungeon,
            bool insideBuilding,
            StaticDoor[] exteriorDoors = null,
            bool forceReposition = false,
            bool importEnemies = true,
            bool start = true)
        {
            // Mark any existing world data for destruction
            if (dungeon)
            {
                Destroy(dungeon.gameObject);
            }
            if (interior)
            {
                Destroy(interior.gameObject);
            }

            // Deregister all serializable objects
            SaveLoadManager.DeregisterAllSerializableGameObjects();

            // Start respawn process
            isRespawning = true;
            SetExteriorDoors(exteriorDoors);
            StartCoroutine(Respawner(worldX, worldZ, insideDungeon, insideBuilding, forceReposition, importEnemies, start));
        }

        IEnumerator Respawner(int worldX, int worldZ, bool insideDungeon, bool insideBuilding, bool forceReposition, bool importEnemies, bool start = true)
        {
            // Wait for end of frame so existing world data can be removed
            yield return new WaitForEndOfFrame();

            // Store if player was inside a dungeon or building before respawning
            bool playerWasInDungeon = IsPlayerInsideDungeon;
            bool playerWasInBuilding = IsPlayerInsideBuilding;

            // Reset dungeon block on new spawn
            lastPlayerDungeonBlockIndex = -1;
            playerDungeonBlockData = new DFLocation.DungeonBlock();

            // Reset inside state
            isPlayerInside = false;
            isPlayerInsideDungeon = false;
            isPlayerInsideDungeonCastle = false;
            blockWaterLevel = 10000;

            // Set player GPS coordinates
            playerGPS.WorldX = worldX;
            playerGPS.WorldZ = worldZ;

            // Set streaming world coordinates
            DFPosition pos = MapsFile.WorldCoordToMapPixel(worldX, worldZ);
            world.MapPixelX = pos.X;
            world.MapPixelY = pos.Y;

            // Get location at this position
            ContentReader.MapSummary summary;
            bool hasLocation = dfUnity.ContentReader.HasLocation(pos.X, pos.Y, out summary);

            if (!insideDungeon && !insideBuilding)
            {
                // Start outside
                EnableExteriorParent();
                if (!forceReposition)
                {
                    // Teleport to explicit world coordinates
                    world.TeleportToWorldCoordinates(worldX, worldZ);
                }
                else
                {
                    // Force reposition to closest start marker if available
                    world.TeleportToCoordinates(pos.X, pos.Y, StreamingWorld.RepositionMethods.RandomStartMarker);
                }

                // Wait until world is ready
                while (world.IsInit)
                    yield return new WaitForEndOfFrame();

                // Raise transition exterior event if player was inside a dungeon or building
                // This helps inform other systems player has transitioned to exterior without clicking a door or reloading game
                if (playerWasInDungeon)
                    RaiseOnTransitionDungeonExteriorEvent();
                else if (playerWasInBuilding)
                    RaiseOnTransitionExteriorEvent();
            }
            else if (hasLocation && insideDungeon)
            {
                // Start in dungeon
                DFLocation location;
                world.TeleportToCoordinates(pos.X, pos.Y, StreamingWorld.RepositionMethods.None);
                dfUnity.ContentReader.GetLocation(summary.RegionIndex, summary.MapIndex, out location);
                StartDungeonInterior(location, true, importEnemies);
                world.suppressWorld = false;
            }
            else if (hasLocation && insideBuilding && exteriorDoors != null)
            {
                // Start in building
                DFLocation location;
                world.TeleportToCoordinates(pos.X, pos.Y, StreamingWorld.RepositionMethods.None);
                dfUnity.ContentReader.GetLocation(summary.RegionIndex, summary.MapIndex, out location);
                StartBuildingInterior(location, exteriorDoors[0], start);
                world.suppressWorld = false;
            }
            else
            {
                // All else fails teleport to map pixel
                DaggerfallUnity.LogMessage("Something went wrong! Teleporting to origin of nearest map pixel.");
                EnableExteriorParent();
                world.TeleportToCoordinates(pos.X, pos.Y);
            }

            // Lower respawn flag
            isRespawning = false;

            RaiseOnRespawnerCompleteEvent();
        }

        /// <summary>
        /// Shows UI message with text for current holiday, if any.
        /// </summary>
        public void ShowHolidayText()
        {
            const int holidaysStartID = 8349;

            uint minutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            int holidayId = Formulas.FormulaHelper.GetHolidayId(minutes, GameManager.Instance.PlayerGPS.CurrentRegionIndex);

            if (holidayId != 0)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
                messageBox.SetTextTokens(holidaysStartID + holidayId);
                messageBox.ClickAnywhereToClose = true;
                messageBox.ParentPanel.BackgroundColor = Color.clear;
                messageBox.ScreenDimColor = new Color32(0, 0, 0, 0);
                messageBox.Show();
            }

            // Set holiday text timer to a somewhat large value so it doesn't show again and again if the player is repeatedly crossing the
            // border of a city.
            holidayTextTimer = 10f;
        }

        /// <summary>
        /// Helper to reposition player to anywhere in world either at load or during player.
        /// </summary>
        /// <param name="playerPosition">Player position data.</param>
        /// <param name="start">Use true if this is a load/start operation, otherwise false.</param>
        public void RestorePositionHelper(PlayerPositionData_v1 playerPosition, bool start, bool importEnemies)
        {
            // Raise reposition flag if terrain sampler changed
            // This is required as changing terrain samplers will invalidate serialized player coordinates
            // Make an exception for dungeons as exterior world does not matter
            bool repositionPlayer = false;
            if ((playerPosition.terrainSamplerName != DaggerfallUnity.Instance.TerrainSampler.ToString() ||
                playerPosition.terrainSamplerVersion != DaggerfallUnity.Instance.TerrainSampler.Version) &&
                !playerPosition.insideDungeon)
            {
                repositionPlayer = true;
                if (DaggerfallUI.Instance.DaggerfallHUD != null)
                    DaggerfallUI.Instance.DaggerfallHUD.PopupText.AddText("Terrain sampler changed. Repositioning player.");
            }

            // Check exterior doors are included in save, we need these to exit building
            bool hasExteriorDoors;
            if (playerPosition.exteriorDoors == null || playerPosition.exteriorDoors.Length == 0)
                hasExteriorDoors = false;
            else
                hasExteriorDoors = true;

            // Raise reposition flag if player is supposed to start indoors but building has no doors
            if (playerPosition.insideBuilding && !hasExteriorDoors)
            {
                repositionPlayer = true;
                if (DaggerfallUI.Instance.DaggerfallHUD != null)
                    DaggerfallUI.Instance.DaggerfallHUD.PopupText.AddText("Building has no exterior doors. Repositioning player.");
            }

            // Start the respawn process based on saved player location
            if (playerPosition.insideDungeon/* && !repositionPlayer*/) // Do not need to resposition outside for dungeons
            {
                // Start in dungeon
                RespawnPlayer(
                    playerPosition.worldPosX,
                    playerPosition.worldPosZ,
                    true,
                    importEnemies);
            }
            else if (playerPosition.insideBuilding && hasExteriorDoors && !repositionPlayer)
            {
                // Start in building
                RespawnPlayer(
                    playerPosition.worldPosX,
                    playerPosition.worldPosZ,
                    playerPosition.insideDungeon,
                    playerPosition.insideBuilding,
                    playerPosition.exteriorDoors,
                    false,
                    false,
                    start);
            }
            else
            {
                // Start outside
                RespawnPlayer(
                    playerPosition.worldPosX,
                    playerPosition.worldPosZ,
                    false,
                    false,
                    null,
                    repositionPlayer);
            }
        }

        #endregion

        #region Building Transitions

        /// <summary>
        /// Transition player through an exterior door into building interior.
        /// </summary>
        /// <param name="doorOwner">Parent transform owning door array..</param>
        /// <param name="door">Exterior door player clicked on.</param>
        public void TransitionInterior(Transform doorOwner, StaticDoor door, bool doFade = false, bool start = true)
        {
            // Store start flag
            lastInteriorStartFlag = start;

            // Ensure we have component references
            if (!ReferenceComponents())
                return;

            // Copy owner position to door
            // This ensures the door itself is all we need to reposition interior
            // Useful when loading a save and doorOwner is null (as outside world does not exist)
            if (doorOwner)
            {
                door.ownerPosition = doorOwner.position;
                door.ownerRotation = doorOwner.rotation;
            }

            if (!start)
            {
                // Update scene cache from serializable state for exterior->interior transition
                SaveLoadManager.CacheScene(world.SceneName);
                // Explicitly deregister all stateful objects since exterior isn't destroyed
                SaveLoadManager.DeregisterAllSerializableGameObjects(true);
                // Clear all stateful objects from world loose object tracking
                world.ClearStatefulLooseObjects();
            }

            // Ensure building variant checks use this location.
            WorldDataVariants.SetLastLocationKeyTo(playerGPS.CurrentRegionIndex, playerGPS.CurrentLocationIndex);

            // Raise event
            RaiseOnPreTransitionEvent(TransitionType.ToBuildingInterior, door);

            // Ensure expired rooms are removed
            GameManager.Instance.PlayerEntity.RemoveExpiredRentedRooms();

            // Get climate
            ClimateBases climateBase = ClimateBases.Temperate;
            if (OverrideLocation)
                climateBase = OverrideLocation.Summary.Climate;
            else if (playerGPS)
                climateBase = ClimateSwaps.FromAPIClimateBase(playerGPS.ClimateSettings.ClimateType);

            // Layout interior
            // This needs to be done first so we know where the enter markers are
            GameObject newInterior = new GameObject(DaggerfallInterior.GetSceneName(playerGPS.CurrentLocation, door));
            newInterior.hideFlags = defaultHideFlags;
            interior = newInterior.AddComponent<DaggerfallInterior>();

            // Try to layout interior
            // If we fail for any reason, use that old chestnut "this house has nothing of value"
            try
            {
                interior.DoLayout(doorOwner, door, climateBase, buildingDiscoveryData);
            }
            catch (Exception e)
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("thisHouseHasNothingOfValue"));
                Debug.LogException(e);
                Destroy(newInterior);
                RaiseOnFailedTransition(TransitionType.ToBuildingInterior);
                return;
            }

            // Position interior directly inside of exterior
            // This helps with finding closest enter/exit point relative to player position
            interior.transform.position = door.ownerPosition + (Vector3)door.buildingMatrix.GetColumn(3);
            interior.transform.rotation = GameObjectHelper.QuaternionFromMatrix(door.buildingMatrix);

            // Find closest enter marker to exterior door position within building interior
            // If a marker is found, it will be used as the new check position to find actual interior door
            Vector3 closestEnterMarkerPosition;
            Vector3 checkPosition = DaggerfallStaticDoors.GetDoorPosition(door);
            if (interior.FindClosestEnterMarker(checkPosition, out closestEnterMarkerPosition))
                checkPosition = closestEnterMarkerPosition;

            // Position player in front of closest interior door
            Vector3 landingPosition = Vector3.zero;
            Vector3 foundDoorNormal = Vector3.zero;
            if (interior.FindClosestInteriorDoor(checkPosition, out landingPosition, out foundDoorNormal))
            {
                landingPosition += foundDoorNormal * (GameManager.Instance.PlayerController.radius + 0.4f);
            }
            else
            {
                // If no door found position player above closest enter marker
                if (interior.FindClosestEnterMarker(checkPosition, out landingPosition))
                {
                    landingPosition += Vector3.up * (controller.height * 0.6f);
                }
                else
                {
                    // Could not find an door or enter marker, probably not a valid interior
                    Destroy(newInterior);
                    RaiseOnFailedTransition(TransitionType.ToBuildingInterior);
                    return;
                }
            }

            // Enumerate all exterior doors belonging to this building
            DaggerfallStaticDoors exteriorStaticDoors = interior.ExteriorDoors;
            if (exteriorStaticDoors && doorOwner)
            {
                List<StaticDoor> buildingDoors = new List<StaticDoor>();
                for (int i = 0; i < exteriorStaticDoors.Doors.Length; i++)
                {
                    if (exteriorStaticDoors.Doors[i].recordIndex == door.recordIndex)
                    {
                        StaticDoor newDoor = exteriorStaticDoors.Doors[i];
                        newDoor.ownerPosition = doorOwner.position;
                        newDoor.ownerRotation = doorOwner.rotation;
                        buildingDoors.Add(newDoor);
                    }
                }
                SetExteriorDoors(buildingDoors.ToArray());
            }

            // Assign new interior to parent
            if (InteriorParent != null)
                newInterior.transform.parent = InteriorParent.transform;

            // Cache some information about this interior
            buildingType = interior.BuildingData.BuildingType;
            factionID = interior.BuildingData.FactionId;

            // Set player to landng position
            transform.position = landingPosition;
            SetStanding();

            EnableInteriorParent();

            // Add quest resources
            GameObjectHelper.AddQuestResourceObjects(SiteTypes.Building, interior.transform, interior.EntryDoor.buildingKey);

            // Update serializable state from scene cache for exterior->interior transition (unless new/load game)
            if (!start)
                SaveLoadManager.RestoreCachedScene(interior.name);

            // Raise event
            RaiseOnTransitionInteriorEvent(door, interior);

            // Fade in from black
            if (doFade)
                DaggerfallUI.Instance.FadeBehaviour.FadeHUDFromBlack();
        }

        /// <summary>
        /// Transition player through an interior door to building exterior. Player must be inside.
        /// Interior stores information about exterior, no need for extra params.
        /// </summary>
        public void TransitionExterior(bool doFade = false)
        {
            // Exit if missing required components or not currently inside
            if (!ReferenceComponents() || !interior || !isPlayerInside)
                return;

            // Redirect to coroutine verion for fade support
            if (doFade)
            {
                StartCoroutine(FadedTransitionExterior());
                return;
            }

            // Perform transition
            BuildingTransitionExteriorLogic();
        }

        private IEnumerator FadedTransitionExterior()
        {
            // Smash to black
            DaggerfallUI.Instance.FadeBehaviour.SmashHUDToBlack();
            yield return new WaitForEndOfFrame();

            // Perform transition
            BuildingTransitionExteriorLogic();

            // Increase fade time if outside world not ready
            // This indicates a first-time transition on fresh load
            float fadeTime = 0.7f;
            if (!GameManager.Instance.StreamingWorld.IsInit)
                fadeTime = 1.5f;

            // Fade in from black
            DaggerfallUI.Instance.FadeBehaviour.FadeHUDFromBlack(fadeTime);
        }

        private void BuildingTransitionExteriorLogic()
        {
            // Raise event
            RaiseOnPreTransitionEvent(TransitionType.ToBuildingExterior);

            // Update scene cache from serializable state for interior->exterior transition
            SaveLoadManager.CacheScene(interior.name);

            // Find closest door and position player outside of it
            StaticDoor closestDoor;
            Vector3 closestDoorPos = DaggerfallStaticDoors.FindClosestDoor(transform.position, ExteriorDoors, out closestDoor);
            Vector3 normal = DaggerfallStaticDoors.GetDoorNormal(closestDoor);
            Vector3 position = closestDoorPos + normal * (controller.radius * 3f);
            world.SetAutoReposition(StreamingWorld.RepositionMethods.Offset, position);

            EnableExteriorParent();

            // Player is now outside building
            isPlayerInside = false;
            isPlayerInsideOpenShop = false;
            IsPlayerInsideTavern = false;
            PlayerTeleportedIntoDungeon = false;
            buildingType = DFLocation.BuildingTypes.None;
            factionID = 0;

            // Update serializable state from scene cache for interior->exterior transition
            SaveLoadManager.RestoreCachedScene(world.SceneName);

            // Fire event
            RaiseOnTransitionExteriorEvent();
        }

        #endregion

        #region Dungeon Transitions

        /// <summary>
        /// Transition player through a dungeon entrance door into dungeon interior.
        /// </summary>
        /// <param name="doorOwner">Parent transform owning door array.</param>
        /// <param name="door">Exterior door player clicked on.</param>
        public void TransitionDungeonInterior(Transform doorOwner, StaticDoor door, DFLocation location, bool doFade = false)
        {
            // Ensure we have component references
            if (!ReferenceComponents())
                return;

            // Reset dungeon block on entering dungeon
            lastPlayerDungeonBlockIndex = -1;
            playerDungeonBlockData = new DFLocation.DungeonBlock();

            // Override location if specified
            if (OverrideLocation != null)
            {
                DFLocation overrideLocation = dfUnity.ContentReader.MapFileReader.GetLocation(OverrideLocation.Summary.RegionName, OverrideLocation.Summary.LocationName);
                if (overrideLocation.Loaded)
                    location = overrideLocation;
            }

            // Raise event
            RaiseOnPreTransitionEvent(TransitionType.ToDungeonInterior, door);

            // Layout dungeon
            GameObject newDungeon;
            dungeon = GameObjectHelper.CreateDaggerfallDungeonGameObject(location, DungeonParent.transform, out newDungeon);
            dungeon.SetDungeon(location);
            newDungeon.hideFlags = defaultHideFlags;

            // Find start marker to position player
            if (!dungeon.StartMarker)
            {
                // Could not find a start marker
                Destroy(newDungeon);
                RaiseOnFailedTransition(TransitionType.ToDungeonInterior);
                return;
            }

            EnableDungeonParent();

            // Set to start position
            MovePlayerToMarker(dungeon.StartMarker);

            // Find closest dungeon exit door to orient player
            StaticDoor[] doors = DaggerfallStaticDoors.FindDoorsInCollections(dungeon.StaticDoorCollections, DoorTypes.DungeonExit);
            if (doors != null && doors.Length > 0)
            {
                Vector3 doorPos;
                int doorIndex;
                if (DaggerfallStaticDoors.FindClosestDoorToPlayer(transform.position, doors, out doorPos, out doorIndex))
                {
                    // Set player facing away from door
                    PlayerMouseLook playerMouseLook = GameManager.Instance.PlayerMouseLook;
                    if (playerMouseLook)
                    {
                        Vector3 normal = DaggerfallStaticDoors.GetDoorNormal(doors[doorIndex]);
                        playerMouseLook.SetFacing(normal);
                    }
                }
            }

            // Add quest resources
            GameObjectHelper.AddQuestResourceObjects(SiteTypes.Dungeon, dungeon.transform);

            // Raise event
            RaiseOnTransitionDungeonInteriorEvent(door, dungeon);

            // Fade in from black
            if (doFade)
                DaggerfallUI.Instance.FadeBehaviour.FadeHUDFromBlack();
        }

        /// <summary>
        /// Starts player inside dungeon with no exterior world.
        /// </summary>
        public void StartDungeonInterior(DFLocation location, bool preferEnterMarker = true, bool importEnemies = true)
        {
            // Ensure we have component references
            if (!ReferenceComponents())
                return;

            // Raise event
            RaiseOnPreTransitionEvent(TransitionType.ToDungeonInterior);

            // Layout dungeon
            GameObject newDungeon;
            dungeon = GameObjectHelper.CreateDaggerfallDungeonGameObject(location, DungeonParent.transform, out newDungeon);
            dungeon.SetDungeon(location, importEnemies);
            newDungeon.hideFlags = defaultHideFlags;

            GameObject marker = null;
            if (preferEnterMarker && dungeon.EnterMarker != null)
                marker = dungeon.EnterMarker;
            else
                marker = dungeon.StartMarker;

            // Find start marker to position player
            if (!marker)
            {
                // Could not find marker
                DaggerfallUnity.LogMessage("No start or enter marker found for this dungeon. Aborting load.");
                Destroy(newDungeon);
                RaiseOnFailedTransition(TransitionType.ToDungeonInterior);
                return;
            }

            EnableDungeonParent();

            // Add quest resources and selectively enable quest foes
            //  -Entering a dungeon normally will add quest foes always
            //  -Loading a game will not add quest foes as these are restored by save state
            //  -Teleporting into a dungeon will add quest foes like going through entrance normally
            GameObjectHelper.AddQuestResourceObjects(SiteTypes.Dungeon, dungeon.transform, 0, true, importEnemies, true);

            // Set to start position
            MovePlayerToMarker(marker);

            // Set player facing north
            PlayerMouseLook playerMouseLook = GameManager.Instance.PlayerMouseLook;
            if (playerMouseLook)
                playerMouseLook.SetFacing(Vector3.forward);

            // Raise event
            RaiseOnTransitionDungeonInteriorEvent(new StaticDoor(), dungeon);
        }

        /// <summary>
        /// Starts player inside building with no exterior world.
        /// </summary>
        public void StartBuildingInterior(DFLocation location, StaticDoor exteriorDoor, bool start = true)
        {
            // Store start flag
            lastInteriorStartFlag = start;

            // Ensure we have component references
            if (!ReferenceComponents())
                return;

            // Discover building
            GameManager.Instance.PlayerGPS.DiscoverBuilding(exteriorDoor.buildingKey);

            TransitionInterior(null, exteriorDoor, false, start);
        }

        public void DisableAllParents(bool cleanup = true)
        {
            if (!GameManager.Instance.IsReady)
                GameManager.Instance.GetProperties();

            if (cleanup)
            {
                if (dungeon) Destroy(dungeon.gameObject);
                if (interior) Destroy(interior.gameObject);
            }

            if (ExteriorParent != null) ExteriorParent.SetActive(false);
            if (InteriorParent != null) InteriorParent.SetActive(false);
            if (DungeonParent != null) DungeonParent.SetActive(false);
        }

        /// <summary>
        /// Enable ExteriorParent.
        /// </summary>
        public void EnableExteriorParent(bool cleanup = true)
        {
            if (cleanup)
            {
                if (dungeon) Destroy(dungeon.gameObject);
                if (interior) Destroy(interior.gameObject);
                SetExteriorDoors(null);
            }
            DisableAllParents(false);
            if (ExteriorParent != null) ExteriorParent.SetActive(true);

            world.suppressWorld = false;
            isPlayerInside = false;
            isPlayerInsideDungeon = false;

            GameManager.UpdateShadowDistance();
        }

        /// <summary>
        /// Enable InteriorParent.
        /// </summary>
        public void EnableInteriorParent(bool cleanup = true)
        {
            if (cleanup)
            {
                if (dungeon) Destroy(dungeon.gameObject);
            }
            DisableAllParents(false);
            if (InteriorParent != null) InteriorParent.SetActive(true);

            isPlayerInside = true;
            isPlayerInsideDungeon = false;

            GameManager.UpdateShadowDistance();
        }

        /// <summary>
        /// Enable DungeonParent.
        /// </summary>
        public void EnableDungeonParent(bool cleanup = true)
        {
            if (cleanup)
            {
                if (interior)
                {
                    Destroy(interior.gameObject);
                    buildingType = DFLocation.BuildingTypes.None;
                    factionID = 0;
                }
            }

            DisableAllParents(false);
            if (DungeonParent != null) DungeonParent.SetActive(true);

            isPlayerInside = true;
            isPlayerInsideOpenShop = false;
            IsPlayerInsideTavern = false;
            isPlayerInsideDungeon = true;

            GameManager.UpdateShadowDistance();
        }

        /// <summary>
        /// Moves player to a start marker inside current dungeon.
        /// </summary>
        /// <param name="marker">Marker gameobject. See <see cref="DaggerfallRDBBlock.StartMarkers"/>.</param>
        public void MovePlayerToMarker(GameObject marker)
        {
            if (!isPlayerInsideDungeon || !marker)
                return;

            // Set player to start position
            transform.position = marker.transform.position + Vector3.up * (controller.height * 0.6f);

            // Fix player standing
            SetStanding();

            // Raise event
            RaiseOnMovePlayerToDungeonStartEvent();
        }

        /// <summary>
        /// Moves player to main start marker inside current dungeon.
        /// </summary>
        public void MovePlayerToDungeonStart()
        {
            MovePlayerToMarker(dungeon.StartMarker);
        }

        /// <summary>
        /// Player is leaving dungeon, transition them back outside.
        /// </summary>
        /// <param name="doFade">Fade HUD after transition if true.</param>
        public void TransitionDungeonExterior(bool doFade = false)
        {
            if (!ReferenceComponents() || !dungeon || !isPlayerInsideDungeon)
                return;

            // Redirect to coroutine verion for fade support
            if (doFade)
            {
                StartCoroutine(FadedTransitionDungeonExterior());
                return;
            }

            // Perform transition
            DungeonTransitionExteriorLogic();
        }

        private IEnumerator FadedTransitionDungeonExterior()
        {
            // Smash to black
            DaggerfallUI.Instance.FadeBehaviour.SmashHUDToBlack();
            yield return new WaitForEndOfFrame();

            // Perform transition
            DungeonTransitionExteriorLogic();

            // Increase fade time if outside world not ready
            // This indicates a first-time transition on fresh load
            float fadeTime = 0.7f;
            if (!GameManager.Instance.StreamingWorld.IsInit)
                fadeTime = 1.5f;

            // Fade in from black
            DaggerfallUI.Instance.FadeBehaviour.FadeHUDFromBlack(fadeTime);
        }

        private void DungeonTransitionExteriorLogic()
        {
            // Raise event
            RaiseOnPreTransitionEvent(TransitionType.ToDungeonExterior);

            EnableExteriorParent();

            // Player is now outside dungeon
            isPlayerInside = false;
            isPlayerInsideDungeon = false;
            isPlayerInsideDungeonCastle = false;
            lastPlayerDungeonBlockIndex = -1;
            playerDungeonBlockData = new DFLocation.DungeonBlock();
            PlayerTeleportedIntoDungeon = false;

            // Position player to door
            world.SetAutoReposition(StreamingWorld.RepositionMethods.DungeonEntrance, Vector3.zero);

            // Raise event
            RaiseOnTransitionDungeonExteriorEvent();
        }

        /// <summary>
        /// Prepares for leaving dungeon, but do not perform transition logic. Reposition process is up to the caller.
        /// </summary>
        public void TransitionDungeonExteriorImmediate()
        {
            if (!ReferenceComponents() || !dungeon || !isPlayerInsideDungeon)
                return;

            RaiseOnPreTransitionEvent(PlayerEnterExit.TransitionType.ToDungeonExterior);
        }

        #endregion

        #region Private Methods

        private void SpecialAreaCheck()
        {
            if (!isPlayerInsideDungeon)
            {
                isPlayerInsideSpecialArea = false;
                return;
            }

            switch (playerDungeonBlockData.BlockName)
            {
                case "S0000161.RDB":    // Daggerfall treasure room
                    isPlayerInsideSpecialArea = true;
                    break;
                default:
                    isPlayerInsideSpecialArea = false;
                    break;
            }
        }

        private void SetStanding()
        {
            // Snap player to ground
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out hit, PlayerHeightChanger.controllerStandingHeight * 2f))
            {
                // Clear falling damage so player doesn't take damage if they transitioned into a dungeon while jumping
                GameManager.Instance.AcrobatMotor.ClearFallingDamage();
                // Position player at hit position plus just over half controller height up
                Vector3 pos = hit.point;
                pos.y += controller.height / 2f + 0.25f;
                transform.position = pos;
            }
        }

        private bool ReferenceComponents()
        {
            // Look for required components
            if (controller == null)
                controller = GetComponent<CharacterController>();
            
            // Fail if missing required components
            if (dfUnity == null || controller == null)
                return false;

            return true;
        }

        private void SetExteriorDoors(StaticDoor[] doors)
        {
            exteriorDoors.Clear();
            if (doors != null && doors.Length > 0)
                exteriorDoors.AddRange(doors);
        }

        private WorldContext GetWorldContext()
        {
            if (!IsPlayerInside)
                return WorldContext.Exterior;
            else if (IsPlayerInsideBuilding)
                return WorldContext.Interior;
            else if (isPlayerInsideDungeon)
                return WorldContext.Dungeon;
            else
                return WorldContext.Nothing;
        }

        #endregion

        #region Event Arguments

        /// <summary>
        /// Types of transition encountered by event system.
        /// </summary>
        public enum TransitionType
        {
            NotDefined,
            ToBuildingInterior,
            ToBuildingExterior,
            ToDungeonInterior,
            ToDungeonExterior,
        }

        /// <summary>
        /// Arguments for PlayerEnterExit events.
        /// All interior/exterior/dungeon transitions use these arguments.
        /// Valid members will depend on which transition event was fired.
        /// </summary>
        public class TransitionEventArgs : System.EventArgs
        {
            /// <summary>The type of transition.</summary>
            public TransitionType TransitionType { get; set; }

            /// <summary>The exterior StaticDoor clicked to initiate transition. For exterior to interior transitions only.</summary>
            public StaticDoor StaticDoor { get; set; }

            /// <summary>The newly instanced building interior. For building interior transitions only.</summary>
            public DaggerfallInterior DaggerfallInterior { get; set; }

            /// <summary>The newly instanced dungeon interior. For dungeon interior transitions only.</summary>
            public DaggerfallDungeon DaggerfallDungeon { get; set; }

            /// <summary>Constructor.</summary>
            public TransitionEventArgs()
            {
                TransitionType = PlayerEnterExit.TransitionType.NotDefined;
                StaticDoor = new StaticDoor();
                DaggerfallInterior = null;
                DaggerfallDungeon = null;
            }

            /// <summary>Constructor helper.</summary>
            public TransitionEventArgs(TransitionType transitionType)
                : base()
            {
                this.TransitionType = transitionType;
            }

            /// <summary>Constructor helper.</summary>
            public TransitionEventArgs(TransitionType transitionType, StaticDoor staticDoor, DaggerfallInterior daggerfallInterior = null, DaggerfallDungeon daggerfallDungeon = null)
                : base()
            {
                this.TransitionType = transitionType;
                this.StaticDoor = staticDoor;
                this.DaggerfallInterior = daggerfallInterior;
                this.DaggerfallDungeon = daggerfallDungeon;
            }
        }

        #endregion

        #region Event Handlers

        // Notify player when they enter location rect
        // For exterior towns, print out "You are entering %s".
        // For exterior dungeons, print out flavour text.
        private void PlayerGPS_OnEnterLocationRect(DFLocation location)
        {
            const int set1StartID = 500;
            const int set2StartID = 520;

            if (playerGPS && !isPlayerInside)
            {
                if (location.MapTableData.LocationType == DFRegion.LocationTypes.DungeonLabyrinth ||
                    location.MapTableData.LocationType == DFRegion.LocationTypes.DungeonKeep ||
                    location.MapTableData.LocationType == DFRegion.LocationTypes.DungeonRuin ||
                    location.MapTableData.LocationType == DFRegion.LocationTypes.Graveyard)
                {
                    // Get text ID based on set start and dungeon type index
                    int dungeonTypeIndex = (int)location.MapTableData.DungeonType;
                    int set1ID = set1StartID + dungeonTypeIndex;
                    int set2ID = set2StartID + dungeonTypeIndex;

                    // Select two sets of flavour text based on dungeon type
                    string flavourText1 = DaggerfallUnity.Instance.TextProvider.GetRandomText(set1ID);
                    string flavourText2 = DaggerfallUnity.Instance.TextProvider.GetRandomText(set2ID);

                    // Show flavour text a bit longer than in classic
                    DaggerfallUI.AddHUDText(flavourText1, 3);
                    DaggerfallUI.AddHUDText(flavourText2, 3);
                }
                else if (location.MapTableData.LocationType != DFRegion.LocationTypes.Coven &&
                    location.MapTableData.LocationType != DFRegion.LocationTypes.HomeYourShips)
                {
                    // Show "You are entering %s"
                    string youAreEntering = TextManager.Instance.GetLocalizedText("youAreEntering");
                    youAreEntering = youAreEntering.Replace("%s", TextManager.Instance.GetLocalizedLocationName(location.MapTableData.MapId, location.Name));
                    DaggerfallUI.AddHUDText(youAreEntering, 2);

                    // Check room rentals in this location, and display how long any rooms are rented for
                    int mapId = playerGPS.CurrentLocation.MapTableData.MapId;
                    PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                    playerEntity.RemoveExpiredRentedRooms();
                    List<RoomRental_v1> rooms = playerEntity.GetRentedRooms(mapId);
                    if (rooms.Count > 0)
                    {
                        foreach (RoomRental_v1 room in rooms)
                        {
                            string remainingHours = PlayerEntity.GetRemainingHours(room).ToString();
                            DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("youHaveRentedRoom").Replace("%s", room.name).Replace("%d", remainingHours), 6);
                        }
                    }

                    if (holidayTextTimer <= 0 && !holidayTextPrimed)
                    {
                        holidayTextTimer = 2.5f; // Short delay to give save game fade-in time to finish
                        holidayTextPrimed = true;
                    }
                    holidayTextLocation = GameManager.Instance.StreamingWorld.CurrentPlayerLocationObject;

                    // note Nystul: this next line is not enough to manage questor dictionary update since player might load a savegame in an interior -
                    // so this never gets triggered and questor list is rebuild always as a consequence
                    // a better thing is if talkmanager handles all this by itself without making changes to PlayerEnterExit necessary and use events/delegates
                    // -> so I will outcomment next line but leave it in so that original author stumbles across this comment
                    // fixed this in TalkManager class
                    // TalkManager.Instance.LastExteriorEntered = location.LocationIndex;
                }
            }
        }

        private void EntityEffectBroker_OnNewMagicRound()
        {
            // Player in holy place
            isPlayerInHolyPlace = false;
            if (WorldContext == WorldContext.Interior && interior != null)
            {
                if (interior.BuildingData.BuildingType == DFLocation.BuildingTypes.Temple ||
                    interior.BuildingData.FactionId == (int)FactionFile.FactionIDs.Fighter_Trainers)
                    isPlayerInHolyPlace = true;
            }
        }

        #endregion

        #region Events

        // OnPreTransition - Called PRIOR to any transition, other events called AFTER transition.
        public delegate void OnPreTransitionEventHandler(TransitionEventArgs args);
        /// <summary>
        /// Unlike other events in this class, this one is raised before the transition has been performed.
        /// It's always followed by <see cref="OnFailedTransition"/> or one of the other events for success.
        /// </summary>
        public static event OnPreTransitionEventHandler OnPreTransition;
        protected virtual void RaiseOnPreTransitionEvent(TransitionType transitionType)
        {
            TransitionEventArgs args = new TransitionEventArgs(transitionType);
            if (OnPreTransition != null)
                OnPreTransition(args);
        }
        protected virtual void RaiseOnPreTransitionEvent(TransitionType transitionType, StaticDoor staticDoor)
        {
            TransitionEventArgs args = new TransitionEventArgs(transitionType, staticDoor);
            if (OnPreTransition != null)
                OnPreTransition(args);
        }

        // OnTransitionInterior
        public delegate void OnTransitionInteriorEventHandler(TransitionEventArgs args);
        public static event OnTransitionInteriorEventHandler OnTransitionInterior;
        protected virtual void RaiseOnTransitionInteriorEvent(StaticDoor staticDoor, DaggerfallInterior daggerfallInterior)
        {
            TransitionEventArgs args = new TransitionEventArgs(TransitionType.ToBuildingInterior, staticDoor, daggerfallInterior);
            if (OnTransitionInterior != null)
                OnTransitionInterior(args);
        }

        // OnTransitionExterior
        public delegate void OnTransitionExteriorEventHandler(TransitionEventArgs args);
        public static event OnTransitionExteriorEventHandler OnTransitionExterior;
        protected virtual void RaiseOnTransitionExteriorEvent()
        {
            TransitionEventArgs args = new TransitionEventArgs(TransitionType.ToBuildingExterior);
            if (OnTransitionExterior != null)
                OnTransitionExterior(args);
        }

        // OnTransitionDungeonInterior
        public delegate void OnTransitionDungeonInteriorEventHandler(TransitionEventArgs args);
        public static event OnTransitionDungeonInteriorEventHandler OnTransitionDungeonInterior;
        protected virtual void RaiseOnTransitionDungeonInteriorEvent(StaticDoor staticDoor, DaggerfallDungeon daggerfallDungeon)
        {
            TransitionEventArgs args = new TransitionEventArgs(TransitionType.ToDungeonInterior, staticDoor, null, daggerfallDungeon);
            if (OnTransitionDungeonInterior != null)
                OnTransitionDungeonInterior(args);
        }

        // OnTransitionDungeonExterior
        public delegate void OnTransitionDungeonExteriorEventHandler(TransitionEventArgs args);
        public static event OnTransitionDungeonExteriorEventHandler OnTransitionDungeonExterior;
        protected virtual void RaiseOnTransitionDungeonExteriorEvent()
        {
            TransitionEventArgs args = new TransitionEventArgs(TransitionType.ToDungeonExterior);
            if (OnTransitionDungeonExterior != null)
                OnTransitionDungeonExterior(args);
        }

        /// <summary>
        /// This event is raised when a transition has started being performed and <see cref="OnPreTransition"/>
        /// was fired but it couldn't be finished correctly due to an unexpected issue (i.e when 
        /// <c>"thisHouseHasNothingOfValue"</c> is also shown).
        /// </summary>
        public static event Action<TransitionEventArgs> OnFailedTransition;
        protected virtual void RaiseOnFailedTransition(TransitionType transitionType)
        {
            if (OnFailedTransition != null)
                OnFailedTransition(new TransitionEventArgs(transitionType));
        }

        // OnMovePlayerToDungeonStart
        public delegate void OnMovePlayerToDungeonStartEventHandler();
        public static event OnMovePlayerToDungeonStartEventHandler OnMovePlayerToDungeonStart;
        protected virtual void RaiseOnMovePlayerToDungeonStartEvent()
        {
            if (OnMovePlayerToDungeonStart != null)
                OnMovePlayerToDungeonStart();
        }

        // OnRespawnerComplete
        public delegate void OnRespawnerCompleteEventHandler();
        public static event OnRespawnerCompleteEventHandler OnRespawnerComplete;
        protected virtual void RaiseOnRespawnerCompleteEvent()
        {
            if (OnRespawnerComplete != null)
                OnRespawnerComplete();
        }

        #endregion
    }
}
