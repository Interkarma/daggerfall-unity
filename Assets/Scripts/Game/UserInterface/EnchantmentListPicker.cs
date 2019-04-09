// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class EnchantmentListPicker : Panel
    {
        #region Fields

        const int panelSpacing = 15;
        const int visiblePanels = 8;
        const int scrollerWidth = 4;
        const int scrollerStep = 4;
        const int panelPosVerticalStartingOffset = 2;
        const string secondarySpacing = "  ";

        Panel listPanel = new Panel();
        List<EnchantmentPanel> enchantmentsPanels = new List<EnchantmentPanel>();
        VerticalScrollBar scroller = new VerticalScrollBar();

        #endregion

        #region Constructors

        public EnchantmentListPicker()
        {
            Components.Add(listPanel);
            Components.Add(scroller);

            listPanel.OnMouseClick += ListPanel_OnMouseClick;
            scroller.OnScroll += Scroller_OnScroll;
        }

        #endregion

        #region Public Methods

        public void AddEnchantment(EnchantmentSettings enchantment)
        {
            EnchantmentPanel panel = new EnchantmentPanel(enchantment, new Rect(0, 0, GetRenderWidth(), InteriorHeight));
            enchantmentsPanels.Add(panel);
            RefreshPanelLayout();
        }

        public void AddEnchantments(EnchantmentSettings[] enchantments)
        {
            foreach(EnchantmentSettings enchantment in enchantments)
            {
                EnchantmentPanel panel = new EnchantmentPanel(enchantment, new Rect(0, 0, GetRenderWidth(), InteriorHeight));
                enchantmentsPanels.Add(panel);
            }
            RefreshPanelLayout();
        }

        public void ClearEnchantments()
        {
            enchantmentsPanels.Clear();
        }

        public EnchantmentSettings[] GetEnchantments()
        {
            List<EnchantmentSettings> enchantments = new List<EnchantmentSettings>();
            foreach(EnchantmentPanel enchantmentPanel in enchantmentsPanels)
            {
                enchantments.Add(enchantmentPanel.Enchantment);
            }

            return enchantments.ToArray();
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            base.Update();
            listPanel.Size = Size;
        }

        protected override void MouseScrollUp()
        {
            base.MouseScrollUp();
            scroller.ScrollIndex -= scrollerStep;
        }

        protected override void MouseScrollDown()
        {
            base.MouseScrollDown();
            scroller.ScrollIndex += scrollerStep;
        }

        #endregion

        #region Private Methods

        void RefreshPanelLayout()
        {
            listPanel.Components.Clear();

            // Enchantment UI has no dedicated area for a scroller
            // Using a slim scroller and fitting child panel area as required so not overlapping with scroller rect
            if (ShowScroller())
            {
                scroller.Enabled = true;
                scroller.Position = new Vector2(InteriorWidth - scrollerWidth, 0);
                scroller.Size = new Vector2(scrollerWidth, InteriorHeight);
                scroller.TotalUnits = enchantmentsPanels.Count * panelSpacing;
                scroller.DisplayUnits = InteriorHeight;
            }
            else
            {
                scroller.Enabled = false;
            }

            Vector2 panelPos = new Vector2(0, panelPosVerticalStartingOffset);
            foreach(EnchantmentPanel enchantmentPanel in enchantmentsPanels)
            {
                listPanel.Components.Add(enchantmentPanel);
                enchantmentPanel.Position = panelPos;
                enchantmentPanel.FitToScroller = scroller.Enabled;
                panelPos.y += panelSpacing;
            }
        }

        int GetRenderWidth()
        {
            return ShowScroller() ? InteriorWidth - scrollerWidth : InteriorWidth;
        }

        bool ShowScroller()
        {
            return enchantmentsPanels.Count > visiblePanels;
        }

        #endregion

        #region Event Handlers

        private void ListPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Find enchantment panel under mouse
            EnchantmentPanel panelToRemove = null;
            foreach (EnchantmentPanel panel in enchantmentsPanels)
            {
                if (panel.MouseOverPanel)
                {
                    panelToRemove = panel;
                    break;
                }
            }

            // Remove panel under mouse and reset list scroll position
            if (panelToRemove != null)
            {
                scroller.ScrollIndex = 0;
                enchantmentsPanels.Remove(panelToRemove);
                RefreshPanelLayout();
            }
        }

        private void Scroller_OnScroll()
        {
            Vector2 panelPos = new Vector2(0, panelPosVerticalStartingOffset - scroller.ScrollIndex);
            foreach (EnchantmentPanel panel in enchantmentsPanels)
            {
                panel.Position = panelPos;
                panelPos.y += panelSpacing;
            }
        }

        #endregion

        #region Enchantment Panel

        public class EnchantmentPanel : Panel
        {
            Vector2 panelSizeWithoutScroller = new Vector2(75, 10);
            Vector2 panelSizeWithScroller = new Vector2(71, 10);
            Vector2 secondaryLabelPos = new Vector2(0, 5);
            TextLabel primaryLabel, secondaryLabel;
            bool lastFitToScroller;

            public Color textColor = DaggerfallUI.DaggerfallDefaultTextColor;
            public Color highlightedTextColor = DaggerfallUI.DaggerfallAlternateHighlightTextColor;

            public bool MouseOverPanel { get; private set; }
            public bool FitToScroller { get; set; }
            public EnchantmentSettings Enchantment { get; set; }
            Rect RenderArea { get; set; }

            public EnchantmentPanel(EnchantmentSettings enchantment, Rect renderArea)
            {
                Size = panelSizeWithoutScroller;
                primaryLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.SmallFont, Vector2.zero, enchantment.PrimaryDisplayName, this);
                secondaryLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.SmallFont, secondaryLabelPos, secondarySpacing + enchantment.SecondaryDisplayName, this);
                primaryLabel.RestrictedRenderAreaCoordinateType = secondaryLabel.RestrictedRenderAreaCoordinateType = TextLabel.RestrictedRenderArea_CoordinateType.DaggerfallNativeCoordinates;
                primaryLabel.RectRestrictedRenderArea = secondaryLabel.RectRestrictedRenderArea = renderArea;
                primaryLabel.TextColor = secondaryLabel.TextColor = textColor;

                OnMouseEnter += EnchantmentPanel_OnMouseEnter;
                OnMouseLeave += EnchantmentPanel_OnMouseLeave;

                RenderArea = renderArea;
                Enchantment = enchantment;
            }

            public override void Update()
            {
                base.Update();

                if (lastFitToScroller != FitToScroller)
                { 
                    Size = (FitToScroller) ? panelSizeWithScroller : panelSizeWithoutScroller;
                    lastFitToScroller = FitToScroller;
                }
            }

            bool PanelInRenderArea()
            {
                return RenderArea.Overlaps(new Rect(Position.x, Position.y, Size.x, Size.y));
            }

            private void EnchantmentPanel_OnMouseEnter(BaseScreenComponent sender)
            {
                if (PanelInRenderArea())
                {
                    MouseOverPanel = true;
                    primaryLabel.TextColor = highlightedTextColor;
                    secondaryLabel.TextColor = highlightedTextColor;
                }
            }

            private void EnchantmentPanel_OnMouseLeave(BaseScreenComponent sender)
            {
                if (PanelInRenderArea())
                {
                    MouseOverPanel = false;
                    primaryLabel.TextColor = textColor;
                    secondaryLabel.TextColor = textColor;
                }
            }
        }

        #endregion
    }
}