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

        #region UI Controls

        // Panels
        List<Panel> pages = new List<Panel>();
        List<Button> pagesButton = new List<Button>();
        Panel bar = new Panel();
        int currentPage = 0;

        // Layout
        readonly Vector2 pageSize = new Vector2(318, 165);
        readonly Vector2 topBarSize = new Vector2(318, 10);
        const float topY = 8;
        const float offset = 10;

        // Colors
        Color backgroundColor           = new Color(0, 0, 0, 0.7f);
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

        // Settings
        Checkbox StartInDungeon;

        HorizontalSlider toolTips;
        Checkbox Crosshair;
        Checkbox inventoryInfoPanel;
        Checkbox enhancedItemLists;

        HorizontalSlider mouseSensitivitySlider;

        Checkbox DungeonLightShadows;
        Checkbox UseLegacyDeferred;

        HorizontalSlider fovSlider;
        HorizontalSlider terrainDistanceSlider;

        HorizontalSlider shadowResolutionMode;

        HorizontalSlider mainFilterMode;
        HorizontalSlider guiFilterMode;
        HorizontalSlider videoFilterMode;

        #endregion

        #region Fields

        // Section titles
        const string closeButtonText = "Close";
        const string gameplayOptionsText = "Gameplay";
        const string guiText = "GUI";
        const string controlsText = "Controls";
        const string infoText = "Info";
        const string filtersText = "Texture Filters";
        const string graphicQualityText = "Graphic Quality";

        // Position controls
        const float itemTextScale = 0.9f;
        const float sectionSpacing = 12f; // Space after title
        const float itemSpacing = 10f; // Space between items
        const float nextPanel = 20f; // Vertical offset for panels
        float y = 2f; // Current position

        #endregion

        #region Override Methods

        /// <summary>
        /// Setup Advanced Settings Panel
        /// </summary>
        protected override void Setup()
        {
            // Set background
            ParentPanel.BackgroundTexture = GetBackground(DaggerfallUnity.Settings.MainFilterMode);
            ParentPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;

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
            closeButton.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
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

        #region Settings

        private void LoadSettings()
        {
            AddPage("GamePlay", Gameplay);
            AddPage("Video", Video);
        }

        private void Gameplay(Panel leftPanel, Panel rightPanel)
        {
            // Gameplay
            AddSectionTitle(leftPanel, gameplayOptionsText);
            StartInDungeon = AddCheckbox(leftPanel, "Start In Dungeon", "Start new game inside the first dungeon", DaggerfallUnity.Settings.StartInDungeon);

            // GUI
            AddSectionTitle(leftPanel, guiText);
            int tootipMode = DaggerfallUnity.Settings.EnableToolTips ? DaggerfallUnity.Settings.HQTooltips ? 2 : 1 : 0;
            toolTips = AddSlider(leftPanel, "Tool Tips", "Shows informations about GUI items", tootipMode, "Off", "On", "HQ");
            Crosshair = AddCheckbox(leftPanel, "Crosshair", "Enable Crosshair on HUD", DaggerfallUnity.Settings.Crosshair);
            inventoryInfoPanel = AddCheckbox(leftPanel, "Inventory Info Panel", "Inventory Info Panel", DaggerfallUnity.Settings.EnableInventoryInfoPanel);
            enhancedItemLists = AddCheckbox(leftPanel, "Enhanced Item Lists", "Inventory 16x item grid", DaggerfallUnity.Settings.EnableEnhancedItemLists);

            // Controls
            AddSectionTitle(leftPanel, controlsText);

            // Mouse
            mouseSensitivitySlider = AddSlider(leftPanel, "Mouse Sensitivity", "Mouse look sensitivity.", 0.1f, 4.0f, DaggerfallUnity.Settings.MouseLookSensitivity);
        }

        private void Video(Panel leftPanel, Panel rightPanel)
        {
            // Info
            AddSectionTitle(leftPanel, infoText);
            AddInfo(leftPanel, "Quality Level: " + QualitySettings.names[DaggerfallUnity.Settings.QualityLevel], "General graphic quality");

            string textureArrayLabel = "Texture Arrays: ";
            if (!SystemInfo.supports2DArrayTextures)
                textureArrayLabel += "Unsupported";
            else
                textureArrayLabel += DaggerfallUnity.Settings.EnableTextureArrays ? "Enabled" : "Disabled";
            const string textureArrayToolTip = "Improved implementation of terrain textures, with better performance and modding support";
            TextLabel labelTextureArrayUsage = AddInfo(leftPanel, textureArrayLabel, textureArrayToolTip);

            // Filter modes
            AddSectionTitle(leftPanel, filtersText);
            string[] filterModes = new string[] { "Point", "Bilinear", "Trilinear" };
            mainFilterMode = AddSlider(leftPanel, "Main Filter", "Filter for game textures", DaggerfallUnity.Settings.MainFilterMode, filterModes);
            mainFilterMode.OnScroll += MainFilterMode_OnScroll;
            guiFilterMode = AddSlider(leftPanel, "GUI Filter", "Filter for HUD images", DaggerfallUnity.Settings.GUIFilterMode, filterModes);
            videoFilterMode = AddSlider(leftPanel, "Video Filter", "Filter for movies", DaggerfallUnity.Settings.VideoFilterMode, filterModes);

            y = nextPanel;

            // Graphic Quality
            AddSectionTitle(rightPanel, graphicQualityText);
            DungeonLightShadows = AddCheckbox(rightPanel, "Dungeon Light Shadows", "Dungeon lights cast shadows", DaggerfallUnity.Settings.DungeonLightShadows);
            UseLegacyDeferred = AddCheckbox(rightPanel, "Use Legacy Deferred", "Use Legacy Deferred", DaggerfallUnity.Settings.UseLegacyDeferred); //TODO: better description

            // FOV
            int fovStartValue = DaggerfallUnity.Settings.FieldOfView;
            fovSlider = AddSlider(rightPanel, "Field Of View", "The observable world that is seen at any given moment", 60, 80, fovStartValue);

            // Terrain distance
            int terraindDistanceStartValue = DaggerfallUnity.Settings.TerrainDistance;
            terrainDistanceSlider = AddSlider(rightPanel, "Terrain Distance", "Maximum distance of active terrains from player position",
                1, 4, terraindDistanceStartValue);

            // Shadow resolution
            shadowResolutionMode = AddSlider(rightPanel, "Shadow Resolution", "Shadow Resolution", //TODO: better description
                DaggerfallUnity.Settings.ShadowResolutionMode, "Low", "Medium", "High", "Very High");
        }

        private void SaveSettings()
        {
            DaggerfallUnity.Settings.StartInDungeon = StartInDungeon.IsChecked;

            DaggerfallUnity.Settings.EnableToolTips = toolTips.ScrollIndex != 0;
            DaggerfallUnity.Settings.HQTooltips = toolTips.ScrollIndex == 2;
            DaggerfallUnity.Settings.Crosshair = Crosshair.IsChecked;
            DaggerfallUnity.Settings.EnableInventoryInfoPanel = inventoryInfoPanel.IsChecked;
            DaggerfallUnity.Settings.EnableEnhancedItemLists = enhancedItemLists.IsChecked;

            DaggerfallUnity.Settings.MouseLookSensitivity = mouseSensitivitySlider.GetValue();

            DaggerfallUnity.Settings.DungeonLightShadows = DungeonLightShadows.IsChecked;
            DaggerfallUnity.Settings.UseLegacyDeferred = UseLegacyDeferred.IsChecked;

            DaggerfallUnity.Settings.FieldOfView = fovSlider.Value;
            DaggerfallUnity.Settings.TerrainDistance = terrainDistanceSlider.Value;

            DaggerfallUnity.Settings.ShadowResolutionMode = shadowResolutionMode.ScrollIndex;

            DaggerfallUnity.Settings.MainFilterMode = mainFilterMode.ScrollIndex;
            DaggerfallUnity.Settings.GUIFilterMode = guiFilterMode.ScrollIndex;
            DaggerfallUnity.Settings.VideoFilterMode = videoFilterMode.ScrollIndex;

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

            Vector2 size = new Vector2(panel.Size.x / 2 - offset * 2, 160);

            Panel leftPanel = new Panel();
            leftPanel.Outline.Enabled = false;
            leftPanel.Position = new Vector2(offset, 0);
            leftPanel.Size = size;
            panel.Components.Add(leftPanel);

            Panel rightPanel = new Panel();
            rightPanel.Outline.Enabled = false;
            rightPanel.Position = new Vector2(panel.Size.x / 2 + offset, 0);
            rightPanel.Size = size;
            panel.Components.Add(rightPanel);

            y = nextPanel;
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

            y += 8;

            // Slider
            var slider = new HorizontalSlider();
            slider.Position = new Vector2(0, y);
            slider.Size = new Vector2(80.0f, 5.0f);
            slider.DisplayUnits = 20;
            slider.BackgroundColor = sliderBackgroundColor;
            slider.TintColor = new Color(153, 153, 0);
            panel.Components.Add(slider);

            y += itemSpacing;
            return slider;
        }

        /// <summary>
        /// Load Background texture.
        /// </summary>
        /// <param name="filtermode">Filtermode index.</param>
        /// <returns></returns>
        private static Texture2D GetBackground(int filtermode)
        {
            Texture2D tex;

            switch (filtermode)
            {
                case 0:
                    tex = Resources.Load<Texture2D>("AdvancedSettings_Point");
                    break;
                case 1:
                    tex = Resources.Load<Texture2D>("AdvancedSettings_Bilinear");
                    break;
                case 2:
                    tex = Resources.Load<Texture2D>("AdvancedSettings_Trilinear");
                    break;
                default:
                    tex = Resources.Load<Texture2D>("AdvancedSettings_Point");
                    Debug.LogError("Advanced Settings Window: error in setting background");
                    break;
            }

            tex.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            return tex;
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

        /// <summary>
        /// Update background texture when filter mode is changed.
        /// </summary>
        private void MainFilterMode_OnScroll()
        {
            ParentPanel.BackgroundTexture = GetBackground(mainFilterMode.ScrollIndex);
        }

        #endregion
    }
}