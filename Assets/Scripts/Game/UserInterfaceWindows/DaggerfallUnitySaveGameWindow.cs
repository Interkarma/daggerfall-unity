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
        TextBox saveNameTextBox = new TextBox();
        TextLabel promptLabel = new TextLabel();
        ListBox savesList = new ListBox();

        Color mainPanelBackgroundColor = new Color(0.0f, 0f, 0.0f, 1.0f);
        Color namePanelBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        Color saveButtonBackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        Color cancelButtonBackgroundColor = new Color(0.7f, 0.0f, 0.0f, 0.4f);
        Color savesListBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
        Color savesListTextColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
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
            //promptLabel.ShadowPosition = Vector2.zero;
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
            saveNameTextBox.MaxCharacters = 45;
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
            savesList.Size = new Vector2(91, 126);
            savesList.TextColor = savesListTextColor;
            savesList.BackgroundColor = savesListBackgroundColor;
            //savesList.SelectedTextColor = Color.white;
            savesList.ShadowPosition = Vector2.zero;
            savesList.RowsDisplayed = 16;
            savesList.OnScroll += SavesList_OnScroll;
            savesList.OnSelectItem += SavesList_OnSelectItem;
            savesList.OnMouseDoubleClick += SavesList_OnMouseDoubleClick;
            savesPanel.Components.Add(savesList);

            // Save scroller
            savesScroller.Position = new Vector2(94, 2);
            savesScroller.Size = new Vector2(5, 126);
            savesScroller.DisplayUnits = 16;
            savesScroller.OnScroll += SavesScroller_OnScroll;
            savesPanel.Components.Add(savesScroller);

            // Save button
            Button saveButton = new Button();
            saveButton.Position = new Vector2(108, 150);
            saveButton.Size = new Vector2(40, 16);
            saveButton.Label.Text = "Save";
            saveButton.Label.ShadowColor = Color.black;
            //saveButton.Label.ShadowPosition = Vector2.zero;
            saveButton.BackgroundColor = saveButtonBackgroundColor;
            saveButton.Outline.Enabled = true;
            saveButton.OnMouseClick += SaveButton_OnMouseClick;
            mainPanel.Components.Add(saveButton);

            // Cancel button
            Button cancelButton = new Button();
            cancelButton.Position = new Vector2(236, 150);
            cancelButton.Size = new Vector2(40, 16);
            cancelButton.Label.Text = "Cancel";
            cancelButton.Label.ShadowColor = Color.black;
            //cancelButton.Label.ShadowPosition = Vector2.zero;
            cancelButton.BackgroundColor = cancelButtonBackgroundColor;
            cancelButton.Outline.Enabled = true;
            cancelButton.OnMouseClick += CancelButton_OnMouseClick;
            mainPanel.Components.Add(cancelButton);
        }

        public override void OnPush()
        {
            base.OnPush();
            base.Update();  // Ensures controls are properly sized for text label clipping

            // Set default text
            saveNameTextBox.DefaultText = DaggerfallUnity.Instance.WorldTime.Now.MidDateTimeString();

            // Update save game prompt
            promptLabel.Text = string.Format("{0} for '{1}'", promptText, GameManager.Instance.PlayerEntity.Name);

            // Update save game enumerations
            GameManager.Instance.SaveLoadManager.EnumerateSaves();

            // Update saves list
            UpdateSavesList();
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

        #endregion

        #region Event Handlers

        private void SaveButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            GameManager.Instance.SaveLoadManager.Save(GameManager.Instance.PlayerEntity.Name, saveNameTextBox.ResultText);
            DaggerfallUI.Instance.PopToHUD();
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
            }
        }
        private void SavesList_OnSelectItem()
        {
            saveNameTextBox.Text = savesList.SelectedItem;
        }

        private void SavesList_OnMouseDoubleClick(BaseScreenComponent sender, Vector2 position)
        {
            GameManager.Instance.SaveLoadManager.Save(GameManager.Instance.PlayerEntity.Name, saveNameTextBox.ResultText);
            DaggerfallUI.Instance.PopToHUD();
        }

        #endregion
    }
}