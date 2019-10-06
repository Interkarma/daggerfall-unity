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
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements popup window on escape during gameplay.
    /// </summary>
    public class DaggerfallPauseOptionsWindow : DaggerfallPopupWindow
    {
        #region Fields

        const string nativeImgName = "OPTN00I0.IMG";
        const int strAreYouSure = 1069;
        const float barMaxLength = 109.1f;

        Texture2D nativeTexture;
        Panel optionsPanel = new Panel();
        Panel fullScreenTick;
        Panel headBobbingTick;
        Panel musicBar;
        Panel soundBar;
        Panel detailBar;
        DaggerfallHUD hud;
        TextLabel versionTextLabel;

        readonly Color versionTextColor = new Color(0.75f, 0.75f, 0.75f, 1);
        readonly Color versionShadowColor = new Color(0.15f, 0.15f, 0.15f, 1);

        bool saveSettings = false;

        #endregion

        #region Constructors

        public DaggerfallPauseOptionsWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            : base(uiManager, previousWindow)
        {
        }

        #endregion

        #region Overrides

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
            optionsPanel.BackgroundColor = Color.black;
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

            // Detail level
            Button detailButton = DaggerfallUI.AddButton(new Rect(6.15f, 39f, barMaxLength, 5.5f), optionsPanel);
            detailButton.OnMouseClick += DetailButton_OnMouseClick;
            detailBar = DaggerfallUI.AddPanel(new Rect(0f, 1f, GetDetailBarWidth(QualitySettings.GetQualityLevel()), 3.5f), detailButton);
            detailBar.BackgroundColor = DaggerfallUI.DaggerfallUnityDefaultCheckboxToggleColor;

            // Controls
            Button controlsButton = DaggerfallUI.AddButton(new Rect(5, 60, 70, 17), optionsPanel);
            controlsButton.OnMouseClick += ControlsButton_OnMouseClick;

            // Full screen
            Button fullScreenButton = DaggerfallUI.AddButton(new Rect(5, 47, 70, 8), optionsPanel);
            fullScreenButton.OnMouseClick += FullScreenButton_OnMouseClick;
            fullScreenTick = DaggerfallUI.AddPanel(new Rect(64f, 3.2f, 3.7f, 3.2f), fullScreenButton);
            fullScreenTick.BackgroundColor = DaggerfallUI.DaggerfallUnityDefaultCheckboxToggleColor;
            fullScreenTick.Enabled = DaggerfallUnity.Settings.LargeHUD;

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
                hud.LargeHUD.Update();
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
                hud.LargeHUD.Draw();
                hud.ActiveSpells.Draw();
                hud.EscortingFaces.Draw();
                hud.HUDVitals.Draw();
            }
        }

        public override void OnPop()
        {
            base.OnPop();

            if (saveSettings)
                DaggerfallUnity.Settings.SaveSettings();
        }

        #endregion

        #region Private Helpers

        private static float GetDetailBarWidth(int value)
        {
            return Mathf.Lerp(0, barMaxLength, value / (float)(QualitySettings.names.Length - 1));
        }

        #endregion

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
            if (!saveSettings)
                saveSettings = true;
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
            if (!saveSettings)
                saveSettings = true;
        }

        private void DetailButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            int value = Mathf.RoundToInt(Mathf.Lerp(0, QualitySettings.names.Length - 1, position.x / sender.Size.x));
            detailBar.Size = new Vector2(GetDetailBarWidth(value), detailBar.Size.y);
            QualitySettings.SetQualityLevel(DaggerfallUnity.Settings.QualityLevel = value);
            GameManager.UpdateShadowDistance();
            GameManager.UpdateShadowResolution();
            if (!saveSettings)
                saveSettings = true;
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
                if (saveSettings)
                    DaggerfallUnity.Settings.SaveSettings();

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

        private void FullScreenButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Fullscreen button toggles large HUD setting
            fullScreenTick.Enabled = DaggerfallUnity.Settings.LargeHUD = !DaggerfallUnity.Settings.LargeHUD;

            if (!saveSettings)
                saveSettings = true;
        }

        private void HeadBobbingButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            //Debug.Log("Head Bobbing clicked, position: x: " + position.x + ", y: " + position.y);
            DaggerfallUnity.Settings.HeadBobbing = !DaggerfallUnity.Settings.HeadBobbing;
            headBobbingTick.Enabled = DaggerfallUnity.Settings.HeadBobbing;
            if (!saveSettings)
                saveSettings = true;
        }

        #endregion
    }
}