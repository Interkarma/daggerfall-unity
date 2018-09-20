using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;

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
        public bool ForcedSwimCrouch { get { return forcedSwimCrouch; } set { } }
        private bool forcedSwimCrouch = true;

        private PlayerMotor playerMotor;
        private CharacterController controller;
        private HeadBobber headBobber;
        private LevitateMotor levitateMotor;
        private ClimbingMotor climbingMotor;
        private Camera mainCamera;
        private float controllerStandHeight = 1.78f;
        private float controllerCrouchHeight = 0.45f;
        private float controllerRideHeight = 2.6f;   // Height of a horse plus seated rider. (1.6m + 1m)
        private float controllerSwimHeight = 0.30f;
        private float controllerSwimHorseDisplacement = 0.30f; // amount added to swim height if on horse
        private float eyeHeight = 0.09f;         // Eye height is 9cm below top of capsule.
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

        private void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            controller = GetComponent<CharacterController>();
            headBobber = GetComponent<HeadBobber>();
            mainCamera = GameManager.Instance.MainCamera;
            levitateMotor = GetComponent<LevitateMotor>();
            climbingMotor = GetComponent<ClimbingMotor>();
            camSwimLevel = controllerSwimHeight / 2f;
            camCrouchLevel = controllerCrouchHeight / 2f;
            camStandLevel = controllerStandHeight / 2f;
            camRideLevel = controllerRideHeight / 2f - eyeHeight;
            //camSwimToCrouchDist = (controllerCrouchHeight - controllerSwimHeight) / 2f;
            camCrouchToStandDist = (controllerStandHeight - controllerCrouchHeight) / 2f;
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
            bool pressedCrouch = InputManager.Instance.ActionComplete(InputManager.Actions.Crouch);
            bool climbing = climbingMotor.IsClimbing;
            timerMax = timerSlow;
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
            else if (!riding && !onWater)
            {
                // if we crouch out of water
                if (!swimming && pressedCrouch)
                {
                    timerMax = timerFast;
                    // Toggle crouching
                    if (crouching)
                        heightAction = HeightChangeAction.DoStanding;
                    else
                        heightAction = HeightChangeAction.DoCrouching;
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
                else if (swimming && !forcedSwimCrouch)
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
        private void DoCrouch() // first lower camera, Controller height last 
        {
            float prevHeight = controller.height;

            timerTick();

            UpdateCameraPosition(Mathf.Lerp(prevHeight/2f, camCrouchLevel, camLerp_T));

            if (camTimer >= timerMax)
            {
                float targetHeight = controllerCrouchHeight;
                ControllerHeightChange(targetHeight - prevHeight);
                UpdateCameraPosition(mainCamera.transform.localPosition.y + camCrouchToStandDist);

                timerResetAction();
                playerMotor.IsCrouching = true;
            }
        }
        private void DoStand() // adjust height first, camera last
        {
            float prevHeight = controller.height;
            
            if (playerMotor.IsCrouching)
            {
                float targetHeight = controllerStandHeight;
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

                targetCamLevel = ControllerHeightChange(targetHeight-prevHeight);
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
                    targetHeight = controllerStandHeight;
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
                    baseHeight = controllerStandHeight;
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
                        baseHeight = controllerStandHeight;
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
            float eyeChange = 0;
            if (!(GameManager.Instance.PlayerMotor.OnExteriorWater == PlayerMotor.OnExteriorWaterMethod.Swimming))
            { 
                if (dismounting)
                    eyeChange = -1 * eyeHeight;
                else if (mounting)
                    eyeChange = eyeHeight;
            }
            controller.transform.position += new Vector3(0, heightChange / 2 + eyeChange);

            return controller.height / 2 + eyeChange;
        }
        /// <summary>
        /// Anti-floating point roundoff
        /// </summary>
        /// <param name="value">The value to evaluate</param>
        /// <returns></returns>
        private float GetNearbyFloat(float value)
        {
            if (CloseEnough(value, controllerStandHeight))
                return controllerStandHeight;
            else if (CloseEnough(value, controllerCrouchHeight))
                return controllerCrouchHeight;
            else if (CloseEnough(value, controllerRideHeight))
                return controllerRideHeight;
            else if (CloseEnough(value, controllerSwimHeight))
                return controllerSwimHeight;
            else if (CloseEnough(value, controllerSwimHeight + controllerSwimHorseDisplacement))
                return controllerSwimHeight + controllerSwimHorseDisplacement;

            return 0;
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