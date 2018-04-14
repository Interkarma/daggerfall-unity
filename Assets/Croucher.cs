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
    public class Croucher : MonoBehaviour
    {
        private CrouchToggleAction toggleAction;
        public CrouchToggleAction ToggleAction
        {
            get { return toggleAction; }
            set { toggleAction = value; }
        }
        public float toggleActionSpeed;
        private PlayerMotor playerMotor;
        private CharacterController controller;
        private float crouchHeight;
        private float standHeight;

        private void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            controller = GetComponent<CharacterController>();
            standHeight = playerMotor.standingHeight;
            crouchHeight = playerMotor.crouchingHeight;
            toggleAction = CrouchToggleAction.DoNothing;
            toggleActionSpeed = 12f;         
        }

        // perform whatever action CrouchToggleAction is set to.
        private void Update()
        {
            if (toggleAction == CrouchToggleAction.DoNothing)
                return;

            bool bFinished = false;

            if (toggleAction != CrouchToggleAction.DoNothing)
            {
                float yChangePerFrame = toggleActionSpeed * Time.deltaTime;
                float upCollisionDistance = standHeight / 2f;
                float downCollisionDistance = crouchHeight / 2f;
                bool bHitHead = (WayIsBlocked(upCollisionDistance) && toggleAction == CrouchToggleAction.DoStanding);
                bool bHitButt = (WayIsBlocked(downCollisionDistance) && toggleAction == CrouchToggleAction.DoCrouching); ;

                if (toggleAction == CrouchToggleAction.DoCrouching)
                    yChangePerFrame *= -1;
                
                if (bHitButt || bHitHead)
                {
                    // reverse direction as if bumped into the blocking object
                    if (bHitButt)
                        toggleAction = CrouchToggleAction.DoStanding;
                    else
                        toggleAction = CrouchToggleAction.DoCrouching;

                    playerMotor.IsCrouching = !playerMotor.IsCrouching;
                    yChangePerFrame *= -1; // reverse player Y direction
                }

                controller.height = Mathf.Clamp(controller.height + yChangePerFrame, crouchHeight, standHeight);

                controller.transform.position += new Vector3(0, yChangePerFrame / 2.0f);
                if (toggleAction == CrouchToggleAction.DoCrouching)
                    bFinished = (controller.height <= crouchHeight);
                else
                    bFinished = (controller.height >= standHeight);
            }

            if (bFinished)
            {
                //Debug.Log("Controller.height = " + controller.height);
                //Debug.Log("Controller.transform.position.y" + controller.transform.position.y);

                toggleAction = CrouchToggleAction.DoNothing;
            }
        }

        private bool HitHead(float distance)
        {
            return (WayIsBlocked(distance) && toggleAction == CrouchToggleAction.DoStanding);
        }
        private bool HitButt(float distance)
        {
            return (WayIsBlocked(distance) && toggleAction == CrouchToggleAction.DoCrouching);
        }
        private bool WayIsBlocked(float distance)
        { 
            RaycastHit hit;
            Vector3 direction;
            if (toggleAction == CrouchToggleAction.DoCrouching)
                direction = Vector3.down;
            else
                direction = Vector3.up;

            Ray ray = new Ray(transform.position, direction);
            return Physics.Raycast(ray, out hit, distance); 
        }
    }
}