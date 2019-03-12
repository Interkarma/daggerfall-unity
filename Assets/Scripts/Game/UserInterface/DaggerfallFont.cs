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
    public class DaggerfallFont
    {
        #region Fields

        public const int SpaceASCII = 32;
        const int defaultAsciiStart = 33;
        const int sdfGlyphsPerRow = 16;
        const int sdfGlyphCount = sdfGlyphsPerRow * sdfGlyphsPerRow;
        public const string invalidAsciiCode = "PixelFont does not contain glyph for ASCII code ";

        int glyphHeight;
        int glyphSpacing = 1;
        FilterMode filterMode = FilterMode.Point;
        Dictionary<int, GlyphInfo> glyphs = new Dictionary<int, GlyphInfo>();

        //string arena2Path;
        FontName font;
        FntFile fntFile = new FntFile();
        Color backgroundColor = Color.clear;
        Color textColor = Color.white;
        protected Texture2D atlasTexture;
        protected Rect[] atlasRects;

        protected Texture2D sdfAtlasTexture;
        protected Rect[] sdfAtlasRects;
        protected int sdfGlyphDimension = 0;
        protected int sdfHorzAdjust = 3;

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

        public struct GlyphInfo
        {
            public Color32[] colors;
            public int width;
        }

        #endregion

        #region Properties

        public int AsciiStart
        {
            get { return asciiStart; }
        }

        public int GlyphHeight
        {
            get { return glyphHeight; }
            set { glyphHeight = value; }
        }

        public int GlyphSpacing
        {
            get { return glyphSpacing; }
            set { glyphSpacing = value; }
        }

        public FilterMode FilterMode
        {
            get { return filterMode; }
            set { filterMode = value; }
        }

        public int GlyphCount
        {
            get { return glyphs.Count; }
        }

        public bool IsSDFCapable
        {
            get { return (DaggerfallUnity.Settings.SDFFontRendering && sdfAtlasTexture && sdfAtlasRects != null); }
        }

        public int SDFGlyphDimension
        {
            get { return sdfGlyphDimension; }
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

        #region Glyph Rendering

        void DrawClassicGlyph(byte rawAscii, Rect targetRect, Color color)
        {
            Rect atlasRect = atlasRects[rawAscii - asciiStart];
            Graphics.DrawTexture(targetRect, atlasTexture, atlasRect, 0, 0, 0, 0, color, DaggerfallUI.Instance.PixelFontMaterial);
        }

        void DrawClassicGlyphWithShadow(byte rawAscii, Rect targetRect, Color color, Vector2 shadowPosition, Color shadowColor)
        {
            if (shadowPosition != Vector2.zero && shadowColor != Color.clear)
            {
                Rect shadowRect = targetRect;
                shadowRect.x += shadowPosition.x;
                shadowRect.y += shadowPosition.y;
                DrawClassicGlyph(rawAscii, shadowRect, shadowColor);
            }

            DrawClassicGlyph(rawAscii, targetRect, color);
        }

        void DrawSDFGlyph(byte rawAscii, Rect targetRect, Color color)
        {
            Rect atlasRect = sdfAtlasRects[rawAscii - asciiStart];
            Graphics.DrawTexture(targetRect, sdfAtlasTexture, atlasRect, 0, 0, 0, 0, color, DaggerfallUI.Instance.SDFFontMaterial);
        }

        void DrawSDFGlyphWithShadow(byte rawAscii, Rect targetRect, Color color, Vector2 shadowPosition, Color shadowColor)
        {
            if (shadowPosition != Vector2.zero && shadowColor != Color.clear)
            {
                Rect shadowRect = targetRect;
                shadowRect.x += shadowPosition.x / 2;           // Shadow position also hacked by half as classic offset scale too much for smoother fonts
                shadowRect.y += shadowPosition.y / 2;
                DrawSDFGlyph(rawAscii, shadowRect, shadowColor);
            }

            DrawSDFGlyph(rawAscii, targetRect, color);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Draws a glyph.
        /// </summary>
        public void DrawGlyph(byte rawAscii, Rect targetRect, Color color)
        {
            if (rawAscii < asciiStart)
                return;

            if (IsSDFCapable)
                DrawSDFGlyph(rawAscii, targetRect, color);
            else
                DrawClassicGlyph(rawAscii, targetRect, color);
        }

        /// <summary>
        /// Draws a glyph with a drop-shadow.
        /// </summary>
        public void DrawGlyph(byte rawAscii, Rect targetRect, Color color, Vector2 shadowPosition, Color shadowColor)
        {
            if (rawAscii < asciiStart)
                return;

            if (IsSDFCapable)
                DrawSDFGlyphWithShadow(rawAscii, targetRect, color, shadowPosition, shadowColor);
            else
                DrawClassicGlyphWithShadow(rawAscii, targetRect, color, shadowPosition, shadowColor);
        }

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
                    asciiBytes[i] = SpaceASCII;

                GlyphInfo glyph = GetGlyph(asciiBytes[i]);

                if (asciiBytes[i] != SpaceASCII)
                {
                    if (IsSDFCapable)
                    {
                        // Draw using SDF shader
                        Rect rect = new Rect(x, y, glyph.width * scale.x + GlyphSpacing * scale.x, GlyphHeight * scale.y);
                        DrawSDFGlyph(asciiBytes[i], rect, color);
                        x += rect.width;
                    }
                    else
                    {
                        // Draw using pixel font shader
                        Rect rect = new Rect(x, y, glyph.width * scale.x, GlyphHeight * scale.y);
                        DrawClassicGlyph(asciiBytes[i], rect, color);
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
                    asciiBytes[i] = SpaceASCII;

                GlyphInfo glyph = GetGlyph(asciiBytes[i]);
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

        public void ClearGlyphs()
        {
            glyphs.Clear();
        }

        public bool HasGlyph(int ascii)
        {
            return glyphs.ContainsKey(ascii);
        }

        public void AddGlyph(int ascii, GlyphInfo info)
        {
            glyphs.Add(ascii, info);
        }

        public GlyphInfo GetGlyph(int ascii)
        {
            if (!glyphs.ContainsKey(ascii))
                throw new Exception(invalidAsciiCode + ascii);

            return glyphs[ascii];
        }

        public int GetGlyphWidth(int ascii)
        {
            if (!glyphs.ContainsKey(ascii))
                throw new Exception(invalidAsciiCode + ascii);

            return glyphs[ascii].width;
        }

        public int GetSDFGlyphWidth(int ascii)
        {
            if (!HasGlyph(ascii))
                throw new Exception(invalidAsciiCode + ascii);

            if (sdfAtlasRects == null || sdfAtlasRects.Length == 0)
                return 0;

            Rect atlasRect = sdfAtlasRects[ascii - asciiStart];

            return (int)(atlasRect.width * sdfGlyphDimension);
        }

        public void RemoveGlyph(int ascii)
        {
            if (!glyphs.ContainsKey(ascii))
                throw new Exception(invalidAsciiCode + ascii);

            glyphs.Remove(ascii);
        }

        public Material GetMaterial()
        {
            return (IsSDFCapable) ? DaggerfallUI.Instance.SDFFontMaterial : DaggerfallUI.Instance.PixelFontMaterial;
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

            // Load an SDF font variant if one is available
            TryLoadSDFFont();

            return true;
        }

        void TryLoadSDFFont()
        {
            // Attempt to load an SDF alternative font
            // Source SDF font atlas must be an equal POW2 width by height with glyphs arranged in 16x16 grid starting from ASCII 33
            // The image must be pre-processed into an alpha-only SDF format prior to import
            string sdfFontFilename = string.Format("{0}-SDF.png", font.ToString());
            string sdfFontPath = Path.Combine(DaggerfallUI.Instance.FontsFolder, sdfFontFilename);
            if (File.Exists(sdfFontPath))
            {
                // Load source image
                Texture2D texture = new Texture2D(4, 4, TextureFormat.ARGB32, false);
                if (!texture.LoadImage(File.ReadAllBytes(sdfFontPath), false))
                {
                    Debug.LogErrorFormat("DaggerfallFont: Found a possible SDF font variant but was unable to load from path {0}", sdfFontPath);
                    return;
                }

                // Source texture width must equal height
                if (texture.width != texture.height)
                {
                    Debug.LogErrorFormat("DaggerfallFont: SDF variant width does not equal height {0}", sdfFontPath);
                    return;
                }

                // Source texture must be POW2
                if (!PowerOfTwo.IsPowerOfTwo(texture.width))
                {
                    Debug.LogErrorFormat("DaggerfallFont: SDF variant is not POW2 {0}", sdfFontPath);
                    return;
                }

                // Discover rects
                int glyphDim = texture.width / sdfGlyphsPerRow;
                Color32[] colors = texture.GetPixels32();
                Rect[] rects = GenerateProportionalRects(ref colors, texture.width, texture.height, sdfGlyphsPerRow, sdfGlyphsPerRow, glyphDim, sdfGlyphCount);

                // Store settings
                sdfGlyphDimension = glyphDim;
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
            int yMin = 0;

            return new Rect(xMin, yMin, xMax + 1 - xMin, dimension - 1);
        }

        #endregion
    }
}