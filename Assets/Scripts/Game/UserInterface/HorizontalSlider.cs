// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
        #region Fields

        Texture2D hScrollThumbLeft;
        Texture2D hScrollThumbBody;
        Texture2D hScrollThumbRight;

        int totalUnits;
        int displayUnits;
        int scrollIndex;
        Rect thumbRect;

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
            set { displayUnits = value; }
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
        }

        protected override void MouseClick(Vector2 clickPosition)
        {
            base.MouseClick(clickPosition);

            if (clickPosition.x < thumbRect.xMin)
                ScrollIndex -= 1;
            else if (clickPosition.x > thumbRect.xMax)
                ScrollIndex += 1;
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

            RaiseOnScrollEvent();
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
