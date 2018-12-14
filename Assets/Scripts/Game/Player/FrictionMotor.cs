using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(CharacterController))]
    public class FrictionMotor : MonoBehaviour
    {
        // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
        public bool slideWhenOverSlopeLimit = false;

        // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
        public bool slideOnTaggedObjects = false;

        public float slideSpeed = 12.0f;

        private PlayerMotor playerMotor;
        private Transform myTransform;
        private RaycastHit hit;
        private CharacterController controller;
        private Vector3 contactPoint;
        private float rayDistance;
        private float slideLimit;
        private bool playerControl = false;
        bool sliding;
        public Vector3 ContactPoint
        {
            get { return contactPoint; }
            set { contactPoint = value; }
        }
        public bool PlayerControl
        {
            get
            {
                return playerControl;
            }
        }
        private void Start()
        {
            controller = GetComponent<CharacterController>();
            playerMotor = GameManager.Instance.PlayerMotor;
            myTransform = playerMotor.transform;
            slideLimit = controller.slopeLimit - .1f;
            rayDistance = controller.height * .5f + controller.radius;
        }

        public void GroundedMovement(ref Vector3 moveDirection)
        {
            SetSliding();

            // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
            if ((sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide"))
            {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                moveDirection *= slideSpeed;
                playerControl = false;
            }
            // Otherwise recalculate moveDirection directly from axes
            else
            {
                float inputX = InputManager.Instance.Horizontal;
                float inputY = InputManager.Instance.Vertical;

                // Cancel all movement input if player is paralyzed
                // Player should still be able to fall or move with platforms
                if (GameManager.Instance.PlayerEntity.IsParalyzed)
                {
                    inputX = 0;
                    inputY = 0;
                }

                float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && playerMotor.limitDiagonalSpeed) ? .7071f : 1.0f;
                moveDirection = new Vector3(inputX * inputModifyFactor, 0, inputY * inputModifyFactor);
                moveDirection = myTransform.TransformDirection(moveDirection) * playerMotor.Speed;
                playerControl = true;
            }
        }

        private void SetSliding()
        {
            sliding = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else
            {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
        }
    }
}