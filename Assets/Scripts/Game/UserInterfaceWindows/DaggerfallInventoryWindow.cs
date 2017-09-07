// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: InconsolableCellist
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

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements inventory window.
    /// </summary>
    public class DaggerfallInventoryWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect weaponsAndArmorRect = new Rect(0, 0, 92, 10);
        Rect magicItemsRect = new Rect(93, 0, 69, 10);
        Rect clothingAndMiscRect = new Rect(163, 0, 91, 10);
        Rect ingredientsRect = new Rect(255, 0, 65, 10);

        Rect localItemsUpButtonRect = new Rect(163, 48, 9, 16);
        Rect localItemsDownButtonRect = new Rect(163, 184, 9, 16);
        Rect remoteItemsUpButtonRect = new Rect(261, 48, 9, 16);
        Rect remoteItemsDownButtonRect = new Rect(261, 184, 9, 16);

        Rect localItemsListPanelRect = new Rect(172, 48, 50, 152);
        Rect remoteItemsListPanelRect = new Rect(270, 48, 50, 152);
        Rect[] itemsButtonRects = new Rect[]
        {
            new Rect(0, 0, 50, 38),
            new Rect(0, 38, 50, 38),
            new Rect(0, 76, 50, 38),
            new Rect(0, 114, 50, 38)
        };

        Rect upArrowRect = new Rect(0, 0, 9, 16);
        Rect downArrowRect = new Rect(0, 136, 9, 16);

        Rect wagonButtonRect = new Rect(226, 14, 31, 14);
        Rect infoButtonRect = new Rect(226, 36, 31, 14);
        Rect equipButtonRect = new Rect(226, 58, 31, 14);
        Rect removeButtonRect = new Rect(226, 80, 31, 14);
        Rect useButtonRect = new Rect(226, 103, 31, 14);
        Rect goldButtonRect = new Rect(226, 126, 31, 14);

        //Rect localTargetIconRect = new Rect(164, 11, 57, 36);
        Rect remoteTargetIconRect = new Rect(262, 11, 57, 36);

        Rect exitButtonRect = new Rect(222, 178, 39, 22);

        #endregion

        #region UI Controls

        Button weaponsAndArmorButton;
        Button magicItemsButton;
        Button clothingAndMiscButton;
        Button ingredientsButton;

        Button wagonButton;
        Button infoButton;
        Button equipButton;
        Button removeButton;
        Button useButton;
        Button goldButton;

        Button localItemsUpButton;
        Button localItemsDownButton;
        Button remoteItemsUpButton;
        Button remoteItemsDownButton;
        VerticalScrollBar localItemsScrollBar;
        VerticalScrollBar remoteItemsScrollBar;

        Button[] localItemsButtons = new Button[listDisplayUnits];
        Panel[] localItemsIconPanels = new Panel[listDisplayUnits];
        TextLabel[] localItemsStackLabels = new TextLabel[listDisplayUnits];
        Button[] remoteItemsButtons = new Button[listDisplayUnits];
        Panel[] remoteItemsIconPanels = new Panel[listDisplayUnits];
        TextLabel[] remoteItemsStackLabels = new TextLabel[listDisplayUnits];

        Button[] accessoryButtons = new Button[accessoryCount];
        Panel[] accessoryIconPanels = new Panel[accessoryCount];

        PaperDoll paperDoll = new PaperDoll();
        //Panel localTargetIconPanel;
        Panel remoteTargetIconPanel;

        Color questItemBackgroundColor = new Color(0f, 0.25f, 0f, 0.5f);

        #endregion

        #region UI Textures

        Texture2D baseTexture;
        Texture2D goldTexture;

        Texture2D weaponsAndArmorNotSelected;
        Texture2D magicItemsNotSelected;
        Texture2D clothingAndMiscNotSelected;
        Texture2D ingredientsNotSelected;
        Texture2D weaponsAndArmorSelected;
        Texture2D magicItemsSelected;
        Texture2D clothingAndMiscSelected;
        Texture2D ingredientsSelected;

        Texture2D wagonNotSelected;
        Texture2D infoNotSelected;
        Texture2D equipNotSelected;
        Texture2D removeNotSelected;
        Texture2D useNotSelected;
        Texture2D wagonSelected;
        Texture2D infoSelected;
        Texture2D equipSelected;
        Texture2D removeSelected;
        Texture2D useSelected;

        Texture2D redUpArrow;
        Texture2D greenUpArrow;
        Texture2D redDownArrow;
        Texture2D greenDownArrow;

        #endregion

        #region Fields

        const string baseTextureName = "INVE00I0.IMG";
        const string goldTextureName = "INVE01I0.IMG";
        const string greenArrowsTextureName = "INVE06I0.IMG";           // Green up/down arrows when more items available
        const string redArrowsTextureName = "INVE07I0.IMG";             // Red up/down arrows when no more items available
        const int listDisplayUnits = 4;                                 // Number of items displayed in scrolling areas
        const int accessoryCount = 12;                                  // Number of accessory slots
        const int itemButtonMarginSize = 2;                             // Margin of item buttons
        const int accessoryButtonMarginSize = 1;                        // Margin of accessory buttons

        PlayerEntity playerEntity;

        TabPages selectedTabPage = TabPages.WeaponsAndArmor;
        ActionModes selectedActionMode = ActionModes.Equip;
        RemoteTargetTypes remoteTargetType = RemoteTargetTypes.Dropped;

        ItemCollection localItems = null;
        ItemCollection remoteItems = null;
        ItemCollection droppedItems = new ItemCollection();
        List<DaggerfallUnityItem> localItemsFiltered = new List<DaggerfallUnityItem>();
        List<DaggerfallUnityItem> remoteItemsFiltered = new List<DaggerfallUnityItem>();

        DaggerfallLoot lootTarget = null;
        bool usingWagon = false;

        ItemCollection lastRemoteItems = null;
        RemoteTargetTypes lastRemoteTargetType;

        int lastMouseOverPaperDollEquipIndex = -1;

        ItemCollection.AddPosition preferredOrder = ItemCollection.AddPosition.DontCare;

        #endregion

        #region Enums

        enum TabPages
        {
            WeaponsAndArmor,
            MagicItems,
            ClothingAndMisc,
            Ingredients,
        }

        enum RemoteTargetTypes
        {
            Dropped,
            Wagon,
            Loot,
        }

        enum ActionModes
        {
            Info,
            Equip,
            Remove,
            Use,
        }

        #endregion

        #region Properties

        public PlayerEntity PlayerEntity
        {
            get { return (playerEntity != null) ? playerEntity : playerEntity = GameManager.Instance.PlayerEntity; }
        }

        /// <summary>
        /// Gets or sets specific loot to view on next open.
        /// Otherwise will default to ground for dropping items.
        /// </summary>
        public DaggerfallLoot LootTarget
        {
            get { return lootTarget; }
            set { lootTarget = value; }
        }

        #endregion

        #region Constructors

        public DaggerfallInventoryWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
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
            NativePanel.Components.Add(paperDoll);
            paperDoll.Position = new Vector2(49, 13);
            paperDoll.OnMouseMove += PaperDoll_OnMouseMove;
            paperDoll.OnMouseClick += PaperDoll_OnMouseClick;
            paperDoll.ToolTip = defaultToolTip;
            paperDoll.Refresh();

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
            //localTargetIconPanel = DaggerfallUI.AddPanel(localTargetIconRect, NativePanel);
            remoteTargetIconPanel = DaggerfallUI.AddPanel(remoteTargetIconRect, NativePanel);

            // Setup initial state
            SelectTabPage(TabPages.WeaponsAndArmor);
            if (lootTarget != null)
                SelectActionMode(ActionModes.Remove);
            else
                SelectActionMode(ActionModes.Equip);

            // Setup initial display
            FilterLocalItems();
            FilterRemoteItems();
            UpdateLocalItemsDisplay();
            UpdateRemoteItemsDisplay();
            UpdateAccessoryItemsDisplay();
            UpdateRemoteTargetIcon();
        }

        void SetupTabPageButtons()
        {
            weaponsAndArmorButton = DaggerfallUI.AddButton(weaponsAndArmorRect, NativePanel);
            weaponsAndArmorButton.OnMouseClick += WeaponsAndArmor_OnMouseClick;

            magicItemsButton = DaggerfallUI.AddButton(magicItemsRect, NativePanel);
            magicItemsButton.OnMouseClick += MagicItems_OnMouseClick;

            clothingAndMiscButton = DaggerfallUI.AddButton(clothingAndMiscRect, NativePanel);
            clothingAndMiscButton.OnMouseClick += ClothingAndMisc_OnMouseClick;

            ingredientsButton = DaggerfallUI.AddButton(ingredientsRect, NativePanel);
            ingredientsButton.OnMouseClick += Ingredients_OnMouseClick;
        }

        void SetupActionButtons()
        {
            wagonButton = DaggerfallUI.AddButton(wagonButtonRect, NativePanel);
            wagonButton.OnMouseClick += WagonButton_OnMouseClick;

            infoButton = DaggerfallUI.AddButton(infoButtonRect, NativePanel);
            infoButton.OnMouseClick += InfoButton_OnMouseClick;

            equipButton = DaggerfallUI.AddButton(equipButtonRect, NativePanel);
            equipButton.OnMouseClick += EquipButton_OnMouseClick;

            removeButton = DaggerfallUI.AddButton(removeButtonRect, NativePanel);
            removeButton.OnMouseClick += RemoveButton_OnMouseClick;

            useButton = DaggerfallUI.AddButton(useButtonRect, NativePanel);
            useButton.OnMouseClick += UseButton_OnMouseClick;

            goldButton = DaggerfallUI.AddButton(goldButtonRect, NativePanel);
            goldButton.OnMouseClick += GoldButton_OnMouseClick;
            //goldButton.BackgroundColor = new Color(1, 0, 0, 0.5f);
        }

        void SetupScrollBars()
        {
            // Local items list scroll bar (e.g. items in character inventory)
            localItemsScrollBar = new VerticalScrollBar();
            localItemsScrollBar.Position = new Vector2(164, 66);
            localItemsScrollBar.Size = new Vector2(6, 117);
            localItemsScrollBar.DisplayUnits = listDisplayUnits;
            localItemsScrollBar.OnScroll += LocalItemsScrollBar_OnScroll;
            NativePanel.Components.Add(localItemsScrollBar);

            // Remote items list scroll bar (e.g. wagon, shop, loot pile, etc.)
            remoteItemsScrollBar = new VerticalScrollBar();
            remoteItemsScrollBar.Position = new Vector2(262, 66);
            remoteItemsScrollBar.Size = new Vector2(6, 117);
            remoteItemsScrollBar.DisplayUnits = listDisplayUnits;
            remoteItemsScrollBar.OnScroll += RemoteItemsScrollBar_OnScroll;
            NativePanel.Components.Add(remoteItemsScrollBar);
        }

        void SetupScrollButtons()
        {
            localItemsUpButton = DaggerfallUI.AddButton(localItemsUpButtonRect, NativePanel);
            localItemsUpButton.BackgroundTexture = redUpArrow;
            localItemsUpButton.OnMouseClick += LocalItemsUpButton_OnMouseClick;

            localItemsDownButton = DaggerfallUI.AddButton(localItemsDownButtonRect, NativePanel);
            localItemsDownButton.BackgroundTexture = redDownArrow;
            localItemsDownButton.OnMouseClick += LocalItemsDownButton_OnMouseClick;

            remoteItemsUpButton = DaggerfallUI.AddButton(remoteItemsUpButtonRect, NativePanel);
            remoteItemsUpButton.BackgroundTexture = redUpArrow;
            remoteItemsUpButton.OnMouseClick += RemoteItemsUpButton_OnMouseClick;

            remoteItemsDownButton = DaggerfallUI.AddButton(remoteItemsDownButtonRect, NativePanel);
            remoteItemsDownButton.BackgroundTexture = redDownArrow;
            remoteItemsDownButton.OnMouseClick += RemoteItemsDownButton_OnMouseClick;
        }

        void SetupLocalItemsElements()
        {
            // List panel for scrolling behaviour
            Panel localItemsListPanel = DaggerfallUI.AddPanel(localItemsListPanelRect, NativePanel);
            localItemsListPanel.OnMouseScrollUp += MyItemsListPanel_OnMouseScrollUp;
            localItemsListPanel.OnMouseScrollDown += MyItemsListPanel_OnMouseScrollDown;

            // Setup buttons
            for (int i = 0; i < listDisplayUnits; i++)
            {
                // Button
                localItemsButtons[i] = DaggerfallUI.AddButton(itemsButtonRects[i], localItemsListPanel);
                localItemsButtons[i].SetMargins(Margins.All, itemButtonMarginSize);
                localItemsButtons[i].ToolTip = defaultToolTip;
                localItemsButtons[i].Tag = i;
                localItemsButtons[i].OnMouseClick += LocalItemsButton_OnMouseClick;

                // Icon image panel
                localItemsIconPanels[i] = DaggerfallUI.AddPanel(localItemsButtons[i], AutoSizeModes.ScaleToFit);
                localItemsIconPanels[i].HorizontalAlignment = HorizontalAlignment.Center;
                localItemsIconPanels[i].VerticalAlignment = VerticalAlignment.Middle;
                localItemsIconPanels[i].MaxAutoScale = 1f;

                // Stack labels
                localItemsStackLabels[i] = DaggerfallUI.AddTextLabel(DaggerfallUI.Instance.Font4, Vector2.zero, string.Empty, localItemsButtons[i]);
                localItemsStackLabels[i].HorizontalAlignment = HorizontalAlignment.Right;
                localItemsStackLabels[i].VerticalAlignment = VerticalAlignment.Bottom;
                localItemsStackLabels[i].ShadowPosition = Vector2.zero;
                localItemsStackLabels[i].TextColor = DaggerfallUI.DaggerfallUnityDefaultToolTipTextColor;
            }
        }

        void SetupRemoteItemsElements()
        {
            // List panel for scrolling behaviour
            Panel remoteItemsListPanel = DaggerfallUI.AddPanel(remoteItemsListPanelRect, NativePanel);
            remoteItemsListPanel.OnMouseScrollUp += RemoteItemsListPanel_OnMouseScrollUp;
            remoteItemsListPanel.OnMouseScrollDown += RemoteItemsListPanel_OnMouseScrollDown;

            // Setup buttons
            for (int i = 0; i < listDisplayUnits; i++)
            {
                // Button
                remoteItemsButtons[i] = DaggerfallUI.AddButton(itemsButtonRects[i], remoteItemsListPanel);
                remoteItemsButtons[i].SetMargins(Margins.All, itemButtonMarginSize);
                remoteItemsButtons[i].ToolTip = defaultToolTip;
                remoteItemsButtons[i].Tag = i;
                remoteItemsButtons[i].OnMouseClick += RemoteItemsButton_OnMouseClick;

                // Icon image panel
                remoteItemsIconPanels[i] = DaggerfallUI.AddPanel(remoteItemsButtons[i], AutoSizeModes.ScaleToFit);
                remoteItemsIconPanels[i].HorizontalAlignment = HorizontalAlignment.Center;
                remoteItemsIconPanels[i].VerticalAlignment = VerticalAlignment.Middle;
                remoteItemsIconPanels[i].MaxAutoScale = 1f;

                // Stack labels
                remoteItemsStackLabels[i] = DaggerfallUI.AddTextLabel(DaggerfallUI.Instance.Font4, Vector2.zero, string.Empty, remoteItemsButtons[i]);
                remoteItemsStackLabels[i].HorizontalAlignment = HorizontalAlignment.Right;
                remoteItemsStackLabels[i].VerticalAlignment = VerticalAlignment.Bottom;
                remoteItemsStackLabels[i].ShadowPosition = Vector2.zero;
                remoteItemsStackLabels[i].TextColor = DaggerfallUI.DaggerfallUnityDefaultToolTipTextColor;
            }
        }

        void SetupAccessoryElements()
        {
            // Starting layout
            Vector2 col0Pos = new Vector2(1, 11);
            Vector2 col1Pos = new Vector2(24, 11);
            Vector2 buttonSize = new Vector2(21, 20);
            int rowOffset = 31;
            bool col0 = true;

            // Follow same order as equip slots
            int minSlot = (int)EquipSlots.Amulet0;
            int maxSlot = (int)EquipSlots.Crystal1;
            for (int i = minSlot; i <= maxSlot; i++)
            {
                // Current button rect
                Rect rect;
                if (col0)
                    rect = new Rect(col0Pos.x, col0Pos.y, buttonSize.x, buttonSize.y);
                else
                    rect = new Rect(col1Pos.x, col1Pos.y, buttonSize.x, buttonSize.y);

                // Create item button
                Button button = DaggerfallUI.AddButton(rect, NativePanel);
                button.SetMargins(Margins.All, accessoryButtonMarginSize);
                button.ToolTip = defaultToolTip;
                button.Tag = i;
                button.OnMouseClick += AccessoryItemsButton_OnMouseClick;
                accessoryButtons[i] = button;

                // Create icon panel
                Panel panel = new Panel();
                panel.AutoSize = AutoSizeModes.ScaleToFit;
                panel.MaxAutoScale = 1f;
                panel.HorizontalAlignment = HorizontalAlignment.Center;
                panel.VerticalAlignment = VerticalAlignment.Middle;
                button.Components.Add(panel);
                accessoryIconPanels[i] = panel;

                // Move to next column, then drop down a row at end of second column
                if (col0)
                {
                    col0 = !col0;
                }
                else
                {
                    col0 = !col0;
                    col0Pos.y += rowOffset;
                    col1Pos.y += rowOffset;
                }
            }
        }

        public override void OnPush()
        {
            // Local items always points to player inventory
            localItems = PlayerEntity.Items;

            // Start a new dropped items target
            droppedItems.Clear();
            remoteItems = droppedItems;
            remoteTargetType = RemoteTargetTypes.Dropped;

            // Use custom loot target if specified
            if (lootTarget != null)
            {
                remoteItems = lootTarget.Items;
                remoteTargetType = RemoteTargetTypes.Loot;
                lootTarget.OnInventoryOpen();
            }

            // Clear wagon button state on open
            if (wagonButton != null)
            {
                usingWagon = false;
                wagonButton.BackgroundTexture = wagonNotSelected;
            }

            // Set default button by context
            if (removeButton != null && lootTarget != null)
            {
                // When looting, make "remove" default action so player does not accidentially equip when picking up
                SelectActionMode(ActionModes.Remove);
            }
            else if (equipButton != null && lootTarget == null)
            {
                // When managing inventory only, make "equip" default action so player can manage gear
                SelectActionMode(ActionModes.Equip);
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
            // Clear any loot target on exit
            if (lootTarget != null)
            {
                // Remove loot container if empty
                if (lootTarget.Items.Count == 0)
                    GameObjectHelper.RemoveLootContainer(lootTarget);

                lootTarget.OnInventoryClose();
                lootTarget = null;
            }

            // Generate serializable loot pile in world for dropped items
            if (droppedItems.Count > 0)
            {
                DaggerfallLoot droppedLootContainer = GameObjectHelper.CreateDroppedLootContainer(GameManager.Instance.PlayerObject, DaggerfallUnity.NextUID);
                droppedLootContainer.Items.TransferAll(droppedItems);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh character portrait and inventory.
        /// Called every time inventory is pushed to top of stack.
        /// </summary>
        public void Refresh(bool refreshPaperDoll = true)
        {
            if (!IsSetup)
                return;

            // Refresh items display
            FilterLocalItems();
            FilterRemoteItems();
            UpdateLocalItemsDisplay();
            UpdateRemoteItemsDisplay();
            UpdateAccessoryItemsDisplay();

            // Refresh remote target icon
            UpdateRemoteTargetIcon();

            // Refresh paper doll
            if (refreshPaperDoll)
                paperDoll.Refresh();
        }

        #endregion

        #region Helper Methods

        // Clears all local list display elements
        void ClearLocalItemsElements()
        {
            for (int i = 0; i < listDisplayUnits; i++)
            {
                localItemsStackLabels[i].Text = string.Empty;
                localItemsButtons[i].ToolTipText = string.Empty;
                localItemsIconPanels[i].BackgroundTexture = null;
                localItemsButtons[i].BackgroundColor = Color.clear;
            }
            localItemsUpButton.BackgroundTexture = redUpArrow;
            localItemsDownButton.BackgroundTexture = redDownArrow;
        }

        // Clears all remote list display elements
        void ClearRemoteItemsElements()
        {
            for (int i = 0; i < listDisplayUnits; i++)
            {
                remoteItemsStackLabels[i].Text = string.Empty;
                remoteItemsButtons[i].ToolTipText = string.Empty;
                remoteItemsIconPanels[i].BackgroundTexture = null;
                remoteItemsButtons[i].BackgroundColor = Color.clear;
            }
            remoteItemsUpButton.BackgroundTexture = redUpArrow;
            remoteItemsDownButton.BackgroundTexture = redDownArrow;
        }

        // Updates red/green state of scroller buttons
        void UpdateListScrollerButtons(int index, int count, Button upButton, Button downButton)
        {
            // Update up button
            if (index > 0)
                upButton.BackgroundTexture = greenUpArrow;
            else
                upButton.BackgroundTexture = redUpArrow;

            // Update down button
            if (index < (count - listDisplayUnits))
                downButton.BackgroundTexture = greenDownArrow;
            else
                downButton.BackgroundTexture = redDownArrow;

            // No items above or below
            if (count <= listDisplayUnits)
            {
                upButton.BackgroundTexture = redUpArrow;
                downButton.BackgroundTexture = redDownArrow;
            }
        }

        // Gets inventory image
        ImageData GetInventoryImage(DaggerfallUnityItem item)
        {
            if (item.TemplateIndex == (int)Transportation.Small_cart)
            {
                // Handle small cart - the template image for this is not correct
                // Correct image actually in CIF files
                return DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Wagon);
            }
            else
            {
                // Get inventory image
                return DaggerfallUnity.ItemHelper.GetItemImage(item, true);
            }
        }

        void SelectTabPage(TabPages tabPage)
        {
            // Select new tab page
            selectedTabPage = tabPage;

            // Reset scrollbar
            localItemsScrollBar.Reset(listDisplayUnits);

            // Clear all button selections
            weaponsAndArmorButton.BackgroundTexture = weaponsAndArmorNotSelected;
            magicItemsButton.BackgroundTexture = magicItemsNotSelected;
            clothingAndMiscButton.BackgroundTexture = clothingAndMiscNotSelected;
            ingredientsButton.BackgroundTexture = ingredientsNotSelected;

            // Set new button selection texture background
            switch (tabPage)
            {
                case TabPages.WeaponsAndArmor:
                    weaponsAndArmorButton.BackgroundTexture = weaponsAndArmorSelected;
                    break;
                case TabPages.MagicItems:
                    magicItemsButton.BackgroundTexture = magicItemsSelected;
                    break;
                case TabPages.ClothingAndMisc:
                    clothingAndMiscButton.BackgroundTexture = clothingAndMiscSelected;
                    break;
                case TabPages.Ingredients:
                    ingredientsButton.BackgroundTexture = ingredientsSelected;
                    break;
            }

            // Update filtered list
            FilterLocalItems();
            UpdateLocalItemsDisplay();
        }

        void SelectActionMode(ActionModes mode)
        {
            selectedActionMode = mode;

            // Clear all button selections
            infoButton.BackgroundTexture = infoNotSelected;
            equipButton.BackgroundTexture = equipNotSelected;
            removeButton.BackgroundTexture = removeNotSelected;
            useButton.BackgroundTexture = useNotSelected;

            // Set button selected texture
            switch(mode)
            {
                case ActionModes.Info:
                    infoButton.BackgroundTexture = infoSelected;
                    break;
                case ActionModes.Equip:
                    equipButton.BackgroundTexture = equipSelected;
                    break;
                case ActionModes.Remove:
                    removeButton.BackgroundTexture = removeSelected;
                    break;
                case ActionModes.Use:
                    useButton.BackgroundTexture = useSelected;
                    break;
            }
        }

        void UpdateRemoteTargetIcon()
        {
            ImageData containerImage;
            switch (remoteTargetType)
            {
                default:
                case RemoteTargetTypes.Dropped:
                    containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Ground);
                    break;
                case RemoteTargetTypes.Wagon:
                    containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Wagon);
                    break;
                case RemoteTargetTypes.Loot:
                    containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(lootTarget.ContainerImage);
                    break;
            }

            remoteTargetIconPanel.BackgroundTexture = containerImage.texture;
        }

        //void SetLocalTarget(ItemTargets target)
        //{
        //    // Only player supported for now
        //    if (target == ItemTargets.Player)
        //    {
        //        localItems = PlayerEntity.Items;
        //    }
        //}

        //void SetRemoteTarget(ItemTargets target)
        //{
        //    //remoteTarget = target;

        //    // Clear selections
        //    wagonButton.BackgroundTexture = wagonNotSelected;

        //    // Only wagon and ground supported for now
        //    if (target == ItemTargets.Wagon)
        //    {
        //        // Show wagon icon
        //        ImageData containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(LootContainerImages.Wagon);
        //        remoteTargetIconPanel.BackgroundTexture = containerImage.texture;

        //        // Highlight wagon button
        //        wagonButton.BackgroundTexture = wagonSelected;

        //        // Set remote items
        //        remoteItems = playerEntity.WagonItems;
        //    }
        //    else if (target == ItemTargets.Ground)
        //    {
        //        // Show ground icon
        //        ImageData containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(LootContainerImages.Ground);
        //        remoteTargetIconPanel.BackgroundTexture = containerImage.texture;

        //        // TODO: Need to create new loot pile on drop containing items
        //        // For now just using an empty, volatile container for bootstrapping
        //        //remoteItems = new ItemCollection();
        //    }
        //}

        //void SetRemoteTarget(ItemCollection items, LootContainerImages containerIcon)
        //{
        //    // Must be setup
        //    if (!IsSetup)
        //        Setup();

        //    // Clear selections
        //    wagonButton.BackgroundTexture = wagonNotSelected;

        //    // Show icon
        //    ImageData containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(containerIcon);
        //    remoteTargetIconPanel.BackgroundTexture = containerImage.texture;

        //    // Set remote items
        //    remoteItems = items;
        //}

        /// <summary>
        /// Creates filtered list of local items based on view state.
        /// </summary>
        void FilterLocalItems()
        {
            // Clear current references
            localItemsFiltered.Clear();

            // Do nothing if no items
            if (localItems == null || localItems.Count == 0)
                return;

            // Add items to list
            for (int i = 0; i < localItems.Count; i++)
            {
                DaggerfallUnityItem item = localItems.GetItem(i);

                // Reject if equipped
                if (item.IsEquipped)
                    continue;

                bool isWeaponOrArmor = (item.ItemGroup == ItemGroups.Weapons || item.ItemGroup == ItemGroups.Armor);

                // Add based on view
                if (selectedTabPage == TabPages.WeaponsAndArmor)
                {
                    // Weapons and armor
                    if (isWeaponOrArmor && !item.IsEnchanted)
                        localItemsFiltered.Add(item);
                }
                else if (selectedTabPage == TabPages.MagicItems)
                {
                    // Enchanted items
                    if (item.IsEnchanted)
                        localItemsFiltered.Add(item);
                }
                else if (selectedTabPage == TabPages.Ingredients)
                {
                    // Ingredients
                    if (item.IsIngredient && !item.IsEnchanted)
                        localItemsFiltered.Add(item);
                }
                else if (selectedTabPage == TabPages.ClothingAndMisc)
                {
                    // Everything else
                    if (!isWeaponOrArmor && !item.IsEnchanted && !item.IsIngredient)
                        localItemsFiltered.Add(item);
                }
            }
        }

        /// <summary>
        /// Creates filtered list of remote items.
        /// For now this just creates a flat list, as that is Daggerfall's behaviour.
        /// </summary>
        void FilterRemoteItems()
        {
            // Clear current references
            remoteItemsFiltered.Clear();

            // Do nothing if no items
            if (remoteItems == null || remoteItems.Count == 0)
                return;

            // Add items to list
            for (int i = 0; i < remoteItems.Count; i++)
            {
                DaggerfallUnityItem item = remoteItems.GetItem(i);
                remoteItemsFiltered.Add(item);
            }
        }

        /// <summary>
        /// Updates local items display.
        /// </summary>
        void UpdateLocalItemsDisplay()
        {
            // Clear list elements
            ClearLocalItemsElements();
            if (localItemsFiltered == null)
                return;

            // Update scroller
            localItemsScrollBar.TotalUnits = localItemsFiltered.Count;
            int scrollIndex = GetSafeScrollIndex(localItemsScrollBar);

            // Update scroller buttons
            UpdateListScrollerButtons(scrollIndex, localItemsFiltered.Count, localItemsUpButton, localItemsDownButton);

            // Update images and tooltips
            for (int i = 0; i < listDisplayUnits; i++)
            {
                // Skip if out of bounds
                if (scrollIndex + i >= localItemsFiltered.Count)
                    continue;

                // Get item and image
                DaggerfallUnityItem item = localItemsFiltered[scrollIndex + i];
                ImageData image = GetInventoryImage(item);

                // TEST: Set green background for local quest items
                if (item.IsQuestItem)
                    localItemsButtons[i].BackgroundColor = questItemBackgroundColor;
                else
                    localItemsButtons[i].BackgroundColor = Color.clear;

                // Set image to button icon
                localItemsIconPanels[i].BackgroundTexture = image.texture;
                localItemsIconPanels[i].Size = new Vector2(image.texture.width, image.texture.height);

                // Set stack count
                if (item.stackCount > 1)
                    localItemsStackLabels[i].Text = item.stackCount.ToString();

                // Tooltip text
                string text;
                if (item.ItemGroup == ItemGroups.Books)
                {
                    text = DaggerfallUnity.Instance.ItemHelper.getBookNameByMessage(item.message, item.LongName);
                } else {
                    text = item.LongName;
                }
                localItemsButtons[i].ToolTipText = text;
            }
        }

        /// <summary>
        /// Updates remote items display.
        /// </summary>
        void UpdateRemoteItemsDisplay()
        {
            // Clear list elements
            ClearRemoteItemsElements();
            if (remoteItems == null)
                return;

            // Update scroller
            remoteItemsScrollBar.TotalUnits = remoteItemsFiltered.Count;
            int scrollIndex = GetSafeScrollIndex(remoteItemsScrollBar);

            // Update scroller buttons
            UpdateListScrollerButtons(scrollIndex, remoteItemsFiltered.Count, remoteItemsUpButton, remoteItemsDownButton);

            // Update images and tooltips
            for (int i = 0; i < listDisplayUnits; i++)
            {
                // Skip if out of bounds
                if (scrollIndex + i >= remoteItemsFiltered.Count)
                    continue;

                // Get item and image
                DaggerfallUnityItem item = remoteItemsFiltered[scrollIndex + i];
                ImageData image = GetInventoryImage(item);

                // TEST: Set green background for remote quest items
                if (item.IsQuestItem)
                    remoteItemsButtons[i].BackgroundColor = questItemBackgroundColor;
                else
                    remoteItemsButtons[i].BackgroundColor = Color.clear;

                // Set image to button icon
                remoteItemsIconPanels[i].BackgroundTexture = image.texture;
                remoteItemsIconPanels[i].Size = new Vector2(image.texture.width, image.texture.height);

                // Set stack count
                if (item.stackCount > 1)
                    remoteItemsStackLabels[i].Text = item.stackCount.ToString();

                // Tooltip text
                remoteItemsButtons[i].ToolTipText = item.LongName;
            }
        }

        /// <summary>
        /// Updates accessory items display.
        /// </summary>
        void UpdateAccessoryItemsDisplay()
        {
            // Follow same order as equip slots
            int minSlot = (int)EquipSlots.Amulet0;
            int maxSlot = (int)EquipSlots.Crystal1;
            for (int i = minSlot; i <= maxSlot; i++)
            {
                // Get button and panel for this slot
                Button button = accessoryButtons[i];
                Panel panel = accessoryIconPanels[i];
                if (button == null || panel == null)
                    return;

                // Get item at this equip index (if any)
                DaggerfallUnityItem item = playerEntity.ItemEquipTable.GetItem((EquipSlots)button.Tag);
                if (item == null)
                {
                    panel.BackgroundTexture = null;
                    button.ToolTipText = string.Empty;
                    continue;
                }

                // Update button and panel
                ImageData image = GetInventoryImage(item);
                panel.BackgroundTexture = image.texture;
                panel.Size = new Vector2(image.width, image.height);
                button.ToolTipText = item.LongName;
            }
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            // Load source textures
            baseTexture = ImageReader.GetTexture(baseTextureName);
            goldTexture = ImageReader.GetTexture(goldTextureName);

            // Cut out tab page not selected button textures
            weaponsAndArmorNotSelected = ImageReader.GetSubTexture(baseTexture, weaponsAndArmorRect);
            magicItemsNotSelected = ImageReader.GetSubTexture(baseTexture, magicItemsRect);
            clothingAndMiscNotSelected = ImageReader.GetSubTexture(baseTexture, clothingAndMiscRect);
            ingredientsNotSelected = ImageReader.GetSubTexture(baseTexture, ingredientsRect);

            // Cut out tab page selected button textures
            weaponsAndArmorSelected = ImageReader.GetSubTexture(goldTexture, weaponsAndArmorRect);
            magicItemsSelected = ImageReader.GetSubTexture(goldTexture, magicItemsRect);
            clothingAndMiscSelected = ImageReader.GetSubTexture(goldTexture, clothingAndMiscRect);
            ingredientsSelected = ImageReader.GetSubTexture(goldTexture, ingredientsRect);

            // Cut out red up/down arrows
            Texture2D redArrowsTexture = ImageReader.GetTexture(redArrowsTextureName);
            redUpArrow = ImageReader.GetSubTexture(redArrowsTexture, upArrowRect);
            redDownArrow = ImageReader.GetSubTexture(redArrowsTexture, downArrowRect);

            // Cut out green up/down arrows
            Texture2D greenArrowsTexture = ImageReader.GetTexture(greenArrowsTextureName);
            greenUpArrow = ImageReader.GetSubTexture(greenArrowsTexture, upArrowRect);
            greenDownArrow = ImageReader.GetSubTexture(greenArrowsTexture, downArrowRect);

            // Cut out action mode not selected buttons
            wagonNotSelected = ImageReader.GetSubTexture(baseTexture, wagonButtonRect);
            infoNotSelected = ImageReader.GetSubTexture(baseTexture, infoButtonRect);
            equipNotSelected = ImageReader.GetSubTexture(baseTexture, equipButtonRect);
            removeNotSelected = ImageReader.GetSubTexture(baseTexture, removeButtonRect);
            useNotSelected = ImageReader.GetSubTexture(baseTexture, useButtonRect);

            // Cut out action mode selected buttons
            wagonSelected = ImageReader.GetSubTexture(goldTexture, wagonButtonRect);
            infoSelected = ImageReader.GetSubTexture(goldTexture, infoButtonRect);
            equipSelected = ImageReader.GetSubTexture(goldTexture, equipButtonRect);
            removeSelected = ImageReader.GetSubTexture(goldTexture, removeButtonRect);
            useSelected = ImageReader.GetSubTexture(goldTexture, useButtonRect);
        }

        /// <summary>
        /// Gets safe scroll index.
        /// Scroller will be adjust to always be inside display range where possible.
        /// </summary>
        int GetSafeScrollIndex(VerticalScrollBar scroller)
        {
            // Get current scroller index
            int scrollIndex = scroller.ScrollIndex;
            if (scrollIndex < 0)
                scrollIndex = 0;

            // Ensure scroll index within current range
            if (scrollIndex + scroller.DisplayUnits > scroller.TotalUnits)
            {
                scrollIndex = scroller.TotalUnits - scroller.DisplayUnits;
                if (scrollIndex < 0) scrollIndex = 0;
                scroller.Reset(scroller.DisplayUnits, scroller.TotalUnits, scrollIndex);
            }

            return scrollIndex;
        }

        void ShowWagon(bool show)
        {
            if (show)
            {
                // Save current target and switch to wagon
                wagonButton.BackgroundTexture = wagonSelected;
                lastRemoteItems = remoteItems;
                lastRemoteTargetType = remoteTargetType;
                remoteItems = PlayerEntity.WagonItems;
                remoteTargetType = RemoteTargetTypes.Wagon;
            }
            else
            {
                // Restore previous target or default to dropped items
                wagonButton.BackgroundTexture = wagonNotSelected;
                if (lastRemoteItems != null)
                {
                    remoteItems = lastRemoteItems;
                    remoteTargetType = lastRemoteTargetType;
                    lastRemoteItems = null;
                }
                else
                {
                    remoteItems = droppedItems;
                    remoteTargetType = RemoteTargetTypes.Dropped;
                    lastRemoteItems = null;
                }
            }

            usingWagon = show;
            Refresh(false);
        }

        #endregion

        #region Tab Page Event Handlers

        private void WeaponsAndArmor_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectTabPage(TabPages.WeaponsAndArmor);
        }

        private void MagicItems_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectTabPage(TabPages.MagicItems);
        }

        private void ClothingAndMisc_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectTabPage(TabPages.ClothingAndMisc);
        }

        private void Ingredients_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectTabPage(TabPages.Ingredients);
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

        private void EquipButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectActionMode(ActionModes.Equip);
        }

        private void RemoveButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectActionMode(ActionModes.Remove);
        }

        private void UseButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectActionMode(ActionModes.Use);
        }

        private void GoldButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            const int goldToDropTextId = 25;

            // Get text tokens
            TextFile.Token[] textTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(goldToDropTextId);

            // Hack to set gold pieces in text token for now
            textTokens[0].text = textTokens[0].text.Replace("%gii", GameManager.Instance.PlayerEntity.GoldPieces.ToString());

            // Show message box
            DaggerfallInputMessageBox mb = new DaggerfallInputMessageBox(uiManager, this);
            mb.SetTextTokens(textTokens);
            mb.TextPanelDistanceY = 0;
            mb.InputDistanceX = 15;
            mb.InputDistanceY = -6;
            mb.TextBox.Numeric = true;
            mb.TextBox.MaxCharacters = 8;
            mb.TextBox.Text = "0";
            mb.OnGotUserInput += DropGoldPopup_OnGotUserInput;
            mb.Show();
        }

        private void DropGoldPopup_OnGotUserInput(DaggerfallInputMessageBox sender, string input)
        {
            // Get player gold count
            int playerGold = GameManager.Instance.PlayerEntity.GoldPieces;

            // Determine how many gold pieces to drop
            int goldToDrop = 0;
            bool result = int.TryParse(input, out goldToDrop);
            if (!result || goldToDrop < 1 || goldToDrop > playerGold)
                return;

            // Create new item for gold pieces and add to other container
            DaggerfallUnityItem goldPieces = ItemBuilder.CreateGoldPieces(goldToDrop);
            remoteItems.AddItem(goldPieces, preferredOrder);

            // Remove gold count from player
            GameManager.Instance.PlayerEntity.GoldPieces -= goldToDrop;

            Refresh(false);
        }

        #endregion

        #region ScrollBar Event Handlers

        private void LocalItemsScrollBar_OnScroll()
        {
            UpdateLocalItemsDisplay();
        }

        private void LocalItemsUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            localItemsScrollBar.ScrollIndex--;
        }

        private void LocalItemsDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            localItemsScrollBar.ScrollIndex++;
        }

        private void MyItemsListPanel_OnMouseScrollUp()
        {
            localItemsScrollBar.ScrollIndex--;
        }

        private void MyItemsListPanel_OnMouseScrollDown()
        {
            localItemsScrollBar.ScrollIndex++;
        }

        #endregion

        #region Remote Items List Events

        private void RemoteItemsScrollBar_OnScroll()
        {
            UpdateRemoteItemsDisplay();
        }

        private void RemoteItemsUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            remoteItemsScrollBar.ScrollIndex--;
        }

        private void RemoteItemsDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            remoteItemsScrollBar.ScrollIndex++;
        }

        private void RemoteItemsListPanel_OnMouseScrollUp()
        {
            remoteItemsScrollBar.ScrollIndex--;
        }

        private void RemoteItemsListPanel_OnMouseScrollDown()
        {
            remoteItemsScrollBar.ScrollIndex++;
        }

        #endregion

        #region Item Action Helpers

        void EquipItem(DaggerfallUnityItem item)
        {
            const int itemBrokenTextId = 29;

            if (item.currentCondition < 1)
            {
                TextFile.Token[] tokens = DaggerfallUnity.TextProvider.GetRSCTokens(itemBrokenTextId);
                if (tokens != null && tokens.Length > 0)
                {
                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                    messageBox.SetTextTokens(tokens);
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.Show();
                }
                return;
            }

            if (playerEntity.ItemEquipTable.EquipItem(item) && item.ItemGroup == ItemGroups.Armor)
            {
                playerEntity.UpdateEquippedArmorValues(item, true);
            }
            Refresh();
        }

        void UnequipItem(DaggerfallUnityItem item, bool refreshPaperDoll = true)
        {
            if (playerEntity.ItemEquipTable.UnequipItem(item.EquipSlot) && item.ItemGroup == ItemGroups.Armor)
            {
                playerEntity.UpdateEquippedArmorValues(item, false);
            }
            playerEntity.Items.ReorderItem(item, preferredOrder);
            Refresh(refreshPaperDoll);
        }

        void NextVariant(DaggerfallUnityItem item)
        {
            item.NextVariant();
            if (item.IsEquipped)
                paperDoll.Refresh();
            else
                Refresh(false);
        }

        void TransferItem(DaggerfallUnityItem item, ItemCollection from, ItemCollection to)
        {
            // Block transfer of horse or cart
            if (item.IsOfTemplate(ItemGroups.Transportation, (int)Transportation.Horse) ||
                item.IsOfTemplate(ItemGroups.Transportation, (int)Transportation.Small_cart))
            {
                return;
            }

            // When transferring gold to player simply add to player's gold count
            if (item.IsOfTemplate(ItemGroups.Currency, (int)Currency.Gold_pieces) && PlayerEntity.Items == to)
            {
                playerEntity.GoldPieces += item.stackCount;
                from.RemoveItem(item);
                Refresh(false);
                DaggerfallUI.Instance.PlayOneShot(SoundClips.GoldPieces);
                return;
            }

            to.Transfer(item, from, preferredOrder);
            Refresh(false);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        void ShowInfoPopup(DaggerfallUnityItem item)
        {
            const int paintingTextId = 250;
            const int armorTextId = 1000;
            const int weaponTextId = 1001;
            const int miscTextId = 1003;
            const int soulTrapTextId = 1004;
            const int letterOfCreditTextId = 1007;
            //const int potionTextId = 1008;
            const int bookTextId = 1009;
            const int arrowTextId = 1011;
            const int weaponNoMaterialTextId = 1012;
            const int armorNoMaterialTextId = 1014;
            const int oghmaInfiniumTextId = 1015;
            const int houseDeedTextId = 1073;

            TextFile.Token[] tokens = null;

            // Handle by item group
            switch (item.ItemGroup)
            {
                case (ItemGroups.Armor):
                    if (item.IsShield || item.TemplateIndex == (int)Armor.Helm || item.IsArtifact)
                        tokens = DaggerfallUnity.TextProvider.GetRSCTokens(armorNoMaterialTextId);
                    else
                        tokens = DaggerfallUnity.TextProvider.GetRSCTokens(armorTextId);
                    break;

                case (ItemGroups.Weapons):
                    if (item.TemplateIndex == (int)Weapons.Arrow)
                        tokens = DaggerfallUnity.TextProvider.GetRSCTokens(arrowTextId);
                    else if (item.IsArtifact)
                        tokens = DaggerfallUnity.TextProvider.GetRSCTokens(weaponNoMaterialTextId);
                    else
                        tokens = DaggerfallUnity.TextProvider.GetRSCTokens(weaponTextId);
                    break;

                case (ItemGroups.Books):
                    // Handle Oghma Infinium
                    if (item.legacyMagic != null && item.legacyMagic[0] == 26)
                    {
                        tokens = DaggerfallUnity.TextProvider.GetRSCTokens(oghmaInfiniumTextId);
                    }
                    // Handle other books
                    else
                    {
                        tokens = DaggerfallUnity.TextProvider.GetRSCTokens(bookTextId);
                    }
                    break;

                // TODO: Check for potion in glass bottle.
                // In classic, the check is whether RecordRoot.SublistHead is non-null
                // and of PotionMix type.

                case (ItemGroups.Paintings):
                    // TODO: Show painting. Uses file paint.dat.
                    tokens = DaggerfallUnity.TextProvider.GetRSCTokens(paintingTextId);
                    break;

                default:
                    // A few items in the MiscItems group have their own text display
                    if (item.ItemGroup == ItemGroups.MiscItems)
                    {
                        // Handle potion recipes
                        if (item.TemplateIndex == (int)MiscItems.Potion_recipe)
                        {
                            DaggerfallPotionRecipeWindow readerWindow = new DaggerfallPotionRecipeWindow(uiManager, item.typeDependentData, this);
                            uiManager.PushWindow(readerWindow);
                            return;
                        }
                        // Handle house deeds
                        else if (item.TemplateIndex == (int)MiscItems.House_Deed)
                        {
                            tokens = DaggerfallUnity.TextProvider.GetRSCTokens(houseDeedTextId);
                        }
                        // Handle soul traps
                        else if (item.TemplateIndex == (int)MiscItems.Soul_trap)
                        {
                            tokens = DaggerfallUnity.TextProvider.GetRSCTokens(soulTrapTextId);
                        }
                        else if (item.TemplateIndex == (int)MiscItems.Letter_of_credit)
                        {
                            tokens = DaggerfallUnity.TextProvider.GetRSCTokens(letterOfCreditTextId);
                        }
                        if (tokens != null)
                            break;
                    }

                    // Handle Azura's Star
                    if (item.legacyMagic != null && item.legacyMagic[0] == 26 && item.legacyMagic[1] == 9)
                    {
                        tokens = DaggerfallUnity.TextProvider.GetRSCTokens(soulTrapTextId);
                        break;
                    }

                    // Default fallback if none of the above applied
                    tokens = DaggerfallUnity.TextProvider.GetRSCTokens(miscTextId);
                    break;
            }

            if (tokens != null && tokens.Length > 0)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(tokens, item);
                if (item.legacyMagic == null)
                {
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.Show();
                }
                else
                {   // Setup the next message box with the magic effect info.
                    DaggerfallMessageBox messageBoxMagic = new DaggerfallMessageBox(uiManager, messageBox);
                    messageBoxMagic.SetTextTokens(1016, item);
                    messageBoxMagic.ClickAnywhereToClose = true;

                    messageBox.AddNextMessageBox(messageBoxMagic);
                    messageBox.Show();
                }
            }
        }

        // Moving local and remote Use item clicks to new method
        // This ensures the items are handled the same except when needed
        // This will need more work as more usable items are available
        void UseItem(DaggerfallUnityItem item)
        {
            const int noSpellsTextId = 12;

            // Handle quest items on use clicks
            if (item.IsQuestItem)
            {
                // Get the quest this item belongs to
                Quest quest = QuestMachine.Instance.GetQuest(item.QuestUID);
                if (quest == null)
                    throw new Exception("DaggerfallUnityItem references a quest that could not be found.");

                // Get the Item resource from quest
                Item questItem = quest.GetItem(item.QuestItemSymbol);

                // Check for an on use value
                if (questItem.UsedMessageID != 0)
                {
                    // Display the message popup
                    quest.ShowMessagePopup(questItem.UsedMessageID);
                }
            }

            // Handle local items
            if (item.ItemGroup == ItemGroups.Books)
            {
                // Unreadable parchment (the one with a note graphic) is actually in UselessItems2
                if (item.TemplateIndex == (int)Books.Book || item.TemplateIndex == (int)Books.Parchment)
                {
                    DaggerfallUI.Instance.BookReaderWindow.BookTarget = item;
                    DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenBookReaderWindow);
                }
                else if (item.TemplateIndex == (int)Books.Parchment)
                {
                    // TODO: implement note viewer? Or is parchment just blank paper? -IC112016
                }
            }
            else if (item.ItemGroup == ItemGroups.MiscItems && item.TemplateIndex == (int)MiscItems.Potion_recipe)
            {
                // TODO: There may be other objects that result in this dialog box, but for now I'm sure this one says it.
                // -IC122016
                DaggerfallMessageBox cannotUse = new DaggerfallMessageBox(uiManager, this);
                cannotUse.SetText(HardStrings.cannotUseThis);
                cannotUse.ClickAnywhereToClose = true;
                cannotUse.Show();
            }
            else if (item.TemplateIndex == (int)MiscItems.Spellbook)
            {
                TextFile.Token[] textTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(noSpellsTextId);
                DaggerfallMessageBox noSpells = new DaggerfallMessageBox(uiManager, this);
                noSpells.SetTextTokens(textTokens);
                noSpells.ClickAnywhereToClose = true;
                noSpells.Show();
            }
            else
            {
                NextVariant(item);
            }
        }

        #endregion

        #region Item Click Event Handlers

        // NOTE: Working through action processes here. Will clean up soon.

        private void AccessoryItemsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Get item
            EquipSlots slot = (EquipSlots)sender.Tag;
            DaggerfallUnityItem item = playerEntity.ItemEquipTable.GetItem(slot);
            if (item == null)
                return;

            // Handle click based on action
            if (selectedActionMode == ActionModes.Equip)
            {
                UnequipItem(item, false);
            }
            else if (selectedActionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        private void PaperDoll_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Get equip value
            byte value = paperDoll.GetEquipIndex((int)position.x, (int)position.y);
            if (value == 0xff)
                return;

            // Get item
            EquipSlots slot = (EquipSlots)value;
            DaggerfallUnityItem item = playerEntity.ItemEquipTable.GetItem(slot);
            if (item == null)
                return;

            // Handle click based on action
            if (selectedActionMode == ActionModes.Equip)
            {
                UnequipItem(item);
            }
            else if (selectedActionMode == ActionModes.Use)
            {
                NextVariant(item);
            }
            else if (selectedActionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        private void LocalItemsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Get index
            int index = localItemsScrollBar.ScrollIndex + (int)sender.Tag;
            if (index >= localItemsFiltered.Count)
                return;

            // Get item
            DaggerfallUnityItem item = localItemsFiltered[index];
            if (item == null)
                return;

            // Handle click based on action
            if (selectedActionMode == ActionModes.Equip)
            {
                EquipItem(item);
            }
            else if (selectedActionMode == ActionModes.Use)
            {
                UseItem(item);
            }
            else if (selectedActionMode == ActionModes.Remove)
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

        private void RemoteItemsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Get index
            int index = remoteItemsScrollBar.ScrollIndex + (int)sender.Tag;
            if (index >= remoteItemsFiltered.Count)
                return;

            // Get item
            DaggerfallUnityItem item = remoteItemsFiltered[index];
            if (item == null)
                return;

            // Send click to quest system
            if (item.IsQuestItem)
            {
                Quest quest = QuestMachine.Instance.GetQuest(item.QuestUID);
                if (quest != null)
                {
                    Item questItem = quest.GetItem(item.QuestItemSymbol);
                    if (quest != null)
                        questItem.SetPlayerClicked();
                }
            }

            // Handle click based on action
            if (selectedActionMode == ActionModes.Equip)
            {
                // Transfer to local items
                if (localItems != null)
                    TransferItem(item, remoteItems, localItems);

                EquipItem(item);
            }
            else if (selectedActionMode == ActionModes.Use)
            {
                UseItem(item);
            }
            else if (selectedActionMode == ActionModes.Remove)
            {
                TransferItem(item, remoteItems, localItems);
            }
            else if (selectedActionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion

        #region Other Event Handlers

        private void PaperDoll_OnMouseMove(int x, int y)
        {
            byte value = paperDoll.GetEquipIndex(x, y);
            if (value != 0xff)
            {
                // Only update when index changed
                if (value == lastMouseOverPaperDollEquipIndex)
                    return;
                else
                    lastMouseOverPaperDollEquipIndex = value;

                // Test index is inside range
                string text = string.Empty;
                if (value >= 0 && value < ItemEquipTable.EquipTableLength)
                {
                    DaggerfallUnityItem item = playerEntity.ItemEquipTable.EquipTable[value];
                    if (item != null)
                        text = item.LongName;
                }

                // Update tooltip text
                paperDoll.ToolTipText = text;
            }
            else
            {
                // Clear tooltip text
                paperDoll.ToolTipText = string.Empty;
                lastMouseOverPaperDollEquipIndex = value;
            }
        }

        private void StartGameBehaviour_OnNewGame()
        {
            // Reset certain elements on a new game
            if (IsSetup)
            {
                SelectActionMode(ActionModes.Equip);
                SelectTabPage(TabPages.WeaponsAndArmor);
                localItemsScrollBar.Reset(listDisplayUnits);
                remoteItemsScrollBar.Reset(listDisplayUnits);
            }
        }

        #endregion
    }
}