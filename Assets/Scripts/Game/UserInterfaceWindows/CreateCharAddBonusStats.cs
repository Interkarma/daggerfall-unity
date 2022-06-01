// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements "Add Bonus Points" to primary stats window.
    /// </summary>
    public class CreateCharAddBonusStats : DaggerfallPopupWindow
    {
        const string nativeImgName = "CHAR02I0.IMG";
        const int strYouMustDistributeYourBonusPoints = 14;

        Texture2D nativeTexture;
        DaggerfallFont font;
        TextLabel damageModifierLabel;
        TextLabel maxEncumbranceLabel;
        TextLabel spellPointsLabel;
        TextLabel magicResistLabel;
        TextLabel toHitModifierLabel;
        TextLabel hitPointsModifierLabel;
        TextLabel healingRateModifierLabel;
        StatsRollout statsRollout;

        DFCareer dfClass;
        bool rollSaved = false;
        DaggerfallStats savedRolledStats = new DaggerfallStats();
        DaggerfallStats savedWorkingStats = new DaggerfallStats();
        int savedBonusPool;

        public DFCareer DFClass
        {
            get { return dfClass; }
            set { dfClass = value; }
        }

        public DaggerfallStats StartingStats
        {
            get { return statsRollout.StartingStats; }
        }

        public DaggerfallStats WorkingStats
        {
            get { return statsRollout.WorkingStats; }
        }

        public CreateCharAddBonusStats(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            if (IsSetup)
                return;

            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharAddBonusStats: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Add stats rollout
            statsRollout = new StatsRollout();
            statsRollout.Position = new Vector2(0, 0);
            statsRollout.OnStatChanged += StatsRollout_OnStatChanged;
            NativePanel.Components.Add(statsRollout);

            // Add secondary stat labels
            font = DaggerfallUI.DefaultFont;
            damageModifierLabel = DaggerfallUI.AddTextLabel(font, new Vector2(83, 22), string.Empty, NativePanel);
            maxEncumbranceLabel = DaggerfallUI.AddTextLabel(font, new Vector2(103, 32), string.Empty, NativePanel);
            spellPointsLabel = DaggerfallUI.AddTextLabel(font, new Vector2(112, 49), string.Empty, NativePanel);
            magicResistLabel = DaggerfallUI.AddTextLabel(font, new Vector2(121, 71), string.Empty, NativePanel);
            toHitModifierLabel = DaggerfallUI.AddTextLabel(font, new Vector2(97, 93), string.Empty, NativePanel);
            hitPointsModifierLabel = DaggerfallUI.AddTextLabel(font, new Vector2(101, 110), string.Empty, NativePanel);
            healingRateModifierLabel = DaggerfallUI.AddTextLabel(font, new Vector2(122, 120), string.Empty, NativePanel);

            // Fix secondary stat shadow colors to match game
            damageModifierLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            maxEncumbranceLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            spellPointsLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            magicResistLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            toHitModifierLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            hitPointsModifierLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            healingRateModifierLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;

            // Add "Reroll" button
            Button rerollButton = DaggerfallUI.AddButton(new Rect(263, 147, 39, 22), NativePanel);
            rerollButton.OnMouseClick += RerollButton_OnMouseClick;

            // Add "Save Roll" button
            Button saveRoll = DaggerfallUI.AddButton(new Rect(162, 162, 71, 9), NativePanel);
            saveRoll.OnMouseClick += SaveRoll_OnMouseClick;

            // Add "Load Roll" button
            Button loadRoll = DaggerfallUI.AddButton(new Rect(162, 171, 71, 9), NativePanel);
            loadRoll.OnMouseClick += LoadRoll_OnMouseClick;

            // Add "OK" button
            Button okButton = DaggerfallUI.AddButton(new Rect(263, 172, 39, 22), NativePanel);
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
            Setup();
            statsRollout.Reroll(dfClass);
            UpdateSecondaryStatLabels();
        }

        #endregion

        #region Private Methods

        void UpdateSecondaryStatLabels()
        {
            DaggerfallStats workingStats = statsRollout.WorkingStats;
            damageModifierLabel.Text = FormulaHelper.DamageModifier(workingStats.LiveStrength).ToString("+0;-0;0");
            maxEncumbranceLabel.Text = FormulaHelper.MaxEncumbrance(workingStats.LiveStrength).ToString();
            spellPointsLabel.Text = FormulaHelper.SpellPoints(workingStats.LiveIntelligence, dfClass.SpellPointMultiplierValue).ToString();
            magicResistLabel.Text = FormulaHelper.MagicResist(workingStats.LiveWillpower).ToString();
            toHitModifierLabel.Text = FormulaHelper.ToHitModifier(workingStats.LiveAgility).ToString("+0;-0;0");
            hitPointsModifierLabel.Text = FormulaHelper.HitPointsModifier(workingStats.LiveEndurance).ToString("+0;-0;0");
            healingRateModifierLabel.Text = FormulaHelper.HealingRateModifier(workingStats.LiveEndurance).ToString("+0;-0;0");
        }

        void RerollButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Reroll();
        }

        void SaveRoll_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            savedRolledStats.Copy(statsRollout.StartingStats);
            savedWorkingStats.Copy(statsRollout.WorkingStats);
            savedBonusPool = statsRollout.BonusPool;
            rollSaved = true;
        }

        void LoadRoll_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (rollSaved)
            {
                statsRollout.SetStats(savedRolledStats, savedWorkingStats, savedBonusPool);
                UpdateSecondaryStatLabels();
            }
        }

        void OkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (statsRollout.BonusPool > 0)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(strYouMustDistributeYourBonusPoints);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
            }
            else
            {
                CloseWindow();
            }
        }

        void StatsRollout_OnStatChanged()
        {
            UpdateSecondaryStatLabels();
        }

        #endregion
    }    
}