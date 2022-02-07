// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2021 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    John Doom
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
        TextLabel customLabel;
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
            promptLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 16), TextManager.Instance.GetLocalizedText("pleaseSelectYourHomeProvince"), NativePanel);
            promptLabel.HorizontalAlignment = HorizontalAlignment.Center;
            promptLabel.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            promptLabel.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
            promptLabel.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;

            // Add custom races prompt
            if (RaceTemplate.CustomRaces.Count > 0)
            {
                customLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 24), "Press R for more races.", NativePanel);
                customLabel.HorizontalAlignment = promptLabel.HorizontalAlignment;
                customLabel.TextColor = promptLabel.TextColor;
                customLabel.ShadowColor = promptLabel.ShadowColor;
                customLabel.ShadowPosition = promptLabel.ShadowPosition;
            }

            // Handle clicks
            NativePanel.OnMouseClick += ClickHandler;
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(exitKey))
            {
                selectedRace = null;
                CancelWindow();
            }

            //test
            if (Input.GetKeyDown(KeyCode.R))
                ShowList();
        }

        public void Reset()
        {
            selectedRace = null;
            if (promptLabel != null)
                promptLabel.Enabled = true;
            if (customLabel != null)
                customLabel.Enabled = true;
        }

        void ClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            int offset = (int)position.y * racePickerBitmap.Width + (int)position.x;
            if (offset < 0 || offset >= racePickerBitmap.Data.Length)
                return;

            int id = racePickerBitmap.Data[offset];
            if (raceDict.ContainsKey(id))
                ShowRace(id);
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

        void ShowRace(int id)
        {
            promptLabel.Enabled = false;
            if (customLabel != null) customLabel.Enabled = false;
            selectedRace = raceDict[id];

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);

            // Get description
            if (selectedRace.DescriptionString != null)
            {
                // If string present, use it
                messageBox.SetText(selectedRace.DescriptionString);
            }
            else
            {
                // Else use from id
                TextFile.Token[] textTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(selectedRace.DescriptionID);
                messageBox.SetTextTokens(textTokens);
            }

            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            Button noButton = messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
            noButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            messageBox.OnButtonClick += ConfirmRacePopup_OnButtonClick;
            messageBox.OnCancel += ConfirmRacePopup_OnCancel;
            uiManager.PushWindow(messageBox);

            DaggerfallAudioSource source = DaggerfallUI.Instance.GetComponent<DaggerfallAudioSource>();
            source.PlayOneShot((uint)selectedRace.ClipID);
        }

        void ShowList()
        {
            DaggerfallListPickerWindow window = new DaggerfallListPickerWindow(uiManager, this);
            foreach(RaceTemplate race in RaceTemplate.CustomRaces)
                window.ListBox.AddItem(race.Name);
            window.OnItemPicked += OnListPicked;
            uiManager.PushWindow(window);
        }

        void OnListPicked(int index, string itemString)
        {
            foreach (RaceTemplate race in RaceTemplate.CustomRaces)
            {
                if (race.Name.Contains(itemString))
                {
                    ShowRace(race.ID);
                    break;
                }
            }
        }
    }
}
