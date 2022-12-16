// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Avernite (avernite@gmail.com)
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
        public const float PitchMax = 90;
        public const float PitchMin = -90;

        const float piover2 = 1.570796f;

        Vector2 lookTarget;
        Vector2 lookCurrent;
        float cameraPitch = 0.0f;
        float cameraYaw = 0.0f;
        public bool cursorActive;
        float pitchMax = PitchMax;
        float pitchMin = PitchMin;

        public bool invertMouseY = false;
        public bool lockCursor;
        public Vector2 sensitivity = new Vector2(2, 2);
        public float sensitivityScale = 1.0f;
        public float joystickSensitivityScale = 1.0f;
        public bool enableMouseLook = true;
        public bool simpleCursorLock = false;
        private bool forceHideCursor;

        public const float SmoothingMax = 0.9f;
        float smoothing = 0.5f; // This value now comes from user-defined settings

        /// <summary>
        /// Gets or sets degree of mouse-look camera smoothing (0.0 is none, 0.1 is a little, 0.9 is a lot).
        /// </summary>
        public float Smoothing
        {
            get { return smoothing; }
            set { smoothing = Mathf.Clamp(value, 0.0f, SmoothingMax); }
        }

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
                if (cameraPitch > piover2 * .99f)
                    cameraPitch = piover2 * .99f;
                else if (cameraPitch < -piover2 * .99f)
                    cameraPitch = -piover2 * .99f;
            }
        }

        /// <summary>
        /// Gets or sets yaw rotation of camera in degrees.
        /// </summary>
        public float Yaw
        {
            get { return cameraYaw * Mathf.Rad2Deg; }
            set { cameraYaw = value * Mathf.Deg2Rad; }
        }

        public float PitchMaxLimit
        {
            get { return pitchMax; }
            set { pitchMax = Mathf.Clamp(value, PitchMin, PitchMax); Pitch = Pitch; }
        }

        public float PitchMinLimit
        {
            get { return pitchMin; }
            set { pitchMin = Mathf.Clamp(value, PitchMin, PitchMax); Pitch = Pitch; }
        }

        // Scales fractional progression (non-linear) to frame rate
        private float GetFrameRateScaledFractionOfProgression(float fractionAt60FPS)
        {
            float frames = Time.unscaledDeltaTime * 60f; // Number of frames to handle this tick, can be partial
            float c = (1.0f - fractionAt60FPS) / fractionAt60FPS;
            return 1.0f - c / (frames + c);
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

            lookTarget += Vector2.Scale(rawMouseDelta, new Vector2(sensitivityX, sensitivityY * (invertMouseY ? -1 : 1)));

            float range = 360.0f;

            if (lookTarget.x < 0.0f || lookTarget.x >= range) // Wrap look yaws to range 0..<360
            {
                float delta = Mathf.Floor(lookTarget.x / range) * range;
                lookTarget.x -= delta;
                lookCurrent.x -= delta;
            }

            // Clamp target look pitch to range of straight down to straight up
            lookTarget.y = Mathf.Clamp(lookTarget.y, pitchMin, pitchMax);

            ApplySmoothing();

            Yaw = lookCurrent.x;
            Pitch = -lookCurrent.y;
        }

        // Updates lookCurrent by moving it a fraction towards lookTarget
        // If smoothing is 0.0 (off) then lookCurrent will be set to lookTarget with no intermediates
        void ApplySmoothing()
        {
            float smoothing = Smoothing;

            // Enforce some minimum smoothing for controllers (if you like)
            if (InputManager.Instance.UsingController && smoothing < 0.5f)
                smoothing = 0.5f;

            // Scale for FPS
            smoothing = 1.0f - GetFrameRateScaledFractionOfProgression(1.0f - smoothing);

            // Move lookCurrent a fraction towards lookTarget (weighted average formula)
            lookCurrent = lookCurrent * smoothing + lookTarget * (1.0f - smoothing);
        }

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (forceHideCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                InputManager.Instance.CursorVisible = false;
                return;
            }

            bool applyLook = true;

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

            // Suppress mouse look if player is swinging weapon
            if (InputManager.Instance.HasAction(InputManager.Actions.SwingWeapon) && DaggerfallUnity.Settings.WeaponSwingMode == 0 && GameManager.Instance.WeaponManager.ScreenWeapon.WeaponType != WeaponTypes.Bow)
                applyLook = false;

            if (applyLook)
                ApplyLook(); // Apply mouse tracking or controller input to look vector
            else
                SetFacing(lookCurrent); // Immediately stop at current heading

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

        public void Init()
        {
            lookTarget.x = Yaw;
            lookTarget.y = -Pitch;
            lookCurrent = lookTarget;
        }

        public void SetFacing(float yaw, float pitch)
        {
            Yaw = yaw;
            Pitch = pitch;
            Init();
        }

        public void SetFacing(Vector2 facing)
        {
            SetFacing(facing.x, -facing.y);
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
