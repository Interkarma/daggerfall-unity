// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
        Color unselectedTextColor       = new Color(0.6f, 0.6f, 0.6f, 1f);
        Color selectedTextColor         = new Color32(243, 239, 44, 255);
        Color listBoxBackgroundColor    = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        Color sliderBackgroundColor     = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        Color pageButtonSelected        = Color.white;
        Color pageButtonUnselected      = Color.gray;

        // Fonts
        DaggerfallFont titleFont        = DaggerfallUI.Instance.Font2;
        DaggerfallFont pageButtonFont   = DaggerfallUI.Instance.Font3;

        int currentPage = 0;
        float y = 0;

        #endregion

        #region Settings Controls

        // GamePlay
        Checkbox startInDungeon;
        HorizontalSlider randomDungeonTextures;
        Checkbox crosshair;
        HorizontalSlider toolTips;
        HorizontalSlider helmAndShieldMaterialDisplay;
        Checkbox inventoryInfoPanel;
        Checkbox enhancedItemLists;
        HorizontalSlider mouseSensitivity;
        HorizontalSlider weaponSensitivity;
        TextBox weaponAttackThreshold;
        Checkbox gameConsole;
        Checkbox modSystem;
        Checkbox assetImport;

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
        Checkbox useLegacyDeferred;

        #endregion

        #region Override Methods

        /// <summary>
        /// Setup Advanced Settings Panel
        /// </summary>
        protected override void Setup()
        {
            // Set background
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
            AddPage("GamePlay", Gameplay);
            AddPage("Video", Video);
        }

        private void Gameplay(Panel leftPanel, Panel rightPanel)
        {
            // Game
            AddSectionTitle(leftPanel, "Game");
            startInDungeon = AddCheckbox(leftPanel, "Start In Dungeon", "Start new game inside the first dungeon", DaggerfallUnity.Settings.StartInDungeon);
            randomDungeonTextures = AddSlider(leftPanel, "Dungeon Textures", "Generates dungeon texture table from random seed",
                DaggerfallUnity.Settings.RandomDungeonTextures, "classic", "climate", "climateOnly", "random", "randomOnly");

            // GUI
            AddSectionTitle(leftPanel, "GUI");
            crosshair = AddCheckbox(leftPanel, "Crosshair", "Enable Crosshair on HUD", DaggerfallUnity.Settings.Crosshair);
            int tootipMode = DaggerfallUnity.Settings.EnableToolTips ? DaggerfallUnity.Settings.HQTooltips ? 2 : 1 : 0;
            toolTips = AddSlider(leftPanel, "Tool Tips", "Shows informations about GUI items", tootipMode, "Off", "On", "HQ");
            helmAndShieldMaterialDisplay = AddSlider(leftPanel, "Helm and Shield Material Display", "Show material for helms and shields in info popups and tooltips",
                DaggerfallUnity.Settings.HelmAndShieldMaterialDisplay, "off", "noLeatChai", "noLeat", "on");
            inventoryInfoPanel = AddCheckbox(leftPanel, "Inventory Info Panel", "Inventory Info Panel", DaggerfallUnity.Settings.EnableInventoryInfoPanel); //TODO: better description
            enhancedItemLists = AddCheckbox(leftPanel, "Enhanced Item Lists", "Inventory 16x item grid", DaggerfallUnity.Settings.EnableEnhancedItemLists);

            y = 0;

            // Controls
            AddSectionTitle(rightPanel, "Controls");
            mouseSensitivity = AddSlider(rightPanel, "Mouse Sensitivity", "Mouse look sensitivity.", 0.1f, 4.0f, DaggerfallUnity.Settings.MouseLookSensitivity);
            weaponSensitivity = AddSlider(rightPanel, "Weapon Sensitivity", "Sensitivity of weapon swings to mouse movements", 0.1f, 10.0f, DaggerfallUnity.Settings.WeaponSensitivity);
            weaponAttackThreshold = AddTextbox(rightPanel, "WeaponAttackThreshold", "Minimum mouse gesture travel distance for an attack", DaggerfallUnity.Settings.WeaponAttackThreshold.ToString());

            // Enhancements
            AddSectionTitle(rightPanel, "Enhancements");
            gameConsole = AddCheckbox(rightPanel, "Game Console", "Enable input for console commands", DaggerfallUnity.Settings.LypyL_GameConsole);
            modSystem = AddCheckbox(rightPanel, "Mod System", "Enable support for mods.", DaggerfallUnity.Settings.LypyL_ModSystem);
            assetImport = AddCheckbox(rightPanel, "Allow Custom Assets", "Import assets from enabled mods and loose files.", DaggerfallUnity.Settings.MeshAndTextureReplacement);
        }

        private void Video(Panel leftPanel, Panel rightPanel)
        {
            // Basic settings
            AddSectionTitle(leftPanel, "Basic");
            resolution = AddSlider(leftPanel, "Resolution", "Screen resolution",
                Array.FindIndex(Screen.resolutions, x => x.width == DaggerfallUnity.Settings.ResolutionWidth && x.height == DaggerfallUnity.Settings.ResolutionHeight),
                Screen.resolutions.Select(x => string.Format("{0}x{1}", x.width, x.height)).ToArray());
            fullscreen = AddCheckbox(leftPanel, "Fullscreen", "Enable fullscreen", DaggerfallUnity.Settings.Fullscreen);
            qualityLevel = AddSlider(leftPanel, "Quality Level", "General graphic quality", DaggerfallUnity.Settings.QualityLevel, QualitySettings.names);
            string[] filterModes = new string[] { "Point", "Bilinear", "Trilinear" };
            mainFilterMode = AddSlider(leftPanel, "Main Filter", "Filter for game textures", DaggerfallUnity.Settings.MainFilterMode, filterModes);
            guiFilterMode = AddSlider(leftPanel, "GUI Filter", "Filter for HUD images", DaggerfallUnity.Settings.GUIFilterMode, filterModes);
            videoFilterMode = AddSlider(leftPanel, "Video Filter", "Filter for movies", DaggerfallUnity.Settings.VideoFilterMode, filterModes);

            y = 0;

            // Advanced settings
            AddSectionTitle(rightPanel, "Advanced");
            fovSlider = AddSlider(rightPanel, "Field Of View", "The observable world that is seen at any given moment",
                60, 80, DaggerfallUnity.Settings.FieldOfView);
            terrainDistance = AddSlider(rightPanel, "Terrain Distance", "Maximum distance of active terrains from player position",
                1, 4, DaggerfallUnity.Settings.TerrainDistance);
            shadowResolutionMode = AddSlider(rightPanel, "Shadow Resolution", "Quality of shadows",
                DaggerfallUnity.Settings.ShadowResolutionMode, "Low", "Medium", "High", "Very High");
            dungeonLightShadows = AddCheckbox(rightPanel, "Dungeon Light Shadows", "Dungeon lights cast shadows", DaggerfallUnity.Settings.DungeonLightShadows);
            useLegacyDeferred = AddCheckbox(rightPanel, "Use Legacy Deferred", "Legacy rendering path", DaggerfallUnity.Settings.UseLegacyDeferred);
            string textureArrayLabel = "Texture Arrays: ";
            if (!SystemInfo.supports2DArrayTextures)
                textureArrayLabel += "Unsupported";
            else
                textureArrayLabel += DaggerfallUnity.Settings.EnableTextureArrays ? "Enabled" : "Disabled";
            AddInfo(rightPanel, textureArrayLabel, "Improved implementation of terrain textures, with better performance and modding support");
        }

        private void SaveSettings()
        {
            /* GamePlay */

            DaggerfallUnity.Settings.StartInDungeon = startInDungeon.IsChecked;
            DaggerfallUnity.Settings.RandomDungeonTextures = randomDungeonTextures.ScrollIndex;

            DaggerfallUnity.Settings.Crosshair = crosshair.IsChecked;
            DaggerfallUnity.Settings.EnableToolTips = toolTips.ScrollIndex != 0;
            DaggerfallUnity.Settings.HQTooltips = toolTips.ScrollIndex == 2;
            DaggerfallUnity.Settings.HelmAndShieldMaterialDisplay = helmAndShieldMaterialDisplay.ScrollIndex;
            DaggerfallUnity.Settings.EnableInventoryInfoPanel = inventoryInfoPanel.IsChecked;
            DaggerfallUnity.Settings.EnableEnhancedItemLists = enhancedItemLists.IsChecked;

            DaggerfallUnity.Settings.MouseLookSensitivity = mouseSensitivity.GetValue();
            DaggerfallUnity.Settings.WeaponSensitivity = weaponSensitivity.GetValue();
            float weaponAttackThresholdValue;
            if (float.TryParse(weaponAttackThreshold.Text, out weaponAttackThresholdValue))
                DaggerfallUnity.Settings.WeaponAttackThreshold = Mathf.Clamp(weaponAttackThresholdValue, 0.001f, 1.0f);

            DaggerfallUnity.Settings.LypyL_GameConsole = gameConsole.IsChecked;
            DaggerfallUnity.Settings.LypyL_ModSystem = modSystem.IsChecked;
            DaggerfallUnity.Settings.MeshAndTextureReplacement = assetImport.IsChecked;

            /* Video */

            Resolution selectedResolution = Screen.resolutions[resolution.ScrollIndex];
            DaggerfallUnity.Settings.ResolutionWidth = selectedResolution.width;
            DaggerfallUnity.Settings.ResolutionHeight = selectedResolution.height;
            DaggerfallUnity.Settings.Fullscreen = fullscreen.IsChecked;
            DaggerfallUnity.Settings.QualityLevel = qualityLevel.ScrollIndex;
            DaggerfallUnity.Settings.MainFilterMode = mainFilterMode.ScrollIndex;
            DaggerfallUnity.Settings.GUIFilterMode = guiFilterMode.ScrollIndex;
            DaggerfallUnity.Settings.VideoFilterMode = videoFilterMode.ScrollIndex;

            DaggerfallUnity.Settings.FieldOfView = fovSlider.Value;
            DaggerfallUnity.Settings.TerrainDistance = terrainDistance.Value;
            DaggerfallUnity.Settings.ShadowResolutionMode = shadowResolutionMode.ScrollIndex;
            DaggerfallUnity.Settings.DungeonLightShadows = dungeonLightShadows.IsChecked;
            DaggerfallUnity.Settings.UseLegacyDeferred = useLegacyDeferred.IsChecked;


            DaggerfallUnity.Settings.SaveSettings();
        }

        #endregion

        #region Panel Setup

        private void AddPage(string title, Action<Panel, Panel> setup)
        {
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
            pageButton.Label.Font = DaggerfallUI.Instance.Font3;
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
        private TextLabel AddSectionTitle(Panel panel, string title)
        {
            TextLabel textLabel = new TextLabel();
            textLabel.Text = title;
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

        /// <summary>
        /// Add a checkbox option.
        /// </summary>
        private Checkbox AddCheckbox(Panel panel, string text, string tip, bool isChecked)
        {
            Checkbox checkbox = new Checkbox();
            checkbox.Label.Text = text;
            checkbox.Label.TextColor = itemColor;
            checkbox.Label.TextScale = itemTextScale;
            checkbox.CheckBoxColor = itemColor;
            checkbox.ToolTip = defaultToolTip;
            checkbox.ToolTipText = tip;
            checkbox.IsChecked = isChecked;
            checkbox.Position = new Vector2(0, y);
            panel.Components.Add(checkbox);
            y += itemSpacing;

            return checkbox;
        }

        /// <summary>
        /// Add a slider with a numerical indicator.
        /// </summary>
        private HorizontalSlider AddSlider(Panel panel, string title, string toolTip, int minValue, int maxValue, int startValue)
        {
            var slider = GetBaseSlider(panel, title, toolTip);
            slider.SetIndicator(minValue, maxValue, startValue);
            slider.IndicatorOffset = 15;
            slider.Indicator.ShadowColor = Color.clear;
            return slider;
        }

        /// <summary>
        /// Add a slider with a numerical indicator.
        /// </summary>
        private HorizontalSlider AddSlider(Panel panel, string title, string toolTip, float minValue, float maxValue, float startValue)
        {
            var slider = GetBaseSlider(panel, title, toolTip);
            slider.SetIndicator(minValue, maxValue, startValue);
            slider.IndicatorOffset = 15;
            slider.Indicator.ShadowColor = Color.clear;
            return slider;
        }

        /// <summary>
        /// Add a multiplechoices slider.
        /// </summary>
        private HorizontalSlider AddSlider(Panel panel, string title, string toolTip, int selected, params string[] choices)
        {
            var slider = GetBaseSlider(panel, title, toolTip);
            slider.SetIndicator(choices, selected);
            slider.IndicatorOffset = 15;
            slider.Indicator.ShadowColor = Color.clear;
            return slider;
        }

        private HorizontalSlider GetBaseSlider(Panel panel, string title, string toolTip)
        {
            // Title
            TextLabel titleLabel = new TextLabel();
            titleLabel.Position = new Vector2(0, y);
            titleLabel.TextScale = itemTextScale;
            titleLabel.Text = title;
            titleLabel.TextColor = itemColor;
            titleLabel.ShadowColor = Color.clear;
            titleLabel.ToolTip = defaultToolTip;
            titleLabel.ToolTipText = toolTip;
            panel.Components.Add(titleLabel);

            y += 6;

            // Slider
            var slider = new HorizontalSlider();
            slider.Position = new Vector2(0, y);
            slider.Size = new Vector2(80.0f, 4.0f);
            slider.DisplayUnits = 20;
            slider.BackgroundColor = sliderBackgroundColor;
            slider.TintColor = new Color(153, 153, 0);
            panel.Components.Add(slider);

            y += itemSpacing;
            return slider;
        }

        private TextBox AddTextbox(Panel panel, string title, string toolTip, string text)
        {
            // Title
            TextLabel titleLabel = new TextLabel();
            titleLabel.Position = new Vector2(0, y);
            titleLabel.TextScale = itemTextScale;
            titleLabel.Text = title;
            titleLabel.TextColor = itemColor;
            titleLabel.ShadowColor = Color.clear;
            titleLabel.ToolTip = defaultToolTip;
            titleLabel.ToolTipText = toolTip;
            panel.Components.Add(titleLabel);

            // TextBox
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

        #endregion
    }
}