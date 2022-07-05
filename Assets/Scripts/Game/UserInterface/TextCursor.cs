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
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Implements a blinking text cursor.
    /// </summary>
    public class TextCursor : BaseScreenComponent
    {
        const int minThickness = 1;
        const int maxThickness = 4;

        float cursorBlinkSpeed = 0.3f;
        int cursorThickness = 1;
        int cursorHeight = 6;
        Color cursorColor = DaggerfallUI.DaggerfallDefaultTextCursorColor;
        Texture2D cursorTexture;
        bool blinkOn;
        float blinkTimer;

        public float BlinkSpeed
        {
            get { return cursorBlinkSpeed; }
            set { cursorBlinkSpeed = value; }
        }

        public int Thickness
        {
            get { return cursorThickness; }
            set { SetThickness(value); }
        }

        public int Height
        {
            get { return cursorHeight; }
            set { cursorHeight = value; }
        }

        public Color Color
        {
            get { return cursorColor; }
            set { cursorColor = value; }
        }

        public TextCursor()
        {
            CreateTexture();
        }

        public override void Update()
        {
            base.Update();

            Size = new Vector2(cursorThickness, cursorHeight);

            if (Time.realtimeSinceStartup > blinkTimer + cursorBlinkSpeed)
            {
                blinkOn = !blinkOn;
                blinkTimer = Time.realtimeSinceStartup;
            }
        }

        public override void Draw()
        {
            base.Draw();

            if (blinkOn)
            {
                DrawCursor();
            }
        }

        // Force blink on when needed, such as when moving text cursor
        public void BlinkOn()
        {
            blinkOn = true;
            blinkTimer = Time.realtimeSinceStartup;
        }

        #region Private Methods

        void DrawCursor()
        {
            Rect rect = Rectangle;
            rect.width = cursorThickness * LocalScale.x;
            DaggerfallUI.DrawTexture(rect, cursorTexture, ScaleMode.StretchToFill, true, cursorColor);
        }

        void SetThickness(int value)
        {
            if (value < minThickness) value = minThickness;
            if (value > maxThickness) value = maxThickness;
            cursorThickness = value;
        }

        void CreateTexture()
        {
            Color32[] colors = new Color32[1];
            colors[0] = Color.white;

            cursorTexture = new Texture2D(1, 1);
            cursorTexture.SetPixels32(colors);
            cursorTexture.Apply(false, true);
        }

        #endregion
    }
}