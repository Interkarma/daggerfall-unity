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
    /// Crosshair for HUD.
    /// </summary>
    public class HUDCrosshair : BaseScreenComponent
    {
        const string defaultCrosshairFilename = "Crosshair";

        public Vector2 crosshairSize;

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

                // Adjust crosshair position when large HUD is docked to match new viewport size
                if (DaggerfallUI.Instance.DaggerfallHUD != null &&
                    DaggerfallUnity.Settings.LargeHUD &&
                    DaggerfallUnity.Settings.LargeHUDDocked)
                {
                    VerticalAlignment = VerticalAlignment.None;
                    HUDLarge largeHUD = DaggerfallUI.Instance.DaggerfallHUD.LargeHUD;
                    float y = (Screen.height - largeHUD.ScreenHeight - crosshairSize.y) / 2;
                    Position = new Vector2(0, y);
                }
                else
                {
                    VerticalAlignment = VerticalAlignment.Middle;
                }

                base.Update();
            }
        }

        public override void Draw()
        {
            // Do not draw crosshair when cursor is active - i.e. player is now using mouse to point and click not crosshair target
            if (GameManager.Instance.PlayerMouseLook.cursorActive)
                return;

            base.Draw();
        }

        void LoadAssets()
        {
            CrosshairTexture = DaggerfallUI.GetTextureFromResources(defaultCrosshairFilename, out crosshairSize);
        }
    }
}