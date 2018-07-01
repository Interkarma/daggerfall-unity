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
    public class PlayerHeightChanger : MonoBehaviour
    {
        private HeightChangeAction heightAction;
        public HeightChangeAction HeightAction
        {
            get { return heightAction; }
            set { heightAction = value; }
        }
        public bool OnWater { get; private set; }
        private PlayerMotor playerMotor;
        private CharacterController controller;
        private HeadBobber headBobber;
        private Camera mainCamera;
        private float controllerStandHeight = 1.78f;
        private float controllerCrouchHeight = 0.45f;
        private float controllerRideHeight = 2.6f;   // Height of a horse plus seated rider. (1.6m + 1m)
        private float controllerSwimHeight = 0.30f;
        private float controllerSwimHorseDisplacement = 0.20f; // amount added to swim height if on horse
        private float eyeHeight = 0.09f;         // Eye height is 9cm below top of capsule.
        private float targetCamLevel;
        private float prevCamLevel;
        private float camCrouchLevel;
        private float camStandLevel;
        private float camRideLevel;
        private float camSwimLevel;
        private float camSwimToCrouchDist;
        private float camCrouchToStandDist;
        private float camStandToRideDist;
        private float camTimer;
        private const float timerMax = 0.1f;
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
            camSwimLevel = controllerSwimHeight / 2f;
            camCrouchLevel = controllerCrouchHeight / 2f;
            camStandLevel = controllerStandHeight / 2f;
            camRideLevel = controllerRideHeight / 2f - eyeHeight;
            camSwimToCrouchDist = (controllerCrouchHeight - controllerSwimHeight) / 2f;
            camCrouchToStandDist = (controllerStandHeight - controllerCrouchHeight) / 2f;
            camStandToRideDist = (controllerRideHeight - controllerStandHeight) / 2f;

            // Use event to set whether player is crouched on load
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
        }

        /// <summary>
        /// Determines what Height-changing action should be taken based on player's input and PlayerMotor.IsRiding
        /// </summary>
        public void DecideHeightAction()
        {
            OnWater = GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 0 && playerMotor.IsGrounded;
            if (OnWater && !toggleSink)
            {
                heightAction = HeightChangeAction.DoSinking;
                toggleSink = true;
            }
            else if (!OnWater && toggleSink)
            {
                heightAction = HeightChangeAction.DoUnsinking;
                toggleSink = false;
            }
            else if (playerMotor.IsRiding && !toggleRiding)
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
            if (heightAction == HeightChangeAction.DoNothing || GameManager.IsGamePaused)
                return;
            Debug.LogFormat("IsGrounded = {0}, \nHeight = {1}", playerMotor.IsGrounded, controller.height);
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
            timerTick();

            UpdateCameraPosition(Mathf.Lerp(camStandLevel, camCrouchLevel, camLerp_T));

            if (camTimer >= timerMax)
            {
                ControllerHeightChange(controllerCrouchHeight - controllerStandHeight);
                UpdateCameraPosition(mainCamera.transform.localPosition.y + camCrouchToStandDist);

                timerResetAction();
                playerMotor.IsCrouching = true;
            }
        }
        private void DoStand() // adjust height first, camera last
        {
            if (playerMotor.IsCrouching)
            {
                ControllerHeightChange(controllerStandHeight - controllerCrouchHeight);
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
            float prevCamLevel = 0;
            if (!controllerMounted)
            {
                float height = controllerRideHeight; 
                if (playerMotor.IsCrouching)
                {
                    height -= controllerCrouchHeight;
                    prevCamLevel = camCrouchLevel;
                }
                else
                {
                    height -= controllerStandHeight;
                    prevCamLevel = camStandLevel;
                }

                ControllerHeightChange(height);
                controllerMounted = true;
                playerMotor.IsCrouching = false;
            }

            timerTick();

            // TODO: can we mount in the water?
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
                ControllerHeightChange(controllerStandHeight - controllerRideHeight);
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
        private void DoUnsinking()
        {
            throw new NotImplementedException();
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
                    prevCamLevel = camStandLevel;
                }

                // TODO: lower just the camera
                float heightLoss = controllerSwimHeight + displacement;
                float height = baseHeight - heightLoss;
                //controller.radius = 0.05f;
                targetCamLevel = ControllerHeightChange(-1 * height);
                controllerSink = true;
                playerMotor.IsCrouching = false;
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
        /// <param name="camChangeAmt">Amount to change controller position</param>
        /// <returns>the target height the camera should change to</returns>
        private float ControllerHeightChange(float heightChange)
        {
            controller.height += heightChange;
            float eyeChange = 0;
            if (heightAction == HeightChangeAction.DoDismounting)
                eyeChange = -1 * eyeHeight;
            else if (heightAction == HeightChangeAction.DoMounting)
                eyeChange = eyeHeight;

            controller.transform.position += new Vector3(0, heightChange / 2 + eyeChange);
            return controller.height / 2 + eyeChange;
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