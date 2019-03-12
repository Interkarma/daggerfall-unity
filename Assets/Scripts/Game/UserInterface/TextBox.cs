// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using System.Linq;
using System.Text;
using System.IO;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public enum NumericMode { Natural, Integer, Float }

    /// <summary>
    /// Implements a single line text edit box.
    /// </summary>
    public class TextBox : Panel
    {
        int maxCharacters = 31;
        int cursorPosition = 0;
        int widthOverride = 0;
        DaggerfallFont font;
        TextCursor textCursor = new TextCursor();
        string text = string.Empty;
        string defaultText = string.Empty;
        Color defaultTextColor = new Color(0.7f, 0.7f, 0.7f, 0.8f);
        int lastStringLength = 0;
        int textOffset = 0;
        Vector2 maxSize = Vector2.zero;
        bool readOnly = false;
        bool numeric = false;
        NumericMode numericMode = NumericMode.Natural;
        bool upperOnly = false;
        bool fixedSize = false;

        public int MaxCharacters
        {
            get { return maxCharacters; }
            set { maxCharacters = value; MaxSize = CalculateMaximumSize(); }
        }

        public int WidthOverride
        {
              get { return widthOverride; }
              set { widthOverride = value; }
        }

        public DaggerfallFont Font
        {
            get { return font; }
            set { font = value; MaxSize = CalculateMaximumSize(); }
        }

        public string Text
        {
            get { return text; }
            set { text = value; SetCursorPosition(text.Length); }
        }

        public string DefaultText
        {
            get { return defaultText; }
            set { defaultText = value; }
        }

        public string ResultText
        {
            get { return GetResultText(); }
        }

        public Color DefaultTextColor
        {
            get { return defaultTextColor; }
            set { defaultTextColor = value; }
        }

        public int TextOffset
        {
            get { return textOffset; }
            set { textOffset = value; }
        }

        public Vector2 MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }

        public bool FixedSize
        {
            get { return fixedSize; }
            set { fixedSize = value; }
        }

        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }

        public TextCursor Cursor
        {
            get { return textCursor; }
        }

        public bool Numeric
        {
            get { return numeric; }
            set { numeric = value; }
        }

        public NumericMode NumericMode
        {
            get { return numericMode; }
            set { numericMode = value; }
        }

        public bool UpperOnly
        {
            get { return upperOnly; }
            set { upperOnly = value; }
        }

        public TextBox(DaggerfallFont font = null)
        {
            if (font == null)
                this.font = DaggerfallUI.DefaultFont;
            else
                this.font = font;

            Components.Add(textCursor);
            MaxSize = CalculateMaximumSize();

            if (!fixedSize)
                this.Size = CalculateCurrentSize();
        }

        public override void Update()
        {
            base.Update();

            // Do nothing if read only
            if (readOnly)
                return;

            // Do nothing if focus-enabled and does not have focus
            if (UseFocus && !HasFocus())
                return;

            // Moving cursor left and right
            if (DaggerfallUI.Instance.LastKeyCode == KeyCode.LeftArrow)
            {
                MoveCursorLeft();
                return;
            }
            else if (DaggerfallUI.Instance.LastKeyCode == KeyCode.RightArrow)
            {
                MoveCursorRight();
                return;
            }

            // Delete and Backspace
            if (DaggerfallUI.Instance.LastKeyCode == KeyCode.Delete)
            {
                if (cursorPosition != text.Length)
                {
                    text = text.Remove(cursorPosition, 1);
                    RaiseOnTypeHandler();
                }
                return;
            }
            else if (DaggerfallUI.Instance.LastKeyCode == KeyCode.Backspace)
            {
                if (cursorPosition != 0)
                {
                    text = text.Remove(cursorPosition - 1, 1);
                    MoveCursorLeft();
                    RaiseOnTypeHandler();
                }
                return;
            }

            // Home and End
            if (DaggerfallUI.Instance.LastKeyCode == KeyCode.Home)
            {
                SetCursorPosition(0);
                return;
            }
            else if (DaggerfallUI.Instance.LastKeyCode == KeyCode.End)
            {
                SetCursorPosition(text.Length);
                return;
            }

            // Return/enter
            if (DaggerfallUI.Instance.LastKeyCode == KeyCode.Return ||
                DaggerfallUI.Instance.LastKeyCode == KeyCode.KeypadEnter)
            {
                return;
            }

            // Typing characters
            char character = DaggerfallUI.Instance.LastCharacterTyped;
            if (character != 0 && text.Length < maxCharacters)
            {
                if (numeric)
                {
                    const char minus = '-';
                    const char dot = '.';

                    int value = (int)character;
                    if (value < 0x30 || value > 0x39)
                    {
                        if (character == minus)
                        {
                            // Allow negative numbers only if not natural
                            if (numericMode == NumericMode.Natural)
                                return;

                            if (cursorPosition != 0 || (text.Length > 0 && text[0] == minus))
                                return;
                        }
                        else if (character == dot)
                        {
                            // Allow dot for floats
                            if (numericMode != NumericMode.Float || text.Contains(dot))
                                return;
                        }
                        else
                        {
                            // For numeric only accept characters 0 - 9
                            return;
                        }
                    }
                }

                // For upper only, force everything to caps
                if (upperOnly)
                {
                    character = Char.ToUpper(character);
                }

                // Accept typed text
                text = text.Insert(cursorPosition, character.ToString());
                MoveCursorRight();
                RaiseOnTypeHandler();
            }

            if (lastStringLength != text.Length)
            {
                if (!fixedSize)
                    this.Size = CalculateCurrentSize();
            }
        }

        public override void Draw()
        {
            base.Draw();

            // Final text offset is value * local scale
            float textOffsetX = textOffset * LocalScale.x;
            float textOffsetY = textOffset * LocalScale.y;

            // Do not draw cursor if read only or focus-enabled and does not have focus
            if (readOnly || (UseFocus && !HasFocus()))
                textCursor.Enabled = false;
            else
                textCursor.Enabled = true;

            if (text.Length > 0)
            {
                Rect rect = Rectangle;
                font.DrawText(text, new Vector2(rect.x + textOffsetX, rect.y + textOffsetY), LocalScale, DaggerfallUI.DaggerfallDefaultInputTextColor);
            }
            else
            {
                // Draw default text while nothing else entered
                if (defaultText.Length > 0)
                {
                    Rect rect = Rectangle;
                    font.DrawText(defaultText, new Vector2(rect.x + textOffsetX, rect.y + textOffsetY), LocalScale, defaultTextColor);
                }
            }
        }

        #region Private Methods

        void MoveCursorLeft()
        {
            SetCursorPosition(cursorPosition - 1);
        }

        void MoveCursorRight()
        {
            SetCursorPosition(cursorPosition + 1);
        }

        void SetCursorPosition(int newCursorPosition)
        {
            if (newCursorPosition < 0)
                newCursorPosition = 0;
            if (newCursorPosition >= text.Length)
                newCursorPosition = text.Length;

            cursorPosition = newCursorPosition;

            float width = 0;
            if (cursorPosition > 0)
                width = GetCharacterWidthToCursor() - font.GlyphSpacing;

            textCursor.Position = new Vector2(width + textOffset, textOffset);
            textCursor.BlinkOn();
        }

        float GetCharacterWidthToCursor()
        {
            if (font == null)
                return 0;

            return font.GetCharacterWidth(text, cursorPosition);
        }

        //calculate the size required for max chars using widest glyphs
        private Vector2 CalculateMaximumSize()
        {
            int width = 0;
            for (int i = 0; i < 128; i++)
            {
                if (!font.HasGlyph(i))
                    continue;

                DaggerfallFont.GlyphInfo glyph = font.GetGlyph(i);
                if (glyph.width > width)
                {
                    width = glyph.width;
                }
            }

            int totalWidth = (width + font.GlyphSpacing) * MaxCharacters;
            return new Vector2(totalWidth, font.GlyphHeight);
        }

        // Calculate the current size
        private Vector2 CalculateCurrentSize()
        {
            int width = 0;
            byte[] asciiBytes;
            asciiBytes = Encoding.ASCII.GetBytes(text);

            for (int i = 0; i < asciiBytes.Length; i++)
            {
                // Invalid ASCII bytes are cast to a space character
                if (!font.HasGlyph(asciiBytes[i]))
                    asciiBytes[i] = DaggerfallFont.SpaceASCII;

                // Calculate total width
                DaggerfallFont.GlyphInfo glyph = font.GetGlyph(asciiBytes[i]);
                width += glyph.width + font.GlyphSpacing;
            }
            return new Vector2(width, font.GlyphHeight);
        }

        string GetResultText()
        {
            if (Text.Length == 0 && DefaultText.Length > 0)
                return DefaultText;
            else
                return Text;
        }

        #endregion

        #region Events

        // OnChange
        public delegate void OnTypeHandler();
        public event OnTypeHandler OnType;
        protected virtual void RaiseOnTypeHandler()
        {
            if (OnType != null)
                OnType();
        }

        #endregion
    }
}