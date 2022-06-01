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

using UnityEngine;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class EnchantmentListPicker : Panel
    {
        #region Fields

        const int panelSpacing = 5;
        const int visiblePanels = 7;
        const int scrollerWidth = 4;
        const int scrollerStep = 8;
        const int panelPosVerticalStartingOffset = 2;
        const string secondarySpacing = "  ";

        Panel listPanel = new Panel();
        List<EnchantmentPanel> enchantmentPanels = new List<EnchantmentPanel>();
        VerticalScrollBar scroller = new VerticalScrollBar();

        public delegate void OnRefreshListEventHandler(EnchantmentListPicker sender);
        public event OnRefreshListEventHandler OnRefreshList;

        public delegate void OnRemoveItemEventHandler(EnchantmentPanel panel);
        public event OnRemoveItemEventHandler OnRemoveItem;

        #endregion

        #region Properties

        public int EnchantmentCount
        {
            get { return enchantmentPanels.Count; }
        }

        #endregion

        #region Constructors

        public EnchantmentListPicker()
        {
            Components.Add(listPanel);
            Components.Add(scroller);

            scroller.OnScroll += Scroller_OnScroll;
        }

        #endregion

        #region Public Methods

        public void AddEnchantment(EnchantmentSettings enchantment)
        {
            EnchantmentPanel panel = new EnchantmentPanel(enchantment, new Rect(0, 0, GetRenderWidth(), InteriorHeight));
            if (enchantment.ParentEnchantment != 0)
            {
                panel.TextColor = DaggerfallUI.DaggerfallForcedEnchantmentTextColor;
                panel.HighlightedTextColor = DaggerfallUI.DaggerfallForcedEnchantmentTextColor;
            }
            panel.OnMouseClick += EnchantmentPanel_OnMouseClick;
            panel.SetRestrictedRenderingPanel(this);
            enchantmentPanels.Add(panel);
            RefreshPanelLayout();
        }

        public void AddEnchantments(EnchantmentSettings[] enchantments)
        {
            foreach(EnchantmentSettings enchantment in enchantments)
            {
                EnchantmentPanel panel = new EnchantmentPanel(enchantment, new Rect(0, 0, GetRenderWidth(), InteriorHeight));
                if (enchantment.ParentEnchantment != 0)
                {
                    panel.TextColor = DaggerfallUI.DaggerfallForcedEnchantmentTextColor;
                    panel.HighlightedTextColor = DaggerfallUI.DaggerfallForcedEnchantmentTextColor;
                }
                panel.SetRestrictedRenderingPanel(this);
                enchantmentPanels.Add(panel);
            }
            RefreshPanelLayout();
        }

        public void ClearEnchantments()
        {
            foreach(EnchantmentPanel panel in enchantmentPanels)
            {
                panel.OnMouseClick -= EnchantmentPanel_OnMouseClick;
            }
            enchantmentPanels.Clear();
            listPanel.Components.Clear();
            scroller.ScrollIndex = 0;
        }

        public EnchantmentSettings[] GetEnchantments()
        {
            List<EnchantmentSettings> enchantments = new List<EnchantmentSettings>();
            foreach(EnchantmentPanel enchantmentPanel in enchantmentPanels)
            {
                enchantments.Add(enchantmentPanel.Enchantment);
            }

            return enchantments.ToArray();
        }

        public bool ContainsEnchantment(EnchantmentSettings other)
        {
            foreach(EnchantmentPanel panel in enchantmentPanels)
            {
                if (panel.Enchantment == other)
                    return true;
            }

            return false;
        }

        public bool ContainsEnchantmentKey(string effectKey)
        {
            foreach (EnchantmentPanel panel in enchantmentPanels)
            {
                if (panel.Enchantment.EffectKey == effectKey)
                    return true;
            }

            return false;
        }

        public int GetTotalEnchantmentCost(bool countForcedEnchantments)
        {
            int cost = 0;
            foreach(EnchantmentPanel panel in enchantmentPanels)
            {
                // Forced enchantments do not contribute to cost unless specified
                if (!countForcedEnchantments && panel.Enchantment.ParentEnchantment != 0)
                    continue;

                cost += panel.Enchantment.EnchantCost;
            }

            return cost;
        }

        public void RemoveForcedEnchantments(int parentHash)
        {
            // Find child panels linked with parent hash
            List<EnchantmentPanel> panelsToRemove = new List<EnchantmentPanel>();
            foreach (EnchantmentPanel panel in enchantmentPanels)
            {
                if (panel.Enchantment.ParentEnchantment == parentHash)
                    panelsToRemove.Add(panel);
            }

            // Remove child panels
            foreach (EnchantmentPanel panel in panelsToRemove)
            {
                RemoveEnchantment(panel);
            }
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
            if (!ShowScroller())
                return;

            base.MouseScrollUp();
            scroller.ScrollIndex -= scrollerStep;
        }

        protected override void MouseScrollDown()
        {
            if (!ShowScroller())
                return;

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
                scroller.DisplayUnits = InteriorHeight;
            }
            else
            {
                scroller.Enabled = false;
            }

            int scrollerUnits = 0;
            Rect myRect = Rectangle;
            Vector2 panelPos = new Vector2(0, panelPosVerticalStartingOffset);
            foreach(EnchantmentPanel enchantmentPanel in enchantmentPanels)
            {
                listPanel.Components.Add(enchantmentPanel);
                enchantmentPanel.Position = panelPos;
                enchantmentPanel.FitToScroller = scroller.Enabled;
                panelPos.y += enchantmentPanel.Size.y + panelSpacing;
                scrollerUnits += (int)(enchantmentPanel.Size.y + panelSpacing);
                enchantmentPanel.Enabled = myRect.Overlaps(enchantmentPanel.Rectangle);
            }
            scroller.TotalUnits = scrollerUnits;

            // Raise event so any UI using this control knows when list has changed
            if (OnRefreshList != null)
                OnRefreshList(this);
        }

        int GetRenderWidth()
        {
            return ShowScroller() ? InteriorWidth - scrollerWidth : InteriorWidth;
        }

        bool ShowScroller()
        {
            return enchantmentPanels.Count > visiblePanels;
        }

        void RemoveEnchantment(EnchantmentPanel panelToRemove)
        {
            panelToRemove.OnMouseClick -= EnchantmentPanel_OnMouseClick;
            enchantmentPanels.Remove(panelToRemove);
            scroller.ScrollIndex = 0;
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            RefreshPanelLayout();
        }

        #endregion

        #region Event Handlers

        private void EnchantmentPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Mouse must be within control area
            if (!MouseOverComponent)
                return;

            // Can only click to remove parent panels, child panels are removed by clicking on parent
            EnchantmentPanel panelToRemove = (EnchantmentPanel)sender;
            if (panelToRemove.Enchantment.ParentEnchantment == 0)
            {
                RemoveEnchantment(panelToRemove);
                if (OnRemoveItem != null)
                    OnRemoveItem(panelToRemove);
            }
        }

        private void Scroller_OnScroll()
        {
            if (!ShowScroller())
                return;

            Rect myRect = Rectangle;
            Vector2 panelPos = new Vector2(0, panelPosVerticalStartingOffset - scroller.ScrollIndex);
            foreach (EnchantmentPanel panel in enchantmentPanels)
            {
                panel.Position = panelPos;
                panelPos.y += panel.Size.y + panelSpacing;
                panel.Enabled = myRect.Overlaps(panel.Rectangle);
            }
        }

        #endregion

        #region Enchantment Panel

        public class EnchantmentPanel : Panel
        {
            const float panelWidthWithoutScroller = 75;
            const float panelWidthWithScroller = 71;
            const float panelHeightWithoutSecondary = 7;
            const float panelHeightWithSecondary = 12;

            Vector2 primaryLabelPos = new Vector2(0, 2);
            Vector2 secondaryLabelPos = new Vector2(0, 8);
            TextLabel primaryLabel, secondaryLabel;
            bool lastFitToScroller;

            Color textColor = DaggerfallUI.DaggerfallDefaultTextColor;
            Color highlightedTextColor = DaggerfallUI.DaggerfallAlternateHighlightTextColor;

            public bool FitToScroller { get; set; }
            public EnchantmentSettings Enchantment { get; set; }
            Rect RenderArea { get; set; }

            public Color TextColor
            {
                get { return textColor; }
                set { SetTextColor(value); }
            }

            public Color HighlightedTextColor
            {
                get { return highlightedTextColor; }
                set { SetHighlightedTextColor(value); }
            }

            public EnchantmentPanel(EnchantmentSettings enchantment, Rect renderArea)
            {
                bool hasSecondaryLabel = !string.IsNullOrEmpty(enchantment.SecondaryDisplayName);
                Size = new Vector2(panelWidthWithoutScroller, hasSecondaryLabel ? panelHeightWithSecondary : panelHeightWithoutSecondary);

                primaryLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.SmallFont, primaryLabelPos, enchantment.PrimaryDisplayName, this);
                secondaryLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.SmallFont, secondaryLabelPos, secondarySpacing + enchantment.SecondaryDisplayName, this);
                primaryLabel.TextColor = secondaryLabel.TextColor = textColor;

                if (!hasSecondaryLabel)
                    secondaryLabel.Enabled = false;

                OnMouseEnter += EnchantmentPanel_OnMouseEnter;
                OnMouseLeave += EnchantmentPanel_OnMouseLeave;

                RenderArea = renderArea;
                Enchantment = enchantment;
            }

            public void SetRestrictedRenderingPanel(Panel panel)
            {
                primaryLabel.RestrictedRenderAreaCoordinateType = secondaryLabel.RestrictedRenderAreaCoordinateType = RestrictedRenderArea_CoordinateType.CustomParent;
                primaryLabel.RestrictedRenderAreaCustomParent = secondaryLabel.RestrictedRenderAreaCustomParent = panel;
            }

            void SetTextColor(Color color)
            {
                textColor = primaryLabel.TextColor = secondaryLabel.TextColor = color;
            }

            void SetHighlightedTextColor(Color color)
            {
                highlightedTextColor = color;
            }

            public override void Update()
            {
                base.Update();

                if (lastFitToScroller != FitToScroller)
                { 
                    Size = new Vector2(FitToScroller ? panelWidthWithScroller : panelWidthWithoutScroller, Size.y);
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
                    primaryLabel.TextColor = highlightedTextColor;
                    secondaryLabel.TextColor = highlightedTextColor;
                }
            }

            private void EnchantmentPanel_OnMouseLeave(BaseScreenComponent sender)
            {
                if (PanelInRenderArea())
                {
                    primaryLabel.TextColor = textColor;
                    secondaryLabel.TextColor = textColor;
                }
            }
        }

        #endregion
    }
}