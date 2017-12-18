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
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// Defines a preset for a mod.
    /// </summary>
    public struct Preset
    {
        public string Title;            // Title (does not correspond to filename).
        public string Description;      // Short description.
        public string Author;           // Optional field (for imported presets).
        public string Version;          // Version of target settings (not version of preset!)
    }

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

        readonly string targetVersion;

        Panel mainPanel                 = new Panel();
        Panel infoPanel                 = new Panel();
        Panel creatorPanel              = new Panel();

        ListBox listBox                 = new ListBox();
        List<Preset> presets            = new List<Preset>();
        TextLabel descriptionLabel      = new TextLabel();
        TextLabel authorLabel           = new TextLabel();
        TextLabel versionLabel          = new TextLabel();
        Paginator paginator             = new Paginator();
        TextBox creatorTitle            = new TextBox();
        TextBox creatorDescription      = new TextBox();

        Color mainBackgroundColor       = new Color(0, 0, 0, 0.7f);
        Color infoBackgroundColor       = new Color(0, 0.8f, 0, 0.1f);
        Color creatorBackgroundColor    = new Color(0, 0, 1, 0.1f);
        Color titleColor                = new Color(0.53f, 0.81f, 0.98f, 1);
        Color selectedTitleColor        = Color.blue;
        Color warningColor              = new Color(1, 0, 0, 0.4f);

        bool creationMode = false;

        #endregion

        #region Constructors

        public PresetPicker(IUserInterfaceManager uiManager, DaggerfallBaseWindow previousWindow, string targetVersion)
            : base(uiManager, previousWindow)
        {
            this.targetVersion = targetVersion;
        }

        #endregion

        #region Overrides

        protected override void Setup()
        {
            base.Setup();

            mainPanel.Size = new Vector2(windowWidth, 120);
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundColor = mainBackgroundColor;
            mainPanel.Outline.Enabled = true;
            NativePanel.Components.Add(mainPanel);

            listBox.Position = new Vector2(5, 5);
            listBox.Size = new Vector2(windowWidth, 100);
            listBox.OnSelectItem += ListBox_OnSelectItem;
            mainPanel.Components.Add(listBox);

            infoPanel.Position = new Vector2(0, 80);
            infoPanel.Size = new Vector2(windowWidth, 20);
            infoPanel.BackgroundColor = infoBackgroundColor;
            infoPanel.Outline.Enabled = false;
            infoPanel.HorizontalAlignment = HorizontalAlignment.Center;
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
            controlPanel.Outline.Enabled = false;
            controlPanel.HorizontalAlignment = HorizontalAlignment.Center;
            controlPanel.Position = new Vector2(0, 100);
            controlPanel.Size = new Vector2(mainPanel.Size.x, 20);
            mainPanel.Components.Add(controlPanel);

            paginator.Size = new Vector2(30, 5);
            paginator.VerticalAlignment = VerticalAlignment.Middle;
            paginator.Position = new Vector2(10, 0);
            paginator.TextColor = titleColor;
            paginator.ArrowColor = Color.blue;
            paginator.DisabledArrowColor = new Color(0.28f, 0.24f, 0.55f);
            paginator.OnSelected += Paginator_OnSelected;
            controlPanel.Components.Add(paginator);

            var cancelButton = new Button();
            cancelButton.Size = new Vector2(30, 10);
            cancelButton.Position = new Vector2(70, 0);
            cancelButton.VerticalAlignment = VerticalAlignment.Middle;
            cancelButton.Label.Text = "Cancel";
            cancelButton.Label.Font = DaggerfallUI.Instance.Font3;
            cancelButton.Label.ShadowColor = Color.clear;
            cancelButton.Label.TextColor = Color.blue;
            cancelButton.OnMouseClick += CancelButton_OnMouseClick;
            controlPanel.Components.Add(cancelButton);

            var applyButton = new Button();
            applyButton.Size = new Vector2(30, 10);
            applyButton.Position = new Vector2(100, 0);
            applyButton.VerticalAlignment = VerticalAlignment.Middle;
            applyButton.Label.Text = "Apply";
            applyButton.Label.Font = DaggerfallUI.Instance.Font3;
            applyButton.Label.ShadowColor = Color.clear;
            applyButton.Label.TextColor = Color.blue;
            applyButton.OnMouseClick += ApplyButton_OnMouseClick;
            controlPanel.Components.Add(applyButton);

            creatorPanel.BackgroundColor = creatorBackgroundColor;
            creatorPanel.Outline.Enabled = false;
            creatorPanel.HorizontalAlignment = HorizontalAlignment.Center;
            creatorPanel.Position = infoPanel.Position;
            creatorPanel.Size = infoPanel.Size;
            creatorPanel.Enabled = false;
            mainPanel.Components.Add(creatorPanel);

            creatorTitle.Size = new Vector2(30, 10);
            creatorTitle.Position = new Vector2(5, 2);
            creatorTitle.DefaultText = "<title>";
            creatorTitle.MaxCharacters = titleMaxChars;
            creatorTitle.UseFocus = true;
            creatorTitle.OnMouseLeave += CreatorTitle_OnMouseLeave;
            creatorPanel.Components.Add(creatorTitle);

            creatorDescription.Size = new Vector2(100, 10);
            creatorDescription.Position = new Vector2(5, 10);
            creatorDescription.DefaultText = "<description>";
            creatorDescription.MaxCharacters = descriptionMaxChars;
            creatorDescription.UseFocus = true;
            creatorPanel.Components.Add(creatorDescription);
        }

        public override void OnPush()
        {
            base.OnPush();

            ListBox.ListItem itemOut;
            AddPreset("<New Preset>", "Create a new presets with current settings.", null, targetVersion, out itemOut);
            itemOut.textLabel.TextScale = 0.6f;
            itemOut.textLabel.Font = DaggerfallUI.Instance.Font4;
            itemOut.textColor = Color.green;
            itemOut.selectedTextColor = selectedTitleColor;
            itemOut.shadowColor = Color.clear;

            ListBox_OnSelectItem();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a preset to the listbox.
        /// </summary>
        public void AddPreset(string title, string description, string author, string version)
        {
            ListBox.ListItem itemOut;
            AddPreset(title, description, author, version, out itemOut);
            itemOut.textLabel.Font = DaggerfallUI.Instance.Font4;
            itemOut.textColor = version == targetVersion ? titleColor : warningColor;
            itemOut.selectedTextColor = selectedTitleColor;
            itemOut.shadowColor = Color.clear;
        }

        #endregion

        #region Private Methods

        private void AddPreset(string title, string description, string author, string version, out ListBox.ListItem itemOut)
        {
            var preset = new Preset
            {
                Title = !string.IsNullOrEmpty(title) ? title : "<Unknown>",
                Description = !string.IsNullOrEmpty(description) ? description : "<Missing description>",
                Author = author, //optional
                Version = version,
            };
            presets.Add(preset);

            listBox.AddItem(preset.Title, out itemOut);
            paginator.Total = listBox.Count;
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
                creatorTitle.Text = presets[listBox.SelectedIndex].Title;
                creatorDescription.Text = presets[listBox.SelectedIndex].Description;
            }
            else
            {
                descriptionLabel.Text = presets[listBox.SelectedIndex].Description;

                string author = presets[listBox.SelectedIndex].Author;
                authorLabel.Text = !string.IsNullOrEmpty(author) ? string.Format("Author: {0}", author) : string.Empty;

                string version = presets[listBox.SelectedIndex].Version;
                if (version != targetVersion)
                    versionLabel.Text = string.Format("Version mismatch! ({0}/{1})", version, targetVersion);
                else
                    versionLabel.Text = string.Empty;
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
            if (listBox.SelectedIndex == listBox.Count - 1)
            {
                if (creationMode)
                {
                    RaiseOnCreatePresetEvent(new Preset()
                    {
                        Title = creatorTitle.ResultText,
                        Description = creatorDescription.ResultText,
                        Author = "local",
                        Version = targetVersion
                    });

                    SetCreationMode(false);
                    CloseWindow();
                }
                else
                {
                    SetCreationMode(true);
                }
            }
            else
            {
                RaiseOnPresetPickedEvent();
                CloseWindow();
            }
        }

        private void CreatorTitle_OnMouseLeave(BaseScreenComponent sender)
        {
            if (presets.Any(x => x.Title == creatorTitle.Text))
                creatorTitle.BackgroundColor = Color.red;
            else if (creatorTitle.BackgroundColor != Color.clear)
                creatorTitle.BackgroundColor = Color.clear;
        }

        public delegate void OnPresetPickedEventHandler(int index);
        public event OnPresetPickedEventHandler OnPresetPicked;
        void RaiseOnPresetPickedEvent()
        {
            if (OnPresetPicked != null)
                OnPresetPicked(listBox.SelectedIndex);
        }

        public delegate void OnCreatePresetEventHandler(Preset preset);
        public event OnCreatePresetEventHandler OnCreatePreset;
        void RaiseOnCreatePresetEvent(Preset preset)
        {
            if (OnCreatePreset != null)
                OnCreatePreset(preset);
        }

        #endregion
    }
}