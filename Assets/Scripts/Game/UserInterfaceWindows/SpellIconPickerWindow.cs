// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Daggerfall Unity spell icon picker interface.
    /// </summary>
    public class SpellIconPickerWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        protected Vector2 mainPanelSize = new Vector2(274, 180);
        protected Vector2 scrollingPanelPosition = new Vector2(2, 2);
        protected Vector2 scrollingPanelSize = new Vector2(262, 176);
        protected Vector2 scrollerPosition = new Vector2(265, 2);
        protected Vector2 scrollerSize = new Vector2(8, 176);

        #endregion

        #region UI Controls

        protected Panel mainPanel = new Panel();
        protected ScrollingPanel scrollingPanel = new ScrollingPanel();
        protected VerticalScrollBar scroller = new VerticalScrollBar();

        #endregion

        #region Fields

        protected SpellIcon? selectedIcon = null;

        #endregion

        #region Properties

        public SpellIcon? SelectedIcon
        {
            get { return selectedIcon; }
        }

        #endregion

        #region Constructors

        public SpellIconPickerWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Main panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.Size = mainPanelSize;
            mainPanel.Outline.Enabled = true;
            mainPanel.BackgroundColor = Color.black;
            NativePanel.Components.Add(mainPanel);

            // Scrolling panel
            scrollingPanel.Position = scrollingPanelPosition;
            scrollingPanel.Size = scrollingPanelSize;
            scrollingPanel.BackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            scrollingPanel.ScrollTransform = iconSpacing;
            scrollingPanel.OnMouseScrollUp += ScrollingPanel_OnMouseScrollUp;
            scrollingPanel.OnMouseScrollDown += ScrollingPanel_OnMouseScrollDown;
            scrollingPanel.OnMouseMove += ScrollingPanel_OnMouseMove;
            scrollingPanel.OnMouseClick += ScrollingPanel_OnMouseClick;
            mainPanel.Components.Add(scrollingPanel);

            // Scroller
            scroller.Position = scrollerPosition;
            scroller.Size = scrollerSize;
            scroller.OnScroll += Scroller_OnScroll;
            mainPanel.Components.Add(scroller);

            // TEST
            int xpos = 2, ypos = 2;
            AddIconPacks(scrollingPanel, ref xpos, ref ypos);
        }

        public override void OnPush()
        {
            base.OnPush();
        }

        public override void CancelWindow()
        {
            selectedIcon = null;
            base.CancelWindow();
        }

        #endregion

        #region Public Methods

        public void ResetScrollPosition()
        {
            scroller.ScrollIndex = 0;
        }

        #endregion

        #region Private Methods

        const int iconsPerRow = 12;
        const int iconSize = 16;
        const int iconSpacing = 22;

        protected virtual void AddIconPacks(ScrollingPanel parent, ref int xpos, ref int ypos)
        {
            int rowCount = 0;
            int startX = xpos;

            SpellIconCollection iconCollection = DaggerfallUI.Instance.SpellIconCollection;

            // TODO: Add suggested icons (if any)

            // Add spell icon collections
            foreach (var kvp in iconCollection.SpellIconPacks)
            {
                string key = kvp.Key;
                SpellIconCollection.SpellIconPack pack = kvp.Value;
                if (pack.icons == null || pack.iconCount == 0)
                    continue;

                AddHeaderLabel(parent, ref xpos, ref ypos, pack.displayName);

                rowCount = 0;
                for (int i = 0; i < pack.iconCount; i++)
                {
                    AddIcon(iconCollection, pack, key, i, parent, ref rowCount, ref startX, ref xpos, ref ypos);
                }

                // Start from a new row after an icon pack
                xpos = startX;
                ypos += iconSpacing;
            }

            // Add classic icons
            AddHeaderLabel(parent, ref xpos, ref ypos, TextManager.Instance.GetLocalizedText("classicIcons"));
            rowCount = 0;
            for (int i = 0; i < iconCollection.SpellIconCount; i++)
            {
                AddIcon(iconCollection, null, null, i, parent, ref rowCount, ref startX, ref xpos, ref ypos);
            }

            // Assign total scroll steps in scrolling panel
            parent.ScrollSteps = ypos / iconSpacing + 1;
            scroller.DisplayUnits = parent.InteriorHeight / iconSpacing;
            scroller.TotalUnits = parent.ScrollSteps;
        }

        protected virtual void AddIcon(SpellIconCollection iconCollection, SpellIconCollection.SpellIconPack pack, string key, int index, Panel parent, ref int rowCount, ref int startX, ref int xpos, ref int ypos)
        {
            // Get pack or classic texture
            Texture2D texture;
            if (pack == null)
                texture = iconCollection.GetSpellIcon(index);
            else
                texture = pack.icons[index].texture;

            // Add image panel
            Panel panel = new Panel();
            panel.BackgroundTexture = texture;
            panel.Position = new Vector2(xpos, ypos);
            panel.Size = new Vector2(iconSize, iconSize);
            parent.Components.Add(panel);
            xpos += iconSpacing;
            if (++rowCount >= iconsPerRow)
            {
                xpos = startX;
                ypos += iconSpacing;
                rowCount = 0;
            }

            // Tag panel for selection, a null key will fallback to classic using index
            panel.Tag = new SpellIcon()
            {
                key = key,
                index = index,
            };
        }

        protected virtual void AddHeaderLabel(Panel parent, ref int xpos, ref int ypos, string text)
        {
            TextLabel header = new TextLabel();
            header.Text = text;
            header.Font = DaggerfallUI.LargeFont;
            header.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            header.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
            header.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
            header.Position = new Vector2(xpos, ypos + 4);
            header.Size = new Vector2(0, 22);
            parent.Components.Add(header);
            ypos += iconSpacing;
        }

        protected virtual void UpdateSelectedIcon()
        {
            bool mouseOverIcon = false;

            Vector2 mousePos = scrollingPanel.ScaledMousePosition;
            Vector2 srollOffset = new Vector2(0, -scrollingPanel.ScrollIndex * scrollingPanel.ScrollTransform);
            foreach (BaseScreenComponent component in scrollingPanel.Components)
            {
                if (component is Panel)
                {
                    Rect rect = new Rect(component.Position + srollOffset, component.Size);
                    if (rect.Contains(mousePos))
                    {
                        (component as Panel).Outline.Enabled = true;
                        selectedIcon = (SpellIcon)component.Tag;
                        mouseOverIcon = true;
                    }
                    else
                    {
                        (component as Panel).Outline.Enabled = false;
                    }
                }
            }

            if (!mouseOverIcon)
                selectedIcon = null;
        }

        protected virtual void Scroller_OnScroll()
        {
            scrollingPanel.ScrollIndex = scroller.ScrollIndex;
            UpdateSelectedIcon();
        }

        protected virtual void ScrollingPanel_OnMouseScrollDown(BaseScreenComponent sender)
        {
            scroller.ScrollIndex++;
            UpdateSelectedIcon();
        }

        protected virtual void ScrollingPanel_OnMouseScrollUp(BaseScreenComponent sender)
        {
            scroller.ScrollIndex--;
            UpdateSelectedIcon();
        }

        protected virtual void ScrollingPanel_OnMouseMove(int x, int y)
        {
            UpdateSelectedIcon();
        }

        protected virtual void ScrollingPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            UpdateSelectedIcon();
            if (selectedIcon != null)
                CloseWindow();
        }

        #endregion

        #region Scrolling Panel Control

        /// <summary>
        /// Allow for vertically scrolling panel layouts.
        /// Only used by spell icon picker at this time with a design specific for that UI.
        /// Might split out later with upgrades to use as a generic control.
        /// </summary>
        public class ScrollingPanel : Panel
        {
            int scrollTransform = 16;
            int scrollSteps = 0;
            int scrollIndex = 0;

            /// <summary>
            /// Gets or sets the number of pixels scrolled per step.
            /// Without good clipping/scissoring support for panel-in-panel, DagUI cannot smoothly scroll ad-hoc items in a panel.
            /// This property aligns all scrolling to a fixed partition so that items are either wholly inside or outside of display area.
            /// Works best with lots of smaller items such as scrolling icons.
            /// </summary>
            public int ScrollTransform
            {
                get { return scrollTransform; }
                set { scrollTransform = value; }
            }

            /// <summary>
            /// Gets or 
            /// </summary>
            public int ScrollSteps
            {
                get { return scrollSteps; }
                set { scrollSteps = value; }
            }

            public int ScrollIndex
            {
                get { return scrollIndex; }
                set { SetScrollIndex(value); }
            }

            void SetScrollIndex(int value)
            {
                scrollIndex = Mathf.Clamp(value, 0, scrollSteps);
            }

            /// <summary>
            /// Custom override to draw clipped scrolling items.
            /// </summary>
            public override void Draw()
            {
                if (!Enabled)
                    return;

                Rect myRect = new Rect(Position, Size);
                foreach (BaseScreenComponent component in Components)
                {
                    if (component.Enabled)
                    {
                        // Store position and adjust for scroll index
                        Vector2 pos = component.Position;
                        component.Position += new Vector2(0, -scrollIndex * scrollTransform);

                        // Draw component if not outside of parent area
                        if (myRect.Contains(component.Position))
                            component.Draw();

                        // Restore saved position
                        component.Position = pos;
                    }
                }
            }
        }

        #endregion
    }
}