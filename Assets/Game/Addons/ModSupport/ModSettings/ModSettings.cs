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
        const int startY = 6;
        int x = 10;
        int y = startY;

        // Fields
        string path;
        IniData data;
        FileIniDataParser parser      = new FileIniDataParser();
        List<TextBox> modTextBoxes    = new List<TextBox>();

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
            modSettingsPanel.BackgroundColor = new Color(0, 0, 0, 0.7f);
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
            Button resetButton = GetButton("Reset", HorizontalAlignment.Left, new Color(1, 0, 0, 0.4f));
            resetButton.OnMouseClick += ResetButton_OnMouseClick;

            // Save button
            Button saveButton = GetButton("Save", HorizontalAlignment.Center, new Color(0.0f, 0.5f, 0.0f, 0.4f));
            saveButton.OnMouseClick += SaveButton_OnMouseClick;

            // Cancel button
            Button cancelButton = GetButton("Cancel", HorizontalAlignment.Right, new Color(0.2f, 0.2f, 0.2f, 0.4f));
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
            int i = 0;
            foreach (SectionData section in data.Sections)
            {
                // Section label
                y += 7;
                TextLabel textLabel = new TextLabel();
                textLabel.Text = section.SectionName;
                textLabel.TextColor = Color.blue;
                textLabel.Position = new Vector2(x, y);
                textLabel.HorizontalAlignment = HorizontalAlignment.None;
                modSettingsPanel.Components.Add(textLabel);

                foreach (KeyData key in section.Keys)
                {
                    y += 7;

                    // Setting label
                    TextLabel settingName = new TextLabel();
                    settingName.Text = key.KeyName;
                    settingName.Position = new Vector2(x, y);
                    settingName.HorizontalAlignment = HorizontalAlignment.None;
                    modSettingsPanel.Components.Add(settingName);
                    
                    // Setting field
                    TextBox textBox = new TextBox();
                    textBox.Position = new Vector2(x + 95, y);
                    textBox.AutoSize = AutoSizeModes.None;
                    textBox.FixedSize = true;
                    textBox.Size = new Vector2(30, 6);
                    textBox.Numeric = false;
                    textBox.MaxCharacters = 20;
                    textBox.DefaultText = key.Value;
                    textBox.UseFocus = true;
                    textBox.Outline.Enabled = true;
                    textBox.HasFocusOutlineColor = Color.green;
                    textBox.LostFocusOutlineColor = Color.white;
                    //textBox.BackgroundColor = Color.yellow;
                    modSettingsPanel.Components.Add(textBox);
                    modTextBoxes.Add(textBox);
                    i++;

                    // Move to right column
                    if (i == 15)
                    {
                        y = startY;
                        x += 140;
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
            int i = 0;
            foreach (SectionData section in data.Sections)
            {
                foreach (KeyData key in section.Keys)
                {
                    data[section.SectionName][key.KeyName] = modTextBoxes[i].ResultText;
                    i++;
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
        /// Load default settings and close the window.
        /// </summary>
        private void ResetButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // TODO: ask confirmation
            IniData defaultSettings = ReadModSettings.GetDefaultSettings(Mod);
            parser.WriteFile(path, defaultSettings);
            DaggerfallUI.UIManager.PopWindow();
        }

        #endregion
    }
}   