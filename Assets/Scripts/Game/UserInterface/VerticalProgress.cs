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

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A vertical progress/indicator bar.
    /// </summary>
    public class VerticalProgress : BaseScreenComponent
    {
        public Texture2D ProgressTexture;
        public Texture2D ColorTexture;

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
            if (Enabled)
            {
                base.Draw();
                DrawProgress();
            }
        }

        public void SetColor(Color32 color)
        {
            ColorTexture = new Texture2D(1, 1);
            Color32[] colors = new Color32[1];
            colors[0] = color;
            ColorTexture.SetPixels32(colors);
            ColorTexture.Apply(false, true);
            ColorTexture.filterMode = FilterMode.Point;
            this.color = color;
        }

        void DrawProgress()
        {
            Rect srcRect = new Rect(0, 0, 1, 1 * amount);
            Rect dstRect = Rectangle;
            dstRect.y += dstRect.height - dstRect.height * amount;
            dstRect.height *= amount;

            if (ProgressTexture)
                GUI.DrawTextureWithTexCoords(dstRect, ProgressTexture, srcRect, false);
            else if (ColorTexture)
            {
                GUI.DrawTextureWithTexCoords(dstRect, ColorTexture, srcRect, false);
            }
        }
    }
}
