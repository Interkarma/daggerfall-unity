using System;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class HangingMotor : MonoBehaviour
    {
        private AcrobatMotor acrobatMotor;
        private LevitateMotor levitateMotor;
        private CharacterController controller;
        private PlayerSpeedChanger speedChanger;
        private PlayerGroundMotor groundMotor;
        private ClimbingMotor climbingMotor;
        private PlayerStepDetector stepDetector;
        private PlayerMotor playerMotor;
        private Entity.PlayerEntity player;
        private float hangingStartTimer = 0;
        private float hangingContinueTimer = 0;
        private float rappelTimer;
        private Vector3 lastPosition = Vector3.zero;
        private Vector2 lastHorizontalPosition = Vector2.zero;
        private bool showHangingModeMessage = true;
        public bool IsHanging { get; private set; }
        public bool EjectToClimbing { get; private set; }
        private void Start()
        {
            player = GameManager.Instance.PlayerEntity;
            playerMotor = GetComponent<PlayerMotor>();
            acrobatMotor = GetComponent<AcrobatMotor>();
            levitateMotor = GetComponent<LevitateMotor>();
            controller = GetComponent<CharacterController>();
            speedChanger = GetComponent<PlayerSpeedChanger>();
            groundMotor = GetComponent<PlayerGroundMotor>();
            climbingMotor = GetComponent<ClimbingMotor>();
            stepDetector = GetComponent<PlayerStepDetector>();
        }

        public void HangingChecks()
        {
            bool advancedClimbingOn = DaggerfallUnity.Settings.AdvancedClimbing;
            float startClimbHorizontalTolerance = 0.14f;
            float startHangSkillCheckFrequency = 14f;
            // true if we should try climbing wall and are airborne
            //bool airborneGraspCeiling = (!IsHanging && acrobatMotor.Jumping);
            bool inputBack = InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards);
            bool inputForward = InputManager.Instance.HasAction(InputManager.Actions.MoveForwards);

            // boolean that means ground directly below us is too close for climbing or rappelling
            //bool tooCloseToGroundForClimb = (((isClimbing && (inputBack || isSlipping)) || airborneGraspWall)
            // short circuit evaluate the raycast, also prevents bug where you could teleport across town
            //    && Physics.Raycast(controller.transform.position, Vector3.down, controller.height / 2 + 0.12f));

            bool inputAbortCondition = (InputManager.Instance.HasAction(InputManager.Actions.Crouch)
                                        || InputManager.Instance.HasAction(InputManager.Actions.Jump));

            // Should we abort hanging?
            if (inputAbortCondition
                || (playerMotor.CollisionFlags & CollisionFlags.Above) == 0
                || levitateMotor.IsLevitating)
            {
                IsHanging = false;
                showHangingModeMessage = true;
                hangingStartTimer = 0;
            }
            else // Schedule Hanging events
            {   
                // schedule hanging start
                if (hangingStartTimer <= (playerMotor.systemTimerUpdatesDivisor * startHangSkillCheckFrequency))
                    hangingStartTimer += Time.deltaTime;
                else
                {
                    // automatic success if not falling
                    //if (!airborneGraspWall)
                    //    StartHanging();
                    // skill check to see if we catch the wall 
                    if (climbingMotor.ClimbingSkillCheck(100))
                        StartHanging();
                    else
                        hangingStartTimer = 0;
                }
            }

            if (IsHanging && 
                inputForward && 
                ((playerMotor.CollisionFlags & CollisionFlags.Sides) != 0) &&
                Vector2.Distance(lastHorizontalPosition, new Vector2(controller.transform.position.x, controller.transform.position.z)) > startClimbHorizontalTolerance)
            {
                // change to climbing from Hanging
            }
            
            lastHorizontalPosition = new Vector2(controller.transform.position.x, controller.transform.position.z);
        }

        private void StartHanging()
        {
            if (!IsHanging)
            {
                if (showHangingModeMessage)
                    // TODO: make this text a string in HardStrings
                    DaggerfallUI.AddHUDText("Hanging Mode");

                showHangingModeMessage = false;
                IsHanging = true;
            }
        }

        private Vector3 GetCeilingHangVector(Vector3 origin, Vector3 direction)
        {
            return Vector3.zero;
        }
    }
}
