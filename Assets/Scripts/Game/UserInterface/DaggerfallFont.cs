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

        const int sdfGlyphsWide = 16;
        const int sdfGlyphsHigh = 16;
        const int sdfGlyphDimension = 64;
        const int sdfGlyphCount = sdfGlyphsWide * sdfGlyphsHigh;

        //string arena2Path;
        FontName font;
        FntFile fntFile = new FntFile();
        Color backgroundColor = Color.clear;
        Color textColor = Color.white;
        protected Texture2D atlasTexture;
        protected Rect[] atlasRects;

        protected Texture2D sdfAtlasTexture;
        protected Rect[] sdfAtlasRects;
        protected int sdfHorzAdjust = 3;
        protected int sdfVertAdjust = 8;

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

                if (asciiBytes[i] != PixelFont.SpaceASCII)
                {
                    if (DaggerfallUnity.Settings.SDFFontRendering && sdfAtlasTexture && sdfAtlasRects != null)
                    {
                        Rect atlasRect = sdfAtlasRects[asciiBytes[i] - asciiStart];

                        // Draw using SDF shader
                        //Rect rect = new Rect(x, y, atlasRect.width * (sdfGlyphDimension * sdfGlyphsWide), sdfGlyphDimension);
                        Rect rect = new Rect(x, y, glyph.width * scale.x, GlyphHeight * scale.y);
                        Graphics.DrawTexture(rect, sdfAtlasTexture, atlasRect, 0, 0, 0, 0, color, DaggerfallUI.Instance.SDFFontMaterial);
                        x += rect.width + GlyphSpacing * scale.x;
                    }
                    else
                    {
                        // Fallback to normal rendering
                        Rect rect = new Rect(x, y, glyph.width * scale.x, GlyphHeight * scale.y);

                        Color guiColor = GUI.color;

                        GUI.color = color;
                        GUI.DrawTextureWithTexCoords(rect, atlasTexture, atlasRects[asciiBytes[i] - asciiStart]);

                        GUI.color = guiColor;

                        x += rect.width + GlyphSpacing * scale.x;
                    }
                }
                else
                {
                    // TODO: Just add space character
                    Rect rect = new Rect(x, y, glyph.width * scale.x, GlyphHeight * scale.y);
                    x += rect.width;
                }
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

            // Try to load an SDF font variant
            if (DaggerfallUnity.Settings.SDFFontRendering)
                TryLoadSDFFont();
            
            return true;
        }

        void TryLoadSDFFont()
        {
            // Attempt to load an SDF alternative font
            // Source SDF font atlas must be 512x512 pixels with 32x32 pixel glyphs arranged in 16x16 grid starting from ASCII 33
            // The image must be pre-processed into an alpha-only SDF format prior to import
            string sdfFontFilename = string.Format("{0}-SDF.png", font.ToString());
            string sdfFontPath = Path.Combine(DaggerfallUI.Instance.FontsFolder, sdfFontFilename);
            if (File.Exists(sdfFontPath))
            {
                // Load source image
                Texture2D texture = new Texture2D(4, 4, TextureFormat.ARGB32, false);
                if (!texture.LoadImage(File.ReadAllBytes(sdfFontPath), false))
                {
                    Debug.LogErrorFormat("Found a possible SDF font variant but was unable to load from path {0}", sdfFontPath);
                    return;
                }

                // Discover rects
                Color32[] colors = texture.GetPixels32();
                Rect[] rects = GenerateProportionalRects(ref colors, texture.width, texture.height, sdfGlyphsWide, sdfGlyphsHigh, sdfGlyphDimension, sdfGlyphCount);

                // Store settings
                sdfAtlasTexture = texture;
                sdfAtlasRects = rects;
            }
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

        #region Utility

        Rect[] GenerateProportionalRects(ref Color32[] colors, int atlasWidth, int atlasHeight, int glyphsWide, int glyphsHigh, int glyphDimension, int count)
        {
            Rect[] rects = new Rect[count];

            int xpos = 0;
            int ypos = 0;
            for (int i = 0; i < count; i++)
            {
                int x = glyphDimension * xpos;
                int y = glyphDimension * (glyphsHigh - 1 - ypos);

                Rect innerRect = FindInnerRect(ref colors, atlasWidth, x, y, glyphDimension);

                rects[i] = new Rect(
                    (float)(x + innerRect.xMin) / (float)atlasWidth,
                    (float)(y + innerRect.yMin) / (float)atlasHeight,
                    (float)(innerRect.width) / (float)atlasWidth,
                    (float)(innerRect.height) / (float)atlasHeight);

                xpos++;
                if (xpos >= glyphsWide)
                {
                    xpos = 0;
                    ypos++;
                }
            }

            return rects;
        }

        Rect FindInnerRect(ref Color32[] colors, int atlasWidth, int xstart, int ystart, int dimension)
        {
            int xMin = dimension;
            int xMax = 0;

            for (int y = 0; y < dimension; y++)
            {
                int ypos = ystart + y;
                for (int x = 0; x < dimension; x++)
                {
                    int xpos = xstart + x;
                    int offset = ypos * atlasWidth + xpos;
                    if (colors[offset].a != 0)
                    {
                        if (x < xMin)
                            xMin = x;
                        if (x > xMax)
                            xMax = x;
                    }
                }
            }

            // Handle blank glpyhs
            if (xMin == dimension && xMax == 0)
            {
                xMin = 0;
                xMax = dimension / 2;
            }

            // Apply kerning
            xMin += sdfHorzAdjust;
            xMax -= sdfHorzAdjust;
            float yMin = -sdfVertAdjust;

            return new Rect(xMin, yMin, xMax + 1 - xMin, dimension - 1 - sdfVertAdjust);
        }

        #endregion
    }
}