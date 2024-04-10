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
    /// Implements final summary window at end of character creation.
    /// </summary>
    public class CreateCharSummary : DaggerfallPopupWindow
    {
        const string nativeImgName = "CHAR04I0.IMG";
        const int strYouMustDistributeYourBonusPoints = 14;

        Texture2D nativeTexture;
        TextBox textBox = new TextBox();
        StatsRollout statsRollout = new StatsRollout();
        SkillsRollout skillsRollout = new SkillsRollout();
        ReflexPicker reflexPicker = new ReflexPicker();
        FacePicker facePicker = new FacePicker();
        CharacterDocument characterDocument;

        IMECompositionMode prevIME;

        public CharacterDocument CharacterDocument
        {
            get { return characterDocument; }
            set { SetCharacterSheet(value); }
        }

        public DaggerfallSkills StartingSkills => skillsRollout.StartingSkills;
        public DaggerfallSkills WorkingSkills => skillsRollout.WorkingSkills;
        public DaggerfallStats StartingStats => statsRollout.StartingStats;
        public DaggerfallStats WorkingStats => statsRollout.WorkingStats;
        public Tuple<int, int, int> BonusSkillPoints => new Tuple<int, int, int>(skillsRollout.PrimarySkillBonusPoints, skillsRollout.MajorSkillBonusPoints, skillsRollout.MinorSkillBonusPoints);
        public int FaceIndex => facePicker.FaceIndex;

        public CreateCharSummary(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharSummary: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Add stats rollout
            NativePanel.Components.Add(statsRollout);

            // Add skills rollout
            NativePanel.Components.Add(skillsRollout);

            // Add reflex picker
            reflexPicker.Position = new Vector2(246, 95);
            NativePanel.Components.Add(reflexPicker);

            // Add face picker
            NativePanel.Components.Add(facePicker);

            // Add name editor
            textBox.Position = new Vector2(100, 5);
            textBox.Size = new Vector2(214, 7);
            NativePanel.Components.Add(textBox);

            // Add "Restart" button
            Button restartButton = DaggerfallUI.AddButton(new Rect(263, 147, 39, 22), NativePanel);
            restartButton.OnMouseClick += RestartButton_OnMouseClick;

            // Add "OK" button
            Button okButton = DaggerfallUI.AddButton(new Rect(263, 172, 39, 22), NativePanel);
            okButton.OnMouseClick += OkButton_OnMouseClick;
        }

        public override void OnPush()
        {
            base.OnPush();

            // Enable IME composition during input
            prevIME = Input.imeCompositionMode;
            Input.imeCompositionMode = IMECompositionMode.On;
        }

        public override void OnPop()
        {
            base.OnPop();

            // Restore previous IME composition mode
            Input.imeCompositionMode = prevIME;
        }

        #region Private Methods

        void SetCharacterSheet(CharacterDocument characterDocument)
        {
            this.characterDocument = characterDocument;
            this.textBox.Text = characterDocument.name;
            this.statsRollout.StartingStats = characterDocument.startingStats;
            this.statsRollout.WorkingStats = characterDocument.workingStats;
            this.statsRollout.BonusPool = 0;
            this.skillsRollout.SetClassSkills(characterDocument.career);
            this.skillsRollout.StartingSkills = characterDocument.startingSkills;
            this.skillsRollout.WorkingSkills = characterDocument.workingSkills;
            this.skillsRollout.SkillBonuses = BiogFile.GetSkillEffects(characterDocument.biographyEffects);
            this.skillsRollout.PrimarySkillBonusPoints = 0;
            this.skillsRollout.MajorSkillBonusPoints = 0;
            this.skillsRollout.MinorSkillBonusPoints = 0;
            this.facePicker.FaceIndex = characterDocument.faceIndex;
            this.facePicker.SetFaceTextures(characterDocument.raceTemplate, characterDocument.gender);
            this.reflexPicker.PlayerReflexes = characterDocument.reflexes;
        }

        public CharacterDocument GetUpdatedCharacterDocument()
        {
            characterDocument.name = textBox.Text;
            characterDocument.startingStats = statsRollout.StartingStats;
            characterDocument.workingStats = statsRollout.WorkingStats;
            characterDocument.startingSkills = skillsRollout.StartingSkills;
            characterDocument.workingSkills = skillsRollout.WorkingSkills;
            characterDocument.faceIndex = facePicker.FaceIndex;
            characterDocument.reflexes = reflexPicker.PlayerReflexes;
            return characterDocument;
        }

        #endregion

        #region Events

        public delegate void OnRestartHandler();
        public event OnRestartHandler OnRestart;
        void RaiseOnRestartEvent()
        {
            if (OnRestart != null)
                OnRestart();
        }

        #endregion

        #region Event Handlers

        void RestartButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            PopWindow();
            RaiseOnRestartEvent();
        }

        void OkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (statsRollout.BonusPool > 0 || 
                skillsRollout.PrimarySkillBonusPoints > 0 ||
                skillsRollout.MajorSkillBonusPoints > 0 ||
                skillsRollout.MinorSkillBonusPoints > 0)
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