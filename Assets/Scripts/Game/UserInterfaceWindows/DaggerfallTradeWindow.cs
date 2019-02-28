// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Guilds;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements trade windows, based on inventory window.
    /// </summary>
    public partial class DaggerfallTradeWindow : DaggerfallInventoryWindow, IMacroContextProvider
    {
        #region UI Rects

        Rect costPanelRect = new Rect(49, 13, 111, 9);

        Rect actionButtonsPanelRect = new Rect(222, 10, 39, 190);
        Rect wagonButtonRect = new Rect(4, 4, 31, 14);
        Rect infoButtonRect = new Rect(4, 26, 31, 14);
        Rect selectButtonRect = new Rect(4, 48, 31, 14);
        Rect stealButtonRect = new Rect(4, 102, 31, 14);
        Rect modeActionButtonRect = new Rect(4, 124, 31, 14);
        Rect clearButtonRect = new Rect(4, 146, 31, 14);

        Rect itemInfoPanelRect = new Rect(223, 87, 37, 32);
        Rect itemBuyInfoPanelRect = new Rect(223, 76, 37, 32);

        #endregion

        #region UI Controls

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
        const string identifyButtonsTextureName = "INVE14I0.IMG";
        const string costPanelTextureName = "SHOP00I0.IMG";

        const int doesNotNeedToBeRepairedTextId = 24;
        const int magicItemsCannotBeRepairedTextId = 33;

        Color repairItemBackgroundColor = new Color(0.17f, 0.32f, 0.7f, 0.6f);

        WindowModes windowMode = WindowModes.Inventory;
        Guild guild;

        PlayerGPS.DiscoveredBuilding buildingDiscoveryData;
        List<ItemGroups> itemTypesAccepted = storeBuysItemType[DFLocation.BuildingTypes.GeneralStore];

        ItemCollection merchantItems = new ItemCollection();
        ItemCollection basketItems = new ItemCollection();

        bool usingWagon = false;
        int cost = 0;
        bool usingIdentifySpell = false;
        DaggerfallUnityItem itemBeingRepaired;

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
                { ItemGroups.Books, ItemGroups.MensClothing, ItemGroups.WomensClothing, ItemGroups.Transportation, ItemGroups.Jewellery, ItemGroups.Weapons, ItemGroups.UselessItems2 } },
            { DFLocation.BuildingTypes.PawnShop, new List<ItemGroups>()
                { ItemGroups.Armor, ItemGroups.Books, ItemGroups.MensClothing, ItemGroups.WomensClothing, ItemGroups.Gems, ItemGroups.Jewellery, ItemGroups.ReligiousItems, ItemGroups.Weapons, ItemGroups.UselessItems2 } },
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
            SellMagic
        }

        #endregion

        #region Properties

        public ItemCollection MerchantItems
        {
            get { return merchantItems; }
            set { merchantItems = value; }
        }

        public bool UsingIdentifySpell
        {
            get { return usingIdentifySpell; }
            set { usingIdentifySpell = value; }
        }

        #endregion

        #region Constructors

        public DaggerfallTradeWindow(IUserInterfaceManager uiManager, WindowModes windowMode, DaggerfallBaseWindow previous = null, Guild guild = null)
            : base(uiManager, previous)
        {
            this.windowMode = windowMode;
            this.guild = guild;
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

            // Setup item info panel if configured
            if (DaggerfallUnity.Settings.EnableInventoryInfoPanel)
            {
                if (windowMode == WindowModes.Buy)
                    itemInfoPanel = DaggerfallUI.AddPanel(itemBuyInfoPanelRect, NativePanel);
                else
                    itemInfoPanel = DaggerfallUI.AddPanel(itemInfoPanelRect, NativePanel);
                SetupItemInfoPanel();
            }

            // Setup UI
            SetupTargetIconPanels();
            SetupTabPageButtons();
            SetupActionButtons();
            SetupAccessoryElements();
            SetupItemListScrollers();

            // Highlight purchasable items
            if (windowMode == WindowModes.Buy)
            {
                localItemListScroller.BackgroundAnimationHandler = BuyItemBackgroundAnimationHandler;
                remoteItemListScroller.BackgroundAnimationHandler = BuyItemBackgroundAnimationHandler;
                localItemListScroller.BackgroundAnimationDelay = coinsAnimationDelay;
                remoteItemListScroller.BackgroundAnimationDelay = coinsAnimationDelay;
            }
            // Setup special behaviour for remote items when repairing
            if (windowMode == WindowModes.Repair) {
                remoteItemListScroller.BackgroundColourHandler = RepairItemBackgroundColourHandler;
                remoteItemListScroller.LabelTextHandler = RepairItemLabelTextHandler;
            }
            // Exit buttons
            Button exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            // Setup initial state
            SelectTabPage((windowMode == WindowModes.Identify) ? TabPages.MagicItems : TabPages.WeaponsAndArmor);
            SelectActionMode(ActionModes.Select);

            // Setup initial display
            FilterLocalItems();
            localItemListScroller.Items = localItemsFiltered;
            FilterRemoteItems();
            remoteItemListScroller.Items = remoteItemsFiltered;
            UpdateAccessoryItemsDisplay();
            UpdateLocalTargetIcon();
            UpdateRemoteTargetIcon();
            UpdateCostAndGold();
        }

        Color RepairItemBackgroundColourHandler(DaggerfallUnityItem item)
        {
            if (DaggerfallUnity.Settings.InstantRepairs)
                return (item.currentCondition == item.maxCondition) ? repairItemBackgroundColor : Color.clear;
            else
                return (item.RepairData.IsBeingRepaired()) ? repairItemBackgroundColor : Color.clear;
        }

        Texture2D[] BuyItemBackgroundAnimationHandler(DaggerfallUnityItem item)
        {
            return (basketItems.Contains(item) || remoteItems.Contains(item)) ? coinsAnimation.animatedTextures : null;
        }

        string RepairItemLabelTextHandler(DaggerfallUnityItem item)
        {
            if (item.RepairData.IsBeingRepaired())
            {
                if (item.RepairData.IsRepairFinished())
                    return TextManager.Instance.GetText(textDatabase, "repairDone");
                else
                    return TextManager.Instance.GetText(textDatabase, "repairDays").Replace("%d", item.RepairData.DaysUntilRepaired().ToString());
            }
            return (item.currentCondition == item.maxCondition) ? TextManager.Instance.GetText(textDatabase, "repairDone") : String.Empty;
        }

        void SetupCostAndGold()
        {
            costPanel = DaggerfallUI.AddPanel(costPanelRect, NativePanel);
            costPanel.BackgroundTexture = costPanelTexture;
            costLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(28, 2), costPanel);
            goldLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(68, 2), costPanel);
        }

        protected override void SetupActionButtons()
        {
            // Can happen using Identify spell https://forums.dfworkshop.net/viewtopic.php?f=24&t=1756
            if (!GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
            {
                wagonButton = DaggerfallUI.AddButton(wagonButtonRect, actionButtonsPanel);
                wagonButton.OnMouseClick += WagonButton_OnMouseClick;
            }

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

        #endregion

        #region Public Methods

        public override void OnPush()
        {
            // Identify spell can run anywhere - only get building info when not using spell
            if (!usingIdentifySpell)
            {
                // Get building info, message if invalid, otherwise setup acccepted item list
                buildingDiscoveryData = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;
                if (buildingDiscoveryData.buildingKey <= 0)
                    DaggerfallUI.MessageBox(HardStrings.oldSaveNoTrade, true);
                else if (windowMode == WindowModes.Sell)
                    itemTypesAccepted = storeBuysItemType[buildingDiscoveryData.buildingType];
            }

            // Local items starts pointing to player inventory
            localItems = PlayerEntity.Items;

            // Initialise remote items
            remoteItems = (windowMode == WindowModes.Repair) ? PlayerEntity.OtherItems : merchantItems;
            remoteTargetType = RemoteTargetTypes.Merchant;

            // Clear wagon button state
            if (wagonButton != null)
            {
                usingWagon = false;
                wagonButton.BackgroundTexture = wagonNotSelected;
            }

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

            // Identify spell remains free
            if (usingIdentifySpell)
            {
                costLabel.Text = cost.ToString();
                goldLabel.Text = PlayerEntity.GetGoldAmount().ToString();
                return;
            }

            if (windowMode == WindowModes.Buy && basketItems != null)
            {
                for (int i = 0; i < basketItems.Count; i++)
                {
                    DaggerfallUnityItem item = basketItems.GetItem(i);
                    cost += FormulaHelper.CalculateCost(item.value, buildingDiscoveryData.quality) * item.stackCount;
                }
            }
            else if (remoteItems != null)
            {
                for (int i = 0; i < remoteItems.Count; i++)
                {
                    DaggerfallUnityItem item = remoteItems.GetItem(i);
                    switch (windowMode)
                    {
                        case WindowModes.Sell:
                            cost += FormulaHelper.CalculateCost(item.value, buildingDiscoveryData.quality) * item.stackCount;
                            break;
                        case WindowModes.SellMagic: // TODO: Fencing base price higher and guild rep affects it. Implement new formula or can this be used?
                            cost += FormulaHelper.CalculateCost(item.value, buildingDiscoveryData.quality);
                            break;
                        case WindowModes.Repair:
                            cost += FormulaHelper.CalculateItemRepairCost(item.value, buildingDiscoveryData.quality, item.currentCondition, item.maxCondition, guild) * item.stackCount;
                            break;
                        case WindowModes.Identify:
                            if (!item.IsIdentified)
                                cost += FormulaHelper.CalculateItemIdentifyCost(item.value, guild);
                            break;
                    }
                }
            }
            costLabel.Text = cost.ToString();
            goldLabel.Text = PlayerEntity.GetGoldAmount().ToString();
        }

        private int GetTradePrice()
        {
            if (windowMode == WindowModes.Sell || windowMode == WindowModes.SellMagic)
                return FormulaHelper.CalculateTradePrice(cost, buildingDiscoveryData.quality, true);
            else if (windowMode == WindowModes.Identify)
                return cost;
            else
                return FormulaHelper.CalculateTradePrice(cost, buildingDiscoveryData.quality, false);
        }

        #endregion

        #region Helper Methods

        protected void SelectActionMode(ActionModes mode)
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

        protected void ClearSelectedItems()
        {
            if (windowMode == WindowModes.Buy)
            {   // Return all basket items to merchant, unequipping if necessary.
                for (int i = 0; i < basketItems.Count; i++)
                {
                    DaggerfallUnityItem item = basketItems.GetItem(i);
                    if (item.IsEquipped)
                        UnequipItem(item);
                }
                remoteItems.TransferAll(basketItems);
            }
            else if (windowMode == WindowModes.Repair)
            {   // Return all items not actively being repaired.
                foreach (DaggerfallUnityItem item in remoteItemsFiltered)
                {
                    if (!item.RepairData.IsBeingRepaired() || item.RepairData.IsRepairFinished())
                    {
                        localItems.Transfer(item, remoteItems);
                        item.RepairData.Collect();
                    }
                }
            }
            else
            {   // Return items to player inventory. 
                // Note: ignoring weight here, like classic. Priority is to not lose any items.
                localItems.TransferAll(remoteItems);
            }
        }

        protected override float GetCarriedWeight()
        {
            return PlayerEntity.CarriedWeight + basketItems.GetWeight();
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
                base.UpdateLocalTargetIcon();
            }
        }

        protected override void UpdateRemoteTargetIcon()
        {
            ImageData containerImage;
            switch (windowMode)
            {
                default:
                case WindowModes.Sell:
                case WindowModes.SellMagic:
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
            localItemsFiltered.Clear();

            // Add any basket items to filtered list first, if not using wagon
            if (windowMode == WindowModes.Buy && !usingWagon && basketItems != null)
            {
                for (int i = 0; i < basketItems.Count; i++)
                {
                    DaggerfallUnityItem item = basketItems.GetItem(i);
                    // Add if not equipped
                    if (!item.IsEquipped)
                        AddLocalItem(item);
                }
            }
            // Add local items to filtered list
            if (localItems != null)
            {
                for (int i = 0; i < localItems.Count; i++)
                {
                    // Add if not equipped & accepted for selling
                    DaggerfallUnityItem item = localItems.GetItem(i);
                    if (!item.IsEquipped && (
                            (windowMode != WindowModes.Sell && windowMode != WindowModes.SellMagic) ||
                            (windowMode == WindowModes.Sell && itemTypesAccepted.Contains(item.ItemGroup)) ||
                            (windowMode == WindowModes.SellMagic && item.IsEnchanted) ))
                    {
                        AddLocalItem(item);
                    }
                }
            }
        }

        protected override void FilterRemoteItems()
        {
            if (windowMode == WindowModes.Repair)
            {
                // Clear current references
                remoteItemsFiltered.Clear();

                // Add items to list if they are not being repaired or are being repaired here. 
                if (remoteItems != null)
                {
                    for (int i = 0; i < remoteItems.Count; i++)
                    {
                        DaggerfallUnityItem item = remoteItems.GetItem(i);
                        if (!item.RepairData.IsBeingRepaired() || item.RepairData.IsBeingRepairedHere())
                            remoteItemsFiltered.Add(item);
                        if (item.RepairData.IsRepairFinished())
                            item.currentCondition = item.maxCondition;
                    }
                }
            }
            else
                base.FilterRemoteItems();
        }

        protected void ShowWagon(bool show)
        {
            if (show)
            {   // Switch to wagon
                wagonButton.BackgroundTexture = wagonSelected;
                localItems = PlayerEntity.WagonItems;
            }
            else
            {   // Restore previous target or default to dropped items
                wagonButton.BackgroundTexture = wagonNotSelected;
                localItems = PlayerEntity.Items;
            }
            usingWagon = show;
            localItemListScroller.ResetScroll();
            Refresh(false);
        }

        protected override void LoadTextures()
        {
            base.LoadTextures();

            // Load special button texture.
            if (windowMode == WindowModes.Sell || windowMode == WindowModes.SellMagic) {
                actionButtonsTexture = ImageReader.GetTexture(sellButtonsTextureName);
            } else if (windowMode == WindowModes.Buy) {
                actionButtonsTexture = ImageReader.GetTexture(buyButtonsTextureName);
            } else if (windowMode == WindowModes.Repair) {
                actionButtonsTexture = ImageReader.GetTexture(repairButtonsTextureName);
            } else if (windowMode == WindowModes.Identify) {
                actionButtonsTexture = ImageReader.GetTexture(identifyButtonsTextureName);
            }
            actionButtonsGoldTexture = ImageReader.GetTexture(sellButtonsGoldTextureName);
            DFSize actionButtonsFullSize = new DFSize(39, 190);
            selectNotSelected = ImageReader.GetSubTexture(actionButtonsTexture, selectButtonRect, actionButtonsFullSize);
            selectSelected = ImageReader.GetSubTexture(actionButtonsGoldTexture, selectButtonRect, actionButtonsFullSize);

            costPanelTexture = ImageReader.GetTexture(costPanelTextureName);
        }

        #endregion

        #region Item Click Event Handlers

        protected override void LocalItemListScroller_OnItemClick(DaggerfallUnityItem item)
        {
            // Handle click based on action & mode
            if (selectedActionMode == ActionModes.Select)
            {
                switch (windowMode)
                {
                    case WindowModes.Sell:
                    case WindowModes.SellMagic:
                        if (remoteItems != null)
                            TransferItem(item, localItems, remoteItems, blockTransport: true);
                        break;

                    case WindowModes.Buy:
                        if (usingWagon)     // Allows player to get & equip stuff from cart while purchasing.
                            TransferItem(item, localItems, PlayerEntity.Items, CanCarryAmount(item), equip: !item.IsAStack());
                        else                // Allows player to equip and unequip while purchasing.
                            EquipItem(item);
                        break;

                    case WindowModes.Repair:
                        // Check that item can be repaired, is damaged & transfer if so.
                        if (item.IsEnchanted)
                            DaggerfallUI.MessageBox(magicItemsCannotBeRepairedTextId);
                        else if ((item.currentCondition < item.maxCondition) && item.TemplateIndex != (int)Weapons.Arrow)
                            TransferItem(item, localItems, remoteItems);
                        else
                            DaggerfallUI.MessageBox(doesNotNeedToBeRepairedTextId);
                        break;

                    case WindowModes.Identify:
                        // Check if item is unidentified & transfer
                        if (!item.IsIdentified)
                            TransferItem(item, localItems, remoteItems);
                        else
                            DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "doesntNeedIdentify"));
                        break;
                }
            }
            else if (selectedActionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        protected override void RemoteItemListScroller_OnItemClick(DaggerfallUnityItem item)
        {
            // Handle click based on action
            if (selectedActionMode == ActionModes.Select)
            {
                if (windowMode == WindowModes.Buy)
                    TransferItem(item, remoteItems, basketItems, CanCarryAmount(item), equip: !item.IsAStack());
                else if (windowMode == WindowModes.Repair)
                {
                    if (item.RepairData.IsBeingRepaired() && !item.RepairData.IsRepairFinished())
                    {
                        itemBeingRepaired = item;
                        String strInterruptRepair = TextManager.Instance.GetText(textDatabase, "interruptRepair");
                        DaggerfallMessageBox confirmInterruptRepairBox = new DaggerfallMessageBox(uiManager, DaggerfallMessageBox.CommonMessageBoxButtons.YesNo, strInterruptRepair, this);
                        confirmInterruptRepairBox.OnButtonClick += ConfirmInterruptRepairBox_OnButtonClick;
                        confirmInterruptRepairBox.Show();
                    }
                    else
                        TakeItemFromRepair(item);
                }
                else
                    TransferItem(item, remoteItems, localItems, usingWagon ? WagonCanHoldAmount(item) : CanCarryAmount(item));
            }
            else if (selectedActionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        private void ConfirmInterruptRepairBox_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                TakeItemFromRepair(itemBeingRepaired);
            }
        }

        private void TakeItemFromRepair(DaggerfallUnityItem item)
        {
            TransferItem(item, remoteItems, localItems, usingWagon ? WagonCanHoldAmount(item) : CanCarryAmount(item));
            item.RepairData.Collect();
        }

        #endregion

        #region Action Button Event Handlers

        private void WagonButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (PlayerEntity.Items.Contains(ItemGroups.Transportation, (int) Transportation.Small_cart))
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
            if (windowMode == WindowModes.Buy && cost > 0)
            {
                // Calculate the weight of all items picked from shelves, then get chance of shoplifting success.
                int weightAndNumItems = (int) basketItems.GetWeight() + basketItems.Count;
                int chance = FormulaHelper.CalculateShopliftingChance(PlayerEntity, null, buildingDiscoveryData.quality, weightAndNumItems);
                PlayerEntity.TallySkill(DFCareer.Skills.Pickpocket, 1);

                if (UnityEngine.Random.Range(0, 101) > chance)
                {
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "youAreSuccessful"), 2);
                    RaiseOnTradeHandler(basketItems.GetNumItems(), 0);
                    PlayerEntity.Items.TransferAll(basketItems);
                    PlayerEntity.TallyCrimeGuildRequirements(true, 1);
                }
                else
                {   // Register crime and start spawning guards.
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "youAreNotSuccessful"), 2);
                    RaiseOnTradeHandler(0, 0);
                    PlayerEntity.CrimeCommitted = PlayerEntity.Crimes.Theft;
                    PlayerEntity.SpawnCityGuards(true);
                }
                CloseWindow();
            }
        }

        private void ModeActionButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (usingIdentifySpell)
            {   // No trade when using a spell, just identify immediately
                for (int i = 0; i < remoteItems.Count; i++)
                    remoteItems.GetItem(i).IdentifyItem();
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "itemsIdentified"));
            }
            else if (cost > 0 || ((windowMode == WindowModes.Repair || windowMode == WindowModes.Identify) && remoteItems.Count > 0))
                ShowTradePopup();
        }

        private void ClearButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ClearSelectedItems();
            Refresh();
        }

        private void ConfirmTrade_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            bool receivedLetterOfCredit = false;
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                // Proceed with trade.
                int tradePrice = GetTradePrice();
                switch (windowMode)
                {
                    case WindowModes.Sell:
                    case WindowModes.SellMagic:
                        float goldWeight = tradePrice * DaggerfallBankManager.goldUnitWeightInKg;
                        if (PlayerEntity.CarriedWeight + goldWeight <= PlayerEntity.MaxEncumbrance)
                        {
                            PlayerEntity.GoldPieces += tradePrice;
                        }
                        else
                        {
                            DaggerfallUnityItem loc = ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Letter_of_credit);
                            loc.value = tradePrice;
                            GameManager.Instance.PlayerEntity.Items.AddItem(loc, Items.ItemCollection.AddPosition.Front);
                            receivedLetterOfCredit = true;
                        }
                        RaiseOnTradeHandler(remoteItems.GetNumItems(), tradePrice);
                        remoteItems.Clear();
                        break;

                    case WindowModes.Buy:
                        PlayerEntity.DeductGoldAmount(tradePrice);
                        RaiseOnTradeHandler(basketItems.GetNumItems(), tradePrice);
                        PlayerEntity.Items.TransferAll(basketItems);
                        break;

                    case WindowModes.Repair:
                        PlayerEntity.DeductGoldAmount(tradePrice);
                        if (DaggerfallUnity.Settings.InstantRepairs)
                        {
                            foreach (DaggerfallUnityItem item in remoteItemsFiltered)
                                item.currentCondition = item.maxCondition;
                        }
                        else
                        {
                            int totalRepairTime = 0, longestRepairTime = 0;
                            DaggerfallUnityItem itemLongestTime = null;
                            foreach (DaggerfallUnityItem item in remoteItemsFiltered)
                            {
                                int repairTime = FormulaHelper.CalculateItemRepairTime(item.currentCondition, item.maxCondition);
                                if (!item.RepairData.IsBeingRepaired())
                                {
                                    item.RepairData.LeaveForRepair(repairTime);
                                    string note = string.Format(TextManager.Instance.GetText(textDatabase, "repairNote"), item.LongName, buildingDiscoveryData.displayName);
                                    GameManager.Instance.PlayerEntity.Notebook.AddNote(note);
                                }
                                totalRepairTime += repairTime;
                                if (repairTime > longestRepairTime)
                                {
                                    longestRepairTime = repairTime;
                                    itemLongestTime = item;
                                }
                                item.RepairData.RepairTime = repairTime;
                            }
                            if (itemLongestTime != null)
                                itemLongestTime.RepairData.RepairTime = longestRepairTime + ((totalRepairTime - longestRepairTime) / 2);
                        }
                        RaiseOnTradeHandler(remoteItems.GetNumItems(), tradePrice);
                        break;

                    case WindowModes.Identify:
                        PlayerEntity.DeductGoldAmount(GetTradePrice());
                        for (int i = 0; i < remoteItems.Count; i++)
                        {
                            DaggerfallUnityItem item = remoteItems.GetItem(i);
                            item.IdentifyItem();
                        }
                        RaiseOnTradeHandler(remoteItems.GetNumItems(), tradePrice);
                        break;
                }
                if (receivedLetterOfCredit)
                    DaggerfallUI.Instance.PlayOneShot(SoundClips.ParchmentScratching);
                else
                    DaggerfallUI.Instance.PlayOneShot(SoundClips.GoldPieces);
                PlayerEntity.TallySkill(DFCareer.Skills.Mercantile, 1);
                Refresh();
            }
            CloseWindow();
            if (receivedLetterOfCredit)
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "letterOfCredit"));
        }

        #endregion

        #region Misc Events & Helpers

        void ShowTradePopup()
        {
            const int tradeMessageBaseId = 260;
            const int notEnoughGoldId = 454;
            int msgOffset = 0;
            int tradePrice = GetTradePrice();

            if (windowMode != WindowModes.Sell && windowMode != WindowModes.SellMagic && PlayerEntity.GetGoldAmount() < tradePrice)
            {
                DaggerfallUI.MessageBox(notEnoughGoldId);
            }
            else
            {
                if (cost >> 1 <= tradePrice)
                {
                    if (cost - (cost >> 2) <= tradePrice)
                        msgOffset = 2;
                    else
                        msgOffset = 1;
                }
                if (windowMode == WindowModes.Sell || windowMode == WindowModes.SellMagic)
                    msgOffset += 3;

                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(tradeMessageBaseId + msgOffset);
                messageBox.SetTextTokens(tokens, this);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.OnButtonClick += ConfirmTrade_OnButtonClick;
                uiManager.PushWindow(messageBox);
            }
        }

        // OnTrade event. (value=0:steal, numItems=0:caught)
        public delegate void OnTradeHandler(WindowModes mode, int numItems, int value);
        public event OnTradeHandler OnTrade;
        protected virtual void RaiseOnTradeHandler(int numItems, int value)
        {
            if (OnTrade != null)
                OnTrade(windowMode, numItems, value);
        }

        protected override void StartGameBehaviour_OnNewGame()
        {
            // Do nothing when game starts, as this window class is not used in a persisted manner like its parent.
        }

        #endregion

        #region Macro handling

        public MacroDataSource GetMacroDataSource()
        {
            return new TradeMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for trade window.
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
                return parent.buildingDiscoveryData.displayName;
            }

            public override string GuildTitle()
            {
                if (parent.guild != null)
                    return parent.guild.GetTitle();
                else
                    return MacroHelper.GetFirstname(GameManager.Instance.PlayerEntity.Name);
            }
        }

        #endregion
    }
}
