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

        int listDisplayUnits = 4;   // Number of item rows displayed in scrolling areas
        int listWidth = 1;          // Number of items on each row
        int listDisplayTotal;       // Total number of items displayed in scrolling areas
        int itemButtonMargin = 2;   // Margin of item buttons
        float textScale = 1f;       // Scale of text on item buttons
        bool scroller = true;       // Scroller active or not
        int miscLabelOffsetDist = 0;// Vertical distance to offset the misc label
        int miscLabelOffsetIdx = 0; //Index of column for which to offset the misc label

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
        /// <summary>Handler for misc label text (defaults to top left)</summary>
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
                UpdateItemsDisplay(true);
            }
        }

        #endregion

        #region Constructors, Public methods

        /// <summary>
        /// Initializes a new instance of the ItemListScroller using default coordinates preset for use on DF inventory screens.
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
                listDisplayUnits = 8;
                listWidth = 2;
                itemButtonRects = itemButtonRects16;
                itemButtonMargin = 1;
                textScale = 0.75f;
            }
            listDisplayTotal = listDisplayUnits * listWidth;
            TextLabel miscLabelTemplate = new TextLabel()
            {
                Position = Vector2.zero,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextScale = textScale
            };

            LoadTextures(enhanced);
            SetupScrollBar();
            SetupScrollButtons();
            SetupItemsList(enhanced, miscLabelTemplate);
        }

        /// <summary>
        /// Initializes a fully customised new instance of ItemListScroller.
        /// </summary>
        /// <param name="listRows">Number of rows of items this list will display at one time.</param>
        /// <param name="listCols">Number of items displayed per row.</param>
        /// <param name="itemListRect">Item list coordinate rect, excluding scrollbar.</param>
        /// <param name="itemsRects">Individual items display coordinate rects. (1 per width*height)</param>
        /// <param name="miscLabelTemplate">Template for misc label: relative position, font, horiz & vert alignment, text scale. (defaults: Vector2.zero, Font4, Left, Top, 1)</param>
        /// <param name="toolTip">Tool tip class to use if items should display tooltips.</param>
        /// <param name="itemMarginSize">Individual item display margin size.</param>
        /// <param name="textScale">Text scale factor for stack labels.</param>
        /// <param name="scroll">True for a scrollable list, false otherwise.</param>
        /// <param name="miscLabelOffsetDist">Vertical distance to offset the misc label, 0 to disable.</param>
        /// <param name="miscLabelOffsetIdx">Index of column for which to offset the misc label.</param>
        public ItemListScroller(int listRows, int listCols, Rect itemListRect, Rect[] itemsRects, TextLabel miscLabelTemplate, ToolTip toolTip = null,
                                int itemMarginSize = 1, float textScale = 1f, bool scroll = true, int miscLabelOffsetDist = 0, int miscLabelOffsetIdx = 0)
            : base()
        {
            listDisplayTotal = listRows * listCols;
            if (itemsRects.Length != listDisplayTotal)
                throw new ArgumentException();

            listDisplayUnits = listRows;
            listWidth = listCols;
            itemListPanelRect = itemListRect;
            itemButtonRects = itemsRects;
            itemButtonMargin = itemMarginSize;
            this.toolTip = toolTip;
            this.textScale = textScale;
            scroller = scroll;
            this.miscLabelOffsetDist = miscLabelOffsetDist;
            this.miscLabelOffsetIdx = miscLabelOffsetIdx;

            LoadTextures(false);
            if (scroller) {
                SetupScrollBar();
                SetupScrollButtons();
            }
            SetupItemsList(false, miscLabelTemplate);
        }

        public void ResetScroll()
        {
            if (scroller)
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
                Size = new Vector2(6, itemListPanelRect.height - 35),
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
                Position = new Vector2(0, itemListPanelRect.height - 16),
                Size = new Vector2(9, 16),
                BackgroundTexture = redDownArrow
            };
            Components.Add(itemListDownButton);
            itemListDownButton.OnMouseClick += ItemsDownButton_OnMouseClick;
        }

        void SetupItemsList(bool enhanced, TextLabel miscLabelTemplate)
        {
            // List panel for scrolling behaviour
            Panel itemsListPanel = DaggerfallUI.AddPanel(itemListPanelRect, this);
            itemsListPanel.OnMouseScrollUp += ItemsListPanel_OnMouseScrollUp;
            itemsListPanel.OnMouseScrollDown += ItemsListPanel_OnMouseScrollDown;
            itemsListPanel.OnMouseLeave += ItemsListPanel_OnMouseLeave;

            // Setup buttons
            itemButtons = new Button[listDisplayTotal];
            itemIconPanels = new Panel[listDisplayTotal];
            itemAnimPanels = new Panel[listDisplayTotal];
            itemStackLabels = new TextLabel[listDisplayTotal];
            itemMiscLabels = new TextLabel[listDisplayTotal];

            // Setup column misc label offsetting.
            Vector2 offsetPosition = miscLabelTemplate.Position + new Vector2(0, miscLabelOffsetDist);
            int osi = miscLabelOffsetIdx;

            for (int i = 0; i < listDisplayTotal; i++)
            {
                // Panel - for backing button in enhanced mode
                if (enhanced) {
                    Panel buttonPanel = DaggerfallUI.AddPanel(itemButtonRects[i], itemsListPanel);
                    buttonPanel.BackgroundTexture = itemListTextures[i];
                }
                // Buttons (also handle highlight colours)
                itemButtons[i] = DaggerfallUI.AddButton(itemButtonRects[i], itemsListPanel);
                itemButtons[i].SetMargins(Margins.All, itemButtonMargin);
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
                Vector2 position = miscLabelTemplate.Position;
                if (miscLabelOffsetDist != 0 && i == osi)
                {
                    position = offsetPosition;
                    osi += listWidth;
                }
                itemMiscLabels[i] = DaggerfallUI.AddTextLabel(miscLabelTemplate.Font, position, string.Empty, itemButtons[i]);
                itemMiscLabels[i].HorizontalAlignment = miscLabelTemplate.HorizontalAlignment;
                itemMiscLabels[i].VerticalAlignment = miscLabelTemplate.VerticalAlignment;
                itemMiscLabels[i].TextScale = miscLabelTemplate.TextScale;
            }
        }

        void ClearItemsList()
        {
            for (int i = 0; i < listDisplayTotal; i++)
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
            if (scroller)
            {
                itemListUpButton.BackgroundTexture = redUpArrow;
                itemListDownButton.BackgroundTexture = redDownArrow;
            }
        }

        void UpdateItemsDisplay(bool delayScrollUp)
        {
            // Clear list elements
            ClearItemsList();
            if (items == null)
                return;

            int scrollIndex = 0;
            if (scroller)
            {
                // Update scrollbar
                int rows = (items.Count + listWidth - 1) / listWidth;
                itemListScrollBar.TotalUnits = rows;
                scrollIndex = GetSafeScrollIndex(itemListScrollBar, delayScrollUp);

                // Update scroller buttons
                UpdateListScrollerButtons(scrollIndex, rows);

                // Convert scroller index to item based scroll index
                scrollIndex *= listWidth;
            }
            // Update images and tooltips
            for (int i = 0; i < listDisplayTotal; i++)
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

        int GetSafeScrollIndex(VerticalScrollBar scroller, bool delayScrollUp)
        {
            // Get current scroller index
            int scrollIndex = scroller.ScrollIndex;
            if (scrollIndex < 0)
                scrollIndex = 0;

            // Ensure scroll index within current range
            if (!delayScrollUp && scrollIndex + scroller.DisplayUnits > scroller.TotalUnits || scrollIndex >= scroller.TotalUnits)
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
            if (scroller)
            {
                // Cut out red up/down arrows
                Texture2D redArrowsTexture = ImageReader.GetTexture(redArrowsTextureName);
                redUpArrow = ImageReader.GetSubTexture(redArrowsTexture, upArrowRect, arrowsFullSize);
                redDownArrow = ImageReader.GetSubTexture(redArrowsTexture, downArrowRect, arrowsFullSize);

                // Cut out green up/down arrows
                Texture2D greenArrowsTexture = ImageReader.GetTexture(greenArrowsTextureName);
                greenUpArrow = ImageReader.GetSubTexture(greenArrowsTexture, upArrowRect, arrowsFullSize);
                greenDownArrow = ImageReader.GetSubTexture(greenArrowsTexture, downArrowRect, arrowsFullSize);
            }
            if (enhanced)
            {
                itemListTextures = new Texture2D[listDisplayTotal];
                Texture2D baseInvTexture = ImageReader.GetTexture(baseInvTextureName);
                for (int i = 0; i < itemCutoutRects16.Length; i++)
                    itemListTextures[i] = ImageReader.GetSubTexture(baseInvTexture, itemCutoutRects16[i], new DFSize(320, 200));
            }
        }

        private int GetScrollIndex()
        {
            return (scroller) ? itemListScrollBar.ScrollIndex : 0;
        }

        #endregion

        #region Event handlers

        void ItemButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Get index
            int index = (GetScrollIndex() * listWidth) + (int)sender.Tag;
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
            int index = (GetScrollIndex() * listWidth) + (int)sender.Tag;
            if (index >= items.Count)
                return;

            // Get item and raise item hover event
            DaggerfallUnityItem item = items[index];
            if (item != null && OnItemHover != null)
                OnItemHover(item);
        }

        void ItemsScrollBar_OnScroll()
        {
            UpdateItemsDisplay(false);
        }

        void ItemsUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (scroller)
                itemListScrollBar.ScrollIndex--;
        }

        void ItemsDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (scroller)
                itemListScrollBar.ScrollIndex++;
        }

        void ItemsListPanel_OnMouseScrollUp(BaseScreenComponent sender)
        {
            if (scroller)
                itemListScrollBar.ScrollIndex--;
        }

        void ItemsListPanel_OnMouseScrollDown(BaseScreenComponent sender)
        {
            if (scroller)
                itemListScrollBar.ScrollIndex++;
        }

        void ItemsListPanel_OnMouseLeave(BaseScreenComponent sender)
        {
            UpdateItemsDisplay(false);
        }

        #endregion
    }
}