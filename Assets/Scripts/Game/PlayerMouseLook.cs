// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Avernite
// 
// Notes:
//

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Game
{
    // 
    // Adapted from the below code by FatiguedArtist.
    // http://forum.unity3d.com/threads/a-free-simple-smooth-mouselook.73117/
    //
    public class PlayerMouseLook : MonoBehaviour
    {
        public const float PITCH_MAX = 90;
        public const float PITCH_MIN = -90;

        const float PI_OVER_2 = Mathf.PI / 2;

        float pitchMax = PITCH_MAX;
        float pitchMin = PITCH_MIN;

        public bool enableMouseLook = true;

        Vector2 lookCurrent;
        Vector2 lookTarget;

        public bool invertMouseY = false;

        public bool enableSmoothing = true; // This field could be eliminated because a smoothing value of 0.0f means smoothing is disabled
        public const float SMOOTHING_MAX = 0.9f;
        float smoothing = 0.5f; // This value could come from user-defined settings if "Mouse Smoothing" checkbox is changed to a slider with values from 0.0 to 0.9

        /// <summary>
        /// Gets or sets degree of mouse-look camera smoothing (0.0 is none, 0.1 is a little, 0.9 is a lot).
        /// </summary>
        public float Smoothing
        {
            get { return smoothing; }
            set { smoothing = Mathf.Clamp(value, 0.0f, SMOOTHING_MAX); }
        }

        const float ZOOM_MIN = 1.0f;
        const float ZOOM_MAX = 4.0f;
        float zoomCurrent = ZOOM_MIN;
        bool zooming;

        float cameraPitch = 0.0f;
        float cameraYaw = 0.0f;

        public bool cursorActive;
        public bool lockCursor;
        public bool simpleCursorLock = false;
        private bool forceHideCursor;
        
        public Vector2 sensitivity = new Vector2(2, 2);
        public float sensitivityScale = 1.0f;
        public float joystickSensitivityScale = 1.0f;

        private bool immediateSet = true; // Alters Pitch and Yaw setter behaviors. Not meant to be changed anywhere but in ApplyLook method

        // Assign this if there's a parent object controlling motion, such as a Character Controller.
        // Yaw rotation will affect this object instead of the camera if set.
        public GameObject characterBody;

        /// <summary>
        /// Gets or sets pitch rotation of camera in degrees.
        /// </summary>
        public float Pitch
        {
            get { return cameraPitch * Mathf.Rad2Deg; }
            set
            {
                value = Mathf.Clamp(value, pitchMin, pitchMax);
                cameraPitch = value * Mathf.Deg2Rad;
                if (cameraPitch > PI_OVER_2 * .99f)
                    cameraPitch = PI_OVER_2 * .99f;
                else if (cameraPitch < -PI_OVER_2 * .99f)
                    cameraPitch = -PI_OVER_2 * .99f;
                if(immediateSet)
                    lookCurrent.y = lookTarget.y = value;
            }
        }

        /// <summary>
        /// Gets or sets yaw rotation of camera in degrees.
        /// </summary>
        public float Yaw
        {
            get { return cameraYaw * Mathf.Rad2Deg; }
            set
            {
                cameraYaw = value * Mathf.Deg2Rad;
                if(immediateSet)
                    lookCurrent.x = lookTarget.x = value;
            }
        }

        // Set a new maximum pitch limit and ensure current pitch is inside of new bounds
        public float PitchMaxLimit
        {
            get { return pitchMax; }
            set { pitchMax = Mathf.Clamp(value, PITCH_MIN, PITCH_MAX); Pitch = Pitch; }
        }

        // Set a new minimum pitch limit and ensure current pitch is inside of new bounds
        public float PitchMinLimit
        {
            get { return pitchMin; }
            set { pitchMin = Mathf.Clamp(value, PITCH_MIN, PITCH_MAX); Pitch = Pitch; }
        }

        // Scales fractional progression (non-linear) to frame rate
        private float GetFrameRateScaledFractionOfProgression(float fractionAt60FPS)
        {
            float frames = Time.unscaledDeltaTime * 60f; // Number of frames to handle this tick, can be partial
            float c = (1.0f - fractionAt60FPS) / fractionAt60FPS;
            return 1.0f - c / (frames + c);
        }

        void HandleZooming() // Latent functionality, uncomment body to activate
        {
            /*if (InputManager.Instance.ActionStarted(InputManager.Actions.Zoom))
                zooming = !zooming;

            float unscaledZoomSpeed = 0.4f; // Higher value will shorten zoom transition period

            // Get frame rate scaled zoomSpeed so that zoom transition period is same regardless of FPS
            float zoomSpeed = GetFrameRateScaledFractionOfProgression(unscaledZoomSpeed * (zooming ? 0.5f : 1.0f));
            
            zoomCurrent = zoomCurrent * (1.0f - zoomSpeed) + (zooming ? ZOOM_MAX : ZOOM_MIN) * zoomSpeed;

            GameManager.Instance.MainCamera.fieldOfView = DaggerfallUnity.Settings.FieldOfView / zoomCurrent;*/
        }

        // Applies scaled raw mouse deltas to lookTarget, then calls ApplySmoothing method to update lookCurrent
        void ApplyLook()
        {
            // Scale sensitivity
            float sensitivityX = 1.0f;
            float sensitivityY = 1.0f;

            if (InputManager.Instance.UsingController)
            {
                // Make sure it keeps consistent speed regardless of framerate
                // Speed = speed * 60 frames / (1 / unscaledDeltaTime) or speed * 60 * unscaledDeltaTime
                // 60 frames -> speed * 60 / 60 = speed * 1.0
                // 30 frames -> speed * 60 / 30 = speed * 2.0
                // 120 frames -> speed * 60 / 120 = speed * 0.5
                sensitivityX = sensitivity.x * joystickSensitivityScale * 60f * Time.unscaledDeltaTime;
                sensitivityY = sensitivity.y * joystickSensitivityScale * 60f * Time.unscaledDeltaTime;
            }
            else
            {
                sensitivityX = sensitivity.x * sensitivityScale;
                sensitivityY = sensitivity.y * sensitivityScale;
            }

            Vector2 rawMouseDelta = new Vector2(InputManager.Instance.LookX, InputManager.Instance.LookY);

            // Sensitivity factors are inversely scaled by zoomCurrent to allow finer control when zoomed in
            lookTarget += Vector2.Scale(rawMouseDelta, new Vector2(sensitivityX / zoomCurrent, sensitivityY / zoomCurrent * (invertMouseY ? 1 : -1)));

            float range = 360.0f; // Wrap look yaws to range 0..<360

            if (lookTarget.x < 0.0f || lookTarget.x >= range)
            {
                float delta = Mathf.Floor(lookTarget.x / range) * range;
                lookTarget.x -= delta;
                lookCurrent.x -= delta;
            }

            // Clamp target look pitch to range of straight down to straight up
            lookTarget.y = Mathf.Clamp(lookTarget.y, pitchMin, pitchMax);

            ApplySmoothing();

            // Access special setter behaviors for Yaw and Pitch
            immediateSet = false; SetFacing(lookCurrent); immediateSet = true;
        }

        // Updates lookCurrent by moving it a fraction towards lookTarget
        // If smoothing is 0.0 (off) then lookCurrent will be set to lookTarget with no intermediates
        void ApplySmoothing()
        {
            // This value could come from user-defined settings (0.0 is none, 0.1 is a little, 0.9 is a lot)
            float smoothing = enableSmoothing ? Smoothing : 0.0f;

            // Enforce some minimum smoothing for controllers (if you like)
            if (InputManager.Instance.UsingController && smoothing < 0.5f)
                smoothing = 0.5f;

            // Scale for FPS
            smoothing = 1.0f - GetFrameRateScaledFractionOfProgression(1.0f - smoothing);

            // Move lookCurrent a fraction towards lookTarget (weighted average formula)
            lookCurrent = lookCurrent * smoothing + lookTarget * (1.0f - smoothing);
        }

        bool SuppressLook()
        {
            // Suppress mouse look if WeaponSwingMode is Classic/Vanilla and player is dragging mouse to swing
            if (InputManager.Instance.HasAction(InputManager.Actions.SwingWeapon) && DaggerfallUnity.Settings.WeaponSwingMode == 0 && GameManager.Instance.WeaponManager.ScreenWeapon.WeaponType != WeaponTypes.Bow)
                return true;

            return false;
        }

        void Update()
        {
            if (forceHideCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                InputManager.Instance.CursorVisible = false;
                return;
            }

            // Cursor activation toggle while game is running
            // This is distinct from cursor being left active when UI open or game is unpaused by esc
            // When cursor activated during gameplay, player can click on world objects to activate them
            // When cursor simply active from closing a popup, etc. a click will recapture cursor
            // We handle activated cursor first as it takes precendence over mouse look and normal cursor recapture
            if (!GameManager.IsGamePaused && InputManager.Instance.ActionStarted(InputManager.Actions.ActivateCursor))
            {
                // Don't allow activate cursor for 0.3 seconds after closing an input message box
                // Helps prevent player accidentally activating cursor when responding to some input
                // For example, responding to guard at Castle Daggerfall and cursor becomes active after pressing return key
                // Players often think this is a bug and don't know the default active cursor toggle is return
                if (Time.realtimeSinceStartup - DaggerfallUI.Instance.timeClosedInputMessageBox > 0.3f)
                    cursorActive = !cursorActive;
            }

            // Show cursor and unlock while active
            // While cursor is active, player can click on objects in scene using mouse similar to activating centre object
            // Clicking on UI element of large HUD will instead operate on that UI
            if (cursorActive)
            {
                Cursor.lockState = CursorLockMode.None;
                InputManager.Instance.CursorVisible = true;

                if (Input.GetMouseButtonDown(0))
                {
                    // TODO: Activate object clicked by mouse - this should take precedence over activate centre object if that is also mouse0
                }

                return;
            }

            // Ensure the cursor always locked when set
            if (lockCursor && enableMouseLook)
            {
                Cursor.lockState = CursorLockMode.Locked;
                InputManager.Instance.CursorVisible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                InputManager.Instance.CursorVisible = true;
            }

            // Handle mouse look enable/disable
            if (simpleCursorLock)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                    enableMouseLook = !enableMouseLook;
                if (!enableMouseLook && Input.GetMouseButtonDown(0) || !enableMouseLook && Input.GetMouseButtonDown(1))
                    enableMouseLook = true;
            }
            else
            {
                // Enable mouse cursor when game paused
                enableMouseLook = !GameManager.IsGamePaused;
            }

            // Exit when mouse look disabled
            if (!enableMouseLook)
                return;

            HandleZooming();

            if (SuppressLook())
                SetFacing(lookCurrent); // Immediately stop at current heading
            else
                ApplyLook(); // Apply mouse tracking or controller input to look heading

            // If there's a character body that acts as a parent to the camera
            if (characterBody)
            {
                transform.localEulerAngles = new Vector3(Pitch, 0, 0);
                characterBody.transform.localEulerAngles = new Vector3(0, Yaw, 0);
            }
            else
            {
                transform.localEulerAngles = new Vector3(Pitch, Yaw, 0);
            }
        }

        public void SetFacing(float yaw, float pitch)
        {
            Yaw = yaw;
            Pitch = pitch;
        }

        public void SetFacing(Vector2 facing)
        {
            Yaw = facing.x;
            Pitch = facing.y;
        }

        public void SetFacing(Vector3 forward)
        {
            Quaternion q = Quaternion.LookRotation(forward);
            Vector3 v = q.eulerAngles;
            SetFacing(v.y, v.x);
        }

        // Set facing but keep pitch level
        public void SetHorizontalFacing(Vector3 forward)
        {
            Quaternion q = Quaternion.LookRotation(forward);
            Vector3 v = q.eulerAngles;
            SetFacing(v.y, 0f);
        }

        public void ForceHideCursor(bool hideCursor)
        {
            this.forceHideCursor = hideCursor;
        }
    }
}
