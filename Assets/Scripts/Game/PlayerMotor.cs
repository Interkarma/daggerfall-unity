using UnityEngine;
using System;
using System.Collections;

namespace DaggerfallWorkshop.Game
{
    //
    // Using FPSWalkerEnhanced from below community wiki entry.
    // http://wiki.unity3d.com/index.php?title=FPSWalkerEnhanced
    //
    // Extended for moving platforms, and ceiling hits, and other tweaks.
    //
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : MonoBehaviour
    {
        // Moving platform support
        Transform activePlatform;
        Vector3 activeLocalPlatformPoint;
        Vector3 activeGlobalPlatformPoint;
        //Vector3 lastPlatformVelocity;
        Quaternion activeLocalPlatformRotation;
        Quaternion activeGlobalPlatformRotation;

        public float walkSpeed = 6.0f;

        public float runSpeed = 11.0f;

        public float standingHeight = 1.78f;
        public float crouchingHeight = 0.45f;
        public float crouchingSpeedDelta = 0.5f;
        public float crouchingJumpDelta = 0.8f;
        bool isCrouching = false;
        bool wasCrouching = false;

        // If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
        public bool limitDiagonalSpeed = true;

        // If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
        // There must be a button set up in the Input Manager called "Run"
        public bool toggleRun = false;

        public float jumpSpeed = 8.0f;
        public float gravity = 20.0f;

        // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
        public float fallingDamageThreshold = 10.0f;

        // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
        public bool slideWhenOverSlopeLimit = false;

        // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
        public bool slideOnTaggedObjects = false;

        public float slideSpeed = 12.0f;

        // If checked, then the player can change direction while in the air
        public bool airControl = false;

        // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
        public float antiBumpFactor = .75f;

        // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
        public int antiBunnyHopFactor = 1;

        // FixedUpdate is too choppy to give smooth camera movement. This handles a smooth following child transform.
        public Transform smoothFollower;                // The Transform that follows; will lerp to this Transform's position.
        public float smoothFollowerLerpSpeed = 25.0f;   // Multiplied by dt.
        Vector3 smoothFollowerPrevWorldPos;
        bool smoothFollowerReset = true;

        [HideInInspector, NonSerialized]
        public CharacterController controller;

        private Vector3 moveDirection = Vector3.zero;
        private bool grounded = false;
        private Transform myTransform;
        private float speed;
        private RaycastHit hit;
        private float fallStartLevel;
        private bool falling;
        private float slideLimit;
        private float rayDistance;
        private Vector3 contactPoint;
        private bool playerControl = false;
        private int jumpTimer;
        private bool jumping = false;
        private bool standingStill = false;

        private bool cancelMovement = false;

        public bool IsGrounded
        {
            get { return grounded; }
        }

        public bool IsRunning
        {
            get { return (speed == runSpeed); }
        }

        public bool IsStandingStill
        {
            get { return standingStill; }
        }

        public bool IsJumping
        {
            get { return jumping; }
        }

        public bool IsCrouching
        {
            get { return isCrouching; }
            set { isCrouching = value; }
        }

        public Transform ActivePlatform
        {
            get { return activePlatform; }
        }

        public Vector3 ContactPoint
        {
            get { return contactPoint; }
        }

        /// <summary>
        /// Cancels all movement impulses next frame.
        /// Used to scrub movement impulse when player dies, opens inventory, or loads game.
        /// Flag will be lowered again after movement cleared.
        /// </summary>
        public bool CancelMovement
        {
            get { return cancelMovement; }
            set { cancelMovement = value; }
        }

        void Start()
        {
            controller = GetComponent<CharacterController>();
            myTransform = transform;
            speed = walkSpeed;
            rayDistance = controller.height * .5f + controller.radius;
            slideLimit = controller.slopeLimit - .1f;
            jumpTimer = antiBunnyHopFactor;
        }

        void FixedUpdate()
        {
            // Clear movement
            if (cancelMovement)
            {
                moveDirection = Vector3.zero;
                cancelMovement = false;
                ClearActivePlatform();
                ClearFallingDamage();
                return;
            }

            //float inputX = Input.GetAxis("Horizontal");
            //float inputY = Input.GetAxis("Vertical");
            float inputX = InputManager.Instance.Horizontal;
            float inputY = InputManager.Instance.Vertical;
            // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
            float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? .7071f : 1.0f;

            // Player assumed to be in movement for now
            standingStill = false;

            if (grounded)
            {
                // Set standing still while grounded flag
                // Casting moveDirection to a Vector2 so constant downward force of gravity not included in magnitude
                standingStill = (new Vector2(moveDirection.x, moveDirection.z).magnitude == 0);

                if (jumping)
                    jumping = false;
                bool sliding = false;
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

                // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
                if (falling)
                {
                    falling = false;
                    float fallDistance = fallStartLevel - myTransform.position.y;
                    if (fallDistance > fallingDamageThreshold)
                        FallingDamageAlert(fallDistance);
                    else if (fallDistance > fallingDamageThreshold / 2f)
                        BadFallDetected(fallDistance);
                    //if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                    //    FallingDamageAlert(fallDistance);
                }

                try
                {
                    // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
                    if (!toggleRun)
                        speed = Input.GetButton("Run") ? runSpeed : walkSpeed;
                }
                catch
                {
                    speed = runSpeed;
                }

                // Manage crouching speed
                if (isCrouching)
                    speed *= crouchingSpeedDelta;
                else
                    controller.height = standingHeight;

                // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
                if ((sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide"))
                {
                    Vector3 hitNormal = hit.normal;
                    moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                    Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                    moveDirection *= slideSpeed;
                    playerControl = false;
                }
                // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
                else
                {
                    moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                    moveDirection = myTransform.TransformDirection(moveDirection) * speed;
                    playerControl = true;
                }

                try
                {
                    // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
                    if (!InputManager.Instance.HasAction(InputManager.Actions.Jump))
                        jumpTimer++;
                    //if (!Input.GetButton("Jump"))
                    //    jumpTimer++;
                    else if (jumpTimer >= antiBunnyHopFactor)
                    {
                        moveDirection.y = jumpSpeed;
                        jumpTimer = 0;
                        jumping = true;

                        // Modify crouching jump speed
                        if (isCrouching)
                            moveDirection.y *= crouchingJumpDelta;
                    }
                }
                catch
                {
                }
            }
            else
            {
                // If we stepped over a cliff or something, set the height at which we started falling
                if (!falling)
                {
                    falling = true;
                    fallStartLevel = myTransform.position.y;
                }

                // If air control is allowed, check movement but don't touch the y component
                if (airControl && playerControl)
                {
                    moveDirection.x = inputX * speed * inputModifyFactor;
                    moveDirection.z = inputY * speed * inputModifyFactor;
                    moveDirection = myTransform.TransformDirection(moveDirection);
                }
            }

            // Apply gravity
            moveDirection.y -= gravity * Time.deltaTime;

            // If we hit something above us AND we are moving up, reverse vertical movement
            if ((controller.collisionFlags & CollisionFlags.Above) != 0)
            {
                if (moveDirection.y > 0)
                    moveDirection.y = -moveDirection.y;
            }

            // Moving platform support
            if (activePlatform != null)
            {
                var newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);
                var moveDistance = (newGlobalPlatformPoint - activeGlobalPlatformPoint);
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

            // Move the controller, and set grounded true or false depending on whether we're standing on something
            grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

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

        // Reset moving platform logic to new player position
        public void ClearActivePlatform()
        {
            activePlatform = null;
        }

        // Call this when floating origin ticks on Y
        // to ensure player doesn't die by jumping right at threshold
        public void AdjustFallStart(float y)
        {
            if (falling)
            {
                fallStartLevel += y;
            }
        }

        /// <summary>
        /// Attempts to find the ground position below player, even if player is jumping/falling
        /// </summary>
        /// <param name="distance">Distance to fire ray.</param>
        /// <returns>Hit point on surface below player, or player position if hit not found in distance.</returns>
        public Vector3 FindGroundPosition(float distance = 10)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out hit, distance))
                return hit.point;

            return transform.position;
        }

        // Snap player to ground
        public bool FixStanding(float extraHeight = 0, float extraDistance = 0)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + (Vector3.up * extraHeight), Vector3.down);
            if (Physics.Raycast(ray, out hit, (controller.height * 2) + extraHeight + extraDistance))
            {
                // Position player at hit position plus just over half controller height up
                transform.position = hit.point + Vector3.up * (controller.height * 0.6f);
                return true;
            }

