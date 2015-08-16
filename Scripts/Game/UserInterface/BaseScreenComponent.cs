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
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Provides features common to all screen components.
    /// </summary>
    public abstract class BaseScreenComponent : IDisposable
    {
        #region Fields

        const int colorTextureDim = 32;

        bool enabled;
        string name;
        object tag;
        BaseScreenComponent parent;
        Vector2 position;
        Vector2 size;

        Vector2 localScale = Vector2.one;
        Scaling scaling = Scaling.None;
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.None;
        VerticalAlignment verticalAlignment = VerticalAlignment.None;

        float doubleClickTime = 0.3f;
        float firstClickTime;

        Vector2 lastMousePosition;
        Vector2 mousePosition;

        Color backgroundColor = Color.clear;
        Texture2D backgroundColorTexture;
        protected Texture2D backgroundTexture;
        protected TextureLayout backgroundTextureLayout = TextureLayout.Tile;

        bool mouseOverComponent = false;

        public delegate void OnMouseEnterHandler();
        public event OnMouseEnterHandler OnMouseEnter;

        public delegate void OnMouseLeaveHandler();
        public event OnMouseLeaveHandler OnMouseLeave;

        public delegate void OnMouseClickHandler(Vector2 position);
        public event OnMouseClickHandler OnMouseClick;

        public delegate void OnMouseDoubleClickHandler(Vector2 position);
        public event OnMouseDoubleClickHandler OnMouseDoubleClick;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets enabled flag.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets custom tag.
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        /// Gets position relative to parent panel.
        /// </summary>
        public virtual Vector2 Position
        {
            get { return position; }
            internal set { position = value;}
        }

        /// <summary>
        /// Gets size of component.
        /// </summary>
        public virtual Vector2 Size
        {
            get { return size; }
            internal set { size = value; }
        }

        /// <summary>
        /// Gets parent panel.
        /// </summary>
        public virtual BaseScreenComponent Parent
        {
            get { return parent; }
            internal set { SetParent(value); }
        }

        /// <summary>
        /// Gets screen area occupied by component.
        /// </summary>
        public Rect Rectangle
        {
            get { return GetRectangle(); }
        }

        /// <summary>
        /// Gets or sets horizontal alignment.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return horizontalAlignment; }
            set { horizontalAlignment = value; }
        }

        /// <summary>
        /// Gets or sets vertical alignment.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return verticalAlignment; }
            set { verticalAlignment = value; }
        }

        /// <summary>
        /// Gets current mouse position from recent update.
        /// </summary>
        public Vector2 MousePosition
        {
            get { return mousePosition; }
        }

        /// <summary>
        /// Gets previous mouse position from last update.
        /// </summary>
        public Vector2 LastMousePosition
        {
            get { return lastMousePosition; }
        }

        /// <summary>
        /// Gets or sets background colour.
        /// </summary>
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { SetBackgroundColor(value); }
        }

        /// <summary>
        /// Gets or sets background texture.
        ///  Will replace BackgroundColor if set.
        /// </summary>
        public Texture2D BackgroundTexture
        {
            get { return backgroundTexture; }
            set { backgroundTexture = value; }
        }

        /// <summary>
        /// Gets or sets background texture layout behaviour.
        /// </summary>
        public TextureLayout BackgroundTextureLayout
        {
            get { return backgroundTextureLayout; }
            set { backgroundTextureLayout = value; }
        }

        /// <summary>
        /// Gets interior width between horiztonal margins
        /// </summary>
        public int InteriorWidth
        {
            get { return GetInteriorWidth(); }
        }

        /// <summary>
        /// Gets interior height between vertical margins
        /// </summary>
        public int InteriorHeight
        {
            get { return GetInteriorHeight(); }
        }

        /// <summary>
        /// Gets or sets scaling.
        /// </summary>
        public Scaling Scaling
        {
            get { return scaling; }
            set { scaling = value; }
        }

        /// <summary>
        /// Gets scale value based on scaling.
        /// </summary>
        public Vector2 LocalScale
        {
            get { return localScale; }
        }

        // Margin properties
        public int TopMargin { get; set; }
        public int BottomMargin { get; set; }
        public int LeftMargin { get; set; }
        public int RightMargin { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public BaseScreenComponent()
        {
            this.enabled = true;
            this.tag = null;
            this.position = Vector2.zero;
            this.size = Vector2.zero;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when screen component should update itself.
        /// </summary>
        public virtual void Update()
        {
            // Do nothing if disabled
            if (!enabled)
                return;

            // Ensure position in inside margins
            FitMargins();

            // Update mouse pos - must to invert mouse position Y as Unity 0,0 is bottom-left
            lastMousePosition = mousePosition;
            mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

            // Check if mouse is inside rectangle
            Rect myRect = Rectangle;
            if (myRect.Contains(mousePosition))
            {
                if (mouseOverComponent == false)
                {
                    // Raise mouse entered event
                    if (OnMouseEnter != null)
                        OnMouseEnter();

                    mouseOverComponent = true;
                }
            }
            else
            {
                if (mouseOverComponent == true)
                {
                    // Raise mouse leaving event
                    if (OnMouseLeave != null)
                        OnMouseLeave();

                    mouseOverComponent = false;
                }
            }

            // Handle left mouse click
            bool leftMouseDown = Input.GetMouseButtonDown(0);
            if (mouseOverComponent && leftMouseDown)
            {
                // Calculate scaled click position relative to top-left corner
                Vector2 topLeft = new Vector2(myRect.xMin, myRect.yMin);
                Vector2 clickPosition = mousePosition - topLeft;
                clickPosition.x *= 1f / localScale.x;
                clickPosition.y *= 1f / localScale.y;

                // Single click event
                if (OnMouseClick != null)
                    OnMouseClick(clickPosition);

                // Double-click event
                if (firstClickTime == 0)
                {
                    firstClickTime = Time.time;
                }
                else
                {
                    float nextClickTime = Time.time;
                    if (nextClickTime - firstClickTime < doubleClickTime)
                    {
                        firstClickTime = 0;
                        if (OnMouseDoubleClick != null)
                            OnMouseDoubleClick(clickPosition);
                    }
                    else
                    {
                        firstClickTime = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Called when screen component should draw itself.
        /// </summary>
        public virtual void Draw()
        {
            // Do nothing if disabled
            if (!enabled)
                return;

            // Draw background
            Rect myRect = Rectangle;
            if (backgroundTexture)
            {
                switch (backgroundTextureLayout)
                {
                    case TextureLayout.Tile:
                        backgroundTexture.wrapMode = TextureWrapMode.Repeat;
                        GUI.DrawTextureWithTexCoords(Rectangle, backgroundTexture, new Rect(0, 0, myRect.width / backgroundTexture.width, myRect.height / backgroundTexture.height));
                        break;
                    case TextureLayout.StretchToFill:
                        backgroundTexture.wrapMode = TextureWrapMode.Clamp;
                        GUI.DrawTexture(Rectangle, backgroundTexture, ScaleMode.StretchToFill);
                        break;
                    case TextureLayout.ScaleToFit:
                        backgroundTexture.wrapMode = TextureWrapMode.Clamp;
                        GUI.DrawTexture(Rectangle, backgroundTexture, ScaleMode.ScaleToFit);
                        break;
                }
            }
            else if (backgroundColor != Color.clear && backgroundColorTexture != null)
            {
                GUI.DrawTexture(Rectangle, backgroundColorTexture, ScaleMode.StretchToFill);
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Called when component is to be disposed.
        ///  Override if special handling needed
        ///  to dispose of component resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets value for one or more margins using flags.
        /// </summary>
        /// <param name="margin">Margin flags.</param>
        /// <param name="value">Value to set.</param>
        public void SetMargins(Margins margin, int value)
        {
            if ((margin & Margins.Top) == Margins.Top)
                TopMargin = value;
            if ((margin & Margins.Bottom) == Margins.Bottom)
                BottomMargin = value;
            if ((margin & Margins.Left) == Margins.Left)
                LeftMargin = value;
            if ((margin & Margins.Right) == Margins.Right)
                RightMargin = value;
        }

        /// <summary>
        /// Offsets position of component relative to another component.
        /// </summary>
        /// <param name="component">Component to offset against.</param>
        /// <param name="side">The side of the component to offset from.</param>
        /// <param name="distance">Distance between offset components.</param>
        public void OffsetFrom(BaseScreenComponent component, Sides side, int distance)
        {
            // Exit if invalid offset
            if (component == null || side == Sides.None)
                return;

            // Get rectangles
            Rect myRect = Rectangle;
            Rect otherRect = component.Rectangle;

            // Offset based on side
            switch (side)
            {
                case Sides.Left:
                    position.x = otherRect.xMin - distance - myRect.width;
                    break;
                case Sides.Right:
                    position.x = otherRect.xMax + distance;
                    break;
                case Sides.Top:
                    position.y = otherRect.yMin - distance - myRect.height;
                    break;
                case Sides.Bottom:
                    position.y = otherRect.yMax + distance;
                    break;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets parent panel.
        /// Should only be used at setup, never to move components between collections.
        /// </summary>
        /// <param name="parent">Parent.</param>
        private void SetParent(BaseScreenComponent parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Helper to get parent rectangle.
        /// </summary>
        /// <returns>Parent's absolute rectangle in viewport area.</returns>
        private Rect GetParentRectangle()
        {
            Rect parentRect = new Rect();

            if (parent != null)
                parentRect = parent.Rectangle;
            else
                parentRect = new Rect(0, 0, Screen.width, Screen.height);

            return parentRect;
        }

        /// <summary>
        /// Gets current screen space rectangle.
        /// </summary>
        /// <returns>Absolute rectangle in viewport area.</returns>
        private Rect GetRectangle()
        {
            Rect rectangle = new Rect();

            // Apply starting position and size
            Rect parentRect = GetParentRectangle();
            if (parent == null)
            {
                // Top-level panel always stretches to fill entire viewport
                rectangle = parentRect;
            }
            else
            {
                // Other panels are scaled within parent area
                rectangle.x = (int)parentRect.xMin;
                rectangle.y = (int)parentRect.yMin;
                rectangle.width = (int)size.x;
                rectangle.height = (int)size.y;
            }

            // Apply scaling
            switch (scaling)
            {
                case Scaling.None:
                    localScale = (parent != null) ? parent.LocalScale : Vector2.one;
                    break;
                case Scaling.StretchToFill:
                    rectangle = StretchToFill(rectangle);
                    break;
                case Scaling.ScaleToFit:
                    rectangle = ScaleToFit(rectangle);
                    break;
            }

            if (parent != null)
            {
                // Scale to parent
                Vector2 parentScale = parent.LocalScale;
                rectangle.x += position.x * parentScale.x;
                rectangle.y += position.y * parentScale.y;
                rectangle.width *= parentScale.x;
                rectangle.height *= parentScale.y;

                // Apply horizontal alignment
                switch (horizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        rectangle.x = parentRect.xMin + Parent.LeftMargin;
                        break;
                    case HorizontalAlignment.Right:
                        rectangle.x = parentRect.xMax - Parent.RightMargin - rectangle.width;
                        break;
                    case HorizontalAlignment.Center:
                        rectangle.x = parentRect.xMin + parentRect.width / 2 - rectangle.width / 2;
                        break;
                }

                // Set vertical position based on alignment
                switch (verticalAlignment)
                {
                    case VerticalAlignment.Top:
                        rectangle.y = parentRect.yMin + Parent.TopMargin;
                        break;
                    case VerticalAlignment.Bottom:
                        rectangle.y = parentRect.yMax - Parent.BottomMargin - rectangle.height;
                        break;
                    case VerticalAlignment.Middle:
                        rectangle.y = parentRect.yMin + parentRect.height / 2 - rectangle.height / 2;
                        break;
                }
            }

            return rectangle;
        }

        /// <summary>
        /// Gets interior width between horizontal margins.
        /// </summary>
        private int GetInteriorWidth()
        {
            return (int)Rectangle.width - LeftMargin - RightMargin;
        }

        /// <summary>
        /// Gets interior height between vertical margins
        /// </summary>
        private int GetInteriorHeight()
        {
            return (int)Rectangle.height - TopMargin - BottomMargin;
        }

        /// <summary>
        /// Force component to always fit inside margins when smaller than parent.
        /// </summary>
        private void FitMargins()
        {
            if (parent != null)
            {
                Rect parentRect = GetParentRectangle();
                Vector2 parentScale = parent.LocalScale;

                // Ensure horizontal position is inside margins
                if (size.x < parentRect.width)
                {
                    if (position.x < parent.LeftMargin)
                        position.x = parent.LeftMargin;
                    if (position.x > parentRect.width - parent.RightMargin)
                        position.x = parentRect.width - parent.RightMargin - size.x * parentScale.x;
                }

                // Ensure vertical position is inside margins
                if (size.y < parentRect.height)
                {
                    if (position.y < parent.TopMargin)
                        position.y = parent.TopMargin;
                    if (position.y > parentRect.height - parent.BottomMargin)
                        position.y = parentRect.height - parent.BottomMargin - size.y * parentScale.y;
                }
            }
        }

        /// <summary>
        /// Stretch to fill parent.
        /// </summary>
        private Rect StretchToFill(Rect myRect)
        {
            Rect finalRect = myRect;

            if (parent != null)
            {
                Rect parentRect = GetParentRectangle();
                finalRect.xMin = parentRect.xMin + parent.LeftMargin;
                finalRect.xMax = parentRect.xMax - parent.RightMargin;
                finalRect.yMin = parentRect.yMin + parent.TopMargin;
                finalRect.yMax = parentRect.yMax - parent.BottomMargin;

                localScale.x = finalRect.width / myRect.width;
                localScale.y = finalRect.height / myRect.height;
            }

            return finalRect;
        }

        /// <summary>
        /// Scale to fit parent while maintaining aspect ratio.
        /// </summary>
        private Rect ScaleToFit(Rect myRect)
        {
            Rect finalRect = myRect;

            if (parent != null)
            {
                int parentWidth = parent.InteriorWidth;
                int parentHeight = parent.InteriorHeight;

                float scale;
                if (parentWidth > parentHeight)
                    scale = parentHeight / size.y;
                else
                    scale = parentWidth / size.x;

                finalRect.width *= scale;
                finalRect.height *= scale;

                localScale.x = localScale.y = scale;
            }

            return finalRect;
        }

        /// <summary>
        /// Sets background colour and updates texture.
        /// </summary>
        /// <param name="color">Color to use as background colour.</param>
        private void SetBackgroundColor(Color color)
        {
            backgroundColor = color;
            backgroundColorTexture = new Texture2D(colorTextureDim, colorTextureDim);
            Color32[] colors = new Color32[colorTextureDim * colorTextureDim];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
            }
            backgroundColorTexture.SetPixels32(colors);
            backgroundColorTexture.Apply(false, true);
            backgroundColorTexture.filterMode = FilterMode.Point;
        }

        #endregion
    }
}
