// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Stores an array of static doors.
    /// Exposes helpers to check doors in correct world space.
    /// </summary>
    [Serializable]
    public class DaggerfallStaticDoors : MonoBehaviour
    {
        public StaticDoor[] Doors;                  // Array of doors attached this building or group of buildings

        void Start()
        {
            //// Debug trigger placement at start
            //for (int i = 0; i < Doors.Length; i++)
            //{
            //    GameObject go = new GameObject();
            //    go.name = "DoorTrigger";
            //    go.transform.parent = transform;
            //    go.transform.position = transform.rotation * Doors[i].buildingMatrix.MultiplyPoint3x4(Doors[i].centre);
            //    go.transform.position += transform.position;
            //    go.transform.rotation = transform.rotation;

            //    BoxCollider c = go.AddComponent<BoxCollider>();
            //    c.size = GameObjectHelper.QuaternionFromMatrix(Doors[i].buildingMatrix) * Doors[i].size;
            //    c.size = new Vector3(Mathf.Abs(c.size.x), Mathf.Abs(c.size.y), Mathf.Abs(c.size.z)); // Abs size components so not negative for collider
            //    c.isTrigger = true;
            //}
            //Debug.LogFormat("Added {0} door triggers to scene", Doors.Length);
        }

        /// <summary>
        /// Check for a door hit in world space.
        /// </summary>
        /// <param name="point">Hit point from ray test in world space.</param>
        /// <param name="doorOut">StaticDoor out if hit found.</param>
        /// <returns>True if point hits a static door.</returns>
        public bool HasHit(Vector3 point, out StaticDoor doorOut)
        {
            doorOut = new StaticDoor();
            if (Doors == null)
                return false;

            // Using a single hidden trigger created when testing door positions
            // This avoids problems with AABBs as trigger rotates nicely with model transform
            // A trigger is also more useful for debugging as its drawn by editor
            GameObject go = new GameObject();
            go.hideFlags = HideFlags.HideAndDontSave;
            go.transform.parent = transform;
            BoxCollider c = go.AddComponent<BoxCollider>();
            c.isTrigger = true;

            // Test each door in array
            bool found = false;
            for (int i = 0; i < Doors.Length; i++)
            {
                // Setup single trigger position and size over each door in turn
                // This method plays nice with transforms
                c.size = GameObjectHelper.QuaternionFromMatrix(Doors[i].buildingMatrix) * Doors[i].size;
                go.transform.position = transform.rotation * Doors[i].buildingMatrix.MultiplyPoint3x4(Doors[i].centre);
                go.transform.position += transform.position;
                go.transform.rotation = transform.rotation;

                // Check if hit was inside trigger
                if (c.bounds.Contains(point))
                {
                    found = true;
                    doorOut = Doors[i];
                    break;
                }
            }

            // Remove temp trigger
            if (go)
                Destroy(go);

            return found;
        }

        /// <summary>
        /// Find closest door to player position in world space.
        /// </summary>
        /// <param name="playerPos">Player position in world space.</param>
        /// <param name="record">Door record index.</param>
        /// <param name="doorPosOut">Position of closest door in world space.</param>
        /// <param name="doorIndexOut">Door index in Doors array of closest door.</param>
        /// <returns></returns>
        public bool FindClosestDoorToPlayer(Vector3 playerPos, int record, out Vector3 doorPosOut, out int doorIndexOut)
        {
            // Init output
            doorPosOut = playerPos;
            doorIndexOut = -1;

            // Must have door array
            if (Doors == null)
                return false;

            // Find closest door to player position
            float minDistance = float.MaxValue;
            for (int i = 0; i < Doors.Length; i++)
            {
                // Get this door centre in world space
                Vector3 centre = transform.rotation * Doors[i].buildingMatrix.MultiplyPoint3x4(Doors[i].centre) + transform.position;

                // Check if door belongs to same building record
                if (Doors[i].recordIndex == record)
                {
                    // Check distance and save closest
                    float distance = Vector3.Distance(playerPos, centre);
                    if (distance < minDistance)
                    {
                        doorPosOut = centre;
                        doorIndexOut = i;
                        minDistance = distance;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Find closest door to player position in world space.
        /// Owner position and rotation must be set.
        /// </summary>
        /// <param name="playerPos">Player position in world space.</param>
        /// <param name="record">Door record index.</param>
        /// <param name="doorPosOut">Position of closest door in world space.</param>
        /// <param name="doorIndexOut">Door index in Doors array of closest door.</param>
        /// <returns></returns>
        public static bool FindClosestDoorToPlayer(Vector3 playerPos, StaticDoor[] doors, out Vector3 doorPosOut, out int doorIndexOut)
        {
            // Init output
            doorPosOut = playerPos;
            doorIndexOut = -1;

            // Must have door array
            if (doors == null || doors.Length == 0)
                return false;

            // Find closest door to player position
            float minDistance = float.MaxValue;
            for (int i = 0; i < doors.Length; i++)
            {
                // Get this door centre in world space
                Vector3 centre = doors[i].ownerRotation * doors[i].buildingMatrix.MultiplyPoint3x4(doors[i].centre) + doors[i].ownerPosition;

                // Check distance and save closest
                float distance = Vector3.Distance(playerPos, centre);
                if (distance < minDistance)
                {
                    doorPosOut = centre;
                    doorIndexOut = i;
                    minDistance = distance;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds closest door in any array of static doors.
        /// Owner position and rotation must be set.
        /// </summary>
        /// <param name="position">Position to find closest door to.</param>
        /// <param name="doors">Door array.</param>
        /// <returns>Position of closest door in world space.</returns>
        public static Vector3 FindClosestDoor(Vector3 position, StaticDoor[] doors, out StaticDoor closestDoorOut)
        {
            closestDoorOut = new StaticDoor();
            Vector3 closestDoorPos = position;
            float minDistance = float.MaxValue;
            for (int i = 0; i < doors.Length; i++)
            {
                // Get this door centre in world space
                Vector3 centre = doors[i].ownerRotation * doors[i].buildingMatrix.MultiplyPoint3x4(doors[i].centre) + doors[i].ownerPosition;

                // Check distance and store closest
                float distance = Vector3.Distance(position, centre);
                if (distance < minDistance)
                {
                    closestDoorPos = centre;
                    minDistance = distance;
                    closestDoorOut = doors[i];
                }
            }

            return closestDoorPos;
        }

        /// <summary>
        /// Gets door position in world space.
        /// Owner position and rotation must be set.
        /// </summary>
        /// <param name="index">Door index.</param>
        /// <returns>Door position in world space.</returns>
        public static Vector3 GetDoorPosition(StaticDoor door)
        {
            Vector3 centre = door.ownerRotation * door.buildingMatrix.MultiplyPoint3x4(door.centre) + door.ownerPosition;

            return centre;
        }

        /// <summary>
        /// Gets door normal in world space.
        /// Owner position and rotation must be set.
        /// </summary>
        /// <param name="door">Door to calculate normal for.</param>
        /// <returns>Normal pointing away from door in world.</returns>
        public static Vector3 GetDoorNormal(StaticDoor door)
        {
            return Vector3.Normalize(door.ownerRotation * door.buildingMatrix.MultiplyVector(door.normal));
        }

        /// <summary>
        /// Finds all doors of type across multiple door collections.
        /// </summary>
        /// <param name="doorCollections">Door collections.</param>
        /// <param name="type">Type of door to search for.</param>
        /// <returns>Array of matching doors.</returns>
        public static StaticDoor[] FindDoorsInCollections(DaggerfallStaticDoors[] doorCollections, DoorTypes type)
        {
            List<StaticDoor> doorsOut = new List<StaticDoor>();
            foreach (var collection in doorCollections)
            {
                for (int i = 0; i < collection.Doors.Length; i++)
                {
                    if (collection.Doors[i].doorType == type)
                    {
                        StaticDoor newDoor = collection.Doors[i];
                        newDoor.ownerPosition = collection.transform.position;
                        newDoor.ownerRotation = collection.transform.rotation;
                        doorsOut.Add(newDoor);
                    }
                }
            }

            return doorsOut.ToArray();
        }

        /// <summary>
        /// Gets door position in world space.
        /// </summary>
        /// <param name="index">Door index.</param>
        /// <returns>Door position in world space.</returns>
        public Vector3 GetDoorPosition(int index)
        {
            Vector3 centre = transform.rotation * Doors[index].buildingMatrix.MultiplyPoint3x4(Doors[index].centre) + transform.position;

            return centre;
        }

        /// <summary>
        /// Gets world transformed normal of door at index.
        /// </summary>
        /// <param name="index">Door index.</param>
        /// <returns>Normal pointing away from door in world.</returns>
        public Vector3 GetDoorNormal(int index)
        {
            return Vector3.Normalize(transform.rotation * Doors[index].buildingMatrix.MultiplyVector(Doors[index].normal));
        }

        /// <summary>
        /// Gets the first parent RMB block of this door component.
        /// </summary>
        public DaggerfallRMBBlock GetParentRMBBlock()
        {
            return GetComponentInParent<DaggerfallRMBBlock>();
        }
    }
}