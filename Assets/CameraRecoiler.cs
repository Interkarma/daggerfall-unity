using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        private CameraRecoilSetting recoilSetting;
        public CameraRecoilSetting RecoilSetting
        {
            get
            {
                return recoilSetting;
            }
        }
        
        protected Transform playerCamTransform;
        protected int previousHealth;
        protected bool bSwaying; // true if player is reeling from damage
        protected Vector2 swayAxis;
        protected float timerStart;
        protected float timer;

        void Start ()
        {
            playerCamTransform = GameManager.Instance.MainCamera.transform;

            if (GameManager.Instance != null && GameManager.Instance.PlayerEntity != null)
                previousHealth = GameManager.Instance.PlayerEntity.CurrentHealth;

            bSwaying = false;
            timerStart = 5 * Mathf.PI;
            timer = timerStart;
        }   
	
	    void Update ()
        {
            if (DaggerfallUnity.Settings.CameraRecoil == false || 
                GameManager.IsGamePaused) // prevent continuous spinning on pause
                return;

            int maxHealth = GameManager.Instance.PlayerEntity.MaxHealth;
            int currentHealth = GameManager.Instance.PlayerEntity.CurrentHealth;
            int healthLost = previousHealth - currentHealth;

            if (currentHealth < previousHealth)
            {
                const float minPercentThreshold = 0.02f;
                float percentLost = (float)healthLost / maxHealth;
                //Debug.Log("Percent loss: " + percentLost);
                if (percentLost >= minPercentThreshold)
                {
                    // Start swaying and timer countdown
                    bSwaying = true;
                    timer = timerStart;
                    // get a random unit vector axis for the sway direction
                    SetSwayAxis();
                    //Debug.Log("Start Swaying");
                }
            }

            // reset previous health to detect next health loss
            previousHealth = currentHealth;  

            // do swaying
            if (bSwaying)
            {
                const float timerSpeed = 2f * Mathf.PI; //how quickly the player's view recoils and how quickly it is finished recoiling.
                timer -= Time.deltaTime * timerSpeed;

                // get new view rotation
                playerCamTransform.Rotate(GetRotationVector(healthLost));

                // keep swaying as long as there's time left
                bSwaying = (timer > 0);
            }
        }

        protected virtual CameraRecoilSetting GetRecoilSetting()
        {
            switch (DaggerfallUnity.Settings.CameraRecoilStrength)
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

        protected virtual Vector3 GetRotationVector(int healthLost)
        {
            const float maxSwaySeverity = 50f; // may need to adjust

            //TODO: decrease sway severity slightly for higher levels of player?

            // each point of health lost makes the sway %1.5 of max effectiveness, up to a max of 100% effectiveness.
            float healthLostFactor = Mathf.Clamp((healthLost * 0.015f), 0.015f, 1f);

            // sway severity is a percentage of the timer remaining, and percentage of health lost factor
            float swaySeverity = maxSwaySeverity * (timer / timerStart) * healthLostFactor;

            //Debug.Log("xAmount: " + xAmount);
            //swayaxis provides direction for the sway
            float xAngle = Mathf.Sin(timer) * swaySeverity * swayAxis.x;
            float yAngle = Mathf.Sin(timer) * swaySeverity * swayAxis.y;
            
            Vector3 newViewPositon = new Vector3(xAngle, yAngle);

            // return euler angles
            return newViewPositon;
        }
    }
}
