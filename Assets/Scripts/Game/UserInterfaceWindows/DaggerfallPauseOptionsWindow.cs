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
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility.AssetInjection;

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
        Panel headBobbingTick = new Panel();
        Panel musicBar = new Panel();
        Panel soundBar = new Panel();
        const float barMaxLength = 109.1f;
        DaggerfallHUD hud;

        TextLabel versionTextLabel;
        Color versionTextColor = new Color(0.75f, 0.75f, 0.75f, 1);
        Color versionShadowColor = new Color(0.15f, 0.15f, 0.15f, 1);

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
            optionsPanel.Size = TextureReplacement.GetSize(nativeTexture, nativeImgName);
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
            //saveButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;
            saveButton.OnMouseClick += SaveButton_OnMouseClick;

            // Load game
            Button loadButton = DaggerfallUI.AddButton(new Rect(52, 4, 46, 16), optionsPanel);
            //loadButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;
            loadButton.OnMouseClick += LoadButton_OnMouseClick;

            // Sound Bar
            Button soundPanel = DaggerfallUI.AddButton(new Rect(6.15f, 23.20f, barMaxLength, 5.5f), optionsPanel);
            soundPanel.OnMouseClick += SoundBar_OnMouseClick;
            soundBar = DaggerfallUI.AddPanel(new Rect(0f, 1f, DaggerfallUnity.Settings.SoundVolume * barMaxLength, 3.5f), soundPanel);
            soundBar.BackgroundColor = DaggerfallUI.DaggerfallUnityDefaultCheckboxToggleColor;

            // Music Bar
            Button musicPanel = DaggerfallUI.AddButton(new Rect(6.15f, 30.85f, barMaxLength, 5.5f), optionsPanel);
            musicPanel.OnMouseClick += MusicBar_OnMouseClick;
            musicBar = DaggerfallUI.AddPanel(new Rect(0f, 1f, DaggerfallUnity.Settings.MusicVolume * barMaxLength, 3.5f), musicPanel);
            musicBar.BackgroundColor = DaggerfallUI.DaggerfallUnityDefaultCheckboxToggleColor;

            // Controls
            Button controlsButton = DaggerfallUI.AddButton(new Rect(5, 60, 70, 17), optionsPanel);
            controlsButton.OnMouseClick += ControlsButton_OnMouseClick;

            // Full screen
            //Button fullScreenButton = DaggerfallUI.AddButton(new Rect(5, 47, 70, 8), optionsPanel);
            //fullScreenButton.BackgroundColor = new Color(1, 0, 0, 0.5f);

            // Head bobbing
            Button headBobbingButton = DaggerfallUI.AddButton(new Rect(76, 47, 70, 8), optionsPanel);
            headBobbingButton.OnMouseClick += HeadBobbingButton_OnMouseClick;
            headBobbingTick = DaggerfallUI.AddPanel(new Rect(64f, 3.2f, 3.7f, 3.2f), headBobbingButton);
            headBobbingTick.BackgroundColor = DaggerfallUI.DaggerfallUnityDefaultCheckboxToggleColor;
            headBobbingTick.Enabled = DaggerfallUnity.Settings.HeadBobbing;

            // Set version text
            versionTextLabel = new TextLabel();
            versionTextLabel.Text = string.Format("{0} {1} {2}", VersionInfo.DaggerfallUnityProductName, VersionInfo.DaggerfallUnityStatus, VersionInfo.DaggerfallUnityVersion);
            versionTextLabel.TextColor = versionTextColor;
            versionTextLabel.ShadowColor = versionShadowColor;
            versionTextLabel.ShadowPosition = Vector2.one;
            versionTextLabel.HorizontalAlignment = HorizontalAlignment.Right;
            ParentPanel.Components.Add(versionTextLabel);
        }

        public override void OnPush()
        {
            base.OnPush();

            hud = DaggerfallUI.Instance.DaggerfallHUD;
        }

        public override void Update()
        {
            base.Update();

            // Update pause-persistent HUD elements
            if (hud != null)
            {
                hud.ActiveSpells.Update();
                hud.EscortingFaces.Update();
                hud.HUDVitals.Update();
            }

            // Scale version text based on native panel scaling
            versionTextLabel.TextScale = NativePanel.LocalScale.x * 0.75f;
        }

        public override void Draw()
        {
            base.Draw();

            // Draw pause-persistent HUD elements
            if (hud != null)
            {
                hud.ActiveSpells.Draw();
                hud.EscortingFaces.Draw();
                hud.HUDVitals.Draw();
            }
        }

        #region Event Handlers

        private void SoundBar_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // make it easier to max out or mute volume
            if ((position.x / barMaxLength) > 0.99f)
                position.x = barMaxLength;
            else if ((position.x / barMaxLength) < 0.01f)
                position.x = 0;
            // resize panel to where user clicked
            soundBar.Size = new Vector2(position.x, 3.5f);
            DaggerfallUnity.Settings.SoundVolume = (position.x / barMaxLength);
            DaggerfallUnity.Settings.SaveSettings();
        }
        private void MusicBar_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // make it easier to max out or mute volume
            if ((position.x / barMaxLength) > 0.99f)
                position.x = barMaxLength;
            else if ((position.x / barMaxLength) < 0.01f)
                position.x = 0;
            // resize panel to where user clicked
            musicBar.Size = new Vector2(position.x, 3.5f);
            DaggerfallUnity.Settings.MusicVolume = (position.x / barMaxLength);
            DaggerfallUnity.Settings.SaveSettings();
        }

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
            //// TEMP: Quick save
            //SaveLoadManager.Instance.QuickSave();
            //CancelWindow();

            DaggerfallUnitySaveGameWindow saveWindow = new DaggerfallUnitySaveGameWindow(uiManager, DaggerfallUnitySaveGameWindow.Modes.SaveGame, this);
            uiManager.PushWindow(saveWindow);
        }

        private void LoadButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // TEMP: Show load window
            //uiManager.PushWindow(new DaggerfallLoadSavedGameWindow(uiManager));

            DaggerfallUnitySaveGameWindow loadWindow = new DaggerfallUnitySaveGameWindow(uiManager, DaggerfallUnitySaveGameWindow.Modes.LoadGame, this);
            uiManager.PushWindow(loadWindow);
        }

        private void ControlsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenControlsWindow);
        }

        private void HeadBobbingButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            //Debug.Log("Head Bobbing clicked, position: x: " + position.x + ", y: " + position.y);
            DaggerfallUnity.Settings.HeadBobbing = !DaggerfallUnity.Settings.HeadBobbing;
            headBobbingTick.Enabled = DaggerfallUnity.Settings.HeadBobbing;
            DaggerfallUnity.Settings.SaveSettings();
        }
        #endregion
    }
}