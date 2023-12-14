// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
using System.Linq;
using System.Globalization;

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
        bool previousSDFState;

        // IME support
        string curCompositionString = string.Empty;
        Vector2 curCompositionSize;
        string textComposition = string.Empty;
        Vector2 cursorControlPos;
        Panel compositionPanel = new Panel();
        bool prevIMESelected;

        // Are those guaranteed to be strings of one character?
        readonly static char minus = CultureInfo.CurrentCulture.NumberFormat.NegativeSign[0];
        readonly static char dot = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

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
            set
            {
                text = value ?? string.Empty;
                SetCursorPosition(text.Length);
            }
        }

        public string DefaultText
        {
            get { return defaultText; }
            set { defaultText = value ?? string.Empty; }
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

        public bool IMECompositionInProgress
        {
            get => Input.imeIsSelected && curCompositionString.Length > 0;
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
                Size = CalculateTextSize(text);

            previousSDFState = this.font.IsSDFCapable;

            // Panel drawn behind IME composition in progress
            compositionPanel.BackgroundColor = Color.blue;
            Components.Add(compositionPanel);
        }

        public override void Update()
        {
            base.Update();

            // Do nothing if read only or using focus and does not have focus
            if (readOnly || (UseFocus && !HasFocus()))
                return;

            // SDF fonts can have different widths to classic so need to recalc cursor position any time this changes
            bool sdfState = font.IsSDFCapable;
            if (sdfState != previousSDFState)
            {
                SetCursorPosition(cursorPosition);
                previousSDFState = sdfState;
            }

            // Update cursor screen position for IME composition popups
            if (IMECompositionInProgress)
            {
                Rect imeRect = compositionPanel.Rectangle;
                Input.compositionCursorPos = new Vector2(imeRect.xMin, imeRect.yMin + font.GlyphHeight * LocalScale.y * 2f);
            }

            // Handle user toggling off IME selection in OS mid-composition
            if (!Input.imeIsSelected && prevIMESelected)
            {
                textComposition = string.Empty;
                curCompositionSize = Vector2.zero;
                compositionPanel.Enabled = false;
                SetCursorPosition(cursorPosition);
            }

            // Return/enter is not a valid input character except at the end of IME composition
            if (DaggerfallUI.Instance.LastKeyCode == KeyCode.Return ||
                DaggerfallUI.Instance.LastKeyCode == KeyCode.KeypadEnter)
            {
                if (IMECompositionInProgress)
                    InsertIMEComposition();
                return;
            }

            // Process control input
            // Exit if this input was handled by below call
            // Otherwise keys like backspace can insert spurious "?" characters on some platforms
            if (HandleControlInput())
                return;

            // Process all other input
            HandleCharacterInput();
            UpdateInlineComposition();

            prevIMESelected = Input.imeIsSelected;
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

            if (textComposition.Length > 0)
            {
                // Draw text with inline IME composition
                Rect rect = Rectangle;
                font.DrawText(textComposition, new Vector2(rect.x + textOffsetX, rect.y + textOffsetY), LocalScale, DaggerfallUI.DaggerfallDefaultInputTextColor);
            }
            else if (text.Length > 0)
            {
                // Draw text as entered so far
                Rect rect = Rectangle;
                font.DrawText(text, new Vector2(rect.x + textOffsetX, rect.y + textOffsetY), LocalScale, DaggerfallUI.DaggerfallDefaultInputTextColor);
            }
            else
            {
                // Draw default text while nothing else entered
                if (defaultText != null && defaultText.Length > 0)
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
            if (!font.IsSDFCapable)
            {
                if (cursorPosition > 0)
                    width = GetCharacterWidthToCursor() - font.GlyphSpacing;
                textCursor.Position = new Vector2(width + textOffset, textOffset);
            }
            else
            {
                width = GetCharacterWidthToCursor();
                textCursor.Position = new Vector2(width, textOffset);
            }
            textCursor.BlinkOn();

            // Store cursor control position then offset for inline composition width
            cursorControlPos = textCursor.Position;
            textCursor.Position += new Vector2(curCompositionSize.x, 0);
        }

        float GetCharacterWidthToCursor()
        {
            if (font == null)
                return 0;

            return font.CalculateTextWidth(text, LocalScale, 0, cursorPosition);
        }

        // Calculate the size required for max chars using widest glyphs
        // This is using classic glyphs which are typically wider that SDF glyphs
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

            float totalWidth = (width + font.GlyphSpacing) * MaxCharacters;
            return new Vector2(totalWidth, font.GlyphHeight);
        }

        // Calculate the current size
        private Vector2 CalculateTextSize(string text)
        {
            return new Vector2(font.CalculateTextWidth(text, Vector2.one), font.GlyphHeight);
        }

        // Output text is either default or what player has entered
        string GetResultText()
        {
            if (Text.Length == 0 && DefaultText.Length > 0)
                return DefaultText;
            else
                return Text;
        }

        // Handle control input such as cursor movement, delete/backspace, home/end
        // Returns true when control input was handled by this method or false when input processing should continue
        // The caller should stop processing input on this loop when this method returns true
        // And continue processing input on this loop when this method returns false
        bool HandleControlInput()
        {
            // During IME composition these keys are always handed over to provider
            if (IMECompositionInProgress)
                return false;

            // Moving cursor left and right
            KeyCode lastKeyCode = DaggerfallUI.Instance.LastKeyCode;
            if (lastKeyCode == KeyCode.LeftArrow)
            {
                MoveCursorLeft();
                return true;
            }
            else if (lastKeyCode == KeyCode.RightArrow)
            {
                MoveCursorRight();
                return true;
            }

            // Delete and Backspace
            if (lastKeyCode == KeyCode.Delete)
            {
                if (cursorPosition != text.Length)
                {
                    text = text.Remove(cursorPosition, 1);
                    RaiseOnTypeHandler();
                }
                return true;
            }
            else if (lastKeyCode == KeyCode.Backspace)
            {
                if (cursorPosition != 0)
                {
                    text = text.Remove(cursorPosition - 1, 1);
                    MoveCursorLeft();
                    RaiseOnTypeHandler();
                }
                return true;
            }

            // Home and End
            if (lastKeyCode == KeyCode.Home)
            {
                SetCursorPosition(0);
                return true;
            }
            else if (lastKeyCode == KeyCode.End)
            {
                SetCursorPosition(text.Length);
                return true;
            }

            // Escape - just absorb this so escape does not send spurious input into textbox
            if (lastKeyCode == KeyCode.Escape)
                return true;

            return false;
        }

        // Handling for basic character input or IME composition insert
        void HandleCharacterInput()
        {
            char character = DaggerfallUI.Instance.LastCharacterTyped;

            // IME composition gets first priority
            // Finalisation is triggered as a single character event but may need to insert a string
            if (character != 0 && IMECompositionInProgress)
            {
                InsertIMEComposition();
                return;
            }

            // Typing characters
            if (character != 0 && text.Length < maxCharacters)
            {
                if (numeric)
                {
                    int value = (int)character;
                    if (value < 0x30 || value > 0x39)
                    {
                        // Allow negative numbers only if not natural
                        if (character == minus && numericMode != NumericMode.Natural)
                        {
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
                            // Also accept numeric keys, even when not shifted
                            KeyCode keyCode = DaggerfallUI.Instance.LastKeyCode;
                            if (numericMode == NumericMode.Natural && keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9)
                                character = (char)(keyCode - KeyCode.Alpha0 + 0x30);
                            else
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

                // Update text size
                if (lastStringLength != text.Length)
                {
                    if (!fixedSize)
                        Size = CalculateTextSize(text);
                    lastStringLength = text.Length;
                }
            }
        }

        // Update inline IME text composition
        void UpdateInlineComposition()
        {
            // Do nothing if IME not enabled
            if (!Input.imeIsSelected)
                return;

            // Update composition string in progress
            curCompositionString = Input.compositionString;
            curCompositionSize = CalculateTextSize(curCompositionString);

            // Create inlined text output
            if (curCompositionString.Length == 0)
            {
                textComposition = string.Empty;
                curCompositionSize = Vector2.zero;
                compositionPanel.Enabled = false;
            }
            else
            {
                textComposition = text.Insert(cursorPosition, curCompositionString);
                compositionPanel.Enabled = true;
            }

            // Refresh cursor virtual position
            if (!fixedSize)
                Size = CalculateTextSize(textComposition);
            SetCursorPosition(cursorPosition);

            // Update composition panel
            compositionPanel.Position = cursorControlPos;
            compositionPanel.Size = curCompositionSize;
        }

        // Inserts IME composition at current cursor position
        void InsertIMEComposition()
        {
            text = text.Insert(cursorPosition, curCompositionString);

            if (text.Length >= maxCharacters)
                text = text.Substring(0, maxCharacters);

            if (!fixedSize)
                Size = CalculateTextSize(textComposition);

            SetCursorPosition(cursorPosition + curCompositionString.Length);
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