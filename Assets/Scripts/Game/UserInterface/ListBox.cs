// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
using System.Linq;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Implements a text list box.
    /// </summary>
    public class ListBox : BaseScreenComponent
    {
        #region Fields

        int maxCharacters = -1;
        DaggerfallFont font;
        int selectedIndex = 0;
        int highlightedIndex = -1;
        int scrollIndex = 0;
        bool enabledHorizontalScroll = false;
        int horizontalScrollIndex = 0;
        int maxHorizontalScrollIndex = 0;
        bool wrapTextItems = false;
        bool wrapWords = false;
        int rowsDisplayed = 9;
        int rowSpacing = 1;
        float textScale = 1.0f;
        HorizontalAlignment rowAlignment = HorizontalAlignment.Left;
        Vector2 shadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
        Vector2 selectedShadowPosition = Vector2.zero;
        Color textColor = DaggerfallUI.DaggerfallDefaultTextColor;
        Color selectedTextColor = DaggerfallUI.DaggerfallDefaultSelectedTextColor;
        Color shadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
        Color selectedShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;

        public enum VerticalScrollModes
        {
            EntryWise,
            PixelWise
        }
        VerticalScrollModes verticalScrollMode = VerticalScrollModes.EntryWise;

        // HorizontalScrollMode.PixelWise is only applied if verticalScrollMode == VerticalScrollModes.PixelWise, otherwise fallback HorizontalScrollMode.Charwise is applied
        public enum HorizontalScrollModes
        {
            CharWise,
            PixelWise
        }
        HorizontalScrollModes horizontalScrollMode = HorizontalScrollModes.CharWise;

        // ListItem class allows for each item to have unique text colors if necessary (needed e.g. in talk window for question and answer color flavors)
        public class ListItem
        {
            public TextLabel textLabel;
            public Color textColor = DaggerfallUI.DaggerfallDefaultTextColor;
            public Color disabledTextColor = DaggerfallUI.DaggerfallDisabledTextColor;
            public Color selectedTextColor = DaggerfallUI.DaggerfallDefaultSelectedTextColor;
            public Color shadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
            public Color selectedShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
            public Color highlightedTextColor = DaggerfallUI.DaggerfallAlternateHighlightTextColor;
            public Color highlightedDisabledTextColor = DaggerfallUI.DaggerfallHighlightDisabledTextColor;
            public Color highlightedSelectedTextColor = DaggerfallUI.DaggerfallBrighterSelectedTextColor;
            public object tag;

            public bool Enabled { get => textLabel.Enabled; set { textLabel.Enabled = value; } }

            public ListItem(TextLabel textLabel)
            {
                this.textLabel = textLabel;
            }
        }
        List<ListItem> listItems = new List<ListItem>();

        #endregion

        #region Properties

        public List<ListItem> ListItems
        {
            // Create a shallow copy to keep record list unmodified
            get { return listItems.ToList(); }
        }

        /// <summary>
        /// Maximum length of label string.
        /// Setting to -1 allows for any length.
        /// </summary>
        public int MaxCharacters
        {
            get { return maxCharacters; }
            set { maxCharacters = value; }
        }

        public DaggerfallFont Font
        {
            get { return font; }
            set { font = value; }
        }

        public int ScrollIndex
        {
            get { return scrollIndex; }
            set { scrollIndex = value; }
        }

        /// <summary>
        /// Enables horizontally scrolling of listbox content (this mode can be used if textlabels do not horizontally fit into listbox entirely)
        /// </summary>
        public bool EnabledHorizontalScroll
        {
            get { return enabledHorizontalScroll; }
            set { enabledHorizontalScroll = value; }
        }

        /// <summary>
        /// Horizontal scroll index - depending on HorizontalScrollMode its unit is either in characters or pixels
        /// </summary>
        public int HorizontalScrollIndex
        {
            get { return horizontalScrollIndex; }
            set
            {
                horizontalScrollIndex = value;
                horizontalScrollIndex = Math.Max(0, Math.Min(maxHorizontalScrollIndex, horizontalScrollIndex));
            }
        }

        /// <summary>
        /// Maximal allowed horizontal scroll index - depending on HorizontalScrollMode its unit is either in characters or pixels
        /// </summary>
        public int MaxHorizontalScrollIndex
        {
            get { return maxHorizontalScrollIndex; }
            set { maxHorizontalScrollIndex = value; }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SelectIndex(value); }
        }

        public string SelectedItem
        {
            get { return listItems[selectedIndex].textLabel.Text; }
        }

        public ListItem SelectedValue
        {
            get { return listItems[selectedIndex]; }
        }

        public int Count
        {
            get { return listItems.Count; }
        }

        /// <summary>
        /// enable wrapping of text items, additional mode for word wrapping can be activated with property WrapWords
        /// </summary>
        public bool WrapTextItems
        {
            get { return wrapTextItems; }
            set { wrapTextItems = value; }
        }

        /// <summary>
        /// enable wrapping of words in text items - Property WrapTextItems must be set to true as well so that this has an effect
        /// </summary>
        public bool WrapWords
        {
            get { return wrapWords; }
            set { wrapWords = value; }
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

        public float TextScale
        {
            get { return textScale; }
            set { textScale = value; }
        }

        /// <summary>
        /// set vertical scroll mode to either character wise or pixel wise
        /// </summary>
        public VerticalScrollModes VerticalScrollMode
        {
            get { return verticalScrollMode; }
            set { verticalScrollMode = value; }
        }

        /// <summary>
        /// set horizontal scroll mode to either character wise or pixel wise
        /// </summary>
        public HorizontalScrollModes HorizontalScrollMode
        {
            get { return horizontalScrollMode; }
            set { horizontalScrollMode = value; }
        }

        /// <summary>
        /// List will accept keyboard input even when mouse not over control.
        /// Do not set this when multiple lists are used within same UI, or any other control where inputs might overlap.
        /// </summary>
        public bool AlwaysAcceptKeyboardInput { get; set; }

        #endregion

        #region Constructors
        public ListBox()
        {
            OnMouseMove += MouseMove;
            OnMouseLeave += MouseLeave;
        }
        #endregion

        #region Overrides
        public override void Update()
        {
            base.Update();

            if (MouseOverComponent || AlwaysAcceptKeyboardInput)
            {
                if (DaggerfallUI.Instance.LastKeyCode == KeyCode.UpArrow)
                    SelectPrevious();
                else if (DaggerfallUI.Instance.LastKeyCode == KeyCode.DownArrow)
                    SelectNext();
                else if (DaggerfallUI.Instance.LastKeyCode == KeyCode.LeftArrow)
                    HorizontalScrollLeft();
                else if (DaggerfallUI.Instance.LastKeyCode == KeyCode.RightArrow)
                    HorizontalScrollRight();
                else if (DaggerfallUI.Instance.LastKeyCode == KeyCode.Return)
                    UseSelectedItem();
            }
        }

        public override void Draw()
        {
            base.Draw();

            if (verticalScrollMode == VerticalScrollModes.EntryWise)
            {
                float x = 0, y = 0;
                float currentLine = 0;
                for (int i = 0; i < listItems.Count; i++)
                {
                    TextLabel label = listItems[i].textLabel;

                    if (currentLine < scrollIndex || currentLine >= scrollIndex + rowsDisplayed)
                    {
                        currentLine += label.NumTextLines;
                        continue;
                    }

                    currentLine += label.NumTextLines;                
                    label.StartCharacterIndex = horizontalScrollIndex;

                    DecideTextColor(label, i);

                    label.Position = new Vector2(x, y);
                    label.Draw();

                    y += label.TextHeight + rowSpacing;
                }
            }
            else if (verticalScrollMode == VerticalScrollModes.PixelWise)
            {
                int x = 0;
                int y = -scrollIndex;
                for (int i = 0; i < listItems.Count; i++)
                {
                    TextLabel label = listItems[i].textLabel;
                    
                    if (y + label.TextHeight < 0 || y >= this.Size.y)
                    {
                        y += label.TextHeight + rowSpacing;
                        continue;
                    }

                    if (horizontalScrollMode == HorizontalScrollModes.CharWise)
                        label.StartCharacterIndex = horizontalScrollIndex;
                    else if (horizontalScrollMode == HorizontalScrollModes.PixelWise)
                        x = -horizontalScrollIndex;

                    DecideTextColor(label, i);

                    label.HorzPixelScrollOffset = x;
                    label.Position = new Vector2(x, y);
                    label.Draw();

                    y += label.TextHeight + rowSpacing;
                }
            }           
        }

        private void DecideTextColor(TextLabel label, int i)
        {
            if (i == highlightedIndex && i == selectedIndex)
            {
                label.TextColor = listItems[i].highlightedSelectedTextColor;
                label.ShadowPosition = selectedShadowPosition;
                label.ShadowColor = listItems[i].selectedShadowColor;
            }
            else if (i == selectedIndex)
            {
                label.TextColor = listItems[i].selectedTextColor;
                label.ShadowPosition = selectedShadowPosition;
                label.ShadowColor = listItems[i].selectedShadowColor;
            }
            else if (i == highlightedIndex)
            {
                label.TextColor = listItems[i].Enabled ?
                    listItems[i].highlightedTextColor
                    : listItems[i].highlightedDisabledTextColor;
                label.ShadowPosition = shadowPosition;
                label.ShadowColor = listItems[i].shadowColor;
            }
            else
            {
                label.TextColor = listItems[i].Enabled ?
                    listItems[i].textColor
                    : listItems[i].disabledTextColor;
                label.ShadowPosition = shadowPosition;
                label.ShadowColor = listItems[i].shadowColor;
            }
        }

        private int FindPreviousEnabled(int currentIndex)
        {
            int val = currentIndex;

            do
            {
                if (val > 0)
                    val--;
            } while (!listItems[val].Enabled && val > 0);

            // From our current index, if everything previous to us was disabled, return to where we currently are
            if (!listItems[val].Enabled)
                return currentIndex;

            return val;
        }

        private int FindNextEnabled(int currentIndex)
        {
            int val = currentIndex;
            int last = listItems.Count - 1;

            do
            {
                if (val < last)
                    val++;
            } while (!listItems[val].Enabled && val < last);

            // From our current index, if everything next of us was disabled, return to where we currently are
            if (!listItems[val].Enabled)
                return currentIndex;

            return val;
        }

        protected override void MouseMove(int x, int y)
        {
            if (listItems.Count == 0)
                return;
            highlightedIndex = -1;
            if (verticalScrollMode == VerticalScrollModes.EntryWise)
            {
                int row = (y / ((int)(font.GlyphHeight * Scale.y) + rowSpacing));
                int index = scrollIndex + row;
                if (index >= 0 && index < Count)
                {
                    highlightedIndex = index;
                }
            }
            else if (verticalScrollMode == VerticalScrollModes.PixelWise)
            {
                int yCurrentItem = 0;
                int yNextItem = 0;
                for (int i = 0; i < listItems.Count; i++)
                {
                    yNextItem = yCurrentItem + listItems[i].textLabel.TextHeight + rowSpacing;
                    int yVal = scrollIndex + y;
                    if (yVal >= yCurrentItem - rowSpacing * 0.5 && yVal < yNextItem - rowSpacing * 0.5)
                    {
                        highlightedIndex = i;
                        break;
                    }
                    yCurrentItem = yNextItem;
                }

            }
        }

        protected override void MouseLeave(BaseScreenComponent sender)
        {
            highlightedIndex = -1;
        }

        protected override void MouseClick(Vector2 clickPosition)
        {
            base.MouseClick(clickPosition);

            if (listItems.Count == 0)
                return;

            int selected = -1;

            if (verticalScrollMode == VerticalScrollModes.EntryWise)
            {
                int row = (int)(clickPosition.y / ((int)(font.GlyphHeight * Scale.y) + rowSpacing));
                int index = scrollIndex + row;
                if (index >= 0 && index < Count && listItems[index].Enabled)
                {
                    selected = index;
                }
            }
            else if (verticalScrollMode == VerticalScrollModes.PixelWise)
            {
                int yCurrentItem = 0;
                int yNextItem = 0;
                for (int i = 0; i < listItems.Count; i++)
                {
                    yNextItem = yCurrentItem + listItems[i].textLabel.TextHeight + rowSpacing;
                    int y = scrollIndex + (int)(clickPosition.y);
                    if (y >= yCurrentItem - rowSpacing * 0.5 && y < yNextItem - rowSpacing * 0.5 && listItems[i].Enabled)
                    {
                        selected = i;
                        break;
                    }
                    yCurrentItem = yNextItem;
                }
            }

            if (selected > -1)
            {
                selectedIndex = selected;
                RaiseOnSelectItemEvent();
            }
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

        public void ClearItems()
        {
            listItems.Clear();
            scrollIndex = 0;
            SelectNone();
        }

        public void AddItem(string text, out ListItem itemOut, int position = -1, object tag = null)
        {
            if (font == null)
                font = DaggerfallUI.DefaultFont;

            TextLabel textLabel = new TextLabel();

            textLabel.Parent = this; //Establish parent early.

            if (UseRestrictedRenderArea)
            {
                textLabel.RectRestrictedRenderArea = RectRestrictedRenderArea;
                textLabel.RestrictedRenderAreaCoordinateType = RestrictedRenderAreaCoordinateType;
                textLabel.RestrictedRenderAreaCustomParent = RestrictedRenderAreaCustomParent;
            }
            if (horizontalScrollMode == HorizontalScrollModes.CharWise)
                textLabel.MaxWidth = (int)Size.x;
            else if (horizontalScrollMode == HorizontalScrollModes.PixelWise)
                textLabel.MaxWidth = -1;
            textLabel.AutoSize = AutoSizeModes.None;
            textLabel.HorizontalAlignment = rowAlignment;
            textLabel.Font = font;
            textLabel.MaxCharacters = maxCharacters;
            textLabel.Text = text;
            textLabel.TextScale = textScale;
            textLabel.WrapText = wrapTextItems;
            textLabel.WrapWords = wrapWords;

            itemOut = new ListItem(textLabel);
            itemOut.textColor = textColor;
            itemOut.selectedTextColor = selectedTextColor;
            itemOut.shadowColor = shadowColor;
            itemOut.selectedShadowColor = selectedShadowColor;
            itemOut.tag = tag;
            if (position < 0)
                listItems.Add(itemOut);
            else
                listItems.Insert(position, itemOut);
        }

        public void AddItem(TextLabel textLabel, out ListItem itemOut, int position = -1, string tag = null)
        {
            if (textLabel == null)
            {
                itemOut = new ListItem(null);
                return;
            }

            textLabel.Parent = this; //Establish parent early.

            if (UseRestrictedRenderArea)
            {
                textLabel.RectRestrictedRenderArea = RectRestrictedRenderArea;
                textLabel.RestrictedRenderAreaCoordinateType = RestrictedRenderAreaCoordinateType;
                textLabel.RestrictedRenderAreaCustomParent = RestrictedRenderAreaCustomParent;
            }
            if (horizontalScrollMode == HorizontalScrollModes.CharWise)
                textLabel.MaxWidth = (int)Size.x;
            else if (horizontalScrollMode == HorizontalScrollModes.PixelWise)
                textLabel.MaxWidth = -1;
            textLabel.WrapText = wrapTextItems;
            textLabel.WrapWords = wrapWords;

            itemOut = new ListItem(textLabel);
            itemOut.textColor = textColor;
            itemOut.selectedTextColor = selectedTextColor;
            itemOut.shadowColor = shadowColor;
            itemOut.selectedShadowColor = selectedShadowColor;
            itemOut.tag = tag;
            if (position < 0)
                listItems.Add(itemOut);
            else
                listItems.Insert(position, itemOut);
        }

        public void AddItem(string text, int position = -1, object tag = null)
        {
            ListItem itemOut;
            AddItem(text, out itemOut, position, tag);
        }

        public void AddItems(IEnumerable<string> items)
        {
            if (items == null)
                return;

            foreach (string item in items)
                AddItem(item);
        }

        public void AddItems(IEnumerable<TextLabel> labels)
        {
            if (labels == null)
                return;

            ListItem itemOut;
            foreach (TextLabel label in labels)
            {
                AddItem(label, out itemOut);
            }
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
                listItems[index].textLabel.Text = label;
        }

        public void SwapItems(int indexA, int indexB)
        {
            if (indexA < 0 || indexB < 0 || indexA >= listItems.Count || indexB >= listItems.Count)
                throw new IndexOutOfRangeException("ListBox: UpdateItem index out of range.");
            else
            {
                ListItem temp = listItems[indexA];
                listItems[indexA] = listItems[indexB];
                listItems[indexB] = temp;
            }
        }

        public ListItem GetItem(int index)
        {
            return listItems[index];
        }

        public int LengthOfLongestItem()
        {
            int maxLength = 0;
            for (int i = 0; i < listItems.Count; i++)
            {
                maxLength = Math.Max(maxLength, listItems[i].textLabel.Text.Length);
            }
            return maxLength;
        }

        public int HeightContent()
        {
            int sumHeight = 0;
            for (int i = 0; i < listItems.Count; i++)
            {
                sumHeight += listItems[i].textLabel.TextHeight;

                if (i < listItems.Count - 1)
                    sumHeight += rowSpacing;
            }
            return sumHeight;
        }

        public int WidthContent()
        {
            int width = 0;
            for (int i = 0; i < listItems.Count; i++)
            {
                width = Math.Max(width, listItems[i].textLabel.TextWidth);
            }
            return width;
        }

        public void SelectPrevious()
        {
            if (selectedIndex > 0)
            {
                selectedIndex = FindPreviousEnabled(selectedIndex);
                if (verticalScrollMode == VerticalScrollModes.EntryWise)
                {
                    if (selectedIndex < scrollIndex)
                        scrollIndex = selectedIndex;
                }
            }

            RaiseOnSelectPreviousEvent();
            RaiseOnSelectItemEvent();
            RaiseOnScrollEvent();
        }

        public void SelectNext()
        {
            if (selectedIndex < listItems.Count - 1)
            {
                selectedIndex = FindNextEnabled(selectedIndex);
                if (verticalScrollMode == VerticalScrollModes.EntryWise)
                {
                    if (selectedIndex > scrollIndex + (rowsDisplayed - 1))
                        scrollIndex++;
                }
            }

            RaiseOnSelectNextEvent();
            RaiseOnSelectItemEvent();
            RaiseOnScrollEvent();
        }

        public void HorizontalScrollLeft()
        {
            if (!enabledHorizontalScroll)
                return;

            horizontalScrollIndex--;
            horizontalScrollIndex = Math.Max(0, horizontalScrollIndex);
        }

        public void HorizontalScrollRight()
        {
            if (!enabledHorizontalScroll)
                return;

            horizontalScrollIndex++;
            horizontalScrollIndex = Math.Min(maxHorizontalScrollIndex, horizontalScrollIndex);
        }

        public void SelectIndex(int index)
        {
            if (index < 0 || index >= listItems.Count)
                return;

            if (listItems[index].Enabled)
            {
                selectedIndex = index;
                RaiseOnSelectItemEvent();
            }
        }

        public void SelectNone()
        {
            selectedIndex = -1;
        }

        public void ScrollToSelected()
        {
            scrollIndex = selectedIndex;
            scrollIndex = Mathf.Clamp(scrollIndex, 0, (listItems.Count - 1) - (rowsDisplayed - 1));
            RaiseOnScrollEvent();
        }

        public void UseSelectedItem()
        {
            if (selectedIndex > -1 && selectedIndex < listItems.Count && listItems[selectedIndex].Enabled)
                RaiseOnUseItemEvent();
        }

        public void ScrollUp()
        {
            if (scrollIndex > 0)
                scrollIndex--;

            RaiseOnScrollEvent();
        }

        public void ScrollDown()
        {
            if (verticalScrollMode == VerticalScrollModes.EntryWise)
            {
                if (scrollIndex < listItems.Count - rowsDisplayed)
                    scrollIndex++;
            }
            else if (verticalScrollMode == VerticalScrollModes.PixelWise)
            {
                if (scrollIndex < this.HeightContent() - this.Size.y)
                    scrollIndex++;
            }
            RaiseOnScrollEvent();
        }

        public void SetRowsDisplayedByHeight()
        {
            if (Count == 0)
                return;

            rowsDisplayed = (int)(Size.y / (int)(font.GlyphHeight * Scale.y)) - 1;
        }

        public int FindIndex(string text)
        {
            for (int i = 0; i < listItems.Count; i++)
            {
                if (string.Compare(listItems[i].textLabel.Text, text, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return i;
                }
            }

            return -1;
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

        public delegate void OnSelectItemEventHandler();
        public event OnSelectItemEventHandler OnSelectItem;
        void RaiseOnSelectItemEvent()
        {
            if (selectedIndex < 0 || selectedIndex >= Count)
                return;

            if (OnSelectItem != null)
                OnSelectItem();
        }

        public delegate void OnUseSelectedItemEventHandler();
        public event OnUseSelectedItemEventHandler OnUseSelectedItem;
        void RaiseOnUseItemEvent()
        {
            if (selectedIndex < 0 || selectedIndex >= Count)
                return;

            if (OnUseSelectedItem != null)
                OnUseSelectedItem();
        }

        public delegate void OnScrollHandler();
        public event OnScrollHandler OnScroll;
        void RaiseOnScrollEvent()
        {
            if (OnScroll != null)
                OnScroll();
        }

        #endregion
    }
}