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
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Draws a solid-colour outline.
    /// </summary>
    public class Outline : BaseScreenComponent
    {
        const int minThickness = 1;
        const int maxThickness = 16;

        Texture2D outlineTexture;
        int outlineThickness = 1;
        Color outlineColor = Color.white;
        Sides outlineSides = Sides.All;

        public int Thickness
        {
            get { return outlineThickness; }
            set { SetThickness(value); }
        }

        public Color Color
        {
            get { return outlineColor; }
            set { outlineColor = value; }
        }

        public Sides Sides
        {
            get { return outlineSides; }
            set { outlineSides = value; }
        }

        public Outline()
        {
            CreateTexture();
        }

        public override void Draw()
        {
            base.Draw();

            if (outlineThickness == 0 || !Enabled)
                return;

            DrawSides(outlineSides);
        }

        #region Private Methods

        void DrawSides(Sides sides)
        {
            Rect rect = Rectangle;
            rect.x = Mathf.Round(rect.x);
            rect.y = Mathf.Round(rect.y);
            rect.width = Mathf.Round(rect.width);
            rect.height = Mathf.Round(rect.height);
            Rect sourceRect = new Rect(0, 0, 1, 1);
            Color color = DaggerfallUI.ModulateColor(outlineColor);

            // Top
            if ((sides & Sides.Top) == Sides.Top)
            {
                Rect topRect = rect;
                topRect.height = Mathf.Round(outlineThickness * LocalScale.y);
                Graphics.DrawTexture(topRect, outlineTexture, sourceRect, 0, 0, 0, 0, color);
            }

            // Left
            if ((sides & Sides.Left) == Sides.Left)
            {
                Rect leftRect = rect;
                leftRect.width = Mathf.Round(outlineThickness * LocalScale.x);
                Graphics.DrawTexture(leftRect, outlineTexture, sourceRect, 0, 0, 0, 0, color);
            }

            // Right
            if ((sides & Sides.Right) == Sides.Right)
            {
                Rect rightRect = rect;
                rightRect.x = Mathf.Round(rect.xMax);
                rightRect.width = Mathf.Round(outlineThickness * LocalScale.x);
                Graphics.DrawTexture(rightRect, outlineTexture, sourceRect, 0, 0, 0, 0, color);
            }

            // Bottom
            if ((sides & Sides.Bottom) == Sides.Bottom)
            {
                Rect bottomRect = rect;
                bottomRect.y = rect.yMax;
                bottomRect.width += Mathf.Round(outlineThickness * LocalScale.y);
                bottomRect.height = Mathf.Round(outlineThickness * LocalScale.y);
                Graphics.DrawTexture(bottomRect, outlineTexture, sourceRect, 0, 0, 0, 0, color);
            }
        }

        void SetThickness(int value)
        {
            if (value < minThickness) value = minThickness;
            if (value > maxThickness) value = maxThickness;
            outlineThickness = value;
        }

        void CreateTexture()
        {
            Color32[] colors = new Color32[1];
            colors[0] = Color.white;

            outlineTexture = new Texture2D(1, 1);
            outlineTexture.SetPixels32(colors);
            outlineTexture.Apply(false, true);
        }

        #endregion
    }
}