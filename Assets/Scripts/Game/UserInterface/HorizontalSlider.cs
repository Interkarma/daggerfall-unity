// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes: Reused code from VerticalScrollBar
//

using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class HorizontalSlider : BaseScreenComponent
    {
        enum Mode { ScrollBar, IntSlider, FloatSlider, MultipleChoices};

        #region Fields

        Texture2D hScrollThumbLeft;
        Texture2D hScrollThumbBody;
        Texture2D hScrollThumbRight;

        int totalUnits;
        int displayUnits;
        int scrollIndex;
        Rect thumbRect;

        Mode mode = Mode.ScrollBar;
        TextLabel indicator;
        int minValue;
        int maxValue;
        string[] items;

        bool draggingThumb = false;
        Vector2 dragStartPosition;
        int dragStartScrollIndex;

        #endregion

        #region Properties

        /// <summary>
        /// The total number of units (pixels, rows, etc.) represented by this slider.
        /// </summary>
        public int TotalUnits
        {
            get { return totalUnits; }
            set { totalUnits = value; }
        }

        /// <summary>
        /// The maximum number of visible units (pixels, rows, etc.) at any one time.
        /// </summary>
        public int DisplayUnits
        {
            get { return displayUnits; }
            set { SetDisplayUnits(value); }
        }

        /// <summary>
        /// The current slider position in units.
        /// </summary>
        public int ScrollIndex
        {
            get { return scrollIndex; }
            set { SetScrollIndex(value); }
        }

        /// <summary>
        /// Gets flag set when user dragging thumb.
        /// </summary>
        public bool DraggingThumb
        {
            get { return draggingThumb; }
        }

        /// <summary>
        /// Tint color of the slider.
        /// </summary>
        public Color? TintColor { get; set; }

        /// <summary>
        /// Indicator for this slider.
        /// </summary>
        public TextLabel Indicator
        {
            get { return indicator; }
        }

        /// <summary>
        /// Distance from slider. Negative is on the left.
        /// </summary>
        public int IndicatorOffset
        {
            set { SetIndicatorOffset(value); }
        }

        /// <summary>
        /// Current value on the slider.
        /// </summary>
        public int Value
        {
            get { return scrollIndex + minValue; }
            set { SetScrollIndex(value - minValue); }
        }

        #endregion

        #region Constructors

        public HorizontalSlider()
            : base()
        {
            hScrollThumbLeft = Resources.Load<Texture2D>("HSliderThumbLeft");
            hScrollThumbBody = Resources.Load<Texture2D>("HSliderThumbBody");
            hScrollThumbRight = Resources.Load<Texture2D>("HSliderThumbRight");
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            base.Update();

            if (Input.GetMouseButton(0))
            {
                Vector2 mousePosition = ScreenToLocal(MousePosition);
                if (!draggingThumb && thumbRect.Contains(mousePosition))
                {
                    draggingThumb = true;
                    dragStartPosition = mousePosition;
                    dragStartScrollIndex = scrollIndex;
                }

                if (draggingThumb)
                {
                    Vector2 dragDistance = mousePosition - dragStartPosition;
                    float scale = Size.x / (float)totalUnits;
                    float unitsMoved = dragDistance.x / scale;
                    SetScrollIndex(dragStartScrollIndex + (int)unitsMoved);
                }
            }
            else
            {
                if (draggingThumb)
                {
                    draggingThumb = false;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();

            if (totalUnits <= displayUnits)
                return;

            DrawSlider();

            if (indicator != null)
                indicator.Draw();
        }

        protected override void MouseClick(Vector2 clickPosition)
        {
            base.MouseClick(clickPosition);

            if (clickPosition.x < thumbRect.xMin)
                ScrollIndex -= displayUnits;
            else if (clickPosition.x > thumbRect.xMax)
                ScrollIndex += displayUnits;
        }

        protected override void MouseScrollUp()
        {
            base.MouseScrollUp();
            ScrollIndex -= 1;
        }

        protected override void MouseScrollDown()
        {
            base.MouseScrollDown();
            ScrollIndex += 1;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Draws a numeric indicator that show the value on slider.
        /// </summary>
        public void SetIndicator(int min, int max, int start)
        {
            this.mode = Mode.IntSlider;
            this.items = null;
            SetupIndicator(min, max, start);
        }

        /// <summary>
        /// Draws a numeric indicator that show the value on slider with one decimal digit.
        /// </summary>
        public void SetIndicator(float min, float max, float start)
        {
            this.mode = Mode.FloatSlider;
            this.items = null;
            SetupIndicator(
                Mathf.RoundToInt(min * 10),
                Mathf.RoundToInt(max * 10),
                Mathf.RoundToInt(start * 10));
        }

        /// <summary>
        /// Draws a text indicator that show selected option on slider.
        /// </summary>
        public void SetIndicator(string[] items, int selected)
        {
            this.mode = Mode.MultipleChoices;
            this.items = items;
            SetupIndicator(0, items.Length - 1, selected);
        }

        /// <summary>
        /// Removes any indicator.
        /// </summary>
        public void RemoveIndicator()
        {
            this.mode = Mode.ScrollBar;
            this.items = null;
            this.indicator = null;
        }

        /// <summary>
        /// Get value on the slider.
        /// </summary>
        public float GetValue()
        {
            return mode == Mode.FloatSlider ? (float)Value / 10 : Value;
        }

        /// <summary>
        /// Set value on the slider.
        /// </summary>
        public void SetValue(float value)
        {
            if (mode == Mode.FloatSlider)
                value *= 10;
            Value = Mathf.RoundToInt(value);
        }

        /// <summary>
        /// Resets slider properties without triggering events.
        /// </summary>
        public void Reset(int displayUnits = 0, int totalUnits = 0, int scrollIndex = 0)
        {
            this.displayUnits = displayUnits;
            this.totalUnits = totalUnits;
            this.scrollIndex = scrollIndex;
        }

        #endregion

        #region Private Methods

        void SetDisplayUnits(int value)
        {
            displayUnits = value;

            if (mode != Mode.ScrollBar)
                totalUnits = (maxValue - minValue) + displayUnits;
        }

        void SetScrollIndex(int value)
        {
            int maxScroll = totalUnits - displayUnits;
            if (maxScroll < 0)
                maxScroll = 0;

            scrollIndex = value;

            if (scrollIndex < 0)
                scrollIndex = 0;
            if (scrollIndex > maxScroll)
                scrollIndex = maxScroll;

            if (indicator != null)
                indicator.Text = GetIndicatorText();

            RaiseOnScrollEvent();
        }

        void SetIndicatorOffset(int value)
        {
            if (indicator == null)
                return;

            if (value >= 0)
                indicator.Position = new Vector2(Size.x + value, 0);
            else
                indicator.Position = new Vector2(value, 0);
        }

        void DrawSlider()
        {
            // Update current thumb rect in local space
            Rect totalRect = Rectangle;
            float thumbWidth = totalRect.width * ((float)displayUnits / (float)totalUnits);
            if (thumbWidth < 10) thumbWidth = 10;
            float thumbX = scrollIndex * (totalRect.width - thumbWidth) / (totalUnits - displayUnits);
            thumbRect = ScreenToLocal(new Rect(totalRect.x + thumbX, totalRect.y, thumbWidth, totalRect.height));

            // Get rects
            float leftTextureWidth = hScrollThumbLeft.width * LocalScale.x;
            float rightTextureWidth = hScrollThumbRight.width * LocalScale.x;
            Rect leftRect = new Rect(totalRect.x + thumbX, totalRect.y, leftTextureWidth, totalRect.height);
            Rect bodyRect = new Rect(leftRect.xMax, totalRect.y, thumbWidth - leftTextureWidth - rightTextureWidth, totalRect.height);
            Rect rightRect = new Rect(bodyRect.xMax, totalRect.y, rightTextureWidth, totalRect.height);

            // Draw thumb texture slices in screen space
            Color color = GUI.color;
            if (TintColor.HasValue)
                GUI.color = TintColor.Value;
            GUI.DrawTexture(leftRect, hScrollThumbLeft, ScaleMode.StretchToFill);
            GUI.DrawTexture(bodyRect, hScrollThumbBody, ScaleMode.StretchToFill);
            GUI.DrawTexture(rightRect, hScrollThumbRight, ScaleMode.StretchToFill);
            GUI.color = color;
        }

        void SetupIndicator(int min, int max, int start)
        {
            if (indicator == null)
            {
                indicator = new TextLabel();
                indicator.Parent = this;
            }

            minValue = min;
            maxValue = max;
            SetDisplayUnits(displayUnits);
            SetScrollIndex(start - min);
        }

        string GetIndicatorText()
        {
            int selected = scrollIndex + minValue;

            switch(mode)
            {
                case Mode.IntSlider:
                    return selected.ToString();

                case Mode.FloatSlider:
                    return ((float)selected / 10).ToString("n1");

                case Mode.MultipleChoices:
                    return selected < items.Length ? items[selected] : string.Empty;

                default:
                    return string.Empty;
            }
        }

        #endregion

        #region EventHandlers

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
