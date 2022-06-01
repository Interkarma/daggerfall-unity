// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Navigation controls for pages or listed items.
    /// </summary>
    public class Paginator : Panel
    {
        #region UI Controls

        TextLabel indicator     = new TextLabel();
        Button leftButton       = new Button();
        Button rightButton      = new Button();

        #endregion

        #region Fields

        const string leftArrow = "<";
        const string rightArrow = ">";

        int previous, selected, total;

        Color arrowColor            = Color.white;
        Color disabledArrowColor    = Color.grey;

        #endregion

        #region Properties

        /// <summary>
        /// Number of positions.
        /// </summary>
        public int Total
        {
            get { return total; }
            set { SetTotal(value); }
        }

        /// <summary>
        /// Selected index.
        /// </summary>
        public int Selected
        {
            get { return selected; }
            set { SetSelected(value); }
        }

        /// <summary>
        /// Is first selected?
        /// </summary>
        public bool IsFirst { get { return selected == 0; } }

        /// <summary>
        /// Is last selected?
        /// </summary>
        public bool IsLast { get { return total - selected == 1; } }

        /// <summary>
        /// Color of previous/next arrows.
        /// </summary>
        public Color ArrowColor
        {
            get { return arrowColor; }
            set { arrowColor = value;
                UpdateArrowsColor(); }
        }

        /// <summary>
        /// Color of previous/next arrows on first/last position.
        /// </summary>
        public Color DisabledArrowColor
        {
            get { return disabledArrowColor; }
            set { disabledArrowColor = value;
                UpdateArrowsColor(); }
        }

        /// <summary>
        /// Color of index label.
        /// </summary>
        public Color TextColor
        {
            get { return indicator.TextColor; }
            set { indicator.TextColor = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a control for pages or listed items.
        /// </summary>
        public Paginator(int total = 1)
        {
            leftButton.Size = new Vector2(5, 5);
            leftButton.VerticalAlignment = VerticalAlignment.Middle;
            leftButton.Label.Text = leftArrow;
            leftButton.Label.ShadowColor = Color.clear;
            leftButton.OnMouseClick += LeftButton_OnMouseClick;
            Components.Add(leftButton);

            rightButton.Size = new Vector2(5, 5);
            rightButton.VerticalAlignment = VerticalAlignment.Middle;
            rightButton.Label.Text = rightArrow;
            rightButton.Label.ShadowColor = Color.clear;
            rightButton.OnMouseClick += RightButton_OnMouseClick;
            Components.Add(rightButton);

            indicator.Font = DaggerfallUI.Instance.Font3;
            indicator.ShadowColor = Color.clear;
            indicator.Size = new Vector2(25, 5);
            indicator.VerticalAlignment = VerticalAlignment.Middle;
            Components.Add(indicator);

            this.total = total;
            ConfirmSelection();
            SetScale();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set text scale and distance between arrows. 
        /// </summary>
        /// <param name="textScale">Scale factor of text.</param>
        /// <param name="arrowSpacing">Distance between arrows.</param>
        public void SetScale(float textScale = 1, float arrowSpacing = 30)
        {
            // Assign text scale
            leftButton.Label.TextScale = textScale;
            rightButton.Label.TextScale = textScale;
            indicator.TextScale = textScale;

            // Move right arrow
            float rightArrowPosX = Mathf.Max(0, arrowSpacing) + leftButton.Position.x;
            rightButton.Position = new Vector2(rightArrowPosX, rightButton.Position.y);

            // Move position indicator between arrows
            float indicatorPosX = (float)(leftButton.Position.x + rightButton.Position.x) / 2;
            indicatorPosX -= indicator.Size.x / 2;
            indicator.Position = new Vector2(indicatorPosX, indicator.Position.y);
        }

        /// <summary>
        /// Move to previous position.
        /// </summary>
        public void Previous()
        {
            if (!IsFirst)
            {
                previous = selected--;
                ConfirmSelection();
            }         
        }

        /// <summary>
        /// Move to next position.
        /// </summary>
        public void Next()
        {
            if (!IsLast)
            {
                previous = selected++;
                ConfirmSelection();
            }
        }

        /// <summary>
        /// Move to first position.
        /// </summary>
        public void First()
        {
            SetSelected(0);
        }

        /// <summary>
        /// Move to last position.
        /// </summary>
        public void Last()
        {
            SetSelected(total - 1);
        }

        /// <summary>
        /// Select position without raising events.
        /// </summary>
        public void Sync(int selected)
        {
            SetSelected(selected, false);
        }

        #endregion

        #region Private Methods

        private void SetTotal(int total)
        {
            if (total <= 0)
                throw new ArgumentOutOfRangeException("total", total, "Total must be a positive number.");

            // Assign total and move selected in new range.
            this.total = total;
            if (selected >= total)
                selected = total - 1;

            ConfirmSelection(false);
        }

        private void SetSelected(int selected, bool raiseEvent = true)
        {
            if (this.selected == selected)
                return;

            if (selected < 0 || selected >= total)
                throw new ArgumentOutOfRangeException("selected", selected, "Selected must be positive and lower than total.");

            previous = this.selected;
            this.selected = selected;

            ConfirmSelection(raiseEvent);
        }

        private void ConfirmSelection(bool raiseEvent = true)
        {
            indicator.Text = string.Format("{0}/{1}", selected + 1, total);

            UpdateArrowsColor();

            if (raiseEvent)
                RaiseOnSelected();
        }

        private void UpdateArrowsColor()
        {
            leftButton.Label.TextColor = IsFirst ? disabledArrowColor : arrowColor;
            rightButton.Label.TextColor =  IsLast ? disabledArrowColor : arrowColor;
        }

        #endregion

        #region Event Handlers

        private void LeftButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Previous();
        }

        private void RightButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Next();
        }

        public delegate void OnSelectedHandler(int previous, int selected);
        public event OnSelectedHandler OnSelected;
        void RaiseOnSelected()
        {
            if (OnSelected != null)
                OnSelected(previous, selected);
        }

        #endregion
    }
}