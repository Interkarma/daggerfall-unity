// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// Spawn one or more enemies near player.
    /// Will attempt to start placing spawns after game objects are set and spawn count greater than 0.
    /// This is a generic spawn helper not tied to any specific system.
    /// NOTES:
    ///  * Spawns foes immediately. Be careful spawning multiple foes as they are likely to become stuck on each other.
    ///  * The spawner will self-destroy once all foes spawned. Do not attach to anything you want to remain in scene.
    ///  * There is a prefab carrying this component in Prefabs/Scene for easy spawner setups.
    ///  * Will attempt to find best parent at time if none specified (e.g. dungeon, interior).
    ///  * Might need to reduce MinDistance if expecting to spawn in tigt confines like small interiors.
    /// </summary>
    public class FoeSpawner : MonoBehaviour
    {
        // Set these values at create time to setup a foe spawn automatically on start
        public MobileTypes FoeType = MobileTypes.None;
        public int SpawnCount = 0;
        public float MinDistance = 4f;
        public float MaxDistance = 20f;
        public Transform Parent = null;
        public bool LineOfSightCheck = true;
        public bool AlliedToPlayer = false;

        public MobileTypes lastFoeType = MobileTypes.None;
        GameObject[] pendingFoeGameObjects;
        int pendingFoesSpawned = 0;
        bool spawnInProgress = false;

        void Update()
        {
            // Create new foe list when changed in editor
            if (FoeType != MobileTypes.None && FoeType != lastFoeType && SpawnCount > 0)
            {
                DestroyOldFoeGameObjects(pendingFoeGameObjects);
                SetFoeGameObjects(GameObjectHelper.CreateFoeGameObjects(Vector3.zero, FoeType, SpawnCount, alliedToPlayer: AlliedToPlayer));
                lastFoeType = FoeType;
            }

            // Do nothing if no spawns or we are done spawning
            if (pendingFoeGameObjects == null || !spawnInProgress)
                return;

            // Clear pending foes if all have been spawned
            if (spawnInProgress && pendingFoesSpawned >= pendingFoeGameObjects.Length)
            {
                spawnInProgress = false;
                Destroy(gameObject);
                return;
            }

            // Try placing foes near player
            PlaceFoeFreely(pendingFoeGameObjects, MinDistance, MaxDistance);

            if (spawnInProgress)
                GameManager.Instance.RaiseOnEncounterEvent();
        }

        #region Public Methods

        /// <summary>
        /// Assign an array of pending foe GameObjects to spawn.
        /// The spawner will then try to place these foes around player until none remain.
        /// Use GameObjectHelper.CreateFoeGameObjects() static method to create foe GameObjects first.
        /// </summary>
        public void SetFoeGameObjects(GameObject[] gameObjects, Transform parent = null)
        {
            // Do nothing if array not valid
            if (gameObjects == null || gameObjects.Length == 0)
            {
                spawnInProgress = false;
                return;
            }

            // Store array and start spawning
            pendingFoeGameObjects = gameObjects;
            pendingFoesSpawned = 0;
            spawnInProgress = true;
            
            // Set parent if specified
            if (parent)
            {
                Parent = parent;
                for (int i = 0; i < pendingFoeGameObjects.Length; i++)
                {
                    pendingFoeGameObjects[i].transform.parent = parent;
                }
            }
        }

        #endregion

        #region Private Methods

        // Uses raycasts to find next spawn position just outside of player's field of view
        void PlaceFoeFreely(GameObject[] gameObjects, float minDistance = 5f, float maxDistance = 20f)
        {
            const float overlapSphereRadius = 0.65f;
            const float separationDistance = 1.25f;
            const float maxFloorDistance = 4f;

            // Must have received a valid array
            if (gameObjects == null || gameObjects.Length == 0)
                return;

            // Skip this foe if destroyed (e.g. player left building where pending)
            if (!gameObjects[pendingFoesSpawned])
            {
                pendingFoesSpawned++;
                return;
            }

            // Set parent if none specified already
            if (!gameObjects[pendingFoesSpawned].transform.parent)
                gameObjects[pendingFoesSpawned].transform.parent = GameObjectHelper.GetBestParent();

            // Get roation of spawn ray
            Quaternion rotation;
            if (LineOfSightCheck)
            {
                // Try to spawn outside of player's field of view
                float directionAngle = GameManager.Instance.MainCamera.fieldOfView;
                directionAngle += UnityEngine.Random.Range(0f, 4f);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    rotation = Quaternion.Euler(0, -directionAngle, 0);
                else
                    rotation = Quaternion.Euler(0, directionAngle, 0);
            }
            else
            {
                // Don't care about player's field of view (e.g. at rest)
                rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            }

            // Get direction vector and create a new ray
            Vector3 angle = (rotation * Vector3.forward).normalized;
            Vector3 spawnDirection = GameManager.Instance.PlayerObject.transform.TransformDirection(angle).normalized;
            Ray ray = new Ray(GameManager.Instance.PlayerObject.transform.position, spawnDirection);

            // Check for a hit
            Vector3 currentPoint;
            RaycastHit initialHit;
            if (Physics.Raycast(ray, out initialHit, maxDistance))
            {
                float cos_normal = Vector3.Dot(-spawnDirection, initialHit.normal.normalized);
                if (cos_normal < 1e-6)
                    return;
                float separationForward = separationDistance / cos_normal;

                // Must be greater than minDistance
                float distanceSlack = initialHit.distance - separationForward - minDistance;
                if (distanceSlack < 0f)
                    return;

                // Separate out from hit point
                float extraDistance = UnityEngine.Random.Range(0f, Mathf.Min(2f, distanceSlack));
                currentPoint = initialHit.point - spawnDirection * (separationForward + extraDistance);
            }
            else
            {
                // Player might be in an open area (e.g. outdoors) pick a random point along spawn direction
                currentPoint = GameManager.Instance.PlayerObject.transform.position + spawnDirection * UnityEngine.Random.Range(minDistance, maxDistance);
            }

            // Must be able to find a surface below
            RaycastHit floorHit;
            ray = new Ray(currentPoint, Vector3.down);
            if (!Physics.Raycast(ray, out floorHit, maxFloorDistance))
                return;

            // Ensure this is open space
            Vector3 testPoint = floorHit.point + Vector3.up * separationDistance;
            Collider[] colliders = Physics.OverlapSphere(testPoint, overlapSphereRadius);
            if (colliders.Length > 0)
                return;

            // This looks like a good spawn position
            pendingFoeGameObjects[pendingFoesSpawned].transform.position = testPoint;
            FinalizeFoe(pendingFoeGameObjects[pendingFoesSpawned]);
            gameObjects[pendingFoesSpawned].transform.LookAt(GameManager.Instance.PlayerObject.transform.position);

            // Increment count
            pendingFoesSpawned++;
        }

        // Fine tunes foe position slightly based on mobility and enables GameObject
        void FinalizeFoe(GameObject go)
        {
            DaggerfallMobileUnit mobileUnit = go.GetComponentInChildren<DaggerfallMobileUnit>();
            if (mobileUnit)
            {
                // Align ground creatures on surface, raise flying creatures slightly into air
                if (mobileUnit.Summary.Enemy.Behaviour != MobileBehaviour.Flying)
                    GameObjectHelper.AlignControllerToGround(go.GetComponent<CharacterController>());
                else
                    go.transform.localPosition += Vector3.up * 1.5f;
            }
            else
            {
                // Just align to ground
                GameObjectHelper.AlignControllerToGround(go.GetComponent<CharacterController>());
            }

            go.SetActive(true);
        }

        // Destroy other foes if replaced during spawn
        void DestroyOldFoeGameObjects(GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0)
                return;

            foreach(GameObject go in gameObjects)
            {
                Destroy(go);
            }
        }

        #endregion
    }
}