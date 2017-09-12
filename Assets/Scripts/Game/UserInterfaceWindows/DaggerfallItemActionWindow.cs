// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
//
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Banking;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements inventory window.
    /// </summary>
    public class DaggerfallItemActionWindow : DaggerfallInventoryWindow
    {
        #region UI Rects

        Rect costPanelRect = new Rect(0, 3, 111, 9);
        Rect costPanelPositionRect = new Rect(49, 13, 111, 9);
        Rect localTargetIconRect = new Rect(164, 11, 57, 36);

        Rect actionButtonsPanelRect = new Rect(222, 10, 39, 190);
        Rect wagonButtonRect = new Rect(4, 4, 31, 14);
        Rect infoButtonRect = new Rect(4, 26, 31, 14);
        Rect selectButtonRect = new Rect(4, 48, 31, 14);
        Rect stealButtonRect = new Rect(4, 102, 31, 14);
        Rect modeActionButtonRect = new Rect(4, 124, 31, 14);
        Rect clearButtonRect = new Rect(4, 146, 31, 14);

        #endregion

        #region UI Controls

        protected Panel localTargetIconPanel;
        protected TextLabel localTargetIconLabel;

        Panel costPanel;
        TextLabel costLabel;
        TextLabel goldLabel;

        Panel actionButtonsPanel;
        Button wagonButton;
        Button infoButton;
        Button selectButton;
        Button stealButton;
        Button modeActionButton;
        Button clearButton;

        #endregion

        #region UI Textures

        Texture2D costPanelBaseTexture;
        Texture2D costPanelTexture;
        Texture2D actionButtonsTexture;
        Texture2D actionButtonsGoldTexture;
        Texture2D selectSelected;
        Texture2D selectNotSelected;

        #endregion

        #region Fields

        const string buyButtonsTextureName = "INVE08I0.IMG";
        const string sellButtonsTextureName = "INVE10I0.IMG";
        const string sellButtonsGoldTextureName = "INVE11I0.IMG";
        const string repairButtonsTextureName = "INVE12I0.IMG";
        const string costPanelTextureName = "REPR02I0.IMG";

        WindowModes windowMode = WindowModes.Inventory;

        ItemCollection merchantItems = new ItemCollection();
        bool usingWagon = false;

        #endregion

        #region Enums

        public enum WindowModes
        {
            Inventory,      // Should never get used, treat as 'none'
            Sell,
            Buy,
            Repair,
            Identify,
        }

        #endregion

        #region Properties

        #endregion

        #region Constructors

        public DaggerfallItemActionWindow(IUserInterfaceManager uiManager, WindowModes windowMode, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
            this.windowMode = windowMode;
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all the textures used by inventory system
            LoadTextures();

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundTexture = baseTexture;

            // Character portrait
            SetupPaperdoll();

            // Cost & gold display
            SetupCostAndGold();

            // Setup action button panel.
            actionButtonsPanel = DaggerfallUI.AddPanel(actionButtonsPanelRect, NativePanel);
            // If not inventory mode, overlay mode button texture.
            if (actionButtonsTexture != null)
                actionButtonsPanel.BackgroundTexture = actionButtonsTexture;

            // Setup UI
            SetupTabPageButtons();
            SetupActionButtons();
            SetupScrollBars();
            SetupScrollButtons();
            SetupLocalItemsElements();
            SetupRemoteItemsElements();
            SetupAccessoryElements();

            // Exit buttons
            Button exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            // Setup local and remote target icon panels
            localTargetIconPanel = DaggerfallUI.AddPanel(localTargetIconRect, NativePanel);
            localTargetIconLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(1, 2), localTargetIconPanel);
            remoteTargetIconPanel = DaggerfallUI.AddPanel(remoteTargetIconRect, NativePanel);

            // Setup initial state
            SelectTabPage(TabPages.WeaponsAndArmor);
            SelectActionMode(ActionModes.Select);

            // Setup initial display
            FilterLocalItems();
            FilterRemoteItems();
            UpdateLocalItemsDisplay();
            UpdateRemoteItemsDisplay();
            UpdateAccessoryItemsDisplay();
            UpdateLocalTargetIcon();
            UpdateRemoteTargetIcon();
        }

        void SetupCostAndGold()
        {
            costPanel = DaggerfallUI.AddPanel(costPanelPositionRect, NativePanel);
            costPanel.BackgroundTexture = costPanelTexture;
            costLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(28, 2), costPanel);
            goldLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(68, 2), costPanel);
        }

        protected override void SetupActionButtons()
        {
            wagonButton = DaggerfallUI.AddButton(wagonButtonRect, actionButtonsPanel);
            wagonButton.OnMouseClick += WagonButton_OnMouseClick;

            infoButton = DaggerfallUI.AddButton(infoButtonRect, actionButtonsPanel);
            infoButton.OnMouseClick += InfoButton_OnMouseClick;

            selectButton = DaggerfallUI.AddButton(selectButtonRect, actionButtonsPanel);
            selectButton.OnMouseClick += SelectButton_OnMouseClick;

            modeActionButton = DaggerfallUI.AddButton(modeActionButtonRect, actionButtonsPanel);
            modeActionButton.OnMouseClick += SellButton_OnMouseClick;
        }

        #endregion

        #region Public Methods

        public override void OnPush()
        {
            // Local items always points to player inventory
            localItems = PlayerEntity.Items;

            // Start a new dropped items target
            merchantItems.Clear();
            remoteItems = merchantItems;
            remoteTargetType = RemoteTargetTypes.Merchant;

            // Clear wagon button state on open
            if (wagonButton != null)
            {
                usingWagon = false;
                wagonButton.BackgroundTexture = wagonNotSelected;
            }

            // Reset scrollbars
            if (localItemsScrollBar != null)
                localItemsScrollBar.ScrollIndex = 0;
            if (remoteItemsScrollBar != null)
                remoteItemsScrollBar.ScrollIndex = 0;

            // Refresh window
            Refresh();
        }

        public override void OnPop()
        {
            // What to do with items?
        }

        public override void Refresh(bool refreshPaperDoll = true)
        {
            if (!IsSetup)
                return;

            base.Refresh(refreshPaperDoll);

            costLabel.Text = "0";
            goldLabel.Text = PlayerEntity.GoldPieces.ToString();

        }

        #endregion

        #region Helper Methods

        void SelectActionMode(ActionModes mode)
        {
            selectedActionMode = mode;
            if (mode == ActionModes.Info)
            {
                infoButton.BackgroundTexture = infoSelected;
                selectButton.BackgroundTexture = selectNotSelected;
            }
            else if (mode == ActionModes.Select)
            {
                infoButton.BackgroundTexture = infoNotSelected;
                selectButton.BackgroundTexture = selectSelected;
            }
        }

        protected override void UpdateRemoteTargetIcon()
        {
            ImageData containerImage;
            switch (windowMode)
            {
                default:
                case WindowModes.Sell:
                    containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Merchant);
                    break;
                case WindowModes.Buy:
                    containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Shelves);
                    break;
                case WindowModes.Repair:
                    containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Anvil);
                    break;
                case WindowModes.Identify:
                    containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Magic);
                    break;
            }
            remoteTargetIconPanel.BackgroundTexture = containerImage.texture;
        }

        protected override void UpdateLocalTargetIcon()
        {
            if (usingWagon)
            {
                localTargetIconPanel.BackgroundTexture = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Wagon).texture;
                float weight = PlayerEntity.WagonWeight;
                localTargetIconLabel.Text = String.Format(weight % 1 == 0 ? "{0:F0} / {1}" : "{0:F2} / {1}", weight, ItemHelper.wagonKgLimit);
            }
            else
            {
                localTargetIconPanel.BackgroundTexture = null;
                localTargetIconLabel.Text = "";
            }
        }

        void ShowWagon(bool show)
        {
            if (show)
            {
                // Switch to wagon
                wagonButton.BackgroundTexture = wagonSelected;
                localItems = PlayerEntity.WagonItems;
            }
            else
            {
                // Restore previous target or default to dropped items
                wagonButton.BackgroundTexture = wagonNotSelected;
                localItems = PlayerEntity.Items;
            }
            usingWagon = show;
            Refresh(false);
        }

        protected override void LoadTextures()
        {
            base.LoadTextures();

            Texture2D costPanelBaseTexture = ImageReader.GetTexture(costPanelTextureName);
            costPanelTexture = ImageReader.GetSubTexture(costPanelBaseTexture, costPanelRect);

            // Load special button texture.
            if (windowMode == WindowModes.Sell)
                actionButtonsTexture = ImageReader.GetTexture(sellButtonsTextureName);
            else if (windowMode == WindowModes.Buy)
                actionButtonsTexture = ImageReader.GetTexture(buyButtonsTextureName);
            else if (windowMode == WindowModes.Repair)
                actionButtonsTexture = ImageReader.GetTexture(repairButtonsTextureName);

            actionButtonsGoldTexture = ImageReader.GetTexture(sellButtonsGoldTextureName);
            selectNotSelected = ImageReader.GetSubTexture(actionButtonsTexture, selectButtonRect);
            selectSelected = ImageReader.GetSubTexture(actionButtonsGoldTexture, selectButtonRect);
        }

        #endregion

        #region Action Handler Methods

        protected override void LocalItemsClick(BaseScreenComponent sender)
        {
            // Get index
            int index = localItemsScrollBar.ScrollIndex + (int)sender.Tag;
            if (index >= localItemsFiltered.Count)
                return;

            // Get item
            DaggerfallUnityItem item = localItemsFiltered[index];
            if (item == null)
                return;

            // Handle click based on action & mode
            if (windowMode == WindowModes.Sell)
            {
                if (selectedActionMode == ActionModes.Select)
                {
                    // Transfer to remote items
                    if (remoteItems != null)
                    {
                        TransferItem(item, localItems, remoteItems);
                    }
                }
                else if (selectedActionMode == ActionModes.Info)
                {
                    ShowInfoPopup(item);
                }
            }
        }

        protected override void RemoteItemsClick(BaseScreenComponent sender)
        {
            // Get index
            int index = remoteItemsScrollBar.ScrollIndex + (int)sender.Tag;
            if (index >= remoteItemsFiltered.Count)
                return;

            // Get item
            DaggerfallUnityItem item = remoteItemsFiltered[index];
            if (item == null)
                return;

            // Handle click based on action
            if (selectedActionMode == ActionModes.Select && CanCarry(item))
            {
                if ((!usingWagon && CanCarry(item)) || WagonCanHold(item))
                    TransferItem(item, remoteItems, localItems);
                else
                    ;// show messgae?
            }
            else if (selectedActionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        #endregion

        #region Action Button Event Handlers

        private void WagonButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ShowWagon(!usingWagon);
        }

        private void InfoButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectActionMode(ActionModes.Info);
        }

        private void SelectButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectActionMode(ActionModes.Select);
        }

        private void SellButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("Sell!");
        }


        #endregion
    }
}