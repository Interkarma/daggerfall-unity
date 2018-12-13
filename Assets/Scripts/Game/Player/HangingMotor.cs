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
        private RappelMotor rappelMotor;
        private ClimbingMotor climbingMotor;
        private PlayerMoveScanner scanner;
        private Transform camTransform;
        private PlayerMotor playerMotor;
        private Entity.PlayerEntity player;

        private enum HangingTransitionState
        {
            None,
            ExitImmediately,
            BeginAfterTimer,
            BeginImmediately
        }
        private float startTimer = 0;
        private float hangingContinueTimer = 0;
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
            rappelMotor = GetComponent<RappelMotor>();
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

            bool touchingSides = (playerMotor.CollisionFlags & CollisionFlags.Sides) != 0;
            bool touchingAbove = (playerMotor.CollisionFlags & CollisionFlags.Above) != 0;
            IsWithinHangingDistance = (scanner.HeadHitDistance > halfHeight - 0.17f && scanner.HeadHitDistance < halfHeight - 0.09f);

            bool inputAbortCondition = (InputManager.Instance.HasAction(InputManager.Actions.Crouch)
                                        || InputManager.Instance.HasAction(InputManager.Actions.Jump));
            
            bool horizontallyStationary = Vector2.Distance(lastHorizontalPosition, new Vector2(controller.transform.position.x, controller.transform.position.z)) < startHangingHorizontalTolerance;
            bool forwardStationaryNearCeiling = inputForward && IsWithinHangingDistance && horizontallyStationary;
            bool pushingHeadAgainstCeilingWhileClimbing = climbingMotor.IsClimbing && !IsHanging && touchingAbove && forwardStationaryNearCeiling;
            bool pushingHeadAgainstCeilingWhileRappeling = rappelMotor.IsRappelling && !IsHanging && touchingAbove;
            bool doHanging;
            if (pushingHeadAgainstCeilingWhileClimbing)
                doHanging = HangingTransition(HangingTransitionState.BeginAfterTimer);
            else if (pushingHeadAgainstCeilingWhileRappeling)
                doHanging = HangingTransition(HangingTransitionState.BeginImmediately);
            else if (IsHanging)
                doHanging = true;
            else
                doHanging = HangingTransition(HangingTransitionState.ExitImmediately);

            // TODO: make condition that allows player to hang from rappel state
            if (inputAbortCondition
                || !IsWithinHangingDistance
                || levitateMotor.IsLevitating
                )
            {
                HangingTransition(HangingTransitionState.ExitImmediately);
            }
            else if (doHanging)// Schedule Hanging events
            {
                // schedule climbing continues, Faster updates if slipping
                if (hangingContinueTimer <= (playerMotor.systemTimerUpdatesDivisor * (isLosingGrip ? regainHoldSkillCheckFrequency : continueHangingSkillCheckFrequency)))
                    hangingContinueTimer += Time.deltaTime;
                else
                {
                    hangingContinueTimer = 0;

                    // TODO: Disable weapon while hanging.
                    // TODO: finish implementing skill based checks
                    // it's harder to regain hold while losing grip than it is to continue hanging with a good hold on ceiling
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
                
                FindAboveBehindWall();      
                FindFrontWall();

                acrobatMotor.Falling = false;
            }
        }

        private void FindAboveBehindWall()
        {
            Vector3 pos = controller.transform.position;
            Vector3 startPos = pos + (controller.transform.forward * 0.4f) + (Vector3.up * 0.5f);
            if (InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards))
            {
                scanner.FindAdjacentSurface(startPos, -controller.transform.forward, PlayerMoveScanner.RotationDirection.YZClockwise);
            }
        }

        private void FindFrontWall()
        {
            Vector3 pos = controller.transform.position;
            Vector3 startPos = pos + (-controller.transform.forward * 0.2f) + (Vector3.down * 0.2f);
            if (InputManager.Instance.HasAction(InputManager.Actions.MoveForwards))
                // initial scan direction is downward
                scanner.FindAdjacentSurface(startPos, Vector3.down, PlayerMoveScanner.RotationDirection.YZCounterClockwise);
        }

        private bool HangingTransition(HangingTransitionState transState)
        {
            const float startTimerMax = 10f;
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
                case HangingTransitionState.ExitImmediately:
                    CancelHanging();
                    GetLastHorizontalPositon();
                    shouldHang = false;
                    break;
                case HangingTransitionState.BeginImmediately:
                    StartHanging();
                    startTimer = 0;
                    shouldHang = true;
                    break;
            }

            return shouldHang;
        }

        private void HangMoveDirection()
        {
            RaycastHit hit;
            if (Physics.SphereCast(controller.transform.position, scanner.HeadHitRadius, controller.transform.up,  out hit, 2f))
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
