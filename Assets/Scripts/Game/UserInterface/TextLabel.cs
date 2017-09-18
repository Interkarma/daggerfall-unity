// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A simple single-line text label using a pixel font.
    /// </summary>
    public class TextLabel : BaseScreenComponent
    {
        const int minTextureDim = 8;

        int maxCharacters = -1;
        int startCharacterIndex = 0; // can be used to offset start character of textlabel's text (used by listbox's entry-wise horizontal scroll mode)
        PixelFont font;
        string text = string.Empty;
        byte[] asciiBytes;
        int totalWidth;
        int totalHeight;
        int textureWidth;
        int textureHeight;
        int numTextLines = 1; // 1 in unwrapped, n in wrapped
        Texture2D labelTexture;
        Vector2 shadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
        Color textColor = DaggerfallUI.DaggerfallDefaultTextColor;
        Color shadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;

        int maxWidth = -1;
        bool wrapText = false; // wrap text - but will tear words that are reaching
        bool wrapWords = false; // wrap words - no word tearing

        bool makeTextureNoLongerReadable = true; // in pixel-wise scroll mode with restricted render area this flag is set to false since textures must be readable for this mode to work

        // restricted render area can be used to force label rendering inside this rect (used for text rendering in window frames where text is larger than frame)
        bool useRestrictedRenderArea = false;
        Rect rectRestrictedRenderArea;

        float textScale = 1.0f; // scale text 

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

        public PixelFont Font
        {
            get { return font; }
            set { font = value; CreateLabelTexture(); }
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

        public Texture2D Texture
        {
            get { return labelTexture; }
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
        /// set the maximum allowed width of the textlabel in pixels
        /// </summary>
        public int MaxWidth
        {
            get { return maxWidth; }
            set { maxWidth = value; }
        }

        /// <summary>
        /// enable text wrapping - use MaxWidth property to set maximum allowed width of the textlabel
        /// </summary>
        public bool WrapText
        {
            get { return wrapText; }
            set { wrapText = value; CreateLabelTexture(); }
        }

        /// <summary>
        /// enable word wrapping - words will no longer get tear apart if possible - make sure to enable property WrapText to make this property to have an effect
        /// </summary>
        public bool WrapWords
        {
            get { return wrapWords; }
            set { wrapWords = value; CreateLabelTexture(); }
        }

        /// <summary>
        /// set a restricted render area for the textlabel - the textlabel's content will only be rendered inside the specified Rect's bounds
        /// </summary>
        public Rect RectRestrictedRenderArea
        {
            get { return rectRestrictedRenderArea; }
            set
            {
                rectRestrictedRenderArea = value;
                useRestrictedRenderArea = true;
                makeTextureNoLongerReadable = false;
            }
        }

        /// <summary>
        /// set text scale factor - 1.0f is default value, 0.5f is half sized text, 2.0f double sized text and so on
        /// </summary>
        public float TextScale
        {
            get { return textScale; }
            set
            {
                textScale = Math.Max(0.1f, Math.Min(2.0f, value));
                CreateLabelTexture();
            }
        }

        public void setPosition(Vector2 newPos)
        {
            this.Position = newPos;
        }

        public TextLabel(DaggerfallFont font = null)
        {
            if (font != null)
                Font = font;
        }

        public override void Draw()
        {
            base.Draw();

            if (string.IsNullOrEmpty(text) || labelTexture == null)
                return;

            // Store starting colour
            Color guiColor = GUI.color;

            Rect totalRect;
            Rect innerRect;
            Texture2D textureToDraw;
            if (useRestrictedRenderArea)
            {
                Rect rectLabel = new Rect(this.Parent.Position + this.Position, this.Size * textScale);

                float leftCut = Math.Max(0, rectRestrictedRenderArea.xMin - rectLabel.xMin) / textScale;
                float rightCut = Math.Max(0, rectLabel.xMax - rectRestrictedRenderArea.xMax) / textScale;
                float topCut = Math.Max(0, rectRestrictedRenderArea.yMin - rectLabel.yMin) / textScale;
                float bottomCut = Math.Max(0, rectLabel.yMax - rectRestrictedRenderArea.yMax) / textScale;

                if ((leftCut == 0) && (rightCut == 0) && (topCut == 0) && (bottomCut == 0))
                {
                    totalRect = Rectangle;

                    // compensate for textScale
                    totalRect.xMax = totalRect.xMin + totalRect.width * textScale;
                    totalRect.yMax = totalRect.yMin + totalRect.height * textScale;

                    innerRect = new Rect(0, 0, (float)totalWidth / (float)textureWidth, (float)totalHeight / (float)textureHeight);
                    textureToDraw = labelTexture;
                }
                else
                {
                    int newWidth = (int)(totalWidth - leftCut - rightCut);
                    int newHeight = (int)(totalHeight - topCut - bottomCut);

                    if ((newWidth <= 0) || (newHeight <= 0))
                        return;

                    Color[] subColors = labelTexture.GetPixels((int)leftCut, (int)bottomCut, newWidth, newHeight);
                    Texture2D subTex = new Texture2D(newWidth, newHeight, labelTexture.format, false);
                    subTex.SetPixels(subColors, 0);
                    subTex.Apply(false);
                    subTex.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

                    float xMinScreen = (rectLabel.xMin + leftCut * textScale) * LocalScale.x + this.Parent.Parent.Rectangle.x;
                    float yMinScreen = (rectLabel.yMin + topCut * textScale) * LocalScale.y + this.Parent.Parent.Rectangle.y;
                    float xMaxScreen = (rectLabel.xMax - rightCut * textScale) * LocalScale.x + this.Parent.Parent.Rectangle.x;
                    float yMaxScreen = (rectLabel.yMax - bottomCut * textScale) * LocalScale.y + this.Parent.Parent.Rectangle.y;
                    totalRect = Rect.MinMaxRect(xMinScreen, yMinScreen, xMaxScreen, yMaxScreen);

                    //totalRect = Rectangle;
                    //totalRect.xMin += leftCut;
                    //totalRect.yMin += topCut;
                    //totalRect.xMax = totalRect.xMin + totalRect.width - rightCut;
                    //totalRect.yMax = totalRect.yMin + totalRect.height - bottomCut;

                    // compensate for textScale
                    //totalRect.xMax = totalRect.xMin + totalRect.width * textScale;
                    //totalRect.yMax = totalRect.yMin + totalRect.height * textScale;

                    innerRect = new Rect(0, 0, 1, 1); //(float)newWidth / (float)textureWidth, (float)newHeight / (float)textureHeight);
                    textureToDraw = subTex;
                }
            }
            else
            {
                totalRect = Rectangle;

                // compensate for textScale
                totalRect.xMax = totalRect.xMin + totalRect.width * textScale;
                totalRect.yMax = totalRect.yMin + totalRect.height * textScale;

                innerRect = new Rect(0, 0, (float)totalWidth / (float)textureWidth, (float)totalHeight / (float)textureHeight);
                textureToDraw = labelTexture;
            }

            // Draw shadow            
            if (shadowPosition != Vector2.zero)
            {
                Rect shadowRect = totalRect;
                shadowRect.x += shadowPosition.x * LocalScale.x;
                shadowRect.y += shadowPosition.y * LocalScale.y;
                GUI.color = shadowColor;
                GUI.DrawTextureWithTexCoords(shadowRect, textureToDraw, innerRect);
            }

            // Draw text
            GUI.color = textColor;
            GUI.DrawTextureWithTexCoords(totalRect, textureToDraw, innerRect);

            // Restore starting colour
            GUI.color = guiColor;
        }

        public virtual void UpdateLabelTexture()
        {
            CreateLabelTexture();
        }

        #region Protected Methods

        protected virtual void SetText(string value)
        {
            // Truncate string to max characters
            if (maxCharacters != -1)
                value = value.Substring(0, Math.Min(value.Length, maxCharacters));

            this.text = value;
            CreateLabelTexture();
        }

        protected virtual void CreateLabelTexture()
        {
            if (!wrapText)
                CreateLabelTextureSingleLine();
            else
                CreateLabelTextureWrapped();
        }

        void CreateLabelTextureSingleLine()
        {
            if (font == null)
                font = DaggerfallUI.DefaultFont;

            // set a local maxWidth that compensates for textScale
            int maxWidth = (int)(this.maxWidth / textScale);

            // First pass encodes ASCII and calculates final dimensions
            int width = 0;
            asciiBytes = Encoding.ASCII.GetBytes(text);
            for (int i = startCharacterIndex; i < asciiBytes.Length; i++)
            {
                // Invalid ASCII bytes are cast to a space character
                if (!font.HasGlyph(asciiBytes[i]))
                    asciiBytes[i] = PixelFont.SpaceASCII;

                // Calculate total width
                PixelFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);
                width += glyph.width + font.GlyphSpacing;
            }

            if (maxWidth > 0 && width > maxWidth)
                width = maxWidth;

            // Create target label texture
            totalWidth = width;
            totalHeight = (int)(font.GlyphHeight);
            numTextLines = 1;
            labelTexture = CreateLabelTexture(totalWidth, totalHeight);
            if (labelTexture == null)
                throw new Exception("TextLabel failed to create labelTexture.");

            // Second pass adds glyphs to label texture
            int xpos = 0;
            for (int i = startCharacterIndex; i < asciiBytes.Length; i++)
            {
                PixelFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);
                if (xpos + glyph.width >= totalWidth)
                    break;

                labelTexture.SetPixels32(xpos, 0, glyph.width, totalHeight, glyph.colors);
                xpos += glyph.width + font.GlyphSpacing;
            }
            labelTexture.Apply(false, makeTextureNoLongerReadable);
            labelTexture.filterMode = font.FilterMode;
            this.Size = new Vector2(totalWidth, totalHeight);
        }

        void CreateLabelTextureWrapped()
        {
            if (font == null)
                font = DaggerfallUI.DefaultFont;

            // set a local maxWidth that compensates for textScale
            int maxWidth = (int)(this.maxWidth / textScale);

            // First pass encodes ASCII and calculates final dimensions
            int width = 0;
            int greatestWidthFound = 0;
            int lastEndOfRowByte = 0;
            asciiBytes = Encoding.ASCII.GetBytes(text);
            List<byte[]> rows = new List<byte[]>();

            for (int i = 0; i < asciiBytes.Length; i++)
            {
                // Invalid ASCII bytes are cast to a space character
                if (!font.HasGlyph(asciiBytes[i]))
                    asciiBytes[i] = PixelFont.SpaceASCII;

                // Calculate total width
                PixelFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);                

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
                                if (width <= maxWidth && asciiBytes[j] == PixelFont.SpaceASCII)
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
                            glyph = font.GetGlyph(asciiBytes[k]);
                            width += glyph.width + font.GlyphSpacing;
                        }
                    }
                    else
                    {
                        rowLength = i - lastEndOfRowByte;
                    }
                    // The row of glyphs exceeded maxWidth. Add it to the list of rows and start
                    // counting width again with the remainder of the ASCII bytes.
                    byte[] trimmed = new List<byte>(asciiBytes).GetRange(lastEndOfRowByte, rowLength).ToArray();
                    rows.Add(trimmed);

                    // update greatest width found so far
                    if (greatestWidthFound < width)
                        greatestWidthFound = width;
                    
                    // reset width for next line
                    width = 0;

                    // Resume interation over remainder of ASCII bytes
                    //asciiBytes = new List<byte>(asciiBytes).GetRange(i, asciiBytes.Length - i).ToArray();
                    //i = 0;

                    lastEndOfRowByte = i + 1; // position after space for next line, note: i will be increased on next loop iteration anyway - so no need to increase i
                }
            }

            if (lastEndOfRowByte > 0)
                asciiBytes = new List<byte>(asciiBytes).GetRange(lastEndOfRowByte, asciiBytes.Length - lastEndOfRowByte).ToArray();

            // also get width of last line
            width = 0;
            for (int i=0; i < asciiBytes.Length; i++)
            {
                PixelFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);
                width += glyph.width + font.GlyphSpacing;
            }

            // update greatest width found so far
            if (width <= maxWidth && greatestWidthFound < width) // width should always be <= maxWidth here
                greatestWidthFound = width;

            rows.Add(asciiBytes);


            width = greatestWidthFound;

            // Create target label texture
            totalWidth = width;
            totalHeight = (int)(rows.Count * font.GlyphHeight);
            numTextLines = rows.Count;
            labelTexture = CreateLabelTexture(totalWidth, totalHeight);
            if (labelTexture == null)
                throw new Exception("TextLabel failed to create labelTexture.");

            // Second pass adds glyphs to label texture
            int xpos = 0;
            int ypos = totalHeight - font.GlyphHeight;

            foreach (byte[] row in rows)
            {
                xpos = 0;
                for (int i = 0; i < row.Length; i++)
                {
                    PixelFont.GlyphInfo glyph = font.GetGlyph(row[i]);
                    if (xpos + glyph.width > totalWidth)
                        break;

                    labelTexture.SetPixels32(xpos, ypos, glyph.width, font.GlyphHeight, glyph.colors);
                    xpos += glyph.width + font.GlyphSpacing;
                }
                ypos -= font.GlyphHeight;
            }

            labelTexture.Apply(false, makeTextureNoLongerReadable);
            labelTexture.filterMode = font.FilterMode;
            this.Size = new Vector2(totalWidth, totalHeight);
        }

        protected virtual Texture2D CreateLabelTexture(int width, int height)
        {
            // Enforce minimum texture dimensions of 8x8 to avoid "degenerate image" error in Unity 5.2
            textureWidth = (width < minTextureDim) ? minTextureDim : width;
            textureHeight = (height < minTextureDim) ? minTextureDim : height;

            // Create target texture and init to clear
            Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
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