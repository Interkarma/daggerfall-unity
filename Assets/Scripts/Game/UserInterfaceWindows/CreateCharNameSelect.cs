// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements the enter name window.
    /// </summary>
    public class CreateCharNameSelect : DaggerfallPopupWindow
    {
        const string nativeImgName = "CHAR00I0.IMG";

        NameHelper nameHelper = new NameHelper();
        RaceTemplate raceTemplate;
        Genders gender;

        Texture2D nativeTexture;
        TextBox textBox = new TextBox();
        Button randomNameButton = new Button();
        Button okButton = new Button();

        public RaceTemplate RaceTemplate
        {
            get { return raceTemplate; }
            set { SetRaceTemplate(value); }
        }

        public Genders Gender
        {
            get { return gender; }
            set { SetGender(value); }
        }

        public string CharacterName
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        public CreateCharNameSelect(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharNameSelect: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Text edit box
            textBox.Position = new Vector2(80, 5);
            textBox.Size = new Vector2(214, 7);
            NativePanel.Components.Add(textBox);

            // Random name button
            randomNameButton = DaggerfallUI.AddButton(new Rect(279, 3, 36, 10), NativePanel);
            randomNameButton.Label.Text = "Random";
            randomNameButton.Label.ShadowColor = Color.black;
            randomNameButton.BackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);
            randomNameButton.OnMouseClick += RandomNameButton_OnMouseClick;

            // OK button
            okButton = DaggerfallUI.AddButton(new Rect(263, 172, 39, 22), NativePanel);
            okButton.OnMouseClick += OkButton_OnMouseClick;

            // First display of random button
            ShowRandomButton();
        }

        public override void OnPush()
        {
            // Subsequent display of random button
            ShowRandomButton();

            base.OnPush();
        }

        void ShowRandomButton()
        {
            // Must have a race template set
            if (raceTemplate == null)
            {
                randomNameButton.Enabled = false;
                return;
            }

            // Disable random name button only for Argonians because their race id
            // would give them Imperial names from namegen
            randomNameButton.Enabled = raceTemplate.ID != (int) Races.Argonian;

            // Randomise DFRandom seed from System.Random
            // A bit of a hack but better than starting with a seed of 0 every time
            System.Random random = new System.Random();
            DFRandom.Seed = (uint)random.Next();
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Return))
                AcceptName();
        }

        void AcceptName()
        {
            if (textBox.Text.Length > 0)
                CloseWindow();
        }

        void SetRaceTemplate(RaceTemplate raceTemplate)
        {
            if (this.raceTemplate != null)
            {
                // Empty name textbox if race ID changed
                if (this.raceTemplate.ID != raceTemplate.ID)
                    textBox.Text = string.Empty;
            }

            this.raceTemplate = raceTemplate;
        }

        void SetGender(Genders gender)
        {
            // Empty name textbox if gender changed
            if (this.gender != gender)
                textBox.Text = string.Empty;

            this.gender = gender;
        }

        #region Event Handlers

        private void RandomNameButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Generate name based on race
            NameHelper.BankTypes bankType = MacroHelper.GetNameBank((Races)raceTemplate.ID);
            textBox.Text = nameHelper.FullName(bankType, gender);
        }

        void OkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            AcceptName();
        }

        #endregion
    }
}