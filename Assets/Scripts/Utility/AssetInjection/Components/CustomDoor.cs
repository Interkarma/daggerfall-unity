// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2021 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:           This is to be considered EXPERIMENTAL and can be changed or removed at any time.
//

using UnityEngine;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Defines a custom door for a building imported by Asset-Injection framework.
    /// </summary>
    /// <remarks>
    /// TODO:
    /// - Interior to exterior transition should be based on position of new doors.
    /// </remarks>
    [ExecuteInEditMode]
    [RequireComponent(typeof(BoxCollider))]
    public class CustomDoor : MonoBehaviour
    {
        private StaticDoor? staticDoor;

        [Tooltip("Number of attached Box Colliders that will be used as doors.")]
        public int NumberOfTriggers = 1;
        
        [Tooltip("Type of door that is being defined. None means building textures are used to determine this.")]
        public DoorTypes DoorType = DoorTypes.None;
        
        /// <summary>
        /// The triggers for the doors.
        /// </summary>
        [HideInInspector]
        public BoxCollider[] DoorTriggers;

        private void Awake()
        {
            if (DoorTriggers.Length < 1)
                DoorTriggers = GetComponents<BoxCollider>();

            string objectName;
            if (DoorTriggers.Length < 1)
            {
                if (name == "default")
                    objectName = transform.parent.name;
                else
                    objectName = name;
                Debug.LogErrorFormat("No Box Colliders are attached to {0}. Did you forget to add them?", objectName);
                return;
            }
            if (NumberOfTriggers < 1)
            {
                if (name == "default")
                    objectName = transform.parent.name;
                else
                    objectName = name;
                Debug.LogErrorFormat("Number of Triggers for {0} was less than 1. Did you put in the wrong number?", objectName);
                return;
            }
            if (NumberOfTriggers > DoorTriggers.Length)
            {
                if (name == "default")
                    objectName = transform.parent.name;
                else
                    objectName = name;
                Debug.LogErrorFormat("Number of Triggers for {0} was greater than the number of attached Box Colliders. Are you missing Box Colliders? Did you put in the wrong number?", objectName);
                return;
            }

            for (int i = 0; i < NumberOfTriggers; i++)
                DoorTriggers[i].isTrigger = true;
        }

        /// <summary>
        /// Set static door data to all doors in the given building gameobject.
        /// </summary>
        /// <param name="building">The imported gameobject which provides building and doors.</param>
        /// <param name="staticDoors">The list of static doors for the vanilla building.</param>
        /// <param name="buildingKey">The key of the building that owns the doors.</param>
        public static void InitDoors(GameObject building, StaticDoor[] staticDoors, int buildingKey)
        {
            // Make data for the static door; only the common data for the building is needed.
            StaticDoor staticDoor = staticDoors[0];
            staticDoor.buildingKey = buildingKey;

            // Set data to all doors in the building
            CustomDoor[] allCustomDoors = building.GetComponentsInChildren<CustomDoor>();
            for (int i = 0; i < allCustomDoors.Length; i++)
            {
                if (allCustomDoors[i].DoorType != DoorTypes.None)
                    staticDoor.doorType = allCustomDoors[i].DoorType;
                allCustomDoors[i].staticDoor = staticDoor;
            }
        }

        /// <summary>
        /// Checks for a door hit.
        /// </summary>
        /// <param name="raycastHit">The raycast result.</param>
        /// <param name="door">The door found at hit position.</param>
        /// <returns>True if a door has been hit.</returns>
        /// <remarks>
        /// Modders should ensure that the CustomDoor component is added to the right gameobject.
        /// This allows to avoid unnecessary seeks on parent/children, which are potentially performance heavy.
        /// </remarks>
        public static bool HasHit(RaycastHit raycastHit, out StaticDoor door, out bool noCustomDoorFound)
        {
            var doors = raycastHit.transform.GetComponents<CustomDoor>();
            for (int i = 0; i < doors.Length; i++)
            {
                for (int j = 0; j < doors[i].NumberOfTriggers; j++)
                {
                    if (doors[i].DoorTriggers[j].bounds.Contains(raycastHit.point))
                    {
                        if (doors[i].staticDoor.HasValue)
                        {
                            door = doors[i].staticDoor.Value;
                            noCustomDoorFound = false;
                            return true;
                        }

                        Debug.LogErrorFormat("Static door for {0} is not set.", raycastHit.transform.name);
                    }
                }
            }

            noCustomDoorFound = false;
            if (doors.Length == 0)
                noCustomDoorFound = true;
            door = new StaticDoor();
            return false;
        }
    }
}
