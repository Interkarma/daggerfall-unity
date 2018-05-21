using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Serialization;
using System.Collections;
using System.Collections.Generic;
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
        DoDismounting
    }

    //Added the RequireComponent attribute to make sure that following components are indeed on this GameObject, since they are require to make this code work
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
        public float standHeight = 1.78f;
        public float crouchHeight = 0.45f;
        private float rideHeight = 2.6f;   // Height of a horse plus seated rider. (1.6m + 1m)
        private float eyeHeight = 0.09f;         // Eye height is 9cm below top of capsule.
        private float crouchChangeDistance;
        //private float rideChangeDistance;
        private float camCrouchLevel;
        private float camStandLevel;
        private float camRideLevel;
        private float heightTimer;
        private bool toggleRiding;
        private const float timerMax = 0.1f;

        private void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            controller = GetComponent<CharacterController>();
            headBobber = GetComponent<HeadBobber>();
            mainCamera = GameManager.Instance.MainCamera;
            crouchChangeDistance = (standHeight - crouchHeight) / 2f;
            //rideChangeDistance = (rideHeight - standHeight) / 2f - eyeHeight;
            camCrouchLevel = crouchHeight / 2f;
            camStandLevel = standHeight / 2f;
            camRideLevel = rideHeight / 2f - eyeHeight;

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

        private void DoCrouch() // first lower camera, Controller height last 
        {
            heightTimer += Time.deltaTime;
            float t = Mathf.Clamp((heightTimer / timerMax), 0, 1);

            UpdateCameraPosition(Mathf.Lerp(camStandLevel, camCrouchLevel, t));

            if (heightTimer >= timerMax)
            {
                ControllerHeightChange();
                UpdateCameraPosition(mainCamera.transform.localPosition.y + crouchChangeDistance);

                heightTimer = 0f;
                heightAction = HeightChangeAction.DoNothing;
            }
        }
        private void DoStand() // adjust height first, camera last
        {
            if (controller.height == crouchHeight)
                ControllerHeightChange();

            heightTimer += Time.deltaTime;
            float t = Mathf.Clamp((heightTimer / timerMax), 0, 1);
           
            UpdateCameraPosition(Mathf.Lerp(camCrouchLevel, camStandLevel, t));

            if (heightTimer >= timerMax)
            {
                heightTimer = 0f;
                heightAction = HeightChangeAction.DoNothing;
            }
        }
        private void DoMount() // adjust height first, camera last
        {
            if (controller.height != rideHeight)
                ControllerHeightChange();

            heightTimer += Time.deltaTime;
            float t = Mathf.Clamp((heightTimer / timerMax), 0, 1);

            float prevCamLevel = playerMotor.IsCrouching ? camCrouchLevel : camStandLevel;
            UpdateCameraPosition(Mathf.Lerp(prevCamLevel, camRideLevel, t));

            if (heightTimer >= timerMax)
            {
                heightTimer = 0f;
                heightAction = HeightChangeAction.DoNothing;
                playerMotor.IsRiding = true;
            }
        }
        private void DoDismount() // adjust height first, camera last
        {
            if (controller.height == rideHeight)
                ControllerHeightChange();

            heightTimer += Time.deltaTime;
            float t = Mathf.Clamp((heightTimer / timerMax), 0, 1);

            UpdateCameraPosition(Mathf.Lerp(camRideLevel, camStandLevel, t));

            if (heightTimer >= timerMax)
            {
                heightTimer = 0f;
                heightAction = HeightChangeAction.DoNothing;
                playerMotor.IsRiding = false;
            }
        }

        private void UpdateCameraPosition(float yPosMod)
        {
            Vector3 camPos = mainCamera.transform.localPosition;
            headBobber.RestPos = new Vector3(headBobber.RestPos.x, yPosMod);
            mainCamera.transform.localPosition = new Vector3(camPos.x, yPosMod, camPos.z);
        }

        private void ControllerHeightChange()
        {
            if (heightAction == HeightChangeAction.DoCrouching)
            {
                controller.height = crouchHeight;
                controller.transform.position -= new Vector3(0, crouchChangeDistance);
                playerMotor.IsCrouching = true;
            }
            else if (heightAction == HeightChangeAction.DoStanding)
            {
                controller.height = standHeight;
                controller.transform.position += new Vector3(0, crouchChangeDistance);
                playerMotor.IsCrouching = false;
            }
            else if (heightAction == HeightChangeAction.DoMounting)
            {
                controller.height = rideHeight;
                Vector3 pos = controller.transform.position;
                float prevHeight = playerMotor.IsCrouching ? crouchHeight : standHeight;
                pos.y += (rideHeight - prevHeight) / 2.0f;
                controller.transform.position = pos;
                playerMotor.IsCrouching = false;
            }
            else if (heightAction == HeightChangeAction.DoDismounting)
            {
                controller.height = standHeight;
                Vector3 pos = controller.transform.position;
                pos.y -= (rideHeight - standHeight) / 2.0f;
                controller.transform.position = pos;
                playerMotor.IsCrouching = false;
            }
        }

        /// <summary>
        /// Does the player have enough room to stand from crouching position?
        /// </summary>
        /// <returns>returns true if enough room</returns>
        private bool CanStand()
        { 
            float distance = crouchChangeDistance;

            Ray ray = new Ray(controller.transform.position, Vector3.up); 
            return !Physics.SphereCast(ray, controller.radius, distance);
        }

        private void SnapToggleCrouching()
        {
            if (playerMotor.IsCrouching && controller.height != crouchHeight)
            {
                controller.height = crouchHeight;
                Vector3 pos = controller.transform.position;
                pos.y -= (standHeight - crouchHeight) / 2.0f;
                controller.transform.position = pos;
            }
            else if (!playerMotor.IsCrouching && controller.height != standHeight)
            {
                controller.height = standHeight;
                Vector3 pos = controller.transform.position;
                pos.y += (standHeight - crouchHeight) / 2.0f;
                controller.transform.position = pos;
            }
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            playerMotor.IsCrouching = saveData.playerData.playerPosition.isCrouching;
            SnapToggleCrouching();
        }
    }
}