// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Michael Rauter (Nystul)
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
        #region Fields

        public const int limitMinTextureDim = 8; // the smallest possible value for minTextureDim (used to enforce minimum texture dimensions of 8x8 to avoid "degenerate image" error in Unity 5.2)

        int minTextureDim = limitMinTextureDim; // set this with property MinTextureDim to higher values if you experience scaling issues with small texts (e.g. inventory infopanel)
        int maxCharacters = -1;
        int startCharacterIndex = 0; // can be used to offset start character of textlabel's text (used by listbox's entry-wise horizontal scroll mode)
        DaggerfallFont font;
        string text = string.Empty;
        byte[] asciiBytes;
        int totalWidth;
        int totalHeight;
        int textureWidth;
        int textureHeight;
        int numTextLines = 1; // 1 in unwrapped, n in wrapped
        Texture2D singleLineLabelTexture;
        Vector2 shadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
        Color textColor = DaggerfallUI.DaggerfallDefaultTextColor;
        Color shadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
        HorizontalTextAlignmentSetting horizontalTextAlignment;
        int maxWidth = -1;
        bool wrapText = false; // wrap text - but will tear words that are reaching
        bool wrapWords = false; // wrap words - no word tearing
        float textScale = 1.0f; // scale text
        LabelLayoutData labelLayout = new LabelLayoutData();

        public enum RestrictedRenderArea_CoordinateType
        {
            ScreenCoordinates = 0,
            DaggerfallNativeCoordinates = 1
        };
        protected RestrictedRenderArea_CoordinateType restrictedRenderAreaCoordinateType = RestrictedRenderArea_CoordinateType.ScreenCoordinates;

        #endregion

        #region Structs & Enums

        public enum HorizontalTextAlignmentSetting
        {
            None,
            Left,
            Center,
            Right,
            Justify
        }

        struct GlyphLayoutData
        {
            public int x;                                       // X position of classic glyph in virtual layout area
            public int y;                                       // Y position of classic glyph in virtual layout area
            public byte glyphRawAscii;                          // Raw ASCII value of classic glyph
            public int glyphWidth;                              // Width of classic glyph
        }

        struct LabelLayoutData
        {
            public int width;                                   // Width of virtual layout area
            public int height;                                  // Height of virtual layout area
            public GlyphLayoutData[] glyphLayout;               // Positions of classic glyphs inside virtual layout area
        }

        #endregion

        #region Properties

        /// <summary>
        /// used to set min texture dims of textlabel to higher values if there would be aspect or scaling issues with small texts otherwise (e.g. some single-lined textlabels in inventory infopanel)
        /// </summary>
        public int MinTextureDim
        {
            get { return minTextureDim; }
            set { minTextureDim = Math.Max(limitMinTextureDim, value); RefreshLayout(); }
        }

        /// <summary>
        /// Maximum length of label string.
        /// Setting to -1 allows for any length.
        /// </summary>
        public int MaxCharacters
        {
            get { return maxCharacters; }
            set { maxCharacters = value; }
        }

        /// <summary>
        /// specify character index inside textlabel's text as new starting index
        /// </summary>
        public int StartCharacterIndex
        {
            get { return startCharacterIndex; }
            set { startCharacterIndex = Math.Max(0, value); }
        }    

        public DaggerfallFont Font
        {
            get { return font; }
            set { font = value; RefreshLayout(); }
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
        
        public HorizontalTextAlignmentSetting HorizontalTextAlignment
        {
            get { return horizontalTextAlignment; }
            set { horizontalTextAlignment = value; RefreshLayout(); }
        }

        ///// <summary>
        ///// Maintaining this only for backwards compatibility with exterior automatap.
        ///// Deprecated and will be removed once exterior automap supports SDF fonts.
        ///// </summary>
        //public Texture2D Texture
        //{
        //    get { return GetSingleLineLabelTexture(); }
        //}

        public int TextWidth
        {
            get { return (int)(totalWidth * textScale); }
        }

        public int TextHeight
        {
            get { return (int)(totalHeight * textScale); }
        }

        public int NumTextLines
        {
            get { return numTextLines; }
        }

        /// <summary>
        /// Set the maximum allowed width of the textlabel in pixels
        /// </summary>
        public int MaxWidth
        {
            get { return maxWidth; }
            set { maxWidth = value; RefreshLayout(); }
        }

        /// <summary>
        /// Enable text wrapping - use MaxWidth property to set maximum allowed width of the textlabel
        /// </summary>
        public bool WrapText
        {
            get { return wrapText; }
            set { wrapText = value; RefreshLayout(); }
        }

        /// <summary>
        /// Enable word wrapping - words will no longer get tear apart if possible - make sure to enable property WrapText to make this property to have an effect
        /// </summary>
        public bool WrapWords
        {
            get { return wrapWords; }
            set { wrapWords = value; RefreshLayout(); }
        }

        // Small hack to help horizontal scrolling work with SDF font rendering and pixel-wise list box
        // Don't want to change how this fundamentally works right now so as not to break classic font layouts
        public float HorzPixelScrollOffset { get; set; }

        /// <summary>
        /// Set a restricted render area for the textlabel - the textlabel's content will only be rendered inside the specified Rect's bounds
        /// </summary>
        new public Rect RectRestrictedRenderArea
        {
            get { return rectRestrictedRenderArea; }
            set
            {
                ((BaseScreenComponent)this).RectRestrictedRenderArea = value;
            }
        }

        /// <summary>
        /// Set text scale factor - 1.0f is default value, 0.5f is half sized text, 2.0f double sized text and so on
        /// </summary>
        public float TextScale
        {
            get { return textScale; }
            set
            {
                textScale = Math.Max(0.1f, value); // Math.Min(2.0f, value));
                RefreshLayout();
            }
        }

        public TextLabel(DaggerfallFont font = null)
        {
            if (font != null)
                Font = font;
        }

        /// <summary>
        /// get/set the type of coordinate specification (e.g. absolute) of the restricted render area
        /// </summary>
        public RestrictedRenderArea_CoordinateType RestrictedRenderAreaCoordinateType
        {
            get { return restrictedRenderAreaCoordinateType; }
            set
            {
                restrictedRenderAreaCoordinateType = value;
            }
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();

            if (font == null || string.IsNullOrEmpty(text))
                return;

            DrawLabel();
        }

        void DrawLabel()
        {
            // Exit if label layout not defined
            if (labelLayout.glyphLayout == null || labelLayout.glyphLayout.Length == 0)
                return;

            // Set render area
            Material material = font.GetMaterial();
            Vector4 scissorRect = (useRestrictedRenderArea) ? GetRestrictedRenderScissorRect() : new Vector4(0, 1, 0, 1);
            material.SetVector("_ScissorRect", scissorRect);

            // Layout glyphs
            Rect totalRect = Rectangle;
            for (int i = 0; i < labelLayout.glyphLayout.Length; i++)
            {
                GlyphLayoutData glyphLayout = labelLayout.glyphLayout[i];

                Rect targetRect = new Rect(
                    totalRect.x + glyphLayout.x * LocalScale.x * textScale + HorzPixelScrollOffset * LocalScale.x * textScale,
                    totalRect.y + glyphLayout.y * LocalScale.y * textScale,
                    glyphLayout.glyphWidth * LocalScale.x * textScale,
                    font.GlyphHeight * LocalScale.y * textScale);

                // Allow SDF glyph to draw into the "single pixel" empty space normally reserved for classic letter spacing
                // As SDF fonts are so much more detailed, this single pixel space ends up looking very large and unnecessary
                // This has the effect of keeping glyphs a bit closer together while using the exact screen rect allowed
                if (font.IsSDFCapable)
                    targetRect.width += font.GlyphSpacing * LocalScale.x * textScale;

                font.DrawGlyph(glyphLayout.glyphRawAscii, targetRect, textColor, shadowPosition * LocalScale, shadowColor);
            }
        }

        #endregion

        #region Public Methods

        public virtual void RefreshLayout()
        {
            if (!wrapText)
                CreateNewLabelLayoutSingleLine();
            else
                CreateNewLabelLayoutWrapped();
        }

        #endregion

        #region Protected Methods

        protected virtual void SetText(string value)
        {
            // Truncate string to max characters
            if (maxCharacters != -1)
                value = value.Substring(0, Math.Min(value.Length, maxCharacters));

            this.text = value;
            RefreshLayout();
        }

        protected Vector4 GetRestrictedRenderScissorRect()
        {
            Rect myRect = this.Rectangle;
            Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
            float xMinScreen = 0.0f, xMaxScreen = 0.0f, yMinScreen = 0.0f, yMaxScreen = 0.0f;
            if (restrictedRenderAreaCoordinateType == RestrictedRenderArea_CoordinateType.ScreenCoordinates)
            {
                xMinScreen = rectRestrictedRenderArea.xMin;
                xMaxScreen = rectRestrictedRenderArea.xMax;
                yMinScreen = rectRestrictedRenderArea.yMin;
                yMaxScreen = rectRestrictedRenderArea.yMax;
            }
            else if (restrictedRenderAreaCoordinateType == RestrictedRenderArea_CoordinateType.DaggerfallNativeCoordinates)
            {
                Rect rectLabel = new Rect(this.Parent.Position + this.Position, this.Size);
                float leftCut = Mathf.Round(Math.Max(0, rectRestrictedRenderArea.xMin - rectLabel.xMin));
                float rightCut = Mathf.Round(Math.Max(0, rectLabel.xMax - rectRestrictedRenderArea.xMax));
                float topCut = Mathf.Round(Math.Max(0, rectRestrictedRenderArea.yMin - rectLabel.yMin));
                float bottomCut = Mathf.Round(Math.Max(0, rectLabel.yMax - rectRestrictedRenderArea.yMax));

                xMinScreen = myRect.xMin + (this.Position.x + leftCut) * this.LocalScale.x;
                xMaxScreen = myRect.xMax + (this.Position.x - rightCut) * this.LocalScale.x;
                yMinScreen = myRect.yMin + (topCut) * this.LocalScale.y;
                yMaxScreen = myRect.yMax - (bottomCut) * this.LocalScale.y;
            } 

            Vector4 scissorRect;
            scissorRect.x = xMinScreen / screenRect.width;
            scissorRect.y = xMaxScreen / screenRect.width;
            scissorRect.z = 1.0f - yMaxScreen / screenRect.height;
            scissorRect.w = 1.0f - yMinScreen / screenRect.height;

            return scissorRect;
        }

        #endregion

        #region Label Layout Methods

        void CreateNewLabelLayoutSingleLine()
        {
            //
            // Stage 1 - Encode glyphs and calculate final dimensions
            //

            // Use default UI font if none set
            if (font == null)
                font = DaggerfallUI.DefaultFont;

            // Start a new layout
            labelLayout = new LabelLayoutData();
            List<GlyphLayoutData> glyphLayout = new List<GlyphLayoutData>();

            // Set a local maxWidth that compensates for textScale
            int maxWidth = (int)(this.maxWidth / textScale);

            // First pass encodes ASCII and calculates final dimensions
            int width = 0;
            asciiBytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("ISO-8859-1"), Encoding.Default.GetBytes(text));
            for (int i = startCharacterIndex; i < asciiBytes.Length; i++)
            {
                // Invalid ASCII bytes are cast to a space character
                if (!font.HasGlyph(asciiBytes[i]))
                    asciiBytes[i] = DaggerfallFont.SpaceASCII;

                // Calculate total width
                DaggerfallFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);
                width += glyph.width + font.GlyphSpacing;               
            }

            // Trim width
            if (maxWidth > 0 && width > maxWidth)
                width = maxWidth;

            // Create virtual layout area
            totalWidth = width;
            totalHeight = (int)(font.GlyphHeight);
            numTextLines = 1;
            labelLayout.width = totalWidth;
            labelLayout.height = totalHeight;

            //
            // Stage 2 - Add glyph to layout
            //

            // Determine horizontal alignment offset
            float alignmentOffset;
            switch (horizontalTextAlignment)
            {
                default:
                case HorizontalTextAlignmentSetting.None:
                case HorizontalTextAlignmentSetting.Left:
                case HorizontalTextAlignmentSetting.Justify:
                    alignmentOffset = 0.0f;
                    break;
                case HorizontalTextAlignmentSetting.Center:
                    alignmentOffset = (totalWidth - width) * 0.5f;
                    break;
                case HorizontalTextAlignmentSetting.Right:
                    alignmentOffset = totalWidth - width;
                    break;
            }

            // Second pass adds glyphs to layout
            int xpos = (int)alignmentOffset;
            for (int i = startCharacterIndex; i < asciiBytes.Length; i++)
            {
                DaggerfallFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);
                if (xpos + glyph.width >= totalWidth)
                    break;

                GlyphLayoutData glyphPos = new GlyphLayoutData()
                {
                    x = xpos,
                    y = 0,
                    glyphRawAscii = asciiBytes[i],
                    glyphWidth = glyph.width,
                };

                glyphLayout.Add(glyphPos);
                xpos += glyph.width + font.GlyphSpacing;
            }

            labelLayout.glyphLayout = glyphLayout.ToArray();
            this.Size = new Vector2(totalWidth * textScale, totalHeight * textScale);
        }

        void CreateNewLabelLayoutWrapped()
        {
            // Use default UI font if none set
            if (font == null)
                font = DaggerfallUI.DefaultFont;

            //
            // Stage 1 - Encode glyphs and calculate final dimensions
            //

            // Start a new layout
            labelLayout = new LabelLayoutData();
            List<GlyphLayoutData> glyphLayout = new List<GlyphLayoutData>();

            // Set a local maxWidth that compensates for textScale
            int maxWidth = (int)(this.maxWidth / textScale);

            // First pass encodes ASCII and calculates final dimensions
            int width = 0;
            int greatestWidthFound = 0;
            int lastEndOfRowByte = 0;
            asciiBytes = Encoding.ASCII.GetBytes(text);
            List<byte[]> rows = new List<byte[]>();
            List<int> rowWidth = new List<int>();

            for (int i = 0; i < asciiBytes.Length; i++)
            {
                // Invalid ASCII bytes are cast to a space character
                if (!font.HasGlyph(asciiBytes[i]))
                    asciiBytes[i] = DaggerfallFont.SpaceASCII;

                // Calculate total width
                DaggerfallFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);

                // If maxWidth is set, don't allow the label texture to exceed it
                if ((maxWidth <= 0) || ((width + glyph.width + font.GlyphSpacing) <= maxWidth))
                {
                    width += glyph.width + font.GlyphSpacing;
                }
                else
                {
                    int rowLength;
                    if (wrapWords)
                    {
                        int j;
                        for (j = i; j >= lastEndOfRowByte; j--)
                        {
                            glyph = font.GetGlyph(asciiBytes[j]);
                            if (j < i) // glyph i has not been added to width
                            {
                                width -= glyph.width + font.GlyphSpacing;
                                if (width <= maxWidth && asciiBytes[j] == DaggerfallFont.SpaceASCII)
                                {
                                    break;
                                }
                            }
                        }

                        if (j > lastEndOfRowByte) // space found in row at position that is not exceeding maxWidth for all summed glyph's widths before this position
                        {
                            i = j; // set new processing position (j is the space position, i will be increased on next loop iteration to point to next chat after the space char)
                            rowLength = j - lastEndOfRowByte; // set length from row start to position before space
                        }
                        else
                        {
                            rowLength = i - lastEndOfRowByte;
                        }

                        // compute width of text-wrapped line
                        width = 0;
                        for (int k = lastEndOfRowByte; k < j; k++)
                        {
                            if (k < j - 1 || (k == j - 1 && asciiBytes[k] != DaggerfallFont.SpaceASCII)) // all expect last character if it is a space
                            {
                                glyph = font.GetGlyph(asciiBytes[k]);
                                width += glyph.width + font.GlyphSpacing;
                            }
                        }
                    }
                    else
                    {
                        rowLength = i - lastEndOfRowByte;
                    }
                    // The row of glyphs exceeded maxWidth. Add it to the list of rows and start
                    // counting width again with the remainder of the ASCII bytes.
                    List<byte> content = new List<byte>(asciiBytes).GetRange(lastEndOfRowByte, rowLength);
                    if (content[content.Count - 1] == DaggerfallFont.SpaceASCII)
                        content.RemoveAt(content.Count - 1);
                    byte[] trimmed = content.ToArray();

                    rows.Add(trimmed);
                    rowWidth.Add(width);

                    // update greatest width found so far
                    if (greatestWidthFound < width)
                        greatestWidthFound = width;

                    // reset width for next line
                    width = 0;

                    lastEndOfRowByte = i + 1; // position after space for next line, note: i will be increased on next loop iteration anyway - so no need to increase i
                }
            }

            if (lastEndOfRowByte > 0)
                asciiBytes = new List<byte>(asciiBytes).GetRange(lastEndOfRowByte, asciiBytes.Length - lastEndOfRowByte).ToArray();

            // also get width of last line
            width = 0;
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                DaggerfallFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);
                width += glyph.width + font.GlyphSpacing;
            }

            // update greatest width found so far
            if (width <= maxWidth && greatestWidthFound < width) // width should always be <= maxWidth here
                greatestWidthFound = width;

            rows.Add(asciiBytes);
            rowWidth.Add(width);

            // Create virtual layout area
            totalWidth = maxWidth;
            totalHeight = (int)(rows.Count * font.GlyphHeight);
            numTextLines = rows.Count;

            //
            // Stage 2 - Add glyph to layout
            //

            // Second pass adds glyphs to label texture
            int xpos = 0;
            int ypos = totalHeight - font.GlyphHeight;

            //foreach (byte[] row in rows)
            for (int r = 0; r < rows.Count; r++)
            {
                byte[] row = rows[r];
                float alignmentOffset;
                switch (horizontalTextAlignment)
                {
                    default:
                    case HorizontalTextAlignmentSetting.None:
                    case HorizontalTextAlignmentSetting.Left:
                    case HorizontalTextAlignmentSetting.Justify:
                        alignmentOffset = 0.0f;
                        break;
                    case HorizontalTextAlignmentSetting.Center:
                        alignmentOffset = (totalWidth - rowWidth[r]) * 0.5f;
                        break;
                    case HorizontalTextAlignmentSetting.Right:
                        alignmentOffset = totalWidth - rowWidth[r];
                        break;
                }

                int numSpaces = 0; // needed to compute extra offset between words for HorizontalTextAlignmentSetting.Justify
                int extraSpaceToDistribute = 0;  // needed to compute extra offset between words for HorizontalTextAlignmentSetting.Justify
                if (horizontalTextAlignment == HorizontalTextAlignmentSetting.Justify)
                {
                    for (int i = 0; i < row.Length; i++)
                    {
                        if (row[i] == DaggerfallFont.SpaceASCII)
                            numSpaces++;
                    }

                    extraSpaceToDistribute = maxWidth - rowWidth[r];
                }

                xpos = (int)alignmentOffset;
                for (int i = 0; i < row.Length; i++)
                {
                    DaggerfallFont.GlyphInfo glyph = font.GetGlyph(row[i]);
                    if (xpos + glyph.width > totalWidth)
                        break;

                    if (row[i] == DaggerfallFont.SpaceASCII)
                    {
                        if (numSpaces > 1)
                        {
                            int currentPortionExtraSpaceToDistribute = (int)Mathf.Round((float)extraSpaceToDistribute / (float)numSpaces);
                            xpos += currentPortionExtraSpaceToDistribute;
                            extraSpaceToDistribute -= currentPortionExtraSpaceToDistribute;
                            numSpaces--;
                        }
                        else if (numSpaces == 1)
                        {
                            xpos += extraSpaceToDistribute;
                        }
                    }

                    GlyphLayoutData glyphPos = new GlyphLayoutData()
                    {
                        x = xpos,
                        y = totalHeight - font.GlyphHeight - ypos,
                        glyphRawAscii = row[i],
                        glyphWidth = glyph.width,
                    };

                    glyphLayout.Add(glyphPos);
                    xpos += glyph.width + font.GlyphSpacing;
                }
                ypos -= font.GlyphHeight;
            }

            labelLayout.glyphLayout = glyphLayout.ToArray();
            this.Size = new Vector2(totalWidth * textScale, totalHeight * textScale);
        }

        #endregion
    }
}