            return false;
        }

        // Gets distance between position and player
        public float DistanceToPlayer(Vector3 position)
        {
            return Vector3.Distance(transform.position, position);
        }

        void Update()
        {
            try
            {
                // If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
                // FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
                if (toggleRun && grounded && InputManager.Instance.HasAction(InputManager.Actions.Run))
                    speed = (speed == walkSpeed ? runSpeed : walkSpeed);
                //if (toggleRun && grounded && Input.GetButtonDown("Run"))
                //    speed = (speed == walkSpeed ? runSpeed : walkSpeed);
            }
            catch
            {
                speed = runSpeed;
            }

            // Toggle crouching
            if (InputManager.Instance.ActionComplete(InputManager.Actions.Crouch))
                isCrouching = !isCrouching;

            // Manage crouching height
            if (isCrouching && !wasCrouching)
            {
                controller.height = crouchingHeight;
                Vector3 pos = controller.transform.position;
                pos.y -= (standingHeight - crouchingHeight) / 2.0f;
                controller.transform.position = pos;
                wasCrouching = isCrouching;
            }
            else if (!isCrouching && wasCrouching)
            {
                controller.height = standingHeight;
                Vector3 pos = controller.transform.position;
                pos.y += (standingHeight - crouchingHeight) / 2.0f;
                controller.transform.position = pos;
                wasCrouching = isCrouching;
            }

            if(smoothFollower != null)
            {
                float distanceMoved = Vector3.Distance(smoothFollowerPrevWorldPos, smoothFollower.position);        // Assuming the follower is a child of this motor transform we can get the distance travelled.
                float maxPossibleDistanceByMotorVelocity = controller.velocity.magnitude * 2.0f * Time.deltaTime;   // Theoretically the max distance the motor can carry the player with a generous margin.
                float speedThreshold = runSpeed * Time.deltaTime;                                                   // Without question any distance travelled less than the running speed is legal.

                // NOTE: Maybe the min distance should also include the height different between crouching / standing.
                if(distanceMoved > speedThreshold && distanceMoved > maxPossibleDistanceByMotorVelocity)
                {
                    smoothFollowerReset = true;
                }
            
                if(smoothFollowerReset) 
                {
                    smoothFollowerPrevWorldPos = transform.position;
                    smoothFollowerReset = false;
                }

                smoothFollower.position = Vector3.Lerp(smoothFollowerPrevWorldPos, transform.position, smoothFollowerLerpSpeed * Time.smoothDeltaTime);
                smoothFollowerPrevWorldPos = smoothFollower.position;
            }
        }

        // Store point that we're in contact with for use in FixedUpdate if needed
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            contactPoint = hit.point;

            // Don't consider enemies as moving platforms
            // Otherwise, if we're positioned just right on top of one, the player camera gets unstable
            // This still allows standing on enemies
            if (hit.collider.gameObject.GetComponent<DaggerfallEnemy>() != null)
                return;

            // Get active platform
            if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.5)
                activePlatform = hit.collider.transform;
        }

        // If falling damage occured, this is the place to do something about it. You can make the player
        // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
        void FallingDamageAlert(float fallDistance)
        {
            SendMessage("ApplyPlayerFallDamage", fallDistance, SendMessageOptions.DontRequireReceiver);
        }

        // This was a bad fall, but not enough to damage player.
        // Might want to play a sound or animation however.
        void BadFallDetected(float fallDistance)
        {
            SendMessage("HardFallAlert", fallDistance, SendMessageOptions.DontRequireReceiver);
        }

        public void ClearFallingDamage()
        {
            falling = false;
            fallStartLevel = transform.position.y;
        }

    }
}