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

        const int panelSpacing = 5;
        const int visiblePanels = 8;
        const int scrollerWidth = 4;
        const int scrollerStep = 8;
        const int panelPosVerticalStartingOffset = 2;
        const string secondarySpacing = "  ";

        Panel listPanel = new Panel();
        List<EnchantmentPanel> enchantmentPanels = new List<EnchantmentPanel>();
        VerticalScrollBar scroller = new VerticalScrollBar();

        public delegate void OnRefreshListEventHandler(EnchantmentListPicker sender);
        public event OnRefreshListEventHandler OnRefreshList;

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
            panel.OnMouseClick += EnchantmentPanel_OnMouseClick;
            enchantmentPanels.Add(panel);
            RefreshPanelLayout();
        }

        public void AddEnchantments(EnchantmentSettings[] enchantments)
        {
            foreach(EnchantmentSettings enchantment in enchantments)
            {
                EnchantmentPanel panel = new EnchantmentPanel(enchantment, new Rect(0, 0, GetRenderWidth(), InteriorHeight));
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

        public int GetTotalEnchantmentCost()
        {
            int cost = 0;
            foreach(EnchantmentPanel panel in enchantmentPanels)
            {
                cost += panel.Enchantment.EnchantCost;
            }

            return cost;
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
            Vector2 panelPos = new Vector2(0, panelPosVerticalStartingOffset);
            foreach(EnchantmentPanel enchantmentPanel in enchantmentPanels)
            {
                listPanel.Components.Add(enchantmentPanel);
                enchantmentPanel.Position = panelPos;
                enchantmentPanel.FitToScroller = scroller.Enabled;
                panelPos.y += enchantmentPanel.Size.y + panelSpacing;
                scrollerUnits += (int)(enchantmentPanel.Size.y + panelSpacing);
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

        #endregion

        #region Event Handlers

        private void EnchantmentPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            EnchantmentPanel panelToRemove = (EnchantmentPanel)sender;
            panelToRemove.OnMouseClick -= EnchantmentPanel_OnMouseClick;
            enchantmentPanels.Remove(panelToRemove);
            scroller.ScrollIndex = 0;
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            RefreshPanelLayout();
        }

        private void Scroller_OnScroll()
        {
            if (!ShowScroller())
                return;

            Vector2 panelPos = new Vector2(0, panelPosVerticalStartingOffset - scroller.ScrollIndex);
            foreach (EnchantmentPanel panel in enchantmentPanels)
            {
                panel.Position = panelPos;
                panelPos.y += panel.Size.y + panelSpacing;
            }
        }

        #endregion

        #region Enchantment Panel

        public class EnchantmentPanel : Panel
        {
            const float panelWidthWithoutScroller = 75;
            const float panelWidthWithScroller = 71;
            const float panelHeightWithoutSecondary = 5;
            const float panelHeightWithSecondary = 10;

            Vector2 secondaryLabelPos = new Vector2(0, 5);
            TextLabel primaryLabel, secondaryLabel;
            bool lastFitToScroller;

            public Color textColor = DaggerfallUI.DaggerfallDefaultTextColor;
            public Color highlightedTextColor = DaggerfallUI.DaggerfallAlternateHighlightTextColor;

            public bool FitToScroller { get; set; }
            public EnchantmentSettings Enchantment { get; set; }
            Rect RenderArea { get; set; }

            public EnchantmentPanel(EnchantmentSettings enchantment, Rect renderArea)
            {
                bool hasSecondaryLabel = !string.IsNullOrEmpty(enchantment.SecondaryDisplayName);
                Size = new Vector2(panelWidthWithoutScroller, hasSecondaryLabel ? panelHeightWithSecondary : panelHeightWithoutSecondary);

                primaryLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.SmallFont, Vector2.zero, enchantment.PrimaryDisplayName, this);
                secondaryLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.SmallFont, secondaryLabelPos, secondarySpacing + enchantment.SecondaryDisplayName, this);
                primaryLabel.RestrictedRenderAreaCoordinateType = secondaryLabel.RestrictedRenderAreaCoordinateType = TextLabel.RestrictedRenderArea_CoordinateType.DaggerfallNativeCoordinates;
                primaryLabel.RectRestrictedRenderArea = secondaryLabel.RectRestrictedRenderArea = renderArea;
                primaryLabel.TextColor = secondaryLabel.TextColor = textColor;

                if (!hasSecondaryLabel)
                    secondaryLabel.Enabled = false;

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