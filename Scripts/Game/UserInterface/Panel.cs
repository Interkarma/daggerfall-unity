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

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A screen component that contains other screen components.
    /// </summary>
    public class Panel : BaseScreenComponent
    {
        bool bordersSet = false;
        Texture2D topBorder, bottomBorder, leftBorder, rightBorder;
        Texture2D tlBorder, trBorder, blBorder, brBorder;

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

        public Panel()
            :base()
        {
            this.components = new ScreenComponentCollection(this);
        }

        public override void Update()
        {
            if (!Enabled)
                return;

            base.Update();

            // Update child components
            foreach (BaseScreenComponent component in components)
            {
                if (component.Enabled)
                    component.Update();
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
            Texture2D right,
            Texture2D bottomLeft,
            Texture2D bottom,
            Texture2D bottomRight)
        {
            // Save texture references
            tlBorder = topLeft;
            topBorder = top;
            trBorder = topRight;
            leftBorder = left;
            rightBorder = right;
            blBorder = bottomLeft;
            bottomBorder = bottom;
            brBorder = bottomRight;

            // Set flags
            bordersSet = true;
            EnableBorder = true;
        }

        #region Private Methods

        /// <summary>
        /// Draws border using textures provided.
        /// </summary>
        private void DrawBorder()
        {
            // Set linear wrap
            //spriteBatch.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // Get draw area
            Rect drawRect = Rectangle;

            // Top-left
            Vector2 tl;
            tl.x = drawRect.x;
            tl.y = drawRect.y;

            // Top-right
            Vector2 tr;
            tr.x = drawRect.xMax - trBorder.width;
            tr.y = drawRect.y;

            // Bottom-left
            Vector2 bl;
            bl.x = drawRect.x;
            bl.y = drawRect.yMin - brBorder.height;

            // Bottom-right
            Vector2 br;
            br.x = drawRect.xMax - brBorder.width;
            br.y = drawRect.yMax - brBorder.height;

            // Top
            Rect top = new Rect();
            top.x = drawRect.x + tlBorder.width;
            top.y = drawRect.y;
            top.width = drawRect.width - tlBorder.width - trBorder.width;
            top.height = topBorder.height;

            // Left
            Rect left = new Rect();
            left.x = drawRect.x;
            left.y = drawRect.y + tlBorder.height;
            left.width = leftBorder.width;
            left.height = drawRect.height - tlBorder.height - blBorder.height;

            // Right
            Rect right = new Rect();
            right.x = drawRect.xMax - rightBorder.width;
            right.y = drawRect.y + trBorder.height;
            right.width = rightBorder.width;
            right.height = drawRect.height - trBorder.height - brBorder.height;

            // Bottom
            Rect bottom = new Rect();
            bottom.x = drawRect.x + blBorder.width;
            bottom.y = drawRect.yMax - bottomBorder.height;
            bottom.width = drawRect.width - blBorder.width - brBorder.width;
            bottom.height = bottomBorder.height;

            // Draw corners
            //spriteBatch.Draw(tlBorder, tl, Color.White);
            //spriteBatch.Draw(trBorder, tr, Color.White);
            //spriteBatch.Draw(blBorder, bl, Color.White);
            //spriteBatch.Draw(brBorder, br, Color.White);

            // Draw edges
            //spriteBatch.Draw(topBorder, top, top, Color.White);
            //spriteBatch.Draw(leftBorder, left, left, Color.White);
            //spriteBatch.Draw(rightBorder, right, right, Color.White);
            //spriteBatch.Draw(bottomBorder, bottom, bottom, Color.White);
        }

        #endregion
    }
}
