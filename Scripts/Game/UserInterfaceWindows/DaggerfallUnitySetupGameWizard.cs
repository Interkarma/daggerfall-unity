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
using DaggerfallConnect.Utility;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallUnitySetupGameWizard : DaggerfallPopupWindow
    {
        #region UI Rects

        Vector2 welcomePanelSize = new Vector2(280, 170);

        #endregion

        #region UI Controls

        Panel previousPanel = null;
        Panel currentPanel = null;
        Panel nextPanel = null;
        Panel welcomePanel = new Panel();
        FolderBrowser browser = new FolderBrowser();
        TextLabel helpLabel = new TextLabel();

        #endregion

        #region UI Textures

        Texture2D titleTexture;

        #endregion

        #region Fields

        const string titleScreenFilename = "StartupBackground2";
        const string exitButtonText = "Exit";
        const float panelSwipeTime = 1;
        const SongFiles titleSongFile = SongFiles.song_5strong;

        const string findArena2Tip = "Tip: Daggerfall contains a folder called 'arena2'";
        const string foundArena2But = "Found 'arena2' but ";
        const string missingTextures = "it's missing one or more TEXTURE files";
        const string missingModels = "it's missing ARCH3D.BSA";
        const string missingBlocks = "it's missing BLOCKS.BSA";
        const string missingMaps = "it's missing MAPS.BSA";
        const string missingSounds = "it's missing DAGGER.SND";
        const string missingWoods = "it's missing WOODS.WLD";
        const string missingVideos = "it's missing one or more .VID files";
        const string justNotValid = "it does not appear to be valid";
        const string pathValidated = "Great! This looks like a valid Daggerfall folder :)";

        Color backgroundColor = new Color(0, 0, 0, 0.7f);
        Color confirmEnabledBackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        Color confirmDisabledBackgroundColor = new Color(0.5f, 0.0f, 0.0f, 0.4f);
        WizardPanels currentWizardPanel = WizardPanels.Nothing;

        #endregion

        #region Enums

        enum WizardPanels
        {
            Nothing,
            Welcome,
        }

        #endregion

        #region Properties
        #endregion

        #region Constructors

        public DaggerfallUnitySetupGameWizard(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            LoadResources();

            // Setup panels
            SetupWelcomePanel();

            // Add exit button
            Button exitButton = new Button();
            exitButton.Size = new Vector2(20, 9);
            exitButton.HorizontalAlignment = HorizontalAlignment.Center;
            exitButton.VerticalAlignment = VerticalAlignment.Bottom;
            exitButton.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
            exitButton.Outline.Enabled = true;
            exitButton.Label.Text = exitButtonText;
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            //exitButton.Label.TextColor = Color.red;
            //exitButton.Label.ShadowPosition = Vector2.zero;
            NativePanel.Components.Add(exitButton);
        }

        public override void Update()
        {
            base.Update();

            // Loop title song
            if (!DaggerfallUI.Instance.DaggerfallSongPlayer.IsPlaying)
            {
                //DaggerfallUI.Instance.DaggerfallSongPlayer.Play(titleSongFile);
            }
        }

        void SetupWelcomePanel()
        {
            // Setup panel
            welcomePanel.BackgroundColor = backgroundColor;
            welcomePanel.HorizontalAlignment = HorizontalAlignment.Center;
            welcomePanel.VerticalAlignment = VerticalAlignment.Middle;
            welcomePanel.Size = welcomePanelSize;
            welcomePanel.Outline.Enabled = true;
            NativePanel.Components.Add(welcomePanel);

            // Setup screen text
            MultiFormatTextLabel screen = new MultiFormatTextLabel();
            screen.HorizontalAlignment = HorizontalAlignment.Center;
            screen.Position = new Vector2(0, 10);
            screen.TextAlignment = HorizontalAlignment.Center;
            screen.SetText(Resources.Load<TextAsset>("Screens/WelcomeScreen"));
            welcomePanel.Components.Add(screen);

            // Setup folder browser
            browser.Position = new Vector2(4, 30);
            browser.Size = new Vector2(250, 104);
            browser.HorizontalAlignment = HorizontalAlignment.Center;
            browser.ConfirmEnabled = false;
            browser.OnConfirmPath += Browser_OnConfirmPath;
            browser.OnPathChanged += Browser_OnPathChanged;
            welcomePanel.Components.Add(browser);

            // Add version number
            TextLabel versionLabel = new TextLabel();
            versionLabel.Position = new Vector2(0, 1);
            versionLabel.HorizontalAlignment = HorizontalAlignment.Right;
            versionLabel.ShadowPosition = Vector2.zero;
            versionLabel.TextColor = Color.gray;
            versionLabel.Text = VersionInfo.DaggerfallUnityVersion;
            welcomePanel.Components.Add(versionLabel);

            // Add help text
            helpLabel.Position = new Vector2(0, 145);
            helpLabel.HorizontalAlignment = HorizontalAlignment.Center;
            helpLabel.ShadowPosition = Vector2.zero;
            helpLabel.Text = findArena2Tip;
            welcomePanel.Components.Add(helpLabel);
        }

        #endregion

        #region Private Methods

        void LoadResources()
        {
            // Load title background texture
            titleTexture = Resources.Load<Texture2D>(titleScreenFilename);
            if (titleTexture != null)
            {
                titleTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
                ParentPanel.BackgroundTexture = titleTexture;
                ParentPanel.BackgroundTextureLayout = BackgroundLayout.ScaleToFit;
            }
        }

        void ShowNextPanel()
        {
            previousPanel = currentPanel;
            switch(currentWizardPanel)
            {
                case WizardPanels.Nothing:
                    currentWizardPanel = WizardPanels.Welcome;
                    nextPanel = welcomePanel;
                    break;
            }
        }

        string GetInvalidPathHelpText(DFValidator.ValidationResults validationResults)
        {
            if (!validationResults.TexturesValid)
                return foundArena2But + missingTextures;
            else if (!validationResults.ModelsValid)
                return foundArena2But + missingModels;
            else if (!validationResults.BlocksValid)
                return foundArena2But + missingBlocks;
            else if (!validationResults.MapsValid)
                return foundArena2But + missingMaps;
            else if (!validationResults.SoundsValid)
                return foundArena2But + missingSounds;
            else if (!validationResults.WoodsValid)
                return foundArena2But + missingWoods;
            else if (!validationResults.VideosValid)
                return foundArena2But + missingVideos;
            else
                return justNotValid;

        }

        #endregion

        #region Event Handlers

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Application.Quit();
        }

        private void Browser_OnPathChanged()
        {
            // Test arena2 exists inside path
            string pathResult = DaggerfallUnity.TestArena2Exists(browser.CurrentPath);
            if (string.IsNullOrEmpty(pathResult))
            {
                helpLabel.Text = findArena2Tip;
                browser.ConfirmEnabled = false;
                browser.BackgroundColor = Color.clear;
                return;
            }

            // Validate this path
            DFValidator.ValidationResults validationResults;
            DFValidator.ValidateArena2Folder(pathResult, out validationResults, true);
            if (!validationResults.AppearsValid)
            {
                helpLabel.Text = GetInvalidPathHelpText(validationResults);
                browser.ConfirmEnabled = false;
                browser.BackgroundColor = confirmDisabledBackgroundColor;
                return;
            }

            // Path is valid
            browser.ConfirmEnabled = true;
            browser.BackgroundColor = confirmEnabledBackgroundColor;
            helpLabel.Text = pathValidated;
        }

        private void Browser_OnConfirmPath()
        {
        }

        #endregion
    }
}