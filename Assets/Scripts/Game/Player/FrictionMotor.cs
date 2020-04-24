using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;
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
        private PlayerHeightChanger heightChanger;
        private Transform myTransform;
        private RaycastHit hit;
        private CharacterController controller;
        private Vector3 contactPoint;
        private float rayDistance;
        private float slideLimit;
        private bool playerControl = false;
        bool sliding;

        Vector3 lastMovePosition = Vector3.zero;
        int stuckFrameCount = 0;

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
            heightChanger = GetComponent<PlayerHeightChanger>();
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

                // Somewhat experimental handling for automatically unsticking player controller in very specific cases
                if (!GameManager.Instance.PlayerEntity.IsParalyzed)
                {
                    HeadDipHandling();
                    UnstickHandling();
                }
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

        void UnstickHandling()
        {
            // Stuck check - is player trying to move forwards/backwards but cannot?
            // This could be because something is in the way, like a wall, or it could be for one of the following reasons:
            //  -A: Sticky geometry such as single-sided polygons poking through floor in cave blocks which sticks the Unity character controller
            //  -B: Restrictive geometry like doors positioned at bottom of stairs and ramps without enough room for capsule to pass
            // If player is stuck and there is nothing in front of them, then teleport player forwards in small steps until clear
            // Notes:
            //  -A better/smoother solution is preferred for case B, as this geometry is so enclosed that unstick is still difficult to aim for clear space
            //  -Player will experience a brief moment of uneven movement while being teleported past sticky geometry - fortunately sticky spots are fairly rare
            //  -Tolerances should prevent false positives, but it's still possible - might require tuning/reworking later
            //  -Works best when player is standing as spherecast test has more clearance
            //  -Enemies will still become stuck as their motor does not have this handling
            const float stuckMovementThreshold = 0.07f;
            const int stuckFrameThreshold = 3;
            bool tryingToMoveForwards = InputManager.Instance.HasAction(InputManager.Actions.MoveForwards);
            bool tryingToMoveBackwards = InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards);
            if ((tryingToMoveForwards || tryingToMoveBackwards) &&
                // Use a sqrmagnitude movement threshold check to see if player is stuck
                // This is fast and overcomes precision issues with a simple position check
                // Player must be stuck for multiple frames before unstuck handler will attempt to resolve
                (lastMovePosition - myTransform.position).sqrMagnitude < Mathf.Pow(stuckMovementThreshold, 2))
            {
                stuckFrameCount++;
                if (stuckFrameCount > stuckFrameThreshold)
                {
                    //Debug.LogFormat("Stuck for {0} frames", stuckFrameCount);
                    TryUnsticking(tryingToMoveForwards);
                }
            }
            else
            {
                // Reset during normal movement
                lastMovePosition = myTransform.position;
                stuckFrameCount = 0;
            }
        }

        private void TryUnsticking(bool tryingToMoveForwards)
        {
            // Attempt resolution by first checking if nothing in the way using a capsulecast
            // Then teleport player forwards until stuck test is cleared by normal movement

            // size of the step
            const float stuckSampleForward = 0.5f;
            const float stuckSampleUpward = 0.35f;

            Vector3 originBottom = myTransform.position - myTransform.up * (controller.height / 2 - controller.radius);
            Vector3 originTop = myTransform.position + myTransform.up * (controller.height / 2 - controller.radius);

            Vector3 sampleDirection = tryingToMoveForwards ? myTransform.forward : -myTransform.forward;
            Vector3 displacementUp = myTransform.up * stuckSampleUpward;
            // Debug.DrawRay(originBottom, displacementUp, Color.green, 2f);
            // Debug.DrawRay(originTop, displacementUp, Color.blue, 2f);
            if (Physics.CapsuleCast(originTop, originBottom, controller.radius, displacementUp, displacementUp.magnitude))
                return;

            originBottom += displacementUp;
            originTop += displacementUp;
            Vector3 displacementForward = sampleDirection * stuckSampleForward;
            // Debug.DrawRay(originBottom, displacementForward, Color.green, 2f);
            // Debug.DrawRay(originTop, displacementForward, Color.blue, 2f);
            if (Physics.CapsuleCast(originTop, originBottom, controller.radius, displacementForward, displacementForward.magnitude))
                return;

            originBottom += displacementForward;
            originTop += displacementForward;
            Vector3 displacementDownward = -myTransform.up * stuckSampleUpward;
            // Debug.DrawRay(originBottom, displacementDownward, Color.green, 2f);
            // Debug.DrawRay(originTop, displacementDownward, Color.blue, 2f);
            //Debug.LogFormat("Trying to resolve stuck for {0} frames", stuckFrameCount);
            RaycastHit hit;
            if (!Physics.CapsuleCast(originTop, originBottom, controller.radius, displacementDownward, out hit, displacementDownward.magnitude))
            {
                myTransform.position += displacementForward;
            }
            else
            {
                // Player landed on something during the last part of the step
                myTransform.position += displacementUp + displacementForward - myTransform.up * hit.distance;
            }
        }

        // Preferential resolution of case B in UnstickHandling()
        // Smoothly dips and undips height of player capsule, like a very tall person ducking through a low doorway
        void HeadDipHandling()
        {
            const float raySampleDistance = 0.5f;
            const float clearanceAdjustment = -0.28f;

            if (!heightChanger || playerMotor.IsCrouching)
                return;

            // Sample forward from very top of player's head and from eye level
            Ray headRay = new Ray(myTransform.position + new Vector3(0, heightChanger.FixedControllerStandingHeight / 2 + 0.25f, 0), myTransform.forward);
            Ray eyeRay = new Ray(GameManager.Instance.MainCamera.transform.position, myTransform.forward);
            RaycastHit headHit;
            bool headRayHit = Physics.Raycast(headRay, out headHit, raySampleDistance);
            bool eyeRayHit = Physics.Raycast(eyeRay, raySampleDistance);

            //Debug.LogFormat("Ray contact: HeadRay: {0}, EyeRay {1}", headRayHit, eyeRayHit);

            // If top of head hits something but eyes are clear, try dipping controller height
            if (headRayHit && !eyeRayHit && GameObjectHelper.IsStaticGeometry(headHit.transform.gameObject))
            {
                // Dip controller height by clearance amount
                heightChanger.StandingHeightAdjustment = clearanceAdjustment;

                //Debug.Log("Dipping controller");
            }
            else
            {
                // Undip player once head/eye test is clear
                // Technically the player will stand up again within a frame or two
                // But in practice it is only required to clear the initial obstacle and player will fit through
                // Player might experience a short "bounce" if they approach problem geometry very slowly
                // Adding extra logic to maintain dip while something is above player's head adds complexity and can result in pronounced "bouncing"
                // After testing several approaches, the most simple one still yielded the best results
                heightChanger.StandingHeightAdjustment = 0;

                //Debug.Log("Undipping controller");
            }
        }
    }
}