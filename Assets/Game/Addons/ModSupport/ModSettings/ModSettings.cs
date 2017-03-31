// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System.IO;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    public class ModSettingsWindow : DaggerfallPopupWindow
    {
        // Constructors
        public ModSettingsWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        // UI Controls
        Panel modSettingsPanel        = new Panel();
        const int elementsForColumn = 19;
        const int spacing = 8;
        const int startY = 6;
        int x = 10;
        int y = startY;

        // Fields
        string path;
        IniData data;
        FileIniDataParser parser      = new FileIniDataParser();
        List<TextBox> modTextBoxes    = new List<TextBox>();
        List<Checkbox> modCheckboxes  = new List<Checkbox>();

        // Colors
        Color panelBackgroundColor    = new Color(0, 0, 0, 0.7f);
        Color resetButtonColor        = new Color(1, 0, 0, 0.4f); //red with alpha
        Color saveButtonColor         = new Color(0.0f, 0.5f, 0.0f, 0.4f); //green with alpha
        Color cancelButtonColor       = new Color(0.2f, 0.2f, 0.2f, 0.4f); //grey with alpha

        // Properties
        public Mod Mod { get; set; }

        #region Setup

        /// <summary>
        /// Setup.
        /// </summary>
        protected override void Setup()
        {
            // Set path
            path = Path.Combine(ModManager.Instance.ModDirectory, Mod.Name + ".ini");

            // Add panel
            ParentPanel.BackgroundColor = Color.clear;
            modSettingsPanel.BackgroundColor = panelBackgroundColor;
            modSettingsPanel.Outline.Enabled = true;
            modSettingsPanel.HorizontalAlignment = HorizontalAlignment.Center;
            modSettingsPanel.Position = new Vector2(0, 8);
            modSettingsPanel.Size = new Vector2(320, 175);
            NativePanel.Components.Add(modSettingsPanel);

            // Add title
            TextLabel titleLabel = new TextLabel();
            titleLabel.Text = Mod.Title + " settings";
            titleLabel.Position = new Vector2(0, y);
            titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            modSettingsPanel.Components.Add(titleLabel);

            // Settings
            LoadSettings();

            // Reset button
            Button resetButton = GetButton("Reset", HorizontalAlignment.Left, resetButtonColor);
            resetButton.OnMouseClick += ResetButton_OnMouseClick;

            // Save button
            Button saveButton = GetButton("Save", HorizontalAlignment.Center, saveButtonColor);
            saveButton.OnMouseClick += SaveButton_OnMouseClick;

            // Cancel button
            Button cancelButton = GetButton("Cancel", HorizontalAlignment.Right, cancelButtonColor);
            cancelButton.OnMouseClick += CancelButton_OnMouseClick;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if a mod support settings. If configuration file
        /// is missing it will be recreated with default values.
        /// </summary>
        public static bool HasSettings(Mod mod)
        {
            // Get path
            string settingPath = Path.Combine(ModManager.Instance.ModDirectory, mod.Name + ".ini");

            // File on disk
            if (File.Exists(settingPath))
                return true;

            if (mod.AssetBundle.Contains(mod.Name + ".ini.txt"))
            {
                // Recreate file on disk using default values
                IniData defaultSettings = ReadModSettings.GetDefaultSettings(mod);
                FileIniDataParser defaultSettingsParser = new FileIniDataParser();
                defaultSettingsParser.WriteFile(settingPath, defaultSettings);
                return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load settings from ini file.
        /// </summary>
        private void LoadSettings ()
        {
            // Read file
            data = parser.ReadFile(path);

            // Read settings
            int numberOfElements = 0;
            foreach (SectionData section in data.Sections)
            {
                // Section label
                y += spacing;
                TextLabel textLabel = new TextLabel();
                textLabel.Text = section.SectionName;
                textLabel.TextColor = Color.blue;
                textLabel.Position = new Vector2(x, y);
                textLabel.HorizontalAlignment = HorizontalAlignment.None;
                modSettingsPanel.Components.Add(textLabel);
                numberOfElements++;

                foreach (KeyData key in section.Keys)
                {
                    y += spacing;

                    // Setting label
                    TextLabel settingName = new TextLabel();
                    settingName.Text = key.KeyName;
                    settingName.Position = new Vector2(x, y);
                    settingName.HorizontalAlignment = HorizontalAlignment.None;
                    modSettingsPanel.Components.Add(settingName);

                    // Setting field
                    if (key.Value == "True")
                        AddCheckBox(true);
                    else if (key.Value == "False")
                        AddCheckBox(false);
                    else
                    {
                        TextBox textBox = new TextBox();
                        textBox.Position = new Vector2(x + 95, y);
                        textBox.AutoSize = AutoSizeModes.None;
                        textBox.FixedSize = true;
                        textBox.Size = new Vector2(40, 6);
                        textBox.Numeric = false;
                        textBox.MaxCharacters = 20;
                        textBox.DefaultText = key.Value;
                        textBox.UseFocus = true;
                        textBox.Outline.Enabled = true;
                        textBox.HasFocusOutlineColor = Color.green;
                        textBox.LostFocusOutlineColor = Color.white;
                        modSettingsPanel.Components.Add(textBox);
                        modTextBoxes.Add(textBox);

                        // Color
                        if (textBox.DefaultText.Length == 8)
                        {
                            // Check if is a hex number or just a string with lenght eight
                            int hexColor = 0;
                            if (int.TryParse(textBox.DefaultText, System.Globalization.NumberStyles.HexNumber,
                                     System.Globalization.CultureInfo.InvariantCulture, out hexColor))
                            {
                                // Use box background as a preview of the color
                                Color32 color = ReadModSettings.ColorFromString(textBox.DefaultText);
                                textBox.BackgroundColor = color;
                                textBox.ToolTip = defaultToolTip;
                                //textBox.ToolTipText = string.Format("{0}, {1}, {2}, {3}", color.r, color.g, color.b, color.a);
                                textBox.ToolTipText = color.ToString();
                            }
                        }
                    }
                    numberOfElements++;

                    // Move to right column
                    if (numberOfElements == elementsForColumn)
                    {
                        y = startY;
                        x += 160;
                    }    
                }
            }
        }

        /// <summary>
        /// Write new settings to configuration file.
        /// </summary>
        private void SaveSettings ()
        {
            // Set new values
            int checkBox = 0, textBox = 0;
            foreach (SectionData section in data.Sections)
            {
                foreach (KeyData key in section.Keys)
                {
                    if (key.Value == "True" || key.Value == "False")
                    {
                        data[section.SectionName][key.KeyName] = modCheckboxes[checkBox].IsChecked.ToString();
                        checkBox++;
                    }
                    else
                    {
                        data[section.SectionName][key.KeyName] = modTextBoxes[textBox].ResultText;
                        textBox++;
                    }     
                }
            }

            // Save to file
            parser.WriteFile(path, data);
        }

        /// <summary>
        /// Get a button.
        /// </summary>
        private Button GetButton (string label, HorizontalAlignment alignment, Color backgroundColor)
        {
            Button button = new Button();
            button.Size = new Vector2(30, 9);
            button.HorizontalAlignment = alignment;
            button.VerticalAlignment = VerticalAlignment.Bottom;
            button.BackgroundColor = backgroundColor;
            button.Outline.Enabled = true;
            button.Label.Text = label;
            modSettingsPanel.Components.Add(button);
            return button;
        }

        /// <summary>
        /// Add a checkbox.
        /// </summary>
        private void AddCheckBox (bool isChecked)
        {
            Checkbox checkbox = new Checkbox();
            checkbox.Position = new Vector2(x + 95, y);
            checkbox.Size = new Vector2(2, 2);
            checkbox.CheckBoxColor = Color.white;
            checkbox.IsChecked = isChecked;
            modSettingsPanel.Components.Add(checkbox);
            modCheckboxes.Add(checkbox);
        }

        #endregion

        #region Event Handlers

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
                // Get and save default settings
                IniData defaultSettings = ReadModSettings.GetDefaultSettings(Mod);
                parser.WriteFile(path, defaultSettings);

                // Close settings window
                CloseWindow();
                DaggerfallUI.UIManager.PopWindow();
            }
            else
                CloseWindow();
        }

        #endregion
    }
}   