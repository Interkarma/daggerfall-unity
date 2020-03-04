// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
using System.Linq;
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
using DaggerfallWorkshop.Utility.AssetInjection;

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
        Checkbox sdfFontRendering;
        Checkbox retro320x200WorldRendering;

        Color unselectedTextColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        Color selectedTextColor = new Color(0.0f, 0.8f, 0.0f, 1.0f);
        Color secondaryTextColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);

        #endregion

        #region UI Textures

        Texture2D titleTexture;

        #endregion

        #region Fields

        const string titleScreenFilename = "StartupBackground2";
        const float panelSwipeTime = 1;
        const SongFiles titleSongFile = SongFiles.song_5strong;

        string findArena2Tip;
        string pathValidated;
        string testText;
        string okText;

        Color backgroundColor = new Color(0, 0, 0, 0.8f);
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
            exitButton.Label.Text = GetText("exit");
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.GameSetupExit);
            NativePanel.Components.Add(exitButton);

            // If actually validated and we just want to see settings then move direct to settings page
            if (DaggerfallUnity.Instance.IsPathValidated && (DaggerfallUnity.Settings.ShowOptionsAtStart || Input.anyKey))
            {
                currentStage = SetupStages.Options - 1;
            }

            moveNextStage = true;

            // Override cursor
            Texture2D tex;
            if (TextureReplacement.TryImportTexture("Cursor", true, out tex))
            {
                Cursor.SetCursor(tex, Vector2.zero, CursorMode.Auto);
                Debug.Log("Cursor texture overridden by mods.");
            }
        }

        public override void Update()
        {
            base.Update();

            // Loop title song
            if (!DaggerfallUI.Instance.DaggerfallSongPlayer.IsPlaying)
            {
                DaggerfallUI.Instance.DaggerfallSongPlayer.Play(titleSongFile);
            }

            // Sync SDF font rendering to current setting
            // This is a special realtime setting as font rendering can change at any time, even during the setup process itself
            if (sdfFontRendering != null)
                sdfFontRendering.IsChecked = DaggerfallUnity.Settings.SDFFontRendering;

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
            versionLabel.TextColor = secondaryTextColor;
            versionLabel.Text = VersionInfo.DaggerfallUnityVersion;
            browserPanel.Components.Add(versionLabel);

            // Add help text
            findArena2Tip = GetText("findArena2Tip");
            pathValidated = GetText("pathValidated");
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
            availableResolutions = DaggerfallUI.GetDistinctResolutions();

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
            resolutionTitleLabel.Text = GetText("resolution");
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
            fullscreenCheckbox.Label.Text = GetText("fullscreen");
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
            qualityTitleLabel.Text = GetText("quality");
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
            testText = GetText("testText");
            okText = GetText("okText");
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
        Checkbox AddOption(float x, string key, bool isChecked)
        {
            Checkbox checkbox = new Checkbox();
            checkbox.Label.Text = GetText(key);
            checkbox.Label.TextColor = selectedTextColor;
            checkbox.CheckBoxColor = selectedTextColor;
            checkbox.ToolTip = defaultToolTip;
            checkbox.ToolTipText = GetText(string.Format("{0}Info", key));
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

            // Add title text
            TextLabel titleLabel = new TextLabel(DaggerfallUI.Instance.Font2);
            titleLabel.Text = "Daggerfall Unity";
            titleLabel.Position = new Vector2(0, 15);
            titleLabel.TextScale = 1.4f;
            titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            optionsPanel.Components.Add(titleLabel);

            // Add version text
            TextLabel versionLabel = new TextLabel(DaggerfallUI.DefaultFont);
            versionLabel.Text = string.Format("{0} v{1}", char.ToUpper(VersionInfo.DaggerfallUnityStatus[0]) + VersionInfo.DaggerfallUnityStatus.Substring(1), VersionInfo.DaggerfallUnityVersion);
            versionLabel.Position = new Vector2(0, 40);
            versionLabel.TextScale = 1.0f;
            versionLabel.HorizontalAlignment = HorizontalAlignment.Center;
            versionLabel.ShadowPosition = Vector2.zero;
            versionLabel.TextColor = secondaryTextColor;
            optionsPanel.Components.Add(versionLabel);

            // Add settings path text
            TextLabel settingsPathLabel = new TextLabel();
            settingsPathLabel.Text = DaggerfallUnity.Settings.PersistentDataPath;
            settingsPathLabel.Position = new Vector2(0, 170);
            settingsPathLabel.HorizontalAlignment = HorizontalAlignment.Center;
            settingsPathLabel.ShadowPosition = Vector2.zero;
            settingsPathLabel.TextColor = secondaryTextColor;
            settingsPathLabel.BackgroundColor = backgroundColor;
            optionsPanel.Components.Add(settingsPathLabel);

            // Setup options checkboxes
            float x = 8;
            optionPos = 60;
            alwayShowOptions = AddOption(x, "alwayShowOptions", DaggerfallUnity.Settings.ShowOptionsAtStart);
            vsync = AddOption(x, "vsync", DaggerfallUnity.Settings.VSync);
            swapHealthAndFatigue = AddOption(x, "swapHealthAndFatigue", DaggerfallUnity.Settings.SwapHealthAndFatigueColors);
            invertMouseVertical = AddOption(x, "invertMouseVertical", DaggerfallUnity.Settings.InvertMouseVertical);
            mouseSmoothing = AddOption(x, "mouseSmoothing", DaggerfallUnity.Settings.MouseLookSmoothing);

            x = 165;
            optionPos = 60;
            leftHandWeapons = AddOption(x, "leftHandWeapons", GetLeftHandWeapons());
            playerNudity = AddOption(x, "playerNudity", DaggerfallUnity.Settings.PlayerNudity);
            clickToAttack = AddOption(x, "clickToAttack", DaggerfallUnity.Settings.ClickToAttack);

            // Setup mods checkboxes
            // TODO: Might rework this, but could still be useful for certain core mods later
            sdfFontRendering = AddOption(x, "sdfFontRendering", DaggerfallUnity.Settings.SDFFontRendering);
            sdfFontRendering.OnToggleState += SDFFontRendering_OnToggleState;
            //bool exampleModCheckbox = AddOption(x, "Example", "Example built-in mod", DaggerfallUnity.Settings.ExampleModOption);

            // Add mod note
            TextLabel modNoteLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 125), GetText("modNote"), optionsPanel);
            modNoteLabel.HorizontalAlignment = HorizontalAlignment.Center;
            modNoteLabel.ShadowPosition = Vector2.zero;

            // Confirm button
            Button optionsConfirmButton = new Button();
            optionsConfirmButton.Position = new Vector2(0, optionsPanel.InteriorHeight - 15);
            optionsConfirmButton.Size = new Vector2(40, 12);
            optionsConfirmButton.Outline.Enabled = true;
            optionsConfirmButton.Label.Text = GetText("play");
            optionsConfirmButton.BackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
            optionsConfirmButton.HorizontalAlignment = HorizontalAlignment.Center;
            optionsConfirmButton.OnMouseClick += OptionsConfirmButton_OnMouseClick;
            optionsConfirmButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.GameSetupPlay);
            optionsPanel.Components.Add(optionsConfirmButton);

            // Restart button
            Button restartButton = new Button();
            restartButton.Size = new Vector2(45, 12);
            restartButton.Label.Text = string.Format("< {0}", GetText("restart"));
            restartButton.Label.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
            restartButton.Label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            restartButton.Label.HorizontalAlignment = HorizontalAlignment.Left;
            restartButton.ToolTip = defaultToolTip;
            restartButton.ToolTipText = GetText("restartInfo");
            restartButton.VerticalAlignment = VerticalAlignment.Top;
            restartButton.HorizontalAlignment = HorizontalAlignment.Left;
            restartButton.OnMouseClick += RestartButton_OnMouseClick;
            restartButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.GameSetupRestart);
            optionsPanel.Components.Add(restartButton);

            if (DaggerfallUnity.Settings.LypyL_ModSystem)
            {
                Button ShowModsButton = new Button();
                ShowModsButton.Label.Text = GetText("mods");
                ShowModsButton.Position = new Vector2(3, optionsConfirmButton.Position.y);
                ShowModsButton.Size = optionsConfirmButton.Size;
                ShowModsButton.BackgroundColor = optionsConfirmButton.BackgroundColor;
                ShowModsButton.Label.TextColor = optionsConfirmButton.Label.TextColor;
                ShowModsButton.Outline.Enabled = true;
                optionsPanel.Components.Add(ShowModsButton);
                ShowModsButton.OnMouseClick += ModsButton_OnOnMouseBlick;
                ShowModsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.GameSetupMods);
            }

            // Advanced Settings
            Button AdvancedSettingsButton = new Button();
            AdvancedSettingsButton.Label.Text = GetText("advanced");
            AdvancedSettingsButton.Size = new Vector2(45, 12);
            AdvancedSettingsButton.Position = new Vector2(optionsPanel.InteriorWidth - AdvancedSettingsButton.Size.x - 3, optionsConfirmButton.Position.y);
            AdvancedSettingsButton.BackgroundColor = optionsConfirmButton.BackgroundColor;
            AdvancedSettingsButton.Label.TextColor = optionsConfirmButton.Label.TextColor;
            AdvancedSettingsButton.Outline.Enabled = true;
            optionsPanel.Components.Add(AdvancedSettingsButton);
            AdvancedSettingsButton.OnMouseClick += AdvancedSettingsButton_OnOnMouseBlick;
            AdvancedSettingsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.GameSetupAdvancedSettings);

        }

        private void SDFFontRendering_OnToggleState()
        {
            // Immediately switch font rendering
            DaggerfallUnity.Settings.SDFFontRendering = sdfFontRendering.IsChecked;
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
                return string.Format("{0} {1}", GetText("foundArena2But"), GetText("missingTextures"));
            else if (!validationResults.ModelsValid)
                return string.Format("{0} {1}", GetText("foundArena2But"), GetText("missingModels"));
            else if (!validationResults.BlocksValid)
                return string.Format("{0} {1}", GetText("foundArena2But"), GetText("missingBlocks"));
            else if (!validationResults.MapsValid)
                return string.Format("{0} {1}", GetText("foundArena2But"), GetText("missingMaps"));
            else if (!validationResults.SoundsValid)
                return string.Format("{0} {1}", GetText("foundArena2But"), GetText("missingSounds"));
            else if (!validationResults.WoodsValid)
                return string.Format("{0} {1}", GetText("foundArena2But"), GetText("missingWoods"));
            else if (!validationResults.VideosValid)
                return string.Format("{0} {1}", GetText("foundArena2But"), GetText("missingVideos"));
            else
                return GetText("justNotValid");
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

        private static string GetText(string key)
        {
            return TextManager.Instance.GetText("MainMenu", key);
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
            if (DaggerfallUnity.Settings.ShowOptionsAtStart && !alwayShowOptions.IsChecked)
            {
                var messageBox = new DaggerfallMessageBox(uiManager, this, true);
                messageBox.SetText(GetText("showOptionsAgain"));
                messageBox.AllowCancel = true;
                messageBox.ClickAnywhereToClose = true;
                messageBox.OnClose += () => {
                    SaveOptionsAndContinue();
                };
                uiManager.PushWindow(messageBox);
                return;
            }

            SaveOptionsAndContinue();
        }

        private void SaveOptionsAndContinue()
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

            if (ModManager.Instance)
            {
                foreach (Mod mod in ModManager.Instance.Mods.Where(x => x.Enabled))
                {
                    bool? isGameVersionSatisfied = mod.IsGameVersionSatisfied();
                    if (isGameVersionSatisfied == false)
                    {
                        var messageBox = new DaggerfallMessageBox(uiManager, this, true);
                        messageBox.SetText(GetText("incompatibleMods"));
                        messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                        messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No, true);
                        messageBox.OnButtonClick += (_, messageBoxButton) =>
                        {
                            moveNextStage = messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes;
                            messageBox.CloseWindow();
                        };
                        uiManager.PushWindow(messageBox);
                        return;
                    }
                    else if (isGameVersionSatisfied == null)
                    {
                        Debug.LogWarningFormat("Unknown format for property \"DFUnity_Version\" of mod {0}. Please use x.y.z version of minimum compatible game build.", mod.Title);
                    }
                }
            }

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