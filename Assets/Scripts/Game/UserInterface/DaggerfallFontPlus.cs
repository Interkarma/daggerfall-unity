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
    /// Extends DaggerfallFont to support generic pixel fonts.
    /// Currently in experimental stages only.
    /// </summary>
    public class DaggerfallFontPlus : DaggerfallFont
    {
        #region Fields
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor to import font.
        /// </summary>
        public DaggerfallFontPlus(Texture2D texture, int glyphsWide, int glyphsHigh, int dimension, bool proportional = true)
        {
            ImportBitmapFont(texture, glyphsWide, glyphsHigh, dimension, proportional);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Imports a generic pixel font from texture.
        /// Source atlas texture:
        ///  * Must be readable
        ///  * Must start from ASCII 0 character
        ///  * Must not have more than 256 characters total
        ///  * Must increment characters left to right, top to bottom (with 0 at top-left)
        /// </summary>
        /// <param name="texture">Source texture atlas.</param>
        /// <param name="glyphsWide">Number of glyphs packed horizontally.</param>
        /// <param name="glyphsHigh">Number of glyphs packed vertically.</param>
        /// <param name="dimension">Dimension of each glyph.</param>
        /// <param name="proportional">Discover proportional width of glyphs.</param>
        public void ImportBitmapFont(Texture2D texture, int glyphsWide, int glyphsHigh, int dimension, bool proportional = true)
        {
            // Validate count
            int count = glyphsWide * glyphsHigh;
            if (count > 256)
            {
                throw new Exception("ImportBitmapFont: Glyph count > 256.");
            }

            // Validate dimensions
            if (glyphsWide * dimension > texture.width ||
                glyphsHigh * dimension > texture.height)
            {
                throw new Exception("ImportBitmapFont: Invalid texture dimensions.");
            }

            // Get texture as Color32 array
            Color32[] colors = texture.GetPixels32();

            // Start new glyph dictionary
            ClearGlyphs();

            // Store settings
            atlasTexture = texture;
            GlyphHeight = dimension;
            asciiStart = 0;

            // Generate rects
            if (!proportional)
                atlasRects = GenerateNonProportionalRects(texture.width, texture.height, glyphsWide, glyphsHigh, dimension, count);
            else
                atlasRects = GenerateProportionalRects(ref colors, texture.width, texture.height, glyphsWide, glyphsHigh, dimension, count);

            // Add glyphs
            //int xpos = 0;
            //int ypos = 0;
            //DFSize srcSize = new DFSize(texture.width, texture.height);
            //DFSize dstSize = new DFSize(dimension, dimension);
            for (int i = 0; i < count; i++)
            {
                //Color32[] dst = new Color32[dimension * dimension];
                //ImageProcessing.CopyColors(ref colors, ref dst, srcSize, dstSize, new DFPosition(xpos, ypos), new DFPosition(0, 0), dstSize);

                GlyphInfo glyph = new GlyphInfo();
                //glyph.colors = dst;
                glyph.width = (int)(atlasRects[i].width * texture.width);
                AddGlyph(i, glyph);

                //xpos += dimension;
                //if (xpos > texture.width)
                //{
                //    xpos = 0;
                //    ypos += dimension;
                //}
            }
        }

        #endregion

        #region Private Methods

        Rect[] GenerateNonProportionalRects(int atlasWidth, int atlasHeight, int glyphsWide, int glyphsHigh, int dimension, int count)
        {
            Rect[] rects = new Rect[count];

            int xpos = 0;
            int ypos = 0;
            for (int i = 0; i < count; i++)
            {
                int x = dimension * xpos;
                int y = dimension * (glyphsHigh - 1 - ypos);

                rects[i] = new Rect(
                    (float)x / (float)atlasWidth,
                    (float)y / (float)atlasHeight,
                    (float)dimension / (float)atlasWidth,
                    (float)dimension / (float)atlasHeight);

                xpos++;
                if (xpos >= glyphsWide)
                {
                    xpos = 0;
                    ypos++;
                }
            }

            return rects;
        }

        Rect[] GenerateProportionalRects(ref Color32[] colors, int atlasWidth, int atlasHeight, int glyphsWide, int glyphsHigh, int dimension, int count)
        {
            Rect[] rects = new Rect[count];

            int xpos = 0;
            int ypos = 0;
            for (int i = 0; i < count; i++)
            {
                int x = dimension * xpos;
                int y = dimension * (glyphsHigh - 1 - ypos);

                Rect innerRect = FindSubRect(ref colors, atlasWidth, x, y, dimension);

                rects[i] = new Rect(
                    (float)(x + innerRect.xMin) / (float)atlasWidth,
                    (float)y / (float)atlasHeight,
                    (float)(innerRect.width) / (float)atlasWidth,
                    (float)dimension / (float)atlasHeight);

                xpos++;
                if (xpos >= glyphsWide)
                {
                    xpos = 0;
                    ypos++;
                }
            }

            return rects;
        }

        Rect FindSubRect(ref Color32[] colors, int atlasWidth, int xstart, int ystart, int dimension)
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

            return new Rect(xMin, 0, xMax + 1 - xMin, dimension - 1);
        }

        #endregion
    }
}
