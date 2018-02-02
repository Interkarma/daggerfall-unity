// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Banking;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallBankPurchasePopUp : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect displayPanelRect = new Rect(117, 12, 104, 91);
        Rect buyButtonRect = new Rect(38, 106, 40, 19);
        Rect exitButtonRect = new Rect(150, 106, 40, 19);
        Rect upArrowRect = new Rect(0, 0, 9, 16);
        Rect downArrowRect = new Rect(0, 64, 9, 16);

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();

        ListBox priceListBox;
        Button priceListUpButton;
        Button priceListDownButton;
        VerticalScrollBar priceListScrollBar;

        Panel displayPanel;
        Button buyButton;
        Button exitButton;

        #endregion

        #region UI Textures

        const string baseTextureName = "BANK01I0.IMG";
        Texture2D baseTexture;
        const string greenArrowsTextureName = "BANK01I1.IMG";   // Green up/down arrows when more items available
        const string redArrowsTextureName = "BANK01I2.IMG";     // Red up/down arrows when no more items available
        Texture2D redUpArrow;
        Texture2D greenUpArrow;
        Texture2D redDownArrow;
        Texture2D greenDownArrow;

        #endregion

        #region Properties & Constants

        DaggerfallBankingWindow bankingWindow;
        private const int listDisplayUnits = 10;  // Number of items displayed in scrolling area
        private const int scrollNum = 1;          // Number of items on each scroll tick
        private bool ships;                       // True if purchasing ships, otherwise showing purchasable houses.

        #endregion

        #region Constructors

        public DaggerfallBankPurchasePopUp(IUserInterfaceManager uiManager, DaggerfallBankingWindow previousWindow = null, bool ships = false)
            : base(uiManager, previousWindow)
        {
            this.ships = ships;
            this.bankingWindow = previousWindow;
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load textures
            LoadTextures();

            // Create interface panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.Position = new Vector2(0, 50);
            mainPanel.Size = new Vector2(baseTexture.width, baseTexture.height);

            displayPanel = DaggerfallUI.AddPanel(displayPanelRect, mainPanel);

            // Buy button
            buyButton = DaggerfallUI.AddButton(buyButtonRect, mainPanel);
            buyButton.OnMouseClick += BuyButton_OnMouseClick;

            // No button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, mainPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            SetupScrollBar();
            SetupScrollButtons();
            SetupPriceList();

            NativePanel.Components.Add(mainPanel);

        }

        #endregion

        #region Private, Setup methods

        private void PopulatePriceList()
        {
            if (ships)
            {
                for (int i = 0; i < 2; i++)
                    priceListBox.AddItem(HardStrings.bankPurchasePrice.Replace("%s", DaggerfallBankManager.GetShipPrice((ShipType) i).ToString()), i);
            }
            else
            {   // List all the houses for sale in this location
                for (int i = 0; i < 20; i++)
                {
                    priceListBox.AddItem("Price : " + (100000 + i) + " gold");
                }
            }
        }

        private void SetupPriceList()
        {
            priceListBox = new ListBox()
            {
                Name = "price_list",
                Position = new Vector2(5, 24),
                Size = new Vector2(99, 78),
                RowsDisplayed = listDisplayUnits,
                MaxCharacters = 20,
            };
            mainPanel.Components.Add(priceListBox);
            priceListBox.OnScroll += PriceListBox_OnScroll;
            priceListBox.OnSelectItem += PriceListBox_OnSelectItem;

            PopulatePriceList();

            priceListBox.SelectNone();
            priceListScrollBar.TotalUnits = priceListBox.Count;
            UpdateListScrollerButtons(priceListBox.ScrollIndex, priceListBox.Count);
        }

        void SetupScrollBar()
        {
            // Price list scroll bar
            priceListScrollBar = new VerticalScrollBar
            {
                Position = new Vector2(106, 39),
                Size = new Vector2(7, 48),
                DisplayUnits = listDisplayUnits
            };
            mainPanel.Components.Add(priceListScrollBar);
            priceListScrollBar.OnScroll += PriceScrollBar_OnScroll;
        }

        void SetupScrollButtons()
        {
            // Item list scroll buttons
            priceListUpButton = new Button
            {
                Position = new Vector2(105, 23),
                Size = new Vector2(9, 16),
                BackgroundTexture = redUpArrow
            };
            mainPanel.Components.Add(priceListUpButton);
            priceListUpButton.OnMouseClick += PriceUpButton_OnMouseClick;

            priceListDownButton = new Button
            {
                Position = new Vector2(105, 87),
                Size = new Vector2(9, 16),
                BackgroundTexture = redDownArrow
            };
            mainPanel.Components.Add(priceListDownButton);
            priceListDownButton.OnMouseClick += PriceDownButton_OnMouseClick;
        }

        // Updates red/green state of scroller buttons
        void UpdateListScrollerButtons(int index, int count)
        {
            // Update up button
            priceListUpButton.BackgroundTexture = (index > 0) ? greenUpArrow : redUpArrow;

            // Update down button
            priceListDownButton.BackgroundTexture = (index < (count - listDisplayUnits)) ? greenDownArrow : redDownArrow;

            // No items above or below
            if (count <= listDisplayUnits)
            {
                priceListUpButton.BackgroundTexture = redUpArrow;
                priceListDownButton.BackgroundTexture = redDownArrow;
            }
        }

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName);

            // Cut out red up/down arrows
            Texture2D redArrowsTexture = ImageReader.GetTexture(redArrowsTextureName);
            redUpArrow = ImageReader.GetSubTexture(redArrowsTexture, upArrowRect);
            redDownArrow = ImageReader.GetSubTexture(redArrowsTexture, downArrowRect);

            // Cut out green up/down arrows
            Texture2D greenArrowsTexture = ImageReader.GetTexture(greenArrowsTextureName);
            greenUpArrow = ImageReader.GetSubTexture(greenArrowsTexture, upArrowRect);
            greenDownArrow = ImageReader.GetSubTexture(greenArrowsTexture, downArrowRect);
        }

        #endregion

        #region Event Handlers

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void BuyButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (priceListBox.SelectedIndex < 0)
                return;
            if (ships)
            {
                CloseWindow();
                bankingWindow.GeneratePurchaseShipPopup((ShipType) priceListBox.SelectedIndex);
            }
        }

        void PriceListBox_OnSelectItem()
        {
            Debug.Log("Selected " + priceListBox.SelectedIndex);
        }


        void PriceListBox_OnScroll()
        {
            UpdateListScrollerButtons(priceListBox.ScrollIndex, priceListBox.Count);
            priceListScrollBar.ScrollIndex = priceListBox.ScrollIndex;
            priceListScrollBar.Update();
        }

        void PriceScrollBar_OnScroll()
        {
            priceListBox.ScrollIndex = priceListScrollBar.ScrollIndex;
        }

        void PriceUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            priceListBox.ScrollUp();
        }

        void PriceDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            priceListBox.ScrollDown();
        }

        void PriceListPanel_OnMouseScrollUp(BaseScreenComponent sender)
        {
            priceListBox.ScrollUp();
        }

        void PriceListPanel_OnMouseScrollDown(BaseScreenComponent sender)
        {
            priceListBox.ScrollDown();
        }

        #endregion
    }
}