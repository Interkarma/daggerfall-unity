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

            Color guiColor = GUI.color;

            GUI.color = outlineColor;
            DrawSides(outlineSides);

            GUI.color = guiColor;
        }

        #region Private Methods

        void DrawSides(Sides sides)
        {
            Rect rect = Rectangle;

            // Top
            if ((sides & Sides.Top) == Sides.Top)
            {
                Rect topRect = rect;
                topRect.height = outlineThickness * LocalScale.y;
                GUI.DrawTexture(topRect, outlineTexture);
            }

            // Left
            if ((sides & Sides.Left) == Sides.Left)
            {
                Rect leftRect = rect;
                leftRect.width = outlineThickness * LocalScale.x;
                GUI.DrawTexture(leftRect, outlineTexture);
            }

            // Right
            if ((sides & Sides.Right) == Sides.Right)
            {
                Rect rightRect = rect;
                rightRect.x = rect.xMax;
                rightRect.width = outlineThickness * LocalScale.x;
                GUI.DrawTexture(rightRect, outlineTexture);
            }

            // Bottom
            if ((sides & Sides.Bottom) == Sides.Bottom)
            {
                Rect bottomRect = rect;
                bottomRect.y = rect.yMax;
                bottomRect.width += outlineThickness * LocalScale.y;
                bottomRect.height = outlineThickness * LocalScale.y;
                GUI.DrawTexture(bottomRect, outlineTexture);
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