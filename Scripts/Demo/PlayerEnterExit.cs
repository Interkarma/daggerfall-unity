// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Assist player controller to enter and exit building interiors and dungeons.
    /// Should be attached to player object with PlayerGPS for climate tracking.
    /// </summary>
    public class PlayerEnterExit : MonoBehaviour
    {
        DaggerfallUnity dfUnity;
        CharacterController controller;
        bool isPlayerInside = false;
        bool isPlayerInsideDungeon = false;
        bool isPlayerInsideDungeonPalace = false;
        DaggerfallInterior interior;
        DaggerfallDungeon dungeon;
        GameObject mainCamera;
        PlayerGPS playerGPS;

        Vector3 dungeonEntrancePosition;
        Vector3 dungeonEntranceForward;

        public GameObject ExteriorParent;
        public GameObject InteriorParent;
        public GameObject DungeonParent;
        public DaggerfallLocation OverrideLocation;

        int lastPlayerDungeonBlockIndex = -1;
        DFLocation.DungeonBlock playerDungeonBlockData = new DFLocation.DungeonBlock();

        DFLocation.BuildingTypes buildingType;

        /// <summary>
        /// True when player is inside any structure.
        /// </summary>
        public bool IsPlayerInside
        {
            get { return isPlayerInside; }
        }

        /// <summary>
        /// True only when player is inside a dungeon.
        /// </summary>
        public bool IsPlayerInsideDungeon
        {
            get { return isPlayerInsideDungeon; }
        }

        /// <summary>
        /// True only when player inside palace blocks of a dungeon.
        /// For example, main hall in Daggerfall castle.
        /// </summary>
        public bool IsPlayerInsideDungeonPalace
        {
            get { return isPlayerInsideDungeonPalace; }
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

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            playerGPS = GetComponent<PlayerGPS>();
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
                    PalaceCheck();
                    //Debug.Log(string.Format("Player is now inside block {0}", playerDungeonBlockData.BlockName));
                }
            }
        }

        #region Building Transitions

        /// <summary>
        /// Transition player through an exterior door into building interior.
        /// </summary>
        /// <param name="doorOwner">Parent transform owning door array.</param>
        /// <param name="door">Exterior door player clicked on.</param>
        public void TransitionInterior(Transform doorOwner, StaticDoor door)
        {
            // Ensure we have component references
            if (!ReferenceComponents())
                return;

            // Raise event
            RaiseOnPreTransitionEvent(TransitionType.ToBuildingInterior, door);

            // Get climate
            ClimateBases climateBase = ClimateBases.Temperate;
            if (OverrideLocation)
            {
                climateBase = OverrideLocation.Summary.Climate;
            }
            else if (playerGPS)
            {
                climateBase = ClimateSwaps.FromAPIClimateBase(playerGPS.ClimateSettings.ClimateType);
            }

            // Layout interior
            // This needs to be done first so we know where the enter markers are
            GameObject newInterior = new GameObject(string.Format("DaggerfallInterior [Block={0}, Record={1}]", door.blockIndex, door.recordIndex));
            newInterior.hideFlags = HideFlags.HideAndDontSave;
            interior = newInterior.AddComponent<DaggerfallInterior>();
            interior.DoLayout(doorOwner, door, climateBase);

            // Position interior directly inside of exterior
            // This helps with finding closest enter/exit point relative to player position
            interior.transform.position = doorOwner.position + (Vector3)door.buildingMatrix.GetColumn(3);
            interior.transform.rotation = GameObjectHelper.QuaternionFromMatrix(door.buildingMatrix);

            // Position player above closest enter marker
            Vector3 marker;
            if (!interior.FindClosestEnterMarker(transform.position, out marker))
            {
                // Could not find an enter marker, probably not a valid interior
                Destroy(newInterior);
                return;
            }

            // Assign new interior to parent
            if (InteriorParent != null)
                newInterior.transform.parent = InteriorParent.transform;

            // Disable exterior parent
            if (ExteriorParent != null)
                ExteriorParent.SetActive(false);

            // Enable interior parent
            if (InteriorParent != null)
                InteriorParent.SetActive(true);

            // Cache some information about this interior
            buildingType = interior.BuildingData.BuildingType;

            // Set player to marker position
            // Not sure how to set facing here as player transitions to a marker, not a door
            // Could always find closest door and use that
            transform.position = marker + Vector3.up * (controller.height * 0.6f);
            SetStanding();

            // Player is now inside building
            isPlayerInside = true;

            // Raise event
            RaiseOnTransitionInteriorEvent(door, interior);
        }

        /// <summary>
        /// Transition player through an interior door to building exterior. Player must be inside.
        /// Interior stores information about exterior, no need for extra params.
        /// </summary>
        public void TransitionExterior()
        {
            // Exit if missing required components or not currently inside
            if (!ReferenceComponents() || !interior || !isPlayerInside)
                return;

            // Raise event
            RaiseOnPreTransitionEvent(TransitionType.ToBuildingExterior);

            // Find closest exterior door
            Vector3 exitDoorPos = Vector3.zero;
            int doorIndex = -1;
            DaggerfallStaticDoors exteriorDoors = interior.ExteriorDoors;
            if (exteriorDoors)
            {
                if (!exteriorDoors.FindClosestDoorToPlayer(transform.position, interior.EntryDoor.recordIndex, out exitDoorPos, out doorIndex))
                {
                    // Could not find exterior door or fall back to entry door
                    // Just push player outside of building, better than having them trapped inside
                    exitDoorPos = transform.position + transform.forward * 4;
                }
            }

            // Enable exterior parent
            if (ExteriorParent != null)
                ExteriorParent.SetActive(true);

            // Disable interior parent
            if (InteriorParent != null)
                InteriorParent.SetActive(false);

            // Destroy interior game object
            Destroy(interior.gameObject);
            interior = null;

            // Set player outside exterior door position
            transform.position = exitDoorPos;
            if (doorIndex >= 0)
            {
                // Adjust player position and facing
                Vector3 normal = exteriorDoors.GetDoorNormal(doorIndex);
                transform.position += normal * (controller.radius * 2f);
                SetFacing(normal);
                SetStanding();
            }

            // Player is now outside building
            isPlayerInside = false;

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
        public void TransitionDungeonInterior(Transform doorOwner, StaticDoor door, DFLocation location)
        {
            // Ensure we have component references
            if (!ReferenceComponents())
                return;

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
            newDungeon.hideFlags = HideFlags.HideAndDontSave;
            dungeon = newDungeon.GetComponent<DaggerfallDungeon>();

            // Find start marker to position player
            if (!dungeon.StartMarker)
            {
                // Could not find a start marker
                Destroy(newDungeon);
                return;
            }

            // Cache player starting position and facing to use on exit
            dungeonEntrancePosition = transform.position;
            dungeonEntranceForward = transform.forward;

            // Disable exterior parent
            if (ExteriorParent != null)
                ExteriorParent.SetActive(false);

            // Enable dungeon parent
            if (DungeonParent != null)
                DungeonParent.SetActive(true);

            // Player is now inside dungeon
            isPlayerInside = true;
            isPlayerInsideDungeon = true;

            // Set to start position
            MovePlayerToDungeonStart();

            // Raise event
            RaiseOnTransitionDungeonInteriorEvent(door, dungeon);
        }

        public void MovePlayerToDungeonStart()
        {
            if (!isPlayerInsideDungeon)
                return;

            // Set player to start position
            transform.position = dungeon.StartMarker.transform.position + Vector3.up * (controller.height * 0.6f);

            // Fix player standing
            SetStanding();

            // Raise event
            RaiseOnMovePlayerToDungeonStartEvent();
        }

        /// <summary>
        /// Player is leaving dungeon, transition them back outside.
        /// </summary>
        public void TransitionDungeonExterior()
        {
            if (!ReferenceComponents() || !dungeon || !isPlayerInsideDungeon)
                return;

            // Raise event
            RaiseOnPreTransitionEvent(TransitionType.ToDungeonExterior);

            // Enable exterior parent
            if (ExteriorParent != null)
                ExteriorParent.SetActive(true);

            // Disable dungeon parent
            if (DungeonParent != null)
                DungeonParent.SetActive(false);

            // Destroy dungeon game object
            Destroy(dungeon.gameObject);
            dungeon = null;

            // Set player outside exterior door position and set facing
            transform.position = dungeonEntrancePosition;
            SetFacing(-dungeonEntranceForward);
            SetStanding();

            // Player is now outside dungeon
            isPlayerInside = false;
            isPlayerInsideDungeon = false;
            isPlayerInsideDungeonPalace = false;
            lastPlayerDungeonBlockIndex = -1;
            playerDungeonBlockData = new DFLocation.DungeonBlock();

            // Raise event
            RaiseOnTransitionDungeonExteriorEvent();
        }

        #endregion

        // Check if current block is a palace block
        private void PalaceCheck()
        {
            if (!isPlayerInsideDungeon)
            {
                isPlayerInsideDungeonPalace = false;
                return;
            }

            switch (playerDungeonBlockData.BlockName)
            {
                case "S0000020.RDB":    // Orsinium palace area
                case "S0000040.RDB":    // Sentinel palace area
                case "S0000041.RDB":
                case "S0000042.RDB":
                case "S0000080.RDB":    // Wayrest palace area
                case "S0000081.RDB":
                case "S0000160.RDB":    // Daggerfall palace area
                    isPlayerInsideDungeonPalace = true;
                    break;
                default:
                    isPlayerInsideDungeonPalace = false;
                    break;
            }
        }

        private void SetFacing(Vector3 forward)
        {
            // Set player facing direction
            if (mainCamera)
            {
                PlayerMouseLook mouseLook = mainCamera.GetComponent<PlayerMouseLook>();
                if (mouseLook)
                {
                    mouseLook.SetFacing(forward);
                }
            }
        }

        private void SetStanding()
        {
            // Snap player to ground
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out hit, controller.height * 2f))
            {
                // Position player at hit position plus just over half controller height up
                transform.position = hit.point + Vector3.up * (controller.height * 0.6f);
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

        // OnPreTransition - Called PRIOR to any transition, other events called AFTER transition.
        public delegate void OnPreTransitionEventHandler(TransitionEventArgs args);
        public static event OnPreTransitionEventHandler OnPreTransition;
        protected virtual void RaiseOnPreTransitionEvent(TransitionType transitionType)
        {
            TransitionEventArgs args = new TransitionEventArgs(TransitionType.ToBuildingInterior);
            if (OnPreTransition != null)
                OnPreTransition(args);
        }
        protected virtual void RaiseOnPreTransitionEvent(TransitionType transitionType, StaticDoor staticDoor)
        {
            TransitionEventArgs args = new TransitionEventArgs(TransitionType.ToBuildingInterior, staticDoor);
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