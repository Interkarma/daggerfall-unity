// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// ListBox with additional features for presets.
    /// Allow importing mod and local presets as well as creating local presets.
    /// </summary>
    public class PresetPicker : DaggerfallPopupWindow
    {
        #region Fields

        const float windowWidth = 150;
        const int titleMaxChars = 20;
        const int descriptionMaxChars = 35;

        readonly Mod mod;
        readonly ModSettingsData settings;

        Panel mainPanel                 = new Panel();
        Panel infoPanel                 = new Panel();
        Panel creatorPanel              = new Panel();
        Panel controlPanel              = new Panel();

        ListBox listBox                 = new ListBox();
        VerticalScrollBar scrollBar     = new VerticalScrollBar();
        TextLabel descriptionLabel      = new TextLabel();
        TextLabel authorLabel           = new TextLabel();
        TextLabel versionLabel          = new TextLabel();
        TextBox creatorTitle            = new TextBox();
        TextBox creatorDescription      = new TextBox();

        Button newPresetButton          = new Button();
        Button loadButton               = new Button();
        Button saveButton               = new Button();
        Button deleteButton             = new Button();

        Color mainBackgroundColor       = new Color(0, 0, 0, 0.7f);
        Color infoBackgroundColor       = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        Color creatorBackgroundColor    = new Color(0, 0, 1, 0.1f);
        Color titleColor                = Color.gray;
        Color selectedTitleColor        = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        Color warningColor              = new Color(1, 0, 0, 0.4f);
        Color enabledButtonsColor       = new Color(0.0f, 0.5f, 0.0f, 0.4f);

        bool creationMode = false;
        bool writeToDiskFlag = false;

        #endregion

        #region Properties

        Preset SelectedPreset
        {
            get { return settings.Presets[listBox.SelectedIndex]; }
        }

        public Action ApplyChangesCallback { private get; set; }

        #endregion

        #region Constructors

        public PresetPicker(IUserInterfaceManager uiManager, Mod mod, ModSettingsData settings)
            : base(uiManager)
        {
            this.mod = mod;
            this.settings = settings;
        }

        #endregion

        #region Overrides

        protected override void Setup()
        {
            ParentPanel.BackgroundColor = Color.clear;

            mainPanel.Size = new Vector2(windowWidth, 120 + 10);
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundColor = mainBackgroundColor;
            mainPanel.Outline.Enabled = true;
            NativePanel.Components.Add(mainPanel);

            TextLabel titleLabel = new TextLabel(DaggerfallUI.Instance.Font3);
            titleLabel.Position = new Vector2(5, 2);
            titleLabel.Text = ModManager.GetText("presets");
            titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            titleLabel.HorizontalTextAlignment = TextLabel.HorizontalTextAlignmentSetting.Center;
            titleLabel.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            titleLabel.ShadowColor = Color.clear;
            titleLabel.TextScale = 1.2f;
            mainPanel.Components.Add(titleLabel);

            Button cancelButton = new Button();
            cancelButton.Size = new Vector2(40, 10);
            cancelButton.Position = new Vector2(0, 0);
            cancelButton.Label.Text = string.Format("< {0}", ModManager.GetText("settings"));
            cancelButton.Label.Font = DaggerfallUI.Instance.Font3;
            cancelButton.Label.ShadowColor = Color.clear;
            cancelButton.Label.TextColor = Color.grey;
            cancelButton.Label.TextScale = 0.8f;
            cancelButton.OnMouseClick += CancelButton_OnMouseClick;
            mainPanel.Components.Add(cancelButton);

            listBox.Position = new Vector2(5, 12);
            listBox.Size = new Vector2(140, 80);
            listBox.BackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            listBox.OnScroll += () => scrollBar.ScrollIndex = listBox.ScrollIndex;
            listBox.OnSelectItem += ListBox_OnSelectItem;
            mainPanel.Components.Add(listBox);

            scrollBar.Size = new Vector2(5, listBox.Size.y);
            scrollBar.Position = new Vector2(0, listBox.Position.y);
            scrollBar.HorizontalAlignment = HorizontalAlignment.Right;
            scrollBar.BackgroundColor = Color.grey;
            scrollBar.DisplayUnits = 9;
            scrollBar.OnScroll += () => listBox.ScrollIndex = scrollBar.ScrollIndex;
            mainPanel.Components.Add(scrollBar);

            infoPanel.Position = new Vector2(5, 100);
            infoPanel.Size = new Vector2(windowWidth - 10, 20);
            infoPanel.BackgroundColor = infoBackgroundColor;         
            infoPanel.HorizontalAlignment = HorizontalAlignment.Center;
            infoPanel.LeftMargin = infoPanel.RightMargin = 3;
            infoPanel.TopMargin = infoPanel.BottomMargin = 1;
            mainPanel.Components.Add(infoPanel);

            authorLabel.Size = new Vector2(windowWidth / 2, 10);
            authorLabel.MaxWidth = Mathf.FloorToInt(windowWidth / 2);
            authorLabel.Position = new Vector2(0, 1);
            authorLabel.HorizontalAlignment = HorizontalAlignment.Left;
            authorLabel.TextScale = 0.6f;
            authorLabel.ShadowColor = Color.clear;
            infoPanel.Components.Add(authorLabel);

            versionLabel.Size = new Vector2(windowWidth / 2, 10);
            versionLabel.MaxWidth = Mathf.FloorToInt(windowWidth / 2);
            versionLabel.Position = new Vector2(0, 1);
            versionLabel.HorizontalAlignment = HorizontalAlignment.Right;
            versionLabel.TextScale = 0.6f;
            versionLabel.ShadowColor = Color.clear;
            versionLabel.BackgroundColor = warningColor;
            infoPanel.Components.Add(versionLabel);

            descriptionLabel.Size = new Vector2(windowWidth, 10);
            descriptionLabel.Position = new Vector2(0, infoPanel.Size.y / 2);
            descriptionLabel.HorizontalAlignment = HorizontalAlignment.Center;
            descriptionLabel.VerticalAlignment = VerticalAlignment.Middle;
            descriptionLabel.TextScale = 0.7f;
            descriptionLabel.ShadowColor = Color.clear;
            descriptionLabel.MaxWidth = Mathf.FloorToInt(infoPanel.Size.x);
            infoPanel.Components.Add(descriptionLabel);

            controlPanel.BackgroundColor = Color.clear;
            controlPanel.HorizontalAlignment = HorizontalAlignment.Center;
            controlPanel.VerticalAlignment = VerticalAlignment.Bottom;
            controlPanel.Size = new Vector2(mainPanel.Size.x, 10);
            controlPanel.SetMargins(Margins.All, 1);
            mainPanel.Components.Add(controlPanel);

            SetupBottomBarButton(0, "new", AddPresetButton_OnMouseClick, newPresetButton);
            SetupBottomBarButton(1, "load", LoadButton_OnMouseClick, loadButton);
            SetupBottomBarButton(2, "save", SaveButton_OnMouseClick, saveButton);
            SetupBottomBarButton(3, "delete", DeleteButton_OnMouseClick, deleteButton);

            creatorPanel.BackgroundColor = creatorBackgroundColor;
            creatorPanel.Outline.Enabled = false;
            creatorPanel.HorizontalAlignment = HorizontalAlignment.Center;
            creatorPanel.Position = infoPanel.Position;
            creatorPanel.Size = infoPanel.Size;
            creatorPanel.Enabled = false;
            mainPanel.Components.Add(creatorPanel);

            creatorTitle.Size = new Vector2(30, 10);
            creatorTitle.Position = new Vector2(5, 2);
            creatorTitle.DefaultText = ModManager.GetText("emptyTitle");
            creatorTitle.MaxCharacters = titleMaxChars;
            creatorTitle.UseFocus = true;
            creatorTitle.OnMouseLeave += CreatorTitle_OnMouseLeave;
            creatorPanel.Components.Add(creatorTitle);

            creatorDescription.Size = new Vector2(100, 10);
            creatorDescription.Position = new Vector2(5, 10);
            creatorDescription.DefaultText = ModManager.GetText("emptyDescription");
            creatorDescription.MaxCharacters = descriptionMaxChars;
            creatorDescription.UseFocus = true;
            creatorDescription.OnMouseLeave += CreatorDescription_OnMouseLeave;
            creatorPanel.Components.Add(creatorDescription);

            ToggleButtons(true, newPresetButton);
            ToggleButtons(false, loadButton, saveButton, deleteButton);
            if (listBox.Count > 0)
                ListBox_OnSelectItem();
        }

        public override void OnPush()
        {
            foreach (Preset preset in settings.Presets)
                RegisterPreset(preset);

            scrollBar.TotalUnits = listBox.Count;
        }

        public override void OnPop()
        {
            if (writeToDiskFlag)
                settings.SavePresets();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add preset to listbox.
        /// </summary>
        private void RegisterPreset(Preset preset)
        {
            string title = preset.Title;
            preset.Title = !string.IsNullOrEmpty(preset.Title) ?
                 mod.TryLocalize("Presets", title, "Title") ?? preset.Title : ModManager.GetText("missingTitle");
            preset.Description = !string.IsNullOrEmpty(preset.Description) ?
                mod.TryLocalize("Presets", title, "Description") ?? preset.Description : ModManager.GetText("missingDescription");

            ListBox.ListItem itemOut;
            listBox.AddItem(preset.Title, out itemOut);
            itemOut.textColor = settings.IsCompatible(preset) ? titleColor : warningColor;
            itemOut.selectedTextColor = selectedTitleColor;
            itemOut.shadowColor = Color.clear;
        }

        /// <summary>
        /// Add new preset to presets list and listbox. 
        /// </summary>
        private void AddPreset(Preset preset)
        {
            settings.Presets.Add(preset);
            RegisterPreset(preset);
            scrollBar.TotalUnits = listBox.Count;
            listBox.SelectedIndex = listBox.Count - 1;
            listBox.ScrollToSelected();
        }

        private void SetCreationMode(bool toggle)
        {
            creatorPanel.Enabled = toggle;
            infoPanel.Enabled = !toggle;
            creationMode = toggle;

            if (!toggle)
                creatorDescription.Text = creatorTitle.Text = string.Empty;
        }

        private void SetupBottomBarButton(int index, string labelKey, BaseScreenComponent.OnMouseClickHandler onMouseClickHandler, Button button = null)
        {
            if (button == null)
                button = new Button();
            button.Size = new Vector2((mainPanel.Size.x - 10) / 4f, 10);
            button.Position = new Vector2(button.Size.x * index, 0);
            button.VerticalAlignment = VerticalAlignment.Middle;
            button.Label.Text = ModManager.GetText(labelKey);
            button.Label.Font = DaggerfallUI.Instance.Font3;
            button.Label.ShadowColor = Color.clear;
            button.Label.TextColor = enabledButtonsColor;
            button.OnMouseClick += onMouseClickHandler;
            controlPanel.Components.Add(button);
        }

        private void ToggleButtons(bool toggle, params Button[] buttons)
        {
            foreach (Button button in buttons)
            {
                button.Label.TextColor = toggle ? enabledButtonsColor : Color.grey;
                button.Tag = toggle;
            }
        }

        #endregion

        #region Event Handlers

        private void CancelButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void ListBox_OnSelectItem()
        {
            if (creationMode)
                SetCreationMode(false);

            Preset preset = SelectedPreset;
            descriptionLabel.Text = preset.Description;
            authorLabel.Text = !string.IsNullOrEmpty(preset.Author) ? string.Format("{0}: {1}", ModManager.GetText("author"), preset.Author) : string.Empty;
            versionLabel.Text = settings.IsCompatible(preset) ?
                string.Empty : string.Format("{0} ({1}/{2})", ModManager.GetText("versionMismatch"), preset.SettingsVersion, settings.Version);

            ToggleButtons(true, newPresetButton, loadButton);
            ToggleButtons(preset.IsLocal, saveButton, deleteButton);
        }

        private void AddPresetButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!(bool)newPresetButton.Tag)
                return;

            ToggleButtons(true, saveButton);
            ToggleButtons(false, newPresetButton, loadButton, deleteButton);
            SetCreationMode(true);
        }

        private void LoadButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!(bool)loadButton.Tag)
                return;

            settings.ApplyPreset(SelectedPreset);
            if (ApplyChangesCallback != null)
                ApplyChangesCallback();
            CloseWindow();
        }

        private void SaveButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!(bool)saveButton.Tag)
                return;

            if (creationMode)
            {
                // Request valid title and description
                if (string.IsNullOrEmpty(creatorTitle.Text) ||
                    string.IsNullOrEmpty(creatorDescription.Text))
                    return;

                // New preset from current values
                var preset = new Preset()
                {
                    Title = creatorTitle.ResultText,
                    Description = creatorDescription.ResultText,
                    SettingsVersion = settings.Version,
                    IsLocal = true
                };
                settings.FillPreset(preset, true);
                AddPreset(preset);
                SetCreationMode(false);
                ListBox_OnSelectItem();
            }
            else
            {
                settings.FillPreset(SelectedPreset, true);  
            }

            writeToDiskFlag = true;
        }

        private void DeleteButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!(bool)deleteButton.Tag)
                return;

            listBox.RemoveItem(listBox.SelectedIndex);
            scrollBar.TotalUnits--;
            settings.Presets.RemoveAt(listBox.SelectedIndex);
            writeToDiskFlag = true;

            if (listBox.Count == 0)
            {
                ToggleButtons(true, newPresetButton);
                ToggleButtons(false, loadButton, saveButton, deleteButton);
            }
        }

        private void CreatorTitle_OnMouseLeave(BaseScreenComponent sender)
        {
            if (settings.Presets.Any(x => x.Title == creatorTitle.Text) ||
                string.IsNullOrEmpty(creatorTitle.Text))
                creatorTitle.BackgroundColor = Color.red;
            else if (creatorTitle.BackgroundColor != Color.clear)
                creatorTitle.BackgroundColor = Color.clear;
        }

        private void CreatorDescription_OnMouseLeave(BaseScreenComponent sender)
        {
            if (string.IsNullOrEmpty(creatorDescription.Text))
                creatorDescription.BackgroundColor = Color.red;
            else if (creatorDescription.BackgroundColor != Color.clear)
                creatorDescription.BackgroundColor = Color.clear;
        }

        #endregion
    }
}