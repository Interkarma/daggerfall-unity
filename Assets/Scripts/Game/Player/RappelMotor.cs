using DaggerfallConnect;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class RappelMotor : MonoBehaviour
    {
        private float rappelTimer;
        private Vector3 lastPosition = Vector3.zero;
        public bool IsRappelling { get; private set; }
        private AcrobatMotor acrobatMotor;
        private CharacterController controller;
        private PlayerSpeedChanger speedChanger;
        private PlayerGroundMotor groundMotor;
        private ClimbingMotor climbingMotor;
        private PlayerStepDetector stepDetector;
        private Entity.PlayerEntity player;

        private void Start()
        {
            player = GameManager.Instance.PlayerEntity;

            acrobatMotor = GetComponent<AcrobatMotor>();
            controller = GetComponent<CharacterController>();
            speedChanger = GetComponent<PlayerSpeedChanger>();
            groundMotor = GetComponent<PlayerGroundMotor>();
            climbingMotor = GetComponent<ClimbingMotor>();
            stepDetector = GetComponent<PlayerStepDetector>();
        }

        /// <summary>
        /// Check if should do rappel, and do rappel and attach to wall.
        /// </summary>
        public void RappelChecks()
        {
            // "rappelling" only happens while moving the player into climbing position 
            // from the ledge he backstepped off and doesn't happen while climbing
            if (climbingMotor.IsClimbing)
                IsRappelling = false;

            bool rappelAllowed = (DaggerfallUnity.Settings.AdvancedClimbing && 
                !climbingMotor.IsClimbing && !climbingMotor.IsSlipping && acrobatMotor.Falling);

            if (!rappelAllowed)
                return;

            float minRange = (0);
            // greater maxRange values mean player won't rappel from longer heights
            float maxRange = minRange + 2.90f;

            // are we going to step off something too short for rappel to be worthwhile?
            bool dropTooShortForRappel = (stepDetector.HitDistance > minRange && stepDetector.HitDistance < maxRange);

            if (!IsRappelling && !dropTooShortForRappel)
            {
                // should rappelling start?
                bool inputBackward = InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards);

                IsRappelling = (inputBackward && !acrobatMotor.Jumping);
                if (IsRappelling)
                    DaggerfallUI.AddHUDText(UserInterfaceWindows.HardStrings.rappelMode);
                lastPosition = controller.transform.position;
                rappelTimer = 0f;
            }

            // Rappel Schedule
            if (IsRappelling)
            {
                const float firstTimerMax = 0.7f;
                player.TallySkill(DFCareer.Skills.Climbing, 1);

                rappelTimer += Time.deltaTime;

                if (rappelTimer <= firstTimerMax)
                {
                    Vector3 rappelPosition = Vector3.zero;
                    // create C-shaped movement to plant self against wall beneath
                    Vector3 pos = lastPosition;
                    float yDist = 1.60f;
                    float xzDist = 0.17f;
                    rappelPosition.x = Mathf.Lerp(pos.x, pos.x - (controller.transform.forward.x * xzDist), Mathf.Sin(Mathf.PI * (rappelTimer / firstTimerMax)));
                    rappelPosition.z = Mathf.Lerp(pos.z, pos.z - (controller.transform.forward.z * xzDist), Mathf.Sin(Mathf.PI * (rappelTimer / firstTimerMax)));
                    rappelPosition.y = Mathf.Lerp(pos.y, pos.y - yDist, rappelTimer / firstTimerMax);

                    controller.transform.position = rappelPosition;
                }
                else
                {
                    Vector3 rappelDirection = Vector3.zero;
                    // Auto forward to grab wall
                    float speed = speedChanger.GetBaseSpeed();
                    if (climbingMotor.LedgeDirection != Vector3.zero)
                        rappelDirection = climbingMotor.LedgeDirection;
                    else
                        rappelDirection = controller.transform.forward;
                    rappelDirection *= speed * 1.25f;
                    groundMotor.MoveWithMovingPlatform(rappelDirection);
                }
            }
        }
    }
}
