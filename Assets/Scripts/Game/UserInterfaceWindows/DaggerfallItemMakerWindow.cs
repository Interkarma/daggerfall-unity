// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut, Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
//
// Notes:
//

using System;
using System.Linq;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.Items;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallConnect.FallExe;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallItemMakerWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect weaponsAndArmorRect = new Rect(175, 6, 81, 9);
        Rect magicItemsRect = new Rect(175, 15, 81, 9);
        Rect clothingAndMiscRect = new Rect(175, 24, 81, 9);
        Rect ingredientsRect = new Rect(175, 33, 81, 9);

        Rect powersButtonRect = new Rect(8, 183, 77, 10);
        Rect sideEffectsButtonRect = new Rect(106, 183, 77, 10);
        Rect exitButtonRect = new Rect(202, 176, 39, 22);

        Rect enchantButtonRect = new Rect(200, 115, 43, 15);
        Rect selectedItemRect = new Rect(196, 68, 50, 37);

        Rect itemListScrollerRect = new Rect(253, 49, 60, 148);
        Rect itemListPanelRect = new Rect(10, 0, 50, 148);
        readonly Rect[] itemButtonRects = new Rect[]
        {
            new Rect(0, 0, 50, 37),
            new Rect(0, 37, 50, 37),
            new Rect(0, 74, 50, 37),
            new Rect(0, 111, 50, 37)
        };

        Rect powersListRect = new Rect(10, 58, 75, 120);
        Rect sideEffectsListRect = new Rect(108, 58, 75, 120);
        Rect nameItemButtonRect = new Rect(4, 2, 157, 7);

        #endregion

        #region UI Controls

        TextLabel itemNameLabel = new TextLabel();
        TextLabel availableGoldLabel = new TextLabel();
        TextLabel goldCostLabel = new TextLabel();
        TextLabel enchantmentCostLabel = new TextLabel();

        Button weaponsAndArmorButton;
        Button magicItemsButton;
        Button clothingAndMiscButton;
        Button ingredientsButton;

        Button powersButton;
        Button sideEffectsButton;
        Button exitButton;

        Button enchantButton;
        Button selectedItemButton;
        Panel selectedItemPanel;

        ItemListScroller itemsListScroller;
        EnchantmentListPicker powersList;
        EnchantmentListPicker sideEffectsList;

        DaggerfallListPickerWindow enchantmentPrimaryPicker;
        DaggerfallListPickerWindow enchantmentSecondaryPicker;

        #endregion

        #region UI Textures

        Texture2D baseTexture;
        Texture2D goldTexture;
        readonly DFSize baseSize = new DFSize(320, 200);
        readonly DFSize goldSize = new DFSize(81, 36);

        Texture2D weaponsAndArmorNotSelected;
        Texture2D magicItemsNotSelected;
        Texture2D clothingAndMiscNotSelected;
        Texture2D ingredientsNotSelected;
        Texture2D weaponsAndArmorSelected;
        Texture2D magicItemsSelected;
        Texture2D clothingAndMiscSelected;
        Texture2D ingredientsSelected;
        Texture2D smoothEnhancedScrollbar;

        #endregion

        #region Fields

        const MagicCraftingStations thisMagicStation = MagicCraftingStations.ItemMaker;

        const string baseTextureName = "ITEM00I0.IMG";
        const string goldTextureName = "ITEM01I0.IMG";
        const int alternateAlphaIndex = 12;
        const int maxEnchantSlots = 10;

        PlayerEntity playerEntity;
        DaggerfallInventoryWindow.TabPages selectedTabPage = DaggerfallInventoryWindow.TabPages.WeaponsAndArmor;
        List<DaggerfallUnityItem> itemsFiltered = new List<DaggerfallUnityItem>();
        DaggerfallUnityItem selectedItem;

        Dictionary<string, IEntityEffect> groupedPowerTemplates = new Dictionary<string, IEntityEffect>();
        Dictionary<string, IEntityEffect> groupedSideEffectTemplates = new Dictionary<string, IEntityEffect>();
        List<EnchantmentSettings> enumeratedPowerEnchantments = new List<EnchantmentSettings>();
        List<EnchantmentSettings> enumeratedSideEffectEnchantments = new List<EnchantmentSettings>();

        bool selectingPowers;
        List<EnchantmentSettings> itemPowers = new List<EnchantmentSettings>();
        List<EnchantmentSettings> itemSideEffects = new List<EnchantmentSettings>();

        #endregion

        #region Properties

        PlayerEntity PlayerEntity {
            get { return (playerEntity != null) ? playerEntity : playerEntity = GameManager.Instance.PlayerEntity; }
        }

        #endregion

        #region Constructors

        public DaggerfallItemMakerWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load textures
            LoadTextures();

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundColor = new Color(0, 0, 0, 0.60f);
            NativePanel.BackgroundTexture = baseTexture;

            // Setup UI
            SetupLabels();
            SetupButtons();
            SetupListBoxes();
            SetupPickers();
            SetupItemListScrollers();

            SelectTabPage(selectedTabPage);
            EnumerateEnchantments();
        }

        public override void OnPush()
        {
            if (!IsSetup)
                return;

            selectedItem = null;
            powersList.ClearEnchantments();
            sideEffectsList.ClearEnchantments();
            EnumerateEnchantments();
            Refresh();
        }

        public override void OnPop()
        {
        }

        void Refresh()
        {
            // Update labels and lists
            availableGoldLabel.Text = PlayerEntity.GetGoldAmount().ToString();
            if (selectedItem == null)
            {
                itemNameLabel.Text = string.Empty;
                enchantmentCostLabel.Text = string.Empty;
                goldCostLabel.Text = string.Empty;
            }
            else
            {
                int totalEnchantmentCost = GetTotalEnchantmentCost();
                int totalGoldCost = GetTotalGoldCost();
                int itemEnchantmentPower = selectedItem.GetEnchantmentPower();

                enchantmentCostLabel.Text = string.Format("{0}/{1}", totalEnchantmentCost, itemEnchantmentPower);
                //Debug.LogFormat("used: {0} onBuild: {1}", itemEnchantmentPower, selectedItem.enchantmentPoints);

                goldCostLabel.Text = totalGoldCost.ToString();
            }

            // Add appropriate items to filtered list
            itemsFiltered.Clear();
            ItemCollection playerItems = PlayerEntity.Items;
            for (int i = 0; i < playerItems.Count; i++)
                AddFilteredItem(playerItems.GetItem(i));

            itemsListScroller.Items = itemsFiltered;

            if (selectedItem != null) {
                ImageData image = DaggerfallUnity.Instance.ItemHelper.GetInventoryImage(selectedItem);
                selectedItemPanel.BackgroundTexture = image.texture;
                selectedItemPanel.Size = new Vector2(image.texture.width, image.texture.height);
            } else {
                selectedItemPanel.BackgroundTexture = null;
            }
        }

        int GetTotalEnchantmentCost()
        {
            return powersList.GetTotalEnchantmentCost(false) + sideEffectsList.GetTotalEnchantmentCost(false);
        }

        int GetTotalGoldCost()
        {
            return powersList.GetTotalEnchantmentCost(true) * 10;
        }

        void EnumerateEnchantments()
        {
            // Clear existing enchantments
            groupedPowerTemplates.Clear();
            groupedSideEffectTemplates.Clear();
            enumeratedPowerEnchantments.Clear();
            enumeratedSideEffectEnchantments.Clear();

            // Get all effect templates with enchantments
            List<IEntityEffect> effectTemplates = GameManager.Instance.EntityEffectBroker.GetEnchantmentEffectTemplates();
            foreach (IEntityEffect effect in effectTemplates)
            {
                // Get enchantments for this effect
                EnchantmentSettings[] enchantments = effect.GetEnchantmentSettings();
                if (enchantments == null || enchantments.Length == 0)
                    Debug.LogWarningFormat("Effect template '{0}' returned no settings from GetEnchantmentSettings()", effect.Key);

                // Sort enchantments into powers and side-effects
                foreach(EnchantmentSettings enchantment in enchantments)
                {
                    if (enchantment.EnchantCost > 0)
                    {
                        // Add grouped key if not present
                        if (!groupedPowerTemplates.ContainsKey(effect.Key))
                            groupedPowerTemplates.Add(effect.Key, effect);

                        // Add to powers list
                        enumeratedPowerEnchantments.Add(enchantment);
                    }
                    else
                    {
                        // Add grouped key if not present
                        if (!groupedSideEffectTemplates.ContainsKey(effect.Key))
                            groupedSideEffectTemplates.Add(effect.Key, effect);

                        // Add to side-effects list
                        enumeratedSideEffectEnchantments.Add(enchantment);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName, 0, 0, true, alternateAlphaIndex);
            goldTexture = ImageReader.GetTexture(goldTextureName);

            // Cut out tab page not selected button textures
            weaponsAndArmorNotSelected = ImageReader.GetSubTexture(baseTexture, weaponsAndArmorRect, baseSize);
            magicItemsNotSelected = ImageReader.GetSubTexture(baseTexture, magicItemsRect, baseSize);
            clothingAndMiscNotSelected = ImageReader.GetSubTexture(baseTexture, clothingAndMiscRect, baseSize);
            ingredientsNotSelected = ImageReader.GetSubTexture(baseTexture, ingredientsRect, baseSize);

            // Cut out tab page selected button textures
            weaponsAndArmorSelected = ImageReader.GetSubTexture(goldTexture, new Rect(0, 0, 81, 9), goldSize);
            magicItemsSelected = ImageReader.GetSubTexture(goldTexture, new Rect(0, 9, 81, 9), goldSize);
            clothingAndMiscSelected = ImageReader.GetSubTexture(goldTexture, new Rect(0, 18, 81, 9), goldSize);
            ingredientsSelected = ImageReader.GetSubTexture(goldTexture, new Rect(0, 27, 81, 9), goldSize);
        }

        void SetupLabels()
        {
            itemNameLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(52, 3), NativePanel);
            availableGoldLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(71, 15), NativePanel);
            goldCostLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(64, 27), NativePanel);
            enchantmentCostLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(98, 39), NativePanel);
        }

        void SetupButtons()
        {
            // Tab page buttons
            weaponsAndArmorButton = DaggerfallUI.AddButton(weaponsAndArmorRect, NativePanel);
            weaponsAndArmorButton.OnMouseClick += WeaponsAndArmor_OnMouseClick;
            magicItemsButton = DaggerfallUI.AddButton(magicItemsRect, NativePanel);
            magicItemsButton.OnMouseClick += MagicItems_OnMouseClick;
            clothingAndMiscButton = DaggerfallUI.AddButton(clothingAndMiscRect, NativePanel);
            clothingAndMiscButton.OnMouseClick += ClothingAndMisc_OnMouseClick;
            ingredientsButton = DaggerfallUI.AddButton(ingredientsRect, NativePanel);
            ingredientsButton.OnMouseClick += Ingredients_OnMouseClick;

            // Add powers & side-effects buttons
            Button powersButton = DaggerfallUI.AddButton(powersButtonRect, NativePanel);
            powersButton.OnMouseClick += PowersButton_OnMouseClick;
            Button sideEffectsButton = DaggerfallUI.AddButton(sideEffectsButtonRect, NativePanel);
            sideEffectsButton.OnMouseClick += SideEffectsButton_OnMouseClick;

            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            Button enchantButton = DaggerfallUI.AddButton(enchantButtonRect, NativePanel);
            enchantButton.OnMouseClick += EnchantButton_OnMouseClick;

            // Selected item button
            selectedItemButton = DaggerfallUI.AddButton(selectedItemRect, NativePanel);
            selectedItemButton.SetMargins(Margins.All, 2);
            selectedItemButton.OnMouseClick += SelectedItemButton_OnMouseClick;

            // Selected item icon image panel
            selectedItemPanel = DaggerfallUI.AddPanel(selectedItemButton, AutoSizeModes.ScaleToFit);
            selectedItemPanel.HorizontalAlignment = HorizontalAlignment.Center;
            selectedItemPanel.VerticalAlignment = VerticalAlignment.Middle;
            selectedItemPanel.MaxAutoScale = 1f;

            // Rename item button
            Button nameItemButon = DaggerfallUI.AddButton(nameItemButtonRect, NativePanel);
            nameItemButon.OnMouseClick += NameItemButon_OnMouseClick;
        }

        void SetupListBoxes()
        {
            powersList = new EnchantmentListPicker();
            powersList.Position = new Vector2(powersListRect.x, powersListRect.y);
            powersList.Size = new Vector2(powersListRect.width, powersListRect.height);
            powersList.OnRefreshList += EnchantmentList_OnRefreshList;
            powersList.OnRemoveItem += EnchantmentList_OnRemoveItem;
            NativePanel.Components.Add(powersList);

            sideEffectsList = new EnchantmentListPicker();
            sideEffectsList.Position = new Vector2(sideEffectsListRect.x, sideEffectsListRect.y);
            sideEffectsList.Size = new Vector2(sideEffectsListRect.width, sideEffectsListRect.height);
            sideEffectsList.OnRefreshList += EnchantmentList_OnRefreshList;
            sideEffectsList.OnRemoveItem += EnchantmentList_OnRemoveItem;
            NativePanel.Components.Add(sideEffectsList);
        }

        void SetupPickers()
        {
            // Use a picker for power/side-effect primary selection
            enchantmentPrimaryPicker = new DaggerfallListPickerWindow(uiManager, this, DaggerfallUI.SmallFont, 12);
            enchantmentPrimaryPicker.ListBox.OnUseSelectedItem += EnchantmentPrimaryPicker_OnUseSelectedItem;

            // Use another picker for power/side-effect secondary selection
            enchantmentSecondaryPicker = new DaggerfallListPickerWindow(uiManager, this, DaggerfallUI.SmallFont, 12);
            enchantmentSecondaryPicker.ListBox.OnUseSelectedItem += EnchantmentSecondaryPicker_OnUseSelectedItem;
        }

        void SetupItemListScrollers()
        {
            if (DaggerfallUnity.Settings.EnableEnhancedItemLists)
            {
                // Clean UI background with 2 panels, and initialise a standard enhanced inventory list.
                Panel panel1 = DaggerfallUI.AddPanel(new Rect(253, 179, 9, 5), NativePanel);
                panel1.BackgroundTexture = ImageReader.GetSubTexture(baseTexture, new Rect(253, 175, 9, 5), baseSize);
                Panel panel2 = DaggerfallUI.AddPanel(new Rect(312, 49, 1, 149), NativePanel);
                panel2.BackgroundColor = Color.black;
                itemsListScroller = new ItemListScroller(defaultToolTip);
            }
            else
            {
                itemsListScroller = new ItemListScroller(4, 1, itemListPanelRect, itemButtonRects, new TextLabel(), defaultToolTip);
            }
            itemsListScroller.Position = new Vector2(itemListScrollerRect.x, itemListScrollerRect.y);
            itemsListScroller.Size = new Vector2(itemListScrollerRect.width, itemListScrollerRect.height);
            NativePanel.Components.Add(itemsListScroller);
            itemsListScroller.OnItemClick += ItemListScroller_OnItemClick;
        }

        void SelectTabPage(DaggerfallInventoryWindow.TabPages tabPage)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);

            // Select new tab page
            selectedTabPage = tabPage;

            // Set all buttons to appropriate state
            weaponsAndArmorButton.BackgroundTexture = (tabPage == DaggerfallInventoryWindow.TabPages.WeaponsAndArmor) ? weaponsAndArmorSelected : weaponsAndArmorNotSelected;
            magicItemsButton.BackgroundTexture = (tabPage == DaggerfallInventoryWindow.TabPages.MagicItems) ? magicItemsSelected : magicItemsNotSelected;
            clothingAndMiscButton.BackgroundTexture = (tabPage == DaggerfallInventoryWindow.TabPages.ClothingAndMisc) ? clothingAndMiscSelected : clothingAndMiscNotSelected;
            ingredientsButton.BackgroundTexture = (tabPage == DaggerfallInventoryWindow.TabPages.Ingredients) ? ingredientsSelected : ingredientsNotSelected;

            // Update items
            Refresh();
        }

        // Add item to filtered items based on selected tab
        void AddFilteredItem(DaggerfallUnityItem item)
        {
            if (item == selectedItem || item.IsEnchanted || item.IsPotion)
                return;

            // Weapon or armour, excluding arrows
            bool isWeaponOrArmor =
                (item.ItemGroup == ItemGroups.Weapons || item.ItemGroup == ItemGroups.Armor) &&
                !item.IsOfTemplate(ItemGroups.Weapons, (int)Weapons.Arrow);

            if (selectedTabPage == DaggerfallInventoryWindow.TabPages.WeaponsAndArmor)
            {   // Weapons and armor
                if (isWeaponOrArmor)
                    itemsFiltered.Add(item);
            }
            else if (selectedTabPage == DaggerfallInventoryWindow.TabPages.MagicItems)
            {
                // Not sure what the intent was in classic here. Possibly modifying/viewing enchantments?
                // Just disabling for now, as classic lists nothing in this tab page anyway.
                //if (item.IsEnchanted)
                //    itemsFiltered.Add(item);
            }
            else if (selectedTabPage == DaggerfallInventoryWindow.TabPages.Ingredients)
            {   // Ingredients
                if (IsEnchantableIngredient(item))
                    itemsFiltered.Add(item);
            }
            else if (selectedTabPage == DaggerfallInventoryWindow.TabPages.ClothingAndMisc)
            {   // Everything else
                if (IsEnchantableMiscItem(item))
                    itemsFiltered.Add(item);
            }
        }

        bool IsEnchantableIngredient(DaggerfallUnityItem item)
        {
            // Gem ingredients like amber/jade/ruby are listed under ingredients
            return item.IsIngredient && item.ItemGroup == ItemGroups.Gems;
        }

        bool IsEnchantableMiscItem(DaggerfallUnityItem item)
        {
            // Clothing and jewellery are listed under misc ingredients
            // Classic will list potions here as simple "glass bottles", not replicating this here
            // Also excluding oil, candles, and torches which have been repurposed by the personal light system
            // Also excluding bandages, which should probably be repurposed into a Medical item
            switch (item.ItemGroup)
            {
                case ItemGroups.MensClothing:
                case ItemGroups.WomensClothing:
                case ItemGroups.Jewellery:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region List Management

        void AddEnchantmentSettings(EnchantmentSettings enchantment)
        {
            if (selectingPowers)
            {
                itemPowers.Add(enchantment);
                powersList.AddEnchantment(enchantment);
                
            }
            else
            {
                itemSideEffects.Add(enchantment);
                sideEffectsList.AddEnchantment(enchantment);
            }
        }

        void AddForcedPowers(EnchantmentSettings[] powerEnchantments)
        {
            if (powerEnchantments == null || powerEnchantments.Length == 0)
                return;

            foreach(EnchantmentSettings enchantment in powerEnchantments)
            {
                itemPowers.Add(enchantment);
                powersList.AddEnchantment(enchantment);
            }
        }

        void AddForcedSideEffects(EnchantmentSettings[] sideEffectEnchantments)
        {
            if (sideEffectEnchantments == null || sideEffectEnchantments.Length == 0)
                return;

            foreach (EnchantmentSettings enchantment in sideEffectEnchantments)
            {
                itemSideEffects.Add(enchantment);
                sideEffectsList.AddEnchantment(enchantment);
            }
        }

        bool ContainsEnchantmentKey(string effectKey)
        {
            if (selectingPowers)
            {
                return powersList.ContainsEnchantmentKey(effectKey);
            }
            else
            {
                return sideEffectsList.ContainsEnchantmentKey(effectKey);
            }
        }

        bool ContainsEnchantmentSettings(EnchantmentSettings enchantment)
        {
            if (selectingPowers)
            {
                return powersList.ContainsEnchantment(enchantment);
            }
            else
            {
                return sideEffectsList.ContainsEnchantment(enchantment);
            }
        }

        EnchantmentSettings[] GetFilteredEnchantments(IEntityEffect effect)
        {
            EnchantmentSettings[] filteredEnchantments = null;
            if (selectingPowers)
            {
                filteredEnchantments = enumeratedPowerEnchantments.Where(e => e.EffectKey == effect.Key).ToArray();
                if (filteredEnchantments == null || filteredEnchantments.Length == 0)
                    throw new Exception(string.Format("Found no power enchantments for effect key '{0}'", effect.Key));
            }
            else
            {
                filteredEnchantments = enumeratedSideEffectEnchantments.Where(e => e.EffectKey == effect.Key).ToArray();
                if (filteredEnchantments == null || filteredEnchantments.Length == 0)
                    throw new Exception(string.Format("Found no side-effect enchantments for effect key '{0}'", effect.Key));
            }

            return filteredEnchantments;
        }

        void SortForcedEnchantments(EnchantmentSettings parentEnchantment, ForcedEnchantmentSet set, out EnchantmentSettings[] forcedPowersOut, out EnchantmentSettings[] forcedSideEffectsOut)
        {
            List<EnchantmentSettings> forcedPowers = new List<EnchantmentSettings>();
            List<EnchantmentSettings> forcedSideEffects = new List<EnchantmentSettings>();

            foreach (ForcedEnchantment forcedEnchantment in set.forcedEffects)
            {
                IEntityEffect effect = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(forcedEnchantment.key);
                if (effect == null)
                    continue;

                EnchantmentSettings? enchantmentSettings = effect.GetEnchantmentSettings(forcedEnchantment.param);
                if (enchantmentSettings == null)
                    continue;

                EnchantmentSettings forcedSettings = enchantmentSettings.Value;
                forcedSettings.ParentEnchantment = parentEnchantment.GetHashCode();
                if (forcedSettings.EnchantCost > 0)
                    forcedPowers.Add(forcedSettings);
                else
                    forcedSideEffects.Add(forcedSettings);
            }

            forcedPowersOut = forcedPowers.ToArray();
            forcedSideEffectsOut = forcedSideEffects.ToArray();
        }

        #endregion

        #region Event Handlers

        private void ItemListScroller_OnItemClick(DaggerfallUnityItem item)
        {
            selectedItem = item;
            powersList.ClearEnchantments();
            sideEffectsList.ClearEnchantments();
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            Refresh();

            // Update item name only when selected item changes - or other refreshes will reset custom item name
            itemNameLabel.Text = (selectedItem != null) ? selectedItem.shortName : string.Empty;
        }

        private void SelectedItemButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            selectedItem = null;
            powersList.ClearEnchantments();
            sideEffectsList.ClearEnchantments();
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            Refresh();
        }

        private void PowersButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);

            // Must have an item selected to be enchanted
            const int mustHaveAnItemSelectedID = 1653;
            if (selectedItem == null)
            {
                DaggerfallUI.MessageBox(mustHaveAnItemSelectedID);
                return;
            }

            // Check for max enchantments and display "You cannot enchant this item with any more powers."
            // Displaying this message before opening selection rather than after player makes selection like classic
            const int cannotEnchantAnyMorePowers = 1657;
            if (powersList.EnchantmentCount + sideEffectsList.EnchantmentCount == 10)
            {
                DaggerfallUI.MessageBox(cannotEnchantAnyMorePowers);
                return;
            }

            enchantmentPrimaryPicker.ListBox.ClearItems();
            selectingPowers = true;

            // Populate and display primary picker
            EnchantmentSettings[] currentPowers = powersList.GetEnchantments();
            EnchantmentSettings[] currentSideEffects = sideEffectsList.GetEnchantments();
            foreach(IEntityEffect effect in groupedPowerTemplates.Values)
            {
                // Filter enchantments where multiple primary instances not allowed
                if (!effect.HasItemMakerFlags(ItemMakerFlags.AllowMultiplePrimaryInstances) && ContainsEnchantmentKey(effect.Key))
                    continue;

                // Filter if enchantment is weapon-only and this is not a weapon item
                if (effect.HasItemMakerFlags(ItemMakerFlags.WeaponOnly) && selectedItem.ItemGroup != ItemGroups.Weapons)
                    continue;

                // Filter if any current primary enchantments are exclusive to this one
                if (effect.IsEnchantmentExclusiveTo(currentPowers) || effect.IsEnchantmentExclusiveTo(currentSideEffects))
                    continue;

                enchantmentPrimaryPicker.ListBox.AddItem(effect.GroupName, -1, effect);
            }
            uiManager.PushWindow(enchantmentPrimaryPicker);
        }

        private void SideEffectsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);

            // Must have an item selected to be enchanted
            const int mustHaveAnItemSelectedID = 1653;
            if (selectedItem == null)
            {
                DaggerfallUI.MessageBox(mustHaveAnItemSelectedID);
                return;
            }

            // Check for max enchantments and display "No further side-effects may be enchanted in this item."
            const int cannotEnchantAnyMoreSideEffects = 1658;
            if (powersList.EnchantmentCount + sideEffectsList.EnchantmentCount == 10)
            {
                DaggerfallUI.MessageBox(cannotEnchantAnyMoreSideEffects);
                return;
            }

            enchantmentPrimaryPicker.ListBox.ClearItems();
            selectingPowers = false;

            // Populate and display primary picker
            EnchantmentSettings[] currentPowers = powersList.GetEnchantments();
            EnchantmentSettings[] currentSideEffects = sideEffectsList.GetEnchantments();
            foreach (IEntityEffect effect in groupedSideEffectTemplates.Values)
            {
                // Filter enchantments where multiple primary instances not allowed
                if (!effect.HasItemMakerFlags(ItemMakerFlags.AllowMultiplePrimaryInstances) && ContainsEnchantmentKey(effect.Key))
                    continue;

                // Filter if enchantment is weapon-only and this is not a weapon item
                if (effect.HasItemMakerFlags(ItemMakerFlags.WeaponOnly) && selectedItem.ItemGroup != ItemGroups.Weapons)
                    continue;

                // Filter if any current primary enchantments are exclusive to this one
                if (effect.IsEnchantmentExclusiveTo(currentPowers) || effect.IsEnchantmentExclusiveTo(currentSideEffects))
                    continue;

                enchantmentPrimaryPicker.ListBox.AddItem(effect.GroupName, -1, effect);
            }
            uiManager.PushWindow(enchantmentPrimaryPicker);
        }

        private void EnchantButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            const int notEnoughGold = 1650;
            const int notEnoughEnchantmentPower = 1651;
            const int itemEnchanted = 1652;
            const int itemMustBeSelected = 1653;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);

            // Must have an item selected or display "An item must be selected to be enchanted."
            if (selectedItem == null)
            {
                DaggerfallUI.MessageBox(itemMustBeSelected);
                return;
            }

            // Must have enchantments to apply or display "You have not prepared enchantments for this item."
            if (powersList.EnchantmentCount == 0 && sideEffectsList.EnchantmentCount == 0)
            {
                DaggerfallUI.MessageBox(TextManager.Instance.GetLocalizedText("noEnchantments"));
                return;
            }

            // Get costs
            int totalEnchantmentCost = GetTotalEnchantmentCost();
            int totalGoldCost = GetTotalGoldCost();
            int itemEnchantmentPower = selectedItem.GetEnchantmentPower();

            // Check for available gold and display "You do not have the gold to properly pay the enchanter." if not enough
            int playerGold = GameManager.Instance.PlayerEntity.GetGoldAmount();
            if (playerGold < totalGoldCost)
            {
                DaggerfallUI.MessageBox(notEnoughGold);
                return;
            }

            // Check for enchantment power and display "You cannot enchant this item beyond its limit." if not enough
            if (itemEnchantmentPower < totalEnchantmentCost)
            {
                DaggerfallUI.MessageBox(notEnoughEnchantmentPower);
                return;
            }

            // Deduct gold from player and display "The item has been enchanted."
            GameManager.Instance.PlayerEntity.DeductGoldAmount(totalGoldCost);
            DaggerfallUI.MessageBox(itemEnchanted);

            // Only enchant one item from stack
            if (selectedItem.IsAStack())
                selectedItem = GameManager.Instance.PlayerEntity.Items.SplitStack(selectedItem, 1);

            // Transfer enchantment settings onto item
            List<EnchantmentSettings> combinedEnchantments = new List<EnchantmentSettings>();
            combinedEnchantments.AddRange(powersList.GetEnchantments());
            combinedEnchantments.AddRange(sideEffectsList.GetEnchantments());
            selectedItem.SetEnchantments(combinedEnchantments.ToArray(), GameManager.Instance.PlayerEntity);
            selectedItem.RenameItem(itemNameLabel.Text);

            // Play enchantment sound effect
            DaggerfallUI.Instance.PlayOneShot(SoundClips.MakeItem);

            // Clear selected item and enchantments
            selectedItem = null;
            powersList.ClearEnchantments();
            sideEffectsList.ClearEnchantments();
            Refresh();
        }

        private void WeaponsAndArmor_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectTabPage(DaggerfallInventoryWindow.TabPages.WeaponsAndArmor);
        }

        private void MagicItems_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectTabPage(DaggerfallInventoryWindow.TabPages.MagicItems);
        }

        private void ClothingAndMisc_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectTabPage(DaggerfallInventoryWindow.TabPages.ClothingAndMisc);
        }

        private void Ingredients_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectTabPage(DaggerfallInventoryWindow.TabPages.Ingredients);
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
        }

        private void NameItemButon_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallInputMessageBox mb = new DaggerfallInputMessageBox(uiManager, this);
            mb.TextBox.Text = itemNameLabel.Text;
            mb.SetTextBoxLabel(TextManager.Instance.GetLocalizedText("enterNewName"));
            mb.OnGotUserInput += EnterName_OnGotUserInput;
            mb.Show();
        }

        private void EnterName_OnGotUserInput(DaggerfallInputMessageBox sender, string input)
        {
            itemNameLabel.Text = input;
        }

        #endregion

        #region Effect Picker Events

        private void EnchantmentPrimaryPicker_OnUseSelectedItem()
    {
            // Clear existing
            enchantmentSecondaryPicker.ListBox.ClearItems();

            // Get the effect template tagged to selected item
            ListBox.ListItem listItem = enchantmentPrimaryPicker.ListBox.SelectedValue;
            IEntityEffect effect = listItem.tag as IEntityEffect;
            if (effect == null)
                throw new Exception(string.Format("ListItem '{0}' has no IEntityEffect tag", listItem.textLabel.Text));

            // Filter enchantments based on effect key
            EnchantmentSettings[] filteredEnchantments = GetFilteredEnchantments(effect);

            // If this is a singleton effect with no secondary options then add directly to powers/side-effects
            // Exclude EnchantmentTypes.SoulBound enchantment where player must select soul to correctly assign enforced side-effects
            if (filteredEnchantments.Length == 1 && filteredEnchantments[0].ClassicType != EnchantmentTypes.SoulBound)
            {
                AddEnchantmentSettings(filteredEnchantments[0]);
                enchantmentPrimaryPicker.CloseWindow();
                return;
            }

            // Order filtered list by alpha when requested by effect flags
            if (effect.HasItemMakerFlags(ItemMakerFlags.AlphaSortSecondaryList))
                filteredEnchantments = filteredEnchantments.OrderBy(o => o.SecondaryDisplayName).ToArray();

            // User must select from available secondary enchantment types
            EnchantmentSettings[] currentPowers = powersList.GetEnchantments();
            EnchantmentSettings[] currentSideEffects = sideEffectsList.GetEnchantments();
            foreach (EnchantmentSettings enchantment in filteredEnchantments)
            {
                // Filter out enchantment when multiple instances not allowed
                if (!effect.HasItemMakerFlags(ItemMakerFlags.AllowMultipleSecondaryInstances))
                {
                    if (ContainsEnchantmentSettings(enchantment))
                        continue;
                }

                // Filter if any current secondary enchantments are exclusive to this one
                EnchantmentParam comparerParam = new EnchantmentParam() { ClassicParam = enchantment.ClassicParam, CustomParam = enchantment.CustomParam };
                if (effect.IsEnchantmentExclusiveTo(currentPowers, comparerParam) || effect.IsEnchantmentExclusiveTo(currentSideEffects, comparerParam))
                    continue;

                enchantmentSecondaryPicker.ListBox.AddItem(enchantment.SecondaryDisplayName, -1, enchantment);
            }

            enchantmentSecondaryPicker.ListBox.SelectedIndex = 0;
            uiManager.PushWindow(enchantmentSecondaryPicker);
        }

        private void EnchantmentSecondaryPicker_OnUseSelectedItem()
        {
            // Get enchantment tagged to selected item
            ListBox.ListItem listItem = enchantmentSecondaryPicker.ListBox.SelectedValue;
            if (listItem.tag == null)
                throw new Exception(string.Format("ListItem '{0}' has no EnchantmentSettings tag", listItem.textLabel.Text));

            // Get enchantment settings
            EnchantmentSettings enchantmentSettings = (EnchantmentSettings)listItem.tag;
            IEntityEffect enchantmentEffect = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(enchantmentSettings.EffectKey);
            EnchantmentParam enchantmentParam = new EnchantmentParam() { ClassicParam = enchantmentSettings.ClassicParam, CustomParam = enchantmentSettings.CustomParam };

            // Get forced enchantments for this effect
            EnchantmentSettings[] forcedPowers = null;
            EnchantmentSettings[] forcedSideEffects = null;
            ForcedEnchantmentSet? forcedEnchantmentSet = enchantmentEffect.GetForcedEnchantments(enchantmentParam);
            if (forcedEnchantmentSet != null)
            {
                // Sort forced enchantments into powers and side effects
                SortForcedEnchantments(enchantmentSettings, forcedEnchantmentSet.Value, out forcedPowers, out forcedSideEffects);

                // Check for overflow from automatic enchantments and display "no room in item..."
                // Also adding +1 to account for incoming enchantment
                if (powersList.EnchantmentCount + sideEffectsList.EnchantmentCount + forcedPowers.Length + forcedSideEffects.Length + 1 > 10)
                {
                    DaggerfallUI.MessageBox(TextManager.Instance.GetLocalizedText("noRoomInItem"));
                    return;
                }
            }

            // Add selected enchantment settings to powers/side-effects
            AddEnchantmentSettings(enchantmentSettings);
            AddForcedPowers(forcedPowers);
            AddForcedSideEffects(forcedSideEffects);

            // Close effect pickers
            enchantmentPrimaryPicker.CloseWindow();
            enchantmentSecondaryPicker.CloseWindow();
        }

        private void EnchantmentList_OnRefreshList(EnchantmentListPicker sender)
        {
            Refresh();
        }

        private void EnchantmentList_OnRemoveItem(EnchantmentListPicker.EnchantmentPanel panel)
        {
            powersList.RemoveForcedEnchantments(panel.Enchantment.GetHashCode());
            sideEffectsList.RemoveForcedEnchantments(panel.Enchantment.GetHashCode());
        }

        #endregion
    }
}