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
using UnityEngine.SceneManagement;
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
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Game setup UI.
    /// Helps user set Daggerfall path and other basic options.
    /// This is very rough and could do some cleanup.
    /// </summary>
    public class DaggerfallUnitySetupGameWizard : DaggerfallPopupWindow
    {
        #region UI Rects

        Vector2 browserPanelSize = new Vector2(280, 170);

        #endregion

        #region UI Controls

        Panel browserPanel = new Panel();
        Panel resolutionPanel = new Panel();
        Panel optionsPanel = new Panel();
        //Panel summaryPanel = new Panel();
        VerticalScrollBar resolutionScroller = new VerticalScrollBar();
        FolderBrowser browser = new FolderBrowser();
        TextLabel helpLabel = new TextLabel();
        Checkbox fullscreenCheckbox = new Checkbox();
        Button testOrConfirmButton = new Button();
        ListBox resolutionList = new ListBox();
        ListBox qualityList = new ListBox();

        Checkbox alwayShowOptions;
        Checkbox vsync;
        Checkbox swapHealthAndFatigue;
        Checkbox invertMouseVertical;
        Checkbox mouseSmoothing;
        Checkbox leftHandWeapons;
        Checkbox playerNudity;
        Checkbox clickToAttack;

        Color unselectedTextColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        Color selectedTextColor = new Color(0.0f, 0.8f, 0.0f, 1.0f);

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
        const string testText = "Test";
        const string okText = "OK";
        const string keepSetting = "Keep this setting? Changes will revert in 8 seconds.";

        Color backgroundColor = new Color(0, 0, 0, 0.7f);
        Color confirmEnabledBackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        Color confirmDisabledBackgroundColor = new Color(0.5f, 0.0f, 0.0f, 0.4f);

        Resolution initialResolution;
        Resolution[] availableResolutions;
        bool resolutionConfirmed = false;

        SetupStages currentStage = SetupStages.None;
        string arena2Path = string.Empty;
        bool moveNextStage = false;

        #endregion

        #region Enums

        public enum SetupStages
        {
            None,
            GameFolder,
            Resolution,
            Options,
            //Summary,
            LaunchGame,
        }

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
            AllowCancel = false;
            LoadResources();

            // Add exit button
            Button exitButton = new Button();
            exitButton.Size = new Vector2(20, 9);
            exitButton.HorizontalAlignment = HorizontalAlignment.Center;
            exitButton.VerticalAlignment = VerticalAlignment.Bottom;
            exitButton.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
            exitButton.Outline.Enabled = true;
            exitButton.Label.Text = exitButtonText;
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            NativePanel.Components.Add(exitButton);

            // If actually validated and we just want to see settings then move direct to settings page
            if (DaggerfallUnity.Instance.IsPathValidated && DaggerfallUnity.Settings.ShowOptionsAtStart)
            {
                currentStage = SetupStages.Options - 1;
            }

            moveNextStage = true;
        }

        public override void Update()
        {
            base.Update();

            // Loop title song
            if (!DaggerfallUI.Instance.DaggerfallSongPlayer.IsPlaying)
            {
                DaggerfallUI.Instance.DaggerfallSongPlayer.Play(titleSongFile);
            }

            // Move to next setup stage
            if (moveNextStage)
            {
                ShowNextStage();
                moveNextStage = false;
            }
        }

        void ShowGameFolderStage()
        {
            // Set temporary background texture
            if (titleTexture != null)
            {
                titleTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
                ParentPanel.BackgroundTexture = titleTexture;
                ParentPanel.BackgroundTextureLayout = BackgroundLayout.ScaleToFit;
            }

            // Setup panel
            browserPanel.BackgroundColor = backgroundColor;
            browserPanel.HorizontalAlignment = HorizontalAlignment.Center;
            browserPanel.VerticalAlignment = VerticalAlignment.Middle;
            browserPanel.Size = browserPanelSize;
            browserPanel.Outline.Enabled = true;
            NativePanel.Components.Add(browserPanel);

            // Setup screen text
            MultiFormatTextLabel screen = new MultiFormatTextLabel();
            screen.HorizontalAlignment = HorizontalAlignment.Center;
            screen.Position = new Vector2(0, 10);
            screen.TextAlignment = HorizontalAlignment.Center;
            screen.SetText(Resources.Load<TextAsset>("Screens/WelcomeScreen"));
            browserPanel.Components.Add(screen);

            // Setup folder browser
            browser.Position = new Vector2(4, 30);
            browser.Size = new Vector2(250, 104);
            browser.HorizontalAlignment = HorizontalAlignment.Center;
            browser.ConfirmEnabled = false;
            browser.OnConfirmPath += Browser_OnConfirmPath;
            browser.OnPathChanged += Browser_OnPathChanged;
            browserPanel.Components.Add(browser);

            // Add version number
            TextLabel versionLabel = new TextLabel();
            versionLabel.Position = new Vector2(0, 1);
            versionLabel.HorizontalAlignment = HorizontalAlignment.Right;
            versionLabel.ShadowPosition = Vector2.zero;
            versionLabel.TextColor = Color.gray;
            versionLabel.Text = VersionInfo.DaggerfallUnityVersion;
            browserPanel.Components.Add(versionLabel);

            // Add help text
            helpLabel.Position = new Vector2(0, 145);
            helpLabel.HorizontalAlignment = HorizontalAlignment.Center;
            helpLabel.ShadowPosition = Vector2.zero;
            helpLabel.Text = findArena2Tip;
            browserPanel.Components.Add(helpLabel);
        }

        bool backdropCreated = false;
        void CreateBackdrop()
        {
            // Add a block into the scene
            GameObjectHelper.CreateRMBBlockGameObject("CUSTAA06.RMB", 0, 0);
            backdropCreated = true;

            // Clear background texture
            ParentPanel.BackgroundTexture = null;
            ParentPanel.BackgroundColor = Color.clear;
        }

        void ShowResolutionPanel()
        {
            // Disable previous stage
            browserPanel.Enabled = false;

            // Get resolutions
            initialResolution = Screen.currentResolution;
            availableResolutions = Screen.resolutions;

            // Create backdrop
            if (!backdropCreated)
                CreateBackdrop();

            // Add resolution panel
            resolutionPanel.Outline.Enabled = true;
            resolutionPanel.BackgroundColor = backgroundColor;
            resolutionPanel.HorizontalAlignment = HorizontalAlignment.Left;
            resolutionPanel.VerticalAlignment = VerticalAlignment.Middle;
            resolutionPanel.Size = new Vector2(120, 175);
            NativePanel.Components.Add(resolutionPanel);

            // Add resolution title text
            TextLabel resolutionTitleLabel = new TextLabel();
            resolutionTitleLabel.Text = "Resolution";
            resolutionTitleLabel.Position = new Vector2(0, 2);
            //resolutionTitleLabel.ShadowPosition = Vector2.zero;
            resolutionTitleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            resolutionPanel.Components.Add(resolutionTitleLabel);

            // Add resolution picker
            resolutionList.BackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            resolutionList.TextColor = unselectedTextColor;
            resolutionList.SelectedTextColor = selectedTextColor;
            resolutionList.ShadowPosition = Vector2.zero;
            resolutionList.HorizontalAlignment = HorizontalAlignment.Center;
            resolutionList.RowsDisplayed = 8;
            resolutionList.RowAlignment = HorizontalAlignment.Center;
            resolutionList.Position = new Vector2(0, 12);
            resolutionList.Size = new Vector2(80, 62);
            resolutionList.SelectedShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
            resolutionList.SelectedShadowColor = Color.black;
            resolutionList.OnMouseClick += ResolutionList_OnMouseClick;
            resolutionList.OnScroll += ResolutionList_OnScroll;
            resolutionPanel.Components.Add(resolutionList);

            // Add resolution scrollbar
            resolutionScroller.Position = new Vector2(100, 12);
            resolutionScroller.Size = new Vector2(5, 62);
            resolutionScroller.OnScroll += ResolutionScroller_OnScroll;
            resolutionPanel.Components.Add(resolutionScroller);

            // Add resolutions
            for (int i = 0; i < availableResolutions.Length; i++)
            {
                string item = string.Format("{0}x{1}", availableResolutions[i].width, availableResolutions[i].height);
                resolutionList.AddItem(item);

                if (availableResolutions[i].width == initialResolution.width &&
                    availableResolutions[i].height == initialResolution.height)
                {
                    resolutionList.SelectedIndex = i;
                }
            }
            resolutionList.ScrollToSelected();

            // Setup scroller
            resolutionScroller.DisplayUnits = 8;
            resolutionScroller.TotalUnits = resolutionList.Count;
            resolutionScroller.BackgroundColor = resolutionList.BackgroundColor;

            // Add fullscreen checkbox
            fullscreenCheckbox.Label.Text = "Fullscreen";
            fullscreenCheckbox.Label.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
            fullscreenCheckbox.Label.ShadowColor = Color.black;
            fullscreenCheckbox.Position = new Vector2(0, 76);
            fullscreenCheckbox.HorizontalAlignment = HorizontalAlignment.Center;
            fullscreenCheckbox.IsChecked = Screen.fullScreen;
            fullscreenCheckbox.CheckBoxColor = selectedTextColor;
            fullscreenCheckbox.Label.TextColor = selectedTextColor;
            fullscreenCheckbox.OnToggleState += FullscreenCheckbox_OnToggleState;
            resolutionPanel.Components.Add(fullscreenCheckbox);

            // Add quality title text
            TextLabel qualityTitleLabel = new TextLabel();
            qualityTitleLabel.Text = "Quality";
            qualityTitleLabel.Position = new Vector2(0, 92);
            //qualityTitleLabel.ShadowPosition = Vector2.zero;
            qualityTitleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            resolutionPanel.Components.Add(qualityTitleLabel);

            // Add quality picker
            qualityList.BackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            qualityList.TextColor = unselectedTextColor;
            qualityList.SelectedTextColor = selectedTextColor;
            qualityList.ShadowPosition = Vector2.zero;
            qualityList.HorizontalAlignment = HorizontalAlignment.Center;
            qualityList.RowsDisplayed = 6;
            qualityList.RowAlignment = HorizontalAlignment.Center;
            qualityList.Position = new Vector2(0, 102);
            qualityList.Size = new Vector2(85, 46);
            qualityList.SelectedShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
            qualityList.SelectedShadowColor = Color.black;
            qualityList.OnMouseClick += QualityList_OnMouseClick;
            resolutionPanel.Components.Add(qualityList);
            foreach(var name in QualitySettings.names)
            {
                qualityList.AddItem(name);
            }
            qualityList.SelectedIndex = DaggerfallUnity.Settings.QualityLevel;

            // Test/confirm button
            testOrConfirmButton.Position = new Vector2(0, 160);
            testOrConfirmButton.Size = new Vector2(40, 12);
            testOrConfirmButton.Outline.Enabled = true;
            testOrConfirmButton.Label.Text = testText;
            testOrConfirmButton.BackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
            testOrConfirmButton.HorizontalAlignment = HorizontalAlignment.Center;
            testOrConfirmButton.OnMouseClick += ResolutionTestOrConfirmButton_OnMouseClick;
            resolutionPanel.Components.Add(testOrConfirmButton);
        }

        float optionPos = 12f;
        float optionSpacing = 12f;
        Checkbox AddOption(float x, string text, string tip, bool isChecked)
        {
            Checkbox checkbox = new Checkbox();
            checkbox.Label.Text = text;
            checkbox.Label.TextColor = selectedTextColor;
            checkbox.CheckBoxColor = selectedTextColor;
            checkbox.ToolTip = defaultToolTip;
            checkbox.ToolTipText = tip;
            checkbox.IsChecked = isChecked;
            checkbox.Position = new Vector2(x, optionPos);
            optionsPanel.Components.Add(checkbox);
            optionPos += optionSpacing;

            return checkbox;
        }

        bool GetLeftHandWeapons()
        {
            if (DaggerfallUnity.Settings.Handedness == 1)
                return true;
            else
                return false;
        }

        int GetHandedness(bool value)
        {
            if (value == true)
                return 1;
            else
                return 0;
        }

        void ShowOptionsPanel()
        {
            // Disable previous stage
            resolutionPanel.Enabled = false;

            // Create backdrop
            if (!backdropCreated)
                CreateBackdrop();

            // Add options panel
            optionsPanel.Outline.Enabled = true;
            optionsPanel.BackgroundColor = backgroundColor;
            optionsPanel.HorizontalAlignment = HorizontalAlignment.Center;
            //optionsPanel.VerticalAlignment = VerticalAlignment.Middle;
            optionsPanel.Position = new Vector2(0, 8);
            optionsPanel.Size = new Vector2(318, 165);
            NativePanel.Components.Add(optionsPanel);

            // Add options title text
            TextLabel titleLabel = new TextLabel();
            titleLabel.Text = "Options";
            titleLabel.Position = new Vector2(0, 2);
            titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            optionsPanel.Components.Add(titleLabel);

            // Add settings path text
            TextLabel settingsPathLabel = new TextLabel();
            settingsPathLabel.Text = DaggerfallUnity.Settings.PersistentDataPath;
            settingsPathLabel.Position = new Vector2(0, 170);
            settingsPathLabel.HorizontalAlignment = HorizontalAlignment.Center;
            settingsPathLabel.ShadowPosition = Vector2.zero;
            settingsPathLabel.TextColor = Color.gray;
            settingsPathLabel.BackgroundColor = backgroundColor;
            optionsPanel.Components.Add(settingsPathLabel);

            // Setup options checkboxes
            float x = 8;
            optionPos = 20;
            alwayShowOptions = AddOption(x, "Always show this window", "Always show this window on startup\rOtherwise use settings.ini to configure", DaggerfallUnity.Settings.ShowOptionsAtStart);
            vsync = AddOption(x, "Vertical Sync", "Sync FPS with monitor refresh", DaggerfallUnity.Settings.VSync);
            swapHealthAndFatigue = AddOption(x, "Swap Health & Fatigue", "Swap health & fatigue bar colors", DaggerfallUnity.Settings.SwapHealthAndFatigueColors);
            invertMouseVertical = AddOption(x, "Invert Mouse", "Invert mouse-look vertical", DaggerfallUnity.Settings.InvertMouseVertical);
            mouseSmoothing = AddOption(x, "Mouse Smoothing", "Smooth mouse-look sampling", DaggerfallUnity.Settings.MouseLookSmoothing);
            leftHandWeapons = AddOption(x, "Left Hand Weapons", "Draw weapons on left side of screen", GetLeftHandWeapons());
            playerNudity = AddOption(x, "Player Nudity", "Allow nudity on paper doll", DaggerfallUnity.Settings.PlayerNudity);
            clickToAttack = AddOption(x, "Click to attack", "Enable a simple click to trigger attacks", DaggerfallUnity.Settings.ClickToAttack);

            // Setup mods checkboxes
            // TODO: Might rework this, but could still be useful for certain core mods later
            x = 165;
            optionPos = 20;
            //bool exampleModCheckbox = AddOption(x, "Example", "Example built-in mod", DaggerfallUnity.Settings.ExampleModOption);

            // Add mod note
            string modNote = "Note: Enabling mods can increase performance requirements";
            TextLabel modNoteLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 125), modNote, optionsPanel);
            modNoteLabel.HorizontalAlignment = HorizontalAlignment.Center;
            modNoteLabel.ShadowPosition = Vector2.zero;

            // Confirm button
            Button optionsConfirmButton = new Button();
            optionsConfirmButton.Position = new Vector2(0, optionsPanel.InteriorHeight - 15);
            optionsConfirmButton.Size = new Vector2(40, 12);
            optionsConfirmButton.Outline.Enabled = true;
            optionsConfirmButton.Label.Text = "Play";
            optionsConfirmButton.BackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
            optionsConfirmButton.HorizontalAlignment = HorizontalAlignment.Center;
            optionsConfirmButton.OnMouseClick += OptionsConfirmButton_OnMouseClick;
            optionsPanel.Components.Add(optionsConfirmButton);

            // Restart button
            Button restartButton = new Button();
            restartButton.Size = new Vector2(45, 12);
            restartButton.Label.Text = "< Restart";
            restartButton.Label.ShadowPosition = Vector2.zero;
            restartButton.Label.TextColor = Color.gray;
            restartButton.ToolTip = defaultToolTip;
            restartButton.ToolTipText = "Restart setup from beginning";
            restartButton.VerticalAlignment = VerticalAlignment.Top;
            restartButton.HorizontalAlignment = HorizontalAlignment.Left;
            restartButton.OnMouseClick += RestartButton_OnMouseClick;
            optionsPanel.Components.Add(restartButton);

            if (DaggerfallUnity.Settings.LypyL_ModSystem)
            {
                Button ShowModsButton = new Button();
                ShowModsButton.Label.Text = "Mods";
                ShowModsButton.Position = new Vector2(3, optionsConfirmButton.Position.y);
                ShowModsButton.Size = optionsConfirmButton.Size;
                ShowModsButton.BackgroundColor = optionsConfirmButton.BackgroundColor;
                ShowModsButton.Label.TextColor = optionsConfirmButton.Label.TextColor;
                ShowModsButton.Outline.Enabled = true;
                optionsPanel.Components.Add(ShowModsButton);
                ShowModsButton.OnMouseClick += ModsButton_OnOnMouseBlick;
            }

            // Advanced Settings
            Button AdvancedSettingsButton = new Button();
            AdvancedSettingsButton.Label.Text = "Advanced";
            AdvancedSettingsButton.Size = new Vector2(45, 12);
            AdvancedSettingsButton.Position = new Vector2(optionsPanel.InteriorWidth - AdvancedSettingsButton.Size.x - 3, optionsConfirmButton.Position.y);
            AdvancedSettingsButton.BackgroundColor = optionsConfirmButton.BackgroundColor;
            AdvancedSettingsButton.Label.TextColor = optionsConfirmButton.Label.TextColor;
            AdvancedSettingsButton.Outline.Enabled = true;
            optionsPanel.Components.Add(AdvancedSettingsButton);
            AdvancedSettingsButton.OnMouseClick += AdvancedSettingsButton_OnOnMouseBlick;

        }

        //void ShowSummaryPanel()
        //{
        //    // Disable previous stage
        //    optionsPanel.Enabled = false;

        //    // Add summary panel
        //    summaryPanel.Outline.Enabled = true;
        //    summaryPanel.BackgroundColor = backgroundColor;
        //    summaryPanel.HorizontalAlignment = HorizontalAlignment.Center;
        //    summaryPanel.VerticalAlignment = VerticalAlignment.Middle;
        //    summaryPanel.Size = new Vector2(300, 100);
        //    NativePanel.Components.Add(summaryPanel);

        //    // Setup screen text
        //    MultiFormatTextLabel screen = new MultiFormatTextLabel();
        //    screen.HorizontalAlignment = HorizontalAlignment.Center;
        //    screen.Position = new Vector2(0, 10);
        //    screen.TextAlignment = HorizontalAlignment.Center;
        //    screen.SetText(Resources.Load<TextAsset>("Screens/SetupSummary"));
        //    summaryPanel.Components.Add(screen);

        //    // Confirm button
        //    Button summaryConfirmButton = new Button();
        //    summaryConfirmButton.Position = new Vector2(0, 80);
        //    summaryConfirmButton.Size = new Vector2(40, 12);
        //    summaryConfirmButton.Outline.Enabled = true;
        //    summaryConfirmButton.Label.Text = okText;
        //    summaryConfirmButton.BackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        //    summaryConfirmButton.HorizontalAlignment = HorizontalAlignment.Center;
        //    summaryConfirmButton.OnMouseClick += SummaryConfirmButton_OnMouseClick;
        //    summaryPanel.Components.Add(summaryConfirmButton);

        //}

        #endregion

        #region Private Methods

        void LoadResources()
        {
            // Load title background texture
            titleTexture = Resources.Load<Texture2D>(titleScreenFilename);
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

        void ShowNextStage()
        {
            int stage = (int)currentStage + 1;
            currentStage = (SetupStages)stage;
            switch (currentStage)
            {
                case SetupStages.GameFolder:
                    ShowGameFolderStage();
                    break;
                case SetupStages.Resolution:
                    ShowResolutionPanel();
                    break;
                case SetupStages.Options:
                    ShowOptionsPanel();
                    break;
                //case SetupStages.Summary:
                //    ShowSummaryPanel();
                //    break;
                case SetupStages.LaunchGame:
                    SceneManager.LoadScene(DaggerfallWorkshop.Game.Utility.SceneControl.GameSceneIndex);
                    break;
            }
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
                arena2Path = string.Empty;
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
                arena2Path = string.Empty;
                return;
            }

            // Path is valid
            browser.ConfirmEnabled = true;
            browser.BackgroundColor = confirmEnabledBackgroundColor;
            helpLabel.Text = pathValidated;
            arena2Path = pathResult;
        }

        private void Browser_OnConfirmPath()
        {
            if (string.IsNullOrEmpty(arena2Path))
                return;

            // Set new path and save settings
            DaggerfallUnity.Settings.MyDaggerfallPath = browser.CurrentPath;
            DaggerfallUnity.Settings.SaveSettings();

            // Change arena2 path
            DaggerfallUnity.ChangeArena2Path(arena2Path);

            // Move to next setup stage
            if (DaggerfallUnity.IsPathValidated)
                moveNextStage = true;
        }

        private void ResolutionList_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            resolutionConfirmed = false;
            testOrConfirmButton.Label.Text = testText;
        }

        private void QualityList_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            resolutionConfirmed = false;
            testOrConfirmButton.Label.Text = testText;
        }

        private void FullscreenCheckbox_OnToggleState()
        {
            resolutionConfirmed = false;
            testOrConfirmButton.Label.Text = testText;
        }

        private void ResolutionTestOrConfirmButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Resolution selectedResolution = availableResolutions[resolutionList.SelectedIndex];
            if (!resolutionConfirmed)
            {
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullscreenCheckbox.IsChecked);
                resolutionConfirmed = true;
                testOrConfirmButton.Label.Text = okText;
                QualitySettings.SetQualityLevel(qualityList.SelectedIndex);
            }
            else
            {
                DaggerfallUnity.Settings.ResolutionWidth = selectedResolution.width;
                DaggerfallUnity.Settings.ResolutionHeight = selectedResolution.height;
                DaggerfallUnity.Settings.Fullscreen = fullscreenCheckbox.IsChecked;
                DaggerfallUnity.Settings.QualityLevel = qualityList.SelectedIndex;
                moveNextStage = true;
            }
        }

        private void OptionsConfirmButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUnity.Settings.ShowOptionsAtStart = alwayShowOptions.IsChecked;
            DaggerfallUnity.Settings.VSync = vsync.IsChecked;
            DaggerfallUnity.Settings.SwapHealthAndFatigueColors = swapHealthAndFatigue.IsChecked;
            DaggerfallUnity.Settings.InvertMouseVertical = invertMouseVertical.IsChecked;
            DaggerfallUnity.Settings.MouseLookSmoothing = mouseSmoothing.IsChecked;
            DaggerfallUnity.Settings.Handedness = GetHandedness(leftHandWeapons.IsChecked);
            DaggerfallUnity.Settings.PlayerNudity = playerNudity.IsChecked;
            DaggerfallUnity.Settings.ClickToAttack = clickToAttack.IsChecked;

            DaggerfallUnity.Settings.SaveSettings();
            moveNextStage = true;
        }

        private void SummaryConfirmButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            moveNextStage = true;
        }

        private void ResolutionScroller_OnScroll()
        {
            resolutionList.ScrollIndex = resolutionScroller.ScrollIndex;
        }

        private void ResolutionList_OnScroll()
        {
            resolutionScroller.ScrollIndex = resolutionList.ScrollIndex;
        }

        private void RestartButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUnity.Settings.MyDaggerfallPath = string.Empty;
            SceneManager.LoadScene(Utility.SceneControl.StartupSceneIndex);
        }

        //just a quick way to list all the mods loaded during setup
        private void ModsButton_OnOnMouseBlick(BaseScreenComponent sender, Vector2 position)
        {
            if (optionsPanel.Enabled)
            {
                ModLoaderInterfaceWindow modLoaderWindow = new ModLoaderInterfaceWindow(DaggerfallUI.UIManager);
                DaggerfallUI.UIManager.PushWindow(modLoaderWindow);
            }
        }

        // Advanced Settings
        private void AdvancedSettingsButton_OnOnMouseBlick(BaseScreenComponent sender, Vector2 position)
        {
            if (optionsPanel.Enabled)
            {
                AdvancedSettingsWindow advancedSettingsWindow = new AdvancedSettingsWindow(DaggerfallUI.UIManager);
                DaggerfallUI.UIManager.PushWindow(advancedSettingsWindow);
            }
        }

        #endregion
    }
}