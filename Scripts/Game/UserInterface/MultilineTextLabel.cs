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
using System.Collections.Generic;
using System.Text;
using System.IO;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A multiline text label using a pixel font.
    /// Designed for Daggerfall's multiline text records.
    /// This class will be extended later as required.
    /// </summary>
    public class MultilineTextLabel : BaseScreenComponent
    {
        PixelFont font;
        int rowLeading = 0;
        HorizontalAlignment rowAlignment = HorizontalAlignment.None;
        int glyphSpacing = 1;
        Vector2 shadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
        Color textColor = DaggerfallUI.DaggerfallDefaultTextColor;
        Color shadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
        FilterMode filterMode = FilterMode.Point;
        List<TextLabel> labels = new List<TextLabel>();
        TextLabel lastLabel;
        int totalWidth = 0;
        int totalHeight = 0;

        public PixelFont Font
        {
            get { return font; }
            set { font = value; }
        }

        /// <summary>
        /// Gets or sets extra leading between rows.
        /// </summary>
        public int ExtraLeading
        {
            get { return rowLeading; }
            set { SetRowLeading(value); }
        }

        /// <summary>
        /// Gets or sets row horizontal alignment.
        /// If set to None, text will use formatting as defined in token stream.
        /// Otherwise, text will ignore formatting defined in token stream.
        /// </summary>
        public HorizontalAlignment RowAlignment
        {
            get { return rowAlignment; }
            set { SetRowAlignment(value); }
        }

        public int GlyphSpacing
        {
            get { return glyphSpacing; }
            set { SetGlyphSpacing(value); }
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

        public FilterMode FilterMode
        {
            get { return filterMode; }
            set { SetFilterMode(value); }
        }

        public override void Draw()
        {
            base.Draw();

            if (labels.Count == 0)
                return;

            for (int i = 0; i < labels.Count; i++)
            {
                labels[i].Draw();
            }
        }

        /// <summary>
        /// Sets the multiline text from a multiline record token array.
        /// </summary>
        /// <param name="multilineRecordTokens">Multiline record token array.</param>
        public virtual void SetTextTokens(TextFile.Token[] multilineRecordTokens)
        {
            if (font == null)
                font = DaggerfallUI.Instance.DefaultFont;

            LayoutTextRows(multilineRecordTokens);
        }

        #region Protected Methods

        protected virtual void SetGlyphSpacing(int value)
        {
            this.glyphSpacing = value;
            // TODO: Update labels
        }

        protected virtual void SetRowLeading(int amount)
        {
            this.rowLeading = amount;
        }

        protected virtual void SetRowAlignment(HorizontalAlignment alignment)
        {
            this.rowAlignment = alignment;
        }

        protected virtual void SetFilterMode(FilterMode value)
        {
            this.filterMode = value;
        }

        #endregion

        #region Private Methods

        void LayoutTextRows(TextFile.Token[] tokens)
        {
            labels.Clear();
            totalWidth = 0;
            totalHeight = 0;

            int x = 0, y = 0;
            int newLine = font.GlyphHeight + rowLeading;
            foreach (var token in tokens)
            {
                // Handle user-specified alignment
                if (rowAlignment != HorizontalAlignment.None && lastLabel != null)
                    lastLabel.HorizontalAlignment = rowAlignment;

                // A justify code also seems to indicate a newline
                switch (token.formatting)
                {
                    case TextFile.Formatting.NewLine:
                        y += newLine;
                        break;
                    case TextFile.Formatting.JustifyLeft:
                        if (rowAlignment == HorizontalAlignment.None && lastLabel != null)
                            lastLabel.HorizontalAlignment = HorizontalAlignment.Left;
                        y += newLine;
                        break;
                    case TextFile.Formatting.JustifyCenter:
                        if (rowAlignment == HorizontalAlignment.None && lastLabel != null)
                            lastLabel.HorizontalAlignment = HorizontalAlignment.Center;
                        y += newLine;
                        break;
                    case TextFile.Formatting.Text:
                        TextLabel label = AddTextLabel(font, new Vector2(x, y), token.text);
                        label.HorizontalAlignment = rowAlignment;
                        label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
                        label.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
                        label.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
                        labels.Add(label);
                        lastLabel = label;
                        break;
                    default:
                        Debug.Log("MultilineTextLabel: Unknown formatting token: " + (int)token.formatting);
                        break;
                }

                if (lastLabel != null)
                {
                    Vector2 size = lastLabel.Size;
                    if (size.x > totalWidth)
                        totalWidth = (int)size.x;
                }
            }

            totalHeight = labels.Count * (font.GlyphHeight + rowLeading);
            Size = new Vector2(totalWidth, totalHeight);
        }

        TextLabel AddTextLabel(PixelFont font, Vector2 position, string text, int glyphSpacing = 1)
        {
            TextLabel textLabel = new TextLabel();
            textLabel.Scaling = Scaling.None;
            textLabel.HorizontalAlignment = rowAlignment;
            textLabel.Font = font;
            textLabel.Position = position;
            textLabel.FilterMode = filterMode;
            textLabel.GlyphSpacing = glyphSpacing;
            textLabel.Text = text;
            textLabel.Parent = this;

            return textLabel;
        }

        #endregion
    }
}