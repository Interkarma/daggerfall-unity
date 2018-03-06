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
    /// Daggerfall-specific implementation of a pixel font.
    /// </summary>
    public class DaggerfallFont : PixelFont
    {
        #region Fields

        const int defaultAsciiStart = 33;

        //string arena2Path;
        FontName font;
        FntFile fntFile = new FntFile();
        Color backgroundColor = Color.clear;
        Color textColor = Color.white;
        protected Texture2D atlasTexture;
        protected Rect[] atlasRects;

        protected int asciiStart = defaultAsciiStart;

        #endregion

        #region Structs & Enums

        public enum FontName
        {
            FONT0000,
            FONT0001,
            FONT0002,
            FONT0003,
            FONT0004,
        }

        #endregion

        #region Constructors

        public DaggerfallFont(FontName font = FontName.FONT0003)
        {
            //this.arena2Path = string.Empty;
            this.font = font;
            LoadFont();
        }

        public DaggerfallFont(string arena2Path, FontName font = FontName.FONT0003)
        {
            //this.arena2Path = arena2Path;
            this.font = font;
            LoadFont();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Draws string of individual text glyphs for small-scale font rendering.
        /// </summary>
        public void DrawText(
            string text,
            Vector2 position,
            Vector2 scale,
            Color color)
        {
            if (!fntFile.IsLoaded)
                throw new Exception("DaggerfallFont: DrawText() font not loaded.");

            atlasTexture.filterMode = FilterMode;

            byte[] asciiBytes = Encoding.ASCII.GetBytes(text);
            if (asciiBytes == null || asciiBytes.Length == 0)
                return;

            float x = position.x;
            float y = position.y;
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                // Invalid ASCII bytes are cast to a space character
                if (!HasGlyph(asciiBytes[i]))
                    asciiBytes[i] = PixelFont.SpaceASCII;

                PixelFont.GlyphInfo glyph = GetGlyph(asciiBytes[i]);

                Rect rect = new Rect(x, y, glyph.width * scale.x, GlyphHeight * scale.y);
                if (asciiBytes[i] != PixelFont.SpaceASCII)
                {
                    Color guiColor = GUI.color;

                    GUI.color = color;
                    GUI.DrawTextureWithTexCoords(rect, atlasTexture, atlasRects[asciiBytes[i] - asciiStart]);

                    GUI.color = guiColor;
                }

                x += rect.width + GlyphSpacing * scale.x;
            }
        }

        /// <summary>
        /// Draws string of individual text glyphs with a shadow.
        /// </summary>
        public void DrawText(
            string text,
            Vector2 position,
            Vector2 scale,
            Color color,
            Color shadowColor,
            Vector2 shadowPos)
        {
            DrawText(text, position + shadowPos, scale, shadowColor);
            DrawText(text, position, scale, color);
        }

        /// <summary>
        /// Calculates glyph width up to character length of string (-1 for all characters)
        /// </summary>
        public float GetCharacterWidth(string text, int length = -1, float scale = 1)
        {
            byte[] asciiBytes = Encoding.ASCII.GetBytes(text);
            if (asciiBytes == null || asciiBytes.Length == 0)
                return 0;

            if (length < 0)
                length = asciiBytes.Length;

            float width = 0;
            for (int i = 0; i < length; i++)
            {
                // Invalid ASCII bytes are cast to a space character
                if (!HasGlyph(asciiBytes[i]))
                    asciiBytes[i] = PixelFont.SpaceASCII;

                PixelFont.GlyphInfo glyph = GetGlyph(asciiBytes[i]);
                width += (glyph.width + GlyphSpacing) * scale;
            }

            return width;
        }

        /// <summary>
        /// Reloads font glyphs with a different base colour (default is Color.white for normal UI tinting).
        /// This is an expensive operation, only use this at font create time.
        /// </summary>
        /// <param name="color">New colour of glyphs.</param>
        /// <returns>True if successful.</returns>
        public bool ReloadFont(Color color)
        {
            textColor = color;
            return LoadFont();
        }

        #endregion

        #region Private Methods

        bool LoadFont()
        {
            // Load font
            string filename = font.ToString() + ".FNT";
            if (!fntFile.Load(Path.Combine(DaggerfallUI.Instance.FontsFolder, filename), FileUsage.UseMemory, true))
                throw new Exception("DaggerfallFont failed to load font " + filename);

            // Start new glyph dictionary
            // Daggerfall fonts start at ASCII 33 '!' so we must create our own space glyph for ASCII 32
            ClearGlyphs();
            AddGlyph(SpaceASCII, CreateSpaceGlyph());

            // Add remaining glyphs
            int ascii = asciiStart;
            for (int i = 0; i < FntFile.MaxGlyphCount; i++)
            {
                AddGlyph(ascii++, CreateGlyph(i));
            }

            GlyphHeight = fntFile.FixedHeight;

            // Create font atlas
            ImageProcessing.CreateFontAtlas(fntFile, Color.clear, Color.white, out atlasTexture, out atlasRects);
            atlasTexture.filterMode = FilterMode;
            
            return true;
        }

        GlyphInfo CreateSpaceGlyph()
        {
            int width = fntFile.FixedWidth - 1;
            int height = fntFile.FixedHeight;
            Color32[] colors = new Color32[width * height];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = backgroundColor;
            }

            GlyphInfo glyph = new GlyphInfo();
            glyph.colors = colors;
            glyph.width = width;

            return glyph;
        }

        GlyphInfo CreateGlyph(int index)
        {
            GlyphInfo glyph = new GlyphInfo();
            glyph.colors = ImageProcessing.GetProportionalGlyphColors(fntFile, index, backgroundColor, textColor, true);
            glyph.width = fntFile.GetGlyphWidth(index);

            return glyph;
        }

        #endregion
    }
}