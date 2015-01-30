// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

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
        DaggerfallInterior interior;
        GameObject mainCamera;
        PlayerGPS playerGPS;

        public GameObject ExteriorParent;
        public GameObject InteriorParent;
        public GameObject DungeonParent;

        /// <summary>
        /// True when player is inside, otherwise false.
        /// </summary>
        public bool IsPlayerInside
        {
            get { return isPlayerInside; }
        }

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            playerGPS = GetComponent<PlayerGPS>();
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

            // Get current climate
            ClimateBases climateBase = ClimateBases.Temperate;
            if (playerGPS)
            {
                climateBase = ClimateSwaps.FromAPIClimateBase(playerGPS.ClimateSettings.ClimateType);
            }

            // Layout interior
            // This needs to be done first so we know where the enter markers are
            GameObject newInterior = new GameObject(string.Format("DaggerfallInterior [Block={0}, Record={1}]", door.blockIndex, door.recordIndex));
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

            // Set player to marker position
            // Not sure how to set facing here as player transitions to a marker, not a door
            // Could always find closest door and use that
            transform.position = marker + Vector3.up * (controller.height * 0.6f);
            SetStanding();

            // Player is now inside building
            isPlayerInside = true;
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

            // Layout dungeon
            GameObject newDungeon = GameObjectHelper.CreateDaggerfallDungeonGameObject(location, DungeonParent.transform);

            //// Position player above closest enter marker
            //Vector3 marker;
            //if (!interior.FindClosestEnterMarker(transform.position, out marker))
            //{
            //    // Could not find an enter marker, probably not a valid interior
            //    Destroy(newInterior);
            //    return;
            //}

            // Disable exterior parent
            if (ExteriorParent != null)
                ExteriorParent.SetActive(false);

            // Enable dungeon parent
            if (DungeonParent != null)
                DungeonParent.SetActive(true);

            //// Set player to marker position
            //// Not sure how to set facing here as player transitions to a marker, not a door
            //// Could always find closest door and use that
            //transform.position = marker + Vector3.up * (controller.height * 0.6f);
            //SetStanding();

            // Player is now inside dungeon
            isPlayerInside = true;
        }

        #endregion

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
    }
}