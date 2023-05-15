// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallWorkshop.Utility.AssetInjection;
using System;
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
        private const int unitsDisplayed = 10;

        private readonly Rect upArrowRect = new Rect(0, 0, 9, 16);
        private readonly Rect downArrowRect = new Rect(0, 136, 9, 16);
        private readonly DFSize arrowsFullSize = new DFSize(9, 152);
        private Texture2D closedMenuIcon;
        private Texture2D openMenuIcon;

        private List<ListBox.ListItem> listItems;
        private List<Action> clickHandlers;
        private int maxTextWidth = 0;

        private Panel dropdownPanel;
        private ListBox dropdownList;
        private Button dropDownToggleButton;
        private IUserInterfaceManager uiManager;
        private VerticalScrollBar dropdownScroller;

        #endregion

        #region Constructor and Setup

        public PauseOptionsDropdown(IUserInterfaceManager _uiManager)
            : base()
        {
            uiManager = _uiManager;
            clickHandlers = new List<Action>();
            Setup();
        }

        void Setup()
        {
            closedMenuIcon = Resources.Load<Texture2D>("hamburger_button");
            openMenuIcon = Resources.Load<Texture2D>("hamburger_button");

            // Drop down button
            dropDownToggleButton = DaggerfallUI.AddButton(new Rect(0, 0, 7, 7), this);
            dropDownToggleButton.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);//0.5f);
            dropDownToggleButton.OnMouseClick += DropdownButton_OnMouseClick;
            dropDownToggleButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.OptionsDropdown);
            dropDownToggleButton.BackgroundTexture = closedMenuIcon;

            // Dropdown options panel
            dropdownPanel = new Panel();
            dropdownPanel.Position = new Vector2(0, dropDownToggleButton.Position.y + dropDownToggleButton.Size.y);
            dropdownPanel.Size = new Vector2(50, 30);
            dropdownPanel.BackgroundColor = Color.black;
            dropdownPanel.Enabled = false;
            dropdownPanel.Outline.Enabled = true;
            this.Components.Add(dropdownPanel);

            dropdownList = new ListBox();
            dropdownList.Position = new Vector2(2, 2);
            dropdownList.ShadowPosition = Vector2.zero;
            dropdownList.RowsDisplayed = unitsDisplayed;
            SetBackground(dropdownList, Color.black, "pauseDropdownListBackgroundColor");
            dropdownList.OnSelectItem += DropdownList_OnUseSelectedItem;
            dropdownList.SelectedTextColor = dropdownList.TextColor;
            dropdownList.OnScroll += DropdownList_OnScroll;
            dropdownPanel.Components.Add(dropdownList);

            AddOptions();
            var height = dropdownList.HeightContent();
            dropdownList.Size = new Vector2(maxTextWidth, height > 80 ? 80 : height);
            dropdownPanel.Size = new Vector2(dropdownList.Size.x + 6, dropdownList.Size.y + 3);

            dropdownScroller = new VerticalScrollBar();
            dropdownScroller.Position = new Vector2(maxTextWidth, 2);
            dropdownScroller.Size = new Vector2(5, dropdownPanel.Size.y - 3);
            dropdownScroller.DisplayUnits = unitsDisplayed;
            dropdownScroller.TotalUnits = dropdownList.Count;
            dropdownScroller.OnScroll += SavesScroller_OnScroll;
            dropdownPanel.Components.Add(dropdownScroller);
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            if (Enabled)
                base.Update();

            // Adjust scaling, size and positioning when set in the ParentPanel
            float sx = Scale.x;
            float sy = Scale.y;

            dropDownToggleButton.Size = new Vector2(7, 7) * Scale;

            dropdownList.Scale = Scale;
            var height = dropdownList.HeightContent();
            dropdownList.Size = new Vector2(maxTextWidth * sx, height > 70 * sy ? 70 * sy : height);
            dropdownList.Position = new Vector2(2, 2) * Scale;

            dropdownPanel.Size = new Vector2(dropdownList.Size.x + 6 * sx, dropdownList.Size.y + 3 * sy);
            dropdownPanel.Position = new Vector2(0, dropDownToggleButton.Position.y + dropDownToggleButton.Size.y);

            dropdownScroller.Size = new Vector2(5 * sx, dropdownPanel.Size.y - 3 * sy);
            dropdownScroller.Position = new Vector2(maxTextWidth, 2) * Scale;

            for (int i = 0; i < listItems.Count; i++)
                listItems[i].textLabel.TextScale = sx;
        }

        #endregion

        #region Public Methods

        public void SetDropdownExpand(bool expand)
        {
            dropdownPanel.Enabled = expand;

            if (dropdownPanel.Enabled)
                dropDownToggleButton.BackgroundTexture = openMenuIcon;
            else
                dropDownToggleButton.BackgroundTexture = closedMenuIcon;

            HideOverlappingHUDElements(expand);
        }

        #endregion

        #region Private Methods

        void HideOverlappingHUDElements(bool hide)
        {
            if (DaggerfallUI.Instance.DaggerfallHUD == null)
                return;

            // Spell icons can overlap pause options dropdown - disable this HUD element while dropdown is open
            DaggerfallUI.Instance.DaggerfallHUD.ActiveSpells.Enabled = !hide;
        }

        private bool HasApplicableMods()
        {
            if (ModManager.Instance == null)
                return false;

            foreach (var m in ModManager.Instance.Mods)
            {
                if (m.HasSettings && m.LoadSettingsCallback != null)
                    return true;
            }

            return false;
        }

        private void SetBackground(BaseScreenComponent panel, Color color, string textureName)
        {
            if (TextureReplacement.TryImportTexture(textureName, true, out Texture2D tex))
            {
                panel.BackgroundTexture = tex;
                TextureReplacement.LogLegacyUICustomizationMessage(textureName);
            }
            else
                panel.BackgroundColor = color;
        }

        private void AddOptions()
        {
            // Add mod settings option and set that as the current max text width
            dropdownList.AddItem(TextManager.Instance.GetLocalizedText("modSettings"), out ListBox.ListItem modSettings);
            UpdateMaxWidth(modSettings.textLabel.TextWidth);
            modSettings.Enabled = HasApplicableMods();
            clickHandlers.Add(ModSettingsWindowOption_OnClick);

            // Add game effects  option
            dropdownList.AddItem(TextManager.Instance.GetLocalizedText("gameEffectsSettings"), out ListBox.ListItem gameEffectsSettings);
            UpdateMaxWidth(gameEffectsSettings.textLabel.TextWidth);
            gameEffectsSettings.Enabled = true;
            clickHandlers.Add(GameEffectsWindowOption_OnClick);

            foreach (var opt in DaggerfallUI.Instance.GetPauseOptionsDropdownItems())
            {
                dropdownList.AddItem(opt.Item1, out ListBox.ListItem item);
                clickHandlers.Add(opt.Item2);

                if (item.textLabel.TextWidth > maxTextWidth)
                    maxTextWidth = item.textLabel.TextWidth + 8;
            }

            listItems = dropdownList.ListItems;
            // Select nothing, so if the first option is disabled, the selectedIndex
            // will be -1 and won't interfere with the default disabled colors
            dropdownList.SelectNone();
        }

        void UpdateMaxWidth(int itemWidth)
        {
            if (itemWidth > maxTextWidth)
                maxTextWidth = itemWidth + 8;
        }

        #endregion

        #region Private Events

        private void SavesScroller_OnScroll()
        {
            dropdownList.ScrollIndex = dropdownScroller.ScrollIndex;
        }

        private void DropdownList_OnScroll()
        {
            dropdownScroller.ScrollIndex = dropdownList.ScrollIndex;
        }

        private void DropdownList_OnUseSelectedItem()
        {
            int ind = dropdownList.SelectedIndex;

            if (ind < 0)
                return;

            dropdownList.SelectedIndex = -1;

            if (listItems[ind].Enabled)
                clickHandlers[ind]();
        }

        private void DropdownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetDropdownExpand(!dropdownPanel.Enabled);
        }

        private void ModSettingsWindowOption_OnClick()
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

        private void GameEffectsWindowOption_OnClick()
        {
            uiManager.PushWindow(DaggerfallUI.Instance.GameEffectsConfigWindow);
        }

        #endregion
    }
}
