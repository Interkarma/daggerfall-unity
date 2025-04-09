// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Interkarma
// Contributors:    Hazelnut, Allofich, Meteoric Dragon, jefetienne
// 
// Notes:
//

using DaggerfallConnect;
using System.Collections.Generic;
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
        private const float dfRideBase = 375f;
        private const float dfCartBase = 250f;

        public bool walkSpeedOverride = true;
        private float currentWalkSpeed = 0;
        private Dictionary<string, float> walkSpeedModifiers = new Dictionary<string, float>();

        public bool runSpeedOverride = true;
        private float currentRunSpeed = 0;
        private Dictionary<string, float> runSpeedModifiers = new Dictionary<string, float>();

        public bool rideSpeedOverride = true;
        private float currentRideSpeed = 0;
        private Dictionary<string, float> rideSpeedModifiers = new Dictionary<string, float>();

        public delegate bool CanPlayerRun();
        public CanPlayerRun CanRun { get; set; }
        public bool runningMode = false;
        public bool sneakingMode = false;

        public bool isRunning = false;
        public bool isSneaking = false;

        public bool updateWalkSpeed;
        public bool updateRunSpeed;
        public bool updateRideSpeed;

        private float previousBaseWalkSpeed;
        private float previousBaseRunSpeed;
        private float previousBaseRideSpeed;

        private float baseSpeed = 0;

        private void Start()
        {
            playerMotor = GameManager.Instance.PlayerMotor;
            levitateMotor = GetComponent<LevitateMotor>();
            CanRun = CanRunUnlessRiding;
            currentWalkSpeed = GetWalkSpeed(GameManager.Instance.PlayerEntity);
            currentRunSpeed = GetRunSpeed();
            currentRideSpeed = GetRideSpeed();
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

            if (InputManager.Instance.ActionStarted(InputManager.Actions.AutoRun)
                && !InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards))
            {
                InputManager.Instance.ToggleAutorun = !InputManager.Instance.ToggleAutorun;

                ToggleRun = InputManager.Instance.ToggleAutorun;

                // If we enabled autorunning, and we are currently not running, run.
                // This allows a player already running to keep running instead of
                // moving to "autowalking"
                if (ToggleRun && !isRunning)
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
            else if (!CanRun())
            {
                isRunning = false; // you can't switch running on/off while in mid air
            }

            if (isRunning)
            {
                speed = RefreshRunSpeed();
                sneakingMode = false; //switch sneaking off if was previously sneaking
            }
            else if (isSneaking)
            {
                // Handle sneak key. Reduces movement speed to half, then subtracts 1 in classic speed units
                speed /= 2;
                speed -= (1 / classicToUnitySpeedUnitRatio);
            }

            if (playerMotor.IsRiding)
            {
                speed = RefreshRideSpeed();
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
            float playerSpeed = player.Stats.LiveSpeed;
            if (playerMotor == null)
                playerMotor = GameManager.Instance.PlayerMotor;
            // crouching speed penalty doesn't apply if swimming.
            if (playerMotor.IsCrouching && !levitateMotor.IsSwimming)
                baseSpeed = (playerSpeed + dfCrouchBase) / classicToUnitySpeedUnitRatio;
            else if (playerMotor.IsRiding)
            {
                float currentRideBase = 0;
                bool playerIsRidingHorseWithNoCart = GameManager.Instance.TransportManager.TransportMode == TransportModes.Horse;
                if (playerIsRidingHorseWithNoCart)
                    currentRideBase = dfRideBase;
                else
                    currentRideBase = dfCartBase;
                baseSpeed = (playerSpeed + currentRideBase) / classicToUnitySpeedUnitRatio;
            }
            else
            {
                baseSpeed = RefreshWalkSpeed();
            }
            return baseSpeed;
        }

        /// <summary>
        /// Add custom walk speed modifier to speed modifer dictionary. Returns unique ID for referencing of custom speedModifier for future manipulation.
        /// </summary>
        /// <param name="UID">The Unique Universal ID created and provided when original value was added to dictionary. Store this value to reference your speed modifier later.</param>
        /// <param name="walkSpeedModifier">the amount to change players base walk speed by percentages. AKA, .75 will lower player movement by 25%. Using 0 or negatives will do nothing but return null.</param>
        /// <param name="refreshWalkSpeed">will cause routine to also update the player speed using the list to sequentially multiply the current base value by the list modifier values.</param>
        /// <returns></returns>        
        public bool AddWalkSpeedMod(out string UID, float walkSpeedModifier = 0, bool refreshWalkSpeed = true)
        {
            bool added = false;
            UID = null;

            //if they set a speed modifier greater than 0, grab the list index using count, and add item (which will be at the lastID index spot).
            if (walkSpeedModifier > 0)
            {
                UID = System.Guid.NewGuid().ToString();
                walkSpeedModifiers.Add(UID, walkSpeedModifier);
                added = true;
            }
            //trigger an update to the walk speed loop to push updated walk speed value.
            updateWalkSpeed = refreshWalkSpeed;

            return added;
        }

        /// <summary>
        /// Add custom walk speed modifier to speed modifer dictionary. Returns unique ID for referencing of custom speedModifier for future manipulation.
        /// </summary>
        /// <param name="UID">The Unique Universal ID created and provided when original value was added to dictionary. Store this value to reference your speed modifier later.</param>
        /// <param name="speedModifier">the amount to change players base walk speed by percentages. AKA, .75 will lower player movement by 25%. Using 0 or negatives will do nothing but return null.</param>
        /// <param name="refreshRunSpeed">will cause routine to also update the player speed using the list to sequentially multiply the current base value by the list modifier values.</param>
        /// <returns></returns>        
        public bool AddRunSpeedMod(out string UID, float speedModifier = 0, bool refreshRunSpeed = true)
        {
            bool added = false;
            UID = null;

            //if they set a speed modifier greater than 0, grab the list index using count, and add item (which will be at the lastID index spot).
            if (speedModifier > 0)
            {
                UID = System.Guid.NewGuid().ToString();
                runSpeedModifiers.Add(UID, speedModifier);
                added = true;
            }

            //trigger an update to the walk speed loop to push updated walk speed value.
            updateRunSpeed = refreshRunSpeed;

            return added;
        }

        /// <summary>
        /// Add custom ride speed modifier to speed modifer dictionary. Returns unique ID for referencing of custom speedModifier for future manipulation.
        /// </summary>
        /// <param name="UID">The Unique Universal ID created and provided when original value was added to dictionary. Store this value to reference your speed modifier later.</param>
        /// <param name="speedModifier">the amount to change players base ride speed by percentages. AKA, .75 will lower player movement by 25%. Using 0 or negatives will do nothing but return null.</param>
        /// <param name="refreshRideSpeed">will cause routine to also update the player speed using the list to sequentially multiply the current base value by the list modifier values.</param>
        /// <returns></returns>        
        public bool AddRideSpeedMod(out string UID, float speedModifier = 0, bool refreshRideSpeed = true)
        {
            bool added = false;
            UID = null;

            //if they set a speed modifier greater than 0, grab the list index using count, and add item (which will be at the lastID index spot).
            if (speedModifier > 0)
            {
                UID = System.Guid.NewGuid().ToString();
                rideSpeedModifiers.Add(UID, speedModifier);
                added = true;
            }

            //trigger an update to the ride speed loop to push updated ride speed value.
            updateRideSpeed = refreshRideSpeed;

            return added;
        }

        /// <summary>
        /// Remove custom speed modifier from speed modifer dictionary using stored UID. Returns true if removed, false if not found. Ensure to set if it is a run or walk speed modifier being removed.
        /// </summary>
        /// <param name="UID">The Unique Universal ID created and provided when original value was added to dictionary. Store this value to reference your speed modifier later.</param>
        /// <param name="refreshSpeed">will cause routine to also update the player speed using the list to sequentially multiply the current base value by the list modifier values.</param>
        /// <returns></returns>   
        public bool RemoveSpeedMod(string UID, bool refreshSpeed = true)
        {
            //setup false bool for manipulation.
            bool removed = false;

            //if there is no uid put in, return false as error catching.
            if (UID == "" || UID == null)
                return removed;

            //look through all three lists to see if the UID is in any of them. If it is, remove and flag boolean(s) true.
            bool walkSpeedModifierIsRemoved = walkSpeedModifiers.Remove(UID);
            bool runSpeedModifierIsRemoved = runSpeedModifiers.Remove(UID);
            bool rideSpeedModifierIsRemoved = rideSpeedModifiers.Remove(UID);
            removed = walkSpeedModifierIsRemoved || runSpeedModifierIsRemoved || rideSpeedModifierIsRemoved;

            //trigger an update to the walk speed loop to push updated walk speed value.
            updateWalkSpeed = refreshSpeed;
            updateRunSpeed = refreshSpeed;
            updateRideSpeed = refreshSpeed;

            return removed;
        }

        /// <summary>
        /// Clears modifiers on walk, run and ride speeds.
        /// Default values will clear out all three groups of modifiers.
        /// </summary>
        /// <param name="walkSpeedReset">clear out walk speed modifiers.</param>
        /// <param name="runSpeedReset">clear out run speed modifiers</param>
        /// <param name="rideSpeedReset">clear out ride speed modifiers</param>
        public bool ResetSpeed(bool walkSpeedReset = true, bool runSpeedReset = true, bool rideSpeedReset = true)
        {
            bool reset = false;
            if (walkSpeedReset) { walkSpeedModifiers.Clear(); reset = updateWalkSpeed = true; }
            if (runSpeedReset) { runSpeedModifiers.Clear(); reset = updateRunSpeed = true; }
            if (rideSpeedReset) { rideSpeedModifiers.Clear(); reset = updateRideSpeed = true; }
            return reset;
        }

        /// <summary>
        /// Refresh walk speed based on modifiers and override settings.
        /// </summary>
        /// <returns></returns>
        public float RefreshWalkSpeed()
        {
            float baseSpeed = GetWalkSpeed();
            return RefreshSpeed(baseSpeed, walkSpeedModifiers, walkSpeedOverride, ref previousBaseWalkSpeed, ref updateWalkSpeed, ref currentWalkSpeed);
        }

        /// <summary>
        /// Refresh run speed based on modifiers and override settings.
        /// </summary>
        /// <returns></returns>
        public float RefreshRunSpeed()
        {
            float baseSpeed = GetRunSpeed();
            return RefreshSpeed(baseSpeed, runSpeedModifiers, runSpeedOverride, ref previousBaseRunSpeed, ref updateRunSpeed, ref currentRunSpeed);
        }

        /// <summary>
        /// Refresh ride speed based on modifiers and override settings.
        /// </summary>
        /// <returns></returns>
        public float RefreshRideSpeed()
        {
            float baseSpeed = GetRideSpeed();
            return RefreshSpeed(baseSpeed, rideSpeedModifiers, rideSpeedOverride, ref previousBaseRideSpeed, ref updateRideSpeed, ref currentRideSpeed);
        }

        /// <summary>
        /// Refresh speed based on modifiers and override settings.
        /// </summary>
        /// <param name="baseSpeed"></param>
        /// <param name="modifiers"></param>
        /// <param name="overrideEnabled"></param>
        /// <param name="previousBaseSpeed"></param>
        /// <param name="updateFlag"></param>
        /// <param name="currentSpeed"></param>
        /// <returns></returns>
        private float RefreshSpeed(float baseSpeed, IDictionary<string, float> modifiers, bool overrideEnabled, ref float previousBaseSpeed, ref bool updateFlag, ref float currentSpeed)
        {
            // If there are no modifiers or override is disabled, return the base speed.
            if (modifiers.Count == 0 || !overrideEnabled)
                return baseSpeed;

            // If the base speed has changed, store it and flag that an update is needed.
            if (previousBaseSpeed != baseSpeed)
            {
                previousBaseSpeed = baseSpeed;
                updateFlag = true;
            }

            // Process the modifiers only if flagged for an update.
            if (updateFlag)
            {
                float overrideSpeed = baseSpeed;

                // Process each modifier in sequence.
                foreach (var modifier in modifiers)
                    overrideSpeed *= modifier.Value;

                // Update the current speed and reset the update flag.
                currentSpeed = overrideSpeed;
                updateFlag = false;
            }

            return currentSpeed;
        }

        /// <summary>
        /// Get LiveSpeed adjusted for walking
        /// </summary>
        /// <returns></returns>
        public float GetWalkSpeed()
        {
            Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
            float drag = 0.5f * (100 - (player.Stats.LiveSpeed >= 30 ? player.Stats.LiveSpeed : 30));
            return (player.Stats.LiveSpeed + dfWalkBase - drag) / classicToUnitySpeedUnitRatio;
        }

        /// <summary>
        /// Get LiveSpeed adjusted for running
        /// </summary>
        /// <returns></returns>
        public float GetRunSpeed()
        {
            Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
            float baseRunSpeed = playerMotor.IsRiding ? baseSpeed : (player.Stats.LiveSpeed + dfWalkBase) / classicToUnitySpeedUnitRatio;
            return baseRunSpeed * (1.35f + (player.Skills.GetLiveSkillValue(DFCareer.Skills.Running) / 200f));
        }

        /// <summary>
        /// Get LiveSpeed adjusted for riding
        /// </summary>
        /// <returns></returns>
        public float GetRideSpeed()
        {
            Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
            float currentRideBase = 0;
            bool playerIsRidingHorseWithNoCart = GameManager.Instance.TransportManager.TransportMode == TransportModes.Horse;
            if (playerIsRidingHorseWithNoCart)
                currentRideBase = dfRideBase;
            else
                currentRideBase = dfCartBase;
            float baseRideSpeed = (player.Stats.LiveSpeed + currentRideBase) / classicToUnitySpeedUnitRatio; ;
            return baseRideSpeed;
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

        /// <summary>
        /// Legacy method signature for older mods to keep working. Use GetWalkSpeed() instead.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public float GetWalkSpeed(Entity.PlayerEntity player) { return GetWalkSpeed(); }
    }
}