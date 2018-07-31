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
    public class VitalsChangeDetector : MonoBehaviour
    {
        protected int previousMaxHealth;
        protected int previousMaxFatigue;        
        protected int previousMaxMagicka;
        protected int previousHealth;
        protected int previousFatigue;
        protected int previousMagicka;
        public float HealthLostPercent { get; private set; }
        public float FatigueLostPercent { get; private set; }
        public float MagickaLostPercent { get; private set; }
        public float HealthGainPercent { get { return -1 * HealthLostPercent; } }
        public float FatigueGainPercent { get { return -1 * FatigueLostPercent; } }
        public float MagickaGainPercent { get { return -1 * MagickaLostPercent; } }
        public int HealthLost { get; private set; }
        public int FatigueLost { get; private set; }
        public int MagickaLost { get; private set; }
        public int HealthGain { get { return -1 * HealthLost; } }
        public int FatigueGain { get { return -1 * FatigueLost; } }
        public int MagickaGain { get { return -1 * MagickaLost; } }

        void Start()
        {
            // Get starting health and max health
            if (GameManager.Instance != null && GameManager.Instance.PlayerEntity != null)
                ResetVitals();

            // Use events to capture a couple of edge cases
            StreamingWorld.OnInitWorld += StreamingWorld_OnInitWorld;
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            DaggerfallCourtWindow.OnCourtScreen += DaggerfallCourtWindow_OnCourtScreen;
        }

        void Update()
        {
            if (GameManager.IsGamePaused)
                return;

            // Check max vitals hasn't changed - this can indicate user has loaded a different character
            // or current character has levelled up or changed in some way and the cached vital values need to be refreshed.
            // Just reset values and exit for this frame as the current relative vital lost calculation is not valid when Max Vital changes.
            int maxHealth = GameManager.Instance.PlayerEntity.MaxHealth;
            int maxFatigue = GameManager.Instance.PlayerEntity.MaxFatigue;
            int maxMagicka = GameManager.Instance.PlayerEntity.MaxMagicka;
            int currentHealth = GameManager.Instance.PlayerEntity.CurrentHealth;
            int currentFatigue = GameManager.Instance.PlayerEntity.CurrentFatigue;
            int currentMagicka = GameManager.Instance.PlayerEntity.CurrentMagicka;
            if (maxHealth != previousMaxHealth || maxFatigue != previousMaxFatigue || maxMagicka != previousMaxMagicka)
            {
                ResetVitals();
                return;
            }

            // Detect Health loss
            HealthLost = previousHealth - currentHealth;
            if (HealthLost > 0)
                HealthLostPercent = (float)HealthLost / maxHealth;

            FatigueLost = previousFatigue - currentFatigue;
            if (FatigueLost > 0)
                FatigueLostPercent = (float)FatigueLost / maxFatigue;

            MagickaLost = previousMagicka - currentMagicka;
            if (MagickaLost > 0)
                MagickaLostPercent = (float)MagickaLost / maxMagicka;

            // reset previous health to detect next health loss
            previousHealth = currentHealth;
            previousFatigue = currentFatigue;
            previousMagicka = currentMagicka;
        }

        private void ResetVitals()
        {
            previousMaxHealth = GameManager.Instance.PlayerEntity.MaxHealth;
            previousMaxFatigue = GameManager.Instance.PlayerEntity.MaxFatigue;
            previousMaxMagicka = GameManager.Instance.PlayerEntity.MaxMagicka;

            previousHealth = GameManager.Instance.PlayerEntity.CurrentHealth;
            previousFatigue = GameManager.Instance.PlayerEntity.CurrentFatigue;
            previousMagicka = GameManager.Instance.PlayerEntity.CurrentMagicka;
        }

        private void StreamingWorld_OnInitWorld()
        {
            // Player can be moved by one system or another with swaying active
            // This clears sway when player relocated
            ResetVitals();
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            // Loading a character with same MaxHealth but lower current health
            // would also trigger a sway on load
            // This resets on any load so sway is cleared for incoming character
            ResetVitals();
        }

        private void DaggerfallCourtWindow_OnCourtScreen()
        {
            // Clear when player goes to court screen
            ResetVitals();
        }
    }
}