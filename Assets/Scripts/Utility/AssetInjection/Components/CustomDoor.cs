// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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

        /// <summary>
        /// The trigger for this door.
        /// </summary>
        [Tooltip("The trigger for this door.")]
        public BoxCollider DoorTrigger;

        private void Awake()
        {
            if (!DoorTrigger)
                DoorTrigger = GetComponent<BoxCollider>();

            DoorTrigger.isTrigger = true;
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
            var doors = building.GetComponentsInChildren<CustomDoor>();
            for (int i = 0; i < doors.Length; i++)
                doors[i].staticDoor = staticDoor;
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
        public static bool HasHit(RaycastHit raycastHit, out StaticDoor door)
        {
            var doors = raycastHit.transform.GetComponents<CustomDoor>();
            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i].DoorTrigger.bounds.Contains(raycastHit.point))
                {
                    if (doors[i].staticDoor.HasValue)
                    {
                        door = doors[i].staticDoor.Value;
                        return true;
                    }

                    Debug.LogErrorFormat("Static door for {0} is not set.", raycastHit.transform.name);
                }
            }

            door = new StaticDoor();
            return false;
        }
    }
}
