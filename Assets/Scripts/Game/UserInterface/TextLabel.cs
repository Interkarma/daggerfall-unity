// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
        const int minTextureDim = 8;

        int maxCharacters = -1;
        int startCharacterIndex = 0;
        PixelFont font;
        string text = string.Empty;
        byte[] asciiBytes;
        int totalWidth;
        int totalHeight;
        int textureWidth;
        int textureHeight;
        Texture2D labelTexture;
        Vector2 shadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
        Color textColor = DaggerfallUI.DaggerfallDefaultTextColor;
        Color shadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;

        int maxWidth = -1;
        bool wrapText = false;

        /// <summary>
        /// Maximum length of label string.
        /// Setting to -1 allows for any length.
        /// </summary>
        public int MaxCharacters
        {
            get { return maxCharacters; }
            set { maxCharacters = value; }
        }

        public int StartCharacterIndex
        {
            get { return startCharacterIndex; }
            set { startCharacterIndex = Math.Max(0, value); }
        }    

        public PixelFont Font
        {
            get { return font; }
            set { font = value; CreateLabelTexture(); }
        }

        public string Text
        {
            get { return text; }
            set { SetText(value); }
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

        public int TextWidth
        {
            get { return totalWidth; }
        }

        public int TextHeight
        {
            get { return totalHeight; }
        }

        public int MaxWidth
        {
            get { return maxWidth; }
            set { maxWidth = value; }
        }

        public bool WrapText
        {
            get { return wrapText; }
            set { wrapText = value; }
        }

        public TextLabel(DaggerfallFont font = null)
        {
            if (font != null)
                Font = font;
        }

        public override void Draw()
        {
            base.Draw();

            if (string.IsNullOrEmpty(text) || labelTexture == null)
                return;

            // Store starting colour
            Color guiColor = GUI.color;

            // Draw shadow
            Rect totalRect = Rectangle;
            Rect innerRect = new Rect(0, 0, (float)totalWidth / (float)textureWidth, (float)totalHeight / (float)textureHeight);
            if (shadowPosition != Vector2.zero)
            {
                Rect shadowRect = totalRect;
                shadowRect.x += shadowPosition.x * LocalScale.x;
                shadowRect.y += shadowPosition.y * LocalScale.y;
                GUI.color = shadowColor;
                GUI.DrawTextureWithTexCoords(shadowRect, labelTexture, innerRect);
            }

            // Draw text
            GUI.color = textColor;
            GUI.DrawTextureWithTexCoords(totalRect, labelTexture, innerRect);

            // Restore starting colour
            GUI.color = guiColor;
        }

        public virtual void UpdateLabelTexture()
        {
            CreateLabelTexture();
        }

        #region Protected Methods

        protected virtual void SetText(string value)
        {
            // Truncate string to max characters
            if (maxCharacters != -1)
                value = value.Substring(0, Math.Min(value.Length, maxCharacters));

            this.text = value;
            CreateLabelTexture();
        }

        protected virtual void CreateLabelTexture()
        {
            if (!wrapText)
                CreateLabelTextureSingleLine();
            else
                CreateLabelTextureWrapped();
        }

        void CreateLabelTextureSingleLine()
        {
            if (font == null)
                font = DaggerfallUI.DefaultFont;

            // First pass encodes ASCII and calculates final dimensions
            int width = 0;
            asciiBytes = Encoding.ASCII.GetBytes(text);
            for (int i = startCharacterIndex; i < asciiBytes.Length; i++)
            {
                // Invalid ASCII bytes are cast to a space character
                if (!font.HasGlyph(asciiBytes[i]))
                    asciiBytes[i] = PixelFont.SpaceASCII;

                // Calculate total width
                PixelFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);
                width += glyph.width + font.GlyphSpacing;
            }

            if (maxWidth > 0 && width > maxWidth)
                width = maxWidth;

            // Create target label texture
            totalWidth = width;
            totalHeight = font.GlyphHeight;
            labelTexture = CreateLabelTexture(totalWidth, totalHeight);
            if (labelTexture == null)
                throw new Exception("TextLabel failed to create labelTexture.");

            // Second pass adds glyphs to label texture
            int xpos = 0;
            for (int i = startCharacterIndex; i < asciiBytes.Length; i++)
            {
                PixelFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);
                if (xpos + glyph.width >= totalWidth)
                    break;

                labelTexture.SetPixels32(xpos, 0, glyph.width, totalHeight, glyph.colors);
                xpos += glyph.width + font.GlyphSpacing;
            }
            labelTexture.Apply(false, true);
            labelTexture.filterMode = font.FilterMode;
            this.Size = new Vector2(totalWidth, totalHeight);
        }

        void CreateLabelTextureWrapped()
        {
            if (font == null)
                font = DaggerfallUI.DefaultFont;

            // First pass encodes ASCII and calculates final dimensions
            int width = 0;
            int greatestWidthFound = 0;
            int lastEndOfRowByte = 0;
            asciiBytes = Encoding.ASCII.GetBytes(text);
            List<byte[]> rows = new List<byte[]>();

            for (int i = 0; i < asciiBytes.Length; i++)
            {
                // Invalid ASCII bytes are cast to a space character
                if (!font.HasGlyph(asciiBytes[i]))
                    asciiBytes[i] = PixelFont.SpaceASCII;

                // Calculate total width
                PixelFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);

                // If maxWidth is set, don't allow the label texture to exceed it
                if ((maxWidth <= 0) || (width + glyph.width + font.GlyphSpacing) <= maxWidth)
                    width += glyph.width + font.GlyphSpacing;
                else
                {
                    // The row of glyphs exceeded maxWidth. Add it to the list of rows and start
                    // counting width again with the remainder of the ASCII bytes.
                    byte[] trimmed = new List<byte>(asciiBytes).GetRange(lastEndOfRowByte, i - lastEndOfRowByte).ToArray();
                    rows.Add(trimmed);
                    if (greatestWidthFound < width)
                        greatestWidthFound = width;
                    width = 0;
                    lastEndOfRowByte = i;

                    // Resume interation over remainder of ASCII bytes
                    //asciiBytes = new List<byte>(asciiBytes).GetRange(i, asciiBytes.Length - i).ToArray();
                    //i = 0;
                }
            }

            if (lastEndOfRowByte > 0)
                asciiBytes = new List<byte>(asciiBytes).GetRange(lastEndOfRowByte, asciiBytes.Length - lastEndOfRowByte).ToArray();

            rows.Add(asciiBytes);

            if (width < greatestWidthFound)
                width = greatestWidthFound;

            // Create target label texture
            totalWidth = width;
            totalHeight = rows.Count * font.GlyphHeight;

            labelTexture = CreateLabelTexture(totalWidth, totalHeight);
            if (labelTexture == null)
                throw new Exception("TextLabel failed to create labelTexture.");

            // Second pass adds glyphs to label texture
            int xpos = 0;
            int ypos = totalHeight - font.GlyphHeight;

            foreach (byte[] row in rows)
            {
                xpos = 0;
                for (int i = 0; i < row.Length; i++)
                {
                    PixelFont.GlyphInfo glyph = font.GetGlyph(row[i]);
                    if (xpos + glyph.width >= totalWidth)
                        break;

                    labelTexture.SetPixels32(xpos, ypos, glyph.width, font.GlyphHeight, glyph.colors);
                    xpos += glyph.width + font.GlyphSpacing;
                }
                ypos -= font.GlyphHeight;
            }

            labelTexture.Apply(false, true);
            labelTexture.filterMode = font.FilterMode;
            this.Size = new Vector2(totalWidth, totalHeight);
        }

        protected virtual Texture2D CreateLabelTexture(int width, int height)
        {
            // Enforce minimum texture dimensions of 8x8 to avoid "degenerate image" error in Unity 5.2
            textureWidth = (width < minTextureDim) ? minTextureDim : width;
            textureHeight = (height < minTextureDim) ? minTextureDim : height;

            // Create target texture and init to clear
            Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
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