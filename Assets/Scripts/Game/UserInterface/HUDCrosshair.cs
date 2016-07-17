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
    /// Crosshair for HUD.
    /// </summary>
    public class HUDCrosshair : BaseScreenComponent
    {
        const string defaultCrosshairFilename = "Crosshair";

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
                Size = new Vector2(CrosshairTexture.width * CrosshairScale, CrosshairTexture.height * CrosshairScale);
                base.Update();
            }
        }

        void LoadAssets()
        {
            CrosshairTexture = Resources.Load<Texture2D>(defaultCrosshairFilename);
        }
    }
}