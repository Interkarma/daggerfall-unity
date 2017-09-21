// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

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
    /// Implements an item scroller UI Panel.
    /// </summary>
    public class ItemScroller
    {
        Rect itemsListPanelRect = new Rect(9, 0, 50, 152);
        Rect[] itemsButtonRects = new Rect[]
        {
            new Rect(0, 0, 50, 38),
            new Rect(0, 38, 50, 38),
            new Rect(0, 76, 50, 38),
            new Rect(0, 114, 50, 38)
        };

        Rect upArrowRect = new Rect(0, 0, 9, 16);
        Rect downArrowRect = new Rect(0, 136, 9, 16);

        Texture2D redUpArrow;
        Texture2D greenUpArrow;
        Texture2D redDownArrow;
        Texture2D greenDownArrow;

        Button itemsUpButton;
        Button itemsDownButton;
        VerticalScrollBar itemsScrollBar;

        Button[] itemsButtons = new Button[listDisplayUnits];
        Panel[] itemsIconPanels = new Panel[listDisplayUnits];
        TextLabel[] itemsStackLabels = new TextLabel[listDisplayUnits];


        const string greenArrowsTextureName = "INVE06I0.IMG";           // Green up/down arrows when more items available
        const string redArrowsTextureName = "INVE07I0.IMG";             // Red up/down arrows when no more items available

        Color questItemBackgroundColor = new Color(0f, 0.25f, 0f, 0.5f);

        const int listDisplayUnits = 4;                       // Number of items displayed in scrolling areas
        const int itemButtonMarginSize = 2;                   // Margin of item buttons

        Panel parentPanel;
        List<DaggerfallUnityItem> filteredItems = new List<DaggerfallUnityItem>();

        public List<DaggerfallUnityItem> FilteredItems
        {
            get { return filteredItems; }
            set {
                filteredItems = value;
                UpdateItemsDisplay();
            }
        }

        public ItemScroller(Panel panel)
        {
            parentPanel = panel;

            LoadTextures();
            SetupScrollBars();
            SetupScrollButtons();
            SetupItemsElements();
        }


        void SetupScrollBars()
        {
            // Local items list scroll bar (e.g. items in character inventory)
            itemsScrollBar = new VerticalScrollBar
            {
                Position = new Vector2(1, 18),
                Size = new Vector2(6, 117),
                DisplayUnits = listDisplayUnits
            };
            parentPanel.Components.Add(itemsScrollBar);
            itemsScrollBar.OnScroll += ItemsScrollBar_OnScroll;
        }

        void SetupScrollButtons()
        {
            itemsUpButton = new Button
            {
                Position = new Vector2(0, 0),
                Size = new Vector2(9, 16),
                BackgroundTexture = redUpArrow
            };
            parentPanel.Components.Add(itemsUpButton);
            itemsUpButton.OnMouseClick += ItemsUpButton_OnMouseClick;

            itemsDownButton = new Button
            {
                Position = new Vector2(0, 136),
                Size = new Vector2(9, 16),
                BackgroundTexture = redDownArrow
            };
            parentPanel.Components.Add(itemsDownButton);
            itemsDownButton.OnMouseClick += ItemsDownButton_OnMouseClick;
        }

        void SetupItemsElements()
        {
            // List panel for scrolling behaviour
            Panel itemsListPanel = DaggerfallUI.AddPanel(itemsListPanelRect, parentPanel);
            itemsListPanel.OnMouseScrollUp += ItemsListPanel_OnMouseScrollUp;
            itemsListPanel.OnMouseScrollDown += ItemsListPanel_OnMouseScrollDown;

            // Setup buttons
            for (int i = 0; i < listDisplayUnits; i++)
            {
                // Button
                itemsButtons[i] = DaggerfallUI.AddButton(itemsButtonRects[i], itemsListPanel);
                itemsButtons[i].SetMargins(Margins.All, itemButtonMarginSize);
                //itemsButtons[i].ToolTip = defaultToolTip;
                itemsButtons[i].Tag = i;
                itemsButtons[i].OnMouseClick += ItemsButton_OnMouseClick;
                //if (itemInfoPanelLabel != null)
                //    itemsButtons[i].OnMouseEnter += itemsButton_OnMouseEnter;

                // Icon image panel
                itemsIconPanels[i] = DaggerfallUI.AddPanel(itemsButtons[i], AutoSizeModes.ScaleToFit);
                itemsIconPanels[i].HorizontalAlignment = HorizontalAlignment.Center;
                itemsIconPanels[i].VerticalAlignment = VerticalAlignment.Middle;
                itemsIconPanels[i].MaxAutoScale = 1f;

                // Stack labels
                itemsStackLabels[i] = DaggerfallUI.AddTextLabel(DaggerfallUI.Instance.Font4, Vector2.zero, string.Empty, itemsButtons[i]);
                itemsStackLabels[i].HorizontalAlignment = HorizontalAlignment.Right;
                itemsStackLabels[i].VerticalAlignment = VerticalAlignment.Bottom;
                itemsStackLabels[i].ShadowPosition = Vector2.zero;
                itemsStackLabels[i].TextColor = DaggerfallUI.DaggerfallUnityDefaultToolTipTextColor;
            }
        }

        void ClearItemsElements()
        {
            for (int i = 0; i < listDisplayUnits; i++)
            {
                itemsStackLabels[i].Text = string.Empty;
                itemsButtons[i].ToolTipText = string.Empty;
                itemsIconPanels[i].BackgroundTexture = null;
                itemsButtons[i].BackgroundColor = Color.clear;
            }
            itemsUpButton.BackgroundTexture = redUpArrow;
            itemsDownButton.BackgroundTexture = redDownArrow;
        }

        void UpdateItemsDisplay()
        {
            // Clear list elements
            ClearItemsElements();
            if (filteredItems == null)
                return;

            // Update scroller
            itemsScrollBar.TotalUnits = filteredItems.Count;
            int scrollIndex = GetSafeScrollIndex(itemsScrollBar);

            // Update scroller buttons
            UpdateListScrollerButtons(scrollIndex, filteredItems.Count, itemsUpButton, itemsDownButton);

            // Update images and tooltips
            for (int i = 0; i < listDisplayUnits; i++)
            {
                // Skip if out of bounds
                if (scrollIndex + i >= filteredItems.Count)
                    continue;

                // Get item and image
                DaggerfallUnityItem item = filteredItems[scrollIndex + i];
                ImageData image = GetInventoryImage(item);

                SetItemBackgroundColour(item, i, true);

                // Set image to button icon
                itemsIconPanels[i].BackgroundTexture = image.texture;
                itemsIconPanels[i].Size = new Vector2(image.texture.width, image.texture.height);

                // Set stack count
                if (item.stackCount > 1)
                    itemsStackLabels[i].Text = item.stackCount.ToString();

                // Tooltip text
                string text;
                if (item.ItemGroup == ItemGroups.Books)
                {
                    text = DaggerfallUnity.Instance.ItemHelper.getBookNameByMessage(item.message, item.LongName);
                }
                else
                {
                    text = item.LongName;
                }
                itemsButtons[i].ToolTipText = text;
            }
        }

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
                return DaggerfallUnity.Instance.ItemHelper.GetContainerImage(InventoryContainerImages.Wagon);
            }
            else
            {
                // Get inventory image
                return DaggerfallUnity.Instance.ItemHelper.GetItemImage(item, true);
            }
        }

        void SetItemBackgroundColour(DaggerfallUnityItem item, int i, bool local)
        {
            // TEST: Set green background for remote quest items
            if (item.IsQuestItem)
                itemsButtons[i].BackgroundColor = questItemBackgroundColor;
            else
                itemsButtons[i].BackgroundColor = Color.clear;
        }


        void LoadTextures()
        {
            // Cut out red up/down arrows
            Texture2D redArrowsTexture = ImageReader.GetTexture(redArrowsTextureName);
            redUpArrow = ImageReader.GetSubTexture(redArrowsTexture, upArrowRect);
            redDownArrow = ImageReader.GetSubTexture(redArrowsTexture, downArrowRect);

            // Cut out green up/down arrows
            Texture2D greenArrowsTexture = ImageReader.GetTexture(greenArrowsTextureName);
            greenUpArrow = ImageReader.GetSubTexture(greenArrowsTexture, upArrowRect);
            greenDownArrow = ImageReader.GetSubTexture(greenArrowsTexture, downArrowRect);
        }



        void ItemsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Get index
            int index = itemsScrollBar.ScrollIndex + (int)sender.Tag;
            if (index >= filteredItems.Count)
                return;

            // Get item
            DaggerfallUnityItem item = filteredItems[index];
            if (item == null)
                return;

            // Handle click based on action
            Debug.LogFormat("item {0} clicked", item.ItemName);
            // raise event?
        }

        private void ItemsScrollBar_OnScroll()
        {
            UpdateItemsDisplay();
        }

        private void ItemsUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            itemsScrollBar.ScrollIndex--;
        }

        private void ItemsDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            itemsScrollBar.ScrollIndex++;
        }

        private void ItemsListPanel_OnMouseScrollUp()
        {
            itemsScrollBar.ScrollIndex--;
        }

        private void ItemsListPanel_OnMouseScrollDown()
        {
            itemsScrollBar.ScrollIndex++;
        }
    }
}