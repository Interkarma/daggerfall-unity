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
        DoDismounting
    }

    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(HeadBobber))]
    public class PlayerHeightChanger : MonoBehaviour
    {
        private HeightChangeAction heightAction;
        public HeightChangeAction HeightAction
        {
            get { return heightAction; }
            set { heightAction = value; }
        }
        private PlayerMotor playerMotor;
        private CharacterController controller;
        private HeadBobber headBobber;
        private Camera mainCamera;
        private float controllerStandHeight = 1.78f;
        private float controllerCrouchHeight = 0.45f;
        private float controllerRideHeight = 2.6f;   // Height of a horse plus seated rider. (1.6m + 1m)
        private float controllerSwimHeight = 0.30f;
        private float eyeHeight = 0.09f;         // Eye height is 9cm below top of capsule.
        private float camCrouchToStandDist;
        private float camRideToStandDist;
        private float camCrouchLevel;
        private float camStandLevel;
        private float camRideLevel;
        private float camSwimLevel;
        private float camTimer;
        private const float timerMax = 0.1f;
        private float camLerp_T;  // for lerping to new camera position
        private bool toggleRiding;
        private bool controllerMounted;


        private void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            controller = GetComponent<CharacterController>();
            headBobber = GetComponent<HeadBobber>();
            mainCamera = GameManager.Instance.MainCamera;
            camCrouchToStandDist = (controllerStandHeight - controllerCrouchHeight) / 2f;
            camRideToStandDist = (controllerRideHeight - controllerStandHeight) / 2f - eyeHeight;
            camCrouchLevel = controllerCrouchHeight / 2f;
            camStandLevel = controllerStandHeight / 2f;
            camRideLevel = controllerRideHeight / 2f - eyeHeight;
            camSwimLevel = controllerSwimHeight / 2f;

            // Use event to set whether player is crouched on load
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
        }

        /// <summary>
        /// Determines what Height-changing action should be taken based on player's input and PlayerMotor.IsRiding
        /// </summary>
        public void HandlePlayerInput()
        {
            if (playerMotor.IsRiding && !toggleRiding)
            { 
                heightAction = HeightChangeAction.DoMounting;
                toggleRiding = true;
            }   
            else if (!playerMotor.IsRiding && toggleRiding)
            {
                heightAction = HeightChangeAction.DoDismounting;
                toggleRiding = false;
            }
            else if (!playerMotor.IsRiding)
            {
                // Toggle crouching
                if (InputManager.Instance.ActionComplete(InputManager.Actions.Crouch))
                {
                    if (playerMotor.IsCrouching)
                        heightAction = HeightChangeAction.DoStanding;
                    else
                        heightAction = HeightChangeAction.DoCrouching;
                }
            }

                
        }
        /// <summary>
        /// Continue calling actions to increment camera towards destination.
        /// </summary>
        private void Update()
        {
            if (heightAction == HeightChangeAction.DoNothing)
                return;

            if (heightAction == HeightChangeAction.DoCrouching)
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
            timerTick();

            UpdateCameraPosition(Mathf.Lerp(camStandLevel, camCrouchLevel, camLerp_T));

            if (camTimer >= timerMax)
            {
                ControllerHeightChange(controllerCrouchHeight - controllerStandHeight, -1 * camCrouchToStandDist);
                UpdateCameraPosition(mainCamera.transform.localPosition.y + camCrouchToStandDist);

                timerResetAction();
                playerMotor.IsCrouching = true;
            }
        }
        private void DoStand() // adjust height first, camera last
        {
            if (playerMotor.IsCrouching)
            {
                ControllerHeightChange(controllerStandHeight - controllerCrouchHeight, camCrouchToStandDist);
                playerMotor.IsCrouching = false;
            }
            timerTick();

            UpdateCameraPosition(Mathf.Lerp(camCrouchLevel, camStandLevel, camLerp_T));

            if (camTimer >= timerMax)
            {
                timerResetAction();
            }
        }
        private void DoMount() // adjust height first, camera last
        {
            float prevControllerHeight = playerMotor.IsCrouching ? controllerCrouchHeight : controllerStandHeight;
            if (!controllerMounted)
            { 
                ControllerHeightChange(controllerRideHeight - prevControllerHeight, camRideToStandDist);
                controllerMounted = true;
                playerMotor.IsCrouching = false;
            }

            timerTick();

            float prevCamLevel = playerMotor.IsCrouching ? camCrouchLevel : camStandLevel;
            UpdateCameraPosition(Mathf.Lerp(prevCamLevel, camRideLevel, camLerp_T));

            if (camTimer >= timerMax)
            {
                timerResetAction();
            }
        }
        private void DoDismount() // adjust height first, camera last
        {
            if (controllerMounted)
            {
                ControllerHeightChange(controllerStandHeight - controllerRideHeight, -1 * camRideToStandDist);
                controllerMounted = false;
                playerMotor.IsCrouching = false;
            }

            timerTick();

            UpdateCameraPosition(Mathf.Lerp(camRideLevel, camStandLevel, camLerp_T));

            if (camTimer >= timerMax)
            {
                timerResetAction();
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Anti-floating point roundoff
        /// </summary>
        /// <param name="value">The value to evaluate</param>
        /// <returns></returns>
        private float GetNearbyValue(float value)
        {
            if (CloseEnough(value, controllerStandHeight))
                return controllerStandHeight;
            else if (CloseEnough(value, controllerCrouchHeight))
                return controllerCrouchHeight;
            else if (CloseEnough(value, controllerRideHeight))
                return controllerRideHeight;
            else if (CloseEnough(value, controllerSwimHeight))
                return controllerSwimHeight;

            return 0;
    }
        private bool CloseEnough(float value1, float value2, float acceptableDifference = 0.01f)
        {
            return Math.Abs(value1 - value2) <= acceptableDifference;
        }
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
        /// Change controller height and position
        /// </summary>
        /// <param name="heightChange">Amount to modify controller height</param>
        /// <param name="camChangeAmt">Amount to change controller position</param>
        private void ControllerHeightChange(float heightChange, float camChangeAmt)
        {
            controller.height = GetNearbyValue(controller.height + heightChange);
            controller.transform.position += new Vector3(0, camChangeAmt);
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
        /// <summary>
        /// Immediately crouch/stand to match crouch state.
        /// </summary>
        private void SnapToggleCrouching()
        {
            if (playerMotor.IsCrouching && controller.height != controllerCrouchHeight)
            {
                controller.height = controllerCrouchHeight;
                Vector3 pos = controller.transform.position;
                pos.y -= (controllerStandHeight - controllerCrouchHeight) / 2.0f;
                controller.transform.position = pos;
            }
            else if (!playerMotor.IsCrouching && controller.height != controllerStandHeight)
            {
                controller.height = controllerStandHeight;
                Vector3 pos = controller.transform.position;
                pos.y += (controllerStandHeight - controllerCrouchHeight) / 2.0f;
                controller.transform.position = pos;
            }
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            playerMotor.IsCrouching = saveData.playerData.playerPosition.isCrouching;
            controllerMounted = playerMotor.IsRiding;
            SnapToggleCrouching();
        }
        #endregion
    }
}