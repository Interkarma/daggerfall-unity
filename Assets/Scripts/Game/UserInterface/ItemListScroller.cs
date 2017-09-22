// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Item scroller UI panel component composed of scrollbar, scroll buttons & items list.
    /// </summary>
    public class ItemListScroller : Panel
    {
        #region UI Rects, Controls, Textures

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
        TextLabel[] itemMiscLabels = new TextLabel[listDisplayUnits];

        #endregion

        #region Fields

        const string greenArrowsTextureName = "INVE06I0.IMG"; // Green up/down arrows when more items available
        const string redArrowsTextureName = "INVE07I0.IMG";   // Red up/down arrows when no more items available

        const int listDisplayUnits = 4;                       // Number of items displayed in scrolling areas
        const int itemButtonMarginSize = 2;                   // Margin of item buttons

        List<DaggerfallUnityItem> items = new List<DaggerfallUnityItem>();

        ToolTip toolTip;
        ItemBackgroundColourHandler backgroundColourHandler;
        ItemLabelTextHandler labelTextHandler;

        #endregion

        #region Delegates, Events, Properties

        public delegate Color ItemBackgroundColourHandler(DaggerfallUnityItem item);

        public delegate string ItemLabelTextHandler(DaggerfallUnityItem item);

        public delegate void OnItemClickHandler(DaggerfallUnityItem item);
        public event OnItemClickHandler OnItemClick;

        public delegate void OnItemHoverHandler(DaggerfallUnityItem item);
        public event OnItemHoverHandler OnItemHover;

        public ItemBackgroundColourHandler BackgroundColourHandler
        {
            get { return backgroundColourHandler; }
            set { backgroundColourHandler = value; }
        }

        public ItemLabelTextHandler LabelTextHandler
        {
            get { return labelTextHandler; }
            set { labelTextHandler = value; }
        }

        public List<DaggerfallUnityItem> Items
        {
            get { return items; }
            set {
                items = value;
                UpdateItemsDisplay();
            }
        }
        #endregion

        #region Constructors, Public methods

        public ItemListScroller(ToolTip toolTip)
            : base()
        {
            this.toolTip = toolTip;

            LoadTextures();
            SetupScrollBar();
            SetupScrollButtons();
            SetupItemsList();
        }

        public void ResetScroll()
        {
            itemListScrollBar.Reset(listDisplayUnits);
        }

        #endregion

        #region Private, Setup methods

        void SetupScrollBar()
        {
            // Item list scroll bar (e.g. items in character inventory)
            itemListScrollBar = new VerticalScrollBar
            {
                Position = new Vector2(1, 18),
                Size = new Vector2(6, 117),
                DisplayUnits = listDisplayUnits
            };
            Components.Add(itemListScrollBar);
            itemListScrollBar.OnScroll += ItemsScrollBar_OnScroll;
        }

        void SetupScrollButtons()
        {
            // Item list scroll buttons
            itemListUpButton = new Button
            {
                Position = new Vector2(0, 0),
                Size = new Vector2(9, 16),
                BackgroundTexture = redUpArrow
            };
            Components.Add(itemListUpButton);
            itemListUpButton.OnMouseClick += ItemsUpButton_OnMouseClick;

            itemListDownButton = new Button
            {
                Position = new Vector2(0, 136),
                Size = new Vector2(9, 16),
                BackgroundTexture = redDownArrow
            };
            Components.Add(itemListDownButton);
            itemListDownButton.OnMouseClick += ItemsDownButton_OnMouseClick;
        }

        void SetupItemsList()
        {
            // List panel for scrolling behaviour
            Panel itemsListPanel = DaggerfallUI.AddPanel(itemListPanelRect, this);
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
                itemButtons[i].OnMouseEnter += ItemButton_OnMouseEnter;
                itemButtons[i].OnMouseScrollUp += ItemButton_OnMouseEnter;
                itemButtons[i].OnMouseScrollDown += ItemButton_OnMouseEnter;

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

                // Misc labels
                itemMiscLabels[i] = DaggerfallUI.AddTextLabel(DaggerfallUI.Instance.Font4, Vector2.zero, string.Empty, itemButtons[i]);
                itemMiscLabels[i].HorizontalAlignment = HorizontalAlignment.Left;
                itemMiscLabels[i].VerticalAlignment = VerticalAlignment.Top;
            }
        }

        void ClearItemsList()
        {
            for (int i = 0; i < listDisplayUnits; i++)
            {
                itemStackLabels[i].Text = string.Empty;
                itemMiscLabels[i].Text = string.Empty;
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
            ClearItemsList();
            if (items == null)
                return;

            // Update scrollbar
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
                ImageData image = DaggerfallUnity.Instance.ItemHelper.GetInventoryImage(item);

                // Set image to button icon
                itemIconPanels[i].BackgroundTexture = image.texture;
                itemIconPanels[i].Size = new Vector2(image.texture.width, image.texture.height);

                // Set stack count
                if (item.stackCount > 1)
                    itemStackLabels[i].Text = item.stackCount.ToString();

                // Handle context specific background colour & label
                if (backgroundColourHandler != null)
                    itemButtons[i].BackgroundColor = backgroundColourHandler(item);
                if (labelTextHandler != null)
                    itemMiscLabels[i].Text = labelTextHandler(item);

                // Tooltip text
                itemButtons[i].ToolTipText =
                    (item.ItemGroup == ItemGroups.Books) ? DaggerfallUnity.Instance.ItemHelper.getBookNameByMessage(item.message, item.LongName) : item.LongName;
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
            upButton.BackgroundTexture = (index > 0) ? greenUpArrow : redUpArrow;

            // Update down button
            upButton.BackgroundTexture = (index < (count - listDisplayUnits)) ? greenDownArrow : redDownArrow;

            // No items above or below
            if (count <= listDisplayUnits)
            {
                upButton.BackgroundTexture = redUpArrow;
                downButton.BackgroundTexture = redDownArrow;
            }
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

        #endregion

        #region Event handlers

        void ItemButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Get index
            int index = itemListScrollBar.ScrollIndex + (int)sender.Tag;
            if (index >= items.Count)
                return;

            // Get item and raise item click event
            DaggerfallUnityItem item = items[index];
            if (item != null && OnItemClick != null)
                OnItemClick(item);
        }

        void ItemButton_OnMouseEnter(BaseScreenComponent sender)
        {
            // Get index
            int index = itemListScrollBar.ScrollIndex + (int)sender.Tag;
            if (index >= items.Count)
                return;

            // Get item and raise item hover event
            DaggerfallUnityItem item = items[index];
            if (item != null && OnItemClick != null)
                OnItemHover(item);
        }

        void ItemsScrollBar_OnScroll()
        {
            UpdateItemsDisplay();
        }

        void ItemsUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            itemListScrollBar.ScrollIndex--;
        }

        void ItemsDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            itemListScrollBar.ScrollIndex++;
        }

        void ItemsListPanel_OnMouseScrollUp(BaseScreenComponent sender)
        {
            itemListScrollBar.ScrollIndex--;
        }

        void ItemsListPanel_OnMouseScrollDown(BaseScreenComponent sender)
        {
            itemListScrollBar.ScrollIndex++;
        }

        #endregion
    }
}