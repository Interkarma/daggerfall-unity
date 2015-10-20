// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
        const float piover2 = 1.570796f;

        Vector2 _mouseAbsolute;
        Vector2 _smoothMouse;
        float cameraPitch = 0.0f;
        float cameraYaw = 0.0f;

        public bool invertMouseY = false;
        public bool lockCursor;
        public Vector2 sensitivity = new Vector2(2, 2);
        public Vector2 smoothing = new Vector2(3, 3);
        public float sensitivityScale = 1.0f;
        public bool enableMouseLook = true;
        public bool enableSmoothing = true;

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

        void Start()
        {
            Init();
        }

        void Update()
        {
            // Ensure the cursor is always locked when set
			if (lockCursor && enableMouseLook)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}

            // Enable mouse cursor when game paused
            enableMouseLook = !GameManager.IsGamePaused;
            if (!enableMouseLook)
                return;

            // Suppress mouse look if player is swinging weapon
            //if (Input.GetButton("Fire2"))
            if (InputManager.Instance.HasAction(InputManager.Actions.SwingWeapon))
                return;

            Vector2 rawMouseDelta = new Vector2(InputManager.Instance.LookX, InputManager.Instance.LookY);

            // Invert mouse Y
            if (invertMouseY)
                rawMouseDelta.y = -rawMouseDelta.y;

            // Scale sensitivity
            float sensitivityX = sensitivity.x * sensitivityScale;
            float sensitivityY = sensitivity.y * sensitivityScale;

            if (enableSmoothing)
            {
                // Scale raw mouse delta against the smoothing value
                Vector2 smoothMouseDelta = Vector2.Scale(rawMouseDelta, new Vector2(sensitivityX * smoothing.x, sensitivityY * smoothing.y));

                // Interpolate mouse movement over time to apply smoothing delta
                _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, smoothMouseDelta.x, 1f / smoothing.x);
                _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, smoothMouseDelta.y, 1f / smoothing.y);

                // Find the absolute mouse movement value from point zero
                _mouseAbsolute += _smoothMouse;

                // Update pitch and yaw
                Yaw += _smoothMouse.x;
                Pitch += -_smoothMouse.y;
            }
            else
            {
                // Just use scaled raw mouse input without any smoothing
                rawMouseDelta = Vector2.Scale(rawMouseDelta, new Vector2(sensitivityX, sensitivityY));
                _mouseAbsolute += rawMouseDelta;
                Yaw += rawMouseDelta.x;
                Pitch += -rawMouseDelta.y;
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
    }
}