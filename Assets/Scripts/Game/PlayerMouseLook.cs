// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
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

        Vector2 _mouseAbsolute;
        Vector2 _smoothMouse;
        float cameraPitch = 0.0f;
        float cameraYaw = 0.0f;
        public bool cursorActive;
        float pitchMax = PitchMax;
        float pitchMin = PitchMin;

        public bool invertMouseY = false;
        public bool lockCursor;
        public Vector2 sensitivity = new Vector2(2, 2);
        public Vector2 smoothing = new Vector2(3, 3);
        public float sensitivityScale = 1.0f;
        public float joystickSensitivityScale = 1.0f;
        public bool enableMouseLook = true;
        public bool enableSmoothing = true;
        public bool simpleCursorLock = false;
        private bool forceHideCursor;

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
                value = Mathf.Clamp(value, PitchMin, pitchMax);
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
            if (!GameManager.IsGamePaused && InputManager.Instance.ActionComplete(InputManager.Actions.ActivateCursor))
            {
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
            if (InputManager.Instance.HasAction(InputManager.Actions.SwingWeapon) && !DaggerfallUnity.Settings.ClickToAttack && GameManager.Instance.WeaponManager.ScreenWeapon.WeaponType != WeaponTypes.Bow)
                applyLook = false;

            Vector2 rawMouseDelta = new Vector2(InputManager.Instance.LookX, InputManager.Instance.LookY);

            // Invert mouse Y
            if (invertMouseY)
                rawMouseDelta.y = -rawMouseDelta.y;

            // Scale sensitivity
            float sensitivityX = 1.0f;
            float sensitivityY = 1.0f;

            if (InputManager.Instance.UsingController)
            {
                sensitivityX = sensitivity.x * joystickSensitivityScale;
                sensitivityY = sensitivity.y * joystickSensitivityScale;
            }
            else
            {
                sensitivityX = sensitivity.x * sensitivityScale;
                sensitivityY = sensitivity.y * sensitivityScale;
            }

            //controller should just use smoothing
            if (enableSmoothing || InputManager.Instance.UsingController)
            {
                // Scale raw mouse delta against the smoothing value
                Vector2 smoothMouseDelta = Vector2.Scale(rawMouseDelta, new Vector2(sensitivityX * smoothing.x, sensitivityY * smoothing.y));

                // Interpolate mouse movement over time to apply smoothing delta
                _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, smoothMouseDelta.x, 1f / smoothing.x);
                _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, smoothMouseDelta.y, 1f / smoothing.y);

                // Find the absolute mouse movement value from point zero
                _mouseAbsolute += _smoothMouse;

                // Update pitch and yaw
                if (applyLook)
                {
                    Yaw += _smoothMouse.x;
                    Pitch += -_smoothMouse.y;
                }
            }
            else
            {
                // Just use scaled raw mouse input without any smoothing
                rawMouseDelta = Vector2.Scale(rawMouseDelta, new Vector2(sensitivityX, sensitivityY));
                _mouseAbsolute += rawMouseDelta;
                if (applyLook)
                {
                    Yaw += rawMouseDelta.x;
                    Pitch += -rawMouseDelta.y;
                }
            }

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
            // Reset smoothing
            _mouseAbsolute = Vector2.zero;
            _smoothMouse = Vector2.zero;
        }

        public void SetFacing(float yaw, float pitch)
        {
            Yaw = yaw;
            Pitch = pitch;
            Init();
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