// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements an item scroller UI element composed of scrollbar & items.
    /// </summary>
    public class ItemsScroller
    {
        Rect itemListPanelRect = new Rect(9, 0, 50, 152);
        Rect[] itemButtonRects = new Rect[]
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

        Button itemListUpButton;
        Button itemListDownButton;
        VerticalScrollBar itemListScrollBar;

        Button[] itemButtons = new Button[listDisplayUnits];
        Panel[] itemIconPanels = new Panel[listDisplayUnits];
        TextLabel[] itemStackLabels = new TextLabel[listDisplayUnits];


        const string greenArrowsTextureName = "INVE06I0.IMG";           // Green up/down arrows when more items available
        const string redArrowsTextureName = "INVE07I0.IMG";             // Red up/down arrows when no more items available

        Color questItemBackgroundColor = new Color(0f, 0.25f, 0f, 0.5f);

        const int listDisplayUnits = 4;                       // Number of items displayed in scrolling areas
        const int itemButtonMarginSize = 2;                   // Margin of item buttons

        Panel parentPanel;
        ToolTip toolTip;
        List<DaggerfallUnityItem> items = new List<DaggerfallUnityItem>();

        public List<DaggerfallUnityItem> Items
        {
            get { return items; }
            set {
                items = value;
                itemListScrollBar.Reset(listDisplayUnits);
                UpdateItemsDisplay();
            }
        }

        public delegate void OnItemClickHandler(DaggerfallUnityItem item);
        public event OnItemClickHandler OnItemClick;


        public ItemsScroller(Panel panel, ToolTip toolTip)
        {
            parentPanel = panel;
            this.toolTip = toolTip;

            LoadTextures();
            SetupScrollBars();
            SetupScrollButtons();
            SetupItemsElements();
        }


        void SetupScrollBars()
        {
            // Local items list scroll bar (e.g. items in character inventory)
            itemListScrollBar = new VerticalScrollBar
            {
                Position = new Vector2(1, 18),
                Size = new Vector2(6, 117),
                DisplayUnits = listDisplayUnits
            };
            parentPanel.Components.Add(itemListScrollBar);
            itemListScrollBar.OnScroll += ItemsScrollBar_OnScroll;
        }

        void SetupScrollButtons()
        {
            itemListUpButton = new Button
            {
                Position = new Vector2(0, 0),
                Size = new Vector2(9, 16),
                BackgroundTexture = redUpArrow
            };
            parentPanel.Components.Add(itemListUpButton);
            itemListUpButton.OnMouseClick += ItemsUpButton_OnMouseClick;

            itemListDownButton = new Button
            {
                Position = new Vector2(0, 136),
                Size = new Vector2(9, 16),
                BackgroundTexture = redDownArrow
            };
            parentPanel.Components.Add(itemListDownButton);
            itemListDownButton.OnMouseClick += ItemsDownButton_OnMouseClick;
        }

        void SetupItemsElements()
        {
            // List panel for scrolling behaviour
            Panel itemsListPanel = DaggerfallUI.AddPanel(itemListPanelRect, parentPanel);
            itemsListPanel.OnMouseScrollUp += ItemsListPanel_OnMouseScrollUp;
            itemsListPanel.OnMouseScrollDown += ItemsListPanel_OnMouseScrollDown;

            // Setup buttons
            for (int i = 0; i < listDisplayUnits; i++)
            {
                // Button
                itemButtons[i] = DaggerfallUI.AddButton(itemButtonRects[i], itemsListPanel);
                itemButtons[i].SetMargins(Margins.All, itemButtonMarginSize);
                itemButtons[i].ToolTip = toolTip;
                itemButtons[i].Tag = i;
                itemButtons[i].OnMouseClick += ItemButton_OnMouseClick;
                //if (itemInfoPanelLabel != null)
                //    itemsButtons[i].OnMouseEnter += itemsButton_OnMouseEnter;

                // Icon image panel
                itemIconPanels[i] = DaggerfallUI.AddPanel(itemButtons[i], AutoSizeModes.ScaleToFit);
                itemIconPanels[i].HorizontalAlignment = HorizontalAlignment.Center;
                itemIconPanels[i].VerticalAlignment = VerticalAlignment.Middle;
                itemIconPanels[i].MaxAutoScale = 1f;

                // Stack labels
                itemStackLabels[i] = DaggerfallUI.AddTextLabel(DaggerfallUI.Instance.Font4, Vector2.zero, string.Empty, itemButtons[i]);
                itemStackLabels[i].HorizontalAlignment = HorizontalAlignment.Right;
                itemStackLabels[i].VerticalAlignment = VerticalAlignment.Bottom;
                itemStackLabels[i].ShadowPosition = Vector2.zero;
                itemStackLabels[i].TextColor = DaggerfallUI.DaggerfallUnityDefaultToolTipTextColor;
            }
        }

        void ClearItemsElements()
        {
            for (int i = 0; i < listDisplayUnits; i++)
            {
                itemStackLabels[i].Text = string.Empty;
                itemButtons[i].ToolTipText = string.Empty;
                itemIconPanels[i].BackgroundTexture = null;
                itemButtons[i].BackgroundColor = Color.clear;
            }
            itemListUpButton.BackgroundTexture = redUpArrow;
            itemListDownButton.BackgroundTexture = redDownArrow;
        }

        void UpdateItemsDisplay()
        {
            // Clear list elements
            ClearItemsElements();
            if (items == null)
                return;

            // Update scroller
            itemListScrollBar.TotalUnits = items.Count;
            int scrollIndex = GetSafeScrollIndex(itemListScrollBar);

            // Update scroller buttons
            UpdateListScrollerButtons(scrollIndex, items.Count, itemListUpButton, itemListDownButton);

            // Update images and tooltips
            for (int i = 0; i < listDisplayUnits; i++)
            {
                // Skip if out of bounds
                if (scrollIndex + i >= items.Count)
                    continue;

                // Get item and image
                DaggerfallUnityItem item = items[scrollIndex + i];
                ImageData image = GetInventoryImage(item);

                SetItemBackgroundColour(item, i, true);

                // Set image to button icon
                itemIconPanels[i].BackgroundTexture = image.texture;
                itemIconPanels[i].Size = new Vector2(image.texture.width, image.texture.height);

                // Set stack count
                if (item.stackCount > 1)
                    itemStackLabels[i].Text = item.stackCount.ToString();

                // Tooltip text
                string text;
                if (item.ItemGroup == ItemGroups.Books)
                    text = DaggerfallUnity.Instance.ItemHelper.getBookNameByMessage(item.message, item.LongName);
                else
                    text = item.LongName;
                itemButtons[i].ToolTipText = text;
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
                itemButtons[i].BackgroundColor = questItemBackgroundColor;
            else
                itemButtons[i].BackgroundColor = Color.clear;
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



        void ItemButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Get index
            int index = itemListScrollBar.ScrollIndex + (int)sender.Tag;
            if (index >= items.Count)
                return;

            // Get item and raise item click event
            DaggerfallUnityItem item = items[index];
            Debug.LogFormat("item {0} clicked", item.ItemName);
            if (item != null && OnItemClick != null)
                OnItemClick(item);
        }

        private void ItemsScrollBar_OnScroll()
        {
            UpdateItemsDisplay();
        }

        private void ItemsUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            itemListScrollBar.ScrollIndex--;
        }

        private void ItemsDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            itemListScrollBar.ScrollIndex++;
        }

        private void ItemsListPanel_OnMouseScrollUp()
        {
            itemListScrollBar.ScrollIndex--;
        }

        private void ItemsListPanel_OnMouseScrollDown()
        {
            itemListScrollBar.ScrollIndex++;
        }
    }
}