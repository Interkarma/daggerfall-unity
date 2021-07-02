// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2021 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: jefetienne
// Contributors:    
// 
// Notes:
//

using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class PauseOptionsDropdown : Panel
    {
        #region Fields

        // Red up/down arrows
        private const string redArrowsTextureName = "INVE07I0.IMG";
        private const int dropdownButtonHeight = 10;

        private readonly Rect upArrowRect = new Rect(0, 0, 9, 16);
        private readonly Rect downArrowRect = new Rect(0, 136, 9, 16);
        private readonly DFSize arrowsFullSize = new DFSize(9, 152);
        private Texture2D arrowUpTexture;
        private Texture2D arrowDownTexture;

        private List<Button> dropdownButtons;
        private int maxTextWidth = 0;

        private Panel dropdownPanel;
        private Button dropDownToggleButton;
        private IUserInterfaceManager uiManager;

        #endregion

        #region Constructor and Setup

        public PauseOptionsDropdown(IUserInterfaceManager _uiManager)
            : base()
        {
            uiManager = _uiManager;
            dropdownButtons = new List<Button>();
            Setup();

            AddOption("Mod Settings", ModSettingsWindowOption_OnMouseClick);

            foreach (var option in DaggerfallUI.Instance.GetPauseOptionsDropdownItems())
                AddOption(option.Item1, option.Item2);
        }

        void Setup()
        {
            // Cut out red up/down arrows
            Texture2D arrowTexture = ImageReader.GetTexture(redArrowsTextureName);
            arrowUpTexture = ImageReader.GetSubTexture(arrowTexture, upArrowRect, arrowsFullSize);
            arrowDownTexture = ImageReader.GetSubTexture(arrowTexture, downArrowRect, arrowsFullSize);

            // Drop down button
            dropDownToggleButton = DaggerfallUI.AddButton(new Rect(0, 0, 7, 7), this);
            dropDownToggleButton.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);//0.5f);
            dropDownToggleButton.OnMouseClick += DropdownButton_OnMouseClick;
            dropDownToggleButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.OptionsDropdown);
            dropDownToggleButton.BackgroundTexture = arrowUpTexture;

            // Dropdown options panel
            dropdownPanel = new Panel();
            dropdownPanel.Position = new Vector2(0, dropDownToggleButton.Position.y + dropDownToggleButton.Size.y);
            dropdownPanel.Size = new Vector2(50, 30); //* dropdownPanel.Scale;
            dropdownPanel.BackgroundColor = Color.black;
            this.Components.Add(dropdownPanel);
            dropdownPanel.Enabled = false;
        }

        #endregion

        #region Public Methods

        public void AddOption(string text, OnMouseClickHandler onMouseClickHandler)
        {
            Button button = DaggerfallUI.AddButton(
                new Rect(0, 1 + (dropdownButtonHeight + 1) * dropdownButtons.Count, dropdownPanel.Size.x, dropdownButtonHeight),
                dropdownPanel);

            dropdownButtons.Add(button);

            button.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            button.Label.Text = text;
            button.OnMouseClick += onMouseClickHandler;

            if (button.Label.TextWidth > maxTextWidth)
                maxTextWidth = button.Label.TextWidth + 8;

            dropdownPanel.Size = new Vector2(maxTextWidth, 1 + (dropdownButtonHeight + 1) * dropdownButtons.Count);

            foreach (var b in dropdownButtons)
                b.Size = new Vector2(maxTextWidth, b.Size.y);
        }

        #endregion

        #region Private Events

        private void DropdownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            dropdownPanel.Enabled = !dropdownPanel.Enabled;

            if (dropdownPanel.Enabled)
                dropDownToggleButton.BackgroundTexture = arrowDownTexture;
            else
                dropDownToggleButton.BackgroundTexture = arrowUpTexture;
        }

        private void ModSettingsWindowOption_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            var modTitles = ModManager.Instance.Mods
                .Where(x => x.HasSettings && x.LoadSettingsCallback != null)
                .Select(x => x.Title)
                .ToArray();

            var listPicker = new DaggerfallListPickerWindow(uiManager);
            listPicker.ListBox.AddItems(modTitles);
            listPicker.OnItemPicked += (index, modTitle) =>
            {
                listPicker.PopWindow();
                uiManager.PushWindow(new ModSettingsWindow(uiManager, ModManager.Instance.GetMod(modTitle), true));
            };

            uiManager.PushWindow(listPicker);
        }

        #endregion
    }
}
