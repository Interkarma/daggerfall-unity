// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
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
    /// Implements custom class creator window.
    /// </summary>
    public class CreateCharCustomClass : DaggerfallPopupWindow
    {
        const string nativeImgName = "CUST00I0.IMG";
        const string nativeDaggerImgName = "CUST08I0.IMG";
        const int maxHpPerLevel = 30;
        const int minHpPerLevel = 4;
        const int defaultHpPerLevel = 8;

        Texture2D nativeTexture;
        Texture2D nativeDaggerTexture;
        DaggerfallFont font;
        StatsRollout statsRollout;
        TextBox textBox = new TextBox();
        DFCareer createdClass;
        int lastSkillButtonId;
        Dictionary<string, DFCareer.Skills> skillsDict;
        List<string> skillsList;
        int hpPerLevel = defaultHpPerLevel;
        int difficultyPoints = 0;

        public DFCareer CreatedClass
        {
            get { return createdClass; }
        }

        #region UI Panels

        Panel daggerPanel = new Panel();

        #endregion

        #region UI Rects

        Rect[] skillButtonRects = new Rect[]
        {
            new Rect(66, 31, 108, 8),
            new Rect(66, 41, 108, 8),
            new Rect(66, 51, 108, 8),
            new Rect(66, 80, 108, 8),
            new Rect(66, 90, 108, 8),
            new Rect(66, 100, 108, 8),
            new Rect(66, 129, 108, 8),
            new Rect(66, 139, 108, 8),
            new Rect(66, 149, 108, 8),
            new Rect(66, 159, 108, 8),
            new Rect(66, 169, 108, 8),
            new Rect(66, 179, 108, 8)
        };
        Rect hitPointsUpButtonRect = new Rect(252, 46, 8, 10);
        Rect hitPointsDownButtonRect = new Rect(252, 57, 8, 10);
        Rect helpButtonRect = new Rect(249, 74, 66, 22);
        Rect specialAdvantageButtonRect = new Rect(249, 98, 66, 22);
        Rect specialDisadvantageButtonRect = new Rect(249, 122, 66, 22);
        Rect reputationButtonRect = new Rect(249, 146, 66, 22);
        Rect exitButtonRect = new Rect(263, 172, 38, 21);

        #endregion

        #region Buttons

        Button[] skillButtons = new Button[12];
        Button hitPointsUpButton;
        Button hitPointsDownButton;
        Button helpButton;
        Button specialAdvantageButton;
        Button specialDisadvantageButton;
        Button reputationButton;
        Button exitButton;

        #endregion

        #region Text Labels

        TextLabel[] skillLabels = new TextLabel[12];
        TextLabel hpLabel;

        #endregion

        public CreateCharCustomClass(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        #region Setup Methods

        protected override void Setup()
        {
            if (IsSetup)
                return;

            // Load native textures
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            nativeDaggerTexture = DaggerfallUI.GetTextureFromImg(nativeDaggerImgName);
            if (!nativeTexture || !nativeDaggerTexture)
                throw new Exception("CreateCharCustomClass: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Add stats rollout
            statsRollout = new StatsRollout(false, true);
            statsRollout.Position = new Vector2(0, 0);
            NativePanel.Components.Add(statsRollout);

            // Add name textbox
            textBox.Position = new Vector2(100, 5);
            textBox.Size = new Vector2(214, 7);
            NativePanel.Components.Add(textBox);

            // Initialize character class
            createdClass = new DFCareer();

            // Initiate UI components
            font = DaggerfallUI.DefaultFont;
            SetupButtons();
            hpLabel = DaggerfallUI.AddTextLabel(font, new Vector2(285, 55), hpPerLevel.ToString(), NativePanel);
            daggerPanel.Size = new Vector2(24, 9);
            daggerPanel.BackgroundTexture = nativeDaggerTexture;
            NativePanel.Components.Add(daggerPanel);
            UpdateDifficulty();

            // Setup skills dictionary
            skillsDict = new Dictionary<string, DFCareer.Skills>();
            foreach (DFCareer.Skills skill in Enum.GetValues(typeof(DFCareer.Skills)))
            {
                skillsDict.Add(DaggerfallUnity.Instance.TextProvider.GetSkillName(skill), skill);
            }
            skillsDict.Remove(string.Empty); // Don't include "none" skill value.
            skillsList = new List<string>(skillsDict.Keys);
            skillsList.Sort(); // Sort skills alphabetically a la classic.

            IsSetup = true;
        }

        protected void SetupButtons()
        {
            // Add skill selector buttons
            for (int i = 0; i < skillButtons.Length; i++) 
            {
                skillButtons[i] = DaggerfallUI.AddButton(skillButtonRects[i], NativePanel);
                skillButtons[i].Tag = i;
                skillButtons[i].OnMouseClick += skillButton_OnMouseClick;
                skillLabels[i] = DaggerfallUI.AddTextLabel(font, new Vector2(3, 2), string.Empty, skillButtons[i]);
            }
            // HP spinners
            hitPointsUpButton = DaggerfallUI.AddButton(hitPointsUpButtonRect, NativePanel);
            hitPointsUpButton.OnMouseClick += HitPointsUpButton_OnMouseClick;
            hitPointsDownButton = DaggerfallUI.AddButton(hitPointsDownButtonRect, NativePanel);
            hitPointsDownButton.OnMouseUp += HitPointsDownButton_OnMouseClick;

            // Special Advantages/Disadvantages
            specialAdvantageButton = DaggerfallUI.AddButton(specialAdvantageButtonRect, NativePanel);
            specialAdvantageButton.OnMouseClick += specialAdvantageButton_OnMouseClick;
            specialDisadvantageButton = DaggerfallUI.AddButton(specialDisadvantageButtonRect, NativePanel);
            specialDisadvantageButton.OnMouseClick += specialDisadvantageButton_OnMouseClick;

            // Reputations
            reputationButton = DaggerfallUI.AddButton(reputationButtonRect, NativePanel);
            reputationButton.OnMouseClick += ReputationButton_OnMouseClick;
        }

        #endregion

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }

        #region Event Handlers

        void skillButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            DaggerfallListPickerWindow skillPicker = new DaggerfallListPickerWindow(uiManager, this);
            skillPicker.OnItemPicked += SkillPicker_OnItemPicked;
            foreach (string skillName in skillsList)
            {
                skillPicker.ListBox.AddItem(skillName);
            }
            lastSkillButtonId = (int)sender.Tag;
            uiManager.PushWindow(skillPicker);
        }

        void SkillPicker_OnItemPicked(int index, string skillName)
        {
            CloseWindow();
            switch (lastSkillButtonId)
            {
                case 0:
                    createdClass.PrimarySkill1 = skillsDict[skillName];
                    break;
                case 1:
                    createdClass.PrimarySkill2 = skillsDict[skillName];
                    break;
                case 2:
                    createdClass.PrimarySkill3 = skillsDict[skillName];
                    break;
                case 3:
                    createdClass.MajorSkill1 = skillsDict[skillName];
                    break;
                case 4:
                    createdClass.MajorSkill2 = skillsDict[skillName];
                    break;
                case 5:
                    createdClass.MajorSkill3 = skillsDict[skillName];
                    break;
                case 6:
                    createdClass.MinorSkill1 = skillsDict[skillName];
                    break;
                case 7:
                    createdClass.MinorSkill2 = skillsDict[skillName];
                    break;
                case 8:
                    createdClass.MinorSkill3 = skillsDict[skillName];
                    break;
                case 9:
                    createdClass.MinorSkill4 = skillsDict[skillName];
                    break;
                case 10:
                    createdClass.MinorSkill5 = skillsDict[skillName];
                    break;
                case 11:
                    createdClass.MinorSkill6 = skillsDict[skillName];
                    break;
                default:
                    return;
            }
            skillsList.Remove(skillName);
            if (skillLabels[lastSkillButtonId].Text != string.Empty)
            {
                skillsList.Add(skillLabels[lastSkillButtonId].Text);
            }
            skillsList.Sort();
            skillLabels[lastSkillButtonId].Text = skillName;
        }

        public void HitPointsUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            if (hpPerLevel != maxHpPerLevel)
            {
                hpPerLevel++;
                hpLabel.Text = hpPerLevel.ToString();
                UpdateDifficulty();
            }
        }

        public void HitPointsDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            if (hpPerLevel != minHpPerLevel)
            {
                hpPerLevel--;
                hpLabel.Text = hpPerLevel.ToString();
                UpdateDifficulty();
            }
        }

        public void specialAdvantageButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            CreateCharSpecialAdvantageWindow createCharSpecialAdvantageWindow = new CreateCharSpecialAdvantageWindow(uiManager, this);
            uiManager.PushWindow(createCharSpecialAdvantageWindow);
        }

        public void specialDisadvantageButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            CreateCharSpecialAdvantageWindow createCharSpecialAdvantageWindow = new CreateCharSpecialAdvantageWindow(uiManager, this, true);
            uiManager.PushWindow(createCharSpecialAdvantageWindow);
        }

        void ReputationButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CreateCharReputationWindow createCharReputationWindow = new CreateCharReputationWindow(uiManager, this);
            uiManager.PushWindow(createCharReputationWindow);
        }

        #endregion

        #region Private methods

        private void UpdateDifficulty()
        {
            const int defaultDaggerX = 220;
            const int defaultDaggerY = 115;

            // hp adjustment
            if (hpPerLevel >= defaultHpPerLevel)
            {
                difficultyPoints = hpPerLevel - defaultHpPerLevel; // +1 pt for each hp above default
            } 
            else
            {
                difficultyPoints = -(2 * (defaultHpPerLevel - hpPerLevel)); // -2 pts for each hp below default
            }
            // TODO: adjustments for special advantages/disadvantages

            // Reposition the difficulty dagger
            int daggerY = 0;
            if (difficultyPoints >= 0)
            {
                daggerY = (int)(defaultDaggerY - (37 * (difficultyPoints / 40f)));
            } 
            else
            {
                daggerY = (int)(defaultDaggerY + (41 * (-difficultyPoints / 12f)));
            }
            daggerPanel.Position = new Vector2(defaultDaggerX, daggerY);
        }

        #endregion
    }    
}