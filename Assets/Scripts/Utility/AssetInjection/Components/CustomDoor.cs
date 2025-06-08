// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    Hazelnut
// 
// Notes:           This is to be considered EXPERIMENTAL and can be changed or removed at any time.
//

using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Defines a custom door for a building imported by Asset-Injection framework.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(BoxCollider))]
    public class CustomDoor : MonoBehaviour
    {
        private StaticDoor? staticDoor;

        [Tooltip("Sets the door type for this custom door. (ignored if a DF model static door is copied)")]
        public DoorTypes DoorType = DoorTypes.Building;

        [Tooltip("Used to specify a door index to copy from this model's StaticDoor array if available for this custom door, defaults to -1 to not copy.")]
        public int StaticDoorCopied = -1;

        [Tooltip("Disables all classic doors on this model. (Exit a building doesn't work well if this is true and all custom doors are copied)")]
        public bool DisableClassicDoors = false;
        
        [Tooltip("The box collider defining this door. If unset, uses the first box collider found on model gameobject.")]
        public BoxCollider DoorTrigger;

        private void Awake()
        {
            if (!DoorTrigger)
                DoorTrigger = GetComponent<BoxCollider>();
            DoorTrigger.isTrigger = true;
            DoorTrigger.enabled = (StaticDoorCopied == -1) ? false : true;
        }


        /// <summary>
        /// Initialise all interior custom doors found on the given building interior gameobject.
        /// </summary>
        /// <param name="building">The imported gameobject which provides building and doors.</param>
        /// <param name="staticDoors">A list of static doors for the vanilla building for copying, if available.</param>
        /// <param name="blockIndex">Block index for RMB doors.</param>
        /// <param name="recordIndex">Record index of interior.</param>
        /// <param name="buildingMatrix">Individual building matrix.</param>
        /// <param name="disableClassicDoors">If true, at least one custom door component has requested suppression of all classic doors for this model.</param>
        /// <returns>List of StaticDoor data for any custom doors that don't copy a DF model StaticDoor.</returns>
        public static List<StaticDoor> InitDoorsInterior(GameObject building, StaticDoor[] staticDoors, int blockIndex, int recordIndex, Matrix4x4 buildingMatrix, out bool disableClassicDoors)
        {
            return InitDoors(building, staticDoors, -1, blockIndex, recordIndex, buildingMatrix, out disableClassicDoors);
        }

        /// <summary>
        /// Initialise all custom doors found on the given building gameobject.
        /// </summary>
        /// <param name="building">The imported gameobject which provides building and doors.</param>
        /// <param name="staticDoors">A list of static doors for the vanilla building for copying, if available.</param>
        /// <param name="buildingKey">The key of the building that owns the exterior doors, -1 for interior doors.</param>
        /// <param name="blockIndex">Block index for RMB doors.</param>
        /// <param name="recordIndex">Record index of interior.</param>
        /// <param name="buildingMatrix">Individual building matrix.</param>
        /// <param name="disableClassicDoors">If true, at least one custom door component has requested suppression of all classic doors for this model.</param>
        /// <returns>List of StaticDoor data for any custom doors that don't copy a DF model StaticDoor.</returns>
        public static List<StaticDoor> InitDoors(GameObject building, StaticDoor[] staticDoors, int buildingKey, int blockIndex, int recordIndex, Matrix4x4 buildingMatrix, out bool disableClassicDoors)
        {
            // Iterate through all custom door components for this building
            disableClassicDoors = false;
            CustomDoor[] allCustomDoors = building.GetComponentsInChildren<CustomDoor>();
            List<StaticDoor> customStaticDoors = new List<StaticDoor>(allCustomDoors.Length);
            for (int i = 0; i < allCustomDoors.Length; i++)
            {
                if (allCustomDoors[i].DisableClassicDoors == true)
                    disableClassicDoors = true;

                // If no static doors available (i.e. not replacing a DF model with door data) or not configured to copy one
                if (allCustomDoors[i].StaticDoorCopied == -1 || staticDoors == null || staticDoors.Length == 0)
                {
                    // Calculate the door normal using smallest dimension as direction
                    Vector3 doorCenter = allCustomDoors[i].DoorTrigger.center;
                    Vector3 doorSize = allCustomDoors[i].DoorTrigger.size;
                    Vector3 doorNormal = new Vector3();
                    if (doorSize.x < doorSize.z && doorSize.x < doorSize.y) {
                        doorNormal.x = Mathf.Sign(doorCenter.x);
                        doorSize.x = doorSize.z;    // Set x=z so static door hit detection works
                    } else if (doorSize.z < doorSize.x && doorSize.z < doorSize.y) {
                        doorNormal.z = Mathf.Sign(doorCenter.z);
                        doorSize.z = doorSize.x;    // Set z=x so static door hit detection works
                    }
                    else if (doorSize.y < doorSize.x && doorSize.y < doorSize.z) {
                        doorNormal.y = Mathf.Sign(doorCenter.y);
                    }

                    // Invert normal for interior doors (building key = -1)
                    if (buildingKey < 0) {
                        doorNormal.x = -doorNormal.x;
                        doorNormal.y = -doorNormal.y;
                        doorNormal.z = -doorNormal.z;
                    }

                    // Construct new StaticDoor data for this custom door and add it to list
                    StaticDoor staticDoor = new StaticDoor
                    {
                        doorType = allCustomDoors[i].DoorType,
                        buildingKey = buildingKey,
                        blockIndex = blockIndex,
                        recordIndex = recordIndex,
                        buildingMatrix = buildingMatrix,
                        doorIndex = i,
                        centre = doorCenter,
                        size = doorSize,
                        normal = doorNormal
                    };
                    customStaticDoors.Add(staticDoor);
                }
                else if (allCustomDoors[i].StaticDoorCopied < staticDoors.Length)
                {
                    // Copy data from the specified existing static door
                    StaticDoor staticDoor = staticDoors[allCustomDoors[i].StaticDoorCopied];
                    staticDoor.buildingKey = buildingKey;
                    allCustomDoors[i].staticDoor = staticDoor;
                }
                else
                {
                    string objectName;
                    if (allCustomDoors[i].name == "default")
                        objectName = allCustomDoors[i].transform.parent.name + "\\default";
                    else
                        objectName = allCustomDoors[i].name;
                    Debug.LogErrorFormat("StaticDoorCopied for CustomDoor {1} on model {0} is outside the valid range for its Static Door array and has been ignored.", objectName, i);
                }
            }

            return customStaticDoors;
        }

        /// <summary>
        /// Checks for a door hit in custom doors copied (linked) from DF model static doors.
        /// </summary>
        /// <param name="raycastHit">The raycast result.</param>
        /// <param name="door">The door found at hit position.</param>
        /// <returns>True if a door has been hit.</returns>
        /// <remarks>
        /// Only used for custom doors that were linked to a DF model static door, e.g. staticDoor reference is set.
        /// Modders should ensure that the CustomDoor component is added to the right gameobject.
        /// This allows to avoid unnecessary seeks on parent/children, which are potentially performance heavy.
        /// </remarks>
        public static bool HasHit(RaycastHit raycastHit, out StaticDoor door)
        {
            var doors = raycastHit.transform.GetComponents<CustomDoor>();
            for (int i = 0; i < doors.Length; i++)
            {
                if (!doors[i].staticDoor.HasValue)
                    continue;

                if (doors[i].DoorTrigger.bounds.Contains(raycastHit.point))
                {
                    door = doors[i].staticDoor.Value;
                    return true;
                }
            }

            door = new StaticDoor();
            return false;
        }
    }
}
