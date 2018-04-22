using DaggerfallWorkshop.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkShop.Game
{
    public enum CrouchToggleAction
    {
        DoNothing,
        DoStanding,
        DoCrouching
    }

    //Added the RequireComponent attribute to make sure that following components are indeed on this GameObject, since they are require to make this code work
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(HeadBobber))]
    public class Croucher : MonoBehaviour
    {
        private CrouchToggleAction toggleAction;
        public CrouchToggleAction ToggleAction
        {
            get { return toggleAction; }
            set { toggleAction = value; }
        }
        private PlayerMotor playerMotor;
        private CharacterController controller;
        private HeadBobber headBobber;
        private Camera mainCamera;
        private float crouchHeight;
        private float standHeight;
        private float controllerPosChangeDistance;
        private float camCrouchLevel;
        private float camStandLevel;
        private float crouchTimer;

        private const float timerMax = 0.1f;

        private void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            controller = GetComponent<CharacterController>();
            headBobber = GetComponent<HeadBobber>();
            mainCamera = GameManager.Instance.MainCamera;
            standHeight = playerMotor.standingHeight;
            crouchHeight = playerMotor.crouchingHeight;
            controllerPosChangeDistance = (standHeight - crouchHeight) / 2f;

            camStandLevel = mainCamera.transform.localPosition.y; //With the assumption that the camera begins at correct standing position height
            camCrouchLevel = camStandLevel - controllerPosChangeDistance; //we want the camera to lower the same amount as the character
        }

        private void Update()
        {
            if (toggleAction == CrouchToggleAction.DoNothing)
                return;

            if (toggleAction == CrouchToggleAction.DoCrouching)
                DoCrouch();
            else
                DoStand();
        }
        private void DoCrouch() // first lower camera, perform snap crouch last 
        {
            bool bFinished = false;

            crouchTimer += Time.deltaTime;
            float t = Mathf.Clamp((crouchTimer / timerMax), 0, 1);

            UpdateCameraPosition(Mathf.Lerp(camStandLevel, camCrouchLevel, t));

            bFinished = (crouchTimer >= timerMax);

            if (bFinished)
            {
                controller.height = crouchHeight;
                controller.transform.position -= new Vector3(0, controllerPosChangeDistance);
                UpdateCameraPosition(mainCamera.transform.localPosition.y + controllerPosChangeDistance);

                crouchTimer = 0f;
                toggleAction = CrouchToggleAction.DoNothing;
            }

        }
        private void DoStand() // perform snap crouch first, lower camera last
        {
            bool bFinished = false;

            if (controller.height < standHeight)
            {
                controller.height = standHeight;
                controller.transform.position += new Vector3(0, controllerPosChangeDistance);
            }

            if (controller.height >= standHeight)
            { 
                crouchTimer += Time.deltaTime;
                float t = Mathf.Clamp((crouchTimer / timerMax), 0, 1);
           
                UpdateCameraPosition(Mathf.Lerp(camCrouchLevel, camStandLevel, t));

                bFinished = (crouchTimer >= timerMax);
                if (bFinished)
                {
                    crouchTimer = 0f;
                    toggleAction = CrouchToggleAction.DoNothing;
                }
            }
        }

        private void UpdateCameraPosition(float yPosMod)
        {
            Vector3 camPos = mainCamera.transform.localPosition;
            headBobber.restPos.y = yPosMod;
            mainCamera.transform.localPosition = new Vector3(camPos.x, yPosMod, camPos.z);
        }

        private void ControllerCrouchToggle()
        {
            if (toggleAction == CrouchToggleAction.DoCrouching)
            {
                controller.height = crouchHeight;
                controller.transform.position -= new Vector3(0, controllerPosChangeDistance);
            }
            else if (toggleAction == CrouchToggleAction.DoStanding)
            {
                controller.height = standHeight;
                controller.transform.position += new Vector3(0, controllerPosChangeDistance);
            }
        }

        private bool CanStand()
        { 
            RaycastHit hit;
            float distance = controllerPosChangeDistance;

            Ray ray = new Ray(controller.transform.position, Vector3.up);
            return !Physics.Raycast(ray, out hit, distance); 
        }
    }
}