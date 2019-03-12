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
using System.Text;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A screen component that contains other screen components.
    /// </summary>
    public class Panel : BaseScreenComponent
    {
        bool bordersSet = false;
        Texture2D fillBordersTexture;
        Texture2D topBorderTexture, bottomBorderTexture;
        Texture2D leftBorderTexture, rightBorderTexture;
        Texture2D topLeftBorderTexture, topRightBorderTexture;
        Texture2D bottomLeftBorderTexture, bottomRightBorderTexture;
        Border<Vector2Int> virtualSizes;

        Rect lastDrawRect;
        Rect fillBordersRect = new Rect();
        Rect topLeftBorderRect = new Rect();
        Rect topRightBorderRect = new Rect();
        Rect bottomLeftBorderRect = new Rect();
        Rect bottomRightBorderRect = new Rect();
        Rect topBorderRect = new Rect();
        Rect leftBorderRect = new Rect();
        Rect rightBorderRect = new Rect();
        Rect bottomBorderRect = new Rect();

        Outline outline = new Outline();
        Color hasFocusOutlineColor = Color.yellow;
        Color lostFocusOutlineColor = Color.gray;

        ScreenComponentCollection components;
        public ScreenComponentCollection Components
        {
            get { return components; }
        }

        /// <summary>
        /// Gets or sets flag to enable/disable border.
        ///  Must use SetBorderTextures() before enabling border.
        /// </summary>
        public bool EnableBorder { get; set; }

        /// <summary>
        /// Automatically change outline color when gaining focus.
        /// Only works when UseFocus true.
        /// </summary>
        public Color HasFocusOutlineColor
        {
            get { return hasFocusOutlineColor; }
            set { hasFocusOutlineColor = value; }
        }

        /// <summary>
        /// Automatically change outline color when losing focus.
        /// Only works when UseFocus true.
        /// </summary>
        public Color LostFocusOutlineColor
        {
            get { return lostFocusOutlineColor; }
            set { lostFocusOutlineColor = value; }
        }

        public Outline Outline
        {
            get { return outline; }
        }

        public Panel()
            :base()
        {
            this.components = new ScreenComponentCollection(this);
            this.components.Add(outline);

            outline.AutoSize = AutoSizeModes.ResizeToFill;
            outline.Enabled = false;
        }

        public override void Update()
        {
            if (!Enabled)
                return;

            base.Update();

            // Update child components
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].Enabled)
                {
                    Components[i].CustomMousePosition = CustomMousePosition;
                    Components[i].Update();
                }
            }
        }

        public override void Draw()
        {
            if (!Enabled)
                return;

            base.Draw();

            // Draw border
            if (EnableBorder && bordersSet)
                DrawBorder();

            // Draw child components
            foreach (BaseScreenComponent component in components)
            {
                if (component.Enabled)
                    component.Draw();
            }
        }

        /// <summary>
        /// Sets border textures and enables border.
        /// </summary>
        public void SetBorderTextures(
            Texture2D topLeft,
            Texture2D top,
            Texture2D topRight,
            Texture2D left,
            Texture2D fill,
            Texture2D right,
            Texture2D bottomLeft,
            Texture2D bottom,
            Texture2D bottomRight,
            FilterMode filterMode,
            Border<Vector2Int>? virtualSizes = null)
        {
            // Save texture references
            topLeftBorderTexture = topLeft;
            topBorderTexture = top;
            topRightBorderTexture = topRight;
            leftBorderTexture = left;
            fillBordersTexture = fill;
            rightBorderTexture = right;
            bottomLeftBorderTexture = bottomLeft;
            bottomBorderTexture = bottom;
            bottomRightBorderTexture = bottomRight;

            // Set texture filtering
            topLeftBorderTexture.filterMode = filterMode;
            topBorderTexture.filterMode = filterMode;
            topRightBorderTexture.filterMode = filterMode;
            leftBorderTexture.filterMode = filterMode;
            fillBordersTexture.filterMode = filterMode;
            rightBorderTexture.filterMode = filterMode;
            bottomLeftBorderTexture.filterMode = filterMode;
            bottomBorderTexture.filterMode = filterMode;
            bottomRightBorderTexture.filterMode = filterMode;

            // Set texture wrap modes
            topBorderTexture.wrapMode = TextureWrapMode.Repeat;
            bottomBorderTexture.wrapMode = TextureWrapMode.Repeat;
            leftBorderTexture.wrapMode = TextureWrapMode.Repeat;
            rightBorderTexture.wrapMode = TextureWrapMode.Repeat;
            fillBordersTexture.wrapMode = TextureWrapMode.Repeat;

            // Set flags
            bordersSet = true;
            EnableBorder = true;

            // Set sizes
            this.virtualSizes = virtualSizes ?? new Border<Vector2Int>()
            {
                TopLeft = new Vector2Int(topLeft.width, topLeft.height),
                Top = new Vector2Int(top.width, top.height),
                TopRight = new Vector2Int(topRight.width, topRight.height),
                Left = new Vector2Int(left.width, left.height),
                Fill = new Vector2Int(fill.width, fill.height),
                Right = new Vector2Int(right.width, right.height),
                BottomLeft = new Vector2Int(bottomLeft.width, bottomLeft.height),
                Bottom = new Vector2Int(bottom.width, bottom.height),
                BottomRight = new Vector2Int(bottomRight.width, bottomRight.height)
            };
        }

        public override void GotFocus()
        {
            if (UseFocus)
            {
                base.GotFocus();
                Outline.Color = HasFocusOutlineColor;
            }
        }

        public override void LostFocus()
        {
            if (UseFocus)
            {
                base.LostFocus();
                Outline.Color = LostFocusOutlineColor;
            }
        }

        #region Private Methods

        /// <summary>
        /// Draws border using textures provided.
        /// </summary>
        void DrawBorder()
        {
            Rect drawRect = Rectangle;
            if (drawRect != lastDrawRect)
            {
                UpdateBorderDrawRects(drawRect);
                lastDrawRect = drawRect;
            }

            // Draw fill
            GUI.DrawTextureWithTexCoords(fillBordersRect, fillBordersTexture, new Rect(0, 0, fillBordersRect.width / virtualSizes.Fill.x, fillBordersRect.height / virtualSizes.Fill.y));

            // Draw corners
            GUI.DrawTexture(topLeftBorderRect, topLeftBorderTexture);
            GUI.DrawTexture(topRightBorderRect, topRightBorderTexture);
            GUI.DrawTexture(bottomLeftBorderRect, bottomLeftBorderTexture);
            GUI.DrawTexture(bottomRightBorderRect, bottomRightBorderTexture);

            // Draw edges
            GUI.DrawTextureWithTexCoords(topBorderRect, topBorderTexture, new Rect(0, 0, (topBorderRect.width / LocalScale.x) / virtualSizes.Top.x, 1));
            GUI.DrawTextureWithTexCoords(leftBorderRect, leftBorderTexture, new Rect(0, 0, 1, (leftBorderRect.height / LocalScale.y) / virtualSizes.Left.y));
            GUI.DrawTextureWithTexCoords(rightBorderRect, rightBorderTexture, new Rect(0, 0, 1, (rightBorderRect.height / LocalScale.y) / virtualSizes.Right.y));
            GUI.DrawTextureWithTexCoords(bottomBorderRect, bottomBorderTexture, new Rect(0, 0, (bottomBorderRect.width / LocalScale.y) / virtualSizes.Bottom.x, 1));
        }

        void UpdateBorderDrawRects(Rect drawRect)
        {
            // Top-left
            topLeftBorderRect.x = drawRect.x;
            topLeftBorderRect.y = drawRect.y;
            topLeftBorderRect.width = virtualSizes.TopLeft.x * LocalScale.x;
            topLeftBorderRect.height = virtualSizes.TopLeft.y * LocalScale.y;

            // Top-right
            topRightBorderRect.x = drawRect.xMax - virtualSizes.TopRight.x * LocalScale.x;
            topRightBorderRect.y = drawRect.y;
            topRightBorderRect.width = virtualSizes.TopRight.x * LocalScale.x;
            topRightBorderRect.height = virtualSizes.TopRight.y * LocalScale.y;

            // Bottom-left
            bottomLeftBorderRect.x = drawRect.x;
            bottomLeftBorderRect.y = drawRect.yMax - virtualSizes.BottomLeft.x * LocalScale.y;
            bottomLeftBorderRect.width = virtualSizes.BottomLeft.x * LocalScale.x;
            bottomLeftBorderRect.height = virtualSizes.BottomLeft.y * LocalScale.y;

            // Bottom-right
            bottomRightBorderRect.x = drawRect.xMax - virtualSizes.BottomRight.x * LocalScale.x;
            bottomRightBorderRect.y = drawRect.yMax - virtualSizes.BottomRight.y * LocalScale.y;
            bottomRightBorderRect.width = virtualSizes.BottomRight.x * LocalScale.x;
            bottomRightBorderRect.height = virtualSizes.BottomRight.y * LocalScale.y;

            // Top
            topBorderRect.x = drawRect.x + virtualSizes.TopLeft.x * LocalScale.x;
            topBorderRect.y = drawRect.y;
            topBorderRect.width = drawRect.width - virtualSizes.TopLeft.x * LocalScale.x - virtualSizes.TopRight.x * LocalScale.x;
            topBorderRect.height = virtualSizes.Top.y * LocalScale.y;

            // Left
            leftBorderRect.x = drawRect.x;
            leftBorderRect.y = drawRect.y + virtualSizes.TopLeft.y * LocalScale.y;
            leftBorderRect.width = virtualSizes.Left.x * LocalScale.x;
            leftBorderRect.height = drawRect.height - virtualSizes.TopLeft.y * LocalScale.y - virtualSizes.BottomLeft.y * LocalScale.y;

            // Right
            rightBorderRect.x = drawRect.xMax - virtualSizes.Right.x * LocalScale.x;
            rightBorderRect.y = drawRect.y + virtualSizes.TopRight.y * LocalScale.y;
            rightBorderRect.width = virtualSizes.Right.x * LocalScale.x;
            rightBorderRect.height = drawRect.height - virtualSizes.TopRight.y * LocalScale.y - virtualSizes.BottomRight.y * LocalScale.y;

            // Bottom
            bottomBorderRect.x = drawRect.x + virtualSizes.BottomLeft.x * LocalScale.x;
            bottomBorderRect.y = drawRect.yMax - virtualSizes.Bottom.y * LocalScale.y;
            bottomBorderRect.width = drawRect.width - virtualSizes.BottomLeft.x * LocalScale.x - virtualSizes.BottomRight.x * LocalScale.x;
            bottomBorderRect.height = virtualSizes.Bottom.y * LocalScale.y;

            // Fill
            fillBordersRect.xMin = drawRect.xMin + virtualSizes.Left.x * LocalScale.x;
            fillBordersRect.yMin = drawRect.yMin + virtualSizes.Top.y * LocalScale.y;
            fillBordersRect.xMax = drawRect.xMax - virtualSizes.Right.x * LocalScale.x;
            fillBordersRect.yMax = drawRect.yMax - virtualSizes.Bottom.y * LocalScale.y;
        }

        #endregion
    }
}
