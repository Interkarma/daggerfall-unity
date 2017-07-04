// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;

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
        const float halfTile = (CityNavigation.DaggerfallUnitsPerTile * 0.5f) * MeshReader.GlobalScale;

        // Mobile states
        MobileStates currentMobileState = MobileStates.SeekingTile;
        MobileStates lastMobileState = MobileStates.SeekingTile;

        // Navigation settings
        bool triedPlayerLocation = false;
        MobileDirection currentDirection = MobileDirection.Random;
        DFPosition currentNavPosition = new DFPosition(-1, -1);
        DFPosition targetNavPosition;
        DFPosition targetWorldPosition;
        Vector3 targetScenePosition;
        float distanceToTarget;
        float distanceToPlayer;
        int seekCount;

        // References
        DaggerfallMobilePerson mobileBillboard;

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
        /// Gets the number of times this mobile has searched for a new tile.
        /// </summary>
        public int SeekCount
        {
            get { return seekCount; }
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

        #endregion

        #region Unity

        private void Start()
        {
            // Cache references
            mobileBillboard = GetComponentInChildren<DaggerfallMobilePerson>();

            // Need to repath if floating origin ticks while in range
            FloatingOrigin.OnPositionUpdate += FloatingOrigin_OnPositionUpdate;

            // Init mobile
            SetFacing(currentDirection);
        }

        private void Update()
        {
            // Do nothing if game paused
            if (GameManager.IsGamePaused)
                return;

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
            //bool playerStandingStill = GameManager.Instance.PlayerMotor.IsStandingStill;
            //if (!playerStandingStill && mobileBillboard.IsIdle)
            //{
            //    // Switch animation state back to moving
            //    mobileBillboard.IsIdle = false;
            //    currentMobileState = MobileStates.MovingForward;
            //}
            //else if (playerStandingStill && !mobileBillboard.IsIdle && distanceToPlayer < idleDistance)
            //{
            //    // Switch animation state to idle
            //    mobileBillboard.IsIdle = true;
            //    currentMobileState = MobileStates.Idle;
            //}

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
            // Unsubscribe event on destroy
            FloatingOrigin.OnPositionUpdate -= FloatingOrigin_OnPositionUpdate;
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
            seekCount++;

            // Ensure navgrid position is set
            InitNavPosition(currentNavPosition);

            // Try to keep moving in a straight line if possible
            if (GetNextNavPositionWeight(currentDirection) > 0)
            {
                SetTargetPosition();
                return;
            }

            // Turn in a random direction
            SetFacing(MobileDirection.Random);
        }

        void MovingForward()
        {
            // Push mobile towards target position
            Vector3 direction = Vector3.Normalize(targetScenePosition - transform.position);
            transform.position += (direction * movementSpeed) * Time.deltaTime;

            // Update distance to target
            distanceToTarget = Vector3.Distance(transform.position, targetScenePosition);

            // If distance below threshold find a new tile
            if (distanceToTarget < 0.1f)
            {
                currentNavPosition = targetNavPosition;
                ChangeState(MobileStates.SeekingTile);
            }
        }

        #endregion

        #region Private Methods

        void SetTargetPosition()
        {
            // Get target position on navgrid and in world
            targetNavPosition = GetNextNavPosition(currentDirection);
            targetWorldPosition = cityNavigation.NavGridToWorldPosition(targetNavPosition);

            // Get the target position in scene
            targetScenePosition = cityNavigation.WorldToScenePosition(targetWorldPosition);
            distanceToTarget = 0;

            // Navgrid > world > scene conversion results in mobile being aligned exactly on edge of tile in scene
            // Move the mobile transform a half-tile into centre so it appears to be properly aligned
            targetScenePosition.x += halfTile;
            targetScenePosition.z += halfTile;

            // Target point will be at ground level (roughly waist-level for mobile), so adjust up by half mobile height
            targetScenePosition.y += halfMobileHeight;

            // Change state to moving forwards
            ChangeState(MobileStates.MovingForward);
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

        int GetNextNavPositionWeight(MobileDirection direction)
        {
            // Must have a current position on navgrid
            if (!cityNavigation || currentNavPosition.X == -1 || currentNavPosition.Y == -1)
                return 0;

            return cityNavigation.GetNavGridWeightLocal(GetNextNavPosition(direction));
        }

        #endregion

        #region Event Handlers

        // Handle floating origin position update
        // Mobiles will need to repath their next scene position
        private void FloatingOrigin_OnPositionUpdate(Vector3 offset)
        {
        }

        #endregion
    }
}