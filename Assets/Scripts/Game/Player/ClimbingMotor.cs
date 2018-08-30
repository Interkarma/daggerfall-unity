using UnityEngine;
using System;
using System.Collections;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game
{

    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(LevitateMotor))]
    [RequireComponent(typeof(CharacterController))]
    public class ClimbingMotor : MonoBehaviour
    {
        private PlayerMotor playerMotor;
        private LevitateMotor levitateMotor;
        private CharacterController controller;
        private PlayerHeightChanger heightChanger;
        private PlayerGroundMotor groundMotor;
        public Vector3 ledgeDirection; // direction of the ledge they can climb onto
        private bool failedClimbingCheck = false;
        private bool isClimbing = false;
        private float climbingStartTimer = 0;
        private float climbingContinueTimer = 0;
        private uint timeOfLastClimbingCheck = 0;
        private bool showClimbingModeMessage = true;
        private Vector2 lastHorizontalPosition = Vector2.zero;
        public bool IsClimbing
        {
            get { return isClimbing; }
            set
            {
                isClimbing = value;
            }
        }
        void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            levitateMotor = GetComponent<LevitateMotor>();
            controller = GetComponent<CharacterController>();
            heightChanger = GetComponent<PlayerHeightChanger>();
            groundMotor = GetComponent<PlayerGroundMotor>();
        }

        /// <summary>
        /// Perform climbing check, and if successful, start climbing movement.
        /// </summary>
        public void ClimbingCheck()
        {
            //if (!GameManager.IsGamePaused)
            //    Debug.Log("At Climb Check: CollisionFlags: " + playerMotor.CollisionFlags);

            // Get pre-movement position for climbing check
            lastHorizontalPosition = new Vector2(controller.transform.position.x, controller.transform.position.z);

            // this can be a cause of the player climbing into the air.
            if (isClimbing)
                playerMotor.CollisionFlags |= CollisionFlags.Sides;

            // Climbing
            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            if (!InputManager.Instance.HasAction(InputManager.Actions.MoveForwards)
                || (playerMotor.CollisionFlags & (CollisionFlags.Sides | CollisionFlags.CollidedSides)) == 0
                || failedClimbingCheck
                || levitateMotor.IsLevitating
                || playerMotor.IsRiding
                //|| (playerMotor.IsCrouching && !levitateMotor.IsSwimming && !heightChanger.ForcedSwimCrouch)
                || Vector2.Distance(lastHorizontalPosition, new Vector2(controller.transform.position.x, controller.transform.position.z)) >= (0.003f)// Approximation based on observing classic in-game
                || (isClimbing && CanWalkOntoLedge())) // isclimbing for short circuit evaluation
            {
                IsClimbing = false;
                showClimbingModeMessage = true;
                climbingStartTimer = 0;
                timeOfLastClimbingCheck = gameMinutes;
            }
            else
            {
                if (climbingStartTimer <= (playerMotor.systemTimerUpdatesPerSecond * 14))
                    climbingStartTimer += Time.deltaTime;
                else
                {
                    if (!isClimbing)
                    {
                        if (showClimbingModeMessage)
                            DaggerfallUI.AddHUDText(UserInterfaceWindows.HardStrings.climbingMode);
                        // Disable further showing of climbing mode message until current climb attempt is stopped
                        // to keep it from filling message log
                        showClimbingModeMessage = false;
                        IsClimbing = true;
                    }

                    // Initial check to start climbing
                    if ((gameMinutes - timeOfLastClimbingCheck) > 18)
                    {
                        Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
                        player.TallySkill(DFCareer.Skills.Climbing, 1);
                        timeOfLastClimbingCheck = gameMinutes;
                        if (UnityEngine.Random.Range(1, 101) > 95)
                        {
                            if (UnityEngine.Random.Range(1, 101) > player.Skills.GetLiveSkillValue(DFCareer.Skills.Climbing))
                            {
                                IsClimbing = false;
                                failedClimbingCheck = true;
                            }
                        }
                    }
                }
            }

            if (isClimbing)
                ClimbMovement();
        }

        private bool CanWalkOntoLedge()
        {
            RaycastHit hit;

            Vector3 p1 = controller.transform.position + controller.center + Vector3.up * -controller.height * 0.50f;
            Vector3 p2 = p1 + Vector3.up * controller.height;
            Vector3 footPosition = controller.center + (Vector3.down * controller.height * 0.45f);
            float distanceToObstacle = 0;

            // Cast character controller shape forward to see if it is about to hit anything.
            if (Physics.CapsuleCast(p1, p2, controller.radius * 0.5f, transform.forward, out hit, 0.5f))
            //if (Physics.Raycast(footPosition, controller.transform.forward, out hit, 0.5f ))
            {
                distanceToObstacle = hit.distance;
                ledgeDirection = -hit.normal;
            }

            //Debug.Log("DistanceToWall: " + distanceToObstacle);

            bool canWalkOntoLedge = (distanceToObstacle == 0);

            if (canWalkOntoLedge)
                groundMotor.EnableClimbTimer();

            return canWalkOntoLedge;
        }

        private void ClimbMovement()
        {
            controller.Move(Vector3.up * Time.deltaTime);
            if (climbingContinueTimer <= (playerMotor.systemTimerUpdatesPerSecond * 15))
                climbingContinueTimer += Time.deltaTime;
            else
            {
                climbingContinueTimer = 0;
                Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
                player.TallySkill(DFCareer.Skills.Climbing, 1);
                int skill = player.Skills.GetLiveSkillValue(DFCareer.Skills.Climbing);
                if (player.Race == Entity.Races.Khajiit)
                    skill += 30;
                Mathf.Clamp(skill, 5, 95);

                //if ((UnityEngine.Random.Range(1, 101) > 90)
                //    || (UnityEngine.Random.Range(1, 101) > skill))
                //{
                //    IsClimbing = false;
                //}
            }
        }
    }
}


