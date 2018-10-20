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
        private PlayerMoveScanner playerScanner;
        private Transform camTransform;
        private PlayerMotor playerMotor;
        private Entity.PlayerEntity player;
        private float hangingStartTimer = 0;
        private float hangingContinueTimer = 0;
        private float rappelTimer;
        private bool isLosingGrip = false;
        private Vector3 lastPosition = Vector3.zero;
        private Vector2 lastHorizontalPosition = Vector2.zero;
        private bool showHangingModeMessage = true;
        // how long it takes before we do another skill check to see if we can continue hanging
        private const int continueClimbingSkillCheckFrequency = 15;
        // how long it takes before we try to regain hold if losing grip
        private readonly float regainHoldSkillCheckFrequency = 5;
        // maximum chances we have to regain our grip
        private const int chancesToRegainGrip = 3;
        // minimum percent chance to regain hold per skill check if losing grip
        private const int regainHoldMinChance = 100;
        // minimum percent chance to continue climbing per skill check, gets closer to 100 with higher skill
        private const int continueHangMinChance = 100;

        public bool IsHanging {
            get;
            set; } // for simplicity allowing this to set publicly. Only allow climbingMotor to set
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
            playerScanner = GetComponent<PlayerMoveScanner>();
            camTransform = GameManager.Instance.MainCamera.transform;
        }

        public void HangingChecks()
        {
            if (!DaggerfallUnity.Settings.AdvancedClimbing)
                return;

            float startHangSkillCheckFrequency = 14f;
            float continueHangingSkillCheckFrequency = 14f;

            bool inputBack = InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards);
            bool inputForward = InputManager.Instance.HasAction(InputManager.Actions.MoveForwards);

            // boolean that means ground directly below us is too close for climbing or rappelling
            //bool tooCloseToGroundForClimb = (((isClimbing && (inputBack || isSlipping)) || airborneGraspWall)
            // short circuit evaluate the raycast, also prevents bug where you could teleport across town
            //    && Physics.Raycast(controller.transform.position, Vector3.down, controller.height / 2 + 0.12f));

            bool inputAbortCondition = (InputManager.Instance.HasAction(InputManager.Actions.Crouch)
                                        || InputManager.Instance.HasAction(InputManager.Actions.Jump));

            // TODO: toggle from hanging to climbing

            // Should we abort hanging?
            if (inputAbortCondition
                || (playerMotor.CollisionFlags & CollisionFlags.Above) == 0
                || levitateMotor.IsLevitating
                || IsHanging && climbingMotor.IsClimbing)
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
                    // skill check to see if we catch the wall 
                    if (climbingMotor.ClimbingSkillCheck(100))
                        StartHanging();
                    else
                        hangingStartTimer = 0;
                }

                // schedule climbing continues, Faster updates if slipping
                if (hangingContinueTimer <= (playerMotor.systemTimerUpdatesDivisor * (isLosingGrip ? regainHoldSkillCheckFrequency : continueHangingSkillCheckFrequency)))
                    hangingContinueTimer += Time.deltaTime;
                else
                {
                    hangingContinueTimer = 0;

                    // TODO: Disable weapon while hanging.

                    // it's harder to regain hold while losing grip than it is to continue climbing with a good hold on wall
                    if (isLosingGrip)
                        isLosingGrip = !climbingMotor.ClimbingSkillCheck(regainHoldMinChance);
                    else
                        isLosingGrip = !climbingMotor.ClimbingSkillCheck(continueHangMinChance);
                }
            }

            // execute schedule
            if (IsHanging)
            {
                // handle movement direction
                HangMoveDirection();

                // both variables represent similar situations, but different context
                acrobatMotor.Falling = false;
            }
        }

        private void HangMoveDirection()
        {
            RaycastHit hit;
            if (Physics.Raycast(controller.transform.position, controller.transform.up, out hit, (controller.height / 2) + 1f))
            {
                float playerspeed = speedChanger.GetClimbingSpeed();
                Vector3 moveVector = Vector3.zero;
                if (InputManager.Instance.HasAction(InputManager.Actions.MoveForwards))
                    moveVector += camTransform.forward;
                else if (InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards))
                    moveVector -= camTransform.forward;

                if (InputManager.Instance.HasAction(InputManager.Actions.MoveRight))
                    moveVector += camTransform.right;
                else if (InputManager.Instance.HasAction(InputManager.Actions.MoveLeft))
                    moveVector -= camTransform.right;

                if (moveVector != Vector3.zero)
                {  
                    moveVector = (Vector3.ProjectOnPlane(moveVector, hit.normal).normalized * playerspeed) + (-hit.normal * 0.2f * playerspeed);

                    moveVector.y = Mathf.Max(moveVector.y, 0.2f);

                    //if (moveVector.y < 0.05f)
                    //{
                    //    moveVector.y = //= Vector3.Reflect(moveVector, Vector3.up);
                    //}

                    Debug.DrawRay(controller.transform.position, hit.normal, Color.yellow);
                    groundMotor.MoveWithMovingPlatform(moveVector);
                    Debug.DrawRay(controller.transform.position, moveVector, Color.blue);
                }
            }
        }

        private void StartHanging()
        {
            if (!IsHanging)
            {
                if (showHangingModeMessage)
                    DaggerfallUI.AddHUDText(UserInterfaceWindows.HardStrings.hangingMode);

                showHangingModeMessage = false;
                climbingMotor.IsClimbing = false;
                IsHanging = true;
            }
        }
    }
}
