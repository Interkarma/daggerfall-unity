// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Interkarma
// Contributors:    Hazelnut, Allofich, Meteoric Dragon, jefetienne
// 
// Notes:
//

using DaggerfallConnect;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerSpeedChanger : MonoBehaviour
    {
        private PlayerMotor playerMotor;
        private LevitateMotor levitateMotor;

        // If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
        // There must be a button set up in the Input Manager called "Run"
        public bool ToggleRun { get; set; }
        public bool ToggleSneak { get; set; }

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

        public delegate bool CanPlayerRun();
        public CanPlayerRun CanRun { get; set; }
        public bool runningMode = false;
        public bool sneakingMode = false;

        public bool isRunning = false;
        public bool isSneaking = false;

        private void Start()
        {
            playerMotor = GameManager.Instance.PlayerMotor;
            levitateMotor = GetComponent<LevitateMotor>();
            CanRun = CanRunUnlessRiding;
        }



        /// <summary>
        /// Record player input for speed adjustment
        /// </summary>
        public void CaptureInputSpeedAdjustment()
        {
            if (!ToggleRun)
                runningMode = InputManager.Instance.HasAction(InputManager.Actions.Run);
            else
                runningMode = runningMode ^ InputManager.Instance.ActionStarted(InputManager.Actions.Run);

            if (!ToggleSneak)
                sneakingMode = InputManager.Instance.HasAction(InputManager.Actions.Sneak);
            else
                sneakingMode = sneakingMode ^ InputManager.Instance.ActionStarted(InputManager.Actions.Sneak);

            if (InputManager.Instance.ActionStarted(InputManager.Actions.AutoRun))
            {
                InputManager.Instance.ToggleAutorun = !InputManager.Instance.ToggleAutorun;

                ToggleRun = InputManager.Instance.ToggleAutorun;
                runningMode = runningMode ^ InputManager.Instance.ToggleAutorun;
            }

            if (InputManager.Instance.ActionStarted(InputManager.Actions.MoveBackwards))
            {
                ToggleRun = false;
            }
        }

        /// <summary>
        /// Determines how speed should be changed based on player's input
        /// </summary>
        /// <param name="speed"></param>
        public void ApplyInputSpeedAdjustment(ref float speed)
        {
            if (playerMotor.IsGrounded)
            {
                isRunning = CanRun() && runningMode;
                isSneaking = !isRunning && sneakingMode;
            }
            else
            {
                if (!CanRun())
                    isRunning = false;
                // you can't switch running on/off while in mid air
            }

            if (isRunning)
            {
                speed = GetRunSpeed(speed);

                //switch sneaking off if was previously sneaking
                sneakingMode = false;
            }
            else if (isSneaking)
            {
                // Handle sneak key. Reduces movement speed to half, then subtracts 1 in classic speed units
                speed /= 2;
                speed -= (1 / classicToUnitySpeedUnitRatio);
            }

            InputManager.Instance.MaximizeJoystickMovement = isRunning;
        }

        public bool CanRunUnlessRiding()
        {
            return !playerMotor.IsRiding;
        }


        /// <summary>
        /// Get LiveSpeed adjusted for swimming, walking, crouching or riding
        /// </summary>
        /// <returns>Speed based on player.Stats.LiveSpeed</returns>
        public float GetBaseSpeed()
        {
            Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
            float baseSpeed = 0;
            float playerSpeed = player.Stats.LiveSpeed;
            if (playerMotor == null) // fixes null reference bug.
                playerMotor = GameManager.Instance.PlayerMotor;
            // crouching speed penalty doesn't apply if swimming.
            if (playerMotor.IsCrouching && !levitateMotor.IsSwimming)
                baseSpeed = (playerSpeed + dfCrouchBase) / classicToUnitySpeedUnitRatio;
            else if (playerMotor.IsRiding)
            {
                float rideSpeed = (GameManager.Instance.TransportManager.TransportMode == TransportModes.Cart) ? dfCartBase : dfRideBase;
                baseSpeed = (playerSpeed + rideSpeed) / classicToUnitySpeedUnitRatio;
            }
            else
                baseSpeed = GetWalkSpeed(player);
            return baseSpeed;
        }

        /// <summary>
        /// Get LiveSpeed adjusted for walking
        /// </summary>
        /// <param name="player">the PlayerEntity to use</param>
        /// <returns></returns>
        public float GetWalkSpeed(Entity.PlayerEntity player)
        {
            if (useWalkSpeedOverride)
                return walkSpeedOverride;
            else
            {
                float drag = 0.5f * (100 - (player.Stats.LiveSpeed >= 30 ? player.Stats.LiveSpeed : 30));
                return (player.Stats.LiveSpeed + dfWalkBase - drag) / classicToUnitySpeedUnitRatio;
            }
        }

        /// <summary>
        /// Get LiveSpeed adjusted for running
        /// </summary>
        /// <param name="baseSpeed"></param>
        /// <returns></returns>
        public float GetRunSpeed(float baseSpeed)
        {
            if (useRunSpeedOverride)
                return runSpeedOverride;
            else
            {
                Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
                float baseRunSpeed = playerMotor.IsRiding ? baseSpeed : (player.Stats.LiveSpeed + dfWalkBase) / classicToUnitySpeedUnitRatio;
                return baseRunSpeed * (1.35f + (player.Skills.GetLiveSkillValue(DFCareer.Skills.Running) / 200f));
            }
        }

        /// <summary>
        /// Get LiveSpeed adjusted for swimming
        /// </summary>
        /// <param name="baseSpeed"></param>
        /// <returns></returns>
        public float GetSwimSpeed(float baseSpeed)
        {
            Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
            return (baseSpeed * (player.Skills.GetLiveSkillValue(DFCareer.Skills.Swimming) / 200f)) + (baseSpeed / 4);
        }

        public float GetClimbingSpeed(float baseSpeed)
        {
            // Climbing effect states "target can climb twice as well" - doubling climbing speed
            Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
            float climbingBoost = player.IsEnhancedClimbing ? 2f : 1f;
            return (baseSpeed / 3) * climbingBoost;
        }
    }
}
