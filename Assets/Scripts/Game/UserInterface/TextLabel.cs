// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
using TMPro;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A simple single-line text label using a pixel font.
    /// </summary>
    public class TextLabel : BaseScreenComponent
    {
        #region Fields

        const float totalWidthPadding = 1f;

        public const int limitMinTextureDim = 8; // the smallest possible value for minTextureDim (used to enforce minimum texture dimensions of 8x8 to avoid "degenerate image" error in Unity 5.2)

        int minTextureDim = limitMinTextureDim; // set this with property MinTextureDim to higher values if you experience scaling issues with small texts (e.g. inventory infopanel)
        int maxCharacters = -1;
        int startCharacterIndex = 0; // can be used to offset start character of textlabel's text (used by listbox's entry-wise horizontal scroll mode)
        DaggerfallFont font;
        string text = string.Empty;
        float totalWidth;
        float totalHeight;
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
        List<GlyphLayoutData> glyphLayout = new List<GlyphLayoutData>();
        bool previousSDFState;
        Vector2 previousLocalScale;

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
            public float x;                                     // X position of glyph in label layout area
            public float y;                                     // Y position of glyph in label layout area
            public int code;                                    // Glyph code for dictionary lookup
            public float width;                                 // Width of glyph character
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

            // Need to recalculate layout if SDF state or scale changes (e.g. window resized)
            // Do this before drawing so that even unparented label get a chance to recalibrate if needed
            if (font.IsSDFCapable != previousSDFState || previousLocalScale != LocalScale)
            {
                RefreshLayout();
                previousSDFState = font.IsSDFCapable;
                previousLocalScale = LocalScale;
            }

            DrawLabel();
        }

        void DrawLabel()
        {
            // Exit if no layout
            if (glyphLayout.Count == 0)
                return;

            // Set render area
            Material material = font.GetMaterial();
            Vector4 scissorRect = (UseRestrictedRenderArea) ? GetRestrictedRenderScissorRect() : new Vector4(0, 1, 0, 1);
            material.SetVector("_ScissorRect", scissorRect);

            // Draw glyphs with classic layout
            if (!font.IsSDFCapable)
            {
                Rect totalRect = Rectangle;
                for (int i = 0; i < glyphLayout.Count; i++)
                {
                    GlyphLayoutData glyph = glyphLayout[i];

                    Rect targetRect = new Rect(
                        (int)(totalRect.x + glyph.x * LocalScale.x * textScale + HorzPixelScrollOffset * LocalScale.x * textScale),
                        (int)(totalRect.y + glyph.y * LocalScale.y * textScale),
                        (int)(glyph.width * LocalScale.x * textScale),
                        (int)(font.GlyphHeight * LocalScale.y * textScale));

                    font.DrawClassicGlyph((byte)glyph.code, targetRect, textColor, shadowPosition * LocalScale, shadowColor);
                }
            }
            else
            {
                Rect totalRect = Rectangle;
                for (int i = 0; i < glyphLayout.Count; i++)
                {
                    GlyphLayoutData glyph = glyphLayout[i];

                    Vector2 position = new Vector2(
                        (int)(totalRect.x + glyph.x * LocalScale.x * textScale + HorzPixelScrollOffset * LocalScale.x * textScale),
                        (int)(totalRect.y + glyph.y * LocalScale.y * textScale));

                    font.DrawSDFGlyph(glyph.code, position, LocalScale * textScale, textColor, shadowPosition * LocalScale, shadowColor);
                }
            }
        }

        #endregion

        #region Public Methods

        public virtual void RefreshLayout()
        {
            // Use default UI font if none set
            if (font == null)
                font = DaggerfallUI.DefaultFont;

            if (!wrapText)
                CreateNewLabelLayoutSingleLine();
            else
                CreateNewLabelLayoutWrapped();
        }

        #endregion

        #region Protected Methods

        protected virtual void SetText(string value)
        {
            if (value == null)
                value = string.Empty;

            // Truncate string to max characters
            if (maxCharacters != -1)
                value = value.Substring(0, Math.Min(value.Length, maxCharacters));

            this.text = value;
            RefreshLayout();
        }

        protected Vector4 GetRestrictedRenderScissorRect()
        {
            float xMinScreen = 0.0f, xMaxScreen = 0.0f, yMinScreen = 0.0f, yMaxScreen = 0.0f;
            if (restrictedRenderAreaCoordinateType == RestrictedRenderArea_CoordinateType.ScreenCoordinates)
            {
                xMinScreen = rectRestrictedRenderArea.xMin;
                xMaxScreen = rectRestrictedRenderArea.xMax;
                yMinScreen = rectRestrictedRenderArea.yMin;
                yMaxScreen = rectRestrictedRenderArea.yMax;
            }
            else if (restrictedRenderAreaCoordinateType == RestrictedRenderArea_CoordinateType.ParentCoordinates && Parent != null)
            {
                Rect parentRect = Parent.Rectangle;
                xMinScreen = parentRect.xMin;
                xMaxScreen = parentRect.xMax;
                yMinScreen = parentRect.yMin;
                yMaxScreen = parentRect.yMax;
            }
            else if (restrictedRenderAreaCoordinateType == RestrictedRenderArea_CoordinateType.CustomParent && restrictedRenderAreaCustomParent != null)
            {
                Rect customParentRect = restrictedRenderAreaCustomParent.Rectangle;
                xMinScreen = customParentRect.xMin;
                xMaxScreen = customParentRect.xMax;
                yMinScreen = customParentRect.yMin;
                yMaxScreen = customParentRect.yMax;
            }

            Vector4 scissorRect;
            Rect myRect = this.Rectangle;
            Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
            scissorRect.x = xMinScreen / screenRect.width;
            scissorRect.y = xMaxScreen / screenRect.width;
            scissorRect.z = 1.0f - yMaxScreen / screenRect.height;
            scissorRect.w = 1.0f - yMinScreen / screenRect.height;

            return scissorRect;
        }

        #endregion

        #region Label Layout Methods

        int[] GetCodes(int startCharacterIndex)
        {
            int[] codes = new int[text.Length];

            if (!font.IsSDFCapable)
            {
                byte[] asciiBytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("ISO-8859-1"), Encoding.Default.GetBytes(text));
                for (int i = startCharacterIndex; i < asciiBytes.Length; i++)
                {
                    int code = asciiBytes[i];
                    codes[i] = font.HasGlyph(code) ? code : DaggerfallFont.ErrorCode;
                }
            }
            else
            {
                byte[] utf32Bytes = Encoding.UTF32.GetBytes(text);
                for (int i = startCharacterIndex * sizeof(int); i < utf32Bytes.Length; i += sizeof(int))
                {
                    int code = BitConverter.ToInt32(utf32Bytes, i);
                    codes[i / sizeof(int)] = font.HasSDFGlyph(code) ? code : DaggerfallFont.ErrorCode;
                }
            }

            return codes;
        }

        void CreateNewLabelLayoutSingleLine()
        {
            //
            // Stage 1 - Encode glyphs and calculate final dimensions
            //

            // Start a new layout
            glyphLayout.Clear();

            // Set a local maxWidth that compensates for textScale
            int maxWidth = (int)(this.maxWidth / textScale);

            // First pass calculates final dimensions
            float width = 0;
            float spacing = font.GlyphSpacing;
            int[] codes = GetCodes(startCharacterIndex);
            for (int i = 0; i < codes.Length; i++)
            {
                width += font.GetGlyphWidth(codes[i], LocalScale, spacing);
            }

            // Trim width
            if (maxWidth > 0 && width > maxWidth)
                width = maxWidth;

            // Create virtual layout area
            totalWidth = width;
            totalHeight = font.GlyphHeight;
            numTextLines = 1;

            //
            // Stage 2 - Add glyphs to layout
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
            float xpos = (int)alignmentOffset;
            for (int i = 0; i < codes.Length; i++)
            {
                float glyphWidth = font.GetGlyphWidth(codes[i], LocalScale);
                if (xpos + glyphWidth > totalWidth + totalWidthPadding)
                    break;

                GlyphLayoutData glyphPos = new GlyphLayoutData()
                {
                    x = xpos,
                    y = 0,
                    code = codes[i],
                    width = glyphWidth,
                };

                glyphLayout.Add(glyphPos);
                xpos += glyphWidth + spacing;
            }

            Size = new Vector2(totalWidth * textScale, totalHeight * textScale);
        }

        void CreateNewLabelLayoutWrapped()
        {
            //
            // Stage 1 - Encode glyphs and calculate final dimensions
            //

            // Start a new layout
            glyphLayout.Clear();

            if (Parent != null)
                _ = Rectangle; //Calling GetRectangle() to establish LocalScale; discarding return value

            // Set a local maxWidth that compensates for textScale
            int maxWidth = (int)(this.maxWidth / textScale);

            // First pass encodes ASCII and calculates final dimensions
            float width = 0;
            float greatestWidthFound = 0;
            int lastEndOfRowByte = 0;
            float spacing = font.GlyphSpacing;
            int[] codes = GetCodes(startCharacterIndex);
            List<int[]> rows = new List<int[]>();
            List<float> rowWidth = new List<float>();

            for (int i = 0; i < codes.Length; i++)
            {
                // Calculate total width
                float glyphWidth = font.GetGlyphWidth(codes[i], LocalScale, spacing);

                // If maxWidth is set, don't allow the label texture to exceed it
                if ((maxWidth <= 0) || ((width + glyphWidth + font.GlyphSpacing) <= maxWidth))
                {
                    width += glyphWidth;
                }
                else
                {
                    int rowLength;
                    if (wrapWords)
                    {
                        int j;
                        for (j = i; j >= lastEndOfRowByte; j--)
                        {
                            glyphWidth = font.GetGlyphWidth(codes[j], LocalScale, spacing);
                            if (j < i) // glyph i has not been added to width
                            {
                                width -= glyphWidth;
                                if (width <= maxWidth && codes[j] == DaggerfallFont.SpaceCode)
                                    break;
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
                            if (k < j - 1 || (k == j - 1 && codes[k] != DaggerfallFont.SpaceCode)) // all expect last character if it is a space
                            {
                                glyphWidth = font.GetGlyphWidth(codes[k], LocalScale, spacing);
                                width += glyphWidth;
                            }
                        }
                    }
                    else
                    {
                        rowLength = i - lastEndOfRowByte;
                    }
                    // The row of glyphs exceeded maxWidth. Add it to the list of rows and start
                    // counting width again with the remainder of the ASCII bytes.
                    List<int> content = new List<int>(codes).GetRange(lastEndOfRowByte, rowLength);
                    if (content[content.Count - 1] == DaggerfallFont.SpaceCode)
                        content.RemoveAt(content.Count - 1);
                    int[] trimmed = content.ToArray();

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
                codes = new List<int>(codes).GetRange(lastEndOfRowByte, codes.Length - lastEndOfRowByte).ToArray();

            // also get width of last line
            width = 0;
            for (int i = 0; i < codes.Length; i++)
            {
                float glyphWidth = font.GetGlyphWidth(codes[i], LocalScale, spacing);
                width += glyphWidth;
            }

            // update greatest width found so far
            if (width <= maxWidth && greatestWidthFound < width) // width should always be <= maxWidth here
                greatestWidthFound = width;

            rows.Add(codes);
            rowWidth.Add(width);

            // Create virtual layout area
            totalWidth = maxWidth;
            totalHeight = (int)(rows.Count * font.GlyphHeight);
            numTextLines = rows.Count;

            //
            // Stage 2 - Add glyphs to layout
            //

            // Second pass adds glyphs to label texture
            float xpos = 0;
            float ypos = totalHeight - font.GlyphHeight;

            //foreach (byte[] row in rows)
            for (int r = 0; r < rows.Count; r++)
            {
                int[] row = rows[r];
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
                float extraSpaceToDistribute = 0;  // needed to compute extra offset between words for HorizontalTextAlignmentSetting.Justify
                if (horizontalTextAlignment == HorizontalTextAlignmentSetting.Justify)
                {
                    for (int i = 0; i < row.Length; i++)
                    {
                        if (row[i] == DaggerfallFont.SpaceCode)
                            numSpaces++;
                    }

                    extraSpaceToDistribute = maxWidth - rowWidth[r];
                }

                xpos = (int)alignmentOffset;
                for (int i = 0; i < row.Length; i++)
                {
                    float glyphWidth = font.GetGlyphWidth(row[i], LocalScale);
                    if (xpos + glyphWidth > totalWidth + totalWidthPadding)
                        break;

                    if (row[i] == DaggerfallFont.SpaceCode)
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
                        code = row[i],
                        width = glyphWidth,
                    };

                    glyphLayout.Add(glyphPos);
                    xpos += glyphWidth + font.GlyphSpacing;
                }
                ypos -= font.GlyphHeight;
            }

            Size = new Vector2(totalWidth * textScale, totalHeight * textScale);
        }

        #endregion
    }
}