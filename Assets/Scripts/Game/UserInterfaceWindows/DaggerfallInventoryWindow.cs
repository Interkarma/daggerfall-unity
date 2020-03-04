// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: InconsolableCellist, Hazelnut, Numidium
//
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Banking;
using System.Linq;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements inventory window.
    /// </summary>
    public class DaggerfallInventoryWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        protected Rect weaponsAndArmorRect = new Rect(0, 0, 92, 10);
        protected Rect magicItemsRect = new Rect(93, 0, 69, 10);
        protected Rect clothingAndMiscRect = new Rect(163, 0, 91, 10);
        protected Rect ingredientsRect = new Rect(255, 0, 65, 10);

        protected Rect wagonButtonRect = new Rect(226, 14, 31, 14);
        protected Rect infoButtonRect = new Rect(226, 36, 31, 14);
        protected Rect equipButtonRect = new Rect(226, 58, 31, 14);
        protected Rect removeButtonRect = new Rect(226, 80, 31, 14);
        protected Rect useButtonRect = new Rect(226, 103, 31, 14);
        protected Rect goldButtonRect = new Rect(226, 126, 31, 14);

        protected Rect localTargetIconRect = new Rect(165, 12, 55, 34);
        protected Rect remoteTargetIconRect = new Rect(263, 12, 55, 34);

        protected Rect localItemListScrollerRect = new Rect(163, 48, 59, 152);
        protected Rect remoteItemListScrollerRect = new Rect(261, 48, 59, 152);

        protected Rect itemInfoPanelRect = new Rect(223, 145, 37, 32);
        protected Rect infoCutoutRect = new Rect(196, 68, 50, 37);

        protected Rect exitButtonRect = new Rect(222, 178, 39, 22);

        #endregion

        #region UI Controls

        protected Button weaponsAndArmorButton;
        protected Button magicItemsButton;
        protected Button clothingAndMiscButton;
        protected Button ingredientsButton;

        protected Button wagonButton;
        protected Button infoButton;
        protected Button equipButton;
        protected Button removeButton;
        protected Button useButton;
        protected Button goldButton;

        protected Button[] accessoryButtons = new Button[accessoryCount];
        protected Panel[] accessoryIconPanels = new Panel[accessoryCount];

        protected PaperDoll paperDoll = new PaperDoll();

        protected Panel localTargetIconPanel;
        protected TextLabel localTargetIconLabel;
        protected Panel remoteTargetIconPanel;
        protected TextLabel remoteTargetIconLabel;
        protected Panel itemInfoPanel;
        protected MultiFormatTextLabel itemInfoPanelLabel;

        protected ItemListScroller localItemListScroller;
        protected ItemListScroller remoteItemListScroller;

        // Only used for setting equip delay
        protected DaggerfallUnityItem lastRightHandItem;
        protected DaggerfallUnityItem lastLeftHandItem;

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

        protected ImageData coinsAnimation;
        protected ImageData magicAnimation;

        #endregion

        #region Fields

        protected const string textDatabase = "DaggerfallUI";

        protected readonly string kgSrc = TextManager.Instance.GetText(textDatabase, "kgSrc");
        protected readonly string kgRep = TextManager.Instance.GetText(textDatabase, "kgRep");
        protected readonly string damSrc = TextManager.Instance.GetText(textDatabase, "damSrc");
        protected readonly string damRep = TextManager.Instance.GetText(textDatabase, "damRep");
        protected readonly string arSrc = TextManager.Instance.GetText(textDatabase, "arSrc");
        protected readonly string arRep = TextManager.Instance.GetText(textDatabase, "arRep");
        protected readonly string goldAmount = TextManager.Instance.GetText(textDatabase, "goldAmount");
        protected readonly string goldWeight = TextManager.Instance.GetText(textDatabase, "goldWeight");
        protected readonly string wagonFullGold = TextManager.Instance.GetText(textDatabase, "wagonFullGold");

        protected const string baseTextureName = "INVE00I0.IMG";
        protected const string goldTextureName = "INVE01I0.IMG";
        protected const string infoTextureName = "ITEM00I0.IMG";

        protected const string coinsAnimTextureName = "TEXTURE.434";
        protected const string magicAnimTextureName = "TEXTURE.434";

        protected const float coinsAnimationDelay = 0.08f;
        protected const float magicAnimationDelay = 0.15f;

        protected const int accessoryCount = 12;                                  // Number of accessory slots
        protected const int accessoryButtonMarginSize = 1;                        // Margin of accessory buttons

        protected Color questItemBackgroundColor = new Color(0f, 0.25f, 0f, 0.5f);
        protected Color lightSourceBackgroundColor = new Color(0.6f, 0.5f, 0f, 0.5f);
        protected Color summonedItemBackgroundColor = new Color(0.18f, 0.32f, 0.48f, 0.5f);

        protected PlayerEntity playerEntity;

        protected TabPages selectedTabPage = TabPages.WeaponsAndArmor;
        protected ActionModes selectedActionMode = ActionModes.Equip;
        protected RemoteTargetTypes remoteTargetType = RemoteTargetTypes.Dropped;

        protected ItemCollection localItems = null;
        protected ItemCollection remoteItems = null;
        protected ItemCollection theftBasket = null;
        protected ItemCollection droppedItems = new ItemCollection();
        protected List<DaggerfallUnityItem> localItemsFiltered = new List<DaggerfallUnityItem>();
        protected List<DaggerfallUnityItem> remoteItemsFiltered = new List<DaggerfallUnityItem>();

        protected DaggerfallLoot lootTarget = null;
        protected bool usingWagon = false;
        protected bool allowDungeonWagonAccess = false;
        protected bool chooseOne = false;
        protected Action<DaggerfallUnityItem> chooseOneCallback;
        protected bool shopShelfStealing = false;
        protected bool isPrivateProperty = false;
        protected int lootTargetStartCount = 0;

        private DaggerfallUnityItem stackItem;
        private ItemCollection stackFrom;
        private ItemCollection stackTo;
        private bool stackEquip;

        protected ItemCollection lastRemoteItems = null;
        protected RemoteTargetTypes lastRemoteTargetType;

        int lastMouseOverPaperDollEquipIndex = -1;
        int dropIconArchive;
        int dropIconTexture;

        ItemCollection.AddPosition preferredOrder = ItemCollection.AddPosition.DontCare;

        KeyCode toggleClosedBinding;
        private int maxAmount;

        bool suppressInventory = false;
        string suppressInventoryMessage = string.Empty;

        #endregion

        #region Enums

        public enum TabPages
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

        public ItemCollection TheftBasket
        {
            get { return theftBasket; }
        }

        public void AllowDungeonWagonAccess()
        {
            allowDungeonWagonAccess = true;
        }

        public void SetChooseOne(ItemCollection items, Action<DaggerfallUnityItem> callback)
        {
            chooseOne = true;
            remoteItems = items;
            chooseOneCallback = callback;
        }

        public void SetShopShelfStealing()
        {
            shopShelfStealing = true;
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
            exitButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.InventoryExit);

            // Setup initial state
            SelectTabPage(TabPages.WeaponsAndArmor);
            SelectActionMode((lootTarget != null) ? ActionModes.Remove : ActionModes.Equip);
            CheckWagonAccess();

            // Setup initial display
            FilterLocalItems();
            localItemListScroller.Items = localItemsFiltered;
            FilterRemoteItems();
            remoteItemListScroller.Items = remoteItemsFiltered;
            UpdateAccessoryItemsDisplay();
            UpdateLocalTargetIcon();
            UpdateRemoteTargetIcon();

            // Store toggle closed binding for this window
            toggleClosedBinding = InputManager.Instance.GetBinding(InputManager.Actions.Inventory);
        }

        public override void Update()
        {
            base.Update();

            if (!DaggerfallUI.Instance.HotkeySequenceProcessed)
            {
                // Toggle window closed with same hotkey used to open it
                if (Input.GetKeyUp(toggleClosedBinding))
                    CloseWindow();
            }

            // Close window immediately if inventory suppressed
            if (suppressInventory)
            {
                CloseWindow();
                if (!string.IsNullOrEmpty(suppressInventoryMessage))
                    DaggerfallUI.MessageBox(suppressInventoryMessage);
                return;
            }
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
            localItemListScroller.OnItemRightClick += LocalItemListScroller_OnItemRightClick;
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
            remoteItemListScroller.OnItemRightClick += RemoteItemListScroller_OnItemRightClick;
            SetRemoteItemsAnimation();

            if (itemInfoPanelLabel != null)
                remoteItemListScroller.OnItemHover += ItemListScroller_OnHover;
        }

        protected virtual Color ItemBackgroundColourHandler(DaggerfallUnityItem item)
        {
            // Set background for special items
            if (item.IsQuestItem)
                return questItemBackgroundColor;
            else if (playerEntity.LightSource == item)
                return lightSourceBackgroundColor;
            else if (item.IsSummoned)
                return summonedItemBackgroundColor;
            else
                return Color.clear;
        }

        Texture2D[] StealItemBackgroundAnimationHandler(DaggerfallUnityItem item)
        {
            return (remoteItems.Contains(item)) ? coinsAnimation.animatedTextures : null;
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

            remoteTargetIconPanel.OnMouseClick += RemoteTargetIconPanel_OnMouseClick;
            remoteTargetIconPanel.OnRightMouseClick += RemoteTargetIconPanel_OnRightMouseClick;
            remoteTargetIconPanel.OnMiddleMouseClick += RemoteTargetIconPanel_OnMiddleMouseClick;
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
            paperDoll.OnRightMouseClick += PaperDoll_OnRightMouseClick;
            paperDoll.ToolTip = defaultToolTip;
            paperDoll.Refresh();
        }

        protected void SetupTabPageButtons()
        {
            weaponsAndArmorButton = DaggerfallUI.AddButton(weaponsAndArmorRect, NativePanel);
            weaponsAndArmorButton.OnMouseClick += WeaponsAndArmor_OnMouseClick;
            weaponsAndArmorButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.InventoryWeapons);

            magicItemsButton = DaggerfallUI.AddButton(magicItemsRect, NativePanel);
            magicItemsButton.OnMouseClick += MagicItems_OnMouseClick;
            magicItemsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.InventoryMagic);

            clothingAndMiscButton = DaggerfallUI.AddButton(clothingAndMiscRect, NativePanel);
            clothingAndMiscButton.OnMouseClick += ClothingAndMisc_OnMouseClick;
            clothingAndMiscButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.InventoryClothing);

            ingredientsButton = DaggerfallUI.AddButton(ingredientsRect, NativePanel);
            ingredientsButton.OnMouseClick += Ingredients_OnMouseClick;
            ingredientsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.InventoryIngredients);
        }

        protected virtual void SetupActionButtons()
        {
            wagonButton = DaggerfallUI.AddButton(wagonButtonRect, NativePanel);
            wagonButton.OnMouseClick += WagonButton_OnMouseClick;
            wagonButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.InventoryWagon);

            infoButton = DaggerfallUI.AddButton(infoButtonRect, NativePanel);
            infoButton.OnMouseClick += InfoButton_OnMouseClick;
            infoButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.InventoryInfo);

            equipButton = DaggerfallUI.AddButton(equipButtonRect, NativePanel);
            equipButton.OnMouseClick += EquipButton_OnMouseClick;
            equipButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.InventoryEquip);

            removeButton = DaggerfallUI.AddButton(removeButtonRect, NativePanel);
            removeButton.OnMouseClick += RemoveButton_OnMouseClick;
            removeButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.InventoryRemove);

            useButton = DaggerfallUI.AddButton(useButtonRect, NativePanel);
            useButton.OnMouseClick += UseButton_OnMouseClick;
            useButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.InventoryUse);

            goldButton = DaggerfallUI.AddButton(goldButtonRect, NativePanel);
            goldButton.OnMouseClick += GoldButton_OnMouseClick;
            goldButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.InventoryGold);

            if (itemInfoPanel != null)
                goldButton.OnMouseEnter += GoldButton_OnMouseEnter;
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
            // Racial override can suppress inventory
            // We still setup and push window normally, actual suppression is done in Update()
            MagicAndEffects.MagicEffects.RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            if (racialOverride != null)
                suppressInventory = racialOverride.GetSuppressInventory(out suppressInventoryMessage);

            // Local items always points to player inventory
            localItems = PlayerEntity.Items;

            // Start a new dropped items target
            droppedItems.Clear();
            if (chooseOne)
            {
                remoteTargetType = RemoteTargetTypes.Merchant;
                dropIconArchive = DaggerfallLootDataTables.combatArchive;
                dropIconTexture = 11;
            }
            else
            {   // Set dropped items as default target
                remoteItems = droppedItems;
                remoteTargetType = RemoteTargetTypes.Dropped;
                dropIconArchive = DaggerfallLootDataTables.randomTreasureArchive;
                dropIconTexture = -1;
            }
            // Use custom loot target if specified
            if (lootTarget != null)
            {
                remoteItems = lootTarget.Items;
                isPrivateProperty = lootTarget.houseOwned;
                theftBasket = isPrivateProperty ? new ItemCollection() : null;
                remoteTargetType = RemoteTargetTypes.Loot;
                lootTargetStartCount = remoteItems.Count;
                lootTarget.OnInventoryOpen();
                if (lootTarget.playerOwned && lootTarget.TextureArchive > 0)
                {
                    dropIconArchive = lootTarget.TextureArchive;
                    int[] iconIdxs;
                    DaggerfallLootDataTables.dropIconIdxs.TryGetValue(dropIconArchive, out iconIdxs);
                    if (iconIdxs != null)
                    {
                        for (int i = 0; i < iconIdxs.Length; i++)
                        {
                            if (iconIdxs[i] == lootTarget.TextureRecord)
                            {
                                dropIconTexture = i;
                                break;
                            }
                        }
                    }
                }
                OnClose += OnCloseWindow;
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
                CheckWagonAccess();
                // Reset item list scroll
                localItemListScroller.ResetScroll();
                remoteItemListScroller.ResetScroll();
                SetRemoteItemsAnimation();
            }
            // Clear info panel
            if (itemInfoPanelLabel != null)
                itemInfoPanelLabel.SetText(new TextFile.Token[0]);

            // Update tracked weapons for setting equip delay
            SetEquipDelayTime(false);

            // Refresh window
            Refresh();
        }

        public override void OnPop()
        {
            // Reset dungeon wagon access permission
            allowDungeonWagonAccess = false;

            // Reset choose one mode
            chooseOne = false;

            // Handle stealing and reset shop shelf stealing mode
            if (shopShelfStealing && remoteItems.Count < lootTargetStartCount)
            {
                playerEntity.TallyCrimeGuildRequirements(true, 1);
                Debug.Log("Player crime detected: stealing from a shop!!");
            }
            shopShelfStealing = false;

            // If icon has changed move items to dropped list so this loot is removed and a new one created
            if (lootTarget != null && lootTarget.playerOwned && lootTarget.TextureArchive > 0 &&
                (lootTarget.TextureArchive != dropIconArchive || lootTarget.TextureRecord != DaggerfallLootDataTables.dropIconIdxs[dropIconArchive][dropIconTexture]))
            {
                droppedItems.TransferAll(lootTarget.Items);
            }

            // Generate serializable loot pile in world for dropped items
            if (droppedItems.Count > 0)
            {
                DaggerfallLoot droppedLootContainer;
                if (dropIconTexture > -1)
                    droppedLootContainer = GameObjectHelper.CreateDroppedLootContainer(
                        GameManager.Instance.PlayerObject,
                        DaggerfallUnity.NextUID,
                        dropIconArchive,
                        DaggerfallLootDataTables.dropIconIdxs[dropIconArchive][dropIconTexture]);
                else
                    droppedLootContainer = GameObjectHelper.CreateDroppedLootContainer(GameManager.Instance.PlayerObject, DaggerfallUnity.NextUID);

                droppedLootContainer.Items.TransferAll(droppedItems);
                if (lootTarget != null)
                {   // Move newly created loot container to original position in x & z coords.
                    Vector3 pos = new Vector3(lootTarget.transform.position.x, droppedLootContainer.transform.position.y, lootTarget.transform.position.z);
                    droppedLootContainer.transform.position = pos;
                }
            }

            // Clear any loot target on exit
            if (lootTarget != null)
            {
                // Remove loot container if empty
                if (lootTarget.Items.Count == 0)
                    GameObjectHelper.RemoveLootContainer(lootTarget);

                lootTarget.OnInventoryClose();
                lootTarget = null;
            }

            // Add equip delay if weapon was changed
            SetEquipDelayTime(true);

            // Show "equipping" message if a delay was added
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            if (GameManager.Instance.WeaponManager.EquipCountdownRightHand > 0)
            {
                DaggerfallUnityItem currentRightHandWeapon = player.ItemEquipTable.GetItem(EquipSlots.RightHand);
                if (currentRightHandWeapon != null)
                {
                    string message = TextManager.Instance.GetText(textDatabase, "equippingWeapon");
                    message = message.Replace("%s", currentRightHandWeapon.ItemTemplate.name);
                    DaggerfallUI.Instance.PopupMessage(message);
                }
            }

            if (GameManager.Instance.WeaponManager.EquipCountdownLeftHand > 0)
            {
                DaggerfallUnityItem currentLeftHandWeapon = player.ItemEquipTable.GetItem(EquipSlots.LeftHand);
                if (currentLeftHandWeapon != null)
                {
                    string message = TextManager.Instance.GetText(textDatabase, "equippingWeapon");
                    message = message.Replace("%s", currentLeftHandWeapon.ItemTemplate.name);
                    DaggerfallUI.Instance.PopupMessage(message);
                }
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

        protected void SetRemoteItemsAnimation()
        {
            // Add animation handler for shop shelf stealing
            if (shopShelfStealing)
            {
                remoteItemListScroller.BackgroundAnimationHandler = StealItemBackgroundAnimationHandler;
                remoteItemListScroller.BackgroundAnimationDelay = coinsAnimationDelay;
            }
            else
                remoteItemListScroller.BackgroundAnimationHandler = null;
        }

        protected void SelectTabPage(TabPages tabPage)
        {
            // Select new tab page
            selectedTabPage = tabPage;

            // Set all buttons to appropriate state
            weaponsAndArmorButton.BackgroundTexture = (tabPage == TabPages.WeaponsAndArmor) ? weaponsAndArmorSelected : weaponsAndArmorNotSelected;
            magicItemsButton.BackgroundTexture = (tabPage == TabPages.MagicItems) ? magicItemsSelected : magicItemsNotSelected;
            clothingAndMiscButton.BackgroundTexture = (tabPage == TabPages.ClothingAndMisc) ? clothingAndMiscSelected : clothingAndMiscNotSelected;
            ingredientsButton.BackgroundTexture = (tabPage == TabPages.Ingredients) ? ingredientsSelected : ingredientsNotSelected;

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
            if (remoteTargetType == RemoteTargetTypes.Wagon)
            {
                containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Wagon);
                float weight = PlayerEntity.WagonWeight;
                remoteTargetIconLabel.Text = String.Format(weight % 1 == 0 ? "{0:F0} / {1}" : "{0:F2} / {1}", weight, ItemHelper.WagonKgLimit);
            }
            else if (dropIconTexture > -1)
            {
                string filename = TextureFile.IndexToFileName(dropIconArchive);
                containerImage = ImageReader.GetImageData(filename, DaggerfallLootDataTables.dropIconIdxs[dropIconArchive][dropIconTexture], 0, true);
            }
            else if (lootTarget != null && lootTarget.TextureArchive > 0)
            {
                string filename = TextureFile.IndexToFileName(lootTarget.TextureArchive);
                containerImage = ImageReader.GetImageData(filename, lootTarget.TextureRecord, 0, true);
            }
            else
            {
                containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(
                        (remoteTargetType == RemoteTargetTypes.Loot) ? lootTarget.ContainerImage : InventoryContainerImages.Ground);
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
        protected virtual void FilterRemoteItems()
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
                button.AnimatedBackgroundTextures = (item.IsEnchanted) ? magicAnimation.animatedTextures : null;
            }
        }

        #endregion

        #region Private Methods

        protected virtual void LoadTextures()
        {
            // Load source textures
            baseTexture = ImageReader.GetTexture(baseTextureName);
            goldTexture = ImageReader.GetTexture(goldTextureName);
            DFSize baseSize = new DFSize(320, 200);

            // Cut out tab page not selected button textures
            weaponsAndArmorNotSelected = ImageReader.GetSubTexture(baseTexture, weaponsAndArmorRect, baseSize);
            magicItemsNotSelected = ImageReader.GetSubTexture(baseTexture, magicItemsRect, baseSize);
            clothingAndMiscNotSelected = ImageReader.GetSubTexture(baseTexture, clothingAndMiscRect, baseSize);
            ingredientsNotSelected = ImageReader.GetSubTexture(baseTexture, ingredientsRect, baseSize);

            // Cut out tab page selected button textures
            weaponsAndArmorSelected = ImageReader.GetSubTexture(goldTexture, weaponsAndArmorRect, baseSize);
            magicItemsSelected = ImageReader.GetSubTexture(goldTexture, magicItemsRect, baseSize);
            clothingAndMiscSelected = ImageReader.GetSubTexture(goldTexture, clothingAndMiscRect, baseSize);
            ingredientsSelected = ImageReader.GetSubTexture(goldTexture, ingredientsRect, baseSize);

            // Cut out action mode not selected buttons
            wagonNotSelected = ImageReader.GetSubTexture(baseTexture, wagonButtonRect, baseSize);
            infoNotSelected = ImageReader.GetSubTexture(baseTexture, infoButtonRect, baseSize);
            equipNotSelected = ImageReader.GetSubTexture(baseTexture, equipButtonRect, baseSize);
            removeNotSelected = ImageReader.GetSubTexture(baseTexture, removeButtonRect, baseSize);
            useNotSelected = ImageReader.GetSubTexture(baseTexture, useButtonRect, baseSize);

            // Cut out action mode selected buttons
            wagonSelected = ImageReader.GetSubTexture(goldTexture, wagonButtonRect, baseSize);
            infoSelected = ImageReader.GetSubTexture(goldTexture, infoButtonRect, baseSize);
            equipSelected = ImageReader.GetSubTexture(goldTexture, equipButtonRect, baseSize);
            removeSelected = ImageReader.GetSubTexture(goldTexture, removeButtonRect, baseSize);
            useSelected = ImageReader.GetSubTexture(goldTexture, useButtonRect, baseSize);

            // Cut out info panel texture from item maker
            Texture2D infoBaseTexture = ImageReader.GetTexture(infoTextureName);
            infoTexture = ImageReader.GetSubTexture(infoBaseTexture, infoCutoutRect, baseSize);

            // Load coins animation textures
            coinsAnimation = ImageReader.GetImageData(coinsAnimTextureName, 6, 0, true, false, true);

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

        private void CheckWagonAccess()
        {
            if (allowDungeonWagonAccess)
            {
                ShowWagon(true);
                SelectActionMode(ActionModes.Remove);
            }
            else if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon &&
                     PlayerEntity.Items.Contains(ItemGroups.Transportation, (int)Transportation.Small_cart) &&
                     DungeonWagonAccessProximityCheck())
            {
                allowDungeonWagonAccess = true;
                if (lootTarget == null)
                    ShowWagon(true);
            }
        }

        bool DungeonWagonAccessProximityCheck()
        {
            const float proximityWagonAccessDistance = 5f;

            // Get all static doors
            DaggerfallStaticDoors[] allDoors = GameObject.FindObjectsOfType<DaggerfallStaticDoors>();
            if (allDoors != null && allDoors.Length > 0)
            {
                Vector3 playerPos = GameManager.Instance.PlayerObject.transform.position;
                // Find closest door to player
                float closestDoorDistance = float.MaxValue;
                foreach (DaggerfallStaticDoors doors in allDoors)
                {
                    int doorIndex;
                    Vector3 doorPos;
                    if (doors.FindClosestDoorToPlayer(playerPos, -1, out doorPos, out doorIndex, DoorTypes.DungeonExit))
                    {
                        float distance = Vector3.Distance(playerPos, doorPos);
                        if (distance < closestDoorDistance)
                            closestDoorDistance = distance;
                    }
                }

                // Allow wagon access if close enough to any exit door
                if (closestDoorDistance < proximityWagonAccessDistance)
                    return true;
            }
            return false;
        }

        void UpdateItemInfoPanel(DaggerfallUnityItem item)
        {
            // Display info in local target icon panel, replacing justification tokens
            TextFile.Token[] tokens = ItemHelper.GetItemInfo(item, DaggerfallUnity.TextProvider);
            MacroHelper.ExpandMacros(ref tokens, item);

            // Only keep the title part for paintings
            if (item.ItemGroup == ItemGroups.Paintings)
                tokens = new TextFile.Token[] { new TextFile.Token() { formatting = TextFile.Formatting.Text, text = tokens[tokens.Length - 1].text.Trim() } };

            UpdateItemInfoPanel(tokens);
        }

        private void UpdateItemInfoPanel(TextFile.Token[] tokens)
        {
            for (int tokenIdx = 0; tokenIdx < tokens.Length; tokenIdx++)
            {
                if (tokens[tokenIdx].formatting == TextFile.Formatting.JustifyCenter)
                    tokens[tokenIdx].formatting = TextFile.Formatting.NewLine;
                if (tokens[tokenIdx].text != null)
                    tokens[tokenIdx].text = tokens[tokenIdx].text.Replace(kgSrc, kgRep).Replace(damSrc, damRep).Replace(arSrc, arRep);
            }
            itemInfoPanelLabel.SetText(tokens);
        }

        void SetEquipDelayTime(bool setTime)
        {
            int delayTimeRight = 0;
            int delayTimeLeft = 0;
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            DaggerfallUnityItem currentRightHandItem = player.ItemEquipTable.GetItem(EquipSlots.RightHand);
            DaggerfallUnityItem currentLeftHandItem = player.ItemEquipTable.GetItem(EquipSlots.LeftHand);

            if (setTime)
            {
                if (lastRightHandItem != currentRightHandItem)
                {
                    // Add delay for unequipping old item
                    if (lastRightHandItem != null)
                        delayTimeRight = WeaponManager.EquipDelayTimes[lastRightHandItem.GroupIndex];

                    // Add delay for equipping new item
                    if (currentRightHandItem != null)
                        delayTimeRight += WeaponManager.EquipDelayTimes[currentRightHandItem.GroupIndex];
                }
                if (lastLeftHandItem != currentLeftHandItem)
                {
                    // Add delay for unequipping old item
                    if (lastLeftHandItem != null)
                        delayTimeLeft = WeaponManager.EquipDelayTimes[lastLeftHandItem.GroupIndex];

                    // Add delay for equipping new item
                    if (currentLeftHandItem != null)
                        delayTimeLeft += WeaponManager.EquipDelayTimes[currentLeftHandItem.GroupIndex];
                }
            }
            else
            {
                lastRightHandItem = null;
                lastLeftHandItem = null;
                if (currentRightHandItem != null)
                {
                    lastRightHandItem = currentRightHandItem;
                }
                if (currentLeftHandItem != null)
                {
                    lastLeftHandItem = currentLeftHandItem;
                }
            }
            GameManager.Instance.WeaponManager.EquipCountdownRightHand += delayTimeRight;
            GameManager.Instance.WeaponManager.EquipCountdownLeftHand += delayTimeLeft;
        }

        #endregion

        #region Tab Page Event Handlers

        private void WeaponsAndArmor_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectTabPage(TabPages.WeaponsAndArmor);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void MagicItems_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectTabPage(TabPages.MagicItems);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void ClothingAndMisc_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectTabPage(TabPages.ClothingAndMisc);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void Ingredients_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectTabPage(TabPages.Ingredients);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        #endregion

        #region Action Button Event Handlers

        private void WagonButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!PlayerEntity.Items.Contains(ItemGroups.Transportation, (int)Transportation.Small_cart))
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "noWagon"));
            else if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon && !allowDungeonWagonAccess)
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "exitTooFar"));
            else
                ShowWagon(!usingWagon);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void InfoButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectActionMode(ActionModes.Info);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void EquipButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectActionMode(ActionModes.Equip);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void RemoveButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectActionMode(ActionModes.Remove);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void UseButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectActionMode(ActionModes.Use);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void GoldButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Show message box
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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
            if (usingWagon)
            {
                // Check wagon weight limit
                int wagonCanHold = ComputeCanHoldAmount(playerGold, DaggerfallBankManager.goldUnitWeightInKg, ItemHelper.WagonKgLimit - remoteItems.GetWeight());
                if (goldToDrop > wagonCanHold)
                {
                    goldToDrop = wagonCanHold;
                    DaggerfallUI.MessageBox(String.Format(wagonFullGold, wagonCanHold));
                }
            }

            // Create new item for gold pieces and add to other container
            DaggerfallUnityItem goldPieces = ItemBuilder.CreateGoldPieces(goldToDrop);
            remoteItems.AddItem(goldPieces, preferredOrder);

            // Remove gold count from player
            GameManager.Instance.PlayerEntity.GoldPieces -= goldToDrop;

            Refresh(false);
            UpdateItemInfoPanelGold();
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
                foreach (DaggerfallUnityItem unequippedItem in unequippedList)
                {
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

        protected int CanCarryAmount(DaggerfallUnityItem item)
        {
            // Check weight limit
            int canCarry = ComputeCanHoldAmount(item.stackCount, item.EffectiveUnitWeightInKg(), playerEntity.MaxEncumbrance - GetCarriedWeight());
            if (canCarry <= 0)
            {
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "cannotCarryAnymore"));
            }
            return canCarry;
        }

        protected int WagonCanHoldAmount(DaggerfallUnityItem item)
        {
            // Check cart weight limit
            int canCarry = ComputeCanHoldAmount(item.stackCount, item.EffectiveUnitWeightInKg(), ItemHelper.WagonKgLimit - remoteItems.GetWeight());
            if (canCarry <= 0)
            {
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "cannotHoldAnymore"));
            }
            return canCarry;
        }

        private int ComputeCanHoldAmount(int unitsAvailable, float unitWeightInKg, float capacityLeftInKg)
        {
            int canHold = unitsAvailable;
            if (unitWeightInKg > 0f)
                canHold = Math.Min(canHold, (int)(capacityLeftInKg / unitWeightInKg));
            return canHold;
        }

        protected void TransferItem(DaggerfallUnityItem item, ItemCollection from, ItemCollection to, int? maxAmount = null,
                                    bool blockTransport = false, bool equip = false, bool allowSplitting = true)
        {
            // Block transfer of horse or cart (don't allow putting either in wagon)
            if (blockTransport && item.ItemGroup == ItemGroups.Transportation)
                return;

            // Block transfer of summoned items
            if (item.IsSummoned)
            {
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "cannotRemoveItem"));
                return;
            }

            // Handle map items
            if (item.IsOfTemplate(ItemGroups.MiscItems, (int)MiscItems.Map))
            {
                RecordLocationFromMap(item);
                from.RemoveItem(item);
                Refresh(false);
                return;
            }

            // Handle quest item transfer
            if (item.IsQuestItem)
            {
                // Get quest item
                Item questItem = GetQuestItem(item);

                // Player cannot drop most quest items
                if (questItem == null || (!questItem.AllowDrop && from == localItems))
                {
                    DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "cannotRemoveItem"));
                    return;
                }

                // Dropping or picking up quest item
                if (questItem.AllowDrop && from == localItems && remoteTargetType != RemoteTargetTypes.Wagon)
                    questItem.PlayerDropped = true;
                else if (from == remoteItems)
                    questItem.PlayerDropped = false;
            }
            // Extinguish light sources when transferring out of player inventory
            if (item.IsLightSource && playerEntity.LightSource == item && from == localItems)
                playerEntity.LightSource = null;

            // Handle stacks & splitting if needed
            this.maxAmount = maxAmount ?? item.stackCount;
            if (this.maxAmount <= 0)
                return;

            bool splitRequired = maxAmount < item.stackCount;
            bool controlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (splitRequired || controlPressed)
            {
                if (allowSplitting && item.IsAStack())
                {
                    stackItem = item;
                    stackFrom = from;
                    stackTo = to;
                    stackEquip = equip;
                    string defaultValue = controlPressed ? "0" : this.maxAmount.ToString();

                    // Show message box
                    DaggerfallInputMessageBox mb = new DaggerfallInputMessageBox(uiManager, this);
                    mb.SetTextBoxLabel(String.Format(TextManager.Instance.GetText(textDatabase, "howManyItems"), this.maxAmount));
                    mb.TextPanelDistanceY = 0;
                    mb.InputDistanceX = 15;
                    mb.TextBox.Numeric = true;
                    mb.TextBox.MaxCharacters = 8;
                    mb.TextBox.Text = defaultValue;
                    mb.OnGotUserInput += SplitStackPopup_OnGotUserInput;
                    mb.Show();
                    return;
                }
                if (splitRequired)
                    return;
            }

            DoTransferItem(item, from, to, equip);
        }

        private void SplitStackPopup_OnGotUserInput(DaggerfallInputMessageBox sender, string input)
        {
            // Determine how many items to split
            int count = 0;
            bool result = int.TryParse(input, out count);
            if (!result || count > maxAmount)
                return;

            DaggerfallUnityItem item = stackFrom.SplitStack(stackItem, count);
            if (item != null)
                DoTransferItem(item, stackFrom, stackTo, stackEquip);
            else
                Refresh(false);
        }

        protected void DoTransferItem(DaggerfallUnityItem item, ItemCollection from, ItemCollection to, bool equip)
        {
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
            if (equip)
                EquipItem(item);
            Refresh(false);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);

            if (chooseOne && remoteItems == from && !usingWagon)
            {
                while (uiManager.TopWindow != this)
                    uiManager.PopWindow();
                CloseWindow();
                chooseOneCallback(item);
            }
        }

        protected void ShowInfoPopup(DaggerfallUnityItem item)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            TextFile.Token[] tokens = ItemHelper.GetItemInfo(item, DaggerfallUnity.TextProvider);
            if (tokens != null && tokens.Length > 0)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(tokens, item);

                if (item.IsPotionRecipe)
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
                else if (item.ItemGroup == ItemGroups.Paintings)
                {   // Setup the message box with the painting image generated by macro handlers
                    ImageData paintingImg = ImageReader.GetImageData(item.GetPaintingFilename(), item.GetPaintingFileIdx(), 0, true, true);
                    messageBox.ImagePanel.VerticalAlignment = VerticalAlignment.None;
                    messageBox.ImagePanel.Position = new Vector2(0, 5);
                    messageBox.ImagePanel.Size = new Vector2(paintingImg.width, paintingImg.height);
                    messageBox.ImagePanel.BackgroundTexture = paintingImg.texture;
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.Show();
                }
                else
                {
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.Show();
                }
            }
        }

        /// <summary>
        /// Gets virtual quest item from DaggerfallUnityItem.
        /// </summary>
        /// <param name="item">Source DaggerfallUnityItem.</param>
        /// <returns>Quest Item if found.</returns>
        Item GetQuestItem(DaggerfallUnityItem item)
        {
            if (item == null)
                return null;

            Quest quest = QuestMachine.Instance.GetQuest(item.QuestUID);
            if (quest == null)
                throw new Exception("DaggerfallUnityItem references a QuestUID that could not be found.");

            Item questItem = quest.GetItem(item.QuestItemSymbol);
            if (questItem == null)
                throw new Exception("DaggerfallUnityItem references a QuestItemSymbol that could not be found.");

            return questItem;
        }

        // Moving local and remote Use item clicks to new method
        // This ensures the items are handled the same except when needed
        // This will need more work as more usable items are available
        protected void UseItem(DaggerfallUnityItem item, ItemCollection collection = null)
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

                // Use quest item
                if (!questItem.UseClicked && questItem.ActionWatching)
                {
                    questItem.UseClicked = true;

                    // Non-parchment items pop back to HUD so quest system has first shot at a custom click action in game world
                    // This is usually the case when actioning most quest items (e.g. a painting, bell, holy item, etc.)
                    // But when clicking a parchment item this behaviour is usually incorrect (e.g. a letter to read)
                    if (!questItem.DaggerfallUnityItem.IsParchment)
                    {
                        DaggerfallUI.Instance.PopToHUD();
                        return;
                    }
                }

                // Check for an on use value
                if (questItem.UsedMessageID != 0)
                {
                    // Display the message popup
                    quest.ShowMessagePopup(questItem.UsedMessageID, true);
                }
            }

            // Try to handle use with a registered delegate
            ItemHelper.ItemUseHander itemUseHander;
            if (DaggerfallUnity.Instance.ItemHelper.GetItemUseHander(item.TemplateIndex, out itemUseHander))
            {
                if (itemUseHander(item, collection))
                    return;
            }

            // Handle normal items
            if (item.ItemGroup == ItemGroups.Books && !item.IsArtifact)
            {
                DaggerfallUI.Instance.BookReaderWindow.OpenBook(item);
                if (DaggerfallUI.Instance.BookReaderWindow.IsBookOpen)
                {
                    DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenBookReaderWindow);
                }
                else
                {
                    var messageBox = new DaggerfallMessageBox(uiManager, this);
                    messageBox.SetText(TextManager.Instance.GetText(textDatabase, "bookUnavailable"));
                    messageBox.ClickAnywhereToClose = true;
                    uiManager.PushWindow(messageBox);
                }
            }
            else if (item.IsPotion)
            {   // Handle drinking magic potions
                GameManager.Instance.PlayerEffectManager.DrinkPotion(item);
                collection.RemoveOne(item);
            }
            else if (item.IsPotionRecipe)
            {
                // TODO: There may be other objects that result in this dialog box, but for now I'm sure this one says it.
                // -IC122016
                DaggerfallMessageBox cannotUse = new DaggerfallMessageBox(uiManager, this);
                cannotUse.SetText(TextManager.Instance.GetText(textDatabase, "cannotUseThis"));
                cannotUse.ClickAnywhereToClose = true;
                cannotUse.Show();
            }
            else if ((item.IsOfTemplate(ItemGroups.MiscItems, (int)MiscItems.Map) ||
                      item.IsOfTemplate(ItemGroups.Maps, (int)Maps.Map)) && collection != null)
            {   // Handle map items
                RecordLocationFromMap(item);
                collection.RemoveItem(item);
                Refresh(false);
            }
            else if (item.TemplateIndex == (int)MiscItems.Spellbook)
            {
                if (playerEntity.SpellbookCount() == 0)
                {
                    // Player has no spells
                    TextFile.Token[] textTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(noSpellsTextId);
                    DaggerfallMessageBox noSpells = new DaggerfallMessageBox(uiManager, this);
                    noSpells.SetTextTokens(textTokens);
                    noSpells.ClickAnywhereToClose = true;
                    noSpells.Show();
                }
                else
                {
                    // Show spellbook
                    DaggerfallUI.UIManager.PostMessage(DaggerfallUIMessages.dfuiOpenSpellBookWindow);
                }
            }
            else if (item.ItemGroup == ItemGroups.Drugs && collection != null)
            {
                // Drug poison IDs are 136 through 139. Template indexes are 78 through 81, so add to that.
                Formulas.FormulaHelper.InflictPoison(playerEntity, (Poisons)item.TemplateIndex + 66, true);
                collection.RemoveItem(item);
            }
            else if (item.IsLightSource)
            {
                if (item.currentCondition > 0)
                {
                    if (playerEntity.LightSource == item)
                    {
                        DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "lightDouse"), false, item);
                        playerEntity.LightSource = null;
                    }
                    else
                    {
                        DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "lightLight"), false, item);
                        playerEntity.LightSource = item;
                    }
                }
                else
                    DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "lightEmpty"), false, item);
            }
            else if (item.ItemGroup == ItemGroups.UselessItems2 && item.TemplateIndex == (int)UselessItems2.Oil && collection != null)
            {
                DaggerfallUnityItem lantern = localItems.GetItem(ItemGroups.UselessItems2, (int)UselessItems2.Lantern);
                if (lantern != null && lantern.currentCondition <= lantern.maxCondition - item.currentCondition)
                {   // Re-fuel lantern with the oil.
                    lantern.currentCondition += item.currentCondition;
                    collection.RemoveItem(item.IsAStack() ? collection.SplitStack(item, 1) : item);
                    DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "lightRefuel"), false, lantern);
                    Refresh(false);
                }
                else
                    DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "lightFull"), false, lantern);
            }
            else
            {
                NextVariant(item);
            }

            // Handle enchanted item on use clicks - setup spell and pop back to HUD
            // Classic does not close inventory window like this, but this way feels better to me
            // Will see what feedback is like and revert to classic behaviour if widely preferred
            if (item.IsEnchanted)
            {
                // Close the inventory window first. Some artifacts (Azura's Star, the Oghma Infinium) create windows on use and we don't want to close those.
                CloseWindow();
                GameManager.Instance.PlayerEffectManager.DoItemEnchantmentPayloads(MagicAndEffects.EnchantmentPayloadFlags.Used, item, collection);
                return;
            }
        }

        void RecordLocationFromMap(DaggerfallUnityItem item)
        {
            const int mapTextId = 499;
            PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;

            try
            {
                DFLocation revealedLocation = playerGPS.DiscoverRandomLocation();

                if (string.IsNullOrEmpty(revealedLocation.Name))
                    throw new Exception();

                playerGPS.LocationRevealedByMapItem = revealedLocation.Name;
                GameManager.Instance.PlayerEntity.Notebook.AddNote(
                    TextManager.Instance.GetText(textDatabase, "readMap").Replace("%map", revealedLocation.Name));

                DaggerfallMessageBox mapText = new DaggerfallMessageBox(uiManager, this);
                mapText.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRandomTokens(mapTextId));
                mapText.ClickAnywhereToClose = true;
                mapText.Show();
            }
            catch (Exception)
            {
                // Player has already descovered all valid locations in this region!
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "readMapFail"));
            }
        }

        private void AttemptPrivatePropertyTheft()
        {
            GameManager.Instance.PlayerEntity.TallyCrimeGuildRequirements(true, 1);
            PlayerGPS.DiscoveredBuilding buildingDiscoveryData = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;
            int weightAndNumItems = (int)theftBasket.GetWeight() + theftBasket.Count;
            int chanceBeingDetected = FormulaHelper.CalculateShopliftingChance(playerEntity, buildingDiscoveryData.quality, weightAndNumItems);
            // Send the guards if detected
            if (!Dice100.FailedRoll(chanceBeingDetected))
            {
                playerEntity.CrimeCommitted = PlayerEntity.Crimes.Theft;
                playerEntity.SpawnCityGuards(true);
            }
            else
            {
                PlayerEntity.TallySkill(DFCareer.Skills.Pickpocket, 1);
            }
        }

        #endregion

        #region Item Click Event Handlers

        protected virtual void AccessoryItemsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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
            else if (selectedActionMode == ActionModes.Use)
            {
                UseItem(item);
            }
        }

        private DaggerfallUnityItem PaperDoll_GetItem(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            // Get equip value
            byte value = paperDoll.GetEquipIndex((int)position.x, (int)position.y);
            if (value == 0xff)
                return null;

            // Get item
            EquipSlots slot = (EquipSlots)value;
            DaggerfallUnityItem item = playerEntity.ItemEquipTable.GetItem(slot);
            return item;
        }

        protected virtual void PaperDoll_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUnityItem item = PaperDoll_GetItem(sender, position);
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
            else if (selectedActionMode == ActionModes.Use)
            {
                UseItem(item);
            }
        }

        protected virtual void PaperDoll_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUnityItem item = PaperDoll_GetItem(sender, position);
            if (item == null)
                return;

            NextVariant(item);
        }

        protected virtual void LocalItemListScroller_OnItemClick(DaggerfallUnityItem item)
        {
            // Handle click based on action
            if (selectedActionMode == ActionModes.Equip)
            {
                if (item.IsLightSource)
                {
                    UseItem(item);
                    Refresh(false);
                }
                else
                    EquipItem(item);
            }
            else if (selectedActionMode == ActionModes.Use)
            {
                // Allow item to handle its own use, fall through to general use function if unhandled
                if (!item.UseItem(localItems))
                    UseItem(item, localItems);
                Refresh(false);
            }
            else if (selectedActionMode == ActionModes.Remove)
            {
                // Transfer to remote items
                if (remoteItems != null && !chooseOne)
                {
                    int? canHold = null;
                    if (usingWagon)
                        canHold = WagonCanHoldAmount(item);
                    TransferItem(item, localItems, remoteItems, canHold, true);
                    if (theftBasket != null && lootTarget != null && lootTarget.houseOwned)
                        theftBasket.RemoveItem(item);
                }
            }
            else if (selectedActionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        protected virtual void LocalItemListScroller_OnItemRightClick(DaggerfallUnityItem item)
        {
            NextVariant(item);
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
            if (selectedActionMode == ActionModes.Equip)
            {
                // Transfer to local items
                if (localItems != null)
                    TransferItem(item, remoteItems, localItems, CanCarryAmount(item), equip: true);
                if (theftBasket != null && lootTarget != null && lootTarget.houseOwned)
                    theftBasket.AddItem(item);
            }
            else if (selectedActionMode == ActionModes.Use)
            {
                // Allow item to handle its own use, fall through to general use function if unhandled
                if (!item.UseItem(remoteItems))
                    UseItem(item, remoteItems);
                Refresh(false);
            }
            else if (selectedActionMode == ActionModes.Remove)
            {
                TransferItem(item, remoteItems, localItems, CanCarryAmount(item));
                if (theftBasket != null && lootTarget != null && lootTarget.houseOwned)
                    theftBasket.AddItem(item);
            }
            else if (selectedActionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        protected virtual void RemoteItemListScroller_OnItemRightClick(DaggerfallUnityItem item)
        {
            NextVariant(item);
        }

        protected void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();

        }

        #endregion

        #region Dropped items icon selection Event Handlers

        private void RemoteTargetIconPanel_OnMiddleMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // If items are being dropped by player, iterate up through drop archives
            if (CanChangeDropIcon())
            {
                dropIconArchive = GetNextArchive();
                dropIconTexture = 0;
                UpdateRemoteTargetIcon();
            }
        }

        private void RemoteTargetIconPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            // If items are being dropped by player, iterate up through drop textures
            if (CanChangeDropIcon())
            {
                dropIconTexture++;
                if (dropIconTexture >= DaggerfallLootDataTables.dropIconIdxs[dropIconArchive].Length)
                    dropIconTexture = 0;
                UpdateRemoteTargetIcon();
            }
        }

        private void RemoteTargetIconPanel_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            // If items are being dropped by player, iterate down through drop textures
            if (CanChangeDropIcon())
            {
                dropIconTexture--;
                if (dropIconTexture < 0)
                    dropIconTexture = DaggerfallLootDataTables.dropIconIdxs[dropIconArchive].Length - 1;
                UpdateRemoteTargetIcon();
            }
        }

        private bool CanChangeDropIcon()
        {
            return (remoteTargetType == RemoteTargetTypes.Dropped ||
                    (remoteTargetType == RemoteTargetTypes.Loot && lootTarget.playerOwned));
        }

        private int GetNextArchive()
        {
            bool next = false;
            foreach (int ai in DaggerfallLootDataTables.dropIconIdxs.Keys)
            {
                if (next)
                    return ai;
                next = (ai == dropIconArchive);
            }
            return DaggerfallLootDataTables.dropIconIdxs.Keys.First();
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

        protected virtual void GoldButton_OnMouseEnter(BaseScreenComponent sender)
        {
            UpdateItemInfoPanelGold();
        }

        private void UpdateItemInfoPanelGold()
        {
            int gold = GameManager.Instance.PlayerEntity.GoldPieces;
            float weight = gold * DaggerfallBankManager.goldUnitWeightInKg;
            TextFile.Token[] tokens = {
                TextFile.CreateTextToken(string.Format(goldAmount, gold)),
                TextFile.NewLineToken,
                TextFile.CreateTextToken(string.Format(goldWeight, weight.ToString(weight % 1 == 0 ? "F0" : "F2")))
            };
            UpdateItemInfoPanel(tokens);
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

        #region Window event handlers

        private void OnCloseWindow()
        {
            if (isPrivateProperty && theftBasket != null && theftBasket.Count != 0)
            {
                AttemptPrivatePropertyTheft();
            }
            OnClose -= OnCloseWindow;
        }

        #endregion  
    }
}
