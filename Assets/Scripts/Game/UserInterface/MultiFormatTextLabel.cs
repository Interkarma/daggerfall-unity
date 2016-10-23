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
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A multi-format text label supporting variable pixel fonts and layouts.
    /// Designed for Daggerfall's formatted text records such as books and status popups.
    /// This class will be improved over time as needed.
    /// </summary>
    public class MultiFormatTextLabel : BaseScreenComponent
    {
        const int tabWidth = 45;

        PixelFont font;
        int rowLeading = 0;
        Vector2 shadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
        Color textColor = DaggerfallUI.DaggerfallDefaultTextColor;
        Color shadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
        HorizontalAlignment textAlignment = HorizontalAlignment.None;
        List<TextLabel> labels = new List<TextLabel>();
        TextLabel lastLabel;

        int totalWidth = 0;
        int totalHeight = 0;
        int cursorX = 0;
        int cursorY = 0;
        int tabStop = 0;

        public PixelFont Font
        {
            get { return GetFont(); }
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

        public HorizontalAlignment TextAlignment
        {
            get { return textAlignment; }
            set { textAlignment = value; }
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
        /// Adds formatted text labels from a RSC token array.
        /// </summary>
        /// <param name="tokens">Daggerfall RSC token array.</param>
        public virtual void SetText(TextFile.Token[] tokens)
        {
            LayoutTextElements(tokens);
        }

        /// <summary>
        /// Adds formatted text labels from a TextAsset.
        /// Currently only supports newlines, not tabs or other formatting characters.
        /// </summary>
        /// <param name="textAsset">Source TextAsset.</param>
        public virtual void SetText(TextAsset textAsset)
        {
            StringReader reader = new StringReader(textAsset.text);
            if (reader == null)
                return;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                AddTextLabel(line);
                NewLine();
            }
        }

        /// <summary>
        /// Adds a single text item.
        /// Each subsequent text item will be appended to the previous text item position.
        /// Call NewLine() to start a new line.
        /// </summary>
        /// <param name="text">Text for this label.</param>
        /// <param name="font">Font for this label.</param>
        /// <returns>TextLabel.</returns>
        public TextLabel AddTextLabel(string text, PixelFont font = null)
        {
            if (font == null)
                font = GetFont();

            TextLabel textLabel = new TextLabel();
            textLabel.AutoSize = AutoSizeModes.None;
            textLabel.Font = font;
            textLabel.Position = new Vector2(cursorX, cursorY);
            textLabel.Text = text;
            textLabel.Parent = this;

            textLabel.TextColor = TextColor;
            textLabel.ShadowColor = ShadowColor;
            textLabel.ShadowPosition = ShadowPosition;

            if (textAlignment != HorizontalAlignment.None)
                textLabel.HorizontalAlignment = textAlignment;

            labels.Add(textLabel);
            lastLabel = textLabel;

            cursorX += textLabel.TextWidth;

            return textLabel;
        }

        /// <summary>
        /// Start a new line.
        /// </summary>
        public void NewLine()
        {
            cursorX = 0;
            cursorY += LineHeight;
            totalHeight += LineHeight;
            tabStop = 0;
        }

        /// <summary>
        /// Clears current labels.
        /// </summary>
        public void Clear()
        {
            labels.Clear();
            totalWidth = 0;
            totalHeight = 0;
            cursorX = 0;
            cursorY = 0;
        }

        #region Protected Methods

        protected virtual void SetRowLeading(int amount)
        {
            this.rowLeading = amount;
        }

        #endregion

        #region Private Methods

        int LineHeight
        {
            get { return GetFont().GlyphHeight + rowLeading; }
        }

        void LayoutTextElements(TextFile.Token[] tokens)
        {
            Clear();

            TextFile.Token token = new TextFile.Token();
            TextFile.Token nextToken = new TextFile.Token();
            for (int i = 0; i < tokens.Length; i++)
            {
                token = tokens[i];
                nextToken.formatting = TextFile.Formatting.Nothing;
                if (i < tokens.Length - 1)
                    nextToken = tokens[i + 1];

                // Still working out rules for justify logic
                // This will adapt over time as required
                switch (token.formatting)
                {
                    case TextFile.Formatting.NewLine:
                        NewLine();
                        break;
                    case TextFile.Formatting.JustifyLeft:
                        NewLine();
                        break;
                    case TextFile.Formatting.JustifyCenter:
                        if (lastLabel != null)
                            lastLabel.HorizontalAlignment = HorizontalAlignment.Center;
                        NewLine();
                        break;
                    case TextFile.Formatting.PositionPrefix:
                        if (token.x != 0)
                        {
                            // Tab by specific number of pixels
                            cursorX = token.x;
                        }
                        else
                        {
                            // Tab to next tab stop
                            tabStop++;
                            cursorX = tabStop * tabWidth;
                        }
                        break;
                    case TextFile.Formatting.Text:
                        AddTextLabel(token.text, font);
                        break;
                    default:
                        Debug.Log("MultilineTextLabel: Unknown formatting token: " + (int)token.formatting);
                        break;
                }

                if (lastLabel != null)
                {
                    int rowWidth = (int)lastLabel.Position.x + lastLabel.TextWidth;
                    if (rowWidth > totalWidth)
                        totalWidth = rowWidth;
                }
            }

            Size = new Vector2(totalWidth, totalHeight);
        }

        PixelFont GetFont()
        {
            if (font == null)
                font = DaggerfallUI.DefaultFont;

            return font;
        }

        #endregion
    }
}