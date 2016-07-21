// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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

        Color mainPanelBackgroundColor = new Color(0.0f, 0f, 0.0f, 1.0f);
        Color namePanelBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        Color saveButtonBackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        Color cancelButtonBackgroundColor = new Color(0.7f, 0.0f, 0.0f, 0.4f);
        Color savesListBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
        Color savesListTextColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        Color saveFolderColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        VerticalScrollBar savesScroller = new VerticalScrollBar();

        #endregion

        #region UI Textures
        #endregion

        #region Fields

        const string promptText = "Save Game";

        #endregion

        #region Constructors

        public DaggerfallUnitySaveGameWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
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
            mainPanel.BackgroundColor = mainPanelBackgroundColor;
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
            namePanel.BackgroundColor = namePanelBackgroundColor;
            mainPanel.Components.Add(namePanel);

            // Name input
            saveNameTextBox.Position = new Vector2(2, 2);
            saveNameTextBox.MaxCharacters = 26;
            saveNameTextBox.DefaultText = HardStrings.enterSaveName;
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
            savesList.BackgroundColor = savesListBackgroundColor;
            savesList.ShadowPosition = Vector2.zero;
            savesList.RowsDisplayed = 16;
            savesList.OnScroll += SavesList_OnScroll;
            savesList.OnSelectItem += SavesList_OnSelectItem;
            savesList.OnMouseDoubleClick += SaveEventHandler;
            savesPanel.Components.Add(savesList);

            // Save scroller
            savesScroller.Position = new Vector2(94, 2);
            savesScroller.Size = new Vector2(5, 129);
            savesScroller.DisplayUnits = 16;
            savesScroller.OnScroll += SavesScroller_OnScroll;
            savesPanel.Components.Add(savesScroller);

            // Save button
            Button saveButton = new Button();
            saveButton.Position = new Vector2(108, 150);
            saveButton.Size = new Vector2(40, 16);
            saveButton.Label.Text = "Save";
            saveButton.Label.ShadowColor = Color.black;
            saveButton.BackgroundColor = saveButtonBackgroundColor;
            saveButton.Outline.Enabled = true;
            saveButton.OnMouseClick += SaveEventHandler;
            mainPanel.Components.Add(saveButton);

            // Cancel button
            Button cancelButton = new Button();
            cancelButton.Position = new Vector2(236, 150);
            cancelButton.Size = new Vector2(40, 16);
            cancelButton.Label.Text = "Cancel";
            cancelButton.Label.ShadowColor = Color.black;
            cancelButton.BackgroundColor = cancelButtonBackgroundColor;
            cancelButton.Outline.Enabled = true;
            cancelButton.OnMouseClick += CancelButton_OnMouseClick;
            mainPanel.Components.Add(cancelButton);

            // Screenshot panel
            screenshotPanel.Position = new Vector2(108, 25);
            screenshotPanel.Size = new Vector2(168, 95);
            screenshotPanel.BackgroundTextureLayout = BackgroundLayout.ScaleToFit;
            screenshotPanel.BackgroundColor = savesListBackgroundColor;
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
            deleteSaveButton.BackgroundColor = namePanelBackgroundColor;
            deleteSaveButton.Outline.Enabled = false;
            deleteSaveButton.OnMouseClick += DeleteSaveButton_OnMouseClick;
            savesPanel.Components.Add(deleteSaveButton);
        }

        public override void OnPush()
        {
            base.OnPush();
            base.Update();  // Ensures controls are properly sized for text label clipping

            // Update save game prompt
            promptLabel.Text = string.Format("{0} for '{1}'", promptText, GameManager.Instance.PlayerEntity.Name);

            // Update save game enumerations
            GameManager.Instance.SaveLoadManager.EnumerateSaves();

            // Update saves list
            UpdateSavesList();
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Return))
                SaveEventHandler(null, Vector2.zero);
        }

        #endregion

        #region Private Methods

        void UpdateSavesList()
        {
            // Build list of saves
            List<SaveInfo_v1> saves = new List<SaveInfo_v1>();
            int[] saveKeys = GameManager.Instance.SaveLoadManager.GetCharacterSaveKeys(GameManager.Instance.PlayerEntity.Name);
            foreach (int key in saveKeys)
            {
                SaveInfo_v1 saveInfo = GameManager.Instance.SaveLoadManager.GetSaveInfo(key);
                saves.Add(saveInfo);
            }

            // Order by save time
            List<SaveInfo_v1> orderedSaves = saves.OrderByDescending(o => o.dateAndTime.realTime).ToList();

            // Updates saves list
            savesList.ClearItems();
            foreach (SaveInfo_v1 saveInfo in orderedSaves)
            {
                savesList.AddItem(saveInfo.saveName);
            }
            savesScroller.TotalUnits = savesList.Count;
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
            int key = GameManager.Instance.SaveLoadManager.FindSaveFolderByNames(GameManager.Instance.PlayerEntity.Name, saveNameTextBox.Text);
            if (key == -1)
                return;

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
            GameManager.Instance.SaveLoadManager.Save(GameManager.Instance.PlayerEntity.Name, saveNameTextBox.Text);
            DaggerfallUI.Instance.PopToHUD();
        }

        #endregion

        #region Event Handlers

        private void SaveEventHandler(BaseScreenComponent sender, Vector2 position)
        {
            // Must have a save name
            if (saveNameTextBox.Text.Length == 0)
            {
                DaggerfallUI.MessageBox(HardStrings.youMustEnterASaveName);
                return;
            }

            // Get save key and confirm if already exists
            int key = GameManager.Instance.SaveLoadManager.FindSaveFolderByNames(GameManager.Instance.PlayerEntity.Name, saveNameTextBox.Text);
            if (key != -1)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetText(HardStrings.confirmOverwriteSave, "");
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
            messageBox.SetText(HardStrings.confirmDeleteSave, "");
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
                int key = GameManager.Instance.SaveLoadManager.FindSaveFolderByNames(GameManager.Instance.PlayerEntity.Name, saveNameTextBox.Text);
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

        #endregion
    }
}