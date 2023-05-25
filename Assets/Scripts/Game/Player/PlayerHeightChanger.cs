// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Meteoric Dragon
// Contributors:    
// 
// Notes:
//

using DaggerfallWorkshop.Game.Serialization;
using UnityEngine;
using System;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Tells HeightChanger.Update() which method to call to change controller height and camera position
    /// </summary>
    public enum HeightChangeAction
    {
        DoNothing,
        DoStanding,
        DoCrouching,
        DoMounting,
        DoDismounting,
        DoSinking,
        DoUnsinking
    }

    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(HeadBobber))]
    [RequireComponent(typeof(LevitateMotor))]
    public class PlayerHeightChanger : MonoBehaviour
    {
        private HeightChangeAction heightAction;
        public HeightChangeAction HeightAction
        {
            get { return heightAction; }
            set { heightAction = value; }
        }
        public bool IsInWaterTile { get; set; }
        public bool ForcedSwimCrouch { get { return forcedSwimCrouch; } set { forcedSwimCrouch = value; } }
        private bool forcedSwimCrouch = true;

        private PlayerMotor playerMotor;
        private CharacterController controller;
        private HeadBobber headBobber;
        private LevitateMotor levitateMotor;
        private ClimbingMotor climbingMotor;
        private Camera mainCamera;
        public const float controllerStandingHeight = 1.8f;
        public const float controllerCrouchHeight = 0.9f;
        public const float controllerRideHeight = 2.6f;   // Height of a horse plus seated rider. (1.6m + 1m)
        public const float controllerSwimHeight = 0.30f;
        public const float controllerSwimHorseDisplacement = 0.30f; // amount added to swim height if on horse
        public const float eyeHeight = 0.09f;         // Eye height is 9cm below top of capsule.
        private float targetCamLevel;
        private float prevCamLevel;
        private float camCrouchLevel;
        private float camStandLevel;
        private float camRideLevel;
        private float camSwimLevel;
        //private float camSwimToCrouchDist;
        private float camCrouchToStandDist;
        //private float camStandToRideDist;
        private float camTimer;
        private const float timerFast = 0.10f;
        private const float timerMedium = 0.25f;
        private const float timerSlow = 0.4f;
        private float timerMax = 0.1f;
        private float camLerp_T;  // for lerping to new camera position
        private bool toggleRiding;
        private bool toggleSink;
        private bool controllerMounted;
        private bool controllerSink;

        // Intended height of controller while standing
        public float FixedControllerStandingHeight
        {
            get { return controllerStandingHeight; }
        }

        // Intended height of controller while standing plus any adjustment amount
        public float CurrentControllerStandingHeight
        {
            get { return controllerStandingHeight + StandingHeightAdjustment; }
        }

        // Allows for temporary dips in controller standing height to help player clear low doorways at bottom of stairs/ramp
        // Should only be set when required and cleared when no longer required.
        // Does nothing if player is crouched, and crouching/uncrouching will clear this adjustment
        float standingHeightAdjustment;
        public float StandingHeightAdjustment
        {
            get { return standingHeightAdjustment; }
            set { ChangeStandingHeightAdjustment(value); }
        }

        private void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            controller = GetComponent<CharacterController>();
            headBobber = GetComponent<HeadBobber>();
            mainCamera = GameManager.Instance.MainCamera;
            levitateMotor = GetComponent<LevitateMotor>();
            climbingMotor = GetComponent<ClimbingMotor>();
            camSwimLevel = controllerSwimHeight / 2f - eyeHeight;
            camCrouchLevel = controllerCrouchHeight / 2f - eyeHeight;
            camStandLevel = controllerStandingHeight / 2f - eyeHeight;
            camRideLevel = controllerRideHeight / 2f - eyeHeight;
            //camSwimToCrouchDist = (controllerCrouchHeight - controllerSwimHeight) / 2f;
            camCrouchToStandDist = (controllerStandingHeight - controllerCrouchHeight) / 2f;
            //camStandToRideDist = (controllerRideHeight - controllerStandHeight) / 2f;

            // Use event to set whether player is crouched on load
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
        }

        /// <summary>
        /// Determines what Height-changing action should be taken based on player's input and PlayerMotor.IsRiding
        /// </summary>
        public void DecideHeightAction()
        {
            bool onWater = (GameManager.Instance.PlayerMotor.OnExteriorWater == PlayerMotor.OnExteriorWaterMethod.Swimming);
            bool swimming = levitateMotor.IsSwimming;
            bool crouching = playerMotor.IsCrouching;
            bool riding = playerMotor.IsRiding;
            bool pressedCrouch = InputManager.Instance.ActionStarted(InputManager.Actions.Crouch);
            bool climbing = climbingMotor.IsClimbing;
            bool levitating = playerMotor.IsLevitating;
            //timerMax = timerSlow;

            // Handle uncrouch and levitating while swimming
            if (levitating)
            {
                if (crouching)
                {
                    heightAction = HeightChangeAction.DoStanding;
                    return;
                }
                onWater = false;
            }

            if (onWater && !toggleSink)
            {
                timerMax = timerSlow;
                heightAction = HeightChangeAction.DoSinking;
                toggleSink = true;
            }
            else if (!onWater && toggleSink)
            {
                timerMax = timerSlow;
                heightAction = HeightChangeAction.DoUnsinking;
                toggleSink = false;
            }
            else if (riding && !toggleRiding)
            {
                timerMax = timerMedium;
                heightAction = HeightChangeAction.DoMounting;
                toggleRiding = true;
            }
            else if (!riding && toggleRiding)
            {
                timerMax = timerFast;
                heightAction = HeightChangeAction.DoDismounting;
                toggleRiding = false;
            }
            else if (!riding && !onWater && !levitating)
            {
                // if we crouch out of water or while on solid ground
                if ((!swimming || playerMotor.IsGrounded) && pressedCrouch)
                {
                    timerMax = timerFast;
                    // Toggle crouching
                    if (crouching)
                        heightAction = HeightChangeAction.DoStanding;
                    else
                        heightAction = HeightChangeAction.DoCrouching;
                    forcedSwimCrouch = false;
                }
                // if climbing, force into standing
                else if (climbing)
                {
                    timerMax = timerMedium;
                    if (crouching)
                        heightAction = HeightChangeAction.DoStanding;
                    forcedSwimCrouch = false;
                }
                // if swimming but not crouching, crouch.
                else if (swimming && !forcedSwimCrouch && !playerMotor.IsGrounded)
                {
                    timerMax = timerMedium;
                    if (!crouching)
                        heightAction = HeightChangeAction.DoCrouching;
                    forcedSwimCrouch = true;
                }
                // if we're in a forced swim crouch, but not swimming, un-force the crouch
                else if (!swimming && forcedSwimCrouch)
                {
                    timerMax = timerMedium;
                    if (crouching)
                        heightAction = HeightChangeAction.DoStanding;
                    forcedSwimCrouch = false;
                }
            }
        }

        /// <summary>
        /// Continue calling actions to increment camera towards destination.
        /// </summary>
        private void Update()
        {
            if (heightAction == HeightChangeAction.DoNothing || GameManager.IsGamePaused)
                return;

            if (heightAction == HeightChangeAction.DoSinking)
                DoSinking();
            else if (heightAction == HeightChangeAction.DoUnsinking)
                DoUnsinking();
            else if (heightAction == HeightChangeAction.DoCrouching)
                DoCrouch();
            else if (heightAction == HeightChangeAction.DoStanding && CanStand())
                DoStand();
            else if (heightAction == HeightChangeAction.DoMounting)
                DoMount();
            else
                DoDismount();
        }

        #region HeightChangerActions

        void ChangeStandingHeightAdjustment(float amount)
        {
            if (amount == standingHeightAdjustment)
                return;

            //Debug.LogFormat("Set new standing height adjustment {0}", amount);

            standingHeightAdjustment = amount;
            ControllerHeightChange(CurrentControllerStandingHeight - FixedControllerStandingHeight);
        }

        private void DoCrouch() // first lower camera, Controller height last 
        {
            float prevHeight = controller.height;

            timerTick();

            UpdateCameraPosition(Mathf.Lerp(prevHeight / 2f, camCrouchLevel, camLerp_T));

            if (camTimer >= timerMax)
            {
                standingHeightAdjustment = 0;
                float targetHeight = controllerCrouchHeight;
                ControllerHeightChange(targetHeight - prevHeight);

                timerResetAction();
                playerMotor.IsCrouching = true;
            }
        }

        private void DoStand() // adjust height first, camera last
        {
            float prevHeight = controller.height;

            if (playerMotor.IsCrouching)
            {
                standingHeightAdjustment = 0;
                float targetHeight = CurrentControllerStandingHeight;
                prevCamLevel = prevHeight / 2f;
                targetCamLevel = ControllerHeightChange(targetHeight - prevHeight);
                playerMotor.IsCrouching = false;
            }
            timerTick();

            UpdateCameraPosition(Mathf.Lerp(prevCamLevel, targetCamLevel, camLerp_T));

            if (camTimer >= timerMax)
            {
                timerResetAction();
            }
        }

        private void DoMount() // adjust height first, camera last
        {
            if (!controllerMounted)
            {
                float prevHeight = controller.height;
                float targetHeight;
                prevCamLevel = prevHeight / 2f;
                if (GameManager.Instance.PlayerMotor.OnExteriorWater == PlayerMotor.OnExteriorWaterMethod.Swimming)
                {
                    targetHeight = controllerSwimHeight + controllerSwimHorseDisplacement;
                    prevCamLevel = camSwimLevel;
                }
                else // on ground
                {
                    targetHeight = controllerRideHeight;
                }

                targetCamLevel = ControllerHeightChange(targetHeight - prevHeight);
                controllerMounted = true;
                playerMotor.IsCrouching = false;
            }

            timerTick();

            UpdateCameraPosition(Mathf.Lerp(prevCamLevel, targetCamLevel, camLerp_T));

            if (camTimer >= timerMax)
            {
                timerResetAction();
            }
        }

        private void DoDismount() // adjust height first, camera last
        {
            if (controllerMounted)
            {
                float prevHeight = controller.height;
                float targetHeight;

                if (GameManager.Instance.PlayerMotor.OnExteriorWater == PlayerMotor.OnExteriorWaterMethod.Swimming)
                {
                    prevCamLevel = prevHeight / 2f;
                    targetHeight = controllerSwimHeight;
                }
                else
                {
                    prevCamLevel = camRideLevel;
                    targetHeight = CurrentControllerStandingHeight;
                }

                targetCamLevel = ControllerHeightChange(targetHeight - prevHeight);
                controllerMounted = false;
                playerMotor.IsCrouching = false;
            }

            timerTick();

            UpdateCameraPosition(Mathf.Lerp(prevCamLevel, targetCamLevel, camLerp_T));

            if (camTimer >= timerMax)
            {
                timerResetAction();
            }
        }

        private void DoUnsinking()
        {
            if (controllerSink)
            {
                float baseHeight;
                float displacement = 0;
                prevCamLevel = camSwimLevel;
                if (playerMotor.IsRiding)
                {
                    baseHeight = controllerRideHeight;
                    displacement = controllerSwimHorseDisplacement;
                    prevCamLevel += displacement;
                }
                else
                {
                    baseHeight = CurrentControllerStandingHeight;
                }

                float height = controllerSwimHeight + displacement;
                float heightLoss = baseHeight - height;

                targetCamLevel = ControllerHeightChange(heightLoss);
                controllerSink = false;
                IsInWaterTile = false;
                playerMotor.IsCrouching = false;
                GameManager.Instance.PlayerEnterExit.IsPlayerSwimming = false;
            }

            timerTick();

            UpdateCameraPosition(Mathf.Lerp(prevCamLevel, targetCamLevel, camLerp_T));

            if (camTimer >= timerMax)
            {
                timerResetAction();
            }
        }

        private void DoSinking()
        {
            if (!controllerSink)
            {
                float baseHeight;
                float displacement = 0;
                if (!playerMotor.IsCrouching)
                {
                    if (playerMotor.IsRiding)
                    {
                        baseHeight = controllerRideHeight;
                        displacement = controllerSwimHorseDisplacement;
                        prevCamLevel = camRideLevel;
                    }
                    else
                    {
                        baseHeight = CurrentControllerStandingHeight;
                        prevCamLevel = camStandLevel;
                    }
                }
                else
                {
                    baseHeight = controllerCrouchHeight;
                    prevCamLevel = camCrouchLevel;
                }

                float height = controllerSwimHeight + displacement;
                float heightLoss = baseHeight - height;

                targetCamLevel = ControllerHeightChange(-1 * heightLoss);
                controllerSink = true;
                IsInWaterTile = true;
                playerMotor.IsCrouching = false;
                GameManager.Instance.PlayerEnterExit.IsPlayerSwimming = true;
            }

            timerTick();

            UpdateCameraPosition(Mathf.Lerp(prevCamLevel, targetCamLevel, camLerp_T));

            if (camTimer >= timerMax)
            {
                timerResetAction();
            }
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Increment timer and camera LERP T value
        /// </summary>
        private void timerTick()
        {
            camTimer += Time.deltaTime;
            camLerp_T = Mathf.Clamp((camTimer / timerMax), 0, 1);
        }

        /// <summary>
        /// Reset the camera timer and action
        /// </summary>
        private void timerResetAction()
        {
            camTimer = 0f;
            heightAction = HeightChangeAction.DoNothing;
        }

        /// <summary>
        /// Set new camera position
        /// </summary>
        /// <param name="yPosMod">Y amount to change camera position</param>
        private void UpdateCameraPosition(float yPosMod)
        {
            Vector3 camPos = mainCamera.transform.localPosition;
            headBobber.RestPos = new Vector3(headBobber.RestPos.x, yPosMod);
            mainCamera.transform.localPosition = new Vector3(camPos.x, yPosMod, camPos.z);
        }

        /// <summary>
        /// Change controller height and position and return target height for camera to change to
        /// </summary>
        /// <param name="heightChange">Amount to modify controller height</param>
        /// <returns>the target height the camera should change to</returns>
        private float ControllerHeightChange(float heightChange)
        {
            bool dismounting = (heightAction == HeightChangeAction.DoDismounting);
            bool mounting = (heightAction == HeightChangeAction.DoMounting);
            controller.height = GetNearbyFloat(controller.height + heightChange);
            controller.transform.position += new Vector3(0, heightChange / 2f);

            return controller.height / 2f;
        }

        /// <summary>
        /// Anti-floating point roundoff
        /// </summary>
        /// <param name="value">The value to evaluate</param>
        /// <returns></returns>
        private float GetNearbyFloat(float value)
        {
            if (CloseEnough(value, CurrentControllerStandingHeight))
                return CurrentControllerStandingHeight;
            else if (CloseEnough(value, controllerCrouchHeight))
                return controllerCrouchHeight;
            else if (CloseEnough(value, controllerRideHeight))
                return controllerRideHeight;
            else if (CloseEnough(value, controllerSwimHeight))
                return controllerSwimHeight;
            else if (CloseEnough(value, controllerSwimHeight + controllerSwimHorseDisplacement))
                return controllerSwimHeight + controllerSwimHorseDisplacement;

            // Couldn't get to an exact round-off within tolerance - return a fixed value based on current state rather than simply returning 0
            // This fixes method from simply crunching player into a tiny ball during some edge cases
            // Could probably replace the above round-off method completely with this, but just using as an improved failover for now
            if (playerMotor.IsRiding && playerMotor.IsSwimming)
                return controllerSwimHeight + controllerSwimHorseDisplacement;
            else if (playerMotor.IsRiding && !playerMotor.IsSwimming)
                return controllerRideHeight;
            else if (playerMotor.IsSwimming)
                return controllerSwimHeight;
            else if (playerMotor.IsCrouching)
                return controllerCrouchHeight;
            else
                return CurrentControllerStandingHeight;
        }

        private bool CloseEnough(float value1, float value2, float acceptableDifference = 0.01f)
        {
            return Math.Abs(value1 - value2) <= acceptableDifference;
        }

        /// <summary>
        /// Does the player have enough room to stand from crouching position?
        /// </summary>
        /// <returns>returns true if enough room</returns>
        private bool CanStand()
        {
            float distance = camCrouchToStandDist;

            Ray ray = new Ray(controller.transform.position, Vector3.up);
            return !Physics.SphereCast(ray, controller.radius, distance);
        }
        #endregion

        #region Load Game Handling
        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            PlayerPositionData_v1 savePos = saveData.playerData.playerPosition;

            // save is crouched
            if (!savePos.isCrouching && playerMotor.IsCrouching)
            {
                heightAction = HeightChangeAction.DoStanding;
            }
            else if (savePos.isCrouching && !playerMotor.IsCrouching)
            {
                heightAction = HeightChangeAction.DoCrouching;
            }

            toggleRiding = playerMotor.IsRiding;
            toggleSink = GameManager.Instance.PlayerMotor.OnExteriorWater == PlayerMotor.OnExteriorWaterMethod.Swimming;
            forcedSwimCrouch = levitateMotor.IsSwimming;
        }
        #endregion
    }
}
