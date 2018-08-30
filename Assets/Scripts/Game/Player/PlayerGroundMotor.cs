﻿using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DaggerfallWorkshop.Game
{
    //[RequireComponent(typeof(PlayerMotor))]
    public class PlayerGroundMotor : MonoBehaviour
    {
        private PlayerMotor playerMotor;
        private CharacterController controller;
        private ClimbingMotor climbingMotor;
        // Moving platform support
        Transform activePlatform;
        Vector3 activeLocalPlatformPoint;
        Vector3 activeGlobalPlatformPoint;
        //Vector3 lastPlatformVelocity;
        Quaternion activeLocalPlatformRotation;
        Quaternion activeGlobalPlatformRotation;
        public bool runClimbTimer { get; private set; }
        public float climbOffTimer { get; private set; }
        public const float climbOffTimerEnd = 0.5f;

        public Transform ActivePlatform
        {
            get { return activePlatform; }
            set { activePlatform = value; }
        }

        private void Start()
        {
            playerMotor = GameManager.Instance.PlayerMotor;
            controller = GetComponent<CharacterController>();
            climbingMotor = GetComponent<ClimbingMotor>();
        }
        /// <summary>
        /// Allow Timer to run to enable player to walk onto climbed ledge instead of falling
        /// </summary>
        public void EnableClimbTimer()
        {
            runClimbTimer = true;
        }

        /// <summary>
        /// Reset moving platform logic to new player position
        /// </summary>
        public void ClearActivePlatform()
        {
            activePlatform = null;
        }

        private void Update()
        {
            if (runClimbTimer)
            {
                climbOffTimer = Mathf.Min(climbOffTimer + Time.deltaTime, climbOffTimerEnd);

                // move the controller until end of timer or the player is on the ledge forcing them to step on ledge.
                MoveController(climbingMotor.ledgeDirection * 5f);
                if (climbOffTimer == climbOffTimerEnd || playerMotor.IsGrounded)
                    runClimbTimer = false;
            }
            else
            {
                climbOffTimer = 0;
            }
        }

        /// <summary>
        /// Moves the player on solid ground & floating platforms.
        /// </summary>
        /// <param name="moveDirection">the vector the player should move to</param>
        /// <param name="grounded">Is the player grounded?</param>
        public void MoveOnGround(Vector3 moveDirection)
        {
            // Moving platform support
            if (activePlatform != null)
            {
                var newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);
                var moveDistance = (newGlobalPlatformPoint - activeGlobalPlatformPoint);
                // Platform movement of player is performed here
                if (moveDistance != Vector3.zero)
                    controller.Move(moveDistance);
                //lastPlatformVelocity = (newGlobalPlatformPoint - activeGlobalPlatformPoint) / Time.deltaTime;

                // If you want to support moving platform rotation as well:
                var newGlobalPlatformRotation = activePlatform.rotation * activeLocalPlatformRotation;
                var rotationDiff = newGlobalPlatformRotation * Quaternion.Inverse(activeGlobalPlatformRotation);

                // Prevent rotation of the local up vector
                rotationDiff = Quaternion.FromToRotation(rotationDiff * transform.up, transform.up) * rotationDiff;

                transform.rotation = rotationDiff * transform.rotation;
            }
            //else
            //{
            //    lastPlatformVelocity = Vector3.zero;
            //}

            activePlatform = null;

            MoveController(moveDirection);

            // Moving platforms support
            if (activePlatform != null)
            {
                activeGlobalPlatformPoint = transform.position;
                activeLocalPlatformPoint = activePlatform.InverseTransformPoint(transform.position);

                // If you want to support moving platform rotation as well:
                activeGlobalPlatformRotation = transform.rotation;
                activeLocalPlatformRotation = Quaternion.Inverse(activePlatform.rotation) * transform.rotation;
            }
        }

        public void MoveController(Vector3 moveDirection)
        {
            // Move the controller, and set grounded true or false depending on whether we're standing on something
            controller.Move(moveDirection * Time.deltaTime);
            playerMotor.CollisionFlags = controller.collisionFlags;
            playerMotor.IsGrounded = (playerMotor.CollisionFlags & CollisionFlags.Below) != 0;
        }
    }
}