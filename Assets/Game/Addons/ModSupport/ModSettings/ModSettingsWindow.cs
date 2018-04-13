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

using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// A graphical window to edit mod settings. Supports applying presets as well as creating new ones.
    /// </summary>
    public class ModSettingsWindow : DaggerfallPopupWindow
    {
        #region UI Controls

        Panel modSettingsPanel                      = new Panel();
        List<Panel> modSettingsPages                = new List<Panel>();
        Panel currentPanel;
        Paginator paginator;
        PresetPicker presetPicker;

        Dictionary<Key, BaseScreenComponent> UIControls = new Dictionary<Key, BaseScreenComponent>();

        #endregion

        #region Fields

        const int spacing = 8;
        const float textScale = 0.8f;
        const int startX = 10;
        const int startY = 15;

        const KeyCode nextPageKey = KeyCode.Tab;
        const KeyCode previousPageKey = KeyCode.LeftShift;

        Color panelBackgroundColor    = new Color(0, 0, 0, 0.7f);
        Color resetButtonColor        = new Color(1, 0, 0, 0.4f);           // red with alpha
        Color saveButtonColor         = new Color(0.0f, 0.5f, 0.0f, 0.4f);  // green with alpha
        Color cancelButtonColor       = new Color(0.2f, 0.2f, 0.2f, 0.4f);  // grey with alpha
        Color sectionTitleColor       = new Color(0.53f, 0.81f, 0.98f, 1);  // light blue
        Color sectionTitleShadow      = new Color(0.3f, 0.45f, 0.54f, 1);
        Color backgroundTitleColor    = new Color(0, 0.8f, 0, 0.1f);        // green
        Color backgroundTitleAdvColor = new Color(1, 0, 0, 0.1f);           // red

        readonly Mod mod;
        readonly ModSettingsData settings;  

        int x = startX;
        int y = startY;

        #endregion

        #region Properties

        internal IUserInterfaceManager UiManager
        {
            get { return uiManager; }
        }

        internal float TextScale
        {
            get { return textScale; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the mod settings window.
        /// </summary>
        /// <param name="mod">Mod whose values are to be exposed on screen.</param>
        public ModSettingsWindow(IUserInterfaceManager uiManager, Mod mod)
            : base(uiManager)
        {
            this.mod = mod;

            settings = ModSettingsData.Make(mod);
            settings.SaveDefaults();
            settings.LoadLocalValues();
        }

        #endregion

        #region Override Methods

        protected override void Setup()
        {
            // Setup base panel
            ParentPanel.BackgroundColor = Color.clear;
            modSettingsPanel.BackgroundColor = panelBackgroundColor;
            modSettingsPanel.Outline.Enabled = true;
            modSettingsPanel.HorizontalAlignment = HorizontalAlignment.Center;
            modSettingsPanel.Position = new Vector2(0, 8);
            modSettingsPanel.Size = new Vector2(320, 175);
            NativePanel.Components.Add(modSettingsPanel);

            // Initialize window
            Init();
        }

        public override void Update()
        {
            base.Update();

            if (modSettingsPages.Count > 1)
            {
                if (Input.GetKeyDown(nextPageKey))
                    paginator.Next();
                else if (Input.GetKeyDown(previousPageKey))
                    paginator.Previous();
            }
        }

        #endregion

        #region Panel Setup

        /// <summary>
        /// Add controls and load settings.
        /// </summary>
        private void Init()
        {
            // Title
            TextLabel titleLabel = new TextLabel();
            titleLabel.Text = mod.Title; // + " settings";
            titleLabel.Font = DaggerfallUI.Instance.Font2;
            titleLabel.TextScale = 0.75f;
            titleLabel.Position = new Vector2(0, 3);
            titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            modSettingsPanel.Components.Add(titleLabel);
            currentPanel = modSettingsPanel;

            // Reset button
            Button resetButton = GetButton("Reset", HorizontalAlignment.Left, resetButtonColor);
            resetButton.OnMouseClick += ResetButton_OnMouseClick;

            // Save button
            Button saveButton = GetButton("Save", HorizontalAlignment.Center, saveButtonColor);
            saveButton.OnMouseClick += SaveButton_OnMouseClick;

            // Cancel button
            Button cancelButton = GetButton("Cancel", HorizontalAlignment.Right, cancelButtonColor);
            cancelButton.OnMouseClick += CancelButton_OnMouseClick;

            // Presets button
            Button presetButton = new Button();
            presetButton.Size = new Vector2(35, 9);
            presetButton.Position = new Vector2(currentPanel.Size.x - 37, 2);
            presetButton.Label.Text = "Presets";
            presetButton.Label.Font = DaggerfallUI.Instance.Font3;
            presetButton.Label.TextScale = 0.8f;
            presetButton.Label.TextColor = sectionTitleColor;
            presetButton.Label.ShadowColor = sectionTitleShadow;
            presetButton.OnMouseClick += PresetButton_OnMouseClick;
            currentPanel.Components.Add(presetButton);

            // Create first page and load settings
            AddPage();
            currentPanel.Enabled = true;
            LoadSettings();
        }

        private void LoadSettings()
        {
            foreach (Section section in settings.Sections)
            {
                // Title
                AddSectionTitle(section.Name, section.IsAdvanced);
                MovePosition(spacing);

                // Description
                if (!string.IsNullOrEmpty(section.Description))
                    MovePosition(AddSectionDescription(section.Description) + 2);

                foreach (Key key in section.Keys)
                {
                    // Add key to window with corrispective control
                    TextLabel settingName = AddKeyName(key.Name);
                    settingName.ToolTip = defaultToolTip;
                    settingName.ToolTipText = key.Description;

                    int height = 6;
                    BaseScreenComponent control = key.OnWindow(this, x, y, ref height);
                    currentPanel.Components.Add(control);
                    UIControls.Add(key, control);

                    MovePosition(height + 2);
                }
            }
        }

        /// <summary>
        /// Save settings.
        /// </summary>
        /// <param name="writeToDisk">Write settings to file on disk.</param>
        private void SaveSettings(bool writeToDisk = true)
        {
            foreach (Section section in settings.Sections)
                foreach (Key key in section.Keys)
                    key.OnSaveWindow(UIControls[key]);

            // Save to file
            if (writeToDisk)
                settings.Save();
        }

        /// <summary>
        /// Controls page and column disposition.
        /// </summary>
        private void MovePosition(int space)
        {
            const int size = 160;

            y += space;

            if (y >= size)
            {
                if (x != startX)
                {
                    // Add another page
                    AddPage();
                    if (paginator == null)
                    {
                        paginator = new Paginator();
                        paginator.TextColor = Color.yellow;
                        paginator.ArrowColor = Color.green;
                        paginator.DisabledArrowColor = new Color(0.18f, 0.55f, 0.34f);
                        paginator.Position = new Vector2(startX + 3, 3.5f);
                        paginator.Size = new Vector2(35, 6);
                        paginator.OnSelected += Paginator_OnSelected;
                        modSettingsPanel.Components.Add(paginator);
                    }
                    paginator.Total = modSettingsPages.Count;

                    // Move to left column
                    y = startY;
                    x = startX;
                }
                else
                {
                    // Move to right column
                    y = startY;
                    x += 160;
                }
            }
        }

        /// <summary>
        /// Remove all components and initialize the window again.
        /// </summary>
        private void RestartSettingsWindow()
        {
            modSettingsPanel.Components.Clear();
            modSettingsPages.Clear();
            UIControls.Clear();

            x = startX;
            y = startY;

            Init();
        }

        #endregion

        #region Helper Methods

        private void AddSectionTitle(string title, bool isAdvanced)
        {
            Panel background = new Panel();
            background.Position = new Vector2(x, y - 0.5f);
            background.Size = new Vector2(140, 6.5f);
            background.BackgroundColor = isAdvanced ? backgroundTitleAdvColor : backgroundTitleColor;
            background.Outline.Enabled = true;
            background.Outline.Sides = Sides.Bottom;
            background.Outline.Color = isAdvanced ? resetButtonColor : saveButtonColor;
            background.Outline.Thickness = 1;
            currentPanel.Components.Add(background);

            TextLabel textLabel = new TextLabel(DaggerfallUI.Instance.Font5);
            textLabel.Text = ModSettingsData.FormattedName(title);
            textLabel.TextColor = sectionTitleColor;
            textLabel.ShadowColor = sectionTitleShadow;
            textLabel.TextScale = 0.9f;
            textLabel.Position = new Vector2(0, 0.5f);
            textLabel.HorizontalAlignment = HorizontalAlignment.Center;
            background.Components.Add(textLabel);
        }

        private int AddSectionDescription(string description)
        {
            const int width = 140;
            const int margin = 1;
            const int textMargin = 1;

            Vector2 position = new Vector2(x + textMargin + margin, y + textMargin);
            TextLabel textLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.Instance.Font4, position, description, currentPanel);
            textLabel.TextScale = 0.6f;
            textLabel.TextColor = sectionTitleColor;
            textLabel.ShadowColor = Color.clear;
            textLabel.WrapText = true;
            textLabel.WrapWords = true;
            textLabel.MaxWidth = width - (textMargin + margin) * 2;

            int height = textLabel.TextHeight + 2;
            Panel background = DaggerfallUI.AddPanel(new Rect(x + margin, y, width - margin * 2, height), currentPanel);
            background.BackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.1f);
            background.Outline.Enabled = true;
            background.Outline.Color = new Color(0.7f, 0.7f, 0.7f, 0.1f);
            background.Outline.Thickness = 1;
            return height;
        }

        private TextLabel AddKeyName(string name)
        {
            TextLabel textLabel = new TextLabel();
            textLabel.Text = ModSettingsData.FormattedName(name);
            textLabel.ShadowColor = Color.clear;
            textLabel.TextScale = textScale;
            textLabel.Position = new Vector2(x, y);
            textLabel.HorizontalAlignment = HorizontalAlignment.None;
            currentPanel.Components.Add(textLabel);
            return textLabel;
        }

        /// <summary>
        /// Get a button.
        /// </summary>
        /// <param name="label">Text on button.</param>
        /// <param name="alignment">Left, center or right.</param>
        /// <param name="backgroundColor">Color of button.</param>
        /// <returns>Button on modSettingsPanel</returns>
        private Button GetButton (string label, HorizontalAlignment alignment, Color backgroundColor)
        {
            Button button = new Button()
            {
                Size = new Vector2(30, 9),
                HorizontalAlignment = alignment,
                VerticalAlignment = VerticalAlignment.Bottom,
                BackgroundColor = backgroundColor,
            };
            button.Outline.Enabled = true;
            button.Label.Text = label;
            currentPanel.Components.Add(button);
            return button;
        }

        private void AddPage()
        {
            Panel panel = DaggerfallUI.AddPanel(new Rect(0, 0, 320, 175), modSettingsPanel);
            panel.BackgroundColor = Color.clear;
            panel.Outline.Enabled = false;
            panel.Enabled = false;
            modSettingsPages.Add(panel);
            currentPanel = panel;
        }

        #endregion

        #region Event Handlers

        private void Paginator_OnSelected(int previous, int selected)
        {
            modSettingsPages[previous].Enabled = false;
            modSettingsPages[selected].Enabled = true;
        }

        /// <summary>
        /// Save new settings and close the window.
        /// </summary>
        private void SaveButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SaveSettings();
            DaggerfallUI.UIManager.PopWindow();
        }

        /// <summary>
        /// Close the window without saving new settings.
        /// </summary>
        private void CancelButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.UIManager.PopWindow();
        }

        /// <summary>
        /// Ask confirmation for setting default values.
        /// </summary>
        private void ResetButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Open confirmation message box
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.SetText("Revert all settings to default values?");
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Cancel);
            messageBox.OnButtonClick += ConfirmReset_OnButtonClick;
            uiManager.PushWindow(messageBox);
        }

        /// <summary>
        /// Set default settings on confirmation.
        /// </summary>
        private void ConfirmReset_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                settings.RestoreDefaults();
                CloseWindow();
                RestartSettingsWindow();
            }
            else
                CloseWindow();
        }

        private void PresetButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!settings.HasLoadedPresets)
                settings.LoadPresets();

            presetPicker = new PresetPicker(uiManager, this, settings);
            presetPicker.ApplyChangesCallback = () => RestartSettingsWindow();
            uiManager.PushWindow(presetPicker); 
        }

        #endregion
    }
}   