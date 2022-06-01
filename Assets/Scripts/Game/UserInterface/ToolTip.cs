// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
    /// Displays popup tooltip text when mouse hovers over component for an amount of time.
    /// Designed to be shared by any number of components inside the same canvas area.
    /// Use \r escape character to create multi-row tooltips.
    /// </summary>
    public class ToolTip : BaseScreenComponent
    {
        #region Fields

        const int defaultMarginSize = 2;

        DaggerfallFont font;
        float toolTipDelay = 0;
        Vector2 mouseOffset = new Vector2(0, 4);
        private int currentCursorHeight = -1;
        private int currentSystemHeight;
        private int currentRenderingHeight;
        private bool currentFullScreen;

        bool drawToolTip = false;
        Color textColor = DaggerfallUI.DaggerfallUnityDefaultToolTipTextColor;

        string[] textRows;
        float widestRow = 0;
        string lastText = string.Empty;
        bool previousSDFState;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets font used inside tooltip.
        /// </summary>
        public DaggerfallFont Font
        {
            get { return font; }
            set { font = value; }
        }

        /// <summary>
        /// Sets delay time in seconds before tooltip is displayed.
        /// </summary>
        public float ToolTipDelay
        {
            get { return toolTipDelay; }
            set { toolTipDelay = value; }
        }

        /// <summary>
        /// Gets or sets tooltip draw position relative to mouse.
        /// </summary>
        public Vector2 MouseOffset
        {
            get { return mouseOffset; }
            set { mouseOffset = value; }
        }

        /// <summary>
        /// Gets or sets tooltip text colour.
        /// </summary>
        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        #endregion

        #region Constructors

        public ToolTip()
        {
            font = DaggerfallUI.DefaultFont;
            BackgroundColor = DaggerfallUI.DaggerfallUnityDefaultToolTipBackgroundColor;
            SetMargins(Margins.All, defaultMarginSize);
        }

        #endregion

        #region Public Methods

        public override void Update()
        {
            base.Update();
            if (DaggerfallUnity.Settings.CursorHeight != currentCursorHeight ||
                Display.main.systemHeight != currentSystemHeight ||
                Display.main.renderingHeight != currentRenderingHeight || 
                DaggerfallUnity.Settings.Fullscreen != currentFullScreen)
                UpdateMouseOffset();
        }

        private void UpdateMouseOffset()
        {
            currentCursorHeight = DaggerfallUnity.Settings.CursorHeight;
            currentSystemHeight = Display.main.systemHeight;
            currentRenderingHeight = Display.main.renderingHeight;
            currentFullScreen = DaggerfallUnity.Settings.Fullscreen;
            mouseOffset = new Vector2(0, currentCursorHeight * 200f / (currentFullScreen ? currentSystemHeight : currentRenderingHeight));
        }

        /// <summary>
        /// Flags tooltip to be drawn at end of UI update.
        /// </summary>
        /// <param name="text">Text to render inside tooltip.</param>
        public void Draw(string text)
        {
            // Validate
            if (font == null || string.IsNullOrEmpty(text))
            {
                drawToolTip = false;
                return;
            }

            // Update text rows
            UpdateTextRows(text);
            if (textRows == null || textRows.Length == 0)
            {
                drawToolTip = false;
                return;
            }

            // Set tooltip size
            Size = new Vector2(
                widestRow + LeftMargin + RightMargin,
                font.GlyphHeight * textRows.Length + TopMargin + BottomMargin - 1);

            // Set tooltip position
            Position = Parent.ScaledMousePosition + MouseOffset;

            // Ensure tooltip inside screen area
            Rect rect = Rectangle;
            if (rect.xMax > Screen.width)
            {
                float difference = (rect.xMax - Screen.width) * 1f / LocalScale.x;
                Vector2 newPosition = new Vector2(Position.x - difference, Position.y);
                Position = newPosition;
            }
            if (rect.yMax > Screen.height)
            {
                float difference = (rect.yMax - Screen.height) * 1f / LocalScale.y;
                Vector2 newPosition = new Vector2(Position.x, Position.y - difference);
                Position = newPosition;
            }

            // Check if mouse position is in parent's rectangle (to prevent tooltips out of panel's rectangle to be displayed)
            if (Parent != null && (Parent.Rectangle.Contains(Parent.MousePosition)))
            {
                // Raise flag to draw tooltip
                drawToolTip = true;
            }
        }

        public override void Draw()
        {
            if (!Enabled)
                return;

            if (drawToolTip)
            {
                base.Draw();

                // Set render area for tooltip to whole screen (material might have been changed by other component, i.e. _ScissorRect might have been set to a subarea of screen (e.g. by TextLabel class))
                Material material = font.GetMaterial();
                Vector4 scissorRect = new Vector4(0, 1, 0, 1);
                material.SetVector("_ScissorRect", scissorRect);

                // Determine text position
                Rect rect = Rectangle;
                Vector2 textPos = new Vector2(
                    rect.x + LeftMargin * LocalScale.x,
                    rect.y + TopMargin * LocalScale.y);

                //if (rect.xMax > Screen.width) textPos.x -= (rect.xMax - Screen.width);

                // Draw tooltip text
                for (int i = 0; i < textRows.Length; i++)
                {
                    font.DrawText(textRows[i], textPos, LocalScale, textColor);
                    textPos.y += font.GlyphHeight * LocalScale.y;
                }

                // Lower flag
                drawToolTip = false;
            }
        }

        /// <summary>
        /// Copies key setting from another tooltip.
        /// </summary>
        /// <param name="from"></param>
        public void CloneSettings(ToolTip from)
        {
            Font = from.Font;
            ToolTipDelay = from.ToolTipDelay;
            MouseOffset = from.MouseOffset;
            TextColor = from.TextColor;
            BackgroundColor = from.BackgroundColor;
            Parent = from.Parent;
        }

        #endregion

        #region Private Methods

        void UpdateTextRows(string text)
        {
            // Do nothing if text has not changed since last time
            bool sdfState = font.IsSDFCapable;
            if (text == lastText && sdfState == previousSDFState)
                return;

            // Split into rows based on \r escape character
            // Text read from plain-text files will become \\r so need to replace this first
            text = text.Replace("\\r", "\r");
            textRows = text.Split('\r');

            // Set text we just processed
            lastText = text;

            // Find widest row
            widestRow = 0;
            for (int i = 0; i < textRows.Length; i++)
            {
                float width = font.CalculateTextWidth(textRows[i], LocalScale);
                if (width > widestRow)
                    widestRow = width;
            }
            previousSDFState = sdfState;
        }

        #endregion
    }
}