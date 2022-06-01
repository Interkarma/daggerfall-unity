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

using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Defines the Head-bobbing pattern used by HeadBobber
    /// </summary>
    public enum BobbingStyle
    {
        Crouching,
        Walking,
        Running,
        Horse,
        Swimming
    }

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerFootsteps))]
    [RequireComponent(typeof(PlayerEnterExit))]
    public class HeadBobber : MonoBehaviour
    {
        private BobbingStyle bobStyle = BobbingStyle.Walking;

        public BobbingStyle BobStyle
        {
            get { return bobStyle; }
        }

        private PlayerMotor playerMotor;
        private Camera mainCamera;
        private PlayerEnterExit playerEnterExit;
        private ClimbingMotor climbingMotor;

        private Vector3 restPos; //local position where your camera would rest when it's not bobbing.
        public Vector3 RestPos
        { get { return restPos; } set { restPos = value; } }

        //private float transitionSpeed = 20f; //smooths out the transition from moving to not moving.
        private float bobSpeed; //how quickly the player's head bobs.
        private float bobXAmount; //how dramatic the bob is in side motion.
        private float bobYAmount; //how dramatic the bob is in up/down motion.
        private float nodXAmount; // for the nodding motion
        private float nodYAmount;
        private const float bobScalar = 1.0f; // user controlled multiplier for strength of bob

        float bounceTimerDown;
        float bounceTimerUp;
        float timer = Mathf.PI / 2; //initialized as this value because this is where sin = 1. So, this will make the camera always start at the crest of the sin wave, simulating someone picking up their foot and starting to walk--you experience a bob upwards when you start walking as your foot pushes off the ground, the left and right bobs come as you walk.
        float beginTransitionTimer = 0; // timer for smoothing out beginning of headbob.
        float endTransitionTimer = 0; // timer for smoothing out end of headbob. 
        const float endTimerMax = 0.5f;
        //const float beginTimerMax = Mathf.PI;
        private bool bIsStopping;
        private bool readyToBounce;

        void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            playerEnterExit = GetComponent<PlayerEnterExit>();
            climbingMotor = GetComponent<ClimbingMotor>();
            mainCamera = GameManager.Instance.MainCamera;
            restPos = mainCamera.transform.localPosition;

            bobSpeed = GetComponent<PlayerFootsteps>().WalkStepInterval / 2.0f; // 1.20f;
            bIsStopping = false;
        }

        void Update()
        {
            if (!DaggerfallUnity.Settings.HeadBobbing ||
                GameManager.Instance.PlayerEntity.CurrentHealth < 1 ||
                GameManager.Instance.PlayerMouseLook.cursorActive ||
                GameManager.IsGamePaused ||
                climbingMotor.IsClimbing ||
			    !playerMotor.IsGrounded)
                return;

            GetBobbingStyle();
            SetParamsForBobbingStyle();

            Vector3 newCameraPosition = restPos;
            Vector3 newCameraRotation = new Vector3();
            getNewPos(ref newCameraPosition, ref newCameraRotation);
            mainCamera.transform.localPosition += newCameraPosition - mainCamera.transform.localPosition;
            mainCamera.transform.Rotate(newCameraRotation);
        }

        protected void GetBobbingStyle()
        {
            if (playerEnterExit.IsPlayerSwimming)
                bobStyle = BobbingStyle.Swimming;
            else if (playerMotor.IsRunning)
                bobStyle = BobbingStyle.Running;
            else if (playerMotor.IsCrouching)
                bobStyle = BobbingStyle.Crouching;
            else if (playerMotor.IsRiding)
                bobStyle = BobbingStyle.Horse;
            else
                bobStyle = BobbingStyle.Walking;
        }

        protected void SetParamsForBobbingStyle()
        {
            switch (bobStyle)
            {
                // TODO: adjust bob speed to match player footstep sound better
                case BobbingStyle.Crouching:
                    // lot of swaying side to side as shifting legs and pushing up and off each leg
                    bobXAmount = 0.08f * bobScalar;
                    bobYAmount = 0.07f * bobScalar;
                    nodXAmount = 0.5f;
                    nodYAmount = 0.2f;
                    break;
                case BobbingStyle.Walking:
                    // More y than x because walking is pretty balanced side to side, just head bounce
                    bobXAmount = 0.045f * bobScalar;
                    bobYAmount = 0.062f * bobScalar;
                    nodXAmount = 0.25f;
                    nodYAmount = 0.1f;
                    break;
                case BobbingStyle.Running:
                    // both legs pushing off ground and lots of leaning side to side.
                    bobXAmount = 0.09f * bobScalar;
                    bobYAmount = 0.11f * bobScalar;
                    nodXAmount = 0.6f;
                    nodYAmount = 0.15f;
                    break;
                case BobbingStyle.Horse:
                    // horse has 4 legs: balanced, most force pushes player up.
                    bobXAmount = 0.03f * bobScalar;
                    bobYAmount = 0.115f * bobScalar;
                    nodXAmount = 0.2f;
                    nodYAmount = 0.1f;
                    break;
                case BobbingStyle.Swimming:

                    bobXAmount = 0;
                    bobYAmount = 0;
                    nodXAmount = 0;
                    nodYAmount = 0;
                    break;
                default:
                    // error
                    break;
            }
        }

        protected void getNewPos(ref Vector3 newPosition, ref Vector3 newRotation)
        {
            float velocity = new Vector2(playerMotor.MoveDirection.x, playerMotor.MoveDirection.z).magnitude;
            float timeIncrement = velocity * bobSpeed * Time.deltaTime;

            if (InputManager.Instance.Horizontal != 0 || InputManager.Instance.Vertical != 0)
            {   // player is moving on ground
                if (endTransitionTimer > 0) // if we were stopping, but started again, re-initialize timer here.
                {
                    endTransitionTimer = 0;
                    timer = Mathf.PI;
                }

                timer += timeIncrement;
                beginTransitionTimer += timeIncrement;

                newPosition = PlotPath();
                newRotation = PlotRotation();

                if (beginTransitionTimer <= Mathf.PI)
                {
                    newPosition = InterpolateBeginTransition(newPosition); // smooth out start of player's movement
                }
                bIsStopping = true; // next branch of if/else will evaluate to true only after releasing keys.
                endTransitionTimer = 0;
            }
            else if (bIsStopping && endTransitionTimer <= endTimerMax)
            {
                // player is stopping moving now
                if (timer > 0)
                    timer = Mathf.Max(timer - timeIncrement, 0); // timer de-increments for rotation to return to original position
                beginTransitionTimer = 0; // reset

                endTransitionTimer += Time.deltaTime;

                newPosition = InterpolateEndTransition(endTransitionTimer);
                newRotation = PlotRotation();
            }
            else if (bIsStopping)// endTransitionTimer reached max
            {
                endTransitionTimer = 0;
                timer = Mathf.PI;
                bIsStopping = false;
            }

            if (timer > Mathf.PI * 2) //completed a full cycle on the unit circle. Reset to 0 to avoid bloated values.
            {
                timer = 0;
            }

            ApplySimpleBouncing(ref newPosition);
        }

        protected void ApplySimpleBouncing(ref Vector3 newPosition)
        {
            bool isGrounded = playerMotor.IsGrounded;
            bool isSwimming = playerEnterExit.IsPlayerSwimming;
            bool isClimbing = climbingMotor.IsClimbing;
            bool isLevitating = playerMotor.IsLevitating;
            float upSpeed = 1f,
                  downSpeed = 1f;

            if (isClimbing || isLevitating)
                return;

            if (isSwimming)
            {
                upSpeed = 0.40f;
                downSpeed = 0.1f;
            }

            if ((!isGrounded || isSwimming) && !readyToBounce)
            {
                readyToBounce = true;
                bounceTimerUp = 0f;
                bounceTimerDown = 0f;
            }
            else if (readyToBounce && (isGrounded || isSwimming))
            {
                const float bounceMax = 0.17f;
                const float timerMax = 0.10f;
                float t;
                // apply bob
                if (bounceTimerDown < timerMax)
                {
                    bounceTimerDown += Time.deltaTime * downSpeed;
                    t = (bounceTimerDown / timerMax);
                    newPosition += new Vector3(newPosition.x, Mathf.Lerp(restPos.y, restPos.y - bounceMax, t)) - newPosition;
                }
                else if (bounceTimerUp < timerMax)
                {
                    bounceTimerUp += Time.deltaTime * upSpeed;
                    t = (bounceTimerUp / timerMax);
                    newPosition += new Vector3(newPosition.x, Mathf.Lerp(restPos.y - bounceMax, restPos.y, t)) - newPosition;
                }
                else
                    readyToBounce = false;
            }
        }

        protected Vector3 PlotRotation()
        {
            // return vector for euler angles
            return new Vector3(Mathf.Abs(Mathf.Sin(timer) * nodXAmount), -1 * Mathf.Sin(timer) * nodYAmount);
        }

        protected Vector3 PlotPath()
        {
            return new Vector3(Mathf.Cos(timer) * bobXAmount, restPos.y + Mathf.Abs((Mathf.Sin(timer) * bobYAmount)), restPos.z); //abs val of y for a parabolic path
        }

        protected Vector3 InterpolateEndTransition(float endTimer) // interpolates a gradual path from moving to not moving.
        {
            float t = (endTimer / endTimerMax);
            Vector3 camPos = mainCamera.transform.localPosition;
            return Vector3.Lerp(camPos, restPos, t);
        }

        protected Vector3 InterpolateBeginTransition(Vector3 newPosition) // interpolates a gradual path from not moving to moving.
        {
            float t = (timer % Mathf.PI) / Mathf.PI;
            return Vector3.Lerp(restPos, newPosition, t);
        }
    }
}
