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
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class CreateCharAddBonusPoints : DaggerfallPopupWindow
    {
        const string nativeImgName = "CHAR02I0.IMG";

        const int minBonusRoll = 0;         // The minimum number of points added to each base class stat
        const int maxBonusRoll = 10;        // The maximum number of points added to each base class stat
        const int minBonusPool = 6;         // The minimum number of free points to allocate
        const int maxBonusPool = 14;        // The maximum number of free points to allocate
        const int minWorkingValue = 0;
        const int maxWorkingValue = 95;

        Texture2D nativeTexture;
        DaggerfallFont font;
        TextLabel[] statLabels = new TextLabel[8];
        TextLabel damageModifierLabel;
        TextLabel maxEncumbranceLabel;
        TextLabel spellPointsLabel;
        TextLabel magicResistLabel;
        TextLabel toHitModifierLabel;
        TextLabel hitPointsModifierLabel;
        TextLabel healingRateModifierLabel;

        UpDownSpinner spinner;
        int selectedStat = 0;

        DFClass dfClass;
        DaggerfallStats rolledStats;
        DaggerfallStats workingStats;
        int bonusPool;
        Color modifiedStatTextColor = Color.green;

        bool rollSaved = false;
        DaggerfallStats savedRolledStats;
        DaggerfallStats savedWorkingStats;
        int savedBonusPool;

        public DFClass DFClass
        {
            get { return dfClass; }
            set { dfClass = value; }
        }

        public CreateCharAddBonusPoints(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            if (IsSetup)
                return;

            // Load native texture
            nativeTexture = GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharAddBonusPoints: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Add stat labels
            font = DaggerfallUI.Instance.DefaultFont;
            Vector2 pos = new Vector2(19, 33);
            for (int i = 0; i < 8; i++)
            {
                statLabels[i] = AddTextLabel(font, pos, string.Empty);
                statLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
                pos.y += 22f;
            }

            // Add secondary stat labels
            damageModifierLabel = AddTextLabel(font, new Vector2(83, 22), string.Empty);
            maxEncumbranceLabel = AddTextLabel(font, new Vector2(103, 32), string.Empty);
            spellPointsLabel = AddTextLabel(font, new Vector2(112, 49), string.Empty);
            magicResistLabel = AddTextLabel(font, new Vector2(121, 71), string.Empty);
            toHitModifierLabel = AddTextLabel(font, new Vector2(97, 93), string.Empty);
            hitPointsModifierLabel = AddTextLabel(font, new Vector2(101, 110), string.Empty);
            healingRateModifierLabel = AddTextLabel(font, new Vector2(122, 120), string.Empty);

            // Fix secondary stat shadow colors to match game
            damageModifierLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            maxEncumbranceLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            spellPointsLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            magicResistLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            toHitModifierLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            hitPointsModifierLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            healingRateModifierLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;

            // Add stat select buttons
            pos = new Vector2(7, 20);
            Vector2 size = new Vector2(36, 20);
            for (int i = 0; i < 8; i++)
            {
                Button button = AddButton(pos, size);
                button.Tag = i;
                button.OnMouseClick += StatButton_OnMouseClick;
                pos.y += 22;
            }

            // Add up/down spinner
            spinner = new UpDownSpinner();
            NativePanel.Components.Add(spinner);
            spinner.OnUpButtonClicked += Spinner_OnUpButtonClicked;
            spinner.OnDownButtonClicked += Spinner_OnDownButtonClicked;
            SelectStat(0);

            // Add "Reroll" button
            Button rerollButton = AddButton(new Rect(263, 147, 39, 22));
            rerollButton.OnMouseClick += RerollButton_OnMouseClick;

            // Add "Save Roll" button
            Button saveRoll = AddButton(new Rect(162, 162, 71, 9));
            saveRoll.OnMouseClick += SaveRoll_OnMouseClick;

            // Add "Load Roll" button
            Button loadRoll = AddButton(new Rect(162, 171, 71, 9));
            loadRoll.OnMouseClick += LoadRoll_OnMouseClick;

            // Add "OK" button
            Button okButton = AddButton(new Rect(263, 172, 39, 22));
            okButton.OnMouseClick += OkButton_OnMouseClick;

            IsSetup = true;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }

        #region Public Methods

        public void Reroll()
        {
            // Must be setup prior to rolling stats
            Setup();

            // Assign base stats from class template
            rolledStats = CharacterSheet.GetClassBaseStats(dfClass);

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
            bonusPool = UnityEngine.Random.Range(minBonusPool, maxBonusPool + 1);
            spinner.Value = bonusPool;

            // Copy base stats to working stats
            workingStats.Copy(rolledStats);
            UpdateStatLabels();

            // Play "rolling dice" sound
            DaggerfallUI.Instance.PlayOneShot(SoundClips.DiceRoll);
        }

        #endregion

        #region Private Methods

        void UpdateStatLabels()
        {
            // Update primary stat labels
            Vector2 pos = new Vector2(19, 33);
            for (int i = 0; i < 8; i++)
            {
                statLabels[i].Text = workingStats.GetStatValue(i).ToString();
                if (workingStats.GetStatValue(i) != rolledStats.GetStatValue(i))
                    statLabels[i].TextColor = modifiedStatTextColor;
                else
                    statLabels[i].TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }

            // Update secondary stat labels
            damageModifierLabel.Text = DamageModifier(workingStats.Strength).ToString("+0;-0;0");
            maxEncumbranceLabel.Text = MaxEncumbrance(workingStats.Strength).ToString();
            spellPointsLabel.Text = SpellPoints(workingStats.Intelligence, dfClass.SpellPointMultiplierValue).ToString();
            magicResistLabel.Text = MagicResist(workingStats.Willpower).ToString();
            toHitModifierLabel.Text = ToHitModifier(workingStats.Agility).ToString("+0;-0;0");
            hitPointsModifierLabel.Text = HitPointsModifier(workingStats.Endurance).ToString("+0;-0;0");
            healingRateModifierLabel.Text = HealingRateModifier(workingStats.Endurance).ToString("+0;-0;0");
        }

        void SelectStat(int index)
        {
            selectedStat = index;
            spinner.Position = new Vector2(44, 21 + (22 * index));
        }

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
        }

        void Spinner_OnDownButtonClicked()
        {
            // Get working stat value
            int workingValue = workingStats.GetStatValue(selectedStat);

            // Working value cannot reduce below starting value or minWorkingValue
            if (workingValue == rolledStats.GetStatValue(selectedStat) || workingValue == minWorkingValue)
                return;

            // Remove a point from working stat and assign to pool
            workingStats.SetStatValue(selectedStat, workingValue - 1);
            bonusPool += 1;
            spinner.Value = bonusPool;
            UpdateStatLabels();
        }

        void RerollButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Reroll();
        }

        void SaveRoll_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            savedRolledStats.Copy(rolledStats);
            savedWorkingStats.Copy(workingStats);
            savedBonusPool = bonusPool;
            rollSaved = true;
        }

        void LoadRoll_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (rollSaved)
            {
                rolledStats.Copy(savedRolledStats);
                workingStats.Copy(savedWorkingStats);
                bonusPool = savedBonusPool;
                UpdateStatLabels();
                spinner.Value = bonusPool;
            }
        }

        void OkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion

        #region Formulas

        // NOTE:
        // These are currently only for display purposes during development.
        // Will later be moved into a formula provider class for real gaming.

        static int DamageModifier(int strength)
        {
            return (int)Mathf.Floor((float)strength / 10f) - 5;
        }

        static int MaxEncumbrance(int strength)
        {
            return (int)Mathf.Floor((float)strength * 1.5f);
        }

        static int SpellPoints(int intelligence, float multiplier)
        {
            return (int)Mathf.Floor((float)intelligence * multiplier);
        }

        static int MagicResist(int willpower)
        {
            return (int)Mathf.Floor((float)willpower / 10f);
        }

        static int ToHitModifier(int agility)
        {
            return (int)Mathf.Floor((float)agility / 10f) - 5;
        }

        static int HitPointsModifier(int endurance)
        {
            return (int)Mathf.Floor((float)endurance / 10f) - 5;
        }

        static int HealingRateModifier(int endurance)
        {
            return (int)Mathf.Floor((float)endurance / 10f) - 5;
        }

        #endregion
    }    
}