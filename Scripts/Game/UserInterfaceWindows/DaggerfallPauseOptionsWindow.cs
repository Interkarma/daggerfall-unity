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
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements popup window on escape during gameplay.
    /// </summary>
    public class DaggerfallPauseOptionsWindow : DaggerfallPopupWindow
    {
        const string nativeImgName = "OPTN00I0.IMG";
        const int strAreYouSure = 1069;

        Texture2D nativeTexture;
        Panel optionsPanel = new Panel();

        public DaggerfallPauseOptionsWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            :base(uiManager, previousWindow)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallOptionsWindow: Could not load native texture.");

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Native options panel
            optionsPanel.HorizontalAlignment = HorizontalAlignment.Center;
            optionsPanel.Position = new Vector2(0, 40);
            optionsPanel.Size = new Vector2(nativeTexture.width, nativeTexture.height);
            optionsPanel.BackgroundTexture = nativeTexture;
            NativePanel.Components.Add(optionsPanel);

            // Exit game
            Button exitButton = DaggerfallUI.AddButton(new Rect(101, 4, 45, 16), optionsPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            // Continue
            Button continueButton = DaggerfallUI.AddButton(new Rect(76, 60, 70, 17), optionsPanel);
            continueButton.OnMouseClick += ContinueButton_OnMouseClick;

            // Save game
            Button saveButton = DaggerfallUI.AddButton(new Rect(4, 4, 45, 16), optionsPanel);
            saveButton.OnMouseClick += SaveButton_OnMouseClick;

            // Load game
            Button loadButton = DaggerfallUI.AddButton(new Rect(52, 4, 46, 16), optionsPanel);
            loadButton.OnMouseClick += LoadButton_OnMouseClick;

            // Controls
            Button controlsButton = DaggerfallUI.AddButton(new Rect(5, 60, 70, 17), optionsPanel);
            controlsButton.BackgroundColor = new Color(1, 0, 0, 0.5f);

            // Full screen
            Button fullScreenButton = DaggerfallUI.AddButton(new Rect(5, 47, 70, 8), optionsPanel);
            fullScreenButton.BackgroundColor = new Color(1, 0, 0, 0.5f);

            // Head bobbing
            Button headBobbingButton = DaggerfallUI.AddButton(new Rect(76, 47, 70, 8), optionsPanel);
            headBobbingButton.BackgroundColor = new Color(1, 0, 0, 0.5f);
        }

        #region Event Handlers

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallMessageBox confirmExitBox = new DaggerfallMessageBox(uiManager, DaggerfallMessageBox.CommonMessageBoxButtons.YesNo, strAreYouSure, this);
            confirmExitBox.OnButtonClick += ConfirmExitBox_OnButtonClick;
            confirmExitBox.Show();
        }

        private void ConfirmExitBox_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiExitGame);
                CancelWindow();
            }
        }

        private void ContinueButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CancelWindow();
        }

        private void SaveButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUnitySaveGameWindow saveWindow = new DaggerfallUnitySaveGameWindow(uiManager, this);
            uiManager.PushWindow(saveWindow);
        }

        private void LoadButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
        }

        #endregion
    }
}