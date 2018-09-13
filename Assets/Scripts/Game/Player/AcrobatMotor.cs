using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(CharacterController))]
    public class AcrobatMotor : MonoBehaviour
    {
        const float slowFallSpeed = 105f;

        public float jumpSpeed = 8.0f;
        public float gravity = 20.0f;
        public float crouchingJumpDelta = 0.8f;
        public float fallingDamageThreshold = 10.0f;
        public bool airControl = false;

        PlayerMotor playerMotor;
        FrictionMotor frictionMotor;
        ClimbingMotor climbingMotor;
        Transform myTransform;

        private float fallStartLevel;
        private bool falling;
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
            climbingMotor = GetComponent<ClimbingMotor>();
            myTransform = playerMotor.transform;
        }

        /// <summary>
        /// Jump! But only if the jump button has been released and player has been grounded for a given number of frames
        /// </summary>
        /// <param name="moveDirection"></param>
        public void HandleJumpInput(ref Vector3 moveDirection)
        {
            // Cancel jump if player is paralyzed or swimming on a water tile
            // Jump is also ignored when player is slowfalling
            if (GameManager.Instance.PlayerEntity.IsParalyzed ||
                GameManager.Instance.PlayerMotor.OnExteriorWater == PlayerMotor.OnExteriorWaterMethod.Swimming ||
                GameManager.Instance.PlayerEntity.IsSlowFalling)
                return;

            if (InputManager.Instance.HasAction(InputManager.Actions.Jump))
            {
                // Jumping effect states "causes target to jump at twice natural capacity" - multiplying jump speed for more height
                // Not quite double here, as it feels too high and all character have same base jump height anyway
                // This is just temporary as jump height currently not modified by jumping skill or any other bonuses
                // Ideally this spell would double jump skill which in turn increases height (classic matched jumping is todo on roadmap)
                float jumpSpeedMultiplier = 1.0f;
                if (GameManager.Instance.PlayerEntity.IsEnhancedJumping)
                    jumpSpeedMultiplier = 1.6f;

                moveDirection.y = jumpSpeed * jumpSpeedMultiplier;
                jumping = true;

                // Modify crouching jump speed
                if (playerMotor.IsCrouching)
                    moveDirection.y *= crouchingJumpDelta;
            }
            else
            {
                jumping = false;
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
        /// If we stepped over a cliff or something, set the height at which we started falling
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
            // Slowfalling makes player fall at a constant speed with no acceleration
            // Also resets start fall position each tick so that if effect expires during fall
            // then fall damage will only take place from vertical position effect was lost
            if (falling && GameManager.Instance.PlayerEntity.IsSlowFalling)
            {
                fallStartLevel = myTransform.position.y;
                moveDirection.y = -slowFallSpeed * Time.deltaTime;
            }
            else
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
        } 
                       
        /// <summary>
        /// If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
        /// </summary>
        public void CheckFallingDamage()
        {
            if (falling)
            {
                falling = false;
                // don't take damage if landing in outdoor water
                if (GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 0)
                    return;
                float fallDistance = fallStartLevel - myTransform.position.y;

                if (!climbingMotor.IsClimbing || climbingMotor.IsSlipping)
                { 
                    if (fallDistance > fallingDamageThreshold)
                        FallingDamageAlert(fallDistance);
                    else if (fallDistance > fallingDamageThreshold / 2f)
                        BadFallDetected(fallDistance);
                }
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


