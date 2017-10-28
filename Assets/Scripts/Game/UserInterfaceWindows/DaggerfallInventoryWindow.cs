// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: InconsolableCellist, Hazelnut
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
    public class DaggerfallInventoryWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect weaponsAndArmorRect = new Rect(0, 0, 92, 10);
        Rect magicItemsRect = new Rect(93, 0, 69, 10);
        Rect clothingAndMiscRect = new Rect(163, 0, 91, 10);
        Rect ingredientsRect = new Rect(255, 0, 65, 10);

        Rect wagonButtonRect = new Rect(226, 14, 31, 14);
        Rect infoButtonRect = new Rect(226, 36, 31, 14);
        Rect equipButtonRect = new Rect(226, 58, 31, 14);
        Rect removeButtonRect = new Rect(226, 80, 31, 14);
        Rect useButtonRect = new Rect(226, 103, 31, 14);
        Rect goldButtonRect = new Rect(226, 126, 31, 14);

        Rect localTargetIconRect = new Rect(165, 12, 55, 34);
        Rect remoteTargetIconRect = new Rect(263, 12, 55, 34);

        Rect localItemListScrollerRect = new Rect(163, 48, 59, 152);
        Rect remoteItemListScrollerRect = new Rect(261, 48, 59, 152);

        Rect itemInfoPanelRect = new Rect(223, 145, 37, 32);
        Rect infoCutoutRect = new Rect(196, 68, 50, 37);

        protected Rect exitButtonRect = new Rect(222, 178, 39, 22);

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

        Button[] accessoryButtons = new Button[accessoryCount];
        Panel[] accessoryIconPanels = new Panel[accessoryCount];

        PaperDoll paperDoll = new PaperDoll();

        protected Panel localTargetIconPanel;
        protected TextLabel localTargetIconLabel;
        protected Panel remoteTargetIconPanel;
        protected TextLabel remoteTargetIconLabel;
        protected Panel itemInfoPanel;
        protected MultiFormatTextLabel itemInfoPanelLabel;

        protected ItemListScroller localItemListScroller;
        protected ItemListScroller remoteItemListScroller;

        #endregion

        #region UI Textures

        protected Texture2D baseTexture;
        protected Texture2D goldTexture;

        protected Texture2D weaponsAndArmorNotSelected;
        protected Texture2D magicItemsNotSelected;
        protected Texture2D clothingAndMiscNotSelected;
        protected Texture2D ingredientsNotSelected;
        protected Texture2D weaponsAndArmorSelected;
        protected Texture2D magicItemsSelected;
        protected Texture2D clothingAndMiscSelected;
        protected Texture2D ingredientsSelected;

        protected Texture2D wagonNotSelected;
        protected Texture2D infoNotSelected;
        protected Texture2D equipNotSelected;
        protected Texture2D removeNotSelected;
        protected Texture2D useNotSelected;
        protected Texture2D wagonSelected;
        protected Texture2D infoSelected;
        protected Texture2D equipSelected;
        protected Texture2D removeSelected;
        protected Texture2D useSelected;

        protected Texture2D infoTexture;

        ImageData magicAnimation;

        #endregion

        #region Fields

        const string baseTextureName = "INVE00I0.IMG";
        const string goldTextureName = "INVE01I0.IMG";
        const string infoTextureName = "ITEM00I0.IMG";
        const string magicAnimTextureName = "TEXTURE.434";

        const float magicAnimationDelay = 0.15f;

        const int accessoryCount = 12;                                  // Number of accessory slots
        const int accessoryButtonMarginSize = 1;                        // Margin of accessory buttons

        Color questItemBackgroundColor = new Color(0f, 0.25f, 0f, 0.5f);

        PlayerEntity playerEntity;

        TabPages selectedTabPage = TabPages.WeaponsAndArmor;
        protected ActionModes selectedActionMode = ActionModes.Equip;
        protected RemoteTargetTypes remoteTargetType = RemoteTargetTypes.Dropped;

        protected ItemCollection localItems = null;
        protected ItemCollection remoteItems = null;
        ItemCollection droppedItems = new ItemCollection();
        protected List<DaggerfallUnityItem> localItemsFiltered = new List<DaggerfallUnityItem>();
        protected List<DaggerfallUnityItem> remoteItemsFiltered = new List<DaggerfallUnityItem>();

        DaggerfallLoot lootTarget = null;
        bool usingWagon = false;
        bool allowDungeonWagonAccess = false;

        ItemCollection lastRemoteItems = null;
        RemoteTargetTypes lastRemoteTargetType;

        int lastMouseOverPaperDollEquipIndex = -1;

        ItemCollection.AddPosition preferredOrder = ItemCollection.AddPosition.DontCare;

        #endregion

        #region Enums

        protected enum TabPages
        {
            WeaponsAndArmor,
            MagicItems,
            ClothingAndMisc,
            Ingredients,
        }

        protected enum RemoteTargetTypes
        {
            Dropped,
            Wagon,
            Loot,
            Merchant,
        }

        protected enum ActionModes
        {
            Info,
            Equip,
            Remove,
            Use,
            Select,
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

        public void AllowDungeonWagonAccess()
        {
            allowDungeonWagonAccess = true;
        }

        #endregion

        #region Constructors

        public DaggerfallInventoryWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
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
            SetupPaperdoll();

            // Setup item info panel if configured
            if (DaggerfallUnity.Settings.EnableInventoryInfoPanel)
            {
                itemInfoPanel = DaggerfallUI.AddPanel(itemInfoPanelRect, NativePanel);
                SetupItemInfoPanel();
            }

            // Setup UI
            SetupTargetIconPanels();
            SetupTabPageButtons();
            SetupActionButtons();
            SetupItemListScrollers();
            SetupAccessoryElements();

            // Exit buttons
            Button exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            // Setup initial state
            SelectTabPage(TabPages.WeaponsAndArmor);
            if (lootTarget != null)
                SelectActionMode(ActionModes.Remove);
            else
                SelectActionMode(ActionModes.Equip);

            // Setup initial display
            FilterLocalItems();
            localItemListScroller.Items = localItemsFiltered;
            FilterRemoteItems();
            remoteItemListScroller.Items = remoteItemsFiltered;
            UpdateAccessoryItemsDisplay();
            UpdateLocalTargetIcon();
            UpdateRemoteTargetIcon();
        }

        protected void SetupItemListScrollers()
        {
            localItemListScroller = new ItemListScroller(defaultToolTip)
            {
                Position = new Vector2(localItemListScrollerRect.x, localItemListScrollerRect.y),
                Size = new Vector2(localItemListScrollerRect.width, localItemListScrollerRect.height),
                BackgroundColourHandler = ItemBackgroundColourHandler,
                ForegroundAnimationHandler = MagicItemForegroundAnimationHander,
                ForegroundAnimationDelay = magicAnimationDelay
            };
            NativePanel.Components.Add(localItemListScroller);
            localItemListScroller.OnItemClick += LocalItemListScroller_OnItemClick;
            if (itemInfoPanelLabel != null)
                localItemListScroller.OnItemHover += ItemListScroller_OnHover;

            remoteItemListScroller = new ItemListScroller(defaultToolTip)
            {
                Position = new Vector2(remoteItemListScrollerRect.x, remoteItemListScrollerRect.y),
                Size = new Vector2(remoteItemListScrollerRect.width, remoteItemListScrollerRect.height),
                BackgroundColourHandler = ItemBackgroundColourHandler,
                ForegroundAnimationHandler = MagicItemForegroundAnimationHander,
                ForegroundAnimationDelay = magicAnimationDelay
            };
            NativePanel.Components.Add(remoteItemListScroller);
            remoteItemListScroller.OnItemClick += RemoteItemListScroller_OnItemClick;
            if (itemInfoPanelLabel != null)
                remoteItemListScroller.OnItemHover += ItemListScroller_OnHover;
        }

        protected virtual Color ItemBackgroundColourHandler(DaggerfallUnityItem item)
        {
            // TEST: Set green background for remote quest items
            if (item.IsQuestItem)
                return questItemBackgroundColor;
            else
                return Color.clear;
        }

        Texture2D[] MagicItemForegroundAnimationHander(DaggerfallUnityItem item)
        {
            return (item.IsEnchanted) ? magicAnimation.animatedTextures : null;
        }

        protected void SetupTargetIconPanels()
        {
            // Setup local and remote target icon panels
            localTargetIconPanel = DaggerfallUI.AddPanel(localTargetIconRect, NativePanel);
            localTargetIconPanel.BackgroundTextureLayout = BackgroundLayout.ScaleToFit;
            localTargetIconLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(1, 2), localTargetIconPanel);
            localTargetIconLabel.TextColor = DaggerfallUI.DaggerfallUnityDefaultToolTipTextColor;

            remoteTargetIconPanel = DaggerfallUI.AddPanel(remoteTargetIconRect, NativePanel);
            remoteTargetIconPanel.BackgroundTextureLayout = BackgroundLayout.ScaleToFit;
            remoteTargetIconLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(1, 2), remoteTargetIconPanel);
            remoteTargetIconLabel.TextColor = DaggerfallUI.DaggerfallUnityDefaultToolTipTextColor;
        }

        protected void SetupItemInfoPanel()
        {
            itemInfoPanelLabel = new MultiFormatTextLabel
            {
                Position = new Vector2(2, 0),
                VerticalAlignment = VerticalAlignment.Middle,
                MinTextureDimTextLabel = 16, // important to prevent scaling issues for single text lines
                TextScale = 0.43f,
                MaxTextWidth = 37,
                WrapText = true,
                WrapWords = true,
                ExtraLeading = 3, // spacing between info panel elements
                TextColor = new Color32(250, 250, 220, 255),
                ShadowPosition = new Vector2(0.5f, 0.5f),
                ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1
            };
            itemInfoPanel.BackgroundTexture = infoTexture;
            itemInfoPanel.Components.Add(itemInfoPanelLabel);
        }

        protected void SetupPaperdoll()
        {
            NativePanel.Components.Add(paperDoll);
            paperDoll.Position = new Vector2(49, 13);
            paperDoll.OnMouseMove += PaperDoll_OnMouseMove;
            paperDoll.OnMouseClick += PaperDoll_OnMouseClick;
            paperDoll.ToolTip = defaultToolTip;
            paperDoll.Refresh();
        }

        protected void SetupTabPageButtons()
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

        protected virtual void SetupActionButtons()
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
        }

        protected void SetupAccessoryElements()
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
                button.AnimationDelayInSeconds = magicAnimationDelay;
                button.OnMouseClick += AccessoryItemsButton_OnMouseClick;
                if (itemInfoPanelLabel != null)
                    button.OnMouseEnter += AccessoryItemsButton_OnMouseEnter;
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
            if (IsSetup)
            {
                // Start with wagon if accessing from dungeon
                if (allowDungeonWagonAccess) {
                    ShowWagon(true);
                    SelectActionMode(ActionModes.Remove);
                }
                // Reset item list scroll
                localItemListScroller.ResetScroll();
                remoteItemListScroller.ResetScroll();
            }
            // Clear info panel
            if (itemInfoPanelLabel != null)
                itemInfoPanelLabel.SetText(new TextFile.Token[0]);

            // Refresh window
            Refresh();
        }

        public override void OnPop()
        {
            // Reset dungeon wagon access permission
            allowDungeonWagonAccess = false;

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
        public virtual void Refresh(bool refreshPaperDoll = true)
        {
            if (!IsSetup)
                return;

            // Refresh items display
            FilterLocalItems();
            localItemListScroller.Items = localItemsFiltered;
            FilterRemoteItems();
            remoteItemListScroller.Items = remoteItemsFiltered;
            UpdateAccessoryItemsDisplay();

            // Refresh remote target icon
            UpdateLocalTargetIcon();
            UpdateRemoteTargetIcon();

            // Refresh paper doll
            if (refreshPaperDoll)
                paperDoll.Refresh();
        }

        #endregion

        #region Helper Methods

        protected void SelectTabPage(TabPages tabPage)
        {
            // Select new tab page
            selectedTabPage = tabPage;

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
            // Clear info panel
            if (itemInfoPanelLabel != null)
                itemInfoPanelLabel.SetText(new TextFile.Token[0]);

            // Update filtered list
            localItemListScroller.ResetScroll();
            FilterLocalItems();
            localItemListScroller.Items = localItemsFiltered;
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
            switch (mode)
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

        protected virtual float GetCarriedWeight()
        {
            return playerEntity.CarriedWeight;
        }

        protected virtual void UpdateLocalTargetIcon()
        {
            // Never changes on inventory window.
            localTargetIconPanel.BackgroundTexture = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Backpack).texture;
            float weight = GetCarriedWeight();
            localTargetIconLabel.Text = String.Format(weight % 1 == 0 ? "{0:F0} / {1}" : "{0:F2} / {1}", weight, PlayerEntity.MaxEncumbrance);
        }

        protected virtual void UpdateRemoteTargetIcon()
        {
            ImageData containerImage;
            remoteTargetIconLabel.Text = String.Empty;
            switch (remoteTargetType)
            {
                default:
                case RemoteTargetTypes.Dropped:
                    containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Ground);
                    break;
                case RemoteTargetTypes.Wagon:
                    containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Wagon);
                    float weight = PlayerEntity.WagonWeight;
                    remoteTargetIconLabel.Text = String.Format(weight % 1 == 0 ? "{0:F0} / {1}" : "{0:F2} / {1}", weight, ItemHelper.wagonKgLimit);
                    break;
                case RemoteTargetTypes.Loot:
                    containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(lootTarget.ContainerImage);
                    break;
            }
            remoteTargetIconPanel.BackgroundTexture = containerImage.texture;
        }

        /// <summary>
        /// Creates filtered list of local items based on view state.
        /// </summary>
        protected virtual void FilterLocalItems()
        {
            // Clear current references
            localItemsFiltered.Clear();

            if (localItems != null)
            {
                // Add items to list
                for (int i = 0; i < localItems.Count; i++)
                {
                    DaggerfallUnityItem item = localItems.GetItem(i);
                    // Add if not equipped
                    if (!item.IsEquipped)
                        AddLocalItem(item);
                }
            }
        }

        protected void AddLocalItem(DaggerfallUnityItem item)
        {
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
                if (item.IsEnchanted || item.IsOfTemplate((int)MiscItems.Spellbook))
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
                if (!isWeaponOrArmor && !item.IsEnchanted && !item.IsIngredient && !item.IsOfTemplate((int)MiscItems.Spellbook))
                    localItemsFiltered.Add(item);
            }
        }

        /// <summary>
        /// Creates filtered list of remote items.
        /// For now this just creates a flat list, as that is Daggerfall's behaviour.
        /// </summary>
        protected void FilterRemoteItems()
        {
            // Clear current references
            remoteItemsFiltered.Clear();

            // Add items to list
            if (remoteItems != null)
                for (int i = 0; i < remoteItems.Count; i++)
                    remoteItemsFiltered.Add(remoteItems.GetItem(i));
        }

        /// <summary>
        /// Updates accessory items display.
        /// </summary>
        protected void UpdateAccessoryItemsDisplay()
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
                DaggerfallUnityItem item = PlayerEntity.ItemEquipTable.GetItem((EquipSlots)button.Tag);
                if (item == null)
                {
                    panel.BackgroundTexture = null;
                    button.ToolTipText = string.Empty;
                    button.AnimatedBackgroundTextures = null;
                    continue;
                }

                // Update button and panel
                ImageData image = DaggerfallUnity.Instance.ItemHelper.GetInventoryImage(item);
                panel.BackgroundTexture = image.texture;
                panel.Size = new Vector2(image.width, image.height);
                button.ToolTipText = item.LongName;
                if (item.IsEnchanted)
                    button.AnimatedBackgroundTextures = magicAnimation.animatedTextures;
            }
        }

        #endregion

        #region Private Methods

        protected virtual void LoadTextures()
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

            // Cut out info panel texture from item maker
            Texture2D infoBaseTexture = ImageReader.GetTexture(infoTextureName);
            infoTexture = ImageReader.GetSubTexture(infoBaseTexture, infoCutoutRect);

            // Load magic item animation textures
            magicAnimation = ImageReader.GetImageData(magicAnimTextureName, 5, 0, true, false, true);

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
            remoteItemListScroller.ResetScroll();
            Refresh(false);
        }

        void UpdateItemInfoPanel(DaggerfallUnityItem item)
        {
            // Display info in local target icon panel, replacing justification tokens
            TextFile.Token[] tokens = ItemHelper.GetItemInfo(item, DaggerfallUnity.TextProvider);
            MacroHelper.ExpandMacros(ref tokens, item);
            for (int tokenIdx = 0; tokenIdx < tokens.Length; tokenIdx++)
            {
                if (tokens[tokenIdx].formatting == TextFile.Formatting.JustifyCenter)
                    tokens[tokenIdx].formatting = TextFile.Formatting.NewLine;
                if (tokens[tokenIdx].text != null)
                    tokens[tokenIdx].text = tokens[tokenIdx].text.Replace("kilograms", "kg").Replace("points of damage", "damage").Replace("armor rating", "armor");
            }
            itemInfoPanelLabel.SetText(tokens);
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
            if (!GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon || allowDungeonWagonAccess)
                if (PlayerEntity.Items.Contains(ItemGroups.Transportation, (int) Transportation.Small_cart))
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
            // Show message box
            const int goldToDropTextId = 25;
            DaggerfallInputMessageBox mb = new DaggerfallInputMessageBox(uiManager, this);
            mb.SetTextTokens(goldToDropTextId);
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
            // Check wagon weight limit
            if (usingWagon && (remoteItems.GetWeight() + (goldToDrop / DaggerfallBankManager.gold1kg) > ItemHelper.wagonKgLimit))
                return;

            // Create new item for gold pieces and add to other container
            DaggerfallUnityItem goldPieces = ItemBuilder.CreateGoldPieces(goldToDrop);
            remoteItems.AddItem(goldPieces, preferredOrder);

            // Remove gold count from player
            GameManager.Instance.PlayerEntity.GoldPieces -= goldToDrop;

            Refresh(false);
        }

        #endregion

        #region Item Action Helpers

        protected void EquipItem(DaggerfallUnityItem item)
        {
            const int itemBrokenTextId = 29;
            const int forbiddenEquipmentTextId = 1068;

            if (item.ItemGroup == ItemGroups.Weapons && item.TemplateIndex == (int)Weapons.Arrow)
                return;

            if (item.currentCondition < 1)
            {
                TextFile.Token[] tokens = DaggerfallUnity.TextProvider.GetRSCTokens(itemBrokenTextId);
                if (tokens != null && tokens.Length > 0)
                {
                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                    messageBox.SetTextTokens(tokens, item);
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.Show();
                }
                return;
            }

            bool prohibited = false;

            if (item.ItemGroup == ItemGroups.Armor)
            {
                // Check for prohibited shield
                if (item.IsShield && ((1 << (item.TemplateIndex - (int)Armor.Buckler) & (int)playerEntity.Career.ForbiddenShields) != 0))
                    prohibited = true;

                // Check for prohibited armor type (leather, chain or plate)
                else if ((1 << (item.NativeMaterialValue >> 8) & (int)playerEntity.Career.ForbiddenArmors) != 0)
                    prohibited = true;

                // Check for prohibited material
                else if (((item.NativeMaterialValue >> 8) == 2)
                    && (1 << (item.NativeMaterialValue & 0xFF) & (int)playerEntity.Career.ForbiddenMaterials) != 0)
                    prohibited = true;
            }
            else if (item.ItemGroup == ItemGroups.Weapons)
            {
                // Check for prohibited weapon type
                if ((item.GetWeaponSkillUsed() & (int)playerEntity.Career.ForbiddenProficiencies) != 0)
                    prohibited = true;
                // Check for prohibited material
                else if ((1 << item.NativeMaterialValue & (int)playerEntity.Career.ForbiddenMaterials) != 0)
                    prohibited = true;
            }

            if (prohibited)
            {
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(forbiddenEquipmentTextId);
                if (tokens != null && tokens.Length > 0)
                {
                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                    messageBox.SetTextTokens(tokens);
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.Show();
                }
                return;
            }
            // Try to equip the item, and update armour values accordingly
            List<DaggerfallUnityItem> unequippedList = playerEntity.ItemEquipTable.EquipItem(item);
            if (unequippedList != null)
            {
                foreach (DaggerfallUnityItem unequippedItem in unequippedList) {
                    playerEntity.UpdateEquippedArmorValues(unequippedItem, false);
                }
                playerEntity.UpdateEquippedArmorValues(item, true);
            }
            Refresh();
        }

        protected void UnequipItem(DaggerfallUnityItem item, bool refreshPaperDoll = true)
        {
            if (playerEntity.ItemEquipTable.UnequipItem(item.EquipSlot) != null)
            {
                playerEntity.UpdateEquippedArmorValues(item, false);
            }
            playerEntity.Items.ReorderItem(item, preferredOrder);
            Refresh(refreshPaperDoll);
        }

        protected void NextVariant(DaggerfallUnityItem item)
        {
            item.NextVariant();
            if (item.IsEquipped)
                paperDoll.Refresh();
            else
                Refresh(false);
        }

        protected bool CanCarry(DaggerfallUnityItem item)
        {
            // Check weight limit
            if (GetCarriedWeight() + item.weightInKg > playerEntity.MaxEncumbrance)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetText(HardStrings.cannotCarryAnymore);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
                return false;
            }
            return true;
        }

        protected bool WagonCanHold(DaggerfallUnityItem item)
        {
            // Check cart weight limit
            if (remoteItems.GetWeight() + item.weightInKg > ItemHelper.wagonKgLimit)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetText(HardStrings.cannotHoldAnymore);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
                return false;
            }
            return true;
        }

        protected void TransferItem(DaggerfallUnityItem item, ItemCollection from, ItemCollection to)
        {
            // Block transfer of horse or cart
            if (item.IsOfTemplate(ItemGroups.Transportation, (int)Transportation.Horse) ||
                item.IsOfTemplate(ItemGroups.Transportation, (int)Transportation.Small_cart))
            {
                return;
            }

            // Handle quest item transfer
            if (item.IsQuestItem)
            {
                // Player cannot drop most quest items
                if (!item.AllowQuestItemRemoval && from == localItems)
                {
                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                    messageBox.SetText(HardStrings.cannotRemoveThisItem);
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.Show();
                    return;
                }

                // Get the quest this item belongs to
                Quest quest = QuestMachine.Instance.GetQuest(item.QuestUID);
                if (quest == null)
                    throw new Exception("DaggerfallUnityItem references a quest that could not be found.");

                // Get quest item
                Item questItem = quest.GetItem(item.QuestItemSymbol);
                if (questItem == null)
                    throw new Exception("DaggerfallUnityItem references a quest item that could not be found.");

                // Dropping or picking up quest item
                if (item.AllowQuestItemRemoval && from == localItems && remoteTargetType == RemoteTargetTypes.Dropped)
                    questItem.PlayerDropped = true;
                else if (item.AllowQuestItemRemoval && from == remoteItems && remoteTargetType == RemoteTargetTypes.Dropped)
                    questItem.PlayerDropped = false;
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

            // Always place quest item pickups to front of list
            // Otherwise use preferred order
            ItemCollection.AddPosition order = preferredOrder;
            if (item.IsQuestItem)
                order = ItemCollection.AddPosition.Front;

            to.Transfer(item, from, order);
            Refresh(false);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        protected void ShowInfoPopup(DaggerfallUnityItem item)
        {
            TextFile.Token[] tokens = ItemHelper.GetItemInfo(item, DaggerfallUnity.TextProvider);
            if (tokens != null && tokens.Length > 0)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(tokens, item);

                if (item.TemplateIndex == (int)MiscItems.Potion_recipe)
                {   // Setup the next message box with the potion recipe ingredients list.
                    DaggerfallMessageBox messageBoxRecipe = new DaggerfallMessageBox(uiManager, messageBox);
                    messageBoxRecipe.SetTextTokens(item.GetMacroDataSource().PotionRecipeIngredients(TextFile.Formatting.JustifyCenter));
                    messageBoxRecipe.ClickAnywhereToClose = true;
                    messageBox.AddNextMessageBox(messageBoxRecipe);
                    messageBox.Show();
                }
                else if (item.legacyMagic != null)
                {   // Setup the next message box with the magic effect info.
                    DaggerfallMessageBox messageBoxMagic = new DaggerfallMessageBox(uiManager, messageBox);
                    messageBoxMagic.SetTextTokens(1016, item);
                    messageBoxMagic.ClickAnywhereToClose = true;
                    messageBox.AddNextMessageBox(messageBoxMagic);
                    messageBox.Show();
                }
                else
                {
                    messageBox.ClickAnywhereToClose = true;
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

                // If item not already used, and is being watched, then pop back to HUD so quest system has first shot at it
                // On second pass the normal message popup will display instead
                if (!questItem.UseClicked && questItem.ActionWatching)
                {
                    questItem.UseClicked = true;
                    DaggerfallUI.Instance.PopToHUD();
                    return;
                }

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

        protected virtual void AccessoryItemsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Get item
            EquipSlots slot = (EquipSlots)sender.Tag;
            DaggerfallUnityItem item = playerEntity.ItemEquipTable.GetItem(slot);
            if (item == null)
                return;

            // Handle click based on action
            if (selectedActionMode == ActionModes.Equip ||
                selectedActionMode == ActionModes.Select)
            {
                UnequipItem(item, false);
            }
            else if (selectedActionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        protected virtual void PaperDoll_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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
            if (selectedActionMode == ActionModes.Equip ||
                selectedActionMode == ActionModes.Select)
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

        protected virtual void LocalItemListScroller_OnItemClick(DaggerfallUnityItem item)
        {
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
                    // Check wagon weight limit
                    if (!usingWagon || WagonCanHold(item))
                        TransferItem(item, localItems, remoteItems);
                }
            }
            else if (selectedActionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        protected virtual void RemoteItemListScroller_OnItemClick(DaggerfallUnityItem item)
        {
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
            if (selectedActionMode == ActionModes.Equip && CanCarry(item))
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
            else if (selectedActionMode == ActionModes.Remove && CanCarry(item))
            {
                TransferItem(item, remoteItems, localItems);
            }
            else if (selectedActionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        protected void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion

        #region Hover & StartGame Event Handlers

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

                    // Update the info panel if used
                    if (itemInfoPanelLabel != null)
                        UpdateItemInfoPanel(item);
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

        protected virtual void AccessoryItemsButton_OnMouseEnter(BaseScreenComponent sender)
        {
            // Get item
            EquipSlots slot = (EquipSlots)sender.Tag;
            DaggerfallUnityItem item = playerEntity.ItemEquipTable.GetItem(slot);
            if (item == null)
                return;
            UpdateItemInfoPanel(item);
        }

        protected virtual void ItemListScroller_OnHover(DaggerfallUnityItem item)
        {
            UpdateItemInfoPanel(item);
        }

        protected virtual void StartGameBehaviour_OnNewGame()
        {
            // Reset certain elements on a new game
            if (IsSetup)
            {
                SelectActionMode(ActionModes.Equip);
                SelectTabPage(TabPages.WeaponsAndArmor);
            }
        }

        #endregion
    }
}