// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using System.IO;
using UnityEngine;
using DaggerfallWorkshop.Game.Serialization;
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
        #region Types

        private sealed class PresetCreator : DaggerfallPopupWindow
        {
            readonly TextBox titleTextBox = new TextBox();
            readonly TextBox descriptionTextBox = new TextBox();
            readonly TextBox authorTextBox = new TextBox();
            readonly Action<string, string, string> createPresetCallback;

            public PresetCreator(IUserInterfaceManager uiManager, Action<string, string, string> createPresetCallback)
                : base(uiManager)
            {
                if (createPresetCallback == null)
                    throw new ArgumentNullException("createPresetCallback");

                this.createPresetCallback = createPresetCallback;
            }

            protected override void Setup()
            {
                ParentPanel.BackgroundColor = Color.clear;
                Panel mainPanel = new Panel();
                mainPanel.Size = new Vector2(windowWidth, 120 + 10);
                mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
                mainPanel.VerticalAlignment = VerticalAlignment.Middle;
                mainPanel.BackgroundColor = new Color(0, 0, 0, 0.7f);
                mainPanel.Outline.Enabled = true;
                NativePanel.Components.Add(mainPanel);

                mainPanel.Components.Add(MakeCancelButton(uiManager, "presets"));
                mainPanel.Components.Add(MakeTitleLabel("presetCreator"));

                var creatorPanel = new Panel();
                creatorPanel.Outline.Enabled = false;
                creatorPanel.HorizontalAlignment = HorizontalAlignment.Center;
                creatorPanel.VerticalAlignment = VerticalAlignment.Middle;
                creatorPanel.Size = new Vector2(140, 60);
                mainPanel.Components.Add(creatorPanel);

                creatorPanel.Components.Add(MakeLabelledTextBox(titleTextBox, ModManager.GetText("title"), VerticalAlignment.Top));
                creatorPanel.Components.Add(MakeLabelledTextBox(descriptionTextBox, ModManager.GetText("description"), VerticalAlignment.Middle));
                creatorPanel.Components.Add(MakeLabelledTextBox(authorTextBox, ModManager.GetText("author"), VerticalAlignment.Bottom));

                mainPanel.Components.Add(MakeSimpleButton("ok", null, null, new Vector2(0, 110), ConfirmButton_OnMouseClick));
            }

            private Panel MakeLabelledTextBox(TextBox textBox, string text, VerticalAlignment verticalAlignment)
            {
                var panel = new Panel();
                panel.Size = new Vector2(140, 15);
                panel.HorizontalAlignment = HorizontalAlignment.Center;
                panel.VerticalAlignment = verticalAlignment;
                panel.Outline.Enabled = false;

                var textLabel = new TextLabel();
                textLabel.Size = new Vector2(100, 10);
                textLabel.HorizontalAlignment = HorizontalAlignment.Left;
                textLabel.VerticalAlignment = VerticalAlignment.Middle;
                textLabel.TextScale = 0.7f;
                textLabel.Text = text;
                textLabel.ShadowColor = Color.clear;
                panel.Components.Add(textLabel);

                textBox.Size = new Vector2(100, 10);
                textBox.FixedSize = true;
                textBox.HorizontalAlignment = HorizontalAlignment.Right;
                textBox.VerticalAlignment = VerticalAlignment.Middle;
                textBox.UseFocus = true;
                textBox.Outline.Enabled = true;
                panel.Components.Add(textBox);

                return panel;
            }

            private void ConfirmButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
            {
                if (string.IsNullOrEmpty(titleTextBox.ResultText) || string.IsNullOrEmpty(descriptionTextBox.ResultText))
                {
                    var messageBox = new DaggerfallMessageBox(uiManager, this);
                    messageBox.AllowCancel = true;
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.SetText(ModManager.GetText("titleOrDescriptionMissing"));
                    uiManager.PushWindow(messageBox);
                }
                else
                {
                    createPresetCallback(titleTextBox.ResultText, descriptionTextBox.ResultText, authorTextBox.ResultText);
                    uiManager.PopWindow();
                }
            }
        }

        #endregion

        #region Fields

        const float windowWidth = 150;

        readonly Mod mod;
        readonly ModSettingsData settings;

        readonly Panel sideBarPanel = new Panel();

        readonly ListBox listBox = new ListBox();
        readonly VerticalScrollBar scrollBar = new VerticalScrollBar();
        readonly TextLabel descriptionLabel = new TextLabel();
        readonly TextLabel authorLabel = new TextLabel();
        readonly TextLabel versionLabel = new TextLabel();

        readonly Button newPresetButton = new Button();
        readonly Button loadButton = new Button();
        readonly Button saveButton = new Button();
        readonly Button deleteButton = new Button();

        readonly Color mainBackgroundColor = new Color(0, 0, 0, 0.7f);
        readonly Color infoBackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        readonly Color titleColor = Color.gray;
        readonly Color selectedTitleColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        readonly Color warningColor = new Color(1, 0, 0, 0.4f);
        readonly Color enabledButtonColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        readonly Color disabledButtonColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);

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

            var mainPanel = new Panel();
            mainPanel.Size = new Vector2(windowWidth, 120 + 10);
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundColor = mainBackgroundColor;
            mainPanel.Outline.Enabled = true;
            NativePanel.Components.Add(mainPanel);

            mainPanel.Components.Add(MakeTitleLabel("presets"));
            mainPanel.Components.Add(MakeCancelButton(uiManager, "settings"));

            listBox.Position = new Vector2(5, 12);
            listBox.Size = new Vector2((150 / 3 * 2) - 10, 80);
            listBox.BackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            listBox.OnScroll += () => scrollBar.ScrollIndex = listBox.ScrollIndex;
            listBox.OnSelectItem += ListBox_OnSelectItem;
            mainPanel.Components.Add(listBox);

            scrollBar.Size = new Vector2(5, listBox.Size.y);
            scrollBar.Position = new Vector2(listBox.Size.x + 5, listBox.Position.y);
            scrollBar.BackgroundColor = Color.grey;
            scrollBar.DisplayUnits = 9;
            scrollBar.OnScroll += () => listBox.ScrollIndex = scrollBar.ScrollIndex;
            mainPanel.Components.Add(scrollBar);

            var infoPanel = new Panel();
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

            sideBarPanel.Size = new Vector2(150 / 3, listBox.Size.y);
            sideBarPanel.Position = new Vector2(0, listBox.Position.y);
            sideBarPanel.HorizontalAlignment = HorizontalAlignment.Right;
            sideBarPanel.BackgroundColor = Color.clear;
            sideBarPanel.Outline.Enabled = false;
            mainPanel.Components.Add(sideBarPanel);

            SetupSideBarButton(0, "new", "presetNewInfo", AddPresetButton_OnMouseClick, newPresetButton);
            SetupSideBarButton(1, "load", "presetLoadInfo", LoadButton_OnMouseClick, loadButton);
            SetupSideBarButton(2, "save", "presetSaveInfo", SaveButton_OnMouseClick, saveButton);
            SetupSideBarButton(3, "delete", "presetDeleteInfo", DeleteButton_OnMouseClick, deleteButton);
            SetupSideBarButton(4, "export", "presetExportInfo", ExportButton_OnMouseClick);

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

        private void SetupSideBarButton(int index, string labelKey, string toolTipKey, BaseScreenComponent.OnMouseClickHandler onMouseClickHandler, Button button = null)
        {
            const int buttonsCount = 5;
            sideBarPanel.Components.Add(MakeSimpleButton(labelKey, defaultToolTip, toolTipKey, new Vector2(0, sideBarPanel.Size.y / buttonsCount * index), onMouseClickHandler, button));
        }

        private void ToggleButtons(bool toggle, params Button[] buttons)
        {
            foreach (Button button in buttons)
            {
                button.BackgroundColor = toggle ? enabledButtonColor : disabledButtonColor;
                button.Tag = toggle;
            }
        }

        private void CreatePreset(string title, string description, string author)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
                return;

            var preset = new Preset()
            {
                Title = title,
                Description = description,
                Author = !string.IsNullOrEmpty(author) ? author : null,
                SettingsVersion = settings.Version,
                IsLocal = true
            };
            settings.FillPreset(preset, true);
            AddPreset(preset);
            ListBox_OnSelectItem();

            writeToDiskFlag = true;
        }

        #endregion

        #region Event Handlers

        private void CancelButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void ListBox_OnSelectItem()
        {
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

            uiManager.PushWindow(new PresetCreator(uiManager, CreatePreset));
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

            settings.FillPreset(SelectedPreset, true);
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

        private void ExportButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Preset preset = settings.Presets[listBox.SelectedIndex];
            string fileName = string.Join("_", preset.Title.Split(Path.GetInvalidFileNameChars())) + ".json";

            string dirPath = Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, "Mods", "ExportedPresets", mod.FileName);
            string filePath = Path.Combine(dirPath, fileName);

            var messageBox = new DaggerfallMessageBox(uiManager, this, true);
            messageBox.AllowCancel = true;
            messageBox.SetText(string.Format(ModManager.GetText("exportMessage"), preset.Title, filePath));
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.OK, true);
            messageBox.OnButtonClick += (source, messageBoxButton) =>
            {
                Directory.CreateDirectory(dirPath);
                File.WriteAllText(filePath, SaveLoadManager.Serialize(typeof(Preset), preset, true));
                source.PopWindow();
            };
            uiManager.PushWindow(messageBox);
        }

        #endregion

        #region Static Methods

        private static TextLabel MakeTitleLabel(string labelKey)
        {
            var titleLabel = new TextLabel();
            titleLabel.Size = new Vector2(60, 12);
            titleLabel.Position = new Vector2(0, 5);
            titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            titleLabel.HorizontalTextAlignment = TextLabel.HorizontalTextAlignmentSetting.Center;
            titleLabel.Text = ModManager.GetText(labelKey);
            return titleLabel;
        }

        private static Button MakeCancelButton(IUserInterfaceManager uiManager, string labelKey)
        {
            var cancelButton = new Button();
            cancelButton.Size = new Vector2(20, 10);
            cancelButton.Position = new Vector2(0, 0);
            cancelButton.Label.Text = string.Format("< {0}", ModManager.GetText(labelKey));
            cancelButton.Label.Font = DaggerfallUI.Instance.Font3;
            cancelButton.Label.ShadowColor = Color.clear;
            cancelButton.Label.TextColor = Color.grey;
            cancelButton.Label.TextScale = 0.8f;
            cancelButton.OnMouseClick += (sender, position) => uiManager.PopWindow();
            return cancelButton;
        }

        private static Button MakeSimpleButton(string labelKey, ToolTip toolTip, string toolTipKey, Vector2 position, BaseScreenComponent.OnMouseClickHandler onMouseClickHandler, Button button = null)
        {
            if (button == null)
                button = new Button();

            button.Size = new Vector2(40, 12);
            button.Position = position;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.Outline.Enabled = true;
            button.BackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
            button.Label.Text = ModManager.GetText(labelKey);
            if (toolTip != null)
            {
                button.Label.ToolTip = toolTip;
                button.Label.ToolTipText = ModManager.GetText(toolTipKey);
            }
            button.OnMouseClick += onMouseClickHandler;
            return button;
        }

        #endregion
    }
}