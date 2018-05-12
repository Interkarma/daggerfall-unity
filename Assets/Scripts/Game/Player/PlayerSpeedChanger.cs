using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerSpeedChanger : MonoBehaviour
    {
        private PlayerMotor playerMotor;

        // Daggerfall base speed constants. (courtesy Allofich)
        public const float classicToUnitySpeedUnitRatio = 39.5f; // was estimated from comparing a walk over the same distance in classic and DF Unity
        public const float dfWalkBase = 150f;
        private const float dfCrouchBase = 50f;
        private const float dfRideBase = dfWalkBase + 225f;
        private const float dfCartBase = dfWalkBase + 100f;

        public float walkSpeedOverride = 6.0f;
        public bool useWalkSpeedOverride = false;

        public float runSpeedOverride = 11.0f;
        public bool useRunSpeedOverride = false;

        private void Start()
        {
            playerMotor = GameManager.Instance.PlayerMotor;
        }

        private void Update()
        {

        }

        public float GetBaseSpeed()
        {
            Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
            float baseSpeed = 0;
            float playerSpeed = player.Stats.LiveSpeed;
            if (playerMotor == null) // fixes null reference bug.
                playerMotor = GameManager.Instance.PlayerMotor;
            if (playerMotor.IsCrouching)
                baseSpeed = (playerSpeed + dfCrouchBase) / classicToUnitySpeedUnitRatio;
            else if (playerMotor.IsRiding)
                baseSpeed = (playerSpeed + dfRideBase) / classicToUnitySpeedUnitRatio;
            else
                baseSpeed = GetWalkSpeed(player);
            return baseSpeed;
        }

        public float GetWalkSpeed(Entity.PlayerEntity player)
        {
            if (useWalkSpeedOverride == true)
                return walkSpeedOverride;
            else
                return (player.Stats.LiveSpeed + dfWalkBase) / classicToUnitySpeedUnitRatio;
        }

        public float GetRunSpeed(float baseSpeed)
        {
            if (useRunSpeedOverride)
                return runSpeedOverride;
            Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
            float runSpeed = baseSpeed * (1.25f + (player.Skills.GetLiveSkillValue(DFCareer.Skills.Running) / 200f));
            return runSpeed;
        }

        public float GetSwimSpeed(float baseSpeed)
        {
            Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
            float swimSpeed = (baseSpeed * (player.Skills.GetLiveSkillValue(DFCareer.Skills.Swimming) / 200f)) + (baseSpeed / 4);
            return swimSpeed;
        }
    }
}