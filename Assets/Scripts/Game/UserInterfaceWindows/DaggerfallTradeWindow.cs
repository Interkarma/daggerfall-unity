// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    Pango
//
// Notes:
//
using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using UnityEngine;

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
        new Rect wagonButtonRect = new Rect(4, 4, 31, 14);
        new Rect infoButtonRect = new Rect(4, 26, 31, 14);
        Rect selectButtonRect = new Rect(4, 48, 31, 14);
        Rect stealButtonRect = new Rect(4, 102, 31, 14);
        Rect modeActionButtonRect = new Rect(4, 124, 31, 14);
        Rect clearButtonRect = new Rect(4, 146, 31, 14);

        new Rect itemInfoPanelRect = new Rect(223, 87, 37, 32);
        Rect itemBuyInfoPanelRect = new Rect(223, 76, 37, 32);

        #endregion

        #region UI Controls

        Panel costPanel;
        TextLabel costLabel;
        TextLabel goldLabel;

        Panel actionButtonsPanel;
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

        PlayerGPS.DiscoveredBuilding buildingDiscoveryData;
        List<ItemGroups> itemTypesAccepted = storeBuysItemType[DFLocation.BuildingTypes.GeneralStore];

        protected ItemCollection merchantItems = new ItemCollection();
        protected ItemCollection basketItems = new ItemCollection();

        protected int cost = 0;
        bool usingIdentifySpell = false;
        DaggerfallUnityItem itemBeingRepaired;

        bool suppressInventory = false;
        string suppressInventoryMessage = string.Empty;

        bool isStealDeferred = false;
        bool isModeActionDeferred = false;

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
            { DFLocation.BuildingTypes.FurnitureStore, new List<ItemGroups>()
                { ItemGroups.Furniture } },
            { DFLocation.BuildingTypes.GemStore, new List<ItemGroups>()
                { ItemGroups.Gems, ItemGroups.Jewellery } },
            { DFLocation.BuildingTypes.GeneralStore, new List<ItemGroups>()
                { ItemGroups.Books, ItemGroups.MensClothing, ItemGroups.WomensClothing, ItemGroups.Transportation, ItemGroups.Jewellery, ItemGroups.Weapons, ItemGroups.UselessItems2 } },
            { DFLocation.BuildingTypes.PawnShop, new List<ItemGroups>()
                { ItemGroups.Armor, ItemGroups.Books, ItemGroups.MensClothing, ItemGroups.WomensClothing, ItemGroups.Gems, ItemGroups.Jewellery, ItemGroups.ReligiousItems, ItemGroups.Weapons, ItemGroups.UselessItems2, ItemGroups.Paintings } },
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

        protected WindowModes WindowMode { get; private set; }
        protected IGuild Guild { get; private set; }

        protected List<ItemGroups> ItemTypesAccepted
        {
            get { return itemTypesAccepted; }
        }

        protected bool UsingWagon { get; private set; }

        protected ItemCollection BasketItems
        {
            get { return basketItems; }
        }

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

        public DaggerfallTradeWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null, WindowModes windowMode = WindowModes.Sell, IGuild guild = null)
            : base(uiManager, previous)
        {
            this.WindowMode = windowMode;
            this.Guild = guild;
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
                if (WindowMode == WindowModes.Buy)
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
            if (WindowMode == WindowModes.Buy)
            {
                localItemListScroller.BackgroundAnimationHandler = BuyItemBackgroundAnimationHandler;
                remoteItemListScroller.BackgroundAnimationHandler = BuyItemBackgroundAnimationHandler;
                localItemListScroller.BackgroundAnimationDelay = coinsAnimationDelay;
                remoteItemListScroller.BackgroundAnimationDelay = coinsAnimationDelay;
            }
            // Setup special behaviour for remote items when repairing
            if (WindowMode == WindowModes.Repair) {
                remoteItemListScroller.BackgroundColourHandler = RepairItemBackgroundColourHandler;
                remoteItemListScroller.LabelTextHandler = RepairItemLabelTextHandler;
            }
            // Exit buttons
            Button exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeExit);
            //exitButton.OnKeyboardEvent += ExitButton_OnKeyboardEvent;

            // Setup initial state
            SelectTabPage((WindowMode == WindowModes.Identify) ? TabPages.MagicItems : TabPages.WeaponsAndArmor);
            SelectActionMode(ActionModes.Select);

            // Setup initial display
            FilterLocalItems();
            localItemListScroller.Items = localItemsFiltered;
            FilterRemoteItems();
            remoteItemListScroller.Items = remoteItemsFiltered;
            UpdateAccessoryItemsDisplay();
            UpdateLocalTargetIcon();
            UpdateRemoteTargetIcon();
            // UpdateRepairTimes(false);
            UpdateCostAndGold();
            SelectWagon(false);
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
            bool repairDone = item.RepairData.IsBeingRepaired() ? item.RepairData.IsRepairFinished() : item.currentCondition == item.maxCondition;
            return repairDone ? 
                    TextManager.Instance.GetText(textDatabase, "repairDone") : 
                    TextManager.Instance.GetText(textDatabase, "repairDays").Replace("%d", item.RepairData.EstimatedDaysUntilRepaired().ToString());
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
                wagonButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeWagon);
            }

            infoButton = DaggerfallUI.AddButton(infoButtonRect, actionButtonsPanel);
            infoButton.OnMouseClick += InfoButton_OnMouseClick;
            infoButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeInfo);

            selectButton = DaggerfallUI.AddButton(selectButtonRect, actionButtonsPanel);
            selectButton.OnMouseClick += SelectButton_OnMouseClick;
            selectButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeSelect);

            if (WindowMode == WindowModes.Buy)
            {
                stealButton = DaggerfallUI.AddButton(stealButtonRect, actionButtonsPanel);
                stealButton.OnMouseClick += StealButton_OnMouseClick;
                stealButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeSteal);
                stealButton.OnKeyboardEvent += StealButton_OnKeyboardEvent;
            }
            modeActionButton = DaggerfallUI.AddButton(modeActionButtonRect, actionButtonsPanel);
            modeActionButton.OnMouseClick += ModeActionButton_OnMouseClick;
            switch (WindowMode)
            {
                case WindowModes.Buy:
                    modeActionButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeBuy);
                    break;
                case WindowModes.Identify:
                    modeActionButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeIdentify);
                    break;
                case WindowModes.Inventory:
                    // Shouldn't happen
                    break;
                case WindowModes.Repair:
                    modeActionButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeRepair);
                    break;
                case WindowModes.Sell:
                case WindowModes.SellMagic:
                    modeActionButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeSell);
                    break;
            }
            modeActionButton.OnKeyboardEvent += ModeActionButton_OnKeyboardEvent;

            clearButton = DaggerfallUI.AddButton(clearButtonRect, actionButtonsPanel);
            clearButton.OnMouseClick += ClearButton_OnMouseClick;
            clearButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeClear);
        }

        #endregion

        #region Public Methods

        public override void Update()
        {
            base.Update();

            // Close window immediately if trade suppressed
            if (suppressInventory)
            {
                CloseWindow();
                if (!string.IsNullOrEmpty(suppressInventoryMessage))
                    DaggerfallUI.MessageBox(suppressInventoryMessage);
                return;
            }
        }

        public override void OnPush()
        {
            // Racial override can suppress trade
            // We still setup and push window normally, actual suppression is done in Update()
            MagicAndEffects.MagicEffects.RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            if (racialOverride != null)
                suppressInventory = racialOverride.GetSuppressInventory(out suppressInventoryMessage);

            // Identify spell can run anywhere - only get building info when not using spell
            if (!usingIdentifySpell)
            {
                // Get building info, message if invalid, otherwise setup acccepted item list
                buildingDiscoveryData = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;
                if (buildingDiscoveryData.buildingKey <= 0)
                    DaggerfallUI.MessageBox(HardStrings.oldSaveNoTrade, true);
                else if (WindowMode == WindowModes.Sell)
                    itemTypesAccepted = storeBuysItemType[buildingDiscoveryData.buildingType];
            }

            // Local items starts pointing to player inventory
            localItems = PlayerEntity.Items;

            // Initialise remote items
            remoteItems = (WindowMode == WindowModes.Repair) ? PlayerEntity.OtherItems : merchantItems;
            remoteTargetType = RemoteTargetTypes.Merchant;

            // Clear wagon button state
            if (wagonButton != null)
            {
                SelectWagon(false);
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

            UpdateRepairTimes(false);
            UpdateCostAndGold();
        }

        #endregion

        #region Pricing

        private void UpdateCostAndGold()
        {
            bool modeActionEnabled = false;
            cost = 0;

            if (WindowMode == WindowModes.Buy && basketItems != null)
            {
                // Check holidays for half price sales:
                // - Merchants Festival, suns height 10th for normal shops
                // - Tales and Tallows hearth fire 3rd for mages guild
                // - Weapons on Warriors Festival suns dusk 20th
                uint minutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
                int holidayId = FormulaHelper.GetHolidayId(minutes, GameManager.Instance.PlayerGPS.CurrentRegionIndex);

                for (int i = 0; i < basketItems.Count; i++)
                {
                    DaggerfallUnityItem item = basketItems.GetItem(i);
                    modeActionEnabled = true;
                    int itemPrice = FormulaHelper.CalculateCost(item.value, buildingDiscoveryData.quality) * item.stackCount;
                    if ((holidayId == (int)DFLocation.Holidays.Merchants_Festival && Guild == null) ||
                        (holidayId == (int)DFLocation.Holidays.Tales_and_Tallow && Guild != null && Guild.GetFactionId() == (int)FactionFile.FactionIDs.The_Mages_Guild) ||
                        (holidayId == (int)DFLocation.Holidays.Warriors_Festival && Guild == null && item.ItemGroup == ItemGroups.Weapons))
                    {
                        itemPrice /= 2;
                    }
                    cost += itemPrice;
                }
            }
            else if (remoteItems != null)
            {
                for (int i = 0; i < remoteItems.Count; i++)
                {
                    DaggerfallUnityItem item = remoteItems.GetItem(i);
                    switch (WindowMode)
                    {
                        case WindowModes.Sell:
                            modeActionEnabled = true;
                            cost += FormulaHelper.CalculateCost(item.value, buildingDiscoveryData.quality, item.ConditionPercentage) * item.stackCount;
                            break;
                        case WindowModes.SellMagic: // TODO: Fencing base price higher and guild rep affects it. Implement new formula or can this be used?
                            modeActionEnabled = true;
                            cost += FormulaHelper.CalculateCost(item.value, buildingDiscoveryData.quality);
                            break;
                        case WindowModes.Repair:
                            if (!item.RepairData.IsBeingRepaired())
                            {
                                modeActionEnabled = true;
                                cost += FormulaHelper.CalculateItemRepairCost(item.value, buildingDiscoveryData.quality, item.currentCondition, item.maxCondition, Guild) * item.stackCount;
                            }
                            break;
                        case WindowModes.Identify:
                            if (!item.IsIdentified)
                            {
                                modeActionEnabled = true;
                                // Identify spell remains free
                                if (!usingIdentifySpell)
                                    cost += FormulaHelper.CalculateItemIdentifyCost(item.value, Guild);
                            }
                            break;
                    }
                }
            }
            costLabel.Text = cost.ToString();
            goldLabel.Text = PlayerEntity.GetGoldAmount().ToString();
            modeActionButton.Enabled = modeActionEnabled;
        }

        protected int GetTradePrice()
        {
            switch (WindowMode)
            {
                case WindowModes.Buy:
                case WindowModes.Repair:
                    return FormulaHelper.CalculateTradePrice(cost, buildingDiscoveryData.quality, false);

                case WindowModes.Sell:
                case WindowModes.SellMagic:
                    return FormulaHelper.CalculateTradePrice(cost, buildingDiscoveryData.quality, true);

                case WindowModes.Identify:
                    return cost;
            }
            throw new Exception("Unexpected windowMode");
        }

        #endregion

        #region Repairs

        protected void UpdateRepairTimes(bool commit)
        {
            if (WindowMode != WindowModes.Repair || DaggerfallUnity.Settings.InstantRepairs)
                return;

            Debug.Log("UpdateRepairTimes called");
            int totalRepairTime = 0, longestRepairTime = 0;
            DaggerfallUnityItem itemLongestTime = null;
            Dictionary<DaggerfallUnityItem, int> previousRepairTimes = new Dictionary<DaggerfallUnityItem, int>();
            foreach (DaggerfallUnityItem item in remoteItemsFiltered)
            {
                bool repairDone = item.RepairData.IsBeingRepaired() ? item.RepairData.IsRepairFinished() : item.currentCondition == item.maxCondition;
                if (repairDone)
                    continue;

                if (item.RepairData.IsBeingRepaired())
                    previousRepairTimes.Add(item, item.RepairData.RepairTime);

                int repairTime = FormulaHelper.CalculateItemRepairTime(item.currentCondition, item.maxCondition);
                if (commit && !item.RepairData.IsBeingRepaired())
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
                if (commit)
                    item.RepairData.RepairTime = repairTime;
                else
                    item.RepairData.EstimatedRepairTime = repairTime;
            }
            if (itemLongestTime != null)
            {
                int modifiedLongestTime = longestRepairTime + ((totalRepairTime - longestRepairTime) / 2);
                if (commit)
                    itemLongestTime.RepairData.RepairTime = modifiedLongestTime;
                else
                    itemLongestTime.RepairData.EstimatedRepairTime = modifiedLongestTime;
            }

            // Don't allow repair times to decrease (when removing other now repaired items)
            // https://forums.dfworkshop.net/viewtopic.php?f=24&t=2053
            foreach (KeyValuePair<DaggerfallUnityItem, int> entry in previousRepairTimes)
            {
                if (commit)
                    entry.Key.RepairData.RepairTime = Mathf.Max(entry.Key.RepairData.RepairTime, entry.Value);
                else
                    entry.Key.RepairData.EstimatedRepairTime = Mathf.Max(entry.Key.RepairData.EstimatedRepairTime, entry.Value);
            }
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
            if (WindowMode == WindowModes.Buy)
            {   // Return all basket items to merchant, unequipping if necessary.
                for (int i = 0; i < basketItems.Count; i++)
                {
                    DaggerfallUnityItem item = basketItems.GetItem(i);
                    if (item.IsEquipped)
                        UnequipItem(item);
                }
                remoteItems.TransferAll(basketItems);
            }
            else if (WindowMode == WindowModes.Repair)
            {   // Return all items not actively being repaired.
                foreach (DaggerfallUnityItem item in remoteItemsFiltered)
                {
                    if (!item.RepairData.IsBeingRepaired() || item.RepairData.IsRepairFinished())
                    {
                        localItems.Transfer(item, remoteItems);
                        item.RepairData.Collect();
                    }
                }
                // UpdateRepairTimes(false);
            }
            else
            {   // Return items to player inventory. 
                // Note: ignoring weight here, like classic. Priority is to not lose any items.
                if (UsingWagon)
                {
                    // Always clear transport items into player's inventory
                    for (int i = remoteItems.Count; i-- > 0;)
                    {
                        DaggerfallUnityItem item = remoteItems.GetItem(i);
                        if (item.ItemGroup == ItemGroups.Transportation)
                            TransferItem(item, remoteItems, PlayerEntity.Items);
                    }
                }
                localItems.TransferAll(remoteItems);
            }
        }

        protected override float GetCarriedWeight()
        {
            return PlayerEntity.CarriedWeight + basketItems.GetWeight();
        }

        protected override void UpdateLocalTargetIcon()
        {
            if (UsingWagon)
            {
                localTargetIconPanel.BackgroundTexture = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Wagon).texture;
                float weight = PlayerEntity.WagonWeight;
                localTargetIconLabel.Text = String.Format(weight % 1 == 0 ? "{0:F0} / {1}" : "{0:F2} / {1}", weight, ItemHelper.WagonKgLimit);
            }
            else
            {
                base.UpdateLocalTargetIcon();
            }
        }

        protected override void UpdateRemoteTargetIcon()
        {
            ImageData containerImage;
            switch (WindowMode)
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
            if (WindowMode == WindowModes.Buy && !UsingWagon && basketItems != null)
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
                            (WindowMode != WindowModes.Sell && WindowMode != WindowModes.SellMagic) ||
                            (WindowMode == WindowModes.Sell && itemTypesAccepted.Contains(item.ItemGroup)) ||
                            (WindowMode == WindowModes.SellMagic && item.IsEnchanted) ))
                    {
                        AddLocalItem(item);
                    }
                }
            }
        }

        protected override void FilterRemoteItems()
        {
            if (WindowMode == WindowModes.Repair)
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
                UpdateRepairTimes(false);
            }
            else
                base.FilterRemoteItems();
        }

        protected void SelectWagon(bool show)
        {
            if (wagonButton == null)
                return;

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
            UsingWagon = show;
            localItemListScroller.ResetScroll();
            // Caller must now use Refresh
            // Refresh(false);
        }

        protected override void LoadTextures()
        {
            base.LoadTextures();

            // Load special button texture.
            if (WindowMode == WindowModes.Sell || WindowMode == WindowModes.SellMagic) {
                actionButtonsTexture = ImageReader.GetTexture(sellButtonsTextureName);
            } else if (WindowMode == WindowModes.Buy) {
                actionButtonsTexture = ImageReader.GetTexture(buyButtonsTextureName);
            } else if (WindowMode == WindowModes.Repair) {
                actionButtonsTexture = ImageReader.GetTexture(repairButtonsTextureName);
            } else if (WindowMode == WindowModes.Identify) {
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
                switch (WindowMode)
                {
                    case WindowModes.Sell:
                    case WindowModes.SellMagic:
                        if (remoteItems != null)
                        {
                            // Are we trying to sell the non empty wagon?
                            if (item.ItemGroup == ItemGroups.Transportation && PlayerEntity.WagonItems.Count > 0)
                            {
                                DaggerfallUnityItem usedWagon = PlayerEntity.Items.GetItem(ItemGroups.Transportation, (int)Transportation.Small_cart);
                                if (usedWagon.Equals(item))
                                    return;
                            }
                            TransferItem(item, localItems, remoteItems);
                        }
                        break;

                    case WindowModes.Buy:
                        if (UsingWagon)     // Allows player to get & equip stuff from cart while purchasing.
                            TransferItem(item, localItems, PlayerEntity.Items, CanCarryAmount(item), equip: !item.IsAStack());
                        else                // Allows player to equip and unequip while purchasing.
                            EquipItem(item);
                        break;

                    case WindowModes.Repair:
                        // Check that item can be repaired, is damaged & transfer if so.
                        if (item.IsEnchanted && !DaggerfallUnity.Settings.AllowMagicRepairs)
                            DaggerfallUI.MessageBox(magicItemsCannotBeRepairedTextId);
                        else if ((item.currentCondition < item.maxCondition) && item.TemplateIndex != (int)Weapons.Arrow)
                        {
                            TransferItem(item, localItems, remoteItems);
                            // UpdateRepairTimes(false);
                        }
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
                if (WindowMode == WindowModes.Buy)
                    TransferItem(item, remoteItems, basketItems, CanCarryAmount(item), equip: !item.IsAStack());
                else if (WindowMode == WindowModes.Repair)
                {
                    if (item.RepairData.IsBeingRepaired() && !item.RepairData.IsRepairFinished())
                    {
                        itemBeingRepaired = item;
                        string strInterruptRepair = TextManager.Instance.GetText(textDatabase, "interruptRepair");
                        DaggerfallMessageBox confirmInterruptRepairBox = new DaggerfallMessageBox(uiManager, DaggerfallMessageBox.CommonMessageBoxButtons.YesNo, strInterruptRepair, this);
                        confirmInterruptRepairBox.OnButtonClick += ConfirmInterruptRepairBox_OnButtonClick;
                        confirmInterruptRepairBox.Show();
                    }
                    else
                        TakeItemFromRepair(item);
                }
                else
                    TransferItem(item, remoteItems, localItems, UsingWagon ? WagonCanHoldAmount(item) : CanCarryAmount(item), blockTransport: UsingWagon);
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
            TransferItem(item, remoteItems, localItems, UsingWagon ? WagonCanHoldAmount(item) : CanCarryAmount(item));
            item.RepairData.Collect();
            // UpdateRepairTimes(false);
        }

        #endregion

        #region Action Button Event Handlers

        private void WagonButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            if (PlayerEntity.Items.Contains(ItemGroups.Transportation, (int) Transportation.Small_cart))
            {
                SelectWagon(!UsingWagon);
                Refresh(false);
            }
        }

        private void InfoButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            SelectActionMode(ActionModes.Info);
        }

        private void SelectButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            SelectActionMode(ActionModes.Select);
        }

        private void DoSteal()
        {
            if (WindowMode == WindowModes.Buy && cost > 0)
            {
                // Calculate the weight of all items picked from shelves, then get chance of shoplifting success.
                int weightAndNumItems = (int)basketItems.GetWeight() + basketItems.Count;
                int chanceBeingDetected = FormulaHelper.CalculateShopliftingChance(PlayerEntity, buildingDiscoveryData.quality, weightAndNumItems);
                PlayerEntity.TallySkill(DFCareer.Skills.Pickpocket, 1);

                if (Dice100.FailedRoll(chanceBeingDetected))
                {
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "stealSuccess"), 2);
                    RaiseOnTradeHandler(basketItems.GetNumItems(), 0);
                    PlayerEntity.Items.TransferAll(basketItems);
                    PlayerEntity.TallyCrimeGuildRequirements(true, 1);
                }
                else
                {   // Register crime and start spawning guards.
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "stealFailure"), 2);
                    RaiseOnTradeHandler(0, 0);
                    PlayerEntity.CrimeCommitted = PlayerEntity.Crimes.Theft;
                    PlayerEntity.SpawnCityGuards(true);
                }
                CloseWindow();
            }
        }

        private void StealButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DoSteal();
        }

        void StealButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isStealDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isStealDeferred)
            {
                isStealDeferred = false;
                DoSteal();
            }
        }

        private void DoModeAction()
        {
            if (usingIdentifySpell)
            {   // No trade when using a spell, just identify immediately
                for (int i = 0; i < remoteItems.Count; i++)
                    remoteItems.GetItem(i).IdentifyItem();
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "itemsIdentified"));
            }
            else
                ShowTradePopup();
        }

        private void ModeActionButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DoModeAction();
        }

        void ModeActionButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isModeActionDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isModeActionDeferred)
            {
                isModeActionDeferred = false;
                DoModeAction();
            }
        }

        private void ClearButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ClearSelectedItems();
            Refresh();
        }

        protected virtual void ConfirmTrade_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            bool receivedLetterOfCredit = false;
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                // Proceed with trade.
                int tradePrice = GetTradePrice();
                switch (WindowMode)
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
                            UpdateRepairTimes(true);
                        }
                        RaiseOnTradeHandler(remoteItems.GetNumItems(), tradePrice);
                        break;

                    case WindowModes.Identify:
                        PlayerEntity.DeductGoldAmount(tradePrice);
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

        protected virtual void ShowTradePopup()
        {
            const int tradeMessageBaseId = 260;
            const int notEnoughGoldId = 454;
            int msgOffset = 0;
            int tradePrice = GetTradePrice();

            if (WindowMode != WindowModes.Sell && WindowMode != WindowModes.SellMagic && PlayerEntity.GetGoldAmount() < tradePrice)
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
                if (WindowMode == WindowModes.Sell || WindowMode == WindowModes.SellMagic)
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
                OnTrade(WindowMode, numItems, value);
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
                if (parent.Guild != null)
                    return parent.Guild.GetTitle();
                else
                    return MacroHelper.GetFirstname(GameManager.Instance.PlayerEntity.Name);
            }
        }

        #endregion
    }
}
