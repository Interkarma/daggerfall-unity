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
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements race select window.
    /// </summary>
    public class CreateCharRaceSelect : DaggerfallPopupWindow
    {
        const string nativeImgName = "TMAP00I0.IMG";
        const string racePickerImgName = "TAMRIEL2.IMG";

        Texture2D nativeTexture;
        TextLabel promptLabel;
        DFBitmap racePickerBitmap;
        RaceTemplate selectedRace;

        Dictionary<int, RaceTemplate> raceDict = RaceTemplate.GetRaceDictionary();

        public RaceTemplate SelectedRace
        {
            get { return selectedRace; }
        }

        public CreateCharRaceSelect(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharRaceSelect: Could not load native texture.");

            // Load picker colours
            racePickerBitmap = DaggerfallUI.GetImgBitmap(racePickerImgName);

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Add "Please select your home province..." prompt
            promptLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 16), HardStrings.pleaseSelectYourHomeProvince, NativePanel);
            promptLabel.HorizontalAlignment = HorizontalAlignment.Center;
            promptLabel.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            promptLabel.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
            promptLabel.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;

            // Handle clicks
            NativePanel.OnMouseClick += ClickHandler;
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(exitKey))
            {
                selectedRace = null;
                CloseWindow();
            }
        }

        public void Reset()
        {
            selectedRace = null;
            if (promptLabel != null)
                promptLabel.Enabled = true;
        }

        void ClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            int offset = (int)position.y * racePickerBitmap.Width + (int)position.x;
            if (offset < 0 || offset >= racePickerBitmap.Data.Length)
                return;

            int id = racePickerBitmap.Data[offset];
            if (raceDict.ContainsKey(id))
            {
                promptLabel.Enabled = false;
                selectedRace = raceDict[id];

                TextFile.Token[] textTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(selectedRace.DescriptionID);
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(textTokens);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                Button noButton = messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                noButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
                messageBox.OnButtonClick += ConfirmRacePopup_OnButtonClick;
                messageBox.OnCancel += ConfirmRacePopup_OnCancel;
                uiManager.PushWindow(messageBox);

                DaggerfallAudioSource source = DaggerfallUI.Instance.GetComponent<DaggerfallAudioSource>();
                source.PlayOneShot((uint)selectedRace.ClipID);
            }
        }

        void ConfirmRacePopup_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                CloseWindow();
            }
            else if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.No)
            {
                sender.CancelWindow();
            }
        }

        void ConfirmRacePopup_OnCancel(DaggerfallPopupWindow sender)
        {
            Reset();
        }
    }
}
