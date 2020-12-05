using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(CharacterController))]
    public class AcrobatMotor : MonoBehaviour
    {
        const float slowFallSpeed = 105f;

        public float jumpSpeed = 4.5f;
        public float gravity = 20.0f;
        public float crouchingJumpDelta = 0.8f;
        public float fallingDamageThreshold = 5.0f;
        public bool airControl = false;

        PlayerMotor playerMotor;
        CharacterController controller;
        FrictionMotor frictionMotor;
        ClimbingMotor climbingMotor;
        Transform myTransform;
        PlayerMoveScanner playerScanner;
        RappelMotor rappelMotor;

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
            controller = GetComponent<CharacterController>();
            frictionMotor = GetComponent<FrictionMotor>();
            climbingMotor = GetComponent<ClimbingMotor>();
            playerScanner = GetComponent<PlayerMoveScanner>();
            rappelMotor = GetComponent<RappelMotor>();
            myTransform = playerMotor.transform;
        }

        /// <summary>
        /// Jump!
        /// </summary>
        /// <param name="moveDirection"></param>
        public void HandleJumpInput(ref Vector3 moveDirection)
        {
            // Cancel jump if player is paralyzed or swimming on a water tile
            // Jump is also ignored when player is slowfalling or riding cart
            if (GameManager.Instance.PlayerEntity.IsParalyzed ||
                GameManager.Instance.PlayerMotor.OnExteriorWater == PlayerMotor.OnExteriorWaterMethod.Swimming ||
                GameManager.Instance.PlayerEntity.IsSlowFalling ||
                GameManager.Instance.TransportManager.TransportMode == TransportModes.Cart)
                return;

            // Cancel jump if player has not been grounded for long enough
            // Players with low jumping skills don't jump very high, which can result in unwanted bunny-hopping before player can release jump key
            // A grounded time check helps ensure bunny-hops are intended as player has continued to hold down jump key
            // Also the effect of bunny-hopping too quickly feels like player is jumping again before landing properly
            if (!climbingMotor.WasClimbing && playerMotor.GroundedTime < 0.1f)
                return;

            if (InputManager.Instance.HasAction(InputManager.Actions.Jump))
            {
                const float athleticismMultiplier = 0.1f;           // +10%
                const float improvedAthleticismMultiplier = 0.1f;   // +10%
                const float jumpSpellMultiplier = 0.6f;             // +60%

                // Baseline jump speed is improved by a percentage equal to JumpingSkill / 2
                // Jumping in DFU is roughly the same as classic at low skill levels
                // Jumping in DFU is higher at 100 skill than in classic, but still not so high as pre-beta DFU
                float jumpSpeedMultiplier = 1.0f;
                jumpSpeedMultiplier += GameManager.Instance.PlayerEntity.Skills.GetLiveSkillValue(DaggerfallConnect.DFCareer.Skills.Jumping) / 2 / 100f;

                // Add Athleticism and Improved Athleticism multipliers (will together make for +20%)
                if (GameManager.Instance.PlayerEntity.Career.Athleticism)
                {
                    jumpSpeedMultiplier += athleticismMultiplier;
                    if (GameManager.Instance.PlayerEntity.ImprovedAthleticism)
                        jumpSpeedMultiplier += improvedAthleticismMultiplier;
                }

                // Add Jumping effect multiplier
                if (GameManager.Instance.PlayerEntity.IsEnhancedJumping)
                    jumpSpeedMultiplier += jumpSpellMultiplier;

                moveDirection.y = jumpSpeed * jumpSpeedMultiplier;
                jumping = true;

                // HACK: Also adds a small amount of forward boost to player when they jump while moving
                // This (very) loosely simulates classic where player receives more forward momentum than in DFUnity at present
                if (!GameManager.Instance.PlayerMotor.IsStandingStill)
                    moveDirection += (transform.forward * jumpSpeed) * 0.05f;

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

            if ((rappelMotor.IsRappelling || airControl || GameManager.Instance.PlayerEntity.IsEnhancedJumping) && frictionMotor.PlayerControl)
            {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }
        }

        public void HitHead(ref Vector3 moveDirection)
        {
            // If we hit something above us AND we are moving up, reverse vertical movement
            if ((controller.collisionFlags & CollisionFlags.Above) != 0)
            {
                if (moveDirection.y > 0)
                    moveDirection.y = -moveDirection.y;
            }
        }


        /// <summary>
        /// If we stepped over a cliff or something, set the height at which we started falling
        /// </summary>
        public void CheckInitFall(ref Vector3 moveDirection)
        {
            if (!falling)
            {
                falling = true;
                fallStartLevel = myTransform.position.y;
                // begin y movement at 0
                if (!jumping)
                    moveDirection.y = 0;
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
            else if (!rappelMotor.IsRappelling)
            {
                const float antiBumpFactor = 20.75f;
                float minRange = (controller.height / 2f) - 0.15f;
                float maxRange = minRange + 1.10f;

                // should we apply anti-bump gravity?
                if (!climbingMotor.IsClimbing && playerScanner.StepHitDistance > minRange && playerScanner.StepHitDistance < maxRange)
                    moveDirection.y -= antiBumpFactor;

                // apply normal gravity
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


