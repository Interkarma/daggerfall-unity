// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
using System.Collections.Generic;
using System.Text;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Simple cross-platform folder browser.
    /// </summary>
    public class FolderBrowser : Panel
    {
        private const string parentDirectory = "..";

        int confirmButtonWidth = 35;
        int drivePanelWidth = 40;
        int pathPanelHeight = 12;
        int panelSeparatorWidth = 2;
        int maxChars = 43;
        string confirmButtonText = "OK";

        int minWidth = 200;
        int minHeight = 100;
        Panel drivePanel = new Panel();
        Panel folderPanel = new Panel();
        Panel pathPanel = new Panel();
        ListBox driveList = new ListBox();
        ListBox folderList = new ListBox();
        VerticalScrollBar folderScroller = new VerticalScrollBar();
        TextLabel pathLabel = new TextLabel();
        Button confirmButton = new Button();
        Checkbox showHiddenFilesCheck = new Checkbox();
        Vector2 lastSize;

        Color unselectedColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        Color scrollerBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
        Color confirmEnabledButtonColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
        Color confirmDisabledButtonColor = new Color(0.5f, 0.0f, 0.0f, 0.4f);

        List<string> drives = new List<string>();
        List<string> folders = new List<string>();

        string currentPath;
        bool confirmEnabled = true;

        public delegate void OnConfirmPathHandler();
        public event OnConfirmPathHandler OnConfirmPath;

        public delegate void OnPathChangedHandler();
        public event OnPathChangedHandler OnPathChanged;

        #region Properties

        /// <summary>
        /// Maximum right-most characters to display in path label.
        /// </summary>
        public int MaxPathLabelChars
        {
            get { return maxChars; }
            set { maxChars = value; }
        }

        /// <summary>
        /// Gets current path selected by browser.
        /// </summary>
        public string CurrentPath
        {
            get { return currentPath; }
        }

        /// <summary>
        /// Enable or disable confirm button, e.g. based on path validation.
        /// </summary>
        public bool ConfirmEnabled
        {
            get { return confirmEnabled; }
            set { confirmEnabled = value; }
        }

        #endregion

        #region Constructors

        public FolderBrowser()
        {
            Setup();
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            base.Update();

            if (lastSize != Size)
            {
                AdjustPanels();
                lastSize = Size;
                RefreshFolders();
            }

            if (confirmEnabled)
                confirmButton.BackgroundColor = confirmEnabledButtonColor;
            else
                confirmButton.BackgroundColor = confirmDisabledButtonColor;
        }

        #endregion

        #region Private Methods

        void Setup()
        {
            // Setup panels
            drivePanel.Outline.Enabled = true;
            folderPanel.Outline.Enabled = true;
            pathPanel.Outline.Enabled = true;
            Components.Add(drivePanel);
            Components.Add(folderPanel);
            Components.Add(pathPanel);
            Components.Add(confirmButton);
            Components.Add(showHiddenFilesCheck);
            AdjustPanels();

            // Setup drive list
            drivePanel.Components.Add(driveList);
            driveList.TextColor = unselectedColor;
            driveList.SelectedTextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            driveList.ShadowPosition = Vector2.zero;
            driveList.OnSelectItem += DriveList_OnSelectItem;

            // Setup folder list
            folderPanel.Components.Add(folderList);
            folderList.TextColor = unselectedColor;
            folderList.SelectedTextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            folderList.ShadowPosition = Vector2.zero;
            folderList.OnUseSelectedItem += FolderList_OnUseSelectedItem;
            folderList.OnScroll += FolderList_OnScroll;
            //folderList.OnScrollUp += FolderList_OnScrollUp;
            //folderList.OnScrollDown += FolderList_OnScrollDown;

            // Setup path label
            pathPanel.Components.Add(pathLabel);

            // Setup scrollbar
            folderPanel.Components.Add(folderScroller);
            folderScroller.OnScroll += FolderScroller_OnScroll;
            //folderScroller.OnScrollUp += FolderScroller_OnScrollUp;
            //folderScroller.OnScrollDown += FolderScroller_OnScrollDown;

            showHiddenFilesCheck.Label.Text = TextManager.Instance.GetText("MainMenu", "hiddenFolders");
            showHiddenFilesCheck.Label.TextColor = unselectedColor;
            showHiddenFilesCheck.CheckBoxColor = unselectedColor;
            showHiddenFilesCheck.IsChecked = false;
            showHiddenFilesCheck.OnToggleState += HiddenFileCheck_OnToggleState;

            // Setup initial folder conditions
            RefreshDrives();
            RefreshFolders();

            // Setup events
            confirmButton.OnMouseClick += ConfirmButton_OnMouseClick;
        }

        void RefreshDrives()
        {
            // Unix has no concept of logical drives, root is always the correct filesystem root
            drives.Clear();
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.MacOSX:
                case OperatingSystemFamily.Linux:
                    drives.Add("/");
                    break;
                default:
                    drives.AddRange(Directory.GetLogicalDrives());
                    break;
            }

            if (drives.Count == 0)
                return;

            driveList.ClearItems();
            driveList.AddItems(drives);

            driveList.SelectedIndex = 0;
            currentPath = drives[driveList.SelectedIndex];
        }

        void RefreshFolders()
        {
            folders.Clear();
            folderList.ClearItems();

            // Add return path
            if (currentPath != drives[driveList.SelectedIndex])
                folderList.AddItem(parentDirectory);

            try
            {
                string[] directoryList = Directory.GetDirectories(currentPath);

                foreach (var directory in directoryList)
                {
                    DirectoryInfo info = new DirectoryInfo(directory);
                    if (showHiddenFilesCheck.IsChecked || (info.Attributes & FileAttributes.Hidden) == 0)
                    {
                        string name = Path.GetFileName(directory);
                        folders.Add(name);
                        folderList.AddItem(name);
                    }
                }

                folderScroller.TotalUnits = folderList.Count;
                folderScroller.DisplayUnits = folderList.RowsDisplayed;
                folderScroller.ScrollIndex = 0;
                folderList.SelectedIndex = 0;
            }
            catch
            {
                folders.Clear();
                folderList.ClearItems();
                return;
            }
        }

        void AdjustPanels()
        {
            const float scrollerWidth = 5;

            // Enforce minimum size
            Vector2 size = Size;
            if (size.x < minWidth) size.x = minWidth;
            if (size.y < minHeight) size.y = minHeight;
            Size = size;

            // Set drive panel
            float bottomPanelHeight = pathPanelHeight + panelSeparatorWidth;
            drivePanel.Position = Vector2.zero;
            drivePanel.Size = new Vector2(drivePanelWidth, size.y - bottomPanelHeight);

            // Set folder panel
            float leftPanelWidth = drivePanel.Size.x + panelSeparatorWidth;
            folderPanel.Position = new Vector2(drivePanel.Position.x + leftPanelWidth, 0);
            folderPanel.Size = new Vector2(size.x - leftPanelWidth, size.y - bottomPanelHeight);

            // Set path panel
            pathPanel.Position = new Vector2(drivePanel.Position.x, size.y - pathPanelHeight);
            pathPanel.Size = new Vector2(size.x - confirmButtonWidth - panelSeparatorWidth, pathPanelHeight);

            // Set drive list
            driveList.Position = new Vector2(2, 2);
            driveList.Size = new Vector2(drivePanel.Size.x - 2, drivePanel.Size.y - 4);

            // Set folder list
            folderList.Position = new Vector2(2, 2);
            folderList.Size = new Vector2(folderPanel.Size.x - scrollerWidth - 4, folderPanel.Size.y - 4);
            folderList.SetRowsDisplayedByHeight();

            // Set scrollbar
            folderScroller.BackgroundColor = scrollerBackgroundColor;
            folderScroller.Position = new Vector2(folderPanel.Size.x - scrollerWidth - 1, folderPanel.Position.y + 1);
            folderScroller.Size = new Vector2(scrollerWidth, folderPanel.Size.y - 2);

            // Set path label
            pathLabel.Position = new Vector2(2, 2);
            pathLabel.VerticalAlignment = VerticalAlignment.Middle;
            pathLabel.ShadowPosition = Vector2.zero;
            pathLabel.MaxWidth = (int)pathPanel.Size.x - 4;

            // Set confirm button
            //confirmButton.BackgroundColor = confirmButtonColor;
            confirmButton.Position = new Vector2(pathPanel.Position.x + pathPanel.Size.x + panelSeparatorWidth, pathPanel.Position.y);
            confirmButton.Size = new Vector2(confirmButtonWidth, pathPanel.Size.y);
            confirmButton.Outline.Enabled = true;
            confirmButton.Label.Text = confirmButtonText;
            //confirmButton.Label.ShadowPosition = Vector2.zero;

            showHiddenFilesCheck.Position = new Vector2(pathPanel.Position.x, pathPanel.Position.y + pathPanel.Size.y + 4);
        }

        void UpdatePathText()
        {
            int index = currentPath.Length - maxChars;
            if (index < 0)
                index = 0;

            pathLabel.Text = currentPath.Substring(index);
        }

        void RaiseOnConfirmPathEvent()
        {
            if (OnConfirmPath != null)
                OnConfirmPath();
        }

        void RaisePathChangedEvent()
        {
            if (OnPathChanged != null)
                OnPathChanged();
        }

        #endregion

        #region Event Handlers

        private void DriveList_OnSelectItem()
        {
            currentPath = drives[driveList.SelectedIndex];
            UpdatePathText();
            RefreshFolders();
            RaisePathChangedEvent();
        }

        private void FolderList_OnUseSelectedItem()
        {
            if (folderList.SelectedIndex < 0 || folderList.Count == 0)
                return;

            // Get new path
            string newPath = string.Empty;
            if (folderList.SelectedItem == parentDirectory)
            {
                // Handle return path
                DirectoryInfo info = new DirectoryInfo(currentPath);
                newPath = info.Parent.FullName;
            }
            else
            {
                // Select next folder
                string selectedFolder = folderList.SelectedItem;
                newPath = Path.Combine(currentPath, selectedFolder);
            }

            // List folders in new path
            if (Directory.Exists(newPath))
            {
                currentPath = newPath;
                RefreshFolders();
                RaisePathChangedEvent();

                UpdatePathText();
            }
        }

        private void FolderScroller_OnScroll()
        {
            folderList.ScrollIndex = folderScroller.ScrollIndex;
        }

        private void FolderList_OnScroll()
        {
            folderScroller.ScrollIndex = folderList.ScrollIndex;
        }

        private void ConfirmButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (confirmEnabled)
                RaiseOnConfirmPathEvent();
            else
                FolderList_OnUseSelectedItem();
        }

        private void HiddenFileCheck_OnToggleState()
        {
            RefreshFolders();
        }

        #endregion
    }
}
