// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Implements control to distribute bonus skills.
    /// Used both on the "add bonus points" skills window and
    /// end summary window of character creation.
    /// </summary>
    public class SkillsRollout : Panel
    {
        const int bonusPoolPerSkillGroup = 6;

        const int minPrimarySkill = 28;
        const int minMajorSkill = 18;
        const int minMinorSkill = 13;
        const int minPrimaryBonusRoll = 0;
        const int maxPrimaryBonusRoll = 3;
        const int minMajorBonusRoll = 0;
        const int maxMajorBonusRoll = 3;
        const int minMinorBonusRoll = 0;
        const int maxMinorBonusRoll = 3;

        DaggerfallFont font;
        LeftRightSpinner primarySkillSpinner;
        LeftRightSpinner majorSkillSpinner;
        LeftRightSpinner minorSkillSpinner;
        int selectedPrimarySkill = 0;
        int selectedMajorSkill = 0;
        int selectedMinorSkill = 0;
        Color modifiedStatTextColor = Color.green;

        DaggerfallSkills startingSkills = new DaggerfallSkills();
        DaggerfallSkills workingSkills = new DaggerfallSkills();

        DFCareer.Skills[] primarySkills = new DFCareer.Skills[DaggerfallSkills.PrimarySkillsCount];
        DFCareer.Skills[] majorSkills = new DFCareer.Skills[DaggerfallSkills.MajorSkillsCount];
        DFCareer.Skills[] minorSkills = new DFCareer.Skills[DaggerfallSkills.MinorSkillsCount];

        TextLabel[] primarySkillLabels = new TextLabel[DaggerfallSkills.PrimarySkillsCount];
        TextLabel[] majorSkillLabels = new TextLabel[DaggerfallSkills.MajorSkillsCount];
        TextLabel[] minorSkillLabels = new TextLabel[DaggerfallSkills.MinorSkillsCount];

        TextLabel[] primarySkillValueLabels = new TextLabel[DaggerfallSkills.PrimarySkillsCount];
        TextLabel[] majorSkillValueLabels = new TextLabel[DaggerfallSkills.MajorSkillsCount];
        TextLabel[] minorSkillValueLabels = new TextLabel[DaggerfallSkills.MinorSkillsCount];

        IEnumerable<int> skillBonuses;

        public DaggerfallSkills StartingSkills
        {
            get { return startingSkills; }
            set { SetSkills(value, workingSkills); }
        }

        public DaggerfallSkills WorkingSkills
        {
            get { return workingSkills; }
            set { SetSkills(startingSkills, value); }
        }

        public IEnumerable<int> SkillBonuses
        {
            get { return skillBonuses; }
            set { skillBonuses = value; UpdateSkillValueLabels(); }
        }

        public int PrimarySkillBonusPoints
        {
            get { return primarySkillSpinner.Value; }
            set { primarySkillSpinner.Value = value; }
        }

        public int MajorSkillBonusPoints
        {
            get { return majorSkillSpinner.Value; }
            set { majorSkillSpinner.Value = value; }
        }

        public int MinorSkillBonusPoints
        {
            get { return minorSkillSpinner.Value; }
            set { minorSkillSpinner.Value = value; }
        }

        public SkillsRollout()
            : base()
        {
            font = DaggerfallUI.DefaultFont;
            SetupControls();
        }

        #region Public Methods

        public void Reroll()
        {
            // Set skills to all defaults
            startingSkills.SetDefaults();

            // Roll primary skills
            for (int i = 0; i < DaggerfallSkills.PrimarySkillsCount; i++)
            {
                int value = minPrimarySkill + UnityEngine.Random.Range(minPrimaryBonusRoll, maxPrimaryBonusRoll + 1);
                startingSkills.SetPermanentSkillValue(primarySkills[i], (short)value);
            }

            // Roll major skills
            for (int i = 0; i < DaggerfallSkills.MajorSkillsCount; i++)
            {
                int value = minMajorSkill + UnityEngine.Random.Range(minMajorBonusRoll, maxMajorBonusRoll + 1);
                startingSkills.SetPermanentSkillValue(majorSkills[i], (short)value);
            }

            // Roll minor skills
            for (int i = 0; i < DaggerfallSkills.MinorSkillsCount; i++)
            {
                int value = minMinorSkill + UnityEngine.Random.Range(minMinorBonusRoll, maxMinorBonusRoll + 1);
                startingSkills.SetPermanentSkillValue(minorSkills[i], (short)value);
            }

            // Copy to working skills
            workingSkills.Copy(startingSkills);

            // Reset bonus pool values
            primarySkillSpinner.Value = bonusPoolPerSkillGroup;
            majorSkillSpinner.Value = bonusPoolPerSkillGroup;
            minorSkillSpinner.Value = bonusPoolPerSkillGroup;

            // Update value labels
            UpdateSkillValueLabels();
        }

        public void SetClassSkills(DFCareer dfClass)
        {
            SetClassSkills(dfClass, true);
        }

        public void SetClassSkills(DFCareer dfClass, bool doReroll = true)
        {
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
            if (doReroll)
                Reroll();
        }

        #endregion

        #region Private Methods

        void SetupControls()
        {
            // Add primary skill labels
            Vector2 skillLabelPos = new Vector2(68, 32);
            Vector2 skillValueLabelPos = new Vector2(187, 32);
            Vector2 skillSelectButtonSize = new Vector2(106, 7);
            for (int i = 0; i < DaggerfallSkills.PrimarySkillsCount; i++)
            {
                primarySkillLabels[i] = DaggerfallUI.AddTextLabel(font, skillLabelPos, string.Empty, this);
                primarySkillLabels[i].Tag = i;
                primarySkillLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
                Button button = DaggerfallUI.AddButton(skillLabelPos, skillSelectButtonSize, this);
                button.Tag = i;
                button.OnMouseClick += PrimarySkills_OnMouseClick;

                primarySkillValueLabels[i] = DaggerfallUI.AddTextLabel(font, skillValueLabelPos, string.Empty, this);
                primarySkillValueLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;

                skillLabelPos.y += 10;
                skillValueLabelPos.y += 10;
            }

            // Add major skill labels
            skillLabelPos = new Vector2(68, 81);
            skillValueLabelPos = new Vector2(187, 81);
            for (int i = 0; i < DaggerfallSkills.MajorSkillsCount; i++)
            {
                majorSkillLabels[i] = DaggerfallUI.AddTextLabel(font, skillLabelPos, string.Empty, this);
                majorSkillLabels[i].Tag = i;
                majorSkillLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
                Button button = DaggerfallUI.AddButton(skillLabelPos, skillSelectButtonSize, this);
                button.Tag = i;
                button.OnMouseClick += MajorSkills_OnMouseClick;

                majorSkillValueLabels[i] = DaggerfallUI.AddTextLabel(font, skillValueLabelPos, string.Empty, this);
                majorSkillValueLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;

                skillLabelPos.y += 10;
                skillValueLabelPos.y += 10;
            }

            // Add minor skill labels
            skillLabelPos = new Vector2(68, 130);
            skillValueLabelPos = new Vector2(187, 130);
            for (int i = 0; i < DaggerfallSkills.MinorSkillsCount; i++)
            {
                minorSkillLabels[i] = DaggerfallUI.AddTextLabel(font, skillLabelPos, string.Empty, this);
                minorSkillLabels[i].Tag = i;
                minorSkillLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
                Button button = DaggerfallUI.AddButton(skillLabelPos, skillSelectButtonSize, this);
                button.Tag = i;
                button.OnMouseClick += MinorSkills_OnMouseClick;

                minorSkillValueLabels[i] = DaggerfallUI.AddTextLabel(font, skillValueLabelPos, string.Empty, this);
                minorSkillValueLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;

                skillLabelPos.y += 10;
                skillValueLabelPos.y += 10;
            }

            // Add primary skill spinner
            primarySkillSpinner = new LeftRightSpinner();
            this.Components.Add(primarySkillSpinner);
            primarySkillSpinner.OnLeftButtonClicked += PrimarySkillSpinner_OnLeftButtonClicked;
            primarySkillSpinner.OnRightButtonClicked += PrimarySkillSpinner_OnRightButtonClicked;
            primarySkillSpinner.Value = bonusPoolPerSkillGroup;
            SelectPrimarySkill(0);

            // Add major skill spinner
            majorSkillSpinner = new LeftRightSpinner();
            this.Components.Add(majorSkillSpinner);
            majorSkillSpinner.OnLeftButtonClicked += MajorSkillSpinner_OnLeftButtonClicked;
            majorSkillSpinner.OnRightButtonClicked += MajorSkillSpinner_OnRightButtonClicked;
            majorSkillSpinner.Value = bonusPoolPerSkillGroup;
            SelectMajorSkill(0);

            // Add minor skill spinner
            minorSkillSpinner = new LeftRightSpinner();
            this.Components.Add(minorSkillSpinner);
            minorSkillSpinner.OnLeftButtonClicked += MinorSkillSpinner_OnLeftButtonClicked;
            minorSkillSpinner.OnRightButtonClicked += MinorSkillSpinner_OnRightButtonClicked;
            minorSkillSpinner.Value = bonusPoolPerSkillGroup;
            SelectMinorSkill(0);
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
                int startingSkill = startingSkills.GetPermanentSkillValue(primarySkills[i]);
                int workingSkill = workingSkills.GetPermanentSkillValue(primarySkills[i]);

                if (skillBonuses != null)
                {
                    int bonusSkill = skillBonuses.ElementAt((int)primarySkills[i]);
                    primarySkillValueLabels[i].Text = (workingSkill + bonusSkill).ToString();
                }
                else
                {
                    primarySkillValueLabels[i].Text = workingSkill.ToString();
                }

                if (workingSkill != startingSkill)
                    primarySkillValueLabels[i].TextColor = modifiedStatTextColor;
                else
                    primarySkillValueLabels[i].TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }

            // Set major skill values label text
            for (int i = 0; i < DaggerfallSkills.MajorSkillsCount; i++)
            {
                int startingSkill = startingSkills.GetPermanentSkillValue(majorSkills[i]);
                int workingSkill = workingSkills.GetPermanentSkillValue(majorSkills[i]);

                if (skillBonuses != null)
                {
                    int bonusSkill = skillBonuses.ElementAt((int)majorSkills[i]);
                    majorSkillValueLabels[i].Text = (workingSkill + bonusSkill).ToString();
                }
                else
                {
                    majorSkillValueLabels[i].Text = workingSkill.ToString();
                }

                if (workingSkill != startingSkill)
                    majorSkillValueLabels[i].TextColor = modifiedStatTextColor;
                else
                    majorSkillValueLabels[i].TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }

            // Set minor skill values label text
            for (int i = 0; i < DaggerfallSkills.MinorSkillsCount; i++)
            {
                int startingSkill = startingSkills.GetPermanentSkillValue(minorSkills[i]);
                int workingSkill = workingSkills.GetPermanentSkillValue(minorSkills[i]);

                if (skillBonuses != null)
                {
                    int bonusSkill = skillBonuses.ElementAt((int)minorSkills[i]);
                    minorSkillValueLabels[i].Text = (workingSkill + bonusSkill).ToString();
                }
                else
                {
                    minorSkillValueLabels[i].Text = workingSkill.ToString();
                }

                if (workingSkill != startingSkill)
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

        void AddSkillPoint(DFCareer.Skills skill, LeftRightSpinner spinner)
        {
            // Bonus point pool cannot fall below zero
            int workingValue = workingSkills.GetPermanentSkillValue(skill);
            if (spinner.Value == 0)
                return;

            // Remove a point from pool and assign to skill
            spinner.Value -= 1;
            workingSkills.SetPermanentSkillValue(skill, (short)(workingValue + 1));
            UpdateSkillValueLabels();
        }

        void RemoveSkillPoint(DFCareer.Skills skill, LeftRightSpinner spinner)
        {
            // Working skill value cannot fall below rolled skill value
            int workingValue = workingSkills.GetPermanentSkillValue(skill);
            if (workingValue == startingSkills.GetPermanentSkillValue(skill))
                return;

            // Remove a point from skill and assign to pool
            spinner.Value += 1;
            workingSkills.SetPermanentSkillValue(skill, (short)(workingValue - 1));
            UpdateSkillValueLabels();
        }

        void SetSkills(DaggerfallSkills startingSkills, DaggerfallSkills workingSkills)
        {
            this.startingSkills.Copy(startingSkills);
            this.workingSkills.Copy(workingSkills);
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

        #endregion
    }
}