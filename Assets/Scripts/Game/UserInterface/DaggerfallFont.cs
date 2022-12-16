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
using System.IO;
using System.Collections.Generic;
using System.Text;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using TMPro;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Daggerfall-specific implementation of a pixel font.
    /// Supports classic FONT0000-0004 with an SDF variant.
    /// Classic font has the same limitations of 256 characters starting from ASCII 33.
    /// SDF font uses a keyed dictionary so can support any number of glyph codes.
    /// Current implementation will load a TextMeshPro 2.0.x font asset directly for SDF variant.
    /// </summary>
    public class DaggerfallFont
    {
        #region Fields

        public const int SpaceCode = 32;
        public const int ErrorCode = 63;
        const int defaultAsciiStart = 33;
        public const string invalidCode = "Font does not contain glyph for code: ";
        float classicGlyphSpacing = 1;
        float sdfGlyphSpacing = 0.2f;
        float sdfShadowPositionScale = 0.4f;

        int glyphHeight;
        FilterMode filterMode = FilterMode.Point;
        Dictionary<int, GlyphInfo> glyphs = new Dictionary<int, GlyphInfo>();

        FontName font;
        FntFile fntFile = new FntFile();
        Color backgroundColor = Color.clear;
        Color textColor = Color.white;
        protected Texture2D atlasTexture;
        protected Rect[] atlasRects;
        protected int asciiStart = defaultAsciiStart;

        protected SDFFontInfo? sdfFontInfo;

        #endregion

        #region Structs & Enums

        public enum FontName
        {
            FONT0000, // Large Font
            FONT0001, // Title Font
            FONT0002, // Small Font
            FONT0003, // Default Font
            FONT0004, // Unused
        }

        public struct GlyphInfo
        {
            public Color32[] colors;
            public int width;
        }

        public struct SDFFontInfo
        {
            public float pointSize;
            public float baseline;
            public Texture2D atlasTexture;
            public Dictionary<int, SDFGlyphInfo> glyphs;
        }

        public struct SDFGlyphInfo
        {
            public int code;
            public Rect rect;
            public Vector2 offset;
            public Vector2 size;
            public float advance;
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

        public float GlyphSpacing
        {
            get { return GetGlyphSpacing(); }
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
            get { return (DaggerfallUnity.Settings.SDFFontRendering && sdfFontInfo != null); }
        }

        public SDFFontInfo SDFInfo
        {
            get { return sdfFontInfo.Value; }
        }

        #endregion

        #region Constructors

        public DaggerfallFont(FontName font = FontName.FONT0003)
        {
            this.font = font;
            LoadFont();
        }

        public DaggerfallFont(string arena2Path, FontName font = FontName.FONT0003)
        {
            this.font = font;
            LoadFont();
        }

        #endregion

        #region Glyph Rendering

        void DrawClassicGlyph(byte rawAscii, Rect targetRect, Color color)
        {
            Rect atlasRect = atlasRects[rawAscii - asciiStart];
            if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
            {
                DaggerfallUI.Instance.PixelFontMaterial.SetColor(UIShaderParam._Color, color);
                Graphics.DrawTexture(targetRect, atlasTexture, atlasRect, 0, 0, 0, 0, DaggerfallUI.Instance.PixelFontMaterial);
            }
            else
            {
                Graphics.DrawTexture(targetRect, atlasTexture, atlasRect, 0, 0, 0, 0, color, DaggerfallUI.Instance.PixelFontMaterial);
            }
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

        void DrawSDFText(
            string text,
            Vector2 position,
            Vector2 scale,
            Color color)
        {
            float glyphSpacing = GlyphSpacing;
            byte[] utf32Bytes = Encoding.UTF32.GetBytes(text);
            for (int i = 0; i < utf32Bytes.Length; i += sizeof(int))
            {
                // Get code and use ? for any character code not in dictionary
                int code = BitConverter.ToInt32(utf32Bytes, i);
                if (!sdfFontInfo.Value.glyphs.ContainsKey(code))
                    code = ErrorCode;

                // Draw glyph and advance position
                SDFGlyphInfo glyph = sdfFontInfo.Value.glyphs[code];
                DrawSDFGlyph(glyph, position, scale, color);
                position.x += GetGlyphWidth(glyph, scale, glyphSpacing) * scale.x;
            }
        }

        float DrawSDFGlyphWithShadow(int code, Vector2 position, Vector2 scale, Color color, Vector2 shadowPosition, Color shadowColor)
        {
            SDFGlyphInfo glyph = sdfFontInfo.Value.glyphs[code];
            if (shadowPosition != Vector2.zero && shadowColor != Color.clear)
                DrawSDFGlyph(glyph, position + shadowPosition * sdfShadowPositionScale, scale, shadowColor);

            return DrawSDFGlyph(glyph, position, scale, color);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Draws a classic glyph with a drop-shadow.
        /// </summary>
        public void DrawClassicGlyph(byte rawAscii, Rect targetRect, Color color, Vector2 shadowPosition, Color shadowColor)
        {
            if (rawAscii < asciiStart)
                return;

            DrawClassicGlyphWithShadow(rawAscii, targetRect, color, shadowPosition, shadowColor);
        }

        /// <summary>
        /// Draws an SDF glyph with a drop-shadow.
        /// </summary>
        public float DrawSDFGlyph(int code, Vector2 position, Vector2 scale, Color color, Vector2 shadowPosition, Color shadowColor)
        {
            return DrawSDFGlyphWithShadow(code, position, scale, color, shadowPosition, shadowColor);
        }

        public float DrawSDFGlyph(int code, Vector2 position, Vector2 scale, Color color)
        {
            return DrawSDFGlyph(sdfFontInfo.Value.glyphs[code], position, scale, color);
        }

        public float DrawSDFGlyph(SDFGlyphInfo glyph, Vector2 position, Vector2 scale, Color color)
        {
            float scalingRatio = GetSDFGlyphScalingRatio(scale.y);

            // Handle space
            if (glyph.code == SpaceCode)
                return glyph.advance * scalingRatio;

            // Compose target rect - this will change based on current display scale
            // Can use classic glyph height to approximate baseline vertical position
            float baseline = position.y - 2 * scale.y + GlyphHeight * scale.y + sdfFontInfo.Value.baseline;
            float xpos = position.x + glyph.offset.x * scalingRatio;
            float ypos = baseline - glyph.offset.y * scalingRatio;
            Rect targetRect = new Rect(xpos, ypos, glyph.size.x * scalingRatio, glyph.size.y * scalingRatio);

            // Draw glyph
            if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
            {
                DaggerfallUI.Instance.SDFFontMaterial.SetColor(UIShaderParam._Color, color);
                Graphics.DrawTexture(targetRect, sdfFontInfo.Value.atlasTexture, glyph.rect, 0, 0, 0, 0, DaggerfallUI.Instance.SDFFontMaterial);
            }
            else
            {
                Graphics.DrawTexture(targetRect, sdfFontInfo.Value.atlasTexture, glyph.rect, 0, 0, 0, 0, color, DaggerfallUI.Instance.SDFFontMaterial);
            }
            return GetGlyphWidth(glyph, scale, GlyphSpacing);
        }

        /// <summary>
        /// Draws string of classic or SDF glyphs.
        /// </summary>
        public void DrawText(
            string text,
            Vector2 position,
            Vector2 scale,
            Color color)
        {
            if (!fntFile.IsLoaded)
                throw new Exception("DaggerfallFont: DrawText() font not loaded.");

            // Redirect SDF rendering when enabled
            if (IsSDFCapable)
            {
                DrawSDFText(text, position, scale, color);
                return;
            }

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
                    asciiBytes[i] = SpaceCode;

                GlyphInfo glyph = GetGlyph(asciiBytes[i]);

                if (asciiBytes[i] != SpaceCode)
                {
                    Rect rect = new Rect(x, y, glyph.width * scale.x, GlyphHeight * scale.y);
                    DrawClassicGlyph(asciiBytes[i], rect, color);
                    x += rect.width + GlyphSpacing * scale.x;
                }
                else
                {
                    // Just add space character
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
        /// Calculate width of text using whichever font path is active (classic or SDF).
        /// </summary>
        /// <param name="text">Text to calculate width of.</param>
        /// <param name="scale">Scale to use when calculating width.</param>
        /// <returns>Width of string in scaled pixels.</returns>
        public float CalculateTextWidth(string text, Vector2 scale, int start = 0, int length = -1)
        {
            // Must have a string
            if (string.IsNullOrEmpty(text))
                return 0;

            // Get automatic length from start position to end of text
            if (length < 0)
                length = text.Length - start;

            // Get substring if required
            if (start > 0 || length != text.Length)
                text = text.Substring(start, length);

            // Calculate width based on active font path
            float width = 0;
            if (!IsSDFCapable)
            {
                // Classic glyphs
                byte[] asciiBytes = Encoding.ASCII.GetBytes(text);
                for (int i = 0; i < asciiBytes.Length; i++)
                {
                    // Get code and use ? for any character code not in dictionary
                    int code = asciiBytes[i];
                    if (!HasGlyph(code))
                        code = ErrorCode;

                    width += GetGlyphWidth(code, scale, GlyphSpacing);
                }
            }
            else
            {
                // SDF glyphs
                byte[] utf32Bytes = Encoding.UTF32.GetBytes(text);
                for (int i = 0; i < utf32Bytes.Length; i += sizeof(int))
                {
                    // Get code and use ? for any character code not in dictionary
                    int code = BitConverter.ToInt32(utf32Bytes, i);
                    if (!sdfFontInfo.Value.glyphs.ContainsKey(code))
                        code = ErrorCode;

                    width += GetGlyphWidth(code, scale, GlyphSpacing);
                }
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

        public bool HasSDFGlyph(int code)
        {
            if (!IsSDFCapable || sdfFontInfo == null)
                return false;

            return sdfFontInfo.Value.glyphs.ContainsKey(code);
        }

        public void AddGlyph(int ascii, GlyphInfo info)
        {
            glyphs.Add(ascii, info);
        }

        public GlyphInfo GetGlyph(int ascii)
        {
            if (!glyphs.ContainsKey(ascii))
                throw new Exception(invalidCode + ascii);

            return glyphs[ascii];
        }

        public SDFGlyphInfo GetSDFGlyph(int code)
        {
            if (!IsSDFCapable || !sdfFontInfo.Value.glyphs.ContainsKey(code))
                throw new Exception(invalidCode + code);

            return sdfFontInfo.Value.glyphs[code];
        }

        public float GetGlyphWidth(int code, Vector2 scale, float spacing = 0)
        {
            if (!IsSDFCapable)
            {
                GlyphInfo glyph = GetGlyph(code);
                return glyph.width + spacing;
            }
            else
            {
                SDFGlyphInfo glyph = SDFInfo.glyphs[code];
                float scalingRatio = GetSDFGlyphScalingRatio(scale.y);
                return glyph.advance * scalingRatio / scale.x + spacing;
            }
        }

        public float GetGlyphWidth(SDFGlyphInfo glyph, Vector2 scale, float spacing = 0)
        {
            float scalingRatio = GetSDFGlyphScalingRatio(scale.y);
            return glyph.advance * scalingRatio / scale.x + spacing;
        }

        public void RemoveGlyph(int ascii)
        {
            if (!glyphs.ContainsKey(ascii))
                throw new Exception(invalidCode + ascii);

            glyphs.Remove(ascii);
        }

        public Material GetMaterial()
        {
            return (IsSDFCapable) ? DaggerfallUI.Instance.SDFFontMaterial : DaggerfallUI.Instance.PixelFontMaterial;
        }

        /// <summary>
        /// Load a TextMeshPro 2.0.x font asset to use for SDF rendering.
        /// Mods can use DaggerfallUI.Instance.Font0 through Font4 to access the instances used by other game windows.
        /// Custom font should be set only during startup.
        /// Note: Asset must be in a Resources folder.
        /// </summary>
        /// <param name="path">Path to a TextMeshPro 2.0.x font asset.</param>
        /// <returns>True if successful.</returns>
        public bool LoadSDFFontAsset(string path)
        {
            // Attempt to load a TextMeshPro font asset
            TMP_FontAsset tmpFont = Resources.Load<TMP_FontAsset>(path);
            if (!tmpFont)
                return false;

            // Attempt to ingest a StreamingAssets/Fonts .ttf override file
            TMP_FontAsset replacement;
            if (ReplaceTMPFontFromFile(Path.GetFileNameWithoutExtension(path), tmpFont, out replacement))
            {
                tmpFont = replacement;
                // TODO: Output debug text that font was replaced
            }

            UseSDFFontAsset(tmpFont);

            return true;
        }

        /// <summary>
        /// Use provided TextMeshPro 2.0.x font asset for SDF rendering.
        /// </summary>
        /// <param name="tmpFont">TMP font asset to use for this font.</param>
        public void UseSDFFontAsset(TMP_FontAsset tmpFont)
        {
            // Create font info
            SDFFontInfo fi = new SDFFontInfo();
            fi.pointSize = tmpFont.faceInfo.pointSize;
            fi.atlasTexture = tmpFont.atlasTexture;
            fi.baseline = tmpFont.faceInfo.baseline;
            fi.glyphs = new Dictionary<int, SDFGlyphInfo>();

            // Cache glyph info
            float atlasWidth = tmpFont.atlasTexture.width;
            float atlasHeight = tmpFont.atlasTexture.height;
            foreach (var kvp in tmpFont.characterLookupTable)
            {
                // Compose glyph rect inside of atlas
                TMP_Character character = kvp.Value;
                float atlasGlyphX = character.glyph.glyphRect.x / atlasWidth;
                float atlasGlyphY = character.glyph.glyphRect.y / atlasHeight;
                float atlasGlyphWidth = character.glyph.glyphRect.width / atlasWidth;
                float atlasGlyphHeight = character.glyph.glyphRect.height / atlasHeight;
                Rect atlasGlyphRect = new Rect(atlasGlyphX, atlasGlyphY, atlasGlyphWidth, atlasGlyphHeight);

                // Store information about this glyph
                SDFGlyphInfo glyphInfo = new SDFGlyphInfo()
                {
                    code = (int)kvp.Key,
                    rect = atlasGlyphRect,
                    offset = new Vector2(character.glyph.metrics.horizontalBearingX, character.glyph.metrics.horizontalBearingY),
                    size = new Vector2(character.glyph.metrics.width, character.glyph.metrics.height),
                    advance = character.glyph.metrics.horizontalAdvance,
                };
                fi.glyphs.Add((int)kvp.Key, glyphInfo);
            }

            // Set live font info
            sdfFontInfo = fi;
        }

        /// <summary>
        /// Gets array of unicode values from currently loaded SDF font.
        /// </summary>
        /// <returns>Array of unicode characters. Can be null or empty.</returns>
        public uint[] GetUnicodes()
        {
            // Must have a previously loaded SDF font
            if (sdfFontInfo == null)
                return null;

            // Get unicode values loaded for this font
            List<uint> unicodes = new List<uint>();
            foreach (var key in sdfFontInfo.Value.glyphs.Keys)
            {
                unicodes.Add((uint)key);
            }

            return unicodes.ToArray();
        }

        #endregion

        #region Private Methods

        float GetSDFGlyphScalingRatio(float localYScale)
        {
            return GlyphHeight / SDFInfo.pointSize * localYScale;
        }

        bool LoadFont()
        {
            // Load font
            string filename = font.ToString() + ".FNT";
            if (!fntFile.Load(Path.Combine(DaggerfallUI.Instance.FontsFolder, filename), FileUsage.UseMemory, true))
                throw new Exception("DaggerfallFont failed to load font " + filename);

            // Start new glyph dictionary
            // Daggerfall fonts start at ASCII 33 '!' so we must create our own space glyph for ASCII 32
            ClearGlyphs();
            AddGlyph(SpaceCode, CreateSpaceGlyph());

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
            LoadSDFFontAsset(string.Format("Fonts/{0}-SDF", font.ToString()));

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

        float GetGlyphSpacing()
        {
            return (!IsSDFCapable) ? classicGlyphSpacing : sdfGlyphSpacing;
        }

        /// <summary>
        /// Replace TMP font asset using a .ttf or .otf font in StreamingAssets/Fonts.
        /// TODO: Support loading font file from .dfmod.
        /// </summary>
        /// <param name="filename">Filename of replacement font including .ttf extension. Font file must be present in StreamingAssets/Fonts to load.</param>
        /// <param name="source">Source TMP font for initial character table population.</param>
        /// <param name="replacement">Replacement TMP font output.</param>
        /// <returns>True is successfully created replacement TMP font.</returns>
        bool ReplaceTMPFontFromFile(string filename, TMP_FontAsset source, out TMP_FontAsset replacement)
        {
            const string ttfExt = ".ttf";
            const string otfExt = ".otf";

            // Compose path to font file
            string path = Path.Combine(Application.streamingAssetsPath, "Fonts", filename);

            // Check file exists
            replacement = null;
            if (File.Exists(path + ttfExt))
                path += ttfExt;
            else if (File.Exists(path + otfExt))
                path += otfExt;
            else
                return false;

            // Create replacement TMP font asset from path
            Font font = new Font(path);
            replacement = TMP_FontAsset.CreateFontAsset(font, 90, 9, UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, 2048, 2048, AtlasPopulationMode.Dynamic);
            if (replacement == null)
                return false;

            // Get characters of source font
            List<uint> sourceUnicodes = new List<uint>();
            foreach(TMP_Character c in source.characterTable)
            {
                sourceUnicodes.Add(c.unicode);
            }

            // Attempt to add unicode characters from source
            uint[] missingUnicodesSource = null;
            if (!replacement.TryAddCharacters(sourceUnicodes.ToArray(), out missingUnicodesSource))
            {
                // Format list of missing default characters in replacement font
                // This isn't always a problem, and missing characters aren't always important to game
                // Just log message to help with troubleshooting if needed
                string missingCharsString = string.Empty;
                for (int c = 0; c < missingUnicodesSource.Length; c++)
                {
                    missingCharsString += string.Format("{0}[{1}] ", Convert.ToChar(missingUnicodesSource[c]), missingUnicodesSource[c]);
                }
                Debug.LogFormat("Some default characters could not be found in font {0}: '{1}': ", filename, missingCharsString);
            }

            // Attempt to add user-specified unicode characters from a source file
            LoadCustomFontChars(Path.GetFileNameWithoutExtension(path) + ".txt", replacement);

            return true;
        }

        /// <summary>
        /// Attempt to add specific font characters to SDF font loaded from a .txt file with same name as font.
        /// For example, "FONT0003-SDF.ttf" might be a Cyrillic font and "FONT0003-SDF.txt" the Cyrillic alphabet.
        /// Some fonts (e.g. Noto Sans JP) may contain many thousands of character codes for different languages, far too many to add all to SDF atlas.
        /// The .txt file tells DFU which character codes are actually required in SDF atlas by translator matching their alphabet and language.
        /// All Latin character codes are added by default if present in font.
        /// It is recommended custom fonts always have Latin characters for any parts of game not translated or supported yet.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="replacement"></param>
        void LoadCustomFontChars(string filename, TMP_FontAsset replacement)
        {
            // Compose path to character file
            string path = Path.Combine(Application.streamingAssetsPath, "Fonts", filename);

            // Check file exists
            if (!File.Exists(path))
                return;

            // Attempt to add unicode characters from source file
            string text = File.ReadAllText(path);
            if (!string.IsNullOrEmpty(text))
            {
                // Read unicodes from source file
                List<uint> sourceUnicodes = new List<uint>(text.Length);
                for (int c = 0; c < text.Length; c++)
                {
                    if (text[c] >= 0x09 && text[c] <= 0x0d || sourceUnicodes.Contains(text[c]))
                        continue; // Filter out codes 09-0d (tab through carriage return) so input txt can format a bit
                    else
                        sourceUnicodes.Add(text[c]);
                }

                // Attempt to add unicode characters from source file
                uint[] missingUnicodesSource = null;
                if (!replacement.TryAddCharacters(sourceUnicodes.ToArray(), out missingUnicodesSource))
                {
                    // Format list of missing default characters in replacement font
                    // This is considered a warning at least as modder specifically requested these characters from font
                    string missingCharsString = string.Empty;
                    for (int c = 0; c < missingUnicodesSource.Length; c++)
                    {
                        missingCharsString += string.Format("{0}[{1}] ", Convert.ToChar(missingUnicodesSource[c]), missingUnicodesSource[c]);
                    }
                    Debug.LogWarningFormat("Some requested characters could not be found in font {0}: '{1}': ", filename, missingCharsString);
                }
            }
        }

        #endregion
    }
}