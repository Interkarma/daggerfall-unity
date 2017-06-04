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

using System.IO;
using System.Linq;
using System.Collections.Generic;
using IniParser;
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
        /// <summary>
        /// Constructor for the mod settings window.
        /// </summary>
        /// <param name="mod">Mod whose values are to be exposed on screen.</param>
        public ModSettingsWindow(IUserInterfaceManager uiManager, Mod mod)
            : base(uiManager)
        {
            Mod = mod;
        }

        public override void Update()
        {
            base.Update();

            if(Input.GetKeyDown(KeyCode.Tab) && modSettingsPages.Count > 1)
                NextPage();
        }

        #region Fields

        // UI Controls
        Panel modSettingsPanel = new Panel();
        List<Panel> modSettingsPages = new List<Panel>();
        Panel currentPanel;
        int currentPage = 0;
        Button nextPageButton;
        DaggerfallListPickerWindow presetPicker;
        const int spacing = 8;
        const int startX = 10;
        const int startY = 6;
        int x = startX;
        int y = startY;

        // Fields
        private Mod Mod;
        string path;
        IniData data;
        IniData defaultSettings;
        FileIniDataParser parser                = new FileIniDataParser();
        List<TextBox> modTextBoxes              = new List<TextBox>();
        List<Checkbox> modCheckboxes            = new List<Checkbox>();
        List<Tuple<TextBox, TextBox>> modTuples = new List<Tuple<TextBox, TextBox>>();
        int currentPresetIndex;
        List<IniData> presets                   = new List<IniData>();

        // Colors
        Color panelBackgroundColor    = new Color(0, 0, 0, 0.7f);
        Color resetButtonColor        = new Color(1, 0, 0, 0.4f); //red with alpha
        Color saveButtonColor         = new Color(0.0f, 0.5f, 0.0f, 0.4f); //green with alpha
        Color cancelButtonColor       = new Color(0.2f, 0.2f, 0.2f, 0.4f); //grey with alpha
        Color sectionTitleColor       = new Color(0.16f, 0.26f, 1, 1); // light blue
        
        #endregion

        #region Setup

        /// <summary>
        /// Setup the panel.
        /// </summary>
        protected override void Setup()
        {
            // Set path
            path = Path.Combine(Mod.DirPath, Mod.FileName + ".ini");

            // Default settings
            defaultSettings = ModSettingsReader.GetDefaultSettings(Mod);

            // Presets
            presets = ModSettingsReader.GetPresets(Mod);

            // Add panel
            ParentPanel.BackgroundColor = Color.clear;
            modSettingsPanel.BackgroundColor = panelBackgroundColor;
            modSettingsPanel.Outline.Enabled = true;
            modSettingsPanel.HorizontalAlignment = HorizontalAlignment.Center;
            modSettingsPanel.Position = new Vector2(0, 8);
            modSettingsPanel.Size = new Vector2(320, 175);
            NativePanel.Components.Add(modSettingsPanel);
            InitPanel();
        }

        /// <summary>
        /// Add components to panel.
        /// </summary>
        private void InitPanel()
        {
            // Add title
            TextLabel titleLabel = new TextLabel();
            titleLabel.Text = Mod.Title + " settings";
            titleLabel.Position = new Vector2(0, y);
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

            // Settings
            modSettingsPages.Add(GetPanel(true));
            LoadSettings();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load settings from IniData.
        /// This will be read from the ini file on disk the first time.
        /// </summary>
        private void LoadSettings ()
        {
            // Read file
            if (data == null)
            {
                data = parser.ReadFile(path);
                ModSettingsReader.UpdateSettings(ref data, defaultSettings, Mod);
            }

            // Read settings
            int numberOfElements = 0;
            foreach (SectionData section in data.Sections.Where(x => x.SectionName != ModSettingsReader.internalSection))
            {
                // Section label
                y += spacing;
                TextLabel textLabel = new TextLabel();
                textLabel.Text = section.SectionName;
                textLabel.TextColor = sectionTitleColor;
                textLabel.Position = new Vector2(x, y);
                textLabel.HorizontalAlignment = HorizontalAlignment.None;
                currentPanel.Components.Add(textLabel);

                UpdateItemsCount(ref numberOfElements);
                List<string> comments = section.Comments;
                int comment = 0;

                foreach (KeyData key in section.Keys)
                {
                    y += spacing;

                    // Setting label
                    TextLabel settingName = new TextLabel();
                    settingName.Text = key.KeyName;
                    settingName.Position = new Vector2(x, y);
                    settingName.HorizontalAlignment = HorizontalAlignment.None;
                    if (comment < comments.Count)
                    {
                        settingName.ToolTip = defaultToolTip;
                        settingName.ToolTipText = comments[comment];
                        comment++;
                    }
                    currentPanel.Components.Add(settingName);

                    // Setting field
                    if (key.Value == "True")
                        AddCheckBox(true);
                    else if (key.Value == "False")
                        AddCheckBox(false);
                    else if (key.Value.Contains(ModSettingsReader.tupleDelimiterChar)) // Tuple
                    {
                        int index = key.Value.IndexOf(ModSettingsReader.tupleDelimiterChar);
                        var first = GetTextbox(95, 19.6f, key.Value.Substring(0, index));
                        var second = GetTextbox(116, 19.6f, key.Value.Substring(index + ModSettingsReader.tupleDelimiterChar.Length));
                        modTuples.Add(new Tuple<TextBox, TextBox>(first, second));
                    }
                    else
                    {
                        TextBox textBox = GetTextbox(95, 40, key.Value);
                        modTextBoxes.Add(textBox);

                        // Color
                        if (textBox.DefaultText.Length == 8)
                        {
                            // Check if is a hex number or just a string with lenght eight
                            int hexColor;
                            if (int.TryParse(textBox.DefaultText, System.Globalization.NumberStyles.HexNumber,
                                     System.Globalization.CultureInfo.InvariantCulture, out hexColor))
                            {
                                // Use box background as a preview of the color
                                Color32 color = ModSettingsReader.ColorFromString(textBox.DefaultText);
                                textBox.BackgroundColor = color;
                                textBox.ToolTip = defaultToolTip;
                                textBox.ToolTipText = color.ToString();
                            }
                        }
                    }

                    UpdateItemsCount(ref numberOfElements);
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
            int checkBox = 0, textBox = 0, tuple = 0;
            foreach (SectionData section in data.Sections.Where(x => x.SectionName != ModSettingsReader.internalSection))
            {
                foreach (KeyData key in section.Keys)
                {
                    if (key.Value == "True" || key.Value == "False")
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
                parser.WriteFile(path, data);
        }

        /// <summary>
        /// Controls page and column disposition.
        /// </summary>
        /// <param name="numberOfElements">Number of titles and keys already placed on window.</param>
        /// <returns>False on reached capacity limit.</returns>
        private void UpdateItemsCount(ref int numberOfElements)
        {
            const int elementsPerColumn = 19;
            const int elementsPerPage = elementsPerColumn * 2;

            numberOfElements++;
            if (numberOfElements % elementsPerPage == 0)
            {
                if (numberOfElements == elementsPerPage)
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
                modSettingsPages.Add(GetPanel(false));

                // Move to left column
                y = startY;
                x = startX;
            }
            else if (numberOfElements % elementsPerColumn == 0)
            {
                // Move to right column
                y = startY;
                x += 160;
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
        /// Get a panel and set it as current page.
        /// </summary>
        /// <param name="isEnabled">Is panel enabled?</param>
        private Panel GetPanel(bool isEnabled)
        {
            Panel panel = new Panel()
            {
                BackgroundColor = Color.clear,
                Position = new Vector2(0, 0),
                Size = new Vector2(320, 175),
                Enabled = isEnabled
            };
            panel.Outline.Enabled = false;
            modSettingsPanel.Components.Add(panel);
            currentPanel = panel;
            return panel;
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

            currentPage = 0;
            x = startX;
            y = startY;

            InitPanel();
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
                // Save default settings
                data = defaultSettings;
                parser.WriteFile(path, data);

                // Restart settings window
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
                        Debug.LogError("A preset for mod " + Mod.Title + " is missing the key 'PresetName'");
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
                        Debug.LogError("Preset " + presetName + " for mod " + Mod.Title + " is missing the key 'SettingsVersion'");
                    }
                    else if (presetVersion != settingsVersion)
                    {
                        presetName = "[!] " + presetName;
                        Debug.Log("Preset " + presetName + " was made for version " + presetVersion + " but " + 
                            Mod.Title + " has settings version " + settingsVersion);
                    }
                }
                catch
                {
                    presetName = "[Unknown preset]";
                    Debug.LogError("Failed to read the header from a preset for mod " + Mod.Title);
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
                    messageBox.SetText(description, "", message);
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

        #endregion
    }
}   