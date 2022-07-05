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

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A vertical progress/indicator bar.
    /// </summary>
    public class VerticalProgress : BaseScreenComponent
    {
        public Texture2D ProgressTexture;
        Texture2D colorTexture;

        Color32 color;
        float amount = 1.0f;

        public float Amount
        {
            get { return amount; }
            set { amount = Mathf.Clamp01(value); }
        }

        public Color32 Color
        {
            get { return color; }
            set { SetColor(value); }
        }

        public VerticalProgress()
            :base()
        {
        }

        public VerticalProgress(Texture2D texture)
            : base()
        {
            ProgressTexture = texture;
        }

        public override void Draw()
        {
            // Create texture once
            if (!colorTexture)
                colorTexture = DaggerfallUI.CreateSolidTexture(UnityEngine.Color.white, 8);

            if (Enabled)
            {
                base.Draw();
                DrawProgress();
            }
        }

        public void SetColor(Color32 color)
        {
            this.color = color;
        }

        void DrawProgress()
        {
            Rect srcRect = new Rect(0, 0, 1, 1 * amount);
            Rect dstRect = Rectangle;
            float scaledAmount = Mathf.Round(dstRect.height * amount);
            dstRect.y += dstRect.height - scaledAmount;
            dstRect.height = scaledAmount;

            if (ProgressTexture)
                DaggerfallUI.DrawTextureWithTexCoords(dstRect, ProgressTexture, srcRect, false);
            else if (colorTexture)
            {
                DaggerfallUI.DrawTextureWithTexCoords(dstRect, colorTexture, srcRect, false, color);
            }
        }
    }
}
