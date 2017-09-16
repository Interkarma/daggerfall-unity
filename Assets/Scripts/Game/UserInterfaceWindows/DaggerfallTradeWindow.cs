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
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements trade windows, based on inventory window.
    /// </summary>
    public partial class DaggerfallTradeWindow : DaggerfallInventoryWindow, IMacroContextProvider
    {
        #region UI Rects

        Rect costPanelRect = new Rect(0, 3, 111, 9);
        Rect costPanelPositionRect = new Rect(49, 13, 111, 9);

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
        TextLabel[] remoteItemsRepairLabels = new TextLabel[listDisplayUnits];

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

        Color doneItemBackgroundColor = new Color(0.1f, 0.2f, 0.6f, 0.5f);

        #endregion

        #region UI Textures

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
        BuildingSummary buildingSummary;

        ItemCollection merchantItems = new ItemCollection();
        bool usingWagon = false;
        int cost = 0;

        static Dictionary<DFLocation.BuildingTypes, List<ItemGroups>> storeBuysItemType = new Dictionary<DFLocation.BuildingTypes, List<ItemGroups>>()
        {
            { DFLocation.BuildingTypes.Alchemist, new List<ItemGroups>()
                { ItemGroups.Gems, ItemGroups.CreatureIngredients1, ItemGroups.CreatureIngredients2, ItemGroups.CreatureIngredients3, ItemGroups.PlantIngredients1, ItemGroups.PlantIngredients2, ItemGroups.MiscellaneousIngredients1, ItemGroups.MiscellaneousIngredients2, ItemGroups.MetalIngredients } },
            { DFLocation.BuildingTypes.Armorer, new List<ItemGroups>()
                { ItemGroups.Armor, ItemGroups.Weapons } },
            { DFLocation.BuildingTypes.Bookseller, new List<ItemGroups>()
                { ItemGroups.Books } },
            { DFLocation.BuildingTypes.ClothingStore, new List<ItemGroups>()
                { ItemGroups.MensClothing, ItemGroups.WomensClothing } },
            { DFLocation.BuildingTypes.GemStore, new List<ItemGroups>()
                { ItemGroups.Gems, ItemGroups.Jewellery } },
            { DFLocation.BuildingTypes.GeneralStore, new List<ItemGroups>()
                { ItemGroups.Books, ItemGroups.MensClothing, ItemGroups.WomensClothing, /*ItemGroups.Transportation,*/ ItemGroups.Jewellery, ItemGroups.Weapons } },
            { DFLocation.BuildingTypes.PawnShop, new List<ItemGroups>()
                { ItemGroups.Armor, ItemGroups.Books, ItemGroups.MensClothing, ItemGroups.WomensClothing, ItemGroups.Gems, ItemGroups.Jewellery, ItemGroups.Weapons } },
            { DFLocation.BuildingTypes.WeaponSmith, new List<ItemGroups>()
                { ItemGroups.Armor, ItemGroups.Weapons } },
        };

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

        public DaggerfallTradeWindow(IUserInterfaceManager uiManager, WindowModes windowMode, DaggerfallBaseWindow previous = null)
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
            UpdateCostAndGold();
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

            if (windowMode == WindowModes.Buy)
            {
                stealButton = DaggerfallUI.AddButton(stealButtonRect, actionButtonsPanel);
                stealButton.OnMouseClick += StealButton_OnMouseClick;
            }
            modeActionButton = DaggerfallUI.AddButton(modeActionButtonRect, actionButtonsPanel);
            modeActionButton.OnMouseClick += ModeActionButton_OnMouseClick;

            clearButton = DaggerfallUI.AddButton(clearButtonRect, actionButtonsPanel);
            clearButton.OnMouseClick += ClearButton_OnMouseClick;

        }

        protected override void SetupRemoteItemsElements()
        {
            base.SetupRemoteItemsElements();

            // Setup repair labels
            if (windowMode == WindowModes.Repair)
            {
                for (int i = 0; i < listDisplayUnits; i++)
                {
                    remoteItemsRepairLabels[i] = DaggerfallUI.AddTextLabel(DaggerfallUI.Instance.Font4, Vector2.zero, string.Empty, remoteItemsButtons[i]);
                    remoteItemsRepairLabels[i].HorizontalAlignment = HorizontalAlignment.Left;
                    remoteItemsRepairLabels[i].VerticalAlignment = VerticalAlignment.Top;
                }
            }
        }

        #endregion

        #region Public Methods

        public override void OnPush()
        {
            // Get building info, close if invalid
            buildingSummary = GameManager.Instance.PlayerEnterExit.BuildingSummary;
            //Debug.Log(string.Format("{0} {1} {2}", buildingSummary.buildingKey, buildingSummary.BuildingType, buildingSummary.Quality));
            if (buildingSummary.buildingKey <= 0)
                DaggerfallUI.MessageBox(HardStrings.oldSaveNoTrade, true);

            // Local items starts pointing to player inventory
            localItems = PlayerEntity.Items;

            // Initialise merchant items collection
            merchantItems.Clear();
            remoteItems = merchantItems;
            remoteTargetType = RemoteTargetTypes.Merchant;

            // Clear wagon button state
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
            ClearSelectedItems();
        }

        public override void Refresh(bool refreshPaperDoll = true)
        {
            if (!IsSetup)
                return;

            base.Refresh(refreshPaperDoll);

            UpdateCostAndGold();
        }

        #endregion

        #region Pricing

        private void UpdateCostAndGold()
        {
            cost = 0;
            for (int i = 0; i < remoteItems.Count; i++)
            {
                DaggerfallUnityItem item = remoteItems.GetItem(i);
                switch (windowMode)
                {
                    case WindowModes.Sell:
                        cost += FormulaHelper.CalculateItemSellCost(item.value, buildingSummary.Quality) * item.stackCount;
                        break;
                    case WindowModes.Repair:
                        cost += FormulaHelper.CalculateItemRepairCost(item.value, buildingSummary.Quality, item.currentCondition, item.maxCondition) * item.stackCount;
                        break;
                }
            }
            costLabel.Text = cost.ToString();
            goldLabel.Text = PlayerEntity.GoldPieces.ToString();
        }

        private int GetTradePrice()
        {
            return FormulaHelper.CalculateTradePrice(cost, buildingSummary.Quality);
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

        void ClearSelectedItems()
        {
            if (windowMode != WindowModes.Buy)
            {
                // Return items to player inventory. 
                // Note: ignoring weight here, like classic. Priority is to not lose any items.
                PlayerEntity.Items.TransferAll(remoteItems);
            }
        }

        protected override void ClearRemoteItemsElements()
        {
            if (windowMode == WindowModes.Repair)
                for (int i = 0; i < listDisplayUnits; i++)
                    remoteItemsRepairLabels[i].Text = String.Empty;

            base.ClearRemoteItemsElements();
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

        protected override void FilterLocalItems()
        {
            base.FilterLocalItems();

            if (windowMode == WindowModes.Sell)
            {
                // Remove any items not accepted by this merchant type.
                List<ItemGroups> itemTypesAccepted = storeBuysItemType[buildingSummary.BuildingType];
                localItemsFiltered.RemoveAll(i => !itemTypesAccepted.Contains(i.ItemGroup));
            }
            // Do repair/identify have restrictions?
            // repair: not in classic, the condition is checked (which means only weps & armour since only they get damage I think)
            // identify: ?
        }

        protected override void SetItemBackgroundColour(DaggerfallUnityItem item, int i, bool local)
        {
            Button itemButton = (local) ? localItemsButtons[i] : remoteItemsButtons[i];
            TextLabel itemLabel = (local) ? null : remoteItemsRepairLabels[i];

            if (!local && windowMode == WindowModes.Repair && item.currentCondition == item.maxCondition)
            {
                if (itemLabel != null)
                    itemLabel.Text = HardStrings.repairDone + i;
                itemButton.BackgroundColor = doneItemBackgroundColor;
            }
            else
            {
                if (itemLabel != null)
                    itemLabel.Text = String.Empty;
                base.SetItemBackgroundColour(item, i, local);
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

        #region Item Click Event Handlers

        protected override void LocalItemsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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
            if (selectedActionMode == ActionModes.Select)
            {
                if (windowMode == WindowModes.Sell)
                {
                    // Transfer to remote items
                    if (remoteItems != null)
                        TransferItem(item, localItems, remoteItems);
                }
                else if (windowMode == WindowModes.Repair)
                {
                    // Check if item is damaged & transfer
                    if (item.currentCondition < item.maxCondition)
                        TransferItem(item, localItems, remoteItems);
                    else
                        DaggerfallUI.MessageBox(24);
                }

            }
            else if (selectedActionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        protected override void RemoteItemsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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
            if (selectedActionMode == ActionModes.Select)
            {
                if (CanCarry(item) || (usingWagon && WagonCanHold(item)))
                    TransferItem(item, remoteItems, localItems);
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

        private void StealButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (windowMode == WindowModes.Buy)
            {
                // TODO
            }
        }

        private void ModeActionButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ShowTradePopup();
        }

        private void ClearButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ClearSelectedItems();
            Refresh();
        }

        private void ConfirmTrade_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                // Proceed with trade.
                switch (windowMode)
                {
                    case WindowModes.Sell:
                        PlayerEntity.GoldPieces += GetTradePrice();
                        remoteItems.Clear();
                        break;
                    case WindowModes.Repair:
                        PlayerEntity.GoldPieces -= GetTradePrice();
                        for (int i = 0; i < remoteItems.Count; i++)
                        {
                            DaggerfallUnityItem item = remoteItems.GetItem(i);
                            item.currentCondition = item.maxCondition;
                        }
                        break;
                }
                DaggerfallUI.Instance.PlayOneShot(SoundClips.GoldPieces);
                Refresh();
            }
            CloseWindow();
        }

        #endregion

        void ShowTradePopup()
        {
            const int tradeMessageBaseId = 260;
            const int notEnoughGoldId = 454;
            int msgOffset = 0;

            if (windowMode != WindowModes.Sell && PlayerEntity.GoldPieces < GetTradePrice())
            {
                DaggerfallUI.MessageBox(notEnoughGoldId);
            }
            else
            {
                // TODO what is classic algorithm? (seems repair can use all even though not correct contextually)
                if (windowMode == WindowModes.Buy)
                    msgOffset = (buildingSummary.Quality > 10) ? 1 : 0;
                else
                    msgOffset = 1 + (buildingSummary.Quality / 5);

                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(tradeMessageBaseId + msgOffset);
                messageBox.SetTextTokens(tokens, this);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.OnButtonClick += ConfirmTrade_OnButtonClick;
                uiManager.PushWindow(messageBox);
            }
        }


        protected override void StartGameBehaviour_OnNewGame()
        {
            // Do nothing when game starts, as this window class is not used in a persisted manner like its parent.
        }

        #region Macro handling

        public MacroDataSource GetMacroDataSource()
        {
            return new TradeMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for items in Daggerfall Unity.
        /// </summary>
        private class TradeMacroDataSource : MacroDataSource
        {
            private DaggerfallTradeWindow parent;
            public TradeMacroDataSource(DaggerfallTradeWindow tradeWindow)
            {
                this.parent = tradeWindow;
            }

            public override string Amount()
            {
                return parent.GetTradePrice().ToString();
            }

            public override string ShopName()
            {
                return "unknown"; //parent.buildingData.;
            }
        }

        #endregion
    }
}