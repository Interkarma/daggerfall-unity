// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Numidium
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
            //    Quaternion buildingRotation = GameObjectHelper.QuaternionFromMatrix(Doors[i].buildingMatrix);
            //    Vector3 doorNormal = buildingRotation * Doors[i].normal;
            //    Quaternion facingRotation = Quaternion.LookRotation(doorNormal, Vector3.up);

            //    GameObject go = new GameObject();
            //    go.name = "DoorTrigger";

            //    BoxCollider c = go.AddComponent<BoxCollider>();
            //    c.size = Doors[i].size;
            //    go.transform.parent = transform;
            //    go.transform.position = transform.rotation * Doors[i].buildingMatrix.MultiplyPoint3x4(Doors[i].centre);
            //    go.transform.position += transform.position;
            //    go.transform.rotation = facingRotation;
            //    c.isTrigger = true;
            //}
            //Debug.LogFormat("Added {0} door triggers to scene", Doors.Length);

            // Promote dungeon exit doors to full physical colliders rather than just virtual trigger volumes
            // This step is based on the following:
            //  * Daggerfall uses a mixture of discrete exit quads (modelid 70300) and exits baked into surrounding geometry (e.g. modelid 58051)
            //  * During asset import it's simple to add colliders to a specific model ID, but not so much when exit is just one texture face in larger model
            //  * In all cases, the exit is a single-sided quad that doesn't play nice with mesh colliders (allows traversal from behind in areas blocked by exit)
            //  * Work around this by always placing a fully physical BoxCollider in position of virtual trigger volume
            // NOTES:
            //  * Unlike virtual door triggers that use a symmetric volume shape, this door trigger is physical and needs to be nicely sized
            //  * Collider size uses DF units, is scaled based on MeshReader.GlobalScale, then rotated into position
            Vector3 exitColliderSize = new Vector3(50f, 90f, 2f);
            for (int i = 0; i < Doors.Length; i++)
            {
                if (Doors[i].doorType == DoorTypes.DungeonExit)
                {
                    // Get correct facing of exit
                    Quaternion buildingRotation = GameObjectHelper.QuaternionFromMatrix(Doors[i].buildingMatrix);
                    Vector3 doorNormal = buildingRotation * Doors[i].normal;
                    Quaternion facingRotation = Quaternion.LookRotation(doorNormal, Vector3.up);

                    // Create object
                    GameObject exitObject = new GameObject();
                    exitObject.name = "DungeonExit";

                    // Add collider
                    BoxCollider exitCollider = exitObject.AddComponent<BoxCollider>();
                    exitCollider.center = Vector3.zero;
                    exitCollider.size = exitColliderSize * MeshReader.GlobalScale;
                    exitObject.transform.parent = transform;
                    exitObject.transform.position = transform.rotation * Doors[i].buildingMatrix.MultiplyPoint3x4(Doors[i].centre);
                    exitObject.transform.position += transform.position;
                    exitObject.transform.rotation = facingRotation;
                }
            }
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
                Quaternion buildingRotation = GameObjectHelper.QuaternionFromMatrix(Doors[i].buildingMatrix);
                Vector3 doorNormal = buildingRotation * Doors[i].normal;
                Quaternion facingRotation = Quaternion.LookRotation(doorNormal, Vector3.up);

                // Setup single trigger position and size over each door in turn
                // This method plays nice with transforms
                c.size = Doors[i].size;
                go.transform.parent = transform;
                go.transform.position = transform.rotation * Doors[i].buildingMatrix.MultiplyPoint3x4(Doors[i].centre);
                go.transform.position += transform.position;
                go.transform.rotation = facingRotation;

                // Check if hit was inside trigger
                if (c.bounds.Contains(point))
                {
                    found = true;
                    doorOut = Doors[i];
                    if (doorOut.doorType == DoorTypes.DungeonExit)
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
        public bool FindClosestDoorToPlayer(Vector3 playerPos, int record, out Vector3 doorPosOut, out int doorIndexOut, DoorTypes requiredDoorType = DoorTypes.None)
        {
            // Init output
            doorPosOut = Vector3.zero;
            doorIndexOut = -1;

            // Must have door array
            if (Doors == null)
                return false;

            // Find closest door to player position
            float minDistance = float.MaxValue;
            bool found = false;
            for (int i = 0; i < Doors.Length; i++)
            {
                // Must be of door type if set
                if (requiredDoorType != DoorTypes.None && Doors[i].doorType != requiredDoorType)
                    continue;

                // Check if door belongs to same building record or accept any record
                if (record == -1 || Doors[i].recordIndex == record)
                {
                    // Get this door centre in world space
                    Vector3 centre = transform.rotation * Doors[i].buildingMatrix.MultiplyPoint3x4(Doors[i].centre) + transform.position;

                    // Check distance and save closest
                    float distance = Vector3.Distance(playerPos, centre);
                    if (distance < minDistance)
                    {
                        doorPosOut = centre;
                        doorIndexOut = i;
                        minDistance = distance;
                        found = true;
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// Find the lowest, farthest from building origin door position in world space.
        /// </summary>
        /// <param name="record">Door record index.</param>
        /// <param name="doorPosOut">Position of closest door in world space.</param>
        /// <param name="doorIndexOut">Door index in Doors array of closest door.</param>
        /// <returns>Whether or not we found a door</returns>
        public bool FindLowestOutermostDoor(int record, out Vector3 doorPosOut, out int doorIndexOut)
        {
            doorPosOut = Vector3.zero;
            doorIndexOut = -1;

            if (Doors == null)
                return false;

            // Find lowest (and outermost) door in interior
            float lowestY = float.MaxValue;
            double farthestDist = 0d;
            for (int i = 0; i < Doors.Length; i++)
            {
                // Get this door centre in world space
                Vector3 centre = transform.rotation * Doors[i].buildingMatrix.MultiplyPoint3x4(Doors[i].centre) + transform.position;

                // Check if door belongs to same building record or accept any record
                if (Doors[i].recordIndex == record || record == -1)
                {
                    var interiorPos = GameObjectHelper.GetSpawnParentTransform().position;
                    float y = centre.y;
                    var dist = Vector2.Distance(new Vector2(interiorPos.x, interiorPos.z), new Vector2(centre.x, centre.z));
                    if (y <= lowestY && dist > farthestDist)
                    {
                        doorPosOut = centre;
                        doorIndexOut = i;
                        lowestY = y;
                        farthestDist = dist;
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