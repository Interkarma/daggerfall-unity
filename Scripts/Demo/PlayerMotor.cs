using UnityEngine;
using System;
using System.Collections;

namespace DaggerfallWorkshop.Demo
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

        public bool IsGrounded
        {
            get { return grounded; }
        }

        public bool IsRunning
        {
            get { return (speed == runSpeed); }
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
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");
            // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
            float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? .7071f : 1.0f;

            if (grounded)
            {
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
                    if (!Input.GetButton("Jump"))
                        jumpTimer++;
                    else if (jumpTimer >= antiBunnyHopFactor)
                    {
                        moveDirection.y = jumpSpeed;
                        jumpTimer = 0;
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

        void Update()
        {
            try
            {
                // If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
                // FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
                if (toggleRun && grounded && Input.GetButtonDown("Run"))
                    speed = (speed == walkSpeed ? runSpeed : walkSpeed);
            }
            catch
            {
                speed = runSpeed;
            }
        }

        // Store point that we're in contact with for use in FixedUpdate if needed
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            contactPoint = hit.point;

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
    }
}