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
    /// A simple single-line text label using a pixel font.
    /// </summary>
    public class TextLabel : BaseScreenComponent
    {
        PixelFont font;
        string text = string.Empty;
        byte[] asciiBytes;
        int totalWidth;
        int totalHeight;
        Texture2D labelTexture;
        int glyphSpacing = 1;
        Vector2 shadowPosition = Vector2.zero;
        Color textColor = Color.white;
        Color shadowColor = Color.black;
        FilterMode filterMode = FilterMode.Point;

        public PixelFont Font
        {
            get { return font; }
            set { font = value; }
        }

        public string Text
        {
            get { return text; }
            set { SetText(value); }
        }

        public int GlyphSpacing
        {
            get { return glyphSpacing; }
            set { SetGlyphSpacing(value); }
        }

        public Vector2 ShadowPosition
        {
            get { return shadowPosition; }
            set { shadowPosition = value; }
        }

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        public Color ShadowColor
        {
            get { return shadowColor; }
            set { shadowColor = value; }
        }

        public Texture2D Texture
        {
            get { return labelTexture; }
        }

        public FilterMode FilterMode
        {
            get { return filterMode; }
            set { SetFilterMode(value); }
        }

        public override void Draw()
        {
            base.Draw();

            if (string.IsNullOrEmpty(text) || labelTexture == null)
                return;

            // Store starting colour
            Color guiColor = GUI.color;

            // Draw shadow
            Rect rect = Rectangle;
            if (shadowPosition != Vector2.zero)
            {
                Rect shadowRect = rect;
                shadowRect.x += shadowPosition.x * LocalScale.x;
                shadowRect.y += shadowPosition.y * LocalScale.y;
                GUI.color = shadowColor;
                GUI.DrawTexture(shadowRect, labelTexture, ScaleMode.StretchToFill);
            }

            // Draw text
            GUI.color = textColor;
            GUI.DrawTexture(rect, labelTexture, ScaleMode.StretchToFill);

            // Restore starting colour
            GUI.color = guiColor;
        }

        #region Private Methods

        void SetGlyphSpacing(int value)
        {
            this.glyphSpacing = value;
            CreateLabelTexture();
        }

        void SetText(string value)
        {
            this.text = value;
            CreateLabelTexture();
        }

        void SetFilterMode(FilterMode value)
        {
            filterMode = value;
            if (labelTexture)
                labelTexture.filterMode = filterMode;
        }

        void CreateLabelTexture()
        {
            if (font == null)
                font = DaggerfallUI.Instance.DefaultFont;

            // First pass encodes ASCII and calculates final dimensions
            int width = 0;
            asciiBytes = Encoding.ASCII.GetBytes(text);
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                // Invalid ASCII bytes are cast to a space character
                if (!font.HasGlyph(asciiBytes[i]))
                    asciiBytes[i] = PixelFont.SpaceASCII;

                // Calculate total width
                PixelFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);
                width += glyph.width + glyphSpacing;
            }

            // Create target label texture
            totalWidth = width;
            totalHeight = font.GlyphHeight;
            labelTexture = CreateLabelTexture(totalWidth, totalHeight);
            if (labelTexture == null)
                throw new Exception("TextLabel failed to create labelTexture.");

            // Second pass adds glyphs to label texture
            int xpos = 0;
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                PixelFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);
                labelTexture.SetPixels32(xpos, 0, glyph.width, totalHeight, glyph.colors);
                xpos += glyph.width + glyphSpacing;
            }
            labelTexture.Apply(false, true);
            labelTexture.filterMode = filterMode;
            this.Size = new Vector2(totalWidth, totalHeight);
        }

        Texture2D CreateLabelTexture(int width, int height)
        {
            // Create target texture and init to clear
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color32[] colors = texture.GetPixels32();
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.clear;
            }
            texture.SetPixels32(colors);
            texture.Apply(false);
            
            return texture;
        }

        #endregion
    }
}