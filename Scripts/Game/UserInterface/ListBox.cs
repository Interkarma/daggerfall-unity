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

        int maxCharacters = -1;
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

        /// <summary>
        /// Maximum length of label string.
        /// Setting to -1 allows for any length.
        /// </summary>
        public int MaxCharacters
        {
            get { return maxCharacters; }
            set { maxCharacters = value; }
        }

        public int ScrollIndex
        {
            get { return scrollIndex; }
            set { scrollIndex = value; }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaiseOnSelectItemEvent(); }
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

            if (Input.GetKeyDown(KeyCode.UpArrow))
                SelectPrevious();
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                SelectNext();
            else if (Input.GetKeyDown(KeyCode.Return))
                UseSelectedItem();
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

        protected override void MouseClick(Vector2 clickPosition)
        {
            base.MouseClick(clickPosition);

            int row = (int)(clickPosition.y / (font.GlyphHeight + rowSpacing));

            selectedIndex = scrollIndex + row;
            RaiseOnSelectItemEvent();
        }

        protected override void MouseDoubleClick(Vector2 clickPosition)
        {
            base.MouseDoubleClick(clickPosition);

            UseSelectedItem();
        }

        protected override void MouseScrollUp()
        {
            base.MouseScrollUp();

            ScrollUp();
        }

        protected override void MouseScrollDown()
        {
            base.MouseScrollDown();

            ScrollDown();
        }

        #endregion

        #region Public Methods

        public void AddItem(string text)
        {
            if (font == null)
                font = DaggerfallUI.DefaultFont;

            TextLabel textLabel = new TextLabel();
            textLabel.ScalingMode = Scaling.None;
            textLabel.HorizontalAlignment = rowAlignment;
            textLabel.Font = font;
            textLabel.MaxCharacters = maxCharacters;
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

        public void UpdateItem(int index, string label)
        {
            if (index < 0 || index >= listItems.Count)
                throw new IndexOutOfRangeException("ListBox: UpdateItem index out of range.");
            else if (listItems[index] == null)
                throw new IndexOutOfRangeException("ListBox: item at index was null.");
            else
                listItems[index].Text = label;
        }

        public void SelectPrevious()
        {
            if (selectedIndex > 0)
            {
                selectedIndex--;
                if (selectedIndex < scrollIndex)
                    scrollIndex = selectedIndex;
            }

            RaiseOnSelectPreviousEvent();
        }

        public void SelectNext()
        {
            if (selectedIndex < listItems.Count - 1)
            {
                selectedIndex++;
                if (selectedIndex > scrollIndex + (rowsDisplayed - 1))
                    scrollIndex++;
            }

            RaiseOnSelectNextEvent();
        }

        public void UseSelectedItem()
        {
            RaiseOnUseItemEvent();
        }

        public void ScrollUp()
        {
            if (scrollIndex > 0)
                scrollIndex--;

            ClampSelectionToVisibleRange();
            RaiseOnScrollUpEvent();
        }

        public void ScrollDown()
        {
            if (scrollIndex < listItems.Count - rowsDisplayed)
                scrollIndex++;

            ClampSelectionToVisibleRange();
            RaiseOnScrollDownEvent();
        }

        // Clamps selection to inside visible range like Daggerfall
        public void ClampSelectionToVisibleRange()
        {
            if (selectedIndex > scrollIndex + rowsDisplayed - 1)
            {
                selectedIndex = scrollIndex + rowsDisplayed - 1;
                RaiseOnSelectItemEvent();
            }
            if (selectedIndex < scrollIndex)
            {
                selectedIndex = scrollIndex;
                RaiseOnSelectItemEvent();
            }
        }

        #endregion

        #region Event Handlers

        public delegate void OnSelectPreviousEventHandler();
        public event OnSelectPreviousEventHandler OnSelectPrevious;
        void RaiseOnSelectPreviousEvent()
        {
            if (OnSelectPrevious != null)
                OnSelectPrevious();
        }

        public delegate void OnSelectNextEventHandler();
        public event OnSelectNextEventHandler OnSelectNext;
        void RaiseOnSelectNextEvent()
        {
            if (OnSelectNext != null)
                OnSelectNext();
        }

        public delegate void OnScrollUpEventHandler();
        public event OnScrollUpEventHandler OnScrollUp;
        void RaiseOnScrollUpEvent()
        {
            if (OnScrollUp != null)
                OnScrollUp();
        }

        public delegate void OnScrollDownEventHandler();
        public event OnScrollDownEventHandler OnScrollDown;
        void RaiseOnScrollDownEvent()
        {
            if (OnScrollDown != null)
                OnScrollDown();
        }

        public delegate void OnSelectItemEventHandler();
        public event OnSelectItemEventHandler OnSelectItem;
        void RaiseOnSelectItemEvent()
        {
            if (OnSelectItem != null)
                OnSelectItem();
        }

        public delegate void OnUseSelectedItemEventHandler();
        public event OnUseSelectedItemEventHandler OnUseSelectedItem;
        void RaiseOnUseItemEvent()
        {
            if (OnUseSelectedItem != null)
                OnUseSelectedItem();
        }

        #endregion
    }
}