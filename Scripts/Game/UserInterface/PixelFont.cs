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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A pixel font consisting of bitmap glyph data keyed by ASCII code.
    /// </summary>
    public class PixelFont
    {
        public const int SpaceASCII = 32;
        const string invalidAsciiCode = "PixelFont does not contain glyph for ASCII code ";

        int glyphHeight;
        Dictionary<int, GlyphInfo> glyphs = new Dictionary<int, GlyphInfo>();

        public int GlyphHeight
        {
            get { return glyphHeight; }
            set { glyphHeight = value; }
        }

        public int GlyphCount
        {
            get { return glyphs.Count; }
        }

        public struct GlyphInfo
        {
            public Color32[] colors;
            public int width;
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

        public void RemoveGlyph(int ascii)
        {
            if (!glyphs.ContainsKey(ascii))
                throw new Exception(invalidAsciiCode + ascii);

            glyphs.Remove(ascii);
        }
    }
}