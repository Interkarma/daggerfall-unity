// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

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

        Rect myItemsUpButtonRect = new Rect(163, 48, 9, 16);
        Rect myItemsDownButtonRect = new Rect(163, 184, 9, 16);

        Rect myItemsListPanelRect = new Rect(172, 48, 50, 152);
        Rect[] myItemsButtonRects = new Rect[]
        {
            new Rect(0, 0, 50, 38),
            new Rect(0, 38, 50, 38),
            new Rect(0, 76, 50, 38),
            new Rect(0, 114, 50, 38) };

        Rect upArrowRect = new Rect(0, 0, 9, 16);
        Rect downArrowRect = new Rect(0, 136, 9, 16);

        Rect wagonButtonRect = new Rect(226, 14, 31, 14);
        Rect infoButtonRect = new Rect(226, 36, 31, 14);
        Rect equipButtonRect = new Rect(226, 58, 31, 14);
        Rect removeButtonRect = new Rect(226, 80, 31, 14);
        Rect useButtonRect = new Rect(226, 103, 31, 14);
        Rect goldButtonRect = new Rect(226, 126, 31, 14);

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

        Button myItemsUpButton;
        Button myItemsDownButton;

        VerticalScrollBar myItemsScrollBar;
        //VerticalScrollBar otherItemsScrollBar;

        Button[] myItemsButtons = new Button[listDisplayUnits];
        Panel[] myItemIconPanels = new Panel[listDisplayUnits];

        PaperDoll paperDoll = new PaperDoll();

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
        Texture2D wagonSelected;
        Texture2D infoSelected;
        Texture2D equipSelected;

        Texture2D redUpArrow;
        Texture2D greenUpArrow;
        Texture2D redDownArrow;
        Texture2D greenDownArrow;

        ImageData[] myItemImages = new ImageData[listDisplayUnits];

        #endregion

        #region Fields

        const string baseTextureName = "INVE00I0.IMG";
        const string goldTextureName = "INVE01I0.IMG";

        const string greenArrowsTextureName = "INVE06I0.IMG";           // Green up/down arrows when more items available
        const string redArrowsTextureName = "INVE07I0.IMG";             // Red up/down arrows when no more items available

        const int listDisplayUnits = 4;                                 // Number of items displayed in scrolling areas

        //Color32 goldButtonTint = new Color32(200, 200, 0, 128);

        PlayerEntity playerEntity;
        int[] scrollPositions = new int[4];

        TabPages selectedTabPage = TabPages.WeaponsAndArmor;
        List<DaggerfallUnityItem> weaponsAndArmorList = new List<DaggerfallUnityItem>();

        ActionModes selectedActionMode = ActionModes.Equip;

        #endregion

        #region Enums

        enum TabPages
        {
            WeaponsAndArmor,
            MagicItems,
            ClothingAndMisc,
            Ingredients,
        }

        enum ActionModes
        {
            Wagon,
            Info,
            Equip,
            //Remove,
            //Use,
        }

        #endregion

        #region Properties

        public PlayerEntity PlayerEntity
        {
            get { return (playerEntity != null) ? playerEntity : playerEntity = GameManager.Instance.PlayerEntity; }
        }

        #endregion

        #region Constructors

        public DaggerfallInventoryWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
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
            paperDoll.Refresh();

            // Setup UI
            SetupTabPageButtons();
            SetupActionButtons();
            SetupScrollBars();
            SetupScrollButtons();
            SetupMyItemsButtons();

            // Set initial tab page and mode
            SelectTabPage(TabPages.WeaponsAndArmor);
            SelectActionMode(ActionModes.Equip);
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
            removeButton.BackgroundColor = new Color(1, 0, 0, 0.5f);

            useButton = DaggerfallUI.AddButton(useButtonRect, NativePanel);
            useButton.BackgroundColor = new Color(1, 0, 0, 0.5f);

            goldButton = DaggerfallUI.AddButton(goldButtonRect, NativePanel);
            goldButton.BackgroundColor = new Color(1, 0, 0, 0.5f);
        }

        void SetupScrollBars()
        {
            // My items list scroll bar (i.e. items in character inventory)
            myItemsScrollBar = new VerticalScrollBar();
            myItemsScrollBar.Position = new Vector2(164, 66);
            myItemsScrollBar.Size = new Vector2(6, 117);
            myItemsScrollBar.DisplayUnits = listDisplayUnits;
            myItemsScrollBar.OnScroll += MyItemsScrollBar_OnScroll;
            NativePanel.Components.Add(myItemsScrollBar);

            // Other items list scroll bar (i.e. items in shop, monster, loot pile, etc.)
        }

        void SetupScrollButtons()
        {
            myItemsUpButton = DaggerfallUI.AddButton(myItemsUpButtonRect, NativePanel);
            myItemsUpButton.BackgroundTexture = redUpArrow;
            myItemsUpButton.OnMouseClick += MyItemsUpButton_OnMouseClick;

            myItemsDownButton = DaggerfallUI.AddButton(myItemsDownButtonRect, NativePanel);
            myItemsDownButton.BackgroundTexture = redDownArrow;
            myItemsDownButton.OnMouseClick += MyItemsDownButton_OnMouseClick;
        }

        void SetupMyItemsButtons()
        {
            // List panel for scrolling behaviour
            Panel myItemsListPanel = DaggerfallUI.AddPanel(myItemsListPanelRect, NativePanel);
            myItemsListPanel.OnMouseScrollUp += MyItemsListPanel_OnMouseScrollUp;
            myItemsListPanel.OnMouseScrollDown += MyItemsListPanel_OnMouseScrollDown;

            // Setup buttons
            const int marginSize = 2;
            for (int i = 0; i < listDisplayUnits; i++)
            {
                // Button
                myItemsButtons[i] = DaggerfallUI.AddButton(myItemsButtonRects[i], myItemsListPanel);
                myItemsButtons[i].SetMargins(Margins.All, marginSize);
                myItemsButtons[i].ToolTip = defaultToolTip;
                myItemsButtons[i].Tag = i;
                myItemsButtons[i].OnMouseDoubleClick += DaggerfallInventoryWindow_OnMouseDoubleClick;

                // Icon image panel
                myItemIconPanels[i] = DaggerfallUI.AddPanel(myItemsButtons[i], AutoSizeModes.ScaleToFit);
                myItemIconPanels[i].HorizontalAlignment = HorizontalAlignment.Center;
                myItemIconPanels[i].VerticalAlignment = VerticalAlignment.Middle;
                myItemIconPanels[i].MaxAutoScale = 1f;
                myItemIconPanels[i].BackgroundColor = new Color(1, 0, 0, 0.25f);
            }
        }

        private void DaggerfallInventoryWindow_OnMouseDoubleClick(BaseScreenComponent sender, Vector2 position)
        {
            int index = scrollPositions[(int)selectedTabPage] + (int)sender.Tag;

            // Get selected items list
            List<DaggerfallUnityItem> items = SelectedMyItemsList();
            if (items == null)
                return;

            // Get item
            DaggerfallUnityItem item = items[index];

            // Equip item
            playerEntity.ItemEquipTable.EquipItem(item);
            paperDoll.Refresh();
        }

        public override void OnPush()
        {
            Refresh();
        }

        public override void OnPop()
        {
            // Update weapons in hands
            GameManager.Instance.WeaponManager.UpdateWeapons(playerEntity.ItemEquipTable);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh character portrait and inventory.
        /// Called every time inventory is pushed to top of stack.
        /// </summary>
        public void Refresh()
        {
            playerEntity = GameManager.Instance.PlayerEntity;
            RefreshItemLists();
            paperDoll.Refresh();
        }

        #endregion

        #region Helper Methods

        void SelectTabPage(TabPages tabPage)
        {
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
                    ShowWeaponsAndArmorPage();
                    break;
                case TabPages.MagicItems:
                    magicItemsButton.BackgroundTexture = magicItemsSelected;
                    ShowMagicItemsPage();
                    break;
                case TabPages.ClothingAndMisc:
                    clothingAndMiscButton.BackgroundTexture = clothingAndMiscSelected;
                    ShowClothingAndMiscPage();
                    break;
                case TabPages.Ingredients:
                    ingredientsButton.BackgroundTexture = ingredientsSelected;
                    ShowIngredientsPage();
                    break;
            }

            UpdateMyItemsScrollButtons();
            UpdateMyItemsImages();
        }

        void SelectActionMode(ActionModes mode)
        {
            selectedActionMode = mode;

            // Clear all button selections
            wagonButton.BackgroundTexture = wagonNotSelected;
            infoButton.BackgroundTexture = infoNotSelected;
            equipButton.BackgroundTexture = equipNotSelected;

            // Set button selected texture
            switch(mode)
            {
                case ActionModes.Wagon:
                    wagonButton.BackgroundTexture = wagonSelected;
                    break;
                case ActionModes.Info:
                    infoButton.BackgroundTexture = infoSelected;
                    break;
                case ActionModes.Equip:
                    equipButton.BackgroundTexture = equipSelected;
                    break;
            }
        }

        void ShowWeaponsAndArmorPage()
        {
            myItemsScrollBar.TotalUnits = weaponsAndArmorList.Count;
            myItemsScrollBar.ScrollIndex = scrollPositions[(int)TabPages.WeaponsAndArmor];
        }

        void ShowMagicItemsPage()
        {
            myItemsScrollBar.TotalUnits = 0;
            myItemsScrollBar.ScrollIndex = scrollPositions[(int)TabPages.MagicItems];
        }

        void ShowClothingAndMiscPage()
        {
            myItemsScrollBar.TotalUnits = 0;
            myItemsScrollBar.ScrollIndex = scrollPositions[(int)TabPages.ClothingAndMisc];
        }

        void ShowIngredientsPage()
        {
            myItemsScrollBar.TotalUnits = 0;
            myItemsScrollBar.ScrollIndex = scrollPositions[(int)TabPages.Ingredients];
        }

        int SelectedMyItemsCount()
        {
            switch (selectedTabPage)
            {
                case TabPages.WeaponsAndArmor:
                    return weaponsAndArmorList.Count;
                case TabPages.MagicItems:
                    return 0;
                case TabPages.ClothingAndMisc:
                    return 0;
                case TabPages.Ingredients:
                    return 0;
                default:
                    return 0;
            }
        }

        List<DaggerfallUnityItem> SelectedMyItemsList()
        {
            switch (selectedTabPage)
            {
                case TabPages.WeaponsAndArmor:
                    return weaponsAndArmorList;
                case TabPages.MagicItems:
                    return null;
                case TabPages.ClothingAndMisc:
                    return null;
                case TabPages.Ingredients:
                    return null;
                default:
                    return null;
            }
        }

        void UpdateMyItemsScrollButtons()
        {
            int count = SelectedMyItemsCount();
            int index = scrollPositions[(int)selectedTabPage];

            // More items above
            if (index > 0)
                myItemsUpButton.BackgroundTexture = greenUpArrow;
            else
                myItemsUpButton.BackgroundTexture = redUpArrow;

            // More items below
            if (index < (count - listDisplayUnits))
                myItemsDownButton.BackgroundTexture = greenDownArrow;
            else
                myItemsDownButton.BackgroundTexture = redDownArrow;

            // No items above or below
            if (count <= listDisplayUnits)
            {
                myItemsUpButton.BackgroundTexture = redUpArrow;
                myItemsDownButton.BackgroundTexture = redDownArrow;
            }
        }

        void UpdateMyItemsImages()
        {
            int index = scrollPositions[(int)selectedTabPage];

            // Reset all item images
            for (int i = 0; i < myItemImages.Length; i++)
                myItemImages[i].type = ImageTypes.None;

            // Get selected items list
            List<DaggerfallUnityItem> items = SelectedMyItemsList();
            if (items == null)
                return;

            // Get images for inventory items
            for (int i = 0; i < listDisplayUnits; i++)
            {
                // Skip when display larger than total units
                if (index + i >= items.Count)
                {
                    myItemsButtons[i].ToolTipText = string.Empty;
                    continue;
                }

                // Get item
                DaggerfallUnityItem item = items[index + i];

                // Get inventory image
                if (item.TemplateIndex == (int)Transportation.Small_cart)
                {
                    // Handle small cart - the template image for this is not correct
                    // Correct image actually in CIF files
                    myItemImages[i] = DaggerfallUnity.ItemHelper.GetContainerImage(ContainerTypes.Wagon);
                }
                else
                {
                    // Get inventory image
                    myItemImages[i] = DaggerfallUnity.ItemHelper.GetItemImage(item, true);
                }

                // Set image to button icon
                myItemIconPanels[i].BackgroundTexture = myItemImages[i].texture;
                myItemIconPanels[i].Size = new Vector2(myItemImages[i].texture.width, myItemImages[i].texture.height);

                // Set tooltip text
                string text = item.LongName;

                // Show some debug data
                //ItemTemplate template = item.ItemTemplate;
                //int equipIndex = DaggerfallUnity.Instance.ItemHelper.GetLegacyEquipIndex(item, playerEntity.Items);
                //if (equipIndex != -1) text += string.Format("\re:{0}", equipIndex);
                //text += string.Format("\ra:{0} i:{1} c:{2}", item.PlayerTextureArchive, item.PlayerTextureRecord, item.dyeColor);
                //text += string.Format("\ra:{0} i:{1}", template.playerTextureArchive, template.playerTextureRecord);
                //text += string.Format("\rdraw:{0}", item.DrawOrder);

                myItemsButtons[i].ToolTipText = text;
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

            // Cut out action mode selected buttons
            wagonSelected = ImageReader.GetSubTexture(goldTexture, wagonButtonRect);
            infoSelected = ImageReader.GetSubTexture(goldTexture, infoButtonRect);
            equipSelected = ImageReader.GetSubTexture(goldTexture, equipButtonRect);
        }

        void RefreshItemLists()
        {
            // Get current items list
            EntityItems playerItems = PlayerEntity.Items;

            // Setup item arrays
            weaponsAndArmorList = new List<DaggerfallUnityItem>();

            // Enumerate items to group lists
            foreach(DaggerfallUnityItem item in playerItems)
            {
                // Only add appropriate items
                //if (item.ItemGroup == ItemGroups.Weapons || item.ItemGroup == ItemGroups.Armor)
                //{
                //    weaponsAndArmorList.Add(item);
                //}

                // TEST: Just add all items for now
                weaponsAndArmorList.Add(item);
            }

            // Reset scroll positions
            for (int i = 0; i < 4; i++)
            {
                scrollPositions[i] = 0;
            }
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
            SelectActionMode(ActionModes.Wagon);
        }

        private void InfoButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectActionMode(ActionModes.Info);
        }

        private void EquipButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SelectActionMode(ActionModes.Equip);
        }

        #endregion

        #region ScrollBar Event Handlers

        private void MyItemsScrollBar_OnScroll()
        {
            // Save new scroll position and update button states
            scrollPositions[(int)selectedTabPage] = myItemsScrollBar.ScrollIndex;
            UpdateMyItemsScrollButtons();
            UpdateMyItemsImages();
        }

        private void MyItemsUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            myItemsScrollBar.ScrollIndex--;
        }

        private void MyItemsDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            myItemsScrollBar.ScrollIndex++;
        }

        private void MyItemsListPanel_OnMouseScrollUp()
        {
            myItemsScrollBar.ScrollIndex--;
        }

        private void MyItemsListPanel_OnMouseScrollDown()
        {
            myItemsScrollBar.ScrollIndex++;
        }

        #endregion
    }
}