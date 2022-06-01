// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Numidium
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
using DaggerfallWorkshop.Game.Formulas;

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
        const int minBonusRoll = 0;         // The minimum number of points added to each base class stat
        const int maxBonusRoll = 10;        // The maximum number of points added to each base class stat
        const int minBonusPool = 6;         // The minimum number of free points to allocate
        const int maxBonusPool = 14;        // The maximum number of free points to allocate

        DaggerfallFont font;
        UpDownSpinner spinner;
        int selectedStat = 0;
        DaggerfallStats startingStats = new DaggerfallStats();
        DaggerfallStats workingStats = new DaggerfallStats();
        int bonusPool = 0;
        Color modifiedStatTextColor = Color.green;
        Panel[] statPanels = new Panel[DaggerfallStats.Count];
        TextLabel[] statLabels = new TextLabel[DaggerfallStats.Count];
        bool characterSheetPositioning = false;
        bool freeEdit = false;

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

        public StatsRollout(bool onCharacterSheet = false, bool freeEdit = false)
            : base()
        {
            if (onCharacterSheet)
                characterSheetPositioning = true;
            if (freeEdit)
            {
                this.freeEdit = true;
                modifiedStatTextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }

            // Add stat panels and labels
            font = DaggerfallUI.DefaultFont;

            Vector2 panelPos, panelSize;
            if (!onCharacterSheet)
            {
                panelPos = new Vector2(8, 33);
                panelSize = new Vector2(34, 6);
            }
            else
            {
                panelPos = new Vector2(141, 17);
                panelSize = new Vector2(28, 6);
            }

            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                statPanels[i] = DaggerfallUI.AddPanel(new Rect(panelPos.x, panelPos.y, panelSize.x, panelSize.y), this);
                statLabels[i] = DaggerfallUI.AddTextLabel(font, Vector2.zero, string.Empty, statPanels[i]);
                statLabels[i].HorizontalAlignment = HorizontalAlignment.Center;
                statLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
                if (!onCharacterSheet)
                    panelPos.y += 22f;
                else
                    panelPos.y += 24f;
            }

            // Add stat select buttons
            Vector2 pos;
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
            int strength = rolledStats.PermanentStrength + UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            int intelligence = rolledStats.PermanentIntelligence + UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            int willpower = rolledStats.PermanentWillpower + UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            int agility = rolledStats.PermanentAgility + UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            int endurance = rolledStats.PermanentEndurance + UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            int personality = rolledStats.PermanentPersonality + UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            int speed = rolledStats.PermanentSpeed + UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            int luck = rolledStats.PermanentLuck + UnityEngine.Random.Range(minBonusRoll, maxBonusRoll + 1);
            rolledStats.SetPermanentStatValue(DFCareer.Stats.Strength, strength);
            rolledStats.SetPermanentStatValue(DFCareer.Stats.Intelligence, intelligence);
            rolledStats.SetPermanentStatValue(DFCareer.Stats.Willpower, willpower);
            rolledStats.SetPermanentStatValue(DFCareer.Stats.Agility, agility);
            rolledStats.SetPermanentStatValue(DFCareer.Stats.Endurance, endurance);
            rolledStats.SetPermanentStatValue(DFCareer.Stats.Personality, personality);
            rolledStats.SetPermanentStatValue(DFCareer.Stats.Speed, speed);
            rolledStats.SetPermanentStatValue(DFCareer.Stats.Luck, luck);

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
                statLabels[i].Text = workingStats.GetPermanentStatValue(i).ToString();
                if (workingStats.GetPermanentStatValue(i) != startingStats.GetPermanentStatValue(i))
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
            if (freeEdit)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            }
        }

        void Spinner_OnUpButtonClicked()
        {
            const int maxFreeEditValue = 75;

            // Get working stat value
            int workingValue = workingStats.GetPermanentStatValue(selectedStat);

            // Get max working value
            int maxWorkingValue = FormulaHelper.MaxStatValue();

            // Working value cannot rise above maxWorkingValue and bonus cannot fall below zero, unless freeEdit active
            if ((freeEdit && workingValue == maxFreeEditValue) ||
                !freeEdit && (workingValue == maxWorkingValue || bonusPool == 0))
                return;

            // Remove a point from pool stat and assign to working stat
            bonusPool -= 1;
            workingStats.SetPermanentStatValue(selectedStat, workingValue + 1);
            spinner.Value = bonusPool;
            UpdateStatLabels();
            RaiseOnStatChanged();
            if (freeEdit)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            }
        }

        void Spinner_OnDownButtonClicked()
        {
            const int minFreeEditValue = 10;

            // Get working stat value
            int workingValue = workingStats.GetPermanentStatValue(selectedStat);

            // Working value cannot reduce below starting value or minWorkingValue, unless freeEdit active
            if ((freeEdit && workingValue == minFreeEditValue) ||
                (!freeEdit && (workingValue == startingStats.GetPermanentStatValue(selectedStat) || workingValue == minWorkingValue)))
                return;

            // Remove a point from working stat and assign to pool
            workingStats.SetPermanentStatValue(selectedStat, workingValue - 1);
            bonusPool += 1;
            spinner.Value = bonusPool;
            UpdateStatLabels();
            RaiseOnStatChanged();
            if (freeEdit)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            }
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