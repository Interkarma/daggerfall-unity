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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Demo.UserInterface
{
    /// <summary>
    /// Daggerfall-specific implementation of a pixel font.
    /// </summary>
    public class DaggerfallFont : PixelFont
    {
        #region Fields

        const int asciiStart = 33;

        string arena2Path;
        FontName font;
        FntFile fntFile = new FntFile();
        Color backgroundColor = Color.clear;
        Color textColor = Color.white;

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

        public DaggerfallFont(string arena2Path, FontName font = FontName.FONT0003)
        {
            this.arena2Path = arena2Path;
            this.font = font;
            LoadFont();
        }

        #endregion

        #region Public Methods

        public bool ChangeFont(FontName font)
        {
            this.font = font;
            return LoadFont();
        }

        #endregion

        #region Private Methods

        bool LoadFont()
        {
            // Load font
            string filename = font.ToString() + ".FNT";
            if (!fntFile.Load(Path.Combine(arena2Path, filename), FileUsage.UseMemory, true))
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