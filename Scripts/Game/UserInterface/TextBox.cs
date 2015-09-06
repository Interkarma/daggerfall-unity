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

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Implements a single line text edit box.
    /// </summary>
    public class TextBox : Panel
    {
        int maxCharacters = 31;
        int cursorPosition = 0;
        DaggerfallFont font;
        TextCursor textCursor = new TextCursor();
        string text = string.Empty;

        public int MaxCharacters
        {
            get { return maxCharacters; }
            set { maxCharacters = value; }
        }

        public DaggerfallFont Font
        {
            get { return font; }
            set { font = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; SetCursorPosition(text.Length); }
        }

        public TextBox()
        {
            font = DaggerfallUI.DefaultFont;
            Components.Add(textCursor);
        }

        public override void Update()
        {
            base.Update();

            // Moving cursor left and right
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                MoveCursorLeft();
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                MoveCursorRight();

            // Delete and Backspace
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                if (cursorPosition != text.Length)
                    text = text.Remove(cursorPosition, 1);
            }
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (cursorPosition != 0)
                {
                    text = text.Remove(cursorPosition - 1, 1);
                    MoveCursorLeft();
                }
            }

            // Home and End
            if (Input.GetKeyDown(KeyCode.Home))
                SetCursorPosition(0);
            else if (Input.GetKeyDown(KeyCode.End))
                SetCursorPosition(text.Length);

            // Return
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // TODO: Accept text input event
                return;
            }

            // Typing characters
            char character = DaggerfallUI.Instance.LastCharacterTyped;
            if (character != 0 && text.Length < maxCharacters)
            {
                text = text.Insert(cursorPosition, character.ToString());
                MoveCursorRight();
            }
        }

        public override void Draw()
        {
            base.Draw();

            if (text.Length > 0)
            {
                Rect rect = Rectangle;
                font.DrawText(text, new Vector2(rect.x, rect.y), LocalScale, DaggerfallUI.DaggerfallDefaultInputTextColor);
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

            textCursor.Position = new Vector2(width, textCursor.Position.y);
            textCursor.BlinkOn();
        }

        float GetCharacterWidthToCursor()
        {
            if (font == null)
                return 0;

            return font.GetCharacterWidth(text, cursorPosition);
        }

        #endregion
    }
}