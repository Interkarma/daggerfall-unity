// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
        #region UI Rects

        Rect pageRect                               = new Rect(0, 0, 320, 175);

        #endregion

        #region UI Controls

        Panel mainPanel                                     = new Panel();
        List<Panel> pages                                   = new List<Panel>();
        Dictionary<Key, BaseScreenComponent> uiControls     = new Dictionary<Key, BaseScreenComponent>();

        Paginator paginator;
        PresetPicker presetPicker;

        #endregion

        #region Fields

        const float textScale                       = 0.7f;
        const int startX                            = 10;
        const int startY                            = 15;
        const int columnWidth                       = 140;
        const int columnsOffset                     = columnWidth + startX * 2;

        const KeyCode nextPageKey                   = KeyCode.PageDown;
        const KeyCode previousPageKey               = KeyCode.PageUp;

        Color panelBackgroundColor                  = new Color(0, 0, 0, 0.7f);
        Color resetButtonColor                      = new Color(1, 0, 0, 0.4f);           // red with alpha
        Color saveButtonColor                       = new Color(0.0f, 0.5f, 0.0f, 0.4f);  // green with alpha
        Color cancelButtonColor                     = new Color(0.2f, 0.2f, 0.2f, 0.4f);  // grey with alpha
        Color sectionTitleColor                     = new Color(0.53f, 0.81f, 0.98f, 1);  // light blue
        Color sectionTitleShadow                    = new Color(0.3f, 0.45f, 0.54f, 1);
        Color sectionTitleAdvColor                  = new Color(1, 0, 0, 1);
        Color sectionTitleAdvShadow                 = new Color(0.5f, 0, 0, 1);
        Color sectionDescriptionBackgroundColor     = new Color(0.5f, 0.5f, 0.5f, 0.1f);
        Color sectionDescriptionOutlineColor        = new Color(0.7f, 0.7f, 0.7f, 0.1f);

        readonly Mod mod;
        readonly ModSettingsData settings;
        readonly bool liveChange;
        readonly int columnHeight;

        int x = startX;
        int y = startY;

        bool hasChangesFromPresets = false;

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

        internal int LineWidth
        {
            get { return columnWidth; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the mod settings window.
        /// </summary>
        /// <param name="mod">Mod whose values are to be exposed on screen.</param>
        /// <param name="liveChange">True if the game is already running.</param>
        public ModSettingsWindow(IUserInterfaceManager uiManager, Mod mod, bool liveChange = false)
            : base(uiManager)
        {
            this.mod = mod;
            this.liveChange = liveChange;

            // Make room for warning label about applying settings during runtime
            columnHeight = liveChange ? 155 : 165;

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
            mainPanel.BackgroundColor = panelBackgroundColor;
            mainPanel.Outline.Enabled = true;
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.Position = new Vector2(0, 8);
            mainPanel.Size = pageRect.size;
            NativePanel.Components.Add(mainPanel);

            // Title
            TextLabel titleLabel = new TextLabel();
            titleLabel.Text = mod.Title;
            titleLabel.Font = DaggerfallUI.Instance.Font2;
            titleLabel.TextScale = 0.75f;
            titleLabel.Position = new Vector2(0, 3);
            titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.Components.Add(titleLabel);

            if (liveChange)
            {
                // Add warning label that some settings may not be applied while game is running
                TextLabel warningLabel = new TextLabel(DaggerfallUI.DefaultFont);
                warningLabel.Text = TextManager.Instance.GetLocalizedText("settingsNotApplied");
                warningLabel.Position = new Vector2(0, columnHeight + 1);
                warningLabel.TextScale = 0.85f;
                warningLabel.HorizontalAlignment = HorizontalAlignment.Center;
                warningLabel.ShadowPosition = Vector2.zero;
                warningLabel.TextColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
                mainPanel.Components.Add(warningLabel);
            }

            // Reset button
            Button resetButton = GetButton(ModManager.GetText("reset"), HorizontalAlignment.Left, resetButtonColor);
            resetButton.OnMouseClick += ResetButton_OnMouseClick;

            // Save button
            Button saveButton = GetButton(ModManager.GetText("save"), HorizontalAlignment.Center, saveButtonColor);
            saveButton.OnMouseClick += SaveButton_OnMouseClick;

            // Cancel button
            Button cancelButton = GetButton(ModManager.GetText("cancel"), HorizontalAlignment.Right, cancelButtonColor);
            cancelButton.OnMouseClick += CancelButton_OnMouseClick;

            // Presets button
            Button presetButton = new Button();
            presetButton.Size = new Vector2(35, 9);
            presetButton.Position = new Vector2(mainPanel.Size.x - 37, 2);
            presetButton.BackgroundColor = saveButtonColor;
            presetButton.Label.Text = ModManager.GetText("presets");
            presetButton.Outline.Enabled = true;
            presetButton.OnMouseClick += PresetButton_OnMouseClick;
            mainPanel.Components.Add(presetButton);

            // Load settings
            LoadSettings();
        }

        public override void Update()
        {
            base.Update();

            if (paginator != null)
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
        /// Create controls for settings and load current values.
        /// </summary>
        private void LoadSettings()
        {
            // Init first page
            AddPage(true);

            foreach (Section section in settings.Sections)
            {
                // Title
                AddSectionTitleLabel(section);

                // Description
                if (!string.IsNullOrEmpty(section.Description))
                    AddSectionDescriptionBox(mod.TryLocalize("Settings", section.Name, "Description") ?? section.Description);

                // Add keys to window with corrispective controls
                foreach (Key key in section.Keys)
                {
                    int height = 6;
                    TextLabel keyLabel = GetKeyLabel(section, key, height);
                    BaseScreenComponent control = key.OnWindow(this, x, y, ref height);
                    uiControls.Add(key, control);
                    AddAtNextPosition(height, keyLabel, control);
                }
            }
        }

        /// <summary>
        /// Set controls values from settings.
        /// </summary>
        private void RefreshControls()
        {
            foreach (var uiControl in uiControls)
                uiControl.Key.OnRefreshWindow(uiControl.Value);

            hasChangesFromPresets = true;
        }

        /// <summary>
        /// Save settings.
        /// </summary>
        /// <param name="writeToDisk">Write settings to file on disk.</param>
        /// <param name="changedSettings">An hashet where names of changed settings are stored.</param>
        private void SaveSettings(bool writeToDisk, HashSet<string> changedSettings = null)
        {
            if (changedSettings != null)
            {
                foreach (Section section in settings.Sections)
                {
                    bool sectionHasChanged = false;

                    foreach (Key key in section.Keys)
                    {
                        bool hasChanged;
                        key.OnSaveWindow(uiControls[key], out hasChanged);
                        if (hasChanged)
                        {
                            changedSettings.Add(string.Format("{0}.{1}", section.Name, key.Name));
                            sectionHasChanged = true;
                        }
                    }

                    if (sectionHasChanged)
                        changedSettings.Add(section.Name);
                }
            }
            else
            {
                foreach (Section section in settings.Sections)
                    foreach (Key key in section.Keys)
                        key.OnSaveWindow(uiControls[key]);
            }

            // Save to file
            if (writeToDisk)
                settings.Save();
        }

        /// <summary>
        /// Adds components to current panel in the given order.
        /// Makes sure there is enough vertical space for all components. If not, go to next column.
        /// </summary>
        /// <param name="height">Total height of block.</param>
        /// <param name="components">Components to be added to colum in a single block.</param>
        private void AddAtNextPosition(int height, params BaseScreenComponent[] components)
        {
            const int finalSpacing = 2;         // Vertical space after block of components

            // Move to next column if there isn't enough space
            if (y + height >= columnHeight)
            {
                foreach (var component in components)
                {
                    Vector2 position = component.Position;
                    position.y += startY - y;
                    position.x += x == startX ? +columnsOffset : -columnsOffset;
                    component.Position = position;
                }

                if (x != startX)
                {
                    // Add another page
                    AddPage(false);
                    if (paginator == null)
                    {
                        paginator = new Paginator();
                        paginator.TextColor = Color.yellow;
                        paginator.ArrowColor = Color.green;
                        paginator.DisabledArrowColor = new Color(0.18f, 0.55f, 0.34f);
                        paginator.Position = new Vector2(startX + 3, 3.5f);
                        paginator.Size = new Vector2(35, 6);
                        paginator.OnSelected += Paginator_OnSelected;
                        mainPanel.Components.Add(paginator);
                    }
                    paginator.Total = pages.Count;

                    // Move to left column
                    y = startY;
                    x = startX;
                }
                else
                {
                    // Move to right column
                    y = startY;
                    x += columnsOffset;
                }
            }

            // Add components to last panel
            foreach (var component in components)
                pages[pages.Count - 1].Components.Add(component);

            // Insert space after components.
            // Avoid creating empty panel before is required
            y += Mathf.Min(height + finalSpacing, columnHeight - y - 1);
        }

        #endregion

        #region Helper Methods

        private void AddSectionTitleLabel(Section section)
        {
            Panel background = new Panel();
            background.Position = new Vector2(x, y - 0.5f);
            background.Size = new Vector2(columnWidth, 6.5f);
            AddAtNextPosition((int)background.Size.y, background);

            TextLabel textLabel = new TextLabel(DaggerfallUI.Instance.Font4);
            textLabel.Text = ModSettingsData.FormattedName(mod.TryLocalize("Settings", section.Name, "Name") ?? section.Name);
            textLabel.TextColor = section.IsAdvanced ? sectionTitleAdvColor : sectionTitleColor;
            textLabel.ShadowColor = section.IsAdvanced ? sectionTitleAdvShadow : sectionTitleShadow;
            textLabel.TextScale = 0.9f;
            textLabel.Position = new Vector2(0, 0.5f);
            textLabel.HorizontalAlignment = HorizontalAlignment.Center;
            background.Components.Add(textLabel);
        }

        private void AddSectionDescriptionBox(string description)
        {
            const int margin = 1;
            const int textMargin = 1;

            Vector2 position = new Vector2(0 + textMargin + margin, 0 + textMargin);
            TextLabel textLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.Instance.Font4, position, description);
            textLabel.TextScale = 0.5f;
            textLabel.TextColor = sectionTitleColor;
            textLabel.ShadowColor = Color.clear;
            textLabel.WrapText = true;
            textLabel.WrapWords = true;
            textLabel.MaxWidth = columnWidth - (textMargin + margin) * 2;

            int height = textLabel.TextHeight + 2;
            Panel background = DaggerfallUI.AddPanel(new Rect(x + margin, y, columnWidth - margin * 2, height));
            background.BackgroundColor = sectionDescriptionBackgroundColor;
            background.Outline.Enabled = true;
            background.Outline.Color = sectionDescriptionOutlineColor;
            background.Outline.Thickness = 1;
            background.Components.Add(textLabel);
            AddAtNextPosition(height, background);
        }

        private TextLabel GetKeyLabel(Section section, Key key, int height)
        {
            TextLabel textLabel = new TextLabel();
            textLabel.Text = ModSettingsData.FormattedName(mod.TryLocalize("Settings", section.Name, key.Name, "Name") ?? key.Name);
            textLabel.ShadowColor = Color.clear;
            textLabel.TextScale = textScale;
            textLabel.HorizontalAlignment = HorizontalAlignment.None;
            textLabel.Position = new Vector2(x, y + (float)(height - textLabel.TextHeight) / 2);         
            textLabel.ToolTip = defaultToolTip;
            textLabel.ToolTipText = mod.TryLocalize("Settings", section.Name, key.Name, "Description") ?? key.Description;
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
            mainPanel.Components.Add(button);
            return button;
        }

        /// <summary>
        /// Creates a new page.
        /// </summary>
        /// <param name="enabled">Start enabled?</param>
        private void AddPage(bool enabled)
        {
            Panel panel = DaggerfallUI.AddPanel(pageRect, mainPanel);
            panel.BackgroundColor = Color.clear;
            panel.Outline.Enabled = false;
            panel.Enabled = enabled;
            pages.Add(panel);
        }

        #endregion

        #region Event Handlers

        private void Paginator_OnSelected(int previous, int selected)
        {
            pages[previous].Enabled = false;
            pages[selected].Enabled = true;
        }

        /// <summary>
        /// Save new settings and close the window.
        /// </summary>
        private void SaveButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (liveChange)
            {
                if (hasChangesFromPresets)
                {
                    SaveSettings(true);
                    mod.LoadSettingsCallback(new ModSettings(mod, settings), new ModSettingsChange());
                }
                else
                {
                    var changedSettings = new HashSet<string>();
                    SaveSettings(true, changedSettings);

                    if (changedSettings.Count > 0)
                        mod.LoadSettingsCallback(new ModSettings(mod, settings), new ModSettingsChange(changedSettings));
                }
            }
            else
            {
                SaveSettings(true);
            }

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
            messageBox.SetText(ModManager.GetText("resetConfirmation"));
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
                RefreshControls();
            }

            CloseWindow();
        }

        private void PresetButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!settings.HasLoadedPresets)
                settings.LoadPresets();

            presetPicker = new PresetPicker(uiManager, mod, settings);
            presetPicker.ApplyChangesCallback = RefreshControls;
            uiManager.PushWindow(presetPicker);
        }

        #endregion
    }
}