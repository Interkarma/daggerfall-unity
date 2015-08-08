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

namespace DaggerfallWorkshop.Demo.UserInterface
{
    /// <summary>
    /// A simple single-line text label using a pixel font.
    /// </summary>
    public class TextLabel : BaseScreenComponent
    {
        PixelFont font;
        int glyphSpacing = 1;
        string text = string.Empty;
        byte[] asciiBytes;
        int totalWidth;
        int totalHeight;
        Texture2D labelTexture;

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

        public Texture2D Texture
        {
            get { return labelTexture; }
        }

        public override void Draw()
        {
            base.Draw();
            GUI.DrawTexture(Rectangle, labelTexture, ScaleMode.StretchToFill);
        }

        #region Private Methods

        void SetText(string value)
        {
            this.text = value;
            CreateLabelTexture();
        }

        void CreateLabelTexture()
        {
            if (font == null)
                throw new Exception("TextLabel has no Font set.");

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