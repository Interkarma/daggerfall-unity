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
        private PlayerMoveScanner scanner;
        private Transform camTransform;
        private PlayerMotor playerMotor;
        private Entity.PlayerEntity player;
        private enum HangingTransitionState
        {
            None,
            ExitImmediately,
            //ExitAfterTimer,
            BeginAfterTimer
        }
        private float startTimer = 0;
        private float hangingContinueTimer = 0;
        private float exitTimer = 0;
        private float rappelTimer;
        private bool isLosingGrip = false;
        private Vector3 lastPosition = Vector3.zero;
        private Vector2 lastHorizontalPosition = Vector2.zero;
        private bool showHangingModeMessage = true;
        private float startHangingHorizontalTolerance = 0.12f;

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

        public bool IsHanging { get; private set; }
        public bool IsWithinHangingDistance { get; private set; }
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
            scanner = GetComponent<PlayerMoveScanner>();
            camTransform = GameManager.Instance.MainCamera.transform;
        }

        public void HangingChecks()
        {
            if (!DaggerfallUnity.Settings.AdvancedClimbing)
                return;
            if (levitateMotor.IsLevitating)
                return;

            float continueHangingSkillCheckFrequency = 14f;
            float halfHeight = (controller.height / 2f);
            bool inputBack = InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards);
            bool inputForward = InputManager.Instance.HasAction(InputManager.Actions.MoveForwards);
            //inputForward = true;
            bool touchingSides = (playerMotor.CollisionFlags & CollisionFlags.Sides) != 0;
            bool touchingAbove = (playerMotor.CollisionFlags & CollisionFlags.Above) != 0;
            IsWithinHangingDistance = (scanner.HeadHitDistance > halfHeight - 0.17f && scanner.HeadHitDistance < halfHeight - 0.09f);

            bool inputAbortCondition = (InputManager.Instance.HasAction(InputManager.Actions.Crouch)
                                        || InputManager.Instance.HasAction(InputManager.Actions.Jump));
            
            bool horizontallyStationary = Vector2.Distance(lastHorizontalPosition, new Vector2(controller.transform.position.x, controller.transform.position.z)) < startHangingHorizontalTolerance;
            bool forwardStationaryNearCeiling = inputForward && IsWithinHangingDistance && horizontallyStationary;
            //bool pushingFaceAgainstWallNearCeiling = IsHanging && !climbingMotor.IsClimbing && touchingSides && forwardStationaryNearCeiling;
            bool pushingHeadAgainstCeilingWhileClimbing = climbingMotor.IsClimbing && !IsHanging && touchingAbove && forwardStationaryNearCeiling;
            bool doHanging;
            // TODO: maybe this variable should be in climbing motor.  
            /*if (pushingFaceAgainstWallNearCeiling)
                doHanging = HangingTransition(HangingTransitionState.ExitAfterTimer);
            else */if (pushingHeadAgainstCeilingWhileClimbing)
                doHanging = HangingTransition(HangingTransitionState.BeginAfterTimer);
            else if (IsHanging)
                doHanging = true;
            else
                doHanging = HangingTransition(HangingTransitionState.ExitImmediately);

            /*
            if (inputAbortCondition
                || touchingSides
                || touchingSides && inputForward && horizontallyStationary
                || !withinHangingDistance
                || levitateMotor.IsLevitating
                )
            {
                HangingTransition(HangingTransitionState.ExitImmediately);
            }
            else */// Schedule Hanging events
            if (doHanging)
            {
                /*if (!IsHanging)
                {
                    // schedule hanging start
                    if (hangingStartTimer <= (playerMotor.systemTimerUpdatesDivisor * startHangSkillCheckFrequency))
                        hangingStartTimer += Time.deltaTime;
                    else
                    {
                        // skill check to see if we catch the ceiling 
                        //if (climbingMotor.ClimbingSkillCheck(100))
                            StartHanging();
                        //else
                        //    hangingStartTimer = 0;
                    }
                }
                else
                {*/
                    // schedule climbing continues, Faster updates if slipping
                    if (hangingContinueTimer <= (playerMotor.systemTimerUpdatesDivisor * (isLosingGrip ? regainHoldSkillCheckFrequency : continueHangingSkillCheckFrequency)))
                        hangingContinueTimer += Time.deltaTime;
                    else
                    {
                        hangingContinueTimer = 0;

                        // TODO: Disable weapon while hanging.

                        // it's harder to regain hold while losing grip than it is to continue hanging with a good hold on ceiling
                        if (isLosingGrip)
                            isLosingGrip = !climbingMotor.ClimbingSkillCheck(regainHoldMinChance);
                        else
                            isLosingGrip = !climbingMotor.ClimbingSkillCheck(continueHangMinChance);
                    }
                //}

            }

            // execute schedule
            if (IsHanging)
            {
                // handle movement direction
                HangMoveDirection();

                acrobatMotor.Falling = false;
            }
        }

        private bool HangingTransition(HangingTransitionState transState)
        {
            float startTimerMax = 14f;
            float exitTimerMax = 14f;
            bool shouldHang = false;
            switch (transState)
            { 
                case HangingTransitionState.BeginAfterTimer:
                    if (startTimer <= (playerMotor.systemTimerUpdatesDivisor * startTimerMax))
                    {
                        startTimer += Time.deltaTime;
                        shouldHang = false;
                    }
                    else
                    {
                        StartHanging();
                        startTimer = 0;
                        shouldHang = true;
                    }
                    break;
                // TODO: make this exit begin climbing.
                /*case HangingTransitionState.ExitAfterTimer:
                    if (exitTimer <= (playerMotor.systemTimerUpdatesDivisor * exitTimerMax))
                    {
                        exitTimer += Time.deltaTime;
                        shouldHang = true;
                    }
                    else
                    {
                        // Exit Hanging, basically exit immediately
                        CancelHanging();
                        shouldHang = false;
                    }
                    break;*/
                case HangingTransitionState.ExitImmediately:
                    CancelHanging();
                    GetLastHorizontalPositon();
                    shouldHang = false;
                    break;
            }
            //lastHorizontalPosition = new Vector2(controller.transform.position.x, controller.transform.position.z);

            return shouldHang;
        }

        private void HangMoveDirection()
        {
            RaycastHit hit;
            if (Physics.Raycast(controller.transform.position, controller.transform.up, out hit, (controller.height / 2) + 1f))
            {
                float playerspeed = speedChanger.GetClimbingSpeed();
                Vector3 moveVector = Vector3.zero;
                if (InputManager.Instance.HasAction(InputManager.Actions.MoveForwards))
                    moveVector += controller.transform.forward;
                else if (InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards))
                    moveVector -= controller.transform.forward;

                if (InputManager.Instance.HasAction(InputManager.Actions.MoveRight))
                    moveVector += controller.transform.right;
                else if (InputManager.Instance.HasAction(InputManager.Actions.MoveLeft))
                    moveVector -= controller.transform.right;

                if (moveVector != Vector3.zero)
                {  
                    moveVector = (Vector3.ProjectOnPlane(moveVector, hit.normal).normalized * playerspeed) + (-hit.normal * 0.2f * playerspeed);

                    moveVector.y = Mathf.Max(moveVector.y, 0.2f);
                }
                else
                {
                    moveVector = (Vector3.up * 0.001f);
                }
                Debug.DrawRay(controller.transform.position, hit.normal, Color.yellow);
                groundMotor.MoveWithMovingPlatform(moveVector);
                Debug.DrawRay(controller.transform.position, moveVector, Color.blue);
            }
        }

        private void StartHanging()
        {
            if (!IsHanging)
            {
                if (showHangingModeMessage)
                    DaggerfallUI.AddHUDText(UserInterfaceWindows.HardStrings.hangingMode);

                showHangingModeMessage = false;
                IsHanging = true;
                climbingMotor.StopClimbing(IsHanging);
            }
        }

        public void CancelHanging()
        {
            IsHanging = false;
            showHangingModeMessage = true;
            startTimer = 0;
        }

        private void GetLastHorizontalPositon()
        {
            lastHorizontalPosition = new Vector2(controller.transform.position.x, controller.transform.position.z);
        }
    }
}
