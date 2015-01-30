// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

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
            //    go.transform.parent = transform;
            //    go.transform.position = transform.rotation * Doors[i].buildingMatrix.MultiplyPoint3x4(Doors[i].centre);
            //    go.transform.position += transform.position;
            //    go.transform.rotation = transform.rotation;

            //    BoxCollider c = go.AddComponent<BoxCollider>();
            //    c.size = GameObjectHelper.QuaternionFromMatrix(Doors[i].buildingMatrix) * Doors[i].size;
            //    c.isTrigger = true;
            //}
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
                go.transform.position = transform.rotation * Doors[i].buildingMatrix.MultiplyPoint3x4(Doors[i].centre);
                c.size = GameObjectHelper.QuaternionFromMatrix(Doors[i].buildingMatrix) * Doors[i].size;
                go.transform.position += transform.position;
                go.transform.rotation = transform.rotation;

                // Deprecated: Bounds checking method.
                //Vector3 centre = transform.rotation * Doors[i].buildingMatrix.MultiplyPoint3x4(Doors[i].centre) + transform.position;
                //Vector3 size = new Vector3(50, 90, 50) * MeshReader.GlobalScale; // Native door fit
                //Bounds bounds = new Bounds(centre, size);

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
        /// Gets world transformed normal of door at index.
        /// </summary>
        /// <param name="index">Door index.</param>
        /// <returns>Normal pointing away from door in world.</returns>
        public Vector3 GetDoorNormal(int index)
        {
            return Vector3.Normalize(transform.rotation * Doors[index].buildingMatrix.MultiplyVector(Doors[index].normal));
        }
    }
}