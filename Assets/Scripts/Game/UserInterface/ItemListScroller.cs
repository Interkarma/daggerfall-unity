// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Items;
using System;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Item scroller UI panel component composed of scrollbar, scroll buttons & items list.
    /// </summary>
    public class ItemListScroller : Panel
    {
        #region UI Rects, Controls, Textures

        Rect itemListPanelRect = new Rect(9, 0, 50, 152);
        Rect[] itemButtonRects = itemButtonRects4;

        Rect upArrowRect = new Rect(0, 0, 9, 16);
        Rect downArrowRect = new Rect(0, 136, 9, 16);
        DFSize arrowsFullSize = new DFSize(9, 152);

        Texture2D redUpArrow;
        Texture2D greenUpArrow;
        Texture2D redDownArrow;
        Texture2D greenDownArrow;

        Button itemListUpButton;
        Button itemListDownButton;
        VerticalScrollBar itemListScrollBar;

        Button[] itemButtons;
        Panel[] itemIconPanels;
        Panel[] itemAnimPanels;
        TextLabel[] itemStackLabels;
        TextLabel[] itemMiscLabels;

        #endregion

        #region UI Rects for normal 4 item mode

        static Rect[] itemButtonRects4 = new Rect[]
        {
            new Rect(0, 0, 50, 38),
            new Rect(0, 38, 50, 38),
            new Rect(0, 76, 50, 38),
            new Rect(0, 114, 50, 38)
        };

        #endregion

        #region UI Rects, Textures for enhanced 16 item mode

        static Rect[] itemButtonRects16 = new Rect[]
        {
            new Rect(0, 0, 25, 19),     new Rect(25, 0, 25, 19),
            new Rect(0, 19, 25, 19),    new Rect(25, 19, 25, 19),
            new Rect(0, 38, 25, 19),    new Rect(25, 38, 25, 19),
            new Rect(0, 57, 25, 19),    new Rect(25, 57, 25, 19),
            new Rect(0, 76, 25, 19),    new Rect(25, 76, 25, 19),
            new Rect(0, 95, 25, 19),    new Rect(25, 95, 25, 19),
            new Rect(0, 114, 25, 19),   new Rect(25, 114, 25, 19),
            new Rect(0, 133, 25, 19),   new Rect(25, 133, 25, 19)
        };
        static Rect[] itemCutoutRects16 = new Rect[]
        {
            new Rect(23, 72, 23, 22),   new Rect(23, 10, 23, 22),
            new Rect(0, 41, 23, 22),    new Rect(23, 41, 23, 22),
            new Rect(0, 72, 23, 22),    new Rect(23, 72, 23, 22),
            new Rect(0, 103, 23, 22),   new Rect(23, 103, 23, 22),
            new Rect(0, 134, 23, 22),   new Rect(23, 134, 23, 22),
            new Rect(0, 165, 23, 22),   new Rect(23, 165, 23, 22),
            new Rect(23, 72, 23, 22),   new Rect(0, 41, 23, 22),
            new Rect(23, 41, 23, 22),   new Rect(0, 103, 23, 22)
        };

        Texture2D[] itemListTextures;

        #endregion

        #region Fields

        const string greenArrowsTextureName = "INVE06I0.IMG";   // Green up/down arrows when more items available
        const string redArrowsTextureName = "INVE07I0.IMG";     // Red up/down arrows when no more items available
        const string baseInvTextureName = "INVE00I0.IMG";       // Main inventory texture for cutting small item backgrounds

        int listDisplayUnits = 4;       // Number of items displayed in scrolling areas
        int itemButtonMarginSize = 2;   // Margin of item buttons
        float textScale = 1f;           // Scale of text on item buttons
        int scrollNum = 1;              // Number of items on each scroll tick

        float foregroundAnimationDelay = 0.2f;    
        float backgroundAnimationDelay = 0.2f;

        List<DaggerfallUnityItem> items = new List<DaggerfallUnityItem>();

        ToolTip toolTip;
        ItemBackgroundColourHandler backgroundColourHandler;
        ItemBackgroundAnimationHandler backgroundAnimationHandler;
        ItemForegroundAnimationHandler foregroundAnimationHandler;
        ItemLabelTextHandler labelTextHandler;

        #endregion

        #region Delegates, Events, Properties

        public delegate Color ItemBackgroundColourHandler(DaggerfallUnityItem item);

        public delegate Texture2D[] ItemBackgroundAnimationHandler(DaggerfallUnityItem item);
        public delegate Texture2D[] ItemForegroundAnimationHandler(DaggerfallUnityItem item);

        public delegate string ItemLabelTextHandler(DaggerfallUnityItem item);

        public delegate void OnItemClickHandler(DaggerfallUnityItem item);
        public event OnItemClickHandler OnItemClick;

        public delegate void OnItemHoverHandler(DaggerfallUnityItem item);
        public event OnItemHoverHandler OnItemHover;

        /// <summary>Handler for colour highlighting</summary>
        public ItemBackgroundColourHandler BackgroundColourHandler
        {
            get { return backgroundColourHandler; }
            set { backgroundColourHandler = value; }
        }

        /// <summary>Handler for background animations (can't be colour highlighted)</summary>
        public ItemBackgroundAnimationHandler BackgroundAnimationHandler
        {
            get { return backgroundAnimationHandler; }
            set { backgroundAnimationHandler = value; }
        }
        /// <summary>Delay in seconds between each frame of animation</summary>
        public float BackgroundAnimationDelay
        {
            get { return backgroundAnimationDelay; }
            set { backgroundAnimationDelay = value; }
        }
        /// <summary>Handler for foreground animations (can be colour highlighted)</summary>
        public ItemForegroundAnimationHandler ForegroundAnimationHandler
        {
            get { return foregroundAnimationHandler; }
            set { foregroundAnimationHandler = value; }
        }
        /// <summary>Delay in seconds between each frame of animation</summary>
        public float ForegroundAnimationDelay
        {
            get { return foregroundAnimationDelay; }
            set { foregroundAnimationDelay = value; }
        }
        /// <summary>Handler for label text (top left)</summary>
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

        /// <summary>
        /// Initializes a new instance of the ItemListScroller using default values setup for DF inventory screens.
        /// Switches to advanced display of 16 items instead of 4 if user enables enhanced item lists.
        /// </summary>
        /// <param name="toolTip">Tool tip class to use if items should display tooltips.</param>
        /// <param name="disableEnhanced">If set to <c>true</c> disable enhanced regardless of user settings.</param>
        public ItemListScroller(ToolTip toolTip, bool disableEnhanced = false)
            : base()
        {
            this.toolTip = toolTip;

            bool enhanced = DaggerfallUnity.Settings.EnableEnhancedItemLists && !disableEnhanced;
            if (enhanced)
            {
                // Configure enhanced mode to display 16 items
                listDisplayUnits = 16;
                itemButtonRects = itemButtonRects16;
                itemButtonMarginSize = 1;
                textScale = 0.75f;
                scrollNum = 2;
            }
            LoadTextures(enhanced);
            SetupScrollBar();
            SetupScrollButtons();
            SetupItemsList(enhanced, Vector2.zero);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DaggerfallWorkshop.Game.UserInterface.ItemListScroller"/> class.
        /// </summary>
        /// <param name="listDisplayUnits">Number of items this list will display at a time.</param>
        /// <param name="listWidth">Number of items displayed per row. (should be a factor of listDisplayUnits)</param>
        /// <param name="itemListRect">Item list coordinate rect.</param>
        /// <param name="itemRects">Individual item display coordinate rects. (1 per list display)</param>
        /// <param name="miscLabelPos">Misc label relative position. (Vector2.zero for default)</param>
        /// <param name="itemMarginSize">Individual item display margin size.</param>
        /// <param name="textScale">Text scale factor.</param>
        /// <param name="toolTip">Tool tip class to use if items should display tooltips.</param>
        public ItemListScroller(int listDisplay, int listWidth, Rect itemListRect, Rect[] itemRects, Vector2 miscLabelPos, int itemMarginSize = 1, float textScale = 1f, ToolTip toolTip = null)
            : base()
        {
            if (itemRects.Length != listDisplayUnits || listDisplay % listWidth > 0)
                throw new ArgumentException();
            
            listDisplayUnits = listDisplay;
            scrollNum = listWidth;
            itemListPanelRect = itemListRect;
            itemButtonRects = itemRects;
            itemButtonMarginSize = itemMarginSize;
            this.textScale = textScale;
            this.toolTip = toolTip;

            LoadTextures(false);
            SetupScrollBar();
            SetupScrollButtons();
            SetupItemsList(false, miscLabelPos);
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

        void SetupItemsList(bool enhanced, Vector2 miscLabelPos)
        {
            // List panel for scrolling behaviour
            Panel itemsListPanel = DaggerfallUI.AddPanel(itemListPanelRect, this);
            itemsListPanel.OnMouseScrollUp += ItemsListPanel_OnMouseScrollUp;
            itemsListPanel.OnMouseScrollDown += ItemsListPanel_OnMouseScrollDown;

            // Setup buttons
            itemButtons = new Button[listDisplayUnits];
            itemIconPanels = new Panel[listDisplayUnits];
            itemAnimPanels = new Panel[listDisplayUnits];
            itemStackLabels = new TextLabel[listDisplayUnits];
            itemMiscLabels = new TextLabel[listDisplayUnits];

            for (int i = 0; i < listDisplayUnits; i++)
            {
                // Panel - for backing button in enhanced mode
                if (enhanced) {
                    Panel buttonPanel = DaggerfallUI.AddPanel(itemButtonRects[i], itemsListPanel);
                    buttonPanel.BackgroundTexture = itemListTextures[i];
                }
                // Buttons (also handle highlight colours)
                itemButtons[i] = DaggerfallUI.AddButton(itemButtonRects[i], itemsListPanel);
                itemButtons[i].SetMargins(Margins.All, itemButtonMarginSize);
                itemButtons[i].ToolTip = toolTip;
                itemButtons[i].Tag = i;
                itemButtons[i].OnMouseClick += ItemButton_OnMouseClick;
                itemButtons[i].OnMouseEnter += ItemButton_OnMouseEnter;
                itemButtons[i].OnMouseScrollUp += ItemButton_OnMouseEnter;
                itemButtons[i].OnMouseScrollDown += ItemButton_OnMouseEnter;

                // Item foreground animation panel
                itemAnimPanels[i] = DaggerfallUI.AddPanel(itemButtonRects[i], itemsListPanel);
                itemAnimPanels[i].AnimationDelayInSeconds = foregroundAnimationDelay;

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
                itemStackLabels[i].TextScale = textScale;
                itemStackLabels[i].TextColor = DaggerfallUI.DaggerfallUnityDefaultToolTipTextColor;

                // Misc labels
                itemMiscLabels[i] = DaggerfallUI.AddTextLabel(DaggerfallUI.Instance.Font4, miscLabelPos, string.Empty, itemButtons[i]);
                itemMiscLabels[i].HorizontalAlignment = HorizontalAlignment.Left;
                itemMiscLabels[i].VerticalAlignment = VerticalAlignment.Top;
                itemMiscLabels[i].TextScale = textScale;
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
                itemButtons[i].AnimatedBackgroundTextures = null;
                itemIconPanels[i].AnimatedBackgroundTextures = null;
                itemAnimPanels[i].AnimatedBackgroundTextures = null;
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
            UpdateListScrollerButtons(scrollIndex, items.Count);

            // Update images and tooltips
            for (int i = 0; i < listDisplayUnits; i++)
            {
                // Skip if out of bounds
                if (scrollIndex + i >= items.Count)
                    continue;

                // Get item and image
                DaggerfallUnityItem item = items[scrollIndex + i];
                ImageData image = DaggerfallUnity.Instance.ItemHelper.GetInventoryImage(item);

                // Set animated image frames to button icon (if any)
                if (image.animatedTextures != null && image.animatedTextures.Length > 0)
                    itemIconPanels[i].AnimatedBackgroundTextures = image.animatedTextures;

                // Set image to button icon
                itemIconPanels[i].BackgroundTexture = image.texture;
                itemIconPanels[i].Size = new Vector2(image.texture.width, image.texture.height);

                // Set stack count
                if (item.stackCount > 1)
                    itemStackLabels[i].Text = item.stackCount.ToString();

                // Handle context specific background colour, animations & label
                if (backgroundColourHandler != null)
                    itemButtons[i].BackgroundColor = backgroundColourHandler(item);
                if (backgroundAnimationHandler != null) {
                    itemButtons[i].AnimationDelayInSeconds = backgroundAnimationDelay;
                    itemButtons[i].AnimatedBackgroundTextures = backgroundAnimationHandler(item);
                }
                if (foregroundAnimationHandler != null) {
                    itemAnimPanels[i].AnimationDelayInSeconds = foregroundAnimationDelay;
                    itemAnimPanels[i].AnimatedBackgroundTextures = foregroundAnimationHandler(item);
                }
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
        void UpdateListScrollerButtons(int index, int count)
        {
            // Update up button
            itemListUpButton.BackgroundTexture = (index > 0) ? greenUpArrow : redUpArrow;

            // Update down button
            itemListDownButton.BackgroundTexture = (index < (count - listDisplayUnits)) ? greenDownArrow : redDownArrow;

            // No items above or below
            if (count <= listDisplayUnits)
            {
                itemListUpButton.BackgroundTexture = redUpArrow;
                itemListDownButton.BackgroundTexture = redDownArrow;
            }
        }

        void LoadTextures(bool enhanced)
        {
            // Cut out red up/down arrows
            Texture2D redArrowsTexture = ImageReader.GetTexture(redArrowsTextureName);
            redUpArrow = ImageReader.GetSubTexture(redArrowsTexture, upArrowRect, arrowsFullSize);
            redDownArrow = ImageReader.GetSubTexture(redArrowsTexture, downArrowRect, arrowsFullSize);

            // Cut out green up/down arrows
            Texture2D greenArrowsTexture = ImageReader.GetTexture(greenArrowsTextureName);
            greenUpArrow = ImageReader.GetSubTexture(greenArrowsTexture, upArrowRect, arrowsFullSize);
            greenDownArrow = ImageReader.GetSubTexture(greenArrowsTexture, downArrowRect, arrowsFullSize);

            if (enhanced)
            {
                itemListTextures = new Texture2D[listDisplayUnits];
                Texture2D baseInvTexture = ImageReader.GetTexture(baseInvTextureName);
                for (int i = 0; i < itemCutoutRects16.Length; i++)
                    itemListTextures[i] = ImageReader.GetSubTexture(baseInvTexture, itemCutoutRects16[i], new DFSize(320, 200));
            }
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

            ItemButton_OnMouseEnter(sender);
        }

        void ItemButton_OnMouseEnter(BaseScreenComponent sender)
        {
            // Get index
            int index = itemListScrollBar.ScrollIndex + (int)sender.Tag;
            if (index >= items.Count)
                return;

            // Get item and raise item hover event
            DaggerfallUnityItem item = items[index];
            if (item != null && OnItemHover != null)
                OnItemHover(item);
        }

        void ItemsScrollBar_OnScroll()
        {
            UpdateItemsDisplay();
        }

        void ItemsUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            itemListScrollBar.ScrollIndex -= scrollNum;
        }

        void ItemsDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            itemListScrollBar.ScrollIndex += scrollNum;
        }

        void ItemsListPanel_OnMouseScrollUp(BaseScreenComponent sender)
        {
            itemListScrollBar.ScrollIndex -= scrollNum;
        }

        void ItemsListPanel_OnMouseScrollDown(BaseScreenComponent sender)
        {
            itemListScrollBar.ScrollIndex += scrollNum;
        }

        #endregion
    }
}