using UnityEngine;
using System;
using System.Collections;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game
{
    public enum BobbingStyle
    {
        Crouching,
        Walking,
        Running,
        Horse
    }

    [RequireComponent(typeof(CharacterController))]
    public class HeadBobber : MonoBehaviour
    {
        private BobbingStyle bobStyle = BobbingStyle.Walking;
        public BobbingStyle BobStyle
        {
            get { return bobStyle; }
        }
        private PlayerMotor playerMotor;
        private Camera mainCamera;
        public Vector3 restPos; //local position where your camera would rest when it's not bobbing.
        public Vector3 camPos; // current positon of camera
        
        public float transitionSpeed = 10f; //smooths out the transition from moving to not moving.
        public float bobSpeed; //how quickly the player's head bobs.
        public float bobXAmount; //how dramatic the bob is in side motion.
        public float bobYAmount; //how dramatic the bob is in up/down motion.

        float timer = Mathf.PI / 2; //initialized as this value because this is where sin = 1. So, this will make the camera always start at the crest of the sin wave, simulating someone picking up their foot and starting to walk--you experience a bob upwards when you start walking as your foot pushes off the ground, the left and right bobs come as you walk.
        float beginTransitionTimer = 0; // timer for smoothing out beginning of headbob.

        void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            mainCamera = GameManager.Instance.MainCamera;
            camPos = mainCamera.transform.localPosition;
            restPos = mainCamera.transform.localPosition;
        }

        void Update()
        {
            //Debug.Log("HeadBobber running!");
            GetBobbingStyle();
            SetParamsForBobbingStyle();

            Vector3 newCameraPosition = getNewPos();
            mainCamera.transform.localPosition = newCameraPosition;
        }

        public virtual void GetBobbingStyle()
        {
            if (playerMotor.IsRunning)
                bobStyle = BobbingStyle.Running;
            else if (playerMotor.IsCrouching)
                bobStyle = BobbingStyle.Crouching;
            else if (playerMotor.IsRiding)
                bobStyle = BobbingStyle.Horse;
            else
                bobStyle = BobbingStyle.Walking;

        }

        public virtual void SetParamsForBobbingStyle()
        {
            switch (bobStyle)
            { 
                case BobbingStyle.Crouching:
                    bobSpeed = 3.3f;
                    bobXAmount = 0.08f;
                    bobYAmount = 0.08f;
                    break;
                case BobbingStyle.Walking:
                    bobSpeed = 6.6f;
                    bobXAmount = 0.045f;
                    bobYAmount = 0.062f;
                    break;
                case BobbingStyle.Running:
                    bobSpeed = 6.6f;
                    bobXAmount = 0.10f;
                    bobYAmount = 0.11f;
                    break;
                case BobbingStyle.Horse:
                    //Need to adjust this
                    bobSpeed = 6.6f;
                    bobXAmount = 0.03f;
                    bobYAmount = 0.115f;
                    break;
                default:
                    // error
                    break;
            }
        }

        public virtual Vector3 getNewPos()
        { 
            Vector3 newPosition;

            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 && playerMotor.IsGrounded) //moving
            {
                //Debug.Log("BobSpeed: " + bobSpeed + ", BobAmount: " + bobXAmount);
                timer += bobSpeed * Time.deltaTime;
                beginTransitionTimer += bobSpeed * Time.deltaTime;
                newPosition = PlotPath();

                if (beginTransitionTimer <= Mathf.PI / 2)
                    newPosition = InterpolateBeginTransition(); // smooth out beginning
            }
            else
            {
                timer = Mathf.PI / 2; //reinitialize
                beginTransitionTimer = 0; // reset

                newPosition = InterpolateEndTransition();
            }

            if (timer > Mathf.PI * 2) //completed a full cycle on the unit circle. Reset to 0 to avoid bloated values.
                timer = 0;
            // no reset for beginTransitionTimer until player releases movement buttons

            return newPosition;
        }

        public virtual Vector3 PlotPath()
        {
            return new Vector3(Mathf.Cos(timer) * bobXAmount, restPos.y + Mathf.Abs((Mathf.Sin(timer) * bobYAmount)), restPos.z); //abs val of y for a parabolic path
        }

        public Vector3 InterpolateEndTransition() // interpolates a gradual path from moving to not moving.
        {
            return new Vector3(Mathf.Lerp(camPos.x, restPos.x, transitionSpeed * Time.deltaTime), Mathf.Lerp(camPos.y, restPos.y, transitionSpeed * Time.deltaTime), Mathf.Lerp(camPos.z, restPos.z, transitionSpeed * Time.deltaTime)); //transition smoothly from walking to stopping.
        }

        public Vector3 InterpolateBeginTransition() // interpolates a gradual path from not moving to moving.
        {
            return new Vector3(Mathf.Lerp(camPos.x, restPos.x, transitionSpeed / Time.deltaTime), Mathf.Lerp(camPos.y, restPos.y, transitionSpeed / Time.deltaTime), Mathf.Lerp(camPos.z, restPos.z, transitionSpeed / Time.deltaTime)); //transition smoothly from walking to stopping.
        }


    }
}


