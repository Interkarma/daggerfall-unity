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

using System;
using UnityEngine;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game
{
    public enum CameraRecoilSetting
    {
        Off,
        Low,
        Medium,
        High,
        VeryHigh
    }

    public class CameraRecoiler : MonoBehaviour
    {
        private CameraRecoilSetting cameraRecoilSetting;
        public CameraRecoilSetting RecoilSetting
        {
            get
            {
                return cameraRecoilSetting;
            }
        }

        protected Transform playerCamTransform;
        protected VitalsChangeDetector vitalsDetector;
        protected bool bSwaying; // true if player is reeling from damage
        protected Vector2 swayAxis;
        protected float timerStart;
        protected float timer;
        protected const float baseMaxRecoilSeverity = 50f; // may need to adjust

        void Start()
        {
            playerCamTransform = GameManager.Instance.MainCamera.transform;
            vitalsDetector = GetComponent<VitalsChangeDetector>();

            // Get starting health and max health
            if (GameManager.Instance != null && GameManager.Instance.PlayerEntity != null)
                ResetRecoil();

            cameraRecoilSetting = GetRecoilSetting(DaggerfallUnity.Settings.CameraRecoilStrength);

            // Use events to capture a couple of edge cases
            StreamingWorld.OnInitWorld += StreamingWorld_OnInitWorld;
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            DaggerfallCourtWindow.OnCourtScreen += DaggerfallCourtWindow_OnCourtScreen;
        }

        void Update()
        {
            if (GetRecoilSetting(DaggerfallUnity.Settings.CameraRecoilStrength) == CameraRecoilSetting.Off ||
                GameManager.IsGamePaused || GameManager.Instance.PlayerMouseLook.cursorActive) // prevent continuous spinning on pause
                return;
            else
                cameraRecoilSetting = GetRecoilSetting(DaggerfallUnity.Settings.CameraRecoilStrength);

            int healthLost = vitalsDetector.HealthLost;
            float healthLostPercent = vitalsDetector.HealthLostPercent;
            // Detect Health loss
            if (healthLost > 0)
            {
                const float minPercentThreshold = 0.02f;

                // useless to do it for less than a certain percentage
                if (healthLostPercent > minPercentThreshold)
                {
                    // Start swaying and timer countdown
                    bSwaying = true;
                    //Debug.Log("Percent loss: "  percentLost);

                    // longer timer for more health percent lost
                    timerStart = CalculateTimerStart(healthLostPercent);
                    timer = timerStart;

                    // get a random unit vector axis for the sway direction
                    SetSwayAxis();
                    //Debug.Log("Start Swaying");
                }
            }

            // do swaying
            if (bSwaying)
            {
                const float timerSpeed = 2f * Mathf.PI; //how quickly the player's view recoils and how quickly it is finished recoiling.
                timer -= Time.deltaTime * timerSpeed;

                // get new view rotation
                float rotationScalar = CalculateRotationScalar(healthLost);
                playerCamTransform.Rotate(GetRotationVector(healthLost, rotationScalar));

                // keep swaying as long as there's time left
                bSwaying = (timer > 0);
            }
        }

        protected virtual CameraRecoilSetting GetRecoilSetting(int recoilStrength)
        {
            switch (recoilStrength)
            {
                case 0: return CameraRecoilSetting.Off;
                case 1: return CameraRecoilSetting.Low;
                case 2: return CameraRecoilSetting.Medium;
                case 3: return CameraRecoilSetting.High;
                case 4: return CameraRecoilSetting.VeryHigh;
                default: throw new Exception("Camera recoil setting not found!");
            }
        }

        protected virtual void SetSwayAxis()
        {
            swayAxis = UnityEngine.Random.insideUnitCircle.normalized;
        }

        protected virtual float CalculateTimerStart(float percentHealthLost)
        {
            // Timer start should be increased for greater percentages of health lost
            // in stages of each 20% lost
            int piScalar = 5 + Mathf.FloorToInt(percentHealthLost * 5);
            //Debug.Log("timerStart is PI * " + piScalar);
            return piScalar * Mathf.PI;
        }

        protected virtual float AdjustForUserSetting(float maxRotationScalar)
        {
            switch (cameraRecoilSetting)
            {
                case CameraRecoilSetting.Off: return maxRotationScalar * 0f;
                case CameraRecoilSetting.Low: return maxRotationScalar * 0.25f;
                case CameraRecoilSetting.Medium: return maxRotationScalar * 0.50f;
                case CameraRecoilSetting.High: return maxRotationScalar * 0.75f;
                case CameraRecoilSetting.VeryHigh: return maxRotationScalar * 1f;
                default: throw new Exception("Camera Recoil Setting not found!");
            }
        }

        protected virtual float CalculateRotationScalar(int healthLost)
        {
            float maxRotationScalar = AdjustForUserSetting(baseMaxRecoilSeverity);

            // each point of health lost makes the sway %1.5 of max effectiveness, up to a max of 100% effectiveness.
            float healthLostFactor = Mathf.Clamp((healthLost * 0.015f), 0.015f, 1f);

            // sway severity is a percentage of the timer remaining, and percentage of health lost factor
            return maxRotationScalar * (timer / timerStart) * healthLostFactor;
        }

        protected virtual Vector3 GetRotationVector(int healthLost, float rotationScalar)
        {
            //Debug.Log("xAmount: "  xAmount);
            //swayaxis provides direction for the sway
            float xAngle = Mathf.Sin(timer) * rotationScalar * swayAxis.x;
            float yAngle = Mathf.Sin(timer) * rotationScalar * swayAxis.y;

            // return vector for euler angles
            return new Vector3(xAngle, yAngle);
        }

        public void ResetRecoil()
        {
            bSwaying = false;
        }

        private void StreamingWorld_OnInitWorld()
        {
            // Player can be moved by one system or another with swaying active
            // This clears sway when player relocated
            ResetRecoil();
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            // Loading a character with same MaxHealth but lower current health
            // would also trigger a sway on load
            // This resets on any load so sway is cleared for incoming character
            ResetRecoil();
        }

        private void DaggerfallCourtWindow_OnCourtScreen()
        {
            // Clear sway when player goes to court screen
            ResetRecoil();
        }
    }
}
