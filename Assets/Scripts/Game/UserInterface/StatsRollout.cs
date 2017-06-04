// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Implements control to distribute bonus stats.
    /// Used on the "add bonus points" stats window, the
    /// end summary window of character creation and on
    /// the character sheet when leveling.
    /// </summary>
    public class StatsRollout : Panel
    {
        const int minWorkingValue = 0;
        const int maxWorkingValue = 95;
        const int minBonusRoll = 0;         // The minimum number of points added to each base class stat
        const int maxBonusRoll = 10;        // The maximum number of points added to each base class stat
        const int minBonusPool = 6;         // The minimum number of free points to allocate
        const int maxBonusPool = 14;        // The maximum number of free points to allocate

        DaggerfallFont font;
        UpDownSpinner spinner;
        int selectedStat = 0;
        DaggerfallStats startingStats;
        DaggerfallStats workingStats;
        int bonusPool = 0;
        Color modifiedStatTextColor = Color.green;
        TextLabel[] statLabels = new TextLabel[DaggerfallStats.Count];
        bool characterSheetPositioning = false;

        public DaggerfallStats StartingStats
        {
            get { return startingStats; }
            set { SetStats(value, workingStats, bonusPool); }
        }

        public DaggerfallStats WorkingStats
        {
            get { return workingStats; }
            set { SetStats(startingStats, value, bonusPool); }
        }

        public int BonusPool
        {
            get { return bonusPool; }
            set { SetStats(startingStats, workingStats, value); }
        }

        public StatsRollout(bool onCharacterSheet = false)
            : base()
        {
            if (onCharacterSheet)
                characterSheetPositioning = true;

            // Add stat labels
            font = DaggerfallUI.DefaultFont;

            Vector2 pos;

            if (!onCharacterSheet)
                pos = new Vector2(19, 33);
            else
                pos = new Vector2(150, 17);

            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                statLabels[i] = DaggerfallUI.AddTextLabel(font, pos, string.Empty, this);
                statLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
                if (!onCharacterSheet)
                    pos.y += 22f;
                else
                    pos.y += 24f;
            }

            // Add stat select buttons
            if (!onCharacterSheet)
                pos = new Vector2(7, 20);
            else
                pos = new Vector2(141, 6);

            Vector2 size;

            if (!onCharacterSheet)
                size = new Vector2(36, 20);
            else
                size = new Vector2(28, 20);

            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                Button button = DaggerfallUI.AddButton(pos, size, this);
                button.Tag = i;
                button.OnMouseClick += StatButton_OnMouseClick;
                if (!onCharacterSheet)
                    pos.y += 22f;
                else
                    pos.y += 24f;
            }

            // Add up/down spinner
            spinner = new UpDownSpinner();
            this.Components.Add(spinner);
            spinner.OnUpButtonClicked += Spinner_OnUpButtonClicked;
            spinner.OnDownButtonClicked += Spinner_OnDownButtonClicked;
            SelectStat(0);

            UpdateStatLabels();
        }

        #region Public Methods

        public void Reroll(DFCareer dfClass)
        {
            // Assign base stats from class template
            DaggerfallStats rolledStats = CharacterDocument.GetClassBaseStats(dfClass);

            // Roll bonus value for each base stat
            // Using maxBonusRoll + 1 as Unity's Random.Range(int,int) is exclusive
            // of maximum value and we want to be inclusive of maximum value
            rolledStats.Strength += UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            rolledStats.Intelligence += UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            rolledStats.Willpower += UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            rolledStats.Agility += UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            rolledStats.Endurance += UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            rolledStats.Personality += UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            rolledStats.Speed += UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            rolledStats.Luck += UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);

            // Roll bonus pool for player to distribute
            // Using maxBonusPool + 1 for inclusive range as above
            int bonusPool = UnityEngine.Random.Range(minBonusPool, maxBonusPool + 1);

            // Apply stats to control
            SetStats(rolledStats, rolledStats, bonusPool);
            UpdateStatLabels();

            // Play "rolling dice" sound
            DaggerfallUI.Instance.PlayOneShot(SoundClips.DiceRoll);
        }

        public void SetStats(DaggerfallStats startingStats, DaggerfallStats workingStats, int bonusPool)
        {
            this.startingStats.Copy(startingStats);
            this.workingStats.Copy(workingStats);
            this.bonusPool = bonusPool;
            spinner.Value = bonusPool;
            UpdateStatLabels();
            SelectStat(0);
        }

        #endregion

        #region Private Methods

        void UpdateStatLabels()
        {
            // Update primary stat labels
            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                statLabels[i].Text = workingStats.GetStatValue(i).ToString();
                if (workingStats.GetStatValue(i) != startingStats.GetStatValue(i))
                    statLabels[i].TextColor = modifiedStatTextColor;
                else
                    statLabels[i].TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }
        }

        void SelectStat(int index)
        {
            selectedStat = index;
            if (!characterSheetPositioning)
                spinner.Position = new Vector2(44, 21 + (22 * index));
            else
                spinner.Position = new Vector2(176, 6 + (24 * index));
        }

        #endregion

        #region Event Handlers

        void StatButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectStat((int)sender.Tag);
        }

        void Spinner_OnUpButtonClicked()
        {
            // Get working stat value
            int workingValue = workingStats.GetStatValue(selectedStat);

            // Working value cannot rise above maxWorkingValue and bonus cannot fall below zero
            if (workingValue == maxWorkingValue || bonusPool == 0)
                return;

            // Remove a point from pool stat and assign to working stat
            bonusPool -= 1;
            workingStats.SetStatValue(selectedStat, workingValue + 1);
            spinner.Value = bonusPool;
            UpdateStatLabels();
            RaiseOnStatChanged();
        }

        void Spinner_OnDownButtonClicked()
        {
            // Get working stat value
            int workingValue = workingStats.GetStatValue(selectedStat);

            // Working value cannot reduce below starting value or minWorkingValue
            if (workingValue == startingStats.GetStatValue(selectedStat) || workingValue == minWorkingValue)
                return;

            // Remove a point from working stat and assign to pool
            workingStats.SetStatValue(selectedStat, workingValue - 1);
            bonusPool += 1;
            spinner.Value = bonusPool;
            UpdateStatLabels();
            RaiseOnStatChanged();
        }

        #endregion

        #region Events

        public delegate void OnStatChangedEventHandler();
        public event OnStatChangedEventHandler OnStatChanged;
        void RaiseOnStatChanged()
        {
            if (OnStatChanged != null)
                OnStatChanged();
        }

        #endregion
    }
}