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

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A vertical progress/indicator bar.
    /// </summary>
    public class VerticalProgress : BaseScreenComponent
    {
        public Texture2D ProgressTexture;

        float amount = 1.0f;

        public float Amount
        {
            get { return amount; }
            set { amount = Mathf.Clamp01(value); }
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
            if (ProgressTexture && Enabled)
            {
                base.Draw();
                DrawProgress();
            }
        }

        void DrawProgress()
        {
            Rect srcRect = new Rect(0, 0, 1, 1 * amount);
            Rect dstRect = Rectangle;
            dstRect.y += dstRect.height - dstRect.height * amount;
            dstRect.height *= amount;

            GUI.DrawTextureWithTexCoords(dstRect, ProgressTexture, srcRect, false);
        }
    }
}