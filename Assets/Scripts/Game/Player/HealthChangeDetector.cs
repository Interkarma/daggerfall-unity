// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Meteoric Dragon
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game
{
    public class HealthChangeDetector : MonoBehaviour
    {
        protected int previousMaxHealth;
        protected int previousHealth;
        public float HealthLostPercent { get; private set; }
        public float HealthGainPercent { get { return -1 * HealthLostPercent; } }
        public int HealthLost { get; private set; }
        public int HealthGain { get { return -1 * HealthLost; } }

        void Start()
        {
            // Get starting health and max health
            if (GameManager.Instance != null && GameManager.Instance.PlayerEntity != null)
                ResetHealth();

            // Use events to capture a couple of edge cases
            StreamingWorld.OnInitWorld += StreamingWorld_OnInitWorld;
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            DaggerfallCourtWindow.OnCourtScreen += DaggerfallCourtWindow_OnCourtScreen;
        }

        void Update()
        {
            if (GameManager.IsGamePaused)
                return;

            // Check max health hasn't changed - this can indicate user has loaded a different character
            // or current character has levelled up or changed in some way and the cached health values need to be refreshed.
            // Just reset values and exit for this frame as the current relative health lost calculation is not valid when MaxHealth changes.
            int maxHealth = GameManager.Instance.PlayerEntity.MaxHealth;
            int currentHealth = GameManager.Instance.PlayerEntity.CurrentHealth;
            if (maxHealth != previousMaxHealth)
            {
                ResetHealth();
                return;
            }

            // Detect Health loss
            HealthLost = previousHealth - currentHealth;
            if (HealthLost > 0)
            {
                HealthLostPercent = (float)HealthLost / maxHealth;
                //Debug.Log("Health Lost: " + HealthLost);
            }

            // reset previous health to detect next health loss
            previousHealth = currentHealth;
        }

        public void ResetHealth()
        {
            previousMaxHealth = GameManager.Instance.PlayerEntity.MaxHealth;
            previousHealth = GameManager.Instance.PlayerEntity.CurrentHealth;
        }

        private void StreamingWorld_OnInitWorld()
        {
            // Player can be moved by one system or another with swaying active
            // This clears sway when player relocated
            ResetHealth();
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            // Loading a character with same MaxHealth but lower current health
            // would also trigger a sway on load
            // This resets on any load so sway is cleared for incoming character
            ResetHealth();
        }

        private void DaggerfallCourtWindow_OnCourtScreen()
        {
            // Clear when player goes to court screen
            ResetHealth();
        }
    }
}