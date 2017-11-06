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
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Assist player controller to enter and exit building interiors and dungeons.
    /// Should be attached to player object with PlayerGPS for climate tracking.
    /// </summary>
    public class PlayerEnterExit : MonoBehaviour
    {
        const HideFlags defaultHideFlags = HideFlags.None;

        DaggerfallUnity dfUnity;
        CharacterController controller;
        bool isPlayerInside = false;
        bool isPlayerInsideDungeon = false;
        bool isPlayerInsideDungeonCastle = false;
        bool isPlayerInsideSpecialArea = false;
        bool isRespawning = false;
        DaggerfallInterior interior;
        DaggerfallDungeon dungeon;
        StreamingWorld world;
        PlayerGPS playerGPS;

        List<StaticDoor> exteriorDoors = new List<StaticDoor>();

        public GameObject ExteriorParent;
        public GameObject InteriorParent;
        public GameObject DungeonParent;
        public DaggerfallLocation OverrideLocation;

        int lastPlayerDungeonBlockIndex = -1;
        DFLocation.DungeonBlock playerDungeonBlockData = new DFLocation.DungeonBlock();
        public short blockWaterLevel = 10000;

        DFLocation.BuildingTypes buildingType;
        ushort factionID = 0;
        PlayerGPS.DiscoveredBuilding buildingDiscoveryData;

        DFLocation holidayTextLocation;
        bool holidayTextPrimed = false;
        float holidayTextTimer = 0f;

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
        /// True when a player respawn is in progress.
        /// e.g. After loading a game or teleporting back to a marked location.
        /// </summary>
        public bool IsRespawning
        {
            get { return isRespawning; }
        }

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
            internal set { buildingDiscoveryData = value; }
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

        void Awake()
        {
            dfUnity = DaggerfallUnity.Instance;
            playerGPS = GetComponent<PlayerGPS>();
            world = FindObjectOfType<StreamingWorld>();
        }

        void Start()
        {
            // Wire event for when player enters a new location
            PlayerGPS.OnEnterLocationRect += PlayerGPS_OnEnterLocationRect;
        }

        void Update()
        {
            // Track which dungeon block player is inside of
            if (dungeon && isPlayerInsideDungeon)
            {
                int playerBlockIndex = dungeon.GetPlayerBlockIndex(transform.position);
                if (playerBlockIndex != lastPlayerDungeonBlockIndex)
                {
                    dungeon.GetBlockData(playerBlockIndex, out playerDungeonBlockData);
                    lastPlayerDungeonBlockIndex = playerBlockIndex;
                    blockWaterLevel = playerDungeonBlockData.WaterLevel;
                    isPlayerInsideDungeonCastle = playerDungeonBlockData.CastleBlock;
                    SpecialAreaCheck();
                    //Debug.Log(string.Format("Player is now inside block {0}", playerDungeonBlockData.BlockName));
                }
            }

            // Count down holiday text display
            if (holidayTextTimer > 0)
                holidayTextTimer -= Time.deltaTime;
            if (holidayTextTimer <= 0 && holidayTextPrimed)
            {
                holidayTextPrimed = false;
                ShowHolidayText();
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
            bool importEnemies = true)
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
            StartCoroutine(Respawner(worldX, worldZ, insideDungeon, insideBuilding, forceReposition, importEnemies));
        }

        IEnumerator Respawner(int worldX, int worldZ, bool insideDungeon, bool insideBuilding, bool forceReposition, bool importEnemies)
        {
            // Wait for end of frame so existing world data can be removed
            yield return new WaitForEndOfFrame();

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
            }
            else if (hasLocation && insideDungeon)
            {
                // Start in dungeon
                DFLocation location;
                world.TeleportToCoordinates(pos.X, pos.Y, StreamingWorld.RepositionMethods.None);
                dfUnity.ContentReader.GetLocation(summary.RegionIndex, summary.MapIndex, out location);
                StartDungeonInterior(location, true, importEnemies);
                world.suppressWorld = true;
            }
            else if (hasLocation && insideBuilding && exteriorDoors != null)
            {
                // Start in building
                DFLocation location;
                world.TeleportToCoordinates(pos.X, pos.Y, StreamingWorld.RepositionMethods.None);
                dfUnity.ContentReader.GetLocation(summary.RegionIndex, summary.MapIndex, out location);
                StartBuildingInterior(location, exteriorDoors[0]);
                world.suppressWorld = true;
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
        }

        public void ShowHolidayText()
        {
            const int holidaysStartID = 8349;

            uint minutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            int holidayId = Formulas.FormulaHelper.GetHolidayId(minutes, holidayTextLocation.RegionIndex);
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

        #endregion

        #region Building Transitions

        /// <summary>
        /// Transition player through an exterior door into building interior.
        /// </summary>
        /// <param name="doorOwner">Parent transform owning door array..</param>
        /// <param name="door">Exterior door player clicked on.</param>
        public void TransitionInterior(Transform doorOwner, StaticDoor door, bool doFade = false)
        {
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

            // Raise event
            RaiseOnPreTransitionEvent(TransitionType.ToBuildingInterior, door);

            // Get climate
            ClimateBases climateBase = ClimateBases.Temperate;
            if (OverrideLocation)
                climateBase = OverrideLocation.Summary.Climate;
            else if (playerGPS)
                climateBase = ClimateSwaps.FromAPIClimateBase(playerGPS.ClimateSettings.ClimateType);

            // Layout interior
            // This needs to be done first so we know where the enter markers are
            GameObject newInterior = new GameObject(string.Format("DaggerfallInterior [Block={0}, Record={1}]", door.blockIndex, door.recordIndex));
            newInterior.hideFlags = defaultHideFlags;
            interior = newInterior.AddComponent<DaggerfallInterior>();

            // Try to layout interior
            // If we fail for any reason, use that old chestnut "this house has nothing of value"
            try
            {
                interior.DoLayout(doorOwner, door, climateBase);
            }
            catch
            {
                DaggerfallUI.AddHUDText(HardStrings.thisHouseHasNothingOfValue);
                Destroy(newInterior);
                return;
            }

            // Position interior directly inside of exterior
            // This helps with finding closest enter/exit point relative to player position
            interior.transform.position = door.ownerPosition + (Vector3)door.buildingMatrix.GetColumn(3);
            interior.transform.rotation = GameObjectHelper.QuaternionFromMatrix(door.buildingMatrix);

            // Position player above closest enter marker
            Vector3 marker;
            if (!interior.FindClosestEnterMarker(transform.position, out marker))
            {
                // Could not find an enter marker, probably not a valid interior
                Destroy(newInterior);
                return;
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

            // Set player to marker position
            // TODO: Find closest door for player facing
            transform.position = marker + Vector3.up * (controller.height * 0.6f);
            SetStanding();

            EnableInteriorParent();

            // Add quest resources
            GameObjectHelper.AddQuestResourceObjects(SiteTypes.Building, interior.transform, interior.EntryDoor.buildingKey);

            // Raise event
            RaiseOnTransitionInteriorEvent(door, interior);

            // Fade in from black
            if (doFade)
                DaggerfallUI.Instance.FadeHUDFromBlack();
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

            // Raise event
            RaiseOnPreTransitionEvent(TransitionType.ToBuildingExterior);

            // Find closest door and position player outside of it
            StaticDoor closestDoor;
            Vector3 closestDoorPos = DaggerfallStaticDoors.FindClosestDoor(transform.position, ExteriorDoors, out closestDoor);
            Vector3 normal = DaggerfallStaticDoors.GetDoorNormal(closestDoor);
            Vector3 position = closestDoorPos + normal * (controller.radius * 3f);
            world.SetAutoReposition(StreamingWorld.RepositionMethods.Offset, position);

            EnableExteriorParent();

            // Player is now outside building
            isPlayerInside = false;

            // Fire event
            RaiseOnTransitionExteriorEvent();

            // Fade in from black
            if (doFade)
                DaggerfallUI.Instance.FadeHUDFromBlack();
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
            GameObject newDungeon = GameObjectHelper.CreateDaggerfallDungeonGameObject(location, DungeonParent.transform);
            newDungeon.hideFlags = defaultHideFlags;
            dungeon = newDungeon.GetComponent<DaggerfallDungeon>();

            // Find start marker to position player
            if (!dungeon.StartMarker)
            {
                // Could not find a start marker
                Destroy(newDungeon);
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
                DaggerfallUI.Instance.FadeHUDFromBlack();
        }

        /// <summary>
        /// Starts player inside dungeon with no exterior world.
        /// </summary>
        public void StartDungeonInterior(DFLocation location, bool preferEnterMarker = true, bool importEnemies = true)
        {
            // Ensure we have component references
            if (!ReferenceComponents())
                return;

            // Layout dungeon
            GameObject newDungeon = GameObjectHelper.CreateDaggerfallDungeonGameObject(location, DungeonParent.transform, importEnemies);
            newDungeon.hideFlags = defaultHideFlags;
            dungeon = newDungeon.GetComponent<DaggerfallDungeon>();

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
                return;
            }

            EnableDungeonParent();

            // Add quest resources - except foes, these are loaded from enemy save data
            GameObjectHelper.AddQuestResourceObjects(SiteTypes.Dungeon, dungeon.transform, 0, true, false, true);

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
        public void StartBuildingInterior(DFLocation location, StaticDoor exteriorDoor)
        {
            // Ensure we have component references
            if (!ReferenceComponents())
                return;

            // Discover building
            GameManager.Instance.PlayerGPS.DiscoverBuilding(exteriorDoor.buildingKey);

            TransitionInterior(null, exteriorDoor);
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
        }

        /// <summary>
        /// Enable DungeonParent.
        /// </summary>
        public void EnableDungeonParent(bool cleanup = true)
        {
            if (cleanup)
            {
                if (interior) Destroy(interior.gameObject);
            }
            DisableAllParents(false);
            if (DungeonParent != null) DungeonParent.SetActive(true);

            isPlayerInside = true;
            isPlayerInsideDungeon = true;
        }

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

        public void MovePlayerToDungeonStart()
        {
            MovePlayerToMarker(dungeon.StartMarker);
        }

        /// <summary>
        /// Player is leaving dungeon, transition them back outside.
        /// </summary>
        public void TransitionDungeonExterior(bool doFade = false)
        {
            if (!ReferenceComponents() || !dungeon || !isPlayerInsideDungeon)
                return;

            // Raise event
            RaiseOnPreTransitionEvent(TransitionType.ToDungeonExterior);

            EnableExteriorParent();

            // Player is now outside dungeon
            isPlayerInside = false;
            isPlayerInsideDungeon = false;
            isPlayerInsideDungeonCastle = false;
            lastPlayerDungeonBlockIndex = -1;
            playerDungeonBlockData = new DFLocation.DungeonBlock();

            // Position player to door
            world.SetAutoReposition(StreamingWorld.RepositionMethods.DungeonEntrance, Vector3.zero);

            // Raise event
            RaiseOnTransitionDungeonExteriorEvent();

            // Fade in from black
            if (doFade)
                DaggerfallUI.Instance.FadeHUDFromBlack();
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
            if (Physics.Raycast(ray, out hit, controller.height * 2f))
            {
                // Cancel movement so player doesn't take falling damage if they transitioned into a dungeon while jumping
                GameManager.Instance.PlayerMotor.CancelMovement = true;
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
                    string youAreEntering = HardStrings.youAreEntering;
                    youAreEntering = youAreEntering.Replace("%s", location.Name);
                    DaggerfallUI.AddHUDText(youAreEntering, 2);

                    if (holidayTextTimer <= 0 && !holidayTextPrimed)
                    {
                        holidayTextTimer = 2.5f; // Short delay to give save game fade-in time to finish
                        holidayTextPrimed = true;
                    }
                    holidayTextLocation = location;
                }
            }
        }

        #endregion

        #region Events

        // OnPreTransition - Called PRIOR to any transition, other events called AFTER transition.
        public delegate void OnPreTransitionEventHandler(TransitionEventArgs args);
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

        // OnMovePlayerToDungeonStart
        public delegate void OnMovePlayerToDungeonStartEventHandler();
        public static event OnMovePlayerToDungeonStartEventHandler OnMovePlayerToDungeonStart;
        protected virtual void RaiseOnMovePlayerToDungeonStartEvent()
        {
            if (OnMovePlayerToDungeonStart != null)
                OnMovePlayerToDungeonStart();
        }

        #endregion
    }
}