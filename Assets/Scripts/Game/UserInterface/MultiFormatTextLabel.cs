// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Numidium
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
        const int tabWidth = 35;

        DaggerfallFont font;
        float textScale = 1.0f; // scale text 
        int rowLeading = 0;
        Vector2 shadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
        Color textColor = DaggerfallUI.DaggerfallDefaultTextColor;
        Color highlightColor = DaggerfallUI.DaggerfallHighlightTextColor;
        Color shadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
        HorizontalAlignment textAlignment = HorizontalAlignment.None;
        List<TextLabel> labels = new List<TextLabel>();
        TextLabel lastLabel = new TextLabel();

        int totalWidth = 0;
        int totalHeight = 0;
        int cursorX = 0;
        int cursorY = 0;
        int tabStop = 0;

        bool wrapText = false;
        bool wrapWords = false;
        int maxTextWidth = 0;
        int maxTextHeight = 0;
        int actualTextHeight = 0;

        int minTextureDimTextLabel = TextLabel.limitMinTextureDim; // set this with property MinTextureDim to higher values if you experience scaling issues with small texts (e.g. inventory infopanel)

        public DaggerfallFont Font
        {
            get { return GetFont(); }
            set { font = value; }
        }

        public bool WrapText
        {
            get { return wrapText; }
            set { wrapText = value; }
        }

        public bool WrapWords
        {
            get { return wrapWords; }
            set { wrapWords = value; }
        }

        public int MaxTextWidth
        {
            get { return maxTextWidth; }
            set { maxTextWidth = value; }
        }

        public int MaxTextHeight
        {
            get { return maxTextHeight; }
            set { maxTextHeight = value; }
        }

        public int ActualTextHeight
        {
            get { return actualTextHeight; }
        }

        /// <summary>
        /// used to set min texture dims of textlabel to higher values if there would be aspect or scaling issues with small texts otherwise (e.g. some single-lined textlabels in inventory infopanel)
        /// </summary>
        public int MinTextureDimTextLabel
        {
            get { return minTextureDimTextLabel; }
            set { minTextureDimTextLabel = Math.Max(TextLabel.limitMinTextureDim, value); }
        }

        /// <summary>
        /// set text scale factor - 1.0f is default value, 0.5f is half sized text, 2.0f double sized text and so on
        /// </summary>
        public float TextScale
        {
            get { return textScale; }
            set { textScale = value; }
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

        public Color HighlightColor
        {
            get { return highlightColor; }
            set { highlightColor = value; }
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

        public void ResizeY(float newSize)
        {
            Size = new Vector2(totalWidth, newSize);
        }

        public int LineCount
        {
            get { return labels.Count; }
        }

        public List<TextLabel> TextLabels
        {
            get { return labels; }
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
                AddTextLabel(line, GetFont(), DaggerfallUI.DaggerfallDefaultTextColor);
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
        /// <param name="color">Text color for this label.</param>
        /// <returns>TextLabel.</returns>
        public TextLabel AddTextLabel(string text, DaggerfallFont font, Color color)
        {
            TextLabel textLabel = new TextLabel();

            textLabel.Parent = this; //Establish parent early.

            textLabel.AutoSize = AutoSizeModes.None;
            textLabel.MinTextureDim = minTextureDimTextLabel;
            textLabel.Font = font;
            textLabel.Position = new Vector2(cursorX, cursorY);
            textLabel.WrapText = wrapText;
            textLabel.WrapWords = wrapWords;
            textLabel.TextScale = TextScale;

            // Use max width if it has been specified
            if (maxTextWidth > 0)
                textLabel.MaxWidth = maxTextWidth;
            textLabel.Text = text;
            textLabel.TextColor = color;
            textLabel.ShadowColor = ShadowColor;
            textLabel.ShadowPosition = ShadowPosition;

            if (textAlignment != HorizontalAlignment.None)
                textLabel.HorizontalAlignment = textAlignment;

            if (UseRestrictedRenderArea)
            {
                textLabel.RestrictedRenderAreaCoordinateType = RestrictedRenderAreaCoordinateType;
                textLabel.RectRestrictedRenderArea = RectRestrictedRenderArea;
                textLabel.RestrictedRenderAreaCustomParent = RestrictedRenderAreaCustomParent;
            }

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
            cursorY += lastLabel.TextHeight + rowLeading;
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

        public int LineHeight
        {
            get {
                int lineHeight = lastLabel.TextHeight / lastLabel.NumTextLines + rowLeading;
                return lineHeight;
            }
        }

        public void UpdateRestrictedRenderArea()
        {
            for (int i = 0; i < labels.Count; i++)
            {
                TextLabel textLabel = labels[i];
                textLabel.RestrictedRenderAreaCoordinateType = RestrictedRenderAreaCoordinateType;
                textLabel.RectRestrictedRenderArea = RectRestrictedRenderArea;
                textLabel.RestrictedRenderAreaCustomParent = RestrictedRenderAreaCustomParent;
                labels[i] = textLabel;
            }
        }

        public void ChangeScrollPosition(int amount)
        {
            for (int i = 0; i < labels.Count; i++)
            {
                TextLabel textLabel = labels[i];
                textLabel.Position = new Vector2(textLabel.Position.x, textLabel.Position.y + amount);
                labels[i] = textLabel;
            }
        }

        #region Protected Methods

        protected virtual void SetRowLeading(int amount)
        {
            this.rowLeading = amount;
        }

        #endregion

        #region Private Methods

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
                    case TextFile.Formatting.Nothing:
                    case TextFile.Formatting.JustifyLeft:
                        NewLine();
                        totalHeight += LineHeight; // Justify left adds to height regardless of there being anything afterwards
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
                        AddTextLabel(token.text, font, TextColor);
                        break;
                    case TextFile.Formatting.TextHighlight:
                        AddTextLabel(token.text, font, HighlightColor);
                        break;
                    case TextFile.Formatting.TextQuestion:
                        AddTextLabel(token.text, font, DaggerfallUI.DaggerfallQuestionTextColor);
                        break;
                    case TextFile.Formatting.TextAnswer:
                        AddTextLabel(token.text, font, DaggerfallUI.DaggerfallAnswerTextColor);
                        break;
                    case TextFile.Formatting.InputCursorPositioner:
                        break;
                    case TextFile.Formatting.EndOfRecord:
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
                    int rowHeight = (int)lastLabel.Position.y + lastLabel.TextHeight;
                    if (rowHeight > totalHeight)
                        totalHeight = rowHeight;

                    actualTextHeight = totalHeight + lastLabel.TextHeight;
                }
            }

            if (maxTextHeight > 0 && totalHeight > maxTextHeight)
            {
                totalHeight = maxTextHeight;
            }

            Size = new Vector2(totalWidth, totalHeight);
        }

        DaggerfallFont GetFont()
        {
            if (font == null)
                font = DaggerfallUI.DefaultFont;

            return font;
        }

        #endregion
    }
}