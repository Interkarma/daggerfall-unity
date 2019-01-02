// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Entity;

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

            // Check if race supported for random name button
            // Argonian and Khajiit not supported at this time (no generation rules found in NAMEGEN)
            switch (raceTemplate.ID)
            {
                case (int)Races.Breton:
                case (int)Races.Redguard:
                case (int)Races.Nord:
                case (int)Races.DarkElf:
                case (int)Races.HighElf:
                case (int)Races.WoodElf:
                    randomNameButton.Enabled = true;
                    break;

                default:
                    randomNameButton.Enabled = false;
                    break;
            }

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
            NameHelper.BankTypes bankType;
            switch (raceTemplate.ID)
            {
                case (int)Races.Breton:
                    bankType = NameHelper.BankTypes.Breton;
                    break;
                case (int)Races.Redguard:
                    bankType = NameHelper.BankTypes.Redguard;
                    break;
                case (int)Races.Nord:
                    bankType = NameHelper.BankTypes.Nord;
                    break;
                case (int)Races.DarkElf:
                    bankType = NameHelper.BankTypes.DarkElf;
                    break;
                case (int)Races.HighElf:
                    bankType = NameHelper.BankTypes.HighElf;
                    break;
                case (int)Races.WoodElf:
                    bankType = NameHelper.BankTypes.WoodElf;
                    break;

                default:
                    return;
            }

            textBox.Text = nameHelper.FullName(bankType, gender);
        }

        void OkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            AcceptName();
        }

        #endregion
    }
}