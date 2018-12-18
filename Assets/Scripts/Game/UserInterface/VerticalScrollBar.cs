// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
    /// Implements a vertical scrollbar.
    /// </summary>
    public class VerticalScrollBar : BaseScreenComponent
    {
        #region Fields

        Texture2D vScrollThumbTop;
        Texture2D vScrollThumbBody;
        Texture2D vScrollThumbBottom;

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
        /// The total number of units (pixels, rows, etc.) represented by this scrollbar.
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
        /// The current scroll position in units.
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

        #endregion

        #region Constructors

        public VerticalScrollBar()
            : base()
        {
            vScrollThumbTop = Resources.Load<Texture2D>("vScrollThumbTop");
            vScrollThumbBody = Resources.Load<Texture2D>("vScrollThumbBody");
            vScrollThumbBottom = Resources.Load<Texture2D>("vScrollThumbBottom");
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
                    float scale = Size.y / (float)totalUnits;
                    float unitsMoved = dragDistance.y / scale;
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

            DrawScrollBar();
        }

        protected override void MouseClick(Vector2 clickPosition)
        {
            base.MouseClick(clickPosition);

            if (clickPosition.y < thumbRect.yMin)
                ScrollIndex -= displayUnits;
            else if (clickPosition.y > thumbRect.yMax)
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
        /// Resets scroll properties without triggering events.
        /// </summary>
        public void Reset(int displayUnits = 0, int totalUnits = 0, int scrollIndex = 0)
        {
            this.displayUnits = displayUnits;
            this.totalUnits = totalUnits;
            this.scrollIndex = scrollIndex;
        }

        public void SetScrollIndexWithoutRaisingScrollEvent(int value)
        {
            SetScrollIndex(value, true);
        }

        #endregion

            #region Private Methods

        void SetScrollIndex(int value, bool doNotRaiseScrollEvent = false)
        {
            int maxScroll = totalUnits - displayUnits;
            if (maxScroll < 0)
                maxScroll = 0;

            scrollIndex = value;

            if (scrollIndex < 0)
                scrollIndex = 0;
            if (scrollIndex > maxScroll)
                scrollIndex = maxScroll;

            if (!doNotRaiseScrollEvent)
                RaiseOnScrollEvent();
        }

        void DrawScrollBar()
        {
            // Update current thumb rect in local space
            Rect totalRect = Rectangle;
            float thumbHeight = totalRect.height * ((float)displayUnits / (float)totalUnits);
            if (thumbHeight < 10) thumbHeight = 10;
            float thumbY = scrollIndex * (totalRect.height - thumbHeight) / (totalUnits - displayUnits);
            thumbRect = ScreenToLocal(new Rect(totalRect.x, totalRect.y + thumbY, totalRect.width, thumbHeight));

            // Draw thumb texture slices in screen space
            float topTextureHeight = vScrollThumbTop.height * LocalScale.y;
            float bottomTextureHeight = vScrollThumbBottom.height * LocalScale.y;
            Rect topRect = new Rect(totalRect.x, totalRect.y + thumbY, totalRect.width, topTextureHeight);
            Rect bodyRect = new Rect(totalRect.x, topRect.yMax, totalRect.width, thumbHeight - topTextureHeight - bottomTextureHeight);
            Rect bottomRect = new Rect(totalRect.x, bodyRect.yMax, totalRect.width, bottomTextureHeight);
            GUI.DrawTexture(topRect, vScrollThumbTop, ScaleMode.StretchToFill);
            GUI.DrawTexture(bodyRect, vScrollThumbBody, ScaleMode.StretchToFill);
            GUI.DrawTexture(bottomRect, vScrollThumbBottom, ScaleMode.StretchToFill);
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