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
using IniParser.Model;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility;
using System.IO;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// A graphical window to edit mod settings, wich are saved in a ini file.
    /// Supports applying presets created by modders and users,
    /// as well as restoring default values.
    /// </summary>
    public class ModSettingsWindow : DaggerfallPopupWindow
    {
        #region UI Controls

        Panel modSettingsPanel                      = new Panel();
        List<Panel> modSettingsPages                = new List<Panel>();
        Panel currentPanel;
        Paginator paginator;
        PresetPicker presetPicker;

        List<TextBox> modTextBoxes                  = new List<TextBox>();
        List<Checkbox> modCheckboxes                = new List<Checkbox>();
        List<Tuple<TextBox, TextBox>> modTuples     = new List<Tuple<TextBox, TextBox>>();
        List<HorizontalSlider> modSliders           = new List<HorizontalSlider>();
        List<Button> modColorPickers                = new List<Button>();

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

        readonly Mod mod;

        IniData data;
        ModSettingsData settings;
        List<Preset> presets = new List<Preset>();

        int x = startX;
        int y = startY;

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
        }

        #endregion

        #region Override Methods

        protected override void Setup()
        {
            // Get settings
            ModSettingsReader.GetSettings(mod, out data, out settings);
            presets = ModSettingsReader.GetPresets(mod);

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

        #region Load/Save Settings

        /// <summary>
        /// Load settings from IniData.
        /// </summary>
        private void LoadSettings()
        {
            foreach (var section in settings.VisibleSections)
            {
                // Add section title to window
                AddSectionTitle(section.Name);
                MovePosition(spacing);

                // Get section from collection
                SectionDataCollection sectionDataCollection = data.Sections;
                if (!sectionDataCollection.ContainsSection(section.Name))
                    sectionDataCollection.AddSection(section.Name);
                SectionData sectionData = sectionDataCollection.GetSectionData(section.Name);

                foreach (var key in section.Keys)
                {
                    // Get key from collection
                    KeyDataCollection keyDataCollection = sectionData.Keys;
                    if (!keyDataCollection.ContainsKey(key.Name))
                        keyDataCollection.AddKey(key.Name);
                    KeyData keyData = keyDataCollection.GetKeyData(key.Name);

                    // Add key to window with corrispective control
                    TextLabel settingName = AddKeyName(key.Name);
                    settingName.ToolTip = defaultToolTip;
                    settingName.ToolTipText = key.Description;

                    switch (key.KeyType)
                    {
                        case KeyType.Toggle:
                            var toggle = key as ToggleKey;
                            AddCheckBox(toggle.Deserialize(keyData.Value));
                            break;

                        case KeyType.MultipleChoice:
                            var mult = key as MultipleChoiceKey;
                            var multipleChoice = GetSlider();
                            multipleChoice.SetIndicator(mult.Options.ToArray(), mult.Deserialize(keyData.Value));
                            SetSliderIndicator(multipleChoice);
                            break;

                        case KeyType.SliderInt:
                            var intSliderKey = key as SliderIntKey;
                            var intSlider = GetSlider();
                            intSlider.SetIndicator(intSliderKey.Min, intSliderKey.Max, intSliderKey.Deserialize(keyData.Value));
                            SetSliderIndicator(intSlider);
                            break;

                        case KeyType.SliderFloat:
                            var floatSliderKey = key as SliderFloatKey;
                            var floatSlider = GetSlider();
                            floatSlider.SetIndicator(floatSliderKey.Min, floatSliderKey.Max, floatSliderKey.Deserialize(keyData.Value));
                            SetSliderIndicator(floatSlider);
                            break;

                        case KeyType.TupleInt:
                            var tuple = AddTuple(Key.SplitTuple(keyData.Value));
                            tuple.First.Numeric = tuple.Second.Numeric = true;
                            break;

                        case KeyType.TupleFloat:
                            AddTuple(Key.SplitTuple(keyData.Value)); // TextBox.Numeric doesn't allow dot
                            break;

                        case KeyType.Text:
                            TextBox textBox = GetTextbox(95, 40, keyData.Value);
                            modTextBoxes.Add(textBox);
                            break;

                        case KeyType.Color:
                            AddColorPicker((key as ColorKey).Deserialize(keyData.Value));
                            break;
                    }

                    MovePosition(spacing);
                }
            }
        }

        /// <summary>
        /// Save settings inside IniData.
        /// </summary>
        /// <param name="writeToDisk">Write settings to ini file on disk.</param>
        private void SaveSettings (bool writeToDisk = true)
        {
            // Set new values
            int checkBox = 0, textBox = 0, tuple = 0, slider = 0, colorPicker = 0;
            foreach (var section in settings.VisibleSections)
            {
                foreach (var key in section.Keys)
                {
                    switch (key.KeyType)
                    {
                        case KeyType.Toggle:
                            data[section.Name][key.Name] = modCheckboxes[checkBox].IsChecked.ToString();
                            checkBox++;
                            break;

                        case KeyType.MultipleChoice:
                        case KeyType.SliderInt:
                        case KeyType.SliderFloat:
                            data[section.Name][key.Name] = modSliders[slider].GetValue().ToString();
                            slider++;
                            break;

                        case KeyType.TupleInt:
                        case KeyType.TupleFloat:
                            string value = Key.FormatTuple(modTuples[tuple].First.ResultText, modTuples[tuple].First.ResultText);
                            data[section.Name][key.Name] = value;
                            tuple++;
                            break;

                        case KeyType.Text:
                            data[section.Name][key.Name] = modTextBoxes[textBox].ResultText;
                            textBox++;
                            break;

                        case KeyType.Color:
                            string hexColor = ColorUtility.ToHtmlStringRGBA(modColorPickers[colorPicker].BackgroundColor);
                            data[section.Name][key.Name] = hexColor;
                            colorPicker++;
                            break;
                    }
                }
            }

            // Save to file
            if (writeToDisk)
                ModSettingsReader.SaveSettings(mod, data);
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
            if (presets.Count > 0)
            {
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
            }

            // Create first page and load settings
            AddPage();
            currentPanel.Enabled = true;
            LoadSettings();
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

            modTextBoxes.Clear();
            modCheckboxes.Clear();
            modTuples.Clear();
            modSliders.Clear();
            modColorPickers.Clear();

            x = startX;
            y = startY;

            Init();
        }

        #endregion

        #region Helper Methods

        private void AddSectionTitle(string title)
        {
            Panel background = new Panel();
            background.Position = new Vector2(x, y - 0.5f);
            background.Size = new Vector2(140, 6.5f);
            background.BackgroundColor = backgroundTitleColor;
            background.Outline.Enabled = true;
            background.Outline.Sides = Sides.Bottom;
            background.Outline.Color = saveButtonColor;
            background.Outline.Thickness = 1;
            currentPanel.Components.Add(background);

            TextLabel textLabel = new TextLabel(DaggerfallUI.Instance.Font5);
            textLabel.Text = ModSettingsReader.FormattedName(title);
            textLabel.TextColor = sectionTitleColor;
            textLabel.ShadowColor = sectionTitleShadow;
            textLabel.TextScale = 0.9f;
            textLabel.Position = new Vector2(0, 0.5f);
            textLabel.HorizontalAlignment = HorizontalAlignment.Center;
            background.Components.Add(textLabel);
        }

        private TextLabel AddKeyName(string name)
        {
            TextLabel textLabel = new TextLabel();
            textLabel.Text = ModSettingsReader.FormattedName(name);
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

        /// <summary>
        /// Add a checkbox.
        /// </summary>
        /// <param name="isChecked">Should start checked?</param>
        private void AddCheckBox (bool isChecked)
        {
            Checkbox checkbox = new Checkbox()
            {
                Position = new Vector2(x + 95, y),
                Size = new Vector2(2, 2),
                CheckBoxColor = Color.white,
                IsChecked = isChecked
            };
            currentPanel.Components.Add(checkbox);
            modCheckboxes.Add(checkbox);
        }

        /// <summary>
        /// Get a TextBox.
        /// </summary>
        /// <param name="positionOffset">Offset for x position on window.</param>
        /// <param name="sizeLenght">X value of size.</param>
        /// <param name="text">Default value for this textbox.</param>
        /// <returns>TextBox on modSettingsPanel</returns>
        private TextBox GetTextbox(float positionOffset, float sizeLenght, string text)
        {
            TextBox textBox = new TextBox()
            {
                Position = new Vector2(x + positionOffset, y),
                AutoSize = AutoSizeModes.None,
                FixedSize = true,
                Size = new Vector2(sizeLenght, 6),
                Numeric = false,
                MaxCharacters = 20,
                DefaultText = text,
                UseFocus = true,
                HasFocusOutlineColor = Color.green,
                LostFocusOutlineColor = Color.white,
            };
            textBox.Outline.Enabled = true;
            currentPanel.Components.Add(textBox);
            return textBox;
        }

        /// <summary>
        /// Get a slider.
        /// </summary>
        private HorizontalSlider GetSlider()
        {
            MovePosition(6);

            var slider = new HorizontalSlider();
            slider.Position = new Vector2(x, y);
            slider.Size = new Vector2(80.0f, 4.0f);
            slider.DisplayUnits = 20;
            slider.BackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            slider.TintColor = new Color(153, 153, 0);
            modSliders.Add(slider);
            currentPanel.Components.Add(slider);
            return slider;
        }

        private void SetSliderIndicator(HorizontalSlider slider)
        {
            slider.IndicatorOffset = 15;
            slider.Indicator.TextScale = textScale;
            slider.Indicator.TextColor = Color.white;
            slider.Indicator.ShadowColor = Color.clear;
            slider.Indicator.HorizontalTextAlignment = TextLabel.HorizontalTextAlignmentSetting.Right;
        }

        private Tuple<TextBox, TextBox> AddTuple(Tuple<string, string> values)
        {
            var tuple = new Tuple<TextBox, TextBox>(
                GetTextbox(95, 19.6f, values.First),
                GetTextbox(116, 19.6f, values.Second));
            modTuples.Add(tuple);
            return tuple;
        }

        private void AddColorPicker(Color color)
        {
            Button colorPicker = new Button()
            {
                Position = new Vector2(x + 95, y),
                AutoSize = AutoSizeModes.None,
                Size = new Vector2(40, 6),
            };
            colorPicker.Outline.Enabled = true;
            colorPicker.BackgroundColor = color;
            colorPicker.OnMouseClick += ColorPicker_OnMouseClick;
            modColorPickers.Add(colorPicker);
            currentPanel.Components.Add(colorPicker);
        }

        private void AddPage()
        {
            Panel panel = new Panel()
            {
                BackgroundColor = Color.clear,
                Position = new Vector2(0, 0),
                Size = new Vector2(320, 175),
                Enabled = false
            };
            panel.Outline.Enabled = false;
            modSettingsPanel.Components.Add(panel);
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
                ModSettingsReader.ResetSettings(mod, ref data, settings);
                CloseWindow();
                RestartSettingsWindow();
            }
            else
                CloseWindow();
        }

        private void PresetButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            presetPicker = new PresetPicker(uiManager, this, data[ModSettingsReader.internalSection]["SettingsVersion"], presets);
            presetPicker.OnPresetPicked += PresetPicker_OnPresetPicked;
            presetPicker.OnCreatePreset += PresetPicker_OnCreatePreset;
            presetPicker.OnDeletePreset += PresetPicker_OnDeletePreset;
            uiManager.PushWindow(presetPicker); 
        }

        private void PresetPicker_OnPresetPicked(int index)
        {
            // Save current settings
            SaveSettings(false);

            // Confront current settings and preset
            foreach (var section in presets[index].Values)
                foreach (var key in section.Value)
                    data[section.Key][key.Key] = key.Value;

            // Apply changes
            RestartSettingsWindow();
        }

        private void PresetPicker_OnCreatePreset(Preset preset)
        {
            //SaveSettings(false);
            ModSettingsReader.CreatePreset(mod, data, preset);
            //presets = ModSettingsReader.GetPresets(mod);
        }

        private void PresetPicker_OnDeletePreset(Preset preset)
        {
            if (preset.HasPath && File.Exists(preset.DiskPath))
                File.Delete(preset.DiskPath);
        }

        private void ColorPicker_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            var colorPicker = new ColorPicker(uiManager, this, (Button)sender);
            uiManager.PushWindow(colorPicker);
        }

        #endregion
    }
}   