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
            FilterMode filterMode)
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
            GUI.DrawTextureWithTexCoords(fillBordersRect, fillBordersTexture, new Rect(0, 0, fillBordersRect.width / fillBordersTexture.width, fillBordersRect.height / fillBordersTexture.height));

            // Draw corners
            GUI.DrawTexture(topLeftBorderRect, topLeftBorderTexture);
            GUI.DrawTexture(topRightBorderRect, topRightBorderTexture);
            GUI.DrawTexture(bottomLeftBorderRect, bottomLeftBorderTexture);
            GUI.DrawTexture(bottomRightBorderRect, bottomRightBorderTexture);

            // Draw edges
            GUI.DrawTextureWithTexCoords(topBorderRect, topBorderTexture, new Rect(0, 0, (topBorderRect.width / LocalScale.x) / topBorderTexture.width, 1));
            GUI.DrawTextureWithTexCoords(leftBorderRect, leftBorderTexture, new Rect(0, 0, 1, (leftBorderRect.height / LocalScale.y) / leftBorderTexture.height));
            GUI.DrawTextureWithTexCoords(rightBorderRect, rightBorderTexture, new Rect(0, 0, 1, (rightBorderRect.height / LocalScale.y) / rightBorderTexture.height));
            GUI.DrawTextureWithTexCoords(bottomBorderRect, bottomBorderTexture, new Rect(0, 0, (bottomBorderRect.width / LocalScale.y) / bottomBorderTexture.width, 1));
        }

        void UpdateBorderDrawRects(Rect drawRect)
        {
            // Top-left
            topLeftBorderRect.x = drawRect.x;
            topLeftBorderRect.y = drawRect.y;
            topLeftBorderRect.width = topLeftBorderTexture.width * LocalScale.x;
            topLeftBorderRect.height = topLeftBorderTexture.height * LocalScale.y;

            // Top-right
            topRightBorderRect.x = drawRect.xMax - topRightBorderTexture.width * LocalScale.x;
            topRightBorderRect.y = drawRect.y;
            topRightBorderRect.width = topRightBorderTexture.width * LocalScale.x;
            topRightBorderRect.height = topRightBorderTexture.height * LocalScale.y;

            // Bottom-left
            bottomLeftBorderRect.x = drawRect.x;
            bottomLeftBorderRect.y = drawRect.yMax - bottomLeftBorderTexture.height * LocalScale.y;
            bottomLeftBorderRect.width = bottomLeftBorderTexture.width * LocalScale.x;
            bottomLeftBorderRect.height = bottomLeftBorderTexture.height * LocalScale.y;

            // Bottom-right
            bottomRightBorderRect.x = drawRect.xMax - bottomRightBorderTexture.width * LocalScale.x;
            bottomRightBorderRect.y = drawRect.yMax - bottomRightBorderTexture.height * LocalScale.y;
            bottomRightBorderRect.width = bottomRightBorderTexture.width * LocalScale.x;
            bottomRightBorderRect.height = bottomRightBorderTexture.height * LocalScale.y;

            // Top
            topBorderRect.x = drawRect.x + topLeftBorderTexture.width * LocalScale.x;
            topBorderRect.y = drawRect.y;
            topBorderRect.width = drawRect.width - topLeftBorderTexture.width * LocalScale.x - topRightBorderTexture.width * LocalScale.x;
            topBorderRect.height = topBorderTexture.height * LocalScale.y;

            // Left
            leftBorderRect.x = drawRect.x;
            leftBorderRect.y = drawRect.y + topLeftBorderTexture.height * LocalScale.y;
            leftBorderRect.width = leftBorderTexture.width * LocalScale.x;
            leftBorderRect.height = drawRect.height - topLeftBorderTexture.height * LocalScale.y - bottomLeftBorderTexture.height * LocalScale.y;

            // Right
            rightBorderRect.x = drawRect.xMax - rightBorderTexture.width * LocalScale.x;
            rightBorderRect.y = drawRect.y + topRightBorderTexture.height * LocalScale.y;
            rightBorderRect.width = rightBorderTexture.width * LocalScale.x;
            rightBorderRect.height = drawRect.height - topRightBorderTexture.height * LocalScale.y - bottomRightBorderTexture.height * LocalScale.y;

            // Bottom
            bottomBorderRect.x = drawRect.x + bottomLeftBorderTexture.width * LocalScale.x;
            bottomBorderRect.y = drawRect.yMax - bottomBorderTexture.height * LocalScale.y;
            bottomBorderRect.width = drawRect.width - bottomLeftBorderTexture.width * LocalScale.x - bottomRightBorderTexture.width * LocalScale.x;
            bottomBorderRect.height = bottomBorderTexture.height * LocalScale.y;

            // Fill
            fillBordersRect.xMin = drawRect.xMin + leftBorderTexture.width * LocalScale.x;
            fillBordersRect.yMin = drawRect.yMin + topBorderTexture.height * LocalScale.y;
            fillBordersRect.xMax = drawRect.xMax - rightBorderTexture.width * LocalScale.x;
            fillBordersRect.yMax = drawRect.yMax - bottomBorderTexture.height * LocalScale.y;
        }

        #endregion
    }
}
