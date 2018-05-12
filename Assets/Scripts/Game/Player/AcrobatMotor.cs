using UnityEngine;
using System;
using System.Collections;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(CharacterController))]
    public class AcrobatMotor : MonoBehaviour
    {
        private PlayerMotor playerMotor;
        private FrictionMotor frictionMotor;
        private Transform myTransform;
        public float jumpSpeed = 8.0f;
        public float gravity = 20.0f;
        public float crouchingJumpDelta = 0.8f;

        // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
        public float fallingDamageThreshold = 10.0f;

        // If checked, then the player can change direction while in the air
        public bool airControl = false;

        // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
        public int antiBunnyHopFactor = 1;
        private float fallStartLevel;
        private bool falling;
        private int jumpTimer;
        private bool jumping = false;
        public bool Jumping
        {
            get { return jumping; }
            set { jumping = value; }
        }
        public bool Falling
        {
            get { return falling; }
            set { falling = value; }
        }
        void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            frictionMotor = GetComponent<FrictionMotor>();
            myTransform = playerMotor.transform;
            jumpTimer = antiBunnyHopFactor;
        }

        void Update()
        {

        }

        /// <summary>
        /// Jump! But only if the jump button has been released and player has been grounded for a given number of frames
        /// </summary>
        /// <param name="moveDirection"></param>
        public void DoJump(ref Vector3 moveDirection)
        {
            // Cancel jump if player is paralyzed
            if (GameManager.Instance.PlayerEntity.IsParalyzed)
                return;

            try
            {
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
                    if (playerMotor.IsCrouching)
                        moveDirection.y *= crouchingJumpDelta;
                }
                else
                    jumping = false;
            }
            catch
            {
            }
        }
        /// <summary>
        /// If air control is allowed, check movement but don't touch the y component
        /// </summary>
        /// <param name="moveDirection">the Vector to adjust for movement</param>
        /// <param name="speed">The speed multiplier</param>
        public void CheckAirControl(ref Vector3 moveDirection, float speed)
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

            if (airControl && frictionMotor.PlayerControl)
            {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }
        }

        /// <summary>
        /// // If we stepped over a cliff or something, set the height at which we started falling
        /// </summary>
        public void CheckInitFall()
        {
            if (!falling)
            {
                falling = true;
                fallStartLevel = myTransform.position.y;
            }
        }
        public void ApplyGravity(ref Vector3 moveDirection)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        } 
                       
        /// <summary>
        /// If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
        /// </summary>
        public void CheckFallingDamage()
        {
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
        }

        /// <summary>
        /// Call this when floating origin ticks on Y to ensure player doesn't die by jumping right at threshold
        /// </summary>
        /// <param name="y">Amount to increment to fallstart</param>
        public void AdjustFallStart(float y)
        {
            if (falling)
            {
                fallStartLevel += y;
            }
        }

        public void ClearFallingDamage()
        {
            falling = false;
            fallStartLevel = transform.position.y;
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


