// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Implements a text list box.
    /// </summary>
    public class ListBox : BaseScreenComponent
    {
        #region Fields

        PixelFont font;
        int selectedIndex = 0;
        int scrollIndex = 0;
        int rowsDisplayed = 9;
        int rowSpacing = 1;
        HorizontalAlignment rowAlignment = HorizontalAlignment.Left;
        Vector2 shadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
        Vector2 selectedShadowPosition = Vector2.zero;
        Color textColor = DaggerfallUI.DaggerfallDefaultTextColor;
        Color selectedTextColor = DaggerfallUI.DaggerfallDefaultSelectedTextColor;
        Color shadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
        Color selectedShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
        List<TextLabel> listItems = new List<TextLabel>();

        #endregion

        #region Properties

        public int ScrollIndex
        {
            get { return scrollIndex; }
            set { scrollIndex = value; }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; }
        }

        public string SelectedItem
        {
            get { return listItems[selectedIndex].Text; }
        }

        public int Count
        {
            get { return listItems.Count; }
        }

        public int RowsDisplayed
        {
            get { return rowsDisplayed; }
            set { rowsDisplayed = value; }
        }

        public int RowSpacing
        {
            get { return rowSpacing; }
            set { rowSpacing = value; }
        }

        public HorizontalAlignment RowAlignment
        {
            get { return rowAlignment; }
            set { rowAlignment = value; }
        }

        public Vector2 ShadowPosition
        {
            get { return shadowPosition; }
            set { shadowPosition = value; }
        }

        public Vector2 SelectedShadowPosition
        {
            get { return selectedShadowPosition; }
            set { selectedShadowPosition = value; }
        }

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        public Color SelectedTextColor
        {
            get { return selectedTextColor; }
            set { selectedTextColor = value; }
        }

        public Color ShadowColor
        {
            get { return shadowColor; }
            set { shadowColor = value; }
        }

        public Color SelectedShadowColor
        {
            get { return selectedShadowColor; }
            set { selectedShadowColor = value; }
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();

            float x = 0, y = 0;
            for (int i = 0; i < listItems.Count; i++)
            {
                if (i < scrollIndex || i >= scrollIndex + rowsDisplayed)
                    continue;

                TextLabel label = listItems[i];
                if (i == selectedIndex)
                {
                    label.TextColor = selectedTextColor;
                    label.ShadowPosition = selectedShadowPosition;
                    label.ShadowColor = selectedShadowColor;
                }
                else
                {
                    label.TextColor = textColor;
                    label.ShadowPosition = shadowPosition;
                    label.ShadowColor = shadowColor;
                }

                label.Position = new Vector2(x, y);
                label.HorizontalAlignment = rowAlignment;
                label.Draw();

                y += label.Font.GlyphHeight + rowSpacing;
            }
        }

        #endregion

        #region Public Methods

        public void AddItem(string text)
        {
            if (font == null)
                font = DaggerfallUI.Instance.DefaultFont;

            TextLabel textLabel = new TextLabel();
            textLabel.Scaling = Scaling.None;
            textLabel.HorizontalAlignment = rowAlignment;
            textLabel.Font = font;
            textLabel.Text = text;
            textLabel.Parent = this;
            listItems.Add(textLabel);
        }

        public void RemoveItem(int index)
        {
            if (index < 0 || index >= listItems.Count)
                throw new IndexOutOfRangeException("ListBox: RemoveItem index out of range.");

            listItems.RemoveAt(index);
        }

        public void SelectPrevious()
        {
            if (selectedIndex > 0)
            {
                selectedIndex--;
                if (selectedIndex < scrollIndex)
                    scrollIndex = selectedIndex;
            }
        }

        public void SelectNext()
        {
            if (selectedIndex < listItems.Count - 1)
            {
                selectedIndex++;
                if (selectedIndex > scrollIndex + (rowsDisplayed - 1))
                    scrollIndex++;
            }
        }

        public void ScrollUp()
        {
            if (scrollIndex > 0)
                scrollIndex--;

            ClampSelectionToVisibleRange();
        }

        public void ScrollDown()
        {
            if (scrollIndex < listItems.Count - rowsDisplayed)
                scrollIndex++;

            ClampSelectionToVisibleRange();
        }

        // Clamps selection to inside visible range like Daggerfall
        public void ClampSelectionToVisibleRange()
        {
            if (selectedIndex > scrollIndex + rowsDisplayed - 1)
                selectedIndex = scrollIndex + rowsDisplayed - 1;
            if (selectedIndex < scrollIndex)
                selectedIndex = scrollIndex;
        }

        #endregion
    }
}