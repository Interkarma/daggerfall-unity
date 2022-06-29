// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Linq;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Drives mobile NPCs around town exteriors using the local navgrid.
    /// </summary>
    public class MobilePersonMotor : MonoBehaviour
    {
        #region Fields

        public CityNavigation cityNavigation;

        // Constants
        const float idleDistance = 2.5f;
        const float movementSpeed = 1.3f;
        const float halfMobileHeight = 1.0f;
        const float tileDowngradeChance = 0.20f;
        const float randomChangeChance = 0.025f;

        // Mobile states
        MobileStates currentMobileState = MobileStates.SeekingTile;
        MobileStates lastMobileState = MobileStates.SeekingTile;

        // Navigation settings
        int[] localWeights = new int[4];
        bool triedPlayerLocation = false;
        MobileDirection currentDirection = MobileDirection.Random;
        DFPosition currentNavPosition = new DFPosition(-1, -1);
        DFPosition targetNavPosition = new DFPosition(-1, -1);
        DFPosition targetWorldPosition = new DFPosition(-1, -1);
        Vector3 targetScenePosition = Vector3.zero;
        float distanceToTarget;
        float distanceToPlayer;
        int seekCount;
        int moveCount;

        // References
        DaggerfallEntityBehaviour entityBehaviour;
        MobilePersonAsset mobileAsset;
        public MobilePersonAsset MobileAsset
        {
            get { return mobileAsset; }
            set { mobileAsset = value; }
        }

        #endregion

        #region Structs & Enums

        /// <summary>
        /// The direction mobile is currently facing and moving.
        /// Might be expanded to 8 directions in future.
        /// </summary>
        public enum MobileDirection
        {
            Random = -1,
            North = 0,      //  0, -1
            South = 1,      //  0,  1
            East = 2,       //  1,  0
            West = 3,       // -1,  0
        }

        /// <summary>
        /// States the mobile can be shifted into.
        /// </summary>
        public enum MobileStates
        {
            SeekingTile,
            MovingForward,
            Idle,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets current mobile facing and direction of movement.
        /// </summary>
        public MobileDirection CurrentDirection
        {
            get { return currentDirection; }
        }

        /// <summary>
        /// Gets target scene position.
        /// If this is Vector3.zero then mobile is not properly on grid.
        /// </summary>
        public Vector3 TargetScenePosition
        {
            get { return targetScenePosition; }
        }

        /// <summary>
        /// Gets consecutive number of times this mobile has searched for a new tile.
        /// This count is reset every time mobile moves to a new tile.
        /// </summary>
        public int SeekCount
        {
            get { return seekCount; }
        }

        /// <summary>
        /// Gets total number of times this mobile has moved to a new time.
        /// This count is reset when mobile is recycled.
        /// </summary>
        public int MoveCount
        {
            get { return moveCount; }
        }

        /// <summary>
        /// Gets current mobile state.
        /// </summary>
        public MobileStates CurrentState
        {
            get { return currentMobileState; }
        }

        /// <summary>
        /// Gets last mobile state.
        /// </summary>
        public MobileStates LastState
        {
            get { return lastMobileState; }
        }

        /// <summary>
        /// Gets distance remaining to target in scene.
        /// </summary>
        public float DistanceToTarget
        {
            get { return distanceToTarget; }
        }

        /// <summary>
        /// Gets last observed distance between this mobile and player.
        /// </summary>
        public float DistanceToPlayer
        {
            get { return distanceToPlayer; }
        }

        #endregion

        #region Unity

        private void Awake()
        {
            // Cache references
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            mobileAsset = FindMobilePersonAsset();

            // Need to repath if floating origin ticks while in range
            FloatingOrigin.OnPositionUpdate += FloatingOrigin_OnPositionUpdate;
        }

        private void Start()
        {
            // Init mobile
            SetFacing(currentDirection);
        }

        private void Update()
        {
            // Do nothing if game paused
            if (GameManager.IsGamePaused)
                return;

            // Do nothing if paralyzed
            if (entityBehaviour.Entity.IsParalyzed)
            {
                mobileAsset.IsIdle = false;
                return;
            }

            // Must have a navgrid assigned
            if (!cityNavigation)
            {
                // Try to get navgrid from current player location
                // This is for mobiles dropped directly into world from editor
                // May be removed once fully runtime as intended
                if (!triedPlayerLocation)
                {
                    DaggerfallLocation playerLocation = GameManager.Instance.StreamingWorld.CurrentPlayerLocationObject;
                    if (playerLocation)
                    {
                        cityNavigation = playerLocation.GetComponent<CityNavigation>();
                        transform.parent = playerLocation.transform;
                        ChangeState(MobileStates.SeekingTile);
                    }
                    triedPlayerLocation = true;
                }

                return;
            }

            // Go idle if near player
            distanceToPlayer = GameManager.Instance.PlayerMotor.DistanceToPlayer(transform.position);
            bool withinIdleDistance = (distanceToPlayer < idleDistance);
            bool playerStandingStill = GameManager.Instance.PlayerMotor.IsStandingStill;
            bool sheathed = GameManager.Instance.WeaponManager.Sheathed;
            bool invisible = GameManager.Instance.PlayerEntity.IsInvisible;
            bool inBeastForm = GameManager.Instance.PlayerEntity.IsInBeastForm;

            bool wantsToStop = playerStandingStill && withinIdleDistance && sheathed && !invisible && !inBeastForm;

            // greatly reduce # of calls to AreEnemiesNearby() by short-circuit evaluation
            if (wantsToStop && !GameManager.Instance.AreEnemiesNearby())
                wantsToStop = true;
            else
                wantsToStop = false;

            if (!wantsToStop && mobileAsset.IsIdle)
            {
                // Switch animation state back to moving
                mobileAsset.IsIdle = false;
                currentMobileState = MobileStates.MovingForward;
            }
            else if (wantsToStop && !mobileAsset.IsIdle)
            {
                // Switch animation state to idle
                mobileAsset.IsIdle = true;
                currentMobileState = MobileStates.Idle;
            }

            // Update based on current state
            switch (currentMobileState)
            {
                case MobileStates.SeekingTile:
                    SeekingTile();
                    break;
                case MobileStates.MovingForward:
                    MovingForward();
                    break;
                case MobileStates.Idle:
                    // Do nothing for now
                    break;
            }
        }

        private void OnDestroy()
        {
            // Clean up
            ClearMotor();
            FloatingOrigin.OnPositionUpdate -= FloatingOrigin_OnPositionUpdate;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise motor when activating.
        /// </summary>
        public void InitMotor()
        {
            seekCount = 0;
            moveCount = 0;
            SetFacing(MobileDirection.Random);
            currentNavPosition = new DFPosition(-1, -1);
            targetNavPosition = new DFPosition(-1, -1);
            targetScenePosition = transform.position;
            currentMobileState = MobileStates.SeekingTile;
        }

        /// <summary>
        /// Clear motor when deactivating.
        /// </summary>
        public void ClearMotor()
        {
            cityNavigation.ClearFlags(targetNavPosition, CityNavigation.TileFlags.Occupied);
        }

        #endregion

        #region State Updates

        void ChangeState(MobileStates newState)
        {
            lastMobileState = currentMobileState;
            currentMobileState = newState;
        }

        void SeekingTile()
        {
            // Population manager will recycle this mobile if it can't find a target after several attempts
            seekCount++;

            // Ensure mobile is on grid
            InitNavPosition(currentNavPosition);

            // Get surrounding weights
            localWeights[(int)MobileDirection.North] = GetNextNavPositionWeight(MobileDirection.North);
            localWeights[(int)MobileDirection.South] = GetNextNavPositionWeight(MobileDirection.South);
            localWeights[(int)MobileDirection.East] = GetNextNavPositionWeight(MobileDirection.East);
            localWeights[(int)MobileDirection.West] = GetNextNavPositionWeight(MobileDirection.West);

            // Get current and target weights
            int currentWeight = GetCurrentNavPositionWeight();
            int targetWeight = IsDirectionClear(currentDirection) ? localWeights[(int)currentDirection] : 0;

            // A small chance to randomly change direction - this keeps the movement shuffled
            if (Random.Range(0f, 1f) < randomChangeChance)
                targetWeight = 0;

            // Always change direction if target weight is zero - weight of target doesn't matter
            if (targetWeight == 0)
            {
                // Try to move in any valid random direction
                int randomDirection = Random.Range(0, localWeights.Length);
                if (localWeights[randomDirection] == 0 || !IsDirectionClear((MobileDirection)randomDirection))
                    return;

                SetFacing((MobileDirection)randomDirection);
                SetTargetPosition();
                return;
            }

            // High chance to change direction if target weight lower than current weight
            // This will cause mobiles to generally follow roads and other nice surfaces
            if (targetWeight < currentWeight && Random.Range(0f, 1f) > tileDowngradeChance)
            {
                // Evaluate in random order so if multiple equal "bests" are found we go a random direction
                int bestWeight = targetWeight;
                MobileDirection bestDirection = currentDirection;
                System.Random rand = new System.Random();
                foreach (int i in Enumerable.Range(0, 3).OrderBy(x => rand.Next(3)))
                {
                    if (localWeights[i] > bestWeight && IsDirectionClear((MobileDirection)i))
                    {
                        bestWeight = localWeights[i];
                        bestDirection = (MobileDirection)i;
                    }
                }

                // Start heading in new best direction
                SetFacing(bestDirection);
                currentDirection = bestDirection;
                SetTargetPosition();
                return;
            }

            // Otherwise keep marching in current direction
            SetTargetPosition();
        }

        void MovingForward()
        {
            // Push mobile towards target position
            Vector3 direction = Vector3.Normalize(targetScenePosition - transform.position);
            transform.position += (direction * movementSpeed) * Time.deltaTime;

            // Update distance to target
            distanceToTarget = Vector3.Distance(transform.position, targetScenePosition);

            // If distance below threshold start seeking a new tile
            if (distanceToTarget < 0.1f)
            {
                currentNavPosition = targetNavPosition;
                ChangeState(MobileStates.SeekingTile);
                moveCount++;
            }
        }

        #endregion

        #region Private Methods

        private MobilePersonAsset FindMobilePersonAsset()
        {
            var mobilePersonAsset = GetComponentInChildren<MobilePersonAsset>();

            GameObject customMobilePersonAssetGo;
            if (ModManager.Instance && ModManager.Instance.TryGetAsset<GameObject>("MobilePersonAsset", true, out customMobilePersonAssetGo))
            {
                var customMobilePersonAsset = customMobilePersonAssetGo.GetComponent<MobilePersonAsset>();
                if (customMobilePersonAsset)
                {
                    mobilePersonAsset.gameObject.SetActive(false);
                    customMobilePersonAssetGo.transform.SetParent(gameObject.transform);
                    mobilePersonAsset = customMobilePersonAsset;
                    mobilePersonAsset.Trigger = GetComponent<CapsuleCollider>();
                }
                else
                {
                    Debug.LogError("Failed to retrieve MobilePersonAsset component from GameObject.");
                }
            }

            return mobilePersonAsset;
        }

        // Check for geometry in the way (stairs,...)
        bool IsDirectionClear(MobileDirection direction)
        {
            Vector3 tempTargetScenePosition = cityNavigation.WorldToScenePosition(cityNavigation.NavGridToWorldPosition(GetNextNavPosition(direction)));
            // Aim low to better detect stairs
            tempTargetScenePosition.y += 0.1f;
            Ray ray = new Ray(transform.position, tempTargetScenePosition - transform.position);
            bool collision = Physics.Raycast(transform.position, tempTargetScenePosition - transform.position, Vector3.Distance(transform.position, tempTargetScenePosition), ~mobileAsset.GetLayerMask());
            // Debug.DrawRay(transform.position, tempTargetScenePosition - transform.position, collision ? Color.red : Color.green, 1f);
            return !collision;
        }

        void SetTargetPosition()
        {
            // Get target position on navgrid and in world
            targetNavPosition = GetNextNavPosition(currentDirection);
            targetWorldPosition = cityNavigation.NavGridToWorldPosition(targetNavPosition);

            // Get the target position in scene
            targetScenePosition = cityNavigation.WorldToScenePosition(targetWorldPosition);
            distanceToTarget = 0;

            // Target point will be at ground level (roughly waist-level for mobile), so adjust up by half mobile height
            targetScenePosition.y += halfMobileHeight;

            // Mobile now owns target tile and can release current tile
            cityNavigation.ClearFlags(currentNavPosition, CityNavigation.TileFlags.Occupied);
            cityNavigation.SetFlags(targetNavPosition, CityNavigation.TileFlags.Occupied);

            // Change state to moving forwards
            ChangeState(MobileStates.MovingForward);
            seekCount = 0;
        }

        void SetFacing(MobileDirection facing)
        {
            // Select a random facing direction
            if (facing == MobileDirection.Random)
            {
                facing = (MobileDirection)Random.Range((int)MobileDirection.North, (int)MobileDirection.West + 1);
            }

            // Set mobile transform facing in this direction
            switch(facing)
            {
                case MobileDirection.North:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case MobileDirection.South:
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case MobileDirection.East:
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case MobileDirection.West:
                    transform.rotation = Quaternion.Euler(0, -90, 0);
                    break;
            }

            // Store new facing
            currentDirection = facing;
        }

        void InitNavPosition(DFPosition navPosition, bool forceRefresh = false)
        {
            if (!cityNavigation)
                return;

            // Get mobile "back on the grid" if not set
            // The mobile must already be placed over a valid tile in world
            if (navPosition.X == -1 || navPosition.Y == -1 || forceRefresh)
            {
                DFPosition worldPosition = cityNavigation.SceneToWorldPosition(transform.position);
                currentNavPosition = cityNavigation.WorldToNavGridPosition(worldPosition);
            }
        }

        DFPosition GetNextNavPosition(MobileDirection direction)
        {
            DFPosition nextPosition = new DFPosition(0, 0);
            switch (direction)
            {
                case MobileDirection.North:
                    nextPosition = new DFPosition(currentNavPosition.X, currentNavPosition.Y - 1);
                    break;
                case MobileDirection.South:
                    nextPosition = new DFPosition(currentNavPosition.X, currentNavPosition.Y + 1);
                    break;
                case MobileDirection.East:
                    nextPosition = new DFPosition(currentNavPosition.X + 1, currentNavPosition.Y);
                    break;
                case MobileDirection.West:
                    nextPosition = new DFPosition(currentNavPosition.X - 1, currentNavPosition.Y);
                    break;
            }

            return nextPosition;
        }

        int GetCurrentNavPositionWeight()
        {
            // Must have a current position on navgrid
            if (!cityNavigation || currentNavPosition.X == -1 || currentNavPosition.Y == -1)
                return 0;

            return cityNavigation.GetNavGridWeightLocal(currentNavPosition);
        }

        int GetNextNavPositionWeight(MobileDirection direction)
        {
            // Must have a current position on navgrid
            if (!cityNavigation || currentNavPosition.X == -1 || currentNavPosition.Y == -1)
                return 0;

            // Get next nav position and regard occupied tiles as weight 0
            DFPosition nextPosition = GetNextNavPosition(direction);
            if (cityNavigation.HasFlags(nextPosition, CityNavigation.TileFlags.Occupied))
                return 0;

            return cityNavigation.GetNavGridWeightLocal(nextPosition);
        }

        #endregion

        #region Event Handlers

        // Handle floating origin position update
        // Mobiles will need to repath their next scene position
        private void FloatingOrigin_OnPositionUpdate(Vector3 offset)
        {
            // Update target position after floating origin change
            SetTargetPosition();
        }

        #endregion
    }
}