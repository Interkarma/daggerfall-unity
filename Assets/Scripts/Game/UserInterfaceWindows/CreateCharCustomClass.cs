// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.UserInterface;
using System.Collections;

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
        const int minDifficultyPoints = -12;
        const int maxDifficultyPoints = 40;

        const float daggerTrailLingerTime = 1.0f;

        const int strNameYourClass = 301;
        const int strSetSkills = 300;
        const int strDistributeStats = 302;
        const int strAdvancingDaggerInRed = 306;
        Texture2D nativeTexture;
        Texture2D nativeDaggerTexture;
        DaggerfallFont font;
        StatsRollout statsRollout;
        TextBox nameTextBox = new TextBox();
        DFCareer createdClass;
        int lastSkillButtonId;
        Dictionary<string, DFCareer.Skills> skillsDict;
        List<string> skillsList;
        Dictionary<string, int> helpDict;
        int difficultyPoints = 0;
        int advantageAdjust = 0;
        int disadvantageAdjust = 0;
        List<CreateCharSpecialAdvantageWindow.SpecialAdvDis> advantages = new List<CreateCharSpecialAdvantageWindow.SpecialAdvDis>();
        List<CreateCharSpecialAdvantageWindow.SpecialAdvDis> disadvantages = new List<CreateCharSpecialAdvantageWindow.SpecialAdvDis>();
        short merchantsRep = 0;
        short peasantsRep = 0;
        short scholarsRep = 0;
        short nobilityRep = 0;
        short underworldRep = 0;

        #region Windows

        CreateCharReputationWindow createCharReputationWindow;
        CreateCharSpecialAdvantageWindow createCharSpecialAdvantageWindow;
        CreateCharSpecialAdvantageWindow createCharSpecialDisadvantageWindow;
        DaggerfallListPickerWindow helpPicker;
        DaggerfallListPickerWindow skillPicker;

        #endregion

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
            nameTextBox.Position = new Vector2(100, 5);
            nameTextBox.Size = new Vector2(214, 7);
            NativePanel.Components.Add(nameTextBox);

            // Initialize character class
            createdClass = new DFCareer();
            createdClass.HitPointsPerLevel = defaultHpPerLevel;
            createdClass.SpellPointMultiplier = DFCareer.SpellPointMultipliers.Times_0_50;
            createdClass.SpellPointMultiplierValue = .5f;

            // Initiate UI components
            font = DaggerfallUI.DefaultFont;
            SetupButtons();
            hpLabel = DaggerfallUI.AddTextLabel(font, new Vector2(285, 55), createdClass.HitPointsPerLevel.ToString(), NativePanel);
            daggerPanel.Size = new Vector2(24, 9);
            daggerPanel.BackgroundTexture = nativeDaggerTexture;
            NativePanel.Components.Add(daggerPanel);
            UpdateDifficulty();

            // Setup help dictionary
            helpDict = new Dictionary<string, int> 
            {
                { TextManager.Instance.GetLocalizedText("helpAttributes"), 2402 },
                { TextManager.Instance.GetLocalizedText("helpClassName"), 2401 },
                { TextManager.Instance.GetLocalizedText("helpGeneral"), 2400 },
                { TextManager.Instance.GetLocalizedText("helpReputations"), 2406 },
                { TextManager.Instance.GetLocalizedText("helpSkillAdvancement"), 2407 },
                { TextManager.Instance.GetLocalizedText("helpSkills"), 2403 },
                { TextManager.Instance.GetLocalizedText("helpSpecialAdvantages"), 2404 },
                { TextManager.Instance.GetLocalizedText("helpSpecialDisadvantages"), 2405 }
            };

            // Setup skills dictionary
            skillsDict = new Dictionary<string, DFCareer.Skills>();
            foreach (DFCareer.Skills skill in Enum.GetValues(typeof(DFCareer.Skills)))
            {
                string name = DaggerfallUnity.Instance.TextProvider.GetSkillName(skill);
                if(!string.IsNullOrEmpty(name))
                    skillsDict.Add(name, skill);
            }
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
                skillButtons[i].ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
                skillLabels[i] = DaggerfallUI.AddTextLabel(font, new Vector2(3, 2), string.Empty, skillButtons[i]);
            }
            // HP spinners
            hitPointsUpButton = DaggerfallUI.AddButton(hitPointsUpButtonRect, NativePanel);
            hitPointsUpButton.OnMouseClick += HitPointsUpButton_OnMouseClick;
            hitPointsUpButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            hitPointsDownButton = DaggerfallUI.AddButton(hitPointsDownButtonRect, NativePanel);
            hitPointsDownButton.OnMouseUp += HitPointsDownButton_OnMouseClick;
            hitPointsDownButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);

            // Help topics
            helpButton = DaggerfallUI.AddButton(helpButtonRect, NativePanel);
            helpButton.OnMouseClick += HelpButton_OnMouseClick;
            helpButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);

            // Special Advantages/Disadvantages
            specialAdvantageButton = DaggerfallUI.AddButton(specialAdvantageButtonRect, NativePanel);
            specialAdvantageButton.OnMouseClick += specialAdvantageButton_OnMouseClick;
            specialAdvantageButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            specialDisadvantageButton = DaggerfallUI.AddButton(specialDisadvantageButtonRect, NativePanel);
            specialDisadvantageButton.OnMouseClick += specialDisadvantageButton_OnMouseClick;
            specialDisadvantageButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);

            // Reputations
            reputationButton = DaggerfallUI.AddButton(reputationButtonRect, NativePanel);
            reputationButton.OnMouseClick += ReputationButton_OnMouseClick;
            reputationButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);

            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
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
            skillPicker = new DaggerfallListPickerWindow(uiManager, this);
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
            skillPicker.CloseWindow();
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
            if (createdClass.HitPointsPerLevel != maxHpPerLevel)
            {
                createdClass.HitPointsPerLevel++;
                hpLabel.Text = createdClass.HitPointsPerLevel.ToString();
                UpdateDifficulty();
            }
        }

        public void HitPointsDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            if (createdClass.HitPointsPerLevel != minHpPerLevel)
            {
                createdClass.HitPointsPerLevel--;
                hpLabel.Text = createdClass.HitPointsPerLevel.ToString();
                UpdateDifficulty();
            }
        }

        void HelpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            helpPicker = new DaggerfallListPickerWindow(uiManager, this);
            foreach (string str in helpDict.Keys)
            {
                helpPicker.ListBox.AddItem(str);
            }
            helpPicker.OnItemPicked += HelpPicker_OnItemPicked;
            uiManager.PushWindow(helpPicker);
        }

        void HelpPicker_OnItemPicked(int index, string itemString)
        {
            helpPicker.CloseWindow();
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.SetTextTokens(helpDict[itemString]);
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
        }

        public void specialAdvantageButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            createCharSpecialAdvantageWindow = new CreateCharSpecialAdvantageWindow(uiManager, advantages, disadvantages, createdClass, this);
            uiManager.PushWindow(createCharSpecialAdvantageWindow);
        }

        public void specialDisadvantageButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            createCharSpecialDisadvantageWindow = new CreateCharSpecialAdvantageWindow(uiManager, disadvantages, advantages, createdClass, this, true);
            uiManager.PushWindow(createCharSpecialDisadvantageWindow);
        }

        void ReputationButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            createCharReputationWindow = new CreateCharReputationWindow(uiManager, this);
            uiManager.PushWindow(createCharReputationWindow);
        }

        void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallMessageBox messageBox;

            // Is the class name set?
            if (nameTextBox.Text.Length == 0) 
            {
                messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(strNameYourClass);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
                return;
            } 

            // Are all skills set?
            for (int i = 0; i < skillLabels.Length; i++) 
            {
                if (skillLabels [i].Text == string.Empty)
                {
                    messageBox = new DaggerfallMessageBox(uiManager, this);
                    messageBox.SetTextTokens(strSetSkills);
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.Show();
                    return;
                }
            }

            // Are all attribute points distributed?
            if (statsRollout.BonusPool != 0) 
            {
                messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(strDistributeStats);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
                return;
            }

            // Is AdvancementMultiplier off limits?
            if (difficultyPoints < minDifficultyPoints || difficultyPoints > maxDifficultyPoints)
            {
                messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(strAdvancingDaggerInRed);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
                return;
            }

            // Set advantages/disadvantages
            if (createCharSpecialAdvantageWindow != null)
            {
                createCharSpecialAdvantageWindow.ParseCareerData();
            }
            if (createCharSpecialDisadvantageWindow != null)
            {
                createCharSpecialDisadvantageWindow.ParseCareerData();
            }

            CloseWindow();
        }

        #endregion

        #region Private methods

        private void UpdateDifficulty()
        {
            const int defaultDaggerX = 220;
            const int defaultDaggerY = 115;
            const int minDaggerY = 46;     // Visually clamp to gauge size
            const int maxDaggerY = 186;    // Visually clamp to gauge size

            // hp adjustment
            if (createdClass.HitPointsPerLevel >= defaultHpPerLevel)
            {
                difficultyPoints = createdClass.HitPointsPerLevel - defaultHpPerLevel; // +1 pt for each hp above default
            } 
            else
            {
                difficultyPoints = -(2 * (defaultHpPerLevel - createdClass.HitPointsPerLevel)); // -2 pts for each hp below default
            }

            // adjustments for special advantages/disadvantages
            difficultyPoints += advantageAdjust + disadvantageAdjust;

            // Set level advancement difficulty
            createdClass.AdvancementMultiplier = 0.3f + (2.7f * (float)(difficultyPoints + 12) / 52f);

            // Reposition the difficulty dagger
            int daggerY = 0;
            if (difficultyPoints >= 0)
            {
                daggerY = Math.Max(minDaggerY, (int)(defaultDaggerY - (37 * (difficultyPoints / 40f))));
            } 
            else
            {
                daggerY = Math.Min(maxDaggerY, (int)(defaultDaggerY + (41 * (-difficultyPoints / 12f))));
            }

            daggerPanel.Position = new Vector2(defaultDaggerX, daggerY);
            DaggerfallUI.Instance.StartCoroutine(AnimateDagger());
        }

        IEnumerator AnimateDagger()
        {
            Panel daggerTrailPanel = new Panel();
            daggerTrailPanel.Position = daggerPanel.Position;
            daggerTrailPanel.Size = daggerPanel.Size;
            daggerTrailPanel.BackgroundColorTexture = nativeDaggerTexture;
            daggerTrailPanel.BackgroundColor = new Color32(255, 255, 255, 255);
            NativePanel.Components.Add(daggerTrailPanel);
            float daggerTrailTime = daggerTrailLingerTime;

            while ((daggerTrailTime -= Time.unscaledDeltaTime) >= 0f)
            {
                daggerTrailPanel.BackgroundColor = new Color32(255, 255, 255, (byte)(255 * daggerTrailTime / daggerTrailLingerTime));
                yield return new WaitForEndOfFrame();
            }
            NativePanel.Components.Remove(daggerTrailPanel);
            daggerTrailPanel.Dispose();
        }

        #endregion

        #region Properties

        public int AdvantageAdjust
        {
            set { advantageAdjust = value; UpdateDifficulty(); }
        }

        public int DisadvantageAdjust
        {
            set { disadvantageAdjust = value; UpdateDifficulty(); }
        }

        public short MerchantsRep
        {
            get { return merchantsRep; }
            set { merchantsRep = value; }
        }

        public short PeasantsRep
        {
            get { return peasantsRep; }
            set { peasantsRep = value; }
        }

        public short ScholarsRep
        {
            get { return scholarsRep; }
            set { scholarsRep = value; }
        }

        public short NobilityRep
        {
            get { return nobilityRep; }
            set { nobilityRep = value; }
        }

        public short UnderworldRep
        {
            get { return underworldRep; }
            set { underworldRep = value; }
        }

        public DFCareer CreatedClass
        {
            get { return createdClass; }
        }

        public string ClassName
        {
            get { return nameTextBox.Text; }
        }

        public StatsRollout Stats
        {
            get { return statsRollout; }
        }

        #endregion
    }    
}