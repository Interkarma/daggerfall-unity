// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Window with advanced settings for Daggerfall Unity.
    /// </summary>
    public class AdvancedSettingsWindow : DaggerfallPopupWindow
    {
        #region Constructors

        public AdvancedSettingsWindow(IUserInterfaceManager uiManager)
        : base(uiManager)
        {
        }

        #endregion

        #region Fields

        enum InteractionModeIconModes { none, minimal, large, classic, colour, monochrome };

        const string textTable = "GameSettings";

        const float topY = 8;
        readonly Vector2 topBarSize = new Vector2(318, 10);
        readonly Vector2 pageSize = new Vector2(318, 165);
        readonly Vector2 offset = new Vector2(10, 20);
        const float columnHeight = 140;

        const float itemTextScale = 0.9f;
        const float sectionSpacing = 12f;
        const float itemSpacing = 10f;

        const string closeButtonText = "Close";

        // Panels
        List<Panel> pages = new List<Panel>();
        List<Button> pagesButton = new List<Button>();
        Panel bar = new Panel();

        // Colors
        Color backgroundColor           = new Color(0, 0, 0, 0.7f);
        Color closeButtonColor          = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        Color itemColor                 = new Color(0.0f, 0.8f, 0.0f, 1.0f);
        //Color unselectedTextColor       = new Color(0.6f, 0.6f, 0.6f, 1f);
        Color selectedTextColor         = new Color32(243, 239, 44, 255);
        //Color listBoxBackgroundColor    = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        //Color sliderBackgroundColor     = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        Color pageButtonSelected        = Color.white;
        Color pageButtonUnselected      = Color.gray;

        // Fonts
        DaggerfallFont titleFont        = DaggerfallUI.Instance.Font2;
        DaggerfallFont pageButtonFont   = DaggerfallUI.Instance.Font3;

        int currentPage = 0;
        float y = 0;

        bool applyScreenChanges = false;

        #endregion

        #region Settings Controls

        // GamePlay
        Checkbox startInDungeon;
        HorizontalSlider randomDungeonTextures;
        Checkbox crosshair;
        Checkbox toolTips;
        HorizontalSlider helmAndShieldMaterialDisplay;
        Checkbox inventoryInfoPanel;
        Checkbox enhancedItemLists;
        Checkbox vitalsIndicators;
        HorizontalSlider mouseSensitivity;
        HorizontalSlider weaponSensitivity;
        HorizontalSlider movementAcceleration;
        TextBox weaponAttackThreshold;
        Checkbox gameConsole;
        Checkbox modSystem;
        Checkbox assetImport;
        Checkbox compressModdedTextures;
        Checkbox nearDeathWarning;
        Checkbox alternateRandomEnemySelection;
        Checkbox advancedClimbing;
        HorizontalSlider cameraRecoilStrength;

        // Video
        HorizontalSlider resolution;
        Checkbox fullscreen;
        HorizontalSlider qualityLevel;
        HorizontalSlider mainFilterMode;
        HorizontalSlider guiFilterMode;
        HorizontalSlider videoFilterMode;
        HorizontalSlider fovSlider;
        HorizontalSlider terrainDistance;
        HorizontalSlider shadowResolutionMode;
        Checkbox dungeonLightShadows;
        Checkbox interiorLightShadows;
        Checkbox useLegacyDeferred;

        // Theme
        HorizontalSlider toolTipDelayInSeconds;
        Button toolTipTextColor;
        Button toolTipBackgroundColor;
        Checkbox enableModernConversationStyleInTalkWindow;
        HorizontalSlider interactionModeIcon;

        #endregion

        #region Override Methods

        /// <summary>
        /// Setup Advanced Settings Panel
        /// </summary>
        protected override void Setup()
        {
            AllowCancel = false;
            ParentPanel.BackgroundColor = Color.clear;

            // Pages selection top bar
            bar.Outline.Enabled = true;
            bar.BackgroundColor = backgroundColor;
            bar.HorizontalAlignment = HorizontalAlignment.Center;
            bar.Position = new Vector2(0, topY);
            bar.Size = topBarSize;
            NativePanel.Components.Add(bar);

            // Setup pages
            LoadSettings();

            // Add Close button
            Button closeButton = new Button();
            closeButton.Size = new Vector2(25, 9);
            closeButton.HorizontalAlignment = HorizontalAlignment.Center;
            closeButton.VerticalAlignment = VerticalAlignment.Bottom;
            closeButton.BackgroundColor = closeButtonColor;
            closeButton.Outline.Enabled = true;
            closeButton.Label.Text = closeButtonText;
            closeButton.OnMouseClick += CloseButton_OnMouseClick;
            NativePanel.Components.Add(closeButton);
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Tab))
                NextPage();
        }

        #endregion

        #region Load/Save Settings

        private void LoadSettings()
        {
            AddPage("gamePlay", Gameplay);
            AddPage("video", Video);
            AddPage("theme", Theme);
        }

        private void Gameplay(Panel leftPanel, Panel rightPanel)
        {
            // Game
            AddSectionTitle(leftPanel, "game");
            startInDungeon = AddCheckbox(leftPanel, "startInDungeon", DaggerfallUnity.Settings.StartInDungeon);
            randomDungeonTextures = AddSlider(leftPanel, "randomDungeonTextures",
                DaggerfallUnity.Settings.RandomDungeonTextures, "classic", "climate", "climateOnly", "random", "randomOnly");
            cameraRecoilStrength = AddSlider(leftPanel, "cameraRecoilStrength",
                DaggerfallUnity.Settings.CameraRecoilStrength, "Off", "Low (25%)", "Medium (50%)", "High (75%)", "V. High(100%)");

            // GUI
            AddSectionTitle(leftPanel, "gui");
            crosshair = AddCheckbox(leftPanel, "crosshair", DaggerfallUnity.Settings.Crosshair);
            toolTips = AddCheckbox(leftPanel, "toolTips", DaggerfallUnity.Settings.EnableToolTips);
            helmAndShieldMaterialDisplay = AddSlider(leftPanel, "helmAndShieldMaterialDisplay",
                DaggerfallUnity.Settings.HelmAndShieldMaterialDisplay, "off", "noLeatChai", "noLeat", "on");
            inventoryInfoPanel = AddCheckbox(leftPanel, "inventoryInfoPanel", DaggerfallUnity.Settings.EnableInventoryInfoPanel); //TODO: better description
            enhancedItemLists = AddCheckbox(leftPanel, "enhancedItemLists", DaggerfallUnity.Settings.EnableEnhancedItemLists);
            vitalsIndicators = AddCheckbox(leftPanel, "vitalsIndicators", DaggerfallUnity.Settings.EnableVitalsIndicators);

            y = 0;

            // Controls
            AddSectionTitle(rightPanel, "controls");
            mouseSensitivity = AddSlider(rightPanel, "mouseSensitivity", 0.1f, 4.0f, DaggerfallUnity.Settings.MouseLookSensitivity);
            weaponSensitivity = AddSlider(rightPanel, "weaponSensitivity", 0.1f, 10.0f, DaggerfallUnity.Settings.WeaponSensitivity);
            movementAcceleration = AddSlider(rightPanel, "movementAcceleration", InputManager.minAcceleration, InputManager.maxAcceleration, DaggerfallUnity.Settings.MovementAcceleration);
            weaponAttackThreshold = AddTextbox(rightPanel, "weaponAttackThreshold", DaggerfallUnity.Settings.WeaponAttackThreshold.ToString());

            // Enhancements
            AddSectionTitle(rightPanel, "enhancements");
            gameConsole = AddCheckbox(rightPanel, "gameConsole", DaggerfallUnity.Settings.LypyL_GameConsole);
            modSystem = AddCheckbox(rightPanel, "modSystem", DaggerfallUnity.Settings.LypyL_ModSystem);
            assetImport = AddCheckbox(rightPanel, "assetImport", DaggerfallUnity.Settings.MeshAndTextureReplacement);
            compressModdedTextures = AddCheckbox(rightPanel, "compressModdedTextures", DaggerfallUnity.Settings.CompressModdedTextures);
            nearDeathWarning = AddCheckbox(rightPanel, "nearDeathWarning", DaggerfallUnity.Settings.NearDeathWarning);
            alternateRandomEnemySelection = AddCheckbox(rightPanel, "alternateRandomEnemySelection", DaggerfallUnity.Settings.AlternateRandomEnemySelection);
            advancedClimbing = AddCheckbox(rightPanel, "advancedClimbing", DaggerfallUnity.Settings.AdvancedClimbing);
        }

        private void Video(Panel leftPanel, Panel rightPanel)
        {
            // Basic settings
            AddSectionTitle(leftPanel, "basic");
            resolution = AddSlider(leftPanel, "resolution",
                Array.FindIndex(Screen.resolutions, x => x.width == DaggerfallUnity.Settings.ResolutionWidth && x.height == DaggerfallUnity.Settings.ResolutionHeight),
                Screen.resolutions.Select(x => string.Format("{0}x{1}", x.width, x.height)).ToArray());
            resolution.OnScroll += Resolution_OnScroll;
            fullscreen = AddCheckbox(leftPanel, "fullscreen", DaggerfallUnity.Settings.Fullscreen);
            fullscreen.OnToggleState += Fullscreen_OnToggleState;
            qualityLevel = AddSlider(leftPanel, "qualityLevel", DaggerfallUnity.Settings.QualityLevel, QualitySettings.names);
            qualityLevel.OnScroll += QualityLevel_OnScroll;
            string[] filterModes = new string[] { "Point", "Bilinear", "Trilinear" };
            mainFilterMode = AddSlider(leftPanel, "mainFilterMode", DaggerfallUnity.Settings.MainFilterMode, filterModes);
            guiFilterMode = AddSlider(leftPanel, "guiFilterMode", DaggerfallUnity.Settings.GUIFilterMode, filterModes);
            videoFilterMode = AddSlider(leftPanel, "videoFilterMode", DaggerfallUnity.Settings.VideoFilterMode, filterModes);

            y = 0;

            // Advanced settings
            AddSectionTitle(rightPanel, "advanced");
            fovSlider = AddSlider(rightPanel, "fovSlider", 60, 80, DaggerfallUnity.Settings.FieldOfView);
            terrainDistance = AddSlider(rightPanel, "terrainDistance", 1, 4, DaggerfallUnity.Settings.TerrainDistance);
            shadowResolutionMode = AddSlider(rightPanel, "shadowResolutionMode",
                DaggerfallUnity.Settings.ShadowResolutionMode, "Low", "Medium", "High", "Very High");
            dungeonLightShadows = AddCheckbox(rightPanel, "dungeonLightShadows", DaggerfallUnity.Settings.DungeonLightShadows);
            interiorLightShadows = AddCheckbox(rightPanel, "interiorLightShadows", DaggerfallUnity.Settings.InteriorLightShadows);
            useLegacyDeferred = AddCheckbox(rightPanel, "useLegacyDeferred", DaggerfallUnity.Settings.UseLegacyDeferred);
            string textureArrayLabel = "Texture Arrays: ";
            if (!SystemInfo.supports2DArrayTextures)
                textureArrayLabel += "Unsupported";
            else
                textureArrayLabel += DaggerfallUnity.Settings.EnableTextureArrays ? "Enabled" : "Disabled";
            AddInfo(rightPanel, textureArrayLabel, "Improved implementation of terrain textures, with better performance and modding support");
        }

        private void Theme(Panel leftPanel, Panel rightPanel)
        {
            // Tooltips
            AddSectionTitle(leftPanel, "tooltips");
            toolTipDelayInSeconds = AddSlider(leftPanel, "toolTipDelayInSeconds", 0, 10, DaggerfallUnity.Settings.ToolTipDelayInSeconds);
            toolTipTextColor = AddColorPicker(leftPanel, "toolTipTextColor", DaggerfallUnity.Settings.ToolTipTextColor);
            toolTipBackgroundColor = AddColorPicker(leftPanel, "toolTipBackgroundColor", DaggerfallUnity.Settings.ToolTipBackgroundColor);

            // Others
            AddSectionTitle(leftPanel, "others");
            enableModernConversationStyleInTalkWindow = AddCheckbox(leftPanel, "enableModernConversationStyleInTalkWindow", DaggerfallUnity.Settings.EnableModernConversationStyleInTalkWindow);
            interactionModeIcon = AddSlider(leftPanel, "interactionModeIcon",
                Enum.IsDefined(typeof(InteractionModeIconModes), DaggerfallUnity.Settings.InteractionModeIcon) ? (int)Enum.Parse(typeof(InteractionModeIconModes), DaggerfallUnity.Settings.InteractionModeIcon) : 0,
                Enum.GetNames(typeof(InteractionModeIconModes)));
        }

        private void SaveSettings()
        {
            /* GamePlay */

            DaggerfallUnity.Settings.StartInDungeon = startInDungeon.IsChecked;
            DaggerfallUnity.Settings.RandomDungeonTextures = randomDungeonTextures.ScrollIndex;

            DaggerfallUnity.Settings.Crosshair = crosshair.IsChecked;
            DaggerfallUnity.Settings.EnableToolTips = toolTips.IsChecked;
            DaggerfallUnity.Settings.HelmAndShieldMaterialDisplay = helmAndShieldMaterialDisplay.ScrollIndex;
            DaggerfallUnity.Settings.EnableInventoryInfoPanel = inventoryInfoPanel.IsChecked;
            DaggerfallUnity.Settings.EnableEnhancedItemLists = enhancedItemLists.IsChecked;
            DaggerfallUnity.Settings.EnableVitalsIndicators = vitalsIndicators.IsChecked;

            DaggerfallUnity.Settings.MouseLookSensitivity = mouseSensitivity.GetValue();
            DaggerfallUnity.Settings.WeaponSensitivity = weaponSensitivity.GetValue();
            DaggerfallUnity.Settings.MovementAcceleration = movementAcceleration.GetValue();
            float weaponAttackThresholdValue;
            if (float.TryParse(weaponAttackThreshold.Text, out weaponAttackThresholdValue))
                DaggerfallUnity.Settings.WeaponAttackThreshold = Mathf.Clamp(weaponAttackThresholdValue, 0.001f, 1.0f);

            DaggerfallUnity.Settings.LypyL_GameConsole = gameConsole.IsChecked;
            DaggerfallUnity.Settings.LypyL_ModSystem = modSystem.IsChecked;
            DaggerfallUnity.Settings.MeshAndTextureReplacement = assetImport.IsChecked;
            DaggerfallUnity.Settings.CompressModdedTextures = compressModdedTextures.IsChecked;
            DaggerfallUnity.Settings.NearDeathWarning = nearDeathWarning.IsChecked;
            DaggerfallUnity.Settings.AlternateRandomEnemySelection = alternateRandomEnemySelection.IsChecked;
            DaggerfallUnity.Settings.CameraRecoilStrength = cameraRecoilStrength.ScrollIndex;
            DaggerfallUnity.Settings.AdvancedClimbing = advancedClimbing.IsChecked;

            /* Video */

            if (applyScreenChanges)
            {
                Resolution selectedResolution = Screen.resolutions[resolution.ScrollIndex];
                DaggerfallUnity.Settings.ResolutionWidth = selectedResolution.width;
                DaggerfallUnity.Settings.ResolutionHeight = selectedResolution.height;
                DaggerfallUnity.Settings.Fullscreen = fullscreen.IsChecked;
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullscreen.IsChecked);

                DaggerfallUnity.Settings.QualityLevel = qualityLevel.ScrollIndex;
                QualitySettings.SetQualityLevel(qualityLevel.ScrollIndex);
            }
            DaggerfallUnity.Settings.MainFilterMode = mainFilterMode.ScrollIndex;
            DaggerfallUnity.Settings.GUIFilterMode = guiFilterMode.ScrollIndex;
            DaggerfallUnity.Settings.VideoFilterMode = videoFilterMode.ScrollIndex;

            DaggerfallUnity.Settings.FieldOfView = fovSlider.Value;
            DaggerfallUnity.Settings.TerrainDistance = terrainDistance.Value;
            DaggerfallUnity.Settings.ShadowResolutionMode = shadowResolutionMode.ScrollIndex;
            DaggerfallUnity.Settings.DungeonLightShadows = dungeonLightShadows.IsChecked;
            DaggerfallUnity.Settings.InteriorLightShadows = interiorLightShadows.IsChecked;
            DaggerfallUnity.Settings.UseLegacyDeferred = useLegacyDeferred.IsChecked;

            /* Theme */

            DaggerfallUnity.Settings.ToolTipDelayInSeconds = toolTipDelayInSeconds.GetValue();
            DaggerfallUnity.Settings.ToolTipTextColor = toolTipTextColor.BackgroundColor;
            DaggerfallUnity.Settings.ToolTipBackgroundColor = toolTipBackgroundColor.BackgroundColor;

            DaggerfallUnity.Settings.EnableModernConversationStyleInTalkWindow = enableModernConversationStyleInTalkWindow.IsChecked;
            DaggerfallUnity.Settings.InteractionModeIcon = ((InteractionModeIconModes)interactionModeIcon.Value).ToString();

            DaggerfallUnity.Settings.SaveSettings();
        }

        #endregion

        #region Panel Setup

        private void AddPage(string titleKey, Action<Panel, Panel> setup)
        {
            string title = GetText(titleKey);

            Panel panel = new Panel();
            panel.Name = title;
            panel.Outline.Enabled = true;
            panel.BackgroundColor = backgroundColor;
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.Position = new Vector2(0, topY + bar.Size.y);
            panel.Size = pageSize;

            pages.Add(panel);
            NativePanel.Components.Add(panel);

            if (pages.Count > 1)
                panel.Enabled = false;

            TextLabel textLabel = new TextLabel(titleFont);
            textLabel.Text = title;
            textLabel.Position = new Vector2(0, 2);
            textLabel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.Components.Add(textLabel);

            Button pageButton = new Button();
            pageButton.Name = title;
            pageButton.Size = new Vector2(50, 9);
            pageButton.HorizontalAlignment = HorizontalAlignment.None;
            pageButton.Position = new Vector2((pages.Count - 1) * 50, 0);
            pageButton.VerticalAlignment = VerticalAlignment.Middle;
            pageButton.BackgroundColor = Color.clear;
            pageButton.Outline.Enabled = false;
            pageButton.Label.Text = title;
            pageButton.Label.Font = pageButtonFont;
            pageButton.Label.TextColor = pages.Count > 1 ? pageButtonUnselected : pageButtonSelected;
            pageButton.Label.ShadowColor = Color.clear;
            pageButton.OnMouseClick += PageButton_OnMouseClick;
            bar.Components.Add(pageButton);
            pagesButton.Add(pageButton);

            Vector2 size = new Vector2(panel.Size.x / 2 - offset.x * 2, columnHeight);

            Panel leftPanel = new Panel();
            leftPanel.Outline.Enabled = false;
            leftPanel.Position = offset;
            leftPanel.Size = size;
            panel.Components.Add(leftPanel);

            Panel rightPanel = new Panel();
            rightPanel.Outline.Enabled = false;
            rightPanel.Position = new Vector2(panel.Size.x / 2 + offset.x, offset.y);
            rightPanel.Size = size;
            panel.Components.Add(rightPanel);

            y = 0;
            setup(leftPanel, rightPanel);
        }

        private void NextPage()
        {
            pages[currentPage].Enabled = false;
            pagesButton[currentPage].Label.TextColor = pageButtonUnselected;

            currentPage++;
            if (currentPage == pages.Count)
                currentPage = 0;

            pages[currentPage].Enabled = true;
            pagesButton[currentPage].Label.TextColor = pageButtonSelected;
        }

        private void SelectPage(string title)
        {
            pages[currentPage].Enabled = false;
            pagesButton[currentPage].Label.TextColor = pageButtonUnselected;

            for (currentPage = pages.Count - 1; currentPage >= 0; currentPage--)
                if (pages[currentPage].Name == title)
                    break;

            pages[currentPage].Enabled = true;
            pagesButton[currentPage].Label.TextColor = pageButtonSelected;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Add a section title.
        /// </summary>
        private TextLabel AddSectionTitle(Panel panel, string titleKey)
        {
            TextLabel textLabel = new TextLabel();
            textLabel.Text = GetText(titleKey);
            textLabel.Position = new Vector2(0, y);
            textLabel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.Components.Add(textLabel);
            y += sectionSpacing;

            return textLabel;
        }

        /// <summary>
        /// Add a text label.
        /// </summary>
        private TextLabel AddInfo(Panel panel, string text, string description)
        {
            TextLabel textLabel = new TextLabel();
            textLabel.Text = text;
            textLabel.Position = new Vector2(0, y);
            textLabel.HorizontalAlignment = HorizontalAlignment.Left;
            textLabel.ShadowColor = Color.clear;
            textLabel.TextColor = itemColor;
            textLabel.TextScale = itemTextScale;
            textLabel.ToolTip = defaultToolTip;
            textLabel.ToolTipText = description;
            panel.Components.Add(textLabel);
            y += itemSpacing;

            return textLabel;
        }

        private void AddLabel(Panel panel, string key)
        {
            TextLabel titleLabel = new TextLabel();
            titleLabel.Position = new Vector2(0, y);
            titleLabel.TextScale = itemTextScale;
            titleLabel.Text = GetText(key);
            titleLabel.TextColor = itemColor;
            titleLabel.ShadowColor = Color.clear;
            titleLabel.ToolTip = defaultToolTip;
            titleLabel.ToolTipText = GetInfo(key);
            panel.Components.Add(titleLabel);
        }

        /// <summary>
        /// Add a checkbox option.
        /// </summary>
        private Checkbox AddCheckbox(Panel panel, string key, bool isChecked)
        {
            Checkbox checkbox = DaggerfallUI.AddCheckbox(new Vector2(0, y), isChecked, panel);
            checkbox.Label.Text = GetText(key);
            checkbox.Label.TextColor = itemColor;
            checkbox.Label.TextScale = itemTextScale;
            checkbox.CheckBoxColor = itemColor;
            checkbox.ToolTip = defaultToolTip;
            checkbox.ToolTipText = GetInfo(key);
            y += itemSpacing;

            return checkbox;
        }

        /// <summary>
        /// Add a slider with a numerical indicator.
        /// </summary>
        private HorizontalSlider AddSlider(Panel panel, string key, int minValue, int maxValue, int startValue)
        {
            return AddSlider(panel, key, (x) => x.SetIndicator(minValue, maxValue, startValue));
        }

        /// <summary>
        /// Add a slider with a numerical indicator.
        /// </summary>
        private HorizontalSlider AddSlider(Panel panel, string key, float minValue, float maxValue, float startValue)
        {
            return AddSlider(panel, key, (x) => x.SetIndicator(minValue, maxValue, startValue));
        }

        /// <summary>
        /// Add a multiplechoices slider.
        /// </summary>
        private HorizontalSlider AddSlider(Panel panel, string key, int selected, params string[] choices)
        {
            LocalizeOptions(choices);
            return AddSlider(panel, key, (x) => x.SetIndicator(choices, selected));
        }

        private HorizontalSlider AddSlider(Panel panel, string key, Action<HorizontalSlider> setIndicator)
        {
            AddLabel(panel, key);
            y += 6;
            HorizontalSlider slider = DaggerfallUI.AddSlider(new Vector2(0, y), setIndicator, itemTextScale, panel);

            y += itemSpacing;
            return slider;
        }

        private TextBox AddTextbox(Panel panel, string key, string text)
        {
            AddLabel(panel, key);
            TextBox textBox = new TextBox();
            textBox.Position = new Vector2(0, y);
            textBox.HorizontalAlignment = HorizontalAlignment.Right;
            textBox.FixedSize = true;
            textBox.Size = new Vector2(30, 6);
            textBox.MaxCharacters = 5;
            textBox.Cursor.Enabled = false;
            textBox.DefaultText = text;
            textBox.DefaultTextColor = selectedTextColor;
            textBox.UseFocus = true;
            panel.Components.Add(textBox);

            y += itemSpacing;
            return textBox;
        }

        private Button AddColorPicker(Panel panel, string key, Color color)
        {
            AddLabel(panel, key);
            Button colorPicker = DaggerfallUI.AddColorPicker(new Vector2(panel.Size.x / 2, y), color, uiManager, this, panel);

            y += itemSpacing;
            return colorPicker;
        }

        private static string GetText(string key)
        {
            return TextManager.Instance.GetText(textTable, key);
        }

        private static string GetInfo(string key)
        {
            return GetText(string.Format("{0}Info", key));
        }

        private static void LocalizeOptions(string[] options)
        {
            // Use a safe approach for slider options, at least for the moment.
            // Many are based on enums and can be changed at any time.
            for (int i = 0; i < options.Length; i++)
            {
                if (TextManager.Instance.HasText(textTable, options[i]))
                    options[i] = TextManager.Instance.GetText(textTable, options[i]);
            }
        }

        #endregion

        #region Event Handlers

        private void PageButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectPage(sender.Name);
        }

        /// <summary>
        /// Save settings and close the window.
        /// </summary>
        private void CloseButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SaveSettings();
            DaggerfallUI.UIManager.PopWindow();
        }

        private void QualityLevel_OnScroll()
        {
            applyScreenChanges = true;
        }

        private void Fullscreen_OnToggleState()
        {
            applyScreenChanges = true;
        }

        private void Resolution_OnScroll()
        {
            applyScreenChanges = true;
        }

        #endregion
    }
}