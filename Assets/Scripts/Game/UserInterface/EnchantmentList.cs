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
using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class EnchantmentList : Panel
    {
        #region Fields

        const int panelSpacing = 15;
        const int visiblePanels = 8;
        const int scrollerWidth = 4;
        const int panelPosVerticalStartingOffset = 2;
        const string secondarySpacing = "  ";

        Panel listPanel = new Panel();
        List<EnchantmentPanel> panels = new List<EnchantmentPanel>();
        VerticalScrollBar scroller = new VerticalScrollBar();

        #endregion

        #region Constructors

        public EnchantmentList()
        {
            Components.Add(listPanel);
            Components.Add(scroller);

            scroller.OnScroll += Scroller_OnScroll;
        }

        #endregion

        #region Public Methods

        public void AddItem(string primary, string secondary)
        {
            EnchantmentPanel panel = new EnchantmentPanel(primary, secondary, new Rect(0, 0, GetRenderWidth(), InteriorHeight));
            panels.Add(panel);
            RefreshPanelLayout();
        }

        public void AddItem(EnchantmentSettings settings)
        {
            EnchantmentPanel panel = new EnchantmentPanel("Cast when used:", "Levitate", new Rect(0, 0, GetRenderWidth(), InteriorHeight));
            panels.Add(panel);
            RefreshPanelLayout();
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
            scroller.ScrollIndex -= 2;
        }

        protected override void MouseScrollDown()
        {
            base.MouseScrollDown();
            scroller.ScrollIndex += 2;
        }

        #endregion

        #region Private Methods

        void RefreshPanelLayout()
        {
            listPanel.Components.Clear();

            if (ShowScroller())
            {
                scroller.Enabled = true;
                scroller.Position = new Vector2(InteriorWidth - scrollerWidth, 0);
                scroller.Size = new Vector2(scrollerWidth, InteriorHeight);
                scroller.TotalUnits = panels.Count * panelSpacing;
                scroller.DisplayUnits = InteriorHeight;
            }
            else
            {
                scroller.Enabled = false;
            }

            Vector2 panelPos = new Vector2(0, panelPosVerticalStartingOffset);
            foreach(EnchantmentPanel panel in panels)
            {
                listPanel.Components.Add(panel);
                panel.Position = panelPos;
                panelPos.y += panelSpacing;
            }
        }

        int GetRenderWidth()
        {
            return ShowScroller() ? InteriorWidth - scrollerWidth : InteriorWidth;
        }

        bool ShowScroller()
        {
            return panels.Count > visiblePanels;
        }

        private void Scroller_OnScroll()
        {
            Vector2 panelPos = new Vector2(0, panelPosVerticalStartingOffset - scroller.ScrollIndex);
            foreach (EnchantmentPanel panel in panels)
            {
                panel.Position = panelPos;
                panelPos.y += panelSpacing;
            }
        }

        #endregion

        #region Enchantment Panel

        public class EnchantmentPanel : Panel
        {
            Vector2 panelSize = new Vector2(75, 10);
            Vector2 secondaryLabelPos = new Vector2(0, 5);

            public EnchantmentPanel(string primary, string secondary, Rect renderArea)
            {
                Size = panelSize;
                TextLabel label1 = DaggerfallUI.AddTextLabel(DaggerfallUI.SmallFont, Vector2.zero, primary, this);
                TextLabel label2 = DaggerfallUI.AddTextLabel(DaggerfallUI.SmallFont, secondaryLabelPos, secondarySpacing + secondary, this);
                //label1.ShadowPosition = label2.ShadowPosition = Vector2.zero;

                label1.RestrictedRenderAreaCoordinateType = label2.RestrictedRenderAreaCoordinateType = TextLabel.RestrictedRenderArea_CoordinateType.DaggerfallNativeCoordinates;
                label1.RectRestrictedRenderArea = label2.RectRestrictedRenderArea = renderArea;
            }
        }

        #endregion
    }
}