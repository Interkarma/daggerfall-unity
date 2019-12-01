// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Save;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Daggerfall Unity save game interface.
    /// </summary>
    public class DaggerfallUnitySaveGameWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Vector2 mainPanelSize = new Vector2(280, 170);

        #endregion

        #region UI Controls

        const string savePromptText = "Save Game";
        const string loadPromptText = "Load Game";
        const string saveButtonText = "Save";
        const string loadButtonText = "Load";

        Panel mainPanel = new Panel();
        Panel screenshotPanel = new Panel();
        TextBox saveNameTextBox = new TextBox();
        TextLabel promptLabel = new TextLabel();
        TextLabel saveTimeLabel = new TextLabel();
        TextLabel gameTimeLabel = new TextLabel();
        TextLabel saveVersionLabel = new TextLabel();
        TextLabel saveFolderLabel = new TextLabel();
        ListBox savesList = new ListBox();
        Button deleteSaveButton = new Button();
        Button goButton = new Button();
        Button switchCharButton = new Button();
        Button switchClassicButton = new Button();

        Color mainPanelBackgroundColor = new Color(0.0f, 0f, 0.0f, 1.0f);
        Color namePanelBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        Color saveButtonBackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        Color cancelButtonBackgroundColor = new Color(0.7f, 0.0f, 0.0f, 0.4f);
        Color savesListBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
        Color savesListTextColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        Color saveFolderColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        VerticalScrollBar savesScroller = new VerticalScrollBar();

        Modes mode = Modes.SaveGame;
        string currentPlayerName;
        bool displayMostRecentChar;

        #endregion

        #region Properties

        public Modes Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        #endregion

        #region Constructors

        public DaggerfallUnitySaveGameWindow(IUserInterfaceManager uiManager, Modes mode, DaggerfallBaseWindow previous = null, bool displayMostRecentChar = false)
            : base(uiManager, previous)
        {
            this.mode = mode;
            this.displayMostRecentChar = displayMostRecentChar;
        }

        #endregion

        #region Enums

        public enum Modes
        {
            SaveGame,
            LoadGame,
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Main panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.Size = mainPanelSize;
            mainPanel.Outline.Enabled = true;
            SetBackground(mainPanel, mainPanelBackgroundColor, "mainPanelBackgroundColor");
            NativePanel.Components.Add(mainPanel);

            // Prompt
            promptLabel.ShadowPosition = Vector2.zero;
            promptLabel.Position = new Vector2(4, 4);
            mainPanel.Components.Add(promptLabel);

            // Name panel
            Panel namePanel = new Panel();
            namePanel.Position = new Vector2(4, 12);
            namePanel.Size = new Vector2(272, 9);
            namePanel.Outline.Enabled = true;
            SetBackground(namePanel, namePanelBackgroundColor, "namePanelBackgroundColor");
            mainPanel.Components.Add(namePanel);

            // Name input
            saveNameTextBox.Position = new Vector2(2, 2);
            saveNameTextBox.MaxCharacters = 26;
            saveNameTextBox.OnType += SaveNameTextBox_OnType;
            namePanel.Components.Add(saveNameTextBox);

            // Save panel
            Panel savesPanel = new Panel();
            savesPanel.Position = new Vector2(4, 25);
            savesPanel.Size = new Vector2(100, 141);
            savesPanel.Outline.Enabled = true;
            mainPanel.Components.Add(savesPanel);

            // Save list
            savesList.Position = new Vector2(2, 2);
            savesList.Size = new Vector2(91, 129);
            savesList.TextColor = savesListTextColor;
            SetBackground(savesList, savesListBackgroundColor, "savesListBackgroundColor");
            savesList.ShadowPosition = Vector2.zero;
            savesList.RowsDisplayed = 16;
            savesList.OnScroll += SavesList_OnScroll;
            savesList.OnSelectItem += SavesList_OnSelectItem;
            savesList.OnMouseDoubleClick += SaveLoadEventHandler;
            savesPanel.Components.Add(savesList);

            // Save scroller
            savesScroller.Position = new Vector2(94, 2);
            savesScroller.Size = new Vector2(5, 129);
            savesScroller.DisplayUnits = 16;
            savesScroller.OnScroll += SavesScroller_OnScroll;
            savesPanel.Components.Add(savesScroller);

            // Save/Load button
            goButton.Position = new Vector2(108, 150);
            goButton.Size = new Vector2(40, 16);
            goButton.Label.ShadowColor = Color.black;
            SetBackground(goButton, saveButtonBackgroundColor, "saveButtonBackgroundColor");
            goButton.Outline.Enabled = true;
            goButton.OnMouseClick += SaveLoadEventHandler;
            mainPanel.Components.Add(goButton);

            // Switch to classic save window button
            switchClassicButton.Position = new Vector2(172, 150);
            switchClassicButton.Size = new Vector2(40, 16);
            switchClassicButton.Label.Text = "Classic";
            //switchClassicButton.Label.TextColor = new Color(0.6f, 0.3f, 0.6f);
            switchClassicButton.Label.ShadowColor = Color.black;
            SetBackground(switchClassicButton, new Color(0.2f, 0.2f, 0), "switchClassicButtonBackgroundColor");
            switchClassicButton.Outline.Enabled = true;
            switchClassicButton.OnMouseClick += SwitchClassicButton_OnMouseClick;
            mainPanel.Components.Add(switchClassicButton);

            // Cancel button
            Button cancelButton = new Button();
            cancelButton.Position = new Vector2(236, 150);
            cancelButton.Size = new Vector2(40, 16);
            cancelButton.Label.Text = "Cancel";
            cancelButton.Label.ShadowColor = Color.black;
            SetBackground(cancelButton, cancelButtonBackgroundColor, "cancelButtonBackgroundColor");
            cancelButton.Outline.Enabled = true;
            cancelButton.OnMouseClick += CancelButton_OnMouseClick;
            mainPanel.Components.Add(cancelButton);

            // Screenshot panel
            screenshotPanel.Position = new Vector2(108, 25);
            screenshotPanel.Size = new Vector2(168, 95);
            screenshotPanel.BackgroundTextureLayout = BackgroundLayout.ScaleToFit;
            SetBackground(screenshotPanel, savesListBackgroundColor, "screenshotPanelBackgroundColor");
            screenshotPanel.Outline.Enabled = true;
            mainPanel.Components.Add(screenshotPanel);

            // Info panel
            Panel infoPanel = new Panel();
            infoPanel.Position = new Vector2(108, 122);
            infoPanel.Size = new Vector2(168, 26);
            mainPanel.Components.Add(infoPanel);

            // Save version
            saveVersionLabel.ShadowColor = Color.black;
            saveVersionLabel.Position = new Vector2(1, 1);
            saveVersionLabel.TextColor = saveFolderColor;
            screenshotPanel.Components.Add(saveVersionLabel);

            // Save folder
            saveFolderLabel.ShadowColor = Color.black;
            saveFolderLabel.HorizontalAlignment = HorizontalAlignment.Right;
            saveFolderLabel.Position = new Vector2(0, 1);
            saveFolderLabel.TextColor = saveFolderColor;
            screenshotPanel.Components.Add(saveFolderLabel);

            // Allow clicking folder label to open save folder in explorer
            // Currently Windows player and editor platforms only
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                saveFolderLabel.MouseOverBackgroundColor = Color.blue;
                saveFolderLabel.OnMouseClick += SaveFolderLabel_OnMouseClick;
            }

            // Time labels
            saveTimeLabel.ShadowPosition = Vector2.zero;
            saveTimeLabel.HorizontalAlignment = HorizontalAlignment.Center;
            saveTimeLabel.Position = new Vector2(0, 0);
            infoPanel.Components.Add(saveTimeLabel);
            gameTimeLabel.ShadowPosition = Vector2.zero;
            gameTimeLabel.HorizontalAlignment = HorizontalAlignment.Center;
            gameTimeLabel.Position = new Vector2(0, 9);
            infoPanel.Components.Add(gameTimeLabel);

            // Delete save button
            deleteSaveButton.Position = new Vector2(0, 132);
            deleteSaveButton.Size = new Vector2(98, 8);
            deleteSaveButton.HorizontalAlignment = HorizontalAlignment.Center;
            deleteSaveButton.Label.Text = "Delete Save";
            deleteSaveButton.Label.ShadowColor = Color.black;
            SetBackground(deleteSaveButton, namePanelBackgroundColor, "deleteSaveButtonBackgroundColor");
            deleteSaveButton.Outline.Enabled = false;
            deleteSaveButton.OnMouseClick += DeleteSaveButton_OnMouseClick;
            savesPanel.Components.Add(deleteSaveButton);

            // Switch character button
            switchCharButton.Position = new Vector2(216, 2);
            switchCharButton.Size = new Vector2(60, 8);
            switchCharButton.Label.Text = "Switch Char";
            switchCharButton.Label.ShadowColor = Color.black;
            SetBackground(switchCharButton, saveButtonBackgroundColor, "switchCharButtonBackgroundColor");
            switchCharButton.OnMouseClick += SwitchCharButton_OnMouseClick;
            mainPanel.Components.Add(switchCharButton);
        }

        public override void OnPush()
        {
            base.OnPush();
            base.Update();  // Ensures controls are properly sized for text label clipping

            // Update save game enumerations
            GameManager.Instance.SaveLoadManager.EnumerateSaves();

            // Always start window with current player name
            currentPlayerName = GameManager.Instance.PlayerEntity.Name;

            // Update controls for save/load mode
            SetMode(mode);

            // Update saves list
            UpdateSavesList();
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Return))
                SaveLoadEventHandler(null, Vector2.zero);
        }

        #endregion

        #region Private Methods

        void UpdateSavesList()
        {
            // Clear saves list
            savesList.ClearItems();

            // Get most recent save
            int mostRecentSave = GameManager.Instance.SaveLoadManager.FindMostRecentSave();
            if (mode == Modes.LoadGame && mostRecentSave == -1)
            {
                // No saves found, prompt to load a classic save
                promptLabel.Text = HardStrings.noSavesFound;
                return;
            }
            else
            {
                // If set to display most recent character use that instead
                if (displayMostRecentChar)
                {
                    SaveInfo_v1 latestSaveInfo = GameManager.Instance.SaveLoadManager.GetSaveInfo(mostRecentSave);
                    currentPlayerName = latestSaveInfo.characterName;
                }
            }

            // Build list of saves
            List<SaveInfo_v1> saves = new List<SaveInfo_v1>();
            int[] saveKeys = GameManager.Instance.SaveLoadManager.GetCharacterSaveKeys(currentPlayerName);
            foreach (int key in saveKeys)
            {
                SaveInfo_v1 saveInfo = GameManager.Instance.SaveLoadManager.GetSaveInfo(key);
                saves.Add(saveInfo);
            }

            // Order by save time
            List<SaveInfo_v1> orderedSaves = saves.OrderByDescending(o => o.dateAndTime.realTime).ToList();

            // Updates saves list
            foreach (SaveInfo_v1 saveInfo in orderedSaves)
            {
                savesList.AddItem(saveInfo.saveName);
            }
            savesScroller.TotalUnits = savesList.Count;

            // Update prompt
            string promptText = string.Empty;
            if (mode == Modes.SaveGame)
                promptText = savePromptText;
            else if (mode == Modes.LoadGame)
                promptText = loadPromptText;
            promptLabel.Text = string.Format("{0} for '{1}'", promptText, currentPlayerName);
        }

        void UpdateSelectedSaveInfo()
        {
            // Clear info if no save selected
            if (saveNameTextBox.Text.Length == 0 || savesList.SelectedIndex < 0)
            {
                screenshotPanel.BackgroundTexture = null;
                saveVersionLabel.Text = string.Empty;
                saveFolderLabel.Text = string.Empty;
                saveTimeLabel.Text = string.Empty;
                gameTimeLabel.Text = string.Empty;
                deleteSaveButton.BackgroundColor = namePanelBackgroundColor;
                return;
            }

            // Get save key
            int key = GameManager.Instance.SaveLoadManager.FindSaveFolderByNames(currentPlayerName, saveNameTextBox.Text);
            if (key == -1)
                return;

            // Destroy old background texture
            if (screenshotPanel.BackgroundTexture)
            {
                UnityEngine.Object.Destroy(screenshotPanel.BackgroundTexture);
                screenshotPanel.BackgroundTexture = null;
            }

            // Get save info and texture
            string path = GameManager.Instance.SaveLoadManager.GetSaveFolder(key);
            SaveInfo_v1 saveInfo = GameManager.Instance.SaveLoadManager.GetSaveInfo(key);
            Texture2D saveTexture = GameManager.Instance.SaveLoadManager.GetSaveScreenshot(key);
            if (saveTexture != null)
            {
                screenshotPanel.BackgroundTexture = saveTexture;
            }

            // Show save info
            DaggerfallDateTime dfDateTime = new DaggerfallDateTime();
            dfDateTime.FromSeconds(saveInfo.dateAndTime.gameTime);
            saveVersionLabel.Text = string.Format("V{0}", saveInfo.saveVersion);
            saveFolderLabel.Text = Path.GetFileName(path);
            saveTimeLabel.Text = DateTime.FromBinary(saveInfo.dateAndTime.realTime).ToLongDateString();
            gameTimeLabel.Text = dfDateTime.MidDateTimeString();
            deleteSaveButton.BackgroundColor = cancelButtonBackgroundColor;
        }

        void SaveGame()
        {
            GameManager.Instance.SaveLoadManager.Save(currentPlayerName, saveNameTextBox.Text);
            DaggerfallUI.Instance.PopToHUD();
        }

        void LoadGame()
        {
            GameManager.Instance.SaveLoadManager.Load(currentPlayerName, saveNameTextBox.Text);
            DaggerfallUI.Instance.PopToHUD();
        }

        void SetMode(Modes mode)
        {
            if (mode == Modes.SaveGame)
            {
                saveNameTextBox.DefaultText = HardStrings.enterSaveName;
                switchCharButton.Enabled = false;
                switchClassicButton.Enabled = false;
                saveNameTextBox.ReadOnly = false;
                goButton.Label.Text = saveButtonText;
            }
            else if (mode == Modes.LoadGame)
            {
                saveNameTextBox.DefaultText = HardStrings.selectSaveName;
                saveNameTextBox.ReadOnly = true;
                goButton.Label.Text = loadButtonText;
                switchClassicButton.Enabled = true;

                // Enable switch if at least one character
                if (GameManager.Instance.SaveLoadManager.CharacterCount >= 1)
                    switchCharButton.Enabled = true;
                else
                    switchCharButton.Enabled = false;
            }
        }

        void SetBackground(BaseScreenComponent panel, Color color, string textureName)
        {
            Texture2D tex;
            if (TextureReplacement.TryImportTexture(textureName, true, out tex))
            {
                panel.BackgroundTexture = tex;
                TextureReplacement.LogLegacyUICustomizationMessage(textureName);
            }
            else
                panel.BackgroundColor = color;
        }

        void SetBackground(Button button, Color color, string textureName)
        {
#pragma warning disable 0618
            if (!TextureReplacement.TryCustomizeButton(ref button, textureName))
                button.BackgroundColor = color;
#pragma warning restore 0618
        }

        #endregion

        #region Event Handlers

        private void SaveLoadEventHandler(BaseScreenComponent sender, Vector2 position)
        {
            if (mode == Modes.SaveGame)
            {
                // Must have a save name
                if (saveNameTextBox.Text.Length == 0)
                {
                    DaggerfallUI.MessageBox(HardStrings.youMustEnterASaveName);
                    return;
                }

                // Get save key and confirm if already exists
                int key = GameManager.Instance.SaveLoadManager.FindSaveFolderByNames(currentPlayerName, saveNameTextBox.Text);
                if (key != -1)
                {
                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                    messageBox.SetText(new string[] { HardStrings.confirmOverwriteSave, "" });
                    messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                    messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                    messageBox.OnButtonClick += ConfirmOverwrite_OnButtonClick;
                    uiManager.PushWindow(messageBox);
                }
                else
                {
                    SaveGame();
                }
            }
            else if (mode == Modes.LoadGame)
            {
                // Must have a save name
                if (saveNameTextBox.Text.Length == 0)
                {
                    DaggerfallUI.MessageBox(HardStrings.youMustSelectASaveName);
                    return;
                }

                LoadGame();
            }
        }

        private void CancelButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void SavesScroller_OnScroll()
        {
            savesList.ScrollIndex = savesScroller.ScrollIndex;
        }

        private void SavesList_OnScroll()
        {
            savesScroller.ScrollIndex = savesList.ScrollIndex;
        }

        private void SaveNameTextBox_OnType()
        {
            int index = savesList.FindIndex(saveNameTextBox.Text);
            if (index != -1)
            {
                savesList.SelectedIndex = index;
            }
            else
            {
                savesList.SelectNone();
                UpdateSelectedSaveInfo();
            }
        }

        private void SavesList_OnSelectItem()
        {
            saveNameTextBox.Text = savesList.SelectedItem;
            UpdateSelectedSaveInfo();
        }

        private void DeleteSaveButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Must have a save selected
            if (savesList.SelectedIndex < 0)
                return;

            // Confirmation
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.SetText(new string[] { HardStrings.confirmDeleteSave, "" });
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Delete);
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Cancel);
            messageBox.OnButtonClick += ConfirmDelete_OnButtonClick;
            uiManager.PushWindow(messageBox);
        }

        private void ConfirmDelete_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Delete)
            {
                // Get save key
                int key = GameManager.Instance.SaveLoadManager.FindSaveFolderByNames(currentPlayerName, saveNameTextBox.Text);
                if (key == -1)
                    return;

                // Delete save and refresh
                GameManager.Instance.SaveLoadManager.DeleteSaveFolder(key);
                saveNameTextBox.Text = string.Empty;
                UpdateSavesList();
                UpdateSelectedSaveInfo();
            }

            CloseWindow();
        }

        private void ConfirmOverwrite_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                SaveGame();
            }
            else
            {
                CloseWindow();
            }
        }

        private void SwitchCharButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallListPickerWindow picker = new DaggerfallListPickerWindow(uiManager, this);

            // Get ordered list of character names
            List<string> names = new List<string>();
            names.AddRange(GameManager.Instance.SaveLoadManager.CharacterNames);
            List<string> orderedNames = names.OrderBy(o => o).ToList();

            // Add to picker list
            foreach (string name in orderedNames)
            {
                picker.ListBox.AddItem(name);
            }

            // Select current character
            picker.ListBox.SelectedIndex = picker.ListBox.FindIndex(currentPlayerName);

            // Add event for selection
            picker.OnItemPicked += Picker_OnItemPicked;

            // Show window
            uiManager.PushWindow(picker);
        }

        private void SwitchClassicButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            uiManager.PushWindow(UIWindowFactory.GetInstance(UIWindowType.LoadClassicGame, uiManager, null));
        }

        private void Picker_OnItemPicked(int index, string itemString)
        {
            displayMostRecentChar = false;

            currentPlayerName = itemString;
            UpdateSavesList();

            saveNameTextBox.Text = string.Empty;
            UpdateSelectedSaveInfo();

            CloseWindow();
        }

        private void SaveFolderLabel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Get save key
            int key = GameManager.Instance.SaveLoadManager.FindSaveFolderByNames(currentPlayerName, saveNameTextBox.Text);
            if (key == -1)
                return;

            // Get save folder
            string path = GameManager.Instance.SaveLoadManager.GetSaveFolder(key);
            if (string.IsNullOrEmpty(path))
                return;

            // Attempt to open path
            System.Diagnostics.Process.Start(path);
        }

        #endregion
    }
}