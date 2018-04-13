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
                float change = toggleActionSpeed * Time.deltaTime;
                if (toggleAction == CrouchToggleAction.DoCrouching)
                    change = -1 * change;

                //if (WayIsBlocked(change))
                /*if ((controller.collisionFlags & CollisionFlags.Above) != 0)
                {
                    // reverse direction as if bumped into the blocking object
                    if (toggleAction == CrouchToggleAction.DoCrouching)
                    { 
                        toggleAction = CrouchToggleAction.DoStanding;
                        playerMotor.IsCrouching = false;
                    }
                    else
                    {
                        toggleAction = CrouchToggleAction.DoCrouching;
                        playerMotor.IsCrouching = true;
                    }
                }*/

                controller.height = Mathf.Clamp(controller.height + change, crouchHeight, standHeight);

                controller.transform.position += new Vector3(0, change / 2.0f);
                if (toggleAction == CrouchToggleAction.DoCrouching)
                    bFinished = (controller.height <= crouchHeight);
                else
                    bFinished = (controller.height >= standHeight);
            }

            if (bFinished)
            {
                //Debug.Log("Controller.height = " + controller.height);
                Debug.Log("Controller.transform.position.y" + controller.transform.position.y);

                toggleAction = CrouchToggleAction.DoNothing;
            }
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