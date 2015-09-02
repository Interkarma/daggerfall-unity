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
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements "Add Bonus Points" to skills window.
    /// </summary>
    public class CreateCharAddBonusSkills : DaggerfallPopupWindow
    {
        const string nativeImgName = "CHAR03I0.IMG";
        const int strYouMustDistributeYourBonusPoints = 14;

        // This value is normally 6
        // Increased to 10 temporarily to compensate for biography bonuses
        // This will be reverted to 6 once biographies implemented
        const int bonusPoolPerSkillGroup = 10;

        const int minPrimarySkill = 28;
        const int minMajorSkill = 18;
        const int minMinorSkill = 12;
        const int minPrimaryBonusRoll = 0;
        const int maxPrimaryBonusRoll = 3;
        const int minMajorBonusRoll = 0;
        const int maxMajorBonusRoll = 4;
        const int minMinorBonusRoll = 0;
        const int maxMinorBonusRoll = 3;

        Texture2D nativeTexture;
        DaggerfallFont font;

        LeftRightSpinner primarySkillSpinner;
        LeftRightSpinner majorSkillSpinner;
        LeftRightSpinner minorSkillSpinner;
        int selectedPrimarySkill = 0;
        int selectedMajorSkill = 0;
        int selectedMinorSkill = 0;

        DFClass dfClass;
        DaggerfallSkills rolledSkills;
        DaggerfallSkills workingSkills;
        Color modifiedStatTextColor = Color.green;

        DFClass.Skills[] primarySkills = new DFClass.Skills[DaggerfallSkills.PrimarySkillsCount];
        DFClass.Skills[] majorSkills = new DFClass.Skills[DaggerfallSkills.MajorSkillsCount];
        DFClass.Skills[] minorSkills = new DFClass.Skills[DaggerfallSkills.MinorSkillsCount];

        TextLabel[] primarySkillLabels = new TextLabel[DaggerfallSkills.PrimarySkillsCount];
        TextLabel[] majorSkillLabels = new TextLabel[DaggerfallSkills.MajorSkillsCount];
        TextLabel[] minorSkillLabels = new TextLabel[DaggerfallSkills.MinorSkillsCount];

        TextLabel[] primarySkillValueLabels = new TextLabel[DaggerfallSkills.PrimarySkillsCount];
        TextLabel[] majorSkillValueLabels = new TextLabel[DaggerfallSkills.MajorSkillsCount];
        TextLabel[] minorSkillValueLabels = new TextLabel[DaggerfallSkills.MinorSkillsCount];

        public DFClass DFClass
        {
            get { return dfClass; }
            set { SetClass(value); }
        }

        public CreateCharAddBonusSkills(IUserInterfaceManager uiManager)
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
                throw new Exception("CreateCharAddBonusSkillsWindow: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Add primary skill labels
            font = DaggerfallUI.Instance.DefaultFont;
            Vector2 skillLabelPos = new Vector2(68, 32);
            Vector2 skillValueLabelPos = new Vector2(187, 32);
            Vector2 skillSelectButtonSize = new Vector2(106, 7);
            for (int i = 0; i < DaggerfallSkills.PrimarySkillsCount; i++)
            {
                primarySkillLabels[i] = AddTextLabel(font, skillLabelPos, string.Empty);
                primarySkillLabels[i].Tag = i;
                primarySkillLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
                Button button = AddButton(skillLabelPos, skillSelectButtonSize);
                button.Tag = i;
                button.OnMouseClick += PrimarySkills_OnMouseClick;

                primarySkillValueLabels[i] = AddTextLabel(font, skillValueLabelPos, string.Empty);
                primarySkillValueLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;

                skillLabelPos.y += 10;
                skillValueLabelPos.y += 10;
            }

            // Add major skill labels
            skillLabelPos = new Vector2(68, 81);
            skillValueLabelPos = new Vector2(187, 81);
            for (int i = 0; i < DaggerfallSkills.MajorSkillsCount; i++)
            {
                majorSkillLabels[i] = AddTextLabel(font, skillLabelPos, string.Empty);
                majorSkillLabels[i].Tag = i;
                majorSkillLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
                Button button = AddButton(skillLabelPos, skillSelectButtonSize);
                button.Tag = i;
                button.OnMouseClick += MajorSkills_OnMouseClick;

                majorSkillValueLabels[i] = AddTextLabel(font, skillValueLabelPos, string.Empty);
                majorSkillValueLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;

                skillLabelPos.y += 10;
                skillValueLabelPos.y += 10;
            }

            // Add minor skill labels
            skillLabelPos = new Vector2(68, 130);
            skillValueLabelPos = new Vector2(187, 130);
            for (int i = 0; i < DaggerfallSkills.MinorSkillsCount; i++)
            {
                minorSkillLabels[i] = AddTextLabel(font, skillLabelPos, string.Empty);
                minorSkillLabels[i].Tag = i;
                minorSkillLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
                Button button = AddButton(skillLabelPos, skillSelectButtonSize);
                button.Tag = i;
                button.OnMouseClick += MinorSkills_OnMouseClick;

                minorSkillValueLabels[i] = AddTextLabel(font, skillValueLabelPos, string.Empty);
                minorSkillValueLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;

                skillLabelPos.y += 10;
                skillValueLabelPos.y += 10;
            }

            // Add primary skill spinner
            primarySkillSpinner = new LeftRightSpinner();
            NativePanel.Components.Add(primarySkillSpinner);
            primarySkillSpinner.OnLeftButtonClicked += PrimarySkillSpinner_OnLeftButtonClicked;
            primarySkillSpinner.OnRightButtonClicked += PrimarySkillSpinner_OnRightButtonClicked;
            primarySkillSpinner.Value = bonusPoolPerSkillGroup;
            SelectPrimarySkill(0);

            // Add major skill spinner
            majorSkillSpinner = new LeftRightSpinner();
            NativePanel.Components.Add(majorSkillSpinner);
            majorSkillSpinner.OnLeftButtonClicked += MajorSkillSpinner_OnLeftButtonClicked;
            majorSkillSpinner.OnRightButtonClicked += MajorSkillSpinner_OnRightButtonClicked;
            majorSkillSpinner.Value = bonusPoolPerSkillGroup;
            SelectMajorSkill(0);

            // Add minor skill spinner
            minorSkillSpinner = new LeftRightSpinner();
            NativePanel.Components.Add(minorSkillSpinner);
            minorSkillSpinner.OnLeftButtonClicked += MinorSkillSpinner_OnLeftButtonClicked;
            minorSkillSpinner.OnRightButtonClicked += MinorSkillSpinner_OnRightButtonClicked;
            minorSkillSpinner.Value = bonusPoolPerSkillGroup;
            SelectMinorSkill(0);

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

            // Set skills to all defaults
            rolledSkills.SetDefaults();

            // Roll primary skills
            for (int i = 0; i < DaggerfallSkills.PrimarySkillsCount; i++)
            {
                int value = minPrimarySkill + UnityEngine.Random.Range(minPrimaryBonusRoll, maxPrimaryBonusRoll + 1);
                rolledSkills.SetSkillValue(primarySkills[i], (short)value);
            }

            // Roll major skills
            for (int i = 0; i < DaggerfallSkills.MajorSkillsCount; i++)
            {
                int value = minMajorSkill + UnityEngine.Random.Range(minMajorBonusRoll, maxMajorBonusRoll + 1);
                rolledSkills.SetSkillValue(majorSkills[i], (short)value);
            }

            // Roll minor skills
            for (int i = 0; i < DaggerfallSkills.MinorSkillsCount; i++)
            {
                int value = minMinorSkill + UnityEngine.Random.Range(minMinorBonusRoll, maxMinorBonusRoll + 1);
                rolledSkills.SetSkillValue(minorSkills[i], (short)value);
            }

            // Copy to working skills
            workingSkills.Copy(rolledSkills);

            // Reset bonus pool values
            primarySkillSpinner.Value = bonusPoolPerSkillGroup;
            majorSkillSpinner.Value = bonusPoolPerSkillGroup;
            minorSkillSpinner.Value = bonusPoolPerSkillGroup;

            // Update value labels
            UpdateSkillValueLabels();
        }

        #endregion

        #region Private Methods

        void SetClass(DFClass dfClass)
        {
            Setup();

            this.dfClass = dfClass;

            // Set primary, major, minor skills from class template
            primarySkills[0] = dfClass.PrimarySkill1;
            primarySkills[1] = dfClass.PrimarySkill2;
            primarySkills[2] = dfClass.PrimarySkill3;
            majorSkills[0] = dfClass.MajorSkill1;
            majorSkills[1] = dfClass.MajorSkill2;
            majorSkills[2] = dfClass.MajorSkill3;
            minorSkills[0] = dfClass.MinorSkill1;
            minorSkills[1] = dfClass.MinorSkill2;
            minorSkills[2] = dfClass.MinorSkill3;
            minorSkills[3] = dfClass.MinorSkill4;
            minorSkills[4] = dfClass.MinorSkill5;
            minorSkills[5] = dfClass.MinorSkill6;

            UpdateSkillLabels();
            Reroll();
        }

        void UpdateSkillLabels()
        {
            ITextProvider textProvider = DaggerfallUnity.Instance.TextProvider;

            // Set primary skills label text
            for (int i = 0; i < DaggerfallSkills.PrimarySkillsCount; i++)
            {
                primarySkillLabels[i].Text = textProvider.GetSkillName(primarySkills[i]);
            }

            // Set major skills label text
            for (int i = 0; i < DaggerfallSkills.MajorSkillsCount; i++)
            {
                majorSkillLabels[i].Text = textProvider.GetSkillName(majorSkills[i]);
            }

            // Set minor skills label text
            for (int i = 0; i < DaggerfallSkills.MinorSkillsCount; i++)
            {
                minorSkillLabels[i].Text = textProvider.GetSkillName(minorSkills[i]);
            }
        }

        void UpdateSkillValueLabels()
        {
            // Set primary skill values label text
            for (int i = 0; i < DaggerfallSkills.PrimarySkillsCount; i++)
            {
                int rolledSkill = rolledSkills.GetSkillValue(primarySkills[i]);
                int workingSkill = workingSkills.GetSkillValue(primarySkills[i]);

                primarySkillValueLabels[i].Text = workingSkill.ToString();
                if (workingSkill != rolledSkill)
                    primarySkillValueLabels[i].TextColor = modifiedStatTextColor;
                else
                    primarySkillValueLabels[i].TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }

            // Set major skill values label text
            for (int i = 0; i < DaggerfallSkills.MajorSkillsCount; i++)
            {
                int rolledSkill = rolledSkills.GetSkillValue(majorSkills[i]);
                int workingSkill = workingSkills.GetSkillValue(majorSkills[i]);

                majorSkillValueLabels[i].Text = workingSkill.ToString();
                if (workingSkill != rolledSkill)
                    majorSkillValueLabels[i].TextColor = modifiedStatTextColor;
                else
                    majorSkillValueLabels[i].TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }

            // Set minor skill values label text
            for (int i = 0; i < DaggerfallSkills.MinorSkillsCount; i++)
            {
                int rolledSkill = rolledSkills.GetSkillValue(minorSkills[i]);
                int workingSkill = workingSkills.GetSkillValue(minorSkills[i]);

                minorSkillValueLabels[i].Text = workingSkill.ToString();
                if (workingSkill != rolledSkill)
                    minorSkillValueLabels[i].TextColor = modifiedStatTextColor;
                else
                    minorSkillValueLabels[i].TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }
        }

        void SelectPrimarySkill(int index)
        {
            selectedPrimarySkill = index;
            primarySkillSpinner.Position = new Vector2(203, 31 + (10 * index));
        }

        void SelectMajorSkill(int index)
        {
            selectedMajorSkill = index;
            majorSkillSpinner.Position = new Vector2(203, 80 + (10 * index));
        }

        void SelectMinorSkill(int index)
        {
            selectedMinorSkill = index;
            minorSkillSpinner.Position = new Vector2(203, 129 + (10 * index));
        }

        void AddSkillPoint(DFClass.Skills skill, LeftRightSpinner spinner)
        {
            // Bonus point pool cannot fall below zero
            int workingValue = workingSkills.GetSkillValue(skill);
            if (spinner.Value == 0)
                return;

            // Remove a point from pool and assign to skill
            spinner.Value -= 1;
            workingSkills.SetSkillValue(skill, (short)(workingValue + 1));
            UpdateSkillValueLabels();
        }

        void RemoveSkillPoint(DFClass.Skills skill, LeftRightSpinner spinner)
        {
            // Working skill value cannot fall below rolled skill value
            int workingValue = workingSkills.GetSkillValue(skill);
            if (workingValue == rolledSkills.GetSkillValue(skill))
                return;

            // Remove a point from skill and assign to pool
            spinner.Value += 1;
            workingSkills.SetSkillValue(skill, (short)(workingValue - 1));
            UpdateSkillValueLabels();
        }

        #endregion

        #region Event Handlers

        void PrimarySkills_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectPrimarySkill((int)sender.Tag);
        }

        void MajorSkills_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectMajorSkill((int)sender.Tag);
        }

        void MinorSkills_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectMinorSkill((int)sender.Tag);
        }

        void PrimarySkillSpinner_OnLeftButtonClicked()
        {
            RemoveSkillPoint(primarySkills[selectedPrimarySkill], primarySkillSpinner);
        }

        void PrimarySkillSpinner_OnRightButtonClicked()
        {
            AddSkillPoint(primarySkills[selectedPrimarySkill], primarySkillSpinner);
        }

        void MajorSkillSpinner_OnLeftButtonClicked()
        {
            RemoveSkillPoint(majorSkills[selectedMajorSkill], majorSkillSpinner);
        }

        void MajorSkillSpinner_OnRightButtonClicked()
        {
            AddSkillPoint(majorSkills[selectedMajorSkill], majorSkillSpinner);
        }

        void MinorSkillSpinner_OnLeftButtonClicked()
        {
            RemoveSkillPoint(minorSkills[selectedMinorSkill], minorSkillSpinner);
        }

        void MinorSkillSpinner_OnRightButtonClicked()
        {
            AddSkillPoint(minorSkills[selectedMinorSkill], minorSkillSpinner);
        }

        void OkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (primarySkillSpinner.Value > 0 || majorSkillSpinner.Value > 0 || minorSkillSpinner.Value > 0)
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

        #endregion
    }
}