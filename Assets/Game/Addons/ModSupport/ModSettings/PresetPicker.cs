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

        ListBox listBox                 = new ListBox();
        TextLabel descriptionLabel      = new TextLabel();
        TextLabel authorLabel           = new TextLabel();
        TextLabel versionLabel          = new TextLabel();
        Paginator paginator             = new Paginator();
        TextBox creatorTitle            = new TextBox();
        TextBox creatorDescription      = new TextBox();
        Button saveButton               = new Button();
        Button deleteButton             = new Button();

        Color titleBackgroundColor      = new Color(0, 0.27f, 0.36f, 0.5f);
        Color mainBackgroundColor       = new Color(0, 0.38f, 0.35f, 0.1f); 
        Color infoBackgroundColor       = new Color(0, 0.8f, 0, 0.1f);
        Color creatorBackgroundColor    = new Color(0, 0, 1, 0.1f);
        Color titleColor                = Color.gray;
        Color selectedTitleColor        = new Color(0, 0.75f, 0.65f, 1);
        Color warningColor              = new Color(1, 0, 0, 0.4f);

        bool creationMode = false;
        bool writeToDiskFlag = false;

        #endregion

        #region Properties

        bool CreationModeSelected
        {
            get { return listBox.SelectedIndex == listBox.Count - 1; }
        }

        Preset SelectedPreset
        {
            get { return settings.Presets[listBox.SelectedIndex]; }
        }

        public Action ApplyChangesCallback { private get; set; }

        #endregion

        #region Constructors

        public PresetPicker(IUserInterfaceManager uiManager, DaggerfallBaseWindow previousWindow, Mod mod, ModSettingsData settings)
            : base(uiManager, previousWindow)
        {
            this.mod = mod;
            this.settings = settings;
        }

        #endregion

        #region Overrides

        protected override void Setup()
        {
            base.Setup();

            mainPanel.Size = new Vector2(windowWidth, 120 + 10);
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundColor = mainBackgroundColor;
            mainPanel.Outline.Enabled = true;
            NativePanel.Components.Add(mainPanel);

            var titlePanel = new Panel();
            titlePanel.Position = new Vector2(0, 0);
            titlePanel.Size = new Vector2(windowWidth, 10);
            titlePanel.BackgroundColor = titleBackgroundColor;
            titlePanel.Outline.Enabled = true;
            titlePanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.Components.Add(titlePanel);

            var titleLabel = new TextLabel(DaggerfallUI.Instance.Font3);
            titleLabel.Position = new Vector2(5, 0);
            titleLabel.VerticalAlignment = VerticalAlignment.Middle;
            titleLabel.Text = ModManager.GetText("presets");
            titleLabel.TextColor = new Color(0.88f, 0.95f, 0.95f, 1);
            titleLabel.ShadowColor = Color.clear;
            titlePanel.Components.Add(titleLabel);

            var helpButton = new Button();
            helpButton.Size = new Vector2(10, 10);
            helpButton.Position = new Vector2(titlePanel.Size.x - 25, 0);
            helpButton.VerticalAlignment = VerticalAlignment.Middle;
            helpButton.Label.Text = "?";
            helpButton.Label.ShadowColor = Color.clear;
            helpButton.Label.TextColor = Color.white;
            helpButton.Label.TextScale = 0.8f;
            helpButton.ToolTip = defaultToolTip;
            helpButton.ToolTipText = ModManager.GetText("presetsInfo");
            titlePanel.Components.Add(helpButton);

            var cancelButton = new Button();
            cancelButton.Size = new Vector2(15, 10);
            cancelButton.HorizontalAlignment = HorizontalAlignment.Right;
            cancelButton.VerticalAlignment = VerticalAlignment.Middle;
            cancelButton.Label.Text = "X";
            cancelButton.Label.Font = DaggerfallUI.Instance.Font3;
            cancelButton.Label.ShadowColor = Color.clear;
            cancelButton.Label.TextColor = Color.white;
            cancelButton.OnMouseClick += CancelButton_OnMouseClick;
            titlePanel.Components.Add(cancelButton);

            listBox.Position = new Vector2(5, 15);
            listBox.Size = new Vector2(140, 80);
            listBox.OnSelectItem += ListBox_OnSelectItem;
            mainPanel.Components.Add(listBox);

            infoPanel.Position = new Vector2(0, 100);
            infoPanel.Size = new Vector2(windowWidth, 20);
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

            Panel controlPanel = new Panel();
            controlPanel.BackgroundColor = Color.clear;
            controlPanel.HorizontalAlignment = HorizontalAlignment.Center;
            controlPanel.VerticalAlignment = VerticalAlignment.Bottom;
            controlPanel.Size = new Vector2(mainPanel.Size.x, 10);
            controlPanel.SetMargins(Margins.All, 1);
            mainPanel.Components.Add(controlPanel);

            paginator.Size = new Vector2(30, 5);
            paginator.VerticalAlignment = VerticalAlignment.Middle;
            paginator.Position = new Vector2(10, 0);
            paginator.TextColor = titleColor;
            paginator.ArrowColor = Color.blue;
            paginator.DisabledArrowColor = new Color(0.28f, 0.24f, 0.55f);
            paginator.OnSelected += Paginator_OnSelected;
            controlPanel.Components.Add(paginator);

            var loadButton = new Button();
            loadButton.Size = new Vector2(30, 10);
            loadButton.Position = new Vector2(60, 0);
            loadButton.VerticalAlignment = VerticalAlignment.Middle;
            loadButton.Label.Text = ModManager.GetText("load");
            loadButton.Label.Font = DaggerfallUI.Instance.Font3;
            loadButton.Label.ShadowColor = Color.clear;
            loadButton.Label.TextColor = Color.blue;
            loadButton.OnMouseClick += ApplyButton_OnMouseClick;
            controlPanel.Components.Add(loadButton);

            saveButton.Size = new Vector2(10, 10);
            saveButton.Position = new Vector2(100, 0);
            saveButton.VerticalAlignment = VerticalAlignment.Middle;
            saveButton.Label.Text = ModManager.GetText("save");
            saveButton.Label.Font = DaggerfallUI.Instance.Font3;
            saveButton.Label.ShadowColor = Color.clear;
            saveButton.Label.TextColor = Color.blue;
            saveButton.OnMouseClick += OverwriteButton_OnMouseClick;
            controlPanel.Components.Add(saveButton);

            deleteButton.Size = new Vector2(10, 10);
            deleteButton.Label.Text = "X";
            deleteButton.Label.Font = DaggerfallUI.Instance.Font3;
            deleteButton.Label.ShadowColor = Color.clear;
            deleteButton.Label.TextColor = Color.blue;
            deleteButton.OnMouseClick += DeleteButton_OnMouseClick;
            mainPanel.Components.Add(deleteButton);

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
            creatorPanel.Components.Add(creatorDescription);
        }

        public override void OnPush()
        {
            base.OnPush();

            foreach (Preset preset in settings.Presets)
                RegisterPreset(preset);
            AddPresetCreator();

            paginator.Total = listBox.Count;
            ListBox_OnSelectItem();
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
        private void RegisterPreset(Preset preset, int position = -1)
        {
            string title = preset.Title;
            preset.Title = !string.IsNullOrEmpty(preset.Title) ?
                 mod.TryLocalize("Presets", title, "Title") ?? preset.Title : ModManager.GetText("missingTitle");
            preset.Description = !string.IsNullOrEmpty(preset.Description) ?
                mod.TryLocalize("Presets", title, "Description") ?? preset.Description : ModManager.GetText("missingDescription");

            ListBox.ListItem itemOut;
            listBox.AddItem(preset.Title, out itemOut, position);
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
            RegisterPreset(preset, settings.Presets.Count -1);
            paginator.Total += 1;
        }

        /// <summary>
        /// Add a fictional item on last position that allows to create new presets.
        /// </summary>
        private void AddPresetCreator()
        {
            ListBox.ListItem itemOut;
            listBox.AddItem(ModManager.GetText("newPreset"), out itemOut);
            itemOut.textLabel.TextScale = 0.6f;
            itemOut.textLabel.Font = DaggerfallUI.Instance.Font4;
            itemOut.textColor = Color.green;
            itemOut.selectedTextColor = selectedTitleColor;
            itemOut.shadowColor = Color.clear;
        }

        private void SetCreationMode(bool toggle)
        {
            creatorPanel.Enabled = toggle;
            infoPanel.Enabled = !toggle;
            creationMode = toggle;

            if (!toggle)
                creatorDescription.Text = creatorTitle.Text = string.Empty;
        }

        #endregion

        #region Event Handlers

        private void ListBox_OnSelectItem()
        {
            if (creationMode)
            {
                Preset preset = SelectedPreset;
                creatorTitle.Text = preset.Title;
                creatorDescription.Text = preset.Description;

                deleteButton.Enabled = saveButton.Enabled = false;
            }
            else if (CreationModeSelected)
            {
                descriptionLabel.Text = ModManager.GetText("newPresetInfo");
                authorLabel.Text = string.Empty;
                versionLabel.Text = string.Empty;

                deleteButton.Enabled = saveButton.Enabled = false;
            }
            else
            {
                Preset preset = SelectedPreset;
                descriptionLabel.Text = preset.Description;
                authorLabel.Text = !string.IsNullOrEmpty(preset.Author) ? string.Format("{0}: {1}", ModManager.GetText("author"), preset.Author) : string.Empty;
                versionLabel.Text = settings.IsCompatible(preset) ?
                    string.Empty : string.Format("{0} ({1}/{2})", ModManager.GetText("versionMismatch"), preset.SettingsVersion, settings.Version);

                if (deleteButton.Enabled = saveButton.Enabled = preset.IsLocal)
                {
                    Rect rect = listBox.GetItem(listBox.SelectedIndex).textLabel.Rectangle;
                    Vector2 position = mainPanel.ScreenToLocal(rect.position);
                    deleteButton.Position = new Vector2(windowWidth - 15, position.y);
                }
            }
            paginator.Sync(listBox.SelectedIndex);
        }

        private void Paginator_OnSelected(int previous, int selected)
        {
            listBox.SelectedIndex = paginator.Selected;
        }

        private void CancelButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (creationMode)
            {
                SetCreationMode(false);
                ListBox_OnSelectItem();
            }
            else
            {
                CloseWindow();
            }
        }

        private void ApplyButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (CreationModeSelected)
            {
                if (creationMode)
                {
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
                    writeToDiskFlag = true;
                }
                else
                {
                    // Open editor
                    SetCreationMode(true);
                }
            }
            else
            {
                // Apply preset and close
                // TODO: preview ?
                settings.ApplyPreset(SelectedPreset);
                if (ApplyChangesCallback != null)
                    ApplyChangesCallback();
                CloseWindow();
            }
        }

        private void OverwriteButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            settings.FillPreset(SelectedPreset, true);
            writeToDiskFlag = true;
        }

        private void DeleteButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            listBox.RemoveItem(listBox.SelectedIndex);
            settings.Presets.RemoveAt(listBox.SelectedIndex);
            writeToDiskFlag = true;
        }

        private void CreatorTitle_OnMouseLeave(BaseScreenComponent sender)
        {
            if (settings.Presets.Any(x => x.Title == creatorTitle.Text))
                creatorTitle.BackgroundColor = Color.red;
            else if (creatorTitle.BackgroundColor != Color.clear)
                creatorTitle.BackgroundColor = Color.clear;
        }

        #endregion
    }
}