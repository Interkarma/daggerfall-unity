// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Compass for HUD.
    /// </summary>
    public class HUDCompass : BaseScreenComponent
    {
        const string compassFilename = "COMPASS.IMG";
        const string compassBoxFilename = "COMPBOX.IMG";

        Camera compassCamera;
        Texture2D compassTexture;
        Vector2 compassSize;
        Texture2D compassBoxTexture;
        Vector2 compassBoxSize;
        float eulerAngle;

        /// <summary>
        /// Gets or sets a compass camera to automatically determine compass heading.
        /// </summary>
        public Camera CompassCamera
        {
            get { return compassCamera; }
            set { compassCamera = value; }
        }

        /// <summary>
        /// Gets or a sets a Euler angle to use for compass heading.
        /// This value is only observed when CompassCamera is null.
        /// </summary>
        public float EulerAngle
        {
            get { return eulerAngle; }
            set { eulerAngle = Mathf.Clamp(value, 0f, 360f); }
        }

        public HUDCompass()
            : base()
        {
            compassCamera = Camera.main;
            LoadAssets();
        }

        public HUDCompass(Camera camera)
        {
            compassCamera = camera;
            HorizontalAlignment = HorizontalAlignment.Right;
            VerticalAlignment = VerticalAlignment.Bottom;
            LoadAssets();
        }

        public override void Update()
        {
            if (Enabled)
            {
                base.Update();
                Size = new Vector2(compassBoxSize.x * Scale.x, compassBoxSize.y * Scale.y);
            }
        }

        public override void Draw()
        {
            if (Enabled)
            {
                base.Draw();
                DrawCompass();
            }
        }

        void LoadAssets()
        {
            compassTexture = DaggerfallUI.GetTextureFromImg(compassFilename);
            compassSize = TextureReplacement.GetSize(compassTexture, compassFilename, true);
            compassBoxTexture = DaggerfallUI.GetTextureFromImg(compassBoxFilename);
            compassBoxSize = TextureReplacement.GetSize(compassBoxTexture, compassBoxFilename, true);
        }

        void DrawCompass()
        {
            const int boxOutlineSize = 2;       // Pixel width of box outline
            const int boxInterior = 64;         // Pixel width of box interior
            const int nonWrappedPart = 258;     // Pixel width of non-wrapped part of compass strip

            if (!compassBoxTexture || !compassTexture)
                return;

            // Calculate displacement
            float percent;
            if (compassCamera != null)
                percent = compassCamera.transform.eulerAngles.y / 360f;
            else
                percent = eulerAngle;

            // Calculate scroll offset
            int scroll = (int)((float)nonWrappedPart * percent);

            // Compass box rect
            Rect compassBoxRect = new Rect();
            compassBoxRect.x = Position.x;
            compassBoxRect.y = Position.y;

            Vector2 boxRectSize = new Vector2(compassBoxSize.x * Scale.x, compassBoxSize.y * Scale.y);
            compassBoxRect.width = boxRectSize.x;
            compassBoxRect.height = boxRectSize.y;

            // Get compassTexture size
            float compassTextureWidth = compassSize.x;
            float compassTextureHeight = compassSize.y;

            // Compass strip source
            Rect compassSrcRect = new Rect();
            compassSrcRect.xMin = scroll / compassTextureWidth;
            compassSrcRect.yMin = 0;
            compassSrcRect.xMax = compassSrcRect.xMin + (float)boxInterior / compassTextureWidth;
            compassSrcRect.yMax = 1;

            // Compass strip destination
            Rect compassDstRect = new Rect();
            compassDstRect.x = compassBoxRect.x + boxOutlineSize * Scale.x;
            compassDstRect.y = compassBoxRect.y + boxOutlineSize * Scale.y;
            compassDstRect.width = compassBoxRect.width - (boxOutlineSize * 2) * Scale.x;
            compassDstRect.height = compassTextureHeight * Scale.y;

            GUI.DrawTextureWithTexCoords(compassDstRect, compassTexture, compassSrcRect, false);
            GUI.DrawTexture(compassBoxRect, compassBoxTexture, ScaleMode.StretchToFill, true);
        }
    }
}