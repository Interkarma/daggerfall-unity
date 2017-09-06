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

using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

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
    Panel advancedSettingsPanel = new Panel();
    Panel leftPanel = new Panel();
    Panel centerPanel = new Panel();
    Panel rightPanel = new Panel();

    // Colors
    Color backgroundColor = new Color(0, 0, 0, 0.7f);
    Color unselectedTextColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    Color selectedTextColor = new Color(0.0f, 0.8f, 0.0f, 1.0f);
    Color listBoxBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
    Color scrollBarBackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);

    // Settings
    Checkbox StartInDungeon;

    Checkbox EnableToolTips;
    Checkbox HQTooltips;
    Checkbox Crosshair;

    HorizontalSlider mouseSensitivitySlider;
    TextLabel mouseSensitivityNumberLabel;

    Checkbox DungeonLightShadows;
    Checkbox UseLegacyDeferred;

    HorizontalSlider fovSlider;
    TextLabel fovNumberLabel;

    HorizontalSlider terrainDistanceSlider;
    TextLabel terrainDistanceNumberLabel;

    ListBox ShadowResolutionMode;

    ListBox MainFilterMode;
    ListBox GUIFilterMode;
    ListBox VideoFilterMode;    

    #endregion

    #region Fields

    // Labels
    const string mainTitleText = "Advanced Settings";
    const string closeButtonText = "Close";
    const string graphicQualityText = "Graphic Quality";
    const string guiText = "GUI";
    const string gameplayOptionsText = "Gameplay";
    const string infoText = "Info";
    const string controlsText = "Controls";
    const string filterModesText = "Filter Modes";

    // Position controls
    const float spacing = 12f; // Space between items
    const float nextPanel = 20f; // Vertical offset for panels
    float y = 2f; // Current position

    // Fov
    const int fovMin = 60;
    const int fovMax = 80;
    int fovLabel;

    // Mouse Sensitivity
    const int sensitivityMin = 1; /* 0.1 */
    const int sensitivityMax = 40; /* 4.0 */
    float sensitivityLabel;

    // Terrain Distance
    const int terrainDistanceMin = 1;
    const int terrainDistanceMax = 4;
    int terrainDistanceLabel;

    #endregion

    #region Panel Setup

    /// <summary>
    /// Setup Advanced Settings Panel
    /// </summary>
    protected override void Setup()
    {
        // Add advanced settings panel
        advancedSettingsPanel.Outline.Enabled = true;
        advancedSettingsPanel.BackgroundColor = backgroundColor;
        advancedSettingsPanel.HorizontalAlignment = HorizontalAlignment.Center;
        advancedSettingsPanel.Position = new Vector2(0, 8);
        advancedSettingsPanel.Size = new Vector2(318, 165);
        NativePanel.Components.Add(advancedSettingsPanel);

        // Add left Panel
        leftPanel.Outline.Enabled = false;
        leftPanel.Position = new Vector2(4, 0);
        leftPanel.Size = new Vector2(112, 160);
        advancedSettingsPanel.Components.Add(leftPanel);

        // Add centre Panel
        centerPanel.Outline.Enabled = false;
        centerPanel.Position = new Vector2(124, 0);
        centerPanel.Size = new Vector2(112, 160);
        advancedSettingsPanel.Components.Add(centerPanel);

        // Add right Panel
        rightPanel.Outline.Enabled = false;
        rightPanel.Position = new Vector2(244, 0);
        rightPanel.Size = new Vector2(70, 160);
        advancedSettingsPanel.Components.Add(rightPanel);

        // Set background
        ParentPanel.BackgroundTexture = GetBackground(DaggerfallUnity.Settings.MainFilterMode);
        ParentPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;

        // Add advanced settings title text
        TextLabel titleLabel = AddTextlabel (advancedSettingsPanel, mainTitleText);

        y = nextPanel;

        // Gameplay
        AddTextlabel(leftPanel, gameplayOptionsText);
        StartInDungeon = AddCheckbox(leftPanel, "Start In Dungeon", "Start new game inside the first dungeon", DaggerfallUnity.Settings.StartInDungeon);

        // GUI
        AddTextlabel(leftPanel, guiText);
        EnableToolTips = AddCheckbox(leftPanel, "Tool Tips", "Show description when mouse is over an item in GUI", DaggerfallUnity.Settings.EnableToolTips);
        HQTooltips = AddCheckbox(leftPanel, "HQ Tool Tips", "Use High Quality Font for Tool Tips", DaggerfallUnity.Settings.HQTooltips);
        if (!DaggerfallUnity.Settings.EnableToolTips)
            HQTooltips.IsChecked = false;
        Crosshair = AddCheckbox(leftPanel, "Crosshair", "Enable Crosshair on HUD", DaggerfallUnity.Settings.Crosshair);

        // Controls
        AddTextlabel(leftPanel, controlsText);

        // Mouse
        int sensitivityStartValue = (int)(DaggerfallUnity.Settings.MouseLookSensitivity * 10);
        AddSlider(leftPanel, sensitivityMin, sensitivityMax, sensitivityStartValue, "Mouse Sensitivity", "Mouse look sensitivity.", mouseSensitivityScroll_OnScroll, out mouseSensitivitySlider, out mouseSensitivityNumberLabel);

        // Info
        AddTextlabel(leftPanel, infoText);
        AddTextItem(leftPanel, "Quality Level: " + ((QualityLevel)DaggerfallUnity.Settings.QualityLevel).ToString(), "General graphic quality");

        string textureArrayLabel = "Texture Arrays: ";
        if (!SystemInfo.supports2DArrayTextures)
            textureArrayLabel += "Unsupported";
        else
            textureArrayLabel += DaggerfallUnity.Settings.EnableTextureArrays ? "Enabled" : "Disabled";
        const string textureArrayToolTip = "Improved implementation of terrain textures, with better performance and modding support";
        TextLabel labelTextureArrayUsage = AddTextItem(leftPanel, textureArrayLabel, textureArrayToolTip);

        y = nextPanel;

        // Graphic Quality
        AddTextlabel(centerPanel, graphicQualityText);
        DungeonLightShadows = AddCheckbox(centerPanel, "Dungeon Light Shadows", "Dungeon lights cast shadows", DaggerfallUnity.Settings.DungeonLightShadows);
        UseLegacyDeferred = AddCheckbox(centerPanel, "Use Legacy Deferred", "Use Legacy Deferred", DaggerfallUnity.Settings.UseLegacyDeferred);

        // FOV
        int fovStartValue = DaggerfallUnity.Settings.FieldOfView;
        const string fovToolTip = "The observable world that is seen at any given moment";
        AddSlider(centerPanel, fovMin, fovMax, fovStartValue, "Field Of View", fovToolTip, fovScroll_OnScroll, out fovSlider, out fovNumberLabel);

        // Terrain distance
        int terraindDistanceStartValue = DaggerfallUnity.Settings.TerrainDistance;
        const string terrainDistanceToolTiP = "Maximum distance of active terrains from player position";
        AddSlider(centerPanel, terrainDistanceMin, terrainDistanceMax, terraindDistanceStartValue, "Terrain Distance", terrainDistanceToolTiP, terrainDistanceScroll_OnScroll, out terrainDistanceSlider, out terrainDistanceNumberLabel);

        // Shadow resolution
        AddTextlabel(centerPanel, "Shadow Resolution");
        ShadowResolutionMode = AddListbox(centerPanel, ShadowResolutionModes(), DaggerfallUnity.Settings.ShadowResolutionMode);

        y = nextPanel;

        // Filter modes
        TextLabel filterModes = AddTextlabel (rightPanel, filterModesText);
        AddToolTipToTextLabel(filterModes, "Many users want Point filter with vanilla textures.");
        /*TextLabel mainFilterMode = */AddTextlabel (rightPanel, "Main Filter");
        MainFilterMode = AddListbox (rightPanel, FilterModes(), DaggerfallUnity.Settings.MainFilterMode);
        MainFilterMode.OnSelectItem += mainFilterMode_OnSelectItem;
        /*TextLabel guiFilterMode = */AddTextlabel (rightPanel, "GUI Filter");
        GUIFilterMode = AddListbox (rightPanel, FilterModes(), DaggerfallUnity.Settings.GUIFilterMode);
        /*TextLabel videoFilterMode = */AddTextlabel (rightPanel, "Video Filter");
        VideoFilterMode = AddListbox (rightPanel, FilterModes(), DaggerfallUnity.Settings.VideoFilterMode);

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

    #endregion

    #region Setup Methods

    /// <summary>
    /// Add a text label.
    /// </summary>
    /// <param name="panel">leftPanel, centerPanel or rightPanel.</param>
    /// <param name="text">Label.</param>
    /// <param name="alignment">Horizontal alignment.</param>
    TextLabel AddTextlabel (Panel panel, string text, HorizontalAlignment alignment = HorizontalAlignment.Center)
    {
        TextLabel textLabel = new TextLabel();
        textLabel.Text = text;
        textLabel.Position = new Vector2(0, y);
        textLabel.HorizontalAlignment = alignment;
        panel.Components.Add(textLabel);
        y += spacing;

        return textLabel;
    }

    /// <summary>
    /// Add a text label with child item settings.
    /// </summary>
    /// <param name="panel">leftPanel, centerPanel or rightPanel.</param>
    /// <param name="text">Label.</param>
    /// <returns></returns>
    TextLabel AddTextItem (Panel panel, string name, string description)
    {
        var textLabel = AddTextlabel(panel, name, HorizontalAlignment.Left);
        textLabel.ShadowColor = Color.clear;
        textLabel.TextColor = selectedTextColor;
        AddToolTipToTextLabel(textLabel, description);

        return textLabel;
    }

    /// <summary>
    /// Add ToolTip to a TextLabel
    /// </summary>
    /// <param name="textLabel">TextLabel object.</param>
    /// <param name="text">ToolTip text.</param>
    void AddToolTipToTextLabel(TextLabel textLabel, string text)
    {
        textLabel.ToolTip = defaultToolTip;
        textLabel.ToolTipText = text;
    }

    /// <summary>
    /// Add a checkbox option.
    /// </summary>
    /// <param name="panel">leftPanel, centerPanel or rightPanel.</param>
    /// <param name="text">Label.</param>
    /// <param name="tip">Description.</param>
    /// <param name="isChecked"></param>
    Checkbox AddCheckbox(Panel panel, string text, string tip, bool isChecked)
    {
        Checkbox checkbox = new Checkbox();
        checkbox.Label.Text = text;
        checkbox.Label.TextColor = selectedTextColor;
        checkbox.CheckBoxColor = selectedTextColor;
        checkbox.ToolTip = defaultToolTip;
        checkbox.ToolTipText = tip;
        checkbox.IsChecked = isChecked;
        checkbox.Position = new Vector2(0, y);
        panel.Components.Add(checkbox);
        y += spacing;

        return checkbox;
    }

    /// <summary>
    /// Add a list of options.
    /// </summary>
    /// <param name="panel">leftPanel, centerPanel or rightPanel.</param>
    /// <param name="Options">List of labels.</param>
    /// <param name="selected">Selected option.</param>
    ListBox AddListbox (Panel panel, List<string> Options, int selected)
    {
        int displayed = Options.Count;
        int spacing = 9 * displayed;

        ListBox listBox = new ListBox();
        listBox.BackgroundColor = listBoxBackgroundColor;
        listBox.TextColor = unselectedTextColor;
        listBox.SelectedTextColor = selectedTextColor;
        listBox.ShadowPosition = Vector2.zero;
        listBox.RowsDisplayed = displayed;
        listBox.RowAlignment = HorizontalAlignment.Center;
        listBox.HorizontalAlignment = HorizontalAlignment.Center;
        listBox.Position = new Vector2(0, y);
        listBox.Size = new Vector2(panel.Size.x, spacing);
        listBox.SelectedShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
        listBox.SelectedShadowColor = Color.black;
        panel.Components.Add(listBox);
        foreach (var option in Options)
        {
            listBox.AddItem(option);
        }
        listBox.SelectedIndex = selected;
        y += (spacing + 5);

        return listBox;
    }

    /// <summary>
    /// Add a slider with a numerical indicator.
    /// </summary>
    /// <param name="panel">leftPanel, centerPanel or rightPanel.</param>
    /// <param name="minValue">Minimum value on slider.</param>
    /// <param name="maxValue">Maximum value on slider.</param>
    /// <param name="startValue">Initial value on slider.</param>
    /// <param name="title">Descriptive name of settings.</param>
    /// <param name="toolTip">Description of setting.</param>
    /// <param name="updateIndicator">Action to execute on scroll.</param>
    /// <param name="slider">Slider bar.</param>
    /// <param name="indicator">Slider value indicator.</param>
    void AddSlider(
        Panel panel,
        int minValue,
        int maxValue,
        int startValue,
        string title,
        string toolTip,
        HorizontalSlider.OnScrollHandler updateIndicator,
        out HorizontalSlider slider,
        out TextLabel indicator)
    {
        // Title
        TextLabel titleLabel = new TextLabel();
        titleLabel.Position = new Vector2(0, y);
        titleLabel.Text = title;
        titleLabel.TextColor = selectedTextColor;
        titleLabel.ShadowColor = Color.clear;
        AddToolTipToTextLabel(titleLabel, toolTip);
        panel.Components.Add(titleLabel);

        y += 8;

        // Slider
        slider = new HorizontalSlider();
        slider.Position = new Vector2(0, y);
        slider.Size = new Vector2(80.0f, 5.0f);
        slider.DisplayUnits = 20;
        slider.TotalUnits = (maxValue - minValue) + 20;
        slider.ScrollIndex = startValue - minValue;
        slider.BackgroundColor = scrollBarBackgroundColor;
        slider.TintColor = new Color(153, 153, 0);
        slider.OnScroll += updateIndicator;
        panel.Components.Add(slider);

        // Indicator
        indicator = new TextLabel();
        indicator.Position = new Vector2(slider.Size.x + 15, y);
        updateIndicator();
        panel.Components.Add(indicator);

        y += spacing;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Save settings and close the window.
    /// </summary>
    private void CloseButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        DaggerfallUnity.Settings.StartInDungeon = StartInDungeon.IsChecked;

        DaggerfallUnity.Settings.EnableToolTips = EnableToolTips.IsChecked;
        DaggerfallUnity.Settings.HQTooltips = HQTooltips.IsChecked;
        DaggerfallUnity.Settings.Crosshair = Crosshair.IsChecked;

        DaggerfallUnity.Settings.MouseLookSensitivity = sensitivityLabel;

        DaggerfallUnity.Settings.DungeonLightShadows = DungeonLightShadows.IsChecked;
        DaggerfallUnity.Settings.UseLegacyDeferred = UseLegacyDeferred.IsChecked;

        DaggerfallUnity.Settings.FieldOfView = fovLabel;
        DaggerfallUnity.Settings.TerrainDistance = terrainDistanceLabel;

        DaggerfallUnity.Settings.ShadowResolutionMode = ShadowResolutionMode.SelectedIndex;

        DaggerfallUnity.Settings.MainFilterMode = MainFilterMode.SelectedIndex;
        DaggerfallUnity.Settings.GUIFilterMode = GUIFilterMode.SelectedIndex;
        DaggerfallUnity.Settings.VideoFilterMode = VideoFilterMode.SelectedIndex;

        DaggerfallUnity.Settings.SaveSettings();
        DaggerfallUI.UIManager.PopWindow();
    }

    /// <summary>
    /// Update background texture when filter mode is changed.
    /// </summary>
    private void mainFilterMode_OnSelectItem()
    {
        ParentPanel.BackgroundTexture = GetBackground(MainFilterMode.SelectedIndex);
    }

    /// <summary>
    /// Update label of FOV indicator.
    /// </summary>
    private void fovScroll_OnScroll()
    {
        fovLabel = fovSlider.ScrollIndex + fovMin;
        fovNumberLabel.Text = fovLabel.ToString();
    }

    /// <summary>
    /// Update label of Mouse sensitivity indicator.
    /// </summary>
    private void mouseSensitivityScroll_OnScroll()
    {
        sensitivityLabel = mouseSensitivitySlider.ScrollIndex + sensitivityMin;
        sensitivityLabel /= 10;
        mouseSensitivityNumberLabel.Text = sensitivityLabel.ToString();
    }

    /// <summary>
    /// Update label of Terrain distance indicator.
    /// </summary>
    private void terrainDistanceScroll_OnScroll()
    {
        terrainDistanceLabel = terrainDistanceSlider.ScrollIndex + terrainDistanceMin;
        terrainDistanceNumberLabel.Text = terrainDistanceLabel.ToString();
    }

    /// <summary>
    /// Load Background texture.
    /// </summary>
    /// <param name="filtermode">Filtermode index.</param>
    /// <returns></returns>
    static private Texture2D GetBackground (int filtermode)
    {
        Texture2D tex;

        switch(filtermode)
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

    #region Helpers

    /// <summary>
    /// Create a list of options for shadow resolution settings.
    /// </summary>
    static private List<string> ShadowResolutionModes()
    {
        return new List<string> { "Low", "Medium", "High", "Very High" };
    }

    /// <summary>
    /// Create a list of options for filtermode settings.
    /// </summary>
    static private List<string> FilterModes ()
    {
        return new List<string> {"Point", "Bilinear", "Trilinear"};
    }

    /// <summary>
    /// Quality Levels enum.
    /// </summary>
    private enum QualityLevel { Fastest, Fast, Simple, Good, Beautiful, Fantastic }; 

    #endregion
} 