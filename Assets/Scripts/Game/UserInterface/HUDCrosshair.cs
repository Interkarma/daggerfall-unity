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
using DaggerfallWorkshop.Utility.AssetInjection;
 
namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Crosshair for HUD.
    /// </summary>
    public class HUDCrosshair : BaseScreenComponent
    {
        const string defaultCrosshairFilename = "Crosshair";

        Vector2 crosshairSize;

        public Texture2D CrosshairTexture;
        public float CrosshairScale = 1.0f;

        public HUDCrosshair()
            : base()
        {
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Middle;
            LoadAssets();
        }

        public override void Update()
        {
            if (CrosshairTexture && Enabled)
            {
                BackgroundTexture = CrosshairTexture;
                Size = crosshairSize * CrosshairScale;

                base.Update();
            }
        }

        void LoadAssets()
        {
            if (TextureReplacement.CustomTextureExist(defaultCrosshairFilename))
                CrosshairTexture = TextureReplacement.LoadCustomTexture(defaultCrosshairFilename);
            else
                CrosshairTexture = Resources.Load<Texture2D>(defaultCrosshairFilename);

            crosshairSize = TextureReplacement.GetSize(CrosshairTexture, defaultCrosshairFilename, true);
        }
    }
}