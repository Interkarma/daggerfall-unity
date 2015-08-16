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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements race select window.
    /// </summary>
    public class DaggerfallRaceSelectWindow : DaggerfallBaseWindow
    {
        const string nativeImgName = "TMAP00I0.IMG";
        const string racePickerImgName = "TAMRIEL2.IMG";

        Texture2D nativeTexture;
        TextLabel promptLabel;
        DFBitmap racePickerBitmap;

        public DaggerfallRaceSelectWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("RaceSelectWindow: Could not load native texture.");

            // Load picker colours
            racePickerBitmap = GetImgBitmap(racePickerImgName);

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Add "Please select your home province..." prompt
            DaggerfallUI ui = DaggerfallUI.Instance;
            promptLabel = AddTextLabel(ui.DefaultFont, new Vector2(0, 16), HardStrings.pleaseSelectYourHomeProvince);
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
                uiManager.PostMessage(WindowMessages.wmCloseWindow);
        }

        void ClickHandler(Vector2 position)
        {
            int offset = (int)position.y * racePickerBitmap.Width + (int)position.x;
            if (offset < 0 || offset >= racePickerBitmap.Data.Length)
                return;

            Races selectedRace = (Races)racePickerBitmap.Data[offset];

            string selectedText;
            switch (selectedRace)
            {
                default:
                case Races.None:
                    selectedText = HardStrings.pleaseSelectYourHomeProvince;
                    break;
                case Races.Breton:
                    selectedText = "High Rock (Breton)";
                    break;
                case Races.Redguard:
                    selectedText = "Hammerfell (Redguard)";
                    break;
                case Races.Nord:
                    selectedText = "Skyrim (Nord)";
                    break;
                case Races.DarkElf:
                    selectedText = "Morrowind (Dark Elf)";
                    break;
                case Races.HighElf:
                    selectedText = "Sumurset Isle (High Elf)";
                    break;
                case Races.WoodElf:
                    selectedText = "Valenwood (Wood Elf)";
                    break;
                case Races.Khajiit:
                    selectedText = "Elsweyr (Khajiit)";
                    break;
                case Races.Argonian:
                    selectedText = "Black Marsh (Argonian)";
                    break;
            }
            promptLabel.Text = selectedText;

            if (selectedRace > 0)
            {
                promptLabel.Enabled = false;
                DaggerfallPopupWindow popup = new DaggerfallPopupWindow(uiManager, this);
                popup.OnClose += ConfirmRacePopup_OnClose;
                uiManager.PushWindow(popup);
            }
        }

        void ConfirmRacePopup_OnClose()
        {
            promptLabel.Enabled = true;
        }
    }
}