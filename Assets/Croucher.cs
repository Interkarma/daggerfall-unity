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
        private HeadBobber headBobber;
        private Camera mainCamera;
        private float crouchHeight;
        private float standHeight;
        private float crouchTimer;
        private float standTimer;
        private float timerSpeed = 3f;
        private const float timerMax = 0.3f;

        private void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            controller = GetComponent<CharacterController>();
            headBobber = GetComponent<HeadBobber>();
            mainCamera = GameManager.Instance.MainCamera;
            standHeight = playerMotor.standingHeight;
            crouchHeight = playerMotor.crouchingHeight;
            toggleAction = CrouchToggleAction.DoNothing;       
        }

        // perform whatever action CrouchToggleAction is set to.
        private void Update()
        {
            if (toggleAction == CrouchToggleAction.DoNothing)
                return;

            bool bFinished = false;

            if (toggleAction != CrouchToggleAction.DoNothing)
            {
                float newYAmt = 0f;
                if (toggleAction == CrouchToggleAction.DoCrouching)
                {
                    crouchTimer += Time.deltaTime * timerSpeed;
                    float t = (crouchTimer / timerMax);
                    newYAmt = Mathf.Lerp(0f, -1 * (standHeight - crouchHeight) / 2f, t);
                }
                else if (toggleAction == CrouchToggleAction.DoStanding)
                {
                    standTimer += Time.deltaTime * timerSpeed;
                    float t = (standTimer / timerMax);
                    newYAmt = Mathf.Lerp(0f, (standHeight - crouchHeight) / 2f, t);
                }

                #region BumpInto
                /*
                float upCollisionDistance = standHeight / 2f;
                float downCollisionDistance = crouchHeight / 2f;
                bool bHitHead = (WayIsBlocked(upCollisionDistance) && toggleAction == CrouchToggleAction.DoStanding);
                bool bHitButt = (WayIsBlocked(downCollisionDistance) && toggleAction == CrouchToggleAction.DoCrouching); 
                */
                #endregion
                #region BumpInto
                /*if (bHitButt || bHitHead)
                {
                    // reverse direction as if bumped into the blocking object
                    if (bHitButt)
                        toggleAction = CrouchToggleAction.DoStanding;
                    else
                        toggleAction = CrouchToggleAction.DoCrouching;

                    playerMotor.IsCrouching = !playerMotor.IsCrouching;
                    yChangePerFrame *= -1; // reverse player Y direction
                }*/
                #endregion

                ChangeMainCameraPosition(newYAmt);

                bFinished = (crouchTimer >= timerMax || standTimer >= timerMax);
            }

            if (bFinished)
            {
                if (toggleAction == CrouchToggleAction.DoCrouching)
                    ChangeMainCameraPosition((standHeight - crouchHeight) / 2f);
                else
                    ChangeMainCameraPosition( -1 * (standHeight - crouchHeight) / 2f);

                DoSnapToggleAction();
                crouchTimer = 0;
                standTimer = 0;
                toggleAction = CrouchToggleAction.DoNothing;
            }
        }

        private void DoSnapToggleAction()
        {
            if (playerMotor.IsCrouching)
            {
                controller.height = crouchHeight;
                Vector3 pos = controller.transform.position;
                pos.y -= (standHeight - crouchHeight) / 2.0f;
                controller.transform.position = pos;
            }
            else if (!playerMotor.IsCrouching)
            {
                controller.height = standHeight;
                Vector3 pos = controller.transform.position;
                pos.y += (standHeight - crouchHeight) / 2.0f;
                controller.transform.position = pos;
            }
        }

        private void ChangeMainCameraPosition(float yAmt)
        {   
            //Vector3 newPos = new Vector3(headBobber.restPos.x, headBobber.restPos.y + yAmt);
            //headBobber.restPos += newPos - headBobber.restPos;
            headBobber.restPos += new Vector3(0, yAmt) - headBobber.restPos;
            //mainCamera.transform.localPosition += headBobber.restPos - mainCamera.transform.localPosition;
            mainCamera.transform.localPosition += new Vector3(0, yAmt) - mainCamera.transform.localPosition;
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