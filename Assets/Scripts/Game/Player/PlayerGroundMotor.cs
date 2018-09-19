using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    //[RequireComponent(typeof(PlayerMotor))]
    public class PlayerGroundMotor : MonoBehaviour
    {
        //private PlayerMotor playerMotor;
        private CharacterController controller;
        // Moving platform support
        Transform activePlatform;
        Vector3 activeLocalPlatformPoint;
        Vector3 activeGlobalPlatformPoint;
        //Vector3 lastPlatformVelocity;
        Quaternion activeLocalPlatformRotation;
        Quaternion activeGlobalPlatformRotation;

        public Transform ActivePlatform
        {
            get { return activePlatform; }
            set { activePlatform = value; }
        }

        private void Start()
        {
            //playerMotor = GameManager.Instance.PlayerMotor;
            controller = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Reset moving platform logic to new player position
        /// </summary>
        public void ClearActivePlatform()
        {
            activePlatform = null;
        }

        private void Update() { }

        /// <summary>
        /// Moves the player on solid ground & floating platforms.
        /// </summary>
        /// <param name="moveDirection">the vector the player should move to</param>
        /// <param name="collisionFlags"></param>
        /// <param name="grounded">Is the player grounded?</param>
        public void MoveOnGround(Vector3 moveDirection, ref CollisionFlags collisionFlags, ref bool grounded)
        {
            // Moving platform support
            if (activePlatform != null)
            {
                Vector3 newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);
                Vector3 adhesionDirection = (newGlobalPlatformPoint - activeGlobalPlatformPoint);
                // Platform movement of player is performed here
                if (adhesionDirection != Vector3.zero)
                    moveDirection += adhesionDirection;
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

            // Move the controller, and set grounded true or false depending on whether we're standing on something
            collisionFlags = controller.Move(moveDirection * Time.deltaTime);

            grounded = (collisionFlags & CollisionFlags.Below) != 0;

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
    }
}