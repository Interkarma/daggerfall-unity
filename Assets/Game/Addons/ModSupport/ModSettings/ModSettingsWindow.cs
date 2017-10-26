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

using System.Linq;
using System.Collections.Generic;
using IniParser.Model;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// A graphical window to edit mod settings, wich are saved in a ini file.
    /// Supports applying presets created by modders and users,
    /// as well as restoring default values.
    /// </summary>
    public class ModSettingsWindow : DaggerfallPopupWindow
    {
        #region Structs

        struct SliderSetup
        {
            public HorizontalSlider slider;
            public TextLabel indicator;
            public int lowerValue;
            public bool isFloat;
            public string[] items;

            public string IndicatorText
            {
                get
                {
                    int value = slider.ScrollIndex + lowerValue;
                    if (items != null && items.Length > value)
                        return items[slider.ScrollIndex];
                    else if (isFloat)
                        return (value / 10f).ToString();
                    else
                        return value.ToString();
                }
            }

            public string KeyValue
            {
                get
                {
                    if (items != null)
                        return slider.ScrollIndex.ToString();
                    return indicator.Text;
                }
            }
        }

        #endregion

        #region Fields

        readonly Mod mod;

        // UI Controls
        Panel modSettingsPanel = new Panel();
        List<Panel> modSettingsPages = new List<Panel>();
        Panel currentPanel;
        int currentPage = 0;
        Button nextPageButton;
        DaggerfallListPickerWindow presetPicker;
        const int spacing = 8;
        const int startX = 10;
        const int startY = 15;
        int x = startX;
        int y = startY;

        // Settings
        IniData data;
        IniData defaultSettings;
        ModSettingsConfiguration config;
        List<IniData> presets = new List<IniData>();
        int currentPresetIndex;

        // GUI elements
        List<TextBox> modTextBoxes              = new List<TextBox>();
        List<Checkbox> modCheckboxes            = new List<Checkbox>();
        List<Tuple<TextBox, TextBox>> modTuples = new List<Tuple<TextBox, TextBox>>();
        List<SliderSetup> modSliders            = new List<SliderSetup>();

        // Colors
        Color panelBackgroundColor    = new Color(0, 0, 0, 0.7f);
        Color resetButtonColor        = new Color(1, 0, 0, 0.4f); //red with alpha
        Color saveButtonColor         = new Color(0.0f, 0.5f, 0.0f, 0.4f); //green with alpha
        Color cancelButtonColor       = new Color(0.2f, 0.2f, 0.2f, 0.4f); //grey with alpha
        Color sectionTitleColor       = new Color(0.16f, 0.26f, 1, 1); // light blue

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
            ModSettingsReader.GetSettings(mod, out data, out defaultSettings);
            config = ModSettingsReader.GetConfig(mod);
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

            if (Input.GetKeyDown(KeyCode.Tab) && modSettingsPages.Count > 1)
                NextPage();
        }

        #endregion

        #region Load/Save Settings

        /// <summary>
        /// Load settings from IniData.
        /// </summary>
        private void LoadSettings ()
        {
            // Read settings
            foreach (SectionData section in data.Sections.Where(x => x.SectionName != ModSettingsReader.internalSection))
            {
                // Section label
                TextLabel textLabel = new TextLabel();
                textLabel.Text = section.SectionName;
                textLabel.TextColor = sectionTitleColor;
                textLabel.Position = new Vector2(x, y);
                textLabel.HorizontalAlignment = HorizontalAlignment.None;
                currentPanel.Components.Add(textLabel);
                MovePosition(spacing + 4);
                List<string> comments = section.Comments;
                int comment = 0;

                foreach (KeyData key in section.Keys)
                {
                    // Setting label
                    TextLabel settingName = new TextLabel();
                    settingName.Text = key.KeyName;
                    settingName.Position = new Vector2(x, y);
                    settingName.HorizontalAlignment = HorizontalAlignment.None;
                    currentPanel.Components.Add(settingName);

                    // Setting field
                    ModSettingsKey configKey;
                    if (config && config.Key(section.SectionName, key.KeyName, out configKey))
                    {
                        settingName.ToolTip = defaultToolTip;
                        settingName.ToolTipText = configKey.description;

                        // Use config file
                        switch (configKey.type)
                        {
                            case ModSettingsKey.KeyType.Toggle:
                                bool toggle;
                                AddCheckBox((bool.TryParse(key.Value, out toggle) && toggle) || configKey.toggle.value);
                                break;

                            case ModSettingsKey.KeyType.MultipleChoice:
                                int selected;
                                if (!int.TryParse(key.Value, out selected))
                                    selected = configKey.multipleChoice.selected;
                                AddSlider(configKey.multipleChoice.choices, selected, key.KeyName);
                                break;

                            case ModSettingsKey.KeyType.Slider:
                                var slider = configKey.slider;
                                int startValue;
                                if (!int.TryParse(key.Value, out startValue))
                                    startValue = configKey.slider.value;
                                AddSlider(slider.min, slider.max, startValue, key.KeyName);
                                break;

                            case ModSettingsKey.KeyType.FloatSlider:
                                var floatSlider = configKey.floatSlider;
                                float floatStartValue;
                                if (!float.TryParse(key.Value, out floatStartValue))
                                    floatStartValue = configKey.floatSlider.value;
                                AddSlider(floatSlider.min, floatSlider.max, floatStartValue, key.KeyName);
                                break;

                            case ModSettingsKey.KeyType.Tuple:
                                var tuple = AddTuple(key.Value);
                                tuple.First.Numeric = tuple.Second.Numeric = true;
                                break;

                            case ModSettingsKey.KeyType.FloatTuple:
                                AddTuple(key.Value); // TextBox.Numeric doesn't allow dot
                                break;

                            case ModSettingsKey.KeyType.Text:
                                TextBox textBox = GetTextbox(95, 40, key.Value);
                                modTextBoxes.Add(textBox);
                                break;

                            case ModSettingsKey.KeyType.Color:
                                TextBox colorBox = GetTextbox(95, 40, key.Value);
                                if (!ModSettingsReader.IsHexColor(colorBox.DefaultText))
                                    colorBox.DefaultText = configKey.color.HexColor;
                                SetBackgroundTint(colorBox);
                                modTextBoxes.Add(colorBox);
                                break;
                        }
                    }
                    else
                    {
                        // Legacy support
                        if (comment < comments.Count)
                        {
                            settingName.ToolTip = defaultToolTip;
                            settingName.ToolTipText = comments[comment];
                            comment++;
                        }

                        if (key.Value == "True")
                            AddCheckBox(true);
                        else if (key.Value == "False")
                            AddCheckBox(false);
                        else if (key.Value.Contains(ModSettingsReader.tupleDelimiterChar))
                            AddTuple(key.Value);
                        else
                        {
                            TextBox textBox = GetTextbox(95, 40, key.Value);
                            modTextBoxes.Add(textBox);

                            if (ModSettingsReader.IsHexColor(textBox.DefaultText))
                                    SetBackgroundTint(textBox);
                        }
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
            int checkBox = 0, textBox = 0, tuple = 0, slider = 0;
            foreach (SectionData section in data.Sections.Where(x => x.SectionName != ModSettingsReader.internalSection))
            {
                foreach (KeyData key in section.Keys)
                {
                    ModSettingsKey configKey;
                    if (config && config.Key(section.SectionName, key.KeyName, out configKey))
                    {
                        switch(configKey.type)
                        {
                            case ModSettingsKey.KeyType.Toggle:
                                data[section.SectionName][key.KeyName] = modCheckboxes[checkBox].IsChecked.ToString();
                                checkBox++;
                                break;

                            case ModSettingsKey.KeyType.MultipleChoice:
                            case ModSettingsKey.KeyType.Slider:
                            case ModSettingsKey.KeyType.FloatSlider:
                                data[section.SectionName][key.KeyName] = modSliders[slider].KeyValue;
                                slider++;
                                break;

                            case ModSettingsKey.KeyType.Tuple:
                            case ModSettingsKey.KeyType.FloatTuple:
                                string value = modTuples[tuple].First.ResultText + ModSettingsReader.tupleDelimiterChar + modTuples[tuple].Second.ResultText;
                                data[section.SectionName][key.KeyName] = value;
                                tuple++;
                                break;

                            case ModSettingsKey.KeyType.Text:
                            case ModSettingsKey.KeyType.Color:
                                data[section.SectionName][key.KeyName] = modTextBoxes[textBox].ResultText;
                                textBox++;
                                break;
                        }
                    }
                    else if (key.Value == "True" || key.Value == "False")
                    {
                        data[section.SectionName][key.KeyName] = modCheckboxes[checkBox].IsChecked.ToString();
                        checkBox++;
                    }
                    else if (key.Value.Contains(ModSettingsReader.tupleDelimiterChar))
                    {
                        string value = modTuples[tuple].First.ResultText + ModSettingsReader.tupleDelimiterChar + modTuples[tuple].Second.ResultText;
                        data[section.SectionName][key.KeyName] = value;
                        tuple++;
                    }
                    else
                    {
                        data[section.SectionName][key.KeyName] = modTextBoxes[textBox].ResultText;
                        textBox++;
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
            titleLabel.Text = mod.Title + " settings";
            titleLabel.Position = new Vector2(0, 6);
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
                Button presetButton = GetButton("Presets", HorizontalAlignment.Right, saveButtonColor);
                presetButton.VerticalAlignment = VerticalAlignment.Top;
                presetButton.Size = new Vector2(35, 9);
                presetButton.OnMouseClick += PresetButton_OnMouseClick;
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
                    if (nextPageButton == null)
                    {
                        // Create switch button
                        nextPageButton = new Button()
                        {
                            Size = new Vector2(30, 6),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            BackgroundColor = cancelButtonColor
                        };
                        nextPageButton.Outline.Enabled = true;
                        nextPageButton.Label.Text = "Page 1";
                        nextPageButton.OnMouseClick += NextPageButton_OnMouseClick;
                        modSettingsPanel.Components.Add(nextPageButton);
                    }

                    // Add another page
                    AddPage();

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
        /// Switch between pages.
        /// </summary>
        private void NextPage()
        {
            modSettingsPages[currentPage].Enabled = false;

            currentPage++;
            if (currentPage == modSettingsPages.Count)
                currentPage = 0;

            modSettingsPages[currentPage].Enabled = true;
            nextPageButton.Label.Text = "Page " + (currentPage + 1).ToString();
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

            currentPage = 0;
            x = startX;
            y = startY;

            Init();
        }

        #endregion

        #region Helper Methods

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
        /// Add a slider with text options.
        /// </summary>
        void AddSlider(string[] items, int selected, string title)
        {
            AddSlider(0, items.Length - 1, selected, title, false, items);
        }

        /// <summary>
        /// Add a slider with a numerical indicator.
        /// </summary>
        void AddSlider(float minValue, float maxValue, float startValue, string title)
        {
            AddSlider((int)(minValue * 10), (int)(maxValue * 10), (int)(startValue * 10), title, true);
        }

        /// <summary>
        /// Add a slider with a numerical indicator.
        /// </summary>
        void AddSlider(int minValue, int maxValue, int startValue, string title, bool isFloat = false, string[] items = null)
        {
            MovePosition(8);

            // Slider
            var slider = new HorizontalSlider();
            slider.Position = new Vector2(x, y);
            slider.Size = new Vector2(80.0f, 5.0f);
            slider.DisplayUnits = 20;
            slider.TotalUnits = (maxValue - minValue) + 20;
            slider.ScrollIndex = startValue - minValue;
            slider.BackgroundColor = Color.grey;
            slider.TintColor = new Color(153, 153, 0);
            slider.OnScroll += UpdateSliderIndicators;
            currentPanel.Components.Add(slider);

            // Indicator
            var indicator = new TextLabel();
            indicator.Position = new Vector2(x + slider.Size.x + 15, y);
            currentPanel.Components.Add(indicator);

            var sliderSetup = new SliderSetup()
            {
                slider = slider,
                indicator = indicator,
                lowerValue = minValue,
                isFloat = isFloat
            };
            if (items != null)
                sliderSetup.items = items;
            modSliders.Add(sliderSetup);
            UpdateSliderIndicators();
        }

        private Tuple<TextBox, TextBox> AddTuple(string values)
        {
            int index = values.IndexOf(ModSettingsReader.tupleDelimiterChar);
            var tuple = new Tuple<TextBox, TextBox>(
                GetTextbox(95, 19.6f, values.Substring(0, index)),
                GetTextbox(116, 19.6f, values.Substring(index + ModSettingsReader.tupleDelimiterChar.Length)));
            modTuples.Add(tuple);
            return tuple;
        }

        private void SetBackgroundTint(TextBox textBox)
        {
            // Use box background as a preview of the color
            Color32 color = ModSettingsReader.ColorFromString(textBox.DefaultText);
            textBox.BackgroundColor = color;
            textBox.ToolTip = defaultToolTip;
            textBox.ToolTipText = color.ToString();
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

        /// <summary>
        /// Switch between pages.
        /// </summary>
        private void NextPageButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            NextPage();
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
                ModSettingsReader.ResetSettings(mod, ref data, defaultSettings);
                CloseWindow();
                RestartSettingsWindow();
            }
            else
                CloseWindow();
        }

        /// <summary>
        /// Open preset listbox on preset button.
        /// </summary>
        private void PresetButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Create presets listbox
            presetPicker = new DaggerfallListPickerWindow(uiManager, this);
            presetPicker.ListBox.MaxCharacters = 30;
            presetPicker.OnItemPicked += HandlePresetPickEvent;

            foreach (IniData presetData in presets)
            {
                string presetName;

                try
                {
                    var section = presetData[ModSettingsReader.internalSection];

                    // Get Name
                    string name = section["PresetName"];
                    if (name != null)
                    {
                        presetName = name;
                    }
                    else
                    {
                        presetName = "Unknown preset";
                        Debug.LogError("A preset for mod " + mod.Title + " is missing the key 'PresetName'");
                    }

                    // Get Author (if present)
                    string author = section["PresetAuthor"];
                    if (author != null)
                    {
                        presetName += " by " + author;
                    }

                    // Check Version
                    string presetVersion = section["SettingsVersion"];
                    string settingsVersion = data[ModSettingsReader.internalSection]["SettingsVersion"];
                    if (presetVersion == null)
                    {
                        presetName = "[?] " + presetName;
                        Debug.LogError("Preset " + presetName + " for mod " + mod.Title + " is missing the key 'SettingsVersion'");
                    }
                    else if (presetVersion != settingsVersion)
                    {
                        presetName = "[!] " + presetName;
                        Debug.Log("Preset " + presetName + " was made for version " + presetVersion + " but " + 
                            mod.Title + " has settings version " + settingsVersion);
                    }
                }
                catch
                {
                    presetName = "[Unknown preset]";
                    Debug.LogError("Failed to read the header from a preset for mod " + mod.Title);
                }

                if (presetName.Length > 30)
                {
                    presetName = presetName.Substring(0, 27);
                    presetName += "...";
                }

                // Add preset to listbox
                presetPicker.ListBox.AddItem(presetName);
            }

            uiManager.PushWindow(presetPicker); 
        }

        /// <summary>
        /// preset confirmation.
        /// </summary>
        /// <param name="index">Preset index.</param>
        /// <param name="preset">Preset name.</param>
        private void HandlePresetPickEvent (int index, string preset)
        {
            // Selected preset
            currentPresetIndex = index;

            // Open confirmation message box
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            string message = "Import settings from " + preset + " ?";
            try
            {
                string description = presets[currentPresetIndex][ModSettingsReader.internalSection]["Description"];
                if (description != null)
                {
                    messageBox.SetText(new string[] { description, "", message });
                }
                else
                {
                    messageBox.SetText(message);
                }
            }
            catch
            {
                messageBox.SetText(message);
            }
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Cancel);
            messageBox.OnButtonClick += ConfirmPreset_OnButtonClick;
            uiManager.PushWindow(messageBox);
        }

        /// <summary>
        /// Apply selected preset.
        /// </summary>
        private void ConfirmPreset_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                // Save current settings
                SaveSettings(false);

                // Confront current settings and preset
                foreach (SectionData section in presets[currentPresetIndex].Sections.Where(x => x.SectionName != ModSettingsReader.internalSection))
                    foreach (KeyData key in section.Keys)
                        data[section.SectionName][key.KeyName] = key.Value;

                // Apply changes
                CloseWindow();
                presetPicker.CloseWindow();
                RestartSettingsWindow();
            }
            else
                CloseWindow();
        }

        private void UpdateSliderIndicators()
        {
            for (int i = 0; i < modSliders.Count; i++)
                modSliders[i].indicator.Text = modSliders[i].IndicatorText;
        }

        #endregion
    }
}